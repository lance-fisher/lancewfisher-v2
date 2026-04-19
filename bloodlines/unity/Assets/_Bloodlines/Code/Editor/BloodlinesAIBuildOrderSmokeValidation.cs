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
    /// Smoke validator for AIBuildOrderSystem. Verifies five phases:
    ///
    ///   Phase 1: No barracks, can afford -> Barracks fires.
    ///   Phase 2: Has barracks, faith faction, no wayshrine -> Wayshrine fires.
    ///   Phase 3: Keep fortified, barracks+quarry+iron mine present, no siege
    ///            workshop -> SiegeWorkshop fires.
    ///   Phase 4: Population cap nearly full, fewer than 4 houses -> Dwelling fires.
    ///   Phase 5: Food below threshold, fewer than 3 farms -> Farm fires.
    ///
    /// Browser reference: ai.js updateEnemyAi buildTimer<=0 block ~lines 1377-1573.
    /// Artifact: artifacts/unity-ai-build-order-smoke.log.
    /// </summary>
    public static class BloodlinesAIBuildOrderSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-build-order-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Build Order Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIBuildOrderSmokeValidation() =>
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
                message = "AI build order smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_BUILD_ORDER_SMOKE " +
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIBuildOrderSystem>());
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

        private static Entity SeedAIFactionWithBuildOrder(
            EntityManager em,
            AIBuildOrderComponent buildData,
            bool playerKeepFortified = false)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent),
                typeof(AICovertOpsComponent),
                typeof(AIBuildOrderComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(e, new AIStrategyComponent
            {
                WorkerGatherIntervalSeconds = 5f,
                BuildTimer                  = -1f,   // already expired: fires on first update
                PlayerKeepFortified         = playerKeepFortified,
                AssassinationTimer          = 999f,
                MissionaryTimer             = 999f,
                HolyWarTimer                = 999f,
                MarriageProposalTimer       = 999f,
            });
            em.SetComponentData(e, new AISiegeOrchestrationComponent());
            em.SetComponentData(e, new AICovertOpsComponent
            {
                AssassinationTimer        = 999f,
                MissionaryTimer           = 999f,
                HolyWarTimer              = 999f,
                DivineRightTimer          = 999f,
                CaptiveRecoveryTimer      = 999f,
                MarriageProposalTimer     = 999f,
                MarriageInboxTimer        = 999f,
                PactProposalTimer         = 999f,
                LesserHousePromotionTimer = 999f,
            });
            em.SetComponentData(e, buildData);
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        // ------------------------------------------------------------------ Phase 1
        // No barracks, can afford -> Barracks.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("build-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var build = new AIBuildOrderComponent
            {
                HasBuilder       = true,
                HasBarracks      = false,    // no barracks yet
                CanAffordBarracks= true,     // gold >= 85, wood >= 95 (pre-computed)
                // all other branches blocked by defaults
            };
            Entity faction = SeedAIFactionWithBuildOrder(em, build);
            world.Update();

            var result = em.GetComponentData<AIBuildOrderComponent>(faction);
            if (result.NextBuildOp != BuildOrderKind.Barracks)
            {
                sb.AppendLine($"Phase 1 FAIL: no barracks + can afford should dispatch Barracks " +
                              $"(got {result.NextBuildOp})");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: NextBuildOp={result.NextBuildOp} (no barracks -> barracks)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // Has barracks, faith faction, no wayshrine, can afford -> Wayshrine.

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("build-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var build = new AIBuildOrderComponent
            {
                HasBuilder       = true,
                HasBarracks      = true,     // barracks exists (blocks phase 1)
                CanAffordBarracks= false,    // blocked explicitly
                EnemyHasFaith    = true,     // faith faction
                HasWayshrine     = false,    // no wayshrine yet
                CanAffordWayshrine= true,
            };
            Entity faction = SeedAIFactionWithBuildOrder(em, build);
            world.Update();

            var result = em.GetComponentData<AIBuildOrderComponent>(faction);
            if (result.NextBuildOp != BuildOrderKind.Wayshrine)
            {
                sb.AppendLine($"Phase 2 FAIL: faith+barracks+no wayshrine should dispatch Wayshrine " +
                              $"(got {result.NextBuildOp})");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: NextBuildOp={result.NextBuildOp} (faith+barracks -> wayshrine)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // Keep fortified, barracks+quarry+iron mine, no siege workshop -> SiegeWorkshop.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("build-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var build = new AIBuildOrderComponent
            {
                HasBuilder          = true,
                HasBarracks         = true,      // phases 1-2 blocked
                EnemyHasFaith       = false,     // no faith: wayshrine skipped
                CanAffordBarracks   = false,
                HasQuarry           = true,      // quarry present: phase 3 skipped
                HasIronMine         = true,      // iron mine present: phase 4 skipped
                HasSiegeWorkshop    = false,     // siege workshop absent: fires
                CanAffordSiegeWorkshop = true,
            };
            Entity faction = SeedAIFactionWithBuildOrder(em, build, playerKeepFortified: true);
            world.Update();

            var result = em.GetComponentData<AIBuildOrderComponent>(faction);
            if (result.NextBuildOp != BuildOrderKind.SiegeWorkshop)
            {
                sb.AppendLine($"Phase 3 FAIL: barracks+quarry+iron+fortified should dispatch " +
                              $"SiegeWorkshop (got {result.NextBuildOp})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: NextBuildOp={result.NextBuildOp} " +
                          "(barracks+quarry+iron+fortified -> siege workshop)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // Population cap nearly full, fewer than 4 houses -> Dwelling.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("build-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var build = new AIBuildOrderComponent
            {
                HasBuilder           = true,
                HasBarracks          = true,     // phases 1-2 blocked
                EnemyHasFaith        = false,
                CanAffordBarracks    = false,
                HasQuarry            = true,     // phase 3 blocked
                HasIronMine          = true,     // phase 4 blocked
                HasSiegeWorkshop     = true,     // phase 5 blocked
                // faith chain gates all false
                // siege chain: playerKeepFortified=false so supply camp/stable skipped
                PopulationCapAvailable = 1,      // <= 1 triggers dwelling
                HouseCount             = 2,      // < 4
            };
            Entity faction = SeedAIFactionWithBuildOrder(em, build, playerKeepFortified: false);
            world.Update();

            var result = em.GetComponentData<AIBuildOrderComponent>(faction);
            if (result.NextBuildOp != BuildOrderKind.Dwelling)
            {
                sb.AppendLine($"Phase 4 FAIL: cap full + < 4 houses should dispatch Dwelling " +
                              $"(got {result.NextBuildOp})");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: NextBuildOp={result.NextBuildOp} " +
                          "(pop cap full -> dwelling)");
            return true;
        }

        // ------------------------------------------------------------------ Phase 5
        // Food below threshold, fewer than 3 farms -> Farm.

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("build-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            var build = new AIBuildOrderComponent
            {
                HasBuilder           = true,
                HasBarracks          = true,     // phases 1-2 blocked
                EnemyHasFaith        = false,
                CanAffordBarracks    = false,
                HasQuarry            = true,
                HasIronMine          = true,
                HasSiegeWorkshop     = true,
                // faith chain all false
                // supply chain: playerKeepFortified=false so skipped
                PopulationCapAvailable = 3,      // > 1: dwelling skipped
                HouseCount             = 4,      // >= 4: dwelling skipped
                FoodStock              = 5f,     // food (5) < total+4 (8+4=12) -> farm fires
                PopulationTotal        = 8,
                FarmCount              = 1,      // < 3
            };
            Entity faction = SeedAIFactionWithBuildOrder(em, build, playerKeepFortified: false);
            world.Update();

            var result = em.GetComponentData<AIBuildOrderComponent>(faction);
            if (result.NextBuildOp != BuildOrderKind.Farm)
            {
                sb.AppendLine($"Phase 5 FAIL: low food + < 3 farms should dispatch Farm " +
                              $"(got {result.NextBuildOp})");
                return false;
            }
            sb.AppendLine($"Phase 5 PASS: NextBuildOp={result.NextBuildOp} (low food -> farm)");
            return true;
        }
    }
}
#endif
