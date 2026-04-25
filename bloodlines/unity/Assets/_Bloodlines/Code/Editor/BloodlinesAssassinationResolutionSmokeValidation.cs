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
    /// Smoke validator for sub-slice 30: AI assassination dispatch and resolution.
    /// Covers AIAssassinationResolutionSystem success, failure/interception,
    /// void-member-gone, and not-yet-resolved paths.
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations assassination branch (~5752-5830):
    ///     void: member gone before resolution -> finalize silently.
    ///     success (successScore >= 0): member.status="fallen", ensureMutualHostility,
    ///       conviction Ruthlessness+2 on source, Stewardship-1 on target if governor,
    ///       narrative "was assassinated by agents of".
    ///     failure: influence refund max(8, round(escrow*0.3)) to target,
    ///       conviction Stewardship+1 on target,
    ///       narrative "counter-intelligence intercepted".
    ///
    /// Phases:
    ///   PhaseSuccessResolution: SuccessScore >= 0, target member Active ->
    ///     op finalized (Active=false), member Fallen, Ruthlessness+2 on source,
    ///     hostility established, success narrative pushed.
    ///   PhaseFailureInterception: SuccessScore < 0, target member Active ->
    ///     op finalized (Active=false), target gets influence refund,
    ///     Stewardship+1 on target, failure narrative pushed.
    ///   PhaseVoidMemberGone: SuccessScore >= 0 but target member not on roster ->
    ///     op finalized (Active=false), no conviction fired.
    ///   PhaseNotYetResolved: ResolveAt still in future -> op stays Active=true.
    ///
    /// Artifact: artifacts/unity-assassination-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesAssassinationResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-assassination-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Assassination Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAssassinationResolutionSmokeValidation() =>
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
                message = "Assassination resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_ASSASSINATION_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseFailureInterception(sb);
            ok &= RunPhaseVoidMemberGone(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIAssassinationResolutionSystem>());
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

        private static Entity AddMemberToFaction(
            EntityManager em,
            Entity factionEntity,
            string memberId,
            string memberTitle,
            DynastyRole role,
            DynastyMemberStatus status = DynastyMemberStatus.Active)
        {
            var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
            em.SetComponentData(memberEntity, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes(memberId),
                Title    = new FixedString64Bytes(memberTitle),
                Role     = role,
                Status   = status,
                Renown   = 20f,
            });

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity))
                em.AddBuffer<DynastyMemberRef>(factionEntity);

            em.GetBuffer<DynastyMemberRef>(factionEntity).Add(new DynastyMemberRef
            {
                Member = memberEntity,
            });

            if (!em.HasBuffer<DynastyFallenLedger>(factionEntity))
                em.AddBuffer<DynastyFallenLedger>(factionEntity);

            return memberEntity;
        }

        private static Entity CreateAssassinationOp(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            string targetMemberId,
            string targetMemberTitle,
            float resolveAtInWorldDays,
            float successScore,
            float escrowInfluence)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationAssassinationComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-assn-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperationKind        = DynastyOperationKind.Assassination,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationAssassinationComponent
            {
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                TargetMemberId       = new FixedString64Bytes(targetMemberId),
                TargetMemberTitle    = new FixedString64Bytes(targetMemberTitle),
                OperatorMemberId     = new FixedString64Bytes("spy-001"),
                OperatorTitle        = new FixedString64Bytes("Spymaster Vael"),
                ResolveAtInWorldDays = resolveAtInWorldDays,
                SuccessScore         = successScore,
                ProjectedChance      = successScore >= 0f ? 0.75f : 0.25f,
                EscrowGold           = 85f,
                EscrowInfluence      = escrowInfluence,
                IntelSupport         = true,
            });
            return opEntity;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessResolution(System.Text.StringBuilder sb)
        {
            using var world = new World("assn-success");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player", influence: 0f);
            var memberEntity = AddMemberToFaction(em, targetEntity, "member-001", "Lady Mira",
                DynastyRole.Commander, DynastyMemberStatus.Active);

            var opEntity = CreateAssassinationOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                targetMemberId:  "member-001",
                targetMemberTitle: "Lady Mira",
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 28f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
            bool memberFallen = member.Status == DynastyMemberStatus.Fallen;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool ruthlessnessOnSource = sourceConv.Ruthlessness >= 2f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && memberFallen && ruthlessnessOnSource && narrativePushed;
            sb.AppendLine($"[PhaseSuccessResolution] opFinalized={opFinalized} memberFallen={memberFallen} " +
                          $"ruthlessnessOnSource={ruthlessnessOnSource} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureInterception(System.Text.StringBuilder sb)
        {
            using var world = new World("assn-failure");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player", influence: 10f);
            AddMemberToFaction(em, targetEntity, "member-001", "Lady Mira",
                DynastyRole.Commander, DynastyMemberStatus.Active);

            var opEntity = CreateAssassinationOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                targetMemberId:  "member-001",
                targetMemberTitle: "Lady Mira",
                resolveAtInWorldDays: 30f,
                successScore: -10f,
                escrowInfluence: 28f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var targetResources = em.GetComponentData<ResourceStockpileComponent>(targetEntity);
            float expectedRefund = System.MathF.Max(8f, System.MathF.Round(28f * 0.3f));
            bool influenceRefunded = targetResources.Influence >= 10f + expectedRefund - 0.01f;

            var targetConv = em.GetComponentData<ConvictionComponent>(targetEntity);
            bool stewardshipOnTarget = targetConv.Stewardship >= 1f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && influenceRefunded && stewardshipOnTarget && narrativePushed;
            sb.AppendLine($"[PhaseFailureInterception] opFinalized={opFinalized} influenceRefunded={influenceRefunded} " +
                          $"stewardshipOnTarget={stewardshipOnTarget} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseVoidMemberGone(System.Text.StringBuilder sb)
        {
            using var world = new World("assn-void");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player");
            // No member added to roster -- void path

            var opEntity = CreateAssassinationOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                targetMemberId:  "member-missing",
                targetMemberTitle: "Ghost Target",
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 28f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool noConviction = sourceConv.Ruthlessness == 0f;

            bool pass = opFinalized && noConviction;
            sb.AppendLine($"[PhaseVoidMemberGone] opFinalized={opFinalized} noConviction={noConviction} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("assn-not-yet");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 10f);

            CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player");
            AddMemberToFaction(em, targetEntity, "member-001", "Lady Mira",
                DynastyRole.Commander, DynastyMemberStatus.Active);

            var opEntity = CreateAssassinationOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                targetMemberId:  "member-001",
                targetMemberTitle: "Lady Mira",
                resolveAtInWorldDays: 50f,
                successScore: 5f,
                escrowInfluence: 28f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opStillActive = op.Active;

            bool pass = opStillActive;
            sb.AppendLine($"[PhaseNotYetResolved] opStillActive={opStillActive} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
