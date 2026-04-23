using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Live sabotage fallout on a targeted building. The covert-ops lane keeps the
    /// timers local to the building entity so production freeze and damage windows
    /// remain additive without widening the core production or building-type files.
    /// </summary>
    public struct PlayerSabotageStatusComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString32Bytes Subtype;
        public float AppliedAtInWorldDays;
        public float EffectExpiresAtInWorldDays;
        public float ProductionHaltExpiresAtInWorldDays;
        public float GateExposureExpiresAtInWorldDays;
        public float BurnExpiresAtInWorldDays;
        public float BurnDamagePerSecond;
        public float DamageFloorRatio;
    }
}
