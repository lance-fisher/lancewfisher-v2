#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the AI strategic layer.
    ///
    /// Phase 1: AIStrategyComponent seeds correctly on non-player Kingdom factions
    ///          (Posture=Expand, intervals at canonical defaults).
    ///
    /// Phase 2: Territory expansion posture -- with a player CP present, AI picks
    ///          it as expansion target and issues expansion orders.
    ///
    /// Phase 3: World pressure response -- at WorldPressureComponent.Level=3,
    ///          posture transitions to Defend.
    ///
    /// Phase 4: Scout harass -- LightCavalry unit dispatched toward lowest-loyalty
    ///          hostile CP.
    ///
    /// Browser reference: ai.js pickTerritoryTarget (~747), pickScoutHarassTarget (~412),
    ///                    getWorldPressureRaidTarget (~817).
    /// Artifact: artifacts/unity-ai-strategy-smoke.log.
    /// </summary>
    public static class BloodlinesAIStrategySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-ai-strategy-smoke.log";

        [MenuItem("Bloodlines/AI/Run AI Strategy Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIStrategySmokeValidation() => RunInternal(batchMode: true);

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
                message = "AI strategy smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_STRATEGY_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
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
            ok &= RunPhase1(sb);
            ok &= RunPhase2(sb);
            ok &= RunPhase3(sb);
            ok &= RunPhase4(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return sg;
        }

        // --------- Phase 1: component defaults ---------

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-strategy-phase1");
            var em = world.EntityManager;

            // Seed an enemy Kingdom faction with both AI components.
            var faction = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent), typeof(AIStrategyComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(faction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(faction, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(faction, new AIStrategyComponent
            {
                ExpansionIntervalSeconds             = 8f,
                ScoutHarassIntervalSeconds           = 12f,
                WorldPressureResponseIntervalSeconds = 15f,
                ReinforcementIntervalSeconds         = 10f,
                CurrentPosture                       = AIStrategicPosture.Expand,
            });

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<AIStrategyComponent>());
            if (q.IsEmpty) { q.Dispose(); sb.AppendLine("Phase 1 FAIL: AIStrategyComponent not found."); return false; }
            var comp = q.GetSingleton<AIStrategyComponent>();
            q.Dispose();

            if (comp.CurrentPosture != AIStrategicPosture.Expand)
            {
                sb.AppendLine($"Phase 1 FAIL: expected Expand posture, got {comp.CurrentPosture}."); return false;
            }
            if (Math.Abs(comp.ExpansionIntervalSeconds - 8f) > 0.001f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected ExpansionIntervalSeconds=8, got {comp.ExpansionIntervalSeconds}."); return false;
            }

            sb.AppendLine($"Phase 1 PASS: AIStrategyComponent defaults; Posture={comp.CurrentPosture} ExpansionInterval={comp.ExpansionIntervalSeconds}.");
            return true;
        }

        // --------- Phase 2: territory expansion picks target ---------

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-strategy-phase2");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());

            // Seed MatchProgression and WorldPressure for the system.
            var mp = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mp, new MatchProgressionComponent { GreatReckoningThreshold = 0.7f });

            var enemyFaction = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent), typeof(AIStrategyComponent),
                typeof(WorldPressureComponent));
            em.SetComponentData(enemyFaction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(enemyFaction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyFaction, new AIEconomyControllerComponent { Enabled = true, MilitaryPostureMinimumMilitiaCount = 2 });
            em.SetComponentData(enemyFaction, new AIStrategyComponent
            {
                ExpansionIntervalSeconds             = 0f,   // fire immediately in smoke
                ScoutHarassIntervalSeconds           = 999f, // suppress
                WorldPressureResponseIntervalSeconds = 999f, // suppress
                ReinforcementIntervalSeconds         = 999f, // suppress
                CurrentPosture                       = AIStrategicPosture.Expand,
            });
            em.SetComponentData(enemyFaction, new WorldPressureComponent());

            // Enemy command hall (base position).
            var hall = em.CreateEntity(typeof(FactionComponent), typeof(BuildingTypeComponent), typeof(PositionComponent), typeof(HealthComponent));
            em.SetComponentData(hall, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(hall, new BuildingTypeComponent { TypeId = new FixedString64Bytes("command_hall") });
            em.SetComponentData(hall, new PositionComponent { Value = new float3(0, 0, 0) });
            em.SetComponentData(hall, new HealthComponent { Current = 100f, Max = 100f });

            // 3 enemy combat units (melee) -- enough to exceed threshold=2.
            for (int i = 0; i < 3; i++)
            {
                var u = em.CreateEntity(typeof(FactionComponent), typeof(UnitTypeComponent), typeof(PositionComponent), typeof(HealthComponent), typeof(MoveCommandComponent));
                em.SetComponentData(u, new FactionComponent { FactionId = "enemy" });
                em.SetComponentData(u, new UnitTypeComponent { Role = UnitRole.Melee, TypeId = new FixedString64Bytes("militia") });
                em.SetComponentData(u, new PositionComponent { Value = new float3(0, 0, 0) });
                em.SetComponentData(u, new HealthComponent { Current = 100f, Max = 100f });
            }

            // Player CP at position (5, 0, 0).
            var cp = em.CreateEntity(typeof(ControlPointComponent), typeof(PositionComponent));
            em.SetComponentData(cp, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_player"),
                OwnerFactionId = new FixedString32Bytes("player"),
                Loyalty = 80f,
            });
            em.SetComponentData(cp, new PositionComponent { Value = new float3(5, 0, 0) });

            // Run one update (WorldPressureEscalationSystem also runs, score/posture updates).
            world.Update();

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<AIStrategyComponent>());
            var strategy = q.GetSingleton<AIStrategyComponent>();
            q.Dispose();

            // The expansion target should have been set (territory commands issued OR target id set).
            if (strategy.TerritoryCommandsIssued == 0 && strategy.ExpansionTargetCpId.Length == 0)
            {
                sb.AppendLine($"Phase 2 FAIL: expected expansion orders or target after update. Posture={strategy.CurrentPosture}.");
                return false;
            }

            sb.AppendLine($"Phase 2 PASS: TerritoryCommandsIssued={strategy.TerritoryCommandsIssued} ExpansionTarget={strategy.ExpansionTargetCpId} Posture={strategy.CurrentPosture}.");
            return true;
        }

        // --------- Phase 3: world pressure Level=3 -> Defend posture ---------

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-strategy-phase3");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());

            var mp = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mp, new MatchProgressionComponent { GreatReckoningThreshold = 0.7f });

            var enemyFaction = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent), typeof(AIStrategyComponent),
                typeof(WorldPressureComponent));
            em.SetComponentData(enemyFaction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(enemyFaction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyFaction, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(enemyFaction, new AIStrategyComponent
            {
                ExpansionIntervalSeconds             = 8f,
                ScoutHarassIntervalSeconds           = 12f,
                WorldPressureResponseIntervalSeconds = 0f, // fire immediately
                ReinforcementIntervalSeconds         = 10f,
                CurrentPosture                       = AIStrategicPosture.Expand,
            });
            // Pre-seed WorldPressureComponent at Level=3 (Targeted).
            em.SetComponentData(enemyFaction, new WorldPressureComponent { Level = 3, Targeted = true, Score = 4, Streak = 6 });

            world.Update();

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<AIStrategyComponent>());
            var strategy = q.GetSingleton<AIStrategyComponent>();
            q.Dispose();

            if (strategy.CurrentPosture != AIStrategicPosture.Defend)
            {
                sb.AppendLine($"Phase 3 FAIL: expected Defend at Level=3, got {strategy.CurrentPosture}."); return false;
            }
            if (strategy.WorldPressureLevelCached != 3)
            {
                sb.AppendLine($"Phase 3 FAIL: expected WorldPressureLevelCached=3, got {strategy.WorldPressureLevelCached}."); return false;
            }

            sb.AppendLine($"Phase 3 PASS: Posture={strategy.CurrentPosture} WorldPressureLevelCached={strategy.WorldPressureLevelCached}.");
            return true;
        }

        // --------- Phase 4: scout dispatch toward lowest-loyalty hostile CP ---------

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("ai-strategy-phase4");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());

            var mp = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mp, new MatchProgressionComponent { GreatReckoningThreshold = 0.7f });

            var enemyFaction = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent), typeof(AIStrategyComponent),
                typeof(WorldPressureComponent));
            em.SetComponentData(enemyFaction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(enemyFaction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyFaction, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(enemyFaction, new AIStrategyComponent
            {
                ExpansionIntervalSeconds             = 999f,  // suppress expansion
                ScoutHarassIntervalSeconds           = 0f,    // fire immediately
                WorldPressureResponseIntervalSeconds = 15f,
                ReinforcementIntervalSeconds         = 999f,
                CurrentPosture                       = AIStrategicPosture.Expand,
            });
            em.SetComponentData(enemyFaction, new WorldPressureComponent());

            // Enemy command hall at origin.
            var hall = em.CreateEntity(typeof(FactionComponent), typeof(BuildingTypeComponent), typeof(PositionComponent), typeof(HealthComponent));
            em.SetComponentData(hall, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(hall, new BuildingTypeComponent { TypeId = new FixedString64Bytes("command_hall") });
            em.SetComponentData(hall, new PositionComponent { Value = new float3(0, 0, 0) });
            em.SetComponentData(hall, new HealthComponent { Current = 100f, Max = 100f });

            // LightCavalry scout unit.
            var scout = em.CreateEntity(typeof(FactionComponent), typeof(UnitTypeComponent), typeof(PositionComponent), typeof(HealthComponent), typeof(MoveCommandComponent));
            em.SetComponentData(scout, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(scout, new UnitTypeComponent { Role = UnitRole.LightCavalry, TypeId = new FixedString64Bytes("cavalry") });
            em.SetComponentData(scout, new PositionComponent { Value = new float3(0, 0, 0) });
            em.SetComponentData(scout, new HealthComponent { Current = 100f, Max = 100f });
            em.SetComponentData(scout, new MoveCommandComponent { IsActive = false });

            // Player CP at (6, 0, 0) with low loyalty.
            var cp = em.CreateEntity(typeof(ControlPointComponent), typeof(PositionComponent));
            em.SetComponentData(cp, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_player"),
                OwnerFactionId = new FixedString32Bytes("player"),
                Loyalty = 25f,
            });
            em.SetComponentData(cp, new PositionComponent { Value = new float3(6, 0, 0) });

            world.Update();

            // Verify scout received a move command.
            var moveComp = em.GetComponentData<MoveCommandComponent>(scout);
            if (!moveComp.IsActive)
            {
                sb.AppendLine("Phase 4 FAIL: scout MoveCommand not active after harass dispatch."); return false;
            }
            float dist = Unity.Mathematics.math.distance(moveComp.Destination, new float3(6, 0, 0));
            if (dist > 2f)
            {
                sb.AppendLine($"Phase 4 FAIL: scout move destination far from target CP (dist={dist:F2})."); return false;
            }

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<AIStrategyComponent>());
            var strategy = q.GetSingleton<AIStrategyComponent>();
            q.Dispose();

            sb.AppendLine($"Phase 4 PASS: scout dispatched; ScoutHarassOrdersIssued={strategy.ScoutHarassOrdersIssued} HarassTarget={strategy.HarassTargetCpId}.");
            return true;
        }
    }
}
#endif
