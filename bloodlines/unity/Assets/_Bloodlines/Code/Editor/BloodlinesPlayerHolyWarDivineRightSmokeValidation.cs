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
    public static class BloodlinesPlayerHolyWarDivineRightSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-holy-war-divine-right-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Holy War And Divine Right Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerHolyWarDivineRightSmokeValidation() =>
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
                message = "Player holy war/divine right smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_HOLY_WAR_DIVINE_RIGHT_SMOKE " +
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
            ok &= RunHolyWarSuccessPhase(lines);
            ok &= RunHolyWarSameFaithBlockPhase(lines);
            ok &= RunDivineRightSuccessPhase(lines);
            ok &= RunDivineRightLowIntensityBlockPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunHolyWarSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-holy-war-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 50f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                80f,
                CovenantId.OldLight,
                DoctrinePath.Light,
                30f,
                5);
            SeedFaction(
                entityManager,
                "enemy",
                40f,
                CovenantId.TheWild,
                DoctrinePath.Dark,
                44f,
                5);

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugIssuePlayerHolyWarDeclaration("player", "enemy"))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue holy war declaration request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSingleOperation(entityManager, DynastyOperationKind.HolyWar, "player", out var operationEntity, out var operation) ||
                !entityManager.HasComponent<DynastyOperationHolyWarComponent>(operationEntity))
            {
                lines.AppendLine("Phase 1 FAIL: holy war operation was not created.");
                return false;
            }

            var perKind = entityManager.GetComponentData<DynastyOperationHolyWarComponent>(operationEntity);
            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            var faith = entityManager.GetComponentData<FaithStateComponent>(playerFaction);
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);

            if (!operation.TargetFactionId.Equals(new FixedString32Bytes("enemy")) ||
                !Approximately(perKind.ResolveAtInWorldDays, 68f) ||
                !Approximately(perKind.WarExpiresAtInWorldDays, 230f) ||
                !Approximately(resources.Influence, 56f) ||
                !Approximately(faith.Intensity, 12f) ||
                !Approximately(perKind.IntensityPulse, 0.9f) ||
                !Approximately(perKind.LoyaltyPulse, 1.2f) ||
                !perKind.CompatibilityLabel.Equals(new FixedString64Bytes("discordant")) ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: invalid holy war state target={operation.TargetFactionId}, resolve={perKind.ResolveAtInWorldDays}, warExpire={perKind.WarExpiresAtInWorldDays}, influence={resources.Influence}, intensity={faith.Intensity}, pulses={perKind.IntensityPulse}/{perKind.LoyaltyPulse}, label={perKind.CompatibilityLabel}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 1 PASS: holy war created resolveAt={perKind.ResolveAtInWorldDays:0.##}, warExpiresAt={perKind.WarExpiresAtInWorldDays:0.##}, influence=80->56, intensity=30->12.");
            return true;
        }

        private static bool RunHolyWarSameFaithBlockPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-holy-war-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 70f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                90f,
                CovenantId.TheOrder,
                DoctrinePath.Light,
                40f,
                5);
            SeedFaction(
                entityManager,
                "enemy",
                55f,
                CovenantId.TheOrder,
                DoctrinePath.Light,
                50f,
                5);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerHolyWarDeclaration("player", "enemy"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue same-faith holy war request.");
                return false;
            }

            TickOnce(world);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            if (TryGetSingleOperation(entityManager, DynastyOperationKind.HolyWar, "player", out _, out _) ||
                !Approximately(resources.Influence, 90f))
            {
                lines.AppendLine("Phase 2 FAIL: same-faith target should block holy war creation and preserve influence.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: harmonious same-faith target blocked holy war and preserved influence.");
            return true;
        }

        private static bool RunDivineRightSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-divine-right-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 100f);
            SeedFaction(
                entityManager,
                "player",
                70f,
                CovenantId.TheOrder,
                DoctrinePath.Light,
                85f,
                5);

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugIssuePlayerDivineRightDeclaration("player"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue divine right declaration request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSingleOperation(entityManager, DynastyOperationKind.DivineRight, "player", out var operationEntity, out var operation) ||
                !entityManager.HasComponent<DynastyOperationDivineRightComponent>(operationEntity))
            {
                lines.AppendLine("Phase 3 FAIL: divine right operation was not created.");
                return false;
            }

            var perKind = entityManager.GetComponentData<DynastyOperationDivineRightComponent>(operationEntity);
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);
            if (operation.TargetFactionId.Length != 0 ||
                !Approximately(perKind.ResolveAtInWorldDays, 280f) ||
                !perKind.SourceFaithId.Equals(new FixedString64Bytes("the_order")) ||
                perKind.DoctrinePath != DoctrinePath.Light ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: invalid divine right state target='{operation.TargetFactionId}', resolve={perKind.ResolveAtInWorldDays}, sourceFaith={perKind.SourceFaithId}, doctrine={perKind.DoctrinePath}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 3 PASS: divine right created resolveAt={perKind.ResolveAtInWorldDays:0.##}, sourceFaith={perKind.SourceFaithId}, doctrine={perKind.DoctrinePath}.");
            return true;
        }

        private static bool RunDivineRightLowIntensityBlockPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-divine-right-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 120f);
            SeedFaction(
                entityManager,
                "player",
                70f,
                CovenantId.BloodDominion,
                DoctrinePath.Dark,
                60f,
                5);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerDivineRightDeclaration("player"))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue low-intensity divine right request.");
                return false;
            }

            TickOnce(world);

            if (TryGetSingleOperation(entityManager, DynastyOperationKind.DivineRight, "player", out _, out _))
            {
                lines.AppendLine("Phase 4 FAIL: low-intensity divine right request should not create an operation.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: intensity below 80 blocked divine right declaration.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerHolyWarDeclarationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerDivineRightDeclarationSystem>());
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

                hostObject = new GameObject("BloodlinesPlayerHolyWarDivineRightSmokeValidation_CommandSurface")
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
