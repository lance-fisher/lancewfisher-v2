using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerPactProposalSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerPactProposalRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerPactProposalRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerPactProposalRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerPactProposalRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryDispatch(entityManager, ecb, requests[i], inWorldDays);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryDispatch(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            in PlayerPactProposalRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.SourceFactionId.Equals(request.TargetFactionId))
            {
                return;
            }

            var sourceEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, request.SourceFactionId);
            var targetEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, request.TargetFactionId);
            if (sourceEntity == Entity.Null ||
                targetEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, sourceEntity) ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, targetEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(sourceEntity) ||
                !PlayerPactUtility.IsHostile(entityManager, sourceEntity, request.TargetFactionId) ||
                PlayerPactUtility.TryFindActivePact(
                    entityManager,
                    request.SourceFactionId,
                    request.TargetFactionId,
                    out _,
                    out _))
            {
                return;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < PlayerPactUtility.InfluenceCost ||
                resources.Gold < PlayerPactUtility.GoldCost)
            {
                return;
            }

            resources.Influence -= PlayerPactUtility.InfluenceCost;
            resources.Gold -= PlayerPactUtility.GoldCost;
            entityManager.SetComponentData(sourceEntity, resources);

            PlayerPactUtility.DropHostility(entityManager, sourceEntity, request.TargetFactionId);
            PlayerPactUtility.DropHostility(entityManager, targetEntity, request.SourceFactionId);

            var pactEntity = ecb.CreateEntity();
            ecb.AddComponent(pactEntity, new PactComponent
            {
                PactId = PlayerPactUtility.BuildPactId(request.SourceFactionId, request.TargetFactionId, inWorldDays),
                FactionAId = request.SourceFactionId,
                FactionBId = request.TargetFactionId,
                StartedAtInWorldDays = inWorldDays,
                MinimumExpiresAtInWorldDays = inWorldDays + PlayerPactUtility.MinimumDurationInWorldDays,
                Broken = false,
                BrokenByFactionId = default,
            });

            PlayerPactUtility.PushPactEnteredNarrative(entityManager, request.SourceFactionId, request.TargetFactionId);
        }
    }
}
