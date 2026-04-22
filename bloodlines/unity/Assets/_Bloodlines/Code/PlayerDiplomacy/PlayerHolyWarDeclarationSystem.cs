using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of startHolyWarDeclaration. Creates a holy-war dynasty
    /// operation entity using the AI-owned component shape without mutating the
    /// AI lane's code.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerHolyWarDeclarationSystem : ISystem
    {
        public const float HolyWarInfluenceCost = 24f;
        public const float HolyWarIntensityCost = 18f;
        public const float HolyWarDeclarationDurationInWorldDays = 18f;
        public const float HolyWarDurationInWorldDays = 180f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerHolyWarDeclarationRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerHolyWarDeclarationRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerHolyWarDeclarationRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerHolyWarDeclarationRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryDispatch(
                    entityManager,
                    ecb,
                    requests[i],
                    inWorldDays,
                    i);
                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }

        private static void TryDispatch(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            in PlayerHolyWarDeclarationRequestComponent request,
            float inWorldDays,
            int requestIndex)
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
                !entityManager.HasComponent<FaithStateComponent>(sourceEntity) ||
                !entityManager.HasComponent<FaithStateComponent>(targetEntity) ||
                !entityManager.HasComponent<ResourceStockpileComponent>(sourceEntity))
            {
                return;
            }

            var sourceFaith = entityManager.GetComponentData<FaithStateComponent>(sourceEntity);
            var targetFaith = entityManager.GetComponentData<FaithStateComponent>(targetEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None ||
                targetFaith.SelectedFaith == CovenantId.None ||
                (sourceFaith.SelectedFaith == targetFaith.SelectedFaith &&
                 sourceFaith.DoctrinePath == targetFaith.DoctrinePath))
            {
                return;
            }

            if (PlayerFaithDeclarationUtility.HasActiveOperationForTarget(
                    entityManager,
                    request.SourceFactionId,
                    DynastyOperationKind.HolyWar,
                    request.TargetFactionId))
            {
                return;
            }

            if (!PlayerFaithDeclarationUtility.TryGetFaithOperator(
                    entityManager,
                    sourceEntity,
                    out var operatorMemberId,
                    out var operatorTitle))
            {
                return;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < HolyWarInfluenceCost ||
                sourceFaith.Intensity < HolyWarIntensityCost ||
                !DynastyOperationLimits.HasCapacity(entityManager, request.SourceFactionId))
            {
                return;
            }

            resources.Influence -= HolyWarInfluenceCost;
            entityManager.SetComponentData(sourceEntity, resources);

            sourceFaith.Intensity -= HolyWarIntensityCost;
            entityManager.SetComponentData(sourceEntity, sourceFaith);

            float intensityPulse = sourceFaith.DoctrinePath == DoctrinePath.Dark ? 1.2f : 0.9f;
            float loyaltyPulse = sourceFaith.DoctrinePath == DoctrinePath.Dark ? 1.8f : 1.2f;
            var operationEntity = ecb.CreateEntity();
            ecb.AddComponent(operationEntity, new DynastyOperationComponent
            {
                OperationId = BuildOperationId(
                    request.SourceFactionId,
                    request.TargetFactionId,
                    inWorldDays,
                    requestIndex),
                SourceFactionId = request.SourceFactionId,
                OperationKind = DynastyOperationKind.HolyWar,
                StartedAtInWorldDays = inWorldDays,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = default,
                Active = true,
            });
            ecb.AddComponent(operationEntity, new DynastyOperationHolyWarComponent
            {
                ResolveAtInWorldDays = inWorldDays + HolyWarDeclarationDurationInWorldDays,
                WarExpiresAtInWorldDays = inWorldDays + HolyWarDurationInWorldDays,
                OperatorMemberId = operatorMemberId,
                OperatorTitle = operatorTitle,
                IntensityPulse = intensityPulse,
                LoyaltyPulse = loyaltyPulse,
                CompatibilityLabel = PlayerFaithDeclarationUtility.DeriveHolyWarCompatibilityLabel(sourceFaith, targetFaith),
                IntensityCost = HolyWarIntensityCost,
                EscrowCost = new DynastyOperationEscrowCost { Influence = HolyWarInfluenceCost },
            });

            PlayerFaithDeclarationUtility.PushHolyWarNarrative(
                entityManager,
                request.SourceFactionId,
                request.TargetFactionId);
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays,
            int requestIndex)
        {
            var id = new FixedString64Bytes("player-holy-war-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            id.Append("-");
            id.Append(requestIndex);
            return id;
        }
    }
}
