#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesCovenantTestSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-covenant-test-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Covenant Test Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCovenantTestSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Covenant test smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_COVENANT_TEST_SMOKE " +
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
            ok &= RunQualificationPhase(lines);
            ok &= RunSuccessPhase(lines);
            ok &= RunFailureCooldownPhase(lines);
            ok &= RunRetryBlockedPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunQualificationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("covenant-test-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.TheOrder, DoctrinePath.Light, 80f);
            SetDualClock(entityManager, 0f);

            Tick(world);

            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestState("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read covenant-test state.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "Phase") != nameof(CovenantTestPhase.ReadyToTrigger))
            {
                lines.AppendLine($"Phase 1 FAIL: expected ReadyToTrigger, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: intensity 80 held for 180 in-world days advanced the faction to ReadyToTrigger.");
            return true;
        }

        private static bool RunSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("covenant-test-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 80f);
            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugTriggerCovenantTest("player"))
            {
                lines.AppendLine("Phase 2 FAIL: debug trigger could not queue a covenant test request.");
                return false;
            }

            Tick(world);

            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            var testState = entityManager.GetComponentData<CovenantTestStateComponent>(factionEntity);
            if (testState.TestPhase != CovenantTestPhase.Complete ||
                faithState.Intensity < CovenantTestRules.SuccessIntensityFloor)
            {
                lines.AppendLine("Phase 2 FAIL: successful trigger did not complete the test or floor intensity at 82.");
                return false;
            }

            lines.AppendLine(
                "Phase 2 PASS: resource-backed trigger completed the covenant test and floored faith intensity at " +
                faithState.Intensity.ToString("0.0", CultureInfo.InvariantCulture) + ".");
            return true;
        }

        private static bool RunFailureCooldownPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("covenant-test-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 80f);

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            stockpile.Food = 0f;
            stockpile.Influence = 0f;
            entityManager.SetComponentData(factionEntity, stockpile);

            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            debugScope.CommandSurface.TryDebugTriggerCovenantTest("player");
            Tick(world);

            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            var testState = entityManager.GetComponentData<CovenantTestStateComponent>(factionEntity);
            bool hasCooldown = DynastyPoliticalEventUtility.HasActiveEvent(
                entityManager,
                factionEntity,
                DynastyPoliticalEventTypes.CovenantTestCooldown,
                CovenantTestRules.DurationInWorldDays);

            if (testState.TestPhase != CovenantTestPhase.Failed ||
                !hasCooldown ||
                faithState.Intensity > 60f)
            {
                lines.AppendLine("Phase 3 FAIL: missing-resource failure did not apply failure state, intensity loss, and cooldown event.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: missing resources failed the covenant test, cut intensity by 20, and pushed CovenantTestCooldown.");
            return true;
        }

        private static bool RunRetryBlockedPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("covenant-test-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 60f);

            var state = CovenantTestRules.CreateDefaultState();
            state.TestPhase = CovenantTestPhase.Failed;
            state.LastFailedAtInWorldDays = CovenantTestRules.DurationInWorldDays;
            entityManager.SetComponentData(factionEntity, state);
            DynastyPoliticalEventUtility.AddOrRefreshEvent(
                entityManager,
                factionEntity,
                DynastyPoliticalEventTypes.CovenantTestCooldown,
                CovenantTestRules.DurationInWorldDays + CovenantTestRules.RetryCooldownInWorldDays);

            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays + 30f);
            Tick(world);

            debugScope.CommandSurface.TryDebugSetFaithIntensity("player", 80f);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestState("player", out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read covenant-test state during cooldown.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "Phase") != nameof(CovenantTestPhase.Failed))
            {
                lines.AppendLine($"Phase 4 FAIL: cooldown window should block re-qualification, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: active CovenantTestCooldown blocked a retry from re-entering ReadyToTrigger.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CovenantTestQualificationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CovenantTestResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyPoliticalEventSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static Entity SeedFaction(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(DynastyStateComponent),
                typeof(DynastyPoliticalEventAggregateComponent),
                typeof(CovenantTestStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = 0f,
                Level = 0,
            });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Food = 120f,
                Influence = 120f,
            });
            entityManager.SetComponentData(entity, new PopulationComponent
            {
                Total = 12,
                Available = 12,
                Cap = 20,
                BaseCap = 20,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.SetComponentData(entity, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = 60f,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            entityManager.SetComponentData(entity, DynastyPoliticalEventUtility.CreateDefaultAggregate());
            entityManager.SetComponentData(entity, CovenantTestRules.CreateDefaultState());
            entityManager.AddBuffer<DynastyPoliticalEventComponent>(entity);
            entityManager.AddBuffer<FaithExposureElement>(entity);
            return entity;
        }

        private static void SetFaith(
            EntityManager entityManager,
            Entity factionEntity,
            CovenantId faithId,
            DoctrinePath doctrinePath,
            float intensity)
        {
            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            faithState.SelectedFaith = faithId;
            faithState.DoctrinePath = doctrinePath;
            faithState.Intensity = intensity;
            faithState.Level = FaithIntensityTiers.ResolveLevel(intensity);
            entityManager.SetComponentData(factionEntity, faithState);
        }

        private static void SetDualClock(EntityManager entityManager, float inWorldDays)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            if (query.IsEmpty)
            {
                var entity = entityManager.CreateEntity(typeof(DualClockComponent));
                entityManager.SetComponentData(entity, new DualClockComponent
                {
                    InWorldDays = inWorldDays,
                    DaysPerRealSecond = 2f,
                    DeclarationCount = 0,
                });
                return;
            }

            var dualClock = query.GetSingleton<DualClockComponent>();
            dualClock.InWorldDays = inWorldDays;
            query.SetSingleton(dualClock);
        }

        private static void Tick(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Dictionary<string, string> ParseFields(string readout)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parts = readout.Split('|');
            for (int i = 1; i < parts.Length; i++)
            {
                int separatorIndex = parts[i].IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                fields[parts[i][..separatorIndex]] = parts[i][(separatorIndex + 1)..];
            }

            return fields;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
#endif
