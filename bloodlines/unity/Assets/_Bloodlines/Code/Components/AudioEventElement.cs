using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Dynamic buffer payload produced by systems that want to trigger audio cues
    /// for downstream Unity audio systems.
    ///
    /// Fields mirror the minimal dispatch contract for the canonical audio lane:
    ///   - EventId: identifier consumed by runtime mixers / dispatchers.
    ///   - SourcePosition: world-space event location.
    ///   - FactionId: owning faction hash/identifier for team-scoped playback.
    ///   - FiredAtInWorldDays: in-world day timestamp for TTL and replay logic.
    ///   - Priority: 0 low, 1 normal, 2 high, 3 critical.
    /// </summary>
    public struct AudioEventElement : IBufferElementData
    {
        public FixedString64Bytes EventId;
        public float3 SourcePosition;
        public int FactionId;
        public float FiredAtInWorldDays;
        public byte Priority;
    }
}

