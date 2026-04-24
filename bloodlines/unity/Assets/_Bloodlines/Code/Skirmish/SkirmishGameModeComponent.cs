using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Skirmish
{
    public enum SkirmishPhase : byte
    {
        Setup     = 0,
        Playing   = 1,
        PostMatch = 2,
    }

    /// <summary>
    /// Singleton tracking the canonical phase of the active skirmish match.
    /// Created by SkirmishGameModeManagerSystem on first tick in Setup; transitions
    /// to Playing once bootstrap is complete and VictoryStateComponent is seeded,
    /// then to PostMatch when MatchEndStateComponent.IsMatchEnded becomes true.
    ///
    /// Browser equivalent: absent -- match-phase lifecycle is implicit in simulation.js;
    /// this surface makes it explicit for Unity HUD and persistence systems.
    /// </summary>
    public struct SkirmishGameModeComponent : IComponentData
    {
        public SkirmishPhase Phase;
        public byte FactionCount;
        public bool IsPlayerVsAI;
        public FixedString32Bytes PlayerFactionId;
        public float MatchStartInWorldDays;
        public float MatchEndInWorldDays;
    }
}
