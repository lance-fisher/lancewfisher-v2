using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.GameTime
{
    /// <summary>
    /// Request buffer element that any system can push onto the DualClock singleton
    /// entity to declare an in-world time jump for a named event.
    /// Browser equivalent: declareInWorldTime (simulation.js:13800).
    ///
    /// Usage: get the DualClock singleton entity, then:
    ///   var buf = em.GetBuffer&lt;DeclareInWorldTimeRequest&gt;(clockEntity);
    ///   buf.Add(new DeclareInWorldTimeRequest { DaysDelta = 30f, Reason = "Battle of the Ford" });
    ///
    /// DualClockDeclarationSystem processes all pending requests each frame, accumulates
    /// DaysDelta into DualClockComponent.InWorldDays, increments DeclarationCount, and
    /// clears the buffer.
    /// </summary>
    public struct DeclareInWorldTimeRequest : IBufferElementData
    {
        /// In-world days to add. Must be > 0 to be processed.
        public float DaysDelta;

        /// Human-readable reason for the time jump (browser: declaration.reason).
        public FixedString64Bytes Reason;
    }
}
