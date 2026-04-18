using Bloodlines.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Direct port of the browser-side siege logistics constants and the current
    /// canonical engineer / supply-wagon support values from data/units.json.
    /// </summary>
    public static class SiegeSupportCanon
    {
        public const float SiegeSupportRefreshSeconds = 1.25f;
        public const float SiegeUnsuppliedAttackMultiplier = 0.84f;
        public const float SiegeUnsuppliedSpeedMultiplier = 0.88f;
        public const float EngineerSupportRadius = 96f;
        public const float EngineerRepairPerSecond = 5.5f;
        public const float SupplyWagonRadius = 128f;
        public const float SupplyCampLinkRadius = 360f;
        public const float SupplyDurationSeconds = 10f;
        public const float SupplyTransferSeconds = 3f;
        public const float SupplyTransferFoodCost = 1f;
        public const float SupplyTransferWaterCost = 1f;
        public const float SupplyTransferWoodCost = 1f;

        private static readonly FixedString64Bytes SiegeEngineerId = new("siege_engineer");
        private static readonly FixedString64Bytes SupplyWagonId = new("supply_wagon");

        public static bool ShouldTrackSupport(in UnitTypeComponent unitType)
        {
            return IsSiegeEngine(unitType) || IsSiegeEngineer(unitType) || IsSupplyWagon(unitType);
        }

        public static bool IsSiegeEngine(in UnitTypeComponent unitType)
        {
            return unitType.Role == UnitRole.SiegeEngine;
        }

        public static bool IsSiegeEngineer(in UnitTypeComponent unitType)
        {
            return unitType.Role == UnitRole.EngineerSpecialist ||
                unitType.TypeId.Equals(SiegeEngineerId);
        }

        public static bool IsSupplyWagon(in UnitTypeComponent unitType)
        {
            return unitType.TypeId.Equals(SupplyWagonId);
        }

        public static float ResolveSupplyTransferScale(int nearbyEngineCount)
        {
            return math.clamp(nearbyEngineCount, 1, 2);
        }

        public static float ResolveAttackMultiplier(bool hasSupplyTrainSupport)
        {
            return hasSupplyTrainSupport ? 1f : SiegeUnsuppliedAttackMultiplier;
        }

        public static float ResolveSpeedMultiplier(bool hasSupplyTrainSupport)
        {
            return hasSupplyTrainSupport ? 1f : SiegeUnsuppliedSpeedMultiplier;
        }
    }
}
