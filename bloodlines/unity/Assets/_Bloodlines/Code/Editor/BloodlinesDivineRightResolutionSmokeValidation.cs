#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 27: AI divine right resolution effects.
    /// Covers AIDivineRightResolutionSystem success and failure paths.
    ///
    /// Browser reference:
    ///   simulation.js tickFaithDivineRightDeclarations (~10747-10782):
    ///     per-tick failure checks (faith lost, apex structure lost,
    ///     intensity below apex, recognition share collapsed), success
    ///     at resolveAt.
    ///   simulation.js failDivineRightDeclaration (~10691-10713):
    ///     intensity drain, legitimacy penalty (deferred), cooldown set
    ///     (deferred), failure narrative.
    ///   simulation.js completeDivineRightDeclaration (~10715-10741):
    ///     legitimacy +12 (deferred), game-over signaling (deferred),
    ///     success narrative.
    ///   simulation.js startDivineRightDeclaration conviction event
    ///     (~10806-10812): dark=desecration+3 / light=oathkeeping+3
    ///     (deferred by execution system; fired here at resolution).
    ///
    /// Phases:
    ///   PhaseSuccessResolution: Operation past ResolveAtInWorldDays with
    ///     source holding faith -> op finalized (Active=false), oathkeeping
    ///     +3 conviction, success narrative pushed.
    ///   PhaseFailureFaithLost: Source loses faith commitment (SelectedFaith
    ///     == None) mid-window -> op finalized (Active=false), intensity
    ///     drained by 18, desecration +3 conviction (dark doctrine), failure
    ///     narrative pushed.
    ///   PhaseFailureIntensityDropped: Source intensity drops below 80 mid-window
    ///     -> op finalized (Active=false), intensity drained further by 18,
    ///     conviction fired, failure narrative pushed.
    ///   PhaseVoidSourceGone: Source faction entity missing -> op finalized
    ///     silently (Active=false), no crash.
    ///
    /// Artifact: artifacts/unity-divine-right-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesDivineRightResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-divine-right-resolution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Divine Right Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDivineRightResolutionSmokeValidation() =>
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
                message = "Divine right resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_DIVINE_RIGHT_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseFailureFaithLost(sb);
            ok &= RunPhaseFailureIntensityDropped(sb);
            ok &= RunPhaseVoidSourceGone(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupResolutionSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIDivineRightResolutionSystem>());
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

        private static Entity CreateFactionWithFaith(
            EntityManager em,
            string factionId,
            CovenantId faith,
            DoctrinePath doctrine,
            float intensity)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent),
                typeof(ConvictionComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new FaithStateComponent
            {
                SelectedFaith = faith,
                DoctrinePath  = doctrine,
                Intensity     = intensity,
                Level         = 5,
            });
            em.SetComponentData(e, new ConvictionComponent
            {
                Oathkeeping = 0f,
                Desecration = 0f,
                Stewardship = 0f,
                Ruthlessness = 0f,
                Score = 0f,
                Band = ConvictionBand.Neutral,
            });
            return e;
        }

        private static Entity CreateDivineRightOp(
            EntityManager em,
            string sourceFactionId,
            float resolveAtInWorldDays,
            DoctrinePath doctrinePath,
            CovenantId faithId)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationDivineRightComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-divineright-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = default,
                OperationKind        = DynastyOperationKind.DivineRight,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationDivineRightComponent
            {
                ResolveAtInWorldDays    = resolveAtInWorldDays,
                SourceFaithId           = FaithIdToString(faithId),
                DoctrinePath            = doctrinePath,
                RecognitionShare        = 0f,
                RecognitionSharePct     = 0f,
                ActiveApexStructureId   = default,
                ActiveApexStructureName = default,
            });
            return opEntity;
        }

        private static FixedString64Bytes FaithIdToString(CovenantId id)
        {
            switch (id)
            {
                case CovenantId.OldLight:      return new FixedString64Bytes("old_light");
                case CovenantId.BloodDominion: return new FixedString64Bytes("blood_dominion");
                case CovenantId.TheOrder:      return new FixedString64Bytes("the_order");
                case CovenantId.TheWild:       return new FixedString64Bytes("the_wild");
                default:                       return new FixedString64Bytes("none");
            }
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessResolution(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-resolution-success");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            // inWorldDays=200, ResolveAt=180 (past threshold).
            SeedDualClock(em, inWorldDays: 200f);

            // Source faction: enemy, OldLight/Light, intensity=90 (above 80 threshold).
            var sourceEntity = CreateFactionWithFaith(em, "enemy",
                CovenantId.OldLight, DoctrinePath.Light, 90f);

            // Op created with ResolveAt=180 (past inWorldDays=200).
            var opEntity = CreateDivineRightOp(em, "enemy", 180f, DoctrinePath.Light, CovenantId.OldLight);

            int narrativeBefore = CountNarrativeMessages(em);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            bool intensityUnchanged = System.Math.Abs(sourceFaith.Intensity - 90f) < 0.001f;

            var conv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool convictionFired = System.Math.Abs(conv.Oathkeeping - 3f) < 0.001f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && intensityUnchanged && convictionFired && narrativePushed;
            sb.AppendLine($"[PhaseSuccessResolution] op_finalized={opFinalized} intensity_unchanged={intensityUnchanged} oathkeeping+3={convictionFired} narrative={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureFaithLost(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-resolution-failure-faith-lost");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            // inWorldDays=50, ResolveAt=180 (still in window -- failure comes from faith loss).
            SeedDualClock(em, inWorldDays: 50f);

            // Source faction: enemy, dark doctrine. Faith is NONE (lost commitment).
            var sourceEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent),
                typeof(ConvictionComponent));
            em.SetComponentData(sourceEntity, new FactionComponent
            {
                FactionId = new FixedString32Bytes("enemy"),
            });
            em.SetComponentData(sourceEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(sourceEntity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,      // faith lost
                DoctrinePath  = DoctrinePath.Dark,
                Intensity     = 85f,
                Level         = 5,
            });
            em.SetComponentData(sourceEntity, new ConvictionComponent
            {
                Desecration = 0f, Oathkeeping = 0f,
                Stewardship = 0f, Ruthlessness = 0f,
                Score = 0f, Band = ConvictionBand.Neutral,
            });

            // Op still in window (ResolveAt=180, inWorldDays=50).
            var opEntity = CreateDivineRightOp(em, "enemy", 180f, DoctrinePath.Dark, CovenantId.BloodDominion);

            int narrativeBefore = CountNarrativeMessages(em);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            // Intensity was 85; faith=None triggers failure, but intensity drain applies to faith surface.
            // With SelectedFaith=None we still have FaithStateComponent; intensity drain = 18.
            // Expected: max(0, 85 - 18) = 67.
            bool intensityDrained = System.Math.Abs(sourceFaith.Intensity - 67f) < 0.001f;

            var conv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool convictionFired = System.Math.Abs(conv.Desecration - 3f) < 0.001f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && intensityDrained && convictionFired && narrativePushed;
            sb.AppendLine($"[PhaseFailureFaithLost] op_finalized={opFinalized} intensity_67={intensityDrained} desecration+3={convictionFired} narrative={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureIntensityDropped(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-resolution-failure-intensity");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            // inWorldDays=50, ResolveAt=180 (still in window).
            SeedDualClock(em, inWorldDays: 50f);

            // Source faction: intensity=40 (below 80 threshold) with committed faith.
            var sourceEntity = CreateFactionWithFaith(em, "enemy",
                CovenantId.OldLight, DoctrinePath.Light, 40f);

            var opEntity = CreateDivineRightOp(em, "enemy", 180f, DoctrinePath.Light, CovenantId.OldLight);

            int narrativeBefore = CountNarrativeMessages(em);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            // Intensity drain: max(0, 40 - 18) = 22.
            bool intensityDrained = System.Math.Abs(sourceFaith.Intensity - 22f) < 0.001f;

            var conv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool convictionFired = System.Math.Abs(conv.Oathkeeping - 3f) < 0.001f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && intensityDrained && convictionFired && narrativePushed;
            sb.AppendLine($"[PhaseFailureIntensityDropped] op_finalized={opFinalized} intensity_22={intensityDrained} oathkeeping+3={convictionFired} narrative={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseVoidSourceGone(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-resolution-void-source-gone");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, inWorldDays: 200f);

            // No source faction entity -- op should finalize silently.
            var opEntity = CreateDivineRightOp(em, "enemy", 180f, DoctrinePath.Light, CovenantId.OldLight);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            bool pass = opFinalized;
            sb.AppendLine($"[PhaseVoidSourceGone] op_finalized={opFinalized} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
