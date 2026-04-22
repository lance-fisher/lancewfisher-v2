using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    public struct PlayerCovertOpsRequestComponent : IComponentData
    {
        public CovertOpKindPlayer Kind;
        public FixedString32Bytes Subtype;
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public int TargetEntityIndex;
    }
}
