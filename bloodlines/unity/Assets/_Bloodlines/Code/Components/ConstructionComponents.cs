using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// First ECS construction runtime state.
    /// Buildings enter the world as placed sites, then ConstructionSystem advances
    /// them to operational structures over build time.
    /// </summary>
    public struct ConstructionStateComponent : IComponentData
    {
        public float RemainingSeconds;
        public float TotalSeconds;
        public float StartingHealth;
    }
}
