using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the conviction ledger. Gives the
    /// governed smoke validators and in-editor debug pipeline a controlled way
    /// to record conviction events and inspect band state for a faction.
    ///
    /// Browser reference: simulation.js recordConvictionEvent (4245) and
    /// getConvictionBandEffects (1897). The record helper also refreshes the
    /// band immediately so observers see a consistent state without waiting a
    /// simulation tick, matching the browser semantics.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugRecordConvictionEvent(string factionId, ConvictionBucket bucket, float amount)
        {
            if (string.IsNullOrWhiteSpace(factionId) || amount == 0f)
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return false;
            }

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(ref conviction, bucket, amount);
            entityManager.SetComponentData(factionEntity, conviction);
            return true;
        }

        public bool TryDebugGetConvictionState(
            string factionId,
            out ConvictionComponent conviction)
        {
            conviction = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntity(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return false;
            }

            conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            return true;
        }

        public bool TryDebugGetConvictionBandEffects(
            string factionId,
            out ConvictionBandEffects effects)
        {
            effects = ConvictionBandEffects.ForBand(ConvictionBand.Neutral);
            if (!TryDebugGetConvictionState(factionId, out var conviction))
            {
                return false;
            }

            effects = ConvictionBandEffects.ForBand(conviction.Band);
            return true;
        }

        private static Entity FindFactionEntity(EntityManager entityManager, string factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.ToString() == factionId)
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
