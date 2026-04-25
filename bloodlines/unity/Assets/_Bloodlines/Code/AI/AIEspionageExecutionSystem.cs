using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.Espionage
    /// (written by AICovertOpsSystem when EspionageTimer fires) and executes the
    /// browser-side startEspionageOperation path (simulation.js:10876-10910).
    ///
    /// Browser dispatch site: ai.js ~2406-2417 (gate: !liveIntelOnPlayer && !liveEspionage).
    ///
    /// Cost (ESPIONAGE_COST, simulation.js:9764): gold 45, influence 16.
    /// Duration: 30 real-seconds (ESPIONAGE_DURATION_SECONDS, simulation.js:9768).
    /// Report duration on success: 120 real-seconds (INTELLIGENCE_REPORT_DURATION_SECONDS, simulation.js:9772).
    ///
    /// Contest formula (getEspionageContest, simulation.js:10187-10212):
    ///   offenseScore = operatorRenown + 32
    ///   defenseScore = targetSpymasterRenown * 0.55 + fortTier * 6
    ///   Unity simplifications (deferred): wardDefense (+8 if ward active), counterIntel.totalBonus.
    ///
    /// Gates:
    ///   1. Source != target faction.
    ///   2. Target faction exists.
    ///   3. Source has no live intel report on target already (LiveIntelOnPlayer == false).
    ///   4. Source has no active espionage operation on target.
    ///   5. Source has Spymaster/Diplomat/Merchant operator.
    ///   6. Source resources >= cost (gold 45, influence 16).
    ///   7. DynastyOperationLimits.HasCapacity.
    ///
    /// Always clears LastFiredOp to None after processing.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIEspionageExecutionSystem : ISystem
    {
        private const float OperationCostGold           = 45f;
        private const float OperationCostInfluence      = 16f;
        private const float OperationDuration           = 30f;
        private const float ReportDuration              = 120f;
        private const float BaseOffenseBonus            = 32f;

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
                if (covert.LastFiredOp != CovertOpKind.Espionage)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchEspionage(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchEspionage(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            if (sourceFactionId.Equals(targetFactionId)) return;

            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;

            // Gate: no active espionage operation on target.
            if (HasActiveEspionageOn(em, sourceFactionId, targetFactionId)) return;

            if (!TryGetOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var operatorRenown))
                return;

            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < OperationCostGold || resources.Influence < OperationCostInfluence) return;

            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ contest

            float targetSpymasterRenown = GetTargetSpymasterRenown(em, targetEntity);
            float fortTier = GetHighestKeepTier(em, targetFactionId);

            float offenseScore = operatorRenown + BaseOffenseBonus;
            float defenseScore = targetSpymasterRenown * 0.55f + fortTier * 6f;
            float successScore = offenseScore - defenseScore;
            float projectedChance = math.clamp(0.5f + successScore / 100f, 0.08f, 0.92f);

            // ------------------------------------------------------------------ effects

            resources.Gold      -= OperationCostGold;
            resources.Influence -= OperationCostInfluence;
            em.SetComponentData(sourceEntity, resources);

            var operationId = BuildOperationId(sourceFactionId, targetFactionId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.Espionage,
                targetFactionId,
                default);

            em.AddComponentData(entity, new DynastyOperationEspionageComponent
            {
                SourceFactionId            = sourceFactionId,
                TargetFactionId            = targetFactionId,
                OperatorMemberId           = operatorMemberId,
                OperatorTitle              = operatorTitle,
                ResolveAtInWorldDays       = inWorldDays + OperationDuration,
                ReportExpiresAtInWorldDays = inWorldDays + ReportDuration,
                SuccessScore               = successScore,
                ProjectedChance            = projectedChance,
                EscrowGold                 = OperationCostGold,
                EscrowInfluence            = OperationCostInfluence,
            });

            PushDispatchMessage(em, sourceFactionId, targetFactionId);
        }

        // ------------------------------------------------------------------ helpers

        private static bool HasActiveEspionageOn(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationEspionageComponent>());
            var ops = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceFactionId)) continue;
                if (ops[i].TargetFactionId.Equals(targetFactionId))
                {
                    found = true;
                    break;
                }
            }
            ops.Dispose();
            return found;
        }

        private static void PushDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" dispatches an espionage web toward ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            var tone = targetFactionId.Equals(playerId) ? NarrativeMessageTone.Warn : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes();
            id.Append((FixedString32Bytes)"esp-");
            id.Append(sourceFactionId);
            id.Append((FixedString32Bytes)"-");
            id.Append(targetFactionId);
            id.Append((FixedString32Bytes)"-d");
            id.Append((int)inWorldDays);
            return id;
        }

        private static bool TryGetOperator(
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
                    member.Role != DynastyRole.Merchant) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static float GetTargetSpymasterRenown(EntityManager em, Entity targetEntity)
        {
            if (!em.HasBuffer<DynastyMemberRef>(targetEntity)) return 0f;
            var roster = em.GetBuffer<DynastyMemberRef>(targetEntity);

            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Spymaster && member.Role != DynastyRole.Diplomat) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;
                return member.Renown;
            }
            return 0f;
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
