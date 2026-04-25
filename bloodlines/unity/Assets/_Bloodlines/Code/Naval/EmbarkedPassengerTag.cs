using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Zero-size tag attached to land units currently riding inside a transport.
    /// Marks the entity as simulation-inactive: movement, combat, and gather
    /// systems should skip any entity carrying this tag.
    ///
    /// Browser runtime equivalent: the early `if (unit.embarkedInTransportId)
    /// return;` guard in `updateUnits` (src/game/core/simulation.js:8873).
    /// In the Unity layer the tag plus the `MoveCommandComponent.IsActive=false`
    /// flag together prove the same invariant.
    /// </summary>
    public struct EmbarkedPassengerTag : IComponentData
    {
    }
}
