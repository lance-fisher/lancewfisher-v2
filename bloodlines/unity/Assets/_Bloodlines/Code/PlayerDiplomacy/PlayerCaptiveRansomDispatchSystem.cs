using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of startRansomNegotiation using the AI-owned ransom
    /// operation payload shape while keeping request dispatch inside the
    /// PlayerDiplomacy lane.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerCaptiveRansomDispatchSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerCaptiveRansomRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCaptiveRansomRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerCaptiveRansomRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerCaptiveRansomRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryDispatch(entityManager, ecb, requests[i], inWorldDays, i);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryDispatch(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            in PlayerCaptiveRansomRequestComponent request,
            float inWorldDays,
            int requestIndex)
        {
            if (request.SourceFactionId.Length == 0 || request.CaptiveMemberId.Length == 0)
            {
                return;
            }

            var sourceEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, request.SourceFactionId);
            if (sourceEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, sourceEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(sourceEntity))
            {
                return;
            }

            if (!PlayerCaptiveDispatchUtility.TryGetCapturedMember(
                    entityManager,
                    sourceEntity,
                    request.CaptiveMemberId,
                    out var captiveMember))
            {
                return;
            }

            if (!PlayerCaptiveDispatchUtility.TryFindHeldCaptive(
                    entityManager,
                    request.SourceFactionId,
                    request.CaptiveMemberId,
                    out var captiveTitleFromLedger,
                    out var captorFactionId))
            {
                return;
            }

            if (request.TargetFactionId.Length > 0 && !request.TargetFactionId.Equals(captorFactionId))
            {
                return;
            }

            if (AICaptiveRescueExecutionSystem.HasActiveOperationForMember(
                    entityManager,
                    request.CaptiveMemberId))
            {
                return;
            }

            if (!AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(
                    entityManager,
                    sourceEntity,
                    new[] { DynastyRole.Diplomat, DynastyRole.Merchant, DynastyRole.HeirDesignate, DynastyRole.HeadOfBloodline },
                    out var diplomatMemberId,
                    out _,
                    out _) ||
                !AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(
                    entityManager,
                    sourceEntity,
                    new[] { DynastyRole.Merchant, DynastyRole.Governor, DynastyRole.HeadOfBloodline },
                    out var merchantMemberId,
                    out _,
                    out _))
            {
                return;
            }

            if (PlayerPactUtility.IsHostile(entityManager, sourceEntity, captorFactionId))
            {
                return;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < AICaptiveRansomExecutionSystem.RansomBaseGoldCost ||
                resources.Influence < AICaptiveRansomExecutionSystem.RansomBaseInfluenceCost ||
                !DynastyOperationLimits.HasCapacity(entityManager, request.SourceFactionId))
            {
                return;
            }

            if (request.RansomGoldAmount > 0f &&
                request.RansomGoldAmount < AICaptiveRansomExecutionSystem.RansomBaseGoldCost)
            {
                return;
            }

            resources.Gold -= AICaptiveRansomExecutionSystem.RansomBaseGoldCost;
            resources.Influence -= AICaptiveRansomExecutionSystem.RansomBaseInfluenceCost;
            entityManager.SetComponentData(sourceEntity, resources);

            var captiveTitle = captiveMember.Title.Length > 0 ? captiveMember.Title : captiveTitleFromLedger;
            var operationEntity = ecb.CreateEntity();
            ecb.AddComponent(operationEntity, new DynastyOperationComponent
            {
                OperationId = PlayerCaptiveDispatchUtility.BuildRansomOperationId(
                    request.SourceFactionId,
                    captorFactionId,
                    request.CaptiveMemberId,
                    inWorldDays,
                    requestIndex),
                SourceFactionId = request.SourceFactionId,
                OperationKind = DynastyOperationKind.CaptiveRansom,
                StartedAtInWorldDays = inWorldDays,
                TargetFactionId = captorFactionId,
                TargetMemberId = request.CaptiveMemberId,
                Active = true,
            });
            ecb.AddComponent(operationEntity, new DynastyOperationCaptiveRansomComponent
            {
                ResolveAtInWorldDays = inWorldDays + AICaptiveRansomExecutionSystem.RansomBaseDurationInWorldDays,
                CaptiveMemberId = request.CaptiveMemberId,
                CaptiveMemberTitle = captiveTitle,
                CaptorFactionId = captorFactionId,
                DiplomatMemberId = diplomatMemberId,
                MerchantMemberId = merchantMemberId,
                ProjectedChance = 1f,
                IntensityCost = 0f,
                EscrowCost = new DynastyOperationEscrowCost
                {
                    Gold = AICaptiveRansomExecutionSystem.RansomBaseGoldCost,
                    Influence = AICaptiveRansomExecutionSystem.RansomBaseInfluenceCost,
                },
            });

            PlayerCaptiveDispatchUtility.PushRansomNarrative(
                entityManager,
                request.SourceFactionId,
                captiveTitle,
                captorFactionId);
        }
    }
}
