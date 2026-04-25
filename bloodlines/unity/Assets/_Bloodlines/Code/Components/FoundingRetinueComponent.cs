using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction early founding state. Present on all kingdom factions before the
    /// Keep is established. Removed or zeroed after deployment completes.
    ///
    /// Design intent: MCV-style deployment decision. The player may move and scout
    /// before deploying, but moving delays growth and carries risk. Deploying immediately
    /// is always safe and viable at the guaranteed starting location.
    ///
    /// Canon reference: SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine.md
    /// (match opening, founding retinue concept). Prod prompt: early-game-foundation 2026-04-25.
    /// </summary>
    public struct FoundingRetinueComponent : IComponentData
    {
        /// <summary>True once the player issues the Deploy/Establish Keep order.</summary>
        public bool IsDeployed;

        /// <summary>World position where the Keep was established.</summary>
        public float3 DeployPosition;

        /// <summary>
        /// True while the settlement is small enough to uproot and relocate.
        /// Cleared by FoundingRetinueRelocationGatingSystem once the settlement
        /// exceeds the early relocation threshold (e.g., more than the Keep and
        /// one other building exist). Hook for future cost/penalty implementation.
        /// </summary>
        public bool RelocationEligible;

        /// <summary>
        /// Accumulates time spent mobile (undeployed) so pressure / morale hooks
        /// can read it later without replaying history.
        /// </summary>
        public float MobileElapsedSeconds;
    }

    /// <summary>
    /// Tag added to a unit/retinue entity that represents the undeployed founding
    /// retinue on the map. Removed when the Keep is established.
    /// Hook: future systems can query this tag to block normal build/production
    /// actions until deployment completes.
    /// </summary>
    public struct UndeployedKeepTag : IComponentData { }

    /// <summary>
    /// Per-faction build tier. Drives which buildings are visible/buildable.
    ///
    ///   Tier 0  – pre-deployment. No builds available. Only Deploy action.
    ///   Tier 1  – post-deployment first tier: Housing, Well, Woodcutter Camp,
    ///             Forager Camp, Training Yard.
    ///   Tier 2  – expanded: Small Farm unlocks. Future tiers extend from here.
    ///
    /// BuildTierGatingSystem evaluates deployment state and prerequisites and
    /// writes CurrentTier each simulation frame.
    /// </summary>
    public struct BuildTierComponent : IComponentData
    {
        public byte CurrentTier;

        // Cached prerequisite flags so UI and AI can read without re-querying.
        public bool HasHousing;
        public bool HasWater;
        public bool HasFoodSource;
        public bool HasTrainingYard;
    }

    // ---------------------------------------------------------------------------
    // Placeholder tag — Keep relocation (later-seat transfer is distinct from early
    // MCV relocation and is NOT implemented in this slice).
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Tag set when a faction has initiated an early Keep relocation.
    /// Systems can react to this tag to apply costs, pause production, etc.
    /// Stub: full cost/penalty implementation deferred to a later slice.
    /// </summary>
    public struct KeepRelocationInProgressTag : IComponentData { }
}
