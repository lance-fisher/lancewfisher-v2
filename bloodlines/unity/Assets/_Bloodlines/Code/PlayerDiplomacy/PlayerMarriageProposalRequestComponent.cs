using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for player-initiated marriage proposals.
    /// The debug surface currently supplies source faction + target member; the
    /// source member id is optional so the runtime can auto-select the first
    /// eligible non-head dynasty member for the faction, matching the browser
    /// candidate-selection bias used by AI proposal dispatch.
    /// </summary>
    public struct PlayerMarriageProposalRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes RequestedSourceMemberId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
    }
}
