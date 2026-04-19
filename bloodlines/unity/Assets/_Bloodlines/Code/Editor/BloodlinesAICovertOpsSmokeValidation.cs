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
    /// Smoke validator for AICovertOpsSystem. Verifies five phases:
    ///
    ///   Phase 1: DarkExtremesSourceFocused + LiveIntelOnPlayer -> Assassination fires,
    ///            timer capped to 6s, LastFiredOp=Assassination after world update.
    ///   Phase 2: HolyWarSourceFocused + CanPressureFaith conditions -> Missionary fires,
    ///            timer capped to 8s, LastFiredOp=Missionary.
    ///   Phase 3: TensionSignalCount > 0, enemy faith, faith not harmonious, intensity >= 18
    ///            -> HolyWar fires, LastFiredOp=HolyWar.
    ///   Phase 4: EnemyUnderSuccessionPressure + PactTermsAvailable -> PactProposal fires,
    ///            LastFiredOp=PactProposal.
    ///   Phase 5: HighPriorityCaptive + HasCaptiveTarget -> CaptiveRescue fires (rescue-
    ///            first for high-priority members), LastFiredOp=CaptiveRescue.
    ///
    /// Browser reference: ai.js updateEnemyAi covert ops block ~2419-2678.
    /// Artifact: artifacts/unity-ai-covert-ops-smoke.log.
    /// </summary>
    public static class BloodlinesAICovertOpsSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-covert-ops-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Covert Ops Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAICovertOpsSmokeValidation() =>
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
                message = "AI covert ops smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_COVERT_OPS_SMOKE " +
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICovertOpsSystem>());
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

        private static Entity SeedAIFactionWithCovertOps(EntityManager em,
            AICovertOpsComponent covertData)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent),
                typeof(AICovertOpsComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(e, new AIStrategyComponent
                { WorkerGatherIntervalSeconds = 5f });
            em.SetComponentData(e, new AISiegeOrchestrationComponent());
            em.SetComponentData(e, covertData);
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        // ------------------------------------------------------------------ Phase 1
        // DarkExtremesSourceFocused + timer near zero + LiveIntelOnPlayer -> Assassination.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("covert-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var ops = new AICovertOpsComponent
            {
                AssassinationTimer        = -1f,    // already expired: fires on first update
                MissionaryTimer           = 999f,
                HolyWarTimer              = 999f,
                DivineRightTimer          = 999f,
                CaptiveRecoveryTimer      = 999f,
                MarriageProposalTimer     = 999f,
                MarriageInboxTimer        = 999f,
                PactProposalTimer         = 999f,
                LesserHousePromotionTimer = 999f,
                DarkExtremesSourceFocused = true,
                LiveIntelOnPlayer         = true,
            };

            Entity faction = SeedAIFactionWithCovertOps(em, ops);
            world.Update();

            var result = em.GetComponentData<AICovertOpsComponent>(faction);
            if (result.LastFiredOp != CovertOpKind.Assassination)
            {
                sb.AppendLine($"Phase 1 FAIL: DarkExtremes+LiveIntel should fire Assassination " +
                              $"(got {result.LastFiredOp})");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: LastFiredOp={result.LastFiredOp} (dark extremes -> assassination)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // HolyWarSourceFocused + CanPressureFaith -> Missionary.

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("covert-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var ops = new AICovertOpsComponent
            {
                AssassinationTimer          = 999f,
                MissionaryTimer             = -1f,      // already expired
                HolyWarTimer                = 999f,
                DivineRightTimer            = 999f,
                CaptiveRecoveryTimer        = 999f,
                MarriageProposalTimer       = 999f,
                MarriageInboxTimer          = 999f,
                PactProposalTimer           = 999f,
                LesserHousePromotionTimer   = 999f,
                HolyWarSourceFocused        = true,
                EnemyHasFaith               = true,
                EnemyFaithDiffersFromPlayer = true,
                EnemyFaithIntensity         = 20f,      // >= 12 for canPressureFaith
                LiveMissionaryOnPlayer      = false,
            };
            Entity faction = SeedAIFactionWithCovertOps(em, ops);
            world.Update();

            var result = em.GetComponentData<AICovertOpsComponent>(faction);
            if (result.LastFiredOp != CovertOpKind.Missionary)
            {
                sb.AppendLine($"Phase 2 FAIL: HolyWarFocused+CanPressureFaith should fire Missionary " +
                              $"(got {result.LastFiredOp})");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: LastFiredOp={result.LastFiredOp} (holy war source -> missionary)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // TensionSignalCount > 0, enemy faith, not harmonious, intensity >= 18 -> HolyWar.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("covert-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var ops = new AICovertOpsComponent
            {
                AssassinationTimer          = 999f,
                MissionaryTimer             = 999f,
                HolyWarTimer                = -1f,      // already expired
                DivineRightTimer            = 999f,
                CaptiveRecoveryTimer        = 999f,
                MarriageProposalTimer       = 999f,
                MarriageInboxTimer          = 999f,
                PactProposalTimer           = 999f,
                LesserHousePromotionTimer   = 999f,
                EnemyHasFaith               = true,
                PlayerHasFaith              = true,
                FaithCompatibilityHarmonious= false,    // not harmonious -> can declare
                EnemyFaithIntensity         = 25f,      // >= 18
                TensionSignalCount          = 2,        // > 0
                ActiveHolyWarOnPlayer       = false,
            };
            Entity faction = SeedAIFactionWithCovertOps(em, ops);
            world.Update();

            var result = em.GetComponentData<AICovertOpsComponent>(faction);
            if (result.LastFiredOp != CovertOpKind.HolyWar)
            {
                sb.AppendLine($"Phase 3 FAIL: tension signals + faith gap should fire HolyWar " +
                              $"(got {result.LastFiredOp})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: LastFiredOp={result.LastFiredOp} (tension signals -> holy war)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // EnemyUnderSuccessionPressure + PactTermsAvailable -> PactProposal.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("covert-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var ops = new AICovertOpsComponent
            {
                AssassinationTimer              = 999f,
                MissionaryTimer                 = 999f,
                HolyWarTimer                    = 999f,
                DivineRightTimer                = 999f,
                CaptiveRecoveryTimer            = 999f,
                MarriageProposalTimer           = 999f,
                MarriageInboxTimer              = 999f,
                PactProposalTimer               = -1f,      // already expired
                LesserHousePromotionTimer       = 999f,
                PactTermsAvailable              = true,
                EnemyUnderSuccessionPressure    = true,     // shouldPropose
                EnemyArmyCount                  = 5,        // not under army pressure
            };
            Entity faction = SeedAIFactionWithCovertOps(em, ops);
            world.Update();

            var result = em.GetComponentData<AICovertOpsComponent>(faction);
            if (result.LastFiredOp != CovertOpKind.PactProposal)
            {
                sb.AppendLine($"Phase 4 FAIL: succession crisis should fire PactProposal " +
                              $"(got {result.LastFiredOp})");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: LastFiredOp={result.LastFiredOp} (succession crisis -> pact proposal)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 5
        // HighPriorityCaptive + HasCaptiveTarget -> CaptiveRescue (rescue-first for high priority).

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("covert-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var ops = new AICovertOpsComponent
            {
                AssassinationTimer          = 999f,
                MissionaryTimer             = 999f,
                HolyWarTimer                = 999f,
                DivineRightTimer            = 999f,
                CaptiveRecoveryTimer        = -1f,      // already expired
                MarriageProposalTimer       = 999f,
                MarriageInboxTimer          = 999f,
                PactProposalTimer           = 999f,
                LesserHousePromotionTimer   = 999f,
                HasCaptiveTarget            = true,
                HighPriorityCaptive         = true,     // head of bloodline / heir / commander
                EnemyIsHostileToPlayer      = false,
            };
            Entity faction = SeedAIFactionWithCovertOps(em, ops);
            world.Update();

            var result = em.GetComponentData<AICovertOpsComponent>(faction);
            if (result.LastFiredOp != CovertOpKind.CaptiveRescue)
            {
                sb.AppendLine($"Phase 5 FAIL: high priority captive should fire CaptiveRescue " +
                              $"(got {result.LastFiredOp})");
                return false;
            }
            sb.AppendLine($"Phase 5 PASS: LastFiredOp={result.LastFiredOp} " +
                          "(high priority captive -> rescue-first)");
            return true;
        }
    }
}
#endif
