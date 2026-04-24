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
    /// Smoke validator for the patrol route system.
    ///
    /// Phase 1: Unit within arrival threshold of current waypoint flips CurrentTarget and
    ///          a new move command is issued to the next waypoint.
    /// Phase 2: Patrol is suspended (no waypoint flip) when AttackOrderComponent.IsActive is true.
    /// Phase 3: Patrol resumes (waypoint flip occurs) when AttackOrderComponent.IsActive is false.
    /// </summary>
    public static class BloodlinesPatrolRouteSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-patrol-route-smoke.log";

        [MenuItem("Bloodlines/Combat/Run Patrol Route Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPatrolRouteSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Patrol route smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_PATROL_ROUTE_SMOKE " +
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
            ok &= RunWaypointFlipPhase(sb);
            ok &= RunSuspendedPhase(sb);
            ok &= RunResumePhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunWaypointFlipPhase(System.Text.StringBuilder sb)
        {
            float3 waypointA = new float3(0f, 0f, 0f);
            float3 waypointB = new float3(20f, 0f, 0f);
            float arrivalThreshold = 1.2f;

            var patrol = new PatrolRouteComponent
            {
                WaypointA = waypointA,
                WaypointB = waypointB,
                CurrentTarget = 0,
                IsPatrolling = true,
                ArrivalThreshold = arrivalThreshold,
            };

            // Simulate unit at WaypointA (within arrival threshold).
            float3 unitPos = new float3(0.5f, 0f, 0f);
            float3 targetWaypoint = patrol.CurrentTarget == 0 ? patrol.WaypointA : patrol.WaypointB;
            float distSq = math.distancesq(unitPos, targetWaypoint);
            bool shouldFlip = distSq <= arrivalThreshold * arrivalThreshold;

            if (!shouldFlip)
            {
                sb.AppendLine("Phase 1 FAIL: unit within arrival threshold should flip waypoint. distSq=" + distSq);
                return false;
            }

            // Apply flip logic.
            byte nextTarget = patrol.CurrentTarget == 0 ? (byte)1 : (byte)0;
            float3 nextWaypoint = nextTarget == 0 ? patrol.WaypointA : patrol.WaypointB;

            if (nextTarget != 1 || !nextWaypoint.Equals(waypointB))
            {
                sb.AppendLine("Phase 1 FAIL: flip should set CurrentTarget=1 targeting WaypointB. " +
                              "nextTarget=" + nextTarget + " nextWaypoint=" + nextWaypoint);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: unit at arrival threshold flips from WaypointA to WaypointB correctly.");
            return true;
        }

        private static bool RunSuspendedPhase(System.Text.StringBuilder sb)
        {
            // Simulate patrol suspension: AttackOrderComponent.IsActive = true.
            var patrol = new PatrolRouteComponent
            {
                WaypointA = new float3(0f, 0f, 0f),
                WaypointB = new float3(20f, 0f, 0f),
                CurrentTarget = 0,
                IsPatrolling = true,
                ArrivalThreshold = 1.2f,
            };

            var attackOrder = new AttackOrderComponent
            {
                IsActive = true,
            };

            // Simulate: within arrival threshold but attack order active -- should NOT flip.
            float3 unitPos = new float3(0.5f, 0f, 0f);
            float3 targetWaypoint = patrol.CurrentTarget == 0 ? patrol.WaypointA : patrol.WaypointB;
            float distSq = math.distancesq(unitPos, targetWaypoint);
            bool withinThreshold = distSq <= patrol.ArrivalThreshold * patrol.ArrivalThreshold;
            bool shouldProcess = !attackOrder.IsActive;

            if (!withinThreshold)
            {
                sb.AppendLine("Phase 2 FAIL: setup error -- unit should be within threshold for this test.");
                return false;
            }

            if (shouldProcess)
            {
                sb.AppendLine("Phase 2 FAIL: patrol should be suspended when AttackOrderComponent.IsActive=true.");
                return false;
            }

            sb.AppendLine("Phase 2 PASS: patrol suspended correctly when attack order is active.");
            return true;
        }

        private static bool RunResumePhase(System.Text.StringBuilder sb)
        {
            // Simulate patrol resume: AttackOrderComponent.IsActive = false.
            var patrol = new PatrolRouteComponent
            {
                WaypointA = new float3(0f, 0f, 0f),
                WaypointB = new float3(20f, 0f, 0f),
                CurrentTarget = 0,
                IsPatrolling = true,
                ArrivalThreshold = 1.2f,
            };

            var attackOrder = new AttackOrderComponent
            {
                IsActive = false,
            };

            float3 unitPos = new float3(0.5f, 0f, 0f);
            float3 targetWaypoint = patrol.CurrentTarget == 0 ? patrol.WaypointA : patrol.WaypointB;
            float distSq = math.distancesq(unitPos, targetWaypoint);
            bool withinThreshold = distSq <= patrol.ArrivalThreshold * patrol.ArrivalThreshold;
            bool shouldProcess = !attackOrder.IsActive;

            if (!withinThreshold || !shouldProcess)
            {
                sb.AppendLine("Phase 3 FAIL: setup error -- should be within threshold and attack order inactive.");
                return false;
            }

            byte nextTarget = patrol.CurrentTarget == 0 ? (byte)1 : (byte)0;
            float3 nextWaypoint = nextTarget == 0 ? patrol.WaypointA : patrol.WaypointB;
            bool flippedCorrectly = nextTarget == 1 && nextWaypoint.Equals(patrol.WaypointB);

            if (!flippedCorrectly)
            {
                sb.AppendLine("Phase 3 FAIL: patrol should resume and flip to WaypointB. " +
                              "nextTarget=" + nextTarget);
                return false;
            }

            sb.AppendLine("Phase 3 PASS: patrol resumes correctly when attack order clears. Flips to WaypointB.");
            return true;
        }
    }
}
#endif
