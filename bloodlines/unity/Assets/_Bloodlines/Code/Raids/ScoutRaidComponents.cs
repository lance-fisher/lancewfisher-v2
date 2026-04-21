using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Raids
{
    public enum ScoutRaidTargetKind : byte
    {
        Building = 0,
        LogisticsCarrier = 1,
    }

    /// <summary>
    /// Explicit raid order for a scout-capable unit.
    /// The command is consumed by ScoutRaidResolutionSystem once the raider
    /// closes to the canonical raid range.
    /// </summary>
    public struct ScoutRaidCommandComponent : IComponentData
    {
        public Entity TargetEntity;
        public ScoutRaidTargetKind TargetKind;
    }

    /// <summary>
    /// Runtime raid timer for a building.
    /// Mirrors the browser building.raidedUntil state used to suppress
    /// trickle, field-water support, and siege-logistics availability.
    /// </summary>
    public struct BuildingRaidStateComponent : IComponentData
    {
        public double RaidedUntil;
        public double LastRaidedAt;
        public FixedString32Bytes RaidedByFactionId;
    }
}
