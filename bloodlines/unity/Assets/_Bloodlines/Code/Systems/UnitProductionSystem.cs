using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First ECS unit-production queue progression. Queue items are authored by the debug
    /// command shell using canonical definition data, then resolved into live ECS units here.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct UnitProductionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProductionFacilityComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var combatDefinitionsEntity = FindCombatDefinitionsEntity(entityManager);

            foreach (var (facilityRw, buildingPosition, buildingFaction, buildingHealth, buildingEntity) in
                SystemAPI.Query<
                    RefRW<ProductionFacilityComponent>,
                    RefRO<PositionComponent>,
                    RefRO<FactionComponent>,
                    RefRO<HealthComponent>>()
                .WithAll<BuildingTypeComponent>()
                .WithNone<ConstructionStateComponent>()
                .WithEntityAccess())
            {
                var queue = SystemAPI.GetBuffer<ProductionQueueItemElement>(buildingEntity);
                if (buildingHealth.ValueRO.Current <= 0f || queue.Length == 0)
                {
                    continue;
                }

                var queueItem = queue[0];
                queueItem.RemainingSeconds -= dt;
                if (queueItem.RemainingSeconds > 0f)
                {
                    queue[0] = queueItem;
                    continue;
                }

                if (!TryFindFactionPopulation(entityManager, buildingFaction.ValueRO.FactionId, out var factionPopulation, out var factionPopulationEntity))
                {
                    queue[0] = queueItem;
                    continue;
                }

                SpawnQueuedUnit(
                    entityManager,
                    ecb,
                    combatDefinitionsEntity,
                    buildingPosition.ValueRO.Value,
                    buildingFaction.ValueRO.FactionId,
                    facilityRw.ValueRO.SpawnSequence,
                    queueItem);

                facilityRw.ValueRW = new ProductionFacilityComponent
                {
                    SpawnSequence = facilityRw.ValueRO.SpawnSequence + 1,
                };

                queue.RemoveAt(0);

                factionPopulation.Available = math.max(0, factionPopulation.Available);
                entityManager.SetComponentData(factionPopulationEntity, factionPopulation);
            }
        }

        private static bool TryFindFactionPopulation(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out PopulationComponent population,
            out Entity populationEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(PopulationComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                population = populations[i];
                populationEntity = entities[i];
                return true;
            }

            population = default;
            populationEntity = Entity.Null;
            return false;
        }

        private static void SpawnQueuedUnit(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            Entity combatDefinitionsEntity,
            float3 buildingPosition,
            FixedString32Bytes factionId,
            int spawnSequence,
            in ProductionQueueItemElement queueItem)
        {
            var entity = ecb.CreateEntity();
            float3 spawnPosition = ResolveSpawnPosition(buildingPosition, spawnSequence);

            ecb.AddComponent(entity, new FactionComponent { FactionId = factionId });
            ecb.AddComponent(entity, new PositionComponent { Value = spawnPosition });
            ecb.AddComponent(entity, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            ecb.AddComponent(entity, new HealthComponent
            {
                Current = queueItem.MaxHealth,
                Max = queueItem.MaxHealth,
            });
            ecb.AddComponent(entity, new UnitTypeComponent
            {
                TypeId = queueItem.UnitId,
                Role = queueItem.Role,
                SiegeClass = queueItem.SiegeClass,
                PopulationCost = queueItem.PopulationCost,
                Stage = queueItem.Stage,
            });
            ecb.AddComponent(entity, new MovementStatsComponent
            {
                MaxSpeed = queueItem.MaxSpeed,
            });
            ecb.AddComponent(entity, ResolveCombatStats(entityManager, combatDefinitionsEntity, queueItem.UnitId));
            ecb.AddComponent(entity, new MoveCommandComponent
            {
                Destination = spawnPosition,
                StoppingDistance = 0.2f,
                IsActive = false,
            });

            if (TryResolveProjectileFactory(entityManager, combatDefinitionsEntity, queueItem.UnitId, out var projectileFactory))
            {
                ecb.AddComponent(entity, projectileFactory);
            }
        }

        private static Entity FindCombatDefinitionsEntity(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MapBootstrapConfigComponent>(),
                ComponentType.ReadOnly<UnitCombatDefinitionElement>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            return entities.Length > 0 ? entities[0] : Entity.Null;
        }

        private static CombatStatsComponent ResolveCombatStats(
            EntityManager entityManager,
            Entity combatDefinitionsEntity,
            FixedString64Bytes unitId)
        {
            if (combatDefinitionsEntity != Entity.Null &&
                entityManager.Exists(combatDefinitionsEntity) &&
                entityManager.HasBuffer<UnitCombatDefinitionElement>(combatDefinitionsEntity))
            {
                var definitions = entityManager.GetBuffer<UnitCombatDefinitionElement>(combatDefinitionsEntity);
                for (int i = 0; i < definitions.Length; i++)
                {
                    var definition = definitions[i];
                    if (!definition.UnitId.Equals(unitId))
                    {
                        continue;
                    }

                    return new CombatStatsComponent
                    {
                        AttackDamage = definition.AttackDamage,
                        AttackRange = definition.AttackRange,
                        AttackCooldown = definition.AttackCooldown,
                        Sight = definition.Sight,
                        CooldownRemaining = 0f,
                    };
                }
            }

            return default;
        }

        private static bool TryResolveProjectileFactory(
            EntityManager entityManager,
            Entity combatDefinitionsEntity,
            FixedString64Bytes unitId,
            out ProjectileFactoryComponent projectileFactory)
        {
            if (combatDefinitionsEntity != Entity.Null &&
                entityManager.Exists(combatDefinitionsEntity) &&
                entityManager.HasBuffer<UnitCombatDefinitionElement>(combatDefinitionsEntity))
            {
                var definitions = entityManager.GetBuffer<UnitCombatDefinitionElement>(combatDefinitionsEntity);
                for (int i = 0; i < definitions.Length; i++)
                {
                    var definition = definitions[i];
                    if (!definition.UnitId.Equals(unitId) || definition.ProjectileSpeed <= 0f)
                    {
                        continue;
                    }

                    projectileFactory = new ProjectileFactoryComponent
                    {
                        ProjectileSpeed = definition.ProjectileSpeed,
                        ProjectileMaxLifetimeSeconds = math.max(0.1f, definition.ProjectileMaxLifetimeSeconds),
                        ProjectileArrivalRadius = math.max(0.05f, definition.ProjectileArrivalRadius),
                    };
                    return true;
                }
            }

            projectileFactory = default;
            return false;
        }

        private static float3 ResolveSpawnPosition(float3 buildingPosition, int spawnSequence)
        {
            int slot = math.abs(spawnSequence) % 8;
            int ring = math.abs(spawnSequence) / 8;
            float angleRadians = math.radians(slot * 45f);
            float radius = 2.4f + (ring * 0.9f);

            return buildingPosition + new float3(
                math.cos(angleRadians) * radius,
                0f,
                math.sin(angleRadians) * radius);
        }
    }
}
