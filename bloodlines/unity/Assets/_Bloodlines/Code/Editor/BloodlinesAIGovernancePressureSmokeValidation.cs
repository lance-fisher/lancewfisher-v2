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
    /// Smoke validator for the governance/event-context pressure extensions of
    /// AIStrategicPressureSystem. Verifies four phases:
    ///
    ///   Phase 1: HolyWarActive=true -> AttackTimer capped at <=10,
    ///            TerritoryTimer capped at <=8.
    ///   Phase 2: PlayerGovernanceActive + VictoryPressure ->
    ///            AttackTimer<=3, TerritoryTimer<=2, RaidTimer<=4.
    ///   Phase 3: PlayerSuccessionCrisisActive + High severity ->
    ///            AttackTimer<=5, TerritoryTimer<=4, MarriageProposalTimer<=18.
    ///   Phase 4: EnemyGovernanceActive + VictoryPressure ->
    ///            AttackTimer floored >=14 (AI pulls back),
    ///            HolyWarTimer floored >=24.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1129-1215 (governance clamp block).
    /// Artifact: artifacts/unity-ai-governance-pressure-smoke.log.
    /// </summary>
    public static class BloodlinesAIGovernancePressureSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-governance-pressure-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Governance Pressure Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIGovernancePressureSmokeValidation() =>
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
                message = "AI governance pressure smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_GOVERNANCE_PRESSURE_SMOKE " +
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

        private static void SeedMatchProgression(EntityManager em, int stageNumber)
        {
            var e = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(e, new MatchProgressionComponent { StageNumber = stageNumber });
        }

        private static void SeedPlayerFaction(EntityManager em, int wpLevel = 0)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new WorldPressureComponent { Level = wpLevel });
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
        }

        private static Entity SeedAIFaction(EntityManager em, AIStrategyComponent strategy)
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
            em.SetComponentData(e, strategy);
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        private static AIStrategyComponent BaseStrategy(
            float attackTimer = 20f,
            float territoryTimer = 20f,
            float raidTimer = 30f,
            float holyWarTimer = 95f,
            float assassinTimer = 80f,
            float missionaryTimer = 70f,
            float marriageTimer = 90f,
            float buildTimer = 8f)
        {
            return new AIStrategyComponent
            {
                AttackTimer                          = attackTimer,
                TerritoryTimer                       = territoryTimer,
                RaidTimer                            = raidTimer,
                HolyWarTimer                         = holyWarTimer,
                AssassinationTimer                   = assassinTimer,
                MissionaryTimer                      = missionaryTimer,
                MarriageProposalTimer                = marriageTimer,
                BuildTimer                           = buildTimer,
                ExpansionIntervalSeconds             = 8f,
                ScoutHarassIntervalSeconds           = 12f,
                WorldPressureResponseIntervalSeconds = 15f,
                ReinforcementIntervalSeconds         = 10f,
            };
        }

        // ------------------------------------------------------------------ Phase 1
        // HolyWarActive=true: AttackTimer<=10, TerritoryTimer<=8.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("governance-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 0);

            var strategy = BaseStrategy(attackTimer: 20f, territoryTimer: 20f);
            strategy.HolyWarActive = true;
            Entity ai = SeedAIFaction(em, strategy);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(ai);

            if (s.AttackTimer > 10f)
            {
                sb.AppendLine($"Phase 1 FAIL: AttackTimer should be <=10 under HolyWar (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.TerritoryTimer > 8f)
            {
                sb.AppendLine($"Phase 1 FAIL: TerritoryTimer should be <=8 under HolyWar (got {s.TerritoryTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: HolyWar AttackTimer={s.AttackTimer:F3} TerritoryTimer={s.TerritoryTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // PlayerGovernanceActive + VictoryPressure: AttackTimer<=3, TerritoryTimer<=2, RaidTimer<=4.

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("governance-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 0);

            var strategy = BaseStrategy(attackTimer: 20f, territoryTimer: 20f, raidTimer: 30f);
            strategy.PlayerGovernanceActive          = true;
            strategy.PlayerGovernanceVictoryPressure = true;
            Entity ai = SeedAIFaction(em, strategy);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(ai);

            if (s.AttackTimer > 3f)
            {
                sb.AppendLine($"Phase 2 FAIL: AttackTimer should be <=3 under governance victory pressure (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.TerritoryTimer > 2f)
            {
                sb.AppendLine($"Phase 2 FAIL: TerritoryTimer should be <=2 under governance victory pressure (got {s.TerritoryTimer:F3})");
                return false;
            }
            if (s.RaidTimer > 4f)
            {
                sb.AppendLine($"Phase 2 FAIL: RaidTimer should be <=4 under governance victory pressure (got {s.RaidTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: Governance VictoryPressure AttackTimer={s.AttackTimer:F3} " +
                          $"TerritoryTimer={s.TerritoryTimer:F3} RaidTimer={s.RaidTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // PlayerSuccessionCrisisActive + High: AttackTimer<=5, TerritoryTimer<=4, MarriageProposalTimer<=18.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("governance-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 0);

            var strategy = BaseStrategy(attackTimer: 20f, territoryTimer: 20f, marriageTimer: 90f);
            strategy.PlayerSuccessionCrisisActive = true;
            strategy.PlayerSuccessionCrisisHigh   = true;
            Entity ai = SeedAIFaction(em, strategy);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(ai);

            if (s.AttackTimer > 5f)
            {
                sb.AppendLine($"Phase 3 FAIL: AttackTimer should be <=5 under high succession crisis (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.TerritoryTimer > 4f)
            {
                sb.AppendLine($"Phase 3 FAIL: TerritoryTimer should be <=4 under high succession crisis (got {s.TerritoryTimer:F3})");
                return false;
            }
            if (s.MarriageProposalTimer > 18f)
            {
                sb.AppendLine($"Phase 3 FAIL: MarriageProposalTimer should be <=18 under succession crisis (got {s.MarriageProposalTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: Succession Crisis AttackTimer={s.AttackTimer:F3} " +
                          $"TerritoryTimer={s.TerritoryTimer:F3} MarriageProposalTimer={s.MarriageProposalTimer:F3}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // EnemyGovernanceActive + VictoryPressure: AttackTimer floored >=14, HolyWarTimer floored >=24.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("governance-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em, 3);
            SeedPlayerFaction(em, wpLevel: 0);

            // Seed low attack/holyWar timers -- governance should floor them upward.
            var strategy = BaseStrategy(attackTimer: 5f, territoryTimer: 20f, holyWarTimer: 10f);
            strategy.EnemyGovernanceActive          = true;
            strategy.EnemyGovernanceVictoryPressure = true;
            Entity ai = SeedAIFaction(em, strategy);

            world.Update();
            var s = em.GetComponentData<AIStrategyComponent>(ai);

            if (s.AttackTimer < 14f)
            {
                sb.AppendLine($"Phase 4 FAIL: AttackTimer should be floored >=14 under enemy governance victory (got {s.AttackTimer:F3})");
                return false;
            }
            if (s.HolyWarTimer < 24f)
            {
                sb.AppendLine($"Phase 4 FAIL: HolyWarTimer should be floored >=24 under enemy governance victory (got {s.HolyWarTimer:F3})");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: EnemyGovernance AttackTimer={s.AttackTimer:F3} HolyWarTimer={s.HolyWarTimer:F3}");
            return true;
        }
    }
}
#endif
