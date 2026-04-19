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
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 16 narrative message bridge. Six
    /// phases cover direct-push primitives, lazy singleton creation,
    /// multiple-push accumulation, and end-to-end wire-up through
    /// PactBreakSystem (the first system to produce messages via the
    /// bridge):
    ///
    ///   PhaseLazySingleton: call Push on an empty world. Singleton
    ///     is lazy-created; buffer contains one entry with the pushed
    ///     text and tone.
    ///   PhaseMultiplePushes: three Pushes accumulate three buffer
    ///     entries in push order.
    ///   PhaseCreatedAtInWorldDays: DualClock singleton seeded at
    ///     day 42; pushed entry carries CreatedAtInWorldDays = 42.
    ///   PhasePactBreakEarlyMessage: PactBreakSystem breaks a pact
    ///     before minimumExpiresAtInWorldDays; narrative buffer carries
    ///     a message containing "early breach" substring and Warn tone
    ///     because breaker is "player".
    ///   PhasePactBreakLateMessage: PactBreakSystem breaks a pact past
    ///     minimumExpiresAtInWorldDays; narrative buffer carries a
    ///     message containing "Hostility resumes" and Info tone because
    ///     breaker is "enemy".
    ///   PhaseNoMessageOnBadPact: PactBreakSystem processes a break
    ///     request that does not match any pact; no message is pushed.
    ///
    /// Browser reference: simulation.js pushMessage call sites; sub-slice
    /// 16 ports the minimal infrastructure. PactBreakSystem is the
    /// first consumer to go through this path.
    /// Artifact: artifacts/unity-narrative-message-bridge-smoke.log.
    /// </summary>
    public static class BloodlinesNarrativeMessageBridgeSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-narrative-message-bridge-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Narrative Message Bridge Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchNarrativeMessageBridgeSmokeValidation() =>
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
                message = "Narrative message bridge smoke errored: " + e;
            }

            string artifact = "BLOODLINES_NARRATIVE_MESSAGE_BRIDGE_SMOKE " +
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
            ok &= RunPhaseLazySingleton(sb);
            ok &= RunPhaseMultiplePushes(sb);
            ok &= RunPhaseCreatedAtInWorldDays(sb);
            ok &= RunPhasePactBreakEarlyMessage(sb);
            ok &= RunPhasePactBreakLateMessage(sb);
            ok &= RunPhaseNoMessageOnBadPact(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

        private static void SetupPactBreakGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<PactBreakSystem>());
            sg.SortSystems();
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
            EntityManager em, string factionId, float legitimacy, float oathkeeping)
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
            EntityManager em, string pactId, string a, string b,
            float startedAt, float minimumExpiry)
        {
            var e = em.CreateEntity(typeof(PactComponent));
            em.SetComponentData(e, new PactComponent
            {
                PactId                       = new FixedString64Bytes(pactId),
                FactionAId                   = new FixedString32Bytes(a),
                FactionBId                   = new FixedString32Bytes(b),
                StartedAtInWorldDays         = startedAt,
                MinimumExpiresAtInWorldDays  = minimumExpiry,
                Broken                       = false,
            });
            return e;
        }

        private static void SeedBreakRequest(
            EntityManager em, string pactId, string requester)
        {
            var e = em.CreateEntity(typeof(PactBreakRequestComponent));
            em.SetComponentData(e, new PactBreakRequestComponent
            {
                PactId              = new FixedString64Bytes(pactId),
                RequestingFactionId = new FixedString32Bytes(requester),
            });
        }

        private static bool TryGetSingletonBuffer(
            EntityManager em, out DynamicBuffer<NarrativeMessageElement> buffer)
        {
            buffer = default;
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<NarrativeMessageSingleton>());
            if (q.IsEmpty) { q.Dispose(); return false; }
            var singleton = q.GetSingletonEntity();
            q.Dispose();
            buffer = em.GetBuffer<NarrativeMessageElement>(singleton);
            return true;
        }

        private static bool MessageContains(
            FixedString128Bytes text, string substring)
        {
            // FixedString128Bytes does not have a Contains helper; convert
            // to managed string for substring matching in the test only.
            return text.ToString().Contains(substring);
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseLazySingleton(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-lazy-singleton");
            var em = world.EntityManager;

            NarrativeMessageBridge.Push(em,
                new FixedString128Bytes("hello world"),
                NarrativeMessageTone.Info);

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseLazySingleton FAIL: singleton not created");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseLazySingleton FAIL: expected 1 buffer entry, got {buffer.Length}");
                return false;
            }
            if (!MessageContains(buffer[0].Text, "hello world"))
            {
                sb.AppendLine($"PhaseLazySingleton FAIL: expected text 'hello world', got '{buffer[0].Text}'");
                return false;
            }
            if (buffer[0].Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhaseLazySingleton FAIL: expected tone Info, got {buffer[0].Tone}");
                return false;
            }
            sb.AppendLine($"PhaseLazySingleton PASS: singleton lazy-created, entry count=1, text matches, tone=Info");
            return true;
        }

        private static bool RunPhaseMultiplePushes(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-multiple-pushes");
            var em = world.EntityManager;

            NarrativeMessageBridge.Push(em, new FixedString128Bytes("first"),  NarrativeMessageTone.Info);
            NarrativeMessageBridge.Push(em, new FixedString128Bytes("second"), NarrativeMessageTone.Good);
            NarrativeMessageBridge.Push(em, new FixedString128Bytes("third"),  NarrativeMessageTone.Warn);

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseMultiplePushes FAIL: singleton not created");
                return false;
            }
            if (buffer.Length != 3)
            {
                sb.AppendLine($"PhaseMultiplePushes FAIL: expected 3 entries, got {buffer.Length}");
                return false;
            }
            // Expect append-order preservation.
            if (!MessageContains(buffer[0].Text, "first")  ||
                !MessageContains(buffer[1].Text, "second") ||
                !MessageContains(buffer[2].Text, "third"))
            {
                sb.AppendLine($"PhaseMultiplePushes FAIL: buffer order mismatch");
                return false;
            }
            if (buffer[0].Tone != NarrativeMessageTone.Info ||
                buffer[1].Tone != NarrativeMessageTone.Good ||
                buffer[2].Tone != NarrativeMessageTone.Warn)
            {
                sb.AppendLine($"PhaseMultiplePushes FAIL: tone sequence Info,Good,Warn mismatch");
                return false;
            }
            sb.AppendLine($"PhaseMultiplePushes PASS: 3 entries in append order with correct tones");
            return true;
        }

        private static bool RunPhaseCreatedAtInWorldDays(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-timestamp");
            var em = world.EntityManager;

            SeedDualClock(em, inWorldDays: 42f);
            NarrativeMessageBridge.Push(em,
                new FixedString128Bytes("timed"),
                NarrativeMessageTone.Good);

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhaseCreatedAtInWorldDays FAIL: singleton not created");
                return false;
            }
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseCreatedAtInWorldDays FAIL: expected 1 entry, got {buffer.Length}");
                return false;
            }
            if (buffer[0].CreatedAtInWorldDays < 41.99f || buffer[0].CreatedAtInWorldDays > 42.01f)
            {
                sb.AppendLine($"PhaseCreatedAtInWorldDays FAIL: expected CreatedAtInWorldDays=42, got {buffer[0].CreatedAtInWorldDays}");
                return false;
            }
            sb.AppendLine($"PhaseCreatedAtInWorldDays PASS: pushed entry stamped at day 42");
            return true;
        }

        private static bool RunPhasePactBreakEarlyMessage(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-pact-break-early");
            var em = world.EntityManager;
            SetupPactBreakGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            SeedFaction(em, "player", legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "enemy",  legitimacy: 60f, oathkeeping: 4f);
            SeedPact(em, "nap-early-msg", "player", "enemy",
                startedAt: 30f, minimumExpiry: 210f);
            SeedBreakRequest(em, "nap-early-msg", "player");

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhasePactBreakEarlyMessage FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length < 1)
            {
                sb.AppendLine($"PhasePactBreakEarlyMessage FAIL: expected at least 1 message, got {buffer.Length}");
                return false;
            }
            var last = buffer[buffer.Length - 1];
            if (!MessageContains(last.Text, "early breach"))
            {
                sb.AppendLine($"PhasePactBreakEarlyMessage FAIL: message missing 'early breach'; got '{last.Text}'");
                return false;
            }
            if (last.Tone != NarrativeMessageTone.Warn)
            {
                sb.AppendLine($"PhasePactBreakEarlyMessage FAIL: player-breaker tone should be Warn, got {last.Tone}");
                return false;
            }
            sb.AppendLine($"PhasePactBreakEarlyMessage PASS: early-break message contains 'early breach', tone=Warn for player breaker");
            return true;
        }

        private static bool RunPhasePactBreakLateMessage(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-pact-break-late");
            var em = world.EntityManager;
            SetupPactBreakGroup(world);
            SeedDualClock(em, inWorldDays: 250f);
            SeedFaction(em, "player", legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "enemy",  legitimacy: 60f, oathkeeping: 4f);
            SeedPact(em, "nap-late-msg", "enemy", "player",
                startedAt: 10f, minimumExpiry: 190f);
            SeedBreakRequest(em, "nap-late-msg", "enemy");

            world.Update();

            if (!TryGetSingletonBuffer(em, out var buffer))
            {
                sb.AppendLine($"PhasePactBreakLateMessage FAIL: no narrative singleton");
                return false;
            }
            if (buffer.Length < 1)
            {
                sb.AppendLine($"PhasePactBreakLateMessage FAIL: expected at least 1 message, got {buffer.Length}");
                return false;
            }
            var last = buffer[buffer.Length - 1];
            if (!MessageContains(last.Text, "Hostility resumes"))
            {
                sb.AppendLine($"PhasePactBreakLateMessage FAIL: message missing 'Hostility resumes'; got '{last.Text}'");
                return false;
            }
            if (last.Tone != NarrativeMessageTone.Info)
            {
                sb.AppendLine($"PhasePactBreakLateMessage FAIL: non-player breaker tone should be Info, got {last.Tone}");
                return false;
            }
            sb.AppendLine($"PhasePactBreakLateMessage PASS: late-break message contains 'Hostility resumes', tone=Info for enemy breaker");
            return true;
        }

        private static bool RunPhaseNoMessageOnBadPact(System.Text.StringBuilder sb)
        {
            using var world = new World("narrative-pact-break-bad");
            var em = world.EntityManager;
            SetupPactBreakGroup(world);
            SeedDualClock(em, inWorldDays: 50f);
            SeedFaction(em, "player", legitimacy: 70f, oathkeeping: 5f);
            SeedFaction(em, "enemy",  legitimacy: 60f, oathkeeping: 4f);
            // No PactComponent seeded.
            SeedBreakRequest(em, "nap-missing", "player");

            world.Update();

            int count = NarrativeMessageBridge.Count(em);
            if (count != 0)
            {
                sb.AppendLine($"PhaseNoMessageOnBadPact FAIL: expected 0 messages (break short-circuits on missing pact), got {count}");
                return false;
            }
            sb.AppendLine($"PhaseNoMessageOnBadPact PASS: missing pact short-circuits without pushing a message");
            return true;
        }
    }
}
#endif
