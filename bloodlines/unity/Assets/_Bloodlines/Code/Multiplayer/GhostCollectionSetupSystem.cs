using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Registers canonical ghost prefab archetypes after NetworkBootstrapSystem
    /// has created the network singleton. Three archetypes are registered:
    ///   - "archetype_faction" (FactionComponent entity): faction state sync
    ///   - "archetype_control_point" (ControlPointComponent entity): territory sync
    ///   - "archetype_unit" (UnitTypeComponent entity): unit position/health sync
    ///
    /// When Netcode for Entities is installed, these entries drive ghost prefab
    /// collection registration via the GhostAuthoringComponent pipeline.
    /// In this foundation slice they are data-model records of replication intent.
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
        }
    }
}
