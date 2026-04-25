using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Singleton component that owns the audio event dispatch queue.
    /// The attached dynamic buffer carries live <see cref="AudioEventElement"/> entries.
    /// </summary>
    public struct AudioEventDispatchComponent : IComponentData
    {
        public int PendingEventCount;
        public float LastFlushedAtInWorldDays;
    }
}

