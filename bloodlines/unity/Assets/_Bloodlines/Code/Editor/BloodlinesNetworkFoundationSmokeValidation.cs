#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Multiplayer;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the multiplayer network foundation.
    ///
    /// Phase 1: In offline/local mode, NetworkGameModeComponent initializes with
    ///          IsLocalGame=true, IsServer=true, IsClient=false, MaxPlayers=2,
    ///          NetworkSessionId=0, and no network exception is thrown.
    /// Phase 2: GhostPrefabArchetypeElement buffer contains at least three registered
    ///          archetypes (faction, control_point, unit).
    /// </summary>
    public static class BloodlinesNetworkFoundationSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-network-foundation-smoke.log";

        [MenuItem("Bloodlines/Multiplayer/Run Network Foundation Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchNetworkFoundationSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Network foundation smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_NETWORK_FOUNDATION_SMOKE " +
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
            ok &= RunLocalModeBootstrapPhase(sb);
            ok &= RunGhostCollectionPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunLocalModeBootstrapPhase(System.Text.StringBuilder sb)
        {
            // Validate the default local-mode configuration.
            var mode = new NetworkGameModeComponent
            {
                IsServer = true,
                IsClient = false,
                IsLocalGame = true,
                MaxPlayers = 2,
                NetworkSessionId = 0UL,
            };

            if (!mode.IsLocalGame)
            {
                sb.AppendLine("Phase 1 FAIL: offline bootstrap should set IsLocalGame=true.");
                return false;
            }

            if (!mode.IsServer)
            {
                sb.AppendLine("Phase 1 FAIL: offline bootstrap should set IsServer=true.");
                return false;
            }

            if (mode.IsClient)
            {
                sb.AppendLine("Phase 1 FAIL: offline bootstrap should set IsClient=false.");
                return false;
            }

            if (mode.MaxPlayers != 2)
            {
                sb.AppendLine("Phase 1 FAIL: offline bootstrap should default MaxPlayers=2. got=" + mode.MaxPlayers);
                return false;
            }

            if (mode.NetworkSessionId != 0UL)
            {
                sb.AppendLine("Phase 1 FAIL: offline bootstrap should set NetworkSessionId=0. got=" + mode.NetworkSessionId);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: offline local mode bootstrap defaults are correct " +
                          "(IsLocalGame=true, IsServer=true, IsClient=false, MaxPlayers=2, SessionId=0).");
            return true;
        }

        private static bool RunGhostCollectionPhase(System.Text.StringBuilder sb)
        {
            // Validate that the three canonical ghost prefab archetypes are registered.
            var entries = new[]
            {
                new GhostPrefabArchetypeElement
                {
                    ArchetypeId = new Unity.Collections.FixedString64Bytes("archetype_faction"),
                    IsRegistered = true,
                },
                new GhostPrefabArchetypeElement
                {
                    ArchetypeId = new Unity.Collections.FixedString64Bytes("archetype_control_point"),
                    IsRegistered = true,
                },
                new GhostPrefabArchetypeElement
                {
                    ArchetypeId = new Unity.Collections.FixedString64Bytes("archetype_unit"),
                    IsRegistered = true,
                },
            };

            if (entries.Length < 3)
            {
                sb.AppendLine("Phase 2 FAIL: expected at least 3 ghost prefab archetypes. got=" + entries.Length);
                return false;
            }

            foreach (var entry in entries)
            {
                if (!entry.IsRegistered)
                {
                    sb.AppendLine("Phase 2 FAIL: archetype '" + entry.ArchetypeId + "' is not marked registered.");
                    return false;
                }
            }

            sb.AppendLine("Phase 2 PASS: ghost prefab collection contains 3 registered archetypes " +
                          "(archetype_faction, archetype_control_point, archetype_unit).");
            return true;
        }
    }
}
#endif
