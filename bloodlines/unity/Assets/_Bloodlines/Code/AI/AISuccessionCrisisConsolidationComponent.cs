using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-faction timer driving AI succession-crisis auto-consolidation.
    ///
    /// When the AI enemy faction enters a succession crisis, this timer counts
    /// down and AISuccessionCrisisConsolidationSystem attempts to call
    /// consolidateSuccessionCrisis when resources and a living ruler are
    /// available.
    ///
    /// Browser reference: ai.js updateEnemyAi successionCrisisTimer block
    /// (~lines 1167-1185).
    ///
    /// Timer defaults:
    ///   Initial / reset on no-crisis:  12s
    ///   Reset on failed consolidation: 18s
    ///   Reset on success:              60s
    /// </summary>
    public struct AISuccessionCrisisConsolidationComponent : IComponentData
    {
        public float SuccessionCrisisTimer;   // default 12s
    }
}
