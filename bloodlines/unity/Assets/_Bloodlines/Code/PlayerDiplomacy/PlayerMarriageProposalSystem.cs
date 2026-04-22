using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of simulation.js proposeMarriage. Validates source and
    /// target dynasty members, applies the source-side governance legitimacy
    /// cost on success, and spawns a pending MarriageProposalComponent that the
    /// existing dynasty expiration system can age out after 90 in-world days.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MarriageDeathDissolutionSystem))]
    [UpdateAfter(typeof(MarriageProposalExpirationSystem))]
    public partial struct PlayerMarriageProposalSystem : ISystem
    {
        private const float ProposalExpirationInWorldDays =
            MarriageProposalExpirationSystem.ExpirationInWorldDays;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMarriageProposalRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = GetInWorldDays(entityManager);

            var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerMarriageProposalRequestComponent>());
            var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            var requests = requestQuery.ToComponentDataArray<PlayerMarriageProposalRequestComponent>(Allocator.Temp);
            requestQuery.Dispose();

            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryCreateProposal(entityManager, requests[i], inWorldDays);
                entityManager.DestroyEntity(requestEntities[i]);
            }

            requestEntities.Dispose();
            requests.Dispose();
        }

        private static void TryCreateProposal(
            EntityManager entityManager,
            PlayerMarriageProposalRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.TargetMemberId.Length == 0 ||
                request.SourceFactionId.Equals(request.TargetFactionId))
            {
                return;
            }

            var sourceFactionEntity = FindFactionEntity(entityManager, request.SourceFactionId);
            var targetFactionEntity = FindFactionEntity(entityManager, request.TargetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                return;
            }

            if (!HasDynasty(entityManager, sourceFactionEntity) ||
                !HasDynasty(entityManager, targetFactionEntity))
            {
                return;
            }

            if (!TryResolveSourceMember(
                    entityManager,
                    sourceFactionEntity,
                    request.RequestedSourceMemberId,
                    out var sourceMember) ||
                !TryResolveMemberInFaction(
                    entityManager,
                    targetFactionEntity,
                    request.TargetMemberId,
                    out var targetMember))
            {
                return;
            }

            if (!IsMarriageCandidateAvailable(sourceMember.Status) ||
                !IsMarriageCandidateAvailable(targetMember.Status))
            {
                return;
            }

            if (MemberHasActiveMarriage(
                    entityManager,
                    request.SourceFactionId,
                    sourceMember.MemberId) &&
                !PlayerMarriageAuthorityEvaluator.FactionAllowsPolygamy(entityManager, sourceFactionEntity))
            {
                return;
            }

            if (MemberHasActiveMarriage(
                    entityManager,
                    request.TargetFactionId,
                    targetMember.MemberId) &&
                !PlayerMarriageAuthorityEvaluator.FactionAllowsPolygamy(entityManager, targetFactionEntity))
            {
                return;
            }

            if (HasPendingProposal(
                    entityManager,
                    request.SourceFactionId,
                    sourceMember.MemberId,
                    request.TargetFactionId,
                    targetMember.MemberId))
            {
                return;
            }

            var governance = PlayerMarriageAuthorityEvaluator.BuildGovernanceStatus(
                entityManager,
                sourceFactionEntity);
            if (!governance.CanOfferMarriage)
            {
                return;
            }

            var proposalEntity = entityManager.CreateEntity(typeof(MarriageProposalComponent));
            entityManager.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId = BuildProposalId(
                    request.SourceFactionId,
                    request.TargetFactionId,
                    proposalEntity.Index),
                SourceFactionId = request.SourceFactionId,
                SourceMemberId = sourceMember.MemberId,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = targetMember.MemberId,
                Status = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = inWorldDays,
                ExpiresAtInWorldDays = inWorldDays + ProposalExpirationInWorldDays,
            });

            ApplyGovernanceLegitimacyCost(
                entityManager,
                sourceFactionEntity,
                governance.Authority.LegitimacyCost);
        }

        private static bool HasDynasty(EntityManager entityManager, Entity factionEntity)
        {
            return factionEntity != Entity.Null &&
                   entityManager.HasComponent<DynastyStateComponent>(factionEntity) &&
                   entityManager.HasBuffer<DynastyMemberRef>(factionEntity);
        }

        private static bool TryResolveSourceMember(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes requestedSourceMemberId,
            out DynastyMemberComponent member)
        {
            member = default;

            if (requestedSourceMemberId.Length > 0)
            {
                return TryResolveMemberInFaction(
                    entityManager,
                    factionEntity,
                    requestedSourceMemberId,
                    out member);
            }

            if (TrySelectCandidateMember(
                    entityManager,
                    factionEntity,
                    preferNonHead: true,
                    out member))
            {
                return true;
            }

            return TrySelectCandidateMember(
                entityManager,
                factionEntity,
                preferNonHead: false,
                out member);
        }

        private static bool TrySelectCandidateMember(
            EntityManager entityManager,
            Entity factionEntity,
            bool preferNonHead,
            out DynastyMemberComponent member)
        {
            member = default;
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!IsMarriageCandidateAvailable(candidate.Status))
                {
                    continue;
                }

                bool isHead = candidate.Role == DynastyRole.HeadOfBloodline;
                if (preferNonHead && isHead)
                {
                    continue;
                }

                if (!preferNonHead && !isHead)
                {
                    continue;
                }

                member = candidate;
                return true;
            }

            return false;
        }

        private static bool TryResolveMemberInFaction(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId,
            out DynastyMemberComponent member)
        {
            member = default;
            if (memberId.Length == 0 || !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (candidate.MemberId.Equals(memberId))
                {
                    member = candidate;
                    return true;
                }
            }

            return false;
        }

        private static bool MemberHasActiveMarriage(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();

            bool found = false;
            for (int i = 0; i < marriages.Length; i++)
            {
                var marriage = marriages[i];
                if (marriage.Dissolved)
                {
                    continue;
                }

                bool headMatch = marriage.HeadFactionId.Equals(factionId) &&
                                 marriage.HeadMemberId.Equals(memberId);
                bool spouseMatch = marriage.SpouseFactionId.Equals(factionId) &&
                                   marriage.SpouseMemberId.Equals(memberId);
                if (headMatch || spouseMatch)
                {
                    found = true;
                    break;
                }
            }

            marriages.Dispose();
            return found;
        }

        private static bool HasPendingProposal(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes sourceMemberId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            var proposals = query.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            query.Dispose();

            bool found = false;
            for (int i = 0; i < proposals.Length; i++)
            {
                var proposal = proposals[i];
                if (proposal.Status != MarriageProposalStatus.Pending)
                {
                    continue;
                }

                if (!proposal.SourceFactionId.Equals(sourceFactionId) ||
                    !proposal.SourceMemberId.Equals(sourceMemberId) ||
                    !proposal.TargetFactionId.Equals(targetFactionId) ||
                    !proposal.TargetMemberId.Equals(targetMemberId))
                {
                    continue;
                }

                found = true;
                break;
            }

            proposals.Dispose();
            return found;
        }

        private static void ApplyGovernanceLegitimacyCost(
            EntityManager entityManager,
            Entity factionEntity,
            float legitimacyCost)
        {
            if (factionEntity == Entity.Null || legitimacyCost <= 0f)
            {
                return;
            }

            if (entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Legitimacy = math.clamp(dynasty.Legitimacy - legitimacyCost, 0f, 100f);
                entityManager.SetComponentData(factionEntity, dynasty);
            }

            if (entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Stewardship,
                    -legitimacyCost);
                entityManager.SetComponentData(factionEntity, conviction);
            }
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

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

        private static float GetInWorldDays(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            float inWorldDays = query.GetSingleton<DualClockComponent>().InWorldDays;
            query.Dispose();
            return inWorldDays;
        }

        private static bool IsMarriageCandidateAvailable(DynastyMemberStatus status)
        {
            return status == DynastyMemberStatus.Active ||
                   status == DynastyMemberStatus.Ruling;
        }

        private static FixedString64Bytes BuildProposalId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            int entityIndex)
        {
            var id = new FixedString64Bytes("marriage-proposal-");
            id.Append(sourceFactionId);
            id.Append("-to-");
            id.Append(targetFactionId);
            id.Append("-");
            id.Append(entityIndex);
            return id;
        }
    }
}
