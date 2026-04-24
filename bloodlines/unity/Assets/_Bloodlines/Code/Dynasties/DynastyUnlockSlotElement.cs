using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// DynamicBuffer element: one unlock slot per entry on a faction entity.
    /// Populated by DynastyProgressionUnlockSystem when a tier is reached.
    /// Activated by the player (or AI) writing IsActive = true on the desired slot.
    /// Only one slot per UnlockTypeId should be active at a time for balance.
    ///
    /// Browser equivalent: absent -- implemented from canonical progression design.
    /// </summary>
    public struct DynastyUnlockSlotElement : IBufferElementData
    {
        /// Sequential index within this faction's unlock buffer.
        public byte SlotIndex;

        /// 0=SpecialUnitSwap, 1=ResourceBonus, 2=DiplomacyBonus, 3=CombatBonus.
        public byte UnlockTypeId;

        /// Target identifier. For SpecialUnitSwap: the UnitRole ordinal of the
        /// unit to swap in. For others: a resource type or bonus profile id.
        public int UnlockTargetId;

        /// The tier at which this slot was granted.
        public byte GrantedAtTier;

        /// Whether this slot is currently activated. Only one SpecialUnitSwap slot
        /// should be active per faction to preserve multiplayer parity.
        public bool IsActive;
    }

    public enum DynastyUnlockType : byte
    {
        SpecialUnitSwap = 0,
        ResourceBonus   = 1,
        DiplomacyBonus  = 2,
        CombatBonus     = 3,
    }
}
