using UnityEngine;
#if UNITY_NETCODE
using Unity.NetCode;
#endif

namespace Bloodlines.Multiplayer
{
    /// <summary>
    /// Authoring MonoBehaviour for Bloodlines ghost prefabs.
    /// Attach to the faction, control-point, and unit prefab GameObjects to configure
    /// their network replication mode before the NfE baker converts them to ghost entities.
    ///
    /// Ghost modes by archetype:
    ///   archetype_faction       -- Interpolated   (server-owned global state)
    ///   archetype_control_point -- Interpolated   (server-owned territory state)
    ///   archetype_unit          -- OwnerPredicted (player-controlled, client-predicted)
    ///
    /// When com.unity.netcode is not yet installed this component compiles as a
    /// standalone configuration MonoBehaviour. Activate NfE behaviour by defining
    /// UNITY_NETCODE in the project scripting define symbols after the package is installed.
    ///
    /// Browser equivalent: absent -- multiplayer foundation not in simulation.js.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GhostPrefabAuthoring : MonoBehaviour
    {
        public enum GhostReplicationMode
        {
            Interpolated   = 0,
            OwnerPredicted = 1,
            Predicted      = 2,
        }

        [Tooltip("Canonical archetype ID matching GhostPrefabArchetypeElement.ArchetypeId " +
                 "(archetype_faction | archetype_control_point | archetype_unit).")]
        public string archetypeId = string.Empty;

        [Tooltip("Network replication mode for this ghost prefab. Use OwnerPredicted for " +
                 "player-controlled units; Interpolated for server-owned state (factions, control points).")]
        public GhostReplicationMode replicationMode = GhostReplicationMode.Interpolated;

#if UNITY_NETCODE
        private void Reset()
        {
            EnsureGhostAuthoringComponent();
        }

        private void OnValidate()
        {
            EnsureGhostAuthoringComponent();
        }

        private void EnsureGhostAuthoringComponent()
        {
            var ghostAuthoring = GetComponent<GhostAuthoringComponent>();
            if (ghostAuthoring == null)
            {
                ghostAuthoring = gameObject.AddComponent<GhostAuthoringComponent>();
            }

            ghostAuthoring.DefaultGhostMode = replicationMode switch
            {
                GhostReplicationMode.OwnerPredicted => GhostMode.OwnerPredicted,
                GhostReplicationMode.Predicted      => GhostMode.Predicted,
                _                                   => GhostMode.Interpolated,
            };
        }
#endif
    }
}
