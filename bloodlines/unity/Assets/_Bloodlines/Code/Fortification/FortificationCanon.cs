namespace Bloodlines.Fortification
{
    /// <summary>
    /// Direct port of the fortification / siege canon constants defined in
    /// simulation.js lines 188-238.
    /// </summary>
    public static class FortificationCanon
    {
        public const float EcosystemRadiusTiles = 9f;
        public const float AuraRadiusTiles = 10f;
        public const float ThreatRadiusTiles = 8f;
        public const float ReserveRadiusTiles = 12f;
        public const float TriageRadiusTiles = 2.4f;
        public const float KeepPresenceRadiusTiles = 6f;
        public const float ImminentEngagementWarningBufferTiles = 4f;
        public const float ImminentEngagementWatchtowerRadiusTiles = 14f;
        public const float ImminentEngagementMinSeconds = 10f;
        public const float ImminentEngagementMaxSeconds = 30f;
        public const float ImminentEngagementKeepBaseSeconds = 14f;
        public const float ImminentEngagementSettlementBaseSeconds = 11f;
        public const float ImminentEngagementReinforcementSurgeSeconds = 18f;
        public const float ReserveRetreatHealthRatio = 0.42f;
        public const float ReserveRecoveryHealthRatio = 0.82f;
        public const float ReserveTriageHealPerSecond = 5.5f;
        public const float ReserveMusterIntervalSeconds = 3.5f;
        public const float BreachSealingTickRateHz = 1f;
        public const float BreachSealingStoneCostPerBreach = 60f;
        public const float BreachSealingWorkerHoursPerBreach = 8f;
        public const float BreachSealingTier2StoneCostPerBreach = 90f;
        public const float BreachSealingTier2WorkerHoursPerBreach = 12f;
        public const float BreachSealingTier3StoneCostPerBreach = 135f;
        public const float BreachSealingTier3WorkerHoursPerBreach = 18f;
        public const float DestroyedCounterRecoveryTickRateHz = 1f;
        public const float DestroyedCounterRecoveryStoneCostPerSegment = 90f;
        public const float DestroyedCounterRecoveryWorkerHoursPerSegment = 14f;
        public const float DestroyedCounterRecoveryKeepMultiplier = 2f;
        public const float BloodAltarSurgeDurationSeconds = 18f;
        public const float BloodAltarSurgeCooldownSeconds = 34f;
        public const float RealmCycleDefaultSeconds = 90f;
        public const float SiegeSupportRefreshSeconds = 1.25f;
        public const float SiegeUnsuppliedAttackMultiplier = 0.84f;
        public const float SiegeUnsuppliedSpeedMultiplier = 0.88f;
        public const float SiegeUnsuppliedMessageIntervalSeconds = 10f;
        public const float SiegeResuppliedMessageIntervalSeconds = 12f;
        public const float ConvoyRecoveryDurationSeconds = 12f;
        public const float ConvoyEscortScreenRadius = 86f;
        public const int ConvoyEscortMinEscorts = 2;
        public const float FieldWaterLocalSupportRadius = 132f;
        public const float FieldWaterSettlementSupportRadius = 156f;
        public const float FieldWaterSupportDurationSeconds = 14f;
        public const float FieldWaterTransferIntervalSeconds = 4f;
        public const float FieldWaterTransferCost = 0.2f;
        public const float FieldWaterStrainPerSecond = 0.85f;
        public const float FieldWaterRecoveryPerSecond = 1.25f;
        public const float FieldWaterStrainThreshold = 6f;
        public const float FieldWaterCriticalThreshold = 12f;
        public const float FieldWaterStrainAttackMultiplier = 0.88f;
        public const float FieldWaterStrainSpeedMultiplier = 0.9f;
        public const float FieldWaterCriticalAttackMultiplier = 0.72f;
        public const float FieldWaterCriticalSpeedMultiplier = 0.78f;
        public const float FieldWaterMessageIntervalSeconds = 18f;
        public const float FieldWaterAttritionThresholdSeconds = 4f;
        public const float FieldWaterDesertionThresholdSeconds = 10f;
        public const float FieldWaterAttritionDamagePerSecond = 6f;
        public const float FieldWaterCriticalRecoveryPerSecond = 2.1f;
        public const float AssaultStrainThreshold = 6f;
        public const float AssaultStrainDecayPerSecond = 0.12f;
        public const float AssaultCohesionPenaltyDuration = 20f;
        public const float AssaultCohesionPenaltyMultiplier = 0.85f;

        public static float ResolveBreachSealingStoneCostPerBreach(int fortificationTier)
        {
            if (fortificationTier >= 3)
            {
                return BreachSealingTier3StoneCostPerBreach;
            }

            if (fortificationTier == 2)
            {
                return BreachSealingTier2StoneCostPerBreach;
            }

            return BreachSealingStoneCostPerBreach;
        }

        public static float ResolveBreachSealingWorkerHoursPerBreach(int fortificationTier)
        {
            if (fortificationTier >= 3)
            {
                return BreachSealingTier3WorkerHoursPerBreach;
            }

            if (fortificationTier == 2)
            {
                return BreachSealingTier2WorkerHoursPerBreach;
            }

            return BreachSealingWorkerHoursPerBreach;
        }
    }
}
