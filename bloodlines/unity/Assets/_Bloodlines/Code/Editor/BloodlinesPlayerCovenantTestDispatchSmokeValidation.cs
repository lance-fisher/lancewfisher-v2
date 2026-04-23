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
    public static class BloodlinesPlayerCovenantTestDispatchSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-player-covenant-test-dispatch-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Player Covenant Test Dispatch Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCovenantTestDispatchSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Player covenant test dispatch smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_PLAYER_COVENANT_TEST_DISPATCH_SMOKE " +
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
            ok &= RunAvailabilityPhase(lines);
            ok &= RunQueuePhase(lines);
            ok &= RunResolutionPhase(lines);
            ok &= RunUnaffordablePhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunAvailabilityPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covenant-dispatch-availability", includeResolution: false);
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 80f);

            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestDispatchState("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read covenant-test dispatch state.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "ActionAvailable") != "True" ||
                ReadField(fields, "CanAfford") != "True" ||
                ReadField(fields, "Phase") != nameof(CovenantTestPhase.ReadyToTrigger) ||
                ReadField(fields, "ActionLabel") != "Conduct Covenant Rite" ||
                ReadField(fields, "FoodCost") != "45" ||
                ReadField(fields, "InfluenceCost") != "18")
            {
                lines.AppendLine($"Phase 1 FAIL: ready player dispatch state was not exposed correctly: '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: ready-to-trigger Blood Dominion light state exposed action availability and rite costs.");
            return true;
        }

        private static bool RunQueuePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covenant-dispatch-queue", includeResolution: false);
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 80f);

            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugQueueCovenantTestDispatch("player"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue the covenant-test dispatch request.");
                return false;
            }

            Tick(world);

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerCovenantTestRequestComponent>());
            using var requests = query.ToComponentDataArray<PlayerCovenantTestRequestComponent>(Allocator.Temp);
            if (requests.Length != 1 || !requests[0].SourceFactionId.Equals(new FixedString32Bytes("player")))
            {
                lines.AppendLine("Phase 2 FAIL: dispatch system did not create the player covenant-test request entity.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestDispatchState("player", out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: dispatch state disappeared after queueing.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "RequestQueued") != "False" ||
                ReadField(fields, "RequestPending") != "True")
            {
                lines.AppendLine($"Phase 2 FAIL: queued request state was not surfaced correctly: '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: queueing the player dispatch created a request entity and surfaced pending state.");
            return true;
        }

        private static bool RunResolutionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covenant-dispatch-resolution", includeResolution: true);
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Light, 80f);

            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugQueueCovenantTestDispatch("player"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue the covenant-test dispatch for resolution.");
                return false;
            }

            Tick(world);

            var testState = entityManager.GetComponentData<CovenantTestStateComponent>(factionEntity);
            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            if (testState.TestPhase != CovenantTestPhase.Complete ||
                resources.Food > 75f ||
                resources.Influence > 102f)
            {
                lines.AppendLine("Phase 3 FAIL: dispatch request did not resolve through the existing covenant-test resolution system.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: queued dispatch resolved through CovenantTestResolutionSystem and spent the rite costs.");
            return true;
        }

        private static bool RunUnaffordablePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covenant-dispatch-unaffordable", includeResolution: false);
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;
            var factionEntity = SeedFaction(entityManager, "player");
            SetFaith(entityManager, factionEntity, CovenantId.BloodDominion, DoctrinePath.Dark, 80f);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            resources.Influence = 4f;
            entityManager.SetComponentData(factionEntity, resources);

            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = 3f;
            entityManager.SetComponentData(factionEntity, dynasty);

            var population = entityManager.GetComponentData<PopulationComponent>(factionEntity);
            population.Total = 3;
            population.Available = 3;
            entityManager.SetComponentData(factionEntity, population);

            SetDualClock(entityManager, 0f);
            Tick(world);
            SetDualClock(entityManager, CovenantTestRules.DurationInWorldDays);
            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestDispatchState("player", out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read covenant-test dispatch state for the unaffordable rite.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "ActionAvailable") != "False" ||
                ReadField(fields, "CanAfford") != "False" ||
                ReadField(fields, "ActionLabel") != "Offer Binding Sacrifice" ||
                string.IsNullOrWhiteSpace(ReadField(fields, "BlockReason")))
            {
                lines.AppendLine($"Phase 4 FAIL: unaffordable rite state was not exposed correctly: '{readout}'.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugQueueCovenantTestDispatch("player"))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue the unaffordable covenant-test dispatch.");
                return false;
            }

            Tick(world);

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerCovenantTestRequestComponent>());
            if (!query.IsEmpty)
            {
                lines.AppendLine("Phase 4 FAIL: unaffordable covenant-test state should not create a request entity.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: unaffordable rites surface cost blockers and do not emit a player request.");
            return true;
        }

        private static World CreateValidationWorld(string worldName, bool includeResolution)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CovenantTestQualificationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCovenantTestDispatchSystem>());
            if (includeResolution)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CovenantTestResolutionSystem>());
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyPoliticalEventSystem>());
            }

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
