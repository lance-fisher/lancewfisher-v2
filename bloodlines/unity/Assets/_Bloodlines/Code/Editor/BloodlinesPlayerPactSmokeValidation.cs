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
    public static class BloodlinesPlayerPactSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-player-pact-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Pact Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerPactSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Player pact smoke validation errored: " + exception;
            }

            var artifact = "BLOODLINES_PLAYER_PACT_SMOKE " +
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
            ok &= RunProposalSuccessPhase(lines);
            ok &= RunDuplicatePactBlockPhase(lines);
            ok &= RunBreakPenaltyPhase(lines);
            ok &= RunInsufficientResourcePhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunProposalSuccessPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-pact-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 40f);
            var playerFaction = SeedFaction(entityManager, "player", 80f, 120f, 70f, 6f);
            var enemyFaction = SeedFaction(entityManager, "enemy", 40f, 120f, 55f, 3f);
            SeedMutualHostility(entityManager, playerFaction, "enemy", enemyFaction, "player");

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugIssuePlayerPactProposal("player", "enemy"))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue player pact proposal request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSinglePact(entityManager, "player", "enemy", out _, out var pact))
            {
                lines.AppendLine("Phase 1 FAIL: pact entity was not created.");
                return false;
            }

            var playerResources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            bool playerStillHostile = PlayerPactUtility.IsHostile(entityManager, playerFaction, new FixedString32Bytes("enemy"));
            bool enemyStillHostile = PlayerPactUtility.IsHostile(entityManager, enemyFaction, new FixedString32Bytes("player"));
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);
            if (!Approximately(playerResources.Influence, 30f) ||
                !Approximately(playerResources.Gold, 40f) ||
                !Approximately(pact.StartedAtInWorldDays, 40f) ||
                !Approximately(pact.MinimumExpiresAtInWorldDays, 220f) ||
                playerStillHostile ||
                enemyStillHostile ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: invalid pact state influence={playerResources.Influence}, gold={playerResources.Gold}, startedAt={pact.StartedAtInWorldDays}, minExpire={pact.MinimumExpiresAtInWorldDays}, hostilityPlayer={playerStillHostile}, hostilityEnemy={enemyStillHostile}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: proposal created pact, deducted 50 influence and 80 gold, and cleared hostility.");
            return true;
        }

        private static bool RunDuplicatePactBlockPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-pact-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 60f);
            var playerFaction = SeedFaction(entityManager, "player", 90f, 130f, 65f, 5f);
            var enemyFaction = SeedFaction(entityManager, "enemy", 40f, 120f, 50f, 2f);
            SeedMutualHostility(entityManager, playerFaction, "enemy", enemyFaction, "player");
            SeedPact(entityManager, "player", "enemy", 10f, 190f);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerPactProposal("player", "enemy"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue duplicate pact request.");
                return false;
            }

            TickOnce(world);

            var playerResources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            int pactCount = CountPacts(entityManager, "player", "enemy");
            if (pactCount != 1 ||
                !Approximately(playerResources.Influence, 90f) ||
                !Approximately(playerResources.Gold, 130f))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: existing pact should block duplicate proposal. pacts={pactCount}, influence={playerResources.Influence}, gold={playerResources.Gold}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: active pact blocked duplicate proposal and preserved stockpile.");
            return true;
        }

        private static bool RunBreakPenaltyPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-pact-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 100f);
            var playerFaction = SeedFaction(entityManager, "player", 90f, 130f, 70f, 5f);
            var enemyFaction = SeedFaction(entityManager, "enemy", 40f, 120f, 55f, 2f);
            SeedPact(entityManager, "player", "enemy", 20f, 200f);

            int messagesBefore = NarrativeMessageBridge.Count(entityManager);
            if (!debugScope.CommandSurface.TryDebugIssuePlayerPactBreak("player", "enemy"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue pact break request.");
                return false;
            }

            TickOnce(world);

            if (!TryGetSinglePactIncludingBroken(entityManager, "player", "enemy", out var pact))
            {
                lines.AppendLine("Phase 3 FAIL: pact was not found after break.");
                return false;
            }

            var dynastyState = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var conviction = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            bool playerHostile = PlayerPactUtility.IsHostile(entityManager, playerFaction, new FixedString32Bytes("enemy"));
            bool enemyHostile = PlayerPactUtility.IsHostile(entityManager, enemyFaction, new FixedString32Bytes("player"));
            int messagesAfter = NarrativeMessageBridge.Count(entityManager);
            if (!pact.Broken ||
                !pact.BrokenByFactionId.Equals(new FixedString32Bytes("player")) ||
                !playerHostile ||
                !enemyHostile ||
                !Approximately(dynastyState.Legitimacy, 62f) ||
                !Approximately(conviction.Oathkeeping, 3f) ||
                messagesAfter - messagesBefore != 1)
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: broken={pact.Broken}, brokenBy={pact.BrokenByFactionId}, hostilityPlayer={playerHostile}, hostilityEnemy={enemyHostile}, legitimacy={dynastyState.Legitimacy}, oathkeeping={conviction.Oathkeeping}, messages={messagesAfter - messagesBefore}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: early break restored hostility and applied legitimacy/oathkeeping penalties.");
            return true;
        }

        private static bool RunInsufficientResourcePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-pact-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 140f);
            var playerFaction = SeedFaction(entityManager, "player", 20f, 70f, 70f, 4f);
            var enemyFaction = SeedFaction(entityManager, "enemy", 40f, 120f, 50f, 2f);
            SeedMutualHostility(entityManager, playerFaction, "enemy", enemyFaction, "player");

            if (!debugScope.CommandSurface.TryDebugIssuePlayerPactProposal("player", "enemy"))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue insufficient-resource pact request.");
                return false;
            }

            TickOnce(world);

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            int pactCount = CountPacts(entityManager, "player", "enemy");
            if (pactCount != 0 ||
                !Approximately(resources.Influence, 20f) ||
                !Approximately(resources.Gold, 70f))
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: insufficient resources should block proposal. pacts={pactCount}, influence={resources.Influence}, gold={resources.Gold}.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: insufficient influence/gold blocked player pact proposal.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerPactProposalSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerPactBreakSystem>());
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
            float gold,
            float legitimacy,
            float oathkeeping)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(ConvictionComponent));
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
            entityManager.SetComponentData(entity, new ConvictionComponent
            {
                Oathkeeping = oathkeeping,
                Stewardship = 4f,
                Ruthlessness = 0f,
                Desecration = 0f,
                Score = 0f,
                Band = ConvictionBand.Neutral,
            });
            DynastyBootstrap.AttachDynasty(entityManager, entity, new FixedString32Bytes(factionId));
            var dynastyState = entityManager.GetComponentData<DynastyStateComponent>(entity);
            dynastyState.Legitimacy = legitimacy;
            entityManager.SetComponentData(entity, dynastyState);
            entityManager.AddBuffer<HostilityComponent>(entity);
            return entity;
        }

        private static void SeedMutualHostility(
            EntityManager entityManager,
            Entity factionA,
            string factionBId,
            Entity factionB,
            string factionAId)
        {
            entityManager.GetBuffer<HostilityComponent>(factionA).Add(new HostilityComponent
            {
                HostileFactionId = new FixedString32Bytes(factionBId),
            });
            entityManager.GetBuffer<HostilityComponent>(factionB).Add(new HostilityComponent
            {
                HostileFactionId = new FixedString32Bytes(factionAId),
            });
        }

        private static void SeedPact(
            EntityManager entityManager,
            string factionAId,
            string factionBId,
            float startedAtInWorldDays,
            float minimumExpiresAtInWorldDays)
        {
            var pactEntity = entityManager.CreateEntity(typeof(PactComponent));
            entityManager.SetComponentData(pactEntity, new PactComponent
            {
                PactId = new FixedString64Bytes($"nap-{factionAId}-{factionBId}-seed"),
                FactionAId = new FixedString32Bytes(factionAId),
                FactionBId = new FixedString32Bytes(factionBId),
                StartedAtInWorldDays = startedAtInWorldDays,
                MinimumExpiresAtInWorldDays = minimumExpiresAtInWorldDays,
                Broken = false,
                BrokenByFactionId = default,
            });
        }

        private static bool TryGetSinglePact(
            EntityManager entityManager,
            string factionAId,
            string factionBId,
            out Entity pactEntity,
            out PactComponent pact)
        {
            return PlayerPactUtility.TryFindActivePact(
                entityManager,
                new FixedString32Bytes(factionAId),
                new FixedString32Bytes(factionBId),
                out pactEntity,
                out pact);
        }

        private static bool TryGetSinglePactIncludingBroken(
            EntityManager entityManager,
            string factionAId,
            string factionBId,
            out PactComponent pact)
        {
            pact = default;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            if (query.IsEmpty)
            {
                return false;
            }

            using var pacts = query.ToComponentDataArray<PactComponent>(Allocator.Temp);
            var factionAKey = new FixedString32Bytes(factionAId);
            var factionBKey = new FixedString32Bytes(factionBId);
            for (int i = 0; i < pacts.Length; i++)
            {
                bool forward = pacts[i].FactionAId.Equals(factionAKey) && pacts[i].FactionBId.Equals(factionBKey);
                bool reverse = pacts[i].FactionAId.Equals(factionBKey) && pacts[i].FactionBId.Equals(factionAKey);
                if (forward || reverse)
                {
                    pact = pacts[i];
                    return true;
                }
            }

            return false;
        }

        private static int CountPacts(EntityManager entityManager, string factionAId, string factionBId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            if (query.IsEmpty)
            {
                return 0;
            }

            int count = 0;
            using var pacts = query.ToComponentDataArray<PactComponent>(Allocator.Temp);
            var factionAKey = new FixedString32Bytes(factionAId);
            var factionBKey = new FixedString32Bytes(factionBId);
            for (int i = 0; i < pacts.Length; i++)
            {
                bool forward = pacts[i].FactionAId.Equals(factionAKey) && pacts[i].FactionBId.Equals(factionBKey);
                bool reverse = pacts[i].FactionAId.Equals(factionBKey) && pacts[i].FactionBId.Equals(factionAKey);
                if ((forward || reverse) && !pacts[i].Broken)
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

                hostObject = new GameObject("BloodlinesPlayerPactSmokeValidation_CommandSurface")
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
