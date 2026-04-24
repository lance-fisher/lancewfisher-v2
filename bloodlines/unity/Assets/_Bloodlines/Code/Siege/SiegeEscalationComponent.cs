using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Per-settlement escalation state materialized while a siege is active.
    /// Browser equivalent: absent (tickSiegeEscalation not present in simulation.js --
    /// implemented from canonical siege doctrine).
    ///
    /// Attached lazily by SiegeEscalationSystem when FortificationReserveComponent.ThreatActive
    /// becomes true. Removed when the threat clears. Duration is cumulative across siege
    /// re-activations within the same match.
    /// </summary>
    public struct SiegeEscalationComponent : IComponentData
    {
        /// Cumulative in-world days this settlement has been under active siege threat.
        public float SiegeDurationInWorldDays;

        /// 0=Normal, 1=Prolonged, 2=Severe, 3=Critical. See SiegeEscalationTier.
        public byte EscalationTier;

        /// Starvation population-decline multiplier for this settlement's owning faction.
        public float StarvationMultiplier;

        /// Fraction of garrison HP below which a unit carries a desertion risk each day.
        public float DesertionThresholdPct;

        /// Loyalty penalty per in-world day applied to the garrison faction.
        public float MoralePenaltyPerDay;

        /// InWorldDays value when duration was last incremented (per-frame accumulation gate).
        public float LastTickInWorldDays;

        /// FactionId of the settlement owner at the time of last tick.
        public FixedString32Bytes OwnerFactionId;
    }

    /// <summary>
    /// Per-faction aggregate of siege-escalation starvation pressure.
    /// Written each frame by SiegeEscalationSystem from the worst-tier besieged
    /// settlement owned by this faction. Read by StarvationResponseSystem.
    /// </summary>
    public struct FactionSiegeEscalationStateComponent : IComponentData
    {
        /// Max starvation multiplier across all besieged settlements for this faction.
        /// 1.0 = no escalation (Normal tier or no active siege).
        public float StarvationMultiplier;
    }

    public enum SiegeEscalationTier : byte
    {
        Normal = 0,
        Prolonged = 1,
        Severe = 2,
        Critical = 3,
    }
}
