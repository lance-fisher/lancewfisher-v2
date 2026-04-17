using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Player-issued combat order state layered on top of the passive auto-acquire loop.
    /// Explicit target orders chase a chosen hostile while it remains in sight.
    /// Attack-move orders preserve a destination and resume moving after short engagements.
    /// </summary>
    public struct AttackOrderComponent : IComponentData
    {
        public Entity ExplicitTargetEntity;
        public bool IsAttackMoveDestination;
        public float3 AttackMoveDestination;
        public bool IsActive;
    }
}
