using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-building resource trickle rates in units per second.
    ///
    /// Browser runtime equivalent: buildingDefinition.resourceTrickle. Each completed,
    /// alive, non-raided building adds `amount * dt` to the owning faction's
    /// ResourceStockpileComponent for every resource in the set. Canonical examples:
    ///
    ///   farm      -> 0.5 food/sec
    ///   well      -> 0.45 water/sec
    ///   granary   -> 0.2 food/sec (per doctrine, not yet in data)
    ///
    /// Components under construction must NOT trickle. Components raided must NOT
    /// trickle. The system enforces those gates.
    /// </summary>
    public struct ResourceTrickleBuildingComponent : IComponentData
    {
        public float GoldPerSecond;
        public float FoodPerSecond;
        public float WaterPerSecond;
        public float WoodPerSecond;
        public float StonePerSecond;
        public float IronPerSecond;
        public float InfluencePerSecond;
    }
}
