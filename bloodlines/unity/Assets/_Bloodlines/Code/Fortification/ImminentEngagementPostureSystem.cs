using Bloodlines.Components;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Fortification
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ImminentEngagementWarningSystem))]
    [UpdateBefore(typeof(CombatStanceResolutionSystem))]
    [UpdateBefore(typeof(AttackResolutionSystem))]
    public partial struct ImminentEngagementPostureSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ImminentEngagementComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerImminentEngagementPostureRequestComponent>());
            using var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using var requests = requestQuery.ToComponentDataArray<PlayerImminentEngagementPostureRequestComponent>(Allocator.Temp);

            for (int i = 0; i < requestEntities.Length; i++)
            {
                byte postureId = ImminentEngagementPostureUtility.IsValidPostureId(requests[i].PostureId)
                    ? requests[i].PostureId
                    : (byte)ImminentEngagementPostureId.Steady;

                if (!TryFindSettlementEntity(entityManager, requests[i].SettlementId, out var settlementEntity) ||
                    !entityManager.HasComponent<ImminentEngagementComponent>(settlementEntity))
                {
                    ecb.DestroyEntity(requestEntities[i]);
                    continue;
                }

                var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlementEntity);
                engagement.SelectedResponseId = ImminentEngagementPostureUtility.ResolveResponseId(postureId);
                engagement.SelectedResponseLabel = ImminentEngagementPostureUtility.ResolveResponseLabel(postureId);
                entityManager.SetComponentData(settlementEntity, engagement);

                if (engagement.Active)
                {
                    var posture = ImminentEngagementPostureUtility.ResolveComponent(postureId);
                    if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
                    {
                        entityManager.SetComponentData(settlementEntity, posture);
                    }
                    else
                    {
                        entityManager.AddComponentData(settlementEntity, posture);
                    }
                }

                ecb.DestroyEntity(requestEntities[i]);
            }

            foreach (var (engagement, settlementEntity) in
                SystemAPI.Query<RefRO<ImminentEngagementComponent>>().WithEntityAccess())
            {
                if (!engagement.ValueRO.Active)
                {
                    if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
                    {
                        ecb.RemoveComponent<ImminentEngagementPostureComponent>(settlementEntity);
                    }

                    continue;
                }

                var posture = ImminentEngagementPostureUtility.ResolveComponent(engagement.ValueRO.SelectedResponseId);
                if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
                {
                    entityManager.SetComponentData(settlementEntity, posture);
                }
                else
                {
                    ecb.AddComponent(settlementEntity, posture);
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

        private static bool TryFindSettlementEntity(
            EntityManager entityManager,
            FixedString64Bytes settlementId,
            out Entity settlementEntity)
        {
            settlementEntity = Entity.Null;
            if (settlementId.IsEmpty)
            {
                return false;
            }

            var settlementQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<SettlementComponent>());
            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlements = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                if (!settlements[i].SettlementId.Equals(settlementId))
                {
                    continue;
                }

                settlementEntity = settlementEntities[i];
                return true;
            }

            return false;
        }
    }
}
