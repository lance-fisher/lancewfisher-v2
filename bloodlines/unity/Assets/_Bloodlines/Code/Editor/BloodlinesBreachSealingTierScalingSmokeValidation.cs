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
    /// Dedicated smoke validator for tier-scaled breach sealing costs.
    /// </summary>
    public static class BloodlinesBreachSealingTierScalingSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-breach-sealing-tier-scaling-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float Tier1HalfWindowSeconds = 4f;
        private const float Tier2HalfWindowSeconds = 6f;
        private const float Tier3HalfWindowSeconds = 9f;
        private const float MixedPortfolioWindowSeconds = 3f;

        [MenuItem("Bloodlines/Fortification/Run Breach Sealing Tier Scaling Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchBreachSealingTierScalingSmokeValidation()
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
                message = "Breach sealing tier scaling smoke errored: " + e;
            }

            string artifact = "BLOODLINES_BREACH_SEALING_TIER_SCALING_SMOKE " +
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
            ok &= RunTier1BaseCostMatchesPhase(sb);
            ok &= RunTier2CostScalesPhase(sb);
            ok &= RunTier3CostScalesPhase(sb);
            ok &= RunMixedTierPortfolioPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunTier1BaseCostMatchesPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingTierScaling_Tier1");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 200f);
            CreateIdleWorker(em, "player");
            CreateTier1BreachedSettlement(em, "keep_player", "player");

            Tick(world, Tier1HalfWindowSeconds);

            return ValidateSingleSettlementPhase(
                sb,
                phaseName: "PhaseTier1BaseCostMatches",
                entityManager: em,
                settlementId: "keep_player",
                factionId: "player",
                expectedTier: 1,
                expectedAccumulatedWorkerHours: 4f,
                expectedRequiredWorkerHours: FortificationCanon.BreachSealingWorkerHoursPerBreach,
                expectedReservedStone: FortificationCanon.BreachSealingStoneCostPerBreach,
                expectedRemainingStone: 140f,
                expectedProgress01: 0.5f);
        }

        private static bool RunTier2CostScalesPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingTierScaling_Tier2");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 240f);
            CreateIdleWorker(em, "player");
            CreateTier2BreachedSettlement(em, "keep_player", "player");

            Tick(world, Tier2HalfWindowSeconds);

            return ValidateSingleSettlementPhase(
                sb,
                phaseName: "PhaseTier2CostScales",
                entityManager: em,
                settlementId: "keep_player",
                factionId: "player",
                expectedTier: 2,
                expectedAccumulatedWorkerHours: 6f,
                expectedRequiredWorkerHours: FortificationCanon.BreachSealingTier2WorkerHoursPerBreach,
                expectedReservedStone: FortificationCanon.BreachSealingTier2StoneCostPerBreach,
                expectedRemainingStone: 150f,
                expectedProgress01: 0.5f);
        }

        private static bool RunTier3CostScalesPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingTierScaling_Tier3");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 360f);
            CreateIdleWorker(em, "player");
            CreateTier3BreachedSettlement(em, "keep_player", "player");

            Tick(world, Tier3HalfWindowSeconds);

            return ValidateSingleSettlementPhase(
                sb,
                phaseName: "PhaseTier3CostScales",
                entityManager: em,
                settlementId: "keep_player",
                factionId: "player",
                expectedTier: 3,
                expectedAccumulatedWorkerHours: 9f,
                expectedRequiredWorkerHours: FortificationCanon.BreachSealingTier3WorkerHoursPerBreach,
                expectedReservedStone: FortificationCanon.BreachSealingTier3StoneCostPerBreach,
                expectedRemainingStone: 225f,
                expectedProgress01: 0.5f);
        }

        private static bool RunMixedTierPortfolioPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachSealingTierScaling_MixedPortfolio");
            var em = world.EntityManager;
            string alphaError = string.Empty;
            string betaError = string.Empty;
            string gammaError = string.Empty;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 400f);
            CreateIdleWorker(em, "player");
            CreateIdleWorker(em, "player");
            CreateIdleWorker(em, "player");
            CreateTier1BreachedSettlement(em, "keep_alpha", "player");
            CreateTier2BreachedSettlement(em, "keep_beta", "player");
            CreateTier3BreachedSettlement(em, "keep_gamma", "player");

            Tick(world, MixedPortfolioWindowSeconds);

            if (!ValidatePortfolioSettlement(
                    em,
                    "keep_alpha",
                    expectedTier: 1,
                    expectedAccumulatedWorkerHours: 3f,
                    expectedRequiredWorkerHours: FortificationCanon.BreachSealingWorkerHoursPerBreach,
                    expectedReservedStone: FortificationCanon.BreachSealingStoneCostPerBreach,
                    expectedProgress01: 0.375f,
                    out alphaError) ||
                !ValidatePortfolioSettlement(
                    em,
                    "keep_beta",
                    expectedTier: 2,
                    expectedAccumulatedWorkerHours: 3f,
                    expectedRequiredWorkerHours: FortificationCanon.BreachSealingTier2WorkerHoursPerBreach,
                    expectedReservedStone: FortificationCanon.BreachSealingTier2StoneCostPerBreach,
                    expectedProgress01: 0.25f,
                    out betaError) ||
                !ValidatePortfolioSettlement(
                    em,
                    "keep_gamma",
                    expectedTier: 3,
                    expectedAccumulatedWorkerHours: 3f,
                    expectedRequiredWorkerHours: FortificationCanon.BreachSealingTier3WorkerHoursPerBreach,
                    expectedReservedStone: FortificationCanon.BreachSealingTier3StoneCostPerBreach,
                    expectedProgress01: 1f / 6f,
                    out gammaError) ||
                !TryGetFactionStockpile(em, "player", out var stockpile) ||
                !Approximately(stockpile.Stone, 115f, 0.001f))
            {
                string stockpileStatus = TryGetFactionStockpile(em, "player", out var currentStockpile)
                    ? currentStockpile.Stone.ToString("F2")
                    : "missing";
                sb.AppendLine(
                    "PhaseMixedTierPortfolio FAIL: portfolio scaling drifted. " +
                    $"{alphaError} {betaError} {gammaError} stockpileStone={stockpileStatus}");
                return false;
            }

            sb.AppendLine(
                $"PhaseMixedTierPortfolio PASS: tier1/tier2/tier3 reserved stone totals matched with stockpileStone={stockpile.Stone:F2}.");
            return true;
        }

        private static bool ValidateSingleSettlementPhase(
            System.Text.StringBuilder sb,
            string phaseName,
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int expectedTier,
            float expectedAccumulatedWorkerHours,
            float expectedRequiredWorkerHours,
            float expectedReservedStone,
            float expectedRemainingStone,
            float expectedProgress01)
        {
            if (!TryGetSettlement(entityManager, settlementId, out var settlement) ||
                !entityManager.HasComponent<BreachSealingProgressComponent>(settlement) ||
                !TryGetFactionStockpile(entityManager, factionId, out var stockpile))
            {
                sb.AppendLine($"{phaseName} FAIL: expected settlement, progress component, and stockpile.");
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlement);
            var progress = entityManager.GetComponentData<BreachSealingProgressComponent>(settlement);
            float expectedRequiredStone = FortificationCanon.ResolveBreachSealingStoneCostPerBreach(expectedTier);
            float expectedDynamicWorkerHours = FortificationCanon.ResolveBreachSealingWorkerHoursPerBreach(expectedTier);
            if (fortification.Tier != expectedTier ||
                fortification.OpenBreachCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, expectedAccumulatedWorkerHours, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, expectedReservedStone, 0.001f) ||
                !Approximately(stockpile.Stone, expectedRemainingStone, 0.001f) ||
                !Approximately(expectedRequiredWorkerHours, expectedDynamicWorkerHours, 0.001f) ||
                !Approximately(expectedReservedStone, expectedRequiredStone, 0.001f) ||
                !Approximately(progress.AccumulatedWorkerHours / expectedRequiredWorkerHours, expectedProgress01, 0.001f))
            {
                sb.AppendLine(
                    $"{phaseName} FAIL: tier scaling drifted. " +
                    $"tier={fortification.Tier} breaches={fortification.OpenBreachCount} " +
                    $"hours={progress.AccumulatedWorkerHours:F2}/{expectedRequiredWorkerHours:F2} " +
                    $"reservedStone={progress.StoneReservedForCurrentBreach:F2}/{expectedRequiredStone:F2} " +
                    $"stockpileStone={stockpile.Stone:F2}");
                return false;
            }

            sb.AppendLine(
                $"{phaseName} PASS: tier={fortification.Tier} hours={progress.AccumulatedWorkerHours:F2}/{expectedRequiredWorkerHours:F2} reservedStone={progress.StoneReservedForCurrentBreach:F2}/{expectedRequiredStone:F2}.");
            return true;
        }

        private static bool ValidatePortfolioSettlement(
            EntityManager entityManager,
            string settlementId,
            int expectedTier,
            float expectedAccumulatedWorkerHours,
            float expectedRequiredWorkerHours,
            float expectedReservedStone,
            float expectedProgress01,
            out string error)
        {
            error = string.Empty;
            if (!TryGetSettlement(entityManager, settlementId, out var settlement) ||
                !entityManager.HasComponent<BreachSealingProgressComponent>(settlement))
            {
                error = $"{settlementId}=missing";
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlement);
            var progress = entityManager.GetComponentData<BreachSealingProgressComponent>(settlement);
            if (fortification.Tier != expectedTier ||
                fortification.OpenBreachCount != 1 ||
                !Approximately(progress.AccumulatedWorkerHours, expectedAccumulatedWorkerHours, 0.001f) ||
                !Approximately(progress.StoneReservedForCurrentBreach, expectedReservedStone, 0.001f) ||
                !Approximately(progress.AccumulatedWorkerHours / expectedRequiredWorkerHours, expectedProgress01, 0.001f))
            {
                error =
                    $"{settlementId}=tier{fortification.Tier} hours={progress.AccumulatedWorkerHours:F2}/{expectedRequiredWorkerHours:F2} reservedStone={progress.StoneReservedForCurrentBreach:F2}/{expectedReservedStone:F2}";
                return false;
            }

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

        private static Entity CreateTier1BreachedSettlement(EntityManager entityManager, string settlementId, string factionId)
        {
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                PositionFor(settlementId, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                PositionFor(settlementId, 2f, 0f),
                currentHealth: 0f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateTier2BreachedSettlement(EntityManager entityManager, string settlementId, string factionId)
        {
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Gate,
                "gatehouse",
                PositionFor(settlementId, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                PositionFor(settlementId, 2f, 0f),
                currentHealth: 0f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateTier3BreachedSettlement(EntityManager entityManager, string settlementId, string factionId)
        {
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, ceiling: 6);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Gate,
                "gatehouse",
                PositionFor(settlementId, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                PositionFor(settlementId, 2f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateFortificationBuilding(
                entityManager,
                factionId,
                FortificationRole.Wall,
                "wall_segment",
                PositionFor(settlementId, 4f, 0f),
                currentHealth: 0f,
                maxHealth: 900f);
            return settlement;
        }

        private static Entity CreateSettlementAnchor(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int ceiling)
        {
            float3 anchorPosition = AnchorPositionFor(settlementId);
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = anchorPosition,
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
                "keep_alpha" => new float3(0f, 0f, 0f),
                "keep_beta" => new float3(32f, 0f, 0f),
                "keep_gamma" => new float3(64f, 0f, 0f),
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
