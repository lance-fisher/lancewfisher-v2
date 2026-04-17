using Bloodlines.Components;
using Bloodlines.Pathing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Reapplies group-aware destinations before the attack-order and movement loops run so
    /// marching units resume their personal formation slots after combat interruptions.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AttackOrderResolutionSystem))]
    [UpdateBefore(typeof(UnitMovementSystem))]
    public partial struct GroupMovementResolutionSystem : ISystem
    {
        private const float DirectMoveStoppingDistance = 0.6f;
        private const float AttackMoveStoppingDistance = 0.2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GroupMovementOrderComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadWrite<MoveCommandComponent>(),
                ComponentType.ReadWrite<GroupMovementOrderComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var moveCommands = query.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            using var groupOrders = query.ToComponentDataArray<GroupMovementOrderComponent>(Allocator.Temp);
            using var removeGroupOrders = new NativeList<Entity>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (entityManager.HasComponent<DeadTag>(entity) || healthValues[i].Current <= 0f)
                {
                    continue;
                }

                var moveCommand = moveCommands[i];
                var groupOrder = groupOrders[i];
                float3 desiredDestination = groupOrder.DestinationCenter + groupOrder.LocalOffset;
                float stoppingDistance = groupOrder.IsAttackMove
                    ? AttackMoveStoppingDistance
                    : DirectMoveStoppingDistance;
                bool hasAttackTarget = entityManager.HasComponent<AttackTargetComponent>(entity);

                if (groupOrder.IsAttackMove)
                {
                    var attackOrder = entityManager.HasComponent<AttackOrderComponent>(entity)
                        ? entityManager.GetComponentData<AttackOrderComponent>(entity)
                        : default;

                    attackOrder.ExplicitTargetEntity = Entity.Null;
                    attackOrder.AttackMoveDestination = desiredDestination;
                    attackOrder.IsAttackMoveActive = true;
                    attackOrder.IsActive = true;

                    if (math.distancesq(positions[i].Value, desiredDestination) <= stoppingDistance * stoppingDistance &&
                        !hasAttackTarget)
                    {
                        attackOrder.IsActive = false;
                        attackOrder.IsAttackMoveActive = false;
                        moveCommand.Destination = desiredDestination;
                        moveCommand.StoppingDistance = stoppingDistance;
                        moveCommand.IsActive = false;
                        removeGroupOrders.Add(entity);
                    }
                    else if (!hasAttackTarget)
                    {
                        moveCommand.Destination = desiredDestination;
                        moveCommand.StoppingDistance = stoppingDistance;
                        moveCommand.IsActive = true;
                    }

                    if (entityManager.HasComponent<AttackOrderComponent>(entity))
                    {
                        entityManager.SetComponentData(entity, attackOrder);
                    }
                    else
                    {
                        entityManager.AddComponentData(entity, attackOrder);
                    }
                }
                else if (math.distancesq(positions[i].Value, desiredDestination) <= stoppingDistance * stoppingDistance)
                {
                    moveCommand.Destination = desiredDestination;
                    moveCommand.StoppingDistance = stoppingDistance;
                    moveCommand.IsActive = false;
                    removeGroupOrders.Add(entity);
                }
                else if (!hasAttackTarget)
                {
                    moveCommand.Destination = desiredDestination;
                    moveCommand.StoppingDistance = stoppingDistance;
                    moveCommand.IsActive = true;
                }

                entityManager.SetComponentData(entity, moveCommand);
            }

            for (int i = 0; i < removeGroupOrders.Length; i++)
            {
                if (!entityManager.Exists(removeGroupOrders[i]) ||
                    !entityManager.HasComponent<GroupMovementOrderComponent>(removeGroupOrders[i]))
                {
                    continue;
                }

                entityManager.RemoveComponent<GroupMovementOrderComponent>(removeGroupOrders[i]);
            }
        }
    }
}
