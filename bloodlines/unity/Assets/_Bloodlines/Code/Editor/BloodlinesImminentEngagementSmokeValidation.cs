#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for ImminentEngagementWarningSystem. Verifies four phases:
    ///
    ///   Phase 1: tier-0 gate; warning window must not activate.
    ///   Phase 2: no-threat baseline; warning window must stay inactive.
    ///   Phase 3: threat activation; warning window must activate with brace posture.
    ///   Phase 4: active expired window; must consume and close.
    ///
    /// Artifact: artifacts/unity-imminent-engagement-smoke.log.
    /// </summary>
    public static class BloodlinesImminentEngagementSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-imminent-engagement-smoke.log";
        private static readonly FixedString32Bytes BraceId = new("brace");
        private static readonly FixedString32Bytes SteadyId = new("steady");
        private static readonly FixedString32Bytes TradeTownClassId = new("trade_town");
        private static readonly FixedString32Bytes KeepClassId = new("primary_dynastic_keep");

        [MenuItem("Bloodlines/Fortification/Run Imminent Engagement Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchBloodlinesImminentEngagementSmokeValidation() =>
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
                message = "Imminent engagement smoke errored: " + e;
            }

            string artifact = "BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhase1(sb);
            ok &= RunPhase2(sb);
            ok &= RunPhase3(sb);
            ok &= RunPhase4(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<ImminentEngagementWarningSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity SeedSettlement(
            EntityManager em,
            string settlementId,
            string factionId,
            int tier,
            bool threatActive,
            int readyReserveCount,
            int hostileCount,
            bool bloodlineAtRisk,
            bool isPrimaryDynasticKeep,
            bool active = false,
            bool windowConsumed = false,
            float expiresAt = 0f)
        {
            var entity = em.CreateEntity(
                typeof(SettlementComponent),
                typeof(FactionComponent),
                typeof(FortificationComponent),
                typeof(FortificationReserveComponent),
                typeof(ImminentEngagementComponent));

            em.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = settlementId,
                SettlementClassId = isPrimaryDynasticKeep ? KeepClassId : TradeTownClassId,
                FortificationTier = tier,
                FortificationCeiling = 3,
            });
            em.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            em.SetComponentData(entity, new FortificationComponent
            {
                SettlementId = settlementId,
                Tier = tier,
                Ceiling = 3,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
                FaithWardId = "unwarded",
                FaithWardReserveHealMultiplier = 1f,
                FaithWardReserveMusterMultiplier = 1f,
                FaithWardSurgeActive = false,
            });
            em.SetComponentData(entity, new FortificationReserveComponent
            {
                ThreatActive = threatActive,
                ReadyReserveCount = readyReserveCount,
                LastCommitAt = -999d,
            });
            em.SetComponentData(entity, new ImminentEngagementComponent
            {
                SettlementId = settlementId,
                Active = active,
                WindowConsumed = windowConsumed,
                HostileCount = hostileCount,
                WatchtowerCount = 0,
                BloodlineAtRisk = bloodlineAtRisk,
                SelectedResponseId = SteadyId,
                SelectedResponseLabel = ImminentEngagementCanon.Steady.Label,
                CommanderRecallIssuedAt = -999f,
                LastActivationAt = -999f,
                ExpiresAt = expiresAt,
                IsPrimaryDynasticKeep = isPrimaryDynasticKeep,
            });
            return entity;
        }

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("engagement-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);

            Entity settlement = SeedSettlement(
                em,
                settlementId: "tier0",
                factionId: "player",
                tier: 0,
                threatActive: true,
                readyReserveCount: 1,
                hostileCount: 3,
                bloodlineAtRisk: false,
                isPrimaryDynasticKeep: false);

            world.Update();
            var engagement = em.GetComponentData<ImminentEngagementComponent>(settlement);
            if (engagement.Active)
            {
                sb.AppendLine("Phase 1 FAIL: tier-0 settlement activated despite threat.");
                return false;
            }

            sb.AppendLine("Phase 1 PASS: active=False");
            return true;
        }

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("engagement-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);

            Entity settlement = SeedSettlement(
                em,
                settlementId: "baseline",
                factionId: "player",
                tier: 1,
                threatActive: false,
                readyReserveCount: 1,
                hostileCount: 0,
                bloodlineAtRisk: false,
                isPrimaryDynasticKeep: false);

            world.Update();
            var engagement = em.GetComponentData<ImminentEngagementComponent>(settlement);
            if (engagement.Active)
            {
                sb.AppendLine("Phase 2 FAIL: no-threat settlement activated.");
                return false;
            }

            sb.AppendLine("Phase 2 PASS: active=False");
            return true;
        }

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("engagement-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);

            Entity settlement = SeedSettlement(
                em,
                settlementId: "activation",
                factionId: "enemy",
                tier: 1,
                threatActive: true,
                readyReserveCount: 1,
                hostileCount: 3,
                bloodlineAtRisk: false,
                isPrimaryDynasticKeep: false);

            world.Update();
            var engagement = em.GetComponentData<ImminentEngagementComponent>(settlement);
            if (!engagement.Active ||
                engagement.RemainingSeconds <= 0f ||
                !engagement.SelectedResponseId.Equals(BraceId))
            {
                sb.AppendLine(
                    $"Phase 3 FAIL: expected active brace window. active={engagement.Active} " +
                    $"remaining={engagement.RemainingSeconds:F2} response={engagement.SelectedResponseId}.");
                return false;
            }

            sb.AppendLine(
                $"Phase 3 PASS: active=True remaining={engagement.RemainingSeconds:F2} " +
                $"response={engagement.SelectedResponseId}");
            return true;
        }

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("engagement-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);

            Entity settlement = SeedSettlement(
                em,
                settlementId: "expiry",
                factionId: "enemy",
                tier: 1,
                threatActive: true,
                readyReserveCount: 2,
                hostileCount: 1,
                bloodlineAtRisk: false,
                isPrimaryDynasticKeep: false,
                active: true,
                windowConsumed: false,
                expiresAt: -1f);

            world.Update();
            var engagement = em.GetComponentData<ImminentEngagementComponent>(settlement);
            if (engagement.Active || !engagement.WindowConsumed)
            {
                sb.AppendLine(
                    $"Phase 4 FAIL: expected consumed inactive window. active={engagement.Active} " +
                    $"windowConsumed={engagement.WindowConsumed}.");
                return false;
            }

            sb.AppendLine(
                $"Phase 4 PASS: active=False windowConsumed=True engagedAt={engagement.EngagedAt:F2}");
            return true;
        }
    }
}
#endif
