using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Canonical thresholds and XP schedules for cross-match dynasty progression.
    /// Browser equivalent: absent -- derived from canonical progression design.
    ///
    /// Tiers are sideways-customization only: each tier grants one unlock slot.
    /// No tier grants raw power over un-progressed factions in multiplayer.
    /// </summary>
    public static class DynastyProgressionCanon
    {
        public const byte MaxTier = 4;

        // Cumulative XP thresholds to reach each tier.
        public static readonly float[] TierXPThresholds = { 0f, 100f, 350f, 850f, 1850f };

        // XP awarded per match by final placement (1-based index, index 0 unused).
        // Placement 4+ all receive the floor award.
        public const float PlacementXPFirst   = 50f;
        public const float PlacementXPSecond  = 35f;
        public const float PlacementXPThird   = 20f;
        public const float PlacementXPFloor   = 10f;

        public static float XPForPlacement(byte placement)
        {
            return placement switch
            {
                1 => PlacementXPFirst,
                2 => PlacementXPSecond,
                3 => PlacementXPThird,
                _ => PlacementXPFloor,
            };
        }

        /// Returns the tier that corresponds to the given accumulated XP.
        public static byte TierForXP(float xp)
        {
            byte tier = 0;
            for (int i = 1; i < TierXPThresholds.Length; i++)
            {
                if (xp >= TierXPThresholds[i])
                {
                    tier = (byte)i;
                }
            }

            return math.min(tier, MaxTier);
        }

        /// Returns the XP threshold to reach the next tier, or float.MaxValue if at cap.
        public static float NextTierThreshold(byte currentTier)
        {
            int next = currentTier + 1;
            if (next >= TierXPThresholds.Length)
            {
                return float.MaxValue;
            }

            return TierXPThresholds[next];
        }
    }
}
