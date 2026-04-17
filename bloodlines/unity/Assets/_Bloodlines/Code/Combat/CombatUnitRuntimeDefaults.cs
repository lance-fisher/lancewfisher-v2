namespace Bloodlines.Components
{
    public static class CombatUnitRuntimeDefaults
    {
        public const float DefaultSeparationRadius = 0.8f;
        public const float CavalrySeparationRadius = 1.2f;
        public const float SiegeSeparationRadius = 2f;
        public const float DefaultLowHealthRetreatThreshold = 0.25f;
        public const float RetreatResumeHealthFraction = 0.4f;

        public static float ResolveSeparationRadius(UnitRole role, SiegeClass siegeClass, float authoredRadius = 0f)
        {
            if (authoredRadius > 0f)
            {
                return authoredRadius;
            }

            if (siegeClass != SiegeClass.None ||
                role == UnitRole.SiegeEngine ||
                role == UnitRole.SiegeSupport)
            {
                return SiegeSeparationRadius;
            }

            if (role == UnitRole.LightCavalry)
            {
                return CavalrySeparationRadius;
            }

            return DefaultSeparationRadius;
        }

        public static CombatStance ResolveDefaultStance(UnitRole role)
        {
            return role == UnitRole.Worker
                ? CombatStance.HoldPosition
                : CombatStance.PursueInRange;
        }
    }
}
