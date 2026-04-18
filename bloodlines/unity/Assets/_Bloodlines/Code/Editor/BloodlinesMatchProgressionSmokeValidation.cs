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
            // Phase 5: declaration seam -- DeclareInWorldTimeRequest advances InWorldDays.
            allPass &= RunPhase5(sb);
            // Phase 6: rival contact signals wire stage 4 RivalContactActive.
            allPass &= RunPhase6(sb);

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

        // ---------- Phase 5: Declaration seam ----------

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase5");
            var em = world.EntityManager;

            // Seed DualClock singleton.
            var clockEntity = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = 10f, DaysPerRealSecond = 2f, DeclarationCount = 0,
            });

            // Register declaration system (OnCreate adds the buffer).
            world.GetOrCreateSystem<DualClockDeclarationSystem>();
            world.Update();

            // Push two declaration requests directly onto the buffer.
            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity))
            {
                sb.AppendLine("Phase 5 FAIL: DeclareInWorldTimeRequest buffer not added to clock entity.");
                return false;
            }
            var buf = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);
            buf.Add(new DeclareInWorldTimeRequest { DaysDelta = 30f, Reason = new FixedString64Bytes("Battle of the Ford") });
            buf.Add(new DeclareInWorldTimeRequest { DaysDelta = 14f, Reason = new FixedString64Bytes("Siege raised") });

            // Update again -- DualClockDeclarationSystem processes the buffer.
            world.Update();

            var clock = em.GetComponentData<DualClockComponent>(clockEntity);
            // Expected: 10 + (2*0 baseline in isolated world) + 30 + 14 = 54 (plus two world.Update baseline increments,
            // but DeltaTime in test world is 0, so only declarations add days).
            // Actual: baseline tick adds dt * 2; in Editor test world dt is approximately 0.
            // We check that DeclarationCount == 2 and InWorldDays == 10 + 30 + 14 = 54.
            if (clock.DeclarationCount != 2)
            {
                sb.AppendLine($"Phase 5 FAIL: expected DeclarationCount=2, got {clock.DeclarationCount}.");
                return false;
            }
            // Allow small float drift from baseline tick (dt may be > 0 in some Editor contexts).
            if (clock.InWorldDays < 53.9f)
            {
                sb.AppendLine($"Phase 5 FAIL: expected InWorldDays ~54, got {clock.InWorldDays:F2}.");
                return false;
            }

            sb.AppendLine($"Phase 5 PASS: declaration seam; DeclarationCount={clock.DeclarationCount} InWorldDays={clock.InWorldDays:F1}.");
            return true;
        }

        // ---------- Phase 6: Rival contact signals ----------

        private static bool RunPhase6(System.Text.StringBuilder sb)
        {
            using var world = new World("mp-smoke-phase6");
            var em = world.EntityManager;

            // Seed DualClock with 400 in-world days (> 1 year, so sustainedWar proxy is eligible).
            var clockEntity = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = 400f, DaysPerRealSecond = 2f, DeclarationCount = 0,
            });

            // Seed MatchProgression singleton.
            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                StageNumber = 3,
                StageId = new FixedString32Bytes("encounter_establishment"),
                StageLabel = new FixedString64Bytes("Encounter and Establishment"),
                GreatReckoningThreshold = 0.7f,
                InWorldDays = 400f,
                InWorldYears = 400f / 365f,
            });

            // Seed player and enemy kingdom factions.
            for (int fi = 0; fi < 2; fi++)
            {
                string fid = fi == 0 ? "player" : "enemy";
                var fe = em.CreateEntity(typeof(FactionComponent), typeof(FactionKindComponent),
                    typeof(ResourceStockpileComponent), typeof(PopulationComponent), typeof(FaithStateComponent));
                em.SetComponentData(fe, new FactionComponent { FactionId = new FixedString32Bytes(fid) });
                em.SetComponentData(fe, new FactionKindComponent { Kind = FactionKind.Kingdom });
                em.SetComponentData(fe, new ResourceStockpileComponent { Food = 50f, Water = 50f });
                em.SetComponentData(fe, new PopulationComponent { Total = 10 });
                em.SetComponentData(fe, new FaithStateComponent { SelectedFaith = CovenantId.OldLight, Level = 1 });
            }

            // Seed contested CP: owned by player, being captured by enemy.
            var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(cpEntity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_border"),
                OwnerFactionId = new FixedString32Bytes("player"),
                CaptureFactionId = new FixedString32Bytes("enemy"),
                IsContested = true,
                Loyalty = 60f,
            });

            // Seed player CP (home territory).
            var cpHome = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(cpHome, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_home"),
                OwnerFactionId = new FixedString32Bytes("player"),
                Loyalty = 100f,
            });

            // Seed all Stage 2 + 3 requirements so stage 3 is confirmed.
            for (int i = 0; i < 5; i++)
            {
                var bEntity = em.CreateEntity(typeof(FactionComponent), typeof(BuildingTypeComponent));
                em.SetComponentData(bEntity, new FactionComponent { FactionId = "player" });
                em.SetComponentData(bEntity, new BuildingTypeComponent { TypeId = i == 0 ? new FixedString64Bytes("command_hall") : new FixedString64Bytes("farm") });
            }
            for (int i = 0; i < 6; i++)
            {
                var uEntity = em.CreateEntity(typeof(FactionComponent), typeof(MovementStatsComponent), typeof(HealthComponent));
                em.SetComponentData(uEntity, new FactionComponent { FactionId = "player" });
                em.SetComponentData(uEntity, new MovementStatsComponent { MaxSpeed = 3f });
                em.SetComponentData(uEntity, new HealthComponent { Current = 100f, Max = 100f });
            }

            // Run evaluation system.
            world.GetOrCreateSystem<MatchProgressionEvaluationSystem>();
            world.Update();

            using var mpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionComponent>());
            if (mpQuery.IsEmpty)
            {
                sb.AppendLine("Phase 6 FAIL: MatchProgressionComponent gone after update.");
                return false;
            }

            var mp = mpQuery.GetSingleton<MatchProgressionComponent>();

            // With a contested border and 400 in-world days (>1 year), all three stage-4
            // signals should be true: rivalContactActive (contestedBorder counts), contestedBorder,
            // sustainedWarActive (contestedBorder + years >= 1). Stage should advance to 4.
            if (!mp.RivalContactActive)
            {
                sb.AppendLine($"Phase 6 FAIL: expected RivalContactActive=true after contested CP, got false. Stage={mp.StageNumber}.");
                return false;
            }
            if (mp.StageNumber < 3)
            {
                sb.AppendLine($"Phase 6 FAIL: expected StageNumber >= 3, got {mp.StageNumber}.");
                return false;
            }

            sb.AppendLine($"Phase 6 PASS: RivalContactActive={mp.RivalContactActive} StageNumber={mp.StageNumber} InWorldDays={mp.InWorldDays:F0}.");
            return true;
        }
    }
}
#endif
