using Unity.Entities;

namespace Bloodlines.Fortification
{
    public enum ReserveDutyState : byte
    {
        Ready = 0,
        Engaged = 1,
        Muster = 2,
        Fallback = 3,
        Recovering = 4,
    }

    /// <summary>
    /// Minimal per-unit reserve-duty state used by the fortification reserve loop.
    /// Browser reference: unit.reserveDuty in tickFortificationReserves (11875).
    /// </summary>
    public struct FortificationReserveAssignmentComponent : IComponentData
    {
        public ReserveDutyState Duty;
    }
}
