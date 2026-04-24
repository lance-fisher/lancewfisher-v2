using Bloodlines.HUD;
using Bloodlines.Victory;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetMatchEndState(
            out bool isMatchEnded,
            out string winnerFactionId,
            out string victoryType,
            out string victoryReason,
            out float matchEndTimeInWorldDays,
            out bool xpAwarded)
        {
            isMatchEnded = false;
            winnerFactionId = string.Empty;
            victoryType = string.Empty;
            victoryReason = string.Empty;
            matchEndTimeInWorldDays = 0f;
            xpAwarded = false;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<MatchEndStateComponent>());
            if (query.IsEmptyIgnoreFilter)
                return false;

            var state = query.GetSingleton<MatchEndStateComponent>();
            isMatchEnded = state.IsMatchEnded;
            winnerFactionId = state.WinnerFactionId.ToString();
            victoryType = state.VictoryType.ToString();
            victoryReason = state.VictoryReason.ToString();
            matchEndTimeInWorldDays = state.MatchEndTimeInWorldDays;
            xpAwarded = state.XPAwarded;
            return true;
        }

        public bool TryDebugGetMatchEndHUD(
            out bool isVisible,
            out string hudWinnerFactionId,
            out string hudVictoryType,
            out bool playerXPAwarded)
        {
            isVisible = false;
            hudWinnerFactionId = string.Empty;
            hudVictoryType = string.Empty;
            playerXPAwarded = false;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<MatchEndHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
                return false;

            var hud = query.GetSingleton<MatchEndHUDComponent>();
            isVisible = hud.IsVisible;
            hudWinnerFactionId = hud.WinnerFactionId.ToString();
            hudVictoryType = hud.VictoryType.ToString();
            playerXPAwarded = hud.PlayerXPAwarded;
            return true;
        }
    }
}
