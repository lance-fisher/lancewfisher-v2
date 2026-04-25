using Unity.Collections;
using Unity.Entities;
#if UNITY_NETCODE
using Unity.NetCode;
#endif

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Registers canonical ghost prefab archetypes after NetworkBootstrapSystem
    /// has created the network singleton. Three archetypes are registered:
    ///   - "archetype_faction" (FactionComponent entity): faction state sync
    ///   - "archetype_control_point" (ControlPointComponent entity): territory sync
    ///   - "archetype_unit" (UnitTypeComponent entity): unit position/health sync
    ///
    /// When Netcode for Entities is installed the GhostCollection singleton is
    /// confirmed live during networked sessions. Actual ghost prefab entity
    /// registration is handled by GhostAuthoringComponent baking and NfE's
    /// GhostCollectionSystem discovery. This system records replication intent in
    /// GhostPrefabArchetypeElement so authority routing can verify archetype state
    /// without taking a hard NfE dependency.
    ///
    /// Browser equivalent: absent -- multiplayer foundation not in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(NetworkBootstrapSystem))]
    public partial struct GhostCollectionSetupSystem : ISystem
    {
        private bool registered;

        public void OnCreate(ref SystemState state)
        {
            registered = false;
            state.RequireForUpdate<NetworkGameModeComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (registered)
            {
                return;
            }

            var em = state.EntityManager;

            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<NetworkGameModeComponent>(),
                ComponentType.ReadWrite<GhostPrefabArchetypeElement>());
            if (query.IsEmptyIgnoreFilter)
            {
                return;
            }

            Entity networkEntity = query.GetSingletonEntity();
            var buffer = em.GetBuffer<GhostPrefabArchetypeElement>(networkEntity);

            if (buffer.Length > 0)
            {
                registered = true;
#if UNITY_NETCODE
                ConfirmNfECollectionActive(ref state);
#endif
                return;
            }

            buffer.Add(new GhostPrefabArchetypeElement
            {
                ArchetypeId = new FixedString64Bytes("archetype_faction"),
                IsRegistered = true,
            });
            buffer.Add(new GhostPrefabArchetypeElement
            {
                ArchetypeId = new FixedString64Bytes("archetype_control_point"),
                IsRegistered = true,
            });
            buffer.Add(new GhostPrefabArchetypeElement
            {
                ArchetypeId = new FixedString64Bytes("archetype_unit"),
                IsRegistered = true,
            });

            registered = true;

#if UNITY_NETCODE
            ConfirmNfECollectionActive(ref state);
#endif
        }

#if UNITY_NETCODE
        private static void ConfirmNfECollectionActive(ref SystemState state)
        {
            // In networked sessions, verify the NfE GhostCollection singleton is
            // live. Ghost prefab entities are registered automatically by
            // GhostCollectionSystem after GhostAuthoringComponent baking; the
            // GhostCollectionPrefabs buffer on the collection entity holds the
            // resulting entity references. This method is a liveness check only --
            // actual prefab registration is driven by the authoring pipeline.
            if (!SystemAPI.HasSingleton<NetworkGameModeComponent>())
            {
                return;
            }

            var mode = SystemAPI.GetSingleton<NetworkGameModeComponent>();
            if (mode.IsLocalGame)
            {
                return;
            }

            if (!SystemAPI.HasSingleton<GhostCollection>())
            {
                return;
            }

            Entity collectionEntity = SystemAPI.GetSingletonEntity<GhostCollection>();
            var prefabs = state.EntityManager.GetBuffer<GhostCollectionPrefabs>(collectionEntity, isReadOnly: true);
            _ = prefabs.Length;
        }
#endif
    }
}
