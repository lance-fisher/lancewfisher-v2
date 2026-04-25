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
    /// Smoke validator for sub-slice 32: AI counter-intelligence dispatch and resolution.
    /// Covers AICounterIntelligenceResolutionSystem success (watch established),
    /// watch-cap enforcement (max 2), void-source-gone, and not-yet-resolved paths.
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations counter_intelligence branch (~5650-5679):
    ///     always succeeds: createCounterIntelligenceWatch, counterIntelligence.slice(0,2),
    ///     Stewardship+1 on source, narrative, finalize.
    ///     voids if faction not found.
    ///
    /// Phases:
    ///   PhaseSuccessWatchEstablished: op at resolveAt <= worldDays, source exists ->
    ///     op finalized (Active=false), watch buffer length = 1, Stewardship+1 on source.
    ///   PhaseWatchCapEnforced: source already has 2 active watches ->
    ///     op finalized, watch buffer length stays at 2 (new insertion blocked by cap).
    ///   PhaseVoidSourceGone: source faction entity missing ->
    ///     op finalized silently, no watch, no conviction.
    ///   PhaseNotYetResolved: ResolveAt in future -> op stays Active=true.
    ///
    /// Artifact: artifacts/unity-counter-intelligence-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesCounterIntelligenceResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-counter-intelligence-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Counter-Intelligence Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCounterIntelligenceResolutionSmokeValidation() =>
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
                message = "Counter-intelligence resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_CI_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseSuccessWatchEstablished(sb);
            ok &= RunPhaseWatchCapEnforced(sb);
            ok &= RunPhaseVoidSourceGone(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICounterIntelligenceResolutionSystem>());
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

        private static Entity CreateCIOp(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            float resolveAtInWorldDays,
            float watchStrength)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationCounterIntelligenceComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-ci-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperationKind        = DynastyOperationKind.CounterIntelligence,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationCounterIntelligenceComponent
            {
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperatorMemberId     = new FixedString64Bytes("spy-001"),
                OperatorTitle        = new FixedString64Bytes("Spymaster Vael"),
                ResolveAtInWorldDays = resolveAtInWorldDays,
                SuccessScore         = watchStrength,
                ProjectedChance      = 0.75f,
                EscrowGold           = 60f,
                EscrowInfluence      = 18f,
            });
            return opEntity;
        }

        private static bool Check(
            System.Text.StringBuilder sb,
            string phase,
            bool condition,
            string label)
        {
            string status = condition ? "PASS" : "FAIL";
            sb.AppendLine($"  [{status}] {phase}: {label}");
            return condition;
        }

        // ------------------------------------------------------------------ phase 1: success

        private static bool RunPhaseSuccessWatchEstablished(System.Text.StringBuilder sb)
        {
            using var world = new World("CI-Smoke-Phase1");
            var em = world.EntityManager;
            SeedDualClock(em, 100f);

            var sourceEntity = CreateFaction(em, "enemy1");
            CreateFaction(em, "player");

            var opEntity = CreateCIOp(em, "enemy1", "player",
                resolveAtInWorldDays: 90f,
                watchStrength: 18f);

            var sg = SetupResolutionSystems(world);
            sg.Update();
            world.EntityManager.CompleteAllTrackedJobs();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            var conviction = em.GetComponentData<ConvictionComponent>(sourceEntity);
            int watchCount = em.HasBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity)
                ? em.GetBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity).Length
                : 0;

            bool ok = true;
            ok &= Check(sb, "Phase1-WatchEstablished", !op.Active, "op.Active == false");
            ok &= Check(sb, "Phase1-WatchEstablished", watchCount == 1,
                $"watch count == 1 (got {watchCount})");
            ok &= Check(sb, "Phase1-WatchEstablished", conviction.Stewardship >= 1f,
                $"Stewardship >= 1 (got {conviction.Stewardship})");
            return ok;
        }

        // ------------------------------------------------------------------ phase 2: cap at 2

        private static bool RunPhaseWatchCapEnforced(System.Text.StringBuilder sb)
        {
            using var world = new World("CI-Smoke-Phase2");
            var em = world.EntityManager;
            SeedDualClock(em, 100f);

            var sourceEntity = CreateFaction(em, "enemy2");
            CreateFaction(em, "player");

            // Pre-seed 2 active watches (expiresAt far in future -- won't be pruned).
            em.AddBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity);
            var watches = em.GetBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity);
            watches.Add(new DynastyCounterIntelligenceWatchElement
                { TargetFactionId = new FixedString32Bytes("player"), WatchExpiresAtElapsed = 9999f });
            watches.Add(new DynastyCounterIntelligenceWatchElement
                { TargetFactionId = new FixedString32Bytes("player"), WatchExpiresAtElapsed = 9999f });

            var opEntity = CreateCIOp(em, "enemy2", "player",
                resolveAtInWorldDays: 90f,
                watchStrength: 18f);

            var sg = SetupResolutionSystems(world);
            sg.Update();
            world.EntityManager.CompleteAllTrackedJobs();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            int watchCount = em.HasBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity)
                ? em.GetBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity).Length
                : 0;

            bool ok = true;
            ok &= Check(sb, "Phase2-WatchCapEnforced", !op.Active, "op.Active == false");
            ok &= Check(sb, "Phase2-WatchCapEnforced", watchCount == 2,
                $"watch count stays at 2 (got {watchCount})");
            return ok;
        }

        // ------------------------------------------------------------------ phase 3: void source gone

        private static bool RunPhaseVoidSourceGone(System.Text.StringBuilder sb)
        {
            using var world = new World("CI-Smoke-Phase3");
            var em = world.EntityManager;
            SeedDualClock(em, 100f);
            // No source faction entity.
            CreateFaction(em, "player");

            var opEntity = CreateCIOp(em, "ghost-faction", "player",
                resolveAtInWorldDays: 90f,
                watchStrength: 14f);

            var sg = SetupResolutionSystems(world);
            sg.Update();
            world.EntityManager.CompleteAllTrackedJobs();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);

            bool ok = true;
            ok &= Check(sb, "Phase3-VoidSourceGone", !op.Active, "op.Active == false (void silently)");
            return ok;
        }

        // ------------------------------------------------------------------ phase 4: not yet resolved

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("CI-Smoke-Phase4");
            var em = world.EntityManager;
            SeedDualClock(em, 100f);

            CreateFaction(em, "enemy3");
            CreateFaction(em, "player");

            var opEntity = CreateCIOp(em, "enemy3", "player",
                resolveAtInWorldDays: 999f,
                watchStrength: 16f);

            var sg = SetupResolutionSystems(world);
            sg.Update();
            world.EntityManager.CompleteAllTrackedJobs();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);

            bool ok = true;
            ok &= Check(sb, "Phase4-NotYetResolved", op.Active, "op.Active == true (not yet resolved)");
            return ok;
        }
    }
}
#endif
