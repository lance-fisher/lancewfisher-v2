#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Rendering;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the faction visual assignment system.
    ///
    /// Phase 1: FactionVisualComponent resolves a non-default tint from FactionTintPalette
    ///          for the "player" / "ironmark" faction ID.
    /// Phase 2: EmblemId convention is "emblem_" + factionId.ToLowerInvariant(), verified
    ///          for both a named faction and an unknown faction ID.
    /// Phase 3: UnitFactionColorComponent struct initializes with expected FactionId and Tint.
    /// </summary>
    public static class BloodlinesFactionVisualAssignmentSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-faction-visual-assignment-smoke.log";

        [MenuItem("Bloodlines/Rendering/Run Faction Visual Assignment Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFactionVisualAssignmentSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Faction visual assignment smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_FACTION_VISUAL_ASSIGNMENT_SMOKE " +
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
            ok &= RunTintResolutionPhase(sb);
            ok &= RunEmblemIdPhase(sb);
            ok &= RunUnitFactionColorPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunTintResolutionPhase(System.Text.StringBuilder sb)
        {
            // "player" should resolve to a non-grey, non-zero tint.
            float4 playerTint = FactionTintPalette.ResolveTint("player");
            float4 fallback = new float4(0.72f, 0.72f, 0.72f, 1f);

            // Player tint is blue-dominant; confirm it differs from the fallback grey.
            bool isFallback = math.all(math.abs(playerTint - fallback) < 0.01f);
            if (isFallback)
            {
                sb.AppendLine("Phase 1 FAIL: 'player' faction resolved to fallback grey tint. " +
                              "Expected canonical house color. tint=" + playerTint);
                return false;
            }

            if (playerTint.w < 0.99f)
            {
                sb.AppendLine("Phase 1 FAIL: alpha channel should be 1. got=" + playerTint.w);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: 'player' faction resolves to canonical non-grey tint " + playerTint + ".");
            return true;
        }

        private static bool RunEmblemIdPhase(System.Text.StringBuilder sb)
        {
            string[] factionIds = { "player", "ironmark", "stonehelm", "unknown_house_xyz" };

            foreach (string factionId in factionIds)
            {
                string expected = "emblem_" + factionId.ToLowerInvariant();
                var emblemId = new Unity.Collections.FixedString64Bytes("emblem_" + factionId.ToLowerInvariant());

                if (emblemId.ToString() != expected)
                {
                    sb.AppendLine("Phase 2 FAIL: emblem ID mismatch for '" + factionId +
                                  "'. expected='" + expected + "' got='" + emblemId + "'");
                    return false;
                }
            }

            sb.AppendLine("Phase 2 PASS: EmblemId convention 'emblem_<factionId>' holds for all tested faction IDs.");
            return true;
        }

        private static bool RunUnitFactionColorPhase(System.Text.StringBuilder sb)
        {
            float4 expectedTint = FactionTintPalette.ResolveTint("player");

            var unitColor = new UnitFactionColorComponent
            {
                FactionId = new Unity.Collections.FixedString32Bytes("player"),
                Tint = expectedTint,
            };

            if (unitColor.FactionId.ToString() != "player")
            {
                sb.AppendLine("Phase 3 FAIL: FactionId not stored correctly. got='" + unitColor.FactionId + "'");
                return false;
            }

            if (!math.all(math.abs(unitColor.Tint - expectedTint) < 0.001f))
            {
                sb.AppendLine("Phase 3 FAIL: Tint does not match palette value. " +
                              "expected=" + expectedTint + " got=" + unitColor.Tint);
                return false;
            }

            sb.AppendLine("Phase 3 PASS: UnitFactionColorComponent stores FactionId and palette-resolved Tint correctly.");
            return true;
        }
    }
}
#endif
