using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Mathematics;

namespace Bloodlines.Combat
{
    public static class CommanderAuraCanon
    {
        public const float BaseAuraRadius = 126f;
        public const float MaxAttackMultiplier = 1.35f;
        public const float BaseSpeedBonus = 0.025f;
        public const float SpeedBonusPerRenown = 0.0015f;
        public const float MaxSpeedBonus = 0.15f;
        public const float BaseMoraleBonus = 0.015f;
        public const float MoraleBonusPerRenown = 0.001f;
        public const float MaxMoraleBonus = 0.12f;

        public readonly struct DoctrineAuraProfile
        {
            public DoctrineAuraProfile(
                float auraAttackMultiplier,
                float auraRadiusBonus,
                float auraSightBonus)
            {
                AuraAttackMultiplier = auraAttackMultiplier;
                AuraRadiusBonus = auraRadiusBonus;
                AuraSightBonus = auraSightBonus;
            }

            public float AuraAttackMultiplier { get; }
            public float AuraRadiusBonus { get; }
            public float AuraSightBonus { get; }
        }

        public static DoctrineAuraProfile ResolveDoctrineProfile(
            CovenantId covenantId,
            DoctrinePath doctrinePath)
        {
            if (covenantId == CovenantId.None || doctrinePath == DoctrinePath.Unassigned)
            {
                return new DoctrineAuraProfile(1f, 0f, 0f);
            }

            return covenantId switch
            {
                CovenantId.OldLight => doctrinePath == DoctrinePath.Dark
                    ? new DoctrineAuraProfile(1.14f, 10f, 12f)
                    : new DoctrineAuraProfile(1.05f, 18f, 20f),
                CovenantId.BloodDominion => doctrinePath == DoctrinePath.Dark
                    ? new DoctrineAuraProfile(1.18f, 8f, 8f)
                    : new DoctrineAuraProfile(1.08f, 14f, 10f),
                CovenantId.TheOrder => doctrinePath == DoctrinePath.Dark
                    ? new DoctrineAuraProfile(1.11f, 12f, 10f)
                    : new DoctrineAuraProfile(1.04f, 20f, 16f),
                CovenantId.TheWild => doctrinePath == DoctrinePath.Dark
                    ? new DoctrineAuraProfile(1.16f, 14f, 14f)
                    : new DoctrineAuraProfile(1.06f, 22f, 22f),
                _ => new DoctrineAuraProfile(1f, 0f, 0f),
            };
        }

        public static float ResolveConvictionBandMultiplier(ConvictionBand band)
        {
            // Use the existing conviction combat pressure scalar as the first commander-aura
            // band multiplier surface until the browser exposes a dedicated aura table.
            return math.max(1f, ConvictionBandEffects.ForBand(band).AttackMultiplier);
        }

        public static CommanderAuraComponent ResolveAura(
            float renown,
            DoctrineAuraProfile doctrine,
            ConvictionBand band)
        {
            float bandMultiplier = ResolveConvictionBandMultiplier(band);
            float auraRadius = (BaseAuraRadius + doctrine.AuraRadiusBonus + math.max(0f, renown)) * bandMultiplier;
            float attackMultiplier = math.min(
                MaxAttackMultiplier,
                ((1f + math.max(0f, renown) * 0.006f) * doctrine.AuraAttackMultiplier) * bandMultiplier);
            float sightBonus = math.round((doctrine.AuraSightBonus + math.round(math.max(0f, renown) * 0.5f)) * bandMultiplier);
            float speedBonus = math.min(
                MaxSpeedBonus,
                (BaseSpeedBonus + math.max(0f, renown) * SpeedBonusPerRenown) * bandMultiplier);
            float moraleBonus = math.min(
                MaxMoraleBonus,
                (BaseMoraleBonus + math.max(0f, renown) * MoraleBonusPerRenown) * bandMultiplier);

            return new CommanderAuraComponent
            {
                AuraRadius = auraRadius,
                AttackBonus = math.max(0f, attackMultiplier - 1f),
                SightBonus = math.max(0f, sightBonus),
                SpeedBonus = math.max(0f, speedBonus),
                MoraleBonus = math.max(0f, moraleBonus),
                ConvictionBandMultiplier = bandMultiplier,
            };
        }
    }
}
