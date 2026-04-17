using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Resolves explicit attack and attack-move orders into the existing attack-target and
    /// move-command components so the broader combat lane stays additive.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AutoAcquireTargetSystem))]
    public partial struct AttackOrderResolutionSystem : ISystem
    {
        private const float MinimumMoveStoppingDistance = 0.2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AttackOrderComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<CombatStatsComponent>(),
                ComponentType.ReadWrite<MoveCommandComponent>(),
                ComponentType.ReadWrite<AttackOrderComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var combatStats = query.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);
            using var moveCommands = query.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            using var attackOrders = query.ToComponentDataArray<AttackOrderComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (entityManager.HasComponent<DeadTag>(entity) || healthValues[i].Current <= 0f)
                {
                    continue;
                }

                var attackOrder = attackOrders[i];
                if (!attackOrder.IsActive)
                {
                    continue;
                }

                var moveCommand = moveCommands[i];
                if (attackOrder.IsAttackMoveActive)
                {
                    ResolveAttackMoveOrder(
                        entityManager,
                        entity,
                        factions[i].FactionId,
                        positions[i].Value,
                        math.max(0.1f, combatStats[i].AttackRange),
                        ref moveCommand,
                        ref attackOrder);
                }
                else
                {
                    ResolveExplicitAttackOrder(
                        entityManager,
                        entity,
                        factions[i].FactionId,
                        positions[i].Value,
                        math.max(0.1f, combatStats[i].AttackRange),
                        ref moveCommand,
                        ref attackOrder);
                }

                entityManager.SetComponentData(entity, moveCommand);
                entityManager.SetComponentData(entity, attackOrder);
            }
        }

        private static void ResolveExplicitAttackOrder(
            EntityManager entityManager,
            Entity entity,
            FixedString32Bytes attackerFactionId,
            float3 attackerPosition,
            float engagementRange,
            ref MoveCommandComponent moveCommand,
            ref AttackOrderComponent attackOrder)
        {
            if (attackOrder.ExplicitTargetEntity == Entity.Null ||
                !TryGetLiveHostileTargetPosition(
                    entityManager,
                    attackOrder.ExplicitTargetEntity,
                    attackerFactionId,
                    out float3 targetPosition))
            {
                ClearExplicitAttackOrder(entityManager, entity, attackerPosition, ref moveCommand, ref attackOrder);
                return;
            }

            attackOrder.IsActive = true;
            attackOrder.IsAttackMoveActive = false;
            attackOrder.AttackMoveDestination = attackerPosition;

            UpsertAttackTarget(entityManager, entity, attackOrder.ExplicitTargetEntity, engagementRange);

            if (math.distancesq(attackerPosition, targetPosition) > engagementRange * engagementRange)
            {
                moveCommand.Destination = targetPosition;
                moveCommand.StoppingDistance = engagementRange;
                moveCommand.IsActive = true;
                return;
            }

            moveCommand.Destination = attackerPosition;
            moveCommand.StoppingDistance = engagementRange;
            moveCommand.IsActive = false;
        }

        private static void ResolveAttackMoveOrder(
            EntityManager entityManager,
            Entity entity,
            FixedString32Bytes attackerFactionId,
            float3 attackerPosition,
            float engagementRange,
            ref MoveCommandComponent moveCommand,
            ref AttackOrderComponent attackOrder)
        {
            attackOrder.ExplicitTargetEntity = Entity.Null;
            attackOrder.IsAttackMoveActive = true;

            if (TryGetCurrentLiveAttackTargetPosition(
                    entityManager,
                    entity,
                    attackerFactionId,
                    out float3 targetPosition))
            {
                if (math.distancesq(attackerPosition, targetPosition) > engagementRange * engagementRange)
                {
                    moveCommand.Destination = targetPosition;
                    moveCommand.StoppingDistance = engagementRange;
                    moveCommand.IsActive = true;
                    return;
                }

                moveCommand.Destination = attackerPosition;
                moveCommand.StoppingDistance = engagementRange;
                moveCommand.IsActive = false;
                return;
            }

            if (entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                entityManager.RemoveComponent<AttackTargetComponent>(entity);
            }

            float stoppingDistance = MinimumMoveStoppingDistance;
            if (math.distancesq(attackerPosition, attackOrder.AttackMoveDestination) <= stoppingDistance * stoppingDistance)
            {
                attackOrder.IsActive = false;
                attackOrder.IsAttackMoveActive = false;
                moveCommand.Destination = attackOrder.AttackMoveDestination;
                moveCommand.StoppingDistance = stoppingDistance;
                moveCommand.IsActive = false;
                return;
            }

            moveCommand.Destination = attackOrder.AttackMoveDestination;
            moveCommand.StoppingDistance = stoppingDistance;
            moveCommand.IsActive = true;
        }

        private static void ClearExplicitAttackOrder(
            EntityManager entityManager,
            Entity entity,
            float3 attackerPosition,
            ref MoveCommandComponent moveCommand,
            ref AttackOrderComponent attackOrder)
        {
            if (entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                entityManager.RemoveComponent<AttackTargetComponent>(entity);
            }

            attackOrder.ExplicitTargetEntity = Entity.Null;
            attackOrder.AttackMoveDestination = float3.zero;
            attackOrder.IsAttackMoveActive = false;
            attackOrder.IsActive = false;
            moveCommand.Destination = attackerPosition;
            moveCommand.StoppingDistance = MinimumMoveStoppingDistance;
            moveCommand.IsActive = false;
        }

        private static bool TryGetCurrentLiveAttackTargetPosition(
            EntityManager entityManager,
            Entity entity,
            FixedString32Bytes attackerFactionId,
            out float3 targetPosition)
        {
            if (!entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                targetPosition = default;
                return false;
            }

            var attackTarget = entityManager.GetComponentData<AttackTargetComponent>(entity);
            return TryGetLiveHostileTargetPosition(
                entityManager,
                attackTarget.TargetEntity,
                attackerFactionId,
                out targetPosition);
        }

        private static void UpsertAttackTarget(
            EntityManager entityManager,
            Entity entity,
            Entity targetEntity,
            float engagementRange)
        {
            var attackTarget = new AttackTargetComponent
            {
                TargetEntity = targetEntity,
                EngagementRange = math.max(0.1f, engagementRange),
            };

            if (entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                entityManager.SetComponentData(entity, attackTarget);
            }
            else
            {
                entityManager.AddComponentData(entity, attackTarget);
            }
        }

        private static bool TryGetLiveHostileTargetPosition(
            EntityManager entityManager,
            Entity targetEntity,
            FixedString32Bytes attackerFactionId,
            out float3 targetPosition)
        {
            targetPosition = default;

            if (targetEntity == Entity.Null ||
                !entityManager.Exists(targetEntity) ||
                entityManager.HasComponent<DeadTag>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity) ||
                !entityManager.HasComponent<PositionComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity))
            {
                return false;
            }

            var targetHealth = entityManager.GetComponentData<HealthComponent>(targetEntity);
            if (targetHealth.Current <= 0f)
            {
                return false;
            }

            var targetFaction = entityManager.GetComponentData<FactionComponent>(targetEntity);
            if (!IsFactionHostile(entityManager, attackerFactionId, targetFaction.FactionId))
            {
                return false;
            }

            targetPosition = entityManager.GetComponentData<PositionComponent>(targetEntity).Value;
            return true;
        }

        private static bool IsFactionHostile(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            if (sourceFactionId.Equals(targetFactionId))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HostilityComponent>());

            using var factionEntities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(sourceFactionId))
                {
                    continue;
                }

                var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntities[i]);
                for (int j = 0; j < hostilityBuffer.Length; j++)
                {
                    if (hostilityBuffer[j].HostileFactionId.Equals(targetFactionId))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }
    }
}
