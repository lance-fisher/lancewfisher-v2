#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Siege;
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
    /// Dedicated smoke validator for the settlement breach legibility readout seam.
    /// It proves the debug surface packages fortification breach counts,
    /// tier/frontage state, and live breach-assault telemetry without a HUD consumer.
    /// </summary>
    public static class BloodlinesBreachLegibilityReadoutSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-breach-legibility-readout-smoke.log";
        private const float SimStepSeconds = 0.05f;
        private const float HoursPerSecond = 1f;
        private const float DaysPerSecond = HoursPerSecond / 24f;
        private const float HalfSealingWindowSeconds = 4f;
        private const float HalfKeepRebuildWindowSeconds = 14f;

        [MenuItem("Bloodlines/Fortification/Run Breach Legibility Readout Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchBreachLegibilityReadoutSmokeValidation()
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
                message = "Breach legibility readout smoke errored: " + e;
            }

            string artifact = "BLOODLINES_BREACH_LEGIBILITY_READOUT_SMOKE " +
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
            ok &= RunIntactSettlementPhase(sb);
            ok &= RunSingleBreachPhase(sb);
            ok &= RunMultiBreachScalingPhase(sb);
            ok &= RunMissingSettlementPhase(sb);
            ok &= RunPartialDestructionVarietyPhase(sb);
            ok &= RunSealingTelemetryPhase(sb);
            ok &= RunRecoveryTelemetryPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunIntactSettlementPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachLegibilityReadout_Intact");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 4);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-1f, 0f, 0f));
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-2f, 0f, 0f));

            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachReadout(
                    new FixedString32Bytes("keep_player"),
                    out var readout))
            {
                sb.AppendLine("Phase 1 FAIL: intact settlement readout could not be resolved.");
                return false;
            }

            if (!readout.SettlementId.Equals(new FixedString64Bytes("keep_player")) ||
                !readout.OwnerFactionId.Equals(new FixedString32Bytes("player")) ||
                readout.OpenBreachCount != 0 ||
                readout.DestroyedWallSegmentCount != 0 ||
                readout.DestroyedTowerCount != 0 ||
                readout.DestroyedGateCount != 0 ||
                readout.DestroyedKeepCount != 0 ||
                readout.CurrentTier != 1 ||
                readout.ReserveFrontage != 2 ||
                readout.BreachAssaultAdvantageActive ||
                !Approximately(readout.AggregateAttackMultiplier, 1f, 0.001f) ||
                !Approximately(readout.AggregateSpeedMultiplier, 1f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 1 FAIL: intact settlement readout drifted. " +
                    $"settlement={readout.SettlementId} owner={readout.OwnerFactionId} tier={readout.CurrentTier} " +
                    $"frontage={readout.ReserveFrontage} breaches={readout.OpenBreachCount} walls={readout.DestroyedWallSegmentCount} " +
                    $"towers={readout.DestroyedTowerCount} gates={readout.DestroyedGateCount} keeps={readout.DestroyedKeepCount} " +
                    $"active={readout.BreachAssaultAdvantageActive} attack={readout.AggregateAttackMultiplier:F2} " +
                    $"speed={readout.AggregateSpeedMultiplier:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 1 PASS: tier={readout.CurrentTier} frontage={readout.ReserveFrontage} breaches={readout.OpenBreachCount}.");
            return true;
        }

        private static bool RunSingleBreachPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachLegibilityReadout_SingleBreach");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wall = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-1f, 0f, 0f));
            var attacker = CreateUnit(
                em,
                "enemy",
                "breach_shock",
                UnitRole.Melee,
                new float3(6f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, attacker, suppliedUntil: 20d);
            DestroyStructure(em, wall, maxHealth: 900f);
            Tick(world, SimStepSeconds * 2d);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachReadout(
                    new FixedString32Bytes("keep_player"),
                    out var readout))
            {
                sb.AppendLine("Phase 2 FAIL: single-breach settlement readout could not be resolved.");
                return false;
            }

            if (readout.OpenBreachCount != 1 ||
                readout.DestroyedWallSegmentCount != 1 ||
                readout.DestroyedTowerCount != 0 ||
                readout.DestroyedGateCount != 0 ||
                readout.DestroyedKeepCount != 0 ||
                readout.CurrentTier != 0 ||
                readout.ReserveFrontage != 1 ||
                !readout.BreachAssaultAdvantageActive ||
                !Approximately(readout.AggregateAttackMultiplier, 1.08f, 0.001f) ||
                !Approximately(readout.AggregateSpeedMultiplier, 1.04f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 2 FAIL: single breach readout did not expose the expected pressure state. " +
                    $"tier={readout.CurrentTier} frontage={readout.ReserveFrontage} breaches={readout.OpenBreachCount} " +
                    $"walls={readout.DestroyedWallSegmentCount} active={readout.BreachAssaultAdvantageActive} " +
                    $"attack={readout.AggregateAttackMultiplier:F2} speed={readout.AggregateSpeedMultiplier:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 2 PASS: breaches={readout.OpenBreachCount} attack={readout.AggregateAttackMultiplier:F2} speed={readout.AggregateSpeedMultiplier:F2}.");
            return true;
        }

        private static bool RunMultiBreachScalingPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachLegibilityReadout_MultiBreach");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wallA = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var wallB = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(3f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var gate = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(4f, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);
            var attacker = CreateUnit(
                em,
                "enemy",
                "breach_shock",
                UnitRole.Melee,
                new float3(6f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, attacker, suppliedUntil: 20d);
            DestroyStructure(em, wallA, maxHealth: 900f);
            DestroyStructure(em, wallB, maxHealth: 900f);
            DestroyStructure(em, gate, maxHealth: 1200f);
            Tick(world, SimStepSeconds * 2d);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachReadout(
                    new FixedString32Bytes("keep_player"),
                    out var readout))
            {
                sb.AppendLine("Phase 3 FAIL: multi-breach settlement readout could not be resolved.");
                return false;
            }

            if (readout.OpenBreachCount != 3 ||
                readout.DestroyedWallSegmentCount != 2 ||
                readout.DestroyedGateCount != 1 ||
                !readout.BreachAssaultAdvantageActive ||
                !Approximately(readout.AggregateAttackMultiplier, 1.24f, 0.001f) ||
                !Approximately(readout.AggregateSpeedMultiplier, 1.12f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 3 FAIL: multi-breach scaling did not cap correctly at three breaches. " +
                    $"breaches={readout.OpenBreachCount} walls={readout.DestroyedWallSegmentCount} gates={readout.DestroyedGateCount} " +
                    $"active={readout.BreachAssaultAdvantageActive} attack={readout.AggregateAttackMultiplier:F2} " +
                    $"speed={readout.AggregateSpeedMultiplier:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 3 PASS: breaches={readout.OpenBreachCount} attack={readout.AggregateAttackMultiplier:F2} speed={readout.AggregateSpeedMultiplier:F2}.");
            return true;
        }

        private static bool RunMissingSettlementPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachLegibilityReadout_Missing");
            using var scope = new DebugCommandSurfaceScope(world);

            bool found = scope.CommandSurface.TryDebugGetSettlementBreachReadout(
                new FixedString32Bytes("missing_keep"),
                out var readout);
            if (found ||
                readout.SettlementId.Length != 0 ||
                readout.OwnerFactionId.Length != 0 ||
                readout.OpenBreachCount != 0 ||
                readout.DestroyedWallSegmentCount != 0 ||
                readout.DestroyedTowerCount != 0 ||
                readout.DestroyedGateCount != 0 ||
                readout.DestroyedKeepCount != 0 ||
                readout.CurrentTier != 0 ||
                readout.ReserveFrontage != 0 ||
                readout.BreachAssaultAdvantageActive ||
                !Approximately(readout.AggregateAttackMultiplier, 0f, 0.001f) ||
                !Approximately(readout.AggregateSpeedMultiplier, 0f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 4 FAIL: missing settlement should return false with a default readout. " +
                    $"found={found} settlement={readout.SettlementId} owner={readout.OwnerFactionId} " +
                    $"attack={readout.AggregateAttackMultiplier:F2} speed={readout.AggregateSpeedMultiplier:F2}");
                return false;
            }

            sb.AppendLine("Phase 4 PASS: missing settlement returned false with default readout.");
            return true;
        }

        private static bool RunPartialDestructionVarietyPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachLegibilityReadout_Variety");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 8);
            var wallA = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var wallB = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(3f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var gate = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(4f, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);
            var tower = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Tower,
                "watch_tower",
                new float3(5f, 0f, 0f),
                currentHealth: 800f,
                maxHealth: 800f);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Keep,
                "keep_tier_1",
                new float3(1f, 0f, 0f),
                currentHealth: 1600f,
                maxHealth: 1600f);
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-1f, 0f, 0f));
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-2f, 0f, 0f));
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-3f, 0f, 0f));
            CreateLinkedCombatant(em, settlement, "keep_player", "player", new float3(-4f, 0f, 0f));

            Tick(world, 0.1d);
            DestroyStructure(em, wallA, maxHealth: 900f);
            DestroyStructure(em, wallB, maxHealth: 900f);
            DestroyStructure(em, gate, maxHealth: 1200f);
            DestroyStructure(em, tower, maxHealth: 800f);
            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachReadout(
                    new FixedString32Bytes("keep_player"),
                    out var readout))
            {
                sb.AppendLine("Phase 5 FAIL: partial-destruction settlement readout could not be resolved.");
                return false;
            }

            if (readout.OpenBreachCount != 3 ||
                readout.DestroyedWallSegmentCount != 2 ||
                readout.DestroyedTowerCount != 1 ||
                readout.DestroyedGateCount != 1 ||
                readout.DestroyedKeepCount != 0 ||
                readout.CurrentTier != 2 ||
                readout.ReserveFrontage != 3 ||
                readout.BreachAssaultAdvantageActive ||
                !Approximately(readout.AggregateAttackMultiplier, 1f, 0.001f) ||
                !Approximately(readout.AggregateSpeedMultiplier, 1f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 5 FAIL: partial-destruction variety readout drifted. " +
                    $"tier={readout.CurrentTier} frontage={readout.ReserveFrontage} breaches={readout.OpenBreachCount} " +
                    $"walls={readout.DestroyedWallSegmentCount} towers={readout.DestroyedTowerCount} " +
                    $"gates={readout.DestroyedGateCount} keeps={readout.DestroyedKeepCount} " +
                    $"active={readout.BreachAssaultAdvantageActive} attack={readout.AggregateAttackMultiplier:F2} " +
                    $"speed={readout.AggregateSpeedMultiplier:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 5 PASS: tier={readout.CurrentTier} frontage={readout.ReserveFrontage} breaches={readout.OpenBreachCount} walls={readout.DestroyedWallSegmentCount} gates={readout.DestroyedGateCount}.");
            return true;
        }

        private static bool RunSealingTelemetryPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateTelemetryValidationWorld("BloodlinesBreachLegibilityReadout_SealingTelemetry");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 200f);
            CreateIdleWorker(em, "player");
            CreateSettlement(em, "keep_player", "player", ceiling: 6);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 0f,
                maxHealth: 900f);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(4f, 0f, 0f),
                currentHealth: 0f,
                maxHealth: 1200f);

            Tick(world, HalfSealingWindowSeconds);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachTelemetry(
                    new FixedString32Bytes("keep_player"),
                    out var telemetry))
            {
                sb.AppendLine("Phase 6 FAIL: sealing telemetry could not be resolved.");
                return false;
            }

            if (telemetry.Readout.OpenBreachCount != 2 ||
                telemetry.Readout.DestroyedWallSegmentCount != 1 ||
                telemetry.Readout.DestroyedGateCount != 1 ||
                !telemetry.SealingEligible ||
                !telemetry.SealingTracked ||
                !Approximately(telemetry.SealingAccumulatedWorkerHours, 4f, 0.001f) ||
                !Approximately(telemetry.SealingRequiredWorkerHours, FortificationCanon.BreachSealingWorkerHoursPerBreach, 0.001f) ||
                !Approximately(telemetry.SealingReservedStone, FortificationCanon.BreachSealingStoneCostPerBreach, 0.001f) ||
                !Approximately(telemetry.SealingRequiredStone, FortificationCanon.BreachSealingStoneCostPerBreach, 0.001f) ||
                !Approximately(telemetry.SealingProgress01, 0.5f, 0.001f) ||
                telemetry.RecoveryEligible ||
                telemetry.RecoveryTracked ||
                telemetry.RecoveryTargetCounter != DestroyedCounterKind.None ||
                !Approximately(telemetry.RecoveryAccumulatedWorkerHours, 0f, 0.001f) ||
                !Approximately(telemetry.RecoveryRequiredWorkerHours, 0f, 0.001f) ||
                !Approximately(telemetry.RecoveryReservedStone, 0f, 0.001f) ||
                !Approximately(telemetry.RecoveryRequiredStone, 0f, 0.001f) ||
                !Approximately(telemetry.RecoveryProgress01, 0f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 6 FAIL: sealing telemetry drifted. " +
                    $"breaches={telemetry.Readout.OpenBreachCount} walls={telemetry.Readout.DestroyedWallSegmentCount} gates={telemetry.Readout.DestroyedGateCount} " +
                    $"sealEligible={telemetry.SealingEligible} sealTracked={telemetry.SealingTracked} " +
                    $"sealHours={telemetry.SealingAccumulatedWorkerHours:F2}/{telemetry.SealingRequiredWorkerHours:F2} " +
                    $"sealStone={telemetry.SealingReservedStone:F2}/{telemetry.SealingRequiredStone:F2} sealProgress={telemetry.SealingProgress01:F2} " +
                    $"recoveryEligible={telemetry.RecoveryEligible} recoveryTracked={telemetry.RecoveryTracked} " +
                    $"recoveryTarget={telemetry.RecoveryTargetCounter} recoveryHours={telemetry.RecoveryAccumulatedWorkerHours:F2}/{telemetry.RecoveryRequiredWorkerHours:F2} " +
                    $"recoveryStone={telemetry.RecoveryReservedStone:F2}/{telemetry.RecoveryRequiredStone:F2} recoveryProgress={telemetry.RecoveryProgress01:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 6 PASS: sealHours={telemetry.SealingAccumulatedWorkerHours:F2}/{telemetry.SealingRequiredWorkerHours:F2} sealStone={telemetry.SealingReservedStone:F2}/{telemetry.SealingRequiredStone:F2}.");
            return true;
        }

        private static bool RunRecoveryTelemetryPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateTelemetryValidationWorld(
                "BloodlinesBreachLegibilityReadout_RecoveryTelemetry",
                includeDestroyedCounterRecovery: true);
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            SeedDualClockRate(em, DaysPerSecond);
            SeedFactionStockpile(em, "player", stone: 400f);
            CreateIdleWorker(em, "player");
            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 6);
            var fortification = em.GetComponentData<FortificationComponent>(settlement);
            fortification.DestroyedGateCount = 1;
            fortification.DestroyedKeepCount = 1;
            fortification.OpenBreachCount = 0;
            em.SetComponentData(settlement, fortification);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(4f, 0f, 0f),
                currentHealth: 0f,
                maxHealth: 1200f);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Keep,
                "keep_tier_1",
                new float3(0.5f, 0f, 0f),
                currentHealth: 0f,
                maxHealth: 1600f);

            Tick(world, HalfKeepRebuildWindowSeconds);

            if (!scope.CommandSurface.TryDebugGetSettlementBreachTelemetry(
                    new FixedString32Bytes("keep_player"),
                    out var telemetry))
            {
                sb.AppendLine("Phase 7 FAIL: recovery telemetry could not be resolved.");
                return false;
            }

            float expectedRecoveryHours =
                FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment *
                FortificationCanon.DestroyedCounterRecoveryKeepMultiplier;
            float expectedRecoveryStone =
                FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment *
                FortificationCanon.DestroyedCounterRecoveryKeepMultiplier;
            if (telemetry.Readout.OpenBreachCount != 0 ||
                telemetry.Readout.DestroyedGateCount != 1 ||
                telemetry.Readout.DestroyedKeepCount != 1 ||
                telemetry.SealingEligible ||
                telemetry.SealingTracked ||
                !telemetry.RecoveryEligible ||
                !telemetry.RecoveryTracked ||
                telemetry.RecoveryTargetCounter != DestroyedCounterKind.Keep ||
                !Approximately(telemetry.RecoveryAccumulatedWorkerHours, 14f, 0.001f) ||
                !Approximately(telemetry.RecoveryRequiredWorkerHours, expectedRecoveryHours, 0.001f) ||
                !Approximately(telemetry.RecoveryReservedStone, expectedRecoveryStone, 0.001f) ||
                !Approximately(telemetry.RecoveryRequiredStone, expectedRecoveryStone, 0.001f) ||
                !Approximately(telemetry.RecoveryProgress01, 0.5f, 0.001f))
            {
                sb.AppendLine(
                    "Phase 7 FAIL: recovery telemetry drifted. " +
                    $"breaches={telemetry.Readout.OpenBreachCount} gate={telemetry.Readout.DestroyedGateCount} keep={telemetry.Readout.DestroyedKeepCount} " +
                    $"sealEligible={telemetry.SealingEligible} sealTracked={telemetry.SealingTracked} " +
                    $"recoveryEligible={telemetry.RecoveryEligible} recoveryTracked={telemetry.RecoveryTracked} " +
                    $"recoveryTarget={telemetry.RecoveryTargetCounter} recoveryHours={telemetry.RecoveryAccumulatedWorkerHours:F2}/{telemetry.RecoveryRequiredWorkerHours:F2} " +
                    $"recoveryStone={telemetry.RecoveryReservedStone:F2}/{telemetry.RecoveryRequiredStone:F2} recoveryProgress={telemetry.RecoveryProgress01:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 7 PASS: recoveryTarget={telemetry.RecoveryTargetCounter} recoveryHours={telemetry.RecoveryAccumulatedWorkerHours:F2}/{telemetry.RecoveryRequiredWorkerHours:F2} recoveryStone={telemetry.RecoveryReservedStone:F2}/{telemetry.RecoveryRequiredStone:F2}.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeComponentInitializationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationStructureLinkSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AdvanceFortificationTierSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationDestructionResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationReserveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterSupportScanSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachAssaultPressureSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterStrainSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static World CreateTelemetryValidationWorld(
            string worldName,
            bool includeDestroyedCounterRecovery = false)
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
            if (includeDestroyedCounterRecovery)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DestroyedCounterRecoverySystem>());
            }

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

        private static Entity CreateLinkedCombatant(
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
            entityManager.AddComponentData(entity, new FortificationCombatantTag());
            entityManager.AddComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
                SettlementId = settlementId,
            });
            entityManager.AddComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = ReserveDutyState.Ready,
            });
            return entity;
        }

        private static Entity CreateUnit(
            EntityManager entityManager,
            string factionId,
            string unitId,
            UnitRole role,
            float3 position,
            float currentHealth,
            float maxHealth,
            float attackDamage,
            float maxSpeed)
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
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = unitId,
                Role = role,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });
            entityManager.AddComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = attackDamage,
                AttackRange = 18f,
                AttackCooldown = 1f,
                Sight = 120f,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent
            {
                MaxSpeed = maxSpeed,
            });
            return entity;
        }

        private static void PrimeFieldWater(EntityManager entityManager, Entity unitEntity, double suppliedUntil)
        {
            var fieldWater = entityManager.GetComponentData<FieldWaterComponent>(unitEntity);
            fieldWater.SuppliedUntil = suppliedUntil;
            fieldWater.LastTransferAt = 0d;
            fieldWater.LastSupportRefreshAt = 0d;
            fieldWater.SupportRefreshCount = 0;
            fieldWater.Status = FieldWaterStatus.Steady;
            entityManager.SetComponentData(unitEntity, fieldWater);
        }

        private static void DestroyStructure(EntityManager entityManager, Entity structureEntity, float maxHealth)
        {
            entityManager.SetComponentData(structureEntity, new HealthComponent
            {
                Current = 0f,
                Max = maxHealth,
            });
        }

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesBreachLegibilityReadout_CommandSurface")
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
