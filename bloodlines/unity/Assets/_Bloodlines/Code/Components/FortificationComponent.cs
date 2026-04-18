using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical settlement-side fortification profile.
    /// Browser reference: simulation.js advanceFortificationTier (11227) and the
    /// 2026-04-14 fortification canon constants block (188-238).
    /// </summary>
    public struct FortificationComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public int Tier;
        public int Ceiling;
        public float EcosystemRadiusTiles;
        public float AuraRadiusTiles;
        public float ThreatRadiusTiles;
        public float ReserveRadiusTiles;
        public float KeepPresenceRadiusTiles;
        public FixedString32Bytes FaithWardId;
        public float FaithWardSightBonusTiles;
        public float FaithWardDefenderAttackMultiplier;
        public float FaithWardReserveHealMultiplier;
        public float FaithWardReserveMusterMultiplier;
        public float FaithWardLoyaltyProtectionMultiplier;
        public float FaithWardEnemySpeedMultiplier;
        public bool FaithWardSurgeActive;
    }
}
