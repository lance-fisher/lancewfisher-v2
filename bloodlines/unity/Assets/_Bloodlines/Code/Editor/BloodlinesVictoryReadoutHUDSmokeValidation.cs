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
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesVictoryReadoutHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-victory-readout-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Victory Readout HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchVictoryReadoutHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Victory readout HUD smoke validation errored: " + exception;
            }

            var artifact = "BLOODLINES_VICTORY_READOUT_HUD_SMOKE " +
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
            ok &= RunCommandHallPhase(lines);
            ok &= RunTerritorialGovernancePhase(lines);
            ok &= RunDivineRightPhase(lines);
            ok &= RunCompletedVictoryPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunCommandHallPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 40f, 2f);
            SeedVictoryState(entityManager, MatchStatus.Playing, VictoryConditionId.None, default, 0f);
            SeedFactionRoot(entityManager, "player");
            SeedFactionRoot(entityManager, "enemy");
            SeedCommandHall(entityManager, "player");
            SeedCommandHall(entityManager, "enemy");

            TickOnce(world);

            if (!TryGetConditionFields(
                    debugScope.CommandSurface,
                    "player",
                    "CommandHallFall",
                    out var fields,
                    out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read command-hall victory HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "Status") != "Building" ||
                ReadField(fields, "CurrentCount") != "0" ||
                ReadField(fields, "RequiredCount") != "1" ||
                Mathf.Abs(ParseFloat(fields, "Progress01")) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: expected live enemy hall distance readout, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: command-hall victory readout shows 0/1 enemy halls destroyed with zero progress while the rival hall stands.");
            return true;
        }

        private static bool RunTerritorialGovernancePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 120f, 2f);
            SeedVictoryState(
                entityManager,
                MatchStatus.Playing,
                VictoryConditionId.None,
                default,
                60f);
            SeedFactionRoot(entityManager, "player");
            SeedFactionRoot(entityManager, "enemy");
            SeedControlPoint(entityManager, "player_cp_1", "player", 95f);
            SeedControlPoint(entityManager, "player_cp_2", "player", 92f);

            TickOnce(world);

            if (!TryGetConditionFields(
                    debugScope.CommandSurface,
                    "player",
                    "TerritorialGovernance",
                    out var fields,
                    out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read territorial-governance HUD snapshot.");
                return false;
            }

            float progress = ParseFloat(fields, "Progress01");
            float remainingDays = ParseFloat(fields, "TimeRemainingInWorldDays");
            if (ReadField(fields, "Status") != "Ready" ||
                ReadField(fields, "CurrentCount") != "2" ||
                ReadField(fields, "RequiredCount") != "2" ||
                Mathf.Abs(progress - 0.75f) > 0.001f ||
                Mathf.Abs(remainingDays - 120f) > 0.001f)
            {
                lines.AppendLine($"Phase 2 FAIL: expected loyal-march hold telemetry, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: territorial-governance readout shows 2/2 integrated marches, 0.750 progress, and 120 in-world days remaining.");
            return true;
        }

        private static bool RunDivineRightPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 80f, 2f);
            SeedVictoryState(entityManager, MatchStatus.Playing, VictoryConditionId.None, default, 0f);
            var player = SeedFactionRoot(entityManager, "player");
            SeedFactionRoot(entityManager, "enemy");
            entityManager.AddComponentData(player, new FaithStateComponent
            {
                SelectedFaith = CovenantId.BloodDominion,
                DoctrinePath = DoctrinePath.Dark,
                Level = 5,
                Intensity = 60f,
            });

            TickOnce(world);

            if (!TryGetConditionFields(
                    debugScope.CommandSurface,
                    "player",
                    "DivinRight",
                    out var fields,
                    out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read divine-right HUD snapshot.");
                return false;
            }

            float progress = ParseFloat(fields, "Progress01");
            if (ReadField(fields, "Status") != "Building" ||
                ReadField(fields, "CurrentCount") != "5" ||
                ReadField(fields, "RequiredCount") != "5" ||
                Mathf.Abs(progress - 0.875f) > 0.001f)
            {
                lines.AppendLine($"Phase 3 FAIL: expected partial divine-right readiness, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: divine-right readout shows level threshold met, intensity still building, and progress at 0.875.");
            return true;
        }

        private static bool RunCompletedVictoryPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("victory-readout-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 200f, 2f);
            SeedVictoryState(
                entityManager,
                MatchStatus.Won,
                VictoryConditionId.TerritorialGovernance,
                new FixedString32Bytes("player"),
                VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds);
            SeedFactionRoot(entityManager, "player");
            SeedFactionRoot(entityManager, "enemy");
            SeedControlPoint(entityManager, "player_cp_1", "player", 96f);
            SeedControlPoint(entityManager, "player_cp_2", "player", 94f);

            TickOnce(world);

            if (!TryGetConditionFields(
                    debugScope.CommandSurface,
                    "player",
                    "TerritorialGovernance",
                    out var fields,
                    out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read completed victory HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "Status") != "Completed" ||
                ReadField(fields, "IsWinner") != "true" ||
                Mathf.Abs(ParseFloat(fields, "Progress01") - 1f) > 0.001f)
            {
                lines.AppendLine($"Phase 4 FAIL: expected completed territorial-governance victory, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: completed territorial-governance victory surfaces winner state and full progress.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryReadoutSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new Unity.Core.TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays, float daysPerRealSecond)
        {
            var entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = daysPerRealSecond,
                DeclarationCount = 0,
            });
        }

        private static void SeedVictoryState(
            EntityManager entityManager,
            MatchStatus status,
            VictoryConditionId victoryType,
            FixedString32Bytes winnerFactionId,
            float holdSeconds)
        {
            var entity = entityManager.CreateEntity(typeof(VictoryStateComponent));
            entityManager.SetComponentData(entity, new VictoryStateComponent
            {
                Status = status,
                VictoryType = victoryType,
                WinnerFactionId = winnerFactionId,
                VictoryReason = default,
                TerritorialGovernanceHoldSeconds = holdSeconds,
            });
        }

        private static Entity SeedFactionRoot(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            return entity;
        }

        private static void SeedCommandHall(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(BuildingTypeComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = new FixedString64Bytes("command_hall"),
            });
        }

        private static void SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float loyalty)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(ControlPointComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(ownerFactionId),
            });
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = default,
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("march"),
                FortificationTier = 1,
                RadiusTiles = 4f,
                CaptureTimeSeconds = 12f,
            });
        }

        private static bool TryGetConditionFields(
            BloodlinesDebugCommandSurface commandSurface,
            string factionId,
            string conditionId,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetVictoryReadout(factionId, out readout))
            {
                return false;
            }

            var lines = readout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (!line.StartsWith("VictoryReadout|", StringComparison.Ordinal) ||
                    !line.Contains("|ConditionId=" + conditionId + "|", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (string part in line.Split('|'))
                {
                    int equalsIndex = part.IndexOf('=');
                    if (equalsIndex <= 0)
                    {
                        continue;
                    }

                    fields[part.Substring(0, equalsIndex)] = part.Substring(equalsIndex + 1);
                }

                return true;
            }

            return false;
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

                hostObject = new GameObject("BloodlinesVictoryReadoutHUDSmokeValidation_CommandSurface")
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
