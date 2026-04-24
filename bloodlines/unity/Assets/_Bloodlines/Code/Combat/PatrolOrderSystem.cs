using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Combat
{
    /// <summary>
    /// Consumes PlayerPatrolOrderRequestComponent and PlayerPatrolCancelRequestComponent on
    /// unit entities. A patrol request writes PatrolRouteComponent (lazy-added) and issues an
    /// initial MoveCommandComponent targeting WaypointA. A cancel request clears IsPatrolling
    /// and removes both components.
    ///
    /// Browser equivalent: absent -- implemented from canonical garrison/perimeter design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PatrolOrderSystem : ISystem
    {
        private const float DefaultArrivalThreshold = 1.2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerPatrolOrderRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Process set-patrol requests.
            var setQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerPatrolOrderRequestComponent>());
            using var setEntities = setQuery.ToEntityArray(Allocator.Temp);
            using var setRequests = setQuery.ToComponentDataArray<PlayerPatrolOrderRequestComponent>(Allocator.Temp);
            setQuery.Dispose();

            for (int i = 0; i < setEntities.Length; i++)
            {
                Entity unitEntity = setEntities[i];
                var req = setRequests[i];

                var patrol = new PatrolRouteComponent
                {
                    WaypointA = req.WaypointA,
                    WaypointB = req.WaypointB,
                    CurrentTarget = 0,
                    IsPatrolling = true,
                    ArrivalThreshold = DefaultArrivalThreshold,
                };

                if (em.HasComponent<PatrolRouteComponent>(unitEntity))
                {
                    em.SetComponentData(unitEntity, patrol);
                }
                else
                {
                    em.AddComponentData(unitEntity, patrol);
                }

                // Issue initial move to WaypointA.
                var moveCmd = new MoveCommandComponent
                {
                    Destination = req.WaypointA,
                    StoppingDistance = DefaultArrivalThreshold,
                    IsActive = true,
                };

                if (em.HasComponent<MoveCommandComponent>(unitEntity))
                {
                    em.SetComponentData(unitEntity, moveCmd);
                }
                else
                {
                    em.AddComponentData(unitEntity, moveCmd);
                }

                em.RemoveComponent<PlayerPatrolOrderRequestComponent>(unitEntity);
            }

            // Process cancel requests.
            var cancelQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerPatrolCancelRequestComponent>());
            using var cancelEntities = cancelQuery.ToEntityArray(Allocator.Temp);
            cancelQuery.Dispose();

            for (int i = 0; i < cancelEntities.Length; i++)
            {
                Entity unitEntity = cancelEntities[i];

                if (em.HasComponent<PatrolRouteComponent>(unitEntity))
                {
                    em.RemoveComponent<PatrolRouteComponent>(unitEntity);
                }

                if (em.HasComponent<MoveCommandComponent>(unitEntity))
                {
                    em.SetComponentData(unitEntity, new MoveCommandComponent
                    {
                        Destination = default,
                        StoppingDistance = 0.2f,
                        IsActive = false,
                    });
                }

                em.RemoveComponent<PlayerPatrolCancelRequestComponent>(unitEntity);
            }
        }
    }
}
