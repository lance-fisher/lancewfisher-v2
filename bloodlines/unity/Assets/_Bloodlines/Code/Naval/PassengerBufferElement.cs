using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// One entry per embarked passenger held on a transport-class vessel.
    /// The transport entity owns the canonical passenger list. Each passenger
    /// also carries `PassengerTransportLinkComponent` so disembark and
    /// detonation systems can reach back to their carrier in O(1).
    ///
    /// Browser runtime equivalent: the `embarkedUnitIds` array on the transport
    /// unit. The Unity layer mirrors the array as an IBufferElementData buffer
    /// so embarked entities never appear in active-unit queries.
    /// </summary>
    public struct PassengerBufferElement : IBufferElementData
    {
        public Entity Passenger;
        public FixedString64Bytes PassengerTypeId;
    }
}
