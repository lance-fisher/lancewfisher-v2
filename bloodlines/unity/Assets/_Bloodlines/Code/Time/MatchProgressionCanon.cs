namespace Bloodlines.GameTime
{
    /// <summary>
    /// Centralized canonical constants for the five-stage match progression and
    /// the Great Reckoning thresholds. Mirrors browser
    /// src/game/core/simulation.js GREAT_RECKONING_TRIGGER_SHARE,
    /// GREAT_RECKONING_RELEASE_SHARE, MATCH_STAGE_DEFINITIONS, and MATCH_PHASE_LABELS.
    ///
    /// Hoisted from MatchProgressionEvaluationSystem private constants per the
    /// recommendation in docs/migration/constant_parity_audit.md (2026-04-25).
    /// Other systems (HUD, victory evaluator, world pressure escalation) can
    /// reference these without duplicating the values.
    /// </summary>
    public static class MatchProgressionCanon
    {
        /// <summary>
        /// Browser GREAT_RECKONING_TRIGGER_SHARE = 0.7. The dominant kingdom
        /// share above which the Great Reckoning fires. Shared with the Stage 4
        /// dominantKingdomId targeting branch.
        /// </summary>
        public const float GreatReckoningTriggerShare = 0.7f;

        /// <summary>
        /// Browser GREAT_RECKONING_RELEASE_SHARE = 0.66. The share below which
        /// an active Great Reckoning releases. Sustain logic uses this value
        /// to give the dominant kingdom a hysteresis band rather than a hard
        /// cliff at the trigger threshold.
        /// </summary>
        public const float GreatReckoningReleaseShare = 0.66f;

        /// <summary>
        /// Browser GREAT_RECKONING_PRESSURE_SCORE = 4. World-pressure score
        /// contribution when a Great Reckoning is active. The
        /// world-pressure consumer does not exist in Unity yet; this constant
        /// is wired here so it lands correctly when the pressure-score
        /// aggregator ports.
        /// </summary>
        public const int GreatReckoningPressureScore = 4;

        /// <summary>
        /// Browser MATCH_STAGE_DEFINITIONS canonical stage count. Stage 1 is
        /// founding through Stage 5 final convergence.
        /// </summary>
        public const int MaxStageNumber = 5;

        /// <summary>
        /// Phase transition threshold: when stage 2 readiness fraction
        /// (stageThreeRequirements met / 3) reaches this value, the phase
        /// label flips from "emergence" to "commitment" even before Stage 3
        /// formally unlocks. Browser
        /// computeMatchProgressionState (~13508).
        /// </summary>
        public const float CommitmentPhaseStageThreeReadinessThreshold = 0.67f;
    }
}
