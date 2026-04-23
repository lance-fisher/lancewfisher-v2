using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves player espionage operations once their in-world timer matures.
    /// Browser parity keeps the dispatch-time success score authoritative so the
    /// resulting report mirrors the originally projected operation window.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    public partial struct EspionageResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
                if (query.IsEmpty)
                {
                    query.Dispose();
                    return;
                }

                using var entities = query.ToEntityArray(Allocator.Temp);
                using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
                query.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    var operation = operations[i];
                    if (!operation.Active ||
                        operation.Kind != CovertOpKindPlayer.Espionage ||
                        operation.ResolveAtInWorldDays > inWorldDays)
                    {
                        continue;
                    }

                    ResolveEspionageOperation(
                        entityManager,
                        ref ecb,
                        entities[i],
                        operation,
                        inWorldDays);
                }

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static void ResolveEspionageOperation(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
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

            if (operation.SuccessScore >= 0f &&
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
            }

            ecb.DestroyEntity(operationEntity);
        }
    }
}
