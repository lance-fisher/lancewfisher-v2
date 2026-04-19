using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Evaluates the AI build priority decision tree each frame and writes
    /// AIBuildOrderComponent.NextBuildOp when AIStrategyComponent.BuildTimer fires.
    ///
    /// Responsibilities:
    ///   1. Check AIStrategyComponent.BuildTimer (decremented by
    ///      AIStrategicPressureSystem). If timer > 0, clear NextBuildOp and return.
    ///   2. If HasBuilder is false, write None and reset the timer.
    ///   3. Otherwise evaluate the 13-step build priority chain in exact ai.js
    ///      if-else order and write the first match to NextBuildOp.
    ///   4. Reset BuildTimer to 4s (playerKeepFortified) or 6s (default).
    ///
    /// Actual building placement (attemptPlaceBuilding) is deferred to a future
    /// integration pass; this system ports the scheduling and decision logic only.
    ///
    /// Browser reference: ai.js updateEnemyAi buildTimer<=0 block ~lines 1377-1573.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIBuildOrderSystem : ISystem
    {
        // Timer reset values (ai.js line 1573).
        private const float BuildTimerResetDefault       = 6f;
        private const float BuildTimerResetFortifiedKeep = 4f;

        // Faith intensity thresholds.
        private const float CovenantHallIntensityThreshold    = 26f;
        private const float GrandSanctuaryIntensityThreshold  = 48f;
        private const float ApexCovenantIntensityThreshold    = 80f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIBuildOrderComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (strategy, buildOrder) in
                SystemAPI.Query<RefRW<AIStrategyComponent>, RefRW<AIBuildOrderComponent>>())
            {
                ref var s = ref strategy.ValueRW;
                ref var b = ref buildOrder.ValueRW;

                if (s.BuildTimer > 0f)
                {
                    b.NextBuildOp = BuildOrderKind.None;
                    continue;
                }

                // Timer has fired: evaluate priority chain, then reset.
                b.NextBuildOp = b.HasBuilder
                    ? EvaluateBuildPriority(ref b, s.PlayerKeepFortified, s.PlayerDivineRightActive)
                    : BuildOrderKind.None;

                s.BuildTimer = s.PlayerKeepFortified
                    ? BuildTimerResetFortifiedKeep
                    : BuildTimerResetDefault;
            }
        }

        // ------------------------------------------------------------------ priority chain

        /// <summary>
        /// Mirrors the ai.js if-else build priority chain in exact order.
        /// Returns the first BuildOrderKind whose gate conditions are met,
        /// or None if no branch fires.
        /// </summary>
        private static BuildOrderKind EvaluateBuildPriority(
            ref AIBuildOrderComponent b,
            bool playerKeepFortified,
            bool playerDivineRightActive)
        {
            // 1. Barracks (no barracks AND can afford).
            if (!b.HasBarracks && b.CanAffordBarracks)
                return BuildOrderKind.Barracks;

            // 2. Wayshrine (faith faction, has barracks, no wayshrine, can afford).
            if (b.EnemyHasFaith && b.HasBarracks && !b.HasWayshrine && b.CanAffordWayshrine)
                return BuildOrderKind.Wayshrine;

            // 3. Quarry (keep fortified, has barracks, no quarry, can afford).
            if (playerKeepFortified && b.HasBarracks && !b.HasQuarry && b.CanAffordQuarry)
                return BuildOrderKind.Quarry;

            // 4. Iron mine (keep fortified, has barracks, no iron mine, can afford).
            if (playerKeepFortified && b.HasBarracks && !b.HasIronMine && b.CanAffordIronMine)
                return BuildOrderKind.IronMine;

            // 5. Siege workshop (keep fortified, barracks+quarry+iron mine present, can afford).
            if (playerKeepFortified && b.HasBarracks && b.HasQuarry && b.HasIronMine
                && !b.HasSiegeWorkshop && b.CanAffordSiegeWorkshop)
                return BuildOrderKind.SiegeWorkshop;

            // 6. Covenant hall (faith, wayshrine completed, intensity >= 26, can afford).
            if (b.EnemyHasFaith && b.WayshrineCompleted && !b.HasCovenantHall
                && b.FaithIntensity >= CovenantHallIntensityThreshold && b.CanAffordCovenantHall)
                return BuildOrderKind.CovenantHall;

            // 7. Grand sanctuary (faith, covenant hall completed, urgency gate, can afford).
            //    Urgency: intensity >= 48 OR active covenant/divine right pressure.
            bool sanctuaryUrgencyMet = b.FaithIntensity >= GrandSanctuaryIntensityThreshold
                || b.ActiveCovenantTest
                || b.PlayerCovenantActive
                || playerDivineRightActive;
            if (b.EnemyHasFaith && b.CovenantHallCompleted && !b.HasGrandSanctuary
                && sanctuaryUrgencyMet && b.CanAffordGrandSanctuary)
                return BuildOrderKind.GrandSanctuary;

            // 8. Apex covenant (faith, test passed, grand sanctuary completed, intensity >= 80).
            if (b.EnemyHasFaith && b.CovenantTestPassed && b.GrandSanctuaryCompleted
                && !b.HasApexCovenant && b.FaithIntensity >= ApexCovenantIntensityThreshold
                && b.CanAffordApexCovenant)
                return BuildOrderKind.ApexCovenant;

            // 9. Supply camp (keep fortified, siege workshop completed, can afford).
            if (playerKeepFortified && b.SiegeWorkshopCompleted && !b.HasSupplyCamp
                && b.CanAffordSupplyCamp)
                return BuildOrderKind.SupplyCamp;

            // 10. Stable (keep fortified, supply camp completed, has engineer + wagon, can afford).
            if (playerKeepFortified && b.SupplyCampCompleted && b.HasEngineerCorps
                && b.HasSupplyWagon && !b.HasStable && b.CanAffordStable)
                return BuildOrderKind.Stable;

            // 11. Dwelling (population cap nearly full, fewer than 4 houses).
            if (b.PopulationCapAvailable <= 1 && b.HouseCount < 4)
                return BuildOrderKind.Dwelling;

            // 12. Farm (food below population threshold, fewer than 3 farms).
            if (b.FoodStock < b.PopulationTotal + 4 && b.FarmCount < 3)
                return BuildOrderKind.Farm;

            // 13. Well (water below population threshold, fewer than 3 wells).
            if (b.WaterStock < b.PopulationTotal + 4 && b.WellCount < 3)
                return BuildOrderKind.Well;

            return BuildOrderKind.None;
        }
    }
}
