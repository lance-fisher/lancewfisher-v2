using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Runtime siege-support state for siege engines, engineers, and supply wagons.
    /// Mirrors the browser-side expiry-window logistics model without requiring
    /// spawn-time integration changes in shared bootstrap files.
    /// </summary>
    public struct SiegeSupportComponent : IComponentData
    {
        public double SuppliedUntil;
        public double EngineerSupportUntil;
        public double LastRefreshAt;
        public int RefreshCount;
        public bool IsSiegeEngine;
        public bool IsEngineer;
        public bool IsSupplyWagon;
        public bool HasLinkedSupplyCamp;
        public bool HasSupplyTrainSupport;
        public bool HasEngineerSupport;
        public float OperationalAttackMultiplier;
        public float OperationalSpeedMultiplier;
        public SiegeSupportStatus Status;
    }

    public enum SiegeSupportStatus : byte
    {
        None = 0,
        Idle = 1,
        Supporting = 2,
        CutOff = 3,
        Starved = 4,
        Repair = 5,
        Interdicted = 6,
        RecoveringUnscreened = 7,
        RecoveringScreened = 8,
    }
}
