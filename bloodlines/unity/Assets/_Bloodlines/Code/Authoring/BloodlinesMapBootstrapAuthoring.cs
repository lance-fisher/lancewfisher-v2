using System;
using System.Collections.Generic;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodlines.Authoring
{
    /// <summary>
    /// Scene-facing authoring anchor for the first Bloodlines skirmish bootstrap pass.
    /// Attach this to a GameObject in the Bootstrap scene and assign the canonical
    /// MapDefinition asset to bake the Ironmark Frontier seed into ECS buffers.
    /// </summary>
    public sealed class BloodlinesMapBootstrapAuthoring : MonoBehaviour
    {
        private const string BuildingDefinitionsFolder = "Assets/_Bloodlines/Data/BuildingDefinitions";
        private const string UnitDefinitionsFolder = "Assets/_Bloodlines/Data/UnitDefinitions";
        private const string SettlementDefinitionsFolder = "Assets/_Bloodlines/Data/SettlementClassDefinitions";

        [SerializeField] private MapDefinition map = null;
        [SerializeField] private bool spawnFactions = true;
        [SerializeField] private bool spawnBuildings = true;
        [SerializeField] private bool spawnUnits = true;
        [SerializeField] private bool spawnResourceNodes = true;
        [SerializeField] private bool spawnControlPoints = true;
        [SerializeField] private bool spawnSettlements = true;
        [SerializeField, HideInInspector] private BuildingDefinition[] buildingDefinitions = Array.Empty<BuildingDefinition>();
        [SerializeField, HideInInspector] private UnitDefinition[] unitDefinitions = Array.Empty<UnitDefinition>();
        [SerializeField, HideInInspector] private SettlementClassDefinition[] settlementDefinitions = Array.Empty<SettlementClassDefinition>();

        private bool runtimeBootstrapInjected;
#if UNITY_EDITOR
        private bool runtimeBootstrapInjectionFailed;
#endif

        public MapDefinition Map => map;
        public bool SpawnFactions => spawnFactions;
        public bool SpawnBuildings => spawnBuildings;
        public bool SpawnUnits => spawnUnits;
        public bool SpawnResourceNodes => spawnResourceNodes;
        public bool SpawnControlPoints => spawnControlPoints;
        public bool SpawnSettlements => spawnSettlements;
        public BuildingDefinition[] BuildingDefinitions => buildingDefinitions;
        public UnitDefinition[] UnitDefinitions => unitDefinitions;
        public SettlementClassDefinition[] SettlementDefinitions => settlementDefinitions;

#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshDefinitionCachesFromEditor();
        }

        private void Update()
        {
            if (!Application.isPlaying || runtimeBootstrapInjected || runtimeBootstrapInjectionFailed)
            {
                return;
            }

            TryInjectRuntimeBootstrapIntoPlayModeWorld();
        }

        private void RefreshDefinitionCachesFromEditor()
        {
            buildingDefinitions = LoadDefinitions<BuildingDefinition>(BuildingDefinitionsFolder);
            unitDefinitions = LoadDefinitions<UnitDefinition>(UnitDefinitionsFolder);
            settlementDefinitions = LoadDefinitions<SettlementClassDefinition>(SettlementDefinitionsFolder);
        }

        private static TDefinition[] LoadDefinitions<TDefinition>(string folderPath)
            where TDefinition : UnityEngine.Object
        {
            var definitions = new List<TDefinition>();
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(TDefinition).Name}", new[] { folderPath }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<TDefinition>(assetPath);
                if (asset != null)
                {
                    definitions.Add(asset);
                }
            }

            return definitions.ToArray();
        }

        private void TryInjectRuntimeBootstrapIntoPlayModeWorld()
        {
            if (map == null)
            {
                runtimeBootstrapInjectionFailed = true;
                UnityEngine.Debug.LogError("BloodlinesMapBootstrapAuthoring cannot inject runtime bootstrap data because no MapDefinition is assigned.", this);
                return;
            }

            if ((buildingDefinitions == null || buildingDefinitions.Length == 0) ||
                (unitDefinitions == null || unitDefinitions.Length == 0) ||
                (settlementDefinitions == null || settlementDefinitions.Length == 0))
            {
                RefreshDefinitionCachesFromEditor();
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            var entityManager = world.EntityManager;
            using var configQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MapBootstrapConfigComponent>());
            using var pendingQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MapBootstrapPendingTag>());

            if (!configQuery.IsEmptyIgnoreFilter || !pendingQuery.IsEmptyIgnoreFilter)
            {
                runtimeBootstrapInjected = true;
                return;
            }

            if (!TryCreateRuntimeBootstrapEntity(entityManager, out string errorMessage))
            {
                runtimeBootstrapInjectionFailed = true;
                UnityEngine.Debug.LogError(errorMessage, this);
                return;
            }

            runtimeBootstrapInjected = true;
            UnityEngine.Debug.Log(
                "BloodlinesMapBootstrapAuthoring injected runtime bootstrap entity for map '" +
                (map.id ?? string.Empty) + "'.",
                this);
        }
#endif

        private bool TryCreateRuntimeBootstrapEntity(EntityManager entityManager, out string errorMessage)
        {
            var buildingLookup = IndexDefinitions(buildingDefinitions, static definition => definition.id);
            var unitLookup = IndexDefinitions(unitDefinitions, static definition => definition.id);
            var settlementLookup = IndexDefinitions(settlementDefinitions, static definition => definition.id);

            Entity bootstrapEntity = Entity.Null;

            try
            {
                bootstrapEntity = entityManager.CreateEntity();
                entityManager.AddComponent<MapBootstrapPendingTag>(bootstrapEntity);
                entityManager.AddComponentData(bootstrapEntity, new MapBootstrapConfigComponent
                {
                    MapId = map.id ?? string.Empty,
                    MapDisplayName = map.displayName ?? string.Empty,
                    TileSize = map.tileSize,
                    Width = map.width,
                    Height = map.height,
                    CameraStart = new float2(map.cameraStart?.x ?? 0f, map.cameraStart?.y ?? 0f),
                    SpawnFactions = spawnFactions,
                    SpawnBuildings = spawnBuildings,
                    SpawnUnits = spawnUnits,
                    SpawnResourceNodes = spawnResourceNodes,
                    SpawnControlPoints = spawnControlPoints,
                    SpawnSettlements = spawnSettlements,
                });

                entityManager.AddBuffer<MapFactionSeedElement>(bootstrapEntity);
                entityManager.AddBuffer<MapBuildingSeedElement>(bootstrapEntity);
                entityManager.AddBuffer<MapUnitSeedElement>(bootstrapEntity);
                entityManager.AddBuffer<MapResourceNodeSeedElement>(bootstrapEntity);
                entityManager.AddBuffer<MapControlPointSeedElement>(bootstrapEntity);
                entityManager.AddBuffer<MapSettlementSeedElement>(bootstrapEntity);

                var factionBuffer = entityManager.GetBuffer<MapFactionSeedElement>(bootstrapEntity);
                var buildingBuffer = entityManager.GetBuffer<MapBuildingSeedElement>(bootstrapEntity);
                var unitBuffer = entityManager.GetBuffer<MapUnitSeedElement>(bootstrapEntity);
                var resourceNodeBuffer = entityManager.GetBuffer<MapResourceNodeSeedElement>(bootstrapEntity);
                var controlPointBuffer = entityManager.GetBuffer<MapControlPointSeedElement>(bootstrapEntity);
                var settlementBuffer = entityManager.GetBuffer<MapSettlementSeedElement>(bootstrapEntity);

                foreach (var resourceNode in map.resourceNodes ?? Array.Empty<ResourceNodeData>())
                {
                    resourceNodeBuffer.Add(new MapResourceNodeSeedElement
                    {
                        RuntimeId = resourceNode.id ?? string.Empty,
                        ResourceId = resourceNode.type ?? string.Empty,
                        Position = new float3(resourceNode.x, 0f, resourceNode.y),
                        Amount = resourceNode.amount,
                    });
                }

                foreach (var controlPoint in map.controlPoints ?? Array.Empty<ControlPointData>())
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

                foreach (var settlement in map.settlements ?? Array.Empty<SettlementSeedData>())
                {
                    var settlementDefinition = RequireDefinition(settlementLookup, settlement.settlementClass, "settlement class");
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

                foreach (var faction in map.factions ?? Array.Empty<FactionSeedData>())
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

                    foreach (var building in faction.buildings ?? Array.Empty<BuildingSeedData>())
                    {
                        var buildingDefinition = RequireDefinition(buildingLookup, building.typeId, "building");
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
                            GoldTrickle = buildingDefinition.resourceTrickle?.gold ?? 0f,
                            FoodTrickle = buildingDefinition.resourceTrickle?.food ?? 0f,
                            WaterTrickle = buildingDefinition.resourceTrickle?.water ?? 0f,
                            WoodTrickle = buildingDefinition.resourceTrickle?.wood ?? 0f,
                            StoneTrickle = buildingDefinition.resourceTrickle?.stone ?? 0f,
                            IronTrickle = buildingDefinition.resourceTrickle?.iron ?? 0f,
                            InfluenceTrickle = buildingDefinition.resourceTrickle?.influence ?? 0f,
                        });
                    }

                    foreach (var unit in faction.units ?? Array.Empty<UnitSeedData>())
                    {
                        var unitDefinition = RequireDefinition(unitLookup, unit.typeId, "unit");
                        unitBuffer.Add(new MapUnitSeedElement
                        {
                            RuntimeId = unit.id ?? string.Empty,
                            FactionId = faction.id ?? string.Empty,
                            TypeId = unit.typeId ?? string.Empty,
                            Position = new float3(unit.x, 0f, unit.y),
                            MaxHealth = unitDefinition.health,
                            MaxSpeed = unitDefinition.speed,
                            Role = ResolveUnitRole(unitDefinition.role),
                            SiegeClass = ResolveSiegeClass(unitDefinition.siegeClass),
                            PopulationCost = unitDefinition.populationCost,
                            Stage = unitDefinition.stage,
                        });
                    }
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                if (bootstrapEntity != Entity.Null && entityManager.Exists(bootstrapEntity))
                {
                    entityManager.DestroyEntity(bootstrapEntity);
                }

                errorMessage =
                    "BloodlinesMapBootstrapAuthoring failed to inject runtime bootstrap data for map '" +
                    (map?.id ?? string.Empty) + "': " + exception;
                return false;
            }
        }

        private static Dictionary<string, TDefinition> IndexDefinitions<TDefinition>(
            IEnumerable<TDefinition> definitions,
            Func<TDefinition, string> keySelector)
            where TDefinition : UnityEngine.Object
        {
            var lookup = new Dictionary<string, TDefinition>(StringComparer.OrdinalIgnoreCase);
            if (definitions == null)
            {
                return lookup;
            }

            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    continue;
                }

                var key = keySelector(definition);
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                lookup[key] = definition;
            }

            return lookup;
        }

        private static TDefinition RequireDefinition<TDefinition>(
            IReadOnlyDictionary<string, TDefinition> definitions,
            string id,
            string label)
            where TDefinition : UnityEngine.Object
        {
            if (!string.IsNullOrWhiteSpace(id) && definitions.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new InvalidOperationException(
                "Bloodlines runtime bootstrap could not resolve " + label + " definition '" + id + "'.");
        }

        private static FactionKind ResolveFactionKind(string kind)
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

        private static UnitRole ResolveUnitRole(string role)
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

        private static SiegeClass ResolveSiegeClass(string siegeClass)
        {
            if (string.Equals(siegeClass, "ram", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ram;
            if (string.Equals(siegeClass, "siege_tower", StringComparison.OrdinalIgnoreCase)) return SiegeClass.SiegeTower;
            if (string.Equals(siegeClass, "trebuchet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Trebuchet;
            if (string.Equals(siegeClass, "ballista", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ballista;
            if (string.Equals(siegeClass, "mantlet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Mantlet;
            return SiegeClass.None;
        }

        private static FortificationRole ResolveFortificationRole(string fortificationRole)
        {
            if (string.Equals(fortificationRole, "wall", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Wall;
            if (string.Equals(fortificationRole, "tower", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Tower;
            if (string.Equals(fortificationRole, "gate", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Gate;
            if (string.Equals(fortificationRole, "keep", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Keep;
            return FortificationRole.None;
        }
    }
}
