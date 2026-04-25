#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 37: AI holy war context flag refresher.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1129-1132.
    ///   Any active holy war (either direction) caps attackTimer <= 10 and
    ///   territoryTimer <= 8. AIHolyWarContextSystem writes HolyWarActive
    ///   so AIStrategicPressureSystem can apply those clamps.
    ///
    /// Phases:
    ///   PhaseHolyWarDetected: active HolyWar operation with AI faction as source ->
    ///     HolyWarActive=true.
    ///   PhaseHolyWarAsTarget: active HolyWar op with AI faction as target ->
    ///     HolyWarActive=true.
    ///   PhaseNoHolyWar: no active HolyWar operations -> HolyWarActive=false.
    ///
    /// Artifact: artifacts/unity-holy-war-context-smoke.log.
    /// </summary>
    public static class BloodlinesHolyWarContextSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-holy-war-context-smoke.log";

        [MenuItem("Bloodlines/AI/Run Holy War Context Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchHolyWarContextSmokeValidation() =>
            RunInternal(batchMode: true);

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
                message = "Holy war context smoke errored: " + e;
            }

            string artifact = "BLOODLINES_HOLY_WAR_CONTEXT_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhaseHolyWarDetectedAsSource(sb);
            ok &= RunPhaseHolyWarDetectedAsTarget(sb);
            ok &= RunPhaseNoHolyWar(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIHolyWarContextSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity CreateAIFaction(EntityManager em, string factionId)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            return e;
        }

        private static Entity CreateHolyWarOp(EntityManager em, string source, string target, bool active)
        {
            var e = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(e, new DynastyOperationComponent
            {
                OperationId      = new FixedString64Bytes("hw-test-001"),
                SourceFactionId  = new FixedString32Bytes(source),
                TargetFactionId  = new FixedString32Bytes(target),
                OperationKind    = DynastyOperationKind.HolyWar,
                Active           = active,
            });
            return e;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseHolyWarDetectedAsSource(System.Text.StringBuilder sb)
        {
            using var world = new World("hw-context-source");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            CreateHolyWarOp(em, source: "test_enemy", target: "player", active: true);

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool holyWarActive = strategy.HolyWarActive;

            bool pass = holyWarActive;
            sb.AppendLine($"[PhaseHolyWarDetectedAsSource] holyWarActive={holyWarActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseHolyWarDetectedAsTarget(System.Text.StringBuilder sb)
        {
            using var world = new World("hw-context-target");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            CreateHolyWarOp(em, source: "player", target: "test_enemy", active: true);

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool holyWarActive = strategy.HolyWarActive;

            bool pass = holyWarActive;
            sb.AppendLine($"[PhaseHolyWarDetectedAsTarget] holyWarActive={holyWarActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNoHolyWar(System.Text.StringBuilder sb)
        {
            using var world = new World("hw-context-none");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            // Inactive operation -- should not trigger the flag
            CreateHolyWarOp(em, source: "test_enemy", target: "player", active: false);

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool holyWarInactive = !strategy.HolyWarActive;

            bool pass = holyWarInactive;
            sb.AppendLine($"[PhaseNoHolyWar] holyWarActive={strategy.HolyWarActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
