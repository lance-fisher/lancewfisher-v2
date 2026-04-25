using Bloodlines.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Combat
{
    /// <summary>
    /// Applies live commander aura bonuses to nearby friendly units, then removes the prior
    /// frame's aura deltas before re-evaluating proximity. This keeps movement and combat
    /// systems consuming already-buffed runtime stats without permanently mutating unit baselines.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Bloodlines.Systems.CombatStanceResolutionSystem))]
    [UpdateBefore(typeof(Bloodlines.Systems.AutoAcquireTargetSystem))]
    [UpdateBefore(typeof(Bloodlines.Pathing.UnitMovementSystem))]
    public partial struct CommanderAuraSystem : ISystem
    {
        private struct FactionRuntimeRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public ConvictionBand Band;
            public CovenantId SelectedFaith;
            public DoctrinePath DoctrinePath;
        }

        private struct CommanderRuntimeRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public CommanderAuraComponent Aura;
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CommanderComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            try
            {
                RestoreRecipients(entityManager, ref ecb);

                using var factions = CollectFactions(entityManager);
                using var commanders = CollectCommanders(entityManager, factions, ref ecb);
                ApplyAuras(entityManager, commanders, ref ecb);

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static void RestoreRecipients(EntityManager entityManager, ref EntityCommandBuffer ecb)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CommanderAuraRecipientComponent>(),
                ComponentType.ReadWrite<CombatStatsComponent>(),
                ComponentType.ReadWrite<MovementStatsComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var recipients = query.ToComponentDataArray<CommanderAuraRecipientComponent>(Allocator.Temp);
            using var combatStats = query.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);
            using var movementStats = query.ToComponentDataArray<MovementStatsComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var combat = combatStats[i];
                var movement = movementStats[i];
                var recipient = recipients[i];

                if (recipient.AppliedAttackMultiplier > 0.0001f)
                {
                    combat.AttackDamage /= recipient.AppliedAttackMultiplier;
                }

                combat.Sight = math.max(0f, combat.Sight - recipient.AppliedSightBonus);
                movement.MaxSpeed = math.max(0f, movement.MaxSpeed - recipient.AppliedSpeedDelta);

                entityManager.SetComponentData(entities[i], combat);
                entityManager.SetComponentData(entities[i], movement);
                ecb.RemoveComponent<CommanderAuraRecipientComponent>(entities[i]);
            }
        }

        private static NativeList<FactionRuntimeRecord> CollectFactions(EntityManager entityManager)
        {
            var factions = new NativeList<FactionRuntimeRecord>(Allocator.Temp);
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factionComponents = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var band = entityManager.HasComponent<ConvictionComponent>(entities[i])
                    ? entityManager.GetComponentData<ConvictionComponent>(entities[i]).Band
                    : ConvictionBand.Neutral;
                var faith = entityManager.HasComponent<FaithStateComponent>(entities[i])
                    ? entityManager.GetComponentData<FaithStateComponent>(entities[i])
                    : default;

                factions.Add(new FactionRuntimeRecord
                {
                    Entity = entities[i],
                    FactionId = factionComponents[i].FactionId,
                    Band = band,
                    SelectedFaith = faith.SelectedFaith,
                    DoctrinePath = faith.DoctrinePath,
                });
            }

            return factions;
        }

        private static NativeList<CommanderRuntimeRecord> CollectCommanders(
            EntityManager entityManager,
            NativeList<FactionRuntimeRecord> factions,
            ref EntityCommandBuffer ecb)
        {
            var commanders = new NativeList<CommanderRuntimeRecord>(Allocator.Temp);
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CommanderComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factionComponents = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var commanderComponents = query.ToComponentDataArray<CommanderComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (entityManager.HasComponent<DeadTag>(entities[i]) || healthValues[i].Current <= 0f)
                {
                    if (entityManager.HasComponent<CommanderAuraComponent>(entities[i]))
                    {
                        ecb.RemoveComponent<CommanderAuraComponent>(entities[i]);
                    }

                    continue;
                }

                FactionRuntimeRecord faction = FindFaction(factions, factionComponents[i].FactionId);
                var doctrine = CommanderAuraCanon.ResolveDoctrineProfile(faction.SelectedFaith, faction.DoctrinePath);
                CommanderAuraComponent aura =
                    CommanderAuraCanon.ResolveAura(commanderComponents[i].Renown, doctrine, faction.Band);

                if (entityManager.HasComponent<CommanderAuraComponent>(entities[i]))
                {
                    entityManager.SetComponentData(entities[i], aura);
                }
                else
                {
                    ecb.AddComponent(entities[i], aura);
                }

                commanders.Add(new CommanderRuntimeRecord
                {
                    Entity = entities[i],
                    FactionId = factionComponents[i].FactionId,
                    Position = positions[i].Value,
                    Aura = aura,
                });
            }

            return commanders;
        }

        private static void ApplyAuras(
            EntityManager entityManager,
            NativeList<CommanderRuntimeRecord> commanders,
            ref EntityCommandBuffer ecb)
        {
            if (commanders.Length == 0)
            {
                return;
            }

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadWrite<CombatStatsComponent>(),
                ComponentType.ReadWrite<MovementStatsComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healthValues = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var combatStats = unitQuery.ToComponentDataArray<CombatStatsComponent>(Allocator.Temp);
            using var movementStats = unitQuery.ToComponentDataArray<MovementStatsComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (entityManager.HasComponent<DeadTag>(entities[i]) || healthValues[i].Current <= 0f)
                {
                    continue;
                }

                bool foundAura = false;
                CommanderRuntimeRecord strongestAura = default;
                for (int j = 0; j < commanders.Length; j++)
                {
                    if (!factions[i].FactionId.Equals(commanders[j].FactionId))
                    {
                        continue;
                    }

                    float distance = math.distance(positions[i].Value.xz, commanders[j].Position.xz);
                    if (distance > commanders[j].Aura.AuraRadius)
                    {
                        continue;
                    }

                    if (!foundAura || IsStronger(commanders[j].Aura, strongestAura.Aura))
                    {
                        strongestAura = commanders[j];
                        foundAura = true;
                    }
                }

                if (!foundAura)
                {
                    continue;
                }

                var combat = combatStats[i];
                var movement = movementStats[i];
                float attackMultiplier = math.max(1f, 1f + strongestAura.Aura.AttackBonus);
                float speedDelta = math.max(0f, movement.MaxSpeed * strongestAura.Aura.SpeedBonus);

                combat.AttackDamage *= attackMultiplier;
                combat.Sight = math.max(0.1f, combat.Sight + strongestAura.Aura.SightBonus);
                movement.MaxSpeed += speedDelta;

                entityManager.SetComponentData(entities[i], combat);
                entityManager.SetComponentData(entities[i], movement);

                var recipient = new CommanderAuraRecipientComponent
                {
                    SourceCommander = strongestAura.Entity,
                    AppliedAttackMultiplier = attackMultiplier,
                    AppliedSightBonus = strongestAura.Aura.SightBonus,
                    AppliedSpeedDelta = speedDelta,
                    MoraleBonus = strongestAura.Aura.MoraleBonus,
                };

                if (entityManager.HasComponent<CommanderAuraRecipientComponent>(entities[i]))
                {
                    entityManager.SetComponentData(entities[i], recipient);
                }
                else
                {
                    ecb.AddComponent(entities[i], recipient);
                }
            }
        }

        private static FactionRuntimeRecord FindFaction(
            NativeList<FactionRuntimeRecord> factions,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return factions[i];
                }
            }

            return new FactionRuntimeRecord
            {
                Entity = Entity.Null,
                FactionId = factionId,
                Band = ConvictionBand.Neutral,
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
            };
        }

        private static bool IsStronger(
            CommanderAuraComponent candidate,
            CommanderAuraComponent incumbent)
        {
            float candidateScore = candidate.AttackBonus + candidate.SightBonus * 0.001f + candidate.SpeedBonus + candidate.MoraleBonus;
            float incumbentScore = incumbent.AttackBonus + incumbent.SightBonus * 0.001f + incumbent.SpeedBonus + incumbent.MoraleBonus;
            return candidateScore > incumbentScore;
        }
    }
}
