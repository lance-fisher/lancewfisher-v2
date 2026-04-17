using Bloodlines.Components;

namespace Bloodlines.Conviction
{
    /// <summary>
    /// Canonical conviction band effect multipliers. Direct 1:1 port of the browser
    /// runtime table (simulation.js:1849 CONVICTION_BAND_EFFECTS).
    ///
    /// Bands are resolved from score thresholds in data/conviction-states.json:
    ///   Apex Moral  >=  75
    ///   Moral       >=  25
    ///   Neutral     >= -24
    ///   Cruel       >= -74
    ///   Apex Cruel   <  -74
    /// </summary>
    public readonly struct ConvictionBandEffects
    {
        public readonly float StabilizationMultiplier;
        public readonly float ReserveHealMultiplier;
        public readonly float LoyaltyProtectionMultiplier;
        public readonly float CaptureMultiplier;
        public readonly float PopulationGrowthMultiplier;
        public readonly float AttackMultiplier;

        public ConvictionBandEffects(
            float stabilizationMultiplier,
            float reserveHealMultiplier,
            float loyaltyProtectionMultiplier,
            float captureMultiplier,
            float populationGrowthMultiplier,
            float attackMultiplier)
        {
            StabilizationMultiplier = stabilizationMultiplier;
            ReserveHealMultiplier = reserveHealMultiplier;
            LoyaltyProtectionMultiplier = loyaltyProtectionMultiplier;
            CaptureMultiplier = captureMultiplier;
            PopulationGrowthMultiplier = populationGrowthMultiplier;
            AttackMultiplier = attackMultiplier;
        }

        public static ConvictionBandEffects ForBand(ConvictionBand band)
        {
            switch (band)
            {
                case ConvictionBand.ApexMoral:
                    return new ConvictionBandEffects(1.22f, 1.12f, 1.18f, 0.94f, 1.08f, 1f);
                case ConvictionBand.Moral:
                    return new ConvictionBandEffects(1.08f, 1.04f, 1.08f, 0.98f, 1.03f, 1f);
                case ConvictionBand.Cruel:
                    return new ConvictionBandEffects(0.96f, 0.98f, 0.94f, 1.08f, 0.97f, 1.04f);
                case ConvictionBand.ApexCruel:
                    return new ConvictionBandEffects(0.88f, 0.92f, 0.82f, 1.22f, 0.92f, 1.12f);
                case ConvictionBand.Neutral:
                default:
                    return new ConvictionBandEffects(1f, 1f, 1f, 1f, 1f, 1f);
            }
        }
    }
}
