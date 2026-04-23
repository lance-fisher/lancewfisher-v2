using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Bloodlines.Dynasties.DynastySuccessionSystem))]
    public partial struct SuccessionPreferenceResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            if (!SystemAPI.TryGetSingleton<DualClockComponent>(out var dualClock))
            {
                return;
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            PruneInvalidPreferences(entityManager, dualClock.InWorldDays, ecb);

            var requestQuery = SystemAPI.QueryBuilder()
                .WithAll<PlayerSuccessionPreferenceRequestComponent>()
                .Build();
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerSuccessionPreferenceRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerSuccessionPreferenceRequestComponent>(Allocator.Temp);

            for (int i = 0; i < requests.Length; i++)
            {
                TryResolveRequest(entityManager, requests[i], dualClock.InWorldDays, ecb);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryResolveRequest(
            EntityManager entityManager,
            in PlayerSuccessionPreferenceRequestComponent request,
            float inWorldDays,
            EntityCommandBuffer ecb)
        {
            if (request.SourceFactionId.Length == 0 || request.TargetMemberId.Length == 0)
            {
                return;
            }

            Entity factionEntity = PlayerFaithDeclarationUtility.FindFactionEntity(
                entityManager,
                request.SourceFactionId);
            if (factionEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, factionEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(factionEntity) ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity) ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity) ||
                !TryResolveFactionMember(
                    entityManager,
                    factionEntity,
                    request.TargetMemberId,
                    out var targetMemberEntity,
                    out var targetMember) ||
                !IsEligibleSuccessor(targetMember))
            {
                return;
            }

            if (entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
            {
                var existing = entityManager.GetComponentData<SuccessionPreferenceComponent>(factionEntity);
                if (existing.PreferredHeirMemberId.Equals(targetMember.MemberId) &&
                    existing.PreferredHeirEntity == targetMemberEntity &&
                    !SuccessionPreferenceRules.IsExpired(existing.DesignationExpiresAtInWorldDays, inWorldDays))
                {
                    return;
                }
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            if (resources.Gold < SuccessionPreferenceRules.DesignationGoldCost ||
                dynasty.Legitimacy < SuccessionPreferenceRules.DesignationLegitimacyCost)
            {
                return;
            }

            resources.Gold -= SuccessionPreferenceRules.DesignationGoldCost;
            dynasty.Legitimacy -= SuccessionPreferenceRules.DesignationLegitimacyCost;
            entityManager.SetComponentData(factionEntity, resources);
            entityManager.SetComponentData(factionEntity, dynasty);

            var preference = new SuccessionPreferenceComponent
            {
                PreferredHeirEntity = targetMemberEntity,
                PreferredHeirMemberId = targetMember.MemberId,
                DesignationExpiresAtInWorldDays =
                    inWorldDays + SuccessionPreferenceRules.DesignationDurationInWorldDays,
                DesignationCostPaid = 1,
            };

            if (entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
            {
                entityManager.SetComponentData(factionEntity, preference);
            }
            else
            {
                ecb.AddComponent(factionEntity, preference);
            }
        }

        private static void PruneInvalidPreferences(
            EntityManager entityManager,
            float inWorldDays,
            EntityCommandBuffer ecb)
        {
            var preferenceQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SuccessionPreferenceComponent>(),
                ComponentType.ReadOnly<DynastyMemberRef>());
            using NativeArray<Entity> factionEntities = preferenceQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<SuccessionPreferenceComponent> preferences =
                preferenceQuery.ToComponentDataArray<SuccessionPreferenceComponent>(Allocator.Temp);
            preferenceQuery.Dispose();

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (ShouldClearPreference(
                        entityManager,
                        factionEntities[i],
                        preferences[i],
                        inWorldDays))
                {
                    ecb.RemoveComponent<SuccessionPreferenceComponent>(factionEntities[i]);
                }
            }
        }

        private static bool ShouldClearPreference(
            EntityManager entityManager,
            Entity factionEntity,
            in SuccessionPreferenceComponent preference,
            float inWorldDays)
        {
            if (SuccessionPreferenceRules.IsExpired(preference.DesignationExpiresAtInWorldDays, inWorldDays))
            {
                return true;
            }

            if (preference.PreferredHeirEntity == Entity.Null ||
                !entityManager.Exists(preference.PreferredHeirEntity) ||
                !entityManager.HasComponent<DynastyMemberComponent>(preference.PreferredHeirEntity))
            {
                return true;
            }

            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return true;
            }

            var member = entityManager.GetComponentData<DynastyMemberComponent>(preference.PreferredHeirEntity);
            return !preference.PreferredHeirMemberId.Equals(member.MemberId) ||
                   !IsEligibleSuccessor(member) ||
                   !FactionContainsMember(entityManager, factionEntity, preference.PreferredHeirEntity);
        }

        internal static bool IsEligibleSuccessor(in DynastyMemberComponent member)
        {
            return member.Status == DynastyMemberStatus.Active &&
                   member.Role != DynastyRole.HeadOfBloodline;
        }

        internal static bool TryResolveFactionMember(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            memberEntity = Entity.Null;
            member = default;
            if (memberId.Length == 0 || !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var candidateEntity = roster[i].Member;
                if (candidateEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(candidateEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(candidateEntity);
                if (!candidate.MemberId.Equals(memberId))
                {
                    continue;
                }

                memberEntity = candidateEntity;
                member = candidate;
                return true;
            }

            return false;
        }

        internal static bool FactionContainsMember(
            EntityManager entityManager,
            Entity factionEntity,
            Entity memberEntity)
        {
            if (memberEntity == Entity.Null || !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                if (roster[i].Member == memberEntity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
