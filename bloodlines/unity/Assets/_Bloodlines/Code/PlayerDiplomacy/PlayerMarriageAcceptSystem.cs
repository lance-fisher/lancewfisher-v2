using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of simulation.js acceptMarriage and
    /// getMarriageAcceptanceTerms. Consumes a debug-issued accept request,
    /// resolves the target-side authority profile, flips the proposal to
    /// Accepted, creates primary + mirror MarriageComponent records, applies
    /// the target legitimacy cost before the +2 marriage bonus, drops
    /// hostility both ways, records oathkeeping +2 on both dynasties, and
    /// queues the canonical 30 in-world day declaration jump.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MarriageDeathDissolutionSystem))]
    [UpdateAfter(typeof(MarriageProposalExpirationSystem))]
    [UpdateAfter(typeof(PlayerMarriageProposalSystem))]
    [UpdateBefore(typeof(MarriageGestationSystem))]
    [UpdateBefore(typeof(DualClockDeclarationSystem))]
    public partial struct PlayerMarriageAcceptSystem : ISystem
    {
        private const float GestationInWorldDays =
            MarriageGestationSystem.GestationInWorldDays;

        private const float MarriageLegitimacyBonus = 2f;
        private const float MarriageOathkeepingBonus = 2f;
        private const float MarriageTimeJumpInWorldDays = 30f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMarriageAcceptRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = GetInWorldDays(entityManager);
            var clockEntity = SystemAPI.GetSingletonEntity<DualClockComponent>();
            EnsureDeclarationBuffer(entityManager, clockEntity);

            var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerMarriageAcceptRequestComponent>());
            var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            var requests = requestQuery.ToComponentDataArray<PlayerMarriageAcceptRequestComponent>(Allocator.Temp);
            requestQuery.Dispose();

            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryAcceptProposal(
                    entityManager,
                    clockEntity,
                    requests[i].ProposalEntityIndex,
                    inWorldDays);
                entityManager.DestroyEntity(requestEntities[i]);
            }

            requestEntities.Dispose();
            requests.Dispose();
        }

        private static void TryAcceptProposal(
            EntityManager entityManager,
            Entity clockEntity,
            int proposalEntityIndex,
            float inWorldDays)
        {
            if (!TryFindProposalEntityByIndex(
                    entityManager,
                    proposalEntityIndex,
                    out var proposalEntity))
            {
                return;
            }

            var proposal = entityManager.GetComponentData<MarriageProposalComponent>(proposalEntity);
            if (proposal.Status != MarriageProposalStatus.Pending)
            {
                return;
            }

            var sourceFactionEntity = FindFactionEntity(entityManager, proposal.SourceFactionId);
            var targetFactionEntity = FindFactionEntity(entityManager, proposal.TargetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                return;
            }

            if (!HasDynasty(entityManager, sourceFactionEntity) ||
                !HasDynasty(entityManager, targetFactionEntity))
            {
                return;
            }

            if (!TryResolveMemberInFaction(
                    entityManager,
                    sourceFactionEntity,
                    proposal.SourceMemberId,
                    out var sourceMember) ||
                !TryResolveMemberInFaction(
                    entityManager,
                    targetFactionEntity,
                    proposal.TargetMemberId,
                    out var targetMember))
            {
                return;
            }

            if (!IsMarriageCandidateAvailable(sourceMember.Status) ||
                !IsMarriageCandidateAvailable(targetMember.Status))
            {
                return;
            }

            var authority = PlayerMarriageAuthorityEvaluator.BuildGovernanceStatus(
                entityManager,
                targetFactionEntity).Authority;
            if (!authority.Available)
            {
                return;
            }

            proposal.Status = MarriageProposalStatus.Accepted;
            entityManager.SetComponentData(proposalEntity, proposal);

            var marriageId = BuildMarriageId(proposal.ProposalId);
            float expectedChildAt = inWorldDays + GestationInWorldDays;

            CreateMarriageRecord(
                entityManager,
                marriageId,
                proposal.SourceFactionId,
                proposal.SourceMemberId,
                proposal.TargetFactionId,
                proposal.TargetMemberId,
                inWorldDays,
                expectedChildAt,
                isPrimary: true);

            CreateMarriageRecord(
                entityManager,
                marriageId,
                proposal.TargetFactionId,
                proposal.TargetMemberId,
                proposal.SourceFactionId,
                proposal.SourceMemberId,
                inWorldDays,
                expectedChildAt,
                isPrimary: false);

            ApplyGovernanceLegitimacyCost(
                entityManager,
                targetFactionEntity,
                authority.LegitimacyCost);
            RemoveHostility(entityManager, sourceFactionEntity, proposal.TargetFactionId);
            RemoveHostility(entityManager, targetFactionEntity, proposal.SourceFactionId);
            ApplyConvictionBonus(entityManager, sourceFactionEntity);
            ApplyConvictionBonus(entityManager, targetFactionEntity);
            ApplyLegitimacyBonus(entityManager, sourceFactionEntity);
            ApplyLegitimacyBonus(entityManager, targetFactionEntity);
            QueueMarriageDeclaration(
                entityManager,
                clockEntity,
                proposal.SourceFactionId,
                proposal.TargetFactionId);
        }

        private static void CreateMarriageRecord(
            EntityManager entityManager,
            FixedString64Bytes marriageId,
            FixedString32Bytes headFactionId,
            FixedString64Bytes headMemberId,
            FixedString32Bytes spouseFactionId,
            FixedString64Bytes spouseMemberId,
            float inWorldDays,
            float expectedChildAt,
            bool isPrimary)
        {
            var marriageEntity = entityManager.CreateEntity(typeof(MarriageComponent));
            entityManager.SetComponentData(marriageEntity, new MarriageComponent
            {
                MarriageId = marriageId,
                HeadFactionId = headFactionId,
                HeadMemberId = headMemberId,
                SpouseFactionId = spouseFactionId,
                SpouseMemberId = spouseMemberId,
                MarriedAtInWorldDays = inWorldDays,
                ExpectedChildAtInWorldDays = expectedChildAt,
                IsPrimary = isPrimary,
                ChildGenerated = false,
                Dissolved = false,
                DissolvedAtInWorldDays = 0f,
            });
        }

        private static void QueueMarriageDeclaration(
            EntityManager entityManager,
            Entity clockEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            if (clockEntity == Entity.Null)
            {
                return;
            }

            EnsureDeclarationBuffer(entityManager, clockEntity);
            var requests = entityManager.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);

            var reason = new FixedString64Bytes("Marriage ");
            reason.Append(sourceFactionId);
            reason.Append("/");
            reason.Append(targetFactionId);

            requests.Add(new DeclareInWorldTimeRequest
            {
                DaysDelta = MarriageTimeJumpInWorldDays,
                Reason = reason,
            });
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

        private static void ApplyConvictionBonus(EntityManager entityManager, Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return;
            }

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(
                ref conviction,
                ConvictionBucket.Oathkeeping,
                MarriageOathkeepingBonus);
            entityManager.SetComponentData(factionEntity, conviction);
        }

        private static void ApplyLegitimacyBonus(EntityManager entityManager, Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return;
            }

            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = math.clamp(
                dynasty.Legitimacy + MarriageLegitimacyBonus,
                0f,
                100f);
            entityManager.SetComponentData(factionEntity, dynasty);
        }

        private static void RemoveHostility(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes hostileFactionId)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                return;
            }

            var hostility = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = hostility.Length - 1; i >= 0; i--)
            {
                if (hostility[i].HostileFactionId.Equals(hostileFactionId))
                {
                    hostility.RemoveAt(i);
                }
            }
        }

        private static bool TryFindProposalEntityByIndex(
            EntityManager entityManager,
            int proposalEntityIndex,
            out Entity proposalEntity)
        {
            proposalEntity = Entity.Null;
            if (proposalEntityIndex < 0)
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MarriageProposalComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            var proposalEntities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < proposalEntities.Length; i++)
            {
                if (proposalEntities[i].Index == proposalEntityIndex)
                {
                    proposalEntity = proposalEntities[i];
                    break;
                }
            }

            proposalEntities.Dispose();
            return proposalEntity != Entity.Null;
        }

        private static bool HasDynasty(EntityManager entityManager, Entity factionEntity)
        {
            return factionEntity != Entity.Null &&
                   entityManager.HasComponent<DynastyStateComponent>(factionEntity) &&
                   entityManager.HasBuffer<DynastyMemberRef>(factionEntity);
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

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>());
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
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DualClockComponent>());
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

        private static FixedString64Bytes BuildMarriageId(FixedString64Bytes proposalId)
        {
            var id = new FixedString64Bytes("marriage-");
            for (int i = 0; i < proposalId.Length && id.Length < 58; i++)
            {
                id.Append(proposalId[i]);
            }

            return id;
        }

        private static void EnsureDeclarationBuffer(EntityManager entityManager, Entity clockEntity)
        {
            if (clockEntity == Entity.Null ||
                entityManager.HasBuffer<DeclareInWorldTimeRequest>(clockEntity))
            {
                return;
            }

            entityManager.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
        }
    }
}
