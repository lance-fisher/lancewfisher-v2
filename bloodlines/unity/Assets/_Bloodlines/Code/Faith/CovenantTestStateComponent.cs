using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    public enum CovenantTestPhase : byte
    {
        Inactive = 0,
        Qualifying = 1,
        ReadyToTrigger = 2,
        InProgress = 3,
        Complete = 4,
        Failed = 5,
    }

    public struct CovenantTestStateComponent : IComponentData
    {
        public float IntensityThresholdMetAtInWorldDays;
        public CovenantTestPhase TestPhase;
        public float TestStartedAtInWorldDays;
        public float LastFailedAtInWorldDays;
        public int SuccessCount;
    }

    public struct CovenantTestCostProfile
    {
        public float Food;
        public float Influence;
        public int Population;
        public float Legitimacy;
    }

    public static class CovenantTestRules
    {
        public const float IntensityThreshold = 80f;
        public const float DurationInWorldDays = 180f;
        public const float RetryCooldownInWorldDays = 120f;
        public const float FailureIntensityLoss = 20f;
        public const float FailureLegitimacyLoss = 8f;
        public const float SuccessIntensityFloor = 82f;
        public const float SuccessLegitimacyBonus = 8f;

        public static CovenantTestStateComponent CreateDefaultState()
        {
            return new CovenantTestStateComponent
            {
                IntensityThresholdMetAtInWorldDays = float.NaN,
                TestPhase = CovenantTestPhase.Inactive,
                TestStartedAtInWorldDays = float.NaN,
                LastFailedAtInWorldDays = float.NaN,
                SuccessCount = 0,
            };
        }

        public static CovenantTestCostProfile ResolveCostProfile(CovenantId covenantId, DoctrinePath doctrinePath)
        {
            if (covenantId == CovenantId.BloodDominion && doctrinePath == DoctrinePath.Light)
            {
                return new CovenantTestCostProfile
                {
                    Food = 45f,
                    Influence = 18f,
                    Population = 0,
                    Legitimacy = 0f,
                };
            }

            if (covenantId == CovenantId.BloodDominion && doctrinePath == DoctrinePath.Dark)
            {
                return new CovenantTestCostProfile
                {
                    Food = 0f,
                    Influence = 20f,
                    Population = 3,
                    Legitimacy = 6f,
                };
            }

            return default;
        }

        public static bool CanAfford(
            in CovenantTestCostProfile profile,
            in ResourceStockpileComponent resources,
            in PopulationComponent population)
        {
            return resources.Food >= profile.Food &&
                   resources.Influence >= profile.Influence &&
                   population.Total >= profile.Population;
        }

        public static void Spend(
            ref ResourceStockpileComponent resources,
            ref PopulationComponent population,
            ref DynastyStateComponent dynasty,
            in CovenantTestCostProfile profile)
        {
            resources.Food = math.max(0f, resources.Food - profile.Food);
            resources.Influence = math.max(0f, resources.Influence - profile.Influence);
            population.Total = math.max(0, population.Total - profile.Population);
            population.Available = math.max(0, population.Available - profile.Population);
            dynasty.Legitimacy = math.clamp(dynasty.Legitimacy - profile.Legitimacy, 0f, 100f);
        }
    }
}
