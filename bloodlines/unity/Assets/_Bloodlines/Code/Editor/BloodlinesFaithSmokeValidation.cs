#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Faith;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed faith smoke validator. Runs in isolated ECS worlds. Proves:
    ///
    ///   1. A fresh faction resolves to Unawakened level (0) with no commitment.
    ///   2. Recording exposure below the canonical threshold blocks commitment.
    ///   3. Crossing the threshold unlocks commitment; on Commit the faction
    ///      carries the selected covenant, doctrine path, intensity 20, and
    ///      Active tier.
    ///   4. Ramping intensity past each threshold lifts the tier Level.
    ///
    /// Artifact: artifacts/unity-faith-smoke.log.
    /// </summary>
    public static class BloodlinesFaithSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-faith-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Faith/Run Faith Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchFaithSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception e)
            {
                success = false;
                message = "Faith smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunBaselinePhase(out string baselineMessage))
            {
                message = baselineMessage;
                return false;
            }

            if (!RunExposureThresholdPhase(out string thresholdMessage))
            {
                message = thresholdMessage;
                return false;
            }

            if (!RunCommitmentPhase(out string commitMessage))
            {
                message = commitMessage;
                return false;
            }

            if (!RunIntensityTierPhase(out string tierMessage))
            {
                message = tierMessage;
                return false;
            }

            message =
                "Faith smoke validation passed: baselinePhase=True, exposureThresholdPhase=True, commitmentPhase=True, intensityTierPhase=True. " +
                baselineMessage + " " + thresholdMessage + " " + commitMessage + " " + tierMessage;
            return true;
        }

        private static bool RunBaselinePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFaithSmokeValidation_Baseline");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFaithState("player", out var state))
            {
                message = "Faith smoke validation failed: baseline phase could not read faith state.";
                return false;
            }

            if (state.SelectedFaith != CovenantId.None ||
                state.DoctrinePath != DoctrinePath.Unassigned ||
                state.Intensity != 0f ||
                state.Level != FaithIntensityTiers.UnawakenedLevel)
            {
                message =
                    "Faith smoke validation failed: baseline drifted. " +
                    "selected=" + state.SelectedFaith + ", path=" + state.DoctrinePath +
                    ", intensity=" + state.Intensity + ", level=" + state.Level + ".";
                return false;
            }

            message = "Baseline: selected=None, intensity=0, level=Unawakened.";
            return true;
        }

        private static bool RunExposureThresholdPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFaithSmokeValidation_Threshold");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            commandSurfaceScope.CommandSurface.TryDebugRecordFaithExposure("player", CovenantId.OldLight, 60f);

            var blockedResult = commandSurfaceScope.CommandSurface.TryDebugCommitFaith(
                "player",
                CovenantId.OldLight,
                DoctrinePath.Light);
            if (blockedResult != FaithScoring.CommitmentResult.ExposureBelowThreshold)
            {
                message =
                    "Faith smoke validation failed: threshold phase expected ExposureBelowThreshold at exposure 60, got " +
                    blockedResult + ".";
                return false;
            }

            commandSurfaceScope.CommandSurface.TryDebugGetFaithExposure("player", CovenantId.OldLight, out float exposureNow);
            message =
                "Threshold: exposureAtBlock=" + exposureNow +
                ", requiredThreshold=" + FaithIntensityTiers.CommitmentExposureThreshold +
                ", result=" + blockedResult + ".";
            return true;
        }

        private static bool RunCommitmentPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFaithSmokeValidation_Commit");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            commandSurfaceScope.CommandSurface.TryDebugRecordFaithExposure("player", CovenantId.BloodDominion, 100f);
            var result = commandSurfaceScope.CommandSurface.TryDebugCommitFaith(
                "player",
                CovenantId.BloodDominion,
                DoctrinePath.Dark);
            if (result != FaithScoring.CommitmentResult.Committed)
            {
                message =
                    "Faith smoke validation failed: commitment phase expected Committed, got " + result + ".";
                return false;
            }

            Tick(world, seconds: 0.2d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFaithState("player", out var state))
            {
                message = "Faith smoke validation failed: commitment phase lost faith state after tick.";
                return false;
            }

            if (state.SelectedFaith != CovenantId.BloodDominion ||
                state.DoctrinePath != DoctrinePath.Dark ||
                state.Intensity != FaithIntensityTiers.StartingCommitmentIntensity ||
                state.Level != FaithIntensityTiers.ActiveLevel)
            {
                message =
                    "Faith smoke validation failed: commitment phase result drifted. " +
                    "selected=" + state.SelectedFaith + ", path=" + state.DoctrinePath +
                    ", intensity=" + state.Intensity + ", level=" + state.Level + ".";
                return false;
            }

            var alreadyResult = commandSurfaceScope.CommandSurface.TryDebugCommitFaith(
                "player",
                CovenantId.OldLight,
                DoctrinePath.Light);
            if (alreadyResult != FaithScoring.CommitmentResult.AlreadyCommitted)
            {
                message =
                    "Faith smoke validation failed: commitment phase allowed recommit, got " + alreadyResult + ".";
                return false;
            }

            message =
                "Commit: selected=" + state.SelectedFaith + ", path=" + state.DoctrinePath +
                ", intensity=" + state.Intensity + ", level=" + state.Level +
                ", recommitBlock=" + alreadyResult + ".";
            return true;
        }

        private static bool RunIntensityTierPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFaithSmokeValidation_Tier");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var factionEntity = CreateFaction(entityManager, "player");
            commandSurfaceScope.CommandSurface.TryDebugRecordFaithExposure("player", CovenantId.TheOrder, 100f);
            commandSurfaceScope.CommandSurface.TryDebugCommitFaith("player", CovenantId.TheOrder, DoctrinePath.Light);

            int apexLevel = FaithIntensityTiers.ApexLevel;
            int ferventLevel = FaithIntensityTiers.FerventLevel;

            SetIntensity(entityManager, factionEntity, 80f);
            Tick(world, seconds: 0.1d);
            var apexState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            if (apexState.Level != apexLevel)
            {
                message = "Faith smoke validation failed: intensity tier phase expected Apex at 80, got level " + apexState.Level + ".";
                return false;
            }

            SetIntensity(entityManager, factionEntity, 60f);
            Tick(world, seconds: 0.1d);
            var ferventState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            if (ferventState.Level != ferventLevel)
            {
                message = "Faith smoke validation failed: intensity tier phase expected Fervent at 60, got level " + ferventState.Level + ".";
                return false;
            }

            SetIntensity(entityManager, factionEntity, 150f);
            Tick(world, seconds: 0.1d);
            var clampedState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            if (clampedState.Intensity != FaithIntensityTiers.IntensityMax || clampedState.Level != apexLevel)
            {
                message =
                    "Faith smoke validation failed: intensity tier phase did not clamp to IntensityMax. " +
                    "intensity=" + clampedState.Intensity + ", level=" + clampedState.Level + ".";
                return false;
            }

            message =
                "IntensityTier: apex@80=level" + apexState.Level +
                ", fervent@60=level" + ferventState.Level +
                ", clamped@150=" + clampedState.Intensity + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            return world;
        }

        private static void Tick(World world, double seconds)
        {
            double elapsed = world.Time.ElapsedTime;
            double target = elapsed + seconds;
            while (elapsed < target)
            {
                world.SetTime(new TimeData(elapsed, SimStepSeconds));
                world.Update();
                elapsed += SimStepSeconds;
            }
        }

        private static Entity CreateFaction(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = 0f,
                Level = FaithIntensityTiers.UnawakenedLevel,
            });
            entityManager.AddBuffer<FaithExposureElement>(entity);
            return entity;
        }

        private static void SetIntensity(EntityManager entityManager, Entity factionEntity, float intensity)
        {
            var state = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            state.Intensity = intensity;
            entityManager.SetComponentData(factionEntity, state);
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
#endif
