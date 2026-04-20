using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRansom
    /// (written by AICovertOpsSystem in sub-slice 6 when the AI's
    /// captive-recovery decision tree picks ransom over rescue for a
    /// captive belonging to the source faction). Executes the
    /// browser-side startRansomNegotiation path
    /// (simulation.js:11026-11065). Fifth consumer of the sub-slice
    /// 18 dynasty-operation foundation.
    ///
    /// Browser dispatch site: ai.js captive recovery dispatch
    /// ~2566-2607. The "rescue or ransom" decision happens inside
    /// AICovertOpsSystem's TryFireOps based on HighPriorityCaptive /
    /// EnemyIsHostileToPlayer flags; ransom is chosen when neither
    /// condition is met.
    ///
    /// Captive picker: reuses the same picker as
    /// AICaptiveRescueExecutionSystem -- walks every faction's
    /// CapturedMemberElement buffer (sub-slice 19 surface) and picks
    /// the first entry where OriginFactionId matches the source
    /// faction and Status == Held. Simplified Unity-side port of
    /// browser pickAiCaptiveRecoveryTarget (ai.js:3011).
    ///
    /// Gates ported from getCapturedMemberRansomTerms
    /// (~4929-4966) in their simplified Unity form:
    ///   1. Source faction exists with FactionComponent.
    ///   2. At least one CapturedMemberElement entry across all
    ///      factions has OriginFactionId == source and Status == Held.
    ///   3. No existing active dynasty operation with this
    ///      TargetMemberId (preserves browser
    ///      getActiveDynastyOperationForMember check at
    ///      simulation.js:4935).
    ///   4. Source has a Diplomat-equivalent member on the dynasty
    ///      roster (priority [Diplomat, Merchant, HeirDesignate,
    ///      HeadOfBloodline]) with non-Fallen, non-Captured status.
    ///   5. Source has a Merchant-equivalent member (priority
    ///      [Merchant, Governor, HeadOfBloodline]) with non-Fallen,
    ///      non-Captured status.
    ///   6. Source ResourceStockpileComponent.Gold >= 70
    ///      (RANSOM_BASE_GOLD_COST at simulation.js:31).
    ///   7. Source ResourceStockpileComponent.Influence >= 18
    ///      (RANSOM_BASE_INFLUENCE_COST at simulation.js:32).
    ///
    /// Gates intentionally deferred (Unity has no equivalent surface yet):
    ///   - Captor-faction envoy premium (browser captorEnvoy.renown
    ///     premium scaling at simulation.js:4946). Captor-side renown
    ///     surface not yet ported on captives.
    ///
    /// Capacity gate: DynastyOperationLimits.HasCapacity must return
    /// true for the source faction (sub-slice 18).
    ///
    /// Effects on success:
    ///   - Deduct 70 Gold and 18 Influence from source ResourceStockpileComponent.
    ///   - Call DynastyOperationLimits.BeginOperation with
    ///     DynastyOperationKind.CaptiveRansom, source faction id,
    ///     captor faction id as the target, and the captive member id;
    ///     attach DynastyOperationCaptiveRansomComponent to the entity
    ///     with ProjectedChance = 1.0 (ransom is paid, not rolled).
    ///   - Push narrative message via NarrativeMessageBridge.Push
    ///     mirroring simulation.js:11059-11063 ("X opens ransom terms
    ///     for Y with Z"). Tone routing per browser:
    ///       sourceFactionId == "player" -> Info
    ///       else                        -> Good
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8/9/12/13/14/20/21/22/23.
    ///
    /// Per-kind resolution intentionally deferred. The created entity
    /// sits with Active=true and the per-kind component attached until
    /// a future resolution slice walks expired entries at
    /// ResolveAtInWorldDays and applies the canonical browser
    /// semantics: flip captive Status from Held -> RansomOffered ->
    /// Released via CapturedMemberHelpers.ReleaseCaptive, return the
    /// member to the source faction roster, record the conviction
    /// events (oathkeeping +1 on source, stewardship +1 on captor) per
    /// browser simulation.js:11136-11138, and flip Active=false.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AICaptiveRansomExecutionSystem : ISystem
    {
        public const float RansomBaseGoldCost     = 70f;
        public const float RansomBaseInfluenceCost = 18f;
        public const float RansomBaseDurationInWorldDays = 16f;

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
                if (covert.LastFiredOp != CovertOpKind.CaptiveRansom)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                TryDispatchRansom(em, sourceEntity, sourceFaction.FactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchRansom(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            float inWorldDays)
        {
            // Captive picker (shared with the rescue system).
            if (!AICaptiveRescueExecutionSystem.TryPickCaptive(em, sourceFactionId,
                    out var captiveMemberId,
                    out var captiveMemberTitle,
                    out var captorFactionId))
                return;

            // Gate: no existing active dynasty operation for this member.
            if (AICaptiveRescueExecutionSystem.HasActiveOperationForMember(em, captiveMemberId)) return;

            // Gate: source has Diplomat + Merchant operatives.
            if (!AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(em, sourceEntity,
                    new DynastyRole[] { DynastyRole.Diplomat, DynastyRole.Merchant, DynastyRole.HeirDesignate, DynastyRole.HeadOfBloodline },
                    out var diplomatMemberId, out var _, out var _))
                return;
            if (!AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(em, sourceEntity,
                    new DynastyRole[] { DynastyRole.Merchant, DynastyRole.Governor, DynastyRole.HeadOfBloodline },
                    out var merchantMemberId, out var _, out var _))
                return;

            // Gate: source affords gold + influence.
            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < RansomBaseGoldCost) return;
            if (resources.Influence < RansomBaseInfluenceCost) return;

            // Capacity gate (sub-slice 18).
            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ effects

            resources.Gold      -= RansomBaseGoldCost;
            resources.Influence -= RansomBaseInfluenceCost;
            em.SetComponentData(sourceEntity, resources);

            var operationId = BuildOperationId(sourceFactionId, captorFactionId, captiveMemberId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.CaptiveRansom,
                captorFactionId,
                captiveMemberId);

            em.AddComponentData(entity, new DynastyOperationCaptiveRansomComponent
            {
                ResolveAtInWorldDays = inWorldDays + RansomBaseDurationInWorldDays,
                CaptiveMemberId      = captiveMemberId,
                CaptiveMemberTitle   = captiveMemberTitle,
                CaptorFactionId      = captorFactionId,
                DiplomatMemberId     = diplomatMemberId,
                MerchantMemberId     = merchantMemberId,
                ProjectedChance      = 1f,
                IntensityCost        = 0f,
                EscrowCost           = new DynastyOperationEscrowCost
                {
                    Gold      = RansomBaseGoldCost,
                    Influence = RansomBaseInfluenceCost,
                },
            });

            PushRansomDispatchMessage(em, sourceFactionId, captiveMemberTitle, captorFactionId);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushRansomDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes captiveMemberTitle,
            FixedString32Bytes captorFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" opens ransom terms for ");
            message.Append(captiveMemberTitle);
            message.Append((FixedString32Bytes)" with ");
            message.Append(captorFactionId);
            message.Append((FixedString32Bytes)".");

            // Browser tone routing simulation.js:11062: source==player -> info,
            // else -> good.
            var tone = sourceFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Info
                : NarrativeMessageTone.Good;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes captiveMemberId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-ransom-");
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
