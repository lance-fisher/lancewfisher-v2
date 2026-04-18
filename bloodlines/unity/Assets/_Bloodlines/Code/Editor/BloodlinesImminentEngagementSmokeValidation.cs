#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Fortification;
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
    /// Governed imminent-engagement smoke validator. Runs in isolated ECS worlds and proves:
    ///
    ///   1. Tier-0 settlements never activate the warning window even with nearby hostiles.
    ///   2. Tier-1 settlements stay inactive without hostiles inside the warning radius.
    ///   3. Tier-1 settlements activate with three hostile units inside the warning radius.
    ///   4. An already-active engagement expires into WindowConsumed with EngagedAt set.
    ///
    /// Artifact: artifacts/unity-imminent-engagement-smoke.log.
    /// </summary>
    public static class BloodlinesImminentEngagementSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-imminent-engagement-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float TileSize = 64f;

        [MenuItem("Bloodlines/Fortification/Run Imminent Engagement Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchImminentEngagementSmokeValidation()
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
                message = "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunTierGuardPhase(out string tierGuardMessage))
            {
                message = tierGuardMessage;
                return false;
            }

            if (!RunNoThreatPhase(out string idleMessage))
            {
                message = idleMessage;
                return false;
            }

            if (!RunActivationPhase(out string activationMessage))
            {
                message = activationMessage;
                return false;
            }

            if (!RunExpiryPhase(out string expiryMessage))
            {
                message = expiryMessage;
                return false;
            }

            message =
                "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE PASS: phase1=True, phase2=True, phase3=True, phase4=True. " +
                tierGuardMessage + " " + idleMessage + " " + activationMessage + " " + expiryMessage;
            return true;
        }

        private static bool RunTierGuardPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementSmokeValidation_TierGuard");
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", tier: 0);
            CreateHostileUnit(entityManager, "enemy", new float3(2f * TileSize, 0f, 0f));

            Tick(world, seconds: 0.1d);

            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            if (engagement.Active || engagement.WindowConsumed)
            {
                message = "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL: phase 1 activated despite fortification tier 0.";
                return false;
            }

            message = "Phase1: tierGuardHeld=True, active=False.";
            return true;
        }

        private static bool RunNoThreatPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementSmokeValidation_NoThreat");
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", tier: 1);
            CreateFortificationContribution(entityManager, settlement, "keep_player", contribution: 1, position: new float3(TileSize, 0f, 0f));
            Tick(world, seconds: 0.1d);

            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            if (engagement.Active || engagement.WindowConsumed || engagement.HostileCount != 0)
            {
                message =
                    "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL: phase 2 should remain idle. " +
                    "active=" + engagement.Active +
                    ", windowConsumed=" + engagement.WindowConsumed +
                    ", hostileCount=" + engagement.HostileCount + ".";
                return false;
            }

            message = "Phase2: noThreatHeld=True, hostileCount=0.";
            return true;
        }

        private static bool RunActivationPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementSmokeValidation_Activation");
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", tier: 1);
            CreateFortificationContribution(entityManager, settlement, "keep_player", contribution: 1, position: new float3(TileSize, 0f, 0f));
            float hostileDistance = 8f * TileSize;
            CreateHostileUnit(entityManager, "enemy", new float3(hostileDistance, 0f, 0f));
            CreateHostileUnit(entityManager, "enemy", new float3(hostileDistance, 0f, TileSize));
            CreateHostileUnit(entityManager, "enemy", new float3(hostileDistance, 0f, -TileSize));

            Tick(world, seconds: 0.1d);

            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            if (!engagement.Active ||
                engagement.HostileCount != 3 ||
                engagement.WarningRadius <= hostileDistance ||
                engagement.TotalSeconds < ImminentEngagementCanon.MinSeconds ||
                engagement.TotalSeconds > ImminentEngagementCanon.MaxSeconds)
            {
                message =
                    "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL: phase 3 did not enter the warning window correctly. " +
                    "active=" + engagement.Active +
                    ", hostileCount=" + engagement.HostileCount +
                    ", warningRadius=" + engagement.WarningRadius.ToString("0.00") +
                    ", totalSeconds=" + engagement.TotalSeconds.ToString("0.00") + ".";
                return false;
            }

            message =
                "Phase3: active=True, hostileCount=3, warningRadius=" + engagement.WarningRadius.ToString("0.00") +
                ", totalSeconds=" + engagement.TotalSeconds.ToString("0.00") + ".";
            return true;
        }

        private static bool RunExpiryPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementSmokeValidation_Expiry");
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "enemy", tier: 1);
            CreateFortificationContribution(entityManager, settlement, "keep_player", contribution: 1, position: new float3(TileSize, 0f, 0f));
            CreateHostileUnit(entityManager, "player", new float3(6f * TileSize, 0f, 0f));
            CreateHostileUnit(entityManager, "player", new float3(6.5f * TileSize, 0f, TileSize));
            CreateReadyReserve(entityManager, settlement, "keep_player", "enemy", new float3(-TileSize, 0f, 0f));

            Tick(world, seconds: 0.1d);

            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            float elapsed = (float)world.Time.ElapsedTime;
            engagement.Active = true;
            engagement.WindowConsumed = false;
            engagement.SelectedPosture = ImminentEngagementPosture.Counterstroke;
            engagement.TotalSeconds = 14f;
            engagement.RemainingSeconds = 0.001f;
            engagement.ExpiresAt = elapsed - 0.001f;
            entityManager.SetComponentData(settlement, engagement);

            Tick(world, seconds: SimStepSeconds);

            engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            var reserveQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<FortificationReserveAssignmentComponent>());
            using var reserveAssignments = reserveQuery.ToComponentDataArray<FortificationReserveAssignmentComponent>(Allocator.Temp);

            bool sawSortieMuster = false;
            for (int i = 0; i < reserveAssignments.Length; i++)
            {
                if (reserveAssignments[i].Duty == ReserveDutyState.Muster)
                {
                    sawSortieMuster = true;
                    break;
                }
            }

            if (!engagement.WindowConsumed || engagement.Active || engagement.EngagedAt <= 0f)
            {
                message =
                    "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL: phase 4 did not consume the window. " +
                    "active=" + engagement.Active +
                    ", windowConsumed=" + engagement.WindowConsumed +
                    ", engagedAt=" + engagement.EngagedAt.ToString("0.000") + ".";
                return false;
            }

            message =
                "Phase4: windowConsumed=True, engagedAt=" + engagement.EngagedAt.ToString("0.000") +
                ", sortieCommitted=" + sawSortieMuster + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AdvanceFortificationTierSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationReserveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ImminentEngagementWarningSystem>());
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

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int tier)
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
                FortificationTier = tier,
                FortificationCeiling = 3,
            });
            entityManager.AddComponent<PrimaryKeepTag>(entity);
            entityManager.AddComponentData(entity, new FortificationComponent
            {
                SettlementId = settlementId,
                Tier = tier,
                Ceiling = 3,
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
                ReadyReserveCount = 1,
            });
            entityManager.AddComponentData(entity, new ImminentEngagementComponent
            {
                SelectedPosture = ImminentEngagementPosture.Steady,
                LastActivationAt = -999f,
            });
            return entity;
        }

        private static Entity CreateHostileUnit(
            EntityManager entityManager,
            string factionId,
            float3 position)
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
                Current = 12f,
                Max = 12f,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = "militia",
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.25f,
                IsActive = false,
            });
            return entity;
        }

        private static Entity CreateFortificationContribution(
            EntityManager entityManager,
            Entity settlement,
            string settlementId,
            int contribution,
            float3 position)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = 18f,
                Max = 18f,
            });
            entityManager.AddComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
                SettlementId = settlementId,
            });
            entityManager.AddComponentData(entity, new FortificationBuildingContributionComponent
            {
                TierContribution = contribution,
            });
            return entity;
        }

        private static Entity CreateReadyReserve(
            EntityManager entityManager,
            Entity settlement,
            string settlementId,
            string factionId,
            float3 position)
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
                Current = 10f,
                Max = 10f,
            });
            entityManager.AddComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
                SettlementId = settlementId,
            });
            entityManager.AddComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = ReserveDutyState.Ready,
            });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.25f,
                IsActive = false,
            });
            return entity;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
#endif
