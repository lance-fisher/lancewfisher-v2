using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical control-point resource trickle.
    /// Browser runtime equivalent: the owned-territory yield branch in updateControlPoints.
    ///
    /// This first ECS slice intentionally keeps parity narrow:
    ///   - owned occupied control points pay 42% yield
    ///   - stabilized control points pay 100% yield
    ///   - neutral or contested points pay nothing
    ///
    /// Governor, doctrine, and political-event multipliers remain future follow-up work.
    /// The key requirement here is to preserve map-authored trickle values on the live ECS
    /// territory entities and route them into faction stockpiles once ownership exists.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PopulationGrowthSystem))]
    public partial struct ControlPointResourceTrickleSystem : ISystem
    {
        const float OccupiedYieldMultiplier = 0.42f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<ResourceStockpileComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var factionQuery = SystemAPI.QueryBuilder()
                .WithAll<FactionComponent, ResourceStockpileComponent>()
                .Build();

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            foreach (var controlPoint in SystemAPI.Query<RefRO<ControlPointComponent>>())
            {
                var ownerFactionId = controlPoint.ValueRO.OwnerFactionId;
                if (ownerFactionId.Length == 0 || controlPoint.ValueRO.IsContested)
                {
                    continue;
                }

                float territoryYield = controlPoint.ValueRO.ControlState switch
                {
                    ControlState.Stabilized => 1f,
                    ControlState.Occupied => OccupiedYieldMultiplier,
                    _ => 0f,
                };

                if (territoryYield <= 0f)
                {
                    continue;
                }

                int factionIndex = FindFactionIndex(factionIds, ownerFactionId);
                if (factionIndex < 0)
                {
                    continue;
                }

                var factionEntity = factionEntities[factionIndex];
                float politicalYieldMultiplier =
                    entityManager.HasComponent<SuccessionCrisisComponent>(factionEntity)
                        ? entityManager.GetComponentData<SuccessionCrisisComponent>(factionEntity).ResourceTrickleFactor
                        : 1f;
                float finalYield = territoryYield * politicalYieldMultiplier;

                var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
                resources.Gold += controlPoint.ValueRO.GoldTrickle * dt * finalYield;
                resources.Food += controlPoint.ValueRO.FoodTrickle * dt * finalYield;
                resources.Water += controlPoint.ValueRO.WaterTrickle * dt * finalYield;
                resources.Wood += controlPoint.ValueRO.WoodTrickle * dt * finalYield;
                resources.Stone += controlPoint.ValueRO.StoneTrickle * dt * finalYield;
                resources.Iron += controlPoint.ValueRO.IronTrickle * dt * finalYield;
                resources.Influence += controlPoint.ValueRO.InfluenceTrickle * dt * finalYield;
                entityManager.SetComponentData(factionEntity, resources);
            }
        }

        static int FindFactionIndex(NativeArray<FactionComponent> factions, FixedString32Bytes factionId)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
