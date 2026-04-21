#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
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
    /// Dedicated smoke validator for fortification repair narrative push coverage.
    /// </summary>
    public static class BloodlinesFortificationRepairNarrativeSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-fortification-repair-narrative-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float SingleBreachSealWindowSeconds = 20f;
        private const float ThreeBreachSealWindowSeconds = 60f;
        private const float WallRebuildWindowSeconds = 14f;

        [MenuItem("Bloodlines/Fortification/Run Fortification Repair Narrative Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchFortificationRepairNarrativeSmokeValidation()
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
                message = "Fortification repair narrative smoke errored: " + e;
            }

            string artifact = "BLOODLINES_FORTIFICATION_REPAIR_NARRATIVE_SMOKE " +
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
            ok &= RunSingleBreachClosePushesOnePhase(sb);
            ok &= RunThreeBreachClosesPushThreePhase(sb);
            ok &= RunWallRebuildPushesOnePhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunSingleBreachClosePushesOnePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateSealingValidationWorld("BloodlinesFortificationRepairNarrative_SingleSeal");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 180f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateBreachedSettlement(
                em,
                "keep_alpha",
                "player",
                destroyedWalls: 1,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 1);
            CreateWorker(em, "player", PositionFor("keep_alpha", 0.25f, 0f), WorkerGatherPhase.Idle);

            Tick(world, SingleBreachSealWindowSeconds);

            if (!TryGetSettlement(em, "keep_alpha", out var settlement))
            {
                sb.AppendLine("PhaseSingleBreachClosePushesOne FAIL: missing settlement.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            if (fortification.OpenBreachCount != 0)
            {
                sb.AppendLine($"PhaseSingleBreachClosePushesOne FAIL: breach remained open at count {fortification.OpenBreachCount}.");
                return false;
            }

            return ValidateNarrativeBuffer(
                sb,
                "PhaseSingleBreachClosePushesOne",
                em,
                expectedCount: 1,
                expectedText: "player's masons seal a breach at keep_alpha.",
                expectedTone: NarrativeMessageTone.Info);
        }

        private static bool RunThreeBreachClosesPushThreePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateSealingValidationWorld("BloodlinesFortificationRepairNarrative_ThreeSeals");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 360f);
            CreateOwnedControlPoint(em, "cp_alpha", "player", AnchorPositionFor("keep_alpha"));
            CreateBreachedSettlement(
                em,
                "keep_alpha",
                "player",
                destroyedWalls: 3,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 3);
            CreateWorker(em, "player", PositionFor("keep_alpha", 0.25f, 0f), WorkerGatherPhase.Idle);

            Tick(world, ThreeBreachSealWindowSeconds);

            if (!TryGetSettlement(em, "keep_alpha", out var settlement))
            {
                sb.AppendLine("PhaseThreeBreachClosesPushThree FAIL: missing settlement.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            if (fortification.OpenBreachCount != 0)
            {
                sb.AppendLine($"PhaseThreeBreachClosesPushThree FAIL: expected all breaches sealed, got {fortification.OpenBreachCount} remaining.");
                return false;
            }

            return ValidateNarrativeBuffer(
                sb,
                "PhaseThreeBreachClosesPushThree",
                em,
                expectedCount: 3,
                expectedText: "player's masons seal a breach at keep_alpha.",
                expectedTone: NarrativeMessageTone.Info);
        }

        private static bool RunWallRebuildPushesOnePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateRecoveryValidationWorld("BloodlinesFortificationRepairNarrative_WallRebuild");
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 180f);
            CreateWorker(em, "player", PositionFor("keep_alpha", 0.25f, 0f), WorkerGatherPhase.Idle);
            CreateBreachedSettlement(
                em,
                "keep_alpha",
                "player",
                destroyedWalls: 1,
                destroyedTowers: 0,
                destroyedGates: 0,
                destroyedKeeps: 0,
                openBreaches: 0);

            Tick(world, WallRebuildWindowSeconds);

            if (!TryGetSettlement(em, "keep_alpha", out var settlement))
            {
                sb.AppendLine("PhaseWallRebuildPushesOne FAIL: missing settlement.");
                return false;
            }

            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            if (fortification.DestroyedWallSegmentCount != 0)
            {
                sb.AppendLine($"PhaseWallRebuildPushesOne FAIL: wall remained destroyed at count {fortification.DestroyedWallSegmentCount}.");
                return false;
            }

            return ValidateNarrativeBuffer(
                sb,
                "PhaseWallRebuildPushesOne",
                em,
                expectedCount: 1,
                expectedText: "player rebuilds a wall at keep_alpha.",
                expectedTone: NarrativeMessageTone.Info);
        }

        private static bool ValidateNarrativeBuffer(
            System.Text.StringBuilder sb,
            string phaseName,
            EntityManager entityManager,
            int expectedCount,
            string expectedText,
            NarrativeMessageTone expectedTone)
        {
            if (!TryGetNarrativeBuffer(entityManager, out var buffer))
            {
                sb.AppendLine($"{phaseName} FAIL: narrative singleton missing.");
                return false;
            }

            if (buffer.Length != expectedCount)
            {
                sb.AppendLine($"{phaseName} FAIL: expected {expectedCount} messages, got {buffer.Length}.");
                return false;
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].Text.Equals(new FixedString128Bytes(expectedText)) ||
                    buffer[i].Tone != expectedTone)
                {
                    sb.AppendLine(
                        $"{phaseName} FAIL: message[{i}] drifted. " +
                        $"text='{buffer[i].Text}' tone={buffer[i].Tone}.");
                    return false;
                }
            }

            sb.AppendLine($"{phaseName} PASS: {expectedCount} info message(s) matched '{expectedText}'.");
            return true;
        }

        private static World CreateSealingValidationWorld(string worldName)
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

        private static World CreateRecoveryValidationWorld(string worldName)
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
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DestroyedCounterRecoverySystem>());
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
            string factionId,
            int destroyedWalls,
            int destroyedTowers,
            int destroyedGates,
            int destroyedKeeps,
            int openBreaches)
        {
            float3 anchorPosition = AnchorPositionFor(settlementId);
            var settlement = CreateSettlementAnchor(entityManager, settlementId, factionId, anchorPosition, ceiling: 6);

            if (destroyedKeeps == 0)
            {
                CreateFortificationBuilding(
                    entityManager,
                    factionId,
                    FortificationRole.Keep,
                    "keep_tier_1",
                    anchorPosition + new float3(0.5f, 0f, 0f),
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
                    anchorPosition + new float3(2f + i, 0f, 0f),
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
                    anchorPosition + new float3(6f + i, 0f, 0f),
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
                    anchorPosition + new float3(9f + i, 0f, 0f),
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
                    anchorPosition + new float3(0.5f + i, 0f, 0f),
                    currentHealth: 0f,
                    maxHealth: 1600f);
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlement);
            fortification.DestroyedWallSegmentCount = destroyedWalls;
            fortification.DestroyedTowerCount = destroyedTowers;
            fortification.DestroyedGateCount = destroyedGates;
            fortification.DestroyedKeepCount = destroyedKeeps;
            fortification.OpenBreachCount = openBreaches;
            entityManager.SetComponentData(settlement, fortification);
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

        private static bool TryGetNarrativeBuffer(
            EntityManager entityManager,
            out DynamicBuffer<NarrativeMessageElement> buffer)
        {
            buffer = default;
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<NarrativeMessageSingleton>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            var singleton = query.GetSingletonEntity();
            query.Dispose();
            buffer = entityManager.GetBuffer<NarrativeMessageElement>(singleton);
            return true;
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
    }
}
#endif
