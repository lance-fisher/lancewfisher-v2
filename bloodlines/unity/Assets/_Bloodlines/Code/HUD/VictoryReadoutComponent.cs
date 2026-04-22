using Bloodlines.Victory;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    public enum VictoryReadoutStatus : byte
    {
        Blocked = 0,
        Building = 1,
        Ready = 2,
        Completed = 3,
    }

    /// <summary>
    /// Faction-scoped HUD summary for live victory posture.
    /// Mirrors the canonical victory singleton plus per-faction eligibility without
    /// mutating the underlying victory evaluation systems.
    /// </summary>
    public struct VictoryReadoutComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public MatchStatus MatchStatus;
        public VictoryConditionId ResolvedVictoryType;
        public bool IsWinner;
    }

    /// <summary>
    /// Per-condition victory progress read-model attached to faction root entities.
    /// </summary>
    public struct VictoryConditionReadoutElement : IBufferElementData
    {
        public VictoryConditionId ConditionId;
        public VictoryReadoutStatus Status;
        public float Progress01;
        public float TimeRemainingInWorldDays;
        public int CurrentCount;
        public int RequiredCount;
    }
}
