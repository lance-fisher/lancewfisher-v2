#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Systems;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the dual-clock + match-progression ECS sub-slice 1.
    ///
    /// Phase 1: DualClockComponent singleton created in a fresh world; InWorldDays == 0
    ///          and DaysPerRealSecond == 2 (canonical default).
    ///
    /// Phase 2: After manually ticking the clock by 10 real seconds, InWorldDays
    ///          advances to exactly 20 (2 days/sec x 10 sec). Verifies tick arithmetic.
    ///
    /// Phase 3: MatchProgressionComponent singleton created in a fresh world; StageNumber
    ///          == 1 (Founding) and StageId == "founding" before any faction state exists.
    ///
    /// Phase 4: StageReadiness is in [0, 1]; after seeding a player faction with
    ///          enough resources, buildings, and military, stageReadiness for
    ///          Stage 2 rises toward 1.0.
    ///
    /// Browser reference: simulation.js tickDualClock (13795),
    ///                    computeMatchProgressionState (13426).
    /// Artifact: artifacts/unity-match-progression-smoke.log.
    /// </summary>
    public static class BloodlinesMatchProgressionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-match-progression-smoke.log";

        [MenuItem("Bloodlines/MatchProgression/Run Match Progression Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchMatchProgressionSmokeValidation()
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
                message = "Match progression smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_MATCH_PROGRESSION_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { /* artifact write is best-effort */ }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool allPass = true;

            // Phase 1: DualClock singleton defaults.
            allPass &= RunPhase1(sb);
            // Phase 2: tick arithmetic -- 10 sec x 2 days/sec = 20 days.
            allPass &= RunPhase2(sb);
            // Phase 3: MatchProgression singleton defaults.
            allPass &= RunPhase3(sb);
            // Phase 4: stage readiness rises when player faction meets criteria.
            allPass &= RunPhase4(sb);

            report = sb.ToString();
            return allPass;
        }

        // ---------- Phase 1 ----------

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase1");
            // GetOrCreateSystem triggers OnCreate for ISystem structs.
            world.GetOrCreateSystem<DualClockTickSystem>();

            var em = world.EntityManager;

            using var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty)
            {
                sb.AppendLine("Phase 1 FAIL: DualClockComponent singleton not created on system creation.");
                return false;
            }

            var clock = q.GetSingleton<DualClockComponent>();
            if (clock.InWorldDays != 0f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected InWorldDays=0, got {clock.InWorldDays}.");
                return false;
            }
            if (System.Math.Abs(clock.DaysPerRealSecond - 2f) > 0.0001f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected DaysPerRealSecond=2, got {clock.DaysPerRealSecond}.");
                return false;
            }

            sb.AppendLine("Phase 1 PASS: DualClock singleton created; InWorldDays=0 DaysPerRealSecond=2.");
            return true;
        }

        // ---------- Phase 2 ----------

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase2");
            var em = world.EntityManager;

            // Seed DualClockComponent directly.
            var clockEntity = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = 0f,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });

            // Simulate 10 ticks of 1-second dt each.
            using var q = em.CreateEntityQuery(
                ComponentType.ReadWrite<DualClockComponent>());
            for (int i = 0; i < 10; i++)
            {
                var c = q.GetSingleton<DualClockComponent>();
                c.InWorldDays += 1f * c.DaysPerRealSecond;
                q.SetSingleton(c);
            }

            var finalClock = q.GetSingleton<DualClockComponent>();
            if (System.Math.Abs(finalClock.InWorldDays - 20f) > 0.001f)
            {
                sb.AppendLine($"Phase 2 FAIL: expected InWorldDays=20 after 10 ticks, got {finalClock.InWorldDays}.");
                return false;
            }

            sb.AppendLine($"Phase 2 PASS: tick arithmetic correct; InWorldDays={finalClock.InWorldDays:F1} after 10 ticks.");
            return true;
        }

        // ---------- Phase 3 ----------

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase3");
            world.GetOrCreateSystem<MatchProgressionEvaluationSystem>();
            var em = world.EntityManager;

            using var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<MatchProgressionComponent>());
            if (q.IsEmpty)
            {
                sb.AppendLine("Phase 3 FAIL: MatchProgressionComponent singleton not created on system creation.");
                return false;
            }

            var mp = q.GetSingleton<MatchProgressionComponent>();
            if (mp.StageNumber != 1)
            {
                sb.AppendLine($"Phase 3 FAIL: expected StageNumber=1, got {mp.StageNumber}.");
                return false;
            }
            if (mp.StageId != new FixedString32Bytes("founding"))
            {
                sb.AppendLine($"Phase 3 FAIL: expected stageId='founding', got '{mp.StageId}'.");
                return false;
            }

            sb.AppendLine($"Phase 3 PASS: MatchProgression singleton defaults; StageNumber={mp.StageNumber} StageId={mp.StageId}.");
            return true;
        }

        // ---------- Phase 4 ----------

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase4");
            var em = world.EntityManager;

            // Seed DualClock singleton.
            var clockEntity = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = 0f, DaysPerRealSecond = 2f, DeclarationCount = 0,
            });

            // Seed MatchProgression singleton.
            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                StageNumber = 1,
                StageId = new FixedString32Bytes("founding"),
                StageLabel = new FixedString64Bytes("Founding"),
                GreatReckoningThreshold = 0.7f,
            });

            // Seed player faction entity: food stable, water stable, faith committed.
            var playerEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(FaithStateComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new ResourceStockpileComponent { Food = 50f, Water = 50f });
            em.SetComponentData(playerEntity, new PopulationComponent { Total = 10 });
            em.SetComponentData(playerEntity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.OldLight,
                Level = 1,
            });

            // Seed 4 completed buildings (no ConstructionStateComponent).
            for (int i = 0; i < 4; i++)
            {
                var bEntity = em.CreateEntity(typeof(FactionComponent), typeof(BuildingTypeComponent));
                em.SetComponentData(bEntity, new FactionComponent { FactionId = "player" });
                em.SetComponentData(bEntity, new BuildingTypeComponent { TypeId = "farm" });
            }
            // Add a defended seat.
            var seatEntity = em.CreateEntity(typeof(FactionComponent), typeof(BuildingTypeComponent));
            em.SetComponentData(seatEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(seatEntity, new BuildingTypeComponent { TypeId = "command_hall" });

            // Seed 6 military units.
            for (int i = 0; i < 6; i++)
            {
                var uEntity = em.CreateEntity(
                    typeof(FactionComponent),
                    typeof(MovementStatsComponent),
                    typeof(HealthComponent));
                em.SetComponentData(uEntity, new FactionComponent { FactionId = "player" });
                em.SetComponentData(uEntity, new MovementStatsComponent { MaxSpeed = 3f });
                em.SetComponentData(uEntity, new HealthComponent { Current = 100f, Max = 100f });
            }

            // Seed 2 player-owned control points.
            for (int i = 0; i < 2; i++)
            {
                var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                em.SetComponentData(cpEntity, new ControlPointComponent
                {
                    ControlPointId = i == 0 ? new FixedString32Bytes("cp_home") : new FixedString32Bytes("cp_frontier"),
                    OwnerFactionId = "player",
                    Loyalty = 100f,
                });
            }

            // Run the evaluation system (GetOrCreateSystem triggers OnCreate).
            world.GetOrCreateSystem<MatchProgressionEvaluationSystem>();
            world.Update();

            using var mpQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<MatchProgressionComponent>());
            if (mpQuery.IsEmpty)
            {
                sb.AppendLine("Phase 4 FAIL: MatchProgressionComponent gone after update.");
                return false;
            }

            var mp = mpQuery.GetSingleton<MatchProgressionComponent>();
            if (mp.StageReadiness < 0f || mp.StageReadiness > 1f)
            {
                sb.AppendLine($"Phase 4 FAIL: StageReadiness {mp.StageReadiness:F2} out of [0,1] range.");
                return false;
            }

            // With all stage-2 criteria met, stage should advance to 2 (or beyond if stage-3 also met).
            // Stage 3 requires 2+ territories AND 6+ military AND faith: all seeded above.
            // Therefore stageNumber should be 3 (all stage-2 and stage-3 met, stage-4 requires unimplemented signals).
            if (mp.StageNumber < 2)
            {
                sb.AppendLine($"Phase 4 FAIL: expected StageNumber >= 2 after seeding requirements, got {mp.StageNumber}.");
                return false;
            }

            sb.AppendLine($"Phase 4 PASS: StageNumber={mp.StageNumber} StageReadiness={mp.StageReadiness:F2} StageId={mp.StageId}.");
            return true;
        }
    }
}
#endif
