#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.HUD;
using Bloodlines.Victory;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the victory-distance HUD readout slice.
    /// Proves governance progress, divine-right progress, command-hall fall
    /// completion, and per-condition leading flags through the public debug seam.
    /// </summary>
    public static class BloodlinesVictoryReadoutSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-victory-readout-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Victory Readout Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchVictoryReadoutSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Victory readout smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_VICTORY_READOUT_SMOKE " +
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
            ok &= RunGovernanceProgressPhase(lines);
            ok &= RunDivineRightProgressPhase(lines);
            ok &= RunCommandHallFallCompletionPhase(lines);
            ok &= RunLeadingFlagPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunGovernanceProgressPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedVictoryState(entityManager);
            SeedDualClock(entityManager, inWorldDays: 24f, daysPerRealSecond: 2f);
            SeedFaction(entityManager, "player");
            SeedFaction(entityManager, "enemy");
            SeedControlPoint(entityManager, "cp-1", "player", loyalty: 95f);
            SeedControlPoint(entityManager, "cp-2", "player", loyalty: 92f);
            SeedControlPoint(entityManager, "cp-3", "player", loyalty: 48f);

            TickOnce(world);

            if (!TryGetConditionFields(debugScope.CommandSurface, "player", "TerritorialGovernance", out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read player TerritorialGovernance victory readout.");
                return false;
            }

            float progressPct = ParseFloat(fields, "ProgressPct");
            if (progressPct <= 0f || progressPct >= 1f)
            {
                lines.AppendLine($"Phase 1 FAIL: expected partial TerritorialGovernance progress, got '{readout}'.");
                return false;
            }

            if (ReadField(fields, "FactionId") != "player")
            {
                lines.AppendLine($"Phase 1 FAIL: expected player faction readout, got '{readout}'.");
                return false;
            }

            lines.AppendLine($"Phase 1 PASS: player TerritorialGovernance ProgressPct={progressPct.ToString("0.000", CultureInfo.InvariantCulture)} with 2 of 3 loyal claims.");
            return true;
        }

        private static bool RunDivineRightProgressPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedVictoryState(entityManager);
            SeedDualClock(entityManager, inWorldDays: 40f, daysPerRealSecond: 2f);
            SeedFaction(entityManager, "player", faithIntensity: 64f, faithLevel: 2);
            SeedFaction(entityManager, "enemy");

            TickOnce(world);

            if (!TryGetConditionFields(debugScope.CommandSurface, "player", "DivineRight", out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read player DivineRight victory readout.");
                return false;
            }

            float progressPct = ParseFloat(fields, "ProgressPct");
            if (progressPct <= 0f)
            {
                lines.AppendLine($"Phase 2 FAIL: expected positive DivineRight progress, got '{readout}'.");
                return false;
            }

            lines.AppendLine($"Phase 2 PASS: player DivineRight ProgressPct={progressPct.ToString("0.000", CultureInfo.InvariantCulture)} from high faith intensity.");
            return true;
        }

        private static bool RunCommandHallFallCompletionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedVictoryState(entityManager);
            SeedDualClock(entityManager, inWorldDays: 60f, daysPerRealSecond: 2f);
            SeedFaction(entityManager, "player");
            SeedFaction(entityManager, "enemy");
            SeedCommandHall(entityManager, "player", dead: false);
            SeedCommandHall(entityManager, "enemy", dead: true);

            TickOnce(world);

            if (!TryGetConditionFields(debugScope.CommandSurface, "player", "CommandHallFall", out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read player CommandHallFall victory readout.");
                return false;
            }

            float progressPct = ParseFloat(fields, "ProgressPct");
            if (Mathf.Abs(progressPct - 1f) > 0.001f)
            {
                lines.AppendLine($"Phase 3 FAIL: expected CommandHallFall ProgressPct=1.000, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: enemy command-hall destruction surfaces CommandHallFall ProgressPct=1.000 for the player.");
            return true;
        }

        private static bool RunLeadingFlagPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedVictoryState(entityManager);
            SeedDualClock(entityManager, inWorldDays: 80f, daysPerRealSecond: 2f);
            SeedFaction(entityManager, "player", faithIntensity: 20f, faithLevel: 1);
            SeedFaction(entityManager, "enemy", faithIntensity: 70f, faithLevel: 4);
            SeedCommandHall(entityManager, "player", dead: false);
            SeedCommandHall(entityManager, "enemy", dead: false);

            TickOnce(world);

            if (!TryGetConditionFields(debugScope.CommandSurface, "enemy", "DivineRight", out var enemyFields, out var enemyReadout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read enemy DivineRight victory readout.");
                return false;
            }

            if (!TryGetConditionFields(debugScope.CommandSurface, "player", "DivineRight", out var playerFields, out var playerReadout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read player DivineRight victory readout.");
                return false;
            }

            if (ReadField(enemyFields, "IsLeading") != "true" ||
                ReadField(playerFields, "IsLeading") != "false")
            {
                lines.AppendLine($"Phase 4 FAIL: expected enemy to lead DivineRight. Enemy='{enemyReadout}' Player='{playerReadout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: highest DivineRight progress surfaces IsLeading=true for the enemy and false for the trailing player.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionReadoutSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedVictoryState(EntityManager entityManager)
        {
            var victoryEntity = entityManager.CreateEntity(typeof(VictoryStateComponent));
            entityManager.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
                WinnerFactionId = default,
                VictoryReason = default,
                TerritorialGovernanceHoldSeconds = 0f,
            });
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays, float daysPerRealSecond)
        {
            var dualClockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(dualClockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = daysPerRealSecond,
                DeclarationCount = 0,
            });
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float faithIntensity = 0f,
            int faithLevel = 0)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FaithStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = faithIntensity,
                Level = faithLevel,
            });
            return entity;
        }

        private static void SeedControlPoint(EntityManager entityManager, string controlPointId, string ownerFactionId, float loyalty)
        {
            var entity = entityManager.CreateEntity(typeof(ControlPointComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                Loyalty = loyalty,
            });
        }

        private static void SeedCommandHall(EntityManager entityManager, string factionId, bool dead)
        {
            var entity = entityManager.CreateEntity(
                typeof(BuildingTypeComponent),
                typeof(FactionComponent));
            entityManager.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = new FixedString64Bytes("command_hall"),
            });
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });

            if (dead)
            {
                entityManager.AddComponent<DeadTag>(entity);
            }
        }

        private static bool TryGetConditionFields(
            BloodlinesDebugCommandSurface commandSurface,
            string factionId,
            string conditionId,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            readout = string.Empty;

            if (!commandSurface.TryDebugGetVictoryReadout(factionId, out readout))
            {
                return false;
            }

            string[] lines = readout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 0 || !string.Equals(parts[0], "VictoryReadout", StringComparison.Ordinal))
                {
                    continue;
                }

                var candidateFields = new Dictionary<string, string>(StringComparer.Ordinal);
                for (int i = 1; i < parts.Length; i++)
                {
                    int equalsIndex = parts[i].IndexOf('=');
                    if (equalsIndex <= 0)
                    {
                        continue;
                    }

                    string key = parts[i].Substring(0, equalsIndex);
                    string value = parts[i].Substring(equalsIndex + 1);
                    candidateFields[key] = value;
                }

                if (candidateFields.TryGetValue("ConditionId", out var candidateConditionId) &&
                    string.Equals(candidateConditionId, conditionId, StringComparison.Ordinal))
                {
                    fields = candidateFields;
                    return true;
                }
            }

            return false;
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

                hostObject = new GameObject("BloodlinesVictoryReadoutSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave,
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
