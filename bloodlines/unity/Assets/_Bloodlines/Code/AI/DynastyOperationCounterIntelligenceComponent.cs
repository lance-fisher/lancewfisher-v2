using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-operation data for a counter-intelligence operation.
    /// Attached to the entity created by DynastyOperationLimits.BeginOperation
    /// with DynastyOperationKind.CounterIntelligence.
    ///
    /// Browser counterpart: dynasty operation of type "counterIntelligence"
    /// (startCounterIntelligenceOperation, simulation.js ~10837-10874).
    /// Cost: gold 60, influence 18. Duration: 18 real-seconds.
    /// Watch duration on success: 150 real-seconds.
    /// </summary>
    public struct DynastyOperationCounterIntelligenceComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float              ResolveAtInWorldDays;
        public float              SuccessScore;
        public float              ProjectedChance;
        public float              EscrowGold;
        public float              EscrowInfluence;
    }
}
