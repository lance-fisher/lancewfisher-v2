using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First combat acquisition pass. Combat-capable units without an active target scan for
    /// the nearest hostile unit within sight and lock onto it.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AutoAcquireTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CombatStatsComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<CombatStatsComponent>());

            using var unitEntities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var unitHealth = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var unitCombatStats = unitQuery.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);

            for (int i = 0; i < unitEntities.Length; i++)
            {
                var attackerEntity = unitEntities[i];
                var attackerCombat = unitCombatStats[i];
                if (entityManager.HasComponent<DeadTag>(attackerEntity) ||
                    entityManager.HasComponent<AttackTargetComponent>(attackerEntity) ||
                    attackerHealthInvalid(unitHealth[i], attackerCombat))
                {
                    continue;
                }

                Entity nearestTarget = Entity.Null;
                float nearestDistanceSq = attackerCombat.Sight * attackerCombat.Sight;

                if (TryResolveExplicitAttackOrderTarget(
                        entityManager,
                        attackerEntity,
                        unitFactions[i].FactionId,
                        unitPositions[i].Value,
                        attackerCombat.Sight,
                        out var explicitTarget,
                        out var explicitDistanceSq))
                {
                    nearestTarget = explicitTarget;
                    nearestDistanceSq = explicitDistanceSq;
                }

                for (int j = 0; j < unitEntities.Length; j++)
                {
                    var targetEntity = unitEntities[j];
                    if (attackerEntity == targetEntity ||
                        entityManager.HasComponent<DeadTag>(targetEntity) ||
                        unitHealth[j].Current <= 0f ||
                        !IsFactionHostile(entityManager, unitFactions[i].FactionId, unitFactions[j].FactionId))
                    {
                        continue;
                    }

                    if (nearestTarget != Entity.Null && targetEntity == nearestTarget)
                    {
                        continue;
                    }

                    float distanceSq = math.distancesq(unitPositions[i].Value, unitPositions[j].Value);
                    if (distanceSq > nearestDistanceSq)
                    {
                        continue;
                    }

                    nearestDistanceSq = distanceSq;
                    nearestTarget = targetEntity;
                }

                if (nearestTarget == Entity.Null)
                {
                    continue;
                }

                ecb.AddComponent(attackerEntity, new AttackTargetComponent
                {
                    TargetEntity = nearestTarget,
                    EngagementRange = math.max(0.1f, attackerCombat.AttackRange),
                });
            }
        }

        private static bool TryResolveExplicitAttackOrderTarget(
            EntityManager entityManager,
            Entity attackerEntity,
            FixedString32Bytes attackerFactionId,
            float3 attackerPosition,
            float sight,
            out Entity explicitTarget,
            out float distanceSq)
        {
            explicitTarget = Entity.Null;
            distanceSq = 0f;

            if (!entityManager.HasComponent<AttackOrderComponent>(attackerEntity))
            {
                return false;
            }

            var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(attackerEntity);
            if (!attackOrder.IsActive || attackOrder.ExplicitTargetEntity == Entity.Null)
            {
                return false;
            }

            var targetEntity = attackOrder.ExplicitTargetEntity;
            if (!entityManager.Exists(targetEntity) ||
                entityManager.HasComponent<DeadTag>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity) ||
                !entityManager.HasComponent<PositionComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity))
            {
                return false;
            }

            var targetHealth = entityManager.GetComponentData<HealthComponent>(targetEntity);
            var targetFaction = entityManager.GetComponentData<FactionComponent>(targetEntity);
            if (targetHealth.Current <= 0f ||
                !IsFactionHostile(entityManager, attackerFactionId, targetFaction.FactionId))
            {
                return false;
            }

            distanceSq = math.distancesq(attackerPosition, entityManager.GetComponentData<PositionComponent>(targetEntity).Value);
            float sightSq = math.max(0.1f, sight) * math.max(0.1f, sight);
            if (distanceSq > sightSq)
            {
                return false;
            }

            explicitTarget = targetEntity;
            return true;
        }

        private static bool attackerHealthInvalid(in HealthComponent health, in CombatStatsComponent combat)
        {
            return health.Current <= 0f || combat.AttackDamage <= 0f || combat.AttackRange <= 0f || combat.Sight <= 0f;
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
