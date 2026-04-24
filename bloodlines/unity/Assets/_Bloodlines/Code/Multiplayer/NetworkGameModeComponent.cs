using Unity.Entities;

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Singleton that records the active network session mode. Written once by
    /// NetworkBootstrapSystem at world initialization. All game logic that needs to
    /// branch on server vs client vs local reads this component.
    ///
    /// In offline skirmish vs AI sessions: IsLocalGame=true, IsServer=true, IsClient=false.
    /// In online server sessions: IsLocalGame=false, IsServer=true, IsClient=false.
    /// In online client sessions: IsLocalGame=false, IsServer=false, IsClient=true.
    ///
    /// NetworkSessionId is a ulong assigned by the server at session creation and
    /// distributed to all clients for correlation. Zero in local mode.
    ///
    /// Browser equivalent: absent -- multiplayer foundation not in simulation.js.
    /// </summary>
    public struct NetworkGameModeComponent : IComponentData
    {
        public bool IsServer;
        public bool IsClient;
        public bool IsLocalGame;
        public byte MaxPlayers;
        public ulong NetworkSessionId;
    }

    /// <summary>
    /// One entry per ghost prefab archetype registered for network replication.
    /// Written by GhostCollectionSetupSystem during initialization.
    /// When Netcode for Entities is installed, these entries drive ghost prefab
    /// collection registration. In the offline foundation slice they serve as
    /// the data-model record of intended replication scope.
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct GhostPrefabArchetypeElement : IBufferElementData
    {
        public Unity.Collections.FixedString64Bytes ArchetypeId;
        public bool IsRegistered;
    }
}
