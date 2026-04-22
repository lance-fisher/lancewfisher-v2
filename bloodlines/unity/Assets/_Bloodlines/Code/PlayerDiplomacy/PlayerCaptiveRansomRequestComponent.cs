using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public struct PlayerCaptiveRansomRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes CaptiveMemberId;
        public FixedString32Bytes TargetFactionId;
        public float RansomGoldAmount;
    }
}
