using Bloodlines.Multiplayer;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the current network authority state from NetworkGameModeComponent.
        /// Reflects offline defaults (IsLocalGame=true, IsServer=true, IsClient=false) until
        /// a live networked session is established.
        /// </summary>
        public bool TryDebugGetNetworkAuthority(
            out bool isServer,
            out bool isClient,
            out bool isLocalGame)
        {
            isServer    = false;
            isClient    = false;
            isLocalGame = false;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<NetworkGameModeComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            var mode    = query.GetSingleton<NetworkGameModeComponent>();
            isServer    = mode.IsServer;
            isClient    = mode.IsClient;
            isLocalGame = mode.IsLocalGame;
            return true;
        }
    }
}
