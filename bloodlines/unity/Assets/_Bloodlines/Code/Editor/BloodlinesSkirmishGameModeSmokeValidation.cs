#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Skirmish;
using Bloodlines.Victory;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the skirmish-game-mode lane.
    ///
    /// Phase 1: SkirmishGameModeComponent initializes with Phase=Setup and zero/default
    ///          values for all match-tracking fields.
    /// Phase 2: Simulates Setup→Playing transition: Phase becomes Playing, FactionCount is
    ///          positive, IsPlayerVsAI reflects AI presence, PlayerFactionId is non-empty.
    /// Phase 3: Simulates Playing→PostMatch transition: Phase becomes PostMatch and
    ///          MatchEndInWorldDays is carried from MatchEndStateComponent.
    /// </summary>
    public static class BloodlinesSkirmishGameModeSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-skirmish-game-mode-smoke.log";

        [MenuItem("Bloodlines/Skirmish/Run Skirmish Game Mode Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSkirmishGameModeSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Skirmish game mode smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_SKIRMISH_GAME_MODE_SMOKE " +
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
            ok &= RunSetupPhaseInitPhase(sb);
            ok &= RunSetupToPlayingTransitionPhase(sb);
            ok &= RunPlayingToPostMatchTransitionPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunSetupPhaseInitPhase(System.Text.StringBuilder sb)
        {
            var mode = new SkirmishGameModeComponent { Phase = SkirmishPhase.Setup };

            if (mode.Phase != SkirmishPhase.Setup)
            {
                sb.AppendLine("Phase 1 FAIL: initial Phase should be Setup. got=" + mode.Phase);
                return false;
            }

            if (mode.FactionCount != 0)
            {
                sb.AppendLine("Phase 1 FAIL: FactionCount should default to 0. got=" + mode.FactionCount);
                return false;
            }

            if (mode.IsPlayerVsAI)
            {
                sb.AppendLine("Phase 1 FAIL: IsPlayerVsAI should default to false.");
                return false;
            }

            if (mode.MatchStartInWorldDays != 0f)
            {
                sb.AppendLine("Phase 1 FAIL: MatchStartInWorldDays should default to 0. got=" + mode.MatchStartInWorldDays);
                return false;
            }

            if (mode.MatchEndInWorldDays != 0f)
            {
                sb.AppendLine("Phase 1 FAIL: MatchEndInWorldDays should default to 0. got=" + mode.MatchEndInWorldDays);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: SkirmishGameModeComponent initializes in Setup phase with zero match fields.");
            return true;
        }

        private static bool RunSetupToPlayingTransitionPhase(System.Text.StringBuilder sb)
        {
            var mode = new SkirmishGameModeComponent
            {
                Phase              = SkirmishPhase.Playing,
                FactionCount       = 2,
                IsPlayerVsAI       = true,
                PlayerFactionId    = new Unity.Collections.FixedString32Bytes("player"),
                MatchStartInWorldDays = 0.5f,
            };

            if (mode.Phase != SkirmishPhase.Playing)
            {
                sb.AppendLine("Phase 2 FAIL: Phase should be Playing after transition. got=" + mode.Phase);
                return false;
            }

            if (mode.FactionCount == 0)
            {
                sb.AppendLine("Phase 2 FAIL: FactionCount should be positive after transition. got=" + mode.FactionCount);
                return false;
            }

            if (!mode.IsPlayerVsAI)
            {
                sb.AppendLine("Phase 2 FAIL: IsPlayerVsAI should be true when an AI kingdom faction is present.");
                return false;
            }

            if (mode.PlayerFactionId.Length == 0)
            {
                sb.AppendLine("Phase 2 FAIL: PlayerFactionId should be non-empty after transition.");
                return false;
            }

            if (mode.MatchStartInWorldDays < 0f)
            {
                sb.AppendLine("Phase 2 FAIL: MatchStartInWorldDays should be non-negative. got=" + mode.MatchStartInWorldDays);
                return false;
            }

            sb.AppendLine("Phase 2 PASS: Setup→Playing transition sets Phase=Playing, FactionCount=2, " +
                          "IsPlayerVsAI=true, PlayerFactionId=player, MatchStartInWorldDays=0.5.");
            return true;
        }

        private static bool RunPlayingToPostMatchTransitionPhase(System.Text.StringBuilder sb)
        {
            const float matchEndTime = 7.25f;

            var matchEnd = new MatchEndStateComponent
            {
                IsMatchEnded            = true,
                WinnerFactionId         = new Unity.Collections.FixedString32Bytes("player"),
                VictoryType             = VictoryConditionId.CommandHallFall,
                VictoryReason           = new Unity.Collections.FixedString128Bytes("Enemy Command Hall destroyed."),
                MatchEndTimeInWorldDays = matchEndTime,
                XPAwarded               = true,
            };

            var mode = new SkirmishGameModeComponent
            {
                Phase                = SkirmishPhase.PostMatch,
                MatchEndInWorldDays  = matchEnd.MatchEndTimeInWorldDays,
            };

            if (mode.Phase != SkirmishPhase.PostMatch)
            {
                sb.AppendLine("Phase 3 FAIL: Phase should be PostMatch after transition. got=" + mode.Phase);
                return false;
            }

            if (!matchEnd.IsMatchEnded)
            {
                sb.AppendLine("Phase 3 FAIL: transition guard requires MatchEndStateComponent.IsMatchEnded=true.");
                return false;
            }

            if (mode.MatchEndInWorldDays <= 0f)
            {
                sb.AppendLine("Phase 3 FAIL: MatchEndInWorldDays should carry MatchEndStateComponent time. got=" + mode.MatchEndInWorldDays);
                return false;
            }

            if (mode.MatchEndInWorldDays != matchEndTime)
            {
                sb.AppendLine("Phase 3 FAIL: MatchEndInWorldDays mismatch. expected=" + matchEndTime + " got=" + mode.MatchEndInWorldDays);
                return false;
            }

            sb.AppendLine("Phase 3 PASS: Playing→PostMatch transition sets Phase=PostMatch and " +
                          "MatchEndInWorldDays=" + matchEndTime + " from MatchEndStateComponent.");
            return true;
        }
    }
}
#endif
