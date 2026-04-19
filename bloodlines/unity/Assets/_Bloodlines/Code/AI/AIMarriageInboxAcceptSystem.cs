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
    /// Scope: mechanical record creation only. Governance authority costs,
    /// hostility drop, conviction event recording, legitimacy +2, in-world
    /// time declaration, and message pushes are browser-side effects that can
    /// layer on in future slices without reshaping this system.
    ///
    /// Browser reference: ai.js tryAiAcceptIncomingMarriage (~2880-2895) plus
    /// dispatch hook at marriageInboxTimer block (~2632-2636). Simulation-side
    /// sink: simulation.js acceptMarriage (~7388-7469).
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

                TryAcceptIncoming(em, sourceFactionId, aiFaction.FactionId, inWorldDays);

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
            float inWorldDays)
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
