using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Marks a vessel as a fishing boat capable of auto-gathering food while idle
    /// over a water tile. Carries the canonical food-per-second yield from the
    /// unit definition's gatherRate field.
    ///
    /// Browser parity: simulation.js updateVessel (~8782-8801). When
    /// `unitDef.vesselClass === "fishing"` and the vessel has no command (idle)
    /// AND its tile is water, the faction food stockpile is incremented by
    /// `unitDef.gatherRate * dt`.
    ///
    /// Attached at spawn time by SkirmishBootstrapSystem to vessels whose
    /// VesselClass is Fishing. Removed only by destruction.
    /// </summary>
    public struct FishingVesselComponent : IComponentData
    {
        /// <summary>
        /// Food units per real second produced when fishing. Default 1.2 for the
        /// canonical fishing_boat unit; mirror of UnitDefinition.gatherRate.
        /// </summary>
        public float FoodPerSecond;
    }
}
