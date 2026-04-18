#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for AIStrategicPressureSystem. Verifies four phases:
    ///
    ///   Phase 1: stage 1, no world pressure -> RivalryUnlocked=false,
    ///            TerritoryTimer floored at >=24 (TerritoryTimerRivalryFloor).
    ///   Phase 2: stage 3, player WP Level=0 -> RivalryUnlocked=true,
    ///            RaidPressureUnlocked=true, AttackTimer ticked below initial 20s.
    ///   Phase 3: stage 3, player WP Level=2 -> AttackTimer <=8, TerritoryTimer <=6.
    ///   Phase 4: stage 3, player WP Level=2, GreatReckoningScore=4 ->
    ///            AttackTimer <=6, TerritoryTimer <=4, RaidTimer <=8.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1127-1241 (timer clamp/floor block).
    /// Artifact: artifacts/unity-ai-strategic-pressure-smoke.log.
    /// </summary>
    public static class BloodlinesAIStrategicPressureSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-ai-strategic-pressure-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Strategic Pressure Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIStrategicPressureSmokeValidation() =>
            RunInternal(batchMode: true);

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
                message = "AI strategic pressure smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_STRATEGIC_PRESSURE_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
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

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIStrategicPressureSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity SeedMatchProgression(EntityManager em, int stageNumber)
        {
            var e = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(e, new MatchProgressionComponent { StageNumber = stageNumber });
            return e;
        }

        private static Entity SeedPlayerFaction(EntityManager em, int wpLevel,
            int greatReckoningScore = 0)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new WorldPressureComponent
            {
                Level                   = wpLevel,
                Score                   = wpLevel > 0 ? 4 : 0,
                Streak                  = wpLevel > 0 ? 3 : 0,
                Targeted                = false,
                TerritoryExpansionScore = wpLevel > 0 && greatReckoningScore == 0 ? 4 : 0,
                GreatReckoningScore     = greatReckoningScore,
            });
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        private static Entity SeedAIFaction(EntityManager em, float attackTimer = 20f,
            float territoryTimer = 12f, float raidTimer = 18f)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(e, new AIStrategyComponent
            {
                AttackTimer                          = attackTimer,
                TerritoryTimer                       = territoryTimer,
                RaidTimer                            = raidTimer,
                MarriageProposalTimer                = 90f,
                HolyWarTimer                         = 95f,
                AssassinationTimer                   = 80f,
                MissionaryTimer                      = 70f,
                ExpansionIntervalSeconds             = 8f,
                ScoutHarassIntervalSeconds           = 12f,
                WorldPressureResponseIntervalSeconds = 15f,
                ReinforcementIntervalSeconds         = 10f,
            });
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        // ------------------------------------------------------------------ Phase 1

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("pressure-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);

            SeedMatchProgression(em, 1);
            SeedPlayerFaction(em, wpLevel: 0);
            Entity aiEntity = SeedAIFaction(em, territoryTimer: 5f);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(aiEntity);

            if (s.RivalryUnlocked)
            {
                sb.AppendLine($"Phase 1 FAIL: RivalryUnlocked should be false at stage 1 (got {s.RivalryUnlocked})");
                return false;
            }
            if (s.RaidPressureUnlocked)
            {
                sb.AppendLine($"Phase 1 FAIL: RaidPressureUnlocked should be false at stage 1 (got {s.RaidPressureUnlocked})");
                return false;
            }
            if (s.TerritoryTimer < 24f)
            {
                sb.AppendLine($"Phase 1 FAIL: TerritoryTimer should be floored at >=24 when rivalry locked (got {s.TerritoryTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: RivalryUnlocked={s.RivalryUnlocked} " +
                          $"RaidPressureUnlocked={s.RaidPressureUnlocked} " +
                          $"TerritoryTimer={s.TerritoryTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("pressure-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);

            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 0);
            Entity aiEntity = SeedAIFaction(em, attackTimer: 20f, territoryTimer: 12f, raidTimer: 18f);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(aiEntity);

            if (!s.RivalryUnlocked)
            {
                sb.AppendLine($"Phase 2 FAIL: RivalryUnlocked should be true at stage 3 (got {s.RivalryUnlocked})");
                return false;
            }
            if (!s.RaidPressureUnlocked)
            {
                sb.AppendLine($"Phase 2 FAIL: RaidPressureUnlocked should be true at stage 3 (got {s.RaidPressureUnlocked})");
                return false;
            }
            if (s.AttackTimer >= 20f)
            {
                sb.AppendLine($"Phase 2 FAIL: AttackTimer should have ticked below 20 (got {s.AttackTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: RivalryUnlocked={s.RivalryUnlocked} " +
                          $"RaidPressureUnlocked={s.RaidPressureUnlocked} " +
                          $"AttackTimer={s.AttackTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("pressure-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);

            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 2);
            Entity aiEntity = SeedAIFaction(em, attackTimer: 20f, territoryTimer: 20f, raidTimer: 30f);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(aiEntity);

            if (s.AttackTimer > 8f)
            {
                sb.AppendLine($"Phase 3 FAIL: AttackTimer should be <=8 (player WP Level=2) (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.TerritoryTimer > 6f)
            {
                sb.AppendLine($"Phase 3 FAIL: TerritoryTimer should be <=6 (player WP Level=2) (got {s.TerritoryTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: AttackTimer={s.AttackTimer:F3} TerritoryTimer={s.TerritoryTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("pressure-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);

            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 2, greatReckoningScore: 4);
            Entity aiEntity = SeedAIFaction(em, attackTimer: 20f, territoryTimer: 20f, raidTimer: 30f);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(aiEntity);

            if (s.AttackTimer > 6f)
            {
                sb.AppendLine($"Phase 4 FAIL: AttackTimer should be <=6 under GreatReckoning (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.TerritoryTimer > 4f)
            {
                sb.AppendLine($"Phase 4 FAIL: TerritoryTimer should be <=4 under GreatReckoning (got {s.TerritoryTimer:F3})");
                return false;
            }
            if (s.RaidTimer > 8f)
            {
                sb.AppendLine($"Phase 4 FAIL: RaidTimer should be <=8 under GreatReckoning (got {s.RaidTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: AttackTimer={s.AttackTimer:F3} " +
                          $"TerritoryTimer={s.TerritoryTimer:F3} " +
                          $"RaidTimer={s.RaidTimer:F3}");
            return true;
        }
    }
}
#endif
