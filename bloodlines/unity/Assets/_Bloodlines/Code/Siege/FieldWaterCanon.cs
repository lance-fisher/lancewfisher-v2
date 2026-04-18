using Bloodlines.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Direct port of the browser-side field-water sustainment constants plus the
    /// current canonical support-source values already authored in data/*.json.
    /// </summary>
    public static class FieldWaterCanon
    {
        public const float FieldWaterLocalSupportRadius = 132f;
        public const float FieldWaterSettlementSupportRadius = 156f;
        public const float FieldWaterSupportDurationSeconds = 14f;
        public const float FieldWaterTransferIntervalSeconds = 4f;
        public const float FieldWaterTransferCost = 0.2f;
        public const float FieldWaterStrainPerSecond = 0.85f;
        public const float FieldWaterRecoveryPerSecond = 1.25f;
        public const float FieldWaterStrainThreshold = 6f;
        public const float FieldWaterCriticalThreshold = 12f;
        public const float FieldWaterStrainAttackMultiplier = 0.88f;
        public const float FieldWaterStrainSpeedMultiplier = 0.9f;
        public const float FieldWaterCriticalAttackMultiplier = 0.72f;
        public const float FieldWaterCriticalSpeedMultiplier = 0.78f;
        public const float FieldWaterMessageIntervalSeconds = 18f;
        public const float FieldWaterAttritionThresholdSeconds = 4f;
        public const float FieldWaterDesertionThresholdSeconds = 10f;
        public const float FieldWaterAttritionDamagePerSecond = 6f;
        public const float FieldWaterCriticalRecoveryPerSecond = 2.1f;
        public const float FieldWaterCommanderDisciplineBufferSeconds = 4f;
        public const float FieldWaterCommanderAttritionMultiplier = 0.6f;
        public const float FieldWaterDesertionHealthRatio = 0.45f;

        public const float CommanderBaseAuraRadius = 126f;
        public const float WellSupportRadius = 180f;
        public const float WellSupportDurationSeconds = 16f;
        public const float SupplyCampSupportRadius = 160f;
        public const float SupplyCampSupportDurationSeconds = 14f;

        private static readonly FixedString64Bytes FishingBoatId = new("fishing_boat");
        private static readonly FixedString64Bytes ScoutVesselId = new("scout_vessel");
        private static readonly FixedString64Bytes TransportShipId = new("transport_ship");
        private static readonly FixedString64Bytes FireShipId = new("fire_ship");
        private static readonly FixedString64Bytes CapitalShipId = new("capital_ship");
        private static readonly FixedString64Bytes WarGalleyId = new("war_galley");
        private static readonly FixedString64Bytes WellBuildingId = new("well");
        private static readonly FixedString64Bytes SupplyCampBuildingId = new("supply_camp");
        private static readonly FixedString64Bytes SupplyWagonId = new("supply_wagon");

        public static bool ShouldTrackFieldWater(in UnitTypeComponent unitType)
        {
            if (unitType.Role == UnitRole.Worker)
            {
                return false;
            }

            return !IsWaterVessel(unitType.TypeId);
        }

        public static bool TryGetBuildingSupportProfile(
            FixedString64Bytes buildingTypeId,
            out float supportRadius,
            out float supportDurationSeconds)
        {
            supportRadius = 0f;
            supportDurationSeconds = 0f;

            if (buildingTypeId.Equals(WellBuildingId))
            {
                supportRadius = WellSupportRadius;
                supportDurationSeconds = WellSupportDurationSeconds;
                return true;
            }

            if (buildingTypeId.Equals(SupplyCampBuildingId))
            {
                supportRadius = SupplyCampSupportRadius;
                supportDurationSeconds = SupplyCampSupportDurationSeconds;
                return true;
            }

            return false;
        }

        public static bool TryGetSupplyWagonSupportProfile(
            FixedString64Bytes unitTypeId,
            out float supportRadius,
            out float supportDurationSeconds)
        {
            supportRadius = 0f;
            supportDurationSeconds = 0f;

            if (!unitTypeId.Equals(SupplyWagonId))
            {
                return false;
            }

            supportRadius = SiegeSupportCanon.SupplyWagonRadius;
            supportDurationSeconds = SiegeSupportCanon.SupplyDurationSeconds;
            return true;
        }

        public static float ResolveCommanderAuraRadius(float renown)
        {
            return CommanderBaseAuraRadius + math.max(0f, renown);
        }

        public static float ResolveAttackMultiplier(float strain)
        {
            if (strain >= FieldWaterCriticalThreshold)
            {
                return FieldWaterCriticalAttackMultiplier;
            }

            if (strain >= FieldWaterStrainThreshold)
            {
                return FieldWaterStrainAttackMultiplier;
            }

            return 1f;
        }

        public static float ResolveSpeedMultiplier(float strain)
        {
            if (strain >= FieldWaterCriticalThreshold)
            {
                return FieldWaterCriticalSpeedMultiplier;
            }

            if (strain >= FieldWaterStrainThreshold)
            {
                return FieldWaterStrainSpeedMultiplier;
            }

            return 1f;
        }

        private static bool IsWaterVessel(FixedString64Bytes typeId)
        {
            return typeId.Equals(FishingBoatId) ||
                typeId.Equals(ScoutVesselId) ||
                typeId.Equals(TransportShipId) ||
                typeId.Equals(FireShipId) ||
                typeId.Equals(CapitalShipId) ||
                typeId.Equals(WarGalleyId);
        }
    }
}
