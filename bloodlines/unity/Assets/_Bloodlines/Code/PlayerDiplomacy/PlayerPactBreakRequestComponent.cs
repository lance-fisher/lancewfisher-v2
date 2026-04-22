using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public struct PlayerPactBreakRequestComponent : IComponentData
    {
        public FixedString32Bytes RequestingFactionId;
        public FixedString32Bytes TargetFactionId;
    }
}
