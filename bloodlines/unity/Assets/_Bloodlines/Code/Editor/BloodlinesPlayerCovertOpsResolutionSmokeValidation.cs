#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerCovertOps;
using Bloodlines.Systems;
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
    /// Dedicated covert-ops resolution validator. Proves:
    /// 1. assassination success kills the ruling target and triggers same-frame succession fallout
    /// 2. sabotage success damages a target building and freezes queued production
    /// 3. espionage success writes a dossier containing member, building, and resource summaries
    /// 4. assassination, sabotage, and espionage failures all apply the configured legitimacy / counter-intel penalties
    /// </summary>
    public static class BloodlinesPlayerCovertOpsResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-covert-ops-resolution-smoke.log";
        private const float StepSeconds = 0.05f;

        [MenuItem("Bloodlines/Player Covert Ops/Run Player Covert Ops Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCovertOpsResolutionSmokeValidation() =>
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
                message = "Player covert-ops resolution smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_COVERT_OPS_RESOLUTION_SMOKE " +
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
            var lines = new StringBuilder();
            bool ok = true;
            ok &= RunAssassinationSuccessionPhase(lines);
            ok &= RunSabotageProductionHaltPhase(lines);
            ok &= RunEspionageDossierPhase(lines);
            ok &= RunFailurePenaltyPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunAssassinationSuccessionPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 50f, legitimacy: 72f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 220f, influence: 120f, spymasterRenown: 0f, legitimacy: 84f);
            SeedControlPoint(entityManager, "enemy-cp-1", "enemy", new float3(96f, 0f, 96f), 82f);
            SeedControlPoint(entityManager, "enemy-cp-2", "enemy", new float3(104f, 0f, 96f), 78f);
            SeedControlPoint(entityManager, "enemy-cp-3", "enemy", new float3(100f, 0f, 108f), 74f);

            string rulerId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.HeadOfBloodline);
            string successorId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.HeirDesignate);
            float successorRenownBefore = GetMemberById(entityManager, enemyFaction, successorId).Renown;
            SetMemberAge(entityManager, enemyFaction, successorId, 16f);
            float[] loyaltyBefore = GetOwnedControlPointLoyalties(entityManager, "enemy");
            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;

            if (!debugScope.CommandSurface.TryDebugIssuePlayerAssassination("player", "enemy", rulerId))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue assassination request.");
                return false;
            }

            TickOnce(world);
            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Assassination);
            TickOnce(world);

            var fallenRuler = GetMemberById(entityManager, enemyFaction, rulerId);
            var successorAfter = GetMemberById(entityManager, enemyFaction, successorId);
            float[] loyaltyAfter = GetOwnedControlPointLoyalties(entityManager, "enemy");
            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;

            if (fallenRuler.Status != DynastyMemberStatus.Fallen ||
                successorAfter.Status != DynastyMemberStatus.Ruling ||
                successorAfter.Role != DynastyRole.HeadOfBloodline ||
                successorAfter.Renown <= successorRenownBefore ||
                !entityManager.HasComponent<SuccessionCrisisComponent>(enemyFaction))
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: assassination fallout invalid. fallen={fallenRuler.Status} successorStatus={successorAfter.Status} successorRole={successorAfter.Role} successorRenown={successorAfter.Renown}.");
                return false;
            }

            for (int i = 0; i < loyaltyBefore.Length; i++)
            {
                if (!(loyaltyAfter[i] < loyaltyBefore[i]))
                {
                    lines.AppendLine($"Phase 1 FAIL: expected loyalty shock on control point {i}, before={loyaltyBefore[i]:0.##} after={loyaltyAfter[i]:0.##}.");
                    return false;
                }
            }

            if (!(legitimacyAfter < legitimacyBefore))
            {
                lines.AppendLine($"Phase 1 FAIL: legitimacy should fall on ruler assassination, before={legitimacyBefore:0.##} after={legitimacyAfter:0.##}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 1 PASS: ruler {rulerId} fell, successor {successorId} ascended, legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}, loyalty {loyaltyBefore[0]:0.##}->{loyaltyAfter[0]:0.##}/{loyaltyBefore[1]:0.##}->{loyaltyAfter[1]:0.##}/{loyaltyBefore[2]:0.##}->{loyaltyAfter[2]:0.##}.");
            return true;
        }

        private static bool RunSabotageProductionHaltPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 30f);
            SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 24f, legitimacy: 72f);
            SeedFaction(entityManager, "enemy", gold: 220f, influence: 120f, spymasterRenown: 0f, legitimacy: 70f);
            var supplyCamp = CreateBuilding(
                entityManager,
                "enemy",
                "supply_camp",
                new float3(102f, 0f, 100f),
                supportsSiegeLogistics: true);
            QueueProductionItem(entityManager, supplyCamp, "levy-spears", 12f);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "supply_poisoning", "enemy", supplyCamp.Index))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue sabotage request.");
                return false;
            }

            TickOnce(world);
            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Sabotage);
            TickOnce(world);

            float frozenStart = entityManager.GetBuffer<ProductionQueueItemElement>(supplyCamp)[0].RemainingSeconds;
            TickOnce(world);
            float frozenEnd = entityManager.GetBuffer<ProductionQueueItemElement>(supplyCamp)[0].RemainingSeconds;
            var health = entityManager.GetComponentData<HealthComponent>(supplyCamp);

            if (!debugScope.CommandSurface.TryDebugGetPlayerSabotageStatus(supplyCamp.Index, out var sabotageReadout))
            {
                lines.AppendLine("Phase 2 FAIL: sabotage status readout unavailable.");
                return false;
            }

            if (health.Current >= health.Max ||
                math.abs(frozenEnd - frozenStart) > 0.0001f ||
                !sabotageReadout.Contains("SabotageStatusActive=true", StringComparison.Ordinal) ||
                !sabotageReadout.Contains("Subtype=supply_poisoning", StringComparison.Ordinal))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: sabotage fallout invalid. health={health.Current:0.##}/{health.Max:0.##} frozenStart={frozenStart:0.000} frozenEnd={frozenEnd:0.000} readout='{sabotageReadout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: supply camp damaged to {health.Current:0.##}/{health.Max:0.##} and queue froze at {frozenStart:0.000}s with readout '{sabotageReadout}'.");
            return true;
        }

        private static bool RunEspionageDossierPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 36f);
            SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 42f, legitimacy: 72f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 321f, influence: 77f, spymasterRenown: 0f, legitimacy: 66f);
            CreateBuilding(entityManager, "enemy", "gatehouse", new float3(100f, 0f, 100f), FortificationRole.Gate);
            CreateBuilding(entityManager, "enemy", "supply_camp", new float3(106f, 0f, 100f), supportsSiegeLogistics: true);
            string commanderId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.Commander);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue espionage request.");
                return false;
            }

            TickOnce(world);
            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Espionage);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var reportReadout))
            {
                lines.AppendLine("Phase 3 FAIL: intelligence report readout unavailable.");
                return false;
            }

            if (!reportReadout.Contains("IntelligenceReportCount=1", StringComparison.Ordinal) ||
                !reportReadout.Contains("SourceType=espionage", StringComparison.Ordinal) ||
                !reportReadout.Contains(commanderId, StringComparison.Ordinal) ||
                !reportReadout.Contains("gatehouse:1", StringComparison.Ordinal) ||
                !reportReadout.Contains("supply_camp:1", StringComparison.Ordinal) ||
                !reportReadout.Contains("gold=321", StringComparison.Ordinal) ||
                !reportReadout.Contains("influence=77", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 3 FAIL: dossier content missing expected fields: '{reportReadout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 3 PASS: espionage dossier captured member {commanderId}, hostile buildings, and resource summary '{reportReadout}'.");
            return true;
        }

        private static bool RunFailurePenaltyPhase(StringBuilder lines)
        {
            bool assassinationOk = RunAssassinationFailureSubphase(out string assassinationMessage);
            bool espionageOk = RunEspionageFailureSubphase(out string espionageMessage);
            bool sabotageOk = RunSabotageFailureSubphase(out string sabotageMessage);

            if (!assassinationOk || !espionageOk || !sabotageOk)
            {
                lines.AppendLine("Phase 4 FAIL: " + assassinationMessage + " | " + espionageMessage + " | " + sabotageMessage);
                return false;
            }

            lines.AppendLine("Phase 4 PASS: " + assassinationMessage + " | " + espionageMessage + " | " + sabotageMessage);
            return true;
        }

        private static bool RunAssassinationFailureSubphase(out string message)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase4-assassination");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 42f);
            SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 0f, legitimacy: 68f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 220f, influence: 120f, spymasterRenown: 36f, legitimacy: 71f, fortificationTier: 3);
            string targetMemberId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.HeadOfBloodline);

            if (!RaiseCounterIntelligenceWatch(world, debugScope, "enemy", out var watchBefore))
            {
                message = "assassination failure could not establish defending watch";
                return false;
            }

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            if (!debugScope.CommandSurface.TryDebugIssuePlayerAssassination("player", "enemy", targetMemberId))
            {
                message = "assassination failure could not queue request";
                return false;
            }

            TickOnce(world);
            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Assassination);
            TickOnce(world);

            if (!TryGetWatch(entityManager, "enemy", out var watchAfter))
            {
                message = "assassination failure lost defending watch";
                return false;
            }

            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            if (watchAfter.WatchStrength <= watchBefore.WatchStrength ||
                legitimacyAfter <= legitimacyBefore)
            {
                message = $"assassination failure expected stronger defense / legitimacy gain, got watch {watchBefore.WatchStrength:0.##}->{watchAfter.WatchStrength:0.##}, legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}";
                return false;
            }

            message = $"assassination legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}, watch {watchBefore.WatchStrength:0.##}->{watchAfter.WatchStrength:0.##}";
            return true;
        }

        private static bool RunEspionageFailureSubphase(out string message)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase4-espionage");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 46f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 0f, legitimacy: 70f);
            SeedFaction(entityManager, "enemy", gold: 220f, influence: 120f, spymasterRenown: 36f, legitimacy: 71f, fortificationTier: 3);
            SetMemberRenown(entityManager, playerFaction, DynastyRole.Diplomat, 0f);
            SetMemberRenown(entityManager, playerFaction, DynastyRole.Merchant, 0f);

            if (!RaiseCounterIntelligenceWatch(world, debugScope, "player", out var watchBefore))
            {
                message = "espionage failure could not establish attacking watch";
                return false;
            }

            if (!RaiseCounterIntelligenceWatch(world, debugScope, "enemy", out _))
            {
                message = "espionage failure could not establish defending watch";
                return false;
            }

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                message = "espionage failure could not queue request";
                return false;
            }

            TickOnce(world);
            if (!TryGetSingleActiveOperation(entityManager, "player", CovertOpKindPlayer.Espionage, out var operation) ||
                operation.SuccessScore >= 0f)
            {
                message = "espionage failure fixture did not produce a negative success score";
                return false;
            }

            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Espionage);
            TickOnce(world);

            if (!TryGetWatch(entityManager, "player", out var watchAfter))
            {
                message = "espionage failure lost attacking watch";
                return false;
            }

            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (watchAfter.WatchStrength >= watchBefore.WatchStrength ||
                legitimacyAfter >= legitimacyBefore)
            {
                message = $"espionage failure expected weaker watch / legitimacy loss, got watch {watchBefore.WatchStrength:0.##}->{watchAfter.WatchStrength:0.##}, legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}";
                return false;
            }

            message = $"espionage legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}, watch {watchBefore.WatchStrength:0.##}->{watchAfter.WatchStrength:0.##}";
            return true;
        }

        private static bool RunSabotageFailureSubphase(out string message)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase4-sabotage");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 50f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 0f, legitimacy: 69f);
            SeedFaction(entityManager, "enemy", gold: 220f, influence: 120f, spymasterRenown: 22f, legitimacy: 71f, fortificationTier: 4);
            var gatehouse = CreateBuilding(
                entityManager,
                "enemy",
                "gatehouse",
                new float3(102f, 0f, 100f),
                FortificationRole.Gate);

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (!debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "gate_opening", "enemy", gatehouse.Index))
            {
                message = "sabotage failure could not queue request";
                return false;
            }

            TickOnce(world);
            AdvanceToResolvedOperation(entityManager, "player", CovertOpKindPlayer.Sabotage);
            TickOnce(world);

            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (legitimacyAfter >= legitimacyBefore)
            {
                message = $"sabotage failure expected legitimacy loss, got {legitimacyBefore:0.##}->{legitimacyAfter:0.##}";
                return false;
            }

            message = $"sabotage legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            simulationGroup.AddSystemToUpdateList(endSimulation);
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCovertOpsSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCounterIntelligenceSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<EspionageResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<UnitProductionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SabotageResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AssassinationResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastySuccessionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SuccessionCrisisEvaluationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageDeathDissolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DeathResolutionSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, StepSeconds));
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

        private static void AdvanceInWorldDays(EntityManager entityManager, float delta)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            var clockEntity = query.GetSingletonEntity();
            var clock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            clock.InWorldDays += delta;
            entityManager.SetComponentData(clockEntity, clock);
            query.Dispose();
        }

        private static void AdvanceToResolvedOperation(
            EntityManager entityManager,
            string factionId,
            CovertOpKindPlayer kind)
        {
            if (!TryGetSingleActiveOperation(entityManager, factionId, kind, out var operation))
            {
                throw new InvalidOperationException("Expected active operation of kind " + kind + " for faction " + factionId);
            }

            float currentDays = GetInWorldDays(entityManager);
            AdvanceInWorldDays(entityManager, math.max(0f, operation.ResolveAtInWorldDays - currentDays) + 0.0001f);
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float gold,
            float influence,
            float spymasterRenown,
            float legitimacy,
            int fortificationTier = 0)
        {
            var factionEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(RealmConditionComponent),
                typeof(ConvictionComponent));
            entityManager.SetComponentData(factionEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.SetComponentData(factionEntity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = 180f,
                Water = 150f,
                Wood = 60f,
                Stone = 45f,
                Iron = 22f,
                Influence = influence,
            });
            entityManager.SetComponentData(factionEntity, new PopulationComponent
            {
                Total = 28,
                Cap = 34,
                BaseCap = 34,
                CapBonus = 0,
                Available = 16,
                GrowthAccumulator = 0f,
            });
            entityManager.SetComponentData(factionEntity, new RealmConditionComponent());
            entityManager.SetComponentData(factionEntity, new ConvictionComponent());
            entityManager.AddBuffer<HostilityComponent>(factionEntity);

            DynastyBootstrap.AttachDynasty(entityManager, factionEntity, new FixedString32Bytes(factionId));
            SetDynastyLegitimacy(entityManager, factionEntity, legitimacy);
            SetMemberRenown(entityManager, factionEntity, DynastyRole.Spymaster, spymasterRenown);
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
                typeof(SettlementComponent),
                typeof(PositionComponent),
                typeof(PrimaryKeepTag));
            entityManager.SetComponentData(settlementEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(settlementEntity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes("keep_" + factionId),
                SettlementClassId = new FixedString32Bytes("military_fort"),
                FortificationTier = fortificationTier,
                FortificationCeiling = math.max(1, fortificationTier + 1),
            });
            entityManager.SetComponentData(settlementEntity, new PositionComponent
            {
                Value = ResolveFactionPosition(factionId),
            });
        }

        private static void SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float3 position,
            float loyalty)
        {
            var entity = entityManager.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("continent-test"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("border_settlement"),
                FortificationTier = 1,
                RadiusTiles = 8f,
                CaptureTimeSeconds = 15f,
                GoldTrickle = 2f,
                FoodTrickle = 1f,
                WaterTrickle = 1f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 1f,
            });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
        }

        private static Entity CreateBuilding(
            EntityManager entityManager,
            string factionId,
            string buildingId,
            float3 position,
            FortificationRole fortificationRole = FortificationRole.None,
            bool supportsSiegeLogistics = false)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(BuildingTypeComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 400f,
                Max = 400f,
            });
            entityManager.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = new FixedString64Bytes(buildingId),
                FortificationRole = fortificationRole,
                StructuralDamageMultiplier = fortificationRole == FortificationRole.Gate ? 0.3f : 0.1f,
                PopulationCapBonus = 0,
                BlocksPassage = fortificationRole == FortificationRole.Wall,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = supportsSiegeLogistics,
            });
            return entity;
        }

        private static void QueueProductionItem(
            EntityManager entityManager,
            Entity buildingEntity,
            string unitId,
            float remainingSeconds)
        {
            if (!entityManager.HasComponent<ProductionFacilityComponent>(buildingEntity))
            {
                entityManager.AddComponentData(buildingEntity, new ProductionFacilityComponent());
            }

            if (!entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                entityManager.AddBuffer<ProductionQueueItemElement>(buildingEntity);
            }

            entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity).Add(new ProductionQueueItemElement
            {
                UnitId = new FixedString64Bytes(unitId),
                DisplayName = new FixedString64Bytes(unitId),
                RemainingSeconds = remainingSeconds,
                TotalSeconds = remainingSeconds,
                PopulationCost = 1,
                BloodPrice = 0,
                BloodLoadDelta = 0f,
                MaxHealth = 80f,
                MaxSpeed = 4f,
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                Stage = 1,
                GoldCost = 15,
                FoodCost = 0,
                WaterCost = 0,
                WoodCost = 0,
                StoneCost = 0,
                IronCost = 0,
                InfluenceCost = 0,
            });
        }

        private static bool RaiseCounterIntelligenceWatch(
            World world,
            DebugCommandSurfaceScope debugScope,
            string factionId,
            out PlayerCounterIntelligenceComponent watch)
        {
            watch = default;
            var entityManager = world.EntityManager;
            if (!debugScope.CommandSurface.TryDebugIssuePlayerCounterIntelligence(factionId))
            {
                return false;
            }

            TickOnce(world);
            AdvanceInWorldDays(
                entityManager,
                PlayerCovertOpsSystem.CounterIntelligenceDurationInWorldDays + 0.0001f);
            TickOnce(world);
            return TryGetWatch(entityManager, factionId, out watch);
        }

        private static void SetDynastyLegitimacy(
            EntityManager entityManager,
            Entity factionEntity,
            float legitimacy)
        {
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = legitimacy;
            entityManager.SetComponentData(factionEntity, dynasty);
        }

        private static void SetMemberRenown(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role,
            float renown)
        {
            if (!TryGetMemberEntityByRole(entityManager, factionEntity, role, out var memberEntity, out var member))
            {
                throw new InvalidOperationException("Dynasty role not found when setting renown: " + role);
            }

            member.Renown = renown;
            entityManager.SetComponentData(memberEntity, member);
        }

        private static void SetMemberAge(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            float ageYears)
        {
            if (!TryGetMemberEntityById(entityManager, factionEntity, memberId, out var memberEntity, out var member))
            {
                throw new InvalidOperationException("Dynasty member not found when setting age: " + memberId);
            }

            member.AgeYears = ageYears;
            entityManager.SetComponentData(memberEntity, member);
        }

        private static string GetMemberIdByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role)
        {
            if (!TryGetMemberEntityByRole(entityManager, factionEntity, role, out _, out var member))
            {
                throw new InvalidOperationException("Dynasty role not found: " + role);
            }

            return member.MemberId.ToString();
        }

        private static DynastyMemberComponent GetMemberById(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId)
        {
            if (!TryGetMemberEntityById(entityManager, factionEntity, memberId, out _, out var member))
            {
                throw new InvalidOperationException("Dynasty member not found: " + memberId);
            }

            return member;
        }

        private static bool TryGetMemberEntityByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            memberEntity = Entity.Null;
            member = default;
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (candidate.Role != role)
                {
                    continue;
                }

                memberEntity = members[i].Member;
                member = candidate;
                return true;
            }

            return false;
        }

        private static bool TryGetMemberEntityById(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            memberEntity = Entity.Null;
            member = default;
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var memberKey = new FixedString64Bytes(memberId);
            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (!candidate.MemberId.Equals(memberKey))
                {
                    continue;
                }

                memberEntity = members[i].Member;
                member = candidate;
                return true;
            }

            return false;
        }

        private static float[] GetOwnedControlPointLoyalties(
            EntityManager entityManager,
            string factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            query.Dispose();

            using var values = new NativeList<float>(Allocator.Temp);
            var factionKey = new FixedString32Bytes(factionId);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].OwnerFactionId.Equals(factionKey))
                {
                    values.Add(controlPoints[i].Loyalty);
                }
            }

            var array = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i];
            }

            return array;
        }

        private static bool TryGetSingleActiveOperation(
            EntityManager entityManager,
            string factionId,
            CovertOpKindPlayer kind,
            out PlayerCovertOpsResolutionComponent operation)
        {
            operation = default;
            var factionKey = new FixedString32Bytes(factionId);
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < operations.Length; i++)
            {
                if (!operations[i].Active ||
                    operations[i].Kind != kind ||
                    !operations[i].SourceFactionId.Equals(factionKey))
                {
                    continue;
                }

                operation = operations[i];
                return true;
            }

            return false;
        }

        private static bool TryGetWatch(
            EntityManager entityManager,
            string factionId,
            out PlayerCounterIntelligenceComponent watch)
        {
            watch = default;
            var factionEntity = FindFactionEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCounterIntelligenceComponent>(factionEntity))
            {
                return false;
            }

            watch = entityManager.GetComponentData<PlayerCounterIntelligenceComponent>(factionEntity);
            return true;
        }

        private static float GetInWorldDays(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            float inWorldDays = query.GetSingleton<DualClockComponent>().InWorldDays;
            query.Dispose();
            return inWorldDays;
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            Entity fallback = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (entityManager.HasComponent<FactionKindComponent>(entities[i]) ||
                    entityManager.HasComponent<ResourceStockpileComponent>(entities[i]) ||
                    entityManager.HasComponent<DynastyStateComponent>(entities[i]))
                {
                    return entities[i];
                }

                if (fallback == Entity.Null)
                {
                    fallback = entities[i];
                }
            }

            return fallback;
        }

        private static float3 ResolveFactionPosition(string factionId)
        {
            if (string.Equals(factionId, "player", StringComparison.Ordinal))
            {
                return new float3(20f, 0f, 20f);
            }

            if (string.Equals(factionId, "enemy", StringComparison.Ordinal))
            {
                return new float3(100f, 0f, 100f);
            }

            return new float3(60f, 0f, 60f);
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

                hostObject = new GameObject("BloodlinesPlayerCovertOpsResolutionSmokeValidation_CommandSurface")
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
