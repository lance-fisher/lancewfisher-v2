using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First combat resolution pass. Units either move into range or apply direct damage on
    /// cooldown to their hostile target.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AutoAcquireTargetSystem))]
    public partial struct AttackResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AttackTargetComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (combatRw, attackTargetRw, position, faction, moveCommandRw, entity) in
                SystemAPI.Query<
                    RefRW<CombatStatsComponent>,
                    RefRW<AttackTargetComponent>,
                    RefRO<PositionComponent>,
                    RefRO<FactionComponent>,
                    RefRW<MoveCommandComponent>>()
                .WithAll<UnitTypeComponent>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                var combat = combatRw.ValueRO;
                combat.CooldownRemaining = math.max(0f, combat.CooldownRemaining - dt);

                var attackTarget = attackTargetRw.ValueRO;
                if (attackTarget.TargetEntity == Entity.Null ||
                    !entityManager.Exists(attackTarget.TargetEntity) ||
                    entityManager.HasComponent<DeadTag>(attackTarget.TargetEntity) ||
                    !entityManager.HasComponent<HealthComponent>(attackTarget.TargetEntity) ||
                    !entityManager.HasComponent<PositionComponent>(attackTarget.TargetEntity) ||
                    !entityManager.HasComponent<FactionComponent>(attackTarget.TargetEntity))
                {
                    combat.TargetOutOfSightSeconds = 0f;
                    combat.AcquireCooldownRemaining = combat.ResolveTargetAcquireIntervalSeconds();
                    var pursuitCommand = moveCommandRw.ValueRO;
                    StopTargetPursuit(position.ValueRO.Value, ref pursuitCommand);
                    moveCommandRw.ValueRW = pursuitCommand;
                    combatRw.ValueRW = combat;
                    ecb.RemoveComponent<AttackTargetComponent>(entity);
                    continue;
                }

                var targetHealth = entityManager.GetComponentData<HealthComponent>(attackTarget.TargetEntity);
                var targetPosition = entityManager.GetComponentData<PositionComponent>(attackTarget.TargetEntity);
                var targetFaction = entityManager.GetComponentData<FactionComponent>(attackTarget.TargetEntity);
                if (targetHealth.Current <= 0f ||
                    !IsFactionHostile(entityManager, faction.ValueRO.FactionId, targetFaction.FactionId))
                {
                    combat.TargetOutOfSightSeconds = 0f;
                    combat.AcquireCooldownRemaining = combat.ResolveTargetAcquireIntervalSeconds();
                    var pursuitCommand = moveCommandRw.ValueRO;
                    StopTargetPursuit(position.ValueRO.Value, ref pursuitCommand);
                    moveCommandRw.ValueRW = pursuitCommand;
                    combatRw.ValueRW = combat;
                    ecb.RemoveComponent<AttackTargetComponent>(entity);
                    continue;
                }

                float distanceSq = math.distancesq(position.ValueRO.Value, targetPosition.Value);
                bool preserveTargetBeyondSight = HasPersistentExplicitTarget(entityManager, entity, attackTarget.TargetEntity);
                float sightRange = math.max(0.1f, combat.Sight);
                if (!preserveTargetBeyondSight && distanceSq > sightRange * sightRange)
                {
                    combat.TargetOutOfSightSeconds += dt;
                    if (combat.TargetOutOfSightSeconds > combat.ResolveTargetSightGraceSeconds())
                    {
                        combat.TargetOutOfSightSeconds = 0f;
                        combat.AcquireCooldownRemaining = combat.ResolveTargetAcquireIntervalSeconds();
                        var pursuitCommand = moveCommandRw.ValueRO;
                        StopTargetPursuit(position.ValueRO.Value, ref pursuitCommand);
                        moveCommandRw.ValueRW = pursuitCommand;
                        combatRw.ValueRW = combat;
                        ecb.RemoveComponent<AttackTargetComponent>(entity);
                        continue;
                    }

                    combatRw.ValueRW = combat;
                    continue;
                }

                combat.TargetOutOfSightSeconds = 0f;
                float engagementRange = math.max(0.1f, math.max(attackTarget.EngagementRange, combat.AttackRange));
                var moveCommand = moveCommandRw.ValueRO;

                if (distanceSq > engagementRange * engagementRange)
                {
                    moveCommand.Destination = targetPosition.Value;
                    moveCommand.StoppingDistance = engagementRange;
                    moveCommand.IsActive = true;
                    moveCommandRw.ValueRW = moveCommand;
                    combatRw.ValueRW = combat;
                    continue;
                }

                moveCommand.Destination = position.ValueRO.Value;
                moveCommand.StoppingDistance = engagementRange;
                moveCommand.IsActive = false;
                moveCommandRw.ValueRW = moveCommand;

                if (combat.AttackDamage > 0f && combat.CooldownRemaining <= 0f)
                {
                    if (entityManager.HasComponent<ProjectileFactoryComponent>(entity))
                    {
                        var projectileFactory = entityManager.GetComponentData<ProjectileFactoryComponent>(entity);
                        SpawnProjectile(
                            ecb,
                            entity,
                            faction.ValueRO.FactionId,
                            attackTarget.TargetEntity,
                            position.ValueRO.Value,
                            targetPosition.Value,
                            combat.AttackDamage,
                            projectileFactory);
                    }
                    else
                    {
                        targetHealth.Current = math.max(0f, targetHealth.Current - combat.AttackDamage);
                        entityManager.SetComponentData(attackTarget.TargetEntity, targetHealth);
                    }

                    combat.CooldownRemaining = math.max(0.1f, combat.AttackCooldown);
                }

                combatRw.ValueRW = combat;
            }
        }

        private static void SpawnProjectile(
            EntityCommandBuffer ecb,
            Entity ownerEntity,
            FixedString32Bytes ownerFactionId,
            Entity targetEntity,
            float3 launchPosition,
            float3 targetPosition,
            float damage,
            in ProjectileFactoryComponent projectileFactory)
        {
            var projectileEntity = ecb.CreateEntity();
            ecb.AddComponent(projectileEntity, new PositionComponent { Value = launchPosition });
            ecb.AddComponent(projectileEntity, new Unity.Transforms.LocalTransform
            {
                Position = launchPosition,
                Rotation = quaternion.identity,
                Scale = 0.18f,
            });
            ecb.AddComponent(projectileEntity, new ProjectileComponent
            {
                OwnerEntity = ownerEntity,
                OwnerFactionId = ownerFactionId,
                TargetEntity = targetEntity,
                TargetPosition = targetPosition,
                LaunchPosition = launchPosition,
                Damage = damage,
                Speed = math.max(0.05f, projectileFactory.ProjectileSpeed),
                MaxLifetimeSeconds = math.max(0.1f, projectileFactory.ProjectileMaxLifetimeSeconds),
                ElapsedSeconds = 0f,
                ArrivalRadius = math.max(0.05f, projectileFactory.ProjectileArrivalRadius),
            });
        }

        private static bool HasPersistentExplicitTarget(
            EntityManager entityManager,
            Entity attackerEntity,
            Entity targetEntity)
        {
            if (!entityManager.HasComponent<AttackOrderComponent>(attackerEntity))
            {
                return false;
            }

            var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(attackerEntity);
            return attackOrder.IsActive &&
                !attackOrder.IsAttackMoveActive &&
                attackOrder.ExplicitTargetEntity == targetEntity;
        }

        private static void StopTargetPursuit(float3 currentPosition, ref MoveCommandComponent moveCommand)
        {
            moveCommand.Destination = currentPosition;
            moveCommand.StoppingDistance = math.max(0.1f, moveCommand.StoppingDistance);
            moveCommand.IsActive = false;
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
