using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetDynastyProgression(
            string factionId,
            out float accumulatedXP,
            out byte currentTier,
            out float lastMatchXPAward)
        {
            accumulatedXP = 0f;
            currentTier = 0;
            lastMatchXPAward = 0f;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            Entity factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<DynastyProgressionComponent>(factionEntity))
            {
                return false;
            }

            var prog = entityManager.GetComponentData<DynastyProgressionComponent>(factionEntity);
            accumulatedXP = prog.AccumulatedXP;
            currentTier = prog.CurrentTier;
            lastMatchXPAward = prog.LastMatchXPAward;
            return true;
        }

        public bool TryDebugAwardDynastyXP(string factionId, float xpAmount, byte placement)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            Entity factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            if (entityManager.HasComponent<DynastyXPAwardRequestComponent>(factionEntity))
            {
                entityManager.SetComponentData(factionEntity, new DynastyXPAwardRequestComponent
                {
                    XPAmount = xpAmount,
                    MatchPlacement = placement,
                });
            }
            else
            {
                entityManager.AddComponentData(factionEntity, new DynastyXPAwardRequestComponent
                {
                    XPAmount = xpAmount,
                    MatchPlacement = placement,
                });
            }

            return true;
        }

        public bool TryDebugGetUnlockSlots(string factionId, out int slotCount, out int activeCount)
        {
            slotCount = 0;
            activeCount = 0;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            Entity factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyUnlockSlotElement>(factionEntity))
            {
                return false;
            }

            var buffer = entityManager.GetBuffer<DynastyUnlockSlotElement>(factionEntity, true);
            slotCount = buffer.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].IsActive)
                {
                    activeCount++;
                }
            }

            return true;
        }

        public bool TryDebugActivateUnlockSlot(string factionId, byte slotIndex)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            Entity factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyUnlockSlotElement>(factionEntity))
            {
                return false;
            }

            var buffer = entityManager.GetBuffer<DynastyUnlockSlotElement>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                var slot = buffer[i];
                if (slot.SlotIndex == slotIndex)
                {
                    slot.IsActive = true;
                    buffer[i] = slot;
                    return true;
                }
            }

            return false;
        }

    }
}
