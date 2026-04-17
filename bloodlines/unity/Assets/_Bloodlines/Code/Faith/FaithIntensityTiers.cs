namespace Bloodlines.Faith
{
    /// <summary>
    /// Canonical faith intensity tiers. Direct 1:1 port of the browser runtime
    /// FAITH_INTENSITY_TIERS table (simulation.js:179). Tier levels are
    /// progressive thresholds, highest first.
    /// </summary>
    public static class FaithIntensityTiers
    {
        public const float ApexMin = 80f;
        public const float FerventMin = 60f;
        public const float DevoutMin = 40f;
        public const float ActiveMin = 20f;
        public const float LatentMin = 1f;

        public const int UnawakenedLevel = 0;
        public const int LatentLevel = 1;
        public const int ActiveLevel = 2;
        public const int DevoutLevel = 3;
        public const int FerventLevel = 4;
        public const int ApexLevel = 5;

        public const float CommitmentExposureThreshold = 100f;
        public const float StartingCommitmentIntensity = 20f;
        public const float IntensityMax = 100f;

        public static int ResolveLevel(float intensity)
        {
            if (intensity >= ApexMin) return ApexLevel;
            if (intensity >= FerventMin) return FerventLevel;
            if (intensity >= DevoutMin) return DevoutLevel;
            if (intensity >= ActiveMin) return ActiveLevel;
            if (intensity >= LatentMin) return LatentLevel;
            return UnawakenedLevel;
        }
    }
}
