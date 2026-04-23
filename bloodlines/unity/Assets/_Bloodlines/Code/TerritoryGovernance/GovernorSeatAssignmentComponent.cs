using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.TerritoryGovernance
{
    public enum GovernanceAnchorType : byte
    {
        ControlPoint = 0,
        Settlement = 1,
    }

    public enum GovernorSpecializationId : byte
    {
        None = 0,
        BorderMarshal = 1,
        CivicSteward = 2,
        KeepCastellan = 3,
    }

    /// <summary>
    /// Seat assignment materialized on a control point or settlement anchor after
    /// the daily governor-sync pass mirrors the browser's governorAssignments ledger.
    /// </summary>
    public struct GovernorSeatAssignmentComponent : IComponentData
    {
        public FixedString64Bytes GovernorMemberId;
        public GovernorSpecializationId SpecializationId;
        public GovernanceAnchorType AnchorType;
        public float PriorityScore;
        public float LastSyncedInWorldDays;
    }
}
