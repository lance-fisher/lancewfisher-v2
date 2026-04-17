using Unity.Entities;

namespace Bloodlines.Components
{
    public enum CombatStance : byte
    {
        HoldPosition = 0,
        AttackMove = 1,
        PursueInRange = 2,
        RetreatOnLowHealth = 3,
    }

    public struct CombatStanceComponent : IComponentData
    {
        public CombatStance Stance;
        public float LowHealthRetreatThreshold;
    }

    public struct CombatStanceRuntimeComponent : IComponentData
    {
        public bool SuspendAutoAcquireUntilMoveStops;
        public bool IsRetreating;
    }
}
