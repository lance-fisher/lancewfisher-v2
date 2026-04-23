using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public struct PlayerSuccessionPreferenceRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes TargetMemberId;
    }
}
