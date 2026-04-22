using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of startMissionaryOperation. Reuses the AI-owned
    /// missionary payload component while keeping all dispatch logic local to
    /// the PlayerDiplomacy lane.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerMissionaryDispatchSystem : ISystem
    {
        public const float MissionaryInfluenceCost = 14f;
        public const float MissionaryIntensityCost = 12f;
        public const float MissionaryDurationInWorldDays = 32f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMissionaryDispatchRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerMissionaryDispatchRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerMissionaryDispatchRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerMissionaryDispatchRequestComponent>(Allocator.Temp);

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
            in PlayerMissionaryDispatchRequestComponent request,
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
                !entityManager.HasComponent<ResourceStockpileComponent>(sourceEntity))
            {
                return;
            }

            var sourceFaith = entityManager.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None)
            {
                return;
            }

            CovenantId targetSelectedFaith = CovenantId.None;
            float targetIntensity = 0f;
            if (entityManager.HasComponent<FaithStateComponent>(targetEntity))
            {
                var targetFaith = entityManager.GetComponentData<FaithStateComponent>(targetEntity);
                targetSelectedFaith = targetFaith.SelectedFaith;
                targetIntensity = targetFaith.Intensity;
            }

            if (targetSelectedFaith == sourceFaith.SelectedFaith ||
                PlayerFaithDeclarationUtility.HasActiveOperationForTarget(
                    entityManager,
                    request.SourceFactionId,
                    DynastyOperationKind.Missionary,
                    request.TargetFactionId))
            {
                return;
            }

            if (!PlayerFaithDeclarationUtility.TryGetFaithOperator(
                    entityManager,
                    sourceEntity,
                    out var operatorMemberId,
                    out var operatorTitle,
                    out var operatorRenown))
            {
                return;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < MissionaryInfluenceCost ||
                sourceFaith.Intensity < MissionaryIntensityCost ||
                !DynastyOperationLimits.HasCapacity(entityManager, request.SourceFactionId))
            {
                return;
            }

            float sourceIntensity = sourceFaith.Intensity;
            bool isDarkPath = sourceFaith.DoctrinePath == DoctrinePath.Dark;
            float offenseScore = operatorRenown + sourceIntensity * 0.65f + (isDarkPath ? 8f : 4f);
            float defenseScore = targetIntensity * 0.55f;
            float successScore = offenseScore - defenseScore;
            float projectedChance = math.clamp(0.5f + successScore / 100f, 0.1f, 0.9f);
            float exposureGain = math.max(12f, math.round(14f + math.max(0f, successScore) * 0.22f));
            float intensityErosion = (targetSelectedFaith != CovenantId.None &&
                                      targetSelectedFaith != sourceFaith.SelectedFaith)
                ? math.max(4f, math.round(5f + math.max(0f, sourceIntensity - targetIntensity) / 12f) +
                              (isDarkPath ? 2f : 0f))
                : 0f;
            float loyaltyPressure = (targetSelectedFaith != CovenantId.None &&
                                     targetSelectedFaith != sourceFaith.SelectedFaith)
                ? (isDarkPath ? 4f : 2f)
                : 1f;

            resources.Influence -= MissionaryInfluenceCost;
            entityManager.SetComponentData(sourceEntity, resources);

            sourceFaith.Intensity = math.max(0f, sourceFaith.Intensity - MissionaryIntensityCost);
            entityManager.SetComponentData(sourceEntity, sourceFaith);

            var operationEntity = ecb.CreateEntity();
            ecb.AddComponent(operationEntity, new DynastyOperationComponent
            {
                OperationId = BuildOperationId(
                    request.SourceFactionId,
                    request.TargetFactionId,
                    inWorldDays,
                    requestIndex),
                SourceFactionId = request.SourceFactionId,
                OperationKind = DynastyOperationKind.Missionary,
                StartedAtInWorldDays = inWorldDays,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = default,
                Active = true,
            });
            ecb.AddComponent(operationEntity, new DynastyOperationMissionaryComponent
            {
                ResolveAtInWorldDays = inWorldDays + MissionaryDurationInWorldDays,
                OperatorMemberId = operatorMemberId,
                OperatorTitle = operatorTitle,
                SourceFaithId = PlayerFaithDeclarationUtility.SelectedFaithIdString(sourceFaith.SelectedFaith),
                ExposureGain = exposureGain,
                IntensityErosion = intensityErosion,
                LoyaltyPressure = loyaltyPressure,
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
                IntensityCost = MissionaryIntensityCost,
                EscrowCost = new DynastyOperationEscrowCost { Influence = MissionaryInfluenceCost },
            });

            PlayerFaithDeclarationUtility.PushMissionaryNarrative(
                entityManager,
                request.SourceFactionId,
                request.TargetFactionId,
                PlayerFaithDeclarationUtility.SelectedFaithIdString(sourceFaith.SelectedFaith));
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays,
            int requestIndex)
        {
            var id = new FixedString64Bytes("player-missionary-");
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
