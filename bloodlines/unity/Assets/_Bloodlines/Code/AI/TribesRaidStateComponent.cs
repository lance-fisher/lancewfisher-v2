using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-tribes-faction raid state. Attached at bootstrap time to the tribes
    /// faction entity. Tracks the canonical raid timer countdown.
    ///
    /// Browser parity: src/game/core/ai.js updateNeutralAi (~3044-3141).
    /// faction.ai.raidTimer drives the canonical 30-second base interval before
    /// raiders are dispatched. World-pressure convergence and Great Reckoning
    /// shorten the interval; Apex Cruel dark-extremes kingdoms shorten it
    /// further.
    /// </summary>
    public struct TribesRaidStateComponent : IComponentData
    {
        /// <summary>
        /// Real-seconds countdown until the next raid dispatch. Decremented by
        /// SystemAPI.Time.DeltaTime each tick. When &lt;= 0 the raid fires and
        /// the timer is reset using BaseRaidIntervalSeconds (subject to
        /// world-pressure multipliers).
        /// </summary>
        public float RaidTimerSeconds;

        /// <summary>
        /// Canonical 30-second baseline (browser ai.js:3140). Stored on the
        /// component so future tuning can adjust per-faction without touching
        /// the system.
        /// </summary>
        public float BaseRaidIntervalSeconds;
    }
}
