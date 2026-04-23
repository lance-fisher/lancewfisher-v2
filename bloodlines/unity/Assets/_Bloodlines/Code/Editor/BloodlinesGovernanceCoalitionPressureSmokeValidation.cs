#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Bloodlines.TerritoryGovernance;
using Bloodlines.Victory;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the governance coalition-pressure slice.
    ///
    /// Phase 1: trigger-ready player kingdom auto-issues a live territorial-governance
    ///          recognition and contributes world pressure score 3.
    /// Phase 2: alliance-threshold acceptance (65) with two hostile kingdoms applies
    ///          weakest-march loyalty erosion and legitimacy strain in the same 90s cycle.
    /// Phase 3: sub-threshold acceptance (50) does not trigger coalition pressure.
    /// Phase 4: completed recognition resolves Territorial Governance victory and enemy
    ///          AI sees its own governance-victory flags.
    ///
    /// Browser reference: simulation.js territorial-governance acceptance / world-pressure /
    /// governanceAlliancePressure block and ai.js enemy governance clamp block.
    /// Artifact: artifacts/unity-governance-coalition-pressure-smoke.log.
    /// </summary>
    public static class BloodlinesGovernanceCoalitionPressureSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-governance-coalition-pressure-smoke.log";

        [MenuItem("Bloodlines/WorldPressure/Run Governance Coalition Pressure Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static string RunBatchGovernanceCoalitionPressureSmokeValidation() =>
            RunInternal(batchMode: true);

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                var report = new System.Text.StringBuilder();
                report.Append(RunBootstrapRecognitionPhase()).Append("; ");
                report.Append(RunAlliancePressurePhase()).Append("; ");
                report.Append(RunBelowThresholdPhase()).Append("; ");
                report.Append(RunVictoryAndEnemyGovernancePhase());
                string summary =
                    "BLOODLINES_GOVERNANCE_COALITION_PRESSURE_SMOKE PASS " + report;
                UnityDebug.Log(summary);
                TryWriteArtifact(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary =
                    "BLOODLINES_GOVERNANCE_COALITION_PRESSURE_SMOKE FAIL " + ex;
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

        private static string RunBootstrapRecognitionPhase()
        {
            using var world = CreateValidationWorld("governance-bootstrap");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 240f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 55f, withAi: false);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 70f, withAi: true);
            SeedFaction(entityManager, "ally", legitimacy: 68f, withAi: false);
            SeedFaction(entityManager, "rival", legitimacy: 66f, withAi: false);
            SeedHostility(entityManager, player, "enemy", "rival");
            entityManager.SetComponentData(player, new ResourceStockpileComponent
            {
                Gold = 10f,
                Food = 16f,
                Water = 16f,
                Wood = 10f,
                Stone = 10f,
                Iron = 10f,
                Influence = 10f,
            });

            SeedGovernedControlPoint(entityManager, "player-cp-a", "player", 92f, true);
            SeedGovernedControlPoint(entityManager, "player-cp-b", "player", 91f, true);
            SeedGovernedControlPoint(entityManager, "enemy-cp-a", "enemy", 72f, false);
            SeedGovernedControlPoint(entityManager, "ally-cp-a", "ally", 70f, false);
            SeedGovernedControlPoint(entityManager, "rival-cp-a", "rival", 69f, false);
            SeedPrimaryKeep(entityManager, "player", true);

            Tick(world, elapsedSeconds: 1f, deltaSeconds: 1f);

            TerritorialGovernanceRecognitionComponent recognition =
                ReadRecognition(entityManager, "player");
            WorldPressureComponent pressure = ReadWorldPressure(entityManager, "player");
            AIStrategyComponent enemyStrategy = entityManager.GetComponentData<AIStrategyComponent>(enemy);

            if (!recognition.Active ||
                recognition.RecognitionEstablished ||
                recognition.WorldPressureContribution < 3 ||
                pressure.TerritorialGovernanceRecognitionScore < 3 ||
                !enemyStrategy.PlayerGovernanceActive)
            {
                throw new InvalidOperationException(
                    $"Phase 1 failed: active={recognition.Active} recognized={recognition.RecognitionEstablished} recScore={recognition.WorldPressureContribution} wpScore={pressure.TerritorialGovernanceRecognitionScore} aiActive={enemyStrategy.PlayerGovernanceActive} aiAlliance={enemyStrategy.PlayerGovernanceAlliancePressure}");
            }

            return
                $"phase1Active={recognition.Active},acceptance={recognition.PopulationAcceptancePct:0.0},recScore={recognition.WorldPressureContribution}";
        }

        private static string RunAlliancePressurePhase()
        {
            using var world = CreateValidationWorld("governance-alliance-pressure");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 320f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 90f, withAi: false);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 70f, withAi: true);
            SeedFaction(entityManager, "rival", legitimacy: 69f, withAi: false);
            SeedFaction(entityManager, "ally", legitimacy: 68f, withAi: false);
            SeedHostility(entityManager, player, "enemy", "rival");

            SeedGovernedControlPoint(entityManager, "player-cp-a", "player", 82f, true);
            SeedGovernedControlPoint(entityManager, "player-cp-b", "player", 93f, true);
            SeedGovernedControlPoint(entityManager, "enemy-cp-a", "enemy", 72f, false);
            SeedGovernedControlPoint(entityManager, "rival-cp-a", "rival", 71f, false);
            SeedGovernedControlPoint(entityManager, "ally-cp-a", "ally", 70f, false);
            SeedPrimaryKeep(entityManager, "player", true);

            Entity playerEntity = FindFactionEntity(entityManager, "player");
            entityManager.AddComponentData(playerEntity, new TerritorialGovernanceRecognitionComponent
            {
                Active = true,
                RecognitionEstablished = true,
                StartedAtInWorldDays = 250f,
                RecognizedAtInWorldDays = 251f,
                RequiredSustainSeconds = 90f,
                SustainedSeconds = 90f,
                RequiredVictorySeconds = 120f,
                PopulationAcceptancePct = 65f,
                PopulationAcceptanceTargetPct = 70f,
                PopulationAcceptanceThresholdPct = 65f,
                PopulationAcceptanceAllianceThresholdPct = 60f,
                AlliancePressureAccumulatorSeconds = 89.75f,
            });

            Tick(world, elapsedSeconds: 90f, deltaSeconds: 0.5f);

            TerritorialGovernanceRecognitionComponent recognition =
                ReadRecognition(entityManager, "player");
            ControlPointComponent weakest = ReadControlPoint(entityManager, "player-cp-a");
            DynastyStateComponent dynasty =
                entityManager.GetComponentData<DynastyStateComponent>(playerEntity);
            WorldPressureComponent pressure = ReadWorldPressure(entityManager, "player");
            AIStrategyComponent enemyStrategy = entityManager.GetComponentData<AIStrategyComponent>(enemy);

            if (math.abs(weakest.Loyalty - 79f) > 0.01f ||
                math.abs(dynasty.Legitimacy - 88.4f) > 0.02f ||
                recognition.AlliancePressureCycles != 1 ||
                !recognition.AllianceThresholdReady ||
                pressure.TerritorialGovernanceRecognitionScore != 6 ||
                !enemyStrategy.PlayerGovernanceAlliancePressure)
            {
                throw new InvalidOperationException(
                    $"Phase 2 failed: loyalty={weakest.Loyalty:0.00} legitimacy={dynasty.Legitimacy:0.00} cycles={recognition.AlliancePressureCycles} allianceReady={recognition.AllianceThresholdReady} wpScore={pressure.TerritorialGovernanceRecognitionScore} aiAlliance={enemyStrategy.PlayerGovernanceAlliancePressure}");
            }

            return
                $"phase2Loyalty={weakest.Loyalty:0.0},legitimacy={dynasty.Legitimacy:0.0},cycles={recognition.AlliancePressureCycles}";
        }

        private static string RunBelowThresholdPhase()
        {
            using var world = CreateValidationWorld("governance-no-pressure");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 340f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 90f, withAi: false);
            SeedFaction(entityManager, "enemy", legitimacy: 70f, withAi: false);
            SeedFaction(entityManager, "rival", legitimacy: 69f, withAi: false);
            SeedFaction(entityManager, "ally", legitimacy: 68f, withAi: false);
            SeedHostility(entityManager, player, "enemy", "rival");

            SeedGovernedControlPoint(entityManager, "player-cp-a", "player", 82f, true);
            SeedGovernedControlPoint(entityManager, "player-cp-b", "player", 93f, true);
            SeedGovernedControlPoint(entityManager, "enemy-cp-a", "enemy", 72f, false);
            SeedGovernedControlPoint(entityManager, "rival-cp-a", "rival", 71f, false);
            SeedGovernedControlPoint(entityManager, "ally-cp-a", "ally", 70f, false);
            SeedPrimaryKeep(entityManager, "player", true);

            Entity playerEntity = FindFactionEntity(entityManager, "player");
            entityManager.AddComponentData(playerEntity, new TerritorialGovernanceRecognitionComponent
            {
                Active = true,
                RecognitionEstablished = true,
                StartedAtInWorldDays = 250f,
                RecognizedAtInWorldDays = 251f,
                RequiredSustainSeconds = 90f,
                SustainedSeconds = 90f,
                RequiredVictorySeconds = 120f,
                PopulationAcceptancePct = 50f,
                PopulationAcceptanceTargetPct = 58f,
                PopulationAcceptanceThresholdPct = 65f,
                PopulationAcceptanceAllianceThresholdPct = 60f,
                AlliancePressureAccumulatorSeconds = 89.75f,
            });

            Tick(world, elapsedSeconds: 90f, deltaSeconds: 0.5f);

            TerritorialGovernanceRecognitionComponent recognition =
                ReadRecognition(entityManager, "player");
            ControlPointComponent weakest = ReadControlPoint(entityManager, "player-cp-a");
            DynastyStateComponent dynasty =
                entityManager.GetComponentData<DynastyStateComponent>(playerEntity);
            WorldPressureComponent pressure = ReadWorldPressure(entityManager, "player");

            if (math.abs(weakest.Loyalty - 82f) > 0.01f ||
                math.abs(dynasty.Legitimacy - 90f) > 0.02f ||
                recognition.AlliancePressureCycles != 0 ||
                recognition.AllianceThresholdReady ||
                pressure.TerritorialGovernanceRecognitionScore != 5)
            {
                throw new InvalidOperationException(
                    $"Phase 3 failed: loyalty={weakest.Loyalty:0.00} legitimacy={dynasty.Legitimacy:0.00} cycles={recognition.AlliancePressureCycles} allianceReady={recognition.AllianceThresholdReady} wpScore={pressure.TerritorialGovernanceRecognitionScore}");
            }

            return
                $"phase3NoPressureLoyalty={weakest.Loyalty:0.0},wpScore={pressure.TerritorialGovernanceRecognitionScore}";
        }

        private static string RunVictoryAndEnemyGovernancePhase()
        {
            using var world = CreateValidationWorld("governance-victory");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 420f);
            SeedMatchProgression(entityManager, stageNumber: 5);
            Entity player = SeedFaction(entityManager, "player", legitimacy: 90f, withAi: false);
            Entity enemy = SeedFaction(entityManager, "enemy", legitimacy: 84f, withAi: true);
            SeedFaction(entityManager, "ally", legitimacy: 70f, withAi: false);

            SeedGovernedControlPoint(entityManager, "player-cp-a", "player", 93f, true);
            SeedGovernedControlPoint(entityManager, "player-cp-b", "player", 91f, true);
            SeedGovernedControlPoint(entityManager, "enemy-cp-a", "enemy", 92f, true);
            SeedGovernedControlPoint(entityManager, "enemy-cp-b", "enemy", 90f, true);
            SeedGovernedControlPoint(entityManager, "ally-cp-a", "ally", 70f, false);
            SeedPrimaryKeep(entityManager, "player", true);
            SeedPrimaryKeep(entityManager, "enemy", true);

            Entity playerEntity = FindFactionEntity(entityManager, "player");
            entityManager.AddComponentData(playerEntity, new TerritorialGovernanceRecognitionComponent
            {
                Active = true,
                RecognitionEstablished = true,
                Completed = true,
                StartedAtInWorldDays = 300f,
                RecognizedAtInWorldDays = 302f,
                CompletedAtInWorldDays = 330f,
                RequiredSustainSeconds = 90f,
                SustainedSeconds = 90f,
                RequiredVictorySeconds = 120f,
                VictoryHoldSeconds = 120f,
                PopulationAcceptancePct = 70f,
                PopulationAcceptanceThresholdPct = 65f,
                PopulationAcceptanceAllianceThresholdPct = 60f,
            });

            entityManager.AddComponentData(enemy, new TerritorialGovernanceRecognitionComponent
            {
                Active = true,
                RecognitionEstablished = true,
                RequiredSustainSeconds = 90f,
                SustainedSeconds = 90f,
                RequiredVictorySeconds = 120f,
                VictoryHoldSeconds = 90f,
                PopulationAcceptancePct = 68f,
                PopulationAcceptanceThresholdPct = 65f,
                PopulationAcceptanceAllianceThresholdPct = 60f,
                IntegrationReady = true,
                VictoryReady = true,
                WeakestControlPointId = new FixedString32Bytes("enemy-cp-a"),
            });

            Entity victory = entityManager.CreateEntity(typeof(VictoryStateComponent));
            entityManager.SetComponentData(victory, new VictoryStateComponent
            {
                Status = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
            });

            Tick(world, elapsedSeconds: 1f, deltaSeconds: 1f);

            VictoryStateComponent victoryState = entityManager.GetComponentData<VictoryStateComponent>(victory);
            AIStrategyComponent enemyStrategy = entityManager.GetComponentData<AIStrategyComponent>(enemy);

            if (victoryState.Status != MatchStatus.Won ||
                victoryState.VictoryType != VictoryConditionId.TerritorialGovernance ||
                !enemyStrategy.EnemyGovernanceActive ||
                !enemyStrategy.EnemyGovernanceVictoryPressure ||
                !enemyStrategy.EnemyGovernanceHasTargetPoint)
            {
                throw new InvalidOperationException(
                    $"Phase 4 failed: victoryStatus={victoryState.Status} victoryType={victoryState.VictoryType} enemyGov={enemyStrategy.EnemyGovernanceActive} enemyGovVictory={enemyStrategy.EnemyGovernanceVictoryPressure} enemyTarget={enemyStrategy.EnemyGovernanceHasTargetPoint}");
            }

            return
                $"phase4Victory={victoryState.Status},enemyGovernanceVictory={enemyStrategy.EnemyGovernanceVictoryPressure}";
        }

        private static World CreateValidationWorld(string name)
        {
            var world = new World(name);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<GovernanceCoalitionPressureSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryConditionEvaluationSystem>());
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
            float legitimacy,
            bool withAi)
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
                DormantReserve = 0,
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
                Gold = 10f,
                Food = 40f,
                Water = 40f,
                Wood = 10f,
                Stone = 10f,
                Iron = 10f,
                Influence = 10f,
            });
            entityManager.AddBuffer<DynastyMemberRef>(entity);
            entityManager.AddBuffer<DynastyFallenLedger>(entity);
            entityManager.AddBuffer<HostilityComponent>(entity);

            if (withAi)
            {
                entityManager.AddComponentData(entity, new AIStrategyComponent
                {
                    AttackTimer = 20f,
                    TerritoryTimer = 12f,
                    RaidTimer = 18f,
                    HolyWarTimer = 95f,
                    AssassinationTimer = 80f,
                    MissionaryTimer = 70f,
                    MarriageProposalTimer = 90f,
                    BuildTimer = 8f,
                    ExpansionIntervalSeconds = 8f,
                    ScoutHarassIntervalSeconds = 12f,
                    WorldPressureResponseIntervalSeconds = 15f,
                    ReinforcementIntervalSeconds = 10f,
                });
            }

            return entity;
        }

        private static void SeedHostility(
            EntityManager entityManager,
            Entity factionEntity,
            params string[] hostileFactions)
        {
            DynamicBuffer<HostilityComponent> hostility =
                entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < hostileFactions.Length; i++)
            {
                hostility.Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes(hostileFactions[i]),
                });
            }
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

        private static void SeedPrimaryKeep(
            EntityManager entityManager,
            string factionId,
            bool governed)
        {
            Entity settlement = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(SettlementComponent),
                typeof(PrimaryKeepTag));
            entityManager.SetComponentData(settlement, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(settlement, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(factionId + "-keep"),
                SettlementClassId = new FixedString32Bytes("primary_dynastic_keep"),
                FortificationTier = 3,
                FortificationCeiling = 4,
            });

            if (governed)
            {
                entityManager.AddComponentData(settlement, new GovernorSeatAssignmentComponent
                {
                    GovernorMemberId = new FixedString64Bytes(factionId + "-keep-governor"),
                    SpecializationId = GovernorSpecializationId.KeepCastellan,
                    AnchorType = GovernanceAnchorType.Settlement,
                    PriorityScore = 20f,
                });
            }
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
                $"Control point '{controlPointId}' not found.");
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
