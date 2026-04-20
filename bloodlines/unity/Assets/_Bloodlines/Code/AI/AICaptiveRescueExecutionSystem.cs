using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRescue
    /// (written by AICovertOpsSystem in sub-slice 6 when the AI's
    /// captive-recovery decision tree picks rescue over ransom for a
    /// captive belonging to the source faction). Executes the
    /// browser-side startRescueOperation path
    /// (simulation.js:11067-11111). Fourth consumer of the sub-slice
    /// 18 dynasty-operation foundation.
    ///
    /// Browser dispatch site: ai.js captive recovery dispatch
    /// ~2566-2607, hardcoded to source "enemy" with a chosen captive
    /// from pickAiCaptiveRecoveryTarget (ai.js:3011-3042). The
    /// "rescue or ransom" decision happens inside AICovertOpsSystem's
    /// TryFireOps based on HighPriorityCaptive / EnemyIsHostileToPlayer
    /// flags; by the time this system runs the choice is already made.
    ///
    /// Captive picker: walks every faction's CapturedMemberElement
    /// buffer (sub-slice 19 surface) and picks the first entry where
    /// OriginFactionId matches the source faction and Status == Held.
    /// This is a Unity-side simplification of browser
    /// pickAiCaptiveRecoveryTarget (ai.js:3011), which sorts captives
    /// by role priority then by renown. The simplified picker is
    /// sufficient for this slice because CapturedMemberElement does
    /// not carry roleId or renown yet; a future slice that extends the
    /// captive element shape can swap in a sophisticated picker
    /// without reshaping this system.
    ///
    /// Gates ported from getCapturedMemberRescueTerms
    /// (~4968-5028) in their simplified Unity form:
    ///   1. Source faction exists with FactionComponent.
    ///   2. At least one CapturedMemberElement entry across all
    ///      factions has OriginFactionId == source and Status == Held
    ///      (the captive picker returns null otherwise).
    ///   3. No existing active dynasty operation with this
    ///      TargetMemberId (preserves browser
    ///      getActiveDynastyOperationForMember check at
    ///      simulation.js:4974).
    ///   4. Source has a Spymaster-equivalent member on the dynasty
    ///      roster (priority [Spymaster, Diplomat, Merchant]) with
    ///      non-Fallen, non-Captured status.
    ///   5. Source has a Diplomat-equivalent member (priority
    ///      [Diplomat, Merchant, HeirDesignate]) with non-Fallen,
    ///      non-Captured status.
    ///   6. Source ResourceStockpileComponent.Gold >= 42
    ///      (RESCUE_BASE_GOLD_COST at simulation.js:33).
    ///   7. Source ResourceStockpileComponent.Influence >= 26
    ///      (RESCUE_BASE_INFLUENCE_COST at simulation.js:34).
    ///
    /// Gates intentionally deferred (Unity has no equivalent surface yet):
    ///   - Holding-settlement keep-tier and ward-profile gates
    ///     (browser getCaptiveHoldingSettlement +
    ///     getFortificationFaithWardProfile at simulation.js:4979-4981).
    ///     The KeepTier and WardId fields on the per-kind component
    ///     default to 0 / empty; success-score formula omits the
    ///     keep-tier and ward-difficulty contributions until the
    ///     fortification ward surface ports.
    ///   - Captor-faction spymaster contribution to difficulty
    ///     (browser captorSpymaster.renown * 0.42 at simulation.js:4997).
    ///     Defense-side renown surface not yet ported on captives.
    ///
    /// Capacity gate: DynastyOperationLimits.HasCapacity must return
    /// true for the source faction (sub-slice 18). The cap is six per
    /// faction (DYNASTY_OPERATION_ACTIVE_LIMIT at simulation.js:17).
    ///
    /// Effects on success:
    ///   - Deduct 42 Gold and 26 Influence from source ResourceStockpileComponent.
    ///   - Compute SuccessScore from simplified parity (power -
    ///     difficulty using base constants without renown scaling).
    ///   - ProjectedChance clamped 0.12-0.88 from 0.5 + (successScore / 45).
    ///   - Call DynastyOperationLimits.BeginOperation with
    ///     DynastyOperationKind.CaptiveRescue, source faction id,
    ///     captor faction id as the target, and the captive member id;
    ///     attach DynastyOperationCaptiveRescueComponent to the entity.
    ///   - Push narrative message via NarrativeMessageBridge.Push
    ///     mirroring simulation.js:11105-11109 ("X dispatches covert
    ///     agents to recover Y from Z"). Tone routing per browser:
    ///       sourceFactionId == "player" -> Info
    ///       else                        -> Good
    ///     Browser uses good for non-player rescue dispatch because the
    ///     rescue is a positive faction action; Unity matches.
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8/9/12/13/14/20/21/22.
    ///
    /// Per-kind resolution intentionally deferred. The created entity
    /// sits with Active=true and the per-kind component attached until
    /// a future resolution slice walks expired entries at
    /// ResolveAtInWorldDays, rolls against ProjectedChance, and either
    /// flips the captive's Status to Released via
    /// CapturedMemberHelpers.ReleaseCaptive (success) or records a
    /// failed-rescue conviction event on the source faction (failure)
    /// per browser simulation.js:5885-5894. The operation entity flips
    /// Active=false on either resolution branch.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AICaptiveRescueExecutionSystem : ISystem
    {
        public const float RescueBaseGoldCost     = 42f;
        public const float RescueBaseInfluenceCost = 26f;
        public const float RescueBaseDurationInWorldDays = 20f;

        // Simplified parity formula constants from getCapturedMemberRescueTerms
        // at simulation.js:4987-4998. Unity port omits renown scaling
        // and ward/keep-tier difficulty until those surfaces port.
        private const float BasePower      = 12f;
        private const float BaseDifficulty = 16f;

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
                if (covert.LastFiredOp != CovertOpKind.CaptiveRescue)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                TryDispatchRescue(em, sourceEntity, sourceFaction.FactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchRescue(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            float inWorldDays)
        {
            // Captive picker: find a Held captive belonging to source.
            if (!TryPickCaptive(em, sourceFactionId,
                    out var captiveMemberId,
                    out var captiveMemberTitle,
                    out var captorFactionId))
                return;

            // Gate: no existing active dynasty operation for this member.
            if (HasActiveOperationForMember(em, captiveMemberId)) return;

            // Gate: source has Spymaster + Diplomat operatives.
            if (!TryGetMemberByRolePriority(em, sourceEntity,
                    new DynastyRole[] { DynastyRole.Spymaster, DynastyRole.Diplomat, DynastyRole.Merchant },
                    out var spymasterMemberId, out var _, out var spymasterRenown))
                return;
            if (!TryGetMemberByRolePriority(em, sourceEntity,
                    new DynastyRole[] { DynastyRole.Diplomat, DynastyRole.Merchant, DynastyRole.HeirDesignate },
                    out var diplomatMemberId, out var _, out var diplomatRenown))
                return;

            // Gate: source affords gold + influence.
            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < RescueBaseGoldCost) return;
            if (resources.Influence < RescueBaseInfluenceCost) return;

            // Capacity gate (sub-slice 18).
            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ effects

            resources.Gold      -= RescueBaseGoldCost;
            resources.Influence -= RescueBaseInfluenceCost;
            em.SetComponentData(sourceEntity, resources);

            // Simplified parity formula. Browser power/difficulty at
            // simulation.js:4987-4997 includes member.renown scaling and
            // ward/keep-tier difficulty; Unity port uses operatives'
            // renown only.
            float power      = BasePower + spymasterRenown * 0.95f + diplomatRenown * 0.35f;
            float difficulty = BaseDifficulty;
            float successScore   = power - difficulty;
            float projectedChance = math.clamp(0.5f + successScore / 45f, 0.12f, 0.88f);

            var operationId = BuildOperationId(sourceFactionId, captorFactionId, captiveMemberId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.CaptiveRescue,
                captorFactionId,
                captiveMemberId);

            em.AddComponentData(entity, new DynastyOperationCaptiveRescueComponent
            {
                ResolveAtInWorldDays = inWorldDays + RescueBaseDurationInWorldDays,
                CaptiveMemberId      = captiveMemberId,
                CaptiveMemberTitle   = captiveMemberTitle,
                CaptorFactionId      = captorFactionId,
                SpymasterMemberId    = spymasterMemberId,
                DiplomatMemberId     = diplomatMemberId,
                HoldingSettlementId  = default,
                KeepTier             = 0,
                WardId               = default,
                SuccessScore         = successScore,
                ProjectedChance      = projectedChance,
                IntensityCost        = 0f,
                EscrowCost           = new DynastyOperationEscrowCost
                {
                    Gold      = RescueBaseGoldCost,
                    Influence = RescueBaseInfluenceCost,
                },
            });

            PushRescueDispatchMessage(em, sourceFactionId, captiveMemberTitle, captorFactionId);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushRescueDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes captiveMemberTitle,
            FixedString32Bytes captorFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" dispatches covert agents to recover ");
            message.Append(captiveMemberTitle);
            message.Append((FixedString32Bytes)" from ");
            message.Append(captorFactionId);
            message.Append((FixedString32Bytes)".");

            // Browser tone routing simulation.js:11108: source==player -> info,
            // else -> good.
            var tone = sourceFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Info
                : NarrativeMessageTone.Good;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        internal static bool TryPickCaptive(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            out FixedString64Bytes captiveMemberId,
            out FixedString64Bytes captiveMemberTitle,
            out FixedString32Bytes captorFactionId)
        {
            captiveMemberId    = default;
            captiveMemberTitle = default;
            captorFactionId    = default;

            // Walk every faction entity with a CapturedMemberElement
            // buffer; for each, scan for a Held entry whose
            // OriginFactionId matches source.
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CapturedMemberElement>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            try
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    var buffer = em.GetBuffer<CapturedMemberElement>(entities[i]);
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        var entry = buffer[j];
                        if (entry.Status != CapturedMemberStatus.Held) continue;
                        if (!entry.OriginFactionId.Equals(sourceFactionId)) continue;

                        captiveMemberId    = entry.MemberId;
                        captiveMemberTitle = entry.MemberTitle;
                        captorFactionId    = factions[i].FactionId;
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                entities.Dispose();
                factions.Dispose();
            }
        }

        internal static bool HasActiveOperationForMember(
            EntityManager em,
            FixedString64Bytes memberId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var arr = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < arr.Length; i++)
            {
                var op = arr[i];
                if (!op.Active) continue;
                if (!op.TargetMemberId.Equals(memberId)) continue;
                found = true;
                break;
            }
            arr.Dispose();
            return found;
        }

        internal static bool TryGetMemberByRolePriority(
            EntityManager em,
            Entity factionEntity,
            DynastyRole[] rolePriority,
            out FixedString64Bytes memberId,
            out FixedString64Bytes memberTitle,
            out float memberRenown)
        {
            memberId    = default;
            memberTitle = default;
            memberRenown = 0f;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);

            // Walk the priority list; return the first eligible member
            // that fills any of the roles in declared order.
            for (int p = 0; p < rolePriority.Length; p++)
            {
                var targetRole = rolePriority[p];
                for (int k = 0; k < roster.Length; k++)
                {
                    var memberEntity = roster[k].Member;
                    if (memberEntity == Entity.Null) continue;
                    if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                    var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                    if (member.Role != targetRole) continue;
                    if (member.Status == DynastyMemberStatus.Fallen ||
                        member.Status == DynastyMemberStatus.Captured) continue;

                    memberId    = member.MemberId;
                    memberTitle = member.Title;
                    memberRenown = member.Renown;
                    return true;
                }
            }
            return false;
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes captiveMemberId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-rescue-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(captorFactionId);
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
