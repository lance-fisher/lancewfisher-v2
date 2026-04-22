using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for a player-side captive ransom dispatch.
    /// Gold stays explicit on the request so the player surface can present the
    /// offered ransom amount while the dispatch system still enforces the
    /// canonical minimum and fixed influence escrow.
    /// </summary>
    public struct PlayerCaptiveRansomRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes CaptiveMemberId;
        public FixedString32Bytes TargetFactionId;
        public int RansomGoldAmount;
    }
}
