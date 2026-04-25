using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Transient disembark request placed on a transport vessel by the command
    /// surface. Consumed by DisembarkSystem during the next simulation tick.
    ///
    /// Browser runtime equivalent: simulation.js disembarkTransport (~7576-7623).
    /// The browser walks the transport's embarkedUnitIds, scans the 8-tile ring
    /// around the transport for the first non-water tile, and drops every
    /// passenger onto that tile in a 3x3 offset grid.
    ///
    /// DisembarkSystem removes this component on every tick that processes it
    /// (whether the order succeeds or fails), so the order is one-shot. Failure
    /// modes (no passengers, no adjacent land) silently drop the order, mirroring
    /// browser semantics where the function returns ok:false without modifying
    /// state.
    /// </summary>
    public struct DisembarkOrderComponent : IComponentData
    {
    }
}
