using Bloodlines.Multiplayer;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetNetworkState(
            out bool isServer,
            out bool isClient,
            out bool isLocalGame,
            out byte maxPlayers,
            out ulong networkSessionId)
        {
            isServer = false;
            isClient = false;
            isLocalGame = false;
            maxPlayers = 0;
            networkSessionId = 0UL;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<NetworkGameModeComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            var mode = query.GetSingleton<NetworkGameModeComponent>();
            isServer = mode.IsServer;
            isClient = mode.IsClient;
            isLocalGame = mode.IsLocalGame;
            maxPlayers = mode.MaxPlayers;
            networkSessionId = mode.NetworkSessionId;
            return true;
        }

        public bool TryDebugGetGhostArchetypes(out string[] archetypeIds)
        {
            archetypeIds = null;
            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<NetworkGameModeComponent>(),
                ComponentType.ReadOnly<GhostPrefabArchetypeElement>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            Entity networkEntity = query.GetSingletonEntity();
            var buffer = em.GetBuffer<GhostPrefabArchetypeElement>(networkEntity, isReadOnly: true);
            archetypeIds = new string[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                archetypeIds[i] = buffer[i].ArchetypeId.ToString();
            }

            return true;
        }
    }
}
