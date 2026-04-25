using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-operation data for an espionage operation.
    /// Attached to the entity created by DynastyOperationLimits.BeginOperation
    /// with DynastyOperationKind.Espionage.
    ///
    /// Browser counterpart: dynasty operation of type "espionage"
    /// (startEspionageOperation, simulation.js ~10876-10910).
    /// Cost: gold 45, influence 16. Duration: 30 real-seconds.
    /// Report duration on success: 120 real-seconds (stored as world-days offset).
    /// </summary>
    public struct DynastyOperationEspionageComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float              ResolveAtInWorldDays;
        public float              ReportExpiresAtInWorldDays;   // inWorldDays + 120 at dispatch time
        public float              SuccessScore;
        public float              ProjectedChance;
        public float              EscrowGold;
        public float              EscrowInfluence;
    }
}
