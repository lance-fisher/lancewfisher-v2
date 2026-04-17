using Unity.Entities;

namespace Bloodlines.Components
{
    public struct RecentImpactComponent : IComponentData
    {
        public float SecondsRemaining;
    }
}
