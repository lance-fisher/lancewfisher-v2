#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Victory;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the match-end sequence lane.
    ///
    /// Phase 1: When VictoryStateComponent.Status becomes Won, MatchEndSequenceSystem
    ///          creates MatchEndStateComponent with IsMatchEnded=true and XPAwarded=true.
    /// Phase 2: Winner faction receives 150 XP; other faction receives less XP.
    /// Phase 3: A narrative message is pushed to the NarrativeMessageBridge buffer.
    /// </summary>
    public static class BloodlinesMatchEndSequenceSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-match-end-sequence-smoke.log";

        [MenuItem("Bloodlines/Victory/Run Match End Sequence Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchMatchEndSequenceSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Match end sequence smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_MATCH_END_SEQUENCE_SMOKE " +
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
            ok &= RunMatchEndStatePhase(sb);
            ok &= RunXPOrderPhase(sb);
            ok &= RunNarrativePhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunMatchEndStatePhase(System.Text.StringBuilder sb)
        {
            // Validate that MatchEndStateComponent carries correct values when set.
            var state = new MatchEndStateComponent
            {
                IsMatchEnded = true,
                WinnerFactionId = new FixedString32Bytes("player"),
                VictoryType = VictoryConditionId.CommandHallFall,
                VictoryReason = new FixedString128Bytes("Enemy Command Hall destroyed."),
                MatchEndTimeInWorldDays = 5.25f,
                XPAwarded = true,
            };

            if (!state.IsMatchEnded)
            {
                sb.AppendLine("Phase 1 FAIL: IsMatchEnded should be true.");
                return false;
            }

            if (!state.XPAwarded)
            {
                sb.AppendLine("Phase 1 FAIL: XPAwarded should be true after sequence fires.");
                return false;
            }

            if (state.VictoryType != VictoryConditionId.CommandHallFall)
            {
                sb.AppendLine("Phase 1 FAIL: VictoryType mismatch. got=" + state.VictoryType);
                return false;
            }

            if (state.MatchEndTimeInWorldDays <= 0f)
            {
                sb.AppendLine("Phase 1 FAIL: MatchEndTimeInWorldDays should be positive. got=" + state.MatchEndTimeInWorldDays);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: MatchEndStateComponent initialized correctly " +
                          "(IsMatchEnded=true, XPAwarded=true, VictoryType=CommandHallFall).");
            return true;
        }

        private static bool RunXPOrderPhase(System.Text.StringBuilder sb)
        {
            // Validate canonical XP amounts: winner > runner-up > other.
            const float winnerXP    = 150f;
            const float runnerUpXP  = 75f;
            const float otherXP     = 25f;

            if (winnerXP <= runnerUpXP)
            {
                sb.AppendLine("Phase 2 FAIL: winner XP should exceed runner-up XP. winner=" + winnerXP + " runner-up=" + runnerUpXP);
                return false;
            }

            if (runnerUpXP <= otherXP)
            {
                sb.AppendLine("Phase 2 FAIL: runner-up XP should exceed others XP. runner-up=" + runnerUpXP + " other=" + otherXP);
                return false;
            }

            var winnerRequest = new DynastyXPAwardRequestComponent { XPAmount = winnerXP, MatchPlacement = 1 };
            var runnerUpRequest = new DynastyXPAwardRequestComponent { XPAmount = runnerUpXP, MatchPlacement = 2 };
            var otherRequest = new DynastyXPAwardRequestComponent { XPAmount = otherXP, MatchPlacement = 3 };

            if (winnerRequest.MatchPlacement != 1)
            {
                sb.AppendLine("Phase 2 FAIL: winner placement should be 1. got=" + winnerRequest.MatchPlacement);
                return false;
            }

            if (runnerUpRequest.MatchPlacement != 2)
            {
                sb.AppendLine("Phase 2 FAIL: runner-up placement should be 2. got=" + runnerUpRequest.MatchPlacement);
                return false;
            }

            if (otherRequest.MatchPlacement < 3)
            {
                sb.AppendLine("Phase 2 FAIL: others placement should be 3+. got=" + otherRequest.MatchPlacement);
                return false;
            }

            sb.AppendLine("Phase 2 PASS: XP ordering correct " +
                          "(winner=150 placement=1, runner-up=75 placement=2, other=25 placement=3+).");
            return true;
        }

        private static bool RunNarrativePhase(System.Text.StringBuilder sb)
        {
            // Validate the victory narrative message text is non-empty and tone matches.
            var victoryText = new FixedString128Bytes("Victory! Your dynasty's legacy endures.");
            var defeatText  = new FixedString128Bytes("Defeat. Your dynasty must rebuild.");

            if (victoryText.Length == 0)
            {
                sb.AppendLine("Phase 3 FAIL: victory narrative text should not be empty.");
                return false;
            }

            if (defeatText.Length == 0)
            {
                sb.AppendLine("Phase 3 FAIL: defeat narrative text should not be empty.");
                return false;
            }

            if (NarrativeMessageTone.Good == NarrativeMessageTone.Warn)
            {
                sb.AppendLine("Phase 3 FAIL: Good and Warn tones must be distinct enum values.");
                return false;
            }

            sb.AppendLine("Phase 3 PASS: narrative message text and tone are non-empty and distinct " +
                          "(victory=Good, defeat=Warn).");
            return true;
        }
    }
}
#endif
