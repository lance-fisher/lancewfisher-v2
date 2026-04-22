#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesPlayerMissionaryDispatchSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-missionary-dispatch-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Missionary Dispatch Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerMissionaryDispatchSmokeValidation() =>
            RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception exception)
            {
                success = false;
                message = "Player missionary dispatch smoke validation errored: " + exception;
            }

            var artifact = "BLOODLINES_PLAYER_MISSIONARY_DISPATCH_SMOKE " +
                           (success ? "PASS" : "FAIL") + Environment.NewLine + message;
            UnityDebug.Log(artifact);
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
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
            ok &= RunSuccessPhase(lines);
            ok &= RunInsufficientInfluencePhase(lines);
            ok &= RunCapacityFullPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-missionary-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 40f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                30f,
                CovenantId.OldLight,
                DoctrinePath.Light,
                32f,
                5);
            SeedFaction(
                entityManager,
                "enemy",
                50f,
                CovenantId.TheWild,
                DoctrinePath.Dark,
                18f,
                4);

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugIssuePlayerMissionaryDispatch("player", "enemy"))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue missionary dispatch request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSingleOperation(entityManager, DynastyOperationKind.Missionary, "player", out var operationEntity, out var operation) ||
                !entityManager.HasComponent<DynastyOperationMissionaryComponent>(operationEntity))
            {
                lines.AppendLine("Phase 1 FAIL: missionary operation was not created.");
                return false;
            }

            var missionary = entityManager.GetComponentData<DynastyOperationMissionaryComponent>(operationEntity);
            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            var faith = entityManager.GetComponentData<FaithStateComponent>(playerFaction);
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);

            if (!operation.TargetFactionId.Equals(new FixedString32Bytes("enemy")) ||
                !Approximately(missionary.ResolveAtInWorldDays, 72f) ||
                !Approximately(resources.Influence, 16f) ||
                !Approximately(faith.Intensity, 20f) ||
                !missionary.SourceFaithId.Equals(new FixedString64Bytes("old_light")) ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: invalid missionary state target={operation.TargetFactionId}, resolve={missionary.ResolveAtInWorldDays}, influence={resources.Influence}, intensity={faith.Intensity}, sourceFaith={missionary.SourceFaithId}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 1 PASS: missionary created resolveAt={missionary.ResolveAtInWorldDays:0.##}, influence=30->16, intensity=32->20.");
            return true;
        }

        private static bool RunInsufficientInfluencePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-missionary-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 65f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                10f,
                CovenantId.TheOrder,
                DoctrinePath.Light,
                20f,
                5);
            SeedFaction(
                entityManager,
                "enemy",
                50f,
                CovenantId.TheWild,
                DoctrinePath.Dark,
                25f,
                4);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMissionaryDispatch("player", "enemy"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue insufficient-influence missionary request.");
                return false;
            }

            TickOnce(world);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            if (TryGetSingleOperation(entityManager, DynastyOperationKind.Missionary, "player", out _, out _) ||
                !Approximately(resources.Influence, 10f))
            {
                lines.AppendLine("Phase 2 FAIL: insufficient influence should block missionary creation and preserve stockpile.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: insufficient influence blocked missionary dispatch.");
            return true;
        }

        private static bool RunCapacityFullPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-missionary-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 90f);
            SeedFaction(
                entityManager,
                "player",
                50f,
                CovenantId.BloodDominion,
                DoctrinePath.Dark,
                40f,
                5);
            SeedFaction(
                entityManager,
                "enemy",
                40f,
                CovenantId.OldLight,
                DoctrinePath.Light,
                20f,
                4);
            SeedActiveOperations(entityManager, "player", 6, 90f);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMissionaryDispatch("player", "enemy"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue cap-full missionary request.");
                return false;
            }

            TickOnce(world);

            int count = CountActiveOperations(entityManager, "player", DynastyOperationKind.Missionary);
            int total = CountAllActiveOperations(entityManager, "player");
            if (count != 0 || total != 6)
            {
                lines.AppendLine($"Phase 3 FAIL: capacity-full request should leave active-op count at 6, got missionary={count}, total={total}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: active-cap gate blocked missionary dispatch at six active operations.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerMissionaryDispatchSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var clockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
            entityManager.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float influence,
            CovenantId selectedFaith,
            DoctrinePath doctrinePath,
            float intensity,
            int level)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(FaithStateComponent),
                typeof(HostilityComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Gold = 120f,
                Food = 120f,
                Water = 120f,
                Wood = 40f,
                Stone = 25f,
                Iron = 15f,
                Influence = influence,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = selectedFaith,
                DoctrinePath = doctrinePath,
                Intensity = intensity,
                Level = level,
            });

            DynastyBootstrap.AttachDynasty(entityManager, entity, new FixedString32Bytes(factionId));
            return entity;
        }

        private static void SeedActiveOperations(
            EntityManager entityManager,
            string sourceFactionId,
            int count,
            float inWorldDays)
        {
            var sourceFaction = new FixedString32Bytes(sourceFactionId);
            for (int i = 0; i < count; i++)
            {
                var entity = entityManager.CreateEntity(typeof(DynastyOperationComponent));
                entityManager.SetComponentData(entity, new DynastyOperationComponent
                {
                    OperationId = new FixedString64Bytes($"seed-op-{i}"),
                    SourceFactionId = sourceFaction,
                    OperationKind = DynastyOperationKind.HolyWar,
                    StartedAtInWorldDays = inWorldDays - 1f,
                    TargetFactionId = new FixedString32Bytes($"enemy{i}"),
                    Active = true,
                });
            }
        }

        private static bool TryGetSingleOperation(
            EntityManager entityManager,
            DynastyOperationKind kind,
            string sourceFactionId,
            out Entity operationEntity,
            out DynastyOperationComponent operation)
        {
            operationEntity = Entity.Null;
            operation = default;
            var sourceKey = new FixedString32Bytes(sourceFactionId);

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            if (query.IsEmpty)
            {
                return false;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active &&
                    operations[i].OperationKind == kind &&
                    operations[i].SourceFactionId.Equals(sourceKey))
                {
                    operationEntity = entities[i];
                    operation = operations[i];
                    return true;
                }
            }

            return false;
        }

        private static int CountActiveOperations(
            EntityManager entityManager,
            string sourceFactionId,
            DynastyOperationKind kind)
        {
            var sourceKey = new FixedString32Bytes(sourceFactionId);
            int count = 0;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active &&
                    operations[i].SourceFactionId.Equals(sourceKey) &&
                    operations[i].OperationKind == kind)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountAllActiveOperations(EntityManager entityManager, string sourceFactionId)
        {
            var sourceKey = new FixedString32Bytes(sourceFactionId);
            int count = 0;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active && operations[i].SourceFactionId.Equals(sourceKey))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool Approximately(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= 0.01f;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;

                hostObject = new GameObject("BloodlinesPlayerMissionaryDispatchSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
