using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Holds context flags for the AI build priority decision tree and the last
    /// dispatched build order.
    ///
    /// AIBuildOrderSystem reads AIStrategyComponent.BuildTimer (decremented by
    /// AIStrategicPressureSystem each frame). When the timer fires, it evaluates
    /// the 13-step priority chain and writes NextBuildOp. Actual
    /// attemptPlaceBuilding calls are deferred to a future integration pass.
    ///
    /// Browser reference: ai.js updateEnemyAi buildTimer<=0 block ~lines 1377-1573.
    /// </summary>
    public struct AIBuildOrderComponent : IComponentData
    {
        // ------------------------------------------------------------------ worker
        public bool HasBuilder;             // idle enemy worker exists

        // ------------------------------------------------------------------ building existence
        public bool HasBarracks;
        public bool HasWayshrine;
        public bool WayshrineCompleted;     // required for covenant hall
        public bool HasQuarry;
        public bool HasIronMine;
        public bool HasSiegeWorkshop;
        public bool SiegeWorkshopCompleted; // required for supply camp
        public bool HasCovenantHall;
        public bool CovenantHallCompleted;  // required for grand sanctuary
        public bool HasGrandSanctuary;
        public bool GrandSanctuaryCompleted;// required for apex covenant
        public bool HasApexCovenant;
        public bool HasSupplyCamp;
        public bool SupplyCampCompleted;    // required for stable
        public bool HasStable;

        // ------------------------------------------------------------------ affordability (pre-computed by context provider)
        public bool CanAffordBarracks;          // gold >= 85 && wood >= 95
        public bool CanAffordWayshrine;
        public bool CanAffordQuarry;            // gold >= 40 && wood >= 40
        public bool CanAffordIronMine;          // gold >= 55 && wood >= 50
        public bool CanAffordSiegeWorkshop;
        public bool CanAffordCovenantHall;
        public bool CanAffordGrandSanctuary;
        public bool CanAffordApexCovenant;
        public bool CanAffordSupplyCamp;
        public bool CanAffordStable;

        // ------------------------------------------------------------------ faith context
        public bool  EnemyHasFaith;             // selectedFaithId != null
        public bool  CovenantTestPassed;        // enemy.faith.covenantTestPassed
        public float FaithIntensity;            // for 26/48/80 thresholds
        public bool  ActiveCovenantTest;        // enemyActiveCovenantTest != null
        public bool  PlayerCovenantActive;      // playerActiveCovenantTest != null

        // ------------------------------------------------------------------ military support
        public bool HasEngineerCorps;   // at least one engineer unit
        public bool HasSupplyWagon;     // at least one supply wagon

        // ------------------------------------------------------------------ building counts (dwelling/farm/well gates)
        public int HouseCount;          // < 4 gate for dwelling
        public int FarmCount;           // < 3 gate for farm
        public int WellCount;           // < 3 gate for well

        // ------------------------------------------------------------------ population and resources (farm/well/dwelling gates)
        public int   PopulationCapAvailable;    // cap - used - reserved; <= 1 triggers dwelling
        public int   PopulationTotal;           // population.total for farm/well threshold
        public float FoodStock;                 // food < total + 4 triggers farm
        public float WaterStock;                // water < total + 4 triggers well

        // ------------------------------------------------------------------ dispatch result (written by system)
        public BuildOrderKind NextBuildOp;
    }

    /// <summary>
    /// Identifies which building the AI decided to construct on the last build
    /// timer fire. Priority order mirrors the ai.js if-else chain exactly.
    /// </summary>
    public enum BuildOrderKind : byte
    {
        None            = 0,
        Barracks        = 1,
        Wayshrine       = 2,
        Quarry          = 3,
        IronMine        = 4,
        SiegeWorkshop   = 5,
        CovenantHall    = 6,
        GrandSanctuary  = 7,
        ApexCovenant    = 8,
        SupplyCamp      = 9,
        Stable          = 10,
        Dwelling        = 11,
        Farm            = 12,
        Well            = 13,
    }
}
