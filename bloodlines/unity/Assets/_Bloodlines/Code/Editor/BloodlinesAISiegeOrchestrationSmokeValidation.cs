#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for AISiegeOrchestrationSystem. Verifies six phases:
    ///
    ///   Phase 1: keep not fortified -> Phase is Inactive.
    ///   Phase 2: keep fortified + FieldWaterDesertionRisk -> Phase is FieldWaterRetreat
    ///            (field water is highest priority regardless of other flags).
    ///   Phase 3: keep fortified, army >= 3, no siege engines -> Phase is SiegeRefusal.
    ///   Phase 4: keep fortified, has siege, engineering ready, supply camp, supply line,
    ///            supplied siege, escort ready, but FormalSiegeLinesFormed=false
    ///            -> Phase is StagingLines.
    ///   Phase 5: all gates clear, FormalSiegeLinesFormed=true
    ///            -> Phase is Assaulting.
    ///   Phase 6: PostRepulse window: CohesionPenaltyUntil > elapsed
    ///            -> Phase is PostRepulse.
    ///
    /// Browser reference: ai.js updateEnemyAi attackTimer<=0 block (~1825-2090).
    /// Artifact: artifacts/unity-ai-siege-orchestration-smoke.log.
    /// </summary>
    public static class BloodlinesAISiegeOrchestrationSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-siege-orchestration-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Siege Orchestration Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAISiegeOrchestrationSmokeValidation() =>
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
                message = "AI siege orchestration smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_SIEGE_ORCHESTRATION_SMOKE " +
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
            ok &= RunPhase5(sb);
            ok &= RunPhase6(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIWorkerGatherSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AISiegeOrchestrationSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedMatchProgression(EntityManager em, int stageNumber = 3)
        {
            var e = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(e, new MatchProgressionComponent { StageNumber = stageNumber });
        }

        private static void SeedPlayerFaction(EntityManager em)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
        }

        private static Entity SeedAIFactionWithSiege(EntityManager em,
            AISiegeOrchestrationComponent siegeData)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(e, new AIStrategyComponent
            {
                WorkerGatherIntervalSeconds = 5f,
                PlayerKeepFortified         = siegeData.ArmyCount >= 3
                    ? true : false, // set by calling test
                AttackTimer                 = 0.001f,
            });
            em.SetComponentData(e, siegeData);
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        // Override PlayerKeepFortified on the AIStrategyComponent separately so tests
        // can control it independently from the siege component flags.
        private static void SetKeepFortified(EntityManager em, Entity faction, bool value)
        {
            var s = em.GetComponentData<AIStrategyComponent>(faction);
            s.PlayerKeepFortified = value;
            em.SetComponentData(faction, s);
        }

        // ------------------------------------------------------------------ Phase 1
        // Keep not fortified -> Inactive.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var siegeData = new AISiegeOrchestrationComponent { ArmyCount = 5 };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, false);  // keep NOT fortified

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.Inactive)
            {
                sb.AppendLine($"Phase 1 FAIL: keep not fortified should yield Inactive (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: Phase={phase} (keep not fortified)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // FieldWaterDesertionRisk=true -> FieldWaterRetreat (highest priority).

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            // All readiness flags set to true but field water overrides.
            var siegeData = new AISiegeOrchestrationComponent
            {
                ArmyCount               = 5,
                EnemyHasSiegeUnit       = true,
                EngineeringReady        = true,
                SupplyCampCompleted     = true,
                SupplyLineReady         = true,
                SuppliedSiegeReady      = true,
                EscortArmyCount         = 5,
                FormalSiegeLinesFormed  = true,
                FieldWaterDesertionRisk = true,  // <- should override all
            };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, true);

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.FieldWaterRetreat)
            {
                sb.AppendLine($"Phase 2 FAIL: FieldWaterDesertionRisk should override to FieldWaterRetreat (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: Phase={phase} (field water highest priority)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // Keep fortified, army >= 3, no siege engines -> SiegeRefusal.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var siegeData = new AISiegeOrchestrationComponent
            {
                ArmyCount         = 5,
                EnemyHasSiegeUnit = false,  // <- no siege engines
            };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, true);

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.SiegeRefusal)
            {
                sb.AppendLine($"Phase 3 FAIL: no siege engines should yield SiegeRefusal (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: Phase={phase} (no siege engines -> refusal)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // All gates except FormalSiegeLinesFormed -> StagingLines.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var siegeData = new AISiegeOrchestrationComponent
            {
                ArmyCount              = 5,
                EnemyHasSiegeUnit      = true,
                EngineeringReady       = true,
                SupplyCampCompleted    = true,
                SupplyLineReady        = true,
                SuppliedSiegeReady     = true,
                EscortArmyCount        = 5,
                FormalSiegeLinesFormed = false,  // <- lines not yet formed
            };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, true);

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.StagingLines)
            {
                sb.AppendLine($"Phase 4 FAIL: lines not formed should yield StagingLines (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: Phase={phase} (all ready but lines not formed)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 5
        // All gates clear -> Assaulting.

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var siegeData = new AISiegeOrchestrationComponent
            {
                ArmyCount              = 5,
                EnemyHasSiegeUnit      = true,
                EngineeringReady       = true,
                SupplyCampCompleted    = true,
                SupplyLineReady        = true,
                SuppliedSiegeReady     = true,
                EscortArmyCount        = 5,
                FormalSiegeLinesFormed = true,
                ReliefArmyApproaching  = false,
            };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, true);

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.Assaulting)
            {
                sb.AppendLine($"Phase 5 FAIL: all gates clear should yield Assaulting (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 5 PASS: Phase={phase} (all gates clear)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 6
        // CohesionPenaltyUntil > elapsed -> PostRepulse.

        private static bool RunPhase6(System.Text.StringBuilder sb)
        {
            using var world = new World("siege-phase6");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            // Set CohesionPenaltyUntil to a large value so it is always > elapsed.
            var siegeData = new AISiegeOrchestrationComponent
            {
                ArmyCount              = 5,
                EnemyHasSiegeUnit      = true,
                EngineeringReady       = true,
                SupplyCampCompleted    = true,
                SupplyLineReady        = true,
                SuppliedSiegeReady     = true,
                EscortArmyCount        = 5,
                FormalSiegeLinesFormed = true,
                CohesionPenaltyUntil   = 9999f,  // <- far in the future
            };
            Entity faction = SeedAIFactionWithSiege(em, siegeData);
            SetKeepFortified(em, faction, true);

            world.Update();
            var phase = em.GetComponentData<AISiegeOrchestrationComponent>(faction).Phase;

            if (phase != SiegeOrchestrationPhase.PostRepulse)
            {
                sb.AppendLine($"Phase 6 FAIL: CohesionPenaltyUntil > elapsed should yield PostRepulse (got {phase})");
                return false;
            }
            sb.AppendLine($"Phase 6 PASS: Phase={phase} (cohesion penalty -> post-repulse hold)");
            return true;
        }
    }
}
#endif
