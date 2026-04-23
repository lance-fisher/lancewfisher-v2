using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Mirrors active dynasty political cooldowns and shocks into a capped HUD
    /// tray on each faction root. The tray shows the soonest-expiring four
    /// active events for the player-facing political state panel.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct PoliticalEventsTrayHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 1f;
        private const int MaxDisplayedEvents = 4;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (faction, entity) in
                SystemAPI.Query<RefRO<FactionComponent>>().WithEntityAccess())
            {
                if (!entityManager.HasBuffer<PoliticalEventsTrayHUDComponent>(entity))
                {
                    ecb.AddBuffer<PoliticalEventsTrayHUDComponent>(entity);
                }

                if (!entityManager.HasComponent<PoliticalEventsTrayHUDStateComponent>(entity))
                {
                    ecb.AddComponent(entity, new PoliticalEventsTrayHUDStateComponent
                    {
                        FactionId = faction.ValueRO.FactionId,
                        LastRefreshInWorldDays = float.NaN,
                        ActiveEventCount = 0,
                    });
                }
            }

            ecb.Playback(entityManager);

            foreach (var (faction, trayStateRw, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRW<PoliticalEventsTrayHUDStateComponent>>().WithEntityAccess())
            {
                ref var trayState = ref trayStateRw.ValueRW;
                if (!float.IsNaN(trayState.LastRefreshInWorldDays) &&
                    inWorldDays - trayState.LastRefreshInWorldDays < RefreshCadenceInWorldDays)
                {
                    continue;
                }

                trayState.FactionId = faction.ValueRO.FactionId;
                trayState.LastRefreshInWorldDays = inWorldDays;
                trayState.ActiveEventCount = 0;

                DynamicBuffer<PoliticalEventsTrayHUDComponent> tray =
                    entityManager.GetBuffer<PoliticalEventsTrayHUDComponent>(entity);
                tray.Clear();

                if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(entity))
                {
                    continue;
                }

                DynamicBuffer<DynastyPoliticalEventComponent> events =
                    entityManager.GetBuffer<DynastyPoliticalEventComponent>(entity);
                for (int i = 0; i < events.Length; i++)
                {
                    if (events[i].ExpiresAtInWorldDays <= inWorldDays)
                    {
                        continue;
                    }

                    trayState.ActiveEventCount++;
                    InsertEvent(
                        tray,
                        new PoliticalEventsTrayHUDComponent
                        {
                            EventType = events[i].EventType,
                            EventLabel = ResolveEventLabel(events[i].EventType),
                            RemainingInWorldDays = math.max(0f, events[i].ExpiresAtInWorldDays - inWorldDays),
                        });
                }
            }
        }

        private static void InsertEvent(
            DynamicBuffer<PoliticalEventsTrayHUDComponent> tray,
            in PoliticalEventsTrayHUDComponent entry)
        {
            int insertIndex = tray.Length;
            for (int i = 0; i < tray.Length; i++)
            {
                if (entry.RemainingInWorldDays < tray[i].RemainingInWorldDays)
                {
                    insertIndex = i;
                    break;
                }
            }

            tray.Add(default);
            for (int i = tray.Length - 1; i > insertIndex; i--)
            {
                tray[i] = tray[i - 1];
            }

            tray[insertIndex] = entry;
            if (tray.Length > MaxDisplayedEvents)
            {
                tray.RemoveAt(tray.Length - 1);
            }
        }

        private static FixedString64Bytes ResolveEventLabel(FixedString32Bytes eventType)
        {
            if (eventType.Equals(DynastyPoliticalEventTypes.CovenantTestCooldown))
            {
                return new FixedString64Bytes("Covenant Test Cooldown");
            }

            if (eventType.Equals(DynastyPoliticalEventTypes.DivineRightFailedCooldown))
            {
                return new FixedString64Bytes("Divine Right Cooldown");
            }

            if (eventType.Equals(DynastyPoliticalEventTypes.SuccessionShock))
            {
                return new FixedString64Bytes("Succession Shock");
            }

            var label = new FixedString64Bytes();
            label.Append(eventType);
            return label;
        }
    }
}
