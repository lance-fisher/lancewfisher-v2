#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 28: AI captive rescue resolution effects.
    /// Covers AICaptiveRescueResolutionSystem success, failure, void-captive,
    /// and not-yet-resolved paths.
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations rescue branch (~5861-5895):
    ///     successScore >= 0 gate; releaseCapturedMember; legitimacy
    ///     recovery (deferred); stewardship+2 conviction on source (success);
    ///     influence refund max(6, round(escrowInfluence*0.2)) to captor +
    ///     stewardship+1 conviction on captor (failure); narrative push.
    ///
    /// Phases:
    ///   PhaseSuccessResolution: SuccessScore >= 0, captive held, inWorldDays
    ///     past ResolveAt -> op finalized (Active=false), captive Released,
    ///     stewardship+2 on source, success narrative pushed.
    ///   PhaseFailureRescue: SuccessScore < 0, inWorldDays past ResolveAt ->
    ///     op finalized (Active=false), captor gets influence refund,
    ///     stewardship+1 on captor, failure narrative pushed.
    ///   PhaseVoidCaptiveGone: SuccessScore >= 0 but no captive in captor
    ///     buffer -> op finalized (Active=false), no conviction, no narrative.
    ///   PhaseNotYetResolved: ResolveAt still in future -> op stays Active=true.
    ///
    /// Artifact: artifacts/unity-captive-rescue-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesCaptiveRescueResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-captive-rescue-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Captive Rescue Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCaptiveRescueResolutionSmokeValidation() =>
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
                message = "Captive rescue resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_CAPTIVE_RESCUE_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseSuccessResolution(sb);
            ok &= RunPhaseFailureRescue(sb);
            ok &= RunPhaseVoidCaptiveGone(sb);
            ok &= RunPhaseNotYetResolved(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupResolutionSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICaptiveRescueResolutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity CreateFaction(EntityManager em, string factionId, float influence = 0f)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(ConvictionComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new ResourceStockpileComponent
            {
                Gold      = 0f,
                Influence = influence,
            });
            em.SetComponentData(e, new ConvictionComponent
            {
                Stewardship  = 0f,
                Oathkeeping  = 0f,
                Ruthlessness = 0f,
                Desecration  = 0f,
                Score        = 0f,
                Band         = ConvictionBand.Neutral,
            });
            return e;
        }

        private static void AddCaptive(EntityManager em, Entity captorEntity, string memberId, string memberTitle)
        {
            if (!em.HasBuffer<CapturedMemberElement>(captorEntity))
                em.AddBuffer<CapturedMemberElement>(captorEntity);
            var buf = em.GetBuffer<CapturedMemberElement>(captorEntity);
            buf.Add(new CapturedMemberElement
            {
                MemberId              = new FixedString64Bytes(memberId),
                MemberTitle           = new FixedString64Bytes(memberTitle),
                OriginFactionId       = new FixedString32Bytes("source"),
                CapturedAtInWorldDays = 0f,
                RansomCost            = 0f,
                Status                = CapturedMemberStatus.Held,
            });
        }

        private static Entity CreateRescueOp(
            EntityManager em,
            string sourceFactionId,
            string captorFactionId,
            string captiveMemberId,
            string captiveMemberTitle,
            float resolveAtInWorldDays,
            float successScore,
            float escrowInfluence)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationCaptiveRescueComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-rescue-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(captorFactionId),
                OperationKind        = DynastyOperationKind.CaptiveRescue,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationCaptiveRescueComponent
            {
                ResolveAtInWorldDays = resolveAtInWorldDays,
                CaptiveMemberId      = new FixedString64Bytes(captiveMemberId),
                CaptiveMemberTitle   = new FixedString64Bytes(captiveMemberTitle),
                CaptorFactionId      = new FixedString32Bytes(captorFactionId),
                SuccessScore         = successScore,
                ProjectedChance      = successScore >= 0f ? 0.8f : 0.2f,
                EscrowCost           = new DynastyOperationEscrowCost
                {
                    Gold      = 42f,
                    Influence = escrowInfluence,
                },
            });
            return opEntity;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessResolution(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-rescue-success");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player");
            AddCaptive(em, captorEntity, "member-001", "Lord Harren");

            var opEntity = CreateRescueOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-001",
                captiveMemberTitle: "Lord Harren",
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 26f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var buf = em.GetBuffer<CapturedMemberElement>(captorEntity);
            bool captiveReleased = buf.Length > 0 && buf[0].Status == CapturedMemberStatus.Released;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool stewardshipOnSource = sourceConv.Stewardship >= 2f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && captiveReleased && stewardshipOnSource && narrativePushed;
            sb.AppendLine($"[PhaseSuccessResolution] opFinalized={opFinalized} captiveReleased={captiveReleased} " +
                          $"stewardshipOnSource={stewardshipOnSource} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureRescue(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-rescue-failure");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player", influence: 100f);
            AddCaptive(em, captorEntity, "member-002", "Lady Maren");

            // SuccessScore < 0 -> failure
            var opEntity = CreateRescueOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-002",
                captiveMemberTitle: "Lady Maren",
                resolveAtInWorldDays: 30f,
                successScore: -10f,
                escrowInfluence: 26f);

            float captorInfluenceBefore =
                em.GetComponentData<ResourceStockpileComponent>(captorEntity).Influence;

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            float captorInfluenceAfter =
                em.GetComponentData<ResourceStockpileComponent>(captorEntity).Influence;
            // refund = max(6, round(26 * 0.2)) = max(6, 5) = 6
            bool captorGotRefund = captorInfluenceAfter > captorInfluenceBefore;

            var captorConv = em.GetComponentData<ConvictionComponent>(captorEntity);
            bool stewardshipOnCaptor = captorConv.Stewardship >= 1f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && captorGotRefund && stewardshipOnCaptor && narrativePushed;
            sb.AppendLine($"[PhaseFailureRescue] opFinalized={opFinalized} captorGotRefund={captorGotRefund} " +
                          $"stewardshipOnCaptor={stewardshipOnCaptor} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseVoidCaptiveGone(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-rescue-void");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player");
            // No captive seeded on captor buffer.

            var opEntity = CreateRescueOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-999",
                captiveMemberTitle: "Missing Member",
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 26f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            // Conviction should not fire (void path skips effects).
            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool noConviction = sourceConv.Stewardship < 1f;

            bool pass = opFinalized && noConviction;
            sb.AppendLine($"[PhaseVoidCaptiveGone] opFinalized={opFinalized} noConviction={noConviction} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-rescue-not-yet");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 10f);

            CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player");
            AddCaptive(em, captorEntity, "member-003", "Ser Aldric");

            // ResolveAt=30f but inWorldDays=10f -> should not resolve.
            var opEntity = CreateRescueOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-003",
                captiveMemberTitle: "Ser Aldric",
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 26f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opStillActive = op.Active;

            var buf = em.GetBuffer<CapturedMemberElement>(captorEntity);
            bool captiveStillHeld = buf.Length > 0 && buf[0].Status == CapturedMemberStatus.Held;

            bool pass = opStillActive && captiveStillHeld;
            sb.AppendLine($"[PhaseNotYetResolved] opStillActive={opStillActive} captiveStillHeld={captiveStillHeld} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
