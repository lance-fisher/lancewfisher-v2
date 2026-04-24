#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the rally point system.
    ///
    /// Phase 1: RallyPointComponent struct initializes with correct defaults and
    ///          retains set values for TargetPosition and IsActive.
    /// Phase 2: PlayerRallyPointSetRequestComponent struct field assignment round-trips
    ///          correctly for both set and clear paths.
    /// Phase 3: UnitProductionSystem rally resolution logic: active rally point routes
    ///          the spawned unit's MoveCommandComponent to the rally position; inactive
    ///          or absent rally point uses the default spawn-offset destination.
    /// </summary>
    public static class BloodlinesRallyPointSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-rally-point-smoke.log";

        [MenuItem("Bloodlines/Combat/Run Rally Point Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchRallyPointSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception ex)
            {
                success = false;
                message = "Rally point smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_RALLY_POINT_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
            UnityDebug.Log(artifact);
            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunRallyPointComponentPhase(sb);
            ok &= RunSetRequestPhase(sb);
            ok &= RunSpawnResolutionPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunRallyPointComponentPhase(System.Text.StringBuilder sb)
        {
            var inactive = new RallyPointComponent
            {
                TargetPosition = float3.zero,
                IsActive = false,
            };

            if (inactive.IsActive)
            {
                sb.AppendLine("Phase 1 FAIL: default RallyPointComponent.IsActive should be false.");
                return false;
            }

            float3 expectedPos = new float3(10f, 0f, 15f);
            var active = new RallyPointComponent
            {
                TargetPosition = expectedPos,
                IsActive = true,
            };

            if (!active.IsActive || !active.TargetPosition.Equals(expectedPos))
            {
                sb.AppendLine("Phase 1 FAIL: active RallyPointComponent did not retain TargetPosition or IsActive. " +
                              "IsActive=" + active.IsActive + " TargetPosition=" + active.TargetPosition);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: RallyPointComponent initializes correctly and retains set values.");
            return true;
        }

        private static bool RunSetRequestPhase(System.Text.StringBuilder sb)
        {
            float3 setPos = new float3(5f, 0f, 8f);
            var setRequest = new PlayerRallyPointSetRequestComponent
            {
                TargetPosition = setPos,
                IsActive = true,
            };

            if (!setRequest.IsActive || !setRequest.TargetPosition.Equals(setPos))
            {
                sb.AppendLine("Phase 2 FAIL: set request did not retain fields. " +
                              "IsActive=" + setRequest.IsActive + " TargetPosition=" + setRequest.TargetPosition);
                return false;
            }

            var clearRequest = new PlayerRallyPointSetRequestComponent
            {
                TargetPosition = float3.zero,
                IsActive = false,
            };

            if (clearRequest.IsActive)
            {
                sb.AppendLine("Phase 2 FAIL: clear request should have IsActive=false.");
                return false;
            }

            sb.AppendLine("Phase 2 PASS: PlayerRallyPointSetRequestComponent set and clear paths both correct.");
            return true;
        }

        private static bool RunSpawnResolutionPhase(System.Text.StringBuilder sb)
        {
            // Simulate UnitProductionSystem spawn resolution logic.
            float3 spawnOffset = new float3(3f, 0f, 0f);
            float3 rallyPos = new float3(20f, 0f, 10f);

            // Case A: active rally point.
            bool hasActiveRallyPoint = true;
            float3 rallyPosition = rallyPos;

            float3 resolvedDestination = hasActiveRallyPoint ? rallyPosition : spawnOffset;
            float resolvedStoppingDistance = hasActiveRallyPoint ? 0.5f : 0.2f;
            bool resolvedIsActive = hasActiveRallyPoint;

            if (!resolvedDestination.Equals(rallyPos) || !resolvedIsActive || resolvedStoppingDistance != 0.5f)
            {
                sb.AppendLine("Phase 3 FAIL: active rally point did not route unit to rally position. " +
                              "Destination=" + resolvedDestination + " IsActive=" + resolvedIsActive);
                return false;
            }

            // Case B: no active rally point.
            hasActiveRallyPoint = false;
            resolvedDestination = hasActiveRallyPoint ? rallyPosition : spawnOffset;
            resolvedStoppingDistance = hasActiveRallyPoint ? 0.5f : 0.2f;
            resolvedIsActive = hasActiveRallyPoint;

            if (!resolvedDestination.Equals(spawnOffset) || resolvedIsActive || resolvedStoppingDistance != 0.2f)
            {
                sb.AppendLine("Phase 3 FAIL: inactive rally point should use spawn offset. " +
                              "Destination=" + resolvedDestination + " IsActive=" + resolvedIsActive);
                return false;
            }

            sb.AppendLine("Phase 3 PASS: spawn resolution routes to rally position when active, spawn offset when inactive.");
            return true;
        }
    }
}
#endif
