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
    public static class BloodlinesPlayerCaptiveRescueSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-captive-rescue-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Captive Rescue Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCaptiveRescueSmokeValidation() =>
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
                message = "Player captive rescue smoke validation errored: " + exception;
            }

            var artifact = "BLOODLINES_PLAYER_CAPTIVE_RESCUE_SMOKE " +
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
            ok &= RunNoOperativePhase(lines);
            ok &= RunCaptiveNotFoundPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-captive-rescue-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 80f);
            var playerFaction = SeedFaction(entityManager, "player", 100f, 80f);
            SeedFaction(entityManager, "enemy", 60f, 30f);
            var captiveMemberId = MarkPlayerCommanderCaptured(entityManager, playerFaction, "enemy");

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugDispatchCaptiveRescue("player", captiveMemberId.ToString()))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue player captive rescue request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSingleOperation(entityManager, DynastyOperationKind.CaptiveRescue, "player", out var operationEntity, out var operation) ||
                !entityManager.HasComponent<DynastyOperationCaptiveRescueComponent>(operationEntity))
            {
                lines.AppendLine("Phase 1 FAIL: captive rescue operation was not created.");
                return false;
            }

            var rescue = entityManager.GetComponentData<DynastyOperationCaptiveRescueComponent>(operationEntity);
            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);

            if (!operation.TargetFactionId.Equals(new FixedString32Bytes("enemy")) ||
                !operation.TargetMemberId.Equals(captiveMemberId) ||
                !Approximately(resources.Gold, 58f) ||
                !Approximately(resources.Influence, 54f) ||
                !Approximately(rescue.ResolveAtInWorldDays, 100f) ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: invalid rescue state target={operation.TargetFactionId}, member={operation.TargetMemberId}, gold={resources.Gold}, influence={resources.Influence}, resolve={rescue.ResolveAtInWorldDays}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 1 PASS: captive rescue created resolveAt={rescue.ResolveAtInWorldDays:0.##}, gold=100->58, influence=80->54.");
            return true;
        }

        private static bool RunNoOperativePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-captive-rescue-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 44f);
            var playerFaction = SeedFaction(entityManager, "player", 100f, 80f);
            SeedFaction(entityManager, "enemy", 60f, 30f);
            var captiveMemberId = MarkPlayerCommanderCaptured(entityManager, playerFaction, "enemy");
            DisableOperativeRoles(entityManager, playerFaction, DynastyRole.Spymaster, DynastyRole.Diplomat, DynastyRole.Merchant);

            if (!debugScope.CommandSurface.TryDebugDispatchCaptiveRescue("player", captiveMemberId.ToString()))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue no-operative player captive rescue request.");
                return false;
            }

            TickOnce(world);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            if (TryGetSingleOperation(entityManager, DynastyOperationKind.CaptiveRescue, "player", out _, out _) ||
                !Approximately(resources.Gold, 100f) ||
                !Approximately(resources.Influence, 80f))
            {
                lines.AppendLine("Phase 2 FAIL: missing operatives should block rescue dispatch and preserve resources.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: missing covert operatives blocked rescue dispatch.");
            return true;
        }

        private static bool RunCaptiveNotFoundPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-captive-rescue-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 52f);
            var playerFaction = SeedFaction(entityManager, "player", 100f, 80f);
            SeedFaction(entityManager, "enemy", 60f, 30f);
            MarkPlayerCommanderCaptured(entityManager, playerFaction, "enemy");

            if (!debugScope.CommandSurface.TryDebugDispatchCaptiveRescue("player", "missing-member"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue captive-not-found rescue request.");
                return false;
            }

            TickOnce(world);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            if (TryGetSingleOperation(entityManager, DynastyOperationKind.CaptiveRescue, "player", out _, out _) ||
                !Approximately(resources.Gold, 100f) ||
                !Approximately(resources.Influence, 80f))
            {
                lines.AppendLine("Phase 3 FAIL: missing captive should block rescue dispatch and preserve resources.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: captive-not-found gate blocked rescue dispatch.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCaptiveRescueDispatchSystem>());
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
            float gold,
            float influence)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = 120f,
                Water = 120f,
                Wood = 40f,
                Stone = 25f,
                Iron = 15f,
                Influence = influence,
            });

            DynastyBootstrap.AttachDynasty(entityManager, entity, new FixedString32Bytes(factionId));
            return entity;
        }

        private static FixedString64Bytes MarkPlayerCommanderCaptured(
            EntityManager entityManager,
            Entity playerFaction,
            string captorFactionId)
        {
            var roster = entityManager.GetBuffer<DynastyMemberRef>(playerFaction);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Commander)
                {
                    continue;
                }

                member.Status = DynastyMemberStatus.Captured;
                entityManager.SetComponentData(memberEntity, member);
                CapturedMemberHelpers.CaptureMember(
                    entityManager,
                    new FixedString32Bytes(captorFactionId),
                    member.MemberId,
                    member.Title,
                    new FixedString32Bytes("player"));
                return member.MemberId;
            }

            throw new InvalidOperationException("Commander member not found on seeded player dynasty.");
        }

        private static void DisableOperativeRoles(
            EntityManager entityManager,
            Entity factionEntity,
            params DynastyRole[] roles)
        {
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                for (int roleIndex = 0; roleIndex < roles.Length; roleIndex++)
                {
                    if (member.Role != roles[roleIndex])
                    {
                        continue;
                    }

                    member.Status = DynastyMemberStatus.Fallen;
                    entityManager.SetComponentData(memberEntity, member);
                    break;
                }
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

                hostObject = new GameObject("BloodlinesPlayerCaptiveRescueSmokeValidation_CommandSurface")
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
