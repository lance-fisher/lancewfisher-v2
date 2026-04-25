using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.Assassination
    /// (written by AICovertOpsSystem) and executes the browser-side
    /// startAssassinationOperation path (simulation.js:10912-10950).
    ///
    /// Browser dispatch site: ai.js ~2435-2457, source "enemy", target
    /// "player". The browser only dispatches when liveIntelOnPlayer is
    /// true; AICovertOpsSystem already enforces that gate before setting
    /// LastFiredOp, so this system does not re-check LiveIntelOnPlayer.
    ///
    /// Browser constants (simulation.js:9765, 9769):
    ///   ASSASSINATION_COST = { gold: 85, influence: 28 }
    ///   ASSASSINATION_DURATION_SECONDS = 34
    ///
    /// Gates ported from getAssassinationTerms (simulation.js:10284-10323):
    ///   1. source != target faction.
    ///   2. Target faction + dynasty exists.
    ///   3. Target has at least one available member (pickAiAssassinationTarget
    ///      priority: Commander > HeirDesignate > HeadOfBloodline > Governor >
    ///      Spymaster > others, highest renown as tiebreak).
    ///   4. No active assassination already running against the same member
    ///      (getActiveDynastyOperationForTargetMember gate).
    ///   5. Source has a spymaster-class operator member (browser roles:
    ///      "spymaster", "diplomat", "merchant"; Unity: Spymaster, Diplomat,
    ///      Merchant).
    ///   6. Source ResourceStockpileComponent.Gold >= 85 and
    ///      ResourceStockpileComponent.Influence >= 28.
    ///   7. DynastyOperationLimits.HasCapacity.
    ///
    /// Contest formula (getAssassinationContest, simulation.js:10214-10255):
    ///   offenseScore = operatorRenown + 36 + (intelBonus: 12 since
    ///                  dispatch requires LiveIntelOnPlayer) + exposureBonus
    ///   defenseScore = targetSpymasterRenown * 0.55 + keepTier * 7
    ///                + wardDefense(12 if not-none ward)
    ///                + bloodlineProtectionBonus
    ///                + (8 if target member is head_of_bloodline)
    ///                + counterIntelBonus
    ///   Unity simplifications (deferred until respective systems land):
    ///     - exposureBonus: 0 (no locationProfile surface yet)
    ///     - keepTier: highest owned ControlPointComponent.FortificationTier
    ///     - wardDefense: 0 (no faith ward profile surface yet)
    ///     - bloodlineProtectionBonus: 0 (not ported)
    ///     - counterIntelBonus: 0 (PlayerCounterIntelligence is a separate lane)
    ///
    /// Effects on success:
    ///   - Deduct Gold -= 85, Influence -= 28.
    ///   - Create DynastyOperationComponent + DynastyOperationAssassinationComponent
    ///     via DynastyOperationLimits.BeginOperation.
    ///   - Push dispatch narrative (browser simulation.js:10944-10948).
    ///
    /// Always clears LastFiredOp to None after processing (one-shot pattern).
    ///
    /// Duration stored as real-seconds numeric 34 directly on the in-world
    /// timeline (matching the pattern established in sub-slice 20 for
    /// MISSIONARY_DURATION_SECONDS = 32).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIAssassinationExecutionSystem : ISystem
    {
        public const float AssassinationGoldCost      = 85f;
        public const float AssassinationInfluenceCost = 28f;
        public const float AssassinationDurationInWorldDays = 34f;
        public const float IntelBonus = 12f;

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
                if (covert.LastFiredOp != CovertOpKind.Assassination)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchAssassination(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchAssassination(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            if (sourceFactionId.Equals(targetFactionId)) return;

            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;

            if (!TryPickAssassinationTarget(em, targetEntity, out var targetMemberId,
                    out var targetMemberTitle, out var targetMemberRole))
                return;

            if (HasActiveAssassinationOnMember(em, sourceFactionId, targetMemberId)) return;

            if (!TryGetSpymasterOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var operatorRenown))
                return;

            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < AssassinationGoldCost ||
                resources.Influence < AssassinationInfluenceCost) return;

            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ contest

            float targetSpymasterRenown = GetTargetSpymasterRenown(em, targetEntity);
            float keepTier = GetHighestKeepTier(em, targetFactionId);
            float headBonus = targetMemberRole == DynastyRole.HeadOfBloodline ? 8f : 0f;

            float offenseScore = operatorRenown + 36f + IntelBonus;
            float defenseScore = targetSpymasterRenown * 0.55f + keepTier * 7f + headBonus;
            float successScore = offenseScore - defenseScore;
            float projectedChance = math.clamp(0.5f + successScore / 100f, 0.06f, 0.9f);

            // ------------------------------------------------------------------ effects

            resources.Gold      -= AssassinationGoldCost;
            resources.Influence -= AssassinationInfluenceCost;
            em.SetComponentData(sourceEntity, resources);

            var operationId = BuildOperationId(sourceFactionId, targetFactionId, targetMemberId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.Assassination,
                targetFactionId,
                targetMemberId);

            em.AddComponentData(entity, new DynastyOperationAssassinationComponent
            {
                TargetFactionId    = targetFactionId,
                TargetMemberId     = targetMemberId,
                TargetMemberTitle  = targetMemberTitle,
                OperatorMemberId   = operatorMemberId,
                OperatorTitle      = operatorTitle,
                ResolveAtInWorldDays = inWorldDays + AssassinationDurationInWorldDays,
                SuccessScore       = successScore,
                ProjectedChance    = projectedChance,
                EscrowGold         = AssassinationGoldCost,
                EscrowInfluence    = AssassinationInfluenceCost,
                IntelSupport       = true,
            });

            PushDispatchMessage(em, sourceFactionId, targetFactionId, targetMemberTitle);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" sets an assassination cell on ");
            message.Append(targetMemberTitle);
            message.Append((FixedString32Bytes)" inside ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            var tone = targetFactionId.Equals(playerId) ? NarrativeMessageTone.Warn : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ target selection

        private static bool TryPickAssassinationTarget(
            EntityManager em,
            Entity factionEntity,
            out FixedString64Bytes memberId,
            out FixedString64Bytes memberTitle,
            out DynastyRole memberRole)
        {
            memberId    = default;
            memberTitle = default;
            memberRole  = default;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);

            FixedString64Bytes bestId    = default;
            FixedString64Bytes bestTitle = default;
            DynastyRole        bestRole  = default;
            float bestRenown             = float.MinValue;
            int bestPriority             = int.MaxValue;
            bool found                   = false;

            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status != DynastyMemberStatus.Active &&
                    member.Status != DynastyMemberStatus.Ruling) continue;

                int priority = AssassinationPriority(member.Role);
                if (priority < bestPriority ||
                    (priority == bestPriority && member.Renown > bestRenown))
                {
                    bestId       = member.MemberId;
                    bestTitle    = member.Title;
                    bestRole     = member.Role;
                    bestPriority = priority;
                    bestRenown   = member.Renown;
                    found        = true;
                }
            }

            if (!found) return false;
            memberId    = bestId;
            memberTitle = bestTitle;
            memberRole  = bestRole;
            return true;
        }

        private static int AssassinationPriority(DynastyRole role)
        {
            // Mirrors browser pickAiAssassinationTarget role priority order:
            // commander:0, heir_designate:1, head_of_bloodline:2,
            // governor:3, spymaster:4, steward(no Unity equiv):5, other:8
            switch (role)
            {
                case DynastyRole.Commander:      return 0;
                case DynastyRole.HeirDesignate:  return 1;
                case DynastyRole.HeadOfBloodline:return 2;
                case DynastyRole.Governor:       return 3;
                case DynastyRole.Spymaster:      return 4;
                default:                         return 8;
            }
        }

        // ------------------------------------------------------------------ helpers

        private static bool HasActiveAssassinationOnMember(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes targetMemberId)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationAssassinationComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var ops     = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var assasns = q.ToComponentDataArray<DynastyOperationAssassinationComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceFactionId)) continue;
                if (assasns[i].TargetMemberId.Equals(targetMemberId)) { found = true; break; }
            }

            ops.Dispose();
            assasns.Dispose();
            return found;
        }

        private static bool TryGetSpymasterOperator(
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
                if (!IsSpymasterRole(member.Role)) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static bool IsSpymasterRole(DynastyRole role)
        {
            return role == DynastyRole.Spymaster ||
                   role == DynastyRole.Diplomat  ||
                   role == DynastyRole.Merchant;
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

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId)) { match = entities[i]; break; }
            }
            entities.Dispose();
            factions.Dispose();
            return match;
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-assn-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
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
