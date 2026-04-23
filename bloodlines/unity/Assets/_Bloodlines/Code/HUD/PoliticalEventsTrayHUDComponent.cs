using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Player-facing tray row for active dynasty political effects.
    /// Stored as a DynamicBuffer on each faction root; the player HUD consumes the
    /// player faction's first four active events.
    /// </summary>
    [InternalBufferCapacity(4)]
    public struct PoliticalEventsTrayHUDComponent : IBufferElementData
    {
        public FixedString32Bytes EventType;
        public FixedString64Bytes EventLabel;
        public float RemainingInWorldDays;
    }

    public struct PoliticalEventsTrayHUDStateComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float LastRefreshInWorldDays;
        public int ActiveEventCount;
    }
}
