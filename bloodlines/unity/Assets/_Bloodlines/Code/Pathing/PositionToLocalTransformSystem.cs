using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Bloodlines.Pathing
{
    /// <summary>
    /// Keeps ECS simulation position and Unity transform position aligned for entities
    /// that use LocalTransform for presentation or hybrid authoring.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UnitMovementSystem))]
    public partial struct PositionToLocalTransformSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (position, transformRw) in
                SystemAPI.Query<RefRO<PositionComponent>, RefRW<LocalTransform>>())
            {
                var transform = transformRw.ValueRO;
                transform.Position = position.ValueRO.Value;
                transformRw.ValueRW = transform;
            }
        }
    }
}
