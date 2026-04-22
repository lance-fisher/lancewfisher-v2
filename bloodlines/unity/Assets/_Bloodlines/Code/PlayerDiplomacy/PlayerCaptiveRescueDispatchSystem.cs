using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of startRescueOperation using the AI-owned rescue operation
    /// payload shape while keeping the request and dispatch surface in the
    /// PlayerDiplomacy lane.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerCaptiveRescueDispatchSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerCaptiveRescueRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCaptiveRescueRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerCaptiveRescueRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerCaptiveRescueRequestComponent>(Allocator.Temp);

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
            in PlayerCaptiveRescueRequestComponent request,
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

            if (AICaptiveRescueExecutionSystem.HasActiveOperationForMember(
                    entityManager,
                    request.CaptiveMemberId))
            {
                return;
            }

            if (!AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(
                    entityManager,
                    sourceEntity,
                    new[] { DynastyRole.Spymaster, DynastyRole.Diplomat, DynastyRole.Merchant },
                    out var spymasterMemberId,
                    out _,
                    out var spymasterRenown) ||
                !AICaptiveRescueExecutionSystem.TryGetMemberByRolePriority(
                    entityManager,
                    sourceEntity,
                    new[] { DynastyRole.Diplomat, DynastyRole.Merchant, DynastyRole.HeirDesignate },
                    out var diplomatMemberId,
                    out _,
                    out var diplomatRenown))
            {
                return;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Gold < AICaptiveRescueExecutionSystem.RescueBaseGoldCost ||
                resources.Influence < AICaptiveRescueExecutionSystem.RescueBaseInfluenceCost ||
                !DynastyOperationLimits.HasCapacity(entityManager, request.SourceFactionId))
            {
                return;
            }

            resources.Gold -= AICaptiveRescueExecutionSystem.RescueBaseGoldCost;
            resources.Influence -= AICaptiveRescueExecutionSystem.RescueBaseInfluenceCost;
            entityManager.SetComponentData(sourceEntity, resources);

            float power = 12f + spymasterRenown * 0.95f + diplomatRenown * 0.35f;
            float difficulty = 16f;
            float successScore = power - difficulty;
            float projectedChance = math.clamp(0.5f + successScore / 45f, 0.12f, 0.88f);
            var captiveTitle = captiveMember.Title.Length > 0 ? captiveMember.Title : captiveTitleFromLedger;

            var operationEntity = ecb.CreateEntity();
            ecb.AddComponent(operationEntity, new DynastyOperationComponent
            {
                OperationId = PlayerCaptiveDispatchUtility.BuildRescueOperationId(
                    request.SourceFactionId,
                    captorFactionId,
                    request.CaptiveMemberId,
                    inWorldDays,
                    requestIndex),
                SourceFactionId = request.SourceFactionId,
                OperationKind = DynastyOperationKind.CaptiveRescue,
                StartedAtInWorldDays = inWorldDays,
                TargetFactionId = captorFactionId,
                TargetMemberId = request.CaptiveMemberId,
                Active = true,
            });
            ecb.AddComponent(operationEntity, new DynastyOperationCaptiveRescueComponent
            {
                ResolveAtInWorldDays = inWorldDays + AICaptiveRescueExecutionSystem.RescueBaseDurationInWorldDays,
                CaptiveMemberId = request.CaptiveMemberId,
                CaptiveMemberTitle = captiveTitle,
                CaptorFactionId = captorFactionId,
                SpymasterMemberId = spymasterMemberId,
                DiplomatMemberId = diplomatMemberId,
                HoldingSettlementId = default,
                KeepTier = 0,
                WardId = default,
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
                IntensityCost = 0f,
                EscrowCost = new DynastyOperationEscrowCost
                {
                    Gold = AICaptiveRescueExecutionSystem.RescueBaseGoldCost,
                    Influence = AICaptiveRescueExecutionSystem.RescueBaseInfluenceCost,
                },
            });

            PlayerCaptiveDispatchUtility.PushRescueNarrative(
                entityManager,
                request.SourceFactionId,
                captiveTitle,
                captorFactionId);
        }
    }
}
