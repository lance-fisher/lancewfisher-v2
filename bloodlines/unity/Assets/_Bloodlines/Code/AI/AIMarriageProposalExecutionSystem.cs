using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageProposal
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the browser
    /// tryAiMarriageProposal behavior: select a non-head, active/ruling source
    /// member, select a non-head, active/ruling target member, verify no
    /// already-in-force marriage and no already-pending proposal, then create a
    /// MarriageProposalComponent entity that MarriageProposalExpirationSystem
    /// will age out after 30 in-world days.
    ///
    /// The system clears LastFiredOp to None once it has processed the dispatch
    /// regardless of whether the proposal was created, so it behaves as a
    /// one-shot consumer per covert-ops fire. This matches the browser where
    /// tryAiMarriageProposal runs exactly once when the timer expires and then
    /// the timer is reset.
    ///
    /// Browser reference: ai.js tryAiMarriageProposal (~2897-2944) plus the
    /// dispatch at updateEnemyAi marriageProposalTimer block (~2616-2624).
    /// Simulation-side sink: simulation.js proposeMarriage (~7340).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIMarriageProposalExecutionSystem : ISystem
    {
        private const float ProposalExpirationInWorldDays =
            MarriageProposalExpirationSystem.ExpirationInWorldDays;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            // Collect AI factions with a pending MarriageProposal dispatch.
            var dispatchQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>());
            var dispatchEntities = dispatchQuery.ToEntityArray(Allocator.Temp);
            dispatchQuery.Dispose();

            for (int i = 0; i < dispatchEntities.Length; i++)
            {
                var sourceEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(sourceEntity);
                if (covert.LastFiredOp != CovertOpKind.MarriageProposal)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                // Target is hardcoded as "player" matching the browser convention.
                var targetFactionId = new FixedString32Bytes("player");

                TryCreateProposal(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                // Consume the dispatch regardless of gate outcome to match the
                // browser's single-fire-per-timer semantic.
                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ gates

        private static void TryCreateProposal(
            EntityManager em,
            Entity sourceFactionEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            // Gate 1: no in-force marriage already linking these factions.
            if (HasActiveMarriage(em, sourceFactionId, targetFactionId))
                return;

            // Gate 2: no pending proposal already from source to target.
            if (HasPendingProposal(em, sourceFactionId, targetFactionId))
                return;

            // Gate 3: select source member (non-head, active/ruling, not captured).
            if (!TrySelectCandidate(em, sourceFactionEntity,
                    out FixedString64Bytes sourceMemberId))
                return;

            // Gate 4: select target member from target faction (non-head, active/ruling).
            var targetFactionEntity = FindFactionEntity(em, targetFactionId);
            if (targetFactionEntity == Entity.Null)
                return;
            if (!TrySelectCandidate(em, targetFactionEntity,
                    out FixedString64Bytes targetMemberId))
                return;

            // Create the proposal entity.
            var proposalEntity = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId            = BuildProposalId(sourceFactionId, targetFactionId, inWorldDays),
                SourceFactionId       = sourceFactionId,
                SourceMemberId        = sourceMemberId,
                TargetFactionId       = targetFactionId,
                TargetMemberId        = targetMemberId,
                Status                = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = inWorldDays,
                ExpiresAtInWorldDays  = inWorldDays + ProposalExpirationInWorldDays,
            });
        }

        // ------------------------------------------------------------------ queries

        private static bool HasActiveMarriage(
            EntityManager em,
            FixedString32Bytes factionA,
            FixedString32Bytes factionB)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var marriages = q.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < marriages.Length; i++)
            {
                var m = marriages[i];
                if (m.Dissolved) continue;
                bool abMatch = m.HeadFactionId.Equals(factionA) && m.SpouseFactionId.Equals(factionB);
                bool baMatch = m.HeadFactionId.Equals(factionB) && m.SpouseFactionId.Equals(factionA);
                if (abMatch || baMatch) { found = true; break; }
            }
            marriages.Dispose();
            return found;
        }

        private static bool HasPendingProposal(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var proposals = q.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < proposals.Length; i++)
            {
                var p = proposals[i];
                if (p.Status != MarriageProposalStatus.Pending) continue;
                if (!p.SourceFactionId.Equals(sourceFactionId)) continue;
                if (!p.TargetFactionId.Equals(targetFactionId)) continue;
                found = true;
                break;
            }
            proposals.Dispose();
            return found;
        }

        private static bool TrySelectCandidate(
            EntityManager em,
            Entity factionEntity,
            out FixedString64Bytes memberId)
        {
            memberId = default;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity))
                return false;

            var buffer = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                var memberEntity = buffer[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role == DynastyRole.HeadOfBloodline) continue;
                if (member.Status != DynastyMemberStatus.Active &&
                    member.Status != DynastyMemberStatus.Ruling) continue;

                memberId = member.MemberId;
                return true;
            }

            return false;
        }

        private static Entity FindFactionEntity(
            EntityManager em,
            FixedString32Bytes factionId)
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

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }

        private static FixedString64Bytes BuildProposalId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("marriage-proposal-");
            id.Append(sourceFactionId);
            id.Append("-to-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
        }
    }
}
