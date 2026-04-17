using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
        public float TargetAcquireIntervalSeconds;
        public float AcquireCooldownRemaining;
        public float TargetSightGraceSeconds;
        public float TargetOutOfSightSeconds;

        public readonly float ResolveTargetAcquireIntervalSeconds()
        {
            return math.max(0.05f, TargetAcquireIntervalSeconds > 0f ? TargetAcquireIntervalSeconds : 0.25f);
        }

        public readonly float ResolveTargetSightGraceSeconds()
        {
            return math.max(0.05f, TargetSightGraceSeconds > 0f ? TargetSightGraceSeconds : 0.35f);
        }
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
        public float SeparationRadius;
    }
}
