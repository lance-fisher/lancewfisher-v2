#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the sub-slice 15 pact-break path. Five phases
    /// cover the browser-side breakNonAggressionPact semantics plus the
    /// idempotency and no-pact-no-op edges:
    ///
    ///   PhaseEarlyBreak: pact at day 30 minimum expiry 210; current day
    ///     50 so earlyBreak is true. Break request applied -> pact
    ///     Broken=true, BrokenByFactionId=breaker, hostility restored
    ///     both ways, breaker legitimacy 70 -> 62, breaker Oathkeeping
    ///     5 -> 3. Request entity destroyed after processing.
    ///   PhaseLateBreak: pact at day 10 minimum expiry 190; current day
    ///     250 so earlyBreak is false. Same mechanical penalty applies
    ///     (browser: -8 legitimacy and -2 oathkeeping are unconditional
    ///     regardless of early-break).
    ///   PhaseIdempotentBreak: a second break request after pact is
    ///     already Broken does not double-apply the penalty. Legitimacy
    ///     and Oathkeeping stay at the first-break values.
    ///   PhaseNoPactNoOp: break request with a PactId that does not
    ///     match any PactComponent. No state changes; request entity is
    ///     still destroyed.
    ///   PhaseLegitimacyClamp: breaker starts at Legitimacy 5; -8 cost
    ///     clamps to 0 via math.clamp [0, 100]. Oathkeeping also clamps
    ///     at 0 via ConvictionScoring.ApplyEvent's max(0, amount).
    ///
    /// Browser reference: simulation.js breakNonAggressionPact
    /// (~5224-5257), NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8
    /// (~5129).
    /// Artifact: artifacts/unity-pact-break-smoke.log.
    /// </summary>
    public static class BloodlinesPactBreakSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-pact-break-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Pact Break Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPactBreakSmokeValidation() =>
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
                message = "Pact break smoke errored: " + e;
            }

            string artifact = "BLOODLINES_PACT_BREAK_SMOKE " +
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
            ok &= RunPhaseEarlyBreak(sb);
            ok &= RunPhaseLateBreak(sb);
            ok &= RunPhaseIdempotentBreak(sb);
            ok &= RunPhaseNoPactNoOp(sb);
            ok &= RunPhaseLegitimacyClamp(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ setup

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<PactBreakSystem>());
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

        private static Entity SeedFaction(
            EntityManager em,
            string factionId,
            float legitimacy,
            float oathkeeping)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(HostilityComponent));
            em.SetComponentData(e, new FactionComponent { FactionId = factionId });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new DynastyStateComponent { Legitimacy = legitimacy });
            em.SetComponentData(e, new ConvictionComponent { Oathkeeping = oathkeeping });
            return e;
        }

        private static Entity SeedPact(
            EntityManager em,
            string pactId,
            string factionA,
            string factionB,
            float startedAt,
            float minimumExpiry)
        {
            var e = em.CreateEntity(typeof(PactComponent));
            em.SetComponentData(e, new PactComponent
            {
                PactId                       = new FixedString64Bytes(pactId),
                FactionAId                   = new FixedString32Bytes(factionA),
                FactionBId                   = new FixedString32Bytes(factionB),
                StartedAtInWorldDays         = startedAt,
                MinimumExpiresAtInWorldDays  = minimumExpiry,
                Broken                       = false,
                BrokenByFactionId            = default,
            });
            return e;
        }

        private static Entity SeedBreakRequest(
            EntityManager em,
            string pactId,
            string requestingFactionId)
        {
            var e = em.CreateEntity(typeof(PactBreakRequestComponent));
            em.SetComponentData(e, new PactBreakRequestComponent
            {
                PactId              = new FixedString64Bytes(pactId),
                RequestingFactionId = new FixedString32Bytes(requestingFactionId),
            });
            return e;
        }

        private static bool IsHostile(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity sourceEntity = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
                if (factions[i].FactionId.Equals(sourceFactionId))
                {
                    sourceEntity = entities[i];
                    break;
                }
            entities.Dispose();
            factions.Dispose();

            if (sourceEntity == Entity.Null) return false;
            if (!em.HasBuffer<HostilityComponent>(sourceEntity)) return false;

            var buf = em.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = 0; i < buf.Length; i++)
                if (buf[i].HostileFactionId.Equals(targetFactionId)) return true;
            return false;
        }

        private static int CountBreakRequests(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<PactBreakRequestComponent>());
            int n = q.CalculateEntityCount();
            q.Dispose();
            return n;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseEarlyBreak(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-break-early");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            var enemy  = SeedFaction(em, "enemy",  legitimacy: 70f, oathkeeping: 5f);
            var player = SeedFaction(em, "player", legitimacy: 60f, oathkeeping: 4f);
            var pactEntity = SeedPact(em, "nap-early", "enemy", "player",
                startedAt: 30f, minimumExpiry: 210f);
            SeedBreakRequest(em, "nap-early", "enemy");

            world.Update();

            var pact = em.GetComponentData<PactComponent>(pactEntity);
            var enemyDynasty  = em.GetComponentData<DynastyStateComponent>(enemy);
            var enemyConv     = em.GetComponentData<ConvictionComponent>(enemy);
            bool enemyHostile  = IsHostile(em, new FixedString32Bytes("enemy"),  new FixedString32Bytes("player"));
            bool playerHostile = IsHostile(em, new FixedString32Bytes("player"), new FixedString32Bytes("enemy"));
            int pendingRequests = CountBreakRequests(em);

            if (!pact.Broken)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: pact should be marked Broken");
                return false;
            }
            if (!pact.BrokenByFactionId.Equals(new FixedString32Bytes("enemy")))
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: BrokenByFactionId should be enemy; got {pact.BrokenByFactionId}");
                return false;
            }
            if (enemyDynasty.Legitimacy < 61.99f || enemyDynasty.Legitimacy > 62.01f)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: enemy legitimacy expected 62 (70 - 8), got {enemyDynasty.Legitimacy}");
                return false;
            }
            if (enemyConv.Oathkeeping < 2.99f || enemyConv.Oathkeeping > 3.01f)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: enemy oathkeeping expected 3 (5 - 2), got {enemyConv.Oathkeeping}");
                return false;
            }
            if (!enemyHostile || !playerHostile)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: hostility should be re-established both ways; enemy={enemyHostile} player={playerHostile}");
                return false;
            }
            if (pendingRequests != 0)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: break request should be destroyed; got {pendingRequests} pending");
                return false;
            }

            // Player side unchanged.
            var playerDynasty = em.GetComponentData<DynastyStateComponent>(player);
            var playerConv    = em.GetComponentData<ConvictionComponent>(player);
            if (playerDynasty.Legitimacy < 59.99f || playerDynasty.Legitimacy > 60.01f)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: player legitimacy should be untouched at 60; got {playerDynasty.Legitimacy}");
                return false;
            }
            if (playerConv.Oathkeeping < 3.99f || playerConv.Oathkeeping > 4.01f)
            {
                sb.AppendLine($"PhaseEarlyBreak FAIL: player oathkeeping should be untouched at 4; got {playerConv.Oathkeeping}");
                return false;
            }
            sb.AppendLine($"PhaseEarlyBreak PASS: pact broken by enemy, legitimacy 70->62, oathkeeping 5->3, hostility restored, request destroyed");
            return true;
        }

        private static bool RunPhaseLateBreak(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-break-late");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 250f);
            var enemy  = SeedFaction(em, "enemy",  legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "player", legitimacy: 60f, oathkeeping: 4f);
            var pactEntity = SeedPact(em, "nap-late", "enemy", "player",
                startedAt: 10f, minimumExpiry: 190f);
            SeedBreakRequest(em, "nap-late", "enemy");

            world.Update();

            var pact = em.GetComponentData<PactComponent>(pactEntity);
            var enemyDynasty = em.GetComponentData<DynastyStateComponent>(enemy);
            var enemyConv    = em.GetComponentData<ConvictionComponent>(enemy);

            if (!pact.Broken)
            {
                sb.AppendLine($"PhaseLateBreak FAIL: pact should be marked Broken");
                return false;
            }
            // Browser: penalty is unconditional regardless of early-break status.
            if (enemyDynasty.Legitimacy < 61.99f || enemyDynasty.Legitimacy > 62.01f)
            {
                sb.AppendLine($"PhaseLateBreak FAIL: enemy legitimacy expected 62 (penalty applies regardless of timing), got {enemyDynasty.Legitimacy}");
                return false;
            }
            if (enemyConv.Oathkeeping < 2.99f || enemyConv.Oathkeeping > 3.01f)
            {
                sb.AppendLine($"PhaseLateBreak FAIL: enemy oathkeeping expected 3 (penalty applies regardless of timing), got {enemyConv.Oathkeeping}");
                return false;
            }
            sb.AppendLine($"PhaseLateBreak PASS: late-break penalty identical to early-break (legitimacy -8, oathkeeping -2)");
            return true;
        }

        private static bool RunPhaseIdempotentBreak(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-break-idempotent");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            var enemy = SeedFaction(em, "enemy", legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "player", legitimacy: 60f, oathkeeping: 4f);
            SeedPact(em, "nap-idem", "enemy", "player",
                startedAt: 30f, minimumExpiry: 210f);

            // First break request.
            SeedBreakRequest(em, "nap-idem", "enemy");
            world.Update();

            var firstLegitimacy   = em.GetComponentData<DynastyStateComponent>(enemy).Legitimacy;
            var firstOathkeeping  = em.GetComponentData<ConvictionComponent>(enemy).Oathkeeping;

            // Second break request on the same (now-broken) pact.
            SeedBreakRequest(em, "nap-idem", "enemy");
            world.Update();

            var secondLegitimacy  = em.GetComponentData<DynastyStateComponent>(enemy).Legitimacy;
            var secondOathkeeping = em.GetComponentData<ConvictionComponent>(enemy).Oathkeeping;

            if (math.abs(firstLegitimacy - secondLegitimacy) > 0.01f)
            {
                sb.AppendLine($"PhaseIdempotentBreak FAIL: legitimacy drifted on second break ({firstLegitimacy} -> {secondLegitimacy})");
                return false;
            }
            if (math.abs(firstOathkeeping - secondOathkeeping) > 0.01f)
            {
                sb.AppendLine($"PhaseIdempotentBreak FAIL: oathkeeping drifted on second break ({firstOathkeeping} -> {secondOathkeeping})");
                return false;
            }
            sb.AppendLine($"PhaseIdempotentBreak PASS: second break request does not double-apply penalty (legitimacy stable at {secondLegitimacy}, oathkeeping at {secondOathkeeping})");
            return true;
        }

        private static bool RunPhaseNoPactNoOp(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-break-no-pact");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            var enemy = SeedFaction(em, "enemy", legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "player", legitimacy: 60f, oathkeeping: 4f);
            // No PactComponent seeded.
            SeedBreakRequest(em, "nap-missing", "enemy");

            world.Update();

            var enemyDynasty   = em.GetComponentData<DynastyStateComponent>(enemy);
            var enemyConv      = em.GetComponentData<ConvictionComponent>(enemy);
            int pendingRequests = CountBreakRequests(em);

            if (enemyDynasty.Legitimacy < 69.99f || enemyDynasty.Legitimacy > 70.01f)
            {
                sb.AppendLine($"PhaseNoPactNoOp FAIL: legitimacy should be untouched at 70; got {enemyDynasty.Legitimacy}");
                return false;
            }
            if (enemyConv.Oathkeeping < 4.99f || enemyConv.Oathkeeping > 5.01f)
            {
                sb.AppendLine($"PhaseNoPactNoOp FAIL: oathkeeping should be untouched at 5; got {enemyConv.Oathkeeping}");
                return false;
            }
            if (pendingRequests != 0)
            {
                sb.AppendLine($"PhaseNoPactNoOp FAIL: break request should still be destroyed; got {pendingRequests}");
                return false;
            }
            sb.AppendLine($"PhaseNoPactNoOp PASS: no matching pact leaves state untouched, request still destroyed");
            return true;
        }

        private static bool RunPhaseLegitimacyClamp(System.Text.StringBuilder sb)
        {
            using var world = new World("pact-break-clamp");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            // Breaker starts at 5 legitimacy and 0 oathkeeping. Penalty
            // clamps legitimacy to 0 (can't go below 0) and oathkeeping
            // at 0 (ConvictionScoring.ApplyEvent max(0, amount + current)).
            var enemy = SeedFaction(em, "enemy", legitimacy: 5f, oathkeeping: 0f);
            SeedFaction(em, "player", legitimacy: 60f, oathkeeping: 4f);
            SeedPact(em, "nap-clamp", "enemy", "player",
                startedAt: 30f, minimumExpiry: 210f);
            SeedBreakRequest(em, "nap-clamp", "enemy");

            world.Update();

            var enemyDynasty = em.GetComponentData<DynastyStateComponent>(enemy);
            var enemyConv    = em.GetComponentData<ConvictionComponent>(enemy);

            if (enemyDynasty.Legitimacy < -0.01f || enemyDynasty.Legitimacy > 0.01f)
            {
                sb.AppendLine($"PhaseLegitimacyClamp FAIL: legitimacy should clamp to 0 (5 - 8 = -3 -> 0); got {enemyDynasty.Legitimacy}");
                return false;
            }
            if (enemyConv.Oathkeeping < -0.01f || enemyConv.Oathkeeping > 0.01f)
            {
                sb.AppendLine($"PhaseLegitimacyClamp FAIL: oathkeeping should clamp to 0 (0 - 2 = -2 -> 0); got {enemyConv.Oathkeeping}");
                return false;
            }
            sb.AppendLine($"PhaseLegitimacyClamp PASS: legitimacy clamps to 0, oathkeeping clamps to 0");
            return true;
        }
    }
}
#endif
