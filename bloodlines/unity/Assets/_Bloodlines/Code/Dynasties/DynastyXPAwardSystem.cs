using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Consumes DynastyXPAwardRequestComponent one-shots placed on faction entities
    /// after a match. Adds XPAmount to DynastyProgressionComponent.AccumulatedXP,
    /// advances CurrentTier at canonical thresholds, and fires a
    /// TierUnlockNotificationComponent for each tier crossed.
    ///
    /// DynastyProgressionComponent is added lazily if not present when the first
    /// award request arrives.
    ///
    /// Browser equivalent: absent -- implemented from canonical progression design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DynastyXPAwardSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyXPAwardRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var requestQuery = em.CreateEntityQuery(
                Unity.Collections.ComponentType.ReadOnly<FactionComponent>(),
                Unity.Collections.ComponentType.ReadOnly<DynastyXPAwardRequestComponent>());
            using var requestEntities = requestQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factionIds = requestQuery.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var requests = requestQuery.ToComponentDataArray<DynastyXPAwardRequestComponent>(Unity.Collections.Allocator.Temp);
            requestQuery.Dispose();

            var toNotify = new List<(Entity entity, byte newTier)>(4);

            for (int i = 0; i < requestEntities.Length; i++)
            {
                Entity factionEntity = requestEntities[i];
                float awardAmount = math.max(0f, requests[i].XPAmount);

                if (!em.HasComponent<DynastyProgressionComponent>(factionEntity))
                {
                    em.AddComponentData(factionEntity, new DynastyProgressionComponent
                    {
                        AccumulatedXP = 0f,
                        CurrentTier = 0,
                        LastMatchXPAward = 0f,
                        TierUnlocksPending = 0,
                    });
                }

                var progression = em.GetComponentData<DynastyProgressionComponent>(factionEntity);
                byte oldTier = progression.CurrentTier;
                float newXP = progression.AccumulatedXP + awardAmount;
                byte newTier = DynastyProgressionCanon.TierForXP(newXP);
                newTier = (byte)math.min(newTier, DynastyProgressionCanon.MaxTier);

                progression.AccumulatedXP = newXP;
                progression.LastMatchXPAward = awardAmount;
                progression.CurrentTier = newTier;

                if (newTier > oldTier)
                {
                    progression.TierUnlocksPending += (byte)(newTier - oldTier);
                }

                em.SetComponentData(factionEntity, progression);
                em.RemoveComponent<DynastyXPAwardRequestComponent>(factionEntity);

                // Queue unlock notifications for each newly reached tier.
                for (byte t = (byte)(oldTier + 1); t <= newTier; t++)
                {
                    toNotify.Add((factionEntity, t));
                }
            }

            // Fire TierUnlockNotificationComponent for the highest newly reached tier.
            // If multiple tiers were crossed, we fire for the final tier only (the unlock
            // system grants slots per tier sequentially when it consumes the notification).
            for (int i = 0; i < toNotify.Count; i++)
            {
                var (entity, tier) = toNotify[i];
                if (em.HasComponent<TierUnlockNotificationComponent>(entity))
                {
                    // Preserve the highest tier notification if multiple exist.
                    var existing = em.GetComponentData<TierUnlockNotificationComponent>(entity);
                    if (tier > existing.NewTier)
                    {
                        em.SetComponentData(entity, new TierUnlockNotificationComponent { NewTier = tier });
                    }
                }
                else
                {
                    em.AddComponentData(entity, new TierUnlockNotificationComponent { NewTier = tier });
                }
            }
        }
    }
}
