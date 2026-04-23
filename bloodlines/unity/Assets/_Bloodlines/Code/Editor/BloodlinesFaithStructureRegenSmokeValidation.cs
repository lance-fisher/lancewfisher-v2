#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesFaithStructureRegenSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-faith-structure-regen-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Faith Structure Regen Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFaithStructureRegenSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Faith structure regen smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_FAITH_STRUCTURE_REGEN_SMOKE " +
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
            ok &= RunRelativeRegenPhase(lines);
            ok &= RunCapPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunRelativeRegenPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-structure-regen-phase1");
            var entityManager = world.EntityManager;

            SeedClock(entityManager, 0f, 2f);
            Entity playerFaction = SeedCommittedFaction(entityManager, "player", 10f);
            Entity rivalFaction = SeedCommittedFaction(entityManager, "rival", 10f);

            SeedBuilding(entityManager, "player", "wayshrine");
            SeedBuilding(entityManager, "player", "covenant_hall");
            SeedBuilding(entityManager, "player", "grand_sanctuary");
            SeedBuilding(entityManager, "rival", "wayshrine");

            Tick(world);
            SetDualClock(entityManager, 10f, 2f);
            Tick(world);

            var playerFaith = entityManager.GetComponentData<FaithStateComponent>(playerFaction);
            var rivalFaith = entityManager.GetComponentData<FaithStateComponent>(rivalFaction);
            var playerTracker = entityManager.GetComponentData<FaithStructureRegenComponent>(playerFaction);
            var rivalTracker = entityManager.GetComponentData<FaithStructureRegenComponent>(rivalFaction);

            if (!Approximately(playerTracker.CurrentRatePerSecond, 0.98f) ||
                !Approximately(rivalTracker.CurrentRatePerSecond, 0.18f) ||
                !Approximately(playerFaith.Intensity, 14.9f) ||
                !Approximately(rivalFaith.Intensity, 10.9f) ||
                playerFaith.Intensity <= rivalFaith.Intensity)
            {
                lines.AppendLine(
                    "Phase 1 FAIL: expected three completed faith buildings to outpace one building. " +
                    $"playerRate={playerTracker.CurrentRatePerSecond:0.###} " +
                    $"rivalRate={rivalTracker.CurrentRatePerSecond:0.###} " +
                    $"playerIntensity={playerFaith.Intensity:0.###} " +
                    $"rivalIntensity={rivalFaith.Intensity:0.###}");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: three completed faith structures regenerated committed intensity faster than a single wayshrine.");
            return true;
        }

        private static bool RunCapPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-structure-regen-phase2");
            var entityManager = world.EntityManager;

            SeedClock(entityManager, 0f, 2f);
            Entity playerFaction = SeedCommittedFaction(entityManager, "player", 25f);

            SeedBuilding(entityManager, "player", "wayshrine");
            SeedBuilding(entityManager, "player", "covenant_hall");
            SeedBuilding(entityManager, "player", "grand_sanctuary");
            SeedBuilding(entityManager, "player", "grand_sanctuary");
            SeedBuilding(entityManager, "player", "apex_covenant");
            SeedBuilding(entityManager, "player", "apex_covenant");

            Tick(world);
            SetDualClock(entityManager, 10f, 2f);
            Tick(world);

            var faith = entityManager.GetComponentData<FaithStateComponent>(playerFaction);
            var tracker = entityManager.GetComponentData<FaithStructureRegenComponent>(playerFaction);

            if (!Approximately(tracker.CurrentRatePerSecond, FaithStructureRegenRules.MaxRatePerSecond) ||
                tracker.FaithBuildingCount != 6 ||
                !Approximately(tracker.LastAppliedIntensityDelta, 7f) ||
                !Approximately(faith.Intensity, 32f))
            {
                lines.AppendLine(
                    "Phase 2 FAIL: expected capped faith regen regardless of building count. " +
                    $"rate={tracker.CurrentRatePerSecond:0.###} count={tracker.FaithBuildingCount} " +
                    $"delta={tracker.LastAppliedIntensityDelta:0.###} intensity={faith.Intensity:0.###}");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: stacked faith structures clamped regen at the canonical 1.4 per-second ceiling.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithStructureRegenSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void SeedClock(
            EntityManager entityManager,
            float inWorldDays,
            float daysPerRealSecond)
        {
            var entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = daysPerRealSecond,
                DeclarationCount = 0,
            });
        }

        private static void SetDualClock(
            EntityManager entityManager,
            float inWorldDays,
            float daysPerRealSecond)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var clock = query.GetSingleton<DualClockComponent>();
            clock.InWorldDays = inWorldDays;
            clock.DaysPerRealSecond = daysPerRealSecond;
            query.SetSingleton(clock);
        }

        private static Entity SeedCommittedFaction(
            EntityManager entityManager,
            string factionId,
            float intensity)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FaithStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.OldLight,
                DoctrinePath = DoctrinePath.Light,
                Intensity = intensity,
                Level = FaithIntensityTiers.ResolveLevel(intensity),
            });
            return entity;
        }

        private static void SeedBuilding(
            EntityManager entityManager,
            string factionId,
            string buildingTypeId)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(BuildingTypeComponent),
                typeof(HealthComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = buildingTypeId,
                FortificationRole = FortificationRole.None,
                StructuralDamageMultiplier = 1f,
                PopulationCapBonus = 0,
                BlocksPassage = false,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = false,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 50f,
                Max = 50f,
            });
        }

        private static void Tick(World world)
        {
            world.SetTime(new TimeData(0d, 1f));
            world.Update();
        }

        private static bool Approximately(float actual, float expected)
        {
            return math.abs(actual - expected) <= 0.001f;
        }
    }
}
#endif
