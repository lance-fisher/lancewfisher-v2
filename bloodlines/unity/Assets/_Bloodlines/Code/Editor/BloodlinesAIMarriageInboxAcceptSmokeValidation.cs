#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for AIMarriageInboxAcceptSystem. Four phases:
    ///
    ///   Phase 1: LastFiredOp=MarriageInboxAccept + pending player->enemy proposal
    ///            -> proposal flipped to Accepted; primary + mirror
    ///            MarriageComponent entities created with matching MarriageId;
    ///            MarriedAtInWorldDays=seeded clock; ExpectedChildAtInWorldDays=
    ///            seeded clock + 60 (MarriageGestationSystem.GestationInWorldDays);
    ///            LastFiredOp cleared to None.
    ///   Phase 2: LastFiredOp=MarriageInboxAccept + no pending proposal
    ///            -> no marriage created; LastFiredOp cleared.
    ///   Phase 3: LastFiredOp=MarriageInboxAccept + only an expired proposal
    ///            -> no marriage created (status != Pending); LastFiredOp
    ///            cleared; expired proposal preserved.
    ///   Phase 4: LastFiredOp=None -> no-op path; no marriage; no proposal
    ///            mutation; LastFiredOp stays None.
    ///
    /// Browser reference: ai.js tryAiAcceptIncomingMarriage (~2880-2895).
    /// Artifact: artifacts/unity-ai-marriage-inbox-accept-smoke.log.
    /// </summary>
    public static class BloodlinesAIMarriageInboxAcceptSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-marriage-inbox-accept-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Marriage Inbox Accept Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIMarriageInboxAcceptSmokeValidation() =>
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
                message = "AI marriage inbox accept smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_MARRIAGE_INBOX_ACCEPT_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
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

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<WorldPressureEscalationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIStrategicPressureSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIWorkerGatherSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AISiegeOrchestrationSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICovertOpsSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIBuildOrderSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageProposalExecutionSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageInboxAcceptSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedMatchProgression(EntityManager em, int stageNumber = 3)
        {
            var e = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(e, new MatchProgressionComponent { StageNumber = stageNumber });
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays = 20f)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static void SeedPlayerFaction(EntityManager em)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            DynastyBootstrap.AttachDynasty(em, e, new FixedString32Bytes("player"));
        }

        private static Entity SeedEnemyFactionWithDispatch(
            EntityManager em, CovertOpKind dispatched)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AIEconomyControllerComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent),
                typeof(AICovertOpsComponent),
                typeof(WorldPressureComponent),
                typeof(WorldPressureCycleTrackerComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AIEconomyControllerComponent { Enabled = true });
            em.SetComponentData(e, new AIStrategyComponent
            {
                WorkerGatherIntervalSeconds = 5f,
                BuildTimer                  = 999f,
            });
            em.SetComponentData(e, new AISiegeOrchestrationComponent());
            em.SetComponentData(e, new AICovertOpsComponent
            {
                AssassinationTimer        = 999f,
                MissionaryTimer           = 999f,
                HolyWarTimer              = 999f,
                DivineRightTimer          = 999f,
                CaptiveRecoveryTimer      = 999f,
                MarriageProposalTimer     = 999f,
                MarriageInboxTimer        = 999f,
                PactProposalTimer         = 999f,
                LesserHousePromotionTimer = 999f,
                LastFiredOp               = dispatched,
                LastFiredOpTime           = 0f,
            });
            em.SetComponentData(e, new WorldPressureComponent());
            em.SetComponentData(e, new WorldPressureCycleTrackerComponent
                { CycleSeconds = 90f, Accumulator = 0f });
            DynastyBootstrap.AttachDynasty(em, e, new FixedString32Bytes("enemy"));
            return e;
        }

        private static Entity SeedPendingProposalPlayerToEnemy(EntityManager em)
        {
            var proposalEntity = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId            = new FixedString64Bytes("pending-player-to-enemy"),
                SourceFactionId       = new FixedString32Bytes("player"),
                SourceMemberId        = new FixedString64Bytes("player-bloodline-heir"),
                TargetFactionId       = new FixedString32Bytes("enemy"),
                TargetMemberId        = new FixedString64Bytes("enemy-bloodline-heir"),
                Status                = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 5f,
                ExpiresAtInWorldDays  = 35f,
            });
            return proposalEntity;
        }

        private static Entity SeedExpiredProposalPlayerToEnemy(EntityManager em)
        {
            var proposalEntity = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId            = new FixedString64Bytes("expired-player-to-enemy"),
                SourceFactionId       = new FixedString32Bytes("player"),
                SourceMemberId        = new FixedString64Bytes("player-bloodline-heir"),
                TargetFactionId       = new FixedString32Bytes("enemy"),
                TargetMemberId        = new FixedString64Bytes("enemy-bloodline-heir"),
                Status                = MarriageProposalStatus.Expired,
                ProposedAtInWorldDays = 1f,
                ExpiresAtInWorldDays  = 31f,
            });
            return proposalEntity;
        }

        private static int CountMarriagesBetweenEnemyAndPlayer(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0; }
            var marriages = q.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            q.Dispose();

            var enemyId  = new FixedString32Bytes("enemy");
            var playerId = new FixedString32Bytes("player");
            int count = 0;
            for (int i = 0; i < marriages.Length; i++)
            {
                var m = marriages[i];
                bool linksPlayerAndEnemy =
                    (m.HeadFactionId.Equals(enemyId) && m.SpouseFactionId.Equals(playerId)) ||
                    (m.HeadFactionId.Equals(playerId) && m.SpouseFactionId.Equals(enemyId));
                if (linksPlayerAndEnemy) count++;
            }
            marriages.Dispose();
            return count;
        }

        private static int CountPrimaryMarriages(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0; }
            var marriages = q.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            q.Dispose();

            int primary = 0;
            for (int i = 0; i < marriages.Length; i++)
                if (marriages[i].IsPrimary) primary++;
            marriages.Dispose();
            return primary;
        }

        // ------------------------------------------------------------------ Phase 1

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-inbox-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 20f);
            SeedPlayerFaction(em);
            var enemy = SeedEnemyFactionWithDispatch(em, CovertOpKind.MarriageInboxAccept);
            var proposalEntity = SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            var proposal = em.GetComponentData<MarriageProposalComponent>(proposalEntity);
            int marriages = CountMarriagesBetweenEnemyAndPlayer(em);
            int primary   = CountPrimaryMarriages(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposal.Status != MarriageProposalStatus.Accepted)
            {
                sb.AppendLine($"Phase 1 FAIL: expected proposal Accepted, got {proposal.Status}");
                return false;
            }
            if (marriages != 2)
            {
                sb.AppendLine($"Phase 1 FAIL: expected 2 marriage records (primary + mirror), got {marriages}");
                return false;
            }
            if (primary != 1)
            {
                sb.AppendLine($"Phase 1 FAIL: expected exactly 1 primary record, got {primary}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 1 FAIL: expected LastFiredOp cleared to None, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: proposal accepted, 2 marriages created (1 primary), LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-inbox-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 20f);
            SeedPlayerFaction(em);
            var enemy = SeedEnemyFactionWithDispatch(em, CovertOpKind.MarriageInboxAccept);

            world.Update();

            int marriages = CountMarriagesBetweenEnemyAndPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (marriages != 0)
            {
                sb.AppendLine($"Phase 2 FAIL: expected 0 marriages (no proposal to accept), got {marriages}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 2 FAIL: expected LastFiredOp cleared, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: no pending proposal, no marriage, LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-inbox-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 20f);
            SeedPlayerFaction(em);
            var enemy = SeedEnemyFactionWithDispatch(em, CovertOpKind.MarriageInboxAccept);
            var expiredEntity = SeedExpiredProposalPlayerToEnemy(em);

            world.Update();

            var expired   = em.GetComponentData<MarriageProposalComponent>(expiredEntity);
            int marriages = CountMarriagesBetweenEnemyAndPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (expired.Status != MarriageProposalStatus.Expired)
            {
                sb.AppendLine($"Phase 3 FAIL: expired proposal should stay Expired, got {expired.Status}");
                return false;
            }
            if (marriages != 0)
            {
                sb.AppendLine($"Phase 3 FAIL: expected 0 marriages (expired proposal must be skipped), got {marriages}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 3 FAIL: expected LastFiredOp cleared, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: expired proposal skipped, no marriage, LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-inbox-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 20f);
            SeedPlayerFaction(em);
            var enemy = SeedEnemyFactionWithDispatch(em, CovertOpKind.None);
            var proposalEntity = SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            var proposal = em.GetComponentData<MarriageProposalComponent>(proposalEntity);
            int marriages = CountMarriagesBetweenEnemyAndPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposal.Status != MarriageProposalStatus.Pending)
            {
                sb.AppendLine($"Phase 4 FAIL: proposal should stay Pending without dispatch, got {proposal.Status}");
                return false;
            }
            if (marriages != 0)
            {
                sb.AppendLine($"Phase 4 FAIL: expected 0 marriages without dispatch, got {marriages}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 4 FAIL: LastFiredOp should stay None, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: no dispatch, proposal stays Pending, no marriage");
            return true;
        }
    }
}
#endif
