using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Canonical Verdant Warden support profile. Browser reference:
    /// simulation.js DEFAULT_VERDANT_WARDEN_SUPPORT and getVerdantWardenSupportProfile.
    /// The per-tick bonus fields store the additive delta each contributing warden adds
    /// before the stack cap is applied.
    /// </summary>
    public struct VerdantWardenComponent : IComponentData
    {
        public float SupportRadius;
        public int MaxSupportStack;
        public float LoyaltyBonusPerTick;
        public float HealingBonusPerTick;
        public float StabilizationBonusPerTick;
        public float ReserveMusterBonusPerTick;
        public float DefenderAttackBonusPerTick;
        public float LoyaltyProtectionBonusPerTick;
        public int DesiredFrontlineBonusWhenActive;
    }

    public struct VerdantWardenCoverageProfile
    {
        public int Count;
        public int CappedCount;
        public float DefenderAttackMultiplier;
        public float ReserveHealMultiplier;
        public float ReserveMusterMultiplier;
        public float LoyaltyProtectionMultiplier;
        public float LoyaltyGainMultiplier;
        public float StabilizationMultiplier;
        public float LoyaltyBonusPerTick;
        public int DesiredFrontlineBonus;
    }

    public static class VerdantWardenRules
    {
        public static readonly FixedString64Bytes UnitTypeId = new FixedString64Bytes("verdant_warden");

        public const float DefaultSupportRadius = 184f;
        public const int DefaultMaxSupportStack = 3;
        public const float DefaultLoyaltyBonusPerTick = 0.14f;
        public const float DefaultHealingBonusPerTick = 0.08f;
        public const float DefaultStabilizationBonusPerTick = 0.18f;
        public const float DefaultReserveMusterBonusPerTick = 0.10f;
        public const float DefaultDefenderAttackBonusPerTick = 0.06f;
        public const float DefaultLoyaltyProtectionBonusPerTick = 0.18f;
        public const int DefaultDesiredFrontlineBonusWhenActive = 1;
        public const float ControlPointRadiusScale = 0.78f;
        public const float ControlPointRadiusPadding = 44f;

        public static VerdantWardenComponent CreateDefaultComponent()
        {
            return new VerdantWardenComponent
            {
                SupportRadius = DefaultSupportRadius,
                MaxSupportStack = DefaultMaxSupportStack,
                LoyaltyBonusPerTick = DefaultLoyaltyBonusPerTick,
                HealingBonusPerTick = DefaultHealingBonusPerTick,
                StabilizationBonusPerTick = DefaultStabilizationBonusPerTick,
                ReserveMusterBonusPerTick = DefaultReserveMusterBonusPerTick,
                DefenderAttackBonusPerTick = DefaultDefenderAttackBonusPerTick,
                LoyaltyProtectionBonusPerTick = DefaultLoyaltyProtectionBonusPerTick,
                DesiredFrontlineBonusWhenActive = DefaultDesiredFrontlineBonusWhenActive,
            };
        }

        public static bool IsVerdantWarden(in UnitTypeComponent unitType)
        {
            return unitType.TypeId.Equals(UnitTypeId);
        }

        public static float ResolveControlPointSupportRadius(
            in ControlPointComponent controlPoint,
            float tileSize,
            float supportRadius)
        {
            return math.max(
                supportRadius * ControlPointRadiusScale,
                math.max(0f, controlPoint.RadiusTiles) * math.max(1f, tileSize) + ControlPointRadiusPadding);
        }

        public static VerdantWardenCoverageProfile ResolveProfile(
            in VerdantWardenComponent component,
            int rawCount)
        {
            int safeCount = math.max(0, rawCount);
            int cappedCount = math.min(math.max(0, component.MaxSupportStack), safeCount);
            if (cappedCount <= 0)
            {
                return DefaultCoverage;
            }

            return new VerdantWardenCoverageProfile
            {
                Count = safeCount,
                CappedCount = cappedCount,
                DefenderAttackMultiplier = 1f + cappedCount * component.DefenderAttackBonusPerTick,
                ReserveHealMultiplier = 1f + cappedCount * component.HealingBonusPerTick,
                ReserveMusterMultiplier = 1f + cappedCount * component.ReserveMusterBonusPerTick,
                LoyaltyProtectionMultiplier = 1f + cappedCount * component.LoyaltyProtectionBonusPerTick,
                LoyaltyGainMultiplier = 1f + cappedCount * component.LoyaltyBonusPerTick,
                StabilizationMultiplier = 1f + cappedCount * component.StabilizationBonusPerTick,
                LoyaltyBonusPerTick = cappedCount * component.LoyaltyBonusPerTick,
                DesiredFrontlineBonus = component.DesiredFrontlineBonusWhenActive,
            };
        }

        public static VerdantWardenCoverageProfile ResolveControlPointSupport(in ControlPointComponent controlPoint)
        {
            if (controlPoint.VerdantWardenCappedCount <= 0)
            {
                return DefaultCoverage;
            }

            return new VerdantWardenCoverageProfile
            {
                Count = math.max(0, controlPoint.VerdantWardenCount),
                CappedCount = math.max(0, controlPoint.VerdantWardenCappedCount),
                DefenderAttackMultiplier = 1f,
                ReserveHealMultiplier = 1f,
                ReserveMusterMultiplier = 1f,
                LoyaltyProtectionMultiplier = math.max(1f, controlPoint.VerdantWardenLoyaltyProtectionMultiplier),
                LoyaltyGainMultiplier = math.max(1f, controlPoint.VerdantWardenLoyaltyGainMultiplier),
                StabilizationMultiplier = math.max(1f, controlPoint.VerdantWardenStabilizationMultiplier),
                LoyaltyBonusPerTick = math.max(0f, controlPoint.VerdantWardenLoyaltyBonusPerTick),
                DesiredFrontlineBonus = 0,
            };
        }

        public static VerdantWardenCoverageProfile ResolveFortificationSupport(in FortificationComponent fortification)
        {
            if (fortification.VerdantWardenCappedCount <= 0)
            {
                return DefaultCoverage;
            }

            return new VerdantWardenCoverageProfile
            {
                Count = math.max(0, fortification.VerdantWardenCount),
                CappedCount = math.max(0, fortification.VerdantWardenCappedCount),
                DefenderAttackMultiplier = math.max(1f, fortification.VerdantWardenDefenderAttackMultiplier),
                ReserveHealMultiplier = math.max(1f, fortification.VerdantWardenReserveHealMultiplier),
                ReserveMusterMultiplier = math.max(1f, fortification.VerdantWardenReserveMusterMultiplier),
                LoyaltyProtectionMultiplier = 1f,
                LoyaltyGainMultiplier = 1f,
                StabilizationMultiplier = 1f,
                LoyaltyBonusPerTick = 0f,
                DesiredFrontlineBonus = math.max(0, fortification.VerdantWardenDesiredFrontlineBonus),
            };
        }

        public static void ApplyTo(
            ref ControlPointComponent controlPoint,
            in VerdantWardenCoverageProfile profile)
        {
            controlPoint.VerdantWardenCount = profile.Count;
            controlPoint.VerdantWardenCappedCount = profile.CappedCount;
            controlPoint.VerdantWardenLoyaltyBonusPerTick = profile.LoyaltyBonusPerTick;
            controlPoint.VerdantWardenLoyaltyGainMultiplier = profile.LoyaltyGainMultiplier;
            controlPoint.VerdantWardenLoyaltyProtectionMultiplier = profile.LoyaltyProtectionMultiplier;
            controlPoint.VerdantWardenStabilizationMultiplier = profile.StabilizationMultiplier;
        }

        public static void ApplyTo(
            ref FortificationComponent fortification,
            in VerdantWardenCoverageProfile profile)
        {
            fortification.VerdantWardenCount = profile.Count;
            fortification.VerdantWardenCappedCount = profile.CappedCount;
            fortification.VerdantWardenDesiredFrontlineBonus = profile.DesiredFrontlineBonus;
            fortification.VerdantWardenDefenderAttackMultiplier = profile.DefenderAttackMultiplier;
            fortification.VerdantWardenReserveHealMultiplier = profile.ReserveHealMultiplier;
            fortification.VerdantWardenReserveMusterMultiplier = profile.ReserveMusterMultiplier;
        }

        public static bool TryGetFrontlineSupport(
            EntityManager entityManager,
            Entity combatantEntity,
            out VerdantWardenCoverageProfile profile)
        {
            profile = DefaultCoverage;
            if (combatantEntity == Entity.Null ||
                !entityManager.Exists(combatantEntity) ||
                !entityManager.HasComponent<FortificationSettlementLinkComponent>(combatantEntity) ||
                !entityManager.HasComponent<FortificationReserveAssignmentComponent>(combatantEntity))
            {
                return false;
            }

            var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(combatantEntity);
            if (assignment.Duty != ReserveDutyState.Engaged &&
                assignment.Duty != ReserveDutyState.Muster)
            {
                return false;
            }

            var link = entityManager.GetComponentData<FortificationSettlementLinkComponent>(combatantEntity);
            if (link.SettlementEntity == Entity.Null ||
                !entityManager.Exists(link.SettlementEntity) ||
                !entityManager.HasComponent<FortificationComponent>(link.SettlementEntity))
            {
                return false;
            }

            profile = ResolveFortificationSupport(
                entityManager.GetComponentData<FortificationComponent>(link.SettlementEntity));
            return profile.CappedCount > 0;
        }

        public static readonly VerdantWardenCoverageProfile DefaultCoverage =
            new VerdantWardenCoverageProfile
            {
                Count = 0,
                CappedCount = 0,
                DefenderAttackMultiplier = 1f,
                ReserveHealMultiplier = 1f,
                ReserveMusterMultiplier = 1f,
                LoyaltyProtectionMultiplier = 1f,
                LoyaltyGainMultiplier = 1f,
                StabilizationMultiplier = 1f,
                LoyaltyBonusPerTick = 0f,
                DesiredFrontlineBonus = 0,
            };
    }
}
