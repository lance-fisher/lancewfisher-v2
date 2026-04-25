#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.Raids;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 31: AI sabotage dispatch and resolution.
    /// Covers AISabotageResolutionSystem success (well_poisoning), success
    /// (supply_poisoning), failure/interception, void-building-destroyed, and
    /// not-yet-resolved paths.
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations sabotage branch (~5464-5514):
    ///     void: building destroyed before resolution -> finalize silently.
    ///     success (successScore >= 0): applySabotageEffect, conviction
    ///       (Desecration+2 for poisoning, Ruthlessness+2 for others),
    ///       narrative "[source] sabotage succeeds".
    ///     failure: Stewardship+1 on target, narrative "detected and foiled".
    ///
    /// Phases:
    ///   PhaseSuccessWellPoisoning: SuccessScore >= 0, well building alive ->
    ///     op finalized (Active=false), WaterStrainStreak+=2 on target,
    ///     Desecration+2 on source, success narrative pushed.
    ///   PhaseSuccessSupplyPoisoning: SuccessScore >= 0, supply_camp alive ->
    ///     op finalized, BuildingRaidStateComponent set, Desecration+2 on source,
    ///     success narrative pushed.
    ///   PhaseFailureInterception: SuccessScore < 0, building alive ->
    ///     op finalized, Stewardship+1 on target, failure narrative pushed.
    ///   PhaseVoidBuildingDestroyed: building health <= 0 ->
    ///     op finalized silently, no conviction.
    ///   PhaseNotYetResolved: ResolveAt still in future -> op stays Active=true.
    ///
    /// Artifact: artifacts/unity-sabotage-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesSabotageResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-sabotage-resolution-smoke.log";

        [MenuItem("Bloodlines/AI/Run Sabotage Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSabotageResolutionSmokeValidation() =>
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
                message = "Sabotage resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_SABOTAGE_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseSuccessWellPoisoning(sb);
            ok &= RunPhaseSuccessSupplyPoisoning(sb);
            ok &= RunPhaseFailureInterception(sb);
            ok &= RunPhaseVoidBuildingDestroyed(sb);
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
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AISabotageResolutionSystem>());
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
                typeof(ConvictionComponent),
                typeof(RealmConditionComponent));
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
            em.SetComponentData(e, new RealmConditionComponent { WaterStrainStreak = 0 });
            return e;
        }

        private static Entity CreateBuilding(
            EntityManager em,
            string factionId,
            string typeId,
            float health)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(BuildingTypeComponent),
                typeof(HealthComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(e, new BuildingTypeComponent { TypeId = new FixedString64Bytes(typeId) });
            em.SetComponentData(e, new HealthComponent { Current = health, Max = 100f });
            return e;
        }

        private static Entity CreateSabotageOp(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            string subtype,
            int targetBuildingEntityIndex,
            float resolveAtInWorldDays,
            float successScore,
            float escrowInfluence)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationSabotageComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-sab-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperationKind        = DynastyOperationKind.Sabotage,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationSabotageComponent
            {
                TargetFactionId           = new FixedString32Bytes(targetFactionId),
                Subtype                   = new FixedString32Bytes(subtype),
                TargetBuildingTypeId      = new FixedString64Bytes(subtype == "gate_opening" ? "gatehouse" :
                                                                    subtype == "fire_raising"  ? "farm" :
                                                                    subtype == "well_poisoning"? "well" : "supply_camp"),
                TargetBuildingEntityIndex = targetBuildingEntityIndex,
                OperatorMemberId          = new FixedString64Bytes("spy-001"),
                OperatorTitle             = new FixedString64Bytes("Spymaster Vael"),
                ResolveAtInWorldDays      = resolveAtInWorldDays,
                SuccessScore              = successScore,
                ProjectedChance           = successScore >= 0f ? 0.75f : 0.25f,
                EscrowGold                = 50f,
                EscrowInfluence           = escrowInfluence,
            });
            return opEntity;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessWellPoisoning(System.Text.StringBuilder sb)
        {
            using var world = new World("sab-well-success");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player");
            var buildingEntity = CreateBuilding(em, "player", "well", 100f);

            var opEntity = CreateSabotageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                subtype: "well_poisoning",
                targetBuildingEntityIndex: buildingEntity.Index,
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 20f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var targetRealm = em.GetComponentData<RealmConditionComponent>(targetEntity);
            bool waterStrained = targetRealm.WaterStrainStreak >= 2;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool desecrationOnSource = sourceConv.Desecration >= 2f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && waterStrained && desecrationOnSource && narrativePushed;
            sb.AppendLine($"[PhaseSuccessWellPoisoning] opFinalized={opFinalized} waterStrained={waterStrained} " +
                          $"desecrationOnSource={desecrationOnSource} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseSuccessSupplyPoisoning(System.Text.StringBuilder sb)
        {
            using var world = new World("sab-supply-success");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            CreateFaction(em, "player");
            var buildingEntity = CreateBuilding(em, "player", "supply_camp", 100f);

            var opEntity = CreateSabotageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                subtype: "supply_poisoning",
                targetBuildingEntityIndex: buildingEntity.Index,
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 15f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            bool raidStateSet = em.HasComponent<BuildingRaidStateComponent>(buildingEntity);

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool desecrationOnSource = sourceConv.Desecration >= 2f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            bool pass = opFinalized && raidStateSet && desecrationOnSource && narrativePushed;
            sb.AppendLine($"[PhaseSuccessSupplyPoisoning] opFinalized={opFinalized} raidStateSet={raidStateSet} " +
                          $"desecrationOnSource={desecrationOnSource} narrativePushed={narrativePushed} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseFailureInterception(System.Text.StringBuilder sb)
        {
            using var world = new World("sab-failure");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            CreateFaction(em, "enemy");
            var targetEntity = CreateFaction(em, "player");
            var buildingEntity = CreateBuilding(em, "player", "well", 100f);

            var opEntity = CreateSabotageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                subtype: "well_poisoning",
                targetBuildingEntityIndex: buildingEntity.Index,
                resolveAtInWorldDays: 30f,
                successScore: -10f,
                escrowInfluence: 20f);

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

        private static bool RunPhaseVoidBuildingDestroyed(System.Text.StringBuilder sb)
        {
            using var world = new World("sab-void");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 50f);

            var sourceEntity = CreateFaction(em, "enemy");
            CreateFaction(em, "player");
            // Building with health = 0 (destroyed)
            var buildingEntity = CreateBuilding(em, "player", "well", 0f);

            var opEntity = CreateSabotageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                subtype: "well_poisoning",
                targetBuildingEntityIndex: buildingEntity.Index,
                resolveAtInWorldDays: 30f,
                successScore: 5f,
                escrowInfluence: 20f);

            sg.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            bool opFinalized = !op.Active;

            var sourceConv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            bool noConviction = sourceConv.Desecration == 0f && sourceConv.Ruthlessness == 0f;

            bool pass = opFinalized && noConviction;
            sb.AppendLine($"[PhaseVoidBuildingDestroyed] opFinalized={opFinalized} noConviction={noConviction} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetResolved(System.Text.StringBuilder sb)
        {
            using var world = new World("sab-not-yet");
            var em = world.EntityManager;
            var sg = SetupResolutionSystems(world);

            SeedDualClock(em, 10f);

            CreateFaction(em, "enemy");
            CreateFaction(em, "player");
            var buildingEntity = CreateBuilding(em, "player", "well", 100f);

            var opEntity = CreateSabotageOp(em,
                sourceFactionId: "enemy",
                targetFactionId: "player",
                subtype: "well_poisoning",
                targetBuildingEntityIndex: buildingEntity.Index,
                resolveAtInWorldDays: 50f,
                successScore: 5f,
                escrowInfluence: 20f);

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
