using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton refresh marker for the ordered dynasty renown leaderboard.
    /// The actual rows live in a buffer on the same entity.
    /// </summary>
    public struct DynastyRenownLeaderboardHUDSingleton : IComponentData
    {
        public float LastRefreshInWorldDays;
    }

    /// <summary>
    /// One ordered dynasty-renown panel row per faction, sorted by strongest
    /// prestige pressure first.
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct DynastyRenownLeaderboardHUDComponent : IBufferElementData
    {
        public FixedString32Bytes FactionId;
        public int RenownRank;
        public float RenownScore;
        public float PeakRenown;
        public bool IsHumanPlayer;
        public bool IsLeadingDynasty;
        public bool Interregnum;
        public FixedString64Bytes RulerMemberId;
        public FixedString64Bytes RulerTitle;
        public FixedString32Bytes BandLabel;
        public FixedString32Bytes StatusLabel;
    }
}
