using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-unit projectile launch parameters copied from canonical unit definitions during
    /// bootstrap and production so ranged combat can stay data-driven at runtime.
    /// </summary>
    public struct ProjectileFactoryComponent : IComponentData
    {
        public float ProjectileSpeed;
        public float ProjectileMaxLifetimeSeconds;
        public float ProjectileArrivalRadius;
    }
}
