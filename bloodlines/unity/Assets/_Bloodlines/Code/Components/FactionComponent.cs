using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Faction ownership for units, buildings, control points, and settlements.
    /// Canonical browser runtime equivalent: unit.factionId / building.factionId strings
    /// ("player", "enemy", "tribes", "neutral").
    /// Uses FixedString32Bytes so it can be read inside Burst-compiled systems without GC.
    /// </summary>
    public struct FactionComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
    }

    /// <summary>
    /// Canonical faction kinds. Matches browser runtime faction.kind:
    ///   kingdom: player or AI-controlled dynasty house
    ///   tribes:  neutral raiders (canonical frontier pressure)
    ///   neutral: trade hub, Trueborn neutral city, and other non-faction seats
    /// </summary>
    public enum FactionKind : byte
    {
        Kingdom = 0,
        Tribes = 1,
        Neutral = 2,
    }

    /// <summary>
    /// Additional faction metadata that the renderer, AI, and snapshot systems read.
    /// Split from FactionComponent so systems that only need the id do not pull kind data.
    /// </summary>
    public struct FactionKindComponent : IComponentData
    {
        public FactionKind Kind;
    }
}
