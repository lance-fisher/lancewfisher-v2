using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Runtime field-water sustainment state for mobile land units.
    /// Carries the additive strain / recovery timers plus cached base combat and
    /// movement stats so operational penalties can be applied without mutating
    /// canonical unit identity payloads.
    /// </summary>
    public struct FieldWaterComponent : IComponentData
    {
        public float BaseAttackDamage;
        public float BaseMaxSpeed;
        public double SuppliedUntil;
        public double LastTransferAt;
        public double LastSupportRefreshAt;
        public float Strain;
        public float CriticalDuration;
        public int SupportRefreshCount;
        public bool IsSupported;
        public bool AttritionActive;
        public bool DesertionRisk;
        public float OperationalAttackMultiplier;
        public float OperationalSpeedMultiplier;
        public FieldWaterStatus Status;
    }

    public enum FieldWaterStatus : byte
    {
        Steady = 0,
        Recovering = 1,
        Strained = 2,
        Critical = 3,
        Dry = 4,
    }
}
