using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageInboxAccept
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the browser
    /// tryAiAcceptIncomingMarriage behavior: find the first pending
    /// MarriageProposalComponent with source = "player" and target = this AI
    /// faction, flip it to Accepted, and create primary + mirror
    /// MarriageComponent entities so MarriageGestationSystem can schedule child
    /// generation at 60 in-world days.
    ///
    /// Like sub-slice 8, this clears LastFiredOp back to None after processing
    /// regardless of outcome so one dispatch produces one execution attempt.
    ///
    /// Scope: mechanical record creation only. Hostility drop, conviction
    /// event recording, legitimacy +2, in-world time declaration, and
    /// message pushes layer on through AIMarriageAcceptEffectsSystem
    /// (sub-slice 11). Governance authority terms (head-direct vs acting
    /// authority) are resolved here in sub-slice 12 via
    /// MarriageAuthorityEvaluator and attached as a
    /// MarriageAcceptanceTermsComponent on the primary marriage entity; the
    /// effects system then applies the legitimacy cost on the target
    /// (spouse) side before the +2 bonus. If the evaluator reports no
    /// authority path (no head/heir/envoy), the accept is rejected to match
    /// the browser getMarriageAcceptanceTerms guard.
    ///
    /// Browser reference: ai.js tryAiAcceptIncomingMarriage (~2880-2895) plus
    /// dispatch hook at marriageInboxTimer block (~2632-2636). Simulation-side
    /// sink: simulation.js acceptMarriage (~7388-7469). Authority guard:
    /// getMarriageAcceptanceTerms (~6327).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIMarriageProposalExecutionSystem))]
    public partial struct AIMarriageInboxAcceptSystem : ISystem
    {
        private const float GestationInWorldDays =
            MarriageGestationSystem.GestationInWorldDays;

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
                var aiFactionEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(aiFactionEntity);
                if (covert.LastFiredOp != CovertOpKind.MarriageInboxAccept)
                    continue;

                var aiFaction = em.GetComponentData<FactionComponent>(aiFactionEntity);

                // Source is hardcoded as "player" to mirror the browser convention
                // where the AI only accepts player-initiated proposals.
                var sourceFactionId = new FixedString32Bytes("player");

                // Resolve target (AI) authority terms before record creation.
                // Browser getMarriageAcceptanceTerms rejects when no authority
                // is available; MarriageAuthorityEvaluator returns false in
                // that case and TryAcceptIncoming early-exits.
                if (!MarriageAuthorityEvaluator.TryResolve(
                        em, aiFactionEntity,
                        out MarriageAuthorityMode authorityMode,
                        out float legitimacyCost))
                {
                    covert.LastFiredOp = CovertOpKind.None;
                    em.SetComponentData(aiFactionEntity, covert);
                    continue;
                }

                TryAcceptIncoming(em, sourceFactionId, aiFaction.FactionId,
                    inWorldDays, authorityMode, legitimacyCost);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(aiFactionEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ accept

        private static void TryAcceptIncoming(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays,
            MarriageAuthorityMode authorityMode,
            float legitimacyCost)
        {
            // Find first pending proposal matching source -> target.
            var proposalQuery = em.CreateEntityQuery(
                ComponentType.ReadWrite<MarriageProposalComponent>());
            if (proposalQuery.IsEmpty) { proposalQuery.Dispose(); return; }

            var proposalEntities = proposalQuery.ToEntityArray(Allocator.Temp);
            var proposals        = proposalQuery.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            proposalQuery.Dispose();

            Entity matchEntity = Entity.Null;
            MarriageProposalComponent match = default;

            for (int i = 0; i < proposalEntities.Length; i++)
            {
                var p = proposals[i];
                if (p.Status != MarriageProposalStatus.Pending) continue;
                if (!p.SourceFactionId.Equals(sourceFactionId)) continue;
                if (!p.TargetFactionId.Equals(targetFactionId)) continue;
                matchEntity = proposalEntities[i];
                match       = p;
                break;
            }

            proposalEntities.Dispose();
            proposals.Dispose();

            if (matchEntity == Entity.Null) return;

            // Flip proposal to accepted.
            match.Status = MarriageProposalStatus.Accepted;
            em.SetComponentData(matchEntity, match);

            // Create primary marriage record (head = source side, mirrors browser's
            // source.dynasty.marriages primary entry).
            var marriageId = BuildMarriageId(match.ProposalId);
            float expectedChild = inWorldDays + GestationInWorldDays;

            var primary = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(primary, new MarriageComponent
            {
                MarriageId                 = marriageId,
                HeadFactionId              = match.SourceFactionId,
                HeadMemberId               = match.SourceMemberId,
                SpouseFactionId            = match.TargetFactionId,
                SpouseMemberId             = match.TargetMemberId,
                MarriedAtInWorldDays       = inWorldDays,
                ExpectedChildAtInWorldDays = expectedChild,
                IsPrimary                  = true,
                ChildGenerated             = false,
                Dissolved                  = false,
            });
            // Tag the primary record so AIMarriageAcceptEffectsSystem applies the
            // browser's one-shot legitimacy/hostility effects exactly once. Mirror
            // record is not tagged to prevent double application.
            em.AddComponent<MarriageAcceptEffectsPendingTag>(primary);

            // Attach acceptance terms so the effects system can apply the
            // governance-authority legitimacy cost on the target (spouse)
            // side before the +2 bonus, and record the matching Stewardship
            // conviction event. Browser parity: simulation.js acceptMarriage
            // calls applyMarriageGovernanceLegitimacyCost before the
            // legitimacy +2 block (simulation.js ~7449 vs ~7458).
            em.AddComponentData(primary, new MarriageAcceptanceTermsComponent
            {
                AuthorityMode  = authorityMode,
                LegitimacyCost = legitimacyCost,
            });

            // Create mirror marriage record (head = target side). Browser creates
            // two records so both factions can enumerate their own marriages.
            // MarriageGestationSystem filters on IsPrimary so only one child
            // spawns.
            var mirror = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(mirror, new MarriageComponent
            {
                MarriageId                 = marriageId,
                HeadFactionId              = match.TargetFactionId,
                HeadMemberId               = match.TargetMemberId,
                SpouseFactionId            = match.SourceFactionId,
                SpouseMemberId             = match.SourceMemberId,
                MarriedAtInWorldDays       = inWorldDays,
                ExpectedChildAtInWorldDays = expectedChild,
                IsPrimary                  = false,
                ChildGenerated             = false,
                Dissolved                  = false,
            });
        }

        // ------------------------------------------------------------------ helpers

        private static FixedString64Bytes BuildMarriageId(FixedString64Bytes proposalId)
        {
            var id = new FixedString64Bytes("marriage-");
            for (int k = 0; k < proposalId.Length && id.Length < 58; k++)
                id.Append(proposalId[k]);
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
