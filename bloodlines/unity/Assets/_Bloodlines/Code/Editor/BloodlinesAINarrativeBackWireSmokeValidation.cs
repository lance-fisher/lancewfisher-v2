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
    /// Smoke validator for sub-slice 17 narrative back-wire work.
    /// Sub-slice 16 landed the NarrativeMessageBridge producer surface
    /// and wired PactBreakSystem as the first consumer. Sub-slice 17
    /// back-wires three additional AI systems that deferred their
    /// ceremonial pushes in earlier slices:
    ///
    ///   AIMarriageAcceptEffectsSystem (sub-slices 11 + 12): ceremonial
    ///     marriage line with authority-mode suffix.
    ///   AILesserHousePromotionSystem (sub-slice 13): founding line.
    ///   AIPactProposalExecutionSystem (sub-slice 14): non-aggression
    ///     pact entry line.
    ///
    /// Each phase seeds the minimum precondition state for one success
    /// path, runs exactly one update, and verifies:
    ///   (a) the narrative buffer gains exactly one message,
    ///   (b) the message text contains the expected substring(s),
    ///   (c) tone matches the source/target tone rule (player source
    ///       promotes to Good for marriage accept and pact proposal;
    ///       player faction promotes to Good for lesser-house; else Info).
    ///
    /// Browser reference: simulation.js pushMessage call sites:
    ///   - acceptMarriage (~7463)
    ///   - promoteMemberToLesserHouse (~7251)
    ///   - proposeNonAggressionPact (~5216-5220)
    /// Existing sub-slice 11/13/14/15 smoke validators continue to pass
    /// because the wire-ups are purely additive; that regression is
    /// verified by running those smokes separately in the 10-gate chain.
    /// Artifact: artifacts/unity-ai-narrative-back-wire-smoke.log.
    /// </summary>
    public static class BloodlinesAINarrativeBackWireSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-narrative-back-wire-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Narrative Back-Wire Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAINarrativeBackWireSmokeValidation() =>
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
                message = "AI narrative back-wire smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_NARRATIVE_BACK_WIRE_SMOKE " +
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
            ok &= RunPhaseMarriageAcceptPlayerHeadDirect(sb);
            ok &= RunPhaseMarriageAcceptEnemyHeirRegency(sb);
            ok &= RunPhaseMarriageAcceptEnemyEnvoyRegency(sb);
            ok &= RunPhaseLesserHousePromotionPlayer(sb);
            ok &= RunPhaseLesserHousePromotionEnemy(sb);
            ok &= RunPhasePactProposalEnemy(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static void SetupMarriageEffectsGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageAcceptEffectsSystem>());
            sg.SortSystems();
        }

        private static void SetupLesserHouseGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AILesserHousePromotionSystem>());
            sg.SortSystems();
        }

        private static void SetupPactProposalGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIPactProposalExecutionSystem>());
            sg.SortSystems();
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

        private static Entity SeedFaction(
            EntityManager em,
            string factionId,
            float legitimacy)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = legitimacy });
            return e;
        }

        private static Entity SeedMarriageRecord(
            EntityManager em,
            string headFactionId,
            string headMemberId,
            string spouseFactionId,
            string spouseMemberId,
            MarriageAuthorityMode authorityMode,
            float legitimacyCost)
        {
            var e = em.CreateEntity(
                typeof(MarriageComponent),
                typeof(MarriageAcceptEffectsPendingTag),
                typeof(MarriageAcceptanceTermsComponent));
            em.SetComponentData(e, new MarriageComponent
            {
                MarriageId                 = new FixedString64Bytes("m-" + headMemberId),
                HeadFactionId              = new FixedString32Bytes(headFactionId),
                HeadMemberId               = new FixedString64Bytes(headMemberId),
                SpouseFactionId            = new FixedString32Bytes(spouseFactionId),
                SpouseMemberId             = new FixedString64Bytes(spouseMemberId),
                MarriedAtInWorldDays       = 25f,
                ExpectedChildAtInWorldDays = 85f,
                IsPrimary                  = true,
                ChildGenerated             = false,
                Dissolved                  = false,
            });
            em.SetComponentData(e, new MarriageAcceptanceTermsComponent
            {
                AuthorityMode  = authorityMode,
                LegitimacyCost = legitimacyCost,
            });
            return e;
        }

        private static Entity SeedFactionWithLesserHouseRoster(
            EntityManager em, string factionId)
        {
            var factionEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(LesserHouseElement));
            em.SetComponentData(factionEntity, new FactionComponent { FactionId = factionId });
            em.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(factionEntity, new AICovertOpsComponent
            {
                LastFiredOp     = CovertOpKind.LesserHousePromotion,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(factionEntity, new DynastyStateComponent { Legitimacy = 60f });
            em.SetComponentData(factionEntity, new ConvictionComponent { Stewardship = 0f });

            // Seed one eligible Governance candidate with Renown 40.
            var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
            em.SetComponentData(memberEntity, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes(factionId + "-governor"),
                Title    = new FixedString64Bytes("Governor"),
                Role     = DynastyRole.Governor,
                Path     = DynastyPath.Governance,
                Status   = DynastyMemberStatus.Active,
                Renown   = 40f,
            });
            var memberBuffer = em.AddBuffer<DynastyMemberRef>(factionEntity);
            memberBuffer.Add(new DynastyMemberRef { Member = memberEntity });
            return factionEntity;
        }

        private static Entity SeedPactSource(
            EntityManager em,
            string factionId,
            string targetFactionId)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(ResourceStockpileComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new AICovertOpsComponent
            {
                LastFiredOp     = CovertOpKind.PactProposal,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(e, new ResourceStockpileComponent
            {
                Gold      = 200f,
                Influence = 100f,
            });
            var buf = em.GetBuffer<HostilityComponent>(e);
            buf.Add(new HostilityComponent { HostileFactionId = new FixedString32Bytes(targetFactionId) });
            return e;
        }

        private static Entity SeedPactTarget(
            EntityManager em,
            string factionId,
            string hostileTo)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            var buf = em.GetBuffer<HostilityComponent>(e);
            buf.Add(new HostilityComponent { HostileFactionId = new FixedString32Bytes(hostileTo) });
            return e;
        }

        private static bool TryGetSingletonBuffer(
            EntityManager em, out DynamicBuffer<NarrativeMessageElement> buffer)
        {
            buffer = default;
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<NarrativeMessageSingleton>());
            if (q.IsEmpty) { q.Dispose(); return false; }
            var singleton = q.GetSingletonEntity();
            q.Dispose();
            buffer = em.GetBuffer<NarrativeMessageElement>(singleton);
            return true;
        }

        private static bool MessageContains(FixedString128Bytes text, string substring)
        {
            return text.ToString().Contains(substring);
        }

        // ------------------------------------------------------------------ marriage accept (head-direct, player)

        private static bool RunPhaseMarriageAcceptPlayerHeadDirect(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-marriage-head-direct");
            var em = world.EntityManager;
            SetupMarriageEffectsGroup(world);
            SeedDualClock(em);
            SeedFaction(em, "player", 70f);
            SeedFaction(em, "enemy",  65f);
            SeedMarriageRecord(em,
                headFactionId: "player", headMemberId: "player-heir",
                spouseFactionId: "enemy", spouseMemberId: "enemy-heir",
                MarriageAuthorityMode.HeadDirect, legitimacyCost: 0f);

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "weds"))
            {
                sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect FAIL: message missing 'weds'; got '{entry.Text}'");
                return false;
            }
            if (!MessageContains(entry.Text, "under head approval"))
            {
                sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect FAIL: message missing 'under head approval'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Good)
            {
                sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect FAIL: player-source tone should be Good, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhaseMarriageAcceptPlayerHeadDirect PASS: '{entry.Text}' tone=Good");
            return true;
        }

        // ------------------------------------------------------------------ marriage accept (heir regency, enemy)

        private static bool RunPhaseMarriageAcceptEnemyHeirRegency(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-marriage-heir-regency");
            var em = world.EntityManager;
            SetupMarriageEffectsGroup(world);
            SeedDualClock(em);
            SeedFaction(em, "enemy",  65f);
            SeedFaction(em, "player", 70f);
            SeedMarriageRecord(em,
                headFactionId: "enemy", headMemberId: "enemy-heir",
                spouseFactionId: "player", spouseMemberId: "player-heir",
                MarriageAuthorityMode.HeirRegency, legitimacyCost: 1f);

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "heir regency"))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency FAIL: message missing 'heir regency'; got '{entry.Text}'");
                return false;
            }
            if (!MessageContains(entry.Text, "legitimacy -1"))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency FAIL: message missing 'legitimacy -1'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency FAIL: non-player source tone should be Info, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhaseMarriageAcceptEnemyHeirRegency PASS: '{entry.Text}' tone=Info");
            return true;
        }

        // ------------------------------------------------------------------ marriage accept (envoy regency, enemy)

        private static bool RunPhaseMarriageAcceptEnemyEnvoyRegency(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-marriage-envoy-regency");
            var em = world.EntityManager;
            SetupMarriageEffectsGroup(world);
            SeedDualClock(em);
            SeedFaction(em, "enemy",  60f);
            SeedFaction(em, "player", 70f);
            SeedMarriageRecord(em,
                headFactionId: "enemy", headMemberId: "enemy-heir",
                spouseFactionId: "player", spouseMemberId: "player-heir",
                MarriageAuthorityMode.EnvoyRegency, legitimacyCost: 2f);

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "envoy regency"))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency FAIL: message missing 'envoy regency'; got '{entry.Text}'");
                return false;
            }
            if (!MessageContains(entry.Text, "legitimacy -2"))
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency FAIL: message missing 'legitimacy -2'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency FAIL: non-player source tone should be Info, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhaseMarriageAcceptEnemyEnvoyRegency PASS: '{entry.Text}' tone=Info");
            return true;
        }

        // ------------------------------------------------------------------ lesser house (player)

        private static bool RunPhaseLesserHousePromotionPlayer(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-lesser-house-player");
            var em = world.EntityManager;
            SetupLesserHouseGroup(world);
            SeedDualClock(em);
            SeedFactionWithLesserHouseRoster(em, "player");

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseLesserHousePromotionPlayer FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseLesserHousePromotionPlayer FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "founds"))
            {
                sb.AppendLine($"PhaseLesserHousePromotionPlayer FAIL: message missing 'founds'; got '{entry.Text}'");
                return false;
            }
            if (!MessageContains(entry.Text, "honoring"))
            {
                sb.AppendLine($"PhaseLesserHousePromotionPlayer FAIL: message missing 'honoring'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Good)
            {
                sb.AppendLine($"PhaseLesserHousePromotionPlayer FAIL: player faction tone should be Good, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhaseLesserHousePromotionPlayer PASS: '{entry.Text}' tone=Good");
            return true;
        }

        // ------------------------------------------------------------------ lesser house (enemy)

        private static bool RunPhaseLesserHousePromotionEnemy(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-lesser-house-enemy");
            var em = world.EntityManager;
            SetupLesserHouseGroup(world);
            SeedDualClock(em);
            SeedFactionWithLesserHouseRoster(em, "enemy");

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseLesserHousePromotionEnemy FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseLesserHousePromotionEnemy FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "founds"))
            {
                sb.AppendLine($"PhaseLesserHousePromotionEnemy FAIL: message missing 'founds'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhaseLesserHousePromotionEnemy FAIL: non-player faction tone should be Info, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhaseLesserHousePromotionEnemy PASS: '{entry.Text}' tone=Info");
            return true;
        }

        // ------------------------------------------------------------------ pact proposal (enemy)
        // AIPactProposalExecutionSystem hardcodes the target to "player"
        // (matching the browser ai.js dispatch at ~2643-2666). There is no
        // reachable player-source path; the Good-tone branch in the system
        // is defensive for future extension. Only the enemy-source path
        // produces a push in the current system, so this is the only
        // pact-proposal phase.

        private static bool RunPhasePactProposalEnemy(System.Text.StringBuilder sb)
        {
            using var world = new World("back-wire-pact-enemy");
            var em = world.EntityManager;
            SetupPactProposalGroup(world);
            SeedDualClock(em);
            // Target is hardcoded to "player" in the system dispatch.
            SeedPactSource(em, "enemy", "player");
            SeedPactTarget(em, "player", hostileTo: "enemy");

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhasePactProposalEnemy FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhasePactProposalEnemy FAIL: expected exactly 1 message, got {buffer.Length}");
                return false;
            }
            var entry = buffer[0];
            if (!MessageContains(entry.Text, "enter a non-aggression pact"))
            {
                sb.AppendLine($"PhasePactProposalEnemy FAIL: message missing 'enter a non-aggression pact'; got '{entry.Text}'");
                return false;
            }
            if (entry.Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhasePactProposalEnemy FAIL: non-player source tone should be Info, got {entry.Tone}");
                return false;
            }
            sb.AppendLine($"PhasePactProposalEnemy PASS: '{entry.Text}' tone=Info");
            return true;
        }
    }
}
#endif
