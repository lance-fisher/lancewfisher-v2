using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to the DynastyOperationComponent
    /// entity that AIHolyWarExecutionSystem creates via
    /// DynastyOperationLimits.BeginOperation. Carries the per-operation
    /// fields the browser stores on the operation object at
    /// simulation.js:10578-10593 inside startHolyWarDeclaration.
    ///
    /// Field mapping browser -> Unity:
    ///   resolveAt          -> ResolveAtInWorldDays (declaration window
    ///                          end; HOLY_WAR_DECLARATION_DURATION_SECONDS
    ///                          = 18 at simulation.js:9771)
    ///   warDuration        -> WarExpiresAtInWorldDays (full holy-war
    ///                          duration; HOLY_WAR_DURATION_SECONDS
    ///                          = 180 at simulation.js:9775; the war
    ///                          itself is created from this entry by a
    ///                          future resolution slice via
    ///                          createHolyWarEntry at simulation.js:10505)
    ///   operatorId         -> OperatorMemberId
    ///   operatorTitle      -> OperatorTitle
    ///   intensityPulse     -> IntensityPulse (browser doctrine bias:
    ///                          1.2 dark / 0.9 light at simulation.js:10468)
    ///   loyaltyPulse       -> LoyaltyPulse (1.8 dark / 1.2 light at
    ///                          simulation.js:10469)
    ///   compatibilityLabel -> CompatibilityLabel (FixedString64Bytes;
    ///                          mirrors the human-readable tier label
    ///                          from getMarriageFaithCompatibilityProfile,
    ///                          stored verbatim from terms; consumers
    ///                          read the string for narrative display)
    ///   intensityCost      -> IntensityCost (HOLY_WAR_INTENSITY_COST
    ///                          = 18 at simulation.js:9774)
    ///   escrowCost         -> EscrowCost (DynastyOperationEscrowCost
    ///                          struct; only Influence is set for the
    ///                          canonical holy-war cost = 24 at
    ///                          simulation.js:9767)
    ///
    /// Browser parity: the operation object's `id`, `type`,
    /// `sourceFactionId`, `targetFactionId`, and `startedAt` fields
    /// are stored on the parent DynastyOperationComponent (sub-slice
    /// 18) so this per-kind component carries only the fields specific
    /// to holy-war declaration.
    ///
    /// Browser duration translation: HOLY_WAR_DECLARATION_DURATION_SECONDS
    /// = 18 and HOLY_WAR_DURATION_SECONDS = 180 are real seconds.
    /// AIHolyWarExecutionSystem reinterprets both numeric values
    /// directly on the in-world timeline (HolyWarDeclarationDurationInWorldDays
    /// = 18f, HolyWarDurationInWorldDays = 180f) rather than translating
    /// through DualClock.DaysPerRealSecond, matching the sub-slice 20
    /// missionary-duration convention. A future resolution slice may
    /// re-translate at runtime if the canonical clock rate ever
    /// shifts; the data shape stays the same.
    ///
    /// No resolution system ships in this slice. The DynastyOperationComponent
    /// remains Active=true with this per-kind component attached; a
    /// future resolution slice walks expired DynastyOperationHolyWarComponent
    /// entries at ResolveAtInWorldDays, materializes the holy-war entry
    /// on the source faction's faith state (browser
    /// faction.faith.activeHolyWars), applies intensityPulse /
    /// loyaltyPulse to the target faction at war-tick boundaries, and
    /// flips Active=false at WarExpiresAtInWorldDays.
    /// </summary>
    public struct DynastyOperationHolyWarComponent : IComponentData
    {
        public float                       ResolveAtInWorldDays;
        public float                       WarExpiresAtInWorldDays;
        public FixedString64Bytes          OperatorMemberId;
        public FixedString64Bytes          OperatorTitle;
        public float                       IntensityPulse;
        public float                       LoyaltyPulse;
        public FixedString64Bytes          CompatibilityLabel;
        public float                       IntensityCost;
        public DynastyOperationEscrowCost  EscrowCost;
    }
}
