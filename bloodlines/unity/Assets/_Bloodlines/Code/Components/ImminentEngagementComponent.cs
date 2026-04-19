using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Browser reference: simulation.js createDefaultImminentEngagementState (11448-11481).
    /// Per-settlement warning-window state for the fortification/siege lane.
    /// </summary>
    public struct ImminentEngagementComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public bool Active;
        public bool WindowConsumed;
        public FixedString64Bytes HostileFactionId;
        public int HostileCount;
        public int HostileSiegeCount;
        public int HostileScoutCount;
        public int WatchtowerCount;
        public float WarningRadius;
        public float TotalSeconds;
        public float ExpiresAt;
        public float RemainingSeconds;
        public float LocalLoyalty;
        public float LocalLoyaltyMin;
        public bool CommanderPresent;
        public bool CommanderRecallAvailable;
        public bool GovernorPresent;
        public bool BloodlineAtRisk;
        public FixedString32Bytes SelectedResponseId;
        public FixedString64Bytes SelectedResponseLabel;
        public float ReinforcementsCommittedUntil;
        public float StartedAt;
        public float EngagedAt;
        public float LastActivationAt;
        public float BloodlineProtectionUntil;
        public float CommanderRecallIssuedAt;
        public bool IsPrimaryDynasticKeep;
    }
}
