using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Minimal AI-to-UI narrative message push surface. Any system can
    /// call `Push` to append a message entry to the global narrative
    /// message buffer without needing to know where the singleton lives
    /// or how to find the current in-world day.
    ///
    /// The helper mirrors the browser's `pushMessage(state, text, tone)`
    /// API (simulation.js, many call sites). Unity's equivalent writes a
    /// buffer element onto a singleton entity that carries the
    /// NarrativeMessageSingleton tag. The singleton is lazy-created on
    /// first push so callers do not need to coordinate bootstrap.
    ///
    /// Designed for simple producer ergonomics:
    ///   NarrativeMessageBridge.Push(em, "Marriage ...", NarrativeMessageTone.Good);
    ///
    /// Consumer wire-up is out of scope for sub-slice 16. Future slices
    /// can add a HUD reader, an in-game log panel, a drain system, or a
    /// TTL-based eviction system. Until then, entries accumulate on the
    /// buffer for inspection by smoke validators and debug tools.
    ///
    /// No per-faction scoping yet. A future multiplayer slice may
    /// introduce per-faction buffers so each player sees only their own
    /// messages; the bridge API is kept simple so it can evolve into a
    /// faction-aware helper without breaking existing callers (the
    /// current call-site signature would stay the same; the bridge
    /// internals would route to the correct faction buffer).
    /// </summary>
    public static class NarrativeMessageBridge
    {
        public const float DefaultTtlSeconds = 7f;

        /// <summary>
        /// Append a narrative message to the global buffer. Lazy-creates
        /// the NarrativeMessageSingleton entity if missing. Reads the
        /// current in-world day from the DualClockComponent singleton;
        /// if the clock is not yet seeded, stamps 0 for `CreatedAtInWorldDays`.
        /// </summary>
        public static void Push(
            EntityManager em,
            FixedString128Bytes text,
            NarrativeMessageTone tone,
            float ttlSeconds = DefaultTtlSeconds)
        {
            var singleton = GetOrCreateSingleton(em);
            var buffer = em.GetBuffer<NarrativeMessageElement>(singleton);
            buffer.Add(new NarrativeMessageElement
            {
                Text                 = text,
                Tone                 = tone,
                CreatedAtInWorldDays = GetInWorldDays(em),
                Ttl                  = ttlSeconds,
            });
        }

        /// <summary>
        /// Returns the count of narrative messages currently on the
        /// buffer. Returns 0 if no singleton exists yet.
        /// </summary>
        public static int Count(EntityManager em)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<NarrativeMessageSingleton>(),
                ComponentType.ReadOnly<NarrativeMessageElement>());
            if (q.IsEmpty) { q.Dispose(); return 0; }
            var singleton = q.GetSingletonEntity();
            q.Dispose();
            return em.GetBuffer<NarrativeMessageElement>(singleton).Length;
        }

        // ------------------------------------------------------------------ helpers

        private static Entity GetOrCreateSingleton(EntityManager em)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<NarrativeMessageSingleton>());
            if (!q.IsEmpty)
            {
                var existing = q.GetSingletonEntity();
                q.Dispose();
                return existing;
            }
            q.Dispose();

            var entity = em.CreateEntity(
                typeof(NarrativeMessageSingleton),
                typeof(NarrativeMessageElement));
            return entity;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
