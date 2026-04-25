using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Presentation lane pump for ECS audio events.
    /// It prunes aged events and enforces dispatch buffer capacity.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct AudioEventDispatchSystem : ISystem
    {
        public const int MaxPendingEvents = 32;
        public const float EventTtlInWorldDays = 1f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AudioEventDispatchComponent>();
            state.RequireForUpdate<AudioEventElement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var dispatchQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<AudioEventDispatchComponent>(),
                ComponentType.ReadOnly<AudioEventElement>());

            if (dispatchQuery.IsEmpty)
            {
                dispatchQuery.Dispose();
                return;
            }

            var dispatchEntity = dispatchQuery.GetSingletonEntity();
            var dispatch = entityManager.GetComponentData<AudioEventDispatchComponent>(dispatchEntity);
            var events = entityManager.GetBuffer<AudioEventElement>(dispatchEntity);

            float now = GetInWorldDays(entityManager);
            PruneExpired(ref events, now);
            EnforceCapacity(ref events, MaxPendingEvents);

            dispatch.PendingEventCount = events.Length;
            dispatch.LastFlushedAtInWorldDays = now;
            entityManager.SetComponentData(dispatchEntity, dispatch);

            dispatchQuery.Dispose();
        }

        private static void PruneExpired(ref DynamicBuffer<AudioEventElement> events, float now)
        {
            if (events.Length == 0)
            {
                return;
            }

            for (int i = events.Length - 1; i >= 0; i--)
            {
                if (now - events[i].FiredAtInWorldDays <= EventTtlInWorldDays)
                {
                    continue;
                }

                events.RemoveAtSwapBack(i);
            }
        }

        private static void EnforceCapacity(ref DynamicBuffer<AudioEventElement> events, int maxPending)
        {
            int overflow = events.Length - maxPending;
            if (overflow <= 0)
            {
                return;
            }

            for (int drop = 0; drop < overflow; drop++)
            {
                int dropIndex = -1;
                byte lowestPriority = byte.MaxValue;

                for (int i = 0; i < events.Length; i++)
                {
                    if (events[i].Priority >= lowestPriority)
                    {
                        continue;
                    }

                    lowestPriority = events[i].Priority;
                    dropIndex = i;
                }

                if (dropIndex < 0)
                {
                    return;
                }

                events.RemoveAtSwapBack(dropIndex);
            }
        }

        private static float GetInWorldDays(EntityManager entityManager)
        {
            var q = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty)
            {
                q.Dispose();
                return 0f;
            }

            float now = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return now;
        }
    }
}

