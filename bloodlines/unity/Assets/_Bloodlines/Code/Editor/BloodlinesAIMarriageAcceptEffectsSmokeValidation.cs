#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
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
    /// Smoke validator for AIMarriageAcceptEffectsSystem. Four phases that
    /// cover the tag-driven one-shot effects pattern:
    ///
    ///   Phase 1: AIMarriageInboxAcceptSystem accepts a pending proposal ->
    ///            primary MarriageComponent created with
    ///            MarriageAcceptEffectsPendingTag -> effects system applies
    ///            legitimacy +2 on both dynasties (clamped to 100), drops
    ///            hostility both ways, removes tag.
    ///   Phase 2: Legitimacy ceiling. Both dynasties start at 99 -> legitimacy
    ///            goes to exactly 100 (not 101).
    ///   Phase 3: Marriage without the tag is ignored. Synthetic marriage
    ///            created directly with IsPrimary=true but no tag ->
    ///            legitimacy unchanged; hostility unchanged.
    ///   Phase 4: Full pipeline. End-to-end from covert-ops dispatch
    ///            (LastFiredOp=MarriageInboxAccept) through inbox accept
    ///            through effects. One update applies everything.
    ///
    /// Browser reference: simulation.js acceptMarriage (~7388-7469), block
    /// starting at the legitimacy +2 increment.
    /// Artifact: artifacts/unity-ai-marriage-accept-effects-smoke.log.
    /// </summary>
    public static class BloodlinesAIMarriageAcceptEffectsSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-marriage-accept-effects-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Marriage Accept Effects Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIMarriageAcceptEffectsSmokeValidation() =>
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
                message = "AI marriage accept effects smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_MARRIAGE_ACCEPT_EFFECTS_SMOKE " +
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
            ok &= RunPhase6(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageInboxAcceptSystem>());
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageAcceptEffectsSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays = 25f)
        {
            var e = em.CreateEntity(
                typeof(DualClockComponent),
                typeof(DeclareInWorldTimeRequest));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static int CountPendingTimeDeclarations(EntityManager em, float expectedDays)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0; }
            var clockEntity = q.GetSingletonEntity();
            q.Dispose();

            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity)) return 0;
            var buffer = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);
            int matches = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].DaysDelta >= expectedDays - 0.01f
                    && buffer[i].DaysDelta <= expectedDays + 0.01f)
                    matches++;
            }
            return matches;
        }

        private static Entity SeedFactionWithDynastyAndHostility(
            EntityManager em,
            FixedString32Bytes factionId,
            float startingLegitimacy,
            FixedString32Bytes? hostileTo = null)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = startingLegitimacy });

            if (hostileTo.HasValue)
            {
                var buffer = em.GetBuffer<HostilityComponent>(e);
                buffer.Add(new HostilityComponent { HostileFactionId = hostileTo.Value });
            }
            return e;
        }

        private static Entity SeedEnemyWithCovertOpsDispatch(
            EntityManager em,
            CovertOpKind dispatched,
            float startingLegitimacy,
            FixedString32Bytes? hostileTo = null)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(DynastyStateComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = startingLegitimacy });

            if (hostileTo.HasValue)
            {
                var buffer = em.GetBuffer<HostilityComponent>(e);
                buffer.Add(new HostilityComponent { HostileFactionId = hostileTo.Value });
            }
            return e;
        }

        private static Entity SeedPendingProposalPlayerToEnemy(EntityManager em)
        {
            var proposalEntity = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId            = new FixedString64Bytes("effects-pending"),
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

        private static float GetLegitimacy(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            float result = -1f;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    result = em.GetComponentData<DynastyStateComponent>(entities[i]).Legitimacy;
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return result;
        }

        private static bool IsHostile(EntityManager em,
            FixedString32Bytes sourceFactionId, FixedString32Bytes targetFactionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity sourceEntity = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(sourceFactionId))
                {
                    sourceEntity = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();

            if (sourceEntity == Entity.Null) return false;
            if (!em.HasBuffer<HostilityComponent>(sourceEntity)) return false;

            var buffer = em.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].HostileFactionId.Equals(targetFactionId)) return true;
            }
            return false;
        }

        private static int CountPrimaryPendingTags(EntityManager em)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<MarriageComponent>(),
                ComponentType.ReadOnly<MarriageAcceptEffectsPendingTag>());
            int count = q.CalculateEntityCount();
            q.Dispose();
            return count;
        }

        // ------------------------------------------------------------------ Phase 1

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedFactionWithDynastyAndHostility(em,
                new FixedString32Bytes("player"), 80f,
                hostileTo: new FixedString32Bytes("enemy"));
            SeedEnemyWithCovertOpsDispatch(em, CovertOpKind.MarriageInboxAccept, 75f,
                hostileTo: new FixedString32Bytes("player"));
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            int tags = CountPrimaryPendingTags(em);
            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            bool playerHostile = IsHostile(em, new FixedString32Bytes("player"), new FixedString32Bytes("enemy"));
            bool enemyHostile  = IsHostile(em, new FixedString32Bytes("enemy"),  new FixedString32Bytes("player"));

            if (tags != 0)
            {
                sb.AppendLine($"Phase 1 FAIL: expected 0 pending tags after effects, got {tags}");
                return false;
            }
            if (playerLegitimacy < 81.99f || playerLegitimacy > 82.01f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected player legitimacy 82, got {playerLegitimacy}");
                return false;
            }
            if (enemyLegitimacy < 76.99f || enemyLegitimacy > 77.01f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected enemy legitimacy 77, got {enemyLegitimacy}");
                return false;
            }
            if (playerHostile || enemyHostile)
            {
                sb.AppendLine($"Phase 1 FAIL: expected hostility dropped both ways; playerHostile={playerHostile}, enemyHostile={enemyHostile}");
                return false;
            }
            sb.AppendLine($"Phase 1 PASS: legitimacy +2 both sides (player={playerLegitimacy}, enemy={enemyLegitimacy}), hostility dropped, tag removed");
            return true;
        }

        // ------------------------------------------------------------------ Phase 2

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedFactionWithDynastyAndHostility(em, new FixedString32Bytes("player"), 99f);
            SeedEnemyWithCovertOpsDispatch(em, CovertOpKind.MarriageInboxAccept, 99f);
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));

            if (playerLegitimacy < 99.99f || playerLegitimacy > 100.01f)
            {
                sb.AppendLine($"Phase 2 FAIL: expected player legitimacy clamped to 100, got {playerLegitimacy}");
                return false;
            }
            if (enemyLegitimacy < 99.99f || enemyLegitimacy > 100.01f)
            {
                sb.AppendLine($"Phase 2 FAIL: expected enemy legitimacy clamped to 100, got {enemyLegitimacy}");
                return false;
            }
            sb.AppendLine($"Phase 2 PASS: legitimacy clamped to 100 both sides (player={playerLegitimacy}, enemy={enemyLegitimacy})");
            return true;
        }

        // ------------------------------------------------------------------ Phase 3

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedFactionWithDynastyAndHostility(em,
                new FixedString32Bytes("player"), 50f,
                hostileTo: new FixedString32Bytes("enemy"));
            SeedFactionWithDynastyAndHostility(em,
                new FixedString32Bytes("enemy"), 50f,
                hostileTo: new FixedString32Bytes("player"));

            // Create a marriage directly WITHOUT the pending tag. Effects system
            // must not touch it.
            var marriage = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(marriage, new MarriageComponent
            {
                MarriageId      = new FixedString64Bytes("no-tag-marriage"),
                HeadFactionId   = new FixedString32Bytes("player"),
                HeadMemberId    = new FixedString64Bytes("player-bloodline-heir"),
                SpouseFactionId = new FixedString32Bytes("enemy"),
                SpouseMemberId  = new FixedString64Bytes("enemy-bloodline-heir"),
                IsPrimary       = true,
            });

            world.Update();

            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            bool playerHostile = IsHostile(em, new FixedString32Bytes("player"), new FixedString32Bytes("enemy"));
            bool enemyHostile  = IsHostile(em, new FixedString32Bytes("enemy"),  new FixedString32Bytes("player"));

            if (playerLegitimacy < 49.99f || playerLegitimacy > 50.01f ||
                enemyLegitimacy  < 49.99f || enemyLegitimacy  > 50.01f)
            {
                sb.AppendLine($"Phase 3 FAIL: legitimacy must not change without tag; got player={playerLegitimacy}, enemy={enemyLegitimacy}");
                return false;
            }
            if (!playerHostile || !enemyHostile)
            {
                sb.AppendLine($"Phase 3 FAIL: hostility must not drop without tag; playerHostile={playerHostile}, enemyHostile={enemyHostile}");
                return false;
            }
            sb.AppendLine($"Phase 3 PASS: untagged marriage ignored, state unchanged");
            return true;
        }

        // ------------------------------------------------------------------ Phase 4

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedFactionWithDynastyAndHostility(em,
                new FixedString32Bytes("player"), 70f,
                hostileTo: new FixedString32Bytes("enemy"));
            SeedEnemyWithCovertOpsDispatch(em, CovertOpKind.MarriageInboxAccept, 65f,
                hostileTo: new FixedString32Bytes("player"));
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            // Full pipeline: proposal accepted (sub-slice 9) + effects applied
            // (this slice) in one update.
            int tags = CountPrimaryPendingTags(em);
            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            bool playerHostile = IsHostile(em, new FixedString32Bytes("player"), new FixedString32Bytes("enemy"));
            bool enemyHostile  = IsHostile(em, new FixedString32Bytes("enemy"),  new FixedString32Bytes("player"));

            if (tags != 0 || playerHostile || enemyHostile
                || playerLegitimacy < 71.99f || playerLegitimacy > 72.01f
                || enemyLegitimacy  < 66.99f || enemyLegitimacy  > 67.01f)
            {
                sb.AppendLine($"Phase 4 FAIL: end-to-end pipeline should leave legitimacy=72/67, no hostility, no tag; got player={playerLegitimacy}, enemy={enemyLegitimacy}, tags={tags}, playerHostile={playerHostile}, enemyHostile={enemyHostile}");
                return false;
            }
            sb.AppendLine($"Phase 4 PASS: full pipeline leaves legitimacy=72/67, hostility dropped, tag cleared");
            return true;
        }

        // ------------------------------------------------------------------ Phase 5
        // Verifies the 30-day DeclareInWorldTimeRequest is enqueued on the DualClock
        // singleton. Browser declareInWorldTime(state, 30, "Marriage ...").

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedFactionWithDynastyAndHostility(em, new FixedString32Bytes("player"), 60f);
            SeedEnemyWithCovertOpsDispatch(em, CovertOpKind.MarriageInboxAccept, 60f);
            SeedPendingProposalPlayerToEnemy(em);

            int pendingBefore = CountPendingTimeDeclarations(em, 30f);
            world.Update();
            int pendingAfter  = CountPendingTimeDeclarations(em, 30f);

            if (pendingBefore != 0)
            {
                sb.AppendLine($"Phase 5 FAIL: expected 0 pending 30-day declarations before update, got {pendingBefore}");
                return false;
            }
            if (pendingAfter != 1)
            {
                sb.AppendLine($"Phase 5 FAIL: expected exactly 1 pending 30-day declaration after update, got {pendingAfter}");
                return false;
            }
            sb.AppendLine($"Phase 5 PASS: 30-day DeclareInWorldTimeRequest enqueued on DualClock singleton");
            return true;
        }

        // ------------------------------------------------------------------ Phase 6
        // Oathkeeping conviction +2 applied on both dynasties. Browser
        // recordConvictionEvent("oathkeeping", 2) on both source and target.

        private static bool RunPhase6(System.Text.StringBuilder sb)
        {
            using var world = new World("accept-effects-phase6");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Player: dynasty + conviction. Start oathkeeping at 10.
            var playerEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new DynastyStateComponent { Legitimacy = 60f });
            em.SetComponentData(playerEntity, new ConvictionComponent { Oathkeeping = 10f });

            // Enemy: dynasty + conviction + covert ops dispatch. Start oathkeeping at 5.
            var enemyEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(enemyEntity, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(enemyEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyEntity, new AICovertOpsComponent
            {
                LastFiredOp = CovertOpKind.MarriageInboxAccept,
            });
            em.SetComponentData(enemyEntity, new DynastyStateComponent { Legitimacy = 60f });
            em.SetComponentData(enemyEntity, new ConvictionComponent { Oathkeeping = 5f });

            SeedPendingProposalPlayerToEnemy(em);
            world.Update();

            var playerConv = em.GetComponentData<ConvictionComponent>(playerEntity);
            var enemyConv  = em.GetComponentData<ConvictionComponent>(enemyEntity);

            if (playerConv.Oathkeeping < 11.99f || playerConv.Oathkeeping > 12.01f)
            {
                sb.AppendLine($"Phase 6 FAIL: expected player oathkeeping 12 (10 + 2), got {playerConv.Oathkeeping}");
                return false;
            }
            if (enemyConv.Oathkeeping < 6.99f || enemyConv.Oathkeeping > 7.01f)
            {
                sb.AppendLine($"Phase 6 FAIL: expected enemy oathkeeping 7 (5 + 2), got {enemyConv.Oathkeeping}");
                return false;
            }
            // Verify score refresh: score = (stewardship + oathkeeping) - (ruthlessness + desecration)
            // player: 0 + 12 - 0 - 0 = 12
            if (playerConv.Score < 11.99f || playerConv.Score > 12.01f)
            {
                sb.AppendLine($"Phase 6 FAIL: expected player score refreshed to 12, got {playerConv.Score}");
                return false;
            }
            sb.AppendLine($"Phase 6 PASS: oathkeeping +2 applied both sides (player={playerConv.Oathkeeping}, enemy={enemyConv.Oathkeeping}), score refreshed");
            return true;
        }
    }
}
#endif
