using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Dynamic buffer element on a faction entity that records an active
    /// counter-intelligence watch raised by that faction against a target.
    /// Browser counterpart: faction.dynasty.counterIntelligence[] (max 2 entries,
    /// created by createCounterIntelligenceWatch in simulation.js ~10837-10874).
    ///
    /// Duration: 150 real-seconds from creation time (watchDuration param in browser).
    /// Max simultaneous watches per faction: 2 (browser slices to [:2]).
    /// </summary>
    [InternalBufferCapacity(2)]
    public struct DynastyCounterIntelligenceWatchElement : IBufferElementData
    {
        public FixedString32Bytes TargetFactionId;
        public float              WatchExpiresAtElapsed;   // real-seconds elapsed at expiry
    }
}
