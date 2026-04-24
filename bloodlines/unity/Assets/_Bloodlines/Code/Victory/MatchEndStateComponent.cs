using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Victory
{
    /// <summary>
    /// Singleton written by MatchEndSequenceSystem the first frame Status != Playing.
    /// Downstream systems (HUD, persistence, lobby) read this rather than querying
    /// VictoryStateComponent directly so there is one authoritative "match ended" signal.
    ///
    /// Browser equivalent: absent -- match-end result screen not in simulation.js.
    /// </summary>
    public struct MatchEndStateComponent : IComponentData
    {
        public bool IsMatchEnded;
        public FixedString32Bytes WinnerFactionId;
        public VictoryConditionId VictoryType;
        public FixedString128Bytes VictoryReason;
        public float MatchEndTimeInWorldDays;
        public bool XPAwarded;
    }
}
