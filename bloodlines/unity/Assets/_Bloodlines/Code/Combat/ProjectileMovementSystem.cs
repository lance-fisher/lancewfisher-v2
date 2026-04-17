using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Advances live projectiles toward a homing target or the last recorded target position.
    /// Arrival is resolved by ProjectileImpactSystem; expiry is handled here.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AttackResolutionSystem))]
    public partial struct ProjectileMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (projectileRw, positionRw, transformRw, entity) in
                SystemAPI.Query<RefRW<ProjectileComponent>, RefRW<PositionComponent>, RefRW<LocalTransform>>()
                    .WithEntityAccess())
            {
                var projectile = projectileRw.ValueRO;
                projectile.ElapsedSeconds += dt;

                if (projectile.ElapsedSeconds >= math.max(0.1f, projectile.MaxLifetimeSeconds))
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }

                float3 targetPosition = ResolveTargetPosition(entityManager, projectile);
                projectile.TargetPosition = targetPosition;

                float arrivalRadius = math.max(0.05f, projectile.ArrivalRadius);
                float3 currentPosition = positionRw.ValueRO.Value;
                float3 toTarget = targetPosition - currentPosition;
                float distance = math.length(toTarget);

                if (distance > arrivalRadius)
                {
                    float travelDistance = math.max(0.05f, projectile.Speed) * dt;
                    float3 nextPosition = distance <= travelDistance
                        ? targetPosition
                        : currentPosition + (toTarget / distance) * travelDistance;

                    positionRw.ValueRW = new PositionComponent { Value = nextPosition };
                    var localTransform = transformRw.ValueRO;
                    localTransform.Position = nextPosition;
                    transformRw.ValueRW = localTransform;
                }
                else
                {
                    positionRw.ValueRW = new PositionComponent { Value = targetPosition };
                    var localTransform = transformRw.ValueRO;
                    localTransform.Position = targetPosition;
                    transformRw.ValueRW = localTransform;
                }

                projectileRw.ValueRW = projectile;
            }
        }

        private static float3 ResolveTargetPosition(EntityManager entityManager, in ProjectileComponent projectile)
        {
            if (projectile.TargetEntity == Entity.Null ||
                !entityManager.Exists(projectile.TargetEntity) ||
                entityManager.HasComponent<DeadTag>(projectile.TargetEntity) ||
                !entityManager.HasComponent<HealthComponent>(projectile.TargetEntity) ||
                !entityManager.HasComponent<PositionComponent>(projectile.TargetEntity))
            {
                return projectile.TargetPosition;
            }

            var health = entityManager.GetComponentData<HealthComponent>(projectile.TargetEntity);
            if (health.Current <= 0f)
            {
                return projectile.TargetPosition;
            }

            return entityManager.GetComponentData<PositionComponent>(projectile.TargetEntity).Value;
        }
    }
}
