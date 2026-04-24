#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the battlefield-shell binding that renders
    /// the settled player command-deck snapshot into visible overlay lines.
    /// </summary>
    public static class BloodlinesPlayerCommandDeckOverlaySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-player-command-deck-overlay-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Player Command Deck Overlay Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCommandDeckOverlaySmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception exception)
            {
                success = false;
                message = "Player command-deck overlay smoke validation errored: " + exception;
            }

            string artifact = "BLOODLINES_PLAYER_COMMAND_DECK_OVERLAY_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
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
            var lines = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunSummaryPhase(lines);
            ok &= RunGreatReckoningPhase(lines);
            ok &= RunFortificationThreatPhase(lines);
            ok &= RunPressureBandPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunSummaryPhase(System.Text.StringBuilder lines)
        {
            using var world = new World("player-command-deck-overlay-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedPlayerSnapshot(world.EntityManager,
                stageLabel: "Encounter Establishment",
                phaseLabel: "commitment",
                primaryAlertLabel: "stable",
                victoryConditionId: "TerritorialGovernance",
                victoryProgressPct: 0.62f,
                victoryRank: 2,
                victoryEtaDays: 18f,
                renownRank: 2,
                renownScore: 28f,
                renownBandLabel: "rising",
                worldPressureLabel: "watchful",
                worldPressureLevel: 3,
                populationBand: "yellow",
                loyaltyBand: "green",
                faithBand: "green",
                greatReckoningActive: false,
                fortificationThreatActive: false);

            if (!TryReadOverlay(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "FactionId") != "player" ||
                ReadField(fields, "PrimaryAlertLabel") != "stable" ||
                ReadField(fields, "VictoryConditionId") != "TerritorialGovernance" ||
                ReadField(fields, "VictoryRank") != "2" ||
                !ReadField(fields, "StageLine").Contains("Encounter Establishment / commitment", StringComparison.Ordinal) ||
                !ReadField(fields, "VictoryLine").Contains("TerritorialGovernance 62.0%", StringComparison.Ordinal) ||
                !ReadField(fields, "VictoryLine").Contains("ETA 18.0d", StringComparison.Ordinal) ||
                !ReadField(fields, "DynastyLine").Contains("Renown 28.0 rank 2 rising", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected overlay summary '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: battlefield overlay renders stage, victory ETA, and renown summary from the command-deck snapshot.");
            return true;
        }

        private static bool RunGreatReckoningPhase(System.Text.StringBuilder lines)
        {
            using var world = new World("player-command-deck-overlay-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedPlayerSnapshot(world.EntityManager,
                stageLabel: "Final Convergence",
                phaseLabel: "resolution",
                primaryAlertLabel: "great_reckoning",
                victoryConditionId: "TerritorialGovernance",
                victoryProgressPct: 0.91f,
                victoryRank: 1,
                victoryEtaDays: 2f,
                renownRank: 1,
                renownScore: 49f,
                renownBandLabel: "ascendant",
                worldPressureLabel: "high",
                worldPressureLevel: 7,
                populationBand: "green",
                loyaltyBand: "green",
                faithBand: "green",
                greatReckoningActive: true,
                fortificationThreatActive: false);

            if (!TryReadOverlay(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "GreatReckoningActive") != "true" ||
                !ReadField(fields, "AlertLine").Contains("great reckoning", StringComparison.OrdinalIgnoreCase))
            {
                lines.AppendLine($"Phase 2 FAIL: expected great-reckoning overlay alert, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: battlefield overlay surfaces the great-reckoning alert line.");
            return true;
        }

        private static bool RunFortificationThreatPhase(System.Text.StringBuilder lines)
        {
            using var world = new World("player-command-deck-overlay-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedPlayerSnapshot(world.EntityManager,
                stageLabel: "War Turning",
                phaseLabel: "commitment",
                primaryAlertLabel: "fortification_threat",
                victoryConditionId: "DivineRight",
                victoryProgressPct: 0.40f,
                victoryRank: 1,
                victoryEtaDays: float.NaN,
                renownRank: 1,
                renownScore: 30f,
                renownBandLabel: "rising",
                worldPressureLabel: "rising",
                worldPressureLevel: 5,
                populationBand: "green",
                loyaltyBand: "green",
                faithBand: "green",
                greatReckoningActive: false,
                fortificationThreatActive: true);

            if (!TryReadOverlay(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "FortificationThreatActive") != "true" ||
                !ReadField(fields, "AlertLine").Contains("fortifications threatened", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 3 FAIL: expected fortification-threat overlay alert, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: battlefield overlay surfaces fortification threat directly in the alert line.");
            return true;
        }

        private static bool RunPressureBandPhase(System.Text.StringBuilder lines)
        {
            using var world = new World("player-command-deck-overlay-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedPlayerSnapshot(world.EntityManager,
                stageLabel: "Final Convergence",
                phaseLabel: "resolution",
                primaryAlertLabel: "world_pressure",
                victoryConditionId: "CommandHallFall",
                victoryProgressPct: 0.50f,
                victoryRank: 2,
                victoryEtaDays: float.NaN,
                renownRank: 3,
                renownScore: 17f,
                renownBandLabel: "steady",
                worldPressureLabel: "severe",
                worldPressureLevel: 8,
                populationBand: "red",
                loyaltyBand: "yellow",
                faithBand: "green",
                greatReckoningActive: false,
                fortificationThreatActive: false);

            if (!TryReadOverlay(debugScope.CommandSurface, out var fields, out var readout) ||
                !ReadField(fields, "PressureLine").Contains("Pressure severe 8", StringComparison.Ordinal) ||
                !ReadField(fields, "PressureLine").Contains("pop red", StringComparison.Ordinal) ||
                !ReadField(fields, "PressureLine").Contains("loyalty yellow", StringComparison.Ordinal) ||
                !ReadField(fields, "PressureLine").Contains("faith green", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 4 FAIL: expected pressure-band overlay line, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: battlefield overlay renders pressure level plus population, loyalty, and faith bands.");
            return true;
        }

        private static void SeedPlayerSnapshot(
            EntityManager entityManager,
            string stageLabel,
            string phaseLabel,
            string primaryAlertLabel,
            string victoryConditionId,
            float victoryProgressPct,
            int victoryRank,
            float victoryEtaDays,
            int renownRank,
            float renownScore,
            string renownBandLabel,
            string worldPressureLabel,
            int worldPressureLevel,
            string populationBand,
            string loyaltyBand,
            string faithBand,
            bool greatReckoningActive,
            bool fortificationThreatActive)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PlayerCommandDeckHUDComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes("player"),
            });
            entityManager.SetComponentData(entity, new PlayerCommandDeckHUDComponent
            {
                FactionId = new FixedString32Bytes("player"),
                LastRefreshInWorldDays = 12f,
                StageLabel = new FixedString64Bytes(stageLabel),
                PhaseLabel = new FixedString32Bytes(phaseLabel),
                WorldPressureLabel = new FixedString32Bytes(worldPressureLabel),
                WorldPressureLevel = worldPressureLevel,
                GreatReckoningActive = greatReckoningActive,
                LeadingVictoryConditionId = new FixedString32Bytes(victoryConditionId),
                LeadingVictoryProgressPct = victoryProgressPct,
                LeadingVictoryEtaInWorldDays = victoryEtaDays,
                VictoryRank = victoryRank,
                VictoryLeaderFactionId = new FixedString32Bytes(victoryRank == 1 ? "player" : "enemy"),
                RenownRank = renownRank,
                RenownScore = renownScore,
                RenownBandLabel = new FixedString32Bytes(renownBandLabel),
                PopulationBand = new FixedString32Bytes(populationBand),
                LoyaltyBand = new FixedString32Bytes(loyaltyBand),
                FaithBand = new FixedString32Bytes(faithBand),
                FortificationThreatActive = fortificationThreatActive,
                PrimaryAlertLabel = new FixedString32Bytes(primaryAlertLabel),
            });
        }

        private static bool TryReadOverlay(
            BloodlinesDebugCommandSurface commandSurface,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetBattlefieldCommandDeckOverlay(out readout))
            {
                return false;
            }

            string[] parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "BattlefieldCommandDeckOverlay", StringComparison.Ordinal))
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
            return fields.TryGetValue(key, out string value) ? value : string.Empty;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesPlayerCommandDeckOverlaySmokeValidation_CommandSurface")
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
