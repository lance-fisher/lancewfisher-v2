using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AutoAcquireTargetSystem))]
    [UpdateBefore(typeof(AttackOrderResolutionSystem))]
    public partial struct CombatStanceResolutionSystem : ISystem
    {
        private const float RetreatStoppingDistance = 0.6f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CombatStanceComponent>();
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
                ComponentType.ReadWrite<CombatStanceComponent>(),
                ComponentType.ReadWrite<CombatStanceRuntimeComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var combatStats = query.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);
            using var moveCommands = query.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            using var stances = query.ToComponentDataArray<CombatStanceComponent>(Allocator.Temp);
            using var stanceStates = query.ToComponentDataArray<CombatStanceRuntimeComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (entityManager.HasComponent<DeadTag>(entity) || healthValues[i].Current <= 0f)
                {
                    continue;
                }

                var stance = stances[i];
                var runtime = stanceStates[i];
                var moveCommand = moveCommands[i];

                if (stance.Stance == CombatStance.RetreatOnLowHealth)
                {
                    float retreatThreshold = math.saturate(
                        stance.LowHealthRetreatThreshold > 0f
                            ? stance.LowHealthRetreatThreshold
                            : CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold);
                    if (entityManager.HasComponent<CommanderAuraRecipientComponent>(entity))
                    {
                        retreatThreshold = math.saturate(
                            retreatThreshold - entityManager.GetComponentData<CommanderAuraRecipientComponent>(entity).MoraleBonus);
                    }
                    if (ImminentEngagementPostureUtility.TryGetRetreatPosture(
                            entityManager,
                            entity,
                            out var fortificationPosture))
                    {
                        retreatThreshold = ImminentEngagementPostureUtility.ApplyRetreatThreshold(
                            retreatThreshold,
                            fortificationPosture);
                    }
                    float healthFraction = healthValues[i].Max > 0f
                        ? healthValues[i].Current / healthValues[i].Max
                        : 0f;

                    if (!runtime.IsRetreating && healthFraction < retreatThreshold)
                    {
                        runtime.IsRetreating = true;
                    }
                    else if (runtime.IsRetreating &&
                             healthFraction >= CombatUnitRuntimeDefaults.RetreatResumeHealthFraction)
                    {
                        runtime.IsRetreating = false;
                        runtime.SuspendAutoAcquireUntilMoveStops = false;
                        stance.Stance = CombatStance.PursueInRange;
                    }

                    if (runtime.IsRetreating)
                    {
                        ClearCombatIntent(entityManager, entity, positions[i].Value, ref moveCommand);

                        if (TryFindRetreatDestination(
                                entityManager,
                                factions[i].FactionId,
                                positions[i].Value,
                                combatStats[i].Sight,
                                out float3 retreatDestination))
                        {
                            moveCommand.Destination = retreatDestination;
                            moveCommand.StoppingDistance = RetreatStoppingDistance;
                            moveCommand.IsActive = true;
                        }
                    }
                }
                else
                {
                    runtime.IsRetreating = false;
                }

                if (stance.Stance == CombatStance.HoldPosition)
                {
                    if (moveCommand.IsActive && !HasExplicitAttackOrder(entityManager, entity))
                    {
                        runtime.SuspendAutoAcquireUntilMoveStops = true;
                    }
                    else if (!moveCommand.IsActive)
                    {
                        runtime.SuspendAutoAcquireUntilMoveStops = false;
                    }

                    if (!HasExplicitAttackOrder(entityManager, entity) &&
                        entityManager.HasComponent<AttackTargetComponent>(entity))
                    {
                        var attackTarget = entityManager.GetComponentData<AttackTargetComponent>(entity);
                        float range = math.max(0.1f, combatStats[i].AttackRange);
                        if (!entityManager.Exists(attackTarget.TargetEntity) ||
                            !entityManager.HasComponent<PositionComponent>(attackTarget.TargetEntity) ||
                            math.distancesq(
                                positions[i].Value,
                                entityManager.GetComponentData<PositionComponent>(attackTarget.TargetEntity).Value) >
                            range * range)
                        {
                            entityManager.RemoveComponent<AttackTargetComponent>(entity);
                            moveCommand.Destination = positions[i].Value;
                            moveCommand.StoppingDistance = range;
                            moveCommand.IsActive = false;
                        }
                    }
                }
                else if (!moveCommand.IsActive)
                {
                    runtime.SuspendAutoAcquireUntilMoveStops = false;
                }

                if (stance.Stance == CombatStance.AttackMove &&
                    entityManager.HasComponent<AttackOrderComponent>(entity))
                {
                    var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(entity);
                    if (attackOrder.IsActive && attackOrder.ExplicitTargetEntity == Entity.Null)
                    {
                        attackOrder.IsAttackMoveActive = true;
                        entityManager.SetComponentData(entity, attackOrder);
                    }
                }

                entityManager.SetComponentData(entity, moveCommand);
                entityManager.SetComponentData(entity, stance);
                entityManager.SetComponentData(entity, runtime);
            }
        }

        private static bool HasExplicitAttackOrder(EntityManager entityManager, Entity entity)
        {
            if (!entityManager.HasComponent<AttackOrderComponent>(entity))
            {
                return false;
            }

            var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(entity);
            return attackOrder.IsActive &&
                !attackOrder.IsAttackMoveActive &&
                attackOrder.ExplicitTargetEntity != Entity.Null;
        }

        private static void ClearCombatIntent(
            EntityManager entityManager,
            Entity entity,
            float3 currentPosition,
            ref MoveCommandComponent moveCommand)
        {
            if (entityManager.HasComponent<AttackTargetComponent>(entity))
            {
                entityManager.RemoveComponent<AttackTargetComponent>(entity);
            }

            if (entityManager.HasComponent<AttackOrderComponent>(entity))
            {
                var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(entity);
                attackOrder.ExplicitTargetEntity = Entity.Null;
                attackOrder.AttackMoveDestination = currentPosition;
                attackOrder.IsAttackMoveActive = false;
                attackOrder.IsActive = false;
                entityManager.SetComponentData(entity, attackOrder);
            }

            if (entityManager.HasComponent<GroupMovementOrderComponent>(entity))
            {
                entityManager.RemoveComponent<GroupMovementOrderComponent>(entity);
            }

            moveCommand.Destination = currentPosition;
            moveCommand.StoppingDistance = math.max(0.1f, moveCommand.StoppingDistance);
            moveCommand.IsActive = false;
        }

        private static bool TryFindRetreatDestination(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            float3 currentPosition,
            float sight,
            out float3 retreatDestination)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            float sightRangeSq = math.max(0.1f, sight);
            sightRangeSq *= sightRangeSq;
            float bestDistanceSq = float.MaxValue;
            retreatDestination = currentPosition;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId) ||
                    healthValues[i].Current <= 0f ||
                    entityManager.HasComponent<DeadTag>(entities[i]))
                {
                    continue;
                }

                float distanceSq = math.distancesq(currentPosition, positions[i].Value);
                if (distanceSq > sightRangeSq || distanceSq >= bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                retreatDestination = positions[i].Value;
            }

            return bestDistanceSq < float.MaxValue;
        }
    }
}
