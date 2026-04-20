#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for destroyed wall, tower, gate, and keep
    /// recovery after breach sealing has already driven OpenBreachCount to zero.
    /// </summary>
    public static class BloodlinesDestroyedCounterRecoverySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-destroyed-counter-recovery-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float HalfRebuildWindowSeconds = 7f;
        private const float FullRebuildWindowSeconds = 14f;
        private const float FullKeepRebuildWindowSeconds = 28f;

        [MenuItem("Bloodlines/Fortification/Run Destroyed Counter Recovery Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchDestroyedCounterRecoverySmokeValidation()
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
                message = "Destroyed counter recovery smoke errored: " + e;
            }

            string artifact = "BLOODLINES_DESTROYED_COUNTER_RECOVERY_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine +
                              message;
            UnityDebug.Log(artifact);

            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunRebuildProgressAccumulatesAfterBreachesSealedPhase(sb);
            ok &= RunFirstWallRebuiltPhase(sb);
            ok &= RunRecoveryPriorityKeepOverGatePhase(sb);
            ok &= RunOpenBreachBlocksRecoveryPhase(sb);
            ok &= RunAllCountersZeroNoOpPhase(sb);
            ok &= RunFullRebuildClearsAllInPriorityOrderPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunRebuildProgressAccumulatesAfterBreachesSealedPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_Progress");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 300f);
            CreateIdleWorker(em, "player");
            CreateDestroyedRecoverySettlement(
                em,
                "keep_player",
                "player",
                destroyedWalls: 2,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 0);

            Tick(world, HalfRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement))
            {
                sb.AppendLine("PhaseRebuildProgressAccumulatesAfterBreachesSealed FAIL: expected recovery progress component after the first rebuild window.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlement);
            if (fortification.DestroyedWallSegmentCount != 2 ||
                progress.TargetCounter != DestroyedCounterKind.Wall ||
                !Approximately(progress.AccumulatedWorkerHours, 7f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseRebuildProgressAccumulatesAfterBreachesSealed FAIL: wall rebuild progress did not accumulate as expected. " +
                    $"walls={fortification.DestroyedWallSegmentCount} target={progress.TargetCounter} hours={progress.AccumulatedWorkerHours:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseRebuildProgressAccumulatesAfterBreachesSealed PASS: target={progress.TargetCounter} walls={fortification.DestroyedWallSegmentCount} hours={progress.AccumulatedWorkerHours:F2}.");
            return true;
        }

        private static bool RunFirstWallRebuiltPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_FirstWall");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 300f);
            CreateIdleWorker(em, "player");
            CreateDestroyedRecoverySettlement(
                em,
                "keep_player",
                "player",
                destroyedWalls: 2,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 0);

            Tick(world, FullRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseFirstWallRebuilt FAIL: expected settlement progress and faction stockpile surfaces.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlement);
            if (fortification.DestroyedWallSegmentCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentSegment, 0f, 0.001f) ||
                !Approximately(stockpile.Stone, 210f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseFirstWallRebuilt FAIL: first wall did not rebuild cleanly. " +
                    $"walls={fortification.DestroyedWallSegmentCount} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentSegment:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseFirstWallRebuilt PASS: walls={fortification.DestroyedWallSegmentCount} stockpileStone={stockpile.Stone:F2}.");
            return true;
        }

        private static bool RunRecoveryPriorityKeepOverGatePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_KeepPriority");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 400f);
            CreateIdleWorker(em, "player");
            CreateDestroyedRecoverySettlement(
                em,
                "keep_player",
                "player",
                destroyedWalls: 0,
                destroyedTowers: 0,
                destroyedGates: 1,
                destroyedKeeps: 1,
                openBreaches: 0);

            Tick(world, FullRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseRecoveryPriorityKeepOverGate FAIL: expected settlement progress and stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlement);
            if (fortification.DestroyedKeepCount != 1 ||
                fortification.DestroyedGateCount != 1 ||
                progress.TargetCounter != DestroyedCounterKind.Keep ||
                !Approximately(progress.AccumulatedWorkerHours, 14f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentSegment, 180f, 0.001f) ||
                !Approximately(stockpile.Stone, 220f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseRecoveryPriorityKeepOverGate FAIL: keep should be targeted first with the 2x multiplier. " +
                    $"keep={fortification.DestroyedKeepCount} gate={fortification.DestroyedGateCount} " +
                    $"target={progress.TargetCounter} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentSegment:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseRecoveryPriorityKeepOverGate PASS: target={progress.TargetCounter} hours={progress.AccumulatedWorkerHours:F2} reservedStone={progress.StoneReservedForCurrentSegment:F2}.");
            return true;
        }

        private static bool RunOpenBreachBlocksRecoveryPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_OpenBreach", includeBreachSealing: false);
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 500f);
            CreateIdleWorker(em, "player");
            CreateDestroyedRecoverySettlement(
                em,
                "keep_player",
                "player",
                destroyedWalls: 3,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 1);

            Tick(world, HalfRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseOpenBreachBlocksRecovery FAIL: expected settlement and stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            bool hasProgress = em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 1 ||
                fortification.DestroyedWallSegmentCount != 3 ||
                hasProgress ||
                !Approximately(stockpile.Stone, 500f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseOpenBreachBlocksRecovery FAIL: recovery should stay blocked while an open breach remains. " +
                    $"breaches={fortification.OpenBreachCount} walls={fortification.DestroyedWallSegmentCount} " +
                    $"hasProgress={hasProgress} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine("PhaseOpenBreachBlocksRecovery PASS: no recovery component or stone spend while open breaches remain.");
            return true;
        }

        private static bool RunAllCountersZeroNoOpPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_NoCounters");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 200f);
            CreateIdleWorker(em, "player");
            CreateIntactSettlement(em, "keep_player", "player");

            Tick(world, HalfRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseAllCountersZeroNoOp FAIL: expected settlement and stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            if (GetTotalDestroyedCount(fortification) != 0 ||
                em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement) ||
                !Approximately(stockpile.Stone, 200f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseAllCountersZeroNoOp FAIL: intact fortifications should remain untouched. " +
                    $"destroyedTotal={GetTotalDestroyedCount(fortification)} " +
                    $"hasProgress={em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlement)} " +
                    $"stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine("PhaseAllCountersZeroNoOp PASS: no component attached and no stone spent for intact fortifications.");
            return true;
        }

        private static bool RunFullRebuildClearsAllInPriorityOrderPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesDestroyedCounterRecovery_FullPriority");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 600f);
            CreateIdleWorker(em, "player");
            CreateDestroyedRecoverySettlement(
                em,
                "keep_player",
                "player",
                destroyedWalls: 1,
                destroyedTowers: 1,
                destroyedGates: 1,
                destroyedKeeps: 1,
                openBreaches: 0);

            Tick(world, FullKeepRebuildWindowSeconds);
            if (!TryGetSettlement(em, "keep_player", out var settlementAfterKeep))
            {
                sb.AppendLine("PhaseFullRebuildClearsAllInPriorityOrder FAIL: missing settlement after keep rebuild window.");
                return false;
            }

            var afterKeep = em.GetComponentData<FortificationComponent>(settlementAfterKeep);
            var progressAfterKeep = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementAfterKeep);
            bool keepFirst = afterKeep.DestroyedKeepCount == 0 &&
                             afterKeep.DestroyedGateCount == 1 &&
                             afterKeep.DestroyedWallSegmentCount == 1 &&
                             afterKeep.DestroyedTowerCount == 1;

            Tick(world, FullRebuildWindowSeconds);
            var afterGate = em.GetComponentData<FortificationComponent>(settlementAfterKeep);
            var progressAfterGate = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementAfterKeep);
            bool gateSecond = afterGate.DestroyedKeepCount == 0 &&
                              afterGate.DestroyedGateCount == 0 &&
                              afterGate.DestroyedWallSegmentCount == 1 &&
                              afterGate.DestroyedTowerCount == 1;

            Tick(world, FullRebuildWindowSeconds);
            var afterWall = em.GetComponentData<FortificationComponent>(settlementAfterKeep);
            var progressAfterWall = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementAfterKeep);
            bool wallThird = afterWall.DestroyedKeepCount == 0 &&
                             afterWall.DestroyedGateCount == 0 &&
                             afterWall.DestroyedWallSegmentCount == 0 &&
                             afterWall.DestroyedTowerCount == 1;

            Tick(world, FullRebuildWindowSeconds);

            if (!TryGetFactionStockpile(em, "player", out var stockpile) ||
                !em.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlementAfterKeep))
            {
                sb.AppendLine("PhaseFullRebuildClearsAllInPriorityOrder FAIL: expected final stockpile and progress surfaces.");
                return false;
            }

            var finalFortification = em.GetComponentData<FortificationComponent>(settlementAfterKeep);
            var progress = em.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementAfterKeep);
            bool towerFourth = finalFortification.DestroyedKeepCount == 0 &&
                               finalFortification.DestroyedGateCount == 0 &&
                               finalFortification.DestroyedWallSegmentCount == 0 &&
                               finalFortification.DestroyedTowerCount == 0;

            if (!keepFirst ||
                !gateSecond ||
                !wallThird ||
                !towerFourth ||
                !Approximately(stockpile.Stone, 150f, 0.001f) ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentSegment, 0f, 0.001f) ||
                progress.TargetCounter != DestroyedCounterKind.None)
            {
                sb.AppendLine(
                    "PhaseFullRebuildClearsAllInPriorityOrder FAIL: rebuild order or final state was incorrect. " +
                    $"keepFirst={keepFirst} gateSecond={gateSecond} wallThird={wallThird} towerFourth={towerFourth} " +
                    $"afterKeep=[K:{afterKeep.DestroyedKeepCount} G:{afterKeep.DestroyedGateCount} W:{afterKeep.DestroyedWallSegmentCount} T:{afterKeep.DestroyedTowerCount}] " +
                    $"afterKeepProgress=[target:{progressAfterKeep.TargetCounter} hours:{progressAfterKeep.AccumulatedWorkerHours:F4} stone:{progressAfterKeep.StoneReservedForCurrentSegment:F4}] " +
                    $"afterGate=[K:{afterGate.DestroyedKeepCount} G:{afterGate.DestroyedGateCount} W:{afterGate.DestroyedWallSegmentCount} T:{afterGate.DestroyedTowerCount}] " +
                    $"afterGateProgress=[target:{progressAfterGate.TargetCounter} hours:{progressAfterGate.AccumulatedWorkerHours:F4} stone:{progressAfterGate.StoneReservedForCurrentSegment:F4}] " +
                    $"afterWall=[K:{afterWall.DestroyedKeepCount} G:{afterWall.DestroyedGateCount} W:{afterWall.DestroyedWallSegmentCount} T:{afterWall.DestroyedTowerCount}] " +
                    $"afterWallProgress=[target:{progressAfterWall.TargetCounter} hours:{progressAfterWall.AccumulatedWorkerHours:F4} stone:{progressAfterWall.StoneReservedForCurrentSegment:F4}] " +
                    $"final=[K:{finalFortification.DestroyedKeepCount} G:{finalFortification.DestroyedGateCount} W:{finalFortification.DestroyedWallSegmentCount} T:{finalFortification.DestroyedTowerCount}] " +
                    $"target={progress.TargetCounter} hours={progress.AccumulatedWorkerHours:F2} reservedStone={progress.StoneReservedForCurrentSegment:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseFullRebuildClearsAllInPriorityOrder PASS: keep->gate->wall->tower order preserved and stockpileStone={stockpile.Stone:F2}.");
            return true;
        }

        private static World CreateValidationWorld(string worldName, bool includeBreachSealing = true)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DualClockTickSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DualClockDeclarationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationStructureLinkSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AdvanceFortificationTierSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationDestructionResolutionSystem>());
            if (includeBreachSealing)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachSealingSystem>());
            }
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DestroyedCounterRecoverySystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void Tick(World world, double seconds)
        {
            double elapsed = world.Time.ElapsedTime + world.Time.DeltaTime;
            double target = elapsed + seconds;
            while (elapsed + 0.0000001d < target)
            {
                world.SetTime(new TimeData(elapsed, SimStepSeconds));
                world.Update();
                elapsed += SimStepSeconds;
            }
        }

        private static void SeedDualClockRate(EntityManager entityManager, float daysPerSecond)
        {
            var clockQuery = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var clockEntity = clockQuery.GetSingletonEntity();
            var clock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            clock.InWorldDays = 0f;
            clock.DaysPerRealSecond = daysPerSecond;
            clock.DeclarationCount = 0;
            entityManager.SetComponentData(clockEntity, clock);
            clockQuery.Dispose();
        }

        private static Entity SeedFactionStockpile(EntityManager entityManager, string factionId, float stone)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new ResourceStockpileComponent
            {
                Stone = stone,
            });
            return entity;
        }

        private static Entity CreateIdleWorker(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = "villager",
                Role = UnitRole.Worker,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = 10f,
                Max = 10f,
            });
            entityManager.AddComponentData(entity, new WorkerGatherComponent
            {
                AssignedNode = Entity.Null,
                AssignedResourceId = default,
                CarryResourceId = default,
                CarryAmount = 0f,
                CarryCapacity = 5f,
                GatherRate = 1f,
                Phase = WorkerGatherPhase.Idle,
                GatherRadius = 0.8f,
                DepositRadius = 0.8f,
            });
            return entity;
        }

        private static Entity CreateIntactSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId)
        {
            var settlement = CreateSettlementAnchor(
                entityManager,
                settlementId,
                factionId,
                destroyedWalls: 0,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 0,
                ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Keep,
                "keep_tier_1",
                new float3(0.5f, 0f, 0f),
                currentHealth: 1600f,
                maxHealth: 1600f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateDestroyedRecoverySettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int destroyedWalls,
            int destroyedTowers,
            int destroyedGates,
            int destroyedKeeps,
            int openBreaches)
        {
            var settlement = CreateSettlementAnchor(
                entityManager,
                settlementId,
                factionId,
                destroyedWalls,
                destroyedTowers,
                destroyedGates,
                destroyedKeeps,
                openBreaches,
                ceiling: 6);

            if (destroyedKeeps == 0)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Keep,
                    "keep_tier_1",
                    new float3(0.5f, 0f, 0f),
                    currentHealth: 1600f,
                    maxHealth: 1600f);
            }

            for (int i = 0; i < destroyedWalls; i++)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Wall,
                    "wall_segment",
                    new float3(2f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 900f);
            }

            for (int i = 0; i < destroyedTowers; i++)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Tower,
                    "watch_tower",
                    new float3(4f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 700f);
            }

            for (int i = 0; i < destroyedGates; i++)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Gate,
                    "gatehouse",
                    new float3(6f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 1200f);
            }

            for (int i = 0; i < destroyedKeeps; i++)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Keep,
                    "keep_tier_1",
                    new float3(0.5f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 1600f);
            }

            return settlement;
        }

        private static Entity CreateSettlementAnchor(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int destroyedWalls,
            int destroyedTowers,
            int destroyedGates,
            int destroyedKeeps,
            int openBreaches,
            int ceiling)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = float3.zero,
            });
            entityManager.AddComponentData(entity, new SettlementComponent
            {
                SettlementId = settlementId,
                SettlementClassId = "primary_dynastic_keep",
                FortificationTier = 0,
                FortificationCeiling = ceiling,
            });
            entityManager.AddComponentData(entity, new FortificationComponent
            {
                SettlementId = settlementId,
                Tier = 0,
                Ceiling = ceiling,
                DestroyedWallSegmentCount = destroyedWalls,
                DestroyedTowerCount = destroyedTowers,
                DestroyedGateCount = destroyedGates,
                DestroyedKeepCount = destroyedKeeps,
                OpenBreachCount = openBreaches,
                EcosystemRadiusTiles = FortificationCanon.EcosystemRadiusTiles,
                AuraRadiusTiles = FortificationCanon.AuraRadiusTiles,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
                FaithWardId = "unwarded",
                FaithWardSightBonusTiles = 0f,
                FaithWardDefenderAttackMultiplier = 1f,
                FaithWardReserveHealMultiplier = 1f,
                FaithWardReserveMusterMultiplier = 1f,
                FaithWardLoyaltyProtectionMultiplier = 1f,
                FaithWardEnemySpeedMultiplier = 1f,
                FaithWardSurgeActive = false,
            });
            entityManager.AddComponentData(entity, new FortificationReserveComponent
            {
                MusterIntervalSeconds = FortificationCanon.ReserveMusterIntervalSeconds,
                ReserveHealPerSecond = FortificationCanon.ReserveTriageHealPerSecond,
                RetreatHealthRatio = FortificationCanon.ReserveRetreatHealthRatio,
                RecoveryHealthRatio = FortificationCanon.ReserveRecoveryHealthRatio,
                TriageRadiusTiles = FortificationCanon.TriageRadiusTiles,
                LastCommitAt = -999d,
            });
            return entity;
        }

        private static Entity CreateFortificationBuilding(
            EntityManager entityManager,
            string factionId,
            FortificationRole role,
            string typeId,
            float3 position,
            float currentHealth,
            float maxHealth)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = maxHealth,
            });
            entityManager.AddComponentData(entity, new BuildingTypeComponent
            {
                TypeId = typeId,
                FortificationRole = role,
                StructuralDamageMultiplier = role == FortificationRole.Gate ? 0.3f : role == FortificationRole.Wall ? 0.2f : 0.1f,
                PopulationCapBonus = 0,
                BlocksPassage = role == FortificationRole.Wall,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = false,
            });
            return entity;
        }

        private static bool TryGetSettlement(
            EntityManager entityManager,
            string settlementId,
            out Entity settlementEntity)
        {
            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<FortificationComponent>());

            using var entities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlements = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            settlementQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (settlements[i].SettlementId.Equals(new FixedString64Bytes(settlementId)))
                {
                    settlementEntity = entities[i];
                    return true;
                }
            }

            settlementEntity = Entity.Null;
            return false;
        }

        private static bool TryGetFactionStockpile(
            EntityManager entityManager,
            string factionId,
            out ResourceStockpileComponent stockpile)
        {
            var stockpileQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ResourceStockpileComponent>());

            using var factions = stockpileQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = stockpileQuery.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            stockpileQuery.Dispose();

            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(new FixedString32Bytes(factionId)))
                {
                    stockpile = stockpiles[i];
                    return true;
                }
            }

            stockpile = default;
            return false;
        }

        private static int GetTotalDestroyedCount(FortificationComponent fortification)
        {
            return fortification.DestroyedWallSegmentCount +
                   fortification.DestroyedTowerCount +
                   fortification.DestroyedGateCount +
                   fortification.DestroyedKeepCount;
        }

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }
    }
}
#endif
