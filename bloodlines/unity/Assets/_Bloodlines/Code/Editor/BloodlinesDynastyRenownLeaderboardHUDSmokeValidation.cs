#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the dynasty renown leaderboard HUD panel.
    /// Proves row population, human-player projection, and prestige ordering.
    /// </summary>
    public static class BloodlinesDynastyRenownLeaderboardHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-dynasty-renown-leaderboard-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Dynasty Renown Leaderboard HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDynastyRenownLeaderboardHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Dynasty renown leaderboard HUD smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_DYNASTY_RENOWN_LEADERBOARD_HUD_SMOKE " +
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
            ok &= RunPopulationPhase(lines);
            ok &= RunHumanFlagPhase(lines);
            ok &= RunOrderingPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunPopulationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-leaderboard-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 10f);
            SeedFaction(entityManager, "player", rank: 2, score: 24f, peak: 30f, leading: false, interregnum: false, rulerId: "player-head", rulerTitle: "High King", band: "rising", status: "stable");
            SeedFaction(entityManager, "enemy", rank: 1, score: 38f, peak: 42f, leading: true, interregnum: false, rulerId: "enemy-head", rulerTitle: "Enemy Regent", band: "ascendant", status: "stable");

            TickOnce(world);

            if (!TryGetEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read dynasty renown leaderboard.");
                return false;
            }

            if (entries.Count != 2 ||
                ReadField(entries[0], "FactionId") != "enemy" ||
                ReadField(entries[0], "Rank") != "1" ||
                Math.Abs(ParseFloat(entries[0], "Score") - 38f) > 0.001f ||
                ReadField(entries[0], "BandLabel") != "ascendant" ||
                ReadField(entries[1], "FactionId") != "player" ||
                ReadField(entries[1], "Rank") != "2" ||
                Math.Abs(ParseFloat(entries[1], "Score") - 24f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected leaderboard population '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: leaderboard mirrors dynasty renown rows with enemy(38) ahead of player(24).");
            return true;
        }

        private static bool RunHumanFlagPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-leaderboard-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 16f);
            SeedFaction(entityManager, "player", rank: 1, score: 29f, peak: 34f, leading: true, interregnum: false, rulerId: "player-head", rulerTitle: "High King", band: "ascendant", status: "stable");
            SeedFaction(entityManager, "rival", rank: 2, score: 18f, peak: 22f, leading: false, interregnum: true, rulerId: string.Empty, rulerTitle: string.Empty, band: "rising", status: "interregnum");

            TickOnce(world);

            if (!TryGetEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read dynasty renown leaderboard for human flag test.");
                return false;
            }

            if (entries.Count != 2 ||
                ReadField(entries[0], "FactionId") != "player" ||
                ReadField(entries[0], "IsHumanPlayer") != "true" ||
                ReadField(entries[1], "FactionId") != "rival" ||
                ReadField(entries[1], "IsHumanPlayer") != "false" ||
                ReadField(entries[1], "Interregnum") != "true" ||
                ReadField(entries[1], "StatusLabel") != "interregnum")
            {
                lines.AppendLine($"Phase 2 FAIL: unexpected human/interregnum flags '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: leaderboard marks the player row as human and preserves rival interregnum state.");
            return true;
        }

        private static bool RunOrderingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-leaderboard-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 22f);
            SeedFaction(entityManager, "player", rank: 2, score: 31f, peak: 36f, leading: false, interregnum: false, rulerId: "player-head", rulerTitle: "High King", band: "ascendant", status: "stable");
            SeedFaction(entityManager, "enemy", rank: 1, score: 31f, peak: 40f, leading: true, interregnum: false, rulerId: "enemy-head", rulerTitle: "Enemy Regent", band: "ascendant", status: "stable");
            SeedFaction(entityManager, "ally", rank: 3, score: 18f, peak: 24f, leading: false, interregnum: false, rulerId: "ally-head", rulerTitle: "Allied Marshal", band: "rising", status: "stable");

            TickOnce(world);

            if (!TryGetEntries(debugScope.CommandSurface, out var entries, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read dynasty renown leaderboard ordering.");
                return false;
            }

            if (entries.Count != 3 ||
                ReadField(entries[0], "FactionId") != "enemy" ||
                ReadField(entries[1], "FactionId") != "player" ||
                ReadField(entries[2], "FactionId") != "ally" ||
                ReadField(entries[0], "IsLeadingDynasty") != "true")
            {
                lines.AppendLine($"Phase 3 FAIL: expected peak-aware ordering, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: leaderboard sorts by score, then peak renown, with the leading dynasty first.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyRenownLeaderboardHUDSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new Unity.Core.TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                DaysPerRealSecond = 2f,
                InWorldDays = inWorldDays,
            });
        }

        private static void SeedFaction(
            EntityManager entityManager,
            string factionId,
            int rank,
            float score,
            float peak,
            bool leading,
            bool interregnum,
            string rulerId,
            string rulerTitle,
            string band,
            string status)
        {
            Entity entity = entityManager.CreateEntity(typeof(FactionComponent), typeof(DynastyRenownHUDComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new DynastyRenownHUDComponent
            {
                FactionId = new FixedString32Bytes(factionId),
                RenownScore = score,
                PeakRenown = peak,
                ScoreToPeakRatio = peak > 0f ? score / peak : 0f,
                RenownRank = rank,
                IsLeadingDynasty = leading,
                RulerMemberId = new FixedString64Bytes(rulerId),
                RulerTitle = new FixedString64Bytes(rulerTitle),
                Legitimacy = 0f,
                Interregnum = interregnum,
                StatusLabel = new FixedString32Bytes(status),
                RenownBandLabel = new FixedString32Bytes(band),
                RenownBandColor = default,
            });
        }

        private static bool TryGetEntries(
            BloodlinesDebugCommandSurface commandSurface,
            out List<Dictionary<string, string>> entries,
            out string readout)
        {
            entries = new List<Dictionary<string, string>>();
            if (!commandSurface.TryDebugGetDynastyRenownLeaderboard(out readout))
            {
                return false;
            }

            string[] lines = readout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 0 || !string.Equals(parts[0], "DynastyRenownLeaderboard", StringComparison.Ordinal))
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

                    string key = parts[i].Substring(0, equalsIndex);
                    string value = parts[i].Substring(equalsIndex + 1);
                    fields[key] = value;
                }

                entries.Add(fields);
            }

            return entries.Count > 0;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out string value))
            {
                throw new InvalidOperationException("Missing field: " + key);
            }

            return value;
        }

        private static float ParseFloat(Dictionary<string, string> fields, string key)
        {
            string value = ReadField(fields, key);
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
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

                hostObject = new GameObject("BloodlinesDynastyRenownLeaderboardHUDSmokeValidation_CommandSurface")
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
