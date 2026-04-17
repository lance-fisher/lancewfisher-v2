using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Applies projectile damage when the projectile reaches its terminal target position and
    /// then destroys the projectile entity.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileMovementSystem))]
    public partial struct ProjectileImpactSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (projectile, position, entity) in
                SystemAPI.Query<RefRO<ProjectileComponent>, RefRO<PositionComponent>>()
                    .WithEntityAccess())
            {
                float arrivalRadius = math.max(0.05f, projectile.ValueRO.ArrivalRadius);
                if (math.distancesq(position.ValueRO.Value, projectile.ValueRO.TargetPosition) >
                    (arrivalRadius * arrivalRadius))
                {
                    continue;
                }

                if (projectile.ValueRO.TargetEntity != Entity.Null &&
                    entityManager.Exists(projectile.ValueRO.TargetEntity) &&
                    !entityManager.HasComponent<DeadTag>(projectile.ValueRO.TargetEntity) &&
                    entityManager.HasComponent<HealthComponent>(projectile.ValueRO.TargetEntity))
                {
                    var targetHealth = entityManager.GetComponentData<HealthComponent>(projectile.ValueRO.TargetEntity);
                    if (targetHealth.Current > 0f)
                    {
                        targetHealth.Current = math.max(0f, targetHealth.Current - math.max(0f, projectile.ValueRO.Damage));
                        entityManager.SetComponentData(projectile.ValueRO.TargetEntity, targetHealth);
                    }
                }

                ecb.DestroyEntity(entity);
            }
        }
    }
}
