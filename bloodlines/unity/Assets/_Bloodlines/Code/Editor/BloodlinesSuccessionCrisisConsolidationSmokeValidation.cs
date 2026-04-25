#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 34: AI succession-crisis auto-consolidation.
    ///
    /// Browser reference:
    ///   ai.js updateEnemyAi successionCrisisTimer block (~1167-1185):
    ///     if enemySuccessionCrisis: decrement timer, on fire check terms and
    ///     consolidate if available; reset 60s success, 18s retry.
    ///   simulation.js consolidateSuccessionCrisis (~4695-4741):
    ///     deduct resources, legitimacy+recovery, control-point loyalty+recovery,
    ///     Stewardship+conviction, declareInWorldTime, push narrative.
    ///
    /// Phases:
    ///   PhaseConsolidatesWhenAvailable: Moderate crisis, ruler active, resources ok ->
    ///     crisis removed, resources deducted, legitimacy+6, CP loyalty+4, Stewardship+3,
    ///     narrative pushed, timer reset to 60s.
    ///   PhaseRetryWhenResourcesShort: Minor crisis, ruler active, resources too low ->
    ///     crisis still present, timer reset to 18s.
    ///   PhaseSkipsWhenNoCrisis: no SuccessionCrisisComponent -> timer reset to 12s.
    ///   PhaseNotYetFired: timer > 0 -> no consolidation.
    ///
    /// Artifact: artifacts/unity-succession-crisis-consolidation-smoke.log.
    /// </summary>
    public static class BloodlinesSuccessionCrisisConsolidationSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-succession-crisis-consolidation-smoke.log";

        [MenuItem("Bloodlines/AI/Run Succession Crisis Consolidation Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSuccessionCrisisConsolidationSmokeValidation() =>
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
                message = "Succession crisis consolidation smoke errored: " + e;
            }

            string artifact = "BLOODLINES_SUCCESSION_CRISIS_CONSOLIDATION_SMOKE " +
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
            ok &= RunPhaseConsolidatesWhenAvailable(sb);
            ok &= RunPhaseRetryWhenResourcesShort(sb);
            ok &= RunPhaseSkipsWhenNoCrisis(sb);
            ok &= RunPhaseNotYetFired(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AISuccessionCrisisConsolidationSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays)
        {
            var e = em.CreateEntity(typeof(DualClockComponent), typeof(DeclareInWorldTimeRequest));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity CreateFactionWithConsolidationComp(
            EntityManager em,
            string factionId,
            float gold,
            float influence,
            float timer,
            bool addCrisis,
            byte crisisSeverity)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(AISuccessionCrisisConsolidationComponent));

            em.SetComponentData(e, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new ResourceStockpileComponent { Gold = gold, Influence = influence });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = 40f });
            em.SetComponentData(e, new ConvictionComponent
            {
                Stewardship = 0f, Oathkeeping = 0f, Ruthlessness = 0f,
                Desecration = 0f, Score = 0f, Band = ConvictionBand.Neutral,
            });
            em.SetComponentData(e, new AISuccessionCrisisConsolidationComponent
            {
                SuccessionCrisisTimer = timer,
            });

            if (addCrisis)
            {
                em.AddComponentData(e, new SuccessionCrisisComponent
                {
                    CrisisSeverity           = crisisSeverity,
                    CrisisStartedAtInWorldDays = 0f,
                    RecoveryProgressPct      = 0f,
                    ResourceTrickleFactor    = 0.9f,
                    LoyaltyShockApplied      = true,
                    LegitimacyDrainRatePerDay  = 0.14f,
                    LoyaltyDrainRatePerDay     = 0.2f,
                    LastDailyTickInWorldDays   = 0f,
                    RecoveryRatePerDay         = 0.05f,
                });
            }

            return e;
        }

        private static Entity AddRuler(EntityManager em, Entity factionEntity, bool fallen = false)
        {
            var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
            em.SetComponentData(memberEntity, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes("ruler-001"),
                Title    = new FixedString64Bytes("High King"),
                Role     = DynastyRole.HeadOfBloodline,
                Status   = fallen ? DynastyMemberStatus.Fallen : DynastyMemberStatus.Active,
                Renown   = 30f,
            });
            em.AddBuffer<DynastyMemberRef>(factionEntity);
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);
            roster.Add(new DynastyMemberRef { Member = memberEntity });
            return memberEntity;
        }

        private static Entity CreateControlPoint(EntityManager em, string ownerId, float loyalty)
        {
            var e = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(e, new ControlPointComponent
            {
                OwnerFactionId   = new FixedString32Bytes(ownerId),
                Loyalty          = loyalty,
                FortificationTier = 0,
            });
            return e;
        }

        private static int CountNarrativeMessages(EntityManager em) =>
            NarrativeMessageBridge.Count(em);

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseConsolidatesWhenAvailable(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-consolidate-available");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            SeedDualClock(em, 50f);

            // Moderate crisis: gold 110, influence 24, legit+6, loyalty+4, steward+3
            var factionEntity = CreateFactionWithConsolidationComp(em, "enemy",
                gold: 200f, influence: 50f, timer: 0f,
                addCrisis: true, crisisSeverity: (byte)SuccessionCrisisSeverity.Moderate);
            AddRuler(em, factionEntity);
            var cpEntity = CreateControlPoint(em, "enemy", 40f);

            int narrativeBefore = CountNarrativeMessages(em);
            sg.Update();

            bool crisisRemoved = !em.HasComponent<SuccessionCrisisComponent>(factionEntity);

            var resources = em.GetComponentData<ResourceStockpileComponent>(factionEntity);
            bool resourcesDeducted = resources.Gold <= 90f && resources.Influence <= 26f;

            var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
            bool legitimacyIncreased = dynasty.Legitimacy >= 46f;

            var cp = em.GetComponentData<ControlPointComponent>(cpEntity);
            bool loyaltyIncreased = cp.Loyalty >= 44f;

            var conv = em.GetComponentData<ConvictionComponent>(factionEntity);
            bool stewardshipGained = conv.Stewardship >= 3f;

            int narrativeAfter = CountNarrativeMessages(em);
            bool narrativePushed = narrativeAfter > narrativeBefore;

            var comp = em.GetComponentData<AISuccessionCrisisConsolidationComponent>(factionEntity);
            bool timerReset = comp.SuccessionCrisisTimer >= 55f;

            bool pass = crisisRemoved && resourcesDeducted && legitimacyIncreased &&
                        loyaltyIncreased && stewardshipGained && narrativePushed && timerReset;
            sb.AppendLine($"[PhaseConsolidatesWhenAvailable] crisisRemoved={crisisRemoved} " +
                          $"resourcesDeducted={resourcesDeducted} legitimacyIncreased={legitimacyIncreased} " +
                          $"loyaltyIncreased={loyaltyIncreased} stewardshipGained={stewardshipGained} " +
                          $"narrativePushed={narrativePushed} timerReset={timerReset} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseRetryWhenResourcesShort(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-retry-short");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            SeedDualClock(em, 50f);

            // Minor crisis: gold 80, influence 18 required -- give less
            var factionEntity = CreateFactionWithConsolidationComp(em, "enemy",
                gold: 50f, influence: 10f, timer: 0f,
                addCrisis: true, crisisSeverity: (byte)SuccessionCrisisSeverity.Minor);
            AddRuler(em, factionEntity);

            sg.Update();

            bool crisisStillPresent = em.HasComponent<SuccessionCrisisComponent>(factionEntity);

            var comp = em.GetComponentData<AISuccessionCrisisConsolidationComponent>(factionEntity);
            bool timerRetry = comp.SuccessionCrisisTimer >= 17f && comp.SuccessionCrisisTimer <= 19f;

            bool pass = crisisStillPresent && timerRetry;
            sb.AppendLine($"[PhaseRetryWhenResourcesShort] crisisStillPresent={crisisStillPresent} " +
                          $"timerRetry={timerRetry} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseSkipsWhenNoCrisis(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-no-crisis");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            SeedDualClock(em, 50f);

            // No SuccessionCrisisComponent
            var factionEntity = CreateFactionWithConsolidationComp(em, "enemy",
                gold: 500f, influence: 100f, timer: 0f,
                addCrisis: false, crisisSeverity: 0);

            sg.Update();

            var comp = em.GetComponentData<AISuccessionCrisisConsolidationComponent>(factionEntity);
            bool timerDefault = comp.SuccessionCrisisTimer >= 11f && comp.SuccessionCrisisTimer <= 13f;

            bool pass = timerDefault;
            sb.AppendLine($"[PhaseSkipsWhenNoCrisis] timerDefault={timerDefault} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }

        private static bool RunPhaseNotYetFired(System.Text.StringBuilder sb)
        {
            using var world = new World("sc-not-yet");
            var em = world.EntityManager;
            var sg = SetupSystems(world);

            SeedDualClock(em, 50f);

            // Timer still positive -- should not fire
            var factionEntity = CreateFactionWithConsolidationComp(em, "enemy",
                gold: 200f, influence: 50f, timer: 8f,
                addCrisis: true, crisisSeverity: (byte)SuccessionCrisisSeverity.Minor);
            AddRuler(em, factionEntity);

            sg.Update();

            bool crisisStillPresent = em.HasComponent<SuccessionCrisisComponent>(factionEntity);

            bool pass = crisisStillPresent;
            sb.AppendLine($"[PhaseNotYetFired] crisisStillPresent={crisisStillPresent} => {(pass ? "PASS" : "FAIL")}");
            return pass;
        }
    }
}
#endif
