using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Singleton per faction carrying the canonical 90-second realm cycle accumulator
    /// and strain streaks that trigger famine, water crisis, and cap pressure events.
    /// Browser runtime equivalent: state.realmCycleAccumulator, faction.foodStrainStreak,
    /// faction.waterStrainStreak, etc.
    ///
    /// Snapshot systems read from this component plus per-block components (population,
    /// food/water resource counts, fortification tier from primary keep, etc.) to export
    /// the canonical 11-state realm dashboard (Cycle, Pop, Food, Water, Loyalty, Fort,
    /// Army, Faith, Conviction, Logistics, World).
    /// </summary>
    public struct RealmConditionComponent : IComponentData
    {
        public float CycleAccumulator;
        public int CycleCount;
        public int FoodStrainStreak;
        public int WaterStrainStreak;
        public float AssaultFailureStrain;
        public double CohesionPenaltyUntil;
        public int LastStarvationResponseCycle;
    }

    /// <summary>
    /// Singleton holding the canonical cycle length and thresholds loaded from
    /// data/realm-conditions.json. Default cycle length is 90 seconds (canonical).
    /// </summary>
    public struct RealmCycleConfig : IComponentData
    {
        public float CycleSeconds;
        public float FoodGreenRatio;
        public float FoodYellowRatio;
        public float WaterGreenRatio;
        public float WaterYellowRatio;
        public float LoyaltyGreenFloor;
        public float LoyaltyYellowFloor;
        public float PopulationGreenCapRatio;
        public float PopulationYellowCapRatio;
        public int FoodFamineConsecutiveCycles;
        public int WaterCrisisConsecutiveCycles;
        public int FaminePopulationDeclinePerCycle;
        public int WaterCrisisOutmigrationPerCycle;
        public int FamineLoyaltyDeltaPerCycle;
        public int WaterCrisisLoyaltyDeltaPerCycle;
    }
}
