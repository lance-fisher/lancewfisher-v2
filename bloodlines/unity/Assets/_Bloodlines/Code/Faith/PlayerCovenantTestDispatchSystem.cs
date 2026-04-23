using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Faith
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CovenantTestQualificationSystem))]
    [UpdateBefore(typeof(CovenantTestResolutionSystem))]
    public partial struct PlayerCovenantTestDispatchSystem : ISystem
    {
        private static readonly FixedString32Bytes PlayerFactionId = new("player");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var playerEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, PlayerFactionId);
            if (playerEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, playerEntity) ||
                !entityManager.HasComponent<FaithStateComponent>(playerEntity) ||
                !entityManager.HasComponent<CovenantTestStateComponent>(playerEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(playerEntity) ||
                !entityManager.HasComponent<PopulationComponent>(playerEntity) ||
                !entityManager.HasComponent<DynastyStateComponent>(playerEntity))
            {
                return;
            }

            if (!entityManager.HasComponent<PlayerCovenantTestDispatchStateComponent>(playerEntity))
            {
                entityManager.AddComponentData(
                    playerEntity,
                    PlayerCovenantTestDispatchStateComponent.CreateDefault(PlayerFactionId));
            }

            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);
            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovenantTestRequestComponent>());
            using NativeArray<PlayerCovenantTestRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerCovenantTestRequestComponent>(Allocator.Temp);

            var faithState = entityManager.GetComponentData<FaithStateComponent>(playerEntity);
            var testState = entityManager.GetComponentData<CovenantTestStateComponent>(playerEntity);
            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(playerEntity);
            var population = entityManager.GetComponentData<PopulationComponent>(playerEntity);
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(playerEntity);
            var dispatchState = entityManager.GetComponentData<PlayerCovenantTestDispatchStateComponent>(playerEntity);

            bool pendingRequest = HasPendingRequest(requests, PlayerFactionId);
            dispatchState = BuildDispatchState(
                entityManager,
                playerEntity,
                in dispatchState,
                in faithState,
                in testState,
                in resources,
                in population,
                in dynasty,
                pendingRequest,
                inWorldDays);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            if (dispatchState.RequestQueued)
            {
                if (dispatchState.ActionAvailable && !dispatchState.RequestPending)
                {
                    var requestEntity = ecb.CreateEntity();
                    ecb.AddComponent(requestEntity, new PlayerCovenantTestRequestComponent
                    {
                        SourceFactionId = PlayerFactionId,
                    });
                    dispatchState.RequestQueued = false;
                    dispatchState.RequestPending = true;
                    dispatchState.BlockReason = default;
                }
                else
                {
                    dispatchState.RequestQueued = false;
                }
            }

            entityManager.SetComponentData(playerEntity, dispatchState);
            ecb.Playback(entityManager);
        }

        private static PlayerCovenantTestDispatchStateComponent BuildDispatchState(
            EntityManager entityManager,
            Entity playerEntity,
            in PlayerCovenantTestDispatchStateComponent currentState,
            in FaithStateComponent faithState,
            in CovenantTestStateComponent testState,
            in ResourceStockpileComponent resources,
            in PopulationComponent population,
            in DynastyStateComponent dynasty,
            bool pendingRequest,
            float inWorldDays)
        {
            var next = currentState;
            next.SourceFactionId = PlayerFactionId;
            next.FaithId = faithState.SelectedFaith;
            next.DoctrinePath = faithState.DoctrinePath;
            next.TestPhase = testState.TestPhase;
            next.RequestPending = pendingRequest;

            var costProfile = CovenantTestRules.ResolveCostProfile(faithState.SelectedFaith, faithState.DoctrinePath);
            next.FoodCost = costProfile.Food;
            next.InfluenceCost = costProfile.Influence;
            next.PopulationCost = costProfile.Population;
            next.LegitimacyCost = costProfile.Legitimacy;

            bool hasAction = TryResolveActionDescriptor(
                faithState.SelectedFaith,
                faithState.DoctrinePath,
                out var actionLabel,
                out var actionDetail);
            next.ActionLabel = actionLabel;
            next.ActionDetail = actionDetail;

            next.CanAfford = CanPerformAction(costProfile, resources, population, dynasty);

            bool cooldownActive = DynastyPoliticalEventUtility.HasActiveEvent(
                entityManager,
                playerEntity,
                DynastyPoliticalEventTypes.CovenantTestCooldown,
                inWorldDays);

            next.ActionAvailable =
                hasAction &&
                testState.TestPhase == CovenantTestPhase.ReadyToTrigger &&
                !cooldownActive &&
                next.CanAfford &&
                !pendingRequest;

            next.BlockReason = BuildBlockReason(
                testState.TestPhase,
                hasAction,
                cooldownActive,
                next.CanAfford,
                pendingRequest,
                costProfile,
                resources,
                population,
                dynasty);

            return next;
        }

        private static bool HasPendingRequest(
            NativeArray<PlayerCovenantTestRequestComponent> requests,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < requests.Length; i++)
            {
                if (requests[i].SourceFactionId.Equals(factionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryResolveActionDescriptor(
            CovenantId faithId,
            DoctrinePath doctrinePath,
            out FixedString64Bytes actionLabel,
            out FixedString128Bytes actionDetail)
        {
            actionLabel = default;
            actionDetail = default;

            if (faithId == CovenantId.BloodDominion && doctrinePath == DoctrinePath.Light)
            {
                actionLabel = new FixedString64Bytes("Conduct Covenant Rite");
                actionDetail = new FixedString128Bytes("Spend 45 food and 18 influence to bind the host.");
                return true;
            }

            if (faithId == CovenantId.BloodDominion && doctrinePath == DoctrinePath.Dark)
            {
                actionLabel = new FixedString64Bytes("Offer Binding Sacrifice");
                actionDetail = new FixedString128Bytes("Spend 3 population, 20 influence, and 6 legitimacy.");
                return true;
            }

            return false;
        }

        private static bool CanPerformAction(
            in CovenantTestCostProfile costProfile,
            in ResourceStockpileComponent resources,
            in PopulationComponent population,
            in DynastyStateComponent dynasty)
        {
            bool hasFood = resources.Food >= costProfile.Food;
            bool hasInfluence = resources.Influence >= costProfile.Influence;
            bool hasPopulation = costProfile.Population <= 0 || population.Total > costProfile.Population;
            bool hasLegitimacy = dynasty.Legitimacy >= costProfile.Legitimacy;
            return hasFood && hasInfluence && hasPopulation && hasLegitimacy;
        }

        private static FixedString128Bytes BuildBlockReason(
            CovenantTestPhase phase,
            bool hasAction,
            bool cooldownActive,
            bool canAfford,
            bool pendingRequest,
            in CovenantTestCostProfile costProfile,
            in ResourceStockpileComponent resources,
            in PopulationComponent population,
            in DynastyStateComponent dynasty)
        {
            if (pendingRequest)
            {
                return new FixedString128Bytes("A covenant test request is already pending.");
            }

            if (!hasAction)
            {
                return new FixedString128Bytes("This covenant test resolves through state changes, not a direct rite action.");
            }

            if (phase != CovenantTestPhase.ReadyToTrigger)
            {
                return new FixedString128Bytes("Covenant test phase must be ReadyToTrigger.");
            }

            if (cooldownActive)
            {
                return new FixedString128Bytes("CovenantTestCooldown is still active.");
            }

            if (!canAfford)
            {
                if (resources.Food < costProfile.Food)
                {
                    return new FixedString128Bytes("Store enough food for the covenant rite.");
                }

                if (resources.Influence < costProfile.Influence)
                {
                    return new FixedString128Bytes("Store enough influence for the covenant rite.");
                }

                if (costProfile.Population > 0 && population.Total <= costProfile.Population)
                {
                    return new FixedString128Bytes("Keep more population alive to pay the binding.");
                }

                if (dynasty.Legitimacy < costProfile.Legitimacy)
                {
                    return new FixedString128Bytes("Raise legitimacy high enough to pay the binding.");
                }
            }

            return default;
        }
    }
}
