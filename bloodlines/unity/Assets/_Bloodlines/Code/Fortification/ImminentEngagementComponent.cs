using Unity.Entities;

namespace Bloodlines.Fortification
{
    public enum ImminentEngagementPosture : byte
    {
        Steady = 0,
        Brace = 1,
        Counterstroke = 2,
    }

    /// <summary>
    /// Runtime warning-window state for fortified settlements facing near-term contact.
    /// </summary>
    public struct ImminentEngagementComponent : IComponentData
    {
        public bool Active;
        public bool WindowConsumed;
        public float WarningRadius;
        public float TotalSeconds;
        public float RemainingSeconds;
        public float ExpiresAt;
        public int HostileCount;
        public int HostileSiegeCount;
        public int HostileScoutCount;
        public int WatchtowerCount;
        public ImminentEngagementPosture SelectedPosture;
        public bool BloodlineAtRisk;
        public bool CommanderPresent;
        public bool GovernorPresent;
        public float LastActivationAt;
        public float EngagedAt;
    }
}
