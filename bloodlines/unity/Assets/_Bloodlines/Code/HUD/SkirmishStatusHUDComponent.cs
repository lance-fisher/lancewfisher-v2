using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton header for the skirmish match status panel. Holds match-wide
    /// summary data. Per-faction rows live in a SkirmishStatusFactionRowHUDComponent
    /// buffer on the same entity, sorted by territory share descending.
    ///
    /// Browser equivalent: absent -- skirmish match status HUD not in simulation.js.
    /// </summary>
    public struct SkirmishStatusHUDComponent : IComponentData
    {
        public float InWorldDays;
        public int ActiveFactionCount;
        public int TotalUnitCount;
        public int TotalTerritoryCount;
        public float LastRefreshInWorldDays;
    }

    /// <summary>
    /// One ordered row per faction in the skirmish match status panel.
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct SkirmishStatusFactionRowHUDComponent : IBufferElementData
    {
        public FixedString32Bytes FactionId;
        public int Rank;
        public int UnitCount;
        public int TerritoryCount;
        public float TerritoryShare;
        public float Gold;
        public int TradeRouteCount;
        public float TradeGoldPerDay;
        public bool IsHumanPlayer;
    }
}
