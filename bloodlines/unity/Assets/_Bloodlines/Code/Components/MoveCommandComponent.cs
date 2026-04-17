using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Active move order for a unit or mobile battlefield entity.
    /// Browser runtime equivalent: issueMoveCommand(...) target position plus the
    /// implied "arrived" tolerance used by the steering loop.
    /// </summary>
    public struct MoveCommandComponent : IComponentData
    {
        public float3 Destination;
        public float StoppingDistance;
        public bool IsActive;
    }
}
