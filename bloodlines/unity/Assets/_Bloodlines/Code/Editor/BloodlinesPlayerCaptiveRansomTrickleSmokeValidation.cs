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
    public static class BloodlinesPlayerCaptiveRansomTrickleSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-captive-ransom-trickle-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Captive Ransom Trickle Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCaptiveRansomTrickleSmokeValidation() =>
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
                message = "Player captive ransom trickle smoke validation errored: " + exception;
            }

            var artifact = "BLOODLINES_PLAYER_CAPTIVE_RANSOM_TRICKLE_SMOKE " +
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
            ok &= RunHighVersusLowRenownPhase(lines);
            ok &= RunNoCaptivesPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunHighVersusLowRenownPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-captive-ransom-trickle-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var clockEntity = SeedDualClock(entityManager, 100.2f);
            var sourceHigh = SeedFaction(entityManager, "source_high", 70f, 12f, 4f);
            var sourceLow = SeedFaction(entityManager, "source_low", 70f, 12f, 4f);
            var holderHigh = SeedFaction(entityManager, "holder_high", 40f, 16f, 8f);
            var holderLow = SeedFaction(entityManager, "holder_low", 40f, 16f, 8f);

            SeedHeldCaptive(entityManager, sourceHigh, "holder_high", DynastyRole.Commander, 30f);
            SeedHeldCaptive(entityManager, sourceLow, "holder_low", DynastyRole.Commander, 4f);

            TickAtDay(world, entityManager, clockEntity, 100.2f);
            TickAtDay(world, entityManager, clockEntity, 101.2f);

            var holderHighResources = entityManager.GetComponentData<ResourceStockpileComponent>(holderHigh);
            var holderLowResources = entityManager.GetComponentData<ResourceStockpileComponent>(holderLow);
            var holderHighRenown = entityManager.GetComponentData<DynastyRenownComponent>(holderHigh);
            var holderLowRenown = entityManager.GetComponentData<DynastyRenownComponent>(holderLow);
            var holderHighTrickle = entityManager.GetComponentData<CaptiveRansomTrickleComponent>(holderHigh);
            var holderLowTrickle = entityManager.GetComponentData<CaptiveRansomTrickleComponent>(holderLow);

            float expectedHighInfluenceDelta = ResolveExpectedInfluenceDelta(30f, 0.5f);
            float expectedLowInfluenceDelta = ResolveExpectedInfluenceDelta(4f, 0.5f);
            float expectedHighRenownDelta = ResolveExpectedRenownDelta(30f, 0.5f);
            float expectedLowRenownDelta = ResolveExpectedRenownDelta(4f, 0.5f);

            if (!debugScope.CommandSurface.TryDebugGetCaptiveTrickle("holder_high", out var readout) ||
                !readout.Contains("HeldCaptives=1", StringComparison.Ordinal) ||
                !Approximately(holderHighResources.Influence, 16f + expectedHighInfluenceDelta) ||
                !Approximately(holderLowResources.Influence, 16f + expectedLowInfluenceDelta) ||
                !Approximately(holderHighRenown.RenownScore, 8f + expectedHighRenownDelta) ||
                !Approximately(holderLowRenown.RenownScore, 8f + expectedLowRenownDelta) ||
                holderHighResources.Influence <= holderLowResources.Influence ||
                holderHighRenown.RenownScore <= holderLowRenown.RenownScore ||
                !Approximately(holderHighTrickle.LastAppliedInfluenceDelta, expectedHighInfluenceDelta) ||
                !Approximately(holderLowTrickle.LastAppliedInfluenceDelta, expectedLowInfluenceDelta))
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: high/low captive trickle mismatch. highInfluence={holderHighResources.Influence}, lowInfluence={holderLowResources.Influence}, highRenown={holderHighRenown.RenownScore}, lowRenown={holderLowRenown.RenownScore}, readout={readout ?? "<null>"}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 1 PASS: high-renown captive outpaced low-renown captive (influence {expectedHighInfluenceDelta:0.000} vs {expectedLowInfluenceDelta:0.000}, renown {expectedHighRenownDelta:0.000} vs {expectedLowRenownDelta:0.000}).");
            return true;
        }

        private static bool RunNoCaptivesPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-captive-ransom-trickle-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var clockEntity = SeedDualClock(entityManager, 12.1f);
            var emptyHolder = SeedFaction(entityManager, "empty_holder", 30f, 11f, 5f);

            TickAtDay(world, entityManager, clockEntity, 12.1f);
            TickAtDay(world, entityManager, clockEntity, 13.1f);

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(emptyHolder);
            var renown = entityManager.GetComponentData<DynastyRenownComponent>(emptyHolder);
            var trickle = entityManager.GetComponentData<CaptiveRansomTrickleComponent>(emptyHolder);

            if (!debugScope.CommandSurface.TryDebugGetCaptiveTrickle("empty_holder", out var readout) ||
                !readout.Contains("HeldCaptives=0", StringComparison.Ordinal) ||
                !Approximately(stockpile.Influence, 11f) ||
                !Approximately(renown.RenownScore, 5f) ||
                !Approximately(trickle.LastAppliedInfluenceDelta, 0f) ||
                !Approximately(trickle.LastAppliedDynastyRenownDelta, 0f))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: empty captor should receive zero trickle. influence={stockpile.Influence}, renown={renown.RenownScore}, readout={readout ?? "<null>"}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: factions without captives earned zero influence and zero dynasty renown trickle.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CaptiveRansomTrickleSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickAtDay(World world, EntityManager entityManager, Entity clockEntity, float inWorldDays)
        {
            var clock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            clock.InWorldDays = inWorldDays;
            entityManager.SetComponentData(clockEntity, clock);
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var clockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
            entityManager.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
            return clockEntity;
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float gold,
            float influence,
            float startingRenownScore)
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
            entityManager.AddComponentData(entity, new DynastyRenownComponent
            {
                RenownScore = startingRenownScore,
                LastRenownUpdateInWorldDays = 0f,
                RenownDecayRate = 0.45f,
                PeakRenown = startingRenownScore,
                LastRulingMemberId = default,
                Initialized = 1,
            });
            return entity;
        }

        private static void SeedHeldCaptive(
            EntityManager entityManager,
            Entity sourceFaction,
            string captorFactionId,
            DynastyRole role,
            float renown)
        {
            var faction = entityManager.GetComponentData<FactionComponent>(sourceFaction);
            var roster = entityManager.GetBuffer<DynastyMemberRef>(sourceFaction);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != role)
                {
                    continue;
                }

                member.Renown = renown;
                member.Status = DynastyMemberStatus.Captured;
                entityManager.SetComponentData(memberEntity, member);
                CapturedMemberHelpers.CaptureMember(
                    entityManager,
                    new FixedString32Bytes(captorFactionId),
                    member.MemberId,
                    member.Title,
                    faction.FactionId);
                return;
            }

            throw new InvalidOperationException($"Could not find role {role} on seeded faction {faction.FactionId}.");
        }

        private static bool Approximately(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= 0.001f;
        }

        private static float ResolveExpectedInfluenceDelta(float captiveRenown, float elapsedRealSeconds)
        {
            return (0.022f + Mathf.Max(0f, captiveRenown) * 0.0014f) * elapsedRealSeconds;
        }

        private static float ResolveExpectedRenownDelta(float captiveRenown, float elapsedRealSeconds)
        {
            return Mathf.Max(0f, captiveRenown) * 0.0014f * elapsedRealSeconds;
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

                hostObject = new GameObject("BloodlinesPlayerCaptiveRansomTrickleSmokeValidation_CommandSurface")
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
