namespace Bloodlines.Fortification
{
    /// <summary>
    /// Canonical imminent-engagement tuning directly ported from the browser
    /// constants block in simulation.js lines 199-205.
    /// </summary>
    public static class ImminentEngagementCanon
    {
        public static readonly float WarningBufferTiles = 4f;
        public static readonly float WatchtowerRadiusTiles = 14f;
        public static readonly float MinSeconds = 10f;
        public static readonly float MaxSeconds = 30f;
        public static readonly float KeepBaseSeconds = 14f;
        public static readonly float SettlementBaseSeconds = 11f;
        public static readonly float ReinforcementSurgeSeconds = 18f;
    }
}
