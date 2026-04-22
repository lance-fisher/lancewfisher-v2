#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
using Bloodlines.HUD;
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
    /// Dedicated smoke validator for the fortification HUD follow-up slice.
    /// Proves reserve frontage plus sealing and recovery telemetry through the public
    /// FortHUD debug snapshot.
    /// </summary>
    public static class BloodlinesFortificationHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-fortification-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Fortification HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFortificationHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Fortification HUD smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_FORTIFICATION_HUD_SMOKE " +
                           (success ? "PASS" : "FAIL") +
                           Environment.NewLine + message;
            UnityDebug.Log(artifact);
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
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
            var lines = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunReservePhase(lines);
            ok &= RunSealingPhase(lines);
            ok &= RunRecoveryPhase(lines);
            ok &= RunKeepRecoveryPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunReservePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlementEntity = SeedSettlement(
                entityManager,
                settlementId: "ironmark-keep",
                factionId: "player",
                settlementClassId: "primary_dynastic_keep",
                tier: 2,
                ceiling: 4,
                openBreaches: 0,
                readyReserveCount: 3,
                musteringReserveCount: 1,
                recoveringReserveCount: 1,
                fallbackReserveCount: 0,
                lastCommittedCount: 2,
                threatActive: true,
                isPrimaryKeep: true);

            SeedCombatant(entityManager, settlementEntity, "player", new float3(2f, 0f, 0f));
            SeedCombatant(entityManager, settlementEntity, "player", new float3(4f, 0f, 0f));
            SeedCombatant(entityManager, settlementEntity, "enemy", new float3(3f, 0f, 0f));
            SeedCombatant(entityManager, settlementEntity, "player", new float3(30f, 0f, 0f));
            SeedCombatant(entityManager, settlementEntity, "player", new float3(1f, 0f, 0f), currentHealth: 0f);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "ironmark-keep", out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read fortification HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "Tier") != "2" ||
                ReadField(fields, "ReserveFrontage") != "2" ||
                ReadField(fields, "MusteredDefenders") != "2" ||
                ReadField(fields, "ReadyReserveCount") != "3" ||
                ReadField(fields, "ThreatActive") != "true" ||
                ReadField(fields, "IsPrimaryKeep") != "true")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected reserve snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: primary keep surfaces tier 2, reserve frontage 2, mustered defenders 2, and active reserve pressure.");
            return true;
        }

        private static bool RunSealingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlementEntity = SeedSettlement(
                entityManager,
                settlementId: "stonegate",
                factionId: "player",
                settlementClassId: "regional_stronghold",
                tier: 3,
                ceiling: 4,
                openBreaches: 1);

            entityManager.AddComponentData(settlementEntity, new BreachSealingProgressComponent
            {
                AccumulatedWorkerHours = 9f,
                StoneReservedForCurrentBreach = 45f,
                LastTickInWorldDays = 120f,
            });

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "stonegate", out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read sealing fortification HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "OpenBreachCount") != "1" ||
                ReadField(fields, "SealingTracked") != "true")
            {
                lines.AppendLine($"Phase 2 FAIL: expected active sealing snapshot, got '{readout}'.");
                return false;
            }

            float sealingProgress = ParseFloat(fields, "SealingProgress01");
            float requiredWorkerHours = ParseFloat(fields, "SealingRequiredWorkerHours");
            float requiredStone = ParseFloat(fields, "SealingRequiredStone");
            if (Mathf.Abs(sealingProgress - 0.5f) > 0.001f ||
                Mathf.Abs(requiredWorkerHours - 18f) > 0.001f ||
                Mathf.Abs(requiredStone - 135f) > 0.001f)
            {
                lines.AppendLine($"Phase 2 FAIL: expected tier-3 sealing progress 0.500 / 18h / 135 stone, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: open breach projects sealing progress 0.500 with tier-3 cost 18 worker-hours and 135 stone.");
            return true;
        }

        private static bool RunRecoveryPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlementEntity = SeedSettlement(
                entityManager,
                settlementId: "watchfall",
                factionId: "player",
                settlementClassId: "border_settlement",
                tier: 1,
                ceiling: 2,
                openBreaches: 0,
                destroyedWalls: 1);

            entityManager.AddComponentData(settlementEntity, new DestroyedCounterRecoveryProgressComponent
            {
                AccumulatedWorkerHours = 7f,
                StoneReservedForCurrentSegment = 45f,
                LastTickInWorldDays = 200f,
                TargetCounter = DestroyedCounterKind.Wall,
            });

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "watchfall", out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read recovery fortification HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "RecoveryTracked") != "true" ||
                ReadField(fields, "RecoveryTargetCounter") != nameof(DestroyedCounterKind.Wall))
            {
                lines.AppendLine($"Phase 3 FAIL: expected wall recovery snapshot, got '{readout}'.");
                return false;
            }

            float recoveryProgress = ParseFloat(fields, "RecoveryProgress01");
            float requiredWorkerHours = ParseFloat(fields, "RecoveryRequiredWorkerHours");
            float requiredStone = ParseFloat(fields, "RecoveryRequiredStone");
            if (Mathf.Abs(recoveryProgress - 0.5f) > 0.001f ||
                Mathf.Abs(requiredWorkerHours - 14f) > 0.001f ||
                Mathf.Abs(requiredStone - 90f) > 0.001f)
            {
                lines.AppendLine($"Phase 3 FAIL: expected wall recovery progress 0.500 / 14h / 90 stone, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: destroyed wall recovery projects progress 0.500 with canonical 14 worker-hours and 90 stone.");
            return true;
        }

        private static bool RunKeepRecoveryPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlementEntity = SeedSettlement(
                entityManager,
                settlementId: "highkeep",
                factionId: "player",
                settlementClassId: "primary_dynastic_keep",
                tier: 4,
                ceiling: 5,
                openBreaches: 0,
                destroyedKeeps: 1,
                isPrimaryKeep: true);

            entityManager.AddComponentData(settlementEntity, new DestroyedCounterRecoveryProgressComponent
            {
                AccumulatedWorkerHours = 14f,
                StoneReservedForCurrentSegment = 90f,
                LastTickInWorldDays = 260f,
                TargetCounter = DestroyedCounterKind.Keep,
            });

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "highkeep", out var fields, out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read keep recovery fortification HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "RecoveryTargetCounter") != nameof(DestroyedCounterKind.Keep) ||
                ReadField(fields, "IsPrimaryKeep") != "true")
            {
                lines.AppendLine($"Phase 4 FAIL: expected keep recovery snapshot, got '{readout}'.");
                return false;
            }

            float recoveryProgress = ParseFloat(fields, "RecoveryProgress01");
            float requiredWorkerHours = ParseFloat(fields, "RecoveryRequiredWorkerHours");
            float requiredStone = ParseFloat(fields, "RecoveryRequiredStone");
            if (Mathf.Abs(recoveryProgress - 0.5f) > 0.001f ||
                Mathf.Abs(requiredWorkerHours - 28f) > 0.001f ||
                Mathf.Abs(requiredStone - 180f) > 0.001f)
            {
                lines.AppendLine($"Phase 4 FAIL: expected keep recovery progress 0.500 / 28h / 180 stone, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: primary keep recovery projects keep-multiplied progress 0.500 with canonical 28 worker-hours and 180 stone.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationHUDSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity SeedSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            string settlementClassId,
            int tier,
            int ceiling,
            int openBreaches,
            int destroyedWalls = 0,
            int destroyedTowers = 0,
            int destroyedGates = 0,
            int destroyedKeeps = 0,
            int readyReserveCount = 0,
            int musteringReserveCount = 0,
            int recoveringReserveCount = 0,
            int fallbackReserveCount = 0,
            int lastCommittedCount = 0,
            bool threatActive = false,
            bool isPrimaryKeep = false)
        {
            var entity = entityManager.CreateEntity(
                typeof(SettlementComponent),
                typeof(FactionComponent),
                typeof(FortificationComponent),
                typeof(FortificationReserveComponent),
                typeof(PositionComponent));

            entityManager.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                SettlementClassId = new FixedString32Bytes(settlementClassId),
                FortificationTier = tier,
                FortificationCeiling = ceiling,
            });
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FortificationComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                Tier = tier,
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
                FaithWardId = new FixedString32Bytes("old_light"),
                FaithWardSightBonusTiles = 4f,
                FaithWardDefenderAttackMultiplier = 1.1f,
                FaithWardReserveHealMultiplier = 1.18f,
                FaithWardReserveMusterMultiplier = 1.16f,
                FaithWardLoyaltyProtectionMultiplier = 1.12f,
                FaithWardEnemySpeedMultiplier = 0.92f,
                FaithWardSurgeActive = false,
            });
            entityManager.SetComponentData(entity, new FortificationReserveComponent
            {
                MusterIntervalSeconds = FortificationCanon.ReserveMusterIntervalSeconds,
                ReserveHealPerSecond = FortificationCanon.ReserveTriageHealPerSecond,
                RetreatHealthRatio = FortificationCanon.ReserveRetreatHealthRatio,
                RecoveryHealthRatio = FortificationCanon.ReserveRecoveryHealthRatio,
                TriageRadiusTiles = FortificationCanon.TriageRadiusTiles,
                LastCommitAt = 0d,
                ThreatActive = threatActive,
                ReadyReserveCount = readyReserveCount,
                MusteringReserveCount = musteringReserveCount,
                RecoveringReserveCount = recoveringReserveCount,
                FallbackReserveCount = fallbackReserveCount,
                LastCommittedCount = lastCommittedCount,
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = float3.zero,
            });

            if (isPrimaryKeep)
            {
                entityManager.AddComponent<PrimaryKeepTag>(entity);
            }

            return entity;
        }

        private static void SeedCombatant(
            EntityManager entityManager,
            Entity settlementEntity,
            string factionId,
            float3 position,
            float currentHealth = 100f)
        {
            var settlement = entityManager.GetComponentData<SettlementComponent>(settlementEntity);
            var entity = entityManager.CreateEntity(
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(FactionComponent));

            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
                SettlementId = settlement.SettlementId,
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = 100f,
            });
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
        }

        private static bool TryGetSnapshotFields(
            BloodlinesDebugCommandSurface commandSurface,
            string settlementId,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetFortificationHUDSnapshot(settlementId, out readout))
            {
                return false;
            }

            var parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "FortHUD", StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = 1; i < parts.Length; i++)
            {
                int equalsIndex = parts[i].IndexOf('=');
                if (equalsIndex <= 0)
                {
                    continue;
                }

                fields[parts[i].Substring(0, equalsIndex)] = parts[i].Substring(equalsIndex + 1);
            }

            return true;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException("Missing field: " + key);
            }

            return value;
        }

        private static float ParseFloat(Dictionary<string, string> fields, string key)
        {
            var value = ReadField(fields, key);
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                throw new InvalidOperationException("Invalid float field: " + key);
            }

            return parsed;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;

                hostObject = new GameObject("BloodlinesFortificationHUDSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
