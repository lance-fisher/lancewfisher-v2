#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
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
    /// Dedicated validator for the dynasty renown HUD read-model.
    /// Proves score mirroring, ranking, and succession-status projection.
    /// </summary>
    public static class BloodlinesDynastyRenownHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-dynasty-renown-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Dynasty Renown HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDynastyRenownHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Dynasty renown HUD smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_DYNASTY_RENOWN_HUD_SMOKE " +
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
            ok &= RunMirrorPhase(lines);
            ok &= RunRankingPhase(lines);
            ok &= RunInterregnumPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunMirrorPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 12f);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 64f, interregnum: false);
            SeedRulingMember(entityManager, player, "player-head", "High King");
            SeedDynastyRenown(entityManager, player, 28f, 35f);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 58f, interregnum: false);
            SeedRulingMember(entityManager, enemy, "enemy-head", "Enemy Regent");
            SeedDynastyRenown(entityManager, enemy, 12f, 16f);

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read player dynasty renown HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "RulerMemberId") != "player-head" ||
                ReadField(fields, "RulerTitle") != "High King" ||
                ReadField(fields, "BandLabel") != "ascendant" ||
                ReadField(fields, "BandColor") != "green" ||
                ReadField(fields, "StatusLabel") != "stable" ||
                Math.Abs(ParseFloat(fields, "Score") - 28f) > 0.001f ||
                Math.Abs(ParseFloat(fields, "PeakRenown") - 35f) > 0.001f ||
                Math.Abs(ParseFloat(fields, "ScoreToPeakRatio") - 0.8f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected player HUD snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: renown HUD mirrors score, ruler identity, and ascendant band state.");
            return true;
        }

        private static bool RunRankingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 18f);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 52f, interregnum: false);
            SeedDynastyRenown(entityManager, player, 12f, 18f);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 68f, interregnum: false);
            SeedDynastyRenown(entityManager, enemy, 47f, 50f);
            Entity rival = SeedFaction(entityManager, "rival", legitimacy: 60f, interregnum: false);
            SeedDynastyRenown(entityManager, rival, 29f, 31f);

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, "enemy", out var enemyFields, out var enemyReadout) ||
                !TryReadSnapshot(debugScope.CommandSurface, "rival", out var rivalFields, out var rivalReadout) ||
                !TryReadSnapshot(debugScope.CommandSurface, "player", out var playerFields, out var playerReadout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read one or more renown HUD snapshots.");
                return false;
            }

            if (ReadField(enemyFields, "Rank") != "1" ||
                ReadField(enemyFields, "IsLeadingDynasty") != "true" ||
                ReadField(rivalFields, "Rank") != "2" ||
                ReadField(playerFields, "Rank") != "3")
            {
                lines.AppendLine(
                    "Phase 2 FAIL: unexpected ranking. " +
                    $"enemy='{enemyReadout}' rival='{rivalReadout}' player='{playerReadout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: dynasty renown ranking orders enemy(1), rival(2), player(3).");
            return true;
        }

        private static bool RunInterregnumPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 41f, interregnum: true);
            SeedDynastyRenown(entityManager, player, 7f, 14f);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 66f, interregnum: false);
            SeedDynastyRenown(entityManager, enemy, 21f, 24f);

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read interregnum dynasty renown HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "Interregnum") != "true" ||
                ReadField(fields, "StatusLabel") != "interregnum" ||
                ReadField(fields, "RulerMemberId").Length != 0 ||
                ReadField(fields, "BandLabel") != "obscure")
            {
                lines.AppendLine($"Phase 3 FAIL: unexpected interregnum snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: interregnum state surfaces with no ruler identity and the expected low-renown band.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyRenownHUDSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new Unity.Core.TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                DaysPerRealSecond = 2f,
                InWorldDays = inWorldDays,
            });
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float legitimacy,
            bool interregnum)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(DynastyStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = legitimacy,
                LoyaltyPressure = 0f,
                Interregnum = interregnum,
            });
            entityManager.AddBuffer<DynastyMemberRef>(entity);
            return entity;
        }

        private static void SeedRulingMember(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            string title)
        {
            Entity memberEntity = entityManager.CreateEntity(typeof(DynastyMemberComponent));
            entityManager.SetComponentData(memberEntity, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes(memberId),
                Title = new FixedString64Bytes(title),
                Role = DynastyRole.HeadOfBloodline,
                Path = DynastyPath.Governance,
                AgeYears = 38f,
                Status = DynastyMemberStatus.Ruling,
                Renown = 24f,
                Order = 0,
                FallenAtWorldSeconds = 0f,
            });

            DynamicBuffer<DynastyMemberRef> roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            roster.Add(new DynastyMemberRef
            {
                Member = memberEntity,
            });
        }

        private static void SeedDynastyRenown(
            EntityManager entityManager,
            Entity factionEntity,
            float score,
            float peak)
        {
            entityManager.AddComponentData(factionEntity, new DynastyRenownComponent
            {
                RenownScore = score,
                LastRenownUpdateInWorldDays = 0f,
                RenownDecayRate = 0.45f,
                PeakRenown = peak,
                LastRulingMemberId = default,
                Initialized = 1,
            });
        }

        private static bool TryReadSnapshot(
            BloodlinesDebugCommandSurface commandSurface,
            string factionId,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetDynastyRenownHUDSnapshot(factionId, out readout))
            {
                return false;
            }

            var parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "DynastyRenownHUD", StringComparison.Ordinal))
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

                string key = parts[i].Substring(0, equalsIndex);
                string value = parts[i].Substring(equalsIndex + 1);
                fields[key] = value;
            }

            return true;
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
            string value = ReadField(fields, key);
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

                hostObject = new GameObject("BloodlinesDynastyRenownHUDSmokeValidation_CommandSurface")
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
