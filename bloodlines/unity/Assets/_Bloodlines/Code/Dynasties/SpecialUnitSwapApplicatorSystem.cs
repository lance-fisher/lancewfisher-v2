using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// During the Initializing phase, reads active SpecialUnitSwap unlock slots from each
    /// faction's DynastyUnlockSlotElement buffer and records the swap in a
    /// FactionSpecialUnitSwapComponent so unit production systems can substitute the
    /// target unit role when spawning dynasty-specific special units.
    ///
    /// Only one SpecialUnitSwap slot should be active per faction (enforced here by
    /// taking the last active slot found). Swaps are parity-safe: they exchange one unit
    /// role for another within the same house's roster rather than adding a new unit.
    ///
    /// Browser equivalent: absent -- implemented from canonical progression design.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpecialUnitSwapApplicatorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyProgressionComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyProgressionComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                if (!em.HasBuffer<DynastyUnlockSlotElement>(factionEntity))
                {
                    continue;
                }

                var buffer = em.GetBuffer<DynastyUnlockSlotElement>(factionEntity, true);
                bool hasActiveSwap = false;
                int swapTargetId = -1;

                for (int j = 0; j < buffer.Length; j++)
                {
                    var slot = buffer[j];
                    if (slot.UnlockTypeId == (byte)DynastyUnlockType.SpecialUnitSwap && slot.IsActive)
                    {
                        hasActiveSwap = true;
                        swapTargetId = slot.UnlockTargetId;
                    }
                }

                if (hasActiveSwap)
                {
                    var swap = new FactionSpecialUnitSwapComponent
                    {
                        SwapTargetUnitRole = (byte)swapTargetId,
                        IsActive = true,
                    };

                    if (em.HasComponent<FactionSpecialUnitSwapComponent>(factionEntity))
                    {
                        em.SetComponentData(factionEntity, swap);
                    }
                    else
                    {
                        em.AddComponentData(factionEntity, swap);
                    }
                }
                else if (em.HasComponent<FactionSpecialUnitSwapComponent>(factionEntity))
                {
                    em.RemoveComponent<FactionSpecialUnitSwapComponent>(factionEntity);
                }
            }
        }
    }

    /// <summary>
    /// Per-faction record of the currently active special-unit-swap unlock.
    /// Read by production systems that spawn dynasty-specific special units.
    /// </summary>
    public struct FactionSpecialUnitSwapComponent : IComponentData
    {
        /// UnitRole ordinal of the unit role to substitute when spawning a dynasty special unit.
        public byte SwapTargetUnitRole;

        /// Whether a swap is currently active for this faction.
        public bool IsActive;
    }
}
