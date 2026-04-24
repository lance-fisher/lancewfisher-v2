using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton written by MatchEndHUDSystem when MatchEndStateComponent exists.
    /// Drives the match result screen: winner identity, victory type, reason text,
    /// match duration, and whether the player received XP this match.
    ///
    /// Browser equivalent: absent -- match-end result screen not in simulation.js.
    /// </summary>
    public struct MatchEndHUDComponent : IComponentData
    {
        public bool IsVisible;
        public FixedString32Bytes WinnerFactionId;
        public Victory.VictoryConditionId VictoryType;
        public FixedString128Bytes VictoryReason;
        public float MatchEndTimeInWorldDays;
        public bool PlayerXPAwarded;
    }
}
