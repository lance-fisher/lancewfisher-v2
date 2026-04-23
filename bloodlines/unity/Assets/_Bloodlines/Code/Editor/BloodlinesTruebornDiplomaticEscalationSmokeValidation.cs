#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
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
    /// Dedicated smoke validator for the Trueborn diplomatic-escalation follow-up.
    ///
    /// Browser references:
    /// - simulation.js tickTruebornRiseArc / getTruebornRecognitionTerms
    ///
    /// Canon references:
    /// - 11_MATCHFLOW/MATCH_STRUCTURE.md (Stage 4/5 Trueborn escalation pressure)
    /// - 08_MECHANICS/DIPLOMACY_SYSTEM.md (ultimatum structure)
    /// </summary>
    public static class BloodlinesTruebornDiplomaticEscalationSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-trueborn-diplomatic-escalation-smoke.log";

        [MenuItem("Bloodlines/WorldPressure/Run Trueborn Diplomatic Escalation Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static string RunBatchTruebornDiplomaticEscalationSmokeValidation() =>
            RunInternal(batchMode: true);

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                var report = new System.Text.StringBuilder();
                report.Append(RunPhaseStageFourUltimatumIssued()).Append("; ");
                report.Append(RunPhaseRecognitionClearsUltimatum()).Append("; ");
                report.Append(RunPhaseExpiredStageFivePressure());

                string summary =
                    "BLOODLINES_TRUEBORN_DIPLOMATIC_ESCALATION_SMOKE PASS " + report;
                UnityDebug.Log(summary);
                TryWriteArtifact(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary =
                    "BLOODLINES_TRUEBORN_DIPLOMATIC_ESCALATION_SMOKE FAIL " + ex;
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

        private static string RunPhaseStageFourUltimatumIssued()
        {
            using var world = CreateValidationWorld("trueborn-ultimatum-issued");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 5000f);
            SeedMatchProgression(entityManager, stageNumber: 4, dominantFactionId: "enemy");
            SeedTruebornCity(entityManager);
            SeedKingdom(entityManager, "player", 80f);
            SeedKingdom(entityManager, "enemy", 84f);
            SeedControlPoint(entityManager, "enemy-cp-a", "enemy", 92f);
            SeedControlPoint(entityManager, "player-cp-a", "player", 93f);

            Entity arcEntity = SystemAPIHelper.GetSingletonEntity<TruebornRiseArcComponent>(entityManager);
            entityManager.SetComponentData(arcEntity, new TruebornRiseArcComponent
            {
                CurrentStage = 3,
                StageStartedAtInWorldDays = 4745f,
                GlobalPressurePerDay = 1.4f,
                LoyaltyErosionPerDay = 3.2f,
                ChallengeLevel = 0,
                UnchallengedCycles = 3,
            });

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            GameObject debugHost = new GameObject("TruebornUltimatumIssuedDebugSurface");
            try
            {
                Tick(world, inWorldDays: 5000f, elapsedSeconds: 1f, deltaSeconds: 1f);

                var surface = debugHost.AddComponent<BloodlinesDebugCommandSurface>();
                if (!surface.TryDebugGetTruebornUltimatumState(out var readout) ||
                    !readout.ToString().Contains("Active=true", StringComparison.Ordinal) ||
                    !readout.ToString().Contains("Target=enemy", StringComparison.Ordinal) ||
                    !readout.ToString().Contains("MatchStage=4", StringComparison.Ordinal) ||
                    !readout.ToString().Contains("Expired=false", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Phase 1 failed: ultimatum readout='{readout}'.");
                }

                return $"phase1Readout={readout}";
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(debugHost);
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static string RunPhaseRecognitionClearsUltimatum()
        {
            using var world = CreateValidationWorld("trueborn-ultimatum-cleared");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 5100f);
            SeedMatchProgression(entityManager, stageNumber: 4, dominantFactionId: "player");
            SeedTruebornCity(entityManager);
            Entity player = SeedKingdom(entityManager, "player", 80f);
            SeedKingdom(entityManager, "enemy", 82f);
            SeedRecognitionResources(entityManager, player, influence: 100f, gold: 100f);
            SeedRenown(entityManager, player, 10f);
            SeedControlPoint(entityManager, "enemy-cp-a", "enemy", 91f);

            Entity arcEntity = SystemAPIHelper.GetSingletonEntity<TruebornRiseArcComponent>(entityManager);
            entityManager.SetComponentData(arcEntity, new TruebornRiseArcComponent
            {
                CurrentStage = 1,
                StageStartedAtInWorldDays = 5100f,
                GlobalPressurePerDay = 0f,
                LoyaltyErosionPerDay = 0.8f,
                ChallengeLevel = 0,
                UnchallengedCycles = 3,
            });

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            GameObject debugHost = new GameObject("TruebornUltimatumClearDebugSurface");
            try
            {
                var surface = debugHost.AddComponent<BloodlinesDebugCommandSurface>();

                Tick(world, inWorldDays: 5100f, elapsedSeconds: 1f, deltaSeconds: 1f);
                if (!surface.TryDebugRecognizeTrueborn("player"))
                {
                    throw new InvalidOperationException(
                        "Phase 2 setup failed: could not queue player recognition.");
                }

                Tick(world, inWorldDays: 5101f, elapsedSeconds: 2f, deltaSeconds: 1f);

                if (!surface.TryDebugGetTruebornUltimatumState(out var ultimatumReadout) ||
                    !surface.TryDebugGetTruebornRecognitionState("player", out var recognitionReadout))
                {
                    throw new InvalidOperationException(
                        "Phase 2 failed: missing debug readout.");
                }

                ResourceStockpileComponent resources =
                    entityManager.GetComponentData<ResourceStockpileComponent>(player);
                DynastyStateComponent dynasty =
                    entityManager.GetComponentData<DynastyStateComponent>(player);

                if (!ultimatumReadout.ToString().Contains("Active=false", StringComparison.Ordinal) ||
                    !recognitionReadout.ToString().Contains("Recognized=true", StringComparison.Ordinal) ||
                    math.abs(resources.Influence - 60f) > 0.01f ||
                    math.abs(resources.Gold - 40f) > 0.01f ||
                    math.abs(dynasty.Legitimacy - 75f) > 0.01f)
                {
                    throw new InvalidOperationException(
                        $"Phase 2 failed: ultimatum='{ultimatumReadout}' recognition='{recognitionReadout}' influence={resources.Influence:0.00} gold={resources.Gold:0.00} legitimacy={dynasty.Legitimacy:0.00}.");
                }

                return
                    $"phase2Ultimatum={ultimatumReadout},recognition={recognitionReadout}";
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(debugHost);
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static string RunPhaseExpiredStageFivePressure()
        {
            using var world = CreateValidationWorld("trueborn-ultimatum-expired");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 6000f);
            SeedMatchProgression(entityManager, stageNumber: 5, dominantFactionId: "enemy");
            SeedTruebornCity(entityManager);
            SeedKingdom(entityManager, "player", 80f);
            Entity enemy = SeedKingdom(entityManager, "enemy", 80f);
            SeedControlPoint(entityManager, "enemy-cp-a", "enemy", 90f);
            SeedControlPoint(entityManager, "enemy-cp-b", "enemy", 88f);
            SeedControlPoint(entityManager, "player-cp-a", "player", 91f);

            Entity arcEntity = SystemAPIHelper.GetSingletonEntity<TruebornRiseArcComponent>(entityManager);
            entityManager.SetComponentData(arcEntity, new TruebornRiseArcComponent
            {
                CurrentStage = 3,
                StageStartedAtInWorldDays = 4745f,
                GlobalPressurePerDay = 1.4f,
                LoyaltyErosionPerDay = 3.2f,
                ChallengeLevel = 0,
                UnchallengedCycles = 3,
            });

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            GameObject debugHost = new GameObject("TruebornUltimatumExpiredDebugSurface");
            try
            {
                Tick(world, inWorldDays: 6000f, elapsedSeconds: 1f, deltaSeconds: 1f);
                Tick(world, inWorldDays: 6061f, elapsedSeconds: 2f, deltaSeconds: 1f);

                var surface = debugHost.AddComponent<BloodlinesDebugCommandSurface>();
                if (!surface.TryDebugGetTruebornUltimatumState(out var readout))
                {
                    throw new InvalidOperationException(
                        "Phase 3 failed: ultimatum readout unavailable.");
                }

                ControlPointComponent weakestEnemyControlPoint =
                    ReadControlPoint(entityManager, "enemy-cp-b");
                DynastyStateComponent enemyDynasty =
                    entityManager.GetComponentData<DynastyStateComponent>(enemy);

                if (!readout.ToString().Contains("Active=true", StringComparison.Ordinal) ||
                    !readout.ToString().Contains("MatchStage=5", StringComparison.Ordinal) ||
                    !readout.ToString().Contains("Expired=true", StringComparison.Ordinal) ||
                    math.abs(weakestEnemyControlPoint.Loyalty - 79.2f) > 0.02f ||
                    math.abs(enemyDynasty.Legitimacy - 76f) > 0.02f)
                {
                    throw new InvalidOperationException(
                        $"Phase 3 failed: readout='{readout}' weakestLoyalty={weakestEnemyControlPoint.Loyalty:0.00} legitimacy={enemyDynasty.Legitimacy:0.00}.");
                }

                return
                    $"phase3Readout={readout},weakestLoyalty={weakestEnemyControlPoint.Loyalty:0.0},legitimacy={enemyDynasty.Legitimacy:0.0}";
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
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<TruebornRecognitionResolutionSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<TruebornRiseArcSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<TruebornDiplomaticEscalationSystem>());
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

        private static void SeedMatchProgression(
            EntityManager entityManager,
            int stageNumber,
            string dominantFactionId)
        {
            Entity matchProgression = entityManager.CreateEntity(typeof(MatchProgressionComponent));
            entityManager.SetComponentData(matchProgression, new MatchProgressionComponent
            {
                StageNumber = stageNumber,
                StageId = new FixedString32Bytes(stageNumber >= 5
                    ? "final_convergence"
                    : "war_turning_of_tides"),
                StageLabel = new FixedString64Bytes(stageNumber >= 5
                    ? "Final Convergence"
                    : "War and Turning of Tides"),
                PhaseId = new FixedString32Bytes("resolution"),
                PhaseLabel = new FixedString32Bytes("resolution"),
                StageReadiness = 1f,
                NextStageId = default,
                NextStageLabel = default,
                InWorldDays = 0f,
                InWorldYears = 0f,
                DeclarationCount = 0,
                RivalContactActive = true,
                SustainedWarActive = true,
                GreatReckoningActive = false,
                GreatReckoningTargetFactionId = default,
                GreatReckoningShare = 0f,
                GreatReckoningThreshold = 0.7f,
                DominantKingdomId = new FixedString32Bytes(dominantFactionId),
                DominantTerritoryShare = 0.72f,
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

        private static void SeedRecognitionResources(
            EntityManager entityManager,
            Entity factionEntity,
            float influence,
            float gold)
        {
            entityManager.AddComponentData(factionEntity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = 20f,
                Water = 20f,
                Wood = 10f,
                Stone = 10f,
                Iron = 10f,
                Influence = influence,
            });
        }

        private static void SeedRenown(
            EntityManager entityManager,
            Entity factionEntity,
            float renownScore)
        {
            entityManager.AddComponentData(factionEntity, new DynastyRenownComponent
            {
                RenownScore = renownScore,
                LastRenownUpdateInWorldDays = 5100f,
                RenownDecayRate = TruebornRecognitionUtility.DefaultRenownDecayRate,
                PeakRenown = renownScore,
                LastRulingMemberId = default,
                Initialized = 1,
            });
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
                UnityDebug.LogWarning($"Unable to write Trueborn diplomatic escalation smoke artifact: {ex.Message}");
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
