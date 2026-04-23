using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Active succession-crisis state on a faction entity. Mirrors the browser's
    /// dynastic political-event surface but keeps the first Unity slice scoped to
    /// the load-bearing runtime effects: loyalty shock, legitimacy drain, and
    /// resource trickle disruption.
    /// </summary>
    public struct SuccessionCrisisComponent : IComponentData
    {
        public byte CrisisSeverity;
        public float CrisisStartedAtInWorldDays;
        public float RecoveryProgressPct;
        public float ResourceTrickleFactor;
        public bool LoyaltyShockApplied;
        public float LegitimacyDrainRatePerDay;

        // Internal fields used to preserve deterministic whole-day ticking.
        public float LoyaltyDrainRatePerDay;
        public float LastDailyTickInWorldDays;
        public float RecoveryRatePerDay;
        public FixedString64Bytes FallenMemberId;
        public FixedString64Bytes CurrentRulerMemberId;
    }

    /// <summary>
    /// Persistent watcher that records the last known ruler so a post-succession
    /// system can detect ruler changes without reopening the core dynasty state
    /// shape.
    /// </summary>
    public struct SuccessionCrisisWatchComponent : IComponentData
    {
        public FixedString64Bytes LastKnownRulerMemberId;
        public byte Initialized;
    }

    public enum SuccessionCrisisSeverity : byte
    {
        None = 0,
        Minor = 1,
        Moderate = 2,
        Major = 3,
        Catastrophic = 4,
    }
}
