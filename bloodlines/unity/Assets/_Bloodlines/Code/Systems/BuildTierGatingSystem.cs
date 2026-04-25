using Bloodlines.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Evaluates per-faction build tier every frame and updates BuildTierComponent.
    ///
    /// Tier transitions:
    ///   0 → 1  when FoundingRetinueComponent.IsDeployed becomes true
    ///   1 → 2  when tier-1 prerequisites exist: housing, water, food source, training yard
    ///
    /// Building availability per tier (matches buildings.json buildTier field):
    ///   Tier 0  – nothing (Deploy action only)
    ///   Tier 1  – housing, well, woodcutter_camp, forager_camp, training_yard
    ///   Tier 2  – small_farm and all tier-1 buildings remain available
    ///   Tier 3+ – full build tree (future sessions)
    ///
    /// Canon: early-game-foundation prompt 2026-04-25. No MVP reduction.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BuildTierGatingSystem : ISystem
    {
        // TypeId strings that count toward prerequisite detection.
        // Must match the id fields in buildings.json.
        static readonly FixedString64Bytes HousingId        = new("housing");
        static readonly FixedString64Bytes DwellingId       = new("dwelling");
        static readonly FixedString64Bytes WellId           = new("well");
        static readonly FixedString64Bytes ForagerCampId    = new("forager_camp");
        static readonly FixedString64Bytes FarmId           = new("farm");
        static readonly FixedString64Bytes SmallFarmId      = new("small_farm");
        static readonly FixedString64Bytes TrainingYardId   = new("training_yard");

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Collect building presence per faction in a NativeHashMap.
            // Key = factionId string hash, Value = prerequisite bitmask.
            using var prereqMap = new NativeHashMap<int, byte>(16, Allocator.Temp);

            const byte HasHousingBit     = 1 << 0;
            const byte HasWaterBit       = 1 << 1;
            const byte HasFoodBit        = 1 << 2;
            const byte HasTrainingYardBit = 1 << 3;

            foreach (var (faction, buildingType) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<BuildingTypeComponent>>())
            {
                int key = faction.ValueRO.FactionId.GetHashCode();
                prereqMap.TryGetValue(key, out byte bits);

                var typeId = buildingType.ValueRO.TypeId;

                if (typeId == HousingId || typeId == DwellingId)
                    bits |= HasHousingBit;
                if (typeId == WellId)
                    bits |= HasWaterBit;
                if (typeId == ForagerCampId || typeId == FarmId || typeId == SmallFarmId)
                    bits |= HasFoodBit;
                if (typeId == TrainingYardId)
                    bits |= HasTrainingYardBit;

                prereqMap.Remove(key);
                prereqMap.Add(key, bits);
            }

            // Update BuildTierComponent on each faction entity.
            foreach (var (tierRw, retinueRo, faction) in
                SystemAPI.Query<
                    RefRW<BuildTierComponent>,
                    RefRO<FoundingRetinueComponent>,
                    RefRO<FactionComponent>>())
            {
                ref var tier = ref tierRw.ValueRW;

                if (!retinueRo.ValueRO.IsDeployed)
                {
                    tier.CurrentTier = 0;
                    tier.HasHousing     = false;
                    tier.HasWater       = false;
                    tier.HasFoodSource  = false;
                    tier.HasTrainingYard = false;
                    continue;
                }

                // Deployed: at least tier 1.
                int key = faction.ValueRO.FactionId.GetHashCode();
                prereqMap.TryGetValue(key, out byte bits);

                tier.HasHousing      = (bits & HasHousingBit)      != 0;
                tier.HasWater        = (bits & HasWaterBit)         != 0;
                tier.HasFoodSource   = (bits & HasFoodBit)          != 0;
                tier.HasTrainingYard = (bits & HasTrainingYardBit)  != 0;

                bool tier2Prerequisites = tier.HasHousing
                                       && tier.HasWater
                                       && tier.HasFoodSource
                                       && tier.HasTrainingYard;

                tier.CurrentTier = (byte)(tier2Prerequisites ? 2 : 1);
            }
        }
    }
}
