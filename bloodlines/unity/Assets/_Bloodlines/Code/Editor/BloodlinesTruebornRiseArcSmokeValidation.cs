#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the Trueborn rise arc base-pressure slice.
    ///
    /// Phase 1: low-challenge kingdoms let the Trueborn arc activate at the
    ///          canonical 8-year threshold, then stage 2 and 3 advance after the
    ///          browser delay windows.
    /// Phase 2: a recognized kingdom is exempt from stage-2 pressure while an
    ///          unrecognized kingdom loses loyalty and legitimacy.
    ///
    /// Browser reference:
    ///   - simulation.js tickTruebornRiseArc / getTruebornChallengeLevel
    ///   - TRUEBORN_RISE_STAGE_* constants
    /// Artifact: artifacts/unity-trueborn-rise-arc-smoke.log.
    /// </summary>
    public static class BloodlinesTruebornRiseArcSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-trueborn-rise-arc-smoke-summary.log";

        [MenuItem("Bloodlines/WorldPressure/Run Trueborn Rise Arc Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static string RunBatchTruebornRiseArcSmokeValidation() =>
            RunInternal(batchMode: true);

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                var report = new System.Text.StringBuilder();
                report.Append(RunPhaseStageProgression()).Append("; ");
                report.Append(RunPhaseRecognizedExemption());
                string summary =
                    "BLOODLINES_TRUEBORN_RISE_ARC_SMOKE PASS " + report;
                UnityDebug.Log(summary);
                TryWriteArtifact(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary =
                    "BLOODLINES_TRUEBORN_RISE_ARC_SMOKE FAIL " + ex;
                UnityDebug.LogError(summary);
                TryWriteArtifact(summary);
                return summary;
            }
            finally
            {
                if (batchMode)
                {
                    EditorApplication.Exit(exitCode);
                }
            }
        }

        private static string RunPhaseStageProgression()
        {
            using var world = CreateValidationWorld("trueborn-stage-progress");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 2918f);
            SeedTruebornCity(entityManager);
            SeedKingdom(entityManager, "player", 80f);
            SeedControlPoint(entityManager, "player-cp-a", "player", 90f);
            SeedControlPoint(entityManager, "player-cp-b", "player", 89f);

            Tick(world, inWorldDays: 2918f, elapsedSeconds: 1f, deltaSeconds: 1f);
            Tick(world, inWorldDays: 2919f, elapsedSeconds: 2f, deltaSeconds: 1f);
            Tick(world, inWorldDays: 2920f, elapsedSeconds: 3f, deltaSeconds: 1f);

            TruebornRiseArcComponent arc = ReadArc(entityManager);
            if (arc.CurrentStage != 1 || arc.UnchallengedCycles < 3)
            {
                throw new InvalidOperationException(
                    $"Phase 1 activation failed: stage={arc.CurrentStage} cycles={arc.UnchallengedCycles} challenge={arc.ChallengeLevel}.");
            }

            Tick(world, inWorldDays: 3650f, elapsedSeconds: 4f, deltaSeconds: 1f);
            arc = ReadArc(entityManager);
            if (arc.CurrentStage != 2 || math.abs(arc.GlobalPressurePerDay - 0.6f) > 0.001f)
            {
                throw new InvalidOperationException(
                    $"Phase 1 stage-2 failed: stage={arc.CurrentStage} global={arc.GlobalPressurePerDay:0.00} loyalty={arc.LoyaltyErosionPerDay:0.00}.");
            }

            Tick(world, inWorldDays: 4745f, elapsedSeconds: 5f, deltaSeconds: 1f);
            arc = ReadArc(entityManager);

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            GameObject debugHost = new GameObject("TruebornRiseArcSmokeDebugSurface");
            try
            {
                var surface = debugHost.AddComponent<BloodlinesDebugCommandSurface>();
                if (!surface.TryDebugGetTruebornRiseArc(out var readout) ||
                    arc.CurrentStage != 3 ||
                    math.abs(arc.GlobalPressurePerDay - 1.4f) > 0.001f ||
                    math.abs(arc.LoyaltyErosionPerDay - 3.2f) > 0.001f ||
                    !readout.ToString().Contains("Stage=3", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Phase 1 stage-3 failed: stage={arc.CurrentStage} global={arc.GlobalPressurePerDay:0.00} loyalty={arc.LoyaltyErosionPerDay:0.00} readout='{readout}'.");
                }

                return
                    $"phase1Stage={arc.CurrentStage},challenge={arc.ChallengeLevel},cycles={arc.UnchallengedCycles},readout={readout}";
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(debugHost);
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static string RunPhaseRecognizedExemption()
        {
            using var world = CreateValidationWorld("trueborn-recognition-exemption");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 4200f);
            SeedTruebornCity(entityManager);
            SeedKingdom(entityManager, "player", 80f);
            SeedKingdom(entityManager, "enemy", 80f);
            SeedControlPoint(entityManager, "player-cp-a", "player", 90f);
            SeedControlPoint(entityManager, "enemy-cp-a", "enemy", 90f);

            Entity arcEntity = SystemAPIHelper.GetSingletonEntity<TruebornRiseArcComponent>(entityManager);
            entityManager.SetComponentData(arcEntity, new TruebornRiseArcComponent
            {
                CurrentStage = 2,
                StageStartedAtInWorldDays = 4000f,
                GlobalPressurePerDay = 0.6f,
                LoyaltyErosionPerDay = 1.8f,
                RecognizedFactionsBitmask = 0UL,
                ChallengeLevel = 0,
                UnchallengedCycles = 3,
            });

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            GameObject debugHost = new GameObject("TruebornRiseArcRecognitionDebugSurface");
            try
            {
                var surface = debugHost.AddComponent<BloodlinesDebugCommandSurface>();
                if (!surface.TryDebugSetTruebornRecognition("player", true))
                {
                    throw new InvalidOperationException(
                        "Phase 2 setup failed: debug surface could not mark player as recognized.");
                }

                Tick(world, inWorldDays: 4200f, elapsedSeconds: 10f, deltaSeconds: 1f);

                ControlPointComponent playerPoint = ReadControlPoint(entityManager, "player-cp-a");
                ControlPointComponent enemyPoint = ReadControlPoint(entityManager, "enemy-cp-a");
                DynastyStateComponent playerDynasty = ReadDynasty(entityManager, "player");
                DynastyStateComponent enemyDynasty = ReadDynasty(entityManager, "enemy");

                if (math.abs(playerPoint.Loyalty - 90f) > 0.01f ||
                    math.abs(playerDynasty.Legitimacy - 80f) > 0.01f ||
                    math.abs(enemyPoint.Loyalty - 88.2f) > 0.02f ||
                    math.abs(enemyDynasty.Legitimacy - 79.4f) > 0.02f ||
                    !surface.TryDebugGetTruebornRiseArc(out var readout) ||
                    !readout.ToString().Contains("RecognizedCount=1", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Phase 2 failed: playerLoyalty={playerPoint.Loyalty:0.00} playerLegitimacy={playerDynasty.Legitimacy:0.00} enemyLoyalty={enemyPoint.Loyalty:0.00} enemyLegitimacy={enemyDynasty.Legitimacy:0.00}.");
                }

                return
                    $"phase2PlayerLoyalty={playerPoint.Loyalty:0.0},enemyLoyalty={enemyPoint.Loyalty:0.0},enemyLegitimacy={enemyDynasty.Legitimacy:0.0}";
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(debugHost);
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static World CreateValidationWorld(string name)
        {
            var world = new World(name);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<GovernanceCoalitionPressureSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<TruebornRiseArcSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            simulation.SortSystems();
            return world;
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static Entity SeedTruebornCity(EntityManager entityManager)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes("trueborn_city"),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Neutral,
            });
            return entity;
        }

        private static Entity SeedKingdom(
            EntityManager entityManager,
            string factionId,
            float legitimacy)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            entityManager.SetComponentData(entity, new DynastyStateComponent
            {
                Legitimacy = legitimacy,
            });
            entityManager.AddBuffer<HostilityComponent>(entity);
            return entity;
        }

        private static void SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float loyalty)
        {
            Entity entity = entityManager.CreateEntity(typeof(ControlPointComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("border_settlement"),
                RadiusTiles = 4f,
                CaptureTimeSeconds = 8f,
            });
        }

        private static void Tick(
            World world,
            float inWorldDays,
            float elapsedSeconds,
            float deltaSeconds)
        {
            EntityManager entityManager = world.EntityManager;
            Entity clock = SystemAPIHelper.GetSingletonEntity<DualClockComponent>(entityManager);
            DualClockComponent dualClock = entityManager.GetComponentData<DualClockComponent>(clock);
            dualClock.InWorldDays = inWorldDays;
            entityManager.SetComponentData(clock, dualClock);
            world.SetTime(new Unity.Core.TimeData(elapsedSeconds, deltaSeconds));
            world.Update();
        }

        private static TruebornRiseArcComponent ReadArc(EntityManager entityManager)
        {
            Entity arcEntity = SystemAPIHelper.GetSingletonEntity<TruebornRiseArcComponent>(entityManager);
            return entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
        }

        private static ControlPointComponent ReadControlPoint(
            EntityManager entityManager,
            string controlPointId)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<ControlPointComponent> controlPoints =
                query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.Equals(new FixedString32Bytes(controlPointId)))
                {
                    return controlPoints[i];
                }
            }

            throw new InvalidOperationException(
                $"Control point '{controlPointId}' was not found.");
        }

        private static DynastyStateComponent ReadDynasty(
            EntityManager entityManager,
            string factionId)
        {
            Entity faction = FindFactionEntity(entityManager, factionId);
            if (faction == Entity.Null)
            {
                throw new InvalidOperationException(
                    $"Faction '{factionId}' was not found.");
            }

            return entityManager.GetComponentData<DynastyStateComponent>(faction);
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            string factionId)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            using NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(new FixedString32Bytes(factionId)))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static void TryWriteArtifact(string summary)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string artifactPath = Path.GetFullPath(Path.Combine(projectRoot, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(artifactPath) ?? projectRoot);
                File.WriteAllText(artifactPath, summary);
            }
            catch (Exception ex)
            {
                UnityDebug.LogWarning($"Unable to write Trueborn rise arc smoke artifact: {ex.Message}");
            }
        }

        private static class SystemAPIHelper
        {
            public static Entity GetSingletonEntity<T>(EntityManager entityManager)
                where T : unmanaged, IComponentData
            {
                using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<T>());
                if (query.IsEmpty)
                {
                    throw new InvalidOperationException(
                        $"Singleton '{typeof(T).Name}' was not found.");
                }

                return query.GetSingletonEntity();
            }
        }
    }
}
#endif
