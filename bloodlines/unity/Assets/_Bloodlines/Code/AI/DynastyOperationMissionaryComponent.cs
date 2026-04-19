using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resource cost mirrored from the browser missionary cost shape
    /// (`MISSIONARY_COST = { influence: 14 }` at simulation.js:9766).
    /// Fields mirror ResourceStockpileComponent so future per-kind
    /// component slices can carry the same cost surface without
    /// reshaping. Only the relevant resource is non-zero per operation.
    /// </summary>
    public struct DynastyOperationEscrowCost
    {
        public float Gold;
        public float Food;
        public float Water;
        public float Wood;
        public float Stone;
        public float Iron;
        public float Influence;
    }

    /// <summary>
    /// Per-kind component attached to the DynastyOperationComponent
    /// entity that AIMissionaryExecutionSystem creates via
    /// DynastyOperationLimits.BeginOperation. Carries the per-operation
    /// fields the browser stores on the operation object at
    /// simulation.js:10536-10554 inside startMissionaryOperation.
    ///
    /// Field mapping browser -> Unity:
    ///   resolveAt          -> ResolveAtInWorldDays
    ///   operatorId         -> OperatorMemberId
    ///   operatorTitle      -> OperatorTitle
    ///   sourceFaithId      -> SourceFaithId (canonical faith string id;
    ///                          FaithStateComponent.SelectedFaith
    ///                          enum is converted to a stable string
    ///                          token at the call site)
    ///   exposureGain       -> ExposureGain
    ///   intensityErosion   -> IntensityErosion
    ///   loyaltyPressure    -> LoyaltyPressure
    ///   successScore       -> SuccessScore
    ///   projectedChance    -> ProjectedChance
    ///   intensityCost      -> IntensityCost
    ///   escrowCost         -> EscrowCost (struct mirroring resource
    ///                          stockpile fields; only Influence is set
    ///                          for the canonical missionary cost)
    ///
    /// Browser parity: the operation object's `id`, `type`,
    /// `sourceFactionId`, `targetFactionId`, and `startedAt` fields
    /// are stored on the parent DynastyOperationComponent (sub-slice
    /// 18) so this per-kind component carries only the fields specific
    /// to missionary work.
    ///
    /// Browser duration: `MISSIONARY_DURATION_SECONDS = 32` at
    /// simulation.js:9770 is in real seconds. Unity stores
    /// ResolveAtInWorldDays on the in-world clock instead. The
    /// AIMissionaryExecutionSystem uses
    /// `MISSIONARY_DURATION_IN_WORLD_DAYS = 32f` directly, treating the
    /// browser numeric value as the canonical duration on the in-world
    /// timeline rather than translating through DualClock.DaysPerRealSecond.
    /// A future resolution slice can re-translate at runtime if the
    /// canonical clock rate changes; the data shape stays the same.
    ///
    /// No resolution system ships this slice. The DynastyOperationComponent
    /// remains Active=true with this per-kind component attached; a
    /// future resolution slice walks expired entries at
    /// ResolveAtInWorldDays, applies ExposureGain / IntensityErosion /
    /// LoyaltyPressure to the target faction, and flips Active=false.
    /// </summary>
    public struct DynastyOperationMissionaryComponent : IComponentData
    {
        public float                ResolveAtInWorldDays;
        public FixedString64Bytes   OperatorMemberId;
        public FixedString64Bytes   OperatorTitle;
        public FixedString64Bytes   SourceFaithId;
        public float                ExposureGain;
        public float                IntensityErosion;
        public float                LoyaltyPressure;
        public float                SuccessScore;
        public float                ProjectedChance;
        public float                IntensityCost;
        public DynastyOperationEscrowCost EscrowCost;
    }
}
