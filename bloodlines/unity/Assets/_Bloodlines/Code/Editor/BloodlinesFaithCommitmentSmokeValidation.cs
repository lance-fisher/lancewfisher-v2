#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 35: AI faith commitment auto-selection.
    ///
    /// Browser reference:
    ///   ai.js updateEnemyAi lines 1253-1260:
    ///     if (!enemy.faith.selectedFaithId) pick highest-exposure available faith
    ///     and call chooseFaithCommitment.
    ///   simulation.js chooseFaithCommitment (line 9694):
    ///     set selectedFaithId, doctrinePath, intensity=20, level, Oathkeeping+2.
    ///
    /// Phases:
    ///   PhaseCommitsWhenAvailable: AI faction with None faith and OldLight at 110
    ///     exposure + Discovered -> commits to OldLight, Intensity=20, Oathkeeping+2,
    ///     narrative pushed.
    ///   PhaseSkipsWhenExposureLow: OldLight at 80 (below threshold 100) ->
    ///     SelectedFaith stays None.
    ///   PhaseSkipsWhenAlreadyCommitted: SelectedFaith already set ->
    ///     no conviction change.
    ///
    /// Artifact: artifacts/unity-faith-commitment-smoke.log.
    /// </summary>
    public static class BloodlinesFaithCommitmentSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-faith-commitment-smoke.log";

        [MenuItem("Bloodlines/AI/Run Faith Commitment Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFaithCommitmentSmokeValidation() =>
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
                message = "Faith commitment smoke errored: " + e;
            }

            string artifact = "BLOODLINES_FAITH_COMMITMENT_SMOKE " +
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
            ok &= RunPhaseCommitsWhenAvailable(sb);
            ok &= RunPhaseSkipsWhenExposureLow(sb);
            ok &= RunPhaseSkipsWhenAlreadyCommitted(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIFaithCommitmentSystem>());
            sg.SortSystems();
            return sg;
        }

        private static Entity CreateAIFaction(
            EntityManager em,
            string factionId,
            CovenantId selectedFaith = CovenantId.None,
            float oathkeepingStart = 0f)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(AIEconomyControllerComponent),
                typeof(FaithStateComponent),
                typeof(ConvictionComponent));

            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            em.SetComponentData(e, new FaithStateComponent
            {
                SelectedFaith = selectedFaith,
                DoctrinePath  = DoctrinePath.Unassigned,
                Intensity     = 0f,
                Level         = 0,
            });
            em.SetComponentData(e, new ConvictionComponent
            {
                Stewardship  = 0f,
                Oathkeeping  = oathkeepingStart,
                Ruthlessness = 0f,
                Desecration  = 0f,
                Score        = 0f,
                Band         = ConvictionBand.Neutral,
            });
            return e;
        }

        private static void AddExposure(EntityManager em, Entity entity, CovenantId faith, float exposure, bool discovered)
        {
            if (!em.HasBuffer<FaithExposureElement>(entity))
                em.AddBuffer<FaithExposureElement>(entity);
            var buf = em.GetBuffer<FaithExposureElement>(entity);
            buf.Add(new FaithExposureElement
            {
                Faith      = faith,
                Exposure   = exposure,
                Discovered = discovered,
            });
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseCommitsWhenAvailable(System.Text.StringBuilder sb)
        {
            using var world = new World("faith-commit-available");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var factionEntity = CreateAIFaction(em, "test_enemy");
            AddExposure(em, factionEntity, CovenantId.OldLight, 110f, discovered: true);

            int narrativeBefore = NarrativeMessageBridge.Count(em);
            sg.Update();

            var faith = em.GetComponentData<FaithStateComponent>(factionEntity);
            bool faithSet       = faith.SelectedFaith == CovenantId.OldLight;
            bool intensitySet   = Math.Abs(faith.Intensity - 20f) < 0.01f;
            bool levelSet       = faith.Level == 1;
            bool doctrineSet    = faith.DoctrinePath == DoctrinePath.Light;

            var conv = em.GetComponentData<ConvictionComponent>(factionEntity);
            bool oathkeepingGained = conv.Oathkeeping >= 2f;

            bool narrativePushed = NarrativeMessageBridge.Count(em) > narrativeBefore;

            bool pass = faithSet && intensitySet && levelSet && doctrineSet &&
                        oathkeepingGained && narrativePushed;
            sb.AppendLine($"[PhaseCommitsWhenAvailable] faithSet={faithSet} intensity={faith.Intensity} " +
                          $"level={faith.Level} doctrine={faith.DoctrinePath} " +
                          $"oathkeeping={conv.Oathkeeping} narrativePushed={narrativePushed} " +
                          $"=> {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseSkipsWhenExposureLow(System.Text.StringBuilder sb)
        {
            using var world = new World("faith-commit-low-exposure");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            var factionEntity = CreateAIFaction(em, "test_enemy");
            AddExposure(em, factionEntity, CovenantId.OldLight, 80f, discovered: true);

            sg.Update();

            var faith = em.GetComponentData<FaithStateComponent>(factionEntity);
            bool stillNone = faith.SelectedFaith == CovenantId.None;

            bool pass = stillNone;
            sb.AppendLine($"[PhaseSkipsWhenExposureLow] selectedFaith={faith.SelectedFaith} " +
                          $"=> {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseSkipsWhenAlreadyCommitted(System.Text.StringBuilder sb)
        {
            using var world = new World("faith-commit-already");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            // Already committed -- give high exposure too so the gate check is the only stopper
            var factionEntity = CreateAIFaction(em, "test_enemy",
                selectedFaith: CovenantId.OldLight,
                oathkeepingStart: 5f);
            AddExposure(em, factionEntity, CovenantId.OldLight, 110f, discovered: true);

            sg.Update();

            var conv = em.GetComponentData<ConvictionComponent>(factionEntity);
            bool convictionUnchanged = Math.Abs(conv.Oathkeeping - 5f) < 0.01f;

            var faith = em.GetComponentData<FaithStateComponent>(factionEntity);
            bool faithUnchanged = faith.SelectedFaith == CovenantId.OldLight;

            bool pass = convictionUnchanged && faithUnchanged;
            sb.AppendLine($"[PhaseSkipsWhenAlreadyCommitted] oathkeeping={conv.Oathkeeping} " +
                          $"faithUnchanged={faithUnchanged} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
