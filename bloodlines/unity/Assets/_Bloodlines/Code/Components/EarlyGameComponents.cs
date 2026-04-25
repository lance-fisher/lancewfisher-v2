using Unity.Entities;

namespace Bloodlines.Components
{
    // ==========================================================================
    // EARLY GAME FOUNDATION — 2026-04-25
    // Implements: worker slots, water capacity, population productivity states,
    // military draft, and militia squad duty.
    // Canon: early-game-foundation prompt 2026-04-25.
    // ==========================================================================

    // --------------------------------------------------------------------------
    // WORKER SLOT BUILDING
    // For Woodcutter Camp, Forager Camp, Small Farm.
    // Player assigns workers; output = baseRatePerWorker × assigned × productivity.
    // --------------------------------------------------------------------------

    /// <summary>
    /// Per-building worker slot assignment for slot-based production buildings.
    /// Present on forager_camp, lumber_camp (Woodcutter Camp), small_farm.
    ///
    /// Output flow: WorkerSlotProductionSystem reads this component alongside
    /// the owning faction's PopulationProductivityComponent and adds
    /// (FoodOutputPerWorkerPerSecond * AssignedWorkers * EffectiveProductivity * dt)
    /// to the faction's ResourceStockpileComponent each frame.
    ///
    /// Tooltip: "Assign workers to gather [resource]. Output scales with
    ///          assigned workers and settlement productivity conditions."
    /// </summary>
    public struct WorkerSlotBuildingComponent : IComponentData
    {
        public int MaxWorkerSlots;
        public int AssignedWorkers;

        // Base output rates per worker per second (from buildings.json workerOutputPerSecond).
        // Only the relevant field is non-zero per building type.
        public float FoodOutputPerWorkerPerSecond;
        public float WoodOutputPerWorkerPerSecond;

        // Cached ratio written by WorkerSlotProductionSystem each frame.
        public float FillRatio; // AssignedWorkers / MaxWorkerSlots
    }

    // --------------------------------------------------------------------------
    // WATER CAPACITY
    // Per-faction. Summed from all live Wells by WaterCapacitySystem.
    // --------------------------------------------------------------------------

    /// <summary>
    /// Per-faction water infrastructure capacity.
    ///
    /// MaxSupportedByWater = sum of (waterPopulationSupport) across all live,
    /// unraided Wells owned by this faction.
    ///
    /// If population.Total > MaxSupportedByWater the faction is in water deficit:
    ///   - population growth halts (PopulationGrowthSystem hard gate)
    ///   - EffectiveProductivity in PopulationProductivityComponent is penalized
    ///   - WaterAdequate flag on PopulationProductivityComponent is set false
    ///
    /// The Keep provides a small starting water reserve so the player is not
    /// penalized before they can build a Well.
    /// See EarlyGameConstants.KeepBaseWaterSupport.
    /// </summary>
    public struct WaterCapacityComponent : IComponentData
    {
        public int MaxSupportedByWater;
    }

    // --------------------------------------------------------------------------
    // POPULATION PRODUCTIVITY
    // Per-faction. Drives economic output scaling.
    // --------------------------------------------------------------------------

    /// <summary>
    /// Per-faction effective productivity state.
    ///
    /// Canonical productivity model (early-game-foundation prompt 2026-04-25):
    ///   Civilian (not drafted):       100% base
    ///   Drafted but untrained:         75% base
    ///   Trained military — Reserve:    50% base
    ///   Trained military — Active Duty: 5% base
    ///
    /// BaseProductivity = weighted average of the above across the faction population.
    /// EffectiveProductivity = BaseProductivity × settlement condition multipliers
    ///   (food shortage, water deficit, housing overcrowding each apply penalties).
    ///
    /// EffectiveProductivity drives:
    ///   - WorkerSlotProductionSystem output scaling
    ///   - WorkerGatherSystem throughput (future hook: currently reads stockpile, not productivity)
    ///   - ResourceTrickleBuildingSystem is NOT scaled by productivity (buildings trickle
    ///     passively; productivity governs labor-dependent output only)
    ///
    /// Settlement condition flags are set here so HUD/AI can read them cheaply.
    /// </summary>
    public struct PopulationProductivityComponent : IComponentData
    {
        public float BaseProductivity;       // 0–1, before settlement modifiers
        public float EffectiveProductivity;  // 0–1, after all modifiers

        public bool FoodAdequate;
        public bool WaterAdequate;
        public bool HousingAdequate;

        // Shortage severity for escalating penalty tiers (future event hooks).
        public float FoodShortageAccumulator;   // seconds below food threshold
        public float WaterShortageAccumulator;  // seconds below water threshold
    }

    // --------------------------------------------------------------------------
    // MILITARY DRAFT
    // Per-faction. Utopia-inspired draft slider (0–100%, 5% increments).
    // --------------------------------------------------------------------------

    /// <summary>
    /// Per-faction military draft state.
    ///
    /// DraftRatePct is the player-controlled slider: 0–100, always a multiple of 5
    /// (20 possible values). It determines the fraction of population committed to
    /// military readiness. The slider does NOT instantly create soldiers.
    ///
    /// Flow:
    ///   DraftPool = Total × DraftRatePct / 100
    ///   UntrainedDrafted = max(0, DraftPool − TrainedMilitary)
    ///   Training Yard converts UntrainedDrafted into trained 5-person squads.
    ///
    /// Lowering DraftRatePct below TrainedMilitary triggers OverDraftedFlag;
    /// full demobilization logic is deferred to a later slice (TODO hook).
    ///
    /// Tooltip: "Sets the percentage of population committed to military readiness.
    ///          Drafted population is less productive economically. Trained squads
    ///          can remain in Reserve for partial productivity or be assigned to
    ///          Active Duty."
    /// </summary>
    public struct MilitaryDraftComponent : IComponentData
    {
        // Player-controlled, 0–100 in multiples of 5.
        public int DraftRatePct;

        // Derived by MilitaryDraftSystem each frame.
        public int DraftPool;          // Total * DraftRatePct / 100
        public int TrainedMilitary;    // trained squad members (5 × squad count)
        public int UntrainedDrafted;   // DraftPool − TrainedMilitary, min 0
        public int ReserveMilitary;    // trained on Reserve duty
        public int ActiveDutyMilitary; // trained on Active Duty

        // Flag: trained > DraftPool (draft lowered below existing army).
        // TODO: hook for demobilization policy in a later slice.
        public bool OverDraftedFlag;
    }

    // --------------------------------------------------------------------------
    // MILITIA SQUAD
    // Per-squad unit entity. Canonical squad size = 5.
    // --------------------------------------------------------------------------

    public enum DutyState : byte
    {
        Reserve    = 0,
        ActiveDuty = 1,
    }

    public enum SquadAssignmentType : byte
    {
        None               = 0,
        Guard              = 1,
        Scout              = 2,  // no dedicated Scout unit; squads scout on order
        Patrol             = 3,
        Attack             = 4,
        Escort             = 5,
        HoldPosition       = 6,
        DefendKeep         = 7,
        DefendWoodcutterCamp = 8,
        DefendForagerCamp  = 9,
    }

    /// <summary>
    /// Per-squad entity component. One squad = one selectable RTS unit representing
    /// five soldiers. Training Yard produces these from the drafted population pool.
    ///
    /// DutyState drives productivity contribution:
    ///   Reserve:    50% base productivity (trained, ready, not actively deployed)
    ///   ActiveDuty:  5% base productivity (assigned to military task)
    ///
    /// Scouting is SquadAssignmentType.Scout on a militia squad — no dedicated scout
    /// unit exists (owner direction 2026-04-25, units.json swordsman notes).
    ///
    /// Tooltip — Reserve:    "Trained and ready, contributing partial labor while not actively deployed."
    /// Tooltip — ActiveDuty: "Assigned to military work such as scouting, guarding, patrolling,
    ///                        escorting, or attacking. Provides minimal civilian productivity."
    /// </summary>
    public struct MilitiaSquadComponent : IComponentData
    {
        public int SquadSize;                    // canonical 5
        public DutyState DutyState;
        public SquadAssignmentType AssignmentType;
    }
}
