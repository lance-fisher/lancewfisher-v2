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
    /// Smoke validator for sub-slice 14 AI non-aggression pact proposal
    /// execution. Six phases cover every browser-side gate plus the
    /// success effects on resources, hostility, and the new
    /// PactComponent entity:
    ///
    ///   PhaseSuccessfulPact: source + target both kingdoms, mutual
    ///     hostility, source has 100 Influence + 200 Gold; after one
    ///     update there is exactly 1 PactComponent linking them, source
    ///     resources are 50/120, hostility is dropped both ways,
    ///     dispatch cleared.
    ///   PhaseHostilityRequired: source not hostile to target; pact
    ///     is rejected (browser areHostile gate); resources untouched,
    ///     no PactComponent created.
    ///   PhaseAlreadyPactedRejected: existing PactComponent already
    ///     links the two factions; new pact request short-circuits;
    ///     resources untouched, still 1 pact.
    ///   PhaseInsufficientInfluence: source has 30 Influence (below 50
    ///     threshold); pact rejected; gold untouched.
    ///   PhaseInsufficientGold: source has 60 Gold (below 80 threshold);
    ///     pact rejected; influence untouched.
    ///   PhaseTribeRejected: source is FactionKind.Tribe (not kingdom);
    ///     pact rejected per browser canon ("Both parties must be
    ///     kingdoms").
    ///
    /// Browser reference:
    ///   ai.js pact dispatch block (~2643-2666),
    ///   simulation.js getNonAggressionPactTerms (~5150-5183),
    ///   proposeNonAggressionPact (~5185-5222),
    ///   constants block (~5126-5128).
    /// Artifact: artifacts/unity-ai-pact-proposal-execution-smoke.log.
    /// </summary>
    public static class BloodlinesAIPactProposalExecutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-pact-proposal-execution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Pact Proposal Execution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAIPactProposalExecutionSmokeValidation() =>
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
                message = "AI pact proposal execution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_PACT_PROPOSAL_EXECUTION_SMOKE " +
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
            ok &= RunPhaseSuccessfulPact(sb);
            ok &= RunPhaseHostilityRequired(sb);
            ok &= RunPhaseAlreadyPactedRejected(sb);
            ok &= RunPhaseInsufficientInfluence(sb);
            ok &= RunPhaseInsufficientGold(sb);
            ok &= RunPhaseTribeRejected(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ setup

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIPactProposalExecutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays = 30f)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity SeedSourceFaction(
            EntityManager em,
            string factionId,
            float gold,
            float influence,
            CovertOpKind dispatched,
            FactionKind kind = FactionKind.Kingdom,
            FixedString32Bytes? hostileTo = null)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(ResourceStockpileComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = kind });
            em.SetComponentData(e, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(e, new ResourceStockpileComponent
            {
                Gold      = gold,
                Influence = influence,
            });
            if (hostileTo.HasValue)
            {
                var buf = em.GetBuffer<HostilityComponent>(e);
                buf.Add(new HostilityComponent { HostileFactionId = hostileTo.Value });
            }
            return e;
        }

        private static Entity SeedTargetFaction(
            EntityManager em,
            string factionId,
            FactionKind kind = FactionKind.Kingdom,
            FixedString32Bytes? hostileTo = null)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = kind });
            if (hostileTo.HasValue)
            {
                var buf = em.GetBuffer<HostilityComponent>(e);
                buf.Add(new HostilityComponent { HostileFactionId = hostileTo.Value });
            }
            return e;
        }

        private static int CountPacts(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            int n = q.CalculateEntityCount();
            q.Dispose();
            return n;
        }

        private static bool IsHostile(
            EntityManager em, Entity factionEntity, FixedString32Bytes targetFactionId)
        {
            if (!em.HasBuffer<HostilityComponent>(factionEntity)) return false;
            var buf = em.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < buf.Length; i++)
                if (buf[i].HostileFactionId.Equals(targetFactionId)) return true;
            return false;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessfulPact(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-success");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 30f);
            var enemy = SeedSourceFaction(em, "enemy", gold: 200f, influence: 100f,
                dispatched: CovertOpKind.PactProposal,
                hostileTo: new FixedString32Bytes("player"));
            var player = SeedTargetFaction(em, "player",
                hostileTo: new FixedString32Bytes("enemy"));

            world.Update();

            int pacts = CountPacts(em);
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemy);
            bool enemyHostile  = IsHostile(em, enemy, new FixedString32Bytes("player"));
            bool playerHostile = IsHostile(em, player, new FixedString32Bytes("enemy"));
            var covert = em.GetComponentData<AICovertOpsComponent>(enemy);

            if (pacts != 1)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected 1 PactComponent, got {pacts}");
                return false;
            }
            if (resources.Gold < 119.99f || resources.Gold > 120.01f)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected enemy gold 120 (200 - 80), got {resources.Gold}");
                return false;
            }
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected enemy influence 50 (100 - 50), got {resources.Influence}");
                return false;
            }
            if (enemyHostile || playerHostile)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: hostility should drop both ways; enemyHostile={enemyHostile}, playerHostile={playerHostile}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }

            // Verify pact metadata.
            var pactQuery = em.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            var pact = pactQuery.GetSingleton<PactComponent>();
            pactQuery.Dispose();
            if (!pact.FactionAId.Equals(new FixedString32Bytes("enemy")))
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected FactionAId=enemy, got {pact.FactionAId}");
                return false;
            }
            if (!pact.FactionBId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected FactionBId=player, got {pact.FactionBId}");
                return false;
            }
            if (pact.MinimumExpiresAtInWorldDays < 209.99f || pact.MinimumExpiresAtInWorldDays > 210.01f)
            {
                sb.AppendLine($"PhaseSuccessfulPact FAIL: expected minimum expiry 210 (30 + 180), got {pact.MinimumExpiresAtInWorldDays}");
                return false;
            }
            sb.AppendLine($"PhaseSuccessfulPact PASS: pact created, resources -50/-80, hostility dropped, dispatch cleared, expiry=210");
            return true;
        }

        private static bool RunPhaseHostilityRequired(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-no-hostility");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            // No hostility on either side.
            var enemy = SeedSourceFaction(em, "enemy", gold: 200f, influence: 100f,
                dispatched: CovertOpKind.PactProposal);
            SeedTargetFaction(em, "player");

            world.Update();

            int pacts = CountPacts(em);
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemy);

            if (pacts != 0)
            {
                sb.AppendLine($"PhaseHostilityRequired FAIL: expected 0 pacts (no hostility), got {pacts}");
                return false;
            }
            if (resources.Gold < 199.99f || resources.Gold > 200.01f)
            {
                sb.AppendLine($"PhaseHostilityRequired FAIL: gold should be untouched at 200, got {resources.Gold}");
                return false;
            }
            if (resources.Influence < 99.99f || resources.Influence > 100.01f)
            {
                sb.AppendLine($"PhaseHostilityRequired FAIL: influence should be untouched at 100, got {resources.Influence}");
                return false;
            }
            sb.AppendLine($"PhaseHostilityRequired PASS: no hostility blocks pact, resources untouched");
            return true;
        }

        private static bool RunPhaseAlreadyPactedRejected(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-already-pacted");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            var enemy = SeedSourceFaction(em, "enemy", gold: 200f, influence: 100f,
                dispatched: CovertOpKind.PactProposal,
                hostileTo: new FixedString32Bytes("player"));
            SeedTargetFaction(em, "player",
                hostileTo: new FixedString32Bytes("enemy"));

            // Pre-seed an existing pact between enemy and player.
            var existing = em.CreateEntity(typeof(PactComponent));
            em.SetComponentData(existing, new PactComponent
            {
                PactId                       = new FixedString64Bytes("existing-pact"),
                FactionAId                   = new FixedString32Bytes("enemy"),
                FactionBId                   = new FixedString32Bytes("player"),
                StartedAtInWorldDays         = 5f,
                MinimumExpiresAtInWorldDays  = 185f,
                Broken                       = false,
            });

            world.Update();

            int pacts = CountPacts(em);
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemy);

            if (pacts != 1)
            {
                sb.AppendLine($"PhaseAlreadyPactedRejected FAIL: expected 1 pact (the pre-existing one), got {pacts}");
                return false;
            }
            if (resources.Gold < 199.99f || resources.Gold > 200.01f ||
                resources.Influence < 99.99f || resources.Influence > 100.01f)
            {
                sb.AppendLine($"PhaseAlreadyPactedRejected FAIL: resources should be untouched; got gold={resources.Gold}, influence={resources.Influence}");
                return false;
            }
            sb.AppendLine($"PhaseAlreadyPactedRejected PASS: existing pact blocks new pact, resources untouched");
            return true;
        }

        private static bool RunPhaseInsufficientInfluence(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-no-influence");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            var enemy = SeedSourceFaction(em, "enemy", gold: 200f, influence: 30f,
                dispatched: CovertOpKind.PactProposal,
                hostileTo: new FixedString32Bytes("player"));
            SeedTargetFaction(em, "player",
                hostileTo: new FixedString32Bytes("enemy"));

            world.Update();

            int pacts = CountPacts(em);
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemy);

            if (pacts != 0)
            {
                sb.AppendLine($"PhaseInsufficientInfluence FAIL: expected 0 pacts, got {pacts}");
                return false;
            }
            if (resources.Gold < 199.99f || resources.Gold > 200.01f)
            {
                sb.AppendLine($"PhaseInsufficientInfluence FAIL: gold should be untouched; got {resources.Gold}");
                return false;
            }
            sb.AppendLine($"PhaseInsufficientInfluence PASS: 30 < 50 influence blocks pact, gold untouched");
            return true;
        }

        private static bool RunPhaseInsufficientGold(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-no-gold");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            var enemy = SeedSourceFaction(em, "enemy", gold: 60f, influence: 100f,
                dispatched: CovertOpKind.PactProposal,
                hostileTo: new FixedString32Bytes("player"));
            SeedTargetFaction(em, "player",
                hostileTo: new FixedString32Bytes("enemy"));

            world.Update();

            int pacts = CountPacts(em);
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemy);

            if (pacts != 0)
            {
                sb.AppendLine($"PhaseInsufficientGold FAIL: expected 0 pacts, got {pacts}");
                return false;
            }
            if (resources.Influence < 99.99f || resources.Influence > 100.01f)
            {
                sb.AppendLine($"PhaseInsufficientGold FAIL: influence should be untouched; got {resources.Influence}");
                return false;
            }
            sb.AppendLine($"PhaseInsufficientGold PASS: 60 < 80 gold blocks pact, influence untouched");
            return true;
        }

        private static bool RunPhaseTribeRejected(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-proposal-tribe-rejected");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);
            // Source is a Tribe rather than a Kingdom; canon requires
            // both parties to be kingdoms.
            SeedSourceFaction(em, "enemy", gold: 200f, influence: 100f,
                dispatched: CovertOpKind.PactProposal,
                kind: FactionKind.Tribes,
                hostileTo: new FixedString32Bytes("player"));
            SeedTargetFaction(em, "player",
                hostileTo: new FixedString32Bytes("enemy"));

            world.Update();

            int pacts = CountPacts(em);
            if (pacts != 0)
            {
                sb.AppendLine($"PhaseTribeRejected FAIL: tribe source should not propose pact; got {pacts} pacts");
                return false;
            }
            sb.AppendLine($"PhaseTribeRejected PASS: tribe source blocked from pact proposal");
            return true;
        }
    }
}
#endif
