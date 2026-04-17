using System;
using System.Collections.Generic;
using System.Reflection;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Converts baked map seed buffers into the first live ECS skirmish shell.
    /// This is the bridge between the JSON-synced MapDefinition asset and the runtime
    /// entity world the next scene/bootstrap pass will exercise.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(BloodlinesBootstrapSystem))]
    public partial struct SkirmishBootstrapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapBootstrapPendingTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var query = SystemAPI.QueryBuilder()
                .WithAll<MapBootstrapPendingTag, MapBootstrapConfigComponent>()
                .Build();

            using var bootstrapEntities = query.ToEntityArray(Allocator.Temp);
            foreach (var bootstrapEntity in bootstrapEntities)
            {
                var config = entityManager.GetComponentData<MapBootstrapConfigComponent>(bootstrapEntity);
                var factionSeeds = default(NativeArray<MapFactionSeedElement>);
                var factionHostilitySeeds = default(NativeArray<MapFactionHostilitySeedElement>);
                var settlementSeeds = default(NativeArray<MapSettlementSeedElement>);
                var controlPointSeeds = default(NativeArray<MapControlPointSeedElement>);
                var resourceNodeSeeds = default(NativeArray<MapResourceNodeSeedElement>);
                var buildingSeeds = default(NativeArray<MapBuildingSeedElement>);
                var unitSeeds = default(NativeArray<MapUnitSeedElement>);

                if (config.SpawnFactions)
                {
                    factionSeeds = entityManager.GetBuffer<MapFactionSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                    factionHostilitySeeds = entityManager.GetBuffer<MapFactionHostilitySeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                if (config.SpawnSettlements)
                {
                    settlementSeeds = entityManager.GetBuffer<MapSettlementSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                if (config.SpawnControlPoints)
                {
                    controlPointSeeds = entityManager.GetBuffer<MapControlPointSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                if (config.SpawnResourceNodes)
                {
                    resourceNodeSeeds = entityManager.GetBuffer<MapResourceNodeSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                if (config.SpawnBuildings)
                {
                    buildingSeeds = entityManager.GetBuffer<MapBuildingSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                if (config.SpawnUnits)
                {
                    unitSeeds = entityManager.GetBuffer<MapUnitSeedElement>(bootstrapEntity).ToNativeArray(Allocator.Temp);
                }

                try
                {
                    var factionEntitiesById = new Dictionary<string, Entity>(System.StringComparer.OrdinalIgnoreCase);

                    if (config.SpawnFactions)
                    {
                        for (var i = 0; i < factionSeeds.Length; i++)
                        {
                            var factionEntity = SpawnFactionEntity(entityManager, factionSeeds[i]);
                            factionEntitiesById[factionSeeds[i].FactionId.ToString()] = factionEntity;
                        }

                        for (var i = 0; i < factionHostilitySeeds.Length; i++)
                        {
                            var seed = factionHostilitySeeds[i];
                            if (!factionEntitiesById.TryGetValue(seed.FactionId.ToString(), out var factionEntity))
                            {
                                continue;
                            }

                            if (!entityManager.HasBuffer<HostilityComponent>(factionEntity))
                            {
                                entityManager.AddBuffer<HostilityComponent>(factionEntity);
                            }

                            var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntity);
                            hostilityBuffer.Add(new HostilityComponent
                            {
                                HostileFactionId = seed.HostileFactionId,
                            });
                        }
                    }

                    if (config.SpawnSettlements)
                    {
                        for (var i = 0; i < settlementSeeds.Length; i++)
                        {
                            SpawnSettlementEntity(entityManager, settlementSeeds[i]);
                        }
                    }

                    if (config.SpawnControlPoints)
                    {
                        for (var i = 0; i < controlPointSeeds.Length; i++)
                        {
                            SpawnControlPointEntity(entityManager, controlPointSeeds[i]);
                        }
                    }

                    if (config.SpawnResourceNodes)
                    {
                        for (var i = 0; i < resourceNodeSeeds.Length; i++)
                        {
                            SpawnResourceNodeEntity(entityManager, resourceNodeSeeds[i]);
                        }
                    }

                    if (config.SpawnBuildings)
                    {
                        for (var i = 0; i < buildingSeeds.Length; i++)
                        {
                            SpawnBuildingEntity(entityManager, buildingSeeds[i]);
                        }
                    }

                    if (config.SpawnUnits)
                    {
                        for (var i = 0; i < unitSeeds.Length; i++)
                        {
                            SpawnUnitEntity(entityManager, unitSeeds[i]);
                        }
                    }

                    entityManager.RemoveComponent<MapBootstrapPendingTag>(bootstrapEntity);
                }
                finally
                {
                    if (factionSeeds.IsCreated) factionSeeds.Dispose();
                    if (factionHostilitySeeds.IsCreated) factionHostilitySeeds.Dispose();
                    if (settlementSeeds.IsCreated) settlementSeeds.Dispose();
                    if (controlPointSeeds.IsCreated) controlPointSeeds.Dispose();
                    if (resourceNodeSeeds.IsCreated) resourceNodeSeeds.Dispose();
                    if (buildingSeeds.IsCreated) buildingSeeds.Dispose();
                    if (unitSeeds.IsCreated) unitSeeds.Dispose();
                }
            }
        }

        static Entity SpawnFactionEntity(EntityManager entityManager, MapFactionSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = seed.FactionId });
            entityManager.AddComponentData(entity, new FactionHouseComponent { HouseId = seed.HouseId });
            entityManager.AddComponentData(entity, new FactionKindComponent { Kind = seed.Kind });
            entityManager.AddComponentData(entity, new PopulationComponent
            {
                Total = seed.PopulationTotal,
                Cap = seed.PopulationCap,
                BaseCap = seed.PopulationCap,
                CapBonus = 0,
                Available = math.max(0, seed.PopulationTotal - seed.PopulationReserved),
                GrowthAccumulator = 0f,
            });
            entityManager.AddComponentData(entity, new ResourceStockpileComponent
            {
                Gold = seed.Gold,
                Food = seed.Food,
                Water = seed.Water,
                Wood = seed.Wood,
                Stone = seed.Stone,
                Iron = seed.Iron,
                Influence = seed.Influence,
            });
            entityManager.AddComponentData(entity, new RealmConditionComponent());
            entityManager.AddComponentData(entity, new FactionLoyaltyComponent
            {
                Current = 70f,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.AddComponentData(entity, new ConvictionComponent { Band = ConvictionBand.Neutral });
            entityManager.AddComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = 0f,
                Level = 0,
            });
            entityManager.AddBuffer<FaithExposureElement>(entity);
            entityManager.AddBuffer<HostilityComponent>(entity);

            if (ShouldAttachAIEconomyController(seed))
            {
                TryAttachAIEconomyController(entityManager, entity);
            }

            return entity;
        }

        static bool ShouldAttachAIEconomyController(MapFactionSeedElement seed)
        {
            if (seed.Kind == FactionKind.Neutral)
            {
                return false;
            }

            string factionId = seed.FactionId.ToString();
            if (string.IsNullOrEmpty(factionId) ||
                string.Equals(factionId, "player", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(factionId, "enemy", System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        static void TryAttachAIEconomyController(EntityManager entityManager, Entity entity)
        {
            var controllerType = Type.GetType("Bloodlines.AI.AIEconomyControllerComponent, Assembly-CSharp");
            if (controllerType == null)
            {
                return;
            }

            var addComponentDataMethod = ResolveAddComponentDataMethod(controllerType);
            if (addComponentDataMethod == null)
            {
                return;
            }

            var controller = Activator.CreateInstance(controllerType);
            if (controller == null)
            {
                return;
            }

            SetPublicField(controllerType, controller, "Enabled", false);
            SetPublicField(controllerType, controller, "PrimaryGatherResourceId", new FixedString32Bytes("gold"));
            SetPublicField(controllerType, controller, "TargetWorkerCount", 6);
            SetPublicField(controllerType, controller, "TargetMilitiaCount", 4);
            SetPublicField(controllerType, controller, "GatherAssignmentAccumulator", 0f);
            SetPublicField(controllerType, controller, "GatherAssignmentIntervalSeconds", 2.5f);
            SetPublicField(controllerType, controller, "ProductionAccumulator", 0f);
            SetPublicField(controllerType, controller, "ProductionIntervalSeconds", 2.5f);
            SetPublicField(controllerType, controller, "ConstructionAccumulator", 0f);
            SetPublicField(controllerType, controller, "ConstructionIntervalSeconds", 5f);
            SetPublicField(controllerType, controller, "TargetDwellingCount", 2);
            SetPublicField(controllerType, controller, "TargetFarmCount", 2);
            SetPublicField(controllerType, controller, "TargetWellCount", 1);

            addComponentDataMethod.Invoke(entityManager, new[] { (object)entity, controller });
        }

        static MethodInfo ResolveAddComponentDataMethod(Type componentType)
        {
            foreach (var method in typeof(EntityManager).GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!method.IsGenericMethodDefinition || !string.Equals(method.Name, "AddComponentData", StringComparison.Ordinal))
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (parameters.Length != 2 || parameters[0].ParameterType != typeof(Entity))
                {
                    continue;
                }

                return method.MakeGenericMethod(componentType);
            }

            return null;
        }

        static void SetPublicField(Type componentType, object component, string fieldName, object value)
        {
            var field = componentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(component, value);
            }
        }

        static void SpawnSettlementEntity(EntityManager entityManager, MapSettlementSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = seed.FactionId });
            entityManager.AddComponentData(entity, new PositionComponent { Value = seed.Position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = seed.Position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new SettlementComponent
            {
                SettlementId = seed.RuntimeId,
                SettlementClassId = seed.SettlementClassId,
                FortificationTier = seed.FortificationTier,
                FortificationCeiling = seed.FortificationCeiling,
            });

            if (seed.IsPrimaryKeep)
            {
                entityManager.AddComponent<PrimaryKeepTag>(entity);
            }
        }

        static void SpawnControlPointEntity(EntityManager entityManager, MapControlPointSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new PositionComponent { Value = seed.Position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = seed.Position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new ControlPointComponent
            {
                ControlPointId = seed.RuntimeId,
                OwnerFactionId = default,
                CaptureFactionId = default,
                ContinentId = seed.ContinentId,
                ControlState = ControlState.Neutral,
                IsContested = false,
                Loyalty = 0f,
                CaptureProgress = 0f,
                SettlementClassId = seed.SettlementClassId,
                FortificationTier = 0,
                RadiusTiles = seed.RadiusTiles,
                CaptureTimeSeconds = seed.CaptureTimeSeconds,
                GoldTrickle = seed.GoldTrickle,
                FoodTrickle = seed.FoodTrickle,
                WaterTrickle = seed.WaterTrickle,
                WoodTrickle = seed.WoodTrickle,
                StoneTrickle = seed.StoneTrickle,
                IronTrickle = seed.IronTrickle,
                InfluenceTrickle = seed.InfluenceTrickle,
            });
        }

        static void SpawnResourceNodeEntity(EntityManager entityManager, MapResourceNodeSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new PositionComponent { Value = seed.Position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = seed.Position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new ResourceNodeComponent
            {
                ResourceId = seed.ResourceId,
                Amount = seed.Amount,
                InitialAmount = seed.Amount,
            });
        }

        static void SpawnBuildingEntity(EntityManager entityManager, MapBuildingSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = seed.FactionId });
            entityManager.AddComponentData(entity, new PositionComponent { Value = seed.Position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = seed.Position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = seed.MaxHealth,
                Max = seed.MaxHealth,
            });
            entityManager.AddComponentData(entity, new BuildingTypeComponent
            {
                TypeId = seed.TypeId,
                FortificationRole = seed.FortificationRole,
                StructuralDamageMultiplier = seed.StructuralDamageMultiplier,
                PopulationCapBonus = seed.PopulationCapBonus,
                BlocksPassage = seed.BlocksPassage,
                SupportsSiegePreparation = seed.SupportsSiegePreparation,
                SupportsSiegeLogistics = seed.SupportsSiegeLogistics,
            });
            entityManager.AddComponentData(entity, new ProductionFacilityComponent
            {
                SpawnSequence = 0,
            });
            entityManager.AddBuffer<ProductionQueueItemElement>(entity);

            if (seed.GoldTrickle > 0f ||
                seed.FoodTrickle > 0f ||
                seed.WaterTrickle > 0f ||
                seed.WoodTrickle > 0f ||
                seed.StoneTrickle > 0f ||
                seed.IronTrickle > 0f ||
                seed.InfluenceTrickle > 0f)
            {
                entityManager.AddComponentData(entity, new ResourceTrickleBuildingComponent
                {
                    GoldPerSecond = seed.GoldTrickle,
                    FoodPerSecond = seed.FoodTrickle,
                    WaterPerSecond = seed.WaterTrickle,
                    WoodPerSecond = seed.WoodTrickle,
                    StonePerSecond = seed.StoneTrickle,
                    IronPerSecond = seed.IronTrickle,
                    InfluencePerSecond = seed.InfluenceTrickle,
                });
            }
        }

        static void SpawnUnitEntity(EntityManager entityManager, MapUnitSeedElement seed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = seed.FactionId });
            entityManager.AddComponentData(entity, new PositionComponent { Value = seed.Position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = seed.Position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = seed.MaxHealth,
                Max = seed.MaxHealth,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = seed.TypeId,
                Role = seed.Role,
                SiegeClass = seed.SiegeClass,
                PopulationCost = seed.PopulationCost,
                Stage = seed.Stage,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent
            {
                MaxSpeed = seed.MaxSpeed,
            });
            entityManager.AddComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = seed.AttackDamage,
                AttackRange = seed.AttackRange,
                AttackCooldown = seed.AttackCooldown,
                Sight = seed.Sight,
                CooldownRemaining = 0f,
            });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = seed.Position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });

            if (seed.ProjectileSpeed > 0f)
            {
                entityManager.AddComponentData(entity, new ProjectileFactoryComponent
                {
                    ProjectileSpeed = seed.ProjectileSpeed,
                    ProjectileMaxLifetimeSeconds = math.max(0.1f, seed.ProjectileMaxLifetimeSeconds),
                    ProjectileArrivalRadius = math.max(0.05f, seed.ProjectileArrivalRadius),
                });
            }
        }
    }
}
