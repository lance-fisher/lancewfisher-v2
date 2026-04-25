using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Audio
{
    /// <summary>
    /// Shared helper for writing UI/audio-layer events into the canonical ECS event
    /// dispatch buffer. No audio playback occurs here; this layer remains a
    /// transport seam for future presentation/audio systems.
    /// </summary>
    public static class AudioEventBridge
    {
        public const int DefaultCapacity = 32;
        public const float DefaultTtlInWorldDays = 1f;

        public static void Push(
            EntityManager em,
            FixedString64Bytes eventId,
            float3 position,
            int factionId,
            byte priority)
        {
            var singleton = GetOrCreateSingleton(em);
            var buffer = em.GetBuffer<AudioEventElement>(singleton);
            buffer.Add(new AudioEventElement
            {
                EventId = eventId,
                SourcePosition = position,
                FactionId = factionId,
                FiredAtInWorldDays = GetInWorldDays(em),
                Priority = priority,
            });
        }

        private static Entity GetOrCreateSingleton(EntityManager em)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<AudioEventDispatchComponent>());
            if (!q.IsEmpty)
            {
                var existing = q.GetSingletonEntity();
                q.Dispose();
                return existing;
            }

            q.Dispose();
            return em.CreateEntity(typeof(AudioEventDispatchComponent), typeof(AudioEventElement));
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty)
            {
                q.Dispose();
                return 0f;
            }

            float value = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return value;
        }
    }
}

