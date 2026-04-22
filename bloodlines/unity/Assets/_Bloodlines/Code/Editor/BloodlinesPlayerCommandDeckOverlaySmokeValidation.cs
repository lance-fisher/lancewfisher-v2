#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.HUD;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the on-screen command-deck overlay copy contract.
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
            ok &= RunStableProjectionPhase(lines);
            ok &= RunVictoryEtaPhase(lines);
            ok &= RunGreatReckoningPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunStableProjectionPhase(System.Text.StringBuilder lines)
        {
            var snapshot = BuildSnapshot(
                primaryAlertLabel: "stable",
                leadingVictoryConditionId: "TerritorialGovernance",
                leadingVictoryProgressPct: 0.62f,
                leadingVictoryEtaInWorldDays: 18f,
                fortificationThreatActive: false,
                greatReckoningActive: false);

            string title = PlayerCommandDeckOverlayPresenter.BuildTitle(in snapshot);
            string body = PlayerCommandDeckOverlayPresenter.BuildBody(in snapshot);
            if (!title.Contains("Encounter Establishment", StringComparison.Ordinal) ||
                !body.Contains("Alert: stable", StringComparison.Ordinal) ||
                !body.Contains("Victory: TerritorialGovernance", StringComparison.Ordinal) ||
                !body.Contains("Fortification Threat: Stable", StringComparison.Ordinal))
            {
                lines.AppendLine("Phase 1 FAIL: stable overlay copy omitted core stage/victory/threat lines.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: stable overlay shows stage title, victory line, and stable threat state.");
            return true;
        }

        private static bool RunVictoryEtaPhase(System.Text.StringBuilder lines)
        {
            var snapshot = BuildSnapshot(
                primaryAlertLabel: "victory_imminent",
                leadingVictoryConditionId: "CommandHallFall",
                leadingVictoryProgressPct: 1f,
                leadingVictoryEtaInWorldDays: 0f,
                fortificationThreatActive: true,
                greatReckoningActive: false);

            string body = PlayerCommandDeckOverlayPresenter.BuildBody(in snapshot);
            if (!body.Contains("100.0%", StringComparison.Ordinal) ||
                !body.Contains("ETA 0.0d", StringComparison.Ordinal) ||
                !body.Contains("Fortification Threat: Active", StringComparison.Ordinal))
            {
                lines.AppendLine("Phase 2 FAIL: overlay body did not surface the victory ETA or active threat state.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: overlay body surfaces victory completion, ETA, and active fortification threat.");
            return true;
        }

        private static bool RunGreatReckoningPhase(System.Text.StringBuilder lines)
        {
            var snapshot = BuildSnapshot(
                primaryAlertLabel: "great_reckoning",
                leadingVictoryConditionId: "DivineRight",
                leadingVictoryProgressPct: 0.91f,
                leadingVictoryEtaInWorldDays: float.NaN,
                fortificationThreatActive: false,
                greatReckoningActive: true);

            string alertKey = PlayerCommandDeckOverlayPresenter.ResolveAlertColorKey(in snapshot);
            string body = PlayerCommandDeckOverlayPresenter.BuildBody(in snapshot);
            if (!string.Equals(alertKey, "great_reckoning", StringComparison.Ordinal) ||
                !body.Contains("Great Reckoning: Active", StringComparison.Ordinal))
            {
                lines.AppendLine("Phase 3 FAIL: overlay alert state did not preserve the great reckoning priority.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: overlay preserves the great reckoning alert key and body callout.");
            return true;
        }

        private static PlayerCommandDeckHUDComponent BuildSnapshot(
            string primaryAlertLabel,
            string leadingVictoryConditionId,
            float leadingVictoryProgressPct,
            float leadingVictoryEtaInWorldDays,
            bool fortificationThreatActive,
            bool greatReckoningActive)
        {
            return new PlayerCommandDeckHUDComponent
            {
                FactionId = new FixedString32Bytes("player"),
                StageLabel = new FixedString64Bytes("Encounter Establishment"),
                PhaseLabel = new FixedString32Bytes("commitment"),
                WorldPressureLabel = new FixedString32Bytes("watchful"),
                WorldPressureLevel = 3,
                GreatReckoningActive = greatReckoningActive,
                LeadingVictoryConditionId = new FixedString32Bytes(leadingVictoryConditionId),
                LeadingVictoryProgressPct = leadingVictoryProgressPct,
                LeadingVictoryEtaInWorldDays = leadingVictoryEtaInWorldDays,
                VictoryRank = 1,
                VictoryLeaderFactionId = new FixedString32Bytes("player"),
                RenownRank = 2,
                RenownScore = 28f,
                RenownBandLabel = new FixedString32Bytes("rising"),
                PopulationBand = new FixedString32Bytes("yellow"),
                LoyaltyBand = new FixedString32Bytes("green"),
                FaithBand = new FixedString32Bytes("green"),
                FortificationThreatActive = fortificationThreatActive,
                PrimaryAlertLabel = new FixedString32Bytes(primaryAlertLabel),
            };
        }
    }
}
#endif
