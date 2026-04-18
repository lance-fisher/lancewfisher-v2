using Unity.Entities;

namespace Bloodlines.GameTime
{
    /// <summary>
    /// Singleton component tracking the canonical dual-clock state.
    /// Browser equivalent: state.dualClock (simulation.js ensureDualClockState, tickDualClock).
    /// One real second advances InWorldDays by DaysPerRealSecond (default 2).
    /// declareInWorldTime calls add daysDelta directly and increment DeclarationCount.
    /// </summary>
    public struct DualClockComponent : IComponentData
    {
        /// Accumulated in-world days elapsed since match start.
        public float InWorldDays;

        /// Advancement rate: in-world days per real second. Canonical default is 2.
        public float DaysPerRealSecond;

        /// Total declared in-world time jumps accumulated (browser: clock.declarations.length).
        public int DeclarationCount;
    }
}
