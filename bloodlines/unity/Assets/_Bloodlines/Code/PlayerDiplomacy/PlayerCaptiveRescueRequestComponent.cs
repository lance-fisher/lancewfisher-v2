using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for a player-side captive rescue dispatch.
    /// Captive ids stay string-backed because the live dynasty and captive-ledger
    /// surfaces use canonical member ids rather than integer handles.
    /// </summary>
    public struct PlayerCaptiveRescueRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes CaptiveMemberId;
    }
}
