using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Mode identifying which governance authority ratified a marriage accept
    /// on the target (spouse) side. Ported from the browser governance profile
    /// in simulation.js getMarriageAuthorityProfile (~6134). The mode resolves
    /// the legitimacy cost the target dynasty pays on accept:
    ///
    ///   HeadDirect   - head_of_bloodline, status == ruling. Cost 0.
    ///   HeirRegency  - heir_designate holds the seat in regency. Cost 1.
    ///   EnvoyRegency - diplomat stands in as acting authority. Cost 2.
    ///   None         - no authority found. Sentinel; accept is rejected.
    ///
    /// Browser constant: MARRIAGE_REGENCY_LEGITIMACY_COSTS
    /// { head_direct: 0, heir_regency: 1, envoy_regency: 2 } at simulation.js:6091.
    /// </summary>
    public enum MarriageAuthorityMode : byte
    {
        None = 0,
        HeadDirect = 1,
        HeirRegency = 2,
        EnvoyRegency = 3,
    }

    /// <summary>
    /// Attached to the primary MarriageComponent entity alongside
    /// MarriageAcceptEffectsPendingTag by AIMarriageInboxAcceptSystem. Captures
    /// the authority mode + legitimacy cost resolved on accept so
    /// AIMarriageAcceptEffectsSystem can apply the governance cost on the
    /// target dynasty before the +2 bonus.
    ///
    /// Browser reference: simulation.js getMarriageAcceptanceTerms (~6327) and
    /// applyMarriageGovernanceLegitimacyCost (~6232). Cost is applied to the
    /// spouse (target-side) dynasty and also recorded as a Stewardship
    /// conviction event so reckless regency marriages stain the stewardship
    /// ledger just as the browser records.
    ///
    /// Zero-cost (HeadDirect) terms are still attached so the effects system
    /// can cleanly attribute a +2 legitimacy net outcome to a head-direct
    /// approval versus a regency-reduced outcome.
    /// </summary>
    public struct MarriageAcceptanceTermsComponent : IComponentData
    {
        public MarriageAuthorityMode AuthorityMode;
        public float LegitimacyCost;
    }
}
