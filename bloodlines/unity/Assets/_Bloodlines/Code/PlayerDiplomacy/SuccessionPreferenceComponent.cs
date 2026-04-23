using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    public struct SuccessionPreferenceComponent : IComponentData
    {
        public Entity PreferredHeirEntity;
        public FixedString64Bytes PreferredHeirMemberId;
        public float DesignationExpiresAtInWorldDays;
        public byte DesignationCostPaid;
    }

    internal static class SuccessionPreferenceRules
    {
        internal static readonly FixedString32Bytes PlayerFactionId = new("player");

        internal const float DesignationGoldCost = 50f;
        internal const float DesignationLegitimacyCost = 4f;
        internal const float DesignationDurationInWorldDays = 365f;

        internal static bool IsExpired(float expiresAtInWorldDays, float inWorldDays)
        {
            return math.isfinite(expiresAtInWorldDays) &&
                   inWorldDays > expiresAtInWorldDays;
        }
    }
}
