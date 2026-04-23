#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
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
    public static class BloodlinesImminentEngagementPostureSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-imminent-engagement-posture-smoke.log";

        [MenuItem("Bloodlines/Fortification/Run Imminent Engagement Posture Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchImminentEngagementPostureSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

        private static void RunInternal(bool batchMode)
        {
            string details;
            bool success;
            try
            {
                success = RunAllPhases(out details);
            }
            catch (Exception e)
            {
                success = false;
                details = "Imminent engagement posture smoke errored: " + e;
            }

            WriteResult(batchMode, success, details);
        }

        private static bool RunAllPhases(out string details)
        {
            if (!RunBraceHealingPhase(out string braceDetails))
            {
                details = braceDetails;
                return false;
            }

            if (!RunCounterstrokeFrontlinePhase(out string counterstrokeDetails))
            {
                details = counterstrokeDetails;
                return false;
            }

            if (!RunPostureCleanupPhase(out string cleanupDetails))
            {
                details = cleanupDetails;
                return false;
            }

            details = braceDetails + "; " + counterstrokeDetails + "; " + cleanupDetails;
            return true;
        }

        private static bool RunBraceHealingPhase(out string details)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementPosture_BraceHealing");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", hostileTo: "enemy");
            CreateFaction(entityManager, "enemy", hostileTo: "player");

            Entity braceSettlement = CreateSettlement(
                entityManager,
                "brace_keep",
                "player",
                new float3(0f, 0f, 0f),
                engagementActive: true);
            Entity steadySettlement = CreateSettlement(
                entityManager,
                "steady_keep",
                "player",
                new float3(30f, 0f, 0f),
                engagementActive: true);

            Entity braceDefender = CreateReserveDefender(
                entityManager,
                braceSettlement,
                "brace_keep",
                "player",
                new float3(0.8f, 0f, 0f),
                currentHealth: 1f,
                maxHealth: 10f,
                ReserveDutyState.Fallback);
            Entity steadyDefender = CreateReserveDefender(
                entityManager,
                steadySettlement,
                "steady_keep",
                "player",
                new float3(30.8f, 0f, 0f),
                currentHealth: 1f,
                maxHealth: 10f,
                ReserveDutyState.Fallback);

            CreateHostileCombatant(entityManager, "enemy", new float3(6f, 0f, 0f), 10f, 10f);
            CreateHostileCombatant(entityManager, "enemy", new float3(36f, 0f, 0f), 10f, 10f);

            if (!debugScope.CommandSurface.TryDebugSetImminentEngagementPosture(
                    "brace_keep",
                    (int)ImminentEngagementPostureId.Brace) ||
                !debugScope.CommandSurface.TryDebugSetImminentEngagementPosture(
                    "steady_keep",
                    (int)ImminentEngagementPostureId.Steady))
            {
                details = "Brace healing phase failed: debug posture setter rejected the settlements.";
                return false;
            }

            Tick(world, 0.5f);

            float braceHealth = entityManager.GetComponentData<HealthComponent>(braceDefender).Current;
            float steadyHealth = entityManager.GetComponentData<HealthComponent>(steadyDefender).Current;
            var braceAssignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(braceDefender);
            var steadyAssignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(steadyDefender);

            if (braceHealth <= steadyHealth + 0.2f ||
                braceAssignment.Duty != ReserveDutyState.Recovering ||
                steadyAssignment.Duty != ReserveDutyState.Recovering)
            {
                details =
                    "Brace healing phase failed: posture heal delta was not applied. " +
                    "braceHealth=" + braceHealth.ToString("0.00") +
                    ", steadyHealth=" + steadyHealth.ToString("0.00") +
                    ", braceDuty=" + braceAssignment.Duty +
                    ", steadyDuty=" + steadyAssignment.Duty + ".";
                return false;
            }

            details =
                "braceHealingPhase: braceHealth=" + braceHealth.ToString("0.00") +
                ", steadyHealth=" + steadyHealth.ToString("0.00") + ".";
            return true;
        }

        private static bool RunCounterstrokeFrontlinePhase(out string details)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementPosture_Counterstroke");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", hostileTo: "enemy");
            CreateFaction(entityManager, "enemy", hostileTo: "player");

            Entity counterstrokeSettlement = CreateSettlement(
                entityManager,
                "counter_keep",
                "player",
                new float3(0f, 0f, 0f),
                engagementActive: true);
            Entity steadySettlement = CreateSettlement(
                entityManager,
                "steady_keep",
                "player",
                new float3(30f, 0f, 0f),
                engagementActive: true);

            Entity counterstrokeTarget = CreateHostileCombatant(
                entityManager,
                "enemy",
                new float3(5f, 0f, 0f),
                40f,
                40f);
            Entity steadyTarget = CreateHostileCombatant(
                entityManager,
                "enemy",
                new float3(35f, 0f, 0f),
                40f,
                40f);

            CreateFrontlineDefender(
                entityManager,
                counterstrokeSettlement,
                "counter_keep",
                "player",
                new float3(1f, 0f, 0f),
                attackTarget: counterstrokeTarget);
            CreateFrontlineDefender(
                entityManager,
                steadySettlement,
                "steady_keep",
                "player",
                new float3(31f, 0f, 0f),
                attackTarget: steadyTarget);

            CreatePostureRequest(entityManager, "counter_keep", ImminentEngagementPostureId.Counterstroke);
            CreatePostureRequest(entityManager, "steady_keep", ImminentEngagementPostureId.Steady);

            Tick(world, 0.1f);

            float counterstrokeRemainingHealth = entityManager.GetComponentData<HealthComponent>(counterstrokeTarget).Current;
            float steadyRemainingHealth = entityManager.GetComponentData<HealthComponent>(steadyTarget).Current;
            float counterstrokeDamage = 40f - counterstrokeRemainingHealth;
            float steadyDamage = 40f - steadyRemainingHealth;

            var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerImminentEngagementPostureRequestComponent>());
            int remainingRequests = requestQuery.CalculateEntityCount();

            if (counterstrokeDamage <= steadyDamage + 0.5f ||
                remainingRequests != 0)
            {
                details =
                    "Counterstroke phase failed: frontline bonus or request cleanup was missing. " +
                    "counterstrokeDamage=" + counterstrokeDamage.ToString("0.00") +
                    ", steadyDamage=" + steadyDamage.ToString("0.00") +
                    ", remainingRequests=" + remainingRequests + ".";
                return false;
            }

            details =
                "counterstrokePhase: counterstrokeDamage=" + counterstrokeDamage.ToString("0.00") +
                ", steadyDamage=" + steadyDamage.ToString("0.00") + ".";
            return true;
        }

        private static bool RunPostureCleanupPhase(out string details)
        {
            using var world = CreateValidationWorld("BloodlinesImminentEngagementPosture_Cleanup");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", hostileTo: "enemy");
            CreateFaction(entityManager, "enemy", hostileTo: "player");

            Entity settlement = CreateSettlement(
                entityManager,
                "cleanup_keep",
                "player",
                new float3(0f, 0f, 0f),
                engagementActive: true);
            Entity hostile = CreateHostileCombatant(
                entityManager,
                "enemy",
                new float3(6f, 0f, 0f),
                10f,
                10f);

            if (!debugScope.CommandSurface.TryDebugSetImminentEngagementPosture(
                    "cleanup_keep",
                    (int)ImminentEngagementPostureId.Brace))
            {
                details = "Cleanup phase failed: debug posture setter rejected the active settlement.";
                return false;
            }

            if (!entityManager.HasComponent<ImminentEngagementPostureComponent>(settlement))
            {
                details = "Cleanup phase failed: expected settlement to hold a live posture component before threat loss.";
                return false;
            }

            entityManager.SetComponentData(hostile, new HealthComponent
            {
                Current = 0f,
                Max = 10f,
            });

            Tick(world, 0.1f);

            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlement);
            if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlement) ||
                engagement.Active)
            {
                details =
                    "Cleanup phase failed: posture state persisted after the warning resolved. " +
                    "active=" + engagement.Active +
                    ", windowConsumed=" + engagement.WindowConsumed + ".";
                return false;
            }

            details =
                "cleanupPhase: postureRemoved=True, selectedResponse=" +
                engagement.SelectedResponseId.ToString() + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            simulationGroup.AddSystemToUpdateList(endSimulation);
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationReserveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ImminentEngagementWarningSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ImminentEngagementPostureSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CombatStanceResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackResolutionSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void Tick(World world, float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            double nextElapsed = world.Time.ElapsedTime + deltaTime;
            world.SetTime(new TimeData(nextElapsed, deltaTime));
            world.Update();
        }

        private static Entity CreateFaction(
            EntityManager entityManager,
            string factionId,
            string hostileTo = null)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionLoyaltyComponent),
                typeof(HostilityComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionLoyaltyComponent
            {
                Current = 70f,
                Max = 100f,
                Floor = 0f,
            });

            if (!string.IsNullOrWhiteSpace(hostileTo))
            {
                var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(entity);
                hostilityBuffer.Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes(hostileTo),
                });
            }

            return entity;
        }

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            float3 position,
            bool engagementActive)
        {
            var entity = entityManager.CreateEntity(
                typeof(SettlementComponent),
                typeof(FactionComponent),
                typeof(FortificationComponent),
                typeof(FortificationReserveComponent),
                typeof(ImminentEngagementComponent),
                typeof(PositionComponent));

            entityManager.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                SettlementClassId = new FixedString32Bytes("trade_town"),
                FortificationTier = 1,
                FortificationCeiling = 3,
            });
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new FortificationComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                Tier = 1,
                Ceiling = 3,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
                FaithWardId = new FixedString32Bytes("unwarded"),
                FaithWardDefenderAttackMultiplier = 1f,
                FaithWardReserveHealMultiplier = 1f,
                FaithWardReserveMusterMultiplier = 1f,
                FaithWardLoyaltyProtectionMultiplier = 1f,
                FaithWardEnemySpeedMultiplier = 1f,
                FaithWardSurgeActive = false,
            });
            entityManager.SetComponentData(entity, new FortificationReserveComponent
            {
                MusterIntervalSeconds = FortificationCanon.ReserveMusterIntervalSeconds,
                ReserveHealPerSecond = FortificationCanon.ReserveTriageHealPerSecond,
                RetreatHealthRatio = FortificationCanon.ReserveRetreatHealthRatio,
                RecoveryHealthRatio = FortificationCanon.ReserveRecoveryHealthRatio,
                TriageRadiusTiles = FortificationCanon.TriageRadiusTiles,
                LastCommitAt = -999d,
                ThreatActive = engagementActive,
                ReadyReserveCount = 1,
            });
            entityManager.SetComponentData(entity, new ImminentEngagementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                Active = engagementActive,
                WindowConsumed = false,
                HostileCount = engagementActive ? 1 : 0,
                WarningRadius = FortificationCanon.ThreatRadiusTiles +
                    FortificationCanon.ImminentEngagementWarningBufferTiles,
                TotalSeconds = 12f,
                ExpiresAt = 30f,
                RemainingSeconds = 12f,
                LocalLoyalty = 70f,
                LocalLoyaltyMin = 70f,
                SelectedResponseId = ImminentEngagementPostureUtility.SteadyResponseId,
                SelectedResponseLabel = ImminentEngagementPostureUtility.SteadyLabel,
                StartedAt = 0f,
                LastActivationAt = 0f,
                IsPrimaryDynasticKeep = false,
            });

            return entity;
        }

        private static Entity CreateReserveDefender(
            EntityManager entityManager,
            Entity settlementEntity,
            string settlementId,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth,
            ReserveDutyState duty)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = maxHealth,
            });
            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
                SettlementId = new FixedString64Bytes(settlementId),
            });
            entityManager.SetComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = duty,
            });

            return entity;
        }

        private static Entity CreateFrontlineDefender(
            EntityManager entityManager,
            Entity settlementEntity,
            string settlementId,
            string factionId,
            float3 position,
            Entity attackTarget)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(CombatStatsComponent),
                typeof(MoveCommandComponent),
                typeof(AttackTargetComponent),
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 20f,
                Max = 20f,
            });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes("militia"),
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });
            entityManager.SetComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = 10f,
                AttackRange = 8f,
                AttackCooldown = 0.5f,
                Sight = 20f,
                CooldownRemaining = 0f,
                TargetAcquireIntervalSeconds = 0.25f,
                TargetSightGraceSeconds = 0.35f,
            });
            entityManager.SetComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.5f,
                IsActive = false,
            });
            entityManager.SetComponentData(entity, new AttackTargetComponent
            {
                TargetEntity = attackTarget,
                EngagementRange = 8f,
            });
            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
                SettlementId = new FixedString64Bytes(settlementId),
            });
            entityManager.SetComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = ReserveDutyState.Engaged,
            });

            return entity;
        }

        private static Entity CreateHostileCombatant(
            EntityManager entityManager,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(FortificationCombatantTag));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = maxHealth,
            });

            return entity;
        }

        private static void CreatePostureRequest(
            EntityManager entityManager,
            string settlementId,
            ImminentEngagementPostureId postureId)
        {
            var entity = entityManager.CreateEntity(typeof(PlayerImminentEngagementPostureRequestComponent));
            entityManager.SetComponentData(entity, new PlayerImminentEngagementPostureRequestComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                PostureId = (byte)postureId,
            });
        }

        private static void WriteResult(bool batchMode, bool success, string details)
        {
            string summary =
                "BLOODLINES_IMMINENT_ENGAGEMENT_POSTURE_SMOKE " +
                (success ? "PASS " : "FAIL ") +
                details;

            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, summary);
            }
            catch
            {
            }

            if (success)
            {
                UnityDebug.Log(summary);
            }
            else
            {
                UnityDebug.LogError(summary);
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesImminentEngagementPostureSmoke_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
