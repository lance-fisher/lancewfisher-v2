using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug-only presentation bridge for the first Unity ECS battlefield shell.
    /// This is not final rendering architecture. It exists so early bootstrap and
    /// camera work can be exercised in Play Mode before the production render path
    /// is in place.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BloodlinesDebugEntityPresentationBridge : MonoBehaviour
    {
        [SerializeField] private bool presentUnits = true;
        [SerializeField] private bool presentBuildings = true;
        [SerializeField] private bool presentSettlements = true;
        [SerializeField] private bool presentControlPoints = true;
        [SerializeField] private bool presentResourceNodes = true;
        [SerializeField] private bool presentProjectiles = true;
        [SerializeField] private Color projectileColor = new Color(1f, 0.68f, 0.18f, 1f);
        [SerializeField] private float projectileProxyScale = 0.26f;
        [SerializeField] private bool presentConstructionProgress = true;
        [SerializeField] private Color constructionProgressTrackColor = new Color(0.12f, 0.14f, 0.17f, 1f);
        [SerializeField] private Color constructionProgressFillColor = new Color(0.95f, 0.82f, 0.38f, 1f);
        [SerializeField] private float constructionProgressBarWidth = 1.9f;
        [SerializeField] private float constructionProgressBarHeight = 0.18f;
        [SerializeField] private float constructionProgressBarVerticalOffset = 1.55f;
        [SerializeField] private bool presentProductionProgress = true;
        [SerializeField] private Color productionProgressTrackColor = new Color(0.07f, 0.12f, 0.19f, 1f);
        [SerializeField] private Color productionProgressFillColor = new Color(0.35f, 0.72f, 1f, 1f);
        [SerializeField] private float productionProgressBarWidth = 1.9f;
        [SerializeField] private float productionProgressBarHeight = 0.16f;
        [SerializeField] private float productionProgressBarVerticalOffset = 1.95f;

        private readonly Dictionary<Entity, GameObject> proxyObjects = new();
        private readonly Dictionary<Entity, ConstructionProgressProxy> constructionProgressProxies = new();
        private readonly Dictionary<Entity, ProductionProgressProxy> productionProgressProxies = new();
        private readonly HashSet<Entity> seenThisFrame = new();
        private readonly HashSet<Entity> seenConstructionProgressThisFrame = new();
        private readonly HashSet<Entity> seenProductionProgressThisFrame = new();
        private readonly Dictionary<string, Material> materialCache = new();
        private World boundWorld;
        private EntityQuery unitQuery;
        private EntityQuery buildingQuery;
        private EntityQuery settlementQuery;
        private EntityQuery controlPointQuery;
        private EntityQuery resourceNodeQuery;
        private EntityQuery projectileQuery;

        private void LateUpdate()
        {
            if (!EnsureQueries())
            {
                ClearAllProxies();
                return;
            }

            var entityManager = boundWorld.EntityManager;
            seenThisFrame.Clear();
            seenConstructionProgressThisFrame.Clear();
            seenProductionProgressThisFrame.Clear();

            if (presentUnits)
            {
                SyncUnits();
            }

            if (presentBuildings)
            {
                SyncBuildings();
            }

            if (presentSettlements)
            {
                SyncSettlements();
            }

            if (presentControlPoints)
            {
                SyncControlPoints();
            }

            if (presentResourceNodes)
            {
                SyncResourceNodes();
            }

            if (presentProjectiles)
            {
                SyncProjectiles();
            }

            RemoveStaleProxies();
            RemoveStaleConstructionProgressProxies();
            RemoveStaleProductionProgressProxies();
        }

        private void OnDisable()
        {
            ClearAllProxies();

            foreach (var material in materialCache.Values)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }

            materialCache.Clear();
        }

        private bool EnsureQueries()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            if (ReferenceEquals(boundWorld, world))
            {
                return true;
            }

            boundWorld = world;
            var entityManager = world.EntityManager;
            unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            resourceNodeQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            projectileQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ProjectileComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            return true;
        }

        private void SyncUnits()
        {
            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Capsule, "UNIT");
                proxy.transform.position = ToVector3(positions[i].Value) + new Vector3(0f, 0.9f, 0f);
                bool selected = boundWorld.EntityManager.HasComponent<SelectedTag>(entity);
                proxy.transform.localScale = selected
                    ? new Vector3(0.82f, 1.08f, 0.82f)
                    : new Vector3(0.65f, 0.9f, 0.65f);
                var color = ResolveFactionColor(factions[i].FactionId.ToString());
                if (selected)
                {
                    color = Color.Lerp(color, Color.white, 0.45f);
                }

                ApplyColor(proxy, color);
            }
        }

        private void SyncBuildings()
        {
            using var entities = buildingQuery.ToEntityArray(Allocator.Temp);
            using var positions = buildingQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Cube, "BUILDING");
                proxy.transform.position = ToVector3(positions[i].Value) + new Vector3(0f, 0.6f, 0f);
                bool underConstruction = boundWorld.EntityManager.HasComponent<ConstructionStateComponent>(entity);
                var scale = ResolveBuildingScale(buildingTypes[i]);
                if (underConstruction)
                {
                    scale = new Vector3(scale.x * 0.92f, Mathf.Max(0.45f, scale.y * 0.58f), scale.z * 0.92f);
                }

                bool selected = boundWorld.EntityManager.HasComponent<SelectedTag>(entity);
                proxy.transform.localScale = selected
                    ? scale * 1.08f
                    : scale;
                var color = ResolveFactionColor(factions[i].FactionId.ToString()) * new Color(0.9f, 0.9f, 0.9f, 1f);
                if (underConstruction)
                {
                    color = Color.Lerp(color, new Color(0.82f, 0.76f, 0.54f, 1f), 0.45f);
                }

                if (selected)
                {
                    color = Color.Lerp(color, Color.white, 0.35f);
                }

                ApplyColor(proxy, color);

                if (presentConstructionProgress && underConstruction)
                {
                    SyncConstructionProgressBar(entity, positions[i].Value, scale);
                }

                if (presentProductionProgress && !underConstruction)
                {
                    SyncProductionProgressBar(entity, positions[i].Value, scale);
                }
            }
        }

        private void SyncConstructionProgressBar(Entity entity, float3 buildingPosition, Vector3 buildingScale)
        {
            var construction = boundWorld.EntityManager.GetComponentData<ConstructionStateComponent>(entity);
            float totalSeconds = Mathf.Max(0.1f, construction.TotalSeconds);
            float remainingSeconds = Mathf.Max(0f, construction.RemainingSeconds);
            float progress = Mathf.Clamp01(1f - (remainingSeconds / totalSeconds));

            if (!constructionProgressProxies.TryGetValue(entity, out var barProxy) || !barProxy.IsValid())
            {
                barProxy = CreateConstructionProgressProxy(entity);
                constructionProgressProxies[entity] = barProxy;
            }

            seenConstructionProgressThisFrame.Add(entity);
            float verticalLift = buildingScale.y * 0.5f + constructionProgressBarVerticalOffset;
            var anchor = ToVector3(buildingPosition) + new Vector3(0f, verticalLift, 0f);
            float trackWidth = Mathf.Max(0.1f, constructionProgressBarWidth);
            float fillWidth = trackWidth * progress;

            barProxy.Track.transform.position = anchor;
            barProxy.Track.transform.localScale = new Vector3(trackWidth, constructionProgressBarHeight, constructionProgressBarHeight);

            barProxy.Fill.transform.localScale = new Vector3(
                Mathf.Max(0.01f, fillWidth),
                constructionProgressBarHeight * 1.05f,
                constructionProgressBarHeight * 1.1f);
            barProxy.Fill.transform.position =
                anchor + new Vector3((fillWidth - trackWidth) * 0.5f, 0f, -constructionProgressBarHeight * 0.05f);

            ApplyColor(barProxy.Track, constructionProgressTrackColor);
            ApplyColor(barProxy.Fill, constructionProgressFillColor);
        }

        private ConstructionProgressProxy CreateConstructionProgressProxy(Entity entity)
        {
            var track = GameObject.CreatePrimitive(PrimitiveType.Cube);
            track.name = "CONSTRUCTION_PROGRESS_TRACK_" + entity.Index + "_" + entity.Version;
            track.transform.SetParent(transform, false);
            var trackCollider = track.GetComponent<Collider>();
            if (trackCollider != null)
            {
                Destroy(trackCollider);
            }

            var fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fill.name = "CONSTRUCTION_PROGRESS_FILL_" + entity.Index + "_" + entity.Version;
            fill.transform.SetParent(transform, false);
            var fillCollider = fill.GetComponent<Collider>();
            if (fillCollider != null)
            {
                Destroy(fillCollider);
            }

            return new ConstructionProgressProxy(track, fill);
        }

        private void RemoveStaleConstructionProgressProxies()
        {
            if (constructionProgressProxies.Count == 0)
            {
                return;
            }

            var stale = new List<Entity>();
            foreach (var pair in constructionProgressProxies)
            {
                if (!seenConstructionProgressThisFrame.Contains(pair.Key))
                {
                    stale.Add(pair.Key);
                }
            }

            foreach (var entity in stale)
            {
                if (constructionProgressProxies.TryGetValue(entity, out var proxy))
                {
                    proxy.Destroy();
                }

                constructionProgressProxies.Remove(entity);
            }
        }

        private void SyncProductionProgressBar(Entity entity, float3 buildingPosition, Vector3 buildingScale)
        {
            if (!boundWorld.EntityManager.HasBuffer<ProductionQueueItemElement>(entity))
            {
                return;
            }

            var queue = boundWorld.EntityManager.GetBuffer<ProductionQueueItemElement>(entity);
            if (queue.Length == 0)
            {
                return;
            }

            var active = queue[0];
            float totalSeconds = Mathf.Max(0.1f, active.TotalSeconds);
            float remainingSeconds = Mathf.Max(0f, active.RemainingSeconds);
            float progress = Mathf.Clamp01(1f - (remainingSeconds / totalSeconds));

            if (!productionProgressProxies.TryGetValue(entity, out var barProxy) || !barProxy.IsValid())
            {
                barProxy = CreateProductionProgressProxy(entity);
                productionProgressProxies[entity] = barProxy;
            }

            seenProductionProgressThisFrame.Add(entity);
            float verticalLift = buildingScale.y * 0.5f + productionProgressBarVerticalOffset;
            var anchor = ToVector3(buildingPosition) + new Vector3(0f, verticalLift, 0f);
            float trackWidth = Mathf.Max(0.1f, productionProgressBarWidth);
            float fillWidth = trackWidth * progress;

            barProxy.Track.transform.position = anchor;
            barProxy.Track.transform.localScale = new Vector3(trackWidth, productionProgressBarHeight, productionProgressBarHeight);

            barProxy.Fill.transform.localScale = new Vector3(
                Mathf.Max(0.01f, fillWidth),
                productionProgressBarHeight * 1.05f,
                productionProgressBarHeight * 1.1f);
            barProxy.Fill.transform.position =
                anchor + new Vector3((fillWidth - trackWidth) * 0.5f, 0f, -productionProgressBarHeight * 0.05f);

            ApplyColor(barProxy.Track, productionProgressTrackColor);
            ApplyColor(barProxy.Fill, productionProgressFillColor);
        }

        private ProductionProgressProxy CreateProductionProgressProxy(Entity entity)
        {
            var track = GameObject.CreatePrimitive(PrimitiveType.Cube);
            track.name = "PRODUCTION_PROGRESS_TRACK_" + entity.Index + "_" + entity.Version;
            track.transform.SetParent(transform, false);
            var trackCollider = track.GetComponent<Collider>();
            if (trackCollider != null)
            {
                Destroy(trackCollider);
            }

            var fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fill.name = "PRODUCTION_PROGRESS_FILL_" + entity.Index + "_" + entity.Version;
            fill.transform.SetParent(transform, false);
            var fillCollider = fill.GetComponent<Collider>();
            if (fillCollider != null)
            {
                Destroy(fillCollider);
            }

            return new ProductionProgressProxy(track, fill);
        }

        private void RemoveStaleProductionProgressProxies()
        {
            if (productionProgressProxies.Count == 0)
            {
                return;
            }

            var stale = new List<Entity>();
            foreach (var pair in productionProgressProxies)
            {
                if (!seenProductionProgressThisFrame.Contains(pair.Key))
                {
                    stale.Add(pair.Key);
                }
            }

            foreach (var entity in stale)
            {
                if (productionProgressProxies.TryGetValue(entity, out var proxy))
                {
                    proxy.Destroy();
                }

                productionProgressProxies.Remove(entity);
            }
        }

        private void SyncSettlements()
        {
            using var entities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var positions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Cube, "SETTLEMENT");
                proxy.transform.position = ToVector3(positions[i].Value) + new Vector3(0f, 0.15f, 0f);
                proxy.transform.localScale = new Vector3(2.6f, 0.3f, 2.6f);
                ApplyColor(proxy, ResolveFactionColor(factions[i].FactionId.ToString()) * new Color(1f, 0.95f, 0.85f, 1f));
            }
        }

        private void SyncControlPoints()
        {
            using var entities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var positions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Cylinder, "CONTROL_POINT");
                proxy.transform.position = ToVector3(positions[i].Value) + new Vector3(0f, 0.04f, 0f);
                float radius = Mathf.Max(1.2f, controlPoints[i].RadiusTiles * 2f);
                proxy.transform.localScale = new Vector3(radius, 0.04f, radius);
                var color = controlPoints[i].OwnerFactionId.Length > 0
                    ? ResolveFactionColor(controlPoints[i].OwnerFactionId.ToString())
                    : new Color(0.82f, 0.71f, 0.28f, 1f);
                ApplyColor(proxy, color);
            }
        }

        private void SyncResourceNodes()
        {
            using var entities = resourceNodeQuery.ToEntityArray(Allocator.Temp);
            using var positions = resourceNodeQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var resourceNodes = resourceNodeQuery.ToComponentDataArray<ResourceNodeComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Sphere, "RESOURCE_NODE");
                proxy.transform.position = ToVector3(positions[i].Value) + new Vector3(0f, 0.45f, 0f);
                proxy.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                ApplyColor(proxy, ResolveResourceColor(resourceNodes[i].ResourceId.ToString()));
            }
        }

        private void SyncProjectiles()
        {
            using var entities = projectileQuery.ToEntityArray(Allocator.Temp);
            using var positions = projectileQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                seenThisFrame.Add(entity);
                var proxy = GetOrCreateProxy(entity, PrimitiveType.Sphere, "PROJECTILE");
                proxy.transform.position = ToVector3(positions[i].Value);
                proxy.transform.localScale = new Vector3(projectileProxyScale, projectileProxyScale, projectileProxyScale);
                ApplyColor(proxy, projectileColor);
            }
        }

        private GameObject GetOrCreateProxy(Entity entity, PrimitiveType primitiveType, string prefix)
        {
            if (proxyObjects.TryGetValue(entity, out var existing) && existing != null)
            {
                return existing;
            }

            var proxy = GameObject.CreatePrimitive(primitiveType);
            proxy.name = prefix + "_" + entity.Index + "_" + entity.Version;
            proxy.transform.SetParent(transform, false);
            var collider = proxy.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            proxyObjects[entity] = proxy;
            return proxy;
        }

        private void ApplyColor(GameObject proxy, Color color)
        {
            var renderer = proxy.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            renderer.sharedMaterial = GetOrCreateMaterial(color);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        private Material GetOrCreateMaterial(Color color)
        {
            string key = ColorUtility.ToHtmlStringRGBA(color);
            if (materialCache.TryGetValue(key, out var material) && material != null)
            {
                return material;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Sprites/Default");
            material = new Material(shader)
            {
                color = color
            };
            materialCache[key] = material;
            return material;
        }

        private void RemoveStaleProxies()
        {
            var staleEntities = new List<Entity>();
            foreach (var pair in proxyObjects)
            {
                if (!seenThisFrame.Contains(pair.Key))
                {
                    staleEntities.Add(pair.Key);
                }
            }

            foreach (var entity in staleEntities)
            {
                if (proxyObjects.TryGetValue(entity, out var proxy) && proxy != null)
                {
                    Destroy(proxy);
                }

                proxyObjects.Remove(entity);
            }
        }

        private void ClearAllProxies()
        {
            foreach (var proxy in proxyObjects.Values)
            {
                if (proxy != null)
                {
                    Destroy(proxy);
                }
            }

            proxyObjects.Clear();
            seenThisFrame.Clear();

            foreach (var progress in constructionProgressProxies.Values)
            {
                progress.Destroy();
            }

            constructionProgressProxies.Clear();
            seenConstructionProgressThisFrame.Clear();

            foreach (var progress in productionProgressProxies.Values)
            {
                progress.Destroy();
            }

            productionProgressProxies.Clear();
            seenProductionProgressThisFrame.Clear();
        }

        private readonly struct ConstructionProgressProxy
        {
            public GameObject Track { get; }
            public GameObject Fill { get; }

            public ConstructionProgressProxy(GameObject track, GameObject fill)
            {
                Track = track;
                Fill = fill;
            }

            public bool IsValid()
            {
                return Track != null && Fill != null;
            }

            public void Destroy()
            {
                if (Track != null)
                {
                    Object.Destroy(Track);
                }

                if (Fill != null)
                {
                    Object.Destroy(Fill);
                }
            }
        }

        private readonly struct ProductionProgressProxy
        {
            public GameObject Track { get; }
            public GameObject Fill { get; }

            public ProductionProgressProxy(GameObject track, GameObject fill)
            {
                Track = track;
                Fill = fill;
            }

            public bool IsValid()
            {
                return Track != null && Fill != null;
            }

            public void Destroy()
            {
                if (Track != null)
                {
                    Object.Destroy(Track);
                }

                if (Fill != null)
                {
                    Object.Destroy(Fill);
                }
            }
        }

        private static Vector3 ResolveBuildingScale(BuildingTypeComponent buildingType)
        {
            if (buildingType.FortificationRole == FortificationRole.Tower)
            {
                return new Vector3(1.15f, 2.6f, 1.15f);
            }

            if (buildingType.FortificationRole == FortificationRole.Gate)
            {
                return new Vector3(2.2f, 1.35f, 1.1f);
            }

            if (buildingType.FortificationRole == FortificationRole.Keep)
            {
                return new Vector3(2.6f, 2.1f, 2.6f);
            }

            if (buildingType.FortificationRole == FortificationRole.Wall)
            {
                return new Vector3(2.2f, 1f, 0.7f);
            }

            return new Vector3(1.6f, 1.2f, 1.6f);
        }

        private static Color ResolveFactionColor(string factionId)
        {
            string normalized = (factionId ?? string.Empty).Trim().ToLowerInvariant();
            return normalized switch
            {
                "" => new Color(0.72f, 0.72f, 0.72f, 1f),
                "player" => new Color(0.23f, 0.56f, 0.96f, 1f),
                "ironmark" => new Color(0.23f, 0.56f, 0.96f, 1f),
                "enemy" => new Color(0.87f, 0.3f, 0.29f, 1f),
                "stonehelm" => new Color(0.87f, 0.3f, 0.29f, 1f),
                "tribes" => new Color(0.78f, 0.58f, 0.22f, 1f),
                "neutral" => new Color(0.62f, 0.62f, 0.66f, 1f),
                _ => Color.HSVToRGB(Mathf.Abs(normalized.GetHashCode() % 1000) / 1000f, 0.55f, 0.9f)
            };
        }

        private static Color ResolveResourceColor(string resourceId)
        {
            string normalized = (resourceId ?? string.Empty).Trim().ToLowerInvariant();
            return normalized switch
            {
                "gold" => new Color(0.95f, 0.8f, 0.18f, 1f),
                "wood" => new Color(0.47f, 0.31f, 0.16f, 1f),
                "stone" => new Color(0.62f, 0.62f, 0.66f, 1f),
                "iron" => new Color(0.34f, 0.38f, 0.43f, 1f),
                "food" => new Color(0.48f, 0.72f, 0.28f, 1f),
                "water" => new Color(0.28f, 0.58f, 0.88f, 1f),
                _ => new Color(0.78f, 0.78f, 0.78f, 1f)
            };
        }

        private static Vector3 ToVector3(Unity.Mathematics.float3 value)
        {
            return new Vector3(value.x, value.y, value.z);
        }
    }
}
