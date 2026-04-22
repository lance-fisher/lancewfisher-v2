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
    /// Dedicated smoke validator for the fortification HUD read-model slice.
    /// Proves baseline, threat+muster, sealing-progress, and recovery-progress
    /// snapshots through the public HUD debug surface.
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
            ok &= RunBaselinePhase(lines);
            ok &= RunThreatMusterPhase(lines);
            ok &= RunSealingProgressPhase(lines);
            ok &= RunRecoveryProgressPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = SeedSettlement(entityManager, "keep_player", "player", tier: 1, openBreaches: 0);
            SeedReserve(entityManager, settlement, threatActive: false, readyCount: 2, recoveringCount: 0);
            SeedCombatant(entityManager, settlement, "player", ReserveDutyState.Ready, new float3(-1f, 0f, 0f));
            SeedCombatant(entityManager, settlement, "player", ReserveDutyState.Ready, new float3(-2f, 0f, 0f));

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "keep_player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read fortification HUD baseline.");
                return false;
            }

            if (ReadField(fields, "Tier") != "1" ||
                ReadField(fields, "OpenBreachCount") != "0" ||
                ReadField(fields, "ReserveFrontage") != "2" ||
                ReadField(fields, "MusteredDefenders") != "0" ||
                ReadField(fields, "ThreatActive") != "false")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected baseline snapshot '{readout}'.");
                return false;
            }

            if (Mathf.Abs(ParseFloat(fields, "SealingProgress01")) > 0.001f ||
                Mathf.Abs(ParseFloat(fields, "RecoveryProgress01")) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: intact settlement should report zero repair progress, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: intact keep surfaces Tier=1, ReserveFrontage=2, MusteredDefenders=0, and zero repair progress.");
            return true;
        }

        private static bool RunThreatMusterPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = SeedSettlement(entityManager, "keep_player", "player", tier: 1, openBreaches: 0);
            SeedReserve(entityManager, settlement, threatActive: true, readyCount: 1, recoveringCount: 0);
            SeedCombatant(entityManager, settlement, "player", ReserveDutyState.Engaged, new float3(-1f, 0f, 0f));
            SeedCombatant(entityManager, settlement, "player", ReserveDutyState.Muster, new float3(-2f, 0f, 0f));
            SeedCombatant(entityManager, settlement, "player", ReserveDutyState.Ready, new float3(-3f, 0f, 0f));

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "keep_player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read fortification HUD threat snapshot.");
                return false;
            }

            if (ReadField(fields, "ReserveFrontage") != "2" ||
                ReadField(fields, "MusteredDefenders") != "2" ||
                ReadField(fields, "ReadyReserves") != "1" ||
                ReadField(fields, "ThreatActive") != "true")
            {
                lines.AppendLine($"Phase 2 FAIL: expected active muster telemetry, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: threat-active keep surfaces ReserveFrontage=2, MusteredDefenders=2, ReadyReserves=1.");
            return true;
        }

        private static bool RunSealingProgressPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = SeedSettlement(entityManager, "keep_player", "player", tier: 2, openBreaches: 1);
            SeedReserve(entityManager, settlement, threatActive: true, readyCount: 0, recoveringCount: 1);
            entityManager.AddComponentData(settlement, new BreachSealingProgressComponent
            {
                AccumulatedWorkerHours = 6f,
                StoneReservedForCurrentBreach = FortificationCanon.BreachSealingTier2StoneCostPerBreach,
                LastTickInWorldDays = 12f,
            });

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "keep_player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read fortification HUD sealing snapshot.");
                return false;
            }

            float sealing = ParseFloat(fields, "SealingProgress01");
            if (ReadField(fields, "Tier") != "2" ||
                ReadField(fields, "OpenBreachCount") != "1" ||
                ReadField(fields, "RecoveringReserves") != "1" ||
                Mathf.Abs(sealing - 0.5f) > 0.001f ||
                Mathf.Abs(ParseFloat(fields, "RecoveryProgress01")) > 0.001f)
            {
                lines.AppendLine($"Phase 3 FAIL: expected tier-scaled sealing progress, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: breached Tier-2 keep surfaces SealingProgress01=0.500 and zero recovery progress.");
            return true;
        }

        private static bool RunRecoveryProgressPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("fortification-hud-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var settlement = SeedSettlement(
                entityManager,
                "keep_player",
                "player",
                tier: 1,
                openBreaches: 0,
                destroyedWalls: 1);
            SeedReserve(entityManager, settlement, threatActive: false, readyCount: 1, recoveringCount: 1);
            entityManager.AddComponentData(settlement, new DestroyedCounterRecoveryProgressComponent
            {
                TargetCounter = DestroyedCounterKind.Wall,
                AccumulatedWorkerHours = FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment * 0.5f,
                StoneReservedForCurrentSegment = FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment,
                LastTickInWorldDays = 18f,
            });

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "keep_player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read fortification HUD recovery snapshot.");
                return false;
            }

            float recovery = ParseFloat(fields, "RecoveryProgress01");
            if (ReadField(fields, "OpenBreachCount") != "0" ||
                Mathf.Abs(ParseFloat(fields, "SealingProgress01")) > 0.001f ||
                Mathf.Abs(recovery - 0.5f) > 0.001f)
            {
                lines.AppendLine($"Phase 4 FAIL: expected post-breach rebuild progress, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: sealed keep with one destroyed wall surfaces RecoveryProgress01=0.500 and zero sealing progress.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationHUDSystem>());
            simulationGroup.SortSystems();
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
            int tier,
            int openBreaches,
            int destroyedWalls = 0)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FortificationComponent),
                typeof(SettlementComponent),
                typeof(PositionComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = float3.zero,
            });
            entityManager.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                SettlementClassId = new FixedString32Bytes("primary_dynastic_keep"),
                FortificationTier = tier,
                FortificationCeiling = 4,
            });
            entityManager.SetComponentData(entity, new FortificationComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                Tier = tier,
                Ceiling = 4,
                OpenBreachCount = openBreaches,
                DestroyedWallSegmentCount = destroyedWalls,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
            });
            return entity;
        }

        private static void SeedReserve(
            EntityManager entityManager,
            Entity settlement,
            bool threatActive,
            int readyCount,
            int recoveringCount)
        {
            entityManager.AddComponentData(settlement, new FortificationReserveComponent
            {
                ThreatActive = threatActive,
                ReadyReserveCount = readyCount,
                RecoveringReserveCount = recoveringCount,
            });
        }

        private static void SeedCombatant(
            EntityManager entityManager,
            Entity settlement,
            string factionId,
            ReserveDutyState duty,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent),
                typeof(PositionComponent),
                typeof(HealthComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
            });
            entityManager.SetComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = duty,
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 100f,
                Max = 100f,
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
            if (parts.Length == 0 || !string.Equals(parts[0], "FortificationHUD", StringComparison.Ordinal))
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

                var key = parts[i].Substring(0, equalsIndex);
                var value = parts[i].Substring(equalsIndex + 1);
                fields[key] = value;
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
