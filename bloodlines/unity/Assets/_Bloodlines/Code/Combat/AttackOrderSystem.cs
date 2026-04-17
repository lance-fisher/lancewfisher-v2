using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Converts player-issued combat orders into the existing attack-target and move-command flow.
    /// Explicit target orders only persist while the hostile remains visible. Attack-move orders
    /// preserve their destination so units resume marching after each local engagement.
    /// </summary>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AutoAcquireTargetSystem))]
    public partial struct AttackOrderSystem : ISystem
    {
        private const float DefaultAttackMoveStoppingDistance = 0.6f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AttackOrderComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<CombatStatsComponent>(),
                ComponentType.ReadOnly<MoveCommandComponent>(),
                ComponentType.ReadOnly<AttackOrderComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var combatStats = unitQuery.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);
            using var moveCommands = unitQuery.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            using var attackOrders = unitQuery.ToComponentDataArray<AttackOrderComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var attackOrder = attackOrders[i];
                if (!attackOrder.IsActive)
                {
                    continue;
                }

                if (entityManager.HasComponent<DeadTag>(entity) ||
                    health[i].Current <= 0f ||
                    CombatDisabled(combatStats[i]))
                {
                    DeactivateAttackOrder(entityManager, entity, attackOrder, moveCommands[i], removeAttackTarget: true);
                    continue;
                }

                if (attackOrder.ExplicitTargetEntity != Entity.Null)
                {
                    HandleExplicitTargetOrder(
                        entityManager,
                        entity,
                        factions[i].FactionId,
                        positions[i].Value,
                        combatStats[i],
                        moveCommands[i],
                        attackOrder);
                    continue;
                }

                if (attackOrder.IsAttackMoveDestination)
                {
                    HandleAttackMoveOrder(
                        entityManager,
                        entity,
                        positions[i].Value,
                        moveCommands[i],
                        attackOrder);
                    continue;
                }

                DeactivateAttackOrder(entityManager, entity, attackOrder, moveCommands[i], removeAttackTarget: false);
            }
        }

        private static void HandleExplicitTargetOrder(
            EntityManager entityManager,
            Entity attackerEntity,
            FixedString32Bytes attackerFactionId,
            float3 attackerPosition,
            in CombatStatsComponent combat,
            in MoveCommandComponent moveCommand,
            AttackOrderComponent attackOrder)
        {
            if (!TryResolveExplicitTarget(
                    entityManager,
                    attackerFactionId,
                    attackOrder.ExplicitTargetEntity,
                    out var targetPosition))
            {
                DeactivateAttackOrder(entityManager, attackerEntity, attackOrder, moveCommand, removeAttackTarget: true);
                return;
            }

            float sight = math.max(0.1f, combat.Sight);
            if (math.distancesq(attackerPosition, targetPosition) > sight * sight)
            {
                DeactivateAttackOrder(entityManager, attackerEntity, attackOrder, moveCommand, removeAttackTarget: true);
                return;
            }

            UpsertAttackTarget(entityManager, attackerEntity, attackOrder.ExplicitTargetEntity, combat.AttackRange);
            entityManager.SetComponentData(attackerEntity, attackOrder);
        }

        private static void HandleAttackMoveOrder(
            EntityManager entityManager,
            Entity attackerEntity,
            float3 attackerPosition,
            in MoveCommandComponent moveCommand,
            AttackOrderComponent attackOrder)
        {
            if (entityManager.HasComponent<AttackTargetComponent>(attackerEntity))
            {
                var attackTarget = entityManager.GetComponentData<AttackTargetComponent>(attackerEntity);
                if (attackTarget.TargetEntity != Entity.Null &&
                    entityManager.Exists(attackTarget.TargetEntity) &&
                    !entityManager.HasComponent<DeadTag>(attackTarget.TargetEntity) &&
                    entityManager.HasComponent<HealthComponent>(attackTarget.TargetEntity) &&
                    entityManager.GetComponentData<HealthComponent>(attackTarget.TargetEntity).Current > 0f)
                {
                    entityManager.SetComponentData(attackerEntity, attackOrder);
                    return;
                }

                entityManager.RemoveComponent<AttackTargetComponent>(attackerEntity);
            }

            float stoppingDistance = math.max(
                0.1f,
                moveCommand.StoppingDistance > 0f ? moveCommand.StoppingDistance : DefaultAttackMoveStoppingDistance);

            if (math.distancesq(attackerPosition, attackOrder.AttackMoveDestination) <= stoppingDistance * stoppingDistance)
            {
                DeactivateAttackOrder(entityManager, attackerEntity, attackOrder, moveCommand, removeAttackTarget: false);
                return;
            }

            var nextMoveCommand = moveCommand;
            nextMoveCommand.Destination = attackOrder.AttackMoveDestination;
            nextMoveCommand.StoppingDistance = stoppingDistance;
            nextMoveCommand.IsActive = true;
            entityManager.SetComponentData(attackerEntity, nextMoveCommand);
            entityManager.SetComponentData(attackerEntity, attackOrder);
        }

        private static void DeactivateAttackOrder(
            EntityManager entityManager,
            Entity entity,
            AttackOrderComponent attackOrder,
            MoveCommandComponent moveCommand,
            bool removeAttackTarget)
        {
            attackOrder.ExplicitTargetEntity = Entity.Null;
            attackOrder.IsAttackMoveDestination = false;
            attackOrder.AttackMoveDestination = float3.zero;
            attackOrder.IsActive = false;
            entityManager.SetComponentData(entity, attackOrder);

            moveCommand.IsActive = false;
            entityManager.SetComponentData(entity, moveCommand);

            if (removeAttackTarget && entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                entityManager.RemoveComponent<AttackTargetComponent>(entity);
            }
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

        private static bool CombatDisabled(in CombatStatsComponent combat)
        {
            return combat.AttackDamage <= 0f || combat.AttackRange <= 0f || combat.Sight <= 0f;
        }

        private static bool TryResolveExplicitTarget(
            EntityManager entityManager,
            FixedString32Bytes attackerFactionId,
            Entity targetEntity,
            out float3 targetPosition)
        {
            if (targetEntity == Entity.Null ||
                !entityManager.Exists(targetEntity) ||
                entityManager.HasComponent<DeadTag>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity) ||
                !entityManager.HasComponent<PositionComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity))
            {
                targetPosition = default;
                return false;
            }

            var targetHealth = entityManager.GetComponentData<HealthComponent>(targetEntity);
            var targetFaction = entityManager.GetComponentData<FactionComponent>(targetEntity);
            if (targetHealth.Current <= 0f ||
                !IsFactionHostile(entityManager, attackerFactionId, targetFaction.FactionId))
            {
                targetPosition = default;
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
