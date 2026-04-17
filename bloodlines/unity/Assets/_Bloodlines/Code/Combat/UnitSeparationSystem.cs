using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Pathing.UnitMovementSystem))]
    [UpdateBefore(typeof(Bloodlines.Pathing.PositionToLocalTransformSystem))]
    public partial struct UnitSeparationSystem : ISystem
    {
        private const float MaximumNudgePerFrame = 0.05f;
        private const float MinimumDirectionLengthSq = 0.0001f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitSeparationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<RecentImpactComponent>(),
                ComponentType.ReadOnly<UnitSeparationComponent>(),
                ComponentType.ReadWrite<PositionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var recentImpacts = query.ToComponentDataArray<RecentImpactComponent>(Allocator.Temp);
            using var separations = query.ToComponentDataArray<UnitSeparationComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var accumulatedOffsets = new NativeArray<float3>(entities.Length, Allocator.Temp);
            try
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entityManager.HasComponent<DeadTag>(entities[i]) ||
                        healthValues[i].Current <= 0f ||
                        recentImpacts[i].SecondsRemaining > 0f)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < entities.Length; j++)
                    {
                        if (!factions[i].FactionId.Equals(factions[j].FactionId) ||
                            entityManager.HasComponent<DeadTag>(entities[j]) ||
                            healthValues[j].Current <= 0f ||
                            recentImpacts[j].SecondsRemaining > 0f)
                        {
                            continue;
                        }

                        float minimumDistance = math.max(0.1f, math.max(separations[i].Radius, separations[j].Radius));
                        float3 delta = positions[j].Value - positions[i].Value;
                        float distanceSq = math.lengthsq(delta);
                        float targetDistance = minimumDistance;
                        if (distanceSq >= targetDistance * targetDistance)
                        {
                            continue;
                        }

                        float3 direction;
                        float distance;
                        if (distanceSq <= MinimumDirectionLengthSq)
                        {
                            uint hash = (uint)(entities[i].Index * 397) ^ (uint)entities[j].Index;
                            float angle = math.radians(hash % 360);
                            direction = new float3(math.cos(angle), 0f, math.sin(angle));
                            distance = 0f;
                        }
                        else
                        {
                            distance = math.sqrt(distanceSq);
                            direction = delta / distance;
                        }

                        float overlap = targetDistance - distance;
                        float nudge = math.min(MaximumNudgePerFrame, overlap * 0.5f);
                        float3 offset = direction * nudge;
                        accumulatedOffsets[i] -= offset;
                        accumulatedOffsets[j] += offset;
                    }
                }

                for (int i = 0; i < entities.Length; i++)
                {
                    if (math.lengthsq(accumulatedOffsets[i]) <= 0f)
                    {
                        continue;
                    }

                    float magnitude = math.length(accumulatedOffsets[i]);
                    if (magnitude > MaximumNudgePerFrame)
                    {
                        accumulatedOffsets[i] = accumulatedOffsets[i] / magnitude * MaximumNudgePerFrame;
                    }

                    var position = positions[i];
                    position.Value += accumulatedOffsets[i];
                    entityManager.SetComponentData(entities[i], position);
                }
            }
            finally
            {
                accumulatedOffsets.Dispose();
            }
        }
    }
}
