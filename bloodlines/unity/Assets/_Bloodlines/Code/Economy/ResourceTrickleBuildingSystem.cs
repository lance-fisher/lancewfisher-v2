using Bloodlines.Components;
using Bloodlines.Raids;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical passive-resource trickle system. Ports the browser runtime's
    /// tickPassiveResources behavior to ECS: each completed, alive, non-under-construction
    /// resource-producing building adds `amount * dt` to its owning faction's
    /// ResourceStockpileComponent for each resource it produces.
    ///
    /// Master doctrine sections IV (population), V (water), VI (food), and VII
    /// (economy) all expect this loop to be live. Without it, farms, wells, and
    /// other trickle structures are decorative.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PopulationGrowthSystem))]
    public partial struct ResourceTrickleBuildingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ResourceTrickleBuildingComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                return;
            }

            var entityManager = state.EntityManager;

            var stockpileAccumulator = new NativeParallelHashMap<FixedString32Bytes, ResourceStockpileDelta>(
                16,
                Allocator.Temp);
            try
            {

            foreach (var (trickle, faction, health, entity) in
                SystemAPI.Query<
                    RefRO<ResourceTrickleBuildingComponent>,
                    RefRO<FactionComponent>,
                    RefRO<HealthComponent>>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                if (entityManager.HasComponent<ConstructionStateComponent>(entity))
                {
                    continue;
                }

                if (entityManager.HasComponent<BuildingRaidStateComponent>(entity) &&
                    ScoutRaidCanon.IsBuildingRaided(
                        entityManager.GetComponentData<BuildingRaidStateComponent>(entity),
                        SystemAPI.Time.ElapsedTime))
                {
                    continue;
                }

                var t = trickle.ValueRO;
                var delta = new ResourceStockpileDelta
                {
                    Gold = t.GoldPerSecond * dt,
                    Food = t.FoodPerSecond * dt,
                    Water = t.WaterPerSecond * dt,
                    Wood = t.WoodPerSecond * dt,
                    Stone = t.StonePerSecond * dt,
                    Iron = t.IronPerSecond * dt,
                    Influence = t.InfluencePerSecond * dt,
                };

                if (!HasAnyContribution(in delta))
                {
                    continue;
                }

                if (stockpileAccumulator.TryGetValue(faction.ValueRO.FactionId, out var existing))
                {
                    existing.Gold += delta.Gold;
                    existing.Food += delta.Food;
                    existing.Water += delta.Water;
                    existing.Wood += delta.Wood;
                    existing.Stone += delta.Stone;
                    existing.Iron += delta.Iron;
                    existing.Influence += delta.Influence;
                    stockpileAccumulator[faction.ValueRO.FactionId] = existing;
                }
                else
                {
                    stockpileAccumulator.Add(faction.ValueRO.FactionId, delta);
                }
            }

            if (stockpileAccumulator.Count() == 0)
            {
                return;
            }

            foreach (var (factionRo, stockpileRw, factionEntity) in
                SystemAPI.Query<
                    RefRO<FactionComponent>,
                    RefRW<ResourceStockpileComponent>>()
                .WithEntityAccess())
            {
                if (!stockpileAccumulator.TryGetValue(factionRo.ValueRO.FactionId, out var delta))
                {
                    continue;
                }

                ref var stockpile = ref stockpileRw.ValueRW;
                stockpile.Gold += delta.Gold;
                stockpile.Food += delta.Food;
                stockpile.Water += delta.Water;
                stockpile.Wood += delta.Wood;
                stockpile.Stone += delta.Stone;
                stockpile.Iron += delta.Iron;
                stockpile.Influence += delta.Influence;
            }
            }
            finally
            {
                stockpileAccumulator.Dispose();
            }
        }

        private static bool HasAnyContribution(in ResourceStockpileDelta delta)
        {
            return delta.Gold > 0f ||
                   delta.Food > 0f ||
                   delta.Water > 0f ||
                   delta.Wood > 0f ||
                   delta.Stone > 0f ||
                   delta.Iron > 0f ||
                   delta.Influence > 0f;
        }

        private struct ResourceStockpileDelta
        {
            public float Gold;
            public float Food;
            public float Water;
            public float Wood;
            public float Stone;
            public float Iron;
            public float Influence;
        }
    }
}
