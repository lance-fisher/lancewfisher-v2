using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Movement tuning for mobile battlefield entities.
    /// Maps to the browser unit definition speed field, but stays separate from
    /// UnitTypeComponent so temporary buffs, debuffs, and doctrine effects can adjust
    /// movement without mutating the canonical unit identity payload.
    /// </summary>
    public struct MovementStatsComponent : IComponentData
    {
        public float MaxSpeed;
    }
}
