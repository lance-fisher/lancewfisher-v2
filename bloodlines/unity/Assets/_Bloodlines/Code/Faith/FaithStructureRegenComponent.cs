using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    public struct FaithStructureRegenComponent : IComponentData
    {
        public float LastAppliedInWorldDays;
        public float CurrentRatePerSecond;
        public float LastAppliedIntensityDelta;
        public int FaithBuildingCount;
        public byte Initialized;
    }

    public static class FaithStructureRegenRules
    {
        public const float MaxRatePerSecond = 1.4f;

        private const float WayshrineRatePerSecond = 0.18f;
        private const float CovenantHallRatePerSecond = 0.32f;
        private const float GrandSanctuaryRatePerSecond = 0.48f;
        private const float ApexCovenantRatePerSecond = 0.72f;

        private static readonly FixedString64Bytes WayshrineTypeId = new("wayshrine");
        private static readonly FixedString64Bytes CovenantHallTypeId = new("covenant_hall");
        private static readonly FixedString64Bytes GrandSanctuaryTypeId = new("grand_sanctuary");
        private static readonly FixedString64Bytes ApexCovenantTypeId = new("apex_covenant");

        public static bool TryResolveRatePerSecond(
            in BuildingTypeComponent buildingType,
            out float ratePerSecond)
        {
            if (buildingType.TypeId.Equals(WayshrineTypeId))
            {
                ratePerSecond = WayshrineRatePerSecond;
                return true;
            }

            if (buildingType.TypeId.Equals(CovenantHallTypeId))
            {
                ratePerSecond = CovenantHallRatePerSecond;
                return true;
            }

            if (buildingType.TypeId.Equals(GrandSanctuaryTypeId))
            {
                ratePerSecond = GrandSanctuaryRatePerSecond;
                return true;
            }

            if (buildingType.TypeId.Equals(ApexCovenantTypeId))
            {
                ratePerSecond = ApexCovenantRatePerSecond;
                return true;
            }

            ratePerSecond = 0f;
            return false;
        }

        public static float ResolveIntensityGain(
            float ratePerSecond,
            int elapsedInWorldDays,
            float daysPerRealSecond)
        {
            if (ratePerSecond <= 0f || elapsedInWorldDays <= 0)
            {
                return 0f;
            }

            float normalizedDaysPerRealSecond = math.max(daysPerRealSecond, 0.0001f);
            return ratePerSecond * elapsedInWorldDays / normalizedDaysPerRealSecond;
        }
    }
}
