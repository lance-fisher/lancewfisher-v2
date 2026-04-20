using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to the DynastyOperationComponent
    /// entity that AICaptiveRansomExecutionSystem creates via
    /// DynastyOperationLimits.BeginOperation. Carries the per-operation
    /// fields the browser stores on the operation object at
    /// simulation.js:11043-11055 inside startRansomNegotiation.
    ///
    /// Field mapping browser -> Unity:
    ///   resolveAt          -> ResolveAtInWorldDays
    ///                          (RANSOM_BASE_DURATION_SECONDS = 16 at
    ///                          simulation.js:27; renown-scaled
    ///                          duration adjustment deferred until
    ///                          CapturedMemberElement carries renown)
    ///   memberId           -> CaptiveMemberId
    ///   memberTitle        -> CaptiveMemberTitle (carried for narrative
    ///                          replay without a roster lookup)
    ///   targetFactionId    -> CaptorFactionId (the faction holding the
    ///                          captive; same as the parent
    ///                          DynastyOperationComponent.TargetFactionId)
    ///   diplomatId         -> DiplomatMemberId (resolved from the
    ///                          source dynasty roster via priority
    ///                          [Diplomat, Merchant, HeirDesignate,
    ///                          HeadOfBloodline])
    ///   merchantId         -> MerchantMemberId (priority [Merchant,
    ///                          Governor, HeadOfBloodline])
    ///   intensityCost      -> IntensityCost (ransom does not deduct
    ///                          intensity; field reserved at 0 for
    ///                          surface-shape consistency)
    ///   escrowCost         -> EscrowCost (DynastyOperationEscrowCost
    ///                          struct; Gold and Influence set to the
    ///                          canonical base values, no renown
    ///                          scaling)
    ///   projectedChance    -> ProjectedChance (browser hardcodes 1.0
    ///                          at simulation.js:4964 because ransom
    ///                          is a paid transaction, not a roll)
    ///
    /// Browser parity: the operation object's `id`, `type`,
    /// `sourceFactionId`, `targetFactionId` (as the captor), `memberId`,
    /// and `startedAt` fields are stored on the parent
    /// DynastyOperationComponent (sub-slice 18); this per-kind component
    /// carries only the fields specific to the ransom operation.
    ///
    /// Browser duration translation: RANSOM_BASE_DURATION_SECONDS = 16
    /// at simulation.js:27 is real seconds. Unity stores
    /// ResolveAtInWorldDays = current + 16 (using browser numeric
    /// value directly on the in-world timeline), matching the
    /// sub-slice 20/21/22/23 duration convention.
    ///
    /// Browser cost simplification: the browser cost calculation
    /// (simulation.js:4947-4950) scales the base by member.renown,
    /// roleMultiplier, envoyDiscount, captorPremium. Unity uses the
    /// canonical base (Gold = 70, Influence = 18 from
    /// RANSOM_BASE_GOLD_COST and RANSOM_BASE_INFLUENCE_COST at
    /// simulation.js:31-32) without scaling because
    /// CapturedMemberElement does not yet carry renown or roleId. A
    /// future slice that extends the captive element shape with
    /// renown/role will tighten the calculation.
    ///
    /// No resolution system ships in this slice. The DynastyOperationComponent
    /// remains Active=true with this per-kind component attached; a
    /// future resolution slice walks expired entries at
    /// ResolveAtInWorldDays, applies the canonical browser semantics
    /// (release captive on success via CapturedMemberHelpers.ReleaseCaptive
    /// flipping Status from Held -> RansomOffered -> Released, return
    /// member to source faction roster, record stewardship +1 conviction
    /// on captor + oathkeeping +1 on source per browser
    /// simulation.js:11136-11138), and flips Active=false.
    /// </summary>
    public struct DynastyOperationCaptiveRansomComponent : IComponentData
    {
        public float                       ResolveAtInWorldDays;
        public FixedString64Bytes          CaptiveMemberId;
        public FixedString64Bytes          CaptiveMemberTitle;
        public FixedString32Bytes          CaptorFactionId;
        public FixedString64Bytes          DiplomatMemberId;
        public FixedString64Bytes          MerchantMemberId;
        public float                       ProjectedChance;
        public float                       IntensityCost;
        public DynastyOperationEscrowCost  EscrowCost;
    }
}
