using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Singleton state for the browser Trueborn rise arc.
    /// Browser stores this on factions.trueborn_city.riseArc; Unity keeps it on a
    /// dedicated singleton so later recognition and HUD slices can read/write it
    /// without widening faction bootstrap seams.
    /// </summary>
    public struct TruebornRiseArcComponent : IComponentData
    {
        // 0 = dormant, 1 = banner raised, 2 = escalation, 3 = restoration.
        // Stage 4 remains reserved for future directive slices if the arc widens.
        public byte CurrentStage;
        public float StageStartedAtInWorldDays;
        // Stored as a positive daily drain magnitude applied to dynasty legitimacy.
        public float GlobalPressurePerDay;
        // Stored as a positive daily drain magnitude applied to owned loyalty.
        public float LoyaltyErosionPerDay;
        public ulong RecognizedFactionsBitmask;
        public int ChallengeLevel;
        public int UnchallengedCycles;
    }

    /// <summary>
    /// Stable faction-slot registry for the Trueborn recognition bitmask.
    /// Slots are append-only so recognition bits do not drift when kingdoms are
    /// re-queried in different orders.
    /// </summary>
    public struct TruebornRiseFactionRecognitionSlotElement : IBufferElementData
    {
        public FixedString32Bytes FactionId;
    }
}
