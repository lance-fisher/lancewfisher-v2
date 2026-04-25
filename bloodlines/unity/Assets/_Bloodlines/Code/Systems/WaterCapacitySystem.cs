using Bloodlines.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Sums water infrastructure support across all live Wells per faction and
    /// writes the total to each faction's WaterCapacityComponent.
    ///
    /// Model:
    ///   Each Well building has a fixed waterPopulationSupport value (from
    ///   buildings.json, loaded into BuildingTypeComponent.WaterPopulationSupport
    ///   or read from the Well type directly). The Keep provides a small base
    ///   reserve (EarlyGameConstants.KeepBaseWaterSupport).
    ///
    ///   MaxSupportedByWater is consumed by:
    ///     - PopulationGrowthSystem (hard gate: no growth if Total >= MaxSupportedByWater)
    ///     - PopulationProductivitySystem (deficit penalty if Total > MaxSupportedByWater)
    ///
    /// Raided buildings (BuildingRaidStateComponent) do NOT contribute.
    /// Buildings under construction do NOT contribute.
    ///
    /// Canon: RESOURCE_SYSTEM.md "The Food-Water-Population Triangle".
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PopulationProductivitySystem))]
    public partial struct WaterCapacitySystem : ISystem
    {
        static readonly FixedString64Bytes WellTypeId = new("well");

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Accumulate water support per faction. Key = factionId hash.
            using var waterMap = new NativeHashMap<int, int>(16, Allocator.Temp);

            foreach (var (faction, buildingType) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<BuildingTypeComponent>>())
            {
                if (buildingType.ValueRO.TypeId != WellTypeId)
                    continue;

                int key = faction.ValueRO.FactionId.GetHashCode();
                waterMap.TryGetValue(key, out int current);
                waterMap.Remove(key);
                waterMap.Add(key, current + EarlyGameConstants.WellPopulationSupport);
            }

            // Write results to faction entities. Start from KeepBaseWaterSupport
            // so the player has a small buffer before building a Well.
            foreach (var (waterCapRw, faction) in
                SystemAPI.Query<RefRW<WaterCapacityComponent>, RefRO<FactionComponent>>())
            {
                int key = faction.ValueRO.FactionId.GetHashCode();
                waterMap.TryGetValue(key, out int wellSupport);
                waterCapRw.ValueRW.MaxSupportedByWater =
                    EarlyGameConstants.KeepBaseWaterSupport + wellSupport;
            }
        }
    }
}
