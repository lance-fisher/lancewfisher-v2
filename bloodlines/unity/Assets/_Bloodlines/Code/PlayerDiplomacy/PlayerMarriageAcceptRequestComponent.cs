using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public struct PlayerMarriageAcceptRequestComponent : IComponentData
    {
        public int ProposalEntityIndex;
    }
}
