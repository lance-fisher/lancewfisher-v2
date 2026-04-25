#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 36: AI succession-crisis context flag refresher.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1161-1185.
    ///   Player crisis block (1161-1165): clamps attackTimer and territoryTimer when
    ///     player has an active succession crisis.
    ///   Enemy crisis block (1167-1185): floors attackTimer and territoryTimer upward
    ///     when AI has an active succession crisis.
    ///
    /// AISuccessionCrisisContextSystem reads SuccessionCrisisComponent on both
    /// player and AI factions and writes the four flags into AIStrategyComponent.
    ///
    /// Phases:
    ///   PhasePlayerCrisisHighDetected: player faction has severity=3 (Major) ->
    ///     PlayerSuccessionCrisisActive=true, PlayerSuccessionCrisisHigh=true.
    ///   PhaseEnemyCrisisSevereDetected: AI faction has severity=4 (Catastrophic) ->
    ///     EnemySuccessionCrisisActive=true, EnemySuccessionCrisisSevere=true.
    ///   PhaseNoCrisis: no SuccessionCrisisComponent on either faction ->
    ///     all four flags false.
    ///
    /// Artifact: artifacts/unity-succession-crisis-context-smoke.log.
    /// </summary>
    public static class BloodlinesSuccessionCrisisContextSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-succession-crisis-context-smoke.log";

        [MenuItem("Bloodlines/AI/Run Succession Crisis Context Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSuccessionCrisisContextSmokeValidation() =>
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
                message = "Succession crisis context smoke errored: " + e;
            }

            string artifact = "BLOODLINES_SUCCESSION_CRISIS_CONTEXT_SMOKE " +
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
            ok &= RunPhasePlayerCrisisHighDetected(sb);
            ok &= RunPhaseEnemyCrisisSevereDetected(sb);
            ok &= RunPhaseNoCrisis(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AISuccessionCrisisContextSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity CreatePlayerFaction(EntityManager em, byte crisisSeverity = 0)
        {
            var e = em.CreateEntity(typeof(FactionComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes("player"),
            });
            if (crisisSeverity > 0)
            {
                em.AddComponentData(e, new SuccessionCrisisComponent
                {
                    CrisisSeverity             = crisisSeverity,
                    CrisisStartedAtInWorldDays = 0f,
                    RecoveryProgressPct        = 0f,
                    ResourceTrickleFactor      = 0.9f,
                    LoyaltyShockApplied        = true,
                    LegitimacyDrainRatePerDay  = 0.14f,
                    LoyaltyDrainRatePerDay     = 0.2f,
                    LastDailyTickInWorldDays   = 0f,
                    RecoveryRatePerDay         = 0.05f,
                });
            }
            return e;
        }

        private static Entity CreateAIFaction(EntityManager em, string factionId, byte crisisSeverity = 0)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            if (crisisSeverity > 0)
            {
                em.AddComponentData(e, new SuccessionCrisisComponent
                {
                    CrisisSeverity             = crisisSeverity,
                    CrisisStartedAtInWorldDays = 0f,
                    RecoveryProgressPct        = 0f,
                    ResourceTrickleFactor      = 0.9f,
                    LoyaltyShockApplied        = true,
                    LegitimacyDrainRatePerDay  = 0.14f,
                    LoyaltyDrainRatePerDay     = 0.2f,
                    LastDailyTickInWorldDays   = 0f,
                    RecoveryRatePerDay         = 0.05f,
                });
            }
            return e;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhasePlayerCrisisHighDetected(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-context-player-high");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            CreatePlayerFaction(em, crisisSeverity: (byte)SuccessionCrisisSeverity.Major);
            var aiFactionEntity = CreateAIFaction(em, "test_enemy");

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool playerActive = strategy.PlayerSuccessionCrisisActive;
            bool playerHigh   = strategy.PlayerSuccessionCrisisHigh;
            bool enemyActive  = strategy.EnemySuccessionCrisisActive;

            bool pass = playerActive && playerHigh && !enemyActive;
            sb.AppendLine($"[PhasePlayerCrisisHighDetected] playerActive={playerActive} " +
                          $"playerHigh={playerHigh} enemyActive={enemyActive} " +
                          $"=> {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseEnemyCrisisSevereDetected(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-context-enemy-severe");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            CreatePlayerFaction(em);
            var aiFactionEntity = CreateAIFaction(em, "test_enemy",
                crisisSeverity: (byte)SuccessionCrisisSeverity.Catastrophic);

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool enemyActive = strategy.EnemySuccessionCrisisActive;
            bool enemySevere = strategy.EnemySuccessionCrisisSevere;
            bool playerActive = strategy.PlayerSuccessionCrisisActive;

            bool pass = enemyActive && enemySevere && !playerActive;
            sb.AppendLine($"[PhaseEnemyCrisisSevereDetected] enemyActive={enemyActive} " +
                          $"enemySevere={enemySevere} playerActive={playerActive} " +
                          $"=> {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNoCrisis(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-context-no-crisis");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            CreatePlayerFaction(em);
            var aiFactionEntity = CreateAIFaction(em, "test_enemy");

            sg.Update();

            var strategy = em.GetComponentData<AIStrategyComponent>(aiFactionEntity);
            bool allFalse = !strategy.PlayerSuccessionCrisisActive &&
                            !strategy.PlayerSuccessionCrisisHigh &&
                            !strategy.EnemySuccessionCrisisActive &&
                            !strategy.EnemySuccessionCrisisSevere;

            bool pass = allFalse;
            sb.AppendLine($"[PhaseNoCrisis] allFalse={allFalse} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
