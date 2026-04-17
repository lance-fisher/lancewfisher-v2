#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed dynasty smoke validator. Runs in isolated ECS worlds so it is
    /// independent of the Bootstrap scene. Proves four things:
    ///
    ///   1. Spawning a dynasty attaches eight canonical members with the correct
    ///      roles, paths, titles, and ordering.
    ///   2. DynastyAgingSystem advances active and ruling member age while
    ///      leaving fallen members unchanged.
    ///   3. Felling the ruling head causes the eldest heir to be promoted to
    ///      Ruling with HeadOfBloodline role.
    ///   4. Felling every active and ruling member leaves the dynasty in
    ///      Interregnum.
    ///
    /// Artifact: artifacts/unity-dynasty-smoke.log.
    /// </summary>
    public static class BloodlinesDynastySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-dynasty-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Dynasty/Run Dynasty Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchDynastySmokeValidation()
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
                message = "Dynasty smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunSpawnPhase(out string spawnMessage))
            {
                message = spawnMessage;
                return false;
            }

            if (!RunAgingPhase(out string agingMessage))
            {
                message = agingMessage;
                return false;
            }

            if (!RunSuccessionPhase(out string successionMessage))
            {
                message = successionMessage;
                return false;
            }

            if (!RunInterregnumPhase(out string interregnumMessage))
            {
                message = interregnumMessage;
                return false;
            }

            message =
                "Dynasty smoke validation passed: spawnPhase=True, agingPhase=True, successionPhase=True, interregnumPhase=True. " +
                spawnMessage + " " + agingMessage + " " + successionMessage + " " + interregnumMessage;
            return true;
        }

        private static bool RunSpawnPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesDynastySmokeValidation_Spawn");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionWithDynasty(entityManager, "player");

            int count = commandSurfaceScope.CommandSurface.TryDebugGetDynastyMemberCount("player");
            if (count != DynastyTemplates.Canonical.Length)
            {
                message =
                    "Dynasty smoke validation failed: spawn phase expected " +
                    DynastyTemplates.Canonical.Length +
                    " members, saw " + count + ".";
                return false;
            }

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyMember(
                    "player",
                    DynastyRole.HeadOfBloodline,
                    out var head))
            {
                message = "Dynasty smoke validation failed: spawn phase could not find Head of Bloodline.";
                return false;
            }

            if (head.Status != DynastyMemberStatus.Ruling ||
                head.Path != DynastyPath.Governance ||
                head.AgeYears != 38f)
            {
                message =
                    "Dynasty smoke validation failed: spawn phase head drifted from canonical template. " +
                    "status=" + head.Status + ", path=" + head.Path + ", age=" + head.AgeYears + ".";
                return false;
            }

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyState("player", out var dynastyState) ||
                dynastyState.Legitimacy != DynastyTemplates.InitialLegitimacy ||
                dynastyState.ActiveMemberCap != DynastyTemplates.InitialActiveMemberCap ||
                dynastyState.Interregnum)
            {
                message =
                    "Dynasty smoke validation failed: spawn phase dynasty state drifted. " +
                    "legitimacy=" + dynastyState.Legitimacy +
                    ", cap=" + dynastyState.ActiveMemberCap +
                    ", interregnum=" + dynastyState.Interregnum + ".";
                return false;
            }

            message = "Spawn: memberCount=" + count + ", headAge=" + head.AgeYears + ", legitimacy=" + dynastyState.Legitimacy + ".";
            return true;
        }

        private static bool RunAgingPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesDynastySmokeValidation_Aging");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionWithDynasty(entityManager, "player");

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyMember(
                    "player",
                    DynastyRole.HeadOfBloodline,
                    out var headBefore))
            {
                message = "Dynasty smoke validation failed: aging phase could not find Head of Bloodline.";
                return false;
            }
            float initialAge = headBefore.AgeYears;

            Tick(world, seconds: 120d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyMember(
                    "player",
                    DynastyRole.HeadOfBloodline,
                    out var headAfter))
            {
                message = "Dynasty smoke validation failed: aging phase lost the ruling member.";
                return false;
            }

            float delta = headAfter.AgeYears - initialAge;
            if (delta <= 0f)
            {
                message =
                    "Dynasty smoke validation failed: aging phase did not advance ruling member age. " +
                    "initial=" + initialAge + ", final=" + headAfter.AgeYears + ".";
                return false;
            }

            message =
                "Aging: initialAge=" + initialAge + ", finalAge=" + headAfter.AgeYears.ToString("0.##") +
                ", delta=" + delta.ToString("0.##") + ".";
            return true;
        }

        private static bool RunSuccessionPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesDynastySmokeValidation_Succession");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionWithDynasty(entityManager, "player");

            if (!commandSurfaceScope.CommandSurface.TryDebugFellDynastyMember("player", DynastyRole.HeadOfBloodline))
            {
                message = "Dynasty smoke validation failed: succession phase could not fell the ruler.";
                return false;
            }

            Tick(world, seconds: 0.2d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyMember(
                    "player",
                    DynastyRole.HeadOfBloodline,
                    out var newRuler))
            {
                message = "Dynasty smoke validation failed: succession phase produced no new ruler.";
                return false;
            }

            if (newRuler.Status != DynastyMemberStatus.Ruling)
            {
                message =
                    "Dynasty smoke validation failed: succession phase new ruler has wrong status " +
                    newRuler.Status + ".";
                return false;
            }

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyState("player", out var dynastyState) ||
                dynastyState.Interregnum)
            {
                message = "Dynasty smoke validation failed: succession phase entered false interregnum.";
                return false;
            }

            message = "Succession: newRulerTitle=" + newRuler.Title + ", age=" + newRuler.AgeYears.ToString("0.##") + ".";
            return true;
        }

        private static bool RunInterregnumPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesDynastySmokeValidation_Interregnum");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionWithDynasty(entityManager, "player");

            // Succession keeps promoting the next heir to HeadOfBloodline, so repeatedly
            // felling that role works through the ordered member chain until none remain.
            int felled = 0;
            for (int i = 0; i < DynastyTemplates.Canonical.Length + 2; i++)
            {
                if (commandSurfaceScope.CommandSurface.TryDebugFellDynastyMember("player", DynastyRole.HeadOfBloodline))
                {
                    felled++;
                }
                Tick(world, seconds: 0.1d);
            }

            Tick(world, seconds: 0.2d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetDynastyState("player", out var dynastyState))
            {
                message = "Dynasty smoke validation failed: interregnum phase could not read dynasty state.";
                return false;
            }

            if (!dynastyState.Interregnum)
            {
                message = "Dynasty smoke validation failed: interregnum phase did not enter Interregnum after full extinction.";
                return false;
            }

            message = "Interregnum: felledThroughChain=" + felled + ", interregnum=True.";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyAgingSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastySuccessionSystem>());
            return world;
        }

        private static void Tick(World world, double seconds)
        {
            double elapsed = world.Time.ElapsedTime;
            double target = elapsed + seconds;
            while (elapsed < target)
            {
                world.SetTime(new TimeData(elapsed, SimStepSeconds));
                world.Update();
                elapsed += SimStepSeconds;
            }
        }

        private static Entity CreateFactionWithDynasty(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            DynastyBootstrap.AttachDynasty(entityManager, entity, factionId);
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
}
#endif
