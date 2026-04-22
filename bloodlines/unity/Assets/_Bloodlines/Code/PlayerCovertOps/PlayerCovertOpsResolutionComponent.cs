using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    public struct PlayerCovertOpsResolutionComponent : IComponentData
    {
        public FixedString64Bytes OperationId;
        public CovertOpKindPlayer Kind;
        public FixedString32Bytes Subtype;
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public int TargetEntityIndex;
        public float StartedAtInWorldDays;
        public float ResolveAtInWorldDays;
        public float ReportExpiresAtInWorldDays;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public FixedString64Bytes TargetLabel;
        public FixedString64Bytes LocationLabel;
        public float SuccessScore;
        public float ProjectedChance;
        public bool IntelSupport;
        public float IntelSupportBonus;
        public float CounterIntelligenceDefense;
        public bool CounterIntelligenceActive;
        public float BloodlineGuardBonus;
        public float WatchDurationInWorldDays;
        public float WatchStrength;
        public FixedString64Bytes WardLabel;
        public FixedString128Bytes GuardedRoles;
        public float AverageLoyalty;
        public float WeakestLoyalty;
        public bool Active;
        public float EscrowGold;
        public float EscrowInfluence;
    }
}
