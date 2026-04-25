using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction component carrying the canonical house-mechanics multipliers
    /// resolved from data/houses.json mechanics block. Attached to a faction entity
    /// at bootstrap time alongside FactionHouseComponent.
    ///
    /// Browser parity: data/houses.json `[id].mechanics`. The browser reads these
    /// at the call sites where house-specific behavior fires (faction.id-based
    /// branches in startBuildingConstruction / advanceFortificationTier). Unity
    /// caches them on the faction entity so consumers do not need to re-resolve
    /// from the HouseDefinition table every tick.
    ///
    /// Defaults: 1.0 for both fields when no mechanics block exists.
    ///   - Stonehelm: fortificationCostMultiplier=0.8, fortificationBuildSpeedMultiplier=1.2.
    ///   - Other houses: 1.0 / 1.0 (canonical neutral).
    /// </summary>
    public struct HouseMechanicsComponent : IComponentData
    {
        public float FortificationCostMultiplier;
        public float FortificationBuildSpeedMultiplier;
    }
}
