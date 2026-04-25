using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Marks a building as a smelting drop-off. When a worker deposits the
    /// product resource at this building, the smelting fuel resource is
    /// consumed at the configured ratio (carried amount * FuelRatio).
    /// If the faction stockpile lacks enough fuel, the carried ore is
    /// returned to its source node and the deposit stalls until fuel is
    /// available.
    ///
    /// Browser parity: src/game/core/simulation.js worker deposit branch
    /// (~8454-8481). The browser reads `depositDef.smeltingFuelResource`
    /// and `depositDef.smeltingFuelRatio`; canonically iron_mine carries
    /// fuel="wood" ratio=0.5.
    ///
    /// Bloodlines.DataDefinitions.BuildingDefinition.smeltingFuelResource
    /// and smeltingFuelRatio are the JSON-imported authoring fields.
    /// MapBuildingSeedElement carries them through the baker. The
    /// SkirmishBootstrap spawn path attaches this component to any
    /// completed building seed whose SmeltingFuelRatio &gt; 0.
    /// </summary>
    public struct SmeltingComponent : IComponentData
    {
        public FixedString32Bytes FuelResourceId;
        public float FuelRatio;
    }
}
