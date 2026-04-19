using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Narrative message tone. Matches the browser `pushMessage` tone
    /// parameter at simulation.js (called at acceptMarriage ~7467,
    /// breakNonAggressionPact ~5251, promoteMemberToLesserHouse ~7251,
    /// proposeNonAggressionPact ~5216, and many other sites).
    /// Consumers (HUD, log panel, UI notifications) decide how to
    /// present each tone.
    /// </summary>
    public enum NarrativeMessageTone : byte
    {
        Info = 0,
        Good = 1,
        Warn = 2,
    }

    /// <summary>
    /// Singleton tag attached to the entity that carries the global
    /// narrative message buffer. Lazy-created by NarrativeMessageBridge
    /// on first push if absent.
    /// </summary>
    public struct NarrativeMessageSingleton : IComponentData { }

    /// <summary>
    /// One entry per pushed narrative message. Buffer element on the
    /// NarrativeMessageSingleton entity. Browser reference:
    /// state.messages[*] at simulation.js state construction; fields
    /// mirror the `{ id, text, tone, ttl }` shape the browser writes
    /// (minus id, which Unity derives from buffer index + timestamp
    /// for consumers that need stable ids).
    ///
    /// CreatedAtInWorldDays captures the in-world day the message was
    /// produced, mirroring the browser pattern where message queues
    /// carry an accumulation moment for chronological display.
    ///
    /// No consumer/eviction system ships in sub-slice 16; entries
    /// accumulate until a later slice lands a consumer that drains
    /// or ages them.
    /// </summary>
    public struct NarrativeMessageElement : IBufferElementData
    {
        public FixedString128Bytes Text;
        public NarrativeMessageTone Tone;
        public float CreatedAtInWorldDays;
        public float Ttl;
    }
}
