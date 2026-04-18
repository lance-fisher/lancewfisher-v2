using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical reserve-cycling and triage tuning for a fortified settlement.
    /// Browser reference: simulation.js tickFortificationReserves (11875).
    /// </summary>
    public struct FortificationReserveComponent : IComponentData
    {
        public float MusterIntervalSeconds;
        public float ReserveHealPerSecond;
        public float RetreatHealthRatio;
        public float RecoveryHealthRatio;
        public float TriageRadiusTiles;
        public double LastCommitAt;
        public bool ThreatActive;
        public int ReadyReserveCount;
        public int MusteringReserveCount;
        public int RecoveringReserveCount;
        public int FallbackReserveCount;
        public int LastCommittedCount;
    }
}
