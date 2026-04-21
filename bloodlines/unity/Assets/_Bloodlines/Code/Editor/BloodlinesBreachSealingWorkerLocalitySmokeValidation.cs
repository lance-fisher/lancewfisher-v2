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
    /// Dedicated smoke validator for settlement-local worker gating during breach sealing.
    /// </summary>
    public static class BloodlinesBreachSealingWorkerLocalitySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-breach-sealing-worker-locality-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float HalfSealingWindowSeconds = 4f;
        private const float FullSealingWindowSeconds = 20f;

        [MenuItem("Bloodlines/Fortification/Run Breach Sealing Worker Locality Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchBreachSealingWorkerLocalitySmokeValidation()
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
                message = "Breach sealing worker locality smoke errored: " + e;
            }

            string artifact = "BLOODLINES_BREACH_SEALING_WORKER_LOCALITY_SMOKE " +
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
            ok &= RunWorkersAtCorrectSettlementSealPhase(sb);
            ok &= RunWorkersAtOtherSettlementDoNotSealPhase(sb);
            ok &= RunNoWorkersBlocksPhase(sb);
            ok &= RunNoIdleWorkersBlocksEvenIfPresentPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunWorkersAtCorrectSettlementSealPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingWorkerLocality_CorrectSettlement");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateBreachedSettlement(em, "keep_alpha", "player");
            CreateWorker(em, "player", PositionFor("keep_alpha", 0.25f, 0f), WorkerGatherPhase.Idle);

            Tick(world, FullSealingWindowSeconds);

            return ValidateSettlementProgress(
                sb,
                phaseName: "PhaseWorkersAtCorrectSettlementSeal",
                entityManager: em,
                settlementId: "keep_alpha",
                factionId: "player",
                expectedOpenBreaches: 0,
                expectedAccumulatedWorkerHours: 0f,
                expectedReservedStone: 0f,
                expectedStockpileStone: 30f,
                expectedHasProgress: true);
        }

        private static bool RunWorkersAtOtherSettlementDoNotSealPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingWorkerLocality_OtherSettlement");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateOwnedControlPoint(em, "cp_beta", "player", AnchorPositionFor("keep_beta"));
            CreateBreachedSettlement(em, "keep_alpha", "player");
            CreateIntactSettlement(em, "keep_beta", "player");
            CreateWorker(em, "player", PositionFor("keep_beta", 0.25f, 0f), WorkerGatherPhase.Idle);

            Tick(world, HalfSealingWindowSeconds);

            return ValidateSettlementProgress(
                sb,
                phaseName: "PhaseWorkersAtOtherSettlementDoNotSeal",
                entityManager: em,
                settlementId: "keep_alpha",
                factionId: "player",
                expectedOpenBreaches: 1,
                expectedAccumulatedWorkerHours: 0f,
                expectedReservedStone: 0f,
                expectedStockpileStone: 120f,
                expectedHasProgress: true);
        }

        private static bool RunNoWorkersBlocksPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingWorkerLocality_NoWorkers");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateBreachedSettlement(em, "keep_alpha", "player");

            Tick(world, HalfSealingWindowSeconds);

            return ValidateSettlementProgress(
                sb,
                phaseName: "PhaseNoWorkersBlocks",
                entityManager: em,
                settlementId: "keep_alpha",
                factionId: "player",
                expectedOpenBreaches: 1,
                expectedAccumulatedWorkerHours: 0f,
                expectedReservedStone: 0f,
                expectedStockpileStone: 120f,
                expectedHasProgress: true);
        }

        private static bool RunNoIdleWorkersBlocksEvenIfPresentPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingWorkerLocality_NoIdleWorkers");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 120f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateBreachedSettlement(em, "keep_alpha", "player");
            CreateWorker(em, "player", PositionFor("keep_alpha", 0.25f, 0f), WorkerGatherPhase.Seeking);

            Tick(world, HalfSealingWindowSeconds);

            return ValidateSettlementProgress(
                sb,
                phaseName: "PhaseNoIdleWorkersBlocksEvenIfPresent",
                entityManager: em,
                settlementId: "keep_alpha",
                factionId: "player",
                expectedOpenBreaches: 1,
                expectedAccumulatedWorkerHours: 0f,
                expectedReservedStone: 0f,
                expectedStockpileStone: 120f,
                expectedHasProgress: true);
        }

        private static bool ValidateSettlementProgress(
            System.Text.StringBuilder sb,
            string phaseName,
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int expectedOpenBreaches,
            float expectedAccumulatedWorkerHours,
            float expectedReservedStone,
            float expectedStockpileStone,
            bool expectedHasProgress)
        {
            if (!TryGetSettlement(entityManager, settlementId, out var settlement) ||
                !TryGetFactionStockpile(entityManager, factionId, out var stockpile))
            {
                sb.AppendLine($"{phaseName} FAIL: expected settlement and faction stockpile.");
                return false;
            }

            bool hasProgress = entityManager.HasComponent<BreachSealingProgressComponent>(settlement);
            var fortification = entityManager.GetComponentData<FortificationComponent>(settlement);
            float accumulatedWorkerHours = 0f;
            float reservedStone = 0f;
            if (hasProgress)
            {
                var progress = entityManager.GetComponentData<BreachSealingProgressComponent>(settlement);
                accumulatedWorkerHours = progress.AccumulatedWorkerHours;
                reservedStone = progress.StoneReservedForCurrentBreach;
            }

            if (fortification.OpenBreachCount != expectedOpenBreaches ||
                hasProgress != expectedHasProgress ||
                !Approximately(accumulatedWorkerHours, expectedAccumulatedWorkerHours, 0.001f) ||
                !Approximately(reservedStone, expectedReservedStone, 0.001f) ||
                !Approximately(stockpile.Stone, expectedStockpileStone, 0.001f))
            {
                sb.AppendLine(
                    $"{phaseName} FAIL: locality gating drifted. " +
                    $"breaches={fortification.OpenBreachCount}/{expectedOpenBreaches} " +
                    $"hasProgress={hasProgress}/{expectedHasProgress} " +
                    $"hours={accumulatedWorkerHours:F2}/{expectedAccumulatedWorkerHours:F2} " +
                    $"reservedStone={reservedStone:F2}/{expectedReservedStone:F2} " +
                    $"stockpileStone={stockpile.Stone:F2}/{expectedStockpileStone:F2}");
                return false;
            }

            sb.AppendLine(
                $"{phaseName} PASS: breaches={fortification.OpenBreachCount} hours={accumulatedWorkerHours:F2} reservedStone={reservedStone:F2} stockpileStone={stockpile.Stone:F2}.");
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

        private static Entity CreateOwnedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float3 position)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new ControlPointComponent
            {
                ControlPointId = controlPointId,
                OwnerFactionId = ownerFactionId,
                CaptureFactionId = default,
                ContinentId = "validation",
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = 100f,
                CaptureProgress = 0f,
                SettlementClassId = "primary_dynastic_keep",
                FortificationTier = 1,
                RadiusTiles = 9f,
                CaptureTimeSeconds = 12f,
                GoldTrickle = 0f,
                FoodTrickle = 0f,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0f,
            });
            return entity;
        }

        private static Entity CreateWorker(
            EntityManager entityManager,
            string factionId,
            float3 position,
            WorkerGatherPhase phase)
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
                Phase = phase,
                GatherRadius = 0.8f,
                DepositRadius = 0.8f,
            });
            return entity;
        }

        private static Entity CreateBreachedSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId)
        {
            float3 anchorPosition = AnchorPositionFor(settlementId);
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, anchorPosition, ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Keep,
                "keep_tier_1",
                anchorPosition + new float3(0.5f, 0f, 0f),
                currentHealth: 1600f,
                maxHealth: 1600f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                anchorPosition + new float3(2f, 0f, 0f),
                currentHealth: 0f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateIntactSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId)
        {
            float3 anchorPosition = AnchorPositionFor(settlementId);
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, anchorPosition, ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Keep,
                "keep_tier_1",
                anchorPosition + new float3(0.5f, 0f, 0f),
                currentHealth: 1600f,
                maxHealth: 1600f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                anchorPosition + new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateSettlementAnchor(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            float3 position,
            int ceiling)
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

        private static float3 AnchorPositionFor(string settlementId)
        {
            return settlementId switch
            {
                "keep_beta" => new float3(32f, 0f, 0f),
                _ => float3.zero,
            };
        }

        private static float3 PositionFor(string settlementId, float offsetX, float offsetZ)
        {
            return AnchorPositionFor(settlementId) + new float3(offsetX, 0f, offsetZ);
        }

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }
    }
}
#endif
