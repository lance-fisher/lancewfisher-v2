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
    /// Dedicated smoke validator for the fortification breach sealing / recovery slice.
    /// Proves OpenBreachCount can recover over time through stone plus idle-worker
    /// commitment without repairing destroyed wall, gate, tower, or keep counters.
    /// </summary>
    public static class BloodlinesBreachSealingRecoverySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-breach-sealing-recovery-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float HalfSealingWindowSeconds = 4f;
        private const float FullSealingWindowSeconds = 8f;

        [MenuItem("Bloodlines/Fortification/Run Breach Sealing Recovery Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchBreachSealingRecoverySmokeValidation()
        {
            if (string.Equals(
                    Environment.GetEnvironmentVariable("BLOODLINES_BREACH_SEALING_MODE"),
                    "worker-locality",
                    StringComparison.OrdinalIgnoreCase))
            {
                BloodlinesBreachSealingWorkerLocalitySmokeValidation.RunBatchBreachSealingWorkerLocalitySmokeValidation();
                return;
            }

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
                message = "Breach sealing recovery smoke errored: " + e;
            }

            string artifact = "BLOODLINES_BREACH_SEALING_RECOVERY_SMOKE " +
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
            ok &= RunSealingProgressAccumulatesPhase(sb);
            ok &= RunFirstBreachSealedPhase(sb);
            ok &= RunInsufficientStoneBlocksPhase(sb);
            ok &= RunNoIdleWorkersBlocksPhase(sb);
            ok &= RunNoOpenBreachesNoOpPhase(sb);
            ok &= RunFullSealingClearsAllPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunSealingProgressAccumulatesPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_Progress");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 200f);
            CreateIdleWorker(em, "player");
            CreateBreachedSettlement(em, "keep_player", "player", destroyedWalls: 1, destroyedGates: 1);

            Tick(world, HalfSealingWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<BreachSealingProgressComponent>(settlement))
            {
                sb.AppendLine("PhaseSealingProgressAccumulates FAIL: expected breach sealing progress component after the first sealing window.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 2 ||
                !Approximately(progress.AccumulatedWorkerHours, 4f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseSealingProgressAccumulates FAIL: sealing progress did not accumulate proportionally. " +
                    $"breaches={fortification.OpenBreachCount} hours={progress.AccumulatedWorkerHours:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseSealingProgressAccumulates PASS: breaches={fortification.OpenBreachCount} accumulatedHours={progress.AccumulatedWorkerHours:F2}.");
            return true;
        }

        private static bool RunFirstBreachSealedPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_FirstSeal");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 200f);
            CreateIdleWorker(em, "player");
            CreateBreachedSettlement(em, "keep_player", "player", destroyedWalls: 1, destroyedGates: 1);

            Tick(world, HalfSealingWindowSeconds);
            Tick(world, HalfSealingWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<BreachSealingProgressComponent>(settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseFirstBreachSealed FAIL: expected settlement progress and faction stockpile surfaces.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, 0f, 0.001f) ||
                !Approximately(stockpile.Stone, 140f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseFirstBreachSealed FAIL: first breach did not seal cleanly. " +
                    $"breaches={fortification.OpenBreachCount} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentBreach:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseFirstBreachSealed PASS: breaches={fortification.OpenBreachCount} stockpileStone={stockpile.Stone:F2}.");
            return true;
        }

        private static bool RunInsufficientStoneBlocksPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_NoStone");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 59f);
            CreateIdleWorker(em, "player");
            CreateBreachedSettlement(em, "keep_player", "player", destroyedWalls: 1, destroyedGates: 0);

            Tick(world, HalfSealingWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<BreachSealingProgressComponent>(settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseInsufficientStoneBlocks FAIL: expected progress component and faction stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, 0f, 0.001f) ||
                !Approximately(stockpile.Stone, 59f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseInsufficientStoneBlocks FAIL: sealing should block when stone is below one breach cost. " +
                    $"breaches={fortification.OpenBreachCount} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentBreach:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine("PhaseInsufficientStoneBlocks PASS: no sealing progress with stone below 60.");
            return true;
        }

        private static bool RunNoIdleWorkersBlocksPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_NoWorkers");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateBreachedSettlement(em, "keep_player", "player", destroyedWalls: 1, destroyedGates: 0);

            Tick(world, HalfSealingWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !em.HasComponent<BreachSealingProgressComponent>(settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseNoIdleWorkersBlocks FAIL: expected progress component and faction stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, 0f, 0.001f) ||
                !Approximately(stockpile.Stone, 120f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseNoIdleWorkersBlocks FAIL: sealing should block without idle workers. " +
                    $"breaches={fortification.OpenBreachCount} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentBreach:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine("PhaseNoIdleWorkersBlocks PASS: no sealing progress without idle workers.");
            return true;
        }

        private static bool RunNoOpenBreachesNoOpPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_NoBreaches");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateIdleWorker(em, "player");
            CreateIntactSettlement(em, "keep_player", "player");

            Tick(world, HalfSealingWindowSeconds);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseNoOpenBreachesNoOp FAIL: expected settlement and stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            if (fortification.OpenBreachCount != 0 ||
                em.HasComponent<BreachSealingProgressComponent>(settlement) ||
                !Approximately(stockpile.Stone, 120f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseNoOpenBreachesNoOp FAIL: intact settlements should remain untouched. " +
                    $"breaches={fortification.OpenBreachCount} hasProgress={em.HasComponent<BreachSealingProgressComponent>(settlement)} " +
                    $"stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine("PhaseNoOpenBreachesNoOp PASS: no component added and no stone spent for intact fortifications.");
            return true;
        }

        private static bool RunFullSealingClearsAllPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingRecovery_ClearAll");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 300f);
            CreateIdleWorker(em, "player");
            CreateBreachedSettlement(em, "keep_player", "player", destroyedWalls: 2, destroyedGates: 1);

            Tick(world, FullSealingWindowSeconds * 3f);

            if (!TryGetSettlement(em, "keep_player", out var settlement) ||
                !TryGetFactionStockpile(em, "player", out var stockpile))
            {
                sb.AppendLine("PhaseFullSealingClearsAll FAIL: expected settlement and faction stockpile.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            var progress = em.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.OpenBreachCount != 0 ||
                !Approximately(progress.AccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, 0f, 0.001f) ||
                !Approximately(stockpile.Stone, 120f, 0.001f))
            {
                sb.AppendLine(
                    "PhaseFullSealingClearsAll FAIL: full sealing did not consume the expected stone or clear all open breaches. " +
                    $"breaches={fortification.OpenBreachCount} hours={progress.AccumulatedWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentBreach:F2} stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseFullSealingClearsAll PASS: breaches={fortification.OpenBreachCount} stockpileStone={stockpile.Stone:F2}.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
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
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachSealingSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void Tick(World world, double seconds)
        {
            double elapsed = world.Time.ElapsedTime;
            double target = elapsed + seconds;
            while (elapsed < target)
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
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, ceiling: 4);
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

        private static Entity CreateBreachedSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int destroyedWalls,
            int destroyedGates)
        {
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, ceiling: 6);
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

            for (int i = 0; i < destroyedGates; i++)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Gate,
                    "gatehouse",
                    new float3(5f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 1200f);
            }

            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Keep,
                "keep_tier_1",
                new float3(0.5f, 0f, 0f),
                currentHealth: 1600f,
                maxHealth: 1600f);
            return settlement;
        }

        private static Entity CreateSettlementAnchor(
            EntityManager entityManager,
            string settlementId,
            string factionId,
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
                DestroyedWallSegmentCount = 0,
                DestroyedTowerCount = 0,
                DestroyedGateCount = 0,
                DestroyedKeepCount = 0,
                OpenBreachCount = 0,
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

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }
    }
}
#endif
