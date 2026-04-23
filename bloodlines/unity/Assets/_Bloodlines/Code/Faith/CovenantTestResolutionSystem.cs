using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CovenantTestQualificationSystem))]
    public partial struct CovenantTestResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerCovenantTestRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovenantTestRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerCovenantTestRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerCovenantTestRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryResolve(entityManager, requests[i], inWorldDays);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryResolve(
            EntityManager entityManager,
            in PlayerCovenantTestRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0)
            {
                return;
            }

            var factionEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, request.SourceFactionId);
            if (factionEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, factionEntity) ||
                !entityManager.HasComponent<FaithStateComponent>(factionEntity) ||
                !entityManager.HasComponent<CovenantTestStateComponent>(factionEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(factionEntity) ||
                !entityManager.HasComponent<PopulationComponent>(factionEntity) ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return;
            }

            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            var covenantTestState = entityManager.GetComponentData<CovenantTestStateComponent>(factionEntity);
            if (faithState.SelectedFaith == CovenantId.None ||
                covenantTestState.TestPhase != CovenantTestPhase.ReadyToTrigger ||
                DynastyPoliticalEventUtility.HasActiveEvent(
                    entityManager,
                    factionEntity,
                    DynastyPoliticalEventTypes.CovenantTestCooldown,
                    inWorldDays))
            {
                return;
            }

            covenantTestState.TestPhase = CovenantTestPhase.InProgress;
            covenantTestState.TestStartedAtInWorldDays = inWorldDays;

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            var population = entityManager.GetComponentData<PopulationComponent>(factionEntity);
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            var costProfile = CovenantTestRules.ResolveCostProfile(faithState.SelectedFaith, faithState.DoctrinePath);

            if (!CovenantTestRules.CanAfford(costProfile, resources, population))
            {
                covenantTestState.TestPhase = CovenantTestPhase.Failed;
                covenantTestState.LastFailedAtInWorldDays = inWorldDays;
                covenantTestState.IntensityThresholdMetAtInWorldDays = float.NaN;
                covenantTestState.TestStartedAtInWorldDays = float.NaN;

                faithState.Intensity = math.max(0f, faithState.Intensity - CovenantTestRules.FailureIntensityLoss);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy - CovenantTestRules.FailureLegitimacyLoss,
                    0f,
                    100f);

                DynastyPoliticalEventUtility.AddOrRefreshEvent(
                    entityManager,
                    factionEntity,
                    DynastyPoliticalEventTypes.CovenantTestCooldown,
                    inWorldDays + CovenantTestRules.RetryCooldownInWorldDays);
            }
            else
            {
                CovenantTestRules.Spend(ref resources, ref population, ref dynasty, costProfile);
                faithState.Intensity = math.max(faithState.Intensity, CovenantTestRules.SuccessIntensityFloor);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy + CovenantTestRules.SuccessLegitimacyBonus,
                    0f,
                    100f);

                covenantTestState.TestPhase = CovenantTestPhase.Complete;
                covenantTestState.IntensityThresholdMetAtInWorldDays = inWorldDays;
                covenantTestState.TestStartedAtInWorldDays = inWorldDays;
                covenantTestState.SuccessCount++;
            }

            entityManager.SetComponentData(factionEntity, faithState);
            entityManager.SetComponentData(factionEntity, covenantTestState);
            entityManager.SetComponentData(factionEntity, resources);
            entityManager.SetComponentData(factionEntity, population);
            entityManager.SetComponentData(factionEntity, dynasty);
        }
    }
}
