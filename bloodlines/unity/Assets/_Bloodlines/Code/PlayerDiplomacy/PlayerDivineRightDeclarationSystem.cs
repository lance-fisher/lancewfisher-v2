using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Player-side port of startDivineRightDeclaration using the existing
    /// dynasty-operation entity shape.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerDivineRightDeclarationSystem : ISystem
    {
        public const float DivineRightIntensityThreshold = 80f;
        public const int DivineRightLevelThreshold = 5;
        public const float DivineRightDeclarationDurationInWorldDays = 180f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerDivineRightDeclarationRequestComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerDivineRightDeclarationRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<PlayerDivineRightDeclarationRequestComponent> requests =
                requestQuery.ToComponentDataArray<PlayerDivineRightDeclarationRequestComponent>(Allocator.Temp);

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
            in PlayerDivineRightDeclarationRequestComponent request,
            float inWorldDays,
            int requestIndex)
        {
            if (request.SourceFactionId.Length == 0)
            {
                return;
            }

            var sourceEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, request.SourceFactionId);
            if (sourceEntity == Entity.Null ||
                !PlayerFaithDeclarationUtility.IsKingdom(entityManager, sourceEntity) ||
                !entityManager.HasComponent<FaithStateComponent>(sourceEntity) ||
                !entityManager.HasComponent<CovenantTestStateComponent>(sourceEntity))
            {
                return;
            }

            var sourceFaith = entityManager.GetComponentData<FaithStateComponent>(sourceEntity);
            var covenantTestState = entityManager.GetComponentData<CovenantTestStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None ||
                covenantTestState.TestPhase != CovenantTestPhase.Complete ||
                DynastyPoliticalEventUtility.HasActiveEvent(
                    entityManager,
                    sourceEntity,
                    DynastyPoliticalEventTypes.DivineRightFailedCooldown,
                    inWorldDays) ||
                sourceFaith.Intensity < DivineRightIntensityThreshold ||
                sourceFaith.Level < DivineRightLevelThreshold ||
                PlayerFaithDeclarationUtility.HasActiveOperation(
                    entityManager,
                    request.SourceFactionId,
                    DynastyOperationKind.DivineRight) ||
                !DynastyOperationLimits.HasCapacity(entityManager, request.SourceFactionId))
            {
                return;
            }

            var operationEntity = ecb.CreateEntity();
            var sourceFaithId = PlayerFaithDeclarationUtility.SelectedFaithIdString(sourceFaith.SelectedFaith);
            ecb.AddComponent(operationEntity, new DynastyOperationComponent
            {
                OperationId = BuildOperationId(request.SourceFactionId, inWorldDays, requestIndex),
                SourceFactionId = request.SourceFactionId,
                OperationKind = DynastyOperationKind.DivineRight,
                StartedAtInWorldDays = inWorldDays,
                TargetFactionId = default,
                TargetMemberId = default,
                Active = true,
            });
            ecb.AddComponent(operationEntity, new DynastyOperationDivineRightComponent
            {
                ResolveAtInWorldDays = inWorldDays + DivineRightDeclarationDurationInWorldDays,
                SourceFaithId = sourceFaithId,
                DoctrinePath = sourceFaith.DoctrinePath,
                RecognitionShare = 0f,
                RecognitionSharePct = 0f,
                ActiveApexStructureId = default,
                ActiveApexStructureName = default,
            });

            PlayerFaithDeclarationUtility.PushDivineRightNarrative(
                entityManager,
                request.SourceFactionId,
                sourceFaithId,
                (int)DivineRightDeclarationDurationInWorldDays);
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            float inWorldDays,
            int requestIndex)
        {
            var id = new FixedString64Bytes("player-divine-right-");
            id.Append(sourceFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            id.Append("-");
            id.Append(requestIndex);
            return id;
        }
    }
}
