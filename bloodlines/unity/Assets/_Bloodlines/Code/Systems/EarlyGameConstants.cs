namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical balance constants for early-game foundation mechanics.
    /// All values are configurable here rather than scattered as magic numbers.
    /// Tuning pass expected after gameplay testing.
    ///
    /// Canon: early-game-foundation prompt 2026-04-25.
    /// </summary>
    public static class EarlyGameConstants
    {
        // --- Draft slider ---
        public const int DraftStepSize   = 5;    // slider increments
        public const int DraftMin        = 0;
        public const int DraftMax        = 100;
        public const int SquadSize       = 5;    // canonical squad headcount

        // --- Productivity base rates (0–1 scale) ---
        public const float ProductivityCivilian          = 1.00f;  // not drafted
        public const float ProductivityDraftedUntrained  = 0.75f;  // in pool, not yet squad
        public const float ProductivityTrainedReserve    = 0.50f;  // trained, on standby
        public const float ProductivityTrainedActiveDuty = 0.05f;  // assigned to military task

        // --- Settlement condition productivity modifiers ---
        // Applied multiplicatively when the condition is in shortage.
        public const float FoodShortageModifier    = 0.70f;  // food below threshold
        public const float WaterShortageModifier   = 0.65f;  // water below threshold
        public const float HousingShortageModifier = 0.85f;  // population at cap

        // --- Shortage escalation thresholds (seconds) ---
        public const float ShortageShortDurationSeconds    = 30f;   // short shortage
        public const float ShortageSustainedDurationSeconds = 120f; // sustained shortage
        // Severe: > ShortageSustainedDurationSeconds (desertion/sickness hooks fire here)

        // --- Water infrastructure ---
        // Population a single Well supports (matches buildings.json waterPopulationSupport).
        public const int WellPopulationSupport    = 50;
        // Founding Keep provides a small base water reserve before a Well is built.
        public const int KeepBaseWaterSupport     = 15;

        // --- Population growth ---
        // Mirror of PopulationGrowthSystem constants (kept in sync; source of truth is that system).
        public const float GrowthIntervalSeconds  = 18f;
        public const float FoodPerGrowth          = 1f;
        public const float WaterPerGrowth         = 1f;
    }
}
