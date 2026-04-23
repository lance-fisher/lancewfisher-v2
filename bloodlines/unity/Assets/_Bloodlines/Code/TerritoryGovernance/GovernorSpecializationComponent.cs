using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.TerritoryGovernance
{
    /// <summary>
    /// Control-point governor specialization profile. The raw multiplier fields
    /// mirror the browser specialization table; consumer systems apply the
    /// canonical trickle/stabilization base bonuses where required.
    /// </summary>
    public struct GovernorSpecializationComponent : IComponentData
    {
        public FixedString64Bytes GovernorMemberId;
        public GovernorSpecializationId SpecializationId;
        public float ResourceTrickleMultiplier;
        public float StabilizationMultiplier;
        public float CaptureResistanceBonus;
        public float LoyaltyProtectionMultiplier;
        public float ReserveRegenMultiplier;
        public float HealRegenMultiplier;
    }
}
