using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical population growth. Browser runtime equivalent: tickPopulationGrowth.
    ///
    /// Growth is gated by food AND water AND available housing cap. If any of the three
    /// conditions fails, the growth accumulator stalls at its current value without
    /// resetting. This is the first line of the civilizational-depth vector and must
    /// never be substituted with an unconditional timer.
    ///
    /// Growth cadence: 18 seconds per head under canonical conditions. Consumes 1 food
    /// and 1 water per growth tick (reflects canonical population-to-supply coupling
    /// the master doctrine locks in sections IV and V).
    /// </summary>
    [BurstCompile]
    public partial struct PopulationGrowthSystem : ISystem
    {
        const float GrowthInterval = 18f;
        const float FoodPerGrowth = 1f;
        const float WaterPerGrowth = 1f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (populationRw, resourcesRw, waterCapRo) in
                SystemAPI.Query<
                    RefRW<PopulationComponent>,
                    RefRW<ResourceStockpileComponent>,
                    RefRO<WaterCapacityComponent>>())
            {
                ref var population = ref populationRw.ValueRW;
                ref var resources = ref resourcesRw.ValueRW;

                // Can the faction support another head?
                // Three independent gates per canon (food-water-population triangle):
                //   housing cap, food stockpile, water stockpile, water infrastructure.
                bool hasHousing   = population.Total < population.Cap;
                bool hasFood      = resources.Food >= FoodPerGrowth;
                bool hasWater     = resources.Water >= WaterPerGrowth;
                // Water infrastructure gate: population cannot exceed what the settlement's
                // wells and keep can supply. Canon: RESOURCE_SYSTEM.md, Water Crisis section.
                bool withinWaterCap = population.Total < waterCapRo.ValueRO.MaxSupportedByWater;

                if (!hasHousing || !hasFood || !hasWater || !withinWaterCap)
                {
                    continue;
                }

                population.GrowthAccumulator += dt;
                if (population.GrowthAccumulator < GrowthInterval)
                {
                    continue;
                }

                population.GrowthAccumulator = 0f;
                population.Total += 1;
                population.Available += 1;
                resources.Food -= FoodPerGrowth;
                resources.Water -= WaterPerGrowth;
            }
        }
    }
}
