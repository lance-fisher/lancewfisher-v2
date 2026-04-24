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
    /// Smoke validator for sub-slice 29: AI captive ransom resolution effects.
    /// Covers AICaptiveRansomResolutionSystem resolution, void-captive,
    /// and not-yet-resolved paths.
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations ransom branch (~5838-5858):
    ///     grantResources to captor; releaseCapturedMember; legitimacy
    ///     recovery (deferred); oathkeeping+1 on source; stewardship+1
    ///     on captor; narrative push.
    ///
    /// Phases:
    ///   PhaseRansomResolution: captive held, inWorldDays past ResolveAt ->
    ///     op finalized (Active=false), captor gets escrow resources back,
    ///     captive Released, oathkeeping+1 on source, stewardship+1 on
    ///     captor, ransom narrative pushed.
    ///   PhaseVoidCaptiveGone: no captive in captor buffer -> op finalized
    ///     (Active=false), captor still gets resources, conviction fires,
    ///     no narrative.
    ///   PhaseNotYetResolved: ResolveAt still in future -> op stays
    ///     Active=true, captor resources unchanged.
    ///
    /// Artifact: artifacts/unity-captive-ransom-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesCaptiveRansomResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-captive-ransom-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Captive Ransom Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCaptiveRansomResolutionSmokeValidation() =>
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
                message = "Captive ransom resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_CAPTIVE_RANSOM_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseRansomResolution(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICaptiveRansomResolutionSystem>());
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

        private static Entity CreateFaction(EntityManager em, string factionId, float gold = 0f, float influence = 0f)
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
                Gold      = gold,
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
                RansomCost            = 70f,
                Status                = CapturedMemberStatus.Held,
            });
        }

        private static Entity CreateRansomOp(
            EntityManager em,
            string sourceFactionId,
            string captorFactionId,
            string captiveMemberId,
            string captiveMemberTitle,
            float resolveAtInWorldDays,
            float escrowGold,
            float escrowInfluence)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationCaptiveRansomComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-ransom-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(captorFactionId),
                OperationKind        = DynastyOperationKind.CaptiveRansom,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationCaptiveRansomComponent
            {
                ResolveAtInWorldDays = resolveAtInWorldDays,
                CaptiveMemberId      = new FixedString64Bytes(captiveMemberId),
                CaptiveMemberTitle   = new FixedString64Bytes(captiveMemberTitle),
                CaptorFactionId      = new FixedString32Bytes(captorFactionId),
                ProjectedChance      = 1f,
                EscrowCost           = new DynastyOperationEscrowCost
                {
                    Gold      = escrowGold,
                    Influence = escrowInfluence,
                },
            });
            return opEntity;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseRansomResolution(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-ransom-resolution");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player", gold: 100f, influence: 50f);
            AddCaptive(em, captorEntity, "member-101", "Baron Edric");

            var opEntity = CreateRansomOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-101",
                captiveMemberTitle: "Baron Edric",
                resolveAtInWorldDays: 30f,
                escrowGold: 70f,
                escrowInfluence: 18f);

            float captorGoldBefore     = em.GetComponentData<ResourceStockpileComponent>(captorEntity).Gold;
            float captorInfluenceBefore = em.GetComponentData<ResourceStockpileComponent>(captorEntity).Influence;
            int narrativeBefore = CountNarrativeMessages(em);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var stock = em.GetComponentData<ResourceStockpileComponent>(captorEntity);
            bool captorGotGold     = stock.Gold     >= captorGoldBefore + 70f;
            bool captorGotInfluence = stock.Influence >= captorInfluenceBefore + 18f;

            var buf = em.GetBuffer<CapturedMemberElement>(captorEntity);
            bool captiveReleased = buf.Length > 0 && buf[0].Status == CapturedMemberStatus.Released;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool oathkeepingOnSource = sourceConv.Oathkeeping >= 1f;

            var captorConv = em.GetComponentData<ConvictionComponent>(captorEntity);
            bool stewardshipOnCaptor = captorConv.Stewardship >= 1f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && captorGotGold && captorGotInfluence
                     && captiveReleased && oathkeepingOnSource
                     && stewardshipOnCaptor && narrativePushed;
            sb.AppendLine($"[PhaseRansomResolution] opFinalized={opFinalized} captorGotGold={captorGotGold} " +
                          $"captorGotInfluence={captorGotInfluence} captiveReleased={captiveReleased} " +
                          $"oathkeepingOnSource={oathkeepingOnSource} stewardshipOnCaptor={stewardshipOnCaptor} " +
                          $"narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseVoidCaptiveGone(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-ransom-void");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player", gold: 100f, influence: 50f);
            // No captive seeded on captor buffer.

            var opEntity = CreateRansomOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-999",
                captiveMemberTitle: "Ghost Member",
                resolveAtInWorldDays: 30f,
                escrowGold: 70f,
                escrowInfluence: 18f);

            float captorGoldBefore = em.GetComponentData<ResourceStockpileComponent>(captorEntity).Gold;
            int narrativeBefore = CountNarrativeMessages(em);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            // Captor still gets resources even when captive is gone.
            var stock = em.GetComponentData<ResourceStockpileComponent>(captorEntity);
            bool captorGotGold = stock.Gold >= captorGoldBefore + 70f;

            // Conviction should still fire (escrow already paid).
            var captorConv = em.GetComponentData<ConvictionComponent>(captorEntity);
            bool stewardshipOnCaptor = captorConv.Stewardship >= 1f;

            // Narrative should NOT be pushed when captive not found.
            int narrativeAfter = CountNarrativeMessages(em);
            bool noNarrative = narrativeAfter == narrativeBefore;

            bool pass = opFinalized && captorGotGold && stewardshipOnCaptor && noNarrative;
            sb.AppendLine($"[PhaseVoidCaptiveGone] opFinalized={opFinalized} captorGotGold={captorGotGold} " +
                          $"stewardshipOnCaptor={stewardshipOnCaptor} noNarrative={noNarrative} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-ransom-not-yet");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 10f);

            CreateFaction(em, "enemy");
            var captorEntity = CreateFaction(em, "player", gold: 100f, influence: 50f);
            AddCaptive(em, captorEntity, "member-102", "Count Beld");

            // ResolveAt=30f, inWorldDays=10f -> should not resolve yet.
            var opEntity = CreateRansomOp(em,
                sourceFactionId: "enemy",
                captorFactionId: "player",
                captiveMemberId: "member-102",
                captiveMemberTitle: "Count Beld",
                resolveAtInWorldDays: 30f,
                escrowGold: 70f,
                escrowInfluence: 18f);

            float captorGoldBefore = em.GetComponentData<ResourceStockpileComponent>(captorEntity).Gold;

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opStillActive = op.Active;

            float captorGoldAfter = em.GetComponentData<ResourceStockpileComponent>(captorEntity).Gold;
            bool captorResourcesUnchanged = System.Math.Abs(captorGoldAfter - captorGoldBefore) < 0.01f;

            var buf = em.GetBuffer<CapturedMemberElement>(captorEntity);
            bool captiveStillHeld = buf.Length > 0 && buf[0].Status == CapturedMemberStatus.Held;

            bool pass = opStillActive && captorResourcesUnchanged && captiveStillHeld;
            sb.AppendLine($"[PhaseNotYetResolved] opStillActive={opStillActive} " +
                          $"captorResourcesUnchanged={captorResourcesUnchanged} captiveStillHeld={captiveStillHeld} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
