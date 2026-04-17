using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugSetStance(Entity unit, CombatStance stance, float lowHealthThreshold = 0.25f)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unit) ||
                !entityManager.HasComponent<UnitTypeComponent>(unit))
            {
                return false;
            }

            var stanceComponent = new CombatStanceComponent
            {
                Stance = stance,
                LowHealthRetreatThreshold = lowHealthThreshold > 0f
                    ? lowHealthThreshold
                    : CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold,
            };

            if (entityManager.HasComponent<CombatStanceComponent>(unit))
            {
                entityManager.SetComponentData(unit, stanceComponent);
            }
            else
            {
                entityManager.AddComponentData(unit, stanceComponent);
            }

            if (!entityManager.HasComponent<CombatStanceRuntimeComponent>(unit))
            {
                entityManager.AddComponentData(unit, default(CombatStanceRuntimeComponent));
            }

            return true;
        }

        public bool TryDebugGetStance(Entity unit, out CombatStance stance)
        {
            stance = CombatStance.PursueInRange;

            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unit) ||
                !entityManager.HasComponent<CombatStanceComponent>(unit))
            {
                return false;
            }

            stance = entityManager.GetComponentData<CombatStanceComponent>(unit).Stance;
            return true;
        }
    }
}
