using Bloodlines.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Bloodlines.Raids
{
    /// <summary>
    /// Canonical scout raid and logistics interdiction values lifted from
    /// `simulation.js` and the current raid-authored entries in `data/*.json`.
    /// This stays code-native, matching the existing Siege/FieldWater canon
    /// helpers, so the slice can land without a broad asset-regeneration pass.
    /// </summary>
    public static class ScoutRaidCanon
    {
        public const float TargetRange = 24f;
        public const float RetreatDistance = 84f;
        public const float LoyaltyRadius = 240f;
        public const float StabilizedLoyaltyThreshold = 72f;
        public const float ConvoyRecoveryDurationSeconds = 12f;

        private static readonly FixedString64Bytes ScoutRiderId = new("scout_rider");
        private static readonly FixedString64Bytes CommandHallId = new("command_hall");
        private static readonly FixedString64Bytes FarmId = new("farm");
        private static readonly FixedString64Bytes WellId = new("well");
        private static readonly FixedString64Bytes LumberCampId = new("lumber_camp");
        private static readonly FixedString64Bytes MineWorksId = new("mine_works");
        private static readonly FixedString64Bytes QuarryId = new("quarry");
        private static readonly FixedString64Bytes IronMineId = new("iron_mine");
        private static readonly FixedString64Bytes SupplyCampId = new("supply_camp");
        private static readonly FixedString64Bytes SupplyWagonId = new("supply_wagon");
        private static readonly FixedString32Bytes GoldId = new("gold");
        private static readonly FixedString32Bytes WoodId = new("wood");
        private static readonly FixedString32Bytes StoneId = new("stone");
        private static readonly FixedString32Bytes IronId = new("iron");
        private static readonly FixedString32Bytes FoodId = new("food");
        private static readonly FixedString32Bytes WaterId = new("water");

        public static bool TryGetRaiderProfile(
            in UnitTypeComponent unitType,
            out ScoutRaiderProfile profile)
        {
            if (unitType.TypeId.Equals(ScoutRiderId))
            {
                profile = new ScoutRaiderProfile
                {
                    RaidDurationSeconds = 24f,
                    RaidLoyaltyDamage = 8f,
                };
                return true;
            }

            profile = default;
            return false;
        }

        public static bool TryGetBuildingRaidProfile(
            FixedString64Bytes buildingTypeId,
            out RaidImpactProfile profile)
        {
            if (buildingTypeId.Equals(FarmId))
            {
                profile = new RaidImpactProfile { Food = 12f, Influence = 2f };
                return true;
            }

            if (buildingTypeId.Equals(WellId))
            {
                profile = new RaidImpactProfile { Water = 12f, Influence = 2f };
                return true;
            }

            if (buildingTypeId.Equals(LumberCampId))
            {
                profile = new RaidImpactProfile { Wood = 14f, Influence = 2f };
                return true;
            }

            if (buildingTypeId.Equals(MineWorksId))
            {
                profile = new RaidImpactProfile { Gold = 10f, Influence = 2f };
                return true;
            }

            if (buildingTypeId.Equals(QuarryId))
            {
                profile = new RaidImpactProfile { Stone = 12f, Influence = 2f };
                return true;
            }

            if (buildingTypeId.Equals(IronMineId))
            {
                profile = new RaidImpactProfile { Iron = 10f, Wood = 4f, Influence = 3f };
                return true;
            }

            if (buildingTypeId.Equals(SupplyCampId))
            {
                profile = new RaidImpactProfile { Food = 12f, Water = 12f, Wood = 8f, Influence = 4f };
                return true;
            }

            profile = default;
            return false;
        }

        public static bool TryGetLogisticsCarrierProfile(
            FixedString64Bytes unitTypeId,
            out LogisticsInterdictionProfile profile)
        {
            if (unitTypeId.Equals(SupplyWagonId))
            {
                profile = new LogisticsInterdictionProfile
                {
                    InterdictionDurationSeconds = 18f,
                    ResourceLoss = new RaidImpactProfile
                    {
                        Food = 10f,
                        Water = 10f,
                        Wood = 8f,
                    },
                };
                return true;
            }

            profile = default;
            return false;
        }

        public static bool CanBuildingDropOff(
            FixedString64Bytes buildingTypeId,
            FixedString32Bytes resourceId)
        {
            if (resourceId.Length == 0)
            {
                return false;
            }

            if (buildingTypeId.Equals(CommandHallId))
            {
                return resourceId.Equals(GoldId) ||
                       resourceId.Equals(WoodId) ||
                       resourceId.Equals(StoneId) ||
                       resourceId.Equals(IronId);
            }

            if (buildingTypeId.Equals(LumberCampId))
            {
                return resourceId.Equals(WoodId);
            }

            if (buildingTypeId.Equals(MineWorksId))
            {
                return resourceId.Equals(GoldId);
            }

            if (buildingTypeId.Equals(QuarryId))
            {
                return resourceId.Equals(StoneId);
            }

            if (buildingTypeId.Equals(IronMineId))
            {
                return resourceId.Equals(IronId);
            }

            return false;
        }

        public static bool IsBuildingRaided(
            in BuildingRaidStateComponent raidState,
            double elapsed)
        {
            return raidState.RaidedUntil > elapsed;
        }

        public static RaidImpactProfile ApplyResourceLoss(
            ref ResourceStockpileComponent stockpile,
            in RaidImpactProfile loss)
        {
            var applied = new RaidImpactProfile
            {
                Gold = Drain(ref stockpile.Gold, loss.Gold),
                Food = Drain(ref stockpile.Food, loss.Food),
                Water = Drain(ref stockpile.Water, loss.Water),
                Wood = Drain(ref stockpile.Wood, loss.Wood),
                Stone = Drain(ref stockpile.Stone, loss.Stone),
                Iron = Drain(ref stockpile.Iron, loss.Iron),
                Influence = Drain(ref stockpile.Influence, loss.Influence),
            };
            return applied;
        }

        public static float ResolveInterdictionDuration(
            in ScoutRaiderProfile raider,
            in LogisticsInterdictionProfile target)
        {
            return math.max(
                10f,
                target.InterdictionDurationSeconds > 0f
                    ? target.InterdictionDurationSeconds
                    : math.max(12f, raider.RaidDurationSeconds - 6f));
        }

        private static float Drain(ref float stockpile, float requested)
        {
            if (requested <= 0f)
            {
                return 0f;
            }

            float drained = math.min(stockpile, requested);
            stockpile = math.max(0f, stockpile - requested);
            return math.round(drained * 10f) / 10f;
        }
    }

    public struct ScoutRaiderProfile
    {
        public float RaidDurationSeconds;
        public float RaidLoyaltyDamage;
    }

    public struct RaidImpactProfile
    {
        public float Gold;
        public float Food;
        public float Water;
        public float Wood;
        public float Stone;
        public float Iron;
        public float Influence;

        public bool HasAnyLoss()
        {
            return Gold > 0f ||
                   Food > 0f ||
                   Water > 0f ||
                   Wood > 0f ||
                   Stone > 0f ||
                   Iron > 0f ||
                   Influence > 0f;
        }
    }

    public struct LogisticsInterdictionProfile
    {
        public float InterdictionDurationSeconds;
        public RaidImpactProfile ResourceLoss;
    }
}
