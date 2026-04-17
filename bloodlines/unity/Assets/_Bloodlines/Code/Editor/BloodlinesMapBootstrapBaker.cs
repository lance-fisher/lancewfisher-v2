#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Bloodlines.Authoring;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Bakes a MapDefinition asset into deterministic bootstrap buffers so the runtime
    /// skirmish bootstrap system can create the first ECS battlefield shell without
    /// inventing a second source of gameplay truth.
    /// </summary>
    public sealed class BloodlinesMapBootstrapBaker : Baker<BloodlinesMapBootstrapAuthoring>
    {
        const string BuildingDefinitionsFolder = "Assets/_Bloodlines/Data/BuildingDefinitions";
        const string UnitDefinitionsFolder = "Assets/_Bloodlines/Data/UnitDefinitions";
        const string SettlementDefinitionsFolder = "Assets/_Bloodlines/Data/SettlementClassDefinitions";

        public override void Bake(BloodlinesMapBootstrapAuthoring authoring)
        {
            if (authoring.Map == null)
            {
                UnityDebug.LogWarning("BloodlinesMapBootstrapAuthoring has no MapDefinition assigned.", authoring);
                return;
            }

            DependsOn(authoring.Map);

            var buildingDefinitions = LoadDefinitions(BuildingDefinitionsFolder, (BuildingDefinition definition) => definition.id);
            var unitDefinitions = LoadDefinitions(UnitDefinitionsFolder, (UnitDefinition definition) => definition.id);
            var settlementDefinitions = LoadDefinitions(SettlementDefinitionsFolder, (SettlementClassDefinition definition) => definition.id);
            float combatDistanceScale = math.max(1f, authoring.Map.tileSize > 0 ? authoring.Map.tileSize : 32f);

            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<MapBootstrapPendingTag>(entity);
            AddComponent(entity, new MapBootstrapConfigComponent
            {
                MapId = authoring.Map.id ?? string.Empty,
                MapDisplayName = authoring.Map.displayName ?? string.Empty,
                TileSize = authoring.Map.tileSize,
                Width = authoring.Map.width,
                Height = authoring.Map.height,
                CameraStart = new float2(authoring.Map.cameraStart?.x ?? 0f, authoring.Map.cameraStart?.y ?? 0f),
                SpawnFactions = authoring.SpawnFactions,
                SpawnBuildings = authoring.SpawnBuildings,
                SpawnUnits = authoring.SpawnUnits,
                SpawnResourceNodes = authoring.SpawnResourceNodes,
                SpawnControlPoints = authoring.SpawnControlPoints,
                SpawnSettlements = authoring.SpawnSettlements,
            });

            var factionBuffer = AddBuffer<MapFactionSeedElement>(entity);
            var factionHostilityBuffer = AddBuffer<MapFactionHostilitySeedElement>(entity);
            var buildingBuffer = AddBuffer<MapBuildingSeedElement>(entity);
            var unitBuffer = AddBuffer<MapUnitSeedElement>(entity);
            var unitCombatDefinitionBuffer = AddBuffer<UnitCombatDefinitionElement>(entity);
            var resourceNodeBuffer = AddBuffer<MapResourceNodeSeedElement>(entity);
            var controlPointBuffer = AddBuffer<MapControlPointSeedElement>(entity);
            var settlementBuffer = AddBuffer<MapSettlementSeedElement>(entity);

            foreach (var unitDefinition in authoring.UnitDefinitions ?? Array.Empty<UnitDefinition>())
            {
                if (unitDefinition == null || string.IsNullOrWhiteSpace(unitDefinition.id))
                {
                    continue;
                }

                unitCombatDefinitionBuffer.Add(new UnitCombatDefinitionElement
                {
                    UnitId = unitDefinition.id,
                    AttackDamage = math.max(0f, unitDefinition.attackDamage),
                    AttackRange = NormalizeCombatDistance(unitDefinition.attackRange, combatDistanceScale),
                    AttackCooldown = math.max(0.1f, unitDefinition.attackCooldown),
                    Sight = NormalizeCombatDistance(unitDefinition.sight, combatDistanceScale),
                });
            }

            foreach (var resourceNode in authoring.Map.resourceNodes ?? Array.Empty<ResourceNodeData>())
            {
                resourceNodeBuffer.Add(new MapResourceNodeSeedElement
                {
                    RuntimeId = resourceNode.id ?? string.Empty,
                    ResourceId = resourceNode.type ?? string.Empty,
                    Position = new float3(resourceNode.x, 0f, resourceNode.y),
                    Amount = resourceNode.amount,
                });
            }

            foreach (var controlPoint in authoring.Map.controlPoints ?? Array.Empty<ControlPointData>())
            {
                controlPointBuffer.Add(new MapControlPointSeedElement
                {
                    RuntimeId = controlPoint.id ?? string.Empty,
                    SettlementClassId = controlPoint.settlementClass ?? string.Empty,
                    ContinentId = controlPoint.continentId ?? string.Empty,
                    Position = new float3(controlPoint.x, 0f, controlPoint.y),
                    RadiusTiles = controlPoint.radiusTiles,
                    CaptureTimeSeconds = controlPoint.captureTime,
                    GoldTrickle = controlPoint.resourceTrickle?.gold ?? 0f,
                    FoodTrickle = controlPoint.resourceTrickle?.food ?? 0f,
                    WaterTrickle = controlPoint.resourceTrickle?.water ?? 0f,
                    WoodTrickle = controlPoint.resourceTrickle?.wood ?? 0f,
                    StoneTrickle = controlPoint.resourceTrickle?.stone ?? 0f,
                    IronTrickle = controlPoint.resourceTrickle?.iron ?? 0f,
                    InfluenceTrickle = controlPoint.resourceTrickle?.influence ?? 0f,
                });
            }

            foreach (var settlement in authoring.Map.settlements ?? Array.Empty<SettlementSeedData>())
            {
                var settlementDefinition = RequireDefinition(settlementDefinitions, settlement.settlementClass, "settlement class");
                settlementBuffer.Add(new MapSettlementSeedElement
                {
                    RuntimeId = settlement.id ?? string.Empty,
                    FactionId = settlement.factionId ?? string.Empty,
                    SettlementClassId = settlement.settlementClass ?? string.Empty,
                    AnchorBuildingId = settlement.anchorBuildingId ?? string.Empty,
                    Position = new float3(settlement.x, 0f, settlement.y),
                    FortificationTier = 0,
                    FortificationCeiling = settlementDefinition.defensiveCeiling,
                    IsPrimaryKeep = string.Equals(settlement.settlementClass, "primary_dynastic_keep", StringComparison.OrdinalIgnoreCase),
                });
            }

            foreach (var faction in authoring.Map.factions ?? Array.Empty<FactionSeedData>())
            {
                factionBuffer.Add(new MapFactionSeedElement
                {
                    FactionId = faction.id ?? string.Empty,
                    HouseId = faction.houseId ?? string.Empty,
                    Kind = ResolveFactionKind(faction.kind),
                    Gold = faction.startingResources?.gold ?? 0,
                    Food = faction.startingResources?.food ?? 0,
                    Water = faction.startingResources?.water ?? 0,
                    Wood = faction.startingResources?.wood ?? 0,
                    Stone = faction.startingResources?.stone ?? 0,
                    Iron = faction.startingResources?.iron ?? 0,
                    Influence = faction.startingResources?.influence ?? 0,
                    PopulationTotal = faction.population?.total ?? 0,
                    PopulationCap = faction.population?.cap ?? 0,
                    PopulationReserved = faction.population?.reserved ?? 0,
                });

                foreach (var hostileFactionId in faction.hostileTo ?? Array.Empty<string>())
                {
                    if (string.IsNullOrWhiteSpace(hostileFactionId))
                    {
                        continue;
                    }

                    factionHostilityBuffer.Add(new MapFactionHostilitySeedElement
                    {
                        FactionId = faction.id ?? string.Empty,
                        HostileFactionId = hostileFactionId,
                    });
                }

                foreach (var building in faction.buildings ?? Array.Empty<BuildingSeedData>())
                {
                    var buildingDefinition = RequireDefinition(buildingDefinitions, building.typeId, "building");
                    buildingBuffer.Add(new MapBuildingSeedElement
                    {
                        RuntimeId = building.id ?? string.Empty,
                        FactionId = faction.id ?? string.Empty,
                        TypeId = building.typeId ?? string.Empty,
                        Position = new float3(building.x, 0f, building.y),
                        Completed = building.completed,
                        MaxHealth = buildingDefinition.health,
                        FortificationRole = ResolveFortificationRole(buildingDefinition.fortificationRole),
                        StructuralDamageMultiplier = buildingDefinition.structuralDamageMultiplier <= 0f ? 1f : buildingDefinition.structuralDamageMultiplier,
                        PopulationCapBonus = buildingDefinition.populationCapBonus,
                        BlocksPassage = buildingDefinition.blocksPassage,
                        SupportsSiegePreparation = string.Equals(buildingDefinition.id, "siege_workshop", StringComparison.OrdinalIgnoreCase),
                        SupportsSiegeLogistics = string.Equals(buildingDefinition.id, "supply_camp", StringComparison.OrdinalIgnoreCase),
                    });
                }

                foreach (var unit in faction.units ?? Array.Empty<UnitSeedData>())
                {
                    var unitDefinition = RequireDefinition(unitDefinitions, unit.typeId, "unit");
                    unitBuffer.Add(new MapUnitSeedElement
                    {
                        RuntimeId = unit.id ?? string.Empty,
                        FactionId = faction.id ?? string.Empty,
                        TypeId = unit.typeId ?? string.Empty,
                        Position = new float3(unit.x, 0f, unit.y),
                        MaxHealth = unitDefinition.health,
                        MaxSpeed = unitDefinition.speed,
                        AttackDamage = math.max(0f, unitDefinition.attackDamage),
                        AttackRange = NormalizeCombatDistance(unitDefinition.attackRange, combatDistanceScale),
                        AttackCooldown = math.max(0.1f, unitDefinition.attackCooldown),
                        Sight = NormalizeCombatDistance(unitDefinition.sight, combatDistanceScale),
                        Role = ResolveUnitRole(unitDefinition.role),
                        SiegeClass = ResolveSiegeClass(unitDefinition.siegeClass),
                        PopulationCost = unitDefinition.populationCost,
                        Stage = unitDefinition.stage,
                    });
                }
            }
        }

        static Dictionary<string, TDefinition> LoadDefinitions<TDefinition>(string folderPath, Func<TDefinition, string> keySelector)
            where TDefinition : UnityEngine.Object
        {
            var definitions = new Dictionary<string, TDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(TDefinition).Name}", new[] { folderPath }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<TDefinition>(assetPath);
                if (asset == null)
                {
                    continue;
                }

                var key = keySelector(asset);
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                definitions[key] = asset;
            }

            return definitions;
        }

        static float NormalizeCombatDistance(float rawDistance, float combatDistanceScale)
        {
            return math.max(0.1f, rawDistance / math.max(1f, combatDistanceScale));
        }

        static TDefinition RequireDefinition<TDefinition>(IReadOnlyDictionary<string, TDefinition> definitions, string id, string label)
            where TDefinition : UnityEngine.Object
        {
            if (!string.IsNullOrWhiteSpace(id) && definitions.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new InvalidOperationException($"Bloodlines map bootstrap could not resolve {label} definition '{id}'.");
        }

        static FactionKind ResolveFactionKind(string kind)
        {
            if (string.Equals(kind, "tribe", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "tribes", StringComparison.OrdinalIgnoreCase))
            {
                return FactionKind.Tribes;
            }

            if (string.Equals(kind, "neutral", StringComparison.OrdinalIgnoreCase))
            {
                return FactionKind.Neutral;
            }

            return FactionKind.Kingdom;
        }

        static UnitRole ResolveUnitRole(string role)
        {
            if (string.Equals(role, "worker", StringComparison.OrdinalIgnoreCase)) return UnitRole.Worker;
            if (string.Equals(role, "melee", StringComparison.OrdinalIgnoreCase)) return UnitRole.Melee;
            if (string.Equals(role, "melee-recon", StringComparison.OrdinalIgnoreCase)) return UnitRole.MeleeRecon;
            if (string.Equals(role, "ranged", StringComparison.OrdinalIgnoreCase)) return UnitRole.Ranged;
            if (string.Equals(role, "unique-melee", StringComparison.OrdinalIgnoreCase)) return UnitRole.UniqueMelee;
            if (string.Equals(role, "light-cavalry", StringComparison.OrdinalIgnoreCase)) return UnitRole.LightCavalry;
            if (string.Equals(role, "siege-engine", StringComparison.OrdinalIgnoreCase)) return UnitRole.SiegeEngine;
            if (string.Equals(role, "siege-support", StringComparison.OrdinalIgnoreCase)) return UnitRole.SiegeSupport;
            if (string.Equals(role, "engineer-specialist", StringComparison.OrdinalIgnoreCase)) return UnitRole.EngineerSpecialist;
            if (string.Equals(role, "support", StringComparison.OrdinalIgnoreCase)) return UnitRole.Support;
            return UnitRole.Unknown;
        }

        static SiegeClass ResolveSiegeClass(string siegeClass)
        {
            if (string.Equals(siegeClass, "ram", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ram;
            if (string.Equals(siegeClass, "siege_tower", StringComparison.OrdinalIgnoreCase)) return SiegeClass.SiegeTower;
            if (string.Equals(siegeClass, "trebuchet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Trebuchet;
            if (string.Equals(siegeClass, "ballista", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ballista;
            if (string.Equals(siegeClass, "mantlet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Mantlet;
            return SiegeClass.None;
        }

        static FortificationRole ResolveFortificationRole(string fortificationRole)
        {
            if (string.Equals(fortificationRole, "wall", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Wall;
            if (string.Equals(fortificationRole, "tower", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Tower;
            if (string.Equals(fortificationRole, "gate", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Gate;
            if (string.Equals(fortificationRole, "keep", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Keep;
            return FortificationRole.None;
        }
    }
}
#endif
