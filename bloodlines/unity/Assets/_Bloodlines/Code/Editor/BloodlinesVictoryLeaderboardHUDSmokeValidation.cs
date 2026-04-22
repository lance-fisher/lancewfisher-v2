#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Dedicated smoke validator for the victory leaderboard HUD read-model.
    /// Proves row population, human-player flagging, and ordered leader ranking.
    /// </summary>
    public static class BloodlinesVictoryLeaderboardHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-victory-leaderboard-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Victory Leaderboard HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchVictoryLeaderboardHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Victory leaderboard HUD smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_VICTORY_LEADERBOARD_HUD_SMOKE " +
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
            ok &= RunPopulationPhase(lines);
            ok &= RunHumanFlagPhase(lines);
            ok &= RunOrderingPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunPopulationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-leaderboard-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var player = SeedFaction(entityManager, "player");
            var enemy = SeedFaction(entityManager, "enemy");
            SeedReadout(entityManager, player, ("TerritorialGovernance", 0.80f, true), ("DivineRight", 0.25f, false));
            SeedReadout(entityManager, enemy, ("TerritorialGovernance", 0.45f, false), ("DivineRight", 0.60f, true));

            TickOnce(world);

            if (!TryGetLeaderboardEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read victory leaderboard.");
                return false;
            }

            if (entries.Count != 2 ||
                ReadField(entries[0], "FactionId") != "player" ||
                ReadField(entries[0], "LeadingConditionId") != "TerritorialGovernance" ||
                Math.Abs(ParseFloat(entries[0], "ProgressPct") - 0.8f) > 0.001f ||
                ReadField(entries[1], "FactionId") != "enemy" ||
                ReadField(entries[1], "LeadingConditionId") != "DivineRight" ||
                Math.Abs(ParseFloat(entries[1], "ProgressPct") - 0.6f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected leaderboard population '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: two-faction leaderboard surfaces player TerritorialGovernance=0.800 ahead of enemy DivineRight=0.600.");
            return true;
        }

        private static bool RunHumanFlagPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-leaderboard-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var player = SeedFaction(entityManager, "player");
            var enemy = SeedFaction(entityManager, "enemy");
            SeedReadout(entityManager, player, ("CommandHallFall", 0.15f, true));
            SeedReadout(entityManager, enemy, ("TerritorialGovernance", 0.10f, true));

            TickOnce(world);

            if (!TryGetLeaderboardEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read victory leaderboard for human flag test.");
                return false;
            }

            if (entries.Count != 2 ||
                ReadField(entries[0], "FactionId") != "player" ||
                ReadField(entries[0], "IsHumanPlayer") != "true" ||
                ReadField(entries[1], "FactionId") != "enemy" ||
                ReadField(entries[1], "IsHumanPlayer") != "false")
            {
                lines.AppendLine($"Phase 2 FAIL: unexpected human-player flags '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: leaderboard marks player rows as human and enemy rows as non-human.");
            return true;
        }

        private static bool RunOrderingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-leaderboard-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var player = SeedFaction(entityManager, "player");
            var enemy = SeedFaction(entityManager, "enemy");
            var rival = SeedFaction(entityManager, "rival");
            SeedReadout(entityManager, player, ("TerritorialGovernance", 0.52f, true));
            SeedReadout(entityManager, enemy, ("DivineRight", 0.91f, true));
            SeedReadout(entityManager, rival, ("CommandHallFall", 0.64f, true));

            TickOnce(world);

            if (!TryGetLeaderboardEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read victory leaderboard ordering.");
                return false;
            }

            if (entries.Count != 3 ||
                ReadField(entries[0], "FactionId") != "enemy" ||
                ReadField(entries[0], "Rank") != "1" ||
                ReadField(entries[1], "FactionId") != "rival" ||
                ReadField(entries[1], "Rank") != "2" ||
                ReadField(entries[2], "FactionId") != "player" ||
                ReadField(entries[2], "Rank") != "3")
            {
                lines.AppendLine($"Phase 3 FAIL: expected descending victory ordering, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: highest-progress faction ranks first, followed by descending progress.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryLeaderboardHUDSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new Unity.Core.TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity SeedFaction(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity(typeof(FactionComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.AddBuffer<VictoryConditionReadoutComponent>(entity);
            return entity;
        }

        private static void SeedReadout(
            EntityManager entityManager,
            Entity factionEntity,
            params (string ConditionId, float ProgressPct, bool IsLeading)[] entries)
        {
            var buffer = entityManager.GetBuffer<VictoryConditionReadoutComponent>(factionEntity);
            buffer.Clear();
            foreach (var entry in entries)
            {
                buffer.Add(new VictoryConditionReadoutComponent
                {
                    ConditionId = new FixedString32Bytes(entry.ConditionId),
                    ProgressPct = entry.ProgressPct,
                    IsLeading = entry.IsLeading,
                    TimeRemainingEstimateInWorldDays = float.NaN,
                });
            }
        }

        private static bool TryGetLeaderboardEntries(
            BloodlinesDebugCommandSurface commandSurface,
            out List<Dictionary<string, string>> entries,
            out string readout)
        {
            entries = new List<Dictionary<string, string>>();
            if (!commandSurface.TryDebugGetVictoryLeaderboard(out readout))
            {
                return false;
            }

            var lines = readout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 0 || !string.Equals(parts[0], "VictoryLeaderboard", StringComparison.Ordinal))
                {
                    return false;
                }

                var fields = new Dictionary<string, string>(StringComparer.Ordinal);
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

                entries.Add(fields);
            }

            return entries.Count > 0;
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

                hostObject = new GameObject("BloodlinesVictoryLeaderboardHUDSmokeValidation_CommandSurface")
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
