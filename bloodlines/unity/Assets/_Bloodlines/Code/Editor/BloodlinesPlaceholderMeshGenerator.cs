#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Bloodlines.DataDefinitions;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Procedural low-poly placeholder mesh generator. Infrastructure tool.
    /// Produces distinct silhouettes per unit class and per building class
    /// so that Session 125's graphics lane has something to render before
    /// any commercial art is authored. Final faction-identity art replaces
    /// these one asset at a time without any system rewrite.
    ///
    /// Menu: Bloodlines -> Graphics -> Generate Placeholder Meshes
    /// </summary>
    public static class BloodlinesPlaceholderMeshGenerator
    {
        private const string PlaceholderRoot = "Assets/_Bloodlines/Art/Placeholders";
        private const string ShaderName = "Bloodlines/FactionColor";
        private const string UnitVisualFolder = "Assets/_Bloodlines/Data/UnitVisualDefinitions";
        private const string BuildingVisualFolder = "Assets/_Bloodlines/Data/BuildingVisualDefinitions";

        [MenuItem("Bloodlines/Graphics/Generate Placeholder Meshes")]
        public static void GeneratePlaceholders()
        {
            EnsureFolderChain(PlaceholderRoot);
            EnsureFolderChain(UnitVisualFolder);
            EnsureFolderChain(BuildingVisualFolder);

            var shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                UnityDebug.LogError("BloodlinesPlaceholderMeshGenerator: shader '" + ShaderName + "' not found. Re-import the shader folder and retry.");
                return;
            }

            var sharedMaterial = LoadOrCreateMaterial("Placeholder_FactionShared", shader);

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var spec in UnitPlaceholderSpecs)
                {
                    var mesh = GenerateUnitMesh(spec);
                    SaveMesh(mesh, PlaceholderMeshPath("unit", spec.Id));
                    CreateOrUpdateUnitVisual(spec, mesh, sharedMaterial);
                }

                foreach (var spec in BuildingPlaceholderSpecs)
                {
                    var mesh = GenerateBuildingMesh(spec);
                    SaveMesh(mesh, PlaceholderMeshPath("building", spec.Id));
                    CreateOrUpdateBuildingVisual(spec, mesh, sharedMaterial);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            UnityDebug.Log("BloodlinesPlaceholderMeshGenerator: generated " +
                UnitPlaceholderSpecs.Length + " unit meshes + " +
                BuildingPlaceholderSpecs.Length + " building meshes with paired visual definitions.");
        }

        private static readonly UnitSpec[] UnitPlaceholderSpecs = new[]
        {
            // Units: shape hints unit CLASS, not faction identity.
            new UnitSpec("villager",          PlaceholderSilhouette.Capsule,      new Vector3(0.6f, 1.2f, 0.6f), 0.9f),
            new UnitSpec("worker",            PlaceholderSilhouette.Capsule,      new Vector3(0.6f, 1.2f, 0.6f), 0.9f),
            new UnitSpec("militia",           PlaceholderSilhouette.TaperedPost,  new Vector3(0.7f, 1.5f, 0.7f), 0.95f),
            new UnitSpec("swordsman",         PlaceholderSilhouette.BroadShoulder,new Vector3(0.85f, 1.6f, 0.7f), 1.0f),
            new UnitSpec("bowman",            PlaceholderSilhouette.SlenderBow,   new Vector3(0.6f, 1.5f, 0.55f), 1.0f),
            new UnitSpec("axeman",            PlaceholderSilhouette.Axeman,       new Vector3(0.85f, 1.6f, 0.75f), 1.0f),
            new UnitSpec("light_cavalry",     PlaceholderSilhouette.Cavalry,      new Vector3(1.1f, 1.6f, 1.8f), 1.05f),
            new UnitSpec("scout_rider",       PlaceholderSilhouette.Cavalry,      new Vector3(1.05f, 1.5f, 1.7f), 1.0f),
            new UnitSpec("verdant_warden",    PlaceholderSilhouette.Warden,       new Vector3(0.8f, 1.7f, 0.8f), 1.0f),
            new UnitSpec("commander",         PlaceholderSilhouette.Commander,    new Vector3(0.9f, 1.9f, 0.8f), 1.15f),
            new UnitSpec("tribal_raider",     PlaceholderSilhouette.BroadShoulder,new Vector3(0.75f, 1.5f, 0.7f), 1.0f),
            new UnitSpec("siege_engineer",    PlaceholderSilhouette.SlenderBow,   new Vector3(0.65f, 1.5f, 0.6f), 1.0f),
            new UnitSpec("supply_wagon",      PlaceholderSilhouette.Wagon,        new Vector3(1.6f, 1.1f, 2.4f), 1.0f),
            new UnitSpec("battering_ram",     PlaceholderSilhouette.Wagon,        new Vector3(1.8f, 1.1f, 2.8f), 1.0f),
            new UnitSpec("siege_tower",       PlaceholderSilhouette.Commander,    new Vector3(1.4f, 2.6f, 1.4f), 1.2f),
            new UnitSpec("trebuchet",         PlaceholderSilhouette.Commander,    new Vector3(1.5f, 2.2f, 1.5f), 1.15f),
            new UnitSpec("ballista",          PlaceholderSilhouette.Wagon,        new Vector3(1.4f, 1.2f, 2.2f), 1.0f),
            new UnitSpec("mantlet",           PlaceholderSilhouette.Wagon,        new Vector3(1.8f, 1.2f, 0.6f), 1.0f),
        };

        private static readonly BuildingSpec[] BuildingPlaceholderSpecs = new[]
        {
            new BuildingSpec("command_hall",   PlaceholderStructure.LargeKeepLike,  new Vector3(2.6f, 2.4f, 2.6f)),
            new BuildingSpec("dwelling",       PlaceholderStructure.Cottage,        new Vector3(1.6f, 1.4f, 1.6f)),
            new BuildingSpec("farm",           PlaceholderStructure.LowPlot,        new Vector3(2.0f, 0.7f, 2.0f)),
            new BuildingSpec("well",           PlaceholderStructure.WellShape,      new Vector3(1.1f, 1.0f, 1.1f)),
            new BuildingSpec("barracks",       PlaceholderStructure.BarracksLong,   new Vector3(2.4f, 1.8f, 1.7f)),
            new BuildingSpec("stable",         PlaceholderStructure.BarracksLong,   new Vector3(2.2f, 1.6f, 1.7f)),
            new BuildingSpec("siege_workshop", PlaceholderStructure.Workshop,       new Vector3(2.4f, 1.9f, 2.0f)),
            new BuildingSpec("supply_camp",    PlaceholderStructure.LowPlot,        new Vector3(1.8f, 0.9f, 1.8f)),
            new BuildingSpec("wayshrine",      PlaceholderStructure.Shrine,         new Vector3(1.2f, 1.9f, 1.2f)),
            new BuildingSpec("market",         PlaceholderStructure.Workshop,       new Vector3(2.0f, 1.4f, 2.0f)),
            new BuildingSpec("storehouse",     PlaceholderStructure.BarracksLong,   new Vector3(1.9f, 1.3f, 1.6f)),
            new BuildingSpec("granary",        PlaceholderStructure.Silo,           new Vector3(1.3f, 1.8f, 1.3f)),
            new BuildingSpec("keep_tier_1",    PlaceholderStructure.LargeKeepLike,  new Vector3(2.8f, 2.8f, 2.8f)),
            new BuildingSpec("watch_tower",    PlaceholderStructure.Tower,          new Vector3(1.0f, 2.6f, 1.0f)),
            new BuildingSpec("gatehouse",      PlaceholderStructure.Gate,           new Vector3(2.2f, 1.8f, 1.2f)),
            new BuildingSpec("wall_segment",   PlaceholderStructure.WallSegment,    new Vector3(2.2f, 1.4f, 0.6f)),
            new BuildingSpec("tribal_camp",    PlaceholderStructure.Cottage,        new Vector3(1.5f, 1.3f, 1.5f)),
        };

        private enum PlaceholderSilhouette
        {
            Capsule,
            TaperedPost,
            BroadShoulder,
            SlenderBow,
            Axeman,
            Cavalry,
            Warden,
            Commander,
            Wagon,
        }

        private enum PlaceholderStructure
        {
            LargeKeepLike,
            Cottage,
            LowPlot,
            WellShape,
            BarracksLong,
            Workshop,
            Shrine,
            Silo,
            Tower,
            Gate,
            WallSegment,
        }

        private readonly struct UnitSpec
        {
            public readonly string Id;
            public readonly PlaceholderSilhouette Silhouette;
            public readonly Vector3 Scale;
            public readonly float VerticalOffset;

            public UnitSpec(string id, PlaceholderSilhouette silhouette, Vector3 scale, float verticalOffset)
            {
                Id = id;
                Silhouette = silhouette;
                Scale = scale;
                VerticalOffset = verticalOffset;
            }
        }

        private readonly struct BuildingSpec
        {
            public readonly string Id;
            public readonly PlaceholderStructure Structure;
            public readonly Vector3 Scale;

            public BuildingSpec(string id, PlaceholderStructure structure, Vector3 scale)
            {
                Id = id;
                Structure = structure;
                Scale = scale;
            }
        }

        private static Mesh GenerateUnitMesh(UnitSpec spec)
        {
            switch (spec.Silhouette)
            {
                case PlaceholderSilhouette.TaperedPost:    return BuildTaperedPost(spec.Scale);
                case PlaceholderSilhouette.BroadShoulder:  return BuildBroadShoulder(spec.Scale);
                case PlaceholderSilhouette.SlenderBow:     return BuildSlenderBow(spec.Scale);
                case PlaceholderSilhouette.Axeman:         return BuildAxeman(spec.Scale);
                case PlaceholderSilhouette.Cavalry:        return BuildCavalry(spec.Scale);
                case PlaceholderSilhouette.Warden:         return BuildWarden(spec.Scale);
                case PlaceholderSilhouette.Commander:      return BuildCommander(spec.Scale);
                case PlaceholderSilhouette.Wagon:          return BuildWagon(spec.Scale);
                case PlaceholderSilhouette.Capsule:
                default:
                    return BuildTaperedPost(spec.Scale);
            }
        }

        private static Mesh GenerateBuildingMesh(BuildingSpec spec)
        {
            switch (spec.Structure)
            {
                case PlaceholderStructure.Cottage:       return BuildCottage(spec.Scale);
                case PlaceholderStructure.LowPlot:       return BuildLowPlot(spec.Scale);
                case PlaceholderStructure.WellShape:     return BuildWell(spec.Scale);
                case PlaceholderStructure.BarracksLong:  return BuildBarracksLong(spec.Scale);
                case PlaceholderStructure.Workshop:      return BuildWorkshop(spec.Scale);
                case PlaceholderStructure.Shrine:        return BuildShrine(spec.Scale);
                case PlaceholderStructure.Silo:          return BuildSilo(spec.Scale);
                case PlaceholderStructure.Tower:         return BuildTower(spec.Scale);
                case PlaceholderStructure.Gate:          return BuildGate(spec.Scale);
                case PlaceholderStructure.WallSegment:   return BuildWallSegment(spec.Scale);
                case PlaceholderStructure.LargeKeepLike:
                default:
                    return BuildKeep(spec.Scale);
            }
        }

        private static Mesh BuildTaperedPost(Vector3 s)
        {
            var mesh = new Mesh { name = "Placeholder_TaperedPost" };
            float x = s.x * 0.5f;
            float z = s.z * 0.5f;
            float shoulderX = x * 0.65f;
            float shoulderZ = z * 0.65f;

            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-shoulderX, s.y * 0.85f, -shoulderZ), new Vector3(shoulderX, s.y * 0.85f, -shoulderZ),
                new Vector3(shoulderX, s.y * 0.85f, shoulderZ), new Vector3(-shoulderX, s.y * 0.85f, shoulderZ),
                new Vector3(0f, s.y, 0f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendTriangle(tris, 4, 5, 8);
            AppendTriangle(tris, 5, 6, 8);
            AppendTriangle(tris, 6, 7, 8);
            AppendTriangle(tris, 7, 4, 8);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildBroadShoulder(Vector3 s)
        {
            var mesh = new Mesh { name = "Placeholder_BroadShoulder" };
            float hipX = s.x * 0.35f;
            float shoulderX = s.x * 0.55f;
            float z = s.z * 0.35f;

            var verts = new List<Vector3>
            {
                new Vector3(-hipX, 0f, -z), new Vector3(hipX, 0f, -z), new Vector3(hipX, 0f, z), new Vector3(-hipX, 0f, z),
                new Vector3(-shoulderX, s.y * 0.8f, -z), new Vector3(shoulderX, s.y * 0.8f, -z),
                new Vector3(shoulderX, s.y * 0.8f, z), new Vector3(-shoulderX, s.y * 0.8f, z),
                new Vector3(-hipX * 0.6f, s.y, -z * 0.6f), new Vector3(hipX * 0.6f, s.y, -z * 0.6f),
                new Vector3(hipX * 0.6f, s.y, z * 0.6f), new Vector3(-hipX * 0.6f, s.y, z * 0.6f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendQuad(tris, 4, 5, 9, 8);
            AppendQuad(tris, 5, 6, 10, 9);
            AppendQuad(tris, 6, 7, 11, 10);
            AppendQuad(tris, 7, 4, 8, 11);
            AppendQuad(tris, 8, 9, 10, 11);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildSlenderBow(Vector3 s) => BuildExtrudedDiamond(s, lean: 0.15f, name: "Placeholder_SlenderBow");
        private static Mesh BuildAxeman(Vector3 s) => BuildExtrudedDiamond(new Vector3(s.x * 1.1f, s.y, s.z), lean: 0.05f, name: "Placeholder_Axeman");
        private static Mesh BuildWarden(Vector3 s) => BuildExtrudedDiamond(s, lean: -0.05f, name: "Placeholder_Warden");
        private static Mesh BuildCommander(Vector3 s) => BuildExtrudedDiamond(new Vector3(s.x, s.y * 1.05f, s.z), lean: 0.0f, name: "Placeholder_Commander");

        private static Mesh BuildExtrudedDiamond(Vector3 s, float lean, string name)
        {
            var mesh = new Mesh { name = name };
            float baseX = s.x * 0.5f;
            float baseZ = s.z * 0.5f;
            float midX = baseX * 0.85f;
            float midZ = baseZ * 0.85f;

            var verts = new List<Vector3>
            {
                new Vector3(-baseX, 0f, -baseZ), new Vector3(baseX, 0f, -baseZ),
                new Vector3(baseX, 0f, baseZ), new Vector3(-baseX, 0f, baseZ),
                new Vector3(-midX + lean * s.x, s.y * 0.5f, -midZ),
                new Vector3(midX + lean * s.x, s.y * 0.5f, -midZ),
                new Vector3(midX + lean * s.x, s.y * 0.5f, midZ),
                new Vector3(-midX + lean * s.x, s.y * 0.5f, midZ),
                new Vector3(lean * s.x, s.y, 0f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendTriangle(tris, 4, 5, 8);
            AppendTriangle(tris, 5, 6, 8);
            AppendTriangle(tris, 6, 7, 8);
            AppendTriangle(tris, 7, 4, 8);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildCavalry(Vector3 s)
        {
            var mesh = new Mesh { name = "Placeholder_Cavalry" };
            float x = s.x * 0.5f;
            float z = s.z * 0.5f;
            float bodyY = s.y * 0.6f;
            float riderBaseY = bodyY;
            float riderTopY = s.y;
            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-x, bodyY, -z), new Vector3(x, bodyY, -z),
                new Vector3(x, bodyY, z), new Vector3(-x, bodyY, z),
                new Vector3(-x * 0.45f, riderBaseY, -z * 0.45f), new Vector3(x * 0.45f, riderBaseY, -z * 0.45f),
                new Vector3(x * 0.45f, riderBaseY, z * 0.45f), new Vector3(-x * 0.45f, riderBaseY, z * 0.45f),
                new Vector3(0f, riderTopY, 0f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendQuad(tris, 4, 5, 9, 8);
            AppendQuad(tris, 5, 6, 10, 9);
            AppendQuad(tris, 6, 7, 11, 10);
            AppendQuad(tris, 7, 4, 8, 11);
            AppendTriangle(tris, 8, 9, 12);
            AppendTriangle(tris, 9, 10, 12);
            AppendTriangle(tris, 10, 11, 12);
            AppendTriangle(tris, 11, 8, 12);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildWagon(Vector3 s)
        {
            var mesh = new Mesh { name = "Placeholder_Wagon" };
            float x = s.x * 0.5f;
            float y = s.y;
            float z = s.z * 0.5f;
            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-x, y * 0.8f, -z), new Vector3(x, y * 0.8f, -z),
                new Vector3(x, y * 0.8f, z), new Vector3(-x, y * 0.8f, z),
                new Vector3(-x * 0.8f, y, -z * 0.8f), new Vector3(x * 0.8f, y, -z * 0.8f),
                new Vector3(x * 0.8f, y, z * 0.8f), new Vector3(-x * 0.8f, y, z * 0.8f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendQuad(tris, 4, 5, 9, 8);
            AppendQuad(tris, 5, 6, 10, 9);
            AppendQuad(tris, 6, 7, 11, 10);
            AppendQuad(tris, 7, 4, 8, 11);
            AppendQuad(tris, 8, 9, 10, 11);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildKeep(Vector3 s) => BuildSteppedBox(s, steps: 3, name: "Placeholder_Keep");
        private static Mesh BuildCottage(Vector3 s) => BuildGabledBox(s, name: "Placeholder_Cottage");
        private static Mesh BuildLowPlot(Vector3 s) => BuildFlatBox(s, name: "Placeholder_Plot");
        private static Mesh BuildBarracksLong(Vector3 s) => BuildGabledBox(s, name: "Placeholder_BarracksLong");
        private static Mesh BuildWorkshop(Vector3 s) => BuildGabledBox(new Vector3(s.x, s.y * 0.9f, s.z), name: "Placeholder_Workshop");
        private static Mesh BuildGate(Vector3 s) => BuildSteppedBox(s, steps: 2, name: "Placeholder_Gate");
        private static Mesh BuildWallSegment(Vector3 s) => BuildFlatBox(s, name: "Placeholder_WallSegment");
        private static Mesh BuildTower(Vector3 s) => BuildSteppedBox(new Vector3(s.x, s.y, s.z), steps: 3, name: "Placeholder_Tower");
        private static Mesh BuildSilo(Vector3 s) => BuildSteppedBox(new Vector3(s.x, s.y, s.z), steps: 3, name: "Placeholder_Silo");
        private static Mesh BuildShrine(Vector3 s) => BuildSpireBox(s, name: "Placeholder_Shrine");
        private static Mesh BuildWell(Vector3 s) => BuildFlatBox(s, name: "Placeholder_Well");

        private static Mesh BuildFlatBox(Vector3 s, string name)
        {
            var mesh = new Mesh { name = name };
            float x = s.x * 0.5f;
            float y = s.y;
            float z = s.z * 0.5f;
            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-x, y, -z), new Vector3(x, y, -z), new Vector3(x, y, z), new Vector3(-x, y, z),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendQuad(tris, 4, 5, 6, 7);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildSteppedBox(Vector3 s, int steps, string name)
        {
            steps = Mathf.Max(1, steps);
            var mesh = new Mesh { name = name };
            var verts = new List<Vector3>();
            var tris = new List<int>();
            float stepHeight = s.y / steps;
            float widthStart = 0.5f;
            float widthEnd = 0.35f;
            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / steps;
                float tNext = (float)(i + 1) / steps;
                float wx = Mathf.Lerp(widthStart, widthEnd, t) * s.x;
                float wz = Mathf.Lerp(widthStart, widthEnd, t) * s.z;
                float wxn = Mathf.Lerp(widthStart, widthEnd, tNext) * s.x;
                float wzn = Mathf.Lerp(widthStart, widthEnd, tNext) * s.z;
                float y0 = i * stepHeight;
                float y1 = (i + 1) * stepHeight;
                int baseIdx = verts.Count;
                verts.Add(new Vector3(-wx, y0, -wz));
                verts.Add(new Vector3(wx, y0, -wz));
                verts.Add(new Vector3(wx, y0, wz));
                verts.Add(new Vector3(-wx, y0, wz));
                verts.Add(new Vector3(-wxn, y1, -wzn));
                verts.Add(new Vector3(wxn, y1, -wzn));
                verts.Add(new Vector3(wxn, y1, wzn));
                verts.Add(new Vector3(-wxn, y1, wzn));

                if (i == 0)
                {
                    AppendQuad(tris, baseIdx + 0, baseIdx + 3, baseIdx + 2, baseIdx + 1);
                }

                AppendQuad(tris, baseIdx + 0, baseIdx + 1, baseIdx + 5, baseIdx + 4);
                AppendQuad(tris, baseIdx + 1, baseIdx + 2, baseIdx + 6, baseIdx + 5);
                AppendQuad(tris, baseIdx + 2, baseIdx + 3, baseIdx + 7, baseIdx + 6);
                AppendQuad(tris, baseIdx + 3, baseIdx + 0, baseIdx + 4, baseIdx + 7);

                if (i == steps - 1)
                {
                    AppendQuad(tris, baseIdx + 4, baseIdx + 5, baseIdx + 6, baseIdx + 7);
                }
            }
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildGabledBox(Vector3 s, string name)
        {
            var mesh = new Mesh { name = name };
            float x = s.x * 0.5f;
            float y = s.y * 0.7f;
            float z = s.z * 0.5f;
            float peak = s.y;
            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-x, y, -z), new Vector3(x, y, -z), new Vector3(x, y, z), new Vector3(-x, y, z),
                new Vector3(0f, peak, -z), new Vector3(0f, peak, z),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendTriangle(tris, 4, 5, 8);
            AppendTriangle(tris, 7, 6, 9);
            AppendQuad(tris, 4, 8, 9, 7);
            AppendQuad(tris, 5, 6, 9, 8);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static Mesh BuildSpireBox(Vector3 s, string name)
        {
            var mesh = new Mesh { name = name };
            float x = s.x * 0.5f;
            float z = s.z * 0.5f;
            float baseY = s.y * 0.55f;
            var verts = new List<Vector3>
            {
                new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z),
                new Vector3(-x, baseY, -z), new Vector3(x, baseY, -z), new Vector3(x, baseY, z), new Vector3(-x, baseY, z),
                new Vector3(0f, s.y, 0f),
            };
            var tris = new List<int>();
            AppendQuad(tris, 0, 3, 2, 1);
            AppendQuad(tris, 0, 1, 5, 4);
            AppendQuad(tris, 1, 2, 6, 5);
            AppendQuad(tris, 2, 3, 7, 6);
            AppendQuad(tris, 3, 0, 4, 7);
            AppendTriangle(tris, 4, 5, 8);
            AppendTriangle(tris, 5, 6, 8);
            AppendTriangle(tris, 6, 7, 8);
            AppendTriangle(tris, 7, 4, 8);
            FinalizeMesh(mesh, verts, tris);
            return mesh;
        }

        private static void AppendQuad(List<int> tris, int a, int b, int c, int d)
        {
            tris.Add(a); tris.Add(b); tris.Add(c);
            tris.Add(a); tris.Add(c); tris.Add(d);
        }

        private static void AppendTriangle(List<int> tris, int a, int b, int c)
        {
            tris.Add(a); tris.Add(b); tris.Add(c);
        }

        private static void FinalizeMesh(Mesh mesh, List<Vector3> verts, List<int> tris)
        {
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private static void SaveMesh(Mesh mesh, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null)
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
            else
            {
                existing.Clear();
                existing.SetVertices(mesh.vertices);
                existing.SetTriangles(mesh.triangles, 0);
                existing.RecalculateNormals();
                existing.RecalculateBounds();
                EditorUtility.SetDirty(existing);
            }
        }

        private static Material LoadOrCreateMaterial(string name, Shader shader)
        {
            string path = PlaceholderRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                if (existing.shader != shader)
                {
                    existing.shader = shader;
                    EditorUtility.SetDirty(existing);
                }
                return existing;
            }

            var mat = new Material(shader);
            mat.name = name;
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static void CreateOrUpdateUnitVisual(UnitSpec spec, Mesh mesh, Material material)
        {
            string path = UnitVisualFolder + "/UnitVisual_" + spec.Id + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<UnitVisualDefinition>(path);
            bool created = false;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UnitVisualDefinition>();
                AssetDatabase.CreateAsset(asset, path);
                created = true;
            }

            asset.id = spec.Id;
            asset.mesh = mesh;
            asset.material = material;
            asset.silhouetteScale = 1f;
            asset.verticalOffset = spec.VerticalOffset;
            asset.castsShadows = false;
            EditorUtility.SetDirty(asset);

            if (created)
            {
                UnityDebug.Log("BloodlinesPlaceholderMeshGenerator: created UnitVisual for " + spec.Id);
            }
        }

        private static void CreateOrUpdateBuildingVisual(BuildingSpec spec, Mesh mesh, Material material)
        {
            string path = BuildingVisualFolder + "/BuildingVisual_" + spec.Id + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<BuildingVisualDefinition>(path);
            bool created = false;
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<BuildingVisualDefinition>();
                AssetDatabase.CreateAsset(asset, path);
                created = true;
            }

            asset.id = spec.Id;
            asset.mesh = mesh;
            asset.material = material;
            asset.silhouetteScale = 1f;
            asset.verticalOffset = spec.Scale.y * 0.5f;
            asset.castsShadows = false;
            EditorUtility.SetDirty(asset);

            if (created)
            {
                UnityDebug.Log("BloodlinesPlaceholderMeshGenerator: created BuildingVisual for " + spec.Id);
            }
        }

        private static string PlaceholderMeshPath(string kind, string id)
        {
            return PlaceholderRoot + "/" + kind + "_" + id + ".asset";
        }

        private static void EnsureFolderChain(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var segments = folderPath.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }
                current = next;
            }
        }
    }
}
#endif
