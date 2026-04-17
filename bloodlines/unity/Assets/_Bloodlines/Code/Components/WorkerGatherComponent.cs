using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical worker gather phase. Mirrors the browser runtime's gather loop:
    /// seek node, gather until carry full or node depleted, return to owned drop-off,
    /// deposit, then either resume on the same node or idle.
    /// </summary>
    public enum WorkerGatherPhase : byte
    {
        Idle = 0,
        Seeking = 1,
        Gathering = 2,
        Returning = 3,
        Depositing = 4,
    }

    /// <summary>
    /// Worker gather assignment and carry state.
    ///
    /// Browser runtime equivalent: worker.gatherAssignment + worker.carry.
    /// A worker is a gatherer when this component is present AND phase != Idle.
    /// The system resolves movement, gather, deposit, and resume against the
    /// canonical unit stats (carryCapacity, gatherRate) imported from data.
    /// </summary>
    public struct WorkerGatherComponent : IComponentData
    {
        public Entity AssignedNode;
        public FixedString32Bytes AssignedResourceId;
        public FixedString32Bytes CarryResourceId;
        public float CarryAmount;
        public float CarryCapacity;
        public float GatherRate;
        public WorkerGatherPhase Phase;
        public float GatherRadius;
        public float DepositRadius;
    }
}
