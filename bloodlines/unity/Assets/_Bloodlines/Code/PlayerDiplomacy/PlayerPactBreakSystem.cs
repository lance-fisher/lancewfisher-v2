using Bloodlines.AI;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerPactProposalSystem))]
    public partial struct PlayerPactBreakSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerPactBreakRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerPactBreakRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerPactBreakRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerPactBreakRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryBreak(entityManager, requests[i]);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryBreak(
            EntityManager entityManager,
            in PlayerPactBreakRequestComponent request)
        {
            if (request.RequestingFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.RequestingFactionId.Equals(request.TargetFactionId))
            {
                return;
            }

            if (!PlayerPactUtility.TryFindActivePact(
                    entityManager,
                    request.RequestingFactionId,
                    request.TargetFactionId,
                    out var pactEntity,
                    out var pact))
            {
                return;
            }

            pact.Broken = true;
            pact.BrokenByFactionId = request.RequestingFactionId;
            entityManager.SetComponentData(pactEntity, pact);

            PlayerPactUtility.EnsureHostility(entityManager, request.RequestingFactionId, request.TargetFactionId);
            PlayerPactUtility.EnsureHostility(entityManager, request.TargetFactionId, request.RequestingFactionId);
            PlayerPactUtility.ApplyBreakPenalties(entityManager, request.RequestingFactionId);
            PlayerPactUtility.PushPactBrokenNarrative(
                entityManager,
                request.RequestingFactionId,
                request.TargetFactionId,
                pact.MinimumExpiresAtInWorldDays);
        }
    }
}
