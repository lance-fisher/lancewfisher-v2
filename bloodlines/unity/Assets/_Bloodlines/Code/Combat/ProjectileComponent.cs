using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Runtime projectile payload for ranged and bombard attacks.
    /// Projectiles home on a live target entity while it exists; otherwise they continue toward
    /// the last recorded target position until arrival or expiry.
    /// </summary>
    public struct ProjectileComponent : IComponentData
    {
        public Entity OwnerEntity;
        public FixedString32Bytes OwnerFactionId;
        public Entity TargetEntity;
        public float3 TargetPosition;
        public float3 LaunchPosition;
        public float Damage;
        public float Speed;
        public float MaxLifetimeSeconds;
        public float ElapsedSeconds;
        public float ArrivalRadius;
    }
}
