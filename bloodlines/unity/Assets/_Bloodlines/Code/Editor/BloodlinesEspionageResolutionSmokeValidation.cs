#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.PlayerCovertOps;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 33: AI espionage dispatch and resolution.
    ///
    /// Browser reference: resolveEspionageOperation, simulation.js ~10920-10970.
    ///
    /// Phases:
    ///   PhaseSuccessReportCreated: SuccessScore >= 0, target faction exists ->
    ///     op finalized (Active=false), IntelligenceReportElement on source,
    ///     Stewardship+1 on source, success narrative pushed.
    ///   PhaseVoidTargetGone: target faction missing ->
    ///     op finalized silently, no conviction events.
    ///   PhaseFailureInterception: SuccessScore < 0, target faction exists ->
    ///     op finalized, Stewardship+1 on target, failure narrative pushed.
    ///   PhaseNotYetResolved: ResolveAt still in future ->
    ///     op stays Active=true.
    ///
    /// Artifact: artifacts/unity-espionage-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesEspionageResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-espionage-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Espionage Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchEspionageResolutionSmokeValidation() =>
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
                message = "Espionage resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_ESPIONAGE_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseSuccessReportCreated(sb);
            ok &= RunPhaseVoidTargetGone(sb);
            ok &= RunPhaseFailureInterception(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIEspionageResolutionSystem>());
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

        private static Entity CreateFaction(EntityManager em, string factionId)
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
            em.SetComponentData(e, new ResourceStockpileComponent { Gold = 0f, Influence = 0f });
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

        private static Entity CreateEspionageOp(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            float resolveAtInWorldDays,
            float reportExpiresAtInWorldDays,
            float successScore)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationEspionageComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-esp-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperationKind        = DynastyOperationKind.Espionage,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationEspionageComponent
            {
                SourceFactionId            = new FixedString32Bytes(sourceFactionId),
                TargetFactionId            = new FixedString32Bytes(targetFactionId),
                OperatorMemberId           = new FixedString64Bytes("spy-001"),
                OperatorTitle              = new FixedString64Bytes("Spymaster Vael"),
                ResolveAtInWorldDays       = resolveAtInWorldDays,
                ReportExpiresAtInWorldDays = reportExpiresAtInWorldDays,
                SuccessScore               = successScore,
                ProjectedChance            = successScore >= 0f ? 0.72f : 0.28f,
                EscrowGold                 = 45f,
                EscrowInfluence            = 16f,
            });
            return opEntity;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessReportCreated(System.Text.StringBuilder sb)
        {
            using var world = new World("esp-success-report");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            CreateFaction(em, "player");

            var opEntity = CreateEspionageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                resolveAtInWorldDays: 30f,
                reportExpiresAtInWorldDays: 150f,
                successScore: 8f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            bool hasReport = em.HasBuffer<IntelligenceReportElement>(sourceEntity) &&
                             em.GetBuffer<IntelligenceReportElement>(sourceEntity).Length > 0;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool stewardshipOnSource = sourceConv.Stewardship >= 1f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && hasReport && stewardshipOnSource && narrativePushed;
            sb.AppendLine($"[PhaseSuccessReportCreated] opFinalized={opFinalized} hasReport={hasReport} " +
                          $"stewardshipOnSource={stewardshipOnSource} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseVoidTargetGone(System.Text.StringBuilder sb)
        {
            using var world = new World("esp-void");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            // No target faction entity

            var opEntity = CreateEspionageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "vanished",
                resolveAtInWorldDays: 30f,
                reportExpiresAtInWorldDays: 150f,
                successScore: 5f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            bool noReport = !em.HasBuffer<IntelligenceReportElement>(sourceEntity) ||
                            em.GetBuffer<IntelligenceReportElement>(sourceEntity).Length == 0;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool noConviction = sourceConv.Stewardship == 0f;

            bool pass = opFinalized && noReport && noConviction;
            sb.AppendLine($"[PhaseVoidTargetGone] opFinalized={opFinalized} noReport={noReport} " +
                          $"noConviction={noConviction} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureInterception(System.Text.StringBuilder sb)
        {
            using var world = new World("esp-failure");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player");

            var opEntity = CreateEspionageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                resolveAtInWorldDays: 30f,
                reportExpiresAtInWorldDays: 150f,
                successScore: -12f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var targetConv = em.GetComponentData<ConvictionComponent>(targetEntity);
            bool stewardshipOnTarget = targetConv.Stewardship >= 1f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && stewardshipOnTarget && narrativePushed;
            sb.AppendLine($"[PhaseFailureInterception] opFinalized={opFinalized} stewardshipOnTarget={stewardshipOnTarget} " +
                          $"narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("esp-not-yet");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 10f);

            CreateFaction(em, "enemy");
            CreateFaction(em, "player");

            var opEntity = CreateEspionageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                resolveAtInWorldDays: 50f,
                reportExpiresAtInWorldDays: 170f,
                successScore: 5f);

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
