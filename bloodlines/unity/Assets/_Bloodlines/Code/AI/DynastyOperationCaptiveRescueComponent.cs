using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to the DynastyOperationComponent
    /// entity that AICaptiveRescueExecutionSystem creates via
    /// DynastyOperationLimits.BeginOperation. Carries the per-operation
    /// fields the browser stores on the operation object at
    /// simulation.js:11084-11101 inside startRescueOperation.
    ///
    /// Field mapping browser -> Unity:
    ///   resolveAt          -> ResolveAtInWorldDays (declaration window
    ///                          end; RESCUE_BASE_DURATION_SECONDS = 20
    ///                          at simulation.js:29; renown-scaled
    ///                          duration adjustment deferred until
    ///                          CapturedMemberElement carries renown)
    ///   memberId           -> CaptiveMemberId (the captured member's
    ///                          id; also stored on the parent
    ///                          DynastyOperationComponent.TargetMemberId
    ///                          for query convenience)
    ///   memberTitle        -> CaptiveMemberTitle (carried for narrative
    ///                          replay without a roster lookup)
    ///   targetFactionId    -> CaptorFactionId (the faction holding the
    ///                          captive; same as the parent
    ///                          DynastyOperationComponent.TargetFactionId)
    ///   spymasterId        -> SpymasterMemberId (resolved from the
    ///                          source dynasty roster via priority
    ///                          [Spymaster, Diplomat, Merchant])
    ///   diplomatId         -> DiplomatMemberId (priority [Diplomat,
    ///                          Merchant, HeirDesignate])
    ///   holdingSettlementId-> HoldingSettlementId (default empty;
    ///                          Unity has no holding-settlement port
    ///                          yet, deferred to a future settlement
    ///                          surface)
    ///   keepTier           -> KeepTier (default 0; same deferral)
    ///   wardId             -> WardId (default empty FixedString32Bytes;
    ///                          same deferral)
    ///   successScore       -> SuccessScore (computed from simplified
    ///                          parity formula: power - difficulty
    ///                          using base constants, no member.renown
    ///                          scaling)
    ///   projectedChance    -> ProjectedChance (clamped 0.12-0.88 from
    ///                          0.5 + (successScore / 45))
    ///   intensityCost      -> IntensityCost (rescue does not deduct
    ///                          intensity; field reserved at 0 for
    ///                          surface-shape consistency with other
    ///                          per-kind components)
    ///   escrowCost         -> EscrowCost (DynastyOperationEscrowCost
    ///                          struct; Gold and Influence set to the
    ///                          canonical base values, no renown
    ///                          scaling)
    ///
    /// Browser parity: the operation object's `id`, `type`,
    /// `sourceFactionId`, `targetFactionId` (as the captor), `memberId`,
    /// and `startedAt` fields are stored on the parent
    /// DynastyOperationComponent (sub-slice 18); this per-kind component
    /// carries only the fields specific to the rescue operation.
    ///
    /// Browser duration translation: RESCUE_BASE_DURATION_SECONDS = 20
    /// at simulation.js:29 is real seconds. Unity stores
    /// ResolveAtInWorldDays = current + 20 (using browser numeric value
    /// directly on the in-world timeline), matching the sub-slice
    /// 20/21/22 duration convention.
    ///
    /// Browser cost simplification: the browser cost calculation
    /// (simulation.js:4999-5002) scales the base by member.renown,
    /// roleMultiplier, and keepTier. Unity uses the canonical base
    /// (Gold = 42, Influence = 26 from RESCUE_BASE_GOLD_COST and
    /// RESCUE_BASE_INFLUENCE_COST at simulation.js:33-34) without
    /// scaling because CapturedMemberElement does not yet carry
    /// renown or roleId. A future slice that extends the captive
    /// element shape with renown/role will tighten the calculation.
    ///
    /// No resolution system ships in this slice. The DynastyOperationComponent
    /// remains Active=true with this per-kind component attached; a
    /// future resolution slice walks expired entries at
    /// ResolveAtInWorldDays, rolls success against ProjectedChance,
    /// applies effects (release captive on success via
    /// CapturedMemberHelpers.ReleaseCaptive(Status=Released); record
    /// failed-rescue conviction event on failure per browser
    /// simulation.js:5885-5894), and flips Active=false.
    /// </summary>
    public struct DynastyOperationCaptiveRescueComponent : IComponentData
    {
        public float                       ResolveAtInWorldDays;
        public FixedString64Bytes          CaptiveMemberId;
        public FixedString64Bytes          CaptiveMemberTitle;
        public FixedString32Bytes          CaptorFactionId;
        public FixedString64Bytes          SpymasterMemberId;
        public FixedString64Bytes          DiplomatMemberId;
        public FixedString64Bytes          HoldingSettlementId;
        public int                         KeepTier;
        public FixedString32Bytes          WardId;
        public float                       SuccessScore;
        public float                       ProjectedChance;
        public float                       IntensityCost;
        public DynastyOperationEscrowCost  EscrowCost;
    }
}
