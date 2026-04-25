using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Transient embark request placed on a passenger candidate by the command
    /// surface. Consumed by EmbarkSystem during the next simulation tick.
    ///
    /// Browser runtime equivalent: the per-unit half of `embarkUnitsOnTransport`,
    /// which validates each candidate against the transport before transferring it
    /// into the transport's `embarkedUnitIds` array.
    ///
    /// EmbarkSystem removes this component on every tick that processes it
    /// (whether the order succeeds or fails), so the order is one-shot.
    /// </summary>
    public struct EmbarkOrderComponent : IComponentData
    {
        public Entity TargetTransport;
    }
}
