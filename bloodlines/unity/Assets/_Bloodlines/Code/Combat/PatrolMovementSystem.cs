using Bloodlines.Components;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Combat
{
    /// <summary>
    /// Per-frame patrol waypoint advancement. For each unit with an active PatrolRouteComponent:
    /// - If AttackOrderComponent.IsActive is true: suspend (do not alter MoveCommand or flip target).
    /// - If within ArrivalThreshold of the current waypoint: flip CurrentTarget and issue a new
    ///   MoveCommandComponent to the next waypoint.
    /// - If MoveCommandComponent.IsActive is false and no attack order: reissue the current
    ///   waypoint move to recover from any command that cleared the order without resuming patrol.
    ///
    /// Runs before AttackOrderResolutionSystem so patrol wayfinding cannot race with attack orders.
    ///
    /// Browser equivalent: absent -- implemented from canonical garrison/perimeter design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AttackOrderResolutionSystem))]
    public partial struct PatrolMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PatrolRouteComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var patrolQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<PatrolRouteComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var entities = patrolQuery.ToEntityArray(Allocator.Temp);
            using var patrols = patrolQuery.ToComponentDataArray<PatrolRouteComponent>(Allocator.Temp);
            using var positions = patrolQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            patrolQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                var patrol = patrols[i];

                if (!patrol.IsPatrolling)
                {
                    continue;
                }

                // Suspend patrol while an explicit attack order is active.
                bool attackOrderActive = em.HasComponent<AttackOrderComponent>(entity) &&
                                        em.GetComponentData<AttackOrderComponent>(entity).IsActive;
                if (attackOrderActive)
                {
                    continue;
                }

                float3 currentPos = positions[i].Value;
                float3 targetWaypoint = patrol.CurrentTarget == 0 ? patrol.WaypointA : patrol.WaypointB;
                float distSq = math.distancesq(currentPos, targetWaypoint);

                if (distSq <= patrol.ArrivalThreshold * patrol.ArrivalThreshold)
                {
                    // Arrived -- flip to the other waypoint.
                    byte nextTarget = patrol.CurrentTarget == 0 ? (byte)1 : (byte)0;
                    float3 nextWaypoint = nextTarget == 0 ? patrol.WaypointA : patrol.WaypointB;

                    var updatedPatrol = patrol;
                    updatedPatrol.CurrentTarget = nextTarget;
                    em.SetComponentData(entity, updatedPatrol);

                    var moveCmd = new MoveCommandComponent
                    {
                        Destination = nextWaypoint,
                        StoppingDistance = patrol.ArrivalThreshold,
                        IsActive = true,
                    };

                    if (em.HasComponent<MoveCommandComponent>(entity))
                    {
                        em.SetComponentData(entity, moveCmd);
                    }
                }
                else
                {
                    // Not yet arrived -- reissue move command if it was cleared externally.
                    bool moveCmdActive = em.HasComponent<MoveCommandComponent>(entity) &&
                                        em.GetComponentData<MoveCommandComponent>(entity).IsActive;
                    if (!moveCmdActive)
                    {
                        var moveCmd = new MoveCommandComponent
                        {
                            Destination = targetWaypoint,
                            StoppingDistance = patrol.ArrivalThreshold,
                            IsActive = true,
                        };

                        if (em.HasComponent<MoveCommandComponent>(entity))
                        {
                            em.SetComponentData(entity, moveCmd);
                        }
                    }
                }
            }
        }
    }
}
