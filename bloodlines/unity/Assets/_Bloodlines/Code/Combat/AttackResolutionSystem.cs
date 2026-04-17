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
                    combatRw.ValueRW = combat;
                    ecb.RemoveComponent<AttackTargetComponent>(entity);
                    continue;
                }

                float engagementRange = math.max(0.1f, math.max(attackTarget.EngagementRange, combat.AttackRange));
                float distanceSq = math.distancesq(position.ValueRO.Value, targetPosition.Value);
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
                    targetHealth.Current = math.max(0f, targetHealth.Current - combat.AttackDamage);
                    entityManager.SetComponentData(attackTarget.TargetEntity, targetHealth);
                    combat.CooldownRemaining = math.max(0.1f, combat.AttackCooldown);
                }

                combatRw.ValueRW = combat;
            }
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
