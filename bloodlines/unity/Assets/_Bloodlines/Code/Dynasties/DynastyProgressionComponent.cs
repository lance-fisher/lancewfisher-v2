using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Per-faction cross-match dynasty progression state.
    /// Browser equivalent: absent -- implemented from canonical cross-match progression
    /// design in governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md.
    ///
    /// AccumulatedXP accrues across matches. At each tier threshold the system fires a
    /// TierUnlockNotificationComponent which DynastyProgressionUnlockSystem consumes to
    /// populate the faction's DynastyUnlockSlotElement buffer. Bonuses are sideways
    /// customization only -- no raw power advantage. CurrentTier caps at
    /// DynastyProgressionCanon.MaxTier.
    /// </summary>
    public struct DynastyProgressionComponent : IComponentData
    {
        /// Total XP accumulated across all completed matches.
        public float AccumulatedXP;

        /// Current tier (0-4). Advances at canonical XP thresholds.
        public byte CurrentTier;

        /// XP awarded in the most recently completed match.
        public float LastMatchXPAward;

        /// Number of tier-unlock notifications pending consumption.
        public byte TierUnlocksPending;
    }

    /// <summary>
    /// One-shot per-faction event: a new tier was reached and unlock slots should be added.
    /// Consumed and removed by DynastyProgressionUnlockSystem.
    /// </summary>
    public struct TierUnlockNotificationComponent : IComponentData
    {
        /// The tier that was just reached.
        public byte NewTier;
    }

    /// <summary>
    /// One-shot XP award request placed on a faction entity after a match.
    /// Consumed by DynastyXPAwardSystem.
    /// </summary>
    public struct DynastyXPAwardRequestComponent : IComponentData
    {
        /// XP to award this match.
        public float XPAmount;

        /// Final match placement (1-based). Used for telemetry only.
        public byte MatchPlacement;
    }
}
