using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction civilizational loyalty. Canonical 0..100 range.
    ///
    /// Browser runtime equivalent: faction.loyalty. Canonical thresholds are in
    /// data/realm-conditions.json. Starting value defaults to 70 on faction spawn
    /// unless the map seed explicitly overrides.
    ///
    /// Loyalty is consumed by famine, water crisis, cap pressure, cruelty drift,
    /// tribal raids, dossier-backed sabotage, and other late-game pressure flows.
    /// It is restored by stability surplus, conviction band (Apex Moral), covenant
    /// endorsement, and governor stabilization.
    ///
    /// Effects that consume loyalty (recruitment hesitation, defection risk,
    /// breakaway-march gating, succession-crisis pressure multipliers) remain out
    /// of scope for this first loyalty slice and will be layered in as those
    /// systems come online.
    /// </summary>
    public struct FactionLoyaltyComponent : IComponentData
    {
        public float Current;
        public float Max;
        public float Floor;
    }
}
