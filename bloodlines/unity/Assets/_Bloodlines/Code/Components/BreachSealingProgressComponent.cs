using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-settlement breach sealing progress.
    /// Tracks one active breach repair at a time and is attached lazily when a
    /// fortification first enters the sealing loop.
    /// </summary>
    public struct BreachSealingProgressComponent : IComponentData
    {
        public float AccumulatedWorkerHours;
        public float StoneReservedForCurrentBreach;
        public float LastTickInWorldDays;
    }
}
