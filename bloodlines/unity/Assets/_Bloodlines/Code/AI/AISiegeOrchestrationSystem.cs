using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Evaluates the AI siege orchestration decision tree each frame and writes
    /// AISiegeOrchestrationComponent.Phase for every enemy faction entity that
    /// carries the component.
    ///
    /// This system is a pure state machine: it reads context flags and writes a
    /// phase tag. Actual unit movement and attack commands are deferred to the
    /// movement and combat systems that read the phase.
    ///
    /// Decision tree ported from ai.js updateEnemyAi attackTimer<=0 block
    /// (lines ~1825-2090). Branch priority order is preserved exactly:
    ///
    ///   1. Inactive (keep not fortified or army too small)
    ///   2. FieldWaterRetreat (any field water crisis -- highest priority)
    ///   3. SiegeRefusal (no siege engines)
    ///   4. AwaitingEngineers
    ///   5. AwaitingSupplyCamp
    ///   6. AwaitingSupplyLine
    ///   7. SupplyInterdicted
    ///   8. SupplyRecoveringUnscreened
    ///   9. SupplyRecoveringScreened (hold at stage point)
    ///  10. AwaitingResupply (move to stage)
    ///  11. AwaitingEscort
    ///  12. StagingLines (form lines at stage point)
    ///  13. ReliefHold
    ///  14. PostRepulse (cohesion penalty active)
    ///  15. RepeatedAssault (post-repulse, attempts < 4)
    ///  16. SupplyCollapse
    ///  17. Assaulting (all gates clear)
    ///
    /// Browser reference: ai.js updateEnemyAi attackTimer<=0 block (~1825-2090),
    ///                    areSiegeLinesFormed (947), getSiegeStagePoint (935),
    ///                    isReliefArmyApproaching (727).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIWorkerGatherSystem))]
    public partial struct AISiegeOrchestrationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float elapsed = (float)SystemAPI.Time.ElapsedTime;

            foreach (var (strategy, siege) in
                SystemAPI.Query<RefRO<AIStrategyComponent>,
                                RefRW<AISiegeOrchestrationComponent>>())
            {
                bool keepFortified = strategy.ValueRO.PlayerKeepFortified;
                siege.ValueRW.Phase = DeterminePhase(ref siege.ValueRW, keepFortified, elapsed);
            }
        }

        // ------------------------------------------------------------------ core decision tree

        private static SiegeOrchestrationPhase DeterminePhase(
            ref AISiegeOrchestrationComponent o, bool playerKeepFortified, float elapsed)
        {
            // Gate: keep not fortified or army too small (ai.js 1844: army.length >= 3)
            if (!playerKeepFortified || o.ArmyCount < 3)
                return SiegeOrchestrationPhase.Inactive;

            // Field water conditions -- highest priority (ai.js 1846-1878)
            if (o.FieldWaterDesertionRisk)
                return SiegeOrchestrationPhase.FieldWaterRetreat;
            if (o.FieldWaterAttritionActive)
                return SiegeOrchestrationPhase.FieldWaterRetreat;
            if (o.FieldWaterCriticalCount > 0)
                return SiegeOrchestrationPhase.FieldWaterRetreat;

            // Siege engine presence (ai.js 1879)
            if (!o.EnemyHasSiegeUnit)
                return SiegeOrchestrationPhase.SiegeRefusal;

            // Engineering readiness (ai.js 1889)
            if (!o.EngineeringReady)
                return SiegeOrchestrationPhase.AwaitingEngineers;

            // Supply camp (ai.js 1899)
            if (!o.SupplyCampCompleted)
                return SiegeOrchestrationPhase.AwaitingSupplyCamp;

            // Supply line: camp + wagons (ai.js 1909)
            if (!o.SupplyLineReady)
                return SiegeOrchestrationPhase.AwaitingSupplyLine;

            // Interdicted supply wagons (ai.js 1919)
            if (o.InterdictedWagonCount > 0)
                return SiegeOrchestrationPhase.SupplyInterdicted;

            // Recovering wagons: unscreened = pull back; screened = hold at stage (ai.js 1930-1955)
            if (o.RecoveringWagonCount > 0 && o.ConvoyRecoveringUnscreenedCount > 0)
                return SiegeOrchestrationPhase.SupplyRecoveringUnscreened;
            if (o.RecoveringWagonCount > 0 && o.ConvoyRecoveringUnscreenedCount == 0)
                return SiegeOrchestrationPhase.SupplyRecoveringScreened;

            // Supplied siege readiness (ai.js 1956)
            if (!o.SuppliedSiegeReady)
                return SiegeOrchestrationPhase.AwaitingResupply;

            // Escort threshold: base 3 + verdant warden bonus, capped at 5 (ai.js 1967)
            int escortThreshold = 3 + math.min(2, o.PlayerVerdantWardenCount);
            if (o.EscortArmyCount < escortThreshold)
                return SiegeOrchestrationPhase.AwaitingEscort;

            // Siege lines not yet formed (ai.js 1979)
            if (!o.FormalSiegeLinesFormed)
                return SiegeOrchestrationPhase.StagingLines;

            // Relief army approaching (ai.js 1990)
            if (o.ReliefArmyApproaching)
                return SiegeOrchestrationPhase.ReliefHold;

            // Cohesion penalty (ai.js 2004)
            if (o.CohesionPenaltyUntil > elapsed)
                return SiegeOrchestrationPhase.PostRepulse;

            // Repeated assault window: post-repulse cooldown expired, attempts 1-3 (ai.js 2023)
            if (o.PostRepulseUntil > 0f &&
                o.PostRepulseUntil <= elapsed &&
                o.RepeatedAssaultAttempts > 0 &&
                o.RepeatedAssaultAttempts < 4)
            {
                return SiegeOrchestrationPhase.RepeatedAssault;
            }

            // Mid-siege supply chain collapse (ai.js 2054)
            if (o.SupplyReadyButCollapsed)
                return SiegeOrchestrationPhase.SupplyCollapse;

            // All gates clear: issue attack (ai.js 2070)
            return SiegeOrchestrationPhase.Assaulting;
        }
    }
}
