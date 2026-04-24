using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Consumes TierUnlockNotificationComponent on faction entities.
    /// For each tier newly reached, writes one DynastyUnlockSlotElement into the
    /// faction's buffer with UnlockTypeId cycling through the four bonus types.
    /// Slots are added as pending (IsActive = false); the player or AI activates
    /// exactly one per type to avoid power stacking.
    ///
    /// DynamicBuffer<DynastyUnlockSlotElement> is added lazily if absent.
    ///
    /// Browser equivalent: absent -- implemented from canonical progression design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DynastyXPAwardSystem))]
    public partial struct DynastyProgressionUnlockSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TierUnlockNotificationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var notifyQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TierUnlockNotificationComponent>(),
                ComponentType.ReadOnly<DynastyProgressionComponent>());
            using var notifyEntities = notifyQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var notifications = notifyQuery.ToComponentDataArray<TierUnlockNotificationComponent>(Unity.Collections.Allocator.Temp);
            using var progressions = notifyQuery.ToComponentDataArray<DynastyProgressionComponent>(Unity.Collections.Allocator.Temp);
            notifyQuery.Dispose();

            var toRemoveNotify = new List<Entity>(4);

            for (int i = 0; i < notifyEntities.Length; i++)
            {
                Entity factionEntity = notifyEntities[i];
                byte newTier = notifications[i].NewTier;
                var progression = progressions[i];

                if (!em.HasBuffer<DynastyUnlockSlotElement>(factionEntity))
                {
                    em.AddBuffer<DynastyUnlockSlotElement>(factionEntity);
                }

                var buffer = em.GetBuffer<DynastyUnlockSlotElement>(factionEntity);

                // Grant one slot per tier crossed since last recorded tier.
                // TierUnlocksPending tracks how many are due.
                byte pendingCount = progression.TierUnlocksPending;
                byte startTier = (byte)(newTier - pendingCount + 1);
                if (startTier < 1) startTier = 1;

                for (byte t = startTier; t <= newTier; t++)
                {
                    byte slotIndex = (byte)buffer.Length;
                    // Cycle unlock type through the four bonus categories by tier.
                    byte unlockType = (byte)((t - 1) % 4);

                    buffer.Add(new DynastyUnlockSlotElement
                    {
                        SlotIndex = slotIndex,
                        UnlockTypeId = unlockType,
                        UnlockTargetId = ResolveDefaultTarget(unlockType, t),
                        GrantedAtTier = t,
                        IsActive = false,
                    });
                }

                // Clear pending counter now that slots are written.
                var updatedProgression = progression;
                updatedProgression.TierUnlocksPending = 0;
                em.SetComponentData(factionEntity, updatedProgression);

                toRemoveNotify.Add(factionEntity);
            }

            for (int i = 0; i < toRemoveNotify.Count; i++)
            {
                em.RemoveComponent<TierUnlockNotificationComponent>(toRemoveNotify[i]);
            }
        }

        private static int ResolveDefaultTarget(byte unlockType, byte tier)
        {
            // Default targets cycle by tier so each tier grants a distinct option.
            // SpecialUnitSwap: UnitRole ordinal (Melee=1, Ranged=3, UniqueMelee=4, LightCavalry=5).
            // Others: tier index as a placeholder bonus profile id.
            return unlockType switch
            {
                (byte)DynastyUnlockType.SpecialUnitSwap => tier switch
                {
                    1 => (int)UnitRole.Ranged,
                    2 => (int)UnitRole.UniqueMelee,
                    3 => (int)UnitRole.LightCavalry,
                    _ => (int)UnitRole.Melee,
                },
                _ => tier,
            };
        }
    }
}
