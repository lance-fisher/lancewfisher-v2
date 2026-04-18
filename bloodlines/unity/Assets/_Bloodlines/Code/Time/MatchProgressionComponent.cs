using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.GameTime
{
    /// <summary>
    /// Singleton component carrying the full five-stage match-progression state.
    /// Browser equivalent: state.matchProgression (simulation.js computeMatchProgressionState,
    /// updateMatchProgressionState, getMatchProgressionSnapshot).
    ///
    /// Five stages (canonical browser MATCH_STAGE_DEFINITIONS):
    ///   1 = founding
    ///   2 = expansion_identity
    ///   3 = encounter_establishment
    ///   4 = war_turning_of_tides
    ///   5 = final_convergence
    ///
    /// Three phases (canonical browser MATCH_PHASE_LABELS):
    ///   emergence, commitment, resolution
    ///
    /// GreatReckoning: triggers when a kingdom holds >= 70% of total kingdom territories.
    /// Releases when share falls below 66%.
    /// </summary>
    public struct MatchProgressionComponent : IComponentData
    {
        // --- Stage identity ---
        public int StageNumber;
        public FixedString32Bytes StageId;
        public FixedString64Bytes StageLabel;

        // --- Phase identity ---
        public FixedString32Bytes PhaseId;
        public FixedString32Bytes PhaseLabel;

        // --- Progress toward next stage [0, 1] ---
        public float StageReadiness;

        // --- Next stage descriptors ---
        public FixedString32Bytes NextStageId;
        public FixedString64Bytes NextStageLabel;

        // --- Dual-clock mirror (kept in sync with DualClockComponent each tick) ---
        public float InWorldDays;
        public float InWorldYears;
        public int DeclarationCount;

        // --- War signals ---
        public bool RivalContactActive;
        public bool SustainedWarActive;

        // --- Great Reckoning ---
        public bool GreatReckoningActive;
        public FixedString32Bytes GreatReckoningTargetFactionId;
        public float GreatReckoningShare;
        public float GreatReckoningThreshold;

        // --- Dominant faction ---
        public FixedString32Bytes DominantKingdomId;
        public float DominantTerritoryShare;
    }
}
