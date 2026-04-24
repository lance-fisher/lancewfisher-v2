using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-production-building component. When IsActive, newly spawned units from this
    /// building receive a MoveCommandComponent targeting TargetPosition so they march
    /// to the rally point instead of idling at the spawn offset.
    ///
    /// Set by RallyPointSetSystem consuming PlayerRallyPointSetRequestComponent.
    /// Browser equivalent: absent -- implemented from canonical production design.
    /// </summary>
    public struct RallyPointComponent : IComponentData
    {
        /// World-space rally destination.
        public float3 TargetPosition;

        /// Whether this rally point is active. Inactive building spawns use the default
        /// spawn offset instead.
        public bool IsActive;
    }

    /// <summary>
    /// One-shot request written by the player (or AI) onto a production building entity
    /// to set or clear its rally point. Consumed and removed by RallyPointSetSystem.
    /// </summary>
    public struct PlayerRallyPointSetRequestComponent : IComponentData
    {
        /// World-space target for the rally point.
        public float3 TargetPosition;

        /// True to activate the rally point; false to clear it.
        public bool IsActive;
    }
}
