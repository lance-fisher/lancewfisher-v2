using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Marks a faction entity as AI-driven and carries the first economic AI
    /// loop's tuning parameters and per-cycle accumulators.
    ///
    /// Browser runtime equivalent: the AI kingdom profile that drives enemy
    /// gather assignment, villager training, and militia training against the
    /// same canonical resource economy the player uses.
    ///
    /// Scope for the first AI slice:
    /// - idle worker detection + gather assignment to the nearest faction-aligned
    ///   resource node of the primary gather type
    /// - villager training on the faction's command_hall up to TargetWorkerCount
    /// - militia training on the faction's barracks up to TargetMilitiaCount,
    ///   gated on worker count reaching TargetWorkerCount first
    ///
    /// Out of scope for this slice: construction, combat orders, expansion
    /// beyond the starting base, diplomacy, operations. Those layer in later
    /// AI slices.
    /// </summary>
    public struct AIEconomyControllerComponent : IComponentData
    {
        public bool Enabled;

        public FixedString32Bytes PrimaryGatherResourceId;
        public int TargetWorkerCount;
        public int TargetMilitiaCount;

        public float GatherAssignmentAccumulator;
        public float GatherAssignmentIntervalSeconds;

        public float ProductionAccumulator;
        public float ProductionIntervalSeconds;

        public int ControlledWorkerCountCached;
        public int ControlledMilitiaCountCached;
        public int IdleWorkerCountCached;
        public int ProductionQueueCountCached;

        public float ConstructionAccumulator;
        public float ConstructionIntervalSeconds;
        public int TargetDwellingCount;
        public int TargetFarmCount;
        public int TargetWellCount;
        public int TargetBarracksCount;
        public int ControlledDwellingCountCached;
        public int ControlledFarmCountCached;
        public int ControlledWellCountCached;
        public int ControlledBarracksCountCached;
        public int ConstructionPlacementsAttempted;
        public int ConstructionPlacementsSucceeded;

        public float MilitaryPostureAccumulator;
        public float MilitaryPostureIntervalSeconds;
        public int MilitaryPostureMinimumMilitiaCount;
        public float MilitaryPostureApproachRadius;
        public int MilitaryPostureOrdersIssued;
    }
}
