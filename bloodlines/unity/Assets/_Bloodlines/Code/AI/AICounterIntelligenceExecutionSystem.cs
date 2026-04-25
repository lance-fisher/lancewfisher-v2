using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.CounterIntelligence
    /// (written by AICovertOpsSystem) and executes the browser-side
    /// startCounterIntelligenceOperation path (simulation.js:10837-10874).
    ///
    /// Browser dispatch site: ai.js ~2372-2397; gate: shouldAiRaiseCounterIntelligence
    /// (~2859-2878) -- no active watch, no active CI op, player has covert presence.
    ///
    /// Cost (COUNTER_INTELLIGENCE_COST, simulation.js:9753): gold 60, influence 18.
    /// Duration (operation): 18 real-seconds.
    /// Watch duration on success: 150 real-seconds.
    ///
    /// WatchStrength formula (buildCounterIntelligenceTerms, simulation.js:10003-10013):
    ///   max(8, round(operatorRenown * 0.5 + 10 + fortTier * 3))
    ///   Unity simplifications (deferred): ward +5, loyalty support, legitimacy support,
    ///   instability penalty (none of these surfaces exist on AICovertOpsComponent yet).
    ///
    /// Gates:
    ///   1. Source != target faction.
    ///   2. Source has no active counter-intelligence watch.
    ///   3. Source has spymaster/diplomat/head_of_bloodline operator.
    ///   4. Source resources meet cost (gold >= 60, influence >= 18).
    ///   5. DynastyOperationLimits.HasCapacity.
    ///
    /// Always clears LastFiredOp to None after processing.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AICounterIntelligenceExecutionSystem : ISystem
    {
        private const float OperationCostGold        = 60f;
        private const float OperationCostInfluence   = 18f;
        private const float OperationDuration        = 18f;
        private const float WatchDuration            = 150f;
        private const float WatchStrengthBase        = 10f;
        private const float WatchStrengthMin         = 8f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var dispatchQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>());
            var dispatchEntities = dispatchQuery.ToEntityArray(Allocator.Temp);
            dispatchQuery.Dispose();

            for (int i = 0; i < dispatchEntities.Length; i++)
            {
                var sourceEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(sourceEntity);
                if (covert.LastFiredOp != CovertOpKind.CounterIntelligence)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchCounterIntelligence(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchCounterIntelligence(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            if (sourceFactionId.Equals(targetFactionId)) return;

            // suppress if a watch is already active on this faction
            if (em.HasBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity))
            {
                var watches = em.GetBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity);
                if (watches.Length > 0) return;
            }

            if (!TryGetCIOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var operatorRenown))
                return;

            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < OperationCostGold || resources.Influence < OperationCostInfluence) return;

            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ watchStrength
            // max(8, round(operatorRenown * 0.5 + 10 + fortTier * 3))

            float fortTier = GetHighestKeepTier(em, sourceFactionId);
            float watchStrength = math.max(WatchStrengthMin,
                math.round(operatorRenown * 0.5f + WatchStrengthBase + fortTier * 3f));
            float projectedChance = math.clamp(0.5f + watchStrength / 100f, 0.3f, 0.95f);

            // ------------------------------------------------------------------ effects

            resources.Gold      -= OperationCostGold;
            resources.Influence -= OperationCostInfluence;
            em.SetComponentData(sourceEntity, resources);

            var operationId = BuildOperationId(sourceFactionId, targetFactionId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.CounterIntelligence,
                targetFactionId,
                default);

            em.AddComponentData(entity, new DynastyOperationCounterIntelligenceComponent
            {
                SourceFactionId      = sourceFactionId,
                TargetFactionId      = targetFactionId,
                OperatorMemberId     = operatorMemberId,
                OperatorTitle        = operatorTitle,
                ResolveAtInWorldDays = inWorldDays + OperationDuration,
                SuccessScore         = watchStrength,
                ProjectedChance      = projectedChance,
                EscrowGold           = OperationCostGold,
                EscrowInfluence      = OperationCostInfluence,
            });

            PushDispatchMessage(em, sourceFactionId);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushDispatchMessage(EntityManager em, FixedString32Bytes sourceFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" raises a counter-intelligence watch.");

            NarrativeMessageBridge.Push(em, message, NarrativeMessageTone.Info);
        }

        // ------------------------------------------------------------------ id builder

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes();
            id.Append((FixedString32Bytes)"ci-");
            id.Append(sourceFactionId);
            id.Append((FixedString32Bytes)"-");
            id.Append(targetFactionId);
            id.Append((FixedString32Bytes)"-d");
            id.Append((int)inWorldDays);
            return id;
        }

        // ------------------------------------------------------------------ helpers

        private static bool TryGetCIOperator(
            EntityManager em,
            Entity factionEntity,
            out FixedString64Bytes operatorMemberId,
            out FixedString64Bytes operatorTitle,
            out float operatorRenown)
        {
            operatorMemberId = default;
            operatorTitle    = default;
            operatorRenown   = 0f;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);

            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Spymaster &&
                    member.Role != DynastyRole.Diplomat   &&
                    member.Role != DynastyRole.HeadOfBloodline) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static float GetHighestKeepTier(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }

            var cps = q.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            q.Dispose();

            float highest = 0f;
            for (int i = 0; i < cps.Length; i++)
            {
                if (!cps[i].OwnerFactionId.Equals(factionId)) continue;
                if (cps[i].FortificationTier > highest)
                    highest = cps[i].FortificationTier;
            }
            cps.Dispose();
            return highest;
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity result = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    result = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return result;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
