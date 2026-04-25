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
    /// Smoke validator for sub-slice 38: AI player divine right context flag refresher.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1156-1160.
    ///   When player has an active divine right declaration, AI attack, territory,
    ///   and raid timers are capped downward (AI responds to the threat).
    ///
    /// AIPlayerDivineRightContextSystem detects an active DivineRight operation
    /// on the player faction and writes PlayerDivineRightActive into both
    /// AIStrategyComponent and AICovertOpsComponent on each AI faction.
    ///
    /// Phases:
    ///   PhasePlayerDivineRightDetected: active DivineRight op with source="player" ->
    ///     both flags set to true.
    ///   PhaseNoDivineRight: no active player DivineRight op ->
    ///     both flags false.
    ///   PhaseInactiveOpIgnored: DivineRight op with Active=false ->
    ///     both flags false.
    ///
    /// Artifact: artifacts/unity-player-divine-right-context-smoke.log.
    /// </summary>
    public static class BloodlinesPlayerDivineRightContextSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-divine-right-context-smoke.log";

        [MenuItem("Bloodlines/AI/Run Player Divine Right Context Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerDivineRightContextSmokeValidation() =>
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
                message = "Player divine right context smoke errored: " + e;
            }

            string artifact = "BLOODLINES_PLAYER_DIVINE_RIGHT_CONTEXT_SMOKE " +
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
            ok &= RunPhaseDetected(sb);
            ok &= RunPhaseNoDivineRight(sb);
            ok &= RunPhaseInactiveOpIgnored(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIPlayerDivineRightContextSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity CreateAIFaction(EntityManager em, string factionId)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AICovertOpsComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            return e;
        }

        private static Entity CreateDivineRightOp(EntityManager em, string source, bool active)
        {
            var e = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(e, new DynastyOperationComponent
            {
                OperationId     = new FixedString64Bytes("dr-test-001"),
                SourceFactionId = new FixedString32Bytes(source),
                OperationKind   = DynastyOperationKind.DivineRight,
                Active          = active,
            });
            return e;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseDetected(System.Text.StringBuilder sb)
        {
            using var world = new World("pdr-context-detected");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            CreateDivineRightOp(em, source: "player", active: true);

            sg.Update();

            var strategy  = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            var covertOps = em.GetComponentData<AICovertOpsComponent>(aiFactionEntity);
            bool strategySet  = strategy.PlayerDivineRightActive;
            bool covertOpsSet = covertOps.PlayerDivineRightActive;

            bool pass = strategySet && covertOpsSet;
            sb.AppendLine($"[PhaseDetected] strategyFlag={strategySet} covertOpsFlag={covertOpsSet} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNoDivineRight(System.Text.StringBuilder sb)
        {
            using var world = new World("pdr-context-none");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            // No operation entity at all

            sg.Update();

            var strategy  = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            var covertOps = em.GetComponentData<AICovertOpsComponent>(aiFactionEntity);
            bool bothFalse = !strategy.PlayerDivineRightActive && !covertOps.PlayerDivineRightActive;

            bool pass = bothFalse;
            sb.AppendLine($"[PhaseNoDivineRight] strategyFlag={strategy.PlayerDivineRightActive} " +
                          $"covertOpsFlag={covertOps.PlayerDivineRightActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseInactiveOpIgnored(System.Text.StringBuilder sb)
        {
            using var world = new World("pdr-context-inactive");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var aiFactionEntity = CreateAIFaction(em, "test_enemy");
            CreateDivineRightOp(em, source: "player", active: false);

            sg.Update();

            var strategy  = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            var covertOps = em.GetComponentData<AICovertOpsComponent>(aiFactionEntity);
            bool bothFalse = !strategy.PlayerDivineRightActive && !covertOps.PlayerDivineRightActive;

            bool pass = bothFalse;
            sb.AppendLine($"[PhaseInactiveOpIgnored] strategyFlag={strategy.PlayerDivineRightActive} " +
                          $"covertOpsFlag={covertOps.PlayerDivineRightActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
