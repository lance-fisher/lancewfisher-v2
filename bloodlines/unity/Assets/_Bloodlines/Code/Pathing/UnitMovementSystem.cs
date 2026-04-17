using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Pathing
{
    /// <summary>
    /// First ECS movement foundation for Bloodlines battlefield entities.
    ///
    /// This deliberately mirrors the browser runtime's command semantics instead of
    /// jumping straight to full pathfinding: entities move toward an issued destination
    /// at their current movement rate until they enter the stopping radius. Higher-order
    /// path planners can later feed this system through the same MoveCommandComponent.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct UnitMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                return;
            }

            foreach (var (positionRw, moveCommandRw, movementStats) in
                SystemAPI.Query<
                    RefRW<PositionComponent>,
                    RefRW<MoveCommandComponent>,
                    RefRO<MovementStatsComponent>>()
                .WithNone<DeadTag>())
            {
                ref var position = ref positionRw.ValueRW;
                ref var moveCommand = ref moveCommandRw.ValueRW;

                if (!moveCommand.IsActive)
                {
                    continue;
                }

                float speed = math.max(0f, movementStats.ValueRO.MaxSpeed);
                if (speed <= 0f)
                {
                    continue;
                }

                float3 delta = moveCommand.Destination - position.Value;
                float distanceSq = math.lengthsq(delta);
                float stoppingDistance = math.max(0.05f, moveCommand.StoppingDistance);
                float stoppingDistanceSq = stoppingDistance * stoppingDistance;

                if (distanceSq <= stoppingDistanceSq)
                {
                    position.Value = moveCommand.Destination;
                    moveCommand.IsActive = false;
                    continue;
                }

                float distance = math.sqrt(distanceSq);
                float step = math.min(speed * dt, distance);
                position.Value += (delta / distance) * step;

                float remaining = distance - step;
                if (remaining <= stoppingDistance)
                {
                    position.Value = moveCommand.Destination;
                    moveCommand.IsActive = false;
                }
            }
        }
    }
}
