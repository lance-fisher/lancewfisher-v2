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
    /// Smoke validator for AIMarriageProposalExecutionSystem. Verifies five
    /// phases:
    ///
    ///   Phase 1: LastFiredOp=MarriageProposal + candidates available + no
    ///            existing marriage/proposal -> proposal entity created;
    ///            LastFiredOp cleared to None.
    ///   Phase 2: LastFiredOp=MarriageProposal + active marriage between enemy
    ///            and player exists -> no proposal created; LastFiredOp cleared.
    ///   Phase 3: LastFiredOp=MarriageProposal + pending proposal from enemy to
    ///            player already exists -> no second proposal created; existing
    ///            proposal preserved; LastFiredOp cleared.
    ///   Phase 4: LastFiredOp=MarriageProposal + target faction missing dynasty
    ///            members -> no proposal created; LastFiredOp cleared.
    ///   Phase 5: LastFiredOp=None -> no-op path; no proposal created;
    ///            LastFiredOp stays None. Proves the system does not fire
    ///            without a dispatch signal.
    ///
    /// Browser reference: ai.js tryAiMarriageProposal (~2897-2944).
    /// Artifact: artifacts/unity-ai-marriage-proposal-execution-smoke.log.
    /// </summary>
    public static class BloodlinesAIMarriageProposalExecutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-marriage-proposal-execution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Marriage Proposal Execution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIMarriageProposalExecutionSmokeValidation() =>
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
                message = "AI marriage proposal execution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_MARRIAGE_PROPOSAL_EXECUTION_SMOKE " +
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
            ok &= RunPhase5(sb);
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
            sg.SortSystems();
            return sg;
        }

        private static void SeedMatchProgression(EntityManager em, int stageNumber = 3)
        {
            var e = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(e, new MatchProgressionComponent { StageNumber = stageNumber });
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays = 10f)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity SeedPlayerFactionWithDynasty(EntityManager em, bool withMembers = true)
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

            if (withMembers)
                DynastyBootstrap.AttachDynasty(em, e, new FixedString32Bytes("player"));

            return e;
        }

        private static Entity SeedEnemyFactionWithDispatchAndDynasty(
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

        private static int CountMarriageProposalsFromEnemyToPlayer(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0; }
            var proposals = q.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            q.Dispose();

            var enemyId  = new FixedString32Bytes("enemy");
            var playerId = new FixedString32Bytes("player");
            int count = 0;
            for (int i = 0; i < proposals.Length; i++)
            {
                var p = proposals[i];
                if (p.Status != MarriageProposalStatus.Pending) continue;
                if (!p.SourceFactionId.Equals(enemyId)) continue;
                if (!p.TargetFactionId.Equals(playerId)) continue;
                count++;
            }
            proposals.Dispose();
            return count;
        }

        // ------------------------------------------------------------------ Phase 1
        // LastFiredOp=MarriageProposal + candidates available + no existing marriage/proposal
        // -> proposal entity created; LastFiredOp cleared.

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-exec-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 10f);
            SeedPlayerFactionWithDynasty(em);
            var enemy = SeedEnemyFactionWithDispatchAndDynasty(em, CovertOpKind.MarriageProposal);
            world.Update();

            int proposals = CountMarriageProposalsFromEnemyToPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposals != 1)
            {
                sb.AppendLine($"Phase 1 FAIL: expected 1 new proposal, got {proposals}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 1 FAIL: expected LastFiredOp cleared to None, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: proposal created, LastFiredOp cleared to None");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2
        // Active marriage already exists -> no new proposal; LastFiredOp cleared.

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-exec-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 10f);
            SeedPlayerFactionWithDynasty(em);
            var enemy = SeedEnemyFactionWithDispatchAndDynasty(em, CovertOpKind.MarriageProposal);

            // Seed an active marriage between enemy (head) and player (spouse).
            var marriageEntity = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(marriageEntity, new MarriageComponent
            {
                MarriageId                  = new FixedString64Bytes("prior-marriage"),
                HeadFactionId               = new FixedString32Bytes("enemy"),
                HeadMemberId                = new FixedString64Bytes("enemy-bloodline-heir"),
                SpouseFactionId             = new FixedString32Bytes("player"),
                SpouseMemberId              = new FixedString64Bytes("player-bloodline-heir"),
                MarriedAtInWorldDays        = 5f,
                ExpectedChildAtInWorldDays  = 65f,
                IsPrimary                   = true,
                ChildGenerated              = false,
                Dissolved                   = false,
            });

            world.Update();

            int proposals = CountMarriageProposalsFromEnemyToPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposals != 0)
            {
                sb.AppendLine($"Phase 2 FAIL: expected 0 proposals (blocked by prior marriage), got {proposals}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 2 FAIL: expected LastFiredOp cleared, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: blocked by prior marriage, LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3
        // Pending proposal already exists -> no second proposal; LastFiredOp cleared.

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-exec-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 10f);
            SeedPlayerFactionWithDynasty(em);
            var enemy = SeedEnemyFactionWithDispatchAndDynasty(em, CovertOpKind.MarriageProposal);

            // Seed an existing pending proposal from enemy to player.
            var existingProposal = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(existingProposal, new MarriageProposalComponent
            {
                ProposalId            = new FixedString64Bytes("existing-proposal"),
                SourceFactionId       = new FixedString32Bytes("enemy"),
                SourceMemberId        = new FixedString64Bytes("enemy-bloodline-heir"),
                TargetFactionId       = new FixedString32Bytes("player"),
                TargetMemberId        = new FixedString64Bytes("player-bloodline-heir"),
                Status                = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 5f,
                ExpiresAtInWorldDays  = 35f,
            });

            world.Update();

            int proposals = CountMarriageProposalsFromEnemyToPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposals != 1)
            {
                sb.AppendLine($"Phase 3 FAIL: expected 1 proposal (only the pre-existing one), got {proposals}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 3 FAIL: expected LastFiredOp cleared, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: blocked by pending proposal, no duplicate, LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4
        // Target faction has no dynasty members -> candidate selection fails;
        // no proposal; LastFiredOp cleared.

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-exec-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 10f);
            SeedPlayerFactionWithDynasty(em, withMembers: false); // no dynasty on player
            var enemy = SeedEnemyFactionWithDispatchAndDynasty(em, CovertOpKind.MarriageProposal);
            world.Update();

            int proposals = CountMarriageProposalsFromEnemyToPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposals != 0)
            {
                sb.AppendLine($"Phase 4 FAIL: expected 0 proposals (target has no dynasty), got {proposals}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 4 FAIL: expected LastFiredOp cleared, got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: no target dynasty members, no proposal, LastFiredOp cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 5
        // LastFiredOp=None -> no-op path; no proposal; LastFiredOp stays None.

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("marriage-exec-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedMatchProgression(em);
            SeedDualClock(em, inWorldDays: 10f);
            SeedPlayerFactionWithDynasty(em);
            var enemy = SeedEnemyFactionWithDispatchAndDynasty(em, CovertOpKind.None);
            world.Update();

            int proposals = CountMarriageProposalsFromEnemyToPlayer(em);
            var covert    = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (proposals != 0)
            {
                sb.AppendLine($"Phase 5 FAIL: expected 0 proposals (no dispatch signal), got {proposals}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"Phase 5 FAIL: LastFiredOp was touched when it should have stayed None: {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"Phase 5 PASS: no dispatch signal, no proposal, LastFiredOp stays None");
            return true;
        }
    }
}
#endif
