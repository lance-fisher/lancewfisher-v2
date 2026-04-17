#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed conviction-scoring smoke validator. Runs in an isolated ECS world
    /// so it neither touches the Bootstrap scene nor conflicts with the
    /// combat/graphics smokes. Proves four things end-to-end:
    ///
    ///   1. A neutral faction with zero buckets resolves to the Neutral band.
    ///   2. A stewardship-heavy ledger rises through Moral into Apex Moral.
    ///   3. A ruthlessness+desecration ledger sinks through Cruel into Apex Cruel.
    ///   4. Band effect multipliers match the canonical browser table.
    ///
    /// Artifact: artifacts/unity-conviction-smoke.log.
    /// </summary>
    public static class BloodlinesConvictionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-conviction-smoke.log";

        [MenuItem("Bloodlines/Conviction/Run Conviction Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchConvictionSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

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
                message = "Conviction smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunNeutralBaselinePhase(out string neutralMessage))
            {
                message = neutralMessage;
                return false;
            }

            if (!RunMoralAscentPhase(out string moralMessage))
            {
                message = moralMessage;
                return false;
            }

            if (!RunCruelDescentPhase(out string cruelMessage))
            {
                message = cruelMessage;
                return false;
            }

            if (!RunBandEffectsPhase(out string effectsMessage))
            {
                message = effectsMessage;
                return false;
            }

            message =
                "Conviction smoke validation passed: neutralPhase=True, moralAscentPhase=True, cruelDescentPhase=True, bandEffectsPhase=True. " +
                neutralMessage + " " + moralMessage + " " + cruelMessage + " " + effectsMessage;
            return true;
        }

        private static bool RunNeutralBaselinePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Neutral");
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            TickOnce(world);

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (conviction.Band != ConvictionBand.Neutral || conviction.Score != 0f)
            {
                message =
                    "Conviction smoke validation failed: neutral baseline phase expected Neutral band with score 0. " +
                    "actualBand=" + conviction.Band + ", actualScore=" + conviction.Score + ".";
                return false;
            }

            message = "Neutral baseline: band=" + conviction.Band + ", score=" + conviction.Score + ".";
            return true;
        }

        private static bool RunMoralAscentPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Moral");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            // 50 stewardship crosses into Moral (>= 25) but not Apex Moral.
            if (!commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Stewardship, 50f))
            {
                message = "Conviction smoke validation failed: moral ascent phase could not record stewardship event.";
                return false;
            }

            var moralState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (moralState.Band != ConvictionBand.Moral || moralState.Score != 50f)
            {
                message =
                    "Conviction smoke validation failed: moral ascent phase expected Moral at 50. " +
                    "actualBand=" + moralState.Band + ", actualScore=" + moralState.Score + ".";
                return false;
            }

            // +40 oathkeeping pushes score to 90, into Apex Moral (>= 75).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Oathkeeping, 40f);
            var apexState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (apexState.Band != ConvictionBand.ApexMoral || apexState.Score != 90f)
            {
                message =
                    "Conviction smoke validation failed: moral ascent phase expected Apex Moral at 90. " +
                    "actualBand=" + apexState.Band + ", actualScore=" + apexState.Score + ".";
                return false;
            }

            message =
                "Moral ascent: moralBand=" + moralState.Band + "@" + moralState.Score +
                ", apexBand=" + apexState.Band + "@" + apexState.Score + ".";
            return true;
        }

        private static bool RunCruelDescentPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Cruel");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            // -30 ruthlessness sinks score to -30, Cruel (>= -74).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Ruthlessness, 30f);
            var cruelState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (cruelState.Band != ConvictionBand.Cruel || cruelState.Score != -30f)
            {
                message =
                    "Conviction smoke validation failed: cruel descent phase expected Cruel at -30. " +
                    "actualBand=" + cruelState.Band + ", actualScore=" + cruelState.Score + ".";
                return false;
            }

            // +100 desecration drops score to -130, Apex Cruel (< -74).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Desecration, 100f);
            var apexState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (apexState.Band != ConvictionBand.ApexCruel || apexState.Score != -130f)
            {
                message =
                    "Conviction smoke validation failed: cruel descent phase expected Apex Cruel at -130. " +
                    "actualBand=" + apexState.Band + ", actualScore=" + apexState.Score + ".";
                return false;
            }

            message =
                "Cruel descent: cruelBand=" + cruelState.Band + "@" + cruelState.Score +
                ", apexBand=" + apexState.Band + "@" + apexState.Score + ".";
            return true;
        }

        private static bool RunBandEffectsPhase(out string message)
        {
            var apex = ConvictionBandEffects.ForBand(ConvictionBand.ApexMoral);
            if (apex.StabilizationMultiplier != 1.22f ||
                apex.LoyaltyProtectionMultiplier != 1.18f ||
                apex.CaptureMultiplier != 0.94f)
            {
                message =
                    "Conviction smoke validation failed: apex moral band effects drifted from canonical table. " +
                    "stabilization=" + apex.StabilizationMultiplier +
                    ", loyaltyProtection=" + apex.LoyaltyProtectionMultiplier +
                    ", capture=" + apex.CaptureMultiplier + ".";
                return false;
            }

            var apexCruel = ConvictionBandEffects.ForBand(ConvictionBand.ApexCruel);
            if (apexCruel.StabilizationMultiplier != 0.88f ||
                apexCruel.CaptureMultiplier != 1.22f ||
                apexCruel.AttackMultiplier != 1.12f)
            {
                message =
                    "Conviction smoke validation failed: apex cruel band effects drifted from canonical table. " +
                    "stabilization=" + apexCruel.StabilizationMultiplier +
                    ", capture=" + apexCruel.CaptureMultiplier +
                    ", attack=" + apexCruel.AttackMultiplier + ".";
                return false;
            }

            var neutral = ConvictionBandEffects.ForBand(ConvictionBand.Neutral);
            if (neutral.StabilizationMultiplier != 1f ||
                neutral.CaptureMultiplier != 1f)
            {
                message = "Conviction smoke validation failed: neutral band effects should all be 1. ";
                return false;
            }

            message =
                "Band effects: apexMoral.stabilization=" + apex.StabilizationMultiplier +
                ", apexCruel.capture=" + apexCruel.CaptureMultiplier +
                ", neutral.stabilization=" + neutral.StabilizationMultiplier + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ConvictionScoringSystem>());
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity CreateFaction(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new ConvictionComponent
            {
                Band = ConvictionBand.Neutral,
            });
            return entity;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }

    internal sealed class DebugCommandSurfaceScope : IDisposable
    {
        private readonly World previousDefaultWorld;
        private readonly GameObject hostObject;

        public BloodlinesDebugCommandSurface CommandSurface { get; }

        public DebugCommandSurfaceScope(World world)
        {
            previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;

            hostObject = new GameObject("BloodlinesConvictionSmokeValidation_CommandSurface")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
        }

        public void Dispose()
        {
            if (hostObject != null)
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
            }

            World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
        }
    }
}
#endif
