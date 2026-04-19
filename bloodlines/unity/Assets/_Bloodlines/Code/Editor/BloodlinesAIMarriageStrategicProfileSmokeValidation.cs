#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for AIMarriageStrategicProfileSystem. Six phases that
    /// cover the four browser signals (hostility, population deficit,
    /// legitimacy distress gated by faith, succession pressure) plus the
    /// faith-compatibility "blocks weak match" path and the no-signal idle
    /// path.
    ///
    ///   Phase 1: No signals, unbound faith -> willing=false,
    ///            MarriageProposalGateMet=false,
    ///            MarriageInboxAcceptGate=false.
    ///   Phase 2: Hostility only -> willing=true (single signal sufficient
    ///            when faith does not block).
    ///   Phase 3: Population deficit only -> willing=true.
    ///   Phase 4: Legitimacy distress + harmonious faith ->
    ///            faithBackedLegitimacySignal counts, willing=true.
    ///   Phase 5: Enemy succession crisis only -> willing=true.
    ///   Phase 6: Single hostile signal but incompatible faith that blocks
    ///            weak match -> willing=false (blockedByFaith fires when
    ///            signalCount < 2).
    ///
    /// Browser reference: ai.js getAiMarriageStrategicProfile (~2803-2839).
    /// Artifact: artifacts/unity-ai-marriage-strategic-profile-smoke.log.
    /// </summary>
    public static class BloodlinesAIMarriageStrategicProfileSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-marriage-strategic-profile-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Marriage Strategic Profile Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIMarriageStrategicProfileSmokeValidation() =>
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
                message = "AI marriage strategic profile smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_MARRIAGE_STRATEGIC_PROFILE_SMOKE " +
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
            ok &= RunPhase1(sb);
            ok &= RunPhase2(sb);
            ok &= RunPhase3(sb);
            ok &= RunPhase4(sb);
            ok &= RunPhase5(sb);
            ok &= RunPhase6(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMarriageStrategicProfileSystem>());
            sg.SortSystems();
            return sg;
        }

        private struct PlayerSeed
        {
            public int PopulationTotal;
            public CovenantId Faith;
            public DoctrinePath Doctrine;
        }

        private struct AISeed
        {
            public int PopulationTotal;
            public float Legitimacy;
            public CovenantId Faith;
            public DoctrinePath Doctrine;
            public bool HostileToPlayer;
            public bool EnemySuccessionCrisis;
            public bool PlayerSuccessionCrisis;
        }

        private static void SeedPlayer(EntityManager em, PlayerSeed seed)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(PopulationComponent),
                typeof(FaithStateComponent),
                typeof(DynastyStateComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "player" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new PopulationComponent { Total = seed.PopulationTotal });
            em.SetComponentData(e, new FaithStateComponent
            {
                SelectedFaith = seed.Faith,
                DoctrinePath  = seed.Doctrine,
            });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = 100f });
        }

        private static Entity SeedAI(EntityManager em, AISeed seed)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(PopulationComponent),
                typeof(FaithStateComponent),
                typeof(DynastyStateComponent),
                typeof(AIStrategyComponent),
                typeof(AICovertOpsComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new PopulationComponent { Total = seed.PopulationTotal });
            em.SetComponentData(e, new FaithStateComponent
            {
                SelectedFaith = seed.Faith,
                DoctrinePath  = seed.Doctrine,
            });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = seed.Legitimacy });
            em.SetComponentData(e, new AIStrategyComponent
            {
                EnemySuccessionCrisisActive  = seed.EnemySuccessionCrisis,
                PlayerSuccessionCrisisActive = seed.PlayerSuccessionCrisis,
            });
            em.SetComponentData(e, new AICovertOpsComponent
            {
                MarriageProposalGateMet = false,
                MarriageInboxAcceptGate = false,
            });

            var buffer = em.GetBuffer<HostilityComponent>(e);
            if (seed.HostileToPlayer)
            {
                buffer.Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes("player"),
                });
            }

            return e;
        }

        private static bool CheckWilling(EntityManager em, Entity aiEntity, bool expected,
            System.Text.StringBuilder sb, string phase)
        {
            var covert = em.GetComponentData<AICovertOpsComponent>(aiEntity);
            bool proposalOk = covert.MarriageProposalGateMet == expected;
            bool inboxOk    = covert.MarriageInboxAcceptGate == expected;
            if (!proposalOk || !inboxOk)
            {
                sb.AppendLine($"{phase} FAIL: expected willing={expected}, got proposal={covert.MarriageProposalGateMet}, inbox={covert.MarriageInboxAcceptGate}");
                return false;
            }
            sb.AppendLine($"{phase} PASS: willing={expected} (proposal={covert.MarriageProposalGateMet}, inbox={covert.MarriageInboxAcceptGate})");
            return true;
        }

        // ------------------------------------------------------------------ Phase 1: no signals, unbound faith

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase1");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed
            {
                PopulationTotal = 100,
                Faith = CovenantId.None,
                Doctrine = DoctrinePath.Unassigned,
            });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 100, // no deficit
                Legitimacy = 100f,     // no distress
                Faith = CovenantId.None,
                Doctrine = DoctrinePath.Unassigned,
            });
            world.Update();
            return CheckWilling(em, ai, expected: false, sb, "Phase 1");
        }

        // ------------------------------------------------------------------ Phase 2: hostility only

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase2");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed { PopulationTotal = 100 });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 100,
                Legitimacy = 100f,
                HostileToPlayer = true,
            });
            world.Update();
            return CheckWilling(em, ai, expected: true, sb, "Phase 2");
        }

        // ------------------------------------------------------------------ Phase 3: population deficit only

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase3");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed { PopulationTotal = 100 });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 50, // well below 85% of 100
                Legitimacy = 100f,
            });
            world.Update();
            return CheckWilling(em, ai, expected: true, sb, "Phase 3");
        }

        // ------------------------------------------------------------------ Phase 4: legitimacy distress + harmonious faith

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase4");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed
            {
                PopulationTotal = 100,
                Faith    = CovenantId.OldLight,
                Doctrine = DoctrinePath.Light,
            });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 100,
                Legitimacy      = 30f, // below 50 threshold
                Faith           = CovenantId.OldLight,
                Doctrine        = DoctrinePath.Light, // harmonious match
            });
            world.Update();
            return CheckWilling(em, ai, expected: true, sb, "Phase 4");
        }

        // ------------------------------------------------------------------ Phase 5: enemy succession crisis only

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase5");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed { PopulationTotal = 100 });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 100,
                Legitimacy = 100f,
                EnemySuccessionCrisis = true,
            });
            world.Update();
            return CheckWilling(em, ai, expected: true, sb, "Phase 5");
        }

        // ------------------------------------------------------------------ Phase 6: single signal blocked by incompatible faith

        private static bool RunPhase6(System.Text.StringBuilder sb)
        {
            using var world = new World("strat-profile-phase6");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedPlayer(em, new PlayerSeed
            {
                PopulationTotal = 100,
                Faith    = CovenantId.OldLight,
                Doctrine = DoctrinePath.Light,
            });
            var ai = SeedAI(em, new AISeed
            {
                PopulationTotal = 100,
                Legitimacy = 100f,
                HostileToPlayer = true, // one signal
                Faith    = CovenantId.BloodDominion,
                Doctrine = DoctrinePath.Dark, // incompatible: different faith and doctrine
            });
            world.Update();
            // One signal + incompatible faith blocks weak match -> willing=false.
            return CheckWilling(em, ai, expected: false, sb, "Phase 6");
        }
    }
}
#endif
