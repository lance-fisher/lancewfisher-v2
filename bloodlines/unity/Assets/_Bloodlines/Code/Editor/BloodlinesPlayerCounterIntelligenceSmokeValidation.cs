#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
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
    /// Dedicated smoke validator for player counter-intelligence and intelligence
    /// reports. Proves:
    /// 1. baseline no active watch and no intelligence reports
    /// 2. raising counter-intelligence resolves into a live watch, deducts cost,
    ///    grants stewardship, then lapses after expiry
    /// 3. successful espionage resolves into a live report and expires cleanly
    /// 4. active counter-intelligence depresses hostile espionage odds and a foiled
    ///    attempt yields a dossier, interception counters, and legitimacy gain
    /// </summary>
    public static class BloodlinesPlayerCounterIntelligenceSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-counter-intelligence-smoke.log";

        [MenuItem("Bloodlines/Player Covert Ops/Run Player Counter-Intelligence Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCounterIntelligenceSmokeValidation() =>
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
                message = "Player counter-intelligence smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_COUNTER_INTELLIGENCE_SMOKE " +
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
            ok &= RunWatchActivationAndExpiryPhase(lines);
            ok &= RunEspionageReportLifecyclePhase(lines);
            ok &= RunDossierInterceptionPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-counter-intel-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            SeedFaction(entityManager, "player", gold: 250f, influence: 140f, spymasterRenown: 18f);
            SeedFaction(entityManager, "enemy", gold: 200f, influence: 120f, spymasterRenown: 10f, fortificationTier: 2);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerCounterIntelligence("player", out var watchReadout) ||
                !debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var reportReadout))
            {
                lines.AppendLine("Phase 1 FAIL: baseline debug readout unavailable.");
                return false;
            }

            if (!watchReadout.Contains("CounterIntelligenceActive=false", StringComparison.Ordinal) ||
                !reportReadout.Contains("IntelligenceReportCount=0", StringComparison.Ordinal))
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: expected empty baseline, got watch='{watchReadout}' reports='{reportReadout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: no active watch and no intelligence reports at baseline.");
            return true;
        }

        private static bool RunWatchActivationAndExpiryPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-counter-intel-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                gold: 300f,
                influence: 180f,
                spymasterRenown: 24f,
                fortificationTier: 3,
                loyalty: 76f,
                legitimacy: 68f);
            SeedFaction(entityManager, "enemy", gold: 180f, influence: 100f, spymasterRenown: 12f, fortificationTier: 1);

            var resourcesBefore = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            var convictionBefore = entityManager.GetComponentData<ConvictionComponent>(playerFaction);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerCounterIntelligence("player"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue counter-intelligence request.");
                return false;
            }

            TickOnce(world);
            if (!TryGetSingleActiveOperation(entityManager, "player", CovertOpKindPlayer.CounterIntelligence, out var activeOp))
            {
                lines.AppendLine("Phase 2 FAIL: counter-intelligence operation missing after dispatch.");
                return false;
            }

            AdvanceInWorldDays(
                entityManager,
                PlayerCovertOpsSystem.CounterIntelligenceDurationInWorldDays + 0.0001f);
            TickOnce(world);

            if (!TryGetWatch(entityManager, "player", out var watch) ||
                !debugScope.CommandSurface.TryDebugGetPlayerCounterIntelligence("player", out var watchReadout))
            {
                lines.AppendLine("Phase 2 FAIL: active watch missing after resolution.");
                return false;
            }

            var resourcesAfter = entityManager.GetComponentData<ResourceStockpileComponent>(playerFaction);
            var convictionAfter = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            if (resourcesAfter.Gold != resourcesBefore.Gold - PlayerCovertOpsSystem.CounterIntelligenceCostGold ||
                resourcesAfter.Influence != resourcesBefore.Influence - PlayerCovertOpsSystem.CounterIntelligenceCostInfluence)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: wrong watch cost deduction. gold={resourcesAfter.Gold} influence={resourcesAfter.Influence}.");
                return false;
            }

            if (watch.WatchStrength <= 0f ||
                convictionAfter.Stewardship <= convictionBefore.Stewardship ||
                !watchReadout.Contains("CounterIntelligenceActive=true", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 2 FAIL: malformed active watch state '{watchReadout}'.");
                return false;
            }

            AdvanceInWorldDays(
                entityManager,
                PlayerCovertOpsSystem.CounterIntelligenceWatchDurationInWorldDays + 0.0001f);
            TickOnce(world);

            string lapsedReadout = string.Empty;
            if (TryGetWatch(entityManager, "player", out _) ||
                !debugScope.CommandSurface.TryDebugGetPlayerCounterIntelligence("player", out lapsedReadout) ||
                !lapsedReadout.Contains("CounterIntelligenceActive=false", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 2 FAIL: watch should have lapsed, got '{lapsedReadout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: watchId={watch.WatchId}, strength={watch.WatchStrength:0.##}, opId={activeOp.OperationId}, lapsed cleanly.");
            return true;
        }

        private static bool RunEspionageReportLifecyclePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-counter-intel-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 28f);
            var playerFaction = SeedFaction(entityManager, "player", gold: 320f, influence: 180f, spymasterRenown: 42f, fortificationTier: 1);
            var enemyFaction = SeedFaction(entityManager, "enemy", gold: 180f, influence: 100f, spymasterRenown: 2f, fortificationTier: 0);
            var convictionBefore = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            string enemyCommanderId = GetMemberIdByRole(entityManager, enemyFaction, DynastyRole.Commander);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("player", "enemy"))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue espionage request.");
                return false;
            }

            TickOnce(world);
            AdvanceInWorldDays(entityManager, PlayerCovertOpsSystem.EspionageDurationInWorldDays + 0.0001f);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var reportReadout))
            {
                lines.AppendLine("Phase 3 FAIL: intelligence report readout unavailable after espionage resolution.");
                return false;
            }

            var convictionAfter = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            if (!reportReadout.Contains("IntelligenceReportCount=1", StringComparison.Ordinal) ||
                !reportReadout.Contains("SourceType=espionage", StringComparison.Ordinal) ||
                !reportReadout.Contains("TargetFactionId=enemy", StringComparison.Ordinal) ||
                !reportReadout.Contains(enemyCommanderId, StringComparison.Ordinal) ||
                convictionAfter.Stewardship <= convictionBefore.Stewardship)
            {
                lines.AppendLine($"Phase 3 FAIL: espionage report state invalid '{reportReadout}'.");
                return false;
            }

            AdvanceInWorldDays(
                entityManager,
                PlayerCovertOpsSystem.IntelligenceReportDurationInWorldDays + 0.0001f);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var expiredReadout) ||
                !expiredReadout.Contains("IntelligenceReportCount=0", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 3 FAIL: report should have expired, got '{expiredReadout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 3 PASS: espionage produced report on enemy court containing {enemyCommanderId} and expired cleanly.");
            return true;
        }

        private static bool RunDossierInterceptionPhase(System.Text.StringBuilder lines)
        {
            float baselineChance;
            using (var baselineWorld = CreateValidationWorld("player-counter-intel-phase4-baseline"))
            using (var baselineDebug = new DebugCommandSurfaceScope(baselineWorld))
            {
                var baselineEm = baselineWorld.EntityManager;
                SeedDualClock(baselineEm, 32f);
                SeedFaction(baselineEm, "player", gold: 320f, influence: 180f, spymasterRenown: 18f, fortificationTier: 2);
                SeedFaction(baselineEm, "enemy", gold: 320f, influence: 180f, spymasterRenown: 0f, fortificationTier: 0);

                if (!baselineDebug.CommandSurface.TryDebugIssuePlayerEspionage("enemy", "player"))
                {
                    lines.AppendLine("Phase 4 FAIL: could not queue baseline hostile espionage request.");
                    return false;
                }

                TickOnce(baselineWorld);
                if (!TryGetSingleActiveOperation(baselineEm, "enemy", CovertOpKindPlayer.Espionage, out var baselineOp))
                {
                    lines.AppendLine("Phase 4 FAIL: baseline hostile espionage op missing.");
                    return false;
                }

                baselineChance = baselineOp.ProjectedChance;
            }

            using var world = CreateValidationWorld("player-counter-intel-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 32f);
            var playerFaction = SeedFaction(
                entityManager,
                "player",
                gold: 360f,
                influence: 220f,
                spymasterRenown: 22f,
                fortificationTier: 2,
                loyalty: 78f,
                legitimacy: 70f);
            SeedFaction(entityManager, "enemy", gold: 360f, influence: 220f, spymasterRenown: 0f, fortificationTier: 0);

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (!RaiseCounterIntelligenceWatch(world, debugScope, "player", out var activeWatch))
            {
                lines.AppendLine("Phase 4 FAIL: could not establish defending counter-intelligence watch.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugIssuePlayerEspionage("enemy", "player"))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue defended hostile espionage request.");
                return false;
            }

            TickOnce(world);
            if (!TryGetSingleActiveOperation(entityManager, "enemy", CovertOpKindPlayer.Espionage, out var defendedOp))
            {
                lines.AppendLine("Phase 4 FAIL: defended hostile espionage op missing.");
                return false;
            }

            if (!defendedOp.CounterIntelligenceActive ||
                defendedOp.ProjectedChance >= baselineChance)
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: defended projected chance {defendedOp.ProjectedChance:0.000} should be below baseline {baselineChance:0.000}.");
                return false;
            }

            AdvanceInWorldDays(entityManager, PlayerCovertOpsSystem.EspionageDurationInWorldDays + 0.0001f);
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetIntelligenceReports("player", out var dossierReadout) ||
                !debugScope.CommandSurface.TryDebugGetPlayerCounterIntelligence("player", out var watchReadout))
            {
                lines.AppendLine("Phase 4 FAIL: post-interception debug readout unavailable.");
                return false;
            }

            float legitimacyAfter = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;
            if (!dossierReadout.Contains("IntelligenceReportCount=1", StringComparison.Ordinal) ||
                !dossierReadout.Contains("SourceType=counter_intelligence", StringComparison.Ordinal) ||
                !dossierReadout.Contains("TargetFactionId=enemy", StringComparison.Ordinal) ||
                !dossierReadout.Contains("InterceptType=espionage", StringComparison.Ordinal) ||
                !watchReadout.Contains("FoiledEspionage=1", StringComparison.Ordinal) ||
                !watchReadout.Contains("LastSourceFactionId=enemy", StringComparison.Ordinal) ||
                legitimacyAfter <= legitimacyBefore)
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: dossier/watch state invalid. dossier='{dossierReadout}' watch='{watchReadout}'.");
                return false;
            }

            lines.AppendLine(
                $"Phase 4 PASS: baselineChance={baselineChance:0.000}, defendedChance={defendedOp.ProjectedChance:0.000}, watchId={activeWatch.WatchId}, legitimacy {legitimacyBefore:0.##}->{legitimacyAfter:0.##}.");
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

        private static void AdvanceInWorldDays(EntityManager entityManager, float delta)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            var clockEntity = query.GetSingletonEntity();
            var clock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            clock.InWorldDays += delta;
            entityManager.SetComponentData(clockEntity, clock);
            query.Dispose();
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float gold,
            float influence,
            float spymasterRenown,
            int fortificationTier = 0,
            float loyalty = 70f,
            float legitimacy = 60f)
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
            entityManager.SetComponentData(factionEntity, new FactionLoyaltyComponent
            {
                Current = loyalty,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.SetComponentData(factionEntity, new ConvictionComponent());
            entityManager.AddBuffer<HostilityComponent>(factionEntity);

            DynastyBootstrap.AttachDynasty(entityManager, factionEntity, new FixedString32Bytes(factionId));
            SetDynastyLegitimacy(entityManager, factionEntity, legitimacy);
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
                Value = ResolveSettlementPosition(factionId),
            });
        }

        private static float3 ResolveSettlementPosition(string factionId)
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

        private static void SetDynastyLegitimacy(
            EntityManager entityManager,
            Entity factionEntity,
            float legitimacy)
        {
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = legitimacy;
            entityManager.SetComponentData(factionEntity, dynasty);
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

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;

                hostObject = new GameObject("BloodlinesPlayerCounterIntelSmokeValidation_CommandSurface")
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
