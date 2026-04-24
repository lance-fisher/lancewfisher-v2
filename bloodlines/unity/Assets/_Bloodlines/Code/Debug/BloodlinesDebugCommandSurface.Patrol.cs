using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugSetPatrol(Entity unitEntity, float3 waypointA, float3 waypointB)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity) ||
                !entityManager.HasComponent<UnitTypeComponent>(unitEntity))
            {
                return false;
            }

            var request = new PlayerPatrolOrderRequestComponent
            {
                WaypointA = waypointA,
                WaypointB = waypointB,
            };

            if (entityManager.HasComponent<PlayerPatrolOrderRequestComponent>(unitEntity))
            {
                entityManager.SetComponentData(unitEntity, request);
            }
            else
            {
                entityManager.AddComponentData(unitEntity, request);
            }

            return true;
        }

        public bool TryDebugCancelPatrol(Entity unitEntity)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity))
            {
                return false;
            }

            if (!entityManager.HasComponent<PatrolRouteComponent>(unitEntity) &&
                !entityManager.HasComponent<PlayerPatrolCancelRequestComponent>(unitEntity))
            {
                return false;
            }

            if (entityManager.HasComponent<PlayerPatrolCancelRequestComponent>(unitEntity))
            {
                return true;
            }

            entityManager.AddComponentData(unitEntity, new PlayerPatrolCancelRequestComponent());
            return true;
        }

        public bool TryDebugGetPatrolRoute(
            Entity unitEntity,
            out float3 waypointA,
            out float3 waypointB,
            out byte currentTarget,
            out bool isPatrolling)
        {
            waypointA = float3.zero;
            waypointB = float3.zero;
            currentTarget = 0;
            isPatrolling = false;

            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity) ||
                !entityManager.HasComponent<PatrolRouteComponent>(unitEntity))
            {
                return false;
            }

            var patrol = entityManager.GetComponentData<PatrolRouteComponent>(unitEntity);
            waypointA = patrol.WaypointA;
            waypointB = patrol.WaypointB;
            currentTarget = patrol.CurrentTarget;
            isPatrolling = patrol.IsPatrolling;
            return true;
        }
    }
}
