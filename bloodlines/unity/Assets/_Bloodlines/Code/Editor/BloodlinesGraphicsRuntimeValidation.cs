#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Bloodlines.Rendering;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed graphics runtime validator. Opens the Bootstrap scene,
    /// ensures a BloodlinesVisualPresentationBridge exists on the authoring
    /// anchor, enters Play Mode, then asserts that every spawned unit and
    /// building with a matching UnitVisualDefinition / BuildingVisualDefinition
    /// has a live mesh + material via the visual bridge, and that it carries
    /// a FactionTintComponent. Governance artifact writes to
    /// artifacts/unity-graphics-runtime.log.
    ///
    /// This validator does NOT need every unit or building to have a visual
    /// definition. It only asserts that entities for which a visual definition
    /// exists are actually bound and rendered through the new pipeline.
    /// </summary>
    [InitializeOnLoad]
    public static class BloodlinesGraphicsRuntimeValidation
    {
        private const string BootstrapScenePath = "Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity";
        private const string ArtifactPath = "../artifacts/unity-graphics-runtime.log";
        private const string PendingKey = "Bloodlines.GraphicsRuntimeValidation.Pending";
        private const string BatchModeKey = "Bloodlines.GraphicsRuntimeValidation.BatchMode";
        private const double StartupTimeoutSeconds = 60d;
        private const double WarmupSeconds = 2d;

        static BloodlinesGraphicsRuntimeValidation()
        {
            EditorApplication.update -= ContinuePendingValidation;
            EditorApplication.update += ContinuePendingValidation;
        }

        [MenuItem("Bloodlines/Graphics/Run Graphics Runtime Validation")]
        public static void RunInteractive()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            StartValidation(batchMode: false);
        }

        public static void RunBatch()
        {
            StartValidation(batchMode: true);
        }

        private static void StartValidation(bool batchMode)
        {
            try
            {
                EnsureBridgeAttached();
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                SessionState.SetBool(PendingKey, true);
                SessionState.SetBool(BatchModeKey, batchMode);
                SessionState.SetString("Bloodlines.GraphicsRuntimeValidation.DeadlineTicks",
                    DateTime.UtcNow.AddSeconds(StartupTimeoutSeconds).Ticks.ToString());
                EditorApplication.isPlaying = true;
            }
            catch (Exception e)
            {
                WriteResult(batchMode, success: false, "Graphics runtime validation failed at startup: " + e);
            }
        }

        private static void EnsureBridgeAttached()
        {
            if (!File.Exists(BootstrapScenePath))
            {
                throw new InvalidOperationException("Bootstrap scene not found: " + BootstrapScenePath);
            }

            var scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            var authoring = UnityEngine.Object.FindFirstObjectByType<Bloodlines.Authoring.BloodlinesMapBootstrapAuthoring>();
            if (authoring == null)
            {
                throw new InvalidOperationException("BloodlinesMapBootstrapAuthoring not found in Bootstrap scene.");
            }

            if (authoring.GetComponent<BloodlinesVisualPresentationBridge>() == null)
            {
                authoring.gameObject.AddComponent<BloodlinesVisualPresentationBridge>();
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static void ContinuePendingValidation()
        {
            if (!SessionState.GetBool(PendingKey, false))
            {
                return;
            }

            try
            {
                if (HasTimedOut())
                {
                    WriteResult(SessionState.GetBool(BatchModeKey, false), success: false,
                        "Graphics runtime validation timed out after " + StartupTimeoutSeconds + "s.");
                    return;
                }

                if (!EditorApplication.isPlaying)
                {
                    return;
                }

                if (EditorApplication.isPaused)
                {
                    return;
                }

                if (Time.timeSinceLevelLoad < WarmupSeconds)
                {
                    return;
                }

                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null || !world.IsCreated)
                {
                    return;
                }

                var bridge = BloodlinesVisualPresentationBridge.ActiveInstance;
                if (bridge == null)
                {
                    WriteResult(SessionState.GetBool(BatchModeKey, false), success: false,
                        "BloodlinesVisualPresentationBridge.ActiveInstance is null in Play Mode.");
                    return;
                }

                var diagnostics = EvaluateBridge(world.EntityManager, bridge);
                bool success = string.IsNullOrEmpty(diagnostics.FailureReason);
                string summary = success
                    ? "Graphics runtime validation passed: unitProxies=" + diagnostics.UnitProxies +
                      ", buildingProxies=" + diagnostics.BuildingProxies +
                      ", factionTintAttached=" + diagnostics.FactionTintAttached +
                      ", expectedUnitsWithDefinition=" + diagnostics.ExpectedUnitsWithDefinition +
                      ", expectedBuildingsWithDefinition=" + diagnostics.ExpectedBuildingsWithDefinition + "."
                    : diagnostics.FailureReason;
                WriteResult(SessionState.GetBool(BatchModeKey, false), success, summary);
            }
            catch (Exception e)
            {
                WriteResult(SessionState.GetBool(BatchModeKey, false), success: false,
                    "Graphics runtime validation errored: " + e);
            }
        }

        private static bool HasTimedOut()
        {
            string raw = SessionState.GetString("Bloodlines.GraphicsRuntimeValidation.DeadlineTicks", string.Empty);
            if (string.IsNullOrWhiteSpace(raw) || !long.TryParse(raw, out long ticks))
            {
                return false;
            }

            return DateTime.UtcNow.Ticks > ticks;
        }

        private readonly struct Diagnostics
        {
            public readonly int UnitProxies;
            public readonly int BuildingProxies;
            public readonly int FactionTintAttached;
            public readonly int ExpectedUnitsWithDefinition;
            public readonly int ExpectedBuildingsWithDefinition;
            public readonly string FailureReason;

            public Diagnostics(int unitProxies, int buildingProxies, int factionTint,
                int expectedUnits, int expectedBuildings, string failureReason)
            {
                UnitProxies = unitProxies;
                BuildingProxies = buildingProxies;
                FactionTintAttached = factionTint;
                ExpectedUnitsWithDefinition = expectedUnits;
                ExpectedBuildingsWithDefinition = expectedBuildings;
                FailureReason = failureReason;
            }
        }

        private static Diagnostics EvaluateBridge(EntityManager em, BloodlinesVisualPresentationBridge bridge)
        {
            int unitProxies = 0;
            int buildingProxies = 0;
            int factionTintAttached = 0;
            int expectedUnitsWithDefinition = 0;
            int expectedBuildingsWithDefinition = 0;
            string failure = null;

            var unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using (var entities = unitQuery.ToEntityArray(Allocator.Temp))
            using (var types = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp))
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (!BloodlinesVisualCatalog.TryGetUnitVisual(types[i].TypeId.ToString(), out var def))
                    {
                        continue;
                    }

                    expectedUnitsWithDefinition++;
                    if (bridge.TryGetVisualProxyMeshAndMaterial(entities[i], out var mesh, out var material))
                    {
                        unitProxies++;
                        if (mesh == null || material == null)
                        {
                            failure = "Unit entity " + entities[i].Index + " (" + types[i].TypeId + ") has null mesh or material via visual bridge.";
                            break;
                        }
                    }
                    else
                    {
                        failure = "Unit entity " + entities[i].Index + " (" + types[i].TypeId + ") has a UnitVisualDefinition but no visual proxy in the bridge.";
                        break;
                    }

                    if (em.HasComponent<FactionTintComponent>(entities[i]))
                    {
                        factionTintAttached++;
                    }
                    else
                    {
                        failure = "Unit entity " + entities[i].Index + " missing FactionTintComponent.";
                        break;
                    }
                }
            }

            if (failure == null)
            {
                var buildingQuery = em.CreateEntityQuery(
                    ComponentType.ReadOnly<BuildingTypeComponent>(),
                    ComponentType.ReadOnly<FactionComponent>());
                using var entities = buildingQuery.ToEntityArray(Allocator.Temp);
                using var types = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
                for (int i = 0; i < entities.Length; i++)
                {
                    if (!BloodlinesVisualCatalog.TryGetBuildingVisual(types[i].TypeId.ToString(), out var def))
                    {
                        continue;
                    }

                    expectedBuildingsWithDefinition++;
                    if (bridge.TryGetVisualProxyMeshAndMaterial(entities[i], out var mesh, out var material))
                    {
                        buildingProxies++;
                        if (mesh == null || material == null)
                        {
                            failure = "Building entity " + entities[i].Index + " (" + types[i].TypeId + ") has null mesh or material via visual bridge.";
                            break;
                        }
                    }
                    else
                    {
                        failure = "Building entity " + entities[i].Index + " (" + types[i].TypeId + ") has a BuildingVisualDefinition but no visual proxy in the bridge.";
                        break;
                    }

                    if (em.HasComponent<FactionTintComponent>(entities[i]))
                    {
                        factionTintAttached++;
                    }
                    else
                    {
                        failure = "Building entity " + entities[i].Index + " missing FactionTintComponent.";
                        break;
                    }
                }
            }

            return new Diagnostics(unitProxies, buildingProxies, factionTintAttached,
                expectedUnitsWithDefinition, expectedBuildingsWithDefinition, failure);
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            SessionState.SetBool(PendingKey, false);
            EditorApplication.isPlaying = false;
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
                UnityDebug.Log(message);
            }
            catch
            {
                UnityDebug.Log(message);
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
#endif
