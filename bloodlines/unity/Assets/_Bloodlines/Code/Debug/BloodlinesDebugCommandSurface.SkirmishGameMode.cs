using Bloodlines.Skirmish;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSkirmishGameMode(
            out string phase,
            out int factionCount,
            out bool isPlayerVsAI,
            out string playerFactionId,
            out float matchStartInWorldDays,
            out float matchEndInWorldDays)
        {
            phase = string.Empty;
            factionCount = 0;
            isPlayerVsAI = false;
            playerFactionId = string.Empty;
            matchStartInWorldDays = 0f;
            matchEndInWorldDays = 0f;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<SkirmishGameModeComponent>());
            if (query.IsEmptyIgnoreFilter)
                return false;

            var component = query.GetSingleton<SkirmishGameModeComponent>();
            phase = component.Phase.ToString();
            factionCount = component.FactionCount;
            isPlayerVsAI = component.IsPlayerVsAI;
            playerFactionId = component.PlayerFactionId.ToString();
            matchStartInWorldDays = component.MatchStartInWorldDays;
            matchEndInWorldDays = component.MatchEndInWorldDays;
            return true;
        }
    }
}
