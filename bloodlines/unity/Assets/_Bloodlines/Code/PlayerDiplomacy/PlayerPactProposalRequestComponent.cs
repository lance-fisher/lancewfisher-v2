using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public struct PlayerPactProposalRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
    }
}
