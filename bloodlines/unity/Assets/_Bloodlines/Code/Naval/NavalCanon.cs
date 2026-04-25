namespace Bloodlines.Naval
{
    /// <summary>
    /// Canonical naval-layer constants pulled from the design bible and the
    /// browser runtime. Every naval system reads tuning from this single point
    /// so that future canon changes land in one file.
    ///
    /// Browser runtime references (src/game/core/simulation.js):
    ///   - embark adjacency: `state.world.tileSize * 2.5` (line 7558)
    ///   - transport capacity: data/units.json transport_ship.transportCapacity (6)
    ///   - fire ship sacrifice: data/units.json fire_ship.oneUseSacrifice (true)
    /// </summary>
    public static class NavalCanon
    {
        /// <summary>
        /// Maximum embark distance measured in tile-widths. A passenger
        /// candidate must be within `EmbarkRadiusTileMultiplier * tileSize`
        /// of the target transport to embark. Mirrors the browser runtime
        /// guard at simulation.js line 7558.
        /// </summary>
        public const float EmbarkRadiusTileMultiplier = 2.5f;

        /// <summary>
        /// Default transport capacity used when a vessel is constructed without
        /// a definition-driven capacity (debug/test builds). Production spawns
        /// always read capacity from the unit definition.
        /// </summary>
        public const int DefaultTransportCapacity = 6;

        /// <summary>
        /// Default tile size assumed by validation worlds that do not bake a
        /// MapDefinition. Production worlds read tile size from MapBootstrap.
        /// </summary>
        public const float ValidationDefaultTileSize = 1f;
    }
}
