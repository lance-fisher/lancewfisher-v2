using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Player-issued combat order state layered on top of the passive auto-acquire loop.
    /// Explicit target orders chase a chosen hostile while it remains valid.
    /// Attack-move orders preserve a destination and resume marching after local engagements.
    /// </summary>
    public struct AttackOrderComponent : IComponentData
    {
        public Entity ExplicitTargetEntity;
        public float3 AttackMoveDestination;
        public bool IsAttackMoveActive;
        public bool IsActive;

        public bool IsAttackMoveDestination
        {
            readonly get => IsAttackMoveActive;
            set => IsAttackMoveActive = value;
        }
    }
}
