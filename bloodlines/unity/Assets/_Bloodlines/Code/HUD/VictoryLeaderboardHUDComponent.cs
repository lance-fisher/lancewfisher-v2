using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton tag and refresh state for the ordered HUD victory leaderboard.
    /// The actual rows live in the buffer on the same entity.
    /// </summary>
    public struct VictoryLeaderboardHUDSingleton : IComponentData
    {
        public float LastRefreshInWorldDays;
    }

    /// <summary>
    /// One ordered leaderboard entry per faction, sorted by highest victory
    /// proximity first.
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct VictoryLeaderboardHUDComponent : IBufferElementData
    {
        public FixedString32Bytes FactionId;
        public FixedString32Bytes LeadingConditionId;
        public float ProgressPct;
        public bool IsHumanPlayer;
    }
}
