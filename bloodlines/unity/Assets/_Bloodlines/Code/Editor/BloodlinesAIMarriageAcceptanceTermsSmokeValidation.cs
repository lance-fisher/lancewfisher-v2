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
    /// Smoke validator for the sub-slice 12 marriage acceptance terms layer.
    /// Exercises the four authority paths and the full
    /// MarriageAuthorityEvaluator -> MarriageAcceptanceTermsComponent ->
    /// AIMarriageAcceptEffectsSystem chain:
    ///
    ///   Phase 1 (head-direct): head_of_bloodline Ruling on target. Cost 0.
    ///     Target legitimacy net +2 (identical to sub-slice 11 behavior).
    ///   Phase 2 (heir regency): head fallen, heir_designate present.
    ///     Cost 1. Target legitimacy net +1 (-1 cost, then +2 bonus).
    ///     Target Stewardship -1 (clamped at 0).
    ///   Phase 3 (envoy regency): head fallen, no heir, diplomat present.
    ///     Cost 2. Target legitimacy net 0 (-2 cost, then +2 bonus).
    ///     Target Stewardship -2 (clamped at 0).
    ///   Phase 4 (no authority): head fallen, no heir, no diplomat.
    ///     Evaluator rejects; accept short-circuits. No marriage record
    ///     created; proposal stays pending; dispatch cleared.
    ///   Phase 5 (terms persisted): head-direct accept leaves the
    ///     MarriageAcceptanceTermsComponent on the primary marriage record
    ///     as a durable governance provenance marker. Pending tag removed;
    ///     terms component retained.
    ///
    /// Browser reference:
    ///   simulation.js getMarriageAuthorityProfile (~6134),
    ///   getMarriageAcceptanceTerms (~6327),
    ///   applyMarriageGovernanceLegitimacyCost (~6232),
    ///   acceptMarriage ~7449 (cost before bonus).
    /// Artifact: artifacts/unity-ai-marriage-acceptance-terms-smoke.log.
    /// </summary>
    public static class BloodlinesAIMarriageAcceptanceTermsSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-marriage-acceptance-terms-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Marriage Acceptance Terms Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIMarriageAcceptanceTermsSmokeValidation() =>
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
                message = "AI marriage acceptance terms smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_MARRIAGE_ACCEPTANCE_TERMS_SMOKE " +
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
            ok &= RunPhaseHeadDirect(sb);
            ok &= RunPhaseHeirRegency(sb);
            ok &= RunPhaseEnvoyRegency(sb);
            ok &= RunPhaseNoAuthority(sb);
            ok &= RunPhaseTermsPersisted(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ setup

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

        private static Entity SeedPlayerFaction(EntityManager em, float legitimacy)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = legitimacy });
            em.SetComponentData(e, new ConvictionComponent());
            return e;
        }

        /// <summary>
        /// Seed the AI (target) faction with a dispatched MarriageInboxAccept,
        /// plus a caller-controlled dynasty roster that drives the authority
        /// evaluator outcome. Caller supplies the roles and statuses to test
        /// each authority path.
        /// </summary>
        private static Entity SeedEnemyWithRoster(
            EntityManager em,
            float legitimacy,
            float stewardship,
            (DynastyRole role, DynastyMemberStatus status)[] roster)
        {
            var factionEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(factionEntity, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(factionEntity, new AICovertOpsComponent
            {
                LastFiredOp     = CovertOpKind.MarriageInboxAccept,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(factionEntity, new DynastyStateComponent { Legitimacy = legitimacy });
            em.SetComponentData(factionEntity, new ConvictionComponent { Stewardship = stewardship });

            var memberBuffer = em.AddBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
                em.SetComponentData(memberEntity, new DynastyMemberComponent
                {
                    MemberId = new FixedString64Bytes("enemy-bloodline-" + i),
                    Role     = roster[i].role,
                    Status   = roster[i].status,
                });
                memberBuffer.Add(new DynastyMemberRef { Member = memberEntity });
            }
            return factionEntity;
        }

        private static void SeedPendingProposalPlayerToEnemy(EntityManager em)
        {
            var proposalEntity = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId            = new FixedString64Bytes("terms-pending"),
                SourceFactionId       = new FixedString32Bytes("player"),
                SourceMemberId        = new FixedString64Bytes("player-bloodline-heir"),
                TargetFactionId       = new FixedString32Bytes("enemy"),
                TargetMemberId        = new FixedString64Bytes("enemy-bloodline-0"),
                Status                = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 5f,
                ExpiresAtInWorldDays  = 35f,
            });
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

        private static float GetStewardship(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ConvictionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            float result = -1f;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    result = em.GetComponentData<ConvictionComponent>(entities[i]).Stewardship;
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return result;
        }

        private static int CountPrimaryMarriages(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            var marriages = q.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            q.Dispose();
            int count = 0;
            for (int i = 0; i < marriages.Length; i++)
                if (marriages[i].IsPrimary) count++;
            marriages.Dispose();
            return count;
        }

        private static int CountPendingProposals(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            var proposals = q.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            q.Dispose();
            int count = 0;
            for (int i = 0; i < proposals.Length; i++)
                if (proposals[i].Status == MarriageProposalStatus.Pending) count++;
            proposals.Dispose();
            return count;
        }

        private static bool TryGetPrimaryMarriageEntity(EntityManager em, out Entity entity)
        {
            entity = Entity.Null;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var marriages = q.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            q.Dispose();
            for (int i = 0; i < entities.Length; i++)
            {
                if (marriages[i].IsPrimary)
                {
                    entity = entities[i];
                    break;
                }
            }
            entities.Dispose();
            marriages.Dispose();
            return entity != Entity.Null;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseHeadDirect(System.Text.StringBuilder sb)
        {
            using var world = new World("acceptance-terms-head-direct");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedPlayerFaction(em, 80f);
            SeedEnemyWithRoster(em, 75f, 4f, new[]
            {
                (DynastyRole.HeadOfBloodline, DynastyMemberStatus.Ruling),
                (DynastyRole.HeirDesignate,  DynastyMemberStatus.Active),
                (DynastyRole.Diplomat,        DynastyMemberStatus.Active),
            });
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            float enemyStewardship = GetStewardship(em, new FixedString32Bytes("enemy"));

            if (playerLegitimacy < 81.99f || playerLegitimacy > 82.01f)
            {
                sb.AppendLine($"PhaseHeadDirect FAIL: player legitimacy expected 82, got {playerLegitimacy}");
                return false;
            }
            if (enemyLegitimacy < 76.99f || enemyLegitimacy > 77.01f)
            {
                sb.AppendLine($"PhaseHeadDirect FAIL: enemy legitimacy expected 77 (75 - 0 + 2), got {enemyLegitimacy}");
                return false;
            }
            if (enemyStewardship < 3.99f || enemyStewardship > 4.01f)
            {
                sb.AppendLine($"PhaseHeadDirect FAIL: enemy stewardship should be unchanged at 4, got {enemyStewardship}");
                return false;
            }
            sb.AppendLine($"PhaseHeadDirect PASS: head-direct cost 0, player=82, enemy=77, stewardship unchanged at 4");
            return true;
        }

        private static bool RunPhaseHeirRegency(System.Text.StringBuilder sb)
        {
            using var world = new World("acceptance-terms-heir-regency");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedPlayerFaction(em, 80f);
            SeedEnemyWithRoster(em, 75f, 4f, new[]
            {
                (DynastyRole.HeadOfBloodline, DynastyMemberStatus.Fallen),
                (DynastyRole.HeirDesignate,  DynastyMemberStatus.Active),
                (DynastyRole.Diplomat,        DynastyMemberStatus.Active),
            });
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            float enemyStewardship = GetStewardship(em, new FixedString32Bytes("enemy"));

            if (playerLegitimacy < 81.99f || playerLegitimacy > 82.01f)
            {
                sb.AppendLine($"PhaseHeirRegency FAIL: player legitimacy expected 82 (cost falls only on target), got {playerLegitimacy}");
                return false;
            }
            if (enemyLegitimacy < 75.99f || enemyLegitimacy > 76.01f)
            {
                sb.AppendLine($"PhaseHeirRegency FAIL: enemy legitimacy expected 76 (75 - 1 + 2), got {enemyLegitimacy}");
                return false;
            }
            if (enemyStewardship < 2.99f || enemyStewardship > 3.01f)
            {
                sb.AppendLine($"PhaseHeirRegency FAIL: enemy stewardship expected 3 (4 - 1), got {enemyStewardship}");
                return false;
            }
            sb.AppendLine($"PhaseHeirRegency PASS: heir regency cost 1, enemy legitimacy=76, stewardship=3");
            return true;
        }

        private static bool RunPhaseEnvoyRegency(System.Text.StringBuilder sb)
        {
            using var world = new World("acceptance-terms-envoy-regency");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedPlayerFaction(em, 80f);
            SeedEnemyWithRoster(em, 75f, 4f, new[]
            {
                (DynastyRole.HeadOfBloodline, DynastyMemberStatus.Fallen),
                (DynastyRole.Commander,       DynastyMemberStatus.Active),
                (DynastyRole.Diplomat,        DynastyMemberStatus.Active),
            });
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            float playerLegitimacy = GetLegitimacy(em, new FixedString32Bytes("player"));
            float enemyLegitimacy  = GetLegitimacy(em, new FixedString32Bytes("enemy"));
            float enemyStewardship = GetStewardship(em, new FixedString32Bytes("enemy"));

            if (playerLegitimacy < 81.99f || playerLegitimacy > 82.01f)
            {
                sb.AppendLine($"PhaseEnvoyRegency FAIL: player legitimacy expected 82, got {playerLegitimacy}");
                return false;
            }
            if (enemyLegitimacy < 74.99f || enemyLegitimacy > 75.01f)
            {
                sb.AppendLine($"PhaseEnvoyRegency FAIL: enemy legitimacy expected 75 (75 - 2 + 2), got {enemyLegitimacy}");
                return false;
            }
            if (enemyStewardship < 1.99f || enemyStewardship > 2.01f)
            {
                sb.AppendLine($"PhaseEnvoyRegency FAIL: enemy stewardship expected 2 (4 - 2), got {enemyStewardship}");
                return false;
            }
            sb.AppendLine($"PhaseEnvoyRegency PASS: envoy regency cost 2, enemy legitimacy=75, stewardship=2");
            return true;
        }

        private static bool RunPhaseNoAuthority(System.Text.StringBuilder sb)
        {
            using var world = new World("acceptance-terms-no-authority");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedPlayerFaction(em, 80f);
            var enemy = SeedEnemyWithRoster(em, 75f, 4f, new[]
            {
                (DynastyRole.HeadOfBloodline, DynastyMemberStatus.Fallen),
                (DynastyRole.Commander,       DynastyMemberStatus.Active),
                (DynastyRole.Governor,        DynastyMemberStatus.Active),
            });
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            int primaryCount = CountPrimaryMarriages(em);
            int pendingCount = CountPendingProposals(em);
            var covert = em.GetComponentData<AICovertOpsComponent>(enemy);
            float enemyLegitimacy = GetLegitimacy(em, new FixedString32Bytes("enemy"));

            if (primaryCount != 0)
            {
                sb.AppendLine($"PhaseNoAuthority FAIL: no marriage should be created without authority; primary count {primaryCount}");
                return false;
            }
            if (pendingCount != 1)
            {
                sb.AppendLine($"PhaseNoAuthority FAIL: proposal should stay pending; pending count {pendingCount}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseNoAuthority FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            if (enemyLegitimacy < 74.99f || enemyLegitimacy > 75.01f)
            {
                sb.AppendLine($"PhaseNoAuthority FAIL: enemy legitimacy unchanged at 75, got {enemyLegitimacy}");
                return false;
            }
            sb.AppendLine($"PhaseNoAuthority PASS: accept rejected, proposal kept pending, dispatch cleared, state untouched");
            return true;
        }

        private static bool RunPhaseTermsPersisted(System.Text.StringBuilder sb)
        {
            using var world = new World("acceptance-terms-terms-persisted");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            SeedPlayerFaction(em, 80f);
            SeedEnemyWithRoster(em, 75f, 4f, new[]
            {
                (DynastyRole.HeadOfBloodline, DynastyMemberStatus.Ruling),
            });
            SeedPendingProposalPlayerToEnemy(em);

            world.Update();

            if (!TryGetPrimaryMarriageEntity(em, out Entity primary))
            {
                sb.AppendLine($"PhaseTermsPersisted FAIL: primary marriage not created");
                return false;
            }
            if (em.HasComponent<MarriageAcceptEffectsPendingTag>(primary))
            {
                sb.AppendLine($"PhaseTermsPersisted FAIL: pending tag should be removed after effects pass");
                return false;
            }
            if (!em.HasComponent<MarriageAcceptanceTermsComponent>(primary))
            {
                sb.AppendLine($"PhaseTermsPersisted FAIL: MarriageAcceptanceTermsComponent must persist after effects pass");
                return false;
            }
            var terms = em.GetComponentData<MarriageAcceptanceTermsComponent>(primary);
            if (terms.AuthorityMode != MarriageAuthorityMode.HeadDirect)
            {
                sb.AppendLine($"PhaseTermsPersisted FAIL: expected HeadDirect mode, got {terms.AuthorityMode}");
                return false;
            }
            if (terms.LegitimacyCost != 0f)
            {
                sb.AppendLine($"PhaseTermsPersisted FAIL: expected cost 0 for head-direct, got {terms.LegitimacyCost}");
                return false;
            }
            sb.AppendLine($"PhaseTermsPersisted PASS: terms retained as provenance marker; pending tag removed");
            return true;
        }
    }
}
#endif
