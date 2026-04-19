using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Tracks the AI siege orchestration state for a faction.
    ///
    /// Readiness flags are written by other systems (or seeded directly in smoke
    /// tests). AISiegeOrchestrationSystem reads them each frame and writes Phase.
    ///
    /// Browser reference: ai.js updateEnemyAi attackTimer<=0 block (~1825-2090),
    ///                    areSiegeLinesFormed (947), getSiegeStagePoint (935).
    /// </summary>
    public struct AISiegeOrchestrationComponent : IComponentData
    {
        // Current orchestration phase (written by AISiegeOrchestrationSystem).
        public SiegeOrchestrationPhase Phase;

        // ------------------------------------------------------------------ army
        // Total combat army count. Phase requires >= 3 to engage.
        public int ArmyCount;

        // ------------------------------------------------------------------ siege readiness gates (ai.js 1879-1978)
        public bool EnemyHasSiegeUnit;          // hasSiegeUnit(state, "enemy")
        public bool EngineeringReady;           // engineerCorps.length >= 1
        public bool SupplyCampCompleted;
        public bool SupplyLineReady;            // SupplyCampCompleted && supplyWagons >= 1
        public bool SuppliedSiegeReady;         // siegeArmy.length > 0 && suppliedSiegeArmy >= 1
        public int  EscortArmyCount;
        public int  PlayerVerdantWardenCount;   // adjusts escort threshold
        public bool FormalSiegeLinesFormed;     // siegeLinesFormed && supportLinesFormed
        public bool ReliefArmyApproaching;      // isReliefArmyApproaching result

        // ------------------------------------------------------------------ supply interdiction / recovery (ai.js 1919-1955)
        public int  InterdictedWagonCount;
        public int  RecoveringWagonCount;
        public int  ConvoyRecoveringUnscreenedCount;

        // ------------------------------------------------------------------ supply collapse (ai.js 2054-2068)
        // suppliedSiegeReady was true but suppliedSiegeArmy has dropped to zero.
        public bool SupplyReadyButCollapsed;

        // ------------------------------------------------------------------ field water (ai.js 1846-1878, highest priority)
        public bool FieldWaterDesertionRisk;    // any unit with desertionRisk
        public bool FieldWaterAttritionActive;  // any unit with attritionActive
        public int  FieldWaterCriticalCount;    // units with status=critical or strain>=12

        // ------------------------------------------------------------------ post-repulse (ai.js 2004-2052)
        public float CohesionPenaltyUntil;      // elapsed seconds
        public float PostRepulseUntil;          // elapsed seconds
        public int   RepeatedAssaultAttempts;   // capped at 4 (ai.js 2028)
    }

    /// <summary>
    /// Mirrors the attackTimer branch priority order in ai.js (~1825-2090).
    /// Highest-priority conditions appear with the lowest numeric values
    /// to make priority reasoning explicit.
    /// </summary>
    public enum SiegeOrchestrationPhase : byte
    {
        Inactive                   = 0,   // keep not fortified or army < 3
        FieldWaterRetreat          = 1,   // any field water condition (highest priority)
        SiegeRefusal               = 2,   // keep fortified but no siege engines
        AwaitingEngineers          = 3,
        AwaitingSupplyCamp         = 4,
        AwaitingSupplyLine         = 5,
        SupplyInterdicted          = 6,
        SupplyRecoveringUnscreened = 7,
        SupplyRecoveringScreened   = 8,   // recovering wagons but fully screened: hold at stage
        AwaitingResupply           = 9,   // siege force ready but not supplied: move to stage
        AwaitingEscort             = 10,
        StagingLines               = 11,  // move to stage point to form siege lines
        ReliefHold                 = 12,
        PostRepulse                = 13,
        RepeatedAssault            = 14,
        SupplyCollapse             = 15,  // mid-siege supply chain collapsed
        Assaulting                 = 16,  // all gates clear: issue attack command
    }
}
