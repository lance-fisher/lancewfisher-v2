using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction world pressure state.
    /// Browser runtime equivalents: faction.worldPressureScore, faction.worldPressureStreak,
    /// faction.worldPressureLevel (simulation.js updateWorldPressureEscalation:13709).
    ///
    /// Score is the sum of portably-sourced pressure contributions computed each realm cycle.
    /// Streak increments when this faction is the global dominant leader (score >= 4,
    /// strictly highest). Level is derived from streak via canonical thresholds.
    /// Targeted is true when this faction IS the global dominant leader at level > 0.
    ///
    /// Ported sources (initial slice):
    ///   - territory expansion: max(0, ownedTerritories - 2)   -- simulation.js:13193
    ///   - great reckoning: GREAT_RECKONING_PRESSURE_SCORE = 4  -- simulation.js:406
    ///
    /// Additional ported source:
    ///   - territorial governance recognition: 0 / 3 / 5 / 6 / 7 depending on
    ///     live governance recognition, recognition establishment, alliance threshold,
    ///     and victory readiness
    ///
    /// Reserved sources (future slices): divineRightDeclaration, offHomeHoldings,
    /// holyWar, captives, hostileOperations, darkExtremes.
    /// </summary>
    public struct WorldPressureComponent : IComponentData
    {
        public int Score;
        public int Streak;
        // 0 = quiet, 1 = gathering, 2 = overwhelming, 3 = convergence.
        public int Level;
        public FixedString32Bytes Label;
        // True when this faction is the global dominant pressure leader.
        public bool Targeted;
        // Breakdown components for HUD readout.
        public int TerritorialGovernanceRecognitionScore;
        public int TerritoryExpansionScore;
        public int GreatReckoningScore;
    }

    /// <summary>
    /// Singleton tracking world pressure cycle timing.
    /// WorldPressureEscalationSystem fires streak/consequence updates once per
    /// CycleSeconds (canonical 90-second realm cycle). Score and Targeted are
    /// refreshed every frame for HUD accuracy.
    /// </summary>
    public struct WorldPressureCycleTrackerComponent : IComponentData
    {
        public float Accumulator;
        // Canonical value: 90f (matches realm cycle length). Smoke tests seed
        // this at >= CycleSeconds to force an immediate cycle on first update.
        public float CycleSeconds;
    }
}
