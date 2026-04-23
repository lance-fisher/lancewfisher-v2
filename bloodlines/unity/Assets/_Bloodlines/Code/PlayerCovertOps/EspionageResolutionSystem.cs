using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves ready espionage operations into richer dossier output or exposure
    /// fallout. This stays lane-local so player covert intel can deepen without
    /// widening the AI-owned dynasty operation graph.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    public partial struct EspionageResolutionSystem : ISystem
    {
        private const float EspionageFailureLegitimacyPenalty = 1f;
        private const float EspionageFailureCounterIntelligenceLoss = 2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);

            var operationQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (operationQuery.IsEmpty)
            {
                operationQuery.Dispose();
                return;
            }

            using var operationEntities = operationQuery.ToEntityArray(Allocator.Temp);
            using var operations = operationQuery.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            operationQuery.Dispose();

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            for (int i = 0; i < operationEntities.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active ||
                    operation.Kind != CovertOpKindPlayer.Espionage ||
                    operation.ResolveAtInWorldDays > inWorldDays)
                {
                    continue;
                }

                ResolveOperation(entityManager, ecb, operationEntities[i], operation, inWorldDays);
            }
        }

        private static void ResolveOperation(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.TargetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                ecb.DestroyEntity(operationEntity);
                return;
            }

            float offenseRenown =
                PlayerCovertOpsSystem.TrySelectOperator(entityManager, sourceFactionEntity, out var operatorMember)
                    ? operatorMember.Renown
                    : 0f;
            var contest = PlayerCovertOpsSystem.BuildEspionageContest(
                entityManager,
                operation.SourceFactionId,
                operation.TargetFactionId,
                offenseRenown,
                inWorldDays);

            if (contest.SuccessScore >= 0f &&
                PlayerCounterIntelligenceSystem.TryCreateIntelligenceReport(
                    entityManager,
                    operation.SourceFactionId,
                    operation.TargetFactionId,
                    new FixedString32Bytes("espionage"),
                    new FixedString64Bytes("Court report"),
                    default,
                    0,
                    inWorldDays,
                    operation.ReportExpiresAtInWorldDays,
                    operationEntity.Index,
                    out var report))
            {
                PlayerCounterIntelligenceSystem.StoreIntelligenceReport(entityManager, sourceFactionEntity, report);
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, sourceFactionEntity, 1f);
            }
            else
            {
                PlayerCounterIntelligenceSystem.RecordCounterIntelligenceInterception(
                    entityManager,
                    operation.TargetFactionId,
                    operation.SourceFactionId,
                    new FixedString32Bytes("espionage"),
                    default,
                    inWorldDays);
                PlayerCounterIntelligenceSystem.EnsureMutualHostility(
                    entityManager,
                    operation.SourceFactionId,
                    operation.TargetFactionId);
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, targetFactionEntity, 1f);
                PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                    entityManager,
                    sourceFactionEntity,
                    -EspionageFailureLegitimacyPenalty);
                ApplyCounterIntelligenceLoss(
                    entityManager,
                    sourceFactionEntity,
                    inWorldDays,
                    EspionageFailureCounterIntelligenceLoss);
            }

            ecb.DestroyEntity(operationEntity);
        }

        private static void ApplyCounterIntelligenceLoss(
            EntityManager entityManager,
            Entity factionEntity,
            float inWorldDays,
            float amount)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCounterIntelligenceComponent>(factionEntity))
            {
                return;
            }

            var watch = entityManager.GetComponentData<PlayerCounterIntelligenceComponent>(factionEntity);
            if (watch.ExpiresAtInWorldDays <= inWorldDays)
            {
                return;
            }

            watch.WatchStrength = math.max(0f, watch.WatchStrength - amount);
            entityManager.SetComponentData(factionEntity, watch);
        }
    }
}
