#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed fortification smoke validator. Runs in isolated ECS worlds and proves:
    ///
    ///   1. A fresh fortified settlement stays at tier 0 without completed
    ///      fortification-contributing buildings.
    ///   2. A completed linked building advances the settlement to tier 1.
    ///   3. A low-health frontline defender retreats and a ready reserve unit
    ///      cycles forward into muster.
    ///   4. A recovering defender heals back to the recovery threshold and
    ///      returns to the ready reserve pool.
    ///
    /// Artifact: artifacts/unity-fortification-smoke.log.
    /// </summary>
    public static class BloodlinesFortificationSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-fortification-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Fortification/Run Fortification Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchFortificationSmokeValidation()
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
                message = "Fortification smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunTierZeroBaselinePhase(out string baselineMessage))
            {
                message = baselineMessage;
                return false;
            }

            if (!RunTierAdvancePhase(out string tierMessage))
            {
                message = tierMessage;
                return false;
            }

            if (!RunReserveMusterPhase(out string musterMessage))
            {
                message = musterMessage;
                return false;
            }

            if (!RunReserveRecoveryPhase(out string recoveryMessage))
            {
                message = recoveryMessage;
                return false;
            }

            message =
                "Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. " +
                baselineMessage + " " + tierMessage + " " + musterMessage + " " + recoveryMessage;
            return true;
        }

        private static bool RunTierZeroBaselinePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFortificationSmokeValidation_Baseline");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateSettlement(entityManager, "keep_player", "player", ceiling: 3);
            Tick(world, seconds: 0.05d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out int ceiling))
            {
                message = "Fortification smoke validation failed: baseline phase could not read fortification tier.";
                return false;
            }

            if (tier != 0 || ceiling != 3)
            {
                message =
                    "Fortification smoke validation failed: baseline tier drifted. " +
                    "tier=" + tier + ", ceiling=" + ceiling + ".";
                return false;
            }

            message = "Baseline: tier=0, ceiling=3.";
            return true;
        }

        private static bool RunTierAdvancePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFortificationSmokeValidation_Tier");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", ceiling: 3);
            CreateLinkedBuilding(entityManager, settlement, "keep_player", contribution: 1, new float3(1f, 0f, 0f));

            Tick(world, seconds: 0.05d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out int ceiling))
            {
                message = "Fortification smoke validation failed: tier phase could not read fortification tier.";
                return false;
            }

            if (tier != 1 || ceiling != 3)
            {
                message =
                    "Fortification smoke validation failed: tier phase expected tier 1 of 3. " +
                    "tier=" + tier + ", ceiling=" + ceiling + ".";
                return false;
            }

            message = "TierAdvance: tier=1, ceiling=3, contributionApplied=1.";
            return true;
        }

        private static bool RunReserveMusterPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFortificationSmokeValidation_Muster");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", ceiling: 3);
            CreateLinkedBuilding(entityManager, settlement, "keep_player", contribution: 1, new float3(1f, 0f, 0f));

            var retreatingFrontline = CreateLinkedCombatant(
                entityManager,
                settlement,
                "keep_player",
                "player",
                new float3(1.2f, 0f, 0f),
                currentHealth: 3.5f,
                maxHealth: 10f,
                ReserveDutyState.Engaged);

            var reserveUnit = CreateLinkedCombatant(
                entityManager,
                settlement,
                "keep_player",
                "player",
                new float3(-1f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);

            CreateHostileCombatant(entityManager, "enemy", new float3(6f, 0f, 0f), currentHealth: 10f, maxHealth: 10f);

            Tick(world, seconds: 0.05d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetReserveCount("keep_player", out int readyReserveCount))
            {
                message = "Fortification smoke validation failed: muster phase could not read reserve count.";
                return false;
            }

            var retreatState = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(retreatingFrontline);
            var reserveState = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(reserveUnit);
            if (retreatState.Duty != ReserveDutyState.Fallback ||
                reserveState.Duty != ReserveDutyState.Muster ||
                readyReserveCount != 0)
            {
                message =
                    "Fortification smoke validation failed: muster phase did not cycle reserves correctly. " +
                    "retreatDuty=" + retreatState.Duty +
                    ", reserveDuty=" + reserveState.Duty +
                    ", readyReserveCount=" + readyReserveCount + ".";
                return false;
            }

            var reserveProfile = entityManager.GetComponentData<FortificationReserveComponent>(settlement);
            message =
                "ReserveMuster: retreatDuty=" + retreatState.Duty +
                ", reserveDuty=" + reserveState.Duty +
                ", committed=" + reserveProfile.LastCommittedCount + ".";
            return true;
        }

        private static bool RunReserveRecoveryPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesFortificationSmokeValidation_Recovery");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = CreateSettlement(entityManager, "keep_player", "player", ceiling: 3);
            CreateLinkedBuilding(entityManager, settlement, "keep_player", contribution: 1, new float3(1f, 0f, 0f));

            var recoveringDefender = CreateLinkedCombatant(
                entityManager,
                settlement,
                "keep_player",
                "player",
                new float3(1f, 0f, 0f),
                currentHealth: 4f,
                maxHealth: 10f,
                ReserveDutyState.Fallback);

            Tick(world, seconds: 1.0d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetReserveCount("keep_player", out int readyReserveCount))
            {
                message = "Fortification smoke validation failed: recovery phase could not read reserve count.";
                return false;
            }

            var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(recoveringDefender);
            var health = entityManager.GetComponentData<HealthComponent>(recoveringDefender);
            float healthRatio = health.Current / health.Max;
            if (assignment.Duty != ReserveDutyState.Ready ||
                healthRatio < FortificationCanon.ReserveRecoveryHealthRatio ||
                readyReserveCount != 1)
            {
                message =
                    "Fortification smoke validation failed: recovery phase did not return the defender to ready state. " +
                    "duty=" + assignment.Duty +
                    ", ratio=" + healthRatio.ToString("0.00") +
                    ", readyReserveCount=" + readyReserveCount + ".";
                return false;
            }

            message =
                "ReserveRecovery: duty=" + assignment.Duty +
                ", healthRatio=" + healthRatio.ToString("0.00") +
                ", readyReserveCount=" + readyReserveCount + ".";
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

        private static Entity CreateLinkedBuilding(
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
                Current = 12f,
                Max = 12f,
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

        private static Entity CreateLinkedCombatant(
            EntityManager entityManager,
            Entity settlement,
            string settlementId,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth,
            ReserveDutyState duty)
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
            entityManager.AddComponentData(entity, new FortificationCombatantTag());
            entityManager.AddComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
                SettlementId = settlementId,
            });
            entityManager.AddComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = duty,
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
            entityManager.AddComponentData(entity, new FortificationCombatantTag());
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

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesFortificationSmokeValidation_CommandSurface")
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
