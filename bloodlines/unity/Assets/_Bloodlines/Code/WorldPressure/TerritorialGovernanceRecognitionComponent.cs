using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Live territorial-governance recognition state on a kingdom faction root.
    /// Ports the browser recognition / acceptance / coalition-pressure surface
    /// without reopening the HUD or AI-owned paths.
    /// </summary>
    public struct TerritorialGovernanceRecognitionComponent : IComponentData
    {
        public bool Active;
        public bool RecognitionEstablished;
        public bool Completed;

        public float StartedAtInWorldDays;
        public float RecognizedAtInWorldDays;
        public float CompletedAtInWorldDays;

        public float RequiredSustainSeconds;
        public float SustainedSeconds;
        public float RequiredVictorySeconds;
        public float VictoryHoldSeconds;

        public int TerritoryCount;
        public int LoyalTerritoryCount;
        public int VictoryLoyalTerritoryCount;
        public int ContestedTerritoryCount;
        public float TerritoryShare;
        public float TerritorySharePct;

        public bool StageReady;
        public bool ShareReady;
        public bool TriggerReady;
        public bool HoldReady;
        public bool IntegrationReady;
        public bool VictoryReady;

        public float PopulationAcceptancePct;
        public float PopulationAcceptanceSeedPct;
        public float PopulationAcceptanceTargetPct;
        public float PopulationAcceptanceThresholdPct;
        public float PopulationAcceptanceAllianceThresholdPct;
        public float PopulationAcceptanceGapPct;
        public float PopulationAcceptanceRiseRate;
        public float PopulationAcceptanceFallRate;
        public float PopulationAcceptanceTrendPerSecond;
        public bool PopulationAcceptanceReady;
        public bool AllianceThresholdReady;

        public bool AlliancePressureActive;
        public int AlliancePressureCycles;
        public int AlliancePressureHostileCount;
        public float AlliancePressureDrag;
        public float AlliancePressureAccumulatorSeconds;

        public FixedString32Bytes WeakestControlPointId;
        public float WeakestControlPointLoyalty;

        public int WorldPressureContribution;
    }
}
