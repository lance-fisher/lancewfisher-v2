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
    /// Smoke validator for the AI worker and territory command-dispatch slice.
    ///
    /// Phase 1: Worker Phase=Seeking with AssignedNode set -> AIWorkerCommandSystem
    ///          promotes to Gathering and adds WorkerGatherOrderComponent.
    /// Phase 2: Worker already Gathering -> no re-dispatch; phase and existing order
    ///          remain unchanged.
    /// Phase 3: TerritoryTimer<=0, RivalryUnlocked=true, ExpansionTargetCpId set ->
    ///          non-worker, non-siege combat units receive MoveCommandComponent.
    /// Phase 4: TerritoryTimer>0 -> no dispatch; MoveCommandComponent stays absent.
    ///
    /// Browser reference: ai.js issueGatherCommand in idle-worker dispatch loop
    /// (lines 1243-1260) and territory dispatch block (~1711-1821).
    /// Artifact: artifacts/unity-ai-command-dispatch-smoke.log.
    /// </summary>
    public static class BloodlinesAICommandDispatchSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-command-dispatch-smoke.log";

        [MenuItem("Bloodlines/AI/Run AI Command Dispatch Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAICommandDispatchSmokeValidation() =>
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
                message = "AI command dispatch smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_COMMAND_DISPATCH_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
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
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AIStrategicPressureSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AIWorkerGatherSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AISiegeOrchestrationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AICovertOpsSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AIWorkerCommandSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AITerritoryDispatchSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<WorkerGatherSystem>());
            simulationGroup.SortSystems();
            return simulationGroup;
        }

        private static void SeedMatchProgression(EntityManager entityManager, int stageNumber = 3)
        {
            var entity = entityManager.CreateEntity(typeof(MatchProgressionComponent));
            entityManager.SetComponentData(entity, new MatchProgressionComponent
            {
                StageNumber = stageNumber,
            });
        }

        private static void SeedPlayerFaction(EntityManager entityManager)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = "player" });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new WorldPressureComponent());
            entityManager.SetComponentData(entity, new WorldPressureCycleTrackerComponent
            {
                CycleSeconds = 90f,
                Accumulator = 0f,
            });
        }

        private static Entity SeedAIFaction(
            EntityManager entityManager,
            AIStrategyComponent strategy)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent),
                typeof(AICovertOpsComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = "enemy" });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new AIEconomyControllerComponent { Enabled = true });
            entityManager.SetComponentData(entity, strategy);
            entityManager.SetComponentData(entity, new AISiegeOrchestrationComponent());
            entityManager.SetComponentData(entity, new AICovertOpsComponent());
            entityManager.SetComponentData(entity, new WorldPressureComponent());
            entityManager.SetComponentData(entity, new WorldPressureCycleTrackerComponent
            {
                CycleSeconds = 90f,
                Accumulator = 0f,
            });
            return entity;
        }

        private static Entity SeedResourceNode(
            EntityManager entityManager,
            string resourceId,
            float amount,
            float3 position)
        {
            var entity = entityManager.CreateEntity(typeof(ResourceNodeComponent), typeof(PositionComponent));
            entityManager.SetComponentData(entity, new ResourceNodeComponent
            {
                ResourceId = resourceId,
                Amount = amount,
                InitialAmount = amount,
            });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            return entity;
        }

        private static Entity SeedWorker(
            EntityManager entityManager,
            WorkerGatherComponent gather,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(UnitTypeComponent),
                typeof(HealthComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = "enemy" });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = "villager",
                Role = UnitRole.Worker,
                SiegeClass = SiegeClass.None,
            });
            entityManager.SetComponentData(entity, new HealthComponent { Current = 100f, Max = 100f });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            entityManager.AddComponentData(entity, gather);
            return entity;
        }

        private static Entity SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float3 position)
        {
            var entity = entityManager.CreateEntity(typeof(ControlPointComponent), typeof(PositionComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = controlPointId,
                OwnerFactionId = ownerFactionId,
                Loyalty = 50f,
            });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            return entity;
        }

        private static Entity SeedCombatUnit(
            EntityManager entityManager,
            UnitRole role,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(UnitTypeComponent),
                typeof(HealthComponent),
                typeof(PositionComponent),
                typeof(CombatStatsComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = "enemy" });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = "militia",
                Role = role,
                SiegeClass = SiegeClass.None,
            });
            entityManager.SetComponentData(entity, new HealthComponent { Current = 100f, Max = 100f });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            entityManager.SetComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = 10f,
                AttackRange = 1.5f,
                AttackCooldown = 1f,
                Sight = 10f,
            });
            return entity;
        }

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-command-dispatch-phase1");
            var entityManager = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(entityManager);
            SeedPlayerFaction(entityManager);
            SeedAIFaction(entityManager, new AIStrategyComponent { WorkerGatherIntervalSeconds = 5f });
            Entity node = SeedResourceNode(entityManager, "gold", 100f, new float3(6f, 0f, 0f));
            Entity worker = SeedWorker(entityManager, new WorkerGatherComponent
            {
                AssignedNode = node,
                AssignedResourceId = "gold",
                CarryResourceId = "gold",
                CarryAmount = 0f,
                CarryCapacity = 10f,
                GatherRate = 1f,
                Phase = WorkerGatherPhase.Seeking,
                GatherRadius = 1.25f,
                DepositRadius = 1.75f,
            }, new float3(0f, 0f, 0f));

            world.Update();

            var gather = entityManager.GetComponentData<WorkerGatherComponent>(worker);
            if (gather.Phase != WorkerGatherPhase.Gathering)
            {
                sb.AppendLine($"Phase 1 FAIL: worker should promote to Gathering (got {gather.Phase})");
                return false;
            }
            if (!entityManager.HasComponent<WorkerGatherOrderComponent>(worker))
            {
                sb.AppendLine("Phase 1 FAIL: WorkerGatherOrderComponent missing after dispatch");
                return false;
            }

            sb.AppendLine($"Phase 1 PASS: worker Phase={gather.Phase} orderPresent=True target={gather.AssignedResourceId}.");
            return true;
        }

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-command-dispatch-phase2");
            var entityManager = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(entityManager);
            SeedPlayerFaction(entityManager);
            SeedAIFaction(entityManager, new AIStrategyComponent { WorkerGatherIntervalSeconds = 5f });
            Entity originalNode = SeedResourceNode(entityManager, "wood", 100f, new float3(4f, 0f, 0f));
            Entity worker = SeedWorker(entityManager, new WorkerGatherComponent
            {
                AssignedNode = originalNode,
                AssignedResourceId = "wood",
                CarryResourceId = "wood",
                CarryAmount = 0f,
                CarryCapacity = 10f,
                GatherRate = 1f,
                Phase = WorkerGatherPhase.Gathering,
                GatherRadius = 1.25f,
                DepositRadius = 1.75f,
            }, new float3(0f, 0f, 0f));
            entityManager.AddComponentData(worker, new WorkerGatherOrderComponent
            {
                TargetNode = originalNode,
                ResourceId = "wood",
            });

            world.Update();

            var gather = entityManager.GetComponentData<WorkerGatherComponent>(worker);
            var order = entityManager.GetComponentData<WorkerGatherOrderComponent>(worker);
            if (gather.Phase != WorkerGatherPhase.Gathering)
            {
                sb.AppendLine($"Phase 2 FAIL: worker should remain Gathering (got {gather.Phase})");
                return false;
            }
            if (order.TargetNode != originalNode || !order.ResourceId.Equals(new FixedString32Bytes("wood")))
            {
                sb.AppendLine("Phase 2 FAIL: existing WorkerGatherOrderComponent was changed on an already-gathering worker");
                return false;
            }

            sb.AppendLine("Phase 2 PASS: gathering worker kept existing order with no re-dispatch.");
            return true;
        }

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-command-dispatch-phase3");
            var entityManager = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(entityManager, stageNumber: 3);
            SeedPlayerFaction(entityManager);
            SeedControlPoint(entityManager, "cp_expand", "player", new float3(8f, 0f, 0f));
            SeedAIFaction(entityManager, new AIStrategyComponent
            {
                TerritoryTimer = -1f,
                RivalryUnlocked = true,
                ExpansionTargetCpId = "cp_expand",
            });
            Entity militia = SeedCombatUnit(entityManager, UnitRole.Melee, new float3(0f, 0f, 0f));
            Entity cavalry = SeedCombatUnit(entityManager, UnitRole.LightCavalry, new float3(1f, 0f, 0f));
            SeedCombatUnit(entityManager, UnitRole.Worker, new float3(2f, 0f, 0f));
            SeedCombatUnit(entityManager, UnitRole.SiegeEngine, new float3(3f, 0f, 0f));

            world.Update();

            if (!entityManager.HasComponent<MoveCommandComponent>(militia) ||
                !entityManager.GetComponentData<MoveCommandComponent>(militia).IsActive)
            {
                sb.AppendLine("Phase 3 FAIL: melee combat unit did not receive MoveCommandComponent");
                return false;
            }
            if (!entityManager.HasComponent<MoveCommandComponent>(cavalry) ||
                !entityManager.GetComponentData<MoveCommandComponent>(cavalry).IsActive)
            {
                sb.AppendLine("Phase 3 FAIL: cavalry combat unit did not receive MoveCommandComponent");
                return false;
            }

            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIStrategyComponent>());
            var strategy = factionQuery.GetSingleton<AIStrategyComponent>();
            factionQuery.Dispose();
            if (math.abs(strategy.TerritoryTimer - 12f) > 0.001f)
            {
                sb.AppendLine($"Phase 3 FAIL: TerritoryTimer should reset to 12 after dispatch (got {strategy.TerritoryTimer:0.###})");
                return false;
            }

            sb.AppendLine($"Phase 3 PASS: territory dispatch activated combat move orders; TerritoryTimer={strategy.TerritoryTimer:0.###}.");
            return true;
        }

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-command-dispatch-phase4");
            var entityManager = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(entityManager, stageNumber: 3);
            SeedPlayerFaction(entityManager);
            SeedControlPoint(entityManager, "cp_hold", "player", new float3(8f, 0f, 0f));
            SeedAIFaction(entityManager, new AIStrategyComponent
            {
                TerritoryTimer = 6f,
                RivalryUnlocked = true,
                ExpansionTargetCpId = "cp_hold",
            });
            Entity militia = SeedCombatUnit(entityManager, UnitRole.Melee, new float3(0f, 0f, 0f));

            world.Update();

            if (entityManager.HasComponent<MoveCommandComponent>(militia))
            {
                sb.AppendLine("Phase 4 FAIL: MoveCommandComponent should remain absent while TerritoryTimer > 0");
                return false;
            }

            sb.AppendLine("Phase 4 PASS: territory dispatch stayed idle while TerritoryTimer > 0.");
            return true;
        }
    }
}
#endif
