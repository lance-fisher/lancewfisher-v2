using System.Collections.Generic;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Bloodlines.Rendering
{
    /// <summary>
    /// Runtime visual presentation bridge for the first URP rendering pass.
    ///
    /// Walks the ECS world every LateUpdate, looks up the UnitVisualDefinition
    /// or BuildingVisualDefinition for each entity, spawns a GameObject proxy
    /// with the definition's Mesh and Material, and sets the shader's
    /// _FactionColor property per-instance via MaterialPropertyBlock so
    /// entities of different factions share a material without duplicating
    /// it.
    ///
    /// Companion to BloodlinesDebugEntityPresentationBridge: when a unit or
    /// building has a visual definition bound here, the debug bridge's
    /// primitive proxy falls back to "hidden" so only one proxy renders per
    /// entity. Entities without a visual definition still use the debug
    /// primitive fallback so governance / smoke work never breaks.
    ///
    /// Infrastructure-only. Final faction-identity art replaces the
    /// placeholder meshes and materials; this code does not change.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BloodlinesVisualPresentationBridge : MonoBehaviour
    {
        [SerializeField] private bool presentUnits = true;
        [SerializeField] private bool presentBuildings = true;

        private static readonly int FactionColorId = Shader.PropertyToID("_FactionColor");
        private static readonly int FactionTintStrengthId = Shader.PropertyToID("_FactionTintStrength");
        private static readonly int SelectionHighlightId = Shader.PropertyToID("_SelectionHighlight");

        private readonly Dictionary<Entity, ProxyInstance> proxies = new Dictionary<Entity, ProxyInstance>();
        private readonly HashSet<Entity> seenThisFrame = new HashSet<Entity>();
        private MaterialPropertyBlock propertyBlock;
        private World boundWorld;
        private EntityQuery unitQuery;
        private EntityQuery buildingQuery;

        public static BloodlinesVisualPresentationBridge ActiveInstance { get; private set; }

        public bool TryGetVisualProxyMeshAndMaterial(Entity entity, out Mesh mesh, out Material material)
        {
            mesh = null;
            material = null;
            if (!proxies.TryGetValue(entity, out var proxy))
            {
                return false;
            }

            mesh = proxy.Mesh;
            material = proxy.Material;
            return mesh != null && material != null;
        }

        public bool HasVisualProxy(Entity entity) => proxies.ContainsKey(entity);

        public int VisualProxyCount => proxies.Count;

        private void OnEnable()
        {
            ActiveInstance = this;
        }

        private void OnDisable()
        {
            if (ActiveInstance == this)
            {
                ActiveInstance = null;
            }

            ClearAll();
        }

        private void LateUpdate()
        {
            if (!EnsureQueries())
            {
                ClearAll();
                return;
            }

            seenThisFrame.Clear();
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            if (presentUnits)
            {
                SyncUnits();
            }

            if (presentBuildings)
            {
                SyncBuildings();
            }

            RemoveStaleProxies();
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
            var em = world.EntityManager;
            unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            buildingQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            return true;
        }

        private void SyncUnits()
        {
            var em = boundWorld.EntityManager;
            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var types = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!BloodlinesVisualCatalog.TryGetUnitVisual(types[i].TypeId.ToString(), out var def))
                {
                    continue;
                }

                if (def.mesh == null || def.material == null)
                {
                    continue;
                }

                seenThisFrame.Add(entities[i]);
                var proxy = GetOrCreateProxy(entities[i], def.mesh, def.material, "UNIT");
                float3 pos = positions[i].Value;
                proxy.Transform.position = new Vector3(pos.x, pos.y + def.verticalOffset, pos.z);
                proxy.Transform.localScale = Vector3.one * def.silhouetteScale;
                EnsureFactionTint(em, entities[i], factions[i].FactionId.ToString(), out float4 tint);
                ApplyPerInstance(proxy, tint, em.HasComponent<SelectedTag>(entities[i]));
            }
        }

        private void SyncBuildings()
        {
            var em = boundWorld.EntityManager;
            using var entities = buildingQuery.ToEntityArray(Allocator.Temp);
            using var positions = buildingQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var types = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!BloodlinesVisualCatalog.TryGetBuildingVisual(types[i].TypeId.ToString(), out var def))
                {
                    continue;
                }

                if (def.mesh == null || def.material == null)
                {
                    continue;
                }

                seenThisFrame.Add(entities[i]);
                var proxy = GetOrCreateProxy(entities[i], def.mesh, def.material, "BUILDING");
                float3 pos = positions[i].Value;
                proxy.Transform.position = new Vector3(pos.x, pos.y, pos.z);
                proxy.Transform.localScale = Vector3.one * def.silhouetteScale;
                EnsureFactionTint(em, entities[i], factions[i].FactionId.ToString(), out float4 tint);
                ApplyPerInstance(proxy, tint, em.HasComponent<SelectedTag>(entities[i]));
            }
        }

        private ProxyInstance GetOrCreateProxy(Entity entity, Mesh mesh, Material material, string kind)
        {
            if (proxies.TryGetValue(entity, out var proxy) && proxy.GameObject != null)
            {
                if (proxy.Filter.sharedMesh != mesh)
                {
                    proxy.Filter.sharedMesh = mesh;
                    proxy.Mesh = mesh;
                }

                if (proxy.Renderer.sharedMaterial != material)
                {
                    proxy.Renderer.sharedMaterial = material;
                    proxy.Material = material;
                }

                return proxy;
            }

            var go = new GameObject(kind + "_VISUAL_" + entity.Index + "_" + entity.Version);
            go.transform.SetParent(transform, false);
            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            var instance = new ProxyInstance
            {
                GameObject = go,
                Transform = go.transform,
                Filter = filter,
                Renderer = renderer,
                Mesh = mesh,
                Material = material,
            };
            proxies[entity] = instance;
            return instance;
        }

        private void EnsureFactionTint(EntityManager em, Entity entity, string factionId, out float4 tint)
        {
            if (em.HasComponent<FactionTintComponent>(entity))
            {
                tint = em.GetComponentData<FactionTintComponent>(entity).Tint;
                return;
            }

            tint = FactionTintPalette.ResolveTint(factionId);
            em.AddComponentData(entity, new FactionTintComponent { Tint = tint });
        }

        private void ApplyPerInstance(ProxyInstance proxy, float4 tint, bool selected)
        {
            propertyBlock.Clear();
            propertyBlock.SetColor(FactionColorId, new Color(tint.x, tint.y, tint.z, tint.w));
            propertyBlock.SetFloat(FactionTintStrengthId, 0.55f);
            propertyBlock.SetFloat(SelectionHighlightId, selected ? 1f : 0f);
            proxy.Renderer.SetPropertyBlock(propertyBlock);
        }

        private void RemoveStaleProxies()
        {
            if (proxies.Count == 0)
            {
                return;
            }

            List<Entity> stale = null;
            foreach (var pair in proxies)
            {
                if (!seenThisFrame.Contains(pair.Key))
                {
                    stale ??= new List<Entity>();
                    stale.Add(pair.Key);
                }
            }

            if (stale == null)
            {
                return;
            }

            foreach (var entity in stale)
            {
                if (proxies.TryGetValue(entity, out var proxy) && proxy.GameObject != null)
                {
                    Destroy(proxy.GameObject);
                }

                proxies.Remove(entity);
            }
        }

        private void ClearAll()
        {
            foreach (var pair in proxies)
            {
                if (pair.Value.GameObject != null)
                {
                    Destroy(pair.Value.GameObject);
                }
            }

            proxies.Clear();
            seenThisFrame.Clear();
        }

        private sealed class ProxyInstance
        {
            public GameObject GameObject;
            public Transform Transform;
            public MeshFilter Filter;
            public MeshRenderer Renderer;
            public Mesh Mesh;
            public Material Material;
        }
    }
}
