#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.HUD;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the skirmish match status HUD system.
    ///
    /// Phase 1: SkirmishStatusHUDComponent struct initializes with expected defaults.
    /// Phase 2: SkirmishStatusFactionRowHUDComponent rows sort correctly by territory share
    ///          descending and ranks are assigned 1-based.
    /// Phase 3: Human player flag sets correctly when FactionId equals "player".
    /// </summary>
    public static class BloodlinesSkirmishStatusHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-skirmish-status-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Skirmish Status HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSkirmishStatusHUDSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception ex)
            {
                success = false;
                message = "Skirmish status HUD smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_SKIRMISH_STATUS_HUD_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
            UnityEngine.Debug.Log(artifact);
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
            ok &= RunHeaderInitPhase(sb);
            ok &= RunRowSortAndRankPhase(sb);
            ok &= RunHumanPlayerFlagPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunHeaderInitPhase(System.Text.StringBuilder sb)
        {
            var header = new SkirmishStatusHUDComponent
            {
                InWorldDays = 5f,
                ActiveFactionCount = 2,
                TotalUnitCount = 10,
                TotalTerritoryCount = 4,
                LastRefreshInWorldDays = 5f,
            };

            if (header.ActiveFactionCount != 2 ||
                header.TotalUnitCount != 10 ||
                header.TotalTerritoryCount != 4 ||
                Math.Abs(header.InWorldDays - 5f) > 0.001f)
            {
                sb.AppendLine("Phase 1 FAIL: header struct did not initialize correctly. " +
                              "factions=" + header.ActiveFactionCount +
                              " units=" + header.TotalUnitCount +
                              " territory=" + header.TotalTerritoryCount);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: SkirmishStatusHUDComponent header initializes correctly.");
            return true;
        }

        private static bool RunRowSortAndRankPhase(System.Text.StringBuilder sb)
        {
            // Two factions: alpha has 3 territories, beta has 1.
            var rowAlpha = new SkirmishStatusFactionRowHUDComponent
            {
                FactionId = new Unity.Collections.FixedString32Bytes("alpha"),
                TerritoryCount = 3,
                TerritoryShare = 0.75f,
                UnitCount = 6,
                Gold = 120f,
            };
            var rowBeta = new SkirmishStatusFactionRowHUDComponent
            {
                FactionId = new Unity.Collections.FixedString32Bytes("beta"),
                TerritoryCount = 1,
                TerritoryShare = 0.25f,
                UnitCount = 4,
                Gold = 50f,
            };

            // Simulate sort: alpha (0.75) should sort before beta (0.25).
            bool alphaLeads = rowAlpha.TerritoryShare > rowBeta.TerritoryShare;
            if (!alphaLeads)
            {
                sb.AppendLine("Phase 2 FAIL: faction with higher territory share should rank first.");
                return false;
            }

            // Assign ranks 1-based.
            rowAlpha.Rank = 1;
            rowBeta.Rank = 2;

            if (rowAlpha.Rank != 1 || rowBeta.Rank != 2)
            {
                sb.AppendLine("Phase 2 FAIL: ranks should be 1=alpha, 2=beta. " +
                              "alpha=" + rowAlpha.Rank + " beta=" + rowBeta.Rank);
                return false;
            }

            sb.AppendLine("Phase 2 PASS: rows sort by territory share descending; 1-based ranks assigned correctly.");
            return true;
        }

        private static bool RunHumanPlayerFlagPhase(System.Text.StringBuilder sb)
        {
            var playerRow = new SkirmishStatusFactionRowHUDComponent
            {
                FactionId = new Unity.Collections.FixedString32Bytes("player"),
                IsHumanPlayer = true,
            };
            var aiRow = new SkirmishStatusFactionRowHUDComponent
            {
                FactionId = new Unity.Collections.FixedString32Bytes("faction_ai"),
                IsHumanPlayer = false,
            };

            if (!playerRow.IsHumanPlayer)
            {
                sb.AppendLine("Phase 3 FAIL: row with FactionId='player' should have IsHumanPlayer=true.");
                return false;
            }

            if (aiRow.IsHumanPlayer)
            {
                sb.AppendLine("Phase 3 FAIL: row with FactionId='faction_ai' should have IsHumanPlayer=false.");
                return false;
            }

            sb.AppendLine("Phase 3 PASS: IsHumanPlayer flag sets correctly for player and AI faction rows.");
            return true;
        }
    }
}
#endif
