using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Faith
{
    public struct PlayerCovenantTestRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
    }
}
