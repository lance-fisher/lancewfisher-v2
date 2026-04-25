namespace Bloodlines.WorldPressure
{
    /// <summary>
    /// Centralized canonical constants for the Territorial Governance victory
    /// path, the alliance-threshold coalition pressure seam, and the
    /// governance-related world pressure scores.
    ///
    /// Browser source: src/game/core/simulation.js lines 143-169 (canonical
    /// TERRITORIAL_GOVERNANCE_* and GOVERNANCE_ALLIANCE_* constants).
    ///
    /// Hoisted from GovernanceCoalitionPressureSystem private constants per
    /// the recommendation in
    /// docs/migration/constant_parity_audit.md (2026-04-25). Other systems
    /// (recognition acceptance curve, world-pressure contribution, victory
    /// evaluator) can reference these without duplicating the value.
    ///
    /// One canonical gap remains: GovernanceAllianceLegitimacyPressurePerCycle
    /// is wired here for completeness but the consumer (legitimacy field on
    /// faction-level component) is deferred to the dynasty-core lane. When
    /// that lands, the legitimacy drain consumer can read this value
    /// directly.
    /// </summary>
    public static class GovernanceCanon
    {
        public const int TerritorialGovernanceMinStage = 5;
        public const float TerritorialGovernanceMinTerritoryShare = 0.35f;
        public const float TerritorialGovernanceLoyaltyThreshold = 80f;
        public const float TerritorialGovernanceVictoryLoyaltyThreshold = 90f;
        public const float TerritorialGovernanceBreakLoyaltyThreshold = 65f;
        public const float TerritorialGovernanceCourtLoyaltyThreshold = 72f;
        public const float TerritorialGovernanceLesserHouseLoyaltyThreshold = 25f;
        public const float TerritorialGovernanceSustainSeconds = 90f;
        public const float TerritorialGovernanceVictorySeconds = 120f;
        public const float TerritorialGovernanceAcceptanceThresholdPct = 65f;
        public const float TerritorialGovernanceAcceptanceAllianceThresholdPct = 60f;

        public const float GovernanceAllianceLoyaltyPressureBase = -1.5f;
        public const float GovernanceAllianceLegitimacyPressurePerCycle = 0.8f;
        public const float GovernanceAllianceAcceptanceDragPerHostile = 0.04f;
        public const float GovernanceAllianceCycleSeconds = 90f;

        public const int TerritorialGovernanceWorldPressureScore = 3;
        public const int TerritorialGovernanceRecognizedWorldPressureScore = 5;
        public const int TerritorialGovernanceThresholdWorldPressureScore = 6;
        public const int TerritorialGovernanceVictoryWorldPressureScore = 7;
    }
}
