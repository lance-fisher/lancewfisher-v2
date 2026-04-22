#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerCovertOps;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for player covert ops foundation. Proves:
    /// 1. baseline no player covert ops
    /// 2. successful espionage dispatch creates an op and deducts cost
    /// 3. insufficient influence blocks dispatch
    /// 4. total active operations do not exceed the canonical limit of 6
    /// </summary>
    public static class BloodlinesPlayerCovertOpsSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-covert-ops-smoke.log";

        [MenuItem("Bloodlines/Player Covert Ops/Run Player Covert Ops Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCovertOpsSmokeValidation() =>
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
                message = "Player covert ops smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_COVERT_OPS_SMOKE " +
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
            ok &= RunBaselinePhase(lines);
            ok &= RunSuccessfulDispatchPhase(lines);
            ok &= RunInsufficientInfluencePhase(lines);
            ok &= RunOperationCapPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-ops-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            SeedFaction(entityManager, "player", gold: 200f, influence: 80f, spymasterRenown: 14f);
            SeedFaction(entityManager, "enemy", gold: 120f, influence: 20f, spymasterRenown: 9f, fortificationTier: 2);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerCovertOps("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read player covert ops.");
                return false;
            }

            if (!readout.Contains("ActivePlayerCovertOpCount=0", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 1 FAIL: expected zero ops, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: ActivePlayerCovertOpCount=0");
            return true;
        }

        private static bool RunSuccessfulDispatchPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-ops-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 200f, influence: 80f, spymasterRenown: 18f);
            SeedFaction(entityManager, "enemy", gold: 80f, influence: 15f, spymasterRenown: 6f, fortificationTier: 1);
            var resourcesBefore = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            string expectedOperatorMemberId = GetMemberIdByRole(entityManager, playerFaction, DynastyRole.Spymaster);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue espionage request.");
                return false;
            }

            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerCovertOps("player", out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read ops after dispatch.");
                return false;
            }

            var resourcesAfter = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            int activeOps = CountActivePlayerOperations(entityManager, "player");
            if (activeOps != 1 ||
                !readout.Contains("ActivePlayerCovertOpCount=1", StringComparison.Ordinal) ||
                !readout.Contains("Kind=Espionage", StringComparison.Ordinal) ||
                !readout.Contains("OperatorMemberId=" + expectedOperatorMemberId, StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 2 FAIL: expected one espionage op, got '{readout}'.");
                return false;
            }

            if (resourcesAfter.Gold != resourcesBefore.Gold - PlayerCovertOpsSystem.EspionageCostGold ||
                resourcesAfter.Influence != resourcesBefore.Influence - PlayerCovertOpsSystem.EspionageCostInfluence)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: expected cost deduction to gold={resourcesBefore.Gold - PlayerCovertOpsSystem.EspionageCostGold} " +
                    $"influence={resourcesBefore.Influence - PlayerCovertOpsSystem.EspionageCostInfluence}, got gold={resourcesAfter.Gold} influence={resourcesAfter.Influence}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: espionage created, gold={resourcesAfter.Gold}, influence={resourcesAfter.Influence}, readout='{readout}'");
            return true;
        }

        private static bool RunInsufficientInfluencePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-ops-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 28f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 200f, influence: 10f, spymasterRenown: 18f);
            SeedFaction(entityManager, "enemy", gold: 80f, influence: 15f, spymasterRenown: 6f, fortificationTier: 1);
            var resourcesBefore = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue espionage request.");
                return false;
            }

            TickOnce(world);

            var resourcesAfter = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            int activeOps = CountActivePlayerOperations(entityManager, "player");
            if (activeOps != 0)
            {
                lines.AppendLine($"Phase 3 FAIL: insufficient influence should block op creation, got {activeOps} active ops.");
                return false;
            }

            if (resourcesAfter.Gold != resourcesBefore.Gold ||
                resourcesAfter.Influence != resourcesBefore.Influence)
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: insufficient influence should preserve resources, got gold={resourcesAfter.Gold} influence={resourcesAfter.Influence}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: insufficient influence blocked dispatch and preserved stockpile");
            return true;
        }

        private static bool RunOperationCapPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-ops-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 32f);
            SeedFaction(entityManager, "player", gold: 500f, influence: 200f, spymasterRenown: 20f);
            for (int i = 1; i <= DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT + 1; i++)
            {
                SeedFaction(
                    entityManager,
                    "enemy" + i,
                    gold: 100f,
                    influence: 10f,
                    spymasterRenown: 4f + i,
                    fortificationTier: i % 3);
            }

            for (int i = 1; i <= DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT + 1; i++)
            {
                if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy" + i))
                {
                    lines.AppendLine($"Phase 4 FAIL: could not queue espionage request for enemy{i}.");
                    return false;
                }
            }

            TickOnce(world);

            int activeOps = CountActivePlayerOperations(entityManager, "player");
            if (!debugScope.CommandSurface.TryDebugGetPlayerCovertOps("player", out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read covert ops after cap test.");
                return false;
            }

            if (activeOps != DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT ||
                !readout.Contains("ActivePlayerCovertOpCount=6", StringComparison.Ordinal))
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: expected cap {DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT}, got {activeOps} with readout '{readout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 4 PASS: active espionage ops capped at {activeOps} with readout '{readout}'");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCovertOpsSystem>());
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
            float influence,
            float spymasterRenown,
            int fortificationTier = 0)
        {
            var factionEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(RealmConditionComponent));
            entityManager.SetComponentData(factionEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(factionEntity, new PopulationComponent
            {
                Total = 24,
                Cap = 30,
                BaseCap = 30,
                CapBonus = 0,
                Available = 12,
                GrowthAccumulator = 0f,
            });
            entityManager.SetComponentData(factionEntity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = 120f,
                Water = 120f,
                Wood = 40f,
                Stone = 35f,
                Iron = 15f,
                Influence = influence,
            });
            entityManager.SetComponentData(factionEntity, new RealmConditionComponent());

            DynastyBootstrap.AttachDynasty(entityManager, factionEntity, new FixedString32Bytes(factionId));
            SetSpymasterRenown(entityManager, factionEntity, spymasterRenown, factionId);
            SeedSettlement(entityManager, factionId, fortificationTier);
            return factionEntity;
        }

        private static void SeedSettlement(
            EntityManager entityManager,
            string factionId,
            int fortificationTier)
        {
            var settlementEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(SettlementComponent));
            entityManager.SetComponentData(settlementEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(settlementEntity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes("keep_" + factionId),
                SettlementClassId = new FixedString32Bytes("military_fort"),
                FortificationTier = fortificationTier,
                FortificationCeiling = math.max(1, fortificationTier + 1),
            });
        }

        private static void SetSpymasterRenown(
            EntityManager entityManager,
            Entity factionEntity,
            float renown,
            string factionId)
        {
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                throw new InvalidOperationException("Dynasty buffer missing for faction " + factionId);
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Spymaster)
                {
                    continue;
                }

                member.Renown = renown;
                entityManager.SetComponentData(memberEntity, member);
                return;
            }

            throw new InvalidOperationException("Spymaster not found for faction " + factionId);
        }

        private static string GetMemberIdByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role)
        {
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                throw new InvalidOperationException("Dynasty buffer missing when resolving role " + role);
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role == role)
                {
                    return member.MemberId.ToString();
                }
            }

            throw new InvalidOperationException("Dynasty role not found: " + role);
        }

        private static int CountActivePlayerOperations(
            EntityManager entityManager,
            string factionId)
        {
            var factionKey = new FixedString32Bytes(factionId);
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();

            int count = 0;
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active && operations[i].SourceFactionId.Equals(factionKey))
                {
                    count++;
                }
            }

            return count;
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

                hostObject = new GameObject("BloodlinesPlayerCovertOpsSmokeValidation_CommandSurface")
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
