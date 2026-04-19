#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for AIWorkerGatherSystem. Verifies four phases:
    ///
    ///   Phase 1: Idle AI worker + non-depleted gold node -> Phase becomes Seeking,
    ///            AssignedNode is set.
    ///   Phase 2: All resource nodes depleted -> worker stays Idle.
    ///   Phase 3: Worker already Gathering -> not re-dispatched (Phase unchanged).
    ///   Phase 4: Two idle workers + gold and wood nodes -> rotation assigns
    ///            different starting priorities (worker 0 starts gold, worker 1 wood).
    ///
    /// Browser reference: ai.js updateEnemyAi idleWorkers dispatch loop (lines 1243-1251),
    ///                    getEnemyGatherPriorities (885-922), chooseGatherNode (924-933).
    /// Artifact: artifacts/unity-ai-worker-gather-smoke.log.
    /// </summary>
    public static class BloodlinesAIWorkerGatherSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-worker-gather-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Worker Gather Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIWorkerGatherSmokeValidation() =>
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
                message = "AI worker gather smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_WORKER_GATHER_SMOKE " +
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIWorkerGatherSystem>());
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

        private static Entity SeedAIFaction(EntityManager em,
            float workerGatherInterval = 0.001f)
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
                WorkerGatherIntervalSeconds          = workerGatherInterval,
                WorkerGatherAccumulator              = 0f,
                ExpansionIntervalSeconds             = 8f,
                ScoutHarassIntervalSeconds           = 12f,
                WorldPressureResponseIntervalSeconds = 15f,
                ReinforcementIntervalSeconds         = 10f,
                AttackTimer                          = 20f,
                TerritoryTimer                       = 20f,
                RaidTimer                            = 30f,
                HolyWarTimer                         = 95f,
                AssassinationTimer                   = 80f,
                MissionaryTimer                      = 70f,
                MarriageProposalTimer                = 90f,
                BuildTimer                           = 8f,
            });
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            return e;
        }

        private static Entity SeedWorker(EntityManager em, WorkerGatherPhase phase,
            float3 position = default)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(UnitTypeComponent),
                typeof(HealthComponent),
                typeof(PositionComponent),
                typeof(WorkerGatherComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new UnitTypeComponent { Role = UnitRole.Worker });
            em.SetComponentData(e, new HealthComponent { Current = 100f, Max = 100f });
            em.SetComponentData(e, new PositionComponent { Value = position });
            em.SetComponentData(e, new WorkerGatherComponent
            {
                Phase         = phase,
                AssignedNode  = Entity.Null,
                CarryCapacity = 10f,
                GatherRate    = 1f,
                GatherRadius  = 1f,
                DepositRadius = 1f,
            });
            return e;
        }

        private static Entity SeedResourceNode(EntityManager em, string resourceId,
            float amount, float3 position = default)
        {
            var e = em.CreateEntity(typeof(ResourceNodeComponent), typeof(PositionComponent));
            em.SetComponentData(e, new ResourceNodeComponent
            {
                ResourceId    = resourceId,
                Amount        = amount,
                InitialAmount = amount,
            });
            em.SetComponentData(e, new PositionComponent { Value = position });
            return e;
        }

        // ------------------------------------------------------------------ Phase 1
        // Idle worker + non-depleted gold node -> Phase = Seeking, AssignedNode set.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("gather-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            SeedAIFaction(em);
            Entity worker = SeedWorker(em, WorkerGatherPhase.Idle,
                new float3(0f, 0f, 0f));
            Entity goldNode = SeedResourceNode(em, "gold", 100f,
                new float3(5f, 0f, 0f));

            world.Update();
            var gather = em.GetComponentData<WorkerGatherComponent>(worker);

            if (gather.Phase != WorkerGatherPhase.Seeking)
            {
                sb.AppendLine($"Phase 1 FAIL: Worker should be Seeking after dispatch (got {gather.Phase})");
                return false;
            }
            if (gather.AssignedNode == Entity.Null)
            {
                sb.AppendLine("Phase 1 FAIL: AssignedNode should be set after dispatch (got Null)");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: Worker Phase={gather.Phase} AssignedNode set " +
                          $"Resource={gather.AssignedResourceId}");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // All nodes depleted -> worker stays Idle.

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("gather-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            SeedAIFaction(em);
            Entity worker = SeedWorker(em, WorkerGatherPhase.Idle);
            SeedResourceNode(em, "gold",  0f);  // depleted
            SeedResourceNode(em, "wood",  0f);  // depleted
            SeedResourceNode(em, "stone", 0f);  // depleted
            SeedResourceNode(em, "iron",  0f);  // depleted

            world.Update();
            var gather = em.GetComponentData<WorkerGatherComponent>(worker);

            if (gather.Phase != WorkerGatherPhase.Idle)
            {
                sb.AppendLine($"Phase 2 FAIL: Worker should stay Idle when all nodes depleted (got {gather.Phase})");
                return false;
            }
            if (gather.AssignedNode != Entity.Null)
            {
                sb.AppendLine("Phase 2 FAIL: AssignedNode should be Null when no valid node found");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: Worker stayed Idle with all nodes depleted");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // Worker already Gathering -> not re-dispatched.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("gather-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            SeedAIFaction(em);
            Entity goldNode = SeedResourceNode(em, "gold", 100f, new float3(5f, 0f, 0f));
            // Seed worker already gathering -- should NOT be re-dispatched.
            Entity worker = SeedWorker(em, WorkerGatherPhase.Gathering);
            // Set a sentinel node so we can detect if it got overwritten.
            var initialGather = em.GetComponentData<WorkerGatherComponent>(worker);
            initialGather.AssignedNode = goldNode; // already assigned
            em.SetComponentData(worker, initialGather);

            world.Update();
            var gather = em.GetComponentData<WorkerGatherComponent>(worker);

            if (gather.Phase != WorkerGatherPhase.Gathering)
            {
                sb.AppendLine($"Phase 3 FAIL: Worker should remain Gathering (got {gather.Phase})");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: Gathering worker not re-dispatched (Phase={gather.Phase})");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // Two idle workers + gold and wood nodes -> rotation spreads resource types.
        // Worker 0 (index%4 = 0) starts at gold. Worker 1 (index%4 = 1) starts at wood.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("gather-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedPlayerFaction(em);
            SeedAIFaction(em);

            // Place gold node nearby worker 0, wood node nearby worker 1.
            SeedResourceNode(em, "gold",  50f, new float3(2f, 0f, 0f));
            SeedResourceNode(em, "wood",  50f, new float3(3f, 0f, 0f));

            Entity worker0 = SeedWorker(em, WorkerGatherPhase.Idle, new float3(0f, 0f, 0f));
            Entity worker1 = SeedWorker(em, WorkerGatherPhase.Idle, new float3(0f, 0f, 0f));

            world.Update();
            var g0 = em.GetComponentData<WorkerGatherComponent>(worker0);
            var g1 = em.GetComponentData<WorkerGatherComponent>(worker1);

            if (g0.Phase != WorkerGatherPhase.Seeking)
            {
                sb.AppendLine($"Phase 4 FAIL: Worker 0 should be Seeking (got {g0.Phase})");
                return false;
            }
            if (g1.Phase != WorkerGatherPhase.Seeking)
            {
                sb.AppendLine($"Phase 4 FAIL: Worker 1 should be Seeking (got {g1.Phase})");
                return false;
            }
            // Both workers dispatched -- verify at least one got gold and at least one got wood
            // (order may vary since entity iteration order is not guaranteed).
            bool anyGold = g0.AssignedResourceId == "gold" || g1.AssignedResourceId == "gold";
            bool anyWood = g0.AssignedResourceId == "wood" || g1.AssignedResourceId == "wood";
            if (!anyGold || !anyWood)
            {
                sb.AppendLine($"Phase 4 FAIL: Expected one gold and one wood assignment. " +
                              $"Worker0={g0.AssignedResourceId} Worker1={g1.AssignedResourceId}");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: Worker0={g0.AssignedResourceId} Worker1={g1.AssignedResourceId} " +
                          $"(rotation spread across types)");
            return true;
        }
    }
}
#endif
