using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Resolves player-facing Trueborn recognition requests against the live rise
    /// arc state. Future AI producers can emit the same request component without
    /// widening this lane into AI-owned paths.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GovernanceCoalitionPressureSystem))]
    [UpdateBefore(typeof(TruebornRiseArcSystem))]
    public partial struct TruebornRecognitionResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TruebornRiseArcComponent>();
            state.RequireForUpdate<PlayerTruebornRecognitionRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float currentInWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            Entity arcEntity = SystemAPI.GetSingletonEntity<TruebornRiseArcComponent>();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots =
                entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerTruebornRecognitionRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerTruebornRecognitionRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerTruebornRecognitionRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            bool arcChanged = false;

            for (int i = 0; i < requestEntities.Length; i++)
            {
                if (TryResolve(
                        entityManager,
                        ref arc,
                        recognitionSlots,
                        requests[i],
                        currentInWorldDays))
                {
                    arcChanged = true;
                }

                ecb.DestroyEntity(requestEntities[i]);
            }

            if (arcChanged)
            {
                entityManager.SetComponentData(arcEntity, arc);
            }

            ecb.Playback(entityManager);
        }

        private static bool TryResolve(
            EntityManager entityManager,
            ref TruebornRiseArcComponent arc,
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            in PlayerTruebornRecognitionRequestComponent request,
            float currentInWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
                !TruebornRecognitionUtility.HasActiveRise(arc) ||
                !TruebornRecognitionUtility.HasTruebornCity(entityManager))
            {
                return false;
            }

            Entity factionEntity = FindFactionEntity(entityManager, request.SourceFactionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<FactionKindComponent>(factionEntity) ||
                entityManager.GetComponentData<FactionKindComponent>(factionEntity).Kind != FactionKind.Kingdom ||
                !entityManager.HasComponent<ResourceStockpileComponent>(factionEntity) ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return false;
            }

            if (TruebornRecognitionUtility.IsRecognized(
                    recognitionSlots,
                    arc.RecognizedFactionsBitmask,
                    request.SourceFactionId))
            {
                return false;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            var dynastyState = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            if (dynastyState.Legitimacy < TruebornRecognitionUtility.LegitimacyCost ||
                resources.Influence < TruebornRecognitionUtility.InfluenceCost ||
                resources.Gold < TruebornRecognitionUtility.GoldCost)
            {
                return false;
            }

            resources.Influence = math.max(0f, resources.Influence - TruebornRecognitionUtility.InfluenceCost);
            resources.Gold = math.max(0f, resources.Gold - TruebornRecognitionUtility.GoldCost);
            dynastyState.Legitimacy = math.clamp(
                dynastyState.Legitimacy - TruebornRecognitionUtility.LegitimacyCost,
                0f,
                100f);

            if (!TruebornRecognitionUtility.TrySetRecognition(
                    ref arc,
                    recognitionSlots,
                    request.SourceFactionId,
                    true))
            {
                return false;
            }

            entityManager.SetComponentData(factionEntity, resources);
            entityManager.SetComponentData(factionEntity, dynastyState);
            TruebornRecognitionUtility.ApplyStandingBonus(entityManager, factionEntity, currentInWorldDays);
            TruebornRecognitionUtility.ClearRecognitionCooldowns(entityManager, factionEntity, currentInWorldDays);
            return true;
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
