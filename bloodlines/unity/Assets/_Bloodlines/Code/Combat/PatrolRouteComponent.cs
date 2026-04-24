using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-unit patrol state. When IsPatrolling is true, PatrolMovementSystem alternates
    /// the unit's MoveCommandComponent between WaypointA and WaypointB indefinitely.
    /// Patrol is suspended (but not cleared) while AttackOrderComponent.IsActive is true
    /// and resumes automatically once the attack order clears.
    ///
    /// Set by PatrolOrderSystem consuming PlayerPatrolOrderRequestComponent.
    /// Browser equivalent: absent -- implemented from canonical garrison/perimeter design.
    /// </summary>
    public struct PatrolRouteComponent : IComponentData
    {
        public float3 WaypointA;
        public float3 WaypointB;

        /// 0 = targeting WaypointA, 1 = targeting WaypointB.
        public byte CurrentTarget;

        public bool IsPatrolling;

        /// World-space radius within which a waypoint is considered reached.
        public float ArrivalThreshold;
    }

    /// <summary>
    /// One-shot request placed on a unit entity to begin a patrol route.
    /// Consumed and removed by PatrolOrderSystem.
    /// </summary>
    public struct PlayerPatrolOrderRequestComponent : IComponentData
    {
        public float3 WaypointA;
        public float3 WaypointB;
    }

    /// <summary>
    /// Zero-size tag placed on a unit entity to cancel its active patrol route.
    /// Consumed and removed by PatrolOrderSystem.
    /// </summary>
    public struct PlayerPatrolCancelRequestComponent : IComponentData { }
}
