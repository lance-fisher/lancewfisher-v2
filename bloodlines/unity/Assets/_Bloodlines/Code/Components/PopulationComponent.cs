using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction population pool. Canonical civilizational foundation (master doctrine
    /// section IV: population must be crucial, not decorative).
    /// Browser runtime equivalent: faction.population.
    ///
    /// Cap is the sum of base cap plus housing bonuses. Growth accrues over the canonical
    /// 18-second interval when food and water are both available. Total is the current
    /// population. Available is Total minus units currently alive (each unit consumes its
    /// populationCost slots from the pool).
    /// </summary>
    public struct PopulationComponent : IComponentData
    {
        public int Total;
        public int Available;
        public int Cap;
        public int BaseCap;
        public int CapBonus;
        public float GrowthAccumulator;
    }

    /// <summary>
    /// Per-faction resource stockpiles. Canonical six primary plus influence.
    /// Browser runtime equivalent: faction.resources.
    /// </summary>
    public struct ResourceStockpileComponent : IComponentData
    {
        public float Gold;
        public float Food;
        public float Water;
        public float Wood;
        public float Stone;
        public float Iron;
        public float Influence;
    }
}
