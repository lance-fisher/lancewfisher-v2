using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to a DynastyOperationComponent entity
    /// whose OperationKind == DynastyOperationKind.Assassination.
    ///
    /// Browser reference: the operation object created by
    /// startAssassinationOperation (simulation.js:10912-10950):
    ///   type: "assassination"
    ///   sourceFactionId, targetFactionId, targetMemberId, memberId
    ///   memberTitle, startedAt, resolveAt
    ///   spymasterId, locationLabel, intelSupport
    ///   successScore, projectedChance, escrowCost
    ///
    /// Unity simplifications vs browser:
    ///   - locationLabel: not stored; no location-profile system yet.
    ///   - intelSupport: stored as a bool; AI dispatch already gates on
    ///     LiveIntelOnPlayer in AICovertOpsSystem so this is always true
    ///     at dispatch time; retained for audit and future query use.
    ///   - escrowCost: split into EscrowGold / EscrowInfluence to avoid
    ///     the DynastyOperationEscrowCost struct allocation overhead.
    ///   - spymasterId: stored as OperatorMemberId.
    /// </summary>
    public struct DynastyOperationAssassinationComponent : IComponentData
    {
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public FixedString64Bytes TargetMemberTitle;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float ResolveAtInWorldDays;
        public float SuccessScore;
        public float ProjectedChance;
        public float EscrowGold;
        public float EscrowInfluence;
        public bool IntelSupport;
    }
}
