using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// World-space position for an entity on the battlefield grid.
    /// Canonical browser runtime equivalent: unit.x, unit.y (in tile-scaled world units).
    /// Matches the grid-centric positioning the browser simulation uses so that Unity
    /// scenes can be seeded directly from browser map definitions.
    /// </summary>
    public struct PositionComponent : IComponentData
    {
        public float3 Value;
    }
}
