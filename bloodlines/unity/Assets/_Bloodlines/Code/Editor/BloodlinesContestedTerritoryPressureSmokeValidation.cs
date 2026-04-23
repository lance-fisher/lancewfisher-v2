#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the contested-territory pressure read-model slice.
    ///
    /// Phase 1: external contested territories count for the realm-condition seam
    ///          without creating a new world-pressure score source.
    /// Phase 2: owned contested territory projects onto the faction read-model and
    ///          blocks territorial-governance hold readiness while an equivalent
    ///          uncontested peer remains ready.
    /// Phase 3: clearing the contest clears the read-model and the governance blocker.
    ///
    /// Browser references:
    ///   - simulation.js getRealmConditionSnapshot contestedTerritories
    ///   - simulation.js territorial-governance contested-territory acceptance/hold gates
    /// Artifact: artifacts/unity-contested-territory-pressure-smoke.log.
    /// </summary>
    public static class BloodlinesContestedTerritoryPressureSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-contested-territory-pressure-smoke.log";

        [MenuItem("Bloodlines/WorldPressure/Run Contested Territory Pressure Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static string RunBatchContestedTerritoryPressureSmokeValidation() =>
            RunInternal(batchMode: true);

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                var report = new System.Text.StringBuilder();
                report.Append(RunExternalContestedPhase()).Append("; ");
                report.Append(RunGovernanceBlockingPhase()).Append("; ");
                report.Append(RunContestClearPhase());
                string summary =
                    "BLOODLINES_CONTESTED_TERRITORY_PRESSURE_SMOKE PASS " + report;
                UnityDebug.Log(summary);
                TryWriteArtifact(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary =
                    "BLOODLINES_CONTESTED_TERRITORY_PRESSURE_SMOKE FAIL " + ex;
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

        private static string RunExternalContestedPhase()
        {
            using var world = CreateValidationWorld("territorial-pressure-external");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 200f);
            SeedMatchProgression(entityManager, stageNumber: 4);
            SeedFaction(entityManager, "player", legitimacy: 70f);
            SeedFaction(entityManager, "enemy", legitimacy: 65f);
            SeedFaction(entityManager, "rival", legitimacy: 64f);
            SeedFaction(entityManager, "ally", legitimacy: 63f);

            SeedGovernedControlPoint(entityManager, "player-core", "player", 91f, governed: true);
            SeedGovernedControlPoint(entityManager, "enemy-frontier", "enemy", 82f, governed: true);
            SeedGovernedControlPoint(entityManager, "rival-core", "rival", 84f, governed: true);
            SeedGovernedControlPoint(entityManager, "ally-core", "ally", 86f, governed: true);
            SetControlPointContest(
                entityManager,
                "enemy-frontier",
                contested: true,
                captureFactionId: "rival",
                captureProgress: 0.45f);

            Tick(world, elapsedSeconds: 1f, deltaSeconds: 1f);

            TerritorialPressureComponent pressure =
                ReadTerritorialPressure(entityManager, "player");
            WorldPressureComponent worldPressure = ReadWorldPressure(entityManager, "player");
            string readout = ReadTerritorialPressureDebug(world, "player");

            if (pressure.ExternalContestedTerritoryCount != 1 ||
                pressure.OwnedContestedTerritoryCount != 0 ||
                pressure.GovernanceContestBlockingActive ||
                worldPressure.Score != 0 ||
                !readout.Contains("|ExternalContested=1|") ||
                !readout.Contains("|OwnedContested=0|"))
            {
                throw new InvalidOperationException(
                    $"Phase 1 failed: external={pressure.ExternalContestedTerritoryCount} owned={pressure.OwnedContestedTerritoryCount} block={pressure.GovernanceContestBlockingActive} wpScore={worldPressure.Score} readout={readout}");
            }

            return
                $"phase1External={pressure.ExternalContestedTerritoryCount},wpScore={worldPressure.Score}";
        }

        private static string RunGovernanceBlockingPhase()
        {
            using var world = CreateValidationWorld("territorial-pressure-governance-block");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 260f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            SeedFaction(entityManager, "player", legitimacy: 82f);
            SeedFaction(entityManager, "ally", legitimacy: 80f);

            SeedGovernedControlPoint(entityManager, "player-border", "player", 88f, governed: true);
            SeedGovernedControlPoint(entityManager, "player-heartland", "player", 93f, governed: true);
            SeedGovernedControlPoint(entityManager, "ally-border", "ally", 88f, governed: true);
            SeedGovernedControlPoint(entityManager, "ally-heartland", "ally", 93f, governed: true);

            SetControlPointContest(
                entityManager,
                "player-border",
                contested: true,
                captureFactionId: "ally",
                captureProgress: 0.55f);

            Tick(world, elapsedSeconds: 1f, deltaSeconds: 1f);

            TerritorialPressureComponent playerPressure =
                ReadTerritorialPressure(entityManager, "player");
            TerritorialGovernanceRecognitionComponent playerRecognition =
                ReadRecognition(entityManager, "player");
            TerritorialGovernanceRecognitionComponent allyRecognition =
                ReadRecognition(entityManager, "ally");
            string readout = ReadTerritorialPressureDebug(world, "player");

            if (playerPressure.ExternalContestedTerritoryCount != 0 ||
                playerPressure.OwnedContestedTerritoryCount != 1 ||
                !playerPressure.GovernanceContestBlockingActive ||
                !playerPressure.WeakestOwnedContestedControlPointId.Equals(
                    new FixedString32Bytes("player-border")) ||
                math.abs(playerPressure.WeakestOwnedContestedLoyalty - 88f) > 0.01f ||
                playerRecognition.ContestedTerritoryCount != 1 ||
                playerRecognition.HoldReady ||
                !playerRecognition.StageReady ||
                !playerRecognition.ShareReady ||
                !allyRecognition.HoldReady ||
                !readout.Contains("|GovernanceContestBlocking=true|") ||
                !readout.Contains("|OwnedContested=1|") ||
                !readout.Contains("|WeakestOwnedContested=player-border|"))
            {
                throw new InvalidOperationException(
                    $"Phase 2 failed: owned={playerPressure.OwnedContestedTerritoryCount} block={playerPressure.GovernanceContestBlockingActive} weakest={playerPressure.WeakestOwnedContestedControlPointId} weakestLoyalty={playerPressure.WeakestOwnedContestedLoyalty:0.00} contested={playerRecognition.ContestedTerritoryCount} playerHold={playerRecognition.HoldReady} allyHold={allyRecognition.HoldReady} readout={readout}");
            }

            return
                $"phase2Owned={playerPressure.OwnedContestedTerritoryCount},block={playerPressure.GovernanceContestBlockingActive}";
        }

        private static string RunContestClearPhase()
        {
            using var world = CreateValidationWorld("territorial-pressure-clear");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 320f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            SeedFaction(entityManager, "player", legitimacy: 82f);
            SeedFaction(entityManager, "ally", legitimacy: 80f);

            SeedGovernedControlPoint(entityManager, "player-border", "player", 88f, governed: true);
            SeedGovernedControlPoint(entityManager, "player-heartland", "player", 92f, governed: true);
            SeedGovernedControlPoint(entityManager, "ally-border", "ally", 88f, governed: true);
            SeedGovernedControlPoint(entityManager, "ally-heartland", "ally", 91f, governed: true);

            SetControlPointContest(
                entityManager,
                "player-border",
                contested: true,
                captureFactionId: "ally",
                captureProgress: 0.35f);

            Tick(world, elapsedSeconds: 1f, deltaSeconds: 1f);
            SetControlPointContest(
                entityManager,
                "player-border",
                contested: false,
                captureFactionId: string.Empty,
                captureProgress: 0f);
            Tick(world, elapsedSeconds: 2f, deltaSeconds: 1f);

            TerritorialPressureComponent pressure =
                ReadTerritorialPressure(entityManager, "player");
            TerritorialGovernanceRecognitionComponent recognition =
                ReadRecognition(entityManager, "player");
            string readout = ReadTerritorialPressureDebug(world, "player");

            if (pressure.ExternalContestedTerritoryCount != 0 ||
                pressure.OwnedContestedTerritoryCount != 0 ||
                pressure.GovernanceContestBlockingActive ||
                pressure.WeakestOwnedContestedControlPointId.Length != 0 ||
                math.abs(pressure.WeakestOwnedContestedLoyalty) > 0.01f ||
                recognition.ContestedTerritoryCount != 0 ||
                !recognition.HoldReady ||
                !recognition.TriggerReady ||
                !readout.Contains("|GovernanceContestBlocking=false|") ||
                !readout.Contains("|OwnedContested=0|"))
            {
                throw new InvalidOperationException(
                    $"Phase 3 failed: external={pressure.ExternalContestedTerritoryCount} owned={pressure.OwnedContestedTerritoryCount} block={pressure.GovernanceContestBlockingActive} weakest={pressure.WeakestOwnedContestedControlPointId} weakestLoyalty={pressure.WeakestOwnedContestedLoyalty:0.00} contested={recognition.ContestedTerritoryCount} hold={recognition.HoldReady} trigger={recognition.TriggerReady} readout={readout}");
            }

            return
                $"phase3ClearedOwned={pressure.OwnedContestedTerritoryCount},holdReady={recognition.HoldReady}";
        }

        private static World CreateValidationWorld(string name)
        {
            var world = new World(name);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<GovernanceCoalitionPressureSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<TerritorialPressureEvaluationSystem>());
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

        private static void SeedMatchProgression(EntityManager entityManager, int stageNumber)
        {
            Entity progression = entityManager.CreateEntity(typeof(MatchProgressionComponent));
            entityManager.SetComponentData(progression, new MatchProgressionComponent
            {
                StageNumber = stageNumber,
                StageId = new FixedString32Bytes("final_convergence"),
                StageLabel = new FixedString64Bytes("Final Convergence"),
                GreatReckoningThreshold = 0.7f,
            });
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float legitimacy)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(WorldPressureComponent),
                typeof(PopulationComponent),
                typeof(ResourceStockpileComponent));
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
                ActiveMemberCap = 8,
                Legitimacy = legitimacy,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            entityManager.SetComponentData(entity, new WorldPressureComponent());
            entityManager.SetComponentData(entity, new PopulationComponent
            {
                Total = 20,
                Available = 20,
                Cap = 20,
                BaseCap = 20,
            });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Gold = 20f,
                Food = 40f,
                Water = 40f,
                Wood = 10f,
                Stone = 10f,
                Iron = 10f,
                Influence = 20f,
            });
            entityManager.AddBuffer<DynastyMemberRef>(entity);
            entityManager.AddBuffer<DynastyFallenLedger>(entity);
            entityManager.AddBuffer<HostilityComponent>(entity);
            return entity;
        }

        private static void SeedGovernedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string factionId,
            float loyalty,
            bool governed)
        {
            Entity entity = entityManager.CreateEntity(typeof(ControlPointComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(factionId),
                ContinentId = new FixedString32Bytes("west"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("trade_town"),
                RadiusTiles = 3f,
                CaptureTimeSeconds = 9f,
            });

            if (governed)
            {
                entityManager.AddComponentData(entity, new GovernorSeatAssignmentComponent
                {
                    GovernorMemberId = new FixedString64Bytes(controlPointId + "-governor"),
                    SpecializationId = GovernorSpecializationId.CivicSteward,
                    AnchorType = GovernanceAnchorType.ControlPoint,
                    PriorityScore = 10f,
                });
            }
        }

        private static void SetControlPointContest(
            EntityManager entityManager,
            string controlPointId,
            bool contested,
            string captureFactionId,
            float captureProgress)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!controlPoints[i].ControlPointId.Equals(new FixedString32Bytes(controlPointId)))
                {
                    continue;
                }

                ControlPointComponent updated = controlPoints[i];
                updated.IsContested = contested;
                updated.ControlState = contested ? ControlState.Contested : ControlState.Stabilized;
                updated.CaptureFactionId = new FixedString32Bytes(captureFactionId);
                updated.CaptureProgress = captureProgress;
                entityManager.SetComponentData(entities[i], updated);
                return;
            }

            throw new InvalidOperationException(
                $"Control point '{controlPointId}' not found for contest update.");
        }

        private static void Tick(World world, float elapsedSeconds, float deltaSeconds)
        {
            world.SetTime(new Unity.Core.TimeData(elapsedSeconds, deltaSeconds));
            world.Update();
        }

        private static Entity FindFactionEntity(EntityManager entityManager, string factionId)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
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

        private static TerritorialPressureComponent ReadTerritorialPressure(
            EntityManager entityManager,
            string factionId)
        {
            Entity faction = FindFactionEntity(entityManager, factionId);
            if (faction == Entity.Null || !entityManager.HasComponent<TerritorialPressureComponent>(faction))
            {
                throw new InvalidOperationException(
                    $"Territorial pressure not found for faction '{factionId}'.");
            }

            return entityManager.GetComponentData<TerritorialPressureComponent>(faction);
        }

        private static TerritorialGovernanceRecognitionComponent ReadRecognition(
            EntityManager entityManager,
            string factionId)
        {
            Entity faction = FindFactionEntity(entityManager, factionId);
            if (faction == Entity.Null ||
                !entityManager.HasComponent<TerritorialGovernanceRecognitionComponent>(faction))
            {
                throw new InvalidOperationException(
                    $"Recognition not found for faction '{factionId}'.");
            }

            return entityManager.GetComponentData<TerritorialGovernanceRecognitionComponent>(faction);
        }

        private static WorldPressureComponent ReadWorldPressure(
            EntityManager entityManager,
            string factionId)
        {
            Entity faction = FindFactionEntity(entityManager, factionId);
            if (faction == Entity.Null || !entityManager.HasComponent<WorldPressureComponent>(faction))
            {
                throw new InvalidOperationException(
                    $"World pressure not found for faction '{factionId}'.");
            }

            return entityManager.GetComponentData<WorldPressureComponent>(faction);
        }

        private static string ReadTerritorialPressureDebug(World world, string factionId)
        {
            World previousDefault = World.DefaultGameObjectInjectionWorld;
            try
            {
                World.DefaultGameObjectInjectionWorld = world;
                if (!BloodlinesDebugCommandSurface.TryDebugGetTerritorialPressureState(
                        factionId,
                        out FixedString512Bytes readout))
                {
                    throw new InvalidOperationException(
                        $"Debug territorial pressure readout missing for faction '{factionId}'.");
                }

                return readout.ToString();
            }
            finally
            {
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static void TryWriteArtifact(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, message);
            }
            catch (Exception)
            {
                // Best effort only.
            }
        }
    }
}
#endif
