using Bloodlines.Components;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Canonical convoy and supply-line interdiction constants pulled from the
    /// browser logistics lane. Camp stockpile values are additive Unity runtime
    /// observability used to gate support-anchor availability.
    /// </summary>
    public static class SiegeSupplyInterdictionCanon
    {
        public const float WagonInterdictionDurationSeconds = 12f;
        public const float ConvoyRecoveryDurationSeconds = 12f;
        public const float ConvoyEscortScreenRadius = 86f;
        public const int ConvoyEscortMinEscorts = 2;
        public const float SupplyLineRaidRadius = SiegeSupportCanon.SupplyCampLinkRadius;
        public const float SupplyCampMaxStockpile = 100f;
        public const float SupplyCampOperationalThreshold = 25f;
        public const float SupplyCampRaidDrainPerSecond = 18f;
        public const float SupplyCampRecoveryPerSecond = 6f;
        public const float InterdictionFoodLoss = 8f;
        public const float InterdictionWaterLoss = 8f;
        public const float InterdictionWoodLoss = 6f;

        public static bool IsRaider(in UnitTypeComponent unitType)
        {
            return unitType.Role == UnitRole.LightCavalry ||
                   unitType.Role == UnitRole.MeleeRecon;
        }

        public static bool IsEscortEligible(in UnitTypeComponent unitType)
        {
            return unitType.Role == UnitRole.Melee ||
                   unitType.Role == UnitRole.MeleeRecon ||
                   unitType.Role == UnitRole.Ranged ||
                   unitType.Role == UnitRole.UniqueMelee ||
                   unitType.Role == UnitRole.LightCavalry;
        }

        public static bool IsCampOperational(in SiegeSupplyCampComponent camp)
        {
            return camp.NearbyRaiderCount <= 0 &&
                   camp.Stockpile >= camp.OperationalThreshold;
        }
    }
}
