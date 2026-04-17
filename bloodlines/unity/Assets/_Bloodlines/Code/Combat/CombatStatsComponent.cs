using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Runtime combat payload for battlefield units.
    /// Canonical values are sourced from unit definition attack/sight fields and copied to
    /// spawned units so combat systems do not need to touch ScriptableObjects.
    /// </summary>
    public struct CombatStatsComponent : IComponentData
    {
        public float AttackDamage;
        public float AttackRange;
        public float AttackCooldown;
        public float Sight;
        public float CooldownRemaining;
    }

    /// <summary>
    /// Runtime lookup buffer stored on the bootstrap config entity so produced units can
    /// resolve their combat stats by type id without reaching back into editor-only assets.
    /// </summary>
    public struct UnitCombatDefinitionElement : IBufferElementData
    {
        public FixedString64Bytes UnitId;
        public float AttackDamage;
        public float AttackRange;
        public float AttackCooldown;
        public float Sight;
        public float ProjectileSpeed;
        public float ProjectileMaxLifetimeSeconds;
        public float ProjectileArrivalRadius;
    }
}
