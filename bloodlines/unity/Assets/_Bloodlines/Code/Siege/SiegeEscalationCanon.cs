namespace Bloodlines.Siege
{
    /// <summary>
    /// Canonical thresholds and multipliers for the siege escalation arc.
    /// Browser equivalent: absent -- derived from canonical siege doctrine in
    /// governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md.
    /// </summary>
    public static class SiegeEscalationCanon
    {
        // Duration thresholds (in-world days) for each tier advance.
        public const float ProlongedThresholdDays = 7f;
        public const float SevereThresholdDays = 14f;
        public const float CriticalThresholdDays = 21f;

        // Per-tier starvation multipliers applied to population decline.
        public const float NormalStarvationMultiplier = 1.0f;
        public const float ProlongedStarvationMultiplier = 1.5f;
        public const float SevereStarvationMultiplier = 2.0f;
        public const float CriticalStarvationMultiplier = 3.0f;

        // Per-tier desertion threshold (fraction of garrison HP below which desertion risk activates).
        public const float NormalDesertionThresholdPct = 0.05f;
        public const float ProlongedDesertionThresholdPct = 0.10f;
        public const float SevereDesertionThresholdPct = 0.20f;
        public const float CriticalDesertionThresholdPct = 0.35f;

        // Per-tier loyalty penalty per in-world day applied to the garrison faction.
        public const float NormalMoralePenaltyPerDay = 0.0f;
        public const float ProlongedMoralePenaltyPerDay = 1.0f;
        public const float SevereMoralePenaltyPerDay = 2.5f;
        public const float CriticalMoralePenaltyPerDay = 5.0f;

        public static byte ResolveTier(float durationDays)
        {
            if (durationDays >= CriticalThresholdDays)
            {
                return (byte)SiegeEscalationTier.Critical;
            }

            if (durationDays >= SevereThresholdDays)
            {
                return (byte)SiegeEscalationTier.Severe;
            }

            if (durationDays >= ProlongedThresholdDays)
            {
                return (byte)SiegeEscalationTier.Prolonged;
            }

            return (byte)SiegeEscalationTier.Normal;
        }

        public static float ResolveStarvationMultiplier(byte tier)
        {
            return tier switch
            {
                (byte)SiegeEscalationTier.Critical => CriticalStarvationMultiplier,
                (byte)SiegeEscalationTier.Severe => SevereStarvationMultiplier,
                (byte)SiegeEscalationTier.Prolonged => ProlongedStarvationMultiplier,
                _ => NormalStarvationMultiplier,
            };
        }

        public static float ResolveDesertionThresholdPct(byte tier)
        {
            return tier switch
            {
                (byte)SiegeEscalationTier.Critical => CriticalDesertionThresholdPct,
                (byte)SiegeEscalationTier.Severe => SevereDesertionThresholdPct,
                (byte)SiegeEscalationTier.Prolonged => ProlongedDesertionThresholdPct,
                _ => NormalDesertionThresholdPct,
            };
        }

        public static float ResolveMoralePenaltyPerDay(byte tier)
        {
            return tier switch
            {
                (byte)SiegeEscalationTier.Critical => CriticalMoralePenaltyPerDay,
                (byte)SiegeEscalationTier.Severe => SevereMoralePenaltyPerDay,
                (byte)SiegeEscalationTier.Prolonged => ProlongedMoralePenaltyPerDay,
                _ => NormalMoralePenaltyPerDay,
            };
        }

        public static SiegeEscalationComponent BuildComponent(
            float durationDays,
            float lastTickInWorldDays,
            Unity.Collections.FixedString32Bytes ownerFactionId)
        {
            byte tier = ResolveTier(durationDays);
            return new SiegeEscalationComponent
            {
                SiegeDurationInWorldDays = durationDays,
                EscalationTier = tier,
                StarvationMultiplier = ResolveStarvationMultiplier(tier),
                DesertionThresholdPct = ResolveDesertionThresholdPct(tier),
                MoralePenaltyPerDay = ResolveMoralePenaltyPerDay(tier),
                LastTickInWorldDays = lastTickInWorldDays,
                OwnerFactionId = ownerFactionId,
            };
        }
    }
}
