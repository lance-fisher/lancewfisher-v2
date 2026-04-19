using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.Missionary
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the
    /// browser-side startMissionaryOperation path
    /// (simulation.js:10523-10563). First consumer of the sub-slice 18
    /// dynasty-operation foundation.
    ///
    /// Browser dispatch site: ai.js missionary dispatch ~2469-2496,
    /// hardcoded to source "enemy" / target "player".
    ///
    /// Gates ported from getMissionaryTerms (~10362-10422):
    ///   1. Source != target.
    ///   2. Source has a committed faith (FaithStateComponent.SelectedFaith
    ///      != CovenantId.None).
    ///   3. Target faction exists.
    ///   4. Target has at least one ControlPointComponent with
    ///      OwnerFactionId matching the target. Defensive Unity-side
    ///      gate; the browser permits the call with default ward when
    ///      the target has no primary keep, but a missionary working in
    ///      a court with zero territories produces no meaningful effect.
    ///   5. Source has a faith operator member on the dynasty roster
    ///      (DynastyMemberComponent with one of the canonical faith
    ///      operator roles: HeadOfBloodline, Diplomat,
    ///      IdeologicalLeader, Spymaster). Browser
    ///      getFaithOperatorMember (simulation.js:690) accepts
    ///      ["ideological_leader", "sorcerer", "head_of_bloodline",
    ///      "diplomat"]; Unity has no Sorcerer role yet so Spymaster
    ///      stands in as the closest covert-faith equivalent for parity
    ///      pending a canonical Sorcerer DynastyRole addition.
    ///   6. Source ResourceStockpileComponent.Influence >= 14
    ///      (MISSIONARY_COST.influence at simulation.js:9766).
    ///   7. Source FaithStateComponent.Intensity >= 12
    ///      (MISSIONARY_INTENSITY_COST at simulation.js:9773).
    ///
    /// Capacity gate: DynastyOperationLimits.HasCapacity must return
    /// true for the source faction (sub-slice 18). The cap is six per
    /// faction (DYNASTY_OPERATION_ACTIVE_LIMIT at simulation.js:17).
    ///
    /// Effects on success:
    ///   - Deduct 14 Influence from ResourceStockpileComponent.
    ///   - Deduct 12 Intensity from FaithStateComponent (clamped >= 0).
    ///   - Compute per-kind terms (ExposureGain, IntensityErosion,
    ///     LoyaltyPressure, SuccessScore, ProjectedChance) using the
    ///     simplified browser parity formula (operator renown +
    ///     intensity * 0.65 + dark/light bias).
    ///   - Call DynastyOperationLimits.BeginOperation with
    ///     DynastyOperationKind.Missionary and the player target;
    ///     attach DynastyOperationMissionaryComponent to the created
    ///     entity carrying the per-kind fields.
    ///   - Push narrative message via NarrativeMessageBridge.Push
    ///     mirroring simulation.js:10557-10561 ("dispatches missionaries
    ///     of X toward Y"). Tone routing per browser:
    ///       sourceFactionId == "player" -> Info
    ///       targetFactionId == "player" -> Warn
    ///       else                        -> Info
    ///     The browser uses "warn" when the player is the missionary
    ///     target (player needs to know hostile missionaries are
    ///     incoming); Unity matches.
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8/9/12/13/14.
    ///
    /// Browser duration translation: `MISSIONARY_DURATION_SECONDS = 32`
    /// at simulation.js:9770 is real seconds. Unity stores
    /// ResolveAtInWorldDays = current + 32 (using browser numeric value
    /// directly on the in-world timeline). Future resolution slice can
    /// translate via DualClock.DaysPerRealSecond if the canonical clock
    /// rate ever shifts.
    ///
    /// Deferred to later slices:
    ///   - Per-operation resolution (apply exposure/intensity/loyalty
    ///     pressure to target when ResolveAtInWorldDays elapses).
    ///   - Faith ward profile lookup on target's primary keep
    ///     (browser getPrimaryKeepSeat + getFortificationFaithWardProfile)
    ///     for canonical defense-score weighting; current Unity port
    ///     uses target intensity * 0.55 with no ward bonus, which is
    ///     a slight power adjustment vs browser defense-score formula.
    ///   - Sorcerer DynastyRole canonical mapping (Spymaster currently
    ///     stands in).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIMissionaryExecutionSystem : ISystem
    {
        public const float MissionaryInfluenceCost     = 14f;
        public const float MissionaryIntensityCost     = 12f;
        public const float MissionaryDurationInWorldDays = 32f;

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
                if (covert.LastFiredOp != CovertOpKind.Missionary)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                // Target is hardcoded as "player" matching the browser
                // ai.js dispatch (`startMissionaryOperation(state, "enemy", "player")`).
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchMissionary(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchMissionary(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            // Gate 1: same faction guard.
            if (sourceFactionId.Equals(targetFactionId)) return;

            // Gate 2: source must have a committed faith.
            if (!em.HasComponent<FaithStateComponent>(sourceEntity)) return;
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None) return;

            // Gate 3: target faction must exist.
            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;

            // Gate 4: target must have at least one territory.
            if (!HasAnyTerritory(em, targetFactionId)) return;

            // Gate 5: source must have a faith operator member.
            if (!TryGetFaithOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var operatorRenown))
                return;

            // Gate 6: source must afford the influence cost.
            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < MissionaryInfluenceCost) return;

            // Gate 7: source faith intensity must meet the threshold.
            if (sourceFaith.Intensity < MissionaryIntensityCost) return;

            // Capacity gate (sub-slice 18).
            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ effects

            // Deduct influence + intensity. spendFaithIntensity clamps >= 0 in browser; same here.
            resources.Influence -= MissionaryInfluenceCost;
            em.SetComponentData(sourceEntity, resources);

            sourceFaith.Intensity = math.max(0f, sourceFaith.Intensity - MissionaryIntensityCost);
            em.SetComponentData(sourceEntity, sourceFaith);

            // Compute per-kind terms. Mirror getMissionaryTerms formula
            // (simulation.js:10395-10405) with Unity-side simplifications:
            //   - target operator renown lookup deferred (Unity has no
            //     symmetric target-operator gate yet); defenseScore
            //     uses target intensity only.
            //   - ward profile bonus deferred (Unity has no faith-ward
            //     readout for control points yet); defenseScore omits
            //     the +10 ward bonus.
            float sourceIntensity = sourceFaith.Intensity + MissionaryIntensityCost; // pre-deduction value used in browser formula
            float targetIntensity = 0f;
            CovenantId targetSelectedFaith = CovenantId.None;
            if (em.HasComponent<FaithStateComponent>(targetEntity))
            {
                var targetFaith = em.GetComponentData<FaithStateComponent>(targetEntity);
                targetIntensity = targetFaith.Intensity;
                targetSelectedFaith = targetFaith.SelectedFaith;
            }
            bool isDarkPath = sourceFaith.DoctrinePath == DoctrinePath.Dark;
            float offenseScore = operatorRenown + sourceIntensity * 0.65f + (isDarkPath ? 8f : 4f);
            float defenseScore = targetIntensity * 0.55f;
            float successScore = offenseScore - defenseScore;
            float projectedChance = math.clamp(0.5f + successScore / 100f, 0.1f, 0.9f);

            float exposureGain = math.max(12f, math.round(14f + math.max(0f, successScore) * 0.22f));
            float intensityErosion = (targetSelectedFaith != CovenantId.None &&
                                      targetSelectedFaith != sourceFaith.SelectedFaith)
                ? math.max(4f, math.round(5f + math.max(0f, sourceIntensity - targetIntensity) / 12f) +
                              (isDarkPath ? 2f : 0f))
                : 0f;
            float loyaltyPressure = (targetSelectedFaith != CovenantId.None &&
                                     targetSelectedFaith != sourceFaith.SelectedFaith)
                ? (isDarkPath ? 4f : 2f)
                : 1f;

            // Begin operation entity (sub-slice 18 producer).
            var operationId = BuildOperationId(sourceFactionId, targetFactionId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.Missionary,
                targetFactionId,
                default);

            // Attach per-kind component.
            em.AddComponentData(entity, new DynastyOperationMissionaryComponent
            {
                ResolveAtInWorldDays = inWorldDays + MissionaryDurationInWorldDays,
                OperatorMemberId     = operatorMemberId,
                OperatorTitle        = operatorTitle,
                SourceFaithId        = SelectedFaithIdString(sourceFaith.SelectedFaith),
                ExposureGain         = exposureGain,
                IntensityErosion     = intensityErosion,
                LoyaltyPressure      = loyaltyPressure,
                SuccessScore         = successScore,
                ProjectedChance      = projectedChance,
                IntensityCost        = MissionaryIntensityCost,
                EscrowCost           = new DynastyOperationEscrowCost { Influence = MissionaryInfluenceCost },
            });

            // Narrative push. Browser pushMessage at simulation.js:10557-10561.
            PushMissionaryDispatchMessage(em, sourceFactionId, targetFactionId,
                SelectedFaithIdString(sourceFaith.SelectedFaith));
        }

        // ------------------------------------------------------------------ narrative

        private static void PushMissionaryDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes sourceFaithId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" dispatches missionaries of ");
            message.Append(sourceFaithId);
            message.Append((FixedString32Bytes)" toward ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            // Browser tone routing: source==player -> info, target==player -> warn, else info.
            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Info;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static bool HasAnyTerritory(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var arr = q.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].OwnerFactionId.Equals(factionId)) { found = true; break; }
            }
            arr.Dispose();
            return found;
        }

        private static bool TryGetFaithOperator(
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
                if (!IsFaithOperatorRole(member.Role)) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static bool IsFaithOperatorRole(DynastyRole role)
        {
            // Browser order: ideological_leader, sorcerer, head_of_bloodline, diplomat.
            // Unity has no Sorcerer role; Spymaster stands in pending a canonical addition.
            return role == DynastyRole.IdeologicalLeader ||
                   role == DynastyRole.Spymaster ||
                   role == DynastyRole.HeadOfBloodline ||
                   role == DynastyRole.Diplomat;
        }

        private static FixedString64Bytes SelectedFaithIdString(CovenantId selected)
        {
            switch (selected)
            {
                case CovenantId.OldLight:      return new FixedString64Bytes("old_light");
                case CovenantId.BloodDominion: return new FixedString64Bytes("blood_dominion");
                case CovenantId.TheOrder:      return new FixedString64Bytes("the_order");
                case CovenantId.TheWild:       return new FixedString64Bytes("the_wild");
                default:                       return new FixedString64Bytes("none");
            }
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
                if (factions[i].FactionId.Equals(factionId))
                {
                    match = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return match;
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-missionary-");
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
