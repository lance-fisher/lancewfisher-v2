using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for a player-side missionary dispatch.
    /// </summary>
    public struct PlayerMissionaryDispatchRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
    }
}
