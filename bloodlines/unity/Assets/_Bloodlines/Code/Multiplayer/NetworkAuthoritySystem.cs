using Unity.Entities;
#if UNITY_NETCODE
using Unity.NetCode;
#endif

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Maintains IsServer / IsClient authority flags in NetworkGameModeComponent each frame.
    ///
    /// In offline local-game mode (IsLocalGame=true, the default) the flags are held stable
    /// at IsServer=true, IsClient=false so all skirmish vs AI logic is unaffected.
    ///
    /// In networked mode (IsLocalGame=false, written by the transport integration layer after
    /// a successful handshake) the system reads the active NfE connection state and writes the
    /// correct authority flags. Server worlds get IsServer=true; client worlds get IsClient=true.
    ///
    /// Browser equivalent: absent -- multiplayer authority not in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GhostCollectionSetupSystem))]
    public partial struct NetworkAuthoritySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkGameModeComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            ref var mode = ref SystemAPI.GetSingletonRW<NetworkGameModeComponent>().ValueRW;

            if (mode.IsLocalGame)
            {
                mode.IsServer = true;
                mode.IsClient = false;
                return;
            }

#if UNITY_NETCODE
            bool isServerWorld = state.WorldUnmanaged.IsServer();
            bool hasActiveSession = SystemAPI.HasSingleton<NetworkStreamInGame>();

            if (hasActiveSession)
            {
                mode.IsServer = isServerWorld;
                mode.IsClient = !isServerWorld;
            }
#endif
        }
    }
}
