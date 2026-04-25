using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Transient marker placed on a fire-ship vessel by the attack resolution
    /// system on the same tick that the vessel just dealt its strike damage.
    /// Consumed by <see cref="FireShipDetonationSystem"/> the same tick to
    /// apply the canonical one-use sacrifice destruction.
    ///
    /// Browser parity: simulation.js updateVessel attack branch (~8836-8840),
    /// where the line `if (unitDef.oneUseSacrifice) { unit.health = 0; ... }`
    /// fires after the strike damage is applied.
    ///
    /// The tag is one-shot; the detonation system removes it the same tick
    /// it processes it.
    /// </summary>
    public struct FireShipDetonationPendingTag : IComponentData
    {
    }
}
