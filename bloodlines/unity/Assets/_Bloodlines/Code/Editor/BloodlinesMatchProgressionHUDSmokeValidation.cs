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
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the second player-HUD slice.
    /// Proves the singleton match HUD read-model through four phases:
    /// 1. baseline stage/phase/day projection
    /// 2. non-baseline stage readiness and declaration projection
    /// 3. dominant leader world-pressure projection
    /// 4. Great Reckoning projection
    /// </summary>
    public static class BloodlinesMatchProgressionHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-match-progression-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Match Progression HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchMatchProgressionHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Match progression HUD smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_MATCH_PROGRESSION_HUD_SMOKE " +
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
            ok &= RunBaselinePhase(lines);
            ok &= RunStageProjectionPhase(lines);
            ok &= RunDominantPressurePhase(lines);
            ok &= RunGreatReckoningPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("match-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedMatchProgression(
                world.EntityManager,
                stageNumber: 1,
                stageId: "founding",
                stageLabel: "Founding",
                phaseId: "emergence",
                phaseLabel: "Emergence",
                stageReadiness: 0.250f,
                nextStageId: "expansion_identity",
                nextStageLabel: "Expansion and Identity",
                inWorldDays: 12f,
                inWorldYears: 0.03f,
                declarationCount: 0);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read match HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "StageNumber") != "1" ||
                ReadField(fields, "StageId") != "founding" ||
                ReadField(fields, "PhaseId") != "emergence" ||
                ReadField(fields, "WorldPressureLevel") != "0" ||
                ReadField(fields, "WorldPressureLabel") != "quiet" ||
                ReadField(fields, "GreatReckoningActive") != "false")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected baseline snapshot '{readout}'.");
                return false;
            }

            float inWorldDays = ParseFloat(fields, "InWorldDays");
            if (Mathf.Abs(inWorldDays - 12f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: expected InWorldDays=12.0, got {inWorldDays.ToString("0.0", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: founding stage surfaces emergence phase, quiet world pressure, and InWorldDays=12.0.");
            return true;
        }

        private static bool RunStageProjectionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("match-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            SeedMatchProgression(
                world.EntityManager,
                stageNumber: 4,
                stageId: "war_turning_of_tides",
                stageLabel: "War and Turning of the Tides",
                phaseId: "commitment",
                phaseLabel: "Commitment",
                stageReadiness: 0.660f,
                nextStageId: "final_convergence",
                nextStageLabel: "Final Convergence",
                inWorldDays: 480f,
                inWorldYears: 1.32f,
                declarationCount: 2);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read stage-projection snapshot.");
                return false;
            }

            if (ReadField(fields, "StageNumber") != "4" ||
                ReadField(fields, "StageId") != "war_turning_of_tides" ||
                ReadField(fields, "PhaseLabel") != "Commitment" ||
                ReadField(fields, "NextStageId") != "final_convergence" ||
                ReadField(fields, "DeclarationCount") != "2")
            {
                lines.AppendLine($"Phase 2 FAIL: expected stage-4 projection, got '{readout}'.");
                return false;
            }

            float readiness = ParseFloat(fields, "StageReadiness");
            if (Mathf.Abs(readiness - 0.66f) > 0.001f)
            {
                lines.AppendLine($"Phase 2 FAIL: expected StageReadiness=0.660, got {readiness.ToString("0.000", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: stage-4 commitment snapshot surfaces readiness, next-stage label, and declaration count.");
            return true;
        }

        private static bool RunDominantPressurePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("match-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedMatchProgression(
                entityManager,
                stageNumber: 5,
                stageId: "final_convergence",
                stageLabel: "Final Convergence",
                phaseId: "resolution",
                phaseLabel: "Resolution",
                stageReadiness: 1.000f,
                nextStageId: string.Empty,
                nextStageLabel: string.Empty,
                inWorldDays: 610f,
                inWorldYears: 1.67f,
                declarationCount: 3,
                dominantLeaderFactionId: "player",
                dominantLeaderTerritoryShare: 0.580f);
            SeedWorldPressure(entityManager, "player", level: 2, label: "overwhelming", score: 6, targeted: true);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read dominant-pressure snapshot.");
                return false;
            }

            if (ReadField(fields, "DominantLeaderFactionId") != "player" ||
                ReadField(fields, "WorldPressureLevel") != "2" ||
                ReadField(fields, "WorldPressureLabel") != "overwhelming" ||
                ReadField(fields, "WorldPressureScore") != "6" ||
                ReadField(fields, "WorldPressureTargeted") != "true")
            {
                lines.AppendLine($"Phase 3 FAIL: expected dominant pressure projection, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: dominant leader `player` surfaces overwhelming world pressure level 2 and score 6.");
            return true;
        }

        private static bool RunGreatReckoningPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("match-hud-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedMatchProgression(
                entityManager,
                stageNumber: 5,
                stageId: "final_convergence",
                stageLabel: "Final Convergence",
                phaseId: "resolution",
                phaseLabel: "Resolution",
                stageReadiness: 0.930f,
                nextStageId: string.Empty,
                nextStageLabel: string.Empty,
                inWorldDays: 720f,
                inWorldYears: 1.97f,
                declarationCount: 4,
                dominantLeaderFactionId: "enemy",
                dominantLeaderTerritoryShare: 0.720f,
                greatReckoningActive: true,
                greatReckoningTargetFactionId: "enemy",
                greatReckoningShare: 0.720f);
            SeedWorldPressure(entityManager, "enemy", level: 3, label: "convergence", score: 8, targeted: true);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read Great Reckoning snapshot.");
                return false;
            }

            if (ReadField(fields, "GreatReckoningActive") != "true" ||
                ReadField(fields, "GreatReckoningTargetFactionId") != "enemy" ||
                ReadField(fields, "WorldPressureLevel") != "3" ||
                ReadField(fields, "WorldPressureLabel") != "convergence")
            {
                lines.AppendLine($"Phase 4 FAIL: expected Great Reckoning projection, got '{readout}'.");
                return false;
            }

            float share = ParseFloat(fields, "GreatReckoningShare");
            if (Mathf.Abs(share - 0.72f) > 0.001f)
            {
                lines.AppendLine($"Phase 4 FAIL: expected GreatReckoningShare=0.720, got {share.ToString("0.000", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: Great Reckoning target `enemy` surfaces convergence pressure level 3 at share 0.720.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MatchProgressionHUDSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedMatchProgression(
            EntityManager entityManager,
            int stageNumber,
            string stageId,
            string stageLabel,
            string phaseId,
            string phaseLabel,
            float stageReadiness,
            string nextStageId,
            string nextStageLabel,
            float inWorldDays,
            float inWorldYears,
            int declarationCount,
            string dominantLeaderFactionId = "",
            float dominantLeaderTerritoryShare = 0f,
            bool greatReckoningActive = false,
            string greatReckoningTargetFactionId = "",
            float greatReckoningShare = 0f)
        {
            var entity = entityManager.CreateEntity(typeof(MatchProgressionComponent));
            entityManager.SetComponentData(entity, new MatchProgressionComponent
            {
                StageNumber = stageNumber,
                StageId = new FixedString32Bytes(stageId),
                StageLabel = new FixedString64Bytes(stageLabel),
                PhaseId = new FixedString32Bytes(phaseId),
                PhaseLabel = new FixedString32Bytes(phaseLabel),
                StageReadiness = stageReadiness,
                NextStageId = new FixedString32Bytes(nextStageId),
                NextStageLabel = new FixedString64Bytes(nextStageLabel),
                InWorldDays = inWorldDays,
                InWorldYears = inWorldYears,
                DeclarationCount = declarationCount,
                GreatReckoningActive = greatReckoningActive,
                GreatReckoningTargetFactionId = new FixedString32Bytes(greatReckoningTargetFactionId),
                GreatReckoningShare = greatReckoningShare,
                GreatReckoningThreshold = 0.7f,
                DominantKingdomId = new FixedString32Bytes(dominantLeaderFactionId),
                DominantTerritoryShare = dominantLeaderTerritoryShare,
            });
        }

        private static void SeedWorldPressure(
            EntityManager entityManager,
            string factionId,
            int level,
            string label,
            int score,
            bool targeted)
        {
            var entity = entityManager.CreateEntity(typeof(FactionComponent), typeof(WorldPressureComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new WorldPressureComponent
            {
                Level = level,
                Label = new FixedString32Bytes(label),
                Score = score,
                Targeted = targeted,
            });
        }

        private static bool TryGetSnapshotFields(
            BloodlinesDebugCommandSurface commandSurface,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetMatchHUDSnapshot(out readout))
            {
                return false;
            }

            var parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "MatchHUD", StringComparison.Ordinal))
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

                var key = parts[i].Substring(0, equalsIndex);
                var value = parts[i].Substring(equalsIndex + 1);
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

                hostObject = new GameObject("BloodlinesMatchProgressionHUDSmokeValidation_CommandSurface")
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
