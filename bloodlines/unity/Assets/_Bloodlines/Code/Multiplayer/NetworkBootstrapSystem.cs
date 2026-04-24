using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Initializes the network session singleton on world creation.
    /// In offline skirmish vs AI sessions the component defaults to local mode:
    /// IsLocalGame=true, IsServer=true, MaxPlayers=2.
    /// A future multiplayer transport integration layer replaces IsServer/IsClient
    /// and writes NetworkSessionId after a network handshake.
    ///
    /// Browser equivalent: absent -- multiplayer foundation not in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct NetworkBootstrapSystem : ISystem
    {
        private bool initialized;

        public void OnCreate(ref SystemState state)
        {
            initialized = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            var em = state.EntityManager;

            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<NetworkGameModeComponent>());
            if (!query.IsEmptyIgnoreFilter)
            {
                return;
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity networkEntity = ecb.CreateEntity();
            ecb.AddComponent(networkEntity, new NetworkGameModeComponent
            {
                IsServer = true,
                IsClient = false,
                IsLocalGame = true,
                MaxPlayers = 2,
                NetworkSessionId = 0UL,
            });
            ecb.AddBuffer<GhostPrefabArchetypeElement>(networkEntity);
            ecb.Playback(em);
        }
    }
}
