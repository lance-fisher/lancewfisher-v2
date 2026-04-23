#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.PlayerCovertOps;
using Bloodlines.Siege;
using Bloodlines.TerritoryGovernance;
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
    /// Dedicated smoke validator for covert-op full resolution effects. Proves:
    /// 1. successful espionage resolves into an enriched dossier with resource and building context
    /// 2. commander assassination kills the target, drops legitimacy, clears commander links, and stains ruthlessness
    /// 3. governor assassination clears seat assignments and applies governor-specific legitimacy/stewardship fallout
    /// 4. gate-opening floors gate health and exposes a temporary breach window to hostile assault-pressure reads
    /// 5. fire-raising burns over time, supply poisoning opens a raid-style disruption window, and well poisoning adds water strain
    /// </summary>
    public static class BloodlinesPlayerCovertOpsResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-covert-ops-resolution-smoke.log";

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
                message = "Player covert ops resolution smoke validation errored: " + e;
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
            var lines = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunEspionageResolutionPhase(lines);
            ok &= RunCommanderAssassinationPhase(lines);
            ok &= RunGovernorAssassinationPhase(lines);
            ok &= RunGateOpeningSabotagePhase(lines);
            ok &= RunFireAndPoisonSabotagePhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunEspionageResolutionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 48f, legitimacy: 72f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 180f, influence: 96f, spymasterRenown: 2f, legitimacy: 64f);
            CreateSettlement(entityManager, "keep_enemy", "enemy", new float3(100f, 0f, 100f), fortificationTier: 2);
            CreateBuilding(entityManager, "enemy", "command_hall", new float3(101f, 0f, 100f));
            CreateBuilding(entityManager, "enemy", "gatehouse", new float3(102f, 0f, 100f), FortificationRole.Gate);
            CreateBuilding(entityManager, "enemy", "supply_camp", new float3(103f, 0f, 100f), supportsSiegeLogistics: true);

            string enemyCommanderId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.Commander);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                lines.AppendLine("Phase 1 FAIL: could not queue espionage request.");
                return false;
            }

            Advance(world, 0.05d);
            AdvanceInWorldDays(entityManager, PlayerCovertOpsSystem.EspionageDurationInWorldDays + 0.0001f);
            Advance(world, 0.05d);

            if (!debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var reportReadout))
            {
                lines.AppendLine("Phase 1 FAIL: intelligence report readout unavailable.");
                return false;
            }

            if (!reportReadout.Contains("IntelligenceReportCount=1", StringComparison.Ordinal) ||
                !reportReadout.Contains("SourceType=espionage", StringComparison.Ordinal) ||
                !reportReadout.Contains(enemyCommanderId, StringComparison.Ordinal) ||
                !reportReadout.Contains("TargetResourceSummary=gold=180", StringComparison.Ordinal) ||
                !reportReadout.Contains("TargetBuildingSummary=alive=3;gates=1;logistics=1", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 1 FAIL: enriched espionage report drifted: '{reportReadout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: espionage resolved into an enriched enemy-court dossier.");
            return true;
        }

        private static bool RunCommanderAssassinationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 420f, influence: 240f, spymasterRenown: 60f, legitimacy: 72f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 220f, influence: 110f, spymasterRenown: 0f, legitimacy: 70f);
            CreateSettlement(entityManager, "keep_enemy", "enemy", new float3(100f, 0f, 100f), fortificationTier: 1);

            string commanderId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.Commander);
            CreateCommanderUnit(entityManager, "enemy", commanderId, new float3(100f, 0f, 100f));
            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            float ruthlessnessBefore = entityManager.GetComponentData<ConvictionComponent>(playerFaction).Ruthlessness;

            if (!debugScope.CommandSurface.TryDebugIssuePlayerAssassination("player", "enemy", commanderId))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue commander assassination.");
                return false;
            }

            Advance(world, 0.05d);
            AdvanceInWorldDays(entityManager, PlayerCovertOpsSystem.AssassinationDurationInWorldDays + 0.0001f);
            Advance(world, 0.05d);

            var targetMember = GetMemberById(entityManager, enemyFaction, commanderId);
            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            float ruthlessnessAfter = entityManager.GetComponentData<ConvictionComponent>(playerFaction).Ruthlessness;
            if (targetMember.Status != DynastyMemberStatus.Fallen ||
                math.abs((legitimacyBefore - legitimacyAfter) - 9f) > 0.01f ||
                ruthlessnessAfter <= ruthlessnessBefore ||
                HasCommanderLink(entityManager, "enemy", commanderId) ||
                !HasMutualHostility(entityManager, "player", "enemy"))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: commander assassination fallout invalid. status={targetMember.Status} legitimacy={legitimacyBefore:0.##}->{legitimacyAfter:0.##} ruthlessness={ruthlessnessBefore:0.##}->{ruthlessnessAfter:0.##} commanderLink={HasCommanderLink(entityManager, "enemy", commanderId)} hostility={HasMutualHostility(entityManager, "player", "enemy")}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: commander assassination cleared battlefield links and applied bloodline fallout.");
            return true;
        }

        private static bool RunGovernorAssassinationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 28f);
            SeedFaction(entityManager, "player", gold: 420f, influence: 240f, spymasterRenown: 58f, legitimacy: 72f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 220f, influence: 110f, spymasterRenown: 0f, legitimacy: 72f, stewardship: 3f);
            string governorId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.Governor);
            CreateGovernorSeat(entityManager, governorId);

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            float stewardshipBefore = entityManager.GetComponentData<ConvictionComponent>(enemyFaction).Stewardship;

            if (!debugScope.CommandSurface.TryDebugIssuePlayerAssassination("player", "enemy", governorId))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue governor assassination.");
                return false;
            }

            Advance(world, 0.05d);
            AdvanceInWorldDays(entityManager, PlayerCovertOpsSystem.AssassinationDurationInWorldDays + 0.0001f);
            Advance(world, 0.05d);

            var governor = GetMemberById(entityManager, enemyFaction, governorId);
            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction).Legitimacy;
            float stewardshipAfter = entityManager.GetComponentData<ConvictionComponent>(enemyFaction).Stewardship;
            if (governor.Status != DynastyMemberStatus.Fallen ||
                math.abs((legitimacyBefore - legitimacyAfter) - 5f) > 0.01f ||
                math.abs((stewardshipBefore - stewardshipAfter) - 1f) > 0.01f ||
                HasGovernorAssignment(entityManager, governorId) ||
                HasGovernorSpecialization(entityManager, governorId))
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: governor assassination fallout invalid. status={governor.Status} legitimacy={legitimacyBefore:0.##}->{legitimacyAfter:0.##} stewardship={stewardshipBefore:0.##}->{stewardshipAfter:0.##} assignment={HasGovernorAssignment(entityManager, governorId)} specialization={HasGovernorSpecialization(entityManager, governorId)}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: governor assassination cleared governance seats and applied legitimacy/stewardship loss.");
            return true;
        }

        private static bool RunGateOpeningSabotagePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 32f);
            SeedFaction(entityManager, "player", gold: 420f, influence: 240f, spymasterRenown: 48f);
            SeedFaction(entityManager, "enemy", gold: 220f, influence: 110f, spymasterRenown: 0f);
            CreateSettlement(entityManager, "keep_enemy", "enemy", new float3(100f, 0f, 100f), fortificationTier: 2);
            var gatehouse = CreateBuilding(entityManager, "enemy", "gatehouse", new float3(101f, 0f, 100f), FortificationRole.Gate, currentHealth: 400f, maxHealth: 400f);
            var attacker = CreateFieldWaterAttacker(entityManager, "player", new float3(104f, 0f, 100f));

            Advance(world, 0.05d);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "gate_opening", "enemy", gatehouse.Index))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue gate-opening sabotage.");
                return false;
            }

            Advance(world, 0.05d);
            AdvanceInWorldDays(entityManager, (28f / 86400f) + 0.0001f);
            Advance(world, 0.05d);

            if (!debugScope.CommandSurface.TryDebugGetPlayerSabotageState(gatehouse.Index, out var sabotageReadout))
            {
                lines.AppendLine("Phase 4 FAIL: sabotage state readout unavailable.");
                return false;
            }

            var gateHealth = entityManager.GetComponentData<HealthComponent>(gatehouse);
            var fieldWater = entityManager.GetComponentData<FieldWaterComponent>(attacker);
            if (math.abs(gateHealth.Current - 80f) > 0.01f ||
                !sabotageReadout.Contains("OpenBreachCount=1", StringComparison.Ordinal) ||
                !fieldWater.BreachAssaultAdvantageActive)
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: gate-opening effect drifted. health={gateHealth.Current:0.##} readout='{sabotageReadout}' breachActive={fieldWater.BreachAssaultAdvantageActive}.");
                return false;
            }

            Advance(world, 16d);
            Advance(world, 0.05d);
            fieldWater = entityManager.GetComponentData<FieldWaterComponent>(attacker);
            if (fieldWater.BreachAssaultAdvantageActive)
            {
                lines.AppendLine("Phase 4 FAIL: gate exposure should have expired but breach pressure remained active.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: gate-opening exposed a temporary breach window and then lapsed cleanly.");
            return true;
        }

        private static bool RunFireAndPoisonSabotagePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-covert-resolution-phase5");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 36f);
            SeedFaction(entityManager, "player", gold: 520f, influence: 300f, spymasterRenown: 54f);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 220f, influence: 110f, spymasterRenown: 0f);
            CreateSettlement(entityManager, "keep_enemy", "enemy", new float3(100f, 0f, 100f), fortificationTier: 1);
            var commandHall = CreateBuilding(entityManager, "enemy", "command_hall", new float3(101f, 0f, 100f), currentHealth: 300f, maxHealth: 300f);
            var supplyCamp = CreateBuilding(entityManager, "enemy", "supply_camp", new float3(102f, 0f, 100f), supportsSiegeLogistics: true, currentHealth: 280f, maxHealth: 280f);
            var well = CreateBuilding(entityManager, "enemy", "well", new float3(103f, 0f, 100f), currentHealth: 260f, maxHealth: 260f);

            Advance(world, 0.05d);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "fire_raising", "enemy", commandHall.Index) ||
                !debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "supply_poisoning", "enemy", supplyCamp.Index) ||
                !debugScope.CommandSurface.TryDebugIssuePlayerSabotage("player", "well_poisoning", "enemy", well.Index))
            {
                lines.AppendLine("Phase 5 FAIL: could not queue fire/supply/well sabotage requests.");
                return false;
            }

            Advance(world, 0.05d);
            AdvanceInWorldDays(entityManager, (32f / 86400f) + 0.0001f);
            Advance(world, 0.05d);

            float healthAfterIgnition = entityManager.GetComponentData<HealthComponent>(commandHall).Current;
            Advance(world, 5d);
            float healthAfterBurn = entityManager.GetComponentData<HealthComponent>(commandHall).Current;

            if (!debugScope.CommandSurface.TryDebugGetPlayerSabotageState(commandHall.Index, out var fireReadout) ||
                !debugScope.CommandSurface.TryDebugGetPlayerSabotageState(supplyCamp.Index, out var poisonReadout))
            {
                lines.AppendLine("Phase 5 FAIL: sabotage readouts unavailable.");
                return false;
            }

            int waterStrain = entityManager.GetComponentData<RealmConditionComponent>(enemyFaction).WaterStrainStreak;
            if (healthAfterBurn >= healthAfterIgnition ||
                !poisonReadout.Contains("RaidedUntil=", StringComparison.Ordinal) ||
                poisonReadout.Contains("RaidedUntil=0.000", StringComparison.Ordinal) ||
                waterStrain < 2)
            {
                lines.AppendLine(
                    $"Phase 5 FAIL: fire/supply/well effects drifted. fire='{fireReadout}' supply='{poisonReadout}' health={healthAfterIgnition:0.##}->{healthAfterBurn:0.##} waterStrain={waterStrain}.");
                return false;
            }

            lines.AppendLine("Phase 5 PASS: fire burned over time, supply poisoning opened a disruption window, and well poisoning raised water strain.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCovertOpsSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCounterIntelligenceSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<EspionageResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AssassinationResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationStructureLinkSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationDestructionResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SabotageResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachAssaultPressureSystem>());
            simulationGroup.SortSystems();
            return world;
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

        private static void Advance(World world, double seconds, float maxStepSeconds = 0.05f)
        {
            double remaining = seconds;
            double elapsed = world.Time.ElapsedTime;
            while (remaining > 0.0001d)
            {
                float step = (float)math.min((float)remaining, maxStepSeconds);
                elapsed += step;
                world.SetTime(new TimeData(elapsed, step));
                world.Update();
                remaining -= step;
            }
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float gold,
            float influence,
            float spymasterRenown,
            float legitimacy = 60f,
            float stewardship = 0f)
        {
            var factionEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(RealmConditionComponent),
                typeof(FactionLoyaltyComponent),
                typeof(ConvictionComponent));
            entityManager.SetComponentData(factionEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
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
            entityManager.SetComponentData(factionEntity, new PopulationComponent
            {
                Total = 24,
                Available = 12,
                Cap = 30,
                BaseCap = 30,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.SetComponentData(factionEntity, new RealmConditionComponent());
            entityManager.SetComponentData(factionEntity, new FactionLoyaltyComponent
            {
                Current = 75f,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.SetComponentData(factionEntity, new ConvictionComponent
            {
                Stewardship = stewardship,
                Score = stewardship,
                Band = ConvictionScoring.ResolveBand(stewardship),
            });
            entityManager.AddBuffer<HostilityComponent>(factionEntity);

            DynastyBootstrap.AttachDynasty(entityManager, factionEntity, factionId);
            SetDynastyLegitimacy(entityManager, factionEntity, legitimacy);
            SetRoleRenown(entityManager, factionEntity, DynastyRole.Spymaster, spymasterRenown);
            return factionEntity;
        }

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            float3 position,
            int fortificationTier)
        {
            var settlementEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(SettlementComponent),
                typeof(PositionComponent),
                typeof(FortificationComponent),
                typeof(PrimaryKeepTag));
            entityManager.SetComponentData(settlementEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(settlementEntity, new SettlementComponent
            {
                SettlementId = settlementId,
                SettlementClassId = new FixedString32Bytes("primary_dynastic_keep"),
                FortificationTier = fortificationTier,
                FortificationCeiling = math.max(1, fortificationTier + 1),
            });
            entityManager.SetComponentData(settlementEntity, new PositionComponent { Value = position });
            entityManager.SetComponentData(settlementEntity, new FortificationComponent
            {
                SettlementId = settlementId,
                Tier = fortificationTier,
                Ceiling = math.max(1, fortificationTier + 1),
                EcosystemRadiusTiles = FortificationCanon.EcosystemRadiusTiles,
                AuraRadiusTiles = FortificationCanon.AuraRadiusTiles,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
            });
            return settlementEntity;
        }

        private static Entity CreateBuilding(
            EntityManager entityManager,
            string factionId,
            string typeId,
            float3 position,
            FortificationRole fortificationRole = FortificationRole.None,
            bool supportsSiegeLogistics = false,
            float currentHealth = 240f,
            float maxHealth = 240f)
        {
            var buildingEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(BuildingTypeComponent));
            entityManager.SetComponentData(buildingEntity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(buildingEntity, new PositionComponent { Value = position });
            entityManager.SetComponentData(buildingEntity, new HealthComponent
            {
                Current = currentHealth,
                Max = maxHealth,
            });
            entityManager.SetComponentData(buildingEntity, new BuildingTypeComponent
            {
                TypeId = typeId,
                FortificationRole = fortificationRole,
                SupportsSiegeLogistics = supportsSiegeLogistics,
                StructuralDamageMultiplier = fortificationRole == FortificationRole.Gate ? 0.3f : 0.1f,
                BlocksPassage = fortificationRole == FortificationRole.Wall,
            });
            return buildingEntity;
        }

        private static void CreateCommanderUnit(
            EntityManager entityManager,
            string factionId,
            string memberId,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(CommanderComponent),
                typeof(CommanderAtKeepTag));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            entityManager.SetComponentData(entity, new HealthComponent { Current = 90f, Max = 90f });
            entityManager.SetComponentData(entity, new CommanderComponent
            {
                MemberId = memberId,
                Role = new FixedString32Bytes("commander"),
                Renown = 12f,
            });
        }

        private static Entity CreateFieldWaterAttacker(
            EntityManager entityManager,
            string factionId,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(FieldWaterComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            entityManager.SetComponentData(entity, new HealthComponent { Current = 70f, Max = 70f });
            entityManager.SetComponentData(entity, new FieldWaterComponent
            {
                BaseAttackDamage = 10f,
                BaseMaxSpeed = 5f,
                OperationalAttackMultiplier = 1f,
                OperationalSpeedMultiplier = 1f,
                Status = FieldWaterStatus.Steady,
            });
            return entity;
        }

        private static void CreateGovernorSeat(EntityManager entityManager, string governorMemberId)
        {
            var settlementSeat = entityManager.CreateEntity(typeof(GovernorSeatAssignmentComponent));
            entityManager.SetComponentData(settlementSeat, new GovernorSeatAssignmentComponent
            {
                GovernorMemberId = governorMemberId,
                SpecializationId = GovernorSpecializationId.KeepCastellan,
                AnchorType = GovernanceAnchorType.Settlement,
                PriorityScore = 40f,
                LastSyncedInWorldDays = 0f,
            });

            var controlPointSeat = entityManager.CreateEntity(
                typeof(GovernorSeatAssignmentComponent),
                typeof(GovernorSpecializationComponent));
            entityManager.SetComponentData(controlPointSeat, new GovernorSeatAssignmentComponent
            {
                GovernorMemberId = governorMemberId,
                SpecializationId = GovernorSpecializationId.BorderMarshal,
                AnchorType = GovernanceAnchorType.ControlPoint,
                PriorityScore = 64f,
                LastSyncedInWorldDays = 0f,
            });
            entityManager.SetComponentData(controlPointSeat, new GovernorSpecializationComponent
            {
                GovernorMemberId = governorMemberId,
                SpecializationId = GovernorSpecializationId.BorderMarshal,
                ResourceTrickleMultiplier = 1.15f,
                StabilizationMultiplier = 1.3f,
                CaptureResistanceBonus = 0.2f,
                LoyaltyProtectionMultiplier = 1.1f,
                ReserveRegenMultiplier = 1f,
                HealRegenMultiplier = 1f,
            });
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

        private static void SetRoleRenown(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role,
            float renown)
        {
            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (member.Role != role)
                {
                    continue;
                }

                member.Renown = renown;
                entityManager.SetComponentData(members[i].Member, member);
                return;
            }
        }

        private static string GetMemberIdByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role)
        {
            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (member.Role == role)
                {
                    return member.MemberId.ToString();
                }
            }

            throw new InvalidOperationException("Dynasty role not found: " + role);
        }

        private static DynastyMemberComponent GetMemberById(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId)
        {
            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            var key = new FixedString64Bytes(memberId);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (member.MemberId.Equals(key))
                {
                    return member;
                }
            }

            throw new InvalidOperationException("Dynasty member not found: " + memberId);
        }

        private static bool HasMutualHostility(
            EntityManager entityManager,
            string sourceFactionId,
            string targetFactionId)
        {
            return HasHostility(entityManager, sourceFactionId, targetFactionId) &&
                   HasHostility(entityManager, targetFactionId, sourceFactionId);
        }

        private static bool HasHostility(
            EntityManager entityManager,
            string sourceFactionId,
            string targetFactionId)
        {
            var factionEntity = FindFactionEntity(entityManager, sourceFactionId);
            if (factionEntity == Entity.Null || !entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                return false;
            }

            var hostility = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < hostility.Length; i++)
            {
                if (hostility[i].HostileFactionId.Equals(new FixedString32Bytes(targetFactionId)))
                {
                    return true;
                }
            }

            return false;
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            string factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            var key = new FixedString32Bytes(factionId);
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(key))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static bool HasCommanderLink(
            EntityManager entityManager,
            string factionId,
            string memberId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CommanderComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var commanders = query.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < commanders.Length; i++)
            {
                if (factions[i].FactionId.Equals(new FixedString32Bytes(factionId)) &&
                    commanders[i].MemberId.Equals(new FixedString64Bytes(memberId)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasGovernorAssignment(
            EntityManager entityManager,
            string governorMemberId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            using var assignments = query.ToComponentDataArray<GovernorSeatAssignmentComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < assignments.Length; i++)
            {
                if (assignments[i].GovernorMemberId.Equals(new FixedString64Bytes(governorMemberId)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasGovernorSpecialization(
            EntityManager entityManager,
            string governorMemberId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GovernorSpecializationComponent>());
            using var specializations = query.ToComponentDataArray<GovernorSpecializationComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < specializations.Length; i++)
            {
                if (specializations[i].GovernorMemberId.Equals(new FixedString64Bytes(governorMemberId)))
                {
                    return true;
                }
            }

            return false;
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
