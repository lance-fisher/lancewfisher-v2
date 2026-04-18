#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Systems;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the world-pressure-escalation ECS lane.
    ///
    /// Phase 1: WorldPressureCycleTracker singleton created by WorldPressureEscalationSystem.OnCreate;
    ///          Accumulator == 0, CycleSeconds == 90 (canonical default).
    ///
    /// Phase 2: Player faction with 5 territories gets TerritoryExpansionScore == 3,
    ///          Score == 3. No dominant leader (score &lt; 4), Level stays 0.
    ///
    /// Phase 3: Player faction with 6 territories (Score == 4), enemy has 1 territory
    ///          (Score == 0). Player is dominant. After forcing 6 cycles:
    ///          Streak == 6, Level == 3.
    ///
    /// Phase 4: Loyalty consequence fires at Level 3. Lowest-loyalty CP owned by player
    ///          loses 3 loyalty points per cycle.
    ///
    /// Browser reference: simulation.js updateWorldPressureEscalation (13709),
    ///                    applyWorldPressureConsequences (13695).
    /// Artifact: artifacts/unity-world-pressure-smoke.log.
    /// </summary>
    public static class BloodlinesWorldPressureSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-world-pressure-smoke.log";

        [MenuItem("Bloodlines/WorldPressure/Run World Pressure Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchWorldPressureSmokeValidation()
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
                message = "World pressure smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_WORLD_PRESSURE_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { /* artifact write is best-effort */ }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool allPass = true;
            allPass &= RunPhase1(sb);
            allPass &= RunPhase2(sb);
            allPass &= RunPhase3(sb);
            allPass &= RunPhase4(sb);
            report = sb.ToString();
            return allPass;
        }

        // ---------- helpers ----------

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return simGroup;
        }

        // ---------- Phase 1: CycleTracker singleton defaults ----------

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("wp-smoke-phase1");
            // GetOrCreateSystem triggers OnCreate, which creates the tracker singleton.
            world.GetOrCreateSystem<WorldPressureEscalationSystem>();

            var em = world.EntityManager;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<WorldPressureCycleTrackerComponent>());
            if (q.IsEmpty)
            {
                q.Dispose();
                sb.AppendLine("Phase 1 FAIL: WorldPressureCycleTrackerComponent singleton not created.");
                return false;
            }
            var tracker = q.GetSingleton<WorldPressureCycleTrackerComponent>();
            q.Dispose();

            if (tracker.Accumulator != 0f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected Accumulator=0, got {tracker.Accumulator}.");
                return false;
            }
            if (Math.Abs(tracker.CycleSeconds - 90f) > 0.001f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected CycleSeconds=90, got {tracker.CycleSeconds}.");
                return false;
            }
            sb.AppendLine("Phase 1 PASS: CycleTracker defaults; Accumulator=0 CycleSeconds=90.");
            return true;
        }

        // ---------- Phase 2: 5 territories -> Score == 3, Level == 0 ----------

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("wp-smoke-phase2");
            var em = world.EntityManager;

            var simGroup = SetupSimGroup(world);
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());

            // Seed MatchProgression (no great reckoning).
            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                StageNumber = 1,
                StageId = new FixedString32Bytes("founding"),
                StageLabel = new FixedString64Bytes("Founding"),
                GreatReckoningThreshold = 0.7f,
                GreatReckoningActive = false,
            });

            // Seed player kingdom faction with WorldPressureComponent.
            var playerEntity = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent), typeof(WorldPressureComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new WorldPressureComponent());

            // Seed 5 player-owned CPs.
            for (int i = 0; i < 5; i++)
            {
                var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                em.SetComponentData(cpEntity, new ControlPointComponent
                {
                    ControlPointId = new FixedString32Bytes("cp_p" + i),
                    OwnerFactionId = new FixedString32Bytes("player"),
                    Loyalty = 100f,
                });
            }

            // Leave accumulator at 0 (no cycle fires, but per-frame score IS updated).
            world.Update();

            var wpQ = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            var wfacs = wpQ.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var wcomps = wpQ.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            wpQ.Dispose();

            WorldPressureComponent playerWp = default;
            bool found = false;
            for (int i = 0; i < wfacs.Length; i++)
                if (wfacs[i].FactionId == new FixedString32Bytes("player")) { playerWp = wcomps[i]; found = true; }
            wfacs.Dispose();
            wcomps.Dispose();

            if (!found) { sb.AppendLine("Phase 2 FAIL: player WorldPressureComponent not found."); return false; }
            if (playerWp.TerritoryExpansionScore != 3)
            {
                sb.AppendLine($"Phase 2 FAIL: expected TerritoryExpansionScore=3 (5-2), got {playerWp.TerritoryExpansionScore}.");
                return false;
            }
            if (playerWp.Score != 3)
            {
                sb.AppendLine($"Phase 2 FAIL: expected Score=3, got {playerWp.Score}.");
                return false;
            }
            if (playerWp.Level != 0)
            {
                sb.AppendLine($"Phase 2 FAIL: expected Level=0 (score < 4, no dominant), got {playerWp.Level}.");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: Score={playerWp.Score} TerritoryExpansionScore={playerWp.TerritoryExpansionScore} Level={playerWp.Level}.");
            return true;
        }

        // ---------- Phase 3: 6 territories -> dominant leader, streak advances to Level 3 ----------

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("wp-smoke-phase3");
            var em = world.EntityManager;

            var simGroup = SetupSimGroup(world);
            var systemHandle = world.GetOrCreateSystem<WorldPressureEscalationSystem>();
            simGroup.AddSystemToUpdateList(systemHandle);

            // Find the tracker entity created by OnCreate.
            var trackerQ = em.CreateEntityQuery(ComponentType.ReadOnly<WorldPressureCycleTrackerComponent>());
            var trackerEntity = trackerQ.GetSingletonEntity();
            trackerQ.Dispose();

            // Seed MatchProgression.
            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                StageNumber = 3,
                StageId = new FixedString32Bytes("encounter_establishment"),
                StageLabel = new FixedString64Bytes("Encounter and Establishment"),
                GreatReckoningThreshold = 0.7f,
                GreatReckoningActive = false,
            });

            // Seed player (6 territories) and enemy (1 territory) kingdom factions.
            var playerEntity = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent), typeof(WorldPressureComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new WorldPressureComponent());

            var enemyEntity = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent), typeof(WorldPressureComponent));
            em.SetComponentData(enemyEntity, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(enemyEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyEntity, new WorldPressureComponent());

            // 6 player CPs.
            for (int i = 0; i < 6; i++)
            {
                var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                em.SetComponentData(cpEntity, new ControlPointComponent
                {
                    ControlPointId = new FixedString32Bytes("cp_p" + i),
                    OwnerFactionId = new FixedString32Bytes("player"),
                    Loyalty = 80f,
                });
            }
            // 1 enemy CP.
            var ecpEntity = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(ecpEntity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_enemy"),
                OwnerFactionId = new FixedString32Bytes("enemy"),
                Loyalty = 80f,
            });

            // Run 6 cycle updates to advance streak to 6 (Level 3).
            for (int cycle = 0; cycle < 6; cycle++)
            {
                // Set accumulator to threshold to force a cycle this update.
                var t = em.GetComponentData<WorldPressureCycleTrackerComponent>(trackerEntity);
                t.Accumulator = t.CycleSeconds;
                em.SetComponentData(trackerEntity, t);
                world.Update();
            }

            var wpQ = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            var wfacs = wpQ.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var wcomps = wpQ.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            wpQ.Dispose();

            WorldPressureComponent playerWp = default;
            bool found = false;
            for (int i = 0; i < wfacs.Length; i++)
                if (wfacs[i].FactionId == new FixedString32Bytes("player")) { playerWp = wcomps[i]; found = true; }
            wfacs.Dispose();
            wcomps.Dispose();

            if (!found) { sb.AppendLine("Phase 3 FAIL: player WorldPressureComponent not found."); return false; }
            if (playerWp.Score != 4)
            {
                sb.AppendLine($"Phase 3 FAIL: expected Score=4 (6-2), got {playerWp.Score}.");
                return false;
            }
            if (!playerWp.Targeted)
            {
                sb.AppendLine($"Phase 3 FAIL: expected Targeted=true, got false. Score={playerWp.Score}.");
                return false;
            }
            if (playerWp.Level != 3)
            {
                sb.AppendLine($"Phase 3 FAIL: expected Level=3 after 6 cycles as dominant, got Level={playerWp.Level} Streak={playerWp.Streak}.");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: Score={playerWp.Score} Targeted={playerWp.Targeted} Streak={playerWp.Streak} Level={playerWp.Level} Label={playerWp.Label}.");
            return true;
        }

        // ---------- Phase 4: loyalty consequence drains lowest-loyalty CP ----------

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("wp-smoke-phase4");
            var em = world.EntityManager;

            var simGroup = SetupSimGroup(world);
            var systemHandle = world.GetOrCreateSystem<WorldPressureEscalationSystem>();
            simGroup.AddSystemToUpdateList(systemHandle);

            // Find the tracker created by OnCreate and set accumulator to threshold.
            var trackerQ = em.CreateEntityQuery(ComponentType.ReadOnly<WorldPressureCycleTrackerComponent>());
            var trackerEntity = trackerQ.GetSingletonEntity();
            trackerQ.Dispose();
            var tracker = em.GetComponentData<WorldPressureCycleTrackerComponent>(trackerEntity);
            tracker.Accumulator = tracker.CycleSeconds;
            em.SetComponentData(trackerEntity, tracker);

            // Seed MatchProgression.
            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                GreatReckoningThreshold = 0.7f,
                GreatReckoningActive = false,
            });

            // Seed player faction with WorldPressureComponent pre-set to Streak=5, Level=2.
            // One cycle update advances Streak to 6, Level to 3.
            var playerEntity = em.CreateEntity(
                typeof(FactionComponent), typeof(FactionKindComponent), typeof(WorldPressureComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new WorldPressureComponent { Streak = 5, Level = 2 });

            // 6 player CPs: one with low loyalty (50), others high (90).
            float lowLoyalty = 50f;
            var lowCpEntity = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(lowCpEntity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_low"),
                OwnerFactionId = new FixedString32Bytes("player"),
                Loyalty = lowLoyalty,
            });
            for (int i = 0; i < 5; i++)
            {
                var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                em.SetComponentData(cpEntity, new ControlPointComponent
                {
                    ControlPointId = new FixedString32Bytes("cp_h" + i),
                    OwnerFactionId = new FixedString32Bytes("player"),
                    Loyalty = 90f,
                });
            }

            world.Update();

            // Level becomes 3 (Streak=5+1=6), drain = 3.
            var cpComp = em.GetComponentData<ControlPointComponent>(lowCpEntity);
            float expectedLoyalty = lowLoyalty - 3f;
            if (Math.Abs(cpComp.Loyalty - expectedLoyalty) > 0.01f)
            {
                sb.AppendLine($"Phase 4 FAIL: expected lowest-CP loyalty {expectedLoyalty}, got {cpComp.Loyalty:F2}.");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: lowest-loyalty CP drained by Level=3; loyalty={cpComp.Loyalty:F1} (was {lowLoyalty}).");
            return true;
        }
    }
}
#endif
