using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Status of a captive on the captor faction's roster.
    ///
    /// Held: default state when a member is taken. The captive is in the
    /// captor's keep awaiting either a rescue, a ransom offer, or
    /// execution.
    ///
    /// RansomOffered: captor has signaled willingness to ransom; the
    /// captive's home faction can accept the offer. The captive remains
    /// on the buffer because the home faction has not paid yet.
    ///
    /// Released: captive has left the captor's keep, either by ransom
    /// payment or by negotiated release. The entry is retained on the
    /// buffer for audit and downstream renown / conviction queries; the
    /// captor no longer counts the captive as held.
    ///
    /// Executed: captive was put to death in the captor's keep. The
    /// entry is retained for audit (succession effects, conviction
    /// scoring, dynasty arc consequences) but the captive is no longer
    /// recoverable.
    ///
    /// The browser tracks status implicitly through array membership:
    /// captives in `faction.dynasty.captives` are Held by definition;
    /// rescued captives are removed from the array (line 4225 splice in
    /// simulation.js); ransomed captives go through similar removal
    /// paths. Unity carries status explicitly so audit-retained entries
    /// can stay on the buffer without being mistaken for currently-held
    /// captives.
    /// </summary>
    public enum CapturedMemberStatus : byte
    {
        Held          = 0,
        RansomOffered = 1,
        Released      = 2,
        Executed      = 3,
    }

    /// <summary>
    /// One entry per captured member on the captor faction's
    /// CapturedMemberElement buffer. Mirrors the browser per-faction
    /// `faction.dynasty.captives` array entries created by
    /// `transferMemberToCaptor` at simulation.js:4429-4441.
    ///
    /// Field mapping browser -> Unity:
    ///   member.id            -> MemberId
    ///   member.title         -> MemberTitle
    ///   victimFactionId      -> OriginFactionId (the captive's home
    ///                           faction; from the captor's perspective
    ///                           this is the faction the captive belongs
    ///                           to, not the captor itself)
    ///   state.meta.elapsed   -> CapturedAtInWorldDays (Unity stamps the
    ///                           DualClock InWorldDays at capture; the
    ///                           browser uses elapsed simulation seconds
    ///                           on the same clock surface)
    ///
    /// Browser-extension fields (not present at the
    /// transferMemberToCaptor call site but reserved for downstream
    /// consumers in sub-slices 23/24 captive rescue/ransom):
    ///   RansomCost: gold cost the home faction pays to ransom the
    ///                captive. Default 0 until a rescue/ransom system
    ///                lands and writes the canonical cost (browser
    ///                computes ransom inside startCaptiveRansomOperation).
    ///   Status:     CapturedMemberStatus enum (see above). Default Held
    ///                so newly captured members appear on the buffer as
    ///                actively held.
    ///
    /// The buffer is lazy-attached to the captor faction entity by
    /// CapturedMemberHelpers.CaptureMember when the first capture lands.
    /// Browser parity: the browser caps the array via slice(0, 16) at
    /// CAPTIVE_LEDGER_LIMIT (simulation.js:14, 4443). Unity does not
    /// silently trim; the buffer accepts new entries unconditionally and
    /// downstream consumers (rescue/ransom dispatch, intel reports) read
    /// the full buffer. The cap can be enforced by a future
    /// retention/eviction pass when one is needed.
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct CapturedMemberElement : IBufferElementData
    {
        public FixedString64Bytes MemberId;
        public FixedString64Bytes MemberTitle;
        public FixedString32Bytes OriginFactionId;
        public float              CapturedAtInWorldDays;
        public float              RansomCost;
        public CapturedMemberStatus Status;
    }
}
