using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class GraphicsVisualTestbedBootstrap
    {
        private static readonly TestbedDefinition[] Testbeds =
        {
            new("Assets/_Bloodlines/Scenes/Testbeds/VisualReadability", "VisualReadability_Testbed.unity", "READABILITY_PROBES"),
            new("Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev", "TerrainLookdev_Testbed.unity", "TERRAIN_STRIPS"),
            new("Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev", "MaterialLookdev_Testbed.unity", "MATERIAL_PROBES"),
            new("Assets/_Bloodlines/Scenes/Testbeds/IconLegibility", "IconLegibility_Testbed.unity", "ICON_PROBES")
        };

        [MenuItem("Bloodlines/Graphics/Create Visual Testbed Scene Shells")]
        public static void CreateVisualTestbedSceneShells()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            CreateVisualTestbedSceneShellsInternal();
        }

        [MenuItem("Bloodlines/Graphics/Open Visual Testbed Folder")]
        public static void OpenVisualTestbedFolder()
        {
            EnsureFolderChain("Assets/_Bloodlines/Scenes/Testbeds");
            EditorUtility.RevealInFinder("Assets/_Bloodlines/Scenes/Testbeds");
        }

        public static void RunBatchCreateVisualTestbedSceneShells()
        {
            CreateVisualTestbedSceneShellsInternal();
        }

        private static void CreateVisualTestbedSceneShellsInternal()
        {
            var created = new List<string>();
            var skipped = new List<string>();

            foreach (var testbed in Testbeds)
            {
                EnsureFolderChain(testbed.AssetFolder);
                var scenePath = testbed.AssetFolder + "/" + testbed.SceneFileName;
                if (System.IO.File.Exists(scenePath))
                {
                    skipped.Add(scenePath);
                    continue;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                CreateMetadataRoot(testbed);
                CreateMainCamera();
                CreateDirectionalLight();
                CreateReferenceGround(testbed);
                CreateProbeRoot(testbed);

                EditorSceneManager.SaveScene(scene, scenePath);
                created.Add(scenePath);
            }

            AssetDatabase.Refresh();
            UnityDebug.Log("Bloodlines visual testbed bootstrap complete. Created: " + created.Count + " | Skipped existing: " + skipped.Count);
            foreach (var path in created)
            {
                UnityDebug.Log("Created visual testbed scene: " + path);
            }
        }

        private static void CreateMetadataRoot(TestbedDefinition testbed)
        {
            var root = new GameObject("TESTBED_METADATA");
            root.transform.position = Vector3.zero;

            var descriptor = new GameObject("TESTBED_DESCRIPTOR");
            descriptor.transform.SetParent(root.transform, false);
            descriptor.name = "TESTBED_DESCRIPTOR_" + testbed.SceneFileName.Replace(".unity", string.Empty);
        }

        private static void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 16f, -18f);
            cameraObject.transform.rotation = Quaternion.Euler(33f, 0f, 0f);

            var camera = cameraObject.AddComponent<UnityCamera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.58f, 0.62f, 0.66f, 1f);
            camera.orthographic = false;
            camera.fieldOfView = 38f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 500f;
        }

        private static void CreateDirectionalLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.position = new Vector3(0f, 6f, 0f);
            lightObject.transform.rotation = Quaternion.Euler(44f, -32f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.05f;
            light.color = new Color(1f, 0.97f, 0.92f, 1f);
        }

        private static void CreateReferenceGround(TestbedDefinition testbed)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REFERENCE_GROUND";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = testbed.SceneFileName == "IconLegibility_Testbed.unity"
                ? new Vector3(0.5f, 1f, 0.5f)
                : new Vector3(2f, 1f, 2f);
        }

        private static void CreateProbeRoot(TestbedDefinition testbed)
        {
            var probeRoot = new GameObject(testbed.ProbeRootName);
            probeRoot.transform.position = Vector3.zero;
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

        private readonly struct TestbedDefinition
        {
            public TestbedDefinition(string assetFolder, string sceneFileName, string probeRootName)
            {
                AssetFolder = assetFolder;
                SceneFileName = sceneFileName;
                ProbeRootName = probeRootName;
            }

            public string AssetFolder { get; }
            public string SceneFileName { get; }
            public string ProbeRootName { get; }
        }
    }
}
