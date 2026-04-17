#if UNITY_EDITOR
using System.Collections.Generic;
using Bloodlines.Authoring;
using Bloodlines.Camera;
using Bloodlines.DataDefinitions;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using BloodlinesDebugPresentationBridge = Bloodlines.Debug.BloodlinesDebugEntityPresentationBridge;
using BloodlinesDebugCommandSurface = Bloodlines.Debug.BloodlinesDebugCommandSurface;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Creates the first governed Bootstrap and Gameplay scene shells for the
    /// Bloodlines Unity production lane.
    /// </summary>
    public static class BloodlinesGameplaySceneBootstrap
    {
        private const string MapAssetPath = "Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset";
        private const string BootstrapSceneFolder = "Assets/_Bloodlines/Scenes/Bootstrap";
        private const string GameplaySceneFolder = "Assets/_Bloodlines/Scenes/Gameplay";
        private const string BootstrapScenePath = BootstrapSceneFolder + "/Bootstrap.unity";
        private const string GameplayScenePath = GameplaySceneFolder + "/IronmarkFrontier.unity";
        private const string BootstrapMetadataRootName = "BOOTSTRAP_SCENE_METADATA";
        private const string GameplayMetadataRootName = "IRONMARK_FRONTIER_SCENE_METADATA";
        private const string BootstrapDescriptorName = "SCENE_DESCRIPTOR_BOOTSTRAP";
        private const string GameplayDescriptorName = "SCENE_DESCRIPTOR_GAMEPLAY";

        [MenuItem("Bloodlines/Scenes/Create Bootstrap And Gameplay Scene Shells")]
        public static void CreateBootstrapAndGameplaySceneShells()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            CreateBootstrapAndGameplaySceneShellsInternal();
        }

        [MenuItem("Bloodlines/Scenes/Open Gameplay Scenes Folder")]
        public static void OpenGameplayScenesFolder()
        {
            EnsureFolderChain("Assets/_Bloodlines/Scenes");
            EditorUtility.RevealInFinder("Assets/_Bloodlines/Scenes");
        }

        [MenuItem("Bloodlines/Scenes/Repair Bootstrap Scene Canonical Map Assignment")]
        public static void RepairBootstrapSceneCanonicalMapAssignment()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            RepairBootstrapSceneCanonicalMapAssignmentInternal();
        }

        [MenuItem("Bloodlines/Scenes/Validate Bootstrap Scene Shell")]
        public static void ValidateBootstrapSceneShell()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            ValidateBootstrapSceneShellInternal(throwOnFailure: false);
        }

        [MenuItem("Bloodlines/Scenes/Validate Gameplay Scene Shell")]
        public static void ValidateGameplaySceneShell()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            ValidateGameplaySceneShellInternal(throwOnFailure: false);
        }

        public static void RunBatchCreateBootstrapAndGameplaySceneShells()
        {
            CreateBootstrapAndGameplaySceneShellsInternal();
        }

        public static void RunBatchRepairBootstrapSceneCanonicalMapAssignment()
        {
            RepairBootstrapSceneCanonicalMapAssignmentInternal();
        }

        public static void RunBatchValidateBootstrapSceneShell()
        {
            ValidateBootstrapSceneShellInternal(throwOnFailure: true);
        }

        public static void RunBatchValidateGameplaySceneShell()
        {
            ValidateGameplaySceneShellInternal(throwOnFailure: true);
        }

        private static void CreateBootstrapAndGameplaySceneShellsInternal()
        {
            var created = new List<string>();
            var skipped = new List<string>();
            var map = LoadCanonicalMapDefinition();

            EnsureFolderChain(BootstrapSceneFolder);
            EnsureFolderChain(GameplaySceneFolder);

            if (TryCreateScene(BootstrapScenePath, BootstrapMetadataRootName, map, includeBootstrapAuthoring: true))
            {
                created.Add(BootstrapScenePath);
            }
            else
            {
                skipped.Add(BootstrapScenePath);
            }

            if (TryCreateScene(GameplayScenePath, GameplayMetadataRootName, map, includeBootstrapAuthoring: false))
            {
                created.Add(GameplayScenePath);
            }
            else
            {
                skipped.Add(GameplayScenePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log(
                "Bloodlines gameplay scene bootstrap complete. Created: " + created.Count +
                " | Skipped existing: " + skipped.Count);
            foreach (var path in created)
            {
                UnityEngine.Debug.Log("Created gameplay scene shell: " + path);
            }
        }

        private static void RepairBootstrapSceneCanonicalMapAssignmentInternal()
        {
            var map = LoadCanonicalMapDefinition();
            if (map == null)
            {
                UnityEngine.Debug.LogWarning(
                    "Bootstrap scene repair could not resolve the canonical MapDefinition asset. " +
                    "Repair aborted without modifying the existing scene.");
                return;
            }

            if (!System.IO.File.Exists(BootstrapScenePath))
            {
                UnityEngine.Debug.LogWarning(
                    "Bootstrap scene repair could not find " + BootstrapScenePath +
                    ". Run scene-shell creation first.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            var authoring = Object.FindFirstObjectByType<BloodlinesMapBootstrapAuthoring>();
            if (authoring == null)
            {
                var bootstrapObject = new GameObject("BloodlinesMapBootstrap");
                bootstrapObject.transform.position = Vector3.zero;
                authoring = bootstrapObject.AddComponent<BloodlinesMapBootstrapAuthoring>();
            }

            AssignMapToBootstrapAuthoring(authoring, map);
            RepairCameraRig(map);
            RepairReferenceGround(map);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var persistentReference = AssetDatabase.LoadMainAssetAtPath(MapAssetPath);
            var assignedAssetPath = persistentReference != null
                ? AssetDatabase.GetAssetPath(persistentReference)
                : MapAssetPath;
            UnityEngine.Debug.Log(
                "Bootstrap scene canonical map assignment repaired using asset: " +
                assignedAssetPath);
        }

        private static void ValidateBootstrapSceneShellInternal(bool throwOnFailure)
        {
            ValidateSceneShellInternal(
                scenePath: BootstrapScenePath,
                sceneLabel: "Bootstrap",
                expectedMetadataRootName: BootstrapMetadataRootName,
                expectedDescriptorName: BootstrapDescriptorName,
                expectBootstrapAuthoring: true,
                throwOnFailure: throwOnFailure);
        }

        private static void ValidateGameplaySceneShellInternal(bool throwOnFailure)
        {
            ValidateSceneShellInternal(
                scenePath: GameplayScenePath,
                sceneLabel: "Gameplay",
                expectedMetadataRootName: GameplayMetadataRootName,
                expectedDescriptorName: GameplayDescriptorName,
                expectBootstrapAuthoring: false,
                throwOnFailure: throwOnFailure);
        }

        private static void ValidateSceneShellInternal(
            string scenePath,
            string sceneLabel,
            string expectedMetadataRootName,
            string expectedDescriptorName,
            bool expectBootstrapAuthoring,
            bool throwOnFailure)
        {
            if (!System.IO.File.Exists(scenePath))
            {
                HandleBootstrapValidationFailure(
                    sceneLabel + " scene validation could not find " + scenePath + ".",
                    throwOnFailure);
                return;
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            var failures = new List<string>();

            var authoring = Object.FindFirstObjectByType<BloodlinesMapBootstrapAuthoring>();
            if (expectBootstrapAuthoring)
            {
                if (authoring == null)
                {
                    failures.Add("Missing BloodlinesMapBootstrapAuthoring.");
                }
                else
                {
                    var assignedMapPath = authoring.Map != null
                        ? AssetDatabase.GetAssetPath(authoring.Map)
                        : string.Empty;
                    if (!string.Equals(assignedMapPath, MapAssetPath, System.StringComparison.Ordinal))
                    {
                        failures.Add(
                            "Bootstrap authoring map assignment mismatch. Expected " + MapAssetPath +
                            " but found " + (string.IsNullOrEmpty(assignedMapPath) ? "<null>" : assignedMapPath) + ".");
                    }
                }
            }
            else if (authoring != null)
            {
                failures.Add("Gameplay scene should not contain BloodlinesMapBootstrapAuthoring.");
            }

            if (Object.FindFirstObjectByType<BloodlinesBattlefieldCameraController>() == null)
            {
                failures.Add("Missing BloodlinesBattlefieldCameraController.");
            }

            if (Object.FindFirstObjectByType<BloodlinesDebugPresentationBridge>() == null)
            {
                failures.Add("Missing BloodlinesDebugEntityPresentationBridge.");
            }

            if (Object.FindFirstObjectByType<BloodlinesDebugCommandSurface>() == null)
            {
                failures.Add("Missing BloodlinesDebugCommandSurface.");
            }

            if (GameObject.Find("REFERENCE_GROUND") == null)
            {
                failures.Add("Missing REFERENCE_GROUND.");
            }

            var metadataRoot = GameObject.Find(expectedMetadataRootName);
            if (metadataRoot == null)
            {
                failures.Add("Missing " + expectedMetadataRootName + ".");
            }
            else if (metadataRoot.transform.Find(expectedDescriptorName) == null)
            {
                failures.Add(
                    "Metadata root " + expectedMetadataRootName +
                    " is missing child " + expectedDescriptorName + ".");
            }

            if (failures.Count > 0)
            {
                var message = sceneLabel + " scene shell validation failed:" + System.Environment.NewLine +
                              string.Join(System.Environment.NewLine, failures);
                HandleBootstrapValidationFailure(message, throwOnFailure);
                return;
            }

            UnityEngine.Debug.Log(
                sceneLabel + " scene shell validation passed for " + scenePath +
                (expectBootstrapAuthoring
                    ? " with canonical map " + MapAssetPath + "."
                    : "."));
        }

        private static bool TryCreateScene(string scenePath, string metadataRootName, MapDefinition map, bool includeBootstrapAuthoring)
        {
            if (System.IO.File.Exists(scenePath))
            {
                return false;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateMetadataRoot(metadataRootName, includeBootstrapAuthoring);
            CreateDirectionalLight();
            CreateCameraRig(map);
            CreateDebugPresentationBridge();
            CreateDebugCommandSurface();
            CreateReferenceGround(map);

            if (includeBootstrapAuthoring)
            {
                CreateBootstrapAuthoring(map);
            }

            EditorSceneManager.SaveScene(scene, scenePath);
            return true;
        }

        private static void CreateMetadataRoot(string metadataRootName, bool includeBootstrapAuthoring)
        {
            var root = new GameObject(metadataRootName);
            root.transform.position = Vector3.zero;

            var descriptor = new GameObject("SCENE_DESCRIPTOR");
            descriptor.transform.SetParent(root.transform, false);
            descriptor.name = includeBootstrapAuthoring
                ? BootstrapDescriptorName
                : GameplayDescriptorName;
        }

        private static void CreateBootstrapAuthoring(MapDefinition map)
        {
            var bootstrapObject = new GameObject("BloodlinesMapBootstrap");
            bootstrapObject.transform.position = Vector3.zero;
            var authoring = bootstrapObject.AddComponent<BloodlinesMapBootstrapAuthoring>();

            if (map == null)
            {
                UnityEngine.Debug.LogWarning(
                    "BloodlinesGameplaySceneBootstrap could not find " + MapAssetPath +
                    ". Bootstrap scene was created without a map assignment.");
                return;
            }

            AssignMapToBootstrapAuthoring(authoring, map);
        }

        private static void CreateCameraRig(MapDefinition map)
        {
            var cameraRig = new GameObject("BattlefieldCameraRig");
            cameraRig.transform.position = Vector3.zero;
            var controller = cameraRig.AddComponent<BloodlinesBattlefieldCameraController>();

            Vector2 worldMin = Vector2.zero;
            Vector2 worldMax = map != null
                ? new Vector2(Mathf.Max(1f, map.width), Mathf.Max(1f, map.height))
                : new Vector2(76f, 76f);
            Vector2 focus = map?.cameraStart != null
                ? new Vector2(map.cameraStart.x, map.cameraStart.y)
                : worldMax * 0.5f;

            controller.ApplyMapConfiguration(worldMin, worldMax, focus);
        }

        private static void CreateDebugPresentationBridge()
        {
            var debugObject = new GameObject("DebugEntityPresentation");
            debugObject.transform.position = Vector3.zero;
            debugObject.AddComponent<BloodlinesDebugPresentationBridge>();
        }

        private static void CreateDebugCommandSurface()
        {
            var commandObject = new GameObject("DebugCommandSurface");
            commandObject.transform.position = Vector3.zero;
            commandObject.AddComponent<BloodlinesDebugCommandSurface>();
        }

        private static void CreateReferenceGround(MapDefinition map)
        {
            float width = map != null ? Mathf.Max(10f, map.width) : 76f;
            float height = map != null ? Mathf.Max(10f, map.height) : 76f;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REFERENCE_GROUND";
            ground.transform.position = new Vector3(width * 0.5f, 0f, height * 0.5f);
            ground.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);

            var renderer = ground.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("Standard")
                    ?? Shader.Find("Sprites/Default");
                var material = new Material(shader)
                {
                    color = new Color(0.34f, 0.39f, 0.3f, 1f)
                };
                renderer.sharedMaterial = material;
            }
        }

        private static MapDefinition LoadCanonicalMapDefinition()
        {
            var map = AssetDatabase.LoadAssetAtPath<MapDefinition>(MapAssetPath);
            if (map != null)
            {
                return map;
            }

            var directObject = AssetDatabase.LoadMainAssetAtPath(MapAssetPath);
            if (directObject is MapDefinition directMap)
            {
                UnityEngine.Debug.LogWarning(
                    "Resolved canonical map through direct main-asset loading instead of typed path loading. " +
                    "Resolved path: " + MapAssetPath);
                return directMap;
            }

            var exactMatches = AssetDatabase.FindAssets("ironmark_frontier t:MapDefinition");
            if (exactMatches.Length > 0)
            {
                var resolvedPath = AssetDatabase.GUIDToAssetPath(exactMatches[0]);
                map = AssetDatabase.LoadAssetAtPath<MapDefinition>(resolvedPath);
                if (map != null)
                {
                    UnityEngine.Debug.LogWarning(
                        "Resolved canonical map through asset search instead of the direct path. " +
                        "Resolved path: " + resolvedPath);
                    return map;
                }
            }

            var nameMatches = AssetDatabase.FindAssets("ironmark_frontier");
            for (int i = 0; i < nameMatches.Length; i++)
            {
                var resolvedPath = AssetDatabase.GUIDToAssetPath(nameMatches[i]);
                var candidate = AssetDatabase.LoadMainAssetAtPath(resolvedPath);
                if (candidate is MapDefinition namedMap)
                {
                    UnityEngine.Debug.LogWarning(
                        "Resolved canonical map through name-based main-asset search. " +
                        "Resolved path: " + resolvedPath);
                    return namedMap;
                }
            }

            var anyMaps = AssetDatabase.FindAssets("t:MapDefinition");
            if (anyMaps.Length > 0)
            {
                var resolvedPath = AssetDatabase.GUIDToAssetPath(anyMaps[0]);
                map = AssetDatabase.LoadAssetAtPath<MapDefinition>(resolvedPath);
                if (map != null)
                {
                    UnityEngine.Debug.LogWarning(
                        "Resolved fallback MapDefinition through broad asset search. " +
                        "Resolved path: " + resolvedPath);
                    return map;
                }
            }

            return null;
        }

        private static void AssignMapToBootstrapAuthoring(BloodlinesMapBootstrapAuthoring authoring, MapDefinition map)
        {
            var persistentReference = AssetDatabase.LoadMainAssetAtPath(MapAssetPath);
            var serializedObject = new SerializedObject(authoring);
            serializedObject.FindProperty("map").objectReferenceValue = persistentReference ?? map;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(authoring);
        }

        private static void RepairCameraRig(MapDefinition map)
        {
            var controller = Object.FindFirstObjectByType<BloodlinesBattlefieldCameraController>();
            if (controller == null)
            {
                return;
            }

            Vector2 worldMin = Vector2.zero;
            Vector2 worldMax = map != null
                ? new Vector2(Mathf.Max(1f, map.width), Mathf.Max(1f, map.height))
                : new Vector2(76f, 76f);
            Vector2 focus = map?.cameraStart != null
                ? new Vector2(map.cameraStart.x, map.cameraStart.y)
                : worldMax * 0.5f;

            controller.ApplyMapConfiguration(worldMin, worldMax, focus);
        }

        private static void RepairReferenceGround(MapDefinition map)
        {
            var ground = GameObject.Find("REFERENCE_GROUND");
            if (ground == null)
            {
                return;
            }

            float width = map != null ? Mathf.Max(10f, map.width) : 76f;
            float height = map != null ? Mathf.Max(10f, map.height) : 76f;
            ground.transform.position = new Vector3(width * 0.5f, 0f, height * 0.5f);
            ground.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
        }

        private static void CreateDirectionalLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.position = new Vector3(0f, 8f, 0f);
            lightObject.transform.rotation = Quaternion.Euler(46f, -34f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.05f;
            light.color = new Color(1f, 0.97f, 0.93f, 1f);
        }

        private static void HandleBootstrapValidationFailure(string message, bool throwOnFailure)
        {
            if (throwOnFailure)
            {
                throw new System.InvalidOperationException(message);
            }

            UnityEngine.Debug.LogError(message);
        }

        private static void EnsureFolderChain(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            var parts = assetFolderPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
#endif
