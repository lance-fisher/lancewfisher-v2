using Unity.Entities;

namespace Bloodlines.Components
{
    public enum DestroyedCounterKind : byte
    {
        None = 0,
        Wall = 1,
        Tower = 2,
        Gate = 3,
        Keep = 4,
    }

    /// <summary>
    /// Per-settlement destroyed-structure rebuild progress.
    /// Tracks one active wall, tower, gate, or keep rebuild at a time after all
    /// open breaches have already been sealed.
    /// </summary>
    public struct DestroyedCounterRecoveryProgressComponent : IComponentData
    {
        public float AccumulatedWorkerHours;
        public float StoneReservedForCurrentSegment;
        public float LastTickInWorldDays;
        public DestroyedCounterKind TargetCounter;
    }
}
