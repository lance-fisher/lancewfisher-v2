using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Active hostile target for a combat-capable unit.
    /// EngagementRange mirrors the attack range captured when the target was acquired.
    /// </summary>
    public struct AttackTargetComponent : IComponentData
    {
        public Entity TargetEntity;
        public float EngagementRange;
    }
}
