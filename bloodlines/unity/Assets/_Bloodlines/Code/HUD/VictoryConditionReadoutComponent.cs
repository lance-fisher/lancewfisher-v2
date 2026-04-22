using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD read-model element exposing progress toward each canonical
    /// victory condition. Stored as a DynamicBuffer on the faction root entity.
    /// </summary>
    [InternalBufferCapacity(3)]
    public struct VictoryConditionReadoutComponent : IBufferElementData
    {
        public FixedString32Bytes ConditionId;
        public float ProgressPct;
        public bool IsLeading;
        public float TimeRemainingEstimateInWorldDays;
    }

    /// <summary>
    /// Tracks the last in-world day when the faction's victory readout buffer was
    /// refreshed so the HUD lane can throttle updates.
    /// </summary>
    public struct VictoryConditionReadoutRefreshComponent : IComponentData
    {
        public float LastRefreshInWorldDays;
    }
}
