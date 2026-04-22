using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    public struct PlayerCovertOpsResolutionComponent : IComponentData
    {
        public FixedString64Bytes OperationId;
        public CovertOpKindPlayer Kind;
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public int TargetEntityIndex;
        public float StartedAtInWorldDays;
        public float ResolveAtInWorldDays;
        public float ReportExpiresAtInWorldDays;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float SuccessScore;
        public float ProjectedChance;
        public float CounterIntelligenceDefense;
        public bool CounterIntelligenceActive;
        public bool Active;
        public float EscrowGold;
        public float EscrowInfluence;
    }
}
