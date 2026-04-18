using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Victory
{
    public enum MatchStatus : byte
    {
        Playing = 0,
        Won     = 1,
        Lost    = 2,
    }

    public enum VictoryConditionId : byte
    {
        None                 = 0,
        CommandHallFall      = 1,
        TerritorialGovernance = 2,
        DivinRight           = 3,
    }

    /// <summary>
    /// Singleton component tracking current match outcome and victory progress.
    /// Seeded on world creation. VictoryConditionEvaluationSystem writes to it each tick.
    /// Browser reference: state.meta.status / victoryType / victoryReason,
    /// simulation.js lines 7823-7829 (command_hall_fall), 1684-1685 (territorial_governance),
    /// 10738-10739 (divine_right).
    /// </summary>
    public struct VictoryStateComponent : IComponentData
    {
        public MatchStatus       Status;
        public VictoryConditionId VictoryType;
        public FixedString32Bytes  WinnerFactionId;
        public FixedString128Bytes VictoryReason;

        // Territorial governance: seconds all held control points have been at loyalty threshold.
        // Resets to 0 whenever any falls below threshold.
        public float TerritorialGovernanceHoldSeconds;
    }
}
