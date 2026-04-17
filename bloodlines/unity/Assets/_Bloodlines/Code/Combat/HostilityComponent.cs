using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction hostility buffer sourced from the map faction.hostileTo arrays.
    /// </summary>
    public struct HostilityComponent : IBufferElementData
    {
        public FixedString32Bytes HostileFactionId;
    }

    /// <summary>
    /// Bootstrap-time hostility seed so the skirmish bootstrap can populate faction buffers
    /// after the live faction entities are created.
    /// </summary>
    public struct MapFactionHostilitySeedElement : IBufferElementData
    {
        public FixedString32Bytes FactionId;
        public FixedString32Bytes HostileFactionId;
    }
}
