#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Victory;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the Victory Conditions system.
    ///
    /// Phase 1: Enemy command hall dead -> Won, CommandHallFall.
    /// Phase 2: Player command hall dead -> Lost, CommandHallFall.
    /// Phase 3: All player control points loyal (hold pre-seeded near threshold) -> Won, TerritorialGovernance.
    /// Phase 4: Player faith Level=5 Intensity=85 -> Won, DivinRight.
    ///
    /// Artifact: artifacts/unity-victory-conditions-smoke.log.
    /// </summary>
    public static class BloodlinesVictoryConditionsSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-victory-conditions-smoke.log";

        [MenuItem("Bloodlines/AI/Run Victory Conditions Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchVictoryConditionsSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Victory conditions smoke errored: " + e;
            }

            string artifact = "BLOODLINES_VICTORY_CONDITIONS_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhase1(sb);
            ok &= RunPhase2(sb);
            ok &= RunPhase3(sb);
            ok &= RunPhase4(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return sg;
        }

        // --------- Phase 1: enemy command hall dead -> Won, CommandHallFall ---------

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("victory-phase1");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionEvaluationSystem>());

            var victoryEntity = em.CreateEntity(typeof(VictoryStateComponent));
            em.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status    = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
            });

            // Dead enemy command hall.
            var hall = em.CreateEntity(typeof(BuildingTypeComponent), typeof(FactionComponent), typeof(DeadTag));
            em.SetComponentData(hall, new BuildingTypeComponent { TypeId = "command_hall" });
            em.SetComponentData(hall, new FactionComponent { FactionId = "enemy" });

            world.Update();

            var result = em.GetComponentData<VictoryStateComponent>(victoryEntity);
            if (result.Status != MatchStatus.Won)
            {
                sb.AppendLine($"Phase 1 FAIL: expected Won, got {result.Status}."); return false;
            }
            if (result.VictoryType != VictoryConditionId.CommandHallFall)
            {
                sb.AppendLine($"Phase 1 FAIL: expected CommandHallFall, got {result.VictoryType}."); return false;
            }
            sb.AppendLine($"Phase 1 PASS: Status={result.Status} VictoryType={result.VictoryType}.");
            return true;
        }

        // --------- Phase 2: player command hall dead -> Lost, CommandHallFall ---------

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("victory-phase2");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionEvaluationSystem>());

            var victoryEntity = em.CreateEntity(typeof(VictoryStateComponent));
            em.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status    = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
            });

            // Dead player command hall.
            var hall = em.CreateEntity(typeof(BuildingTypeComponent), typeof(FactionComponent), typeof(DeadTag));
            em.SetComponentData(hall, new BuildingTypeComponent { TypeId = "command_hall" });
            em.SetComponentData(hall, new FactionComponent { FactionId = "player" });

            world.Update();

            var result = em.GetComponentData<VictoryStateComponent>(victoryEntity);
            if (result.Status != MatchStatus.Lost)
            {
                sb.AppendLine($"Phase 2 FAIL: expected Lost, got {result.Status}."); return false;
            }
            if (result.VictoryType != VictoryConditionId.CommandHallFall)
            {
                sb.AppendLine($"Phase 2 FAIL: expected CommandHallFall, got {result.VictoryType}."); return false;
            }
            sb.AppendLine($"Phase 2 PASS: Status={result.Status} VictoryType={result.VictoryType}.");
            return true;
        }

        // --------- Phase 3: territorial governance hold -> Won ---------

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("victory-phase3");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionEvaluationSystem>());

            // Pre-seed hold timer one tick below threshold (dt=0.016 will push it over).
            var victoryEntity = em.CreateEntity(typeof(VictoryStateComponent));
            em.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status    = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
                TerritorialGovernanceHoldSeconds =
                    VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds - 0.001f,
            });

            // Two loyal player control points.
            var cp1 = em.CreateEntity(typeof(ControlPointComponent), typeof(FactionComponent));
            em.SetComponentData(cp1, new ControlPointComponent { Loyalty = 95f });
            em.SetComponentData(cp1, new FactionComponent { FactionId = "player" });

            var cp2 = em.CreateEntity(typeof(ControlPointComponent), typeof(FactionComponent));
            em.SetComponentData(cp2, new ControlPointComponent { Loyalty = 92f });
            em.SetComponentData(cp2, new FactionComponent { FactionId = "player" });

            world.Update();

            var result = em.GetComponentData<VictoryStateComponent>(victoryEntity);
            if (result.Status != MatchStatus.Won)
            {
                sb.AppendLine($"Phase 3 FAIL: expected Won, got {result.Status} (hold={result.TerritorialGovernanceHoldSeconds})."); return false;
            }
            if (result.VictoryType != VictoryConditionId.TerritorialGovernance)
            {
                sb.AppendLine($"Phase 3 FAIL: expected TerritorialGovernance, got {result.VictoryType}."); return false;
            }
            sb.AppendLine($"Phase 3 PASS: Status={result.Status} VictoryType={result.VictoryType} HoldSeconds={result.TerritorialGovernanceHoldSeconds:F3}.");
            return true;
        }

        // --------- Phase 4: divine right faith Level=5 Intensity=85 -> Won ---------

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("victory-phase4");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionEvaluationSystem>());

            var victoryEntity = em.CreateEntity(typeof(VictoryStateComponent));
            em.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status    = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
            });

            // Player faction at Level=5 Intensity=85 (above threshold of 80).
            var faction = em.CreateEntity(typeof(FactionComponent), typeof(FaithStateComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "player" });
            em.SetComponentData(faction, new FaithStateComponent
            {
                Level     = VictoryConditionEvaluationSystem.DivinRightFaithLevel,
                Intensity = VictoryConditionEvaluationSystem.DivinRightIntensityThreshold + 5f,
            });

            world.Update();

            var result = em.GetComponentData<VictoryStateComponent>(victoryEntity);
            if (result.Status != MatchStatus.Won)
            {
                sb.AppendLine($"Phase 4 FAIL: expected Won, got {result.Status}."); return false;
            }
            if (result.VictoryType != VictoryConditionId.DivinRight)
            {
                sb.AppendLine($"Phase 4 FAIL: expected DivinRight, got {result.VictoryType}."); return false;
            }
            sb.AppendLine($"Phase 4 PASS: Status={result.Status} VictoryType={result.VictoryType}.");
            return true;
        }
    }
}
#endif
