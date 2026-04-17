using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-unit snapshot of a player-issued group movement order.
    /// The order keeps each unit marching toward an offset destination without
    /// recentering the rest of the group when members peel off or die.
    /// </summary>
    public struct GroupMovementOrderComponent : IComponentData
    {
        public FixedString32Bytes GroupId;
        public float3 GroupCenter;
        public float3 LocalOffset;
        public float3 DestinationCenter;
        public double OrderTimestamp;
        public bool IsAttackMove;
    }
}
