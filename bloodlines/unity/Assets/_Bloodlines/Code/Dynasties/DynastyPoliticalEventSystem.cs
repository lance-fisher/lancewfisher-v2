using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Maintains timed dynasty political events and their composite runtime
    /// effect multipliers. This first Unity slice ports the browser's active
    /// event container and expiry loop, then wires the already-landed Divine
    /// Right declaration surface into the first concrete cooldown event.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerDivineRightDeclarationSystem))]
    [UpdateBefore(typeof(Bloodlines.Systems.ResourceTrickleBuildingSystem))]
    [UpdateBefore(typeof(Bloodlines.Systems.ControlPointCaptureSystem))]
    public partial struct DynastyPoliticalEventSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var dualClock = SystemAPI.GetSingleton<DualClockComponent>();
            float inWorldDays = dualClock.InWorldDays;

            ProcessDivineRightFailures(entityManager, dualClock, inWorldDays);

            foreach (var (faction, entity) in
                SystemAPI.Query<RefRO<FactionComponent>>().WithEntityAccess())
            {
                if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(entity))
                {
                    EnsureAggregate(entityManager, entity, DynastyPoliticalEventUtility.CreateDefaultAggregate());
                    continue;
                }

                var buffer = entityManager.GetBuffer<DynastyPoliticalEventComponent>(entity);
                for (int i = buffer.Length - 1; i >= 0; i--)
                {
                    if (buffer[i].ExpiresAtInWorldDays <= inWorldDays)
                    {
                        buffer.RemoveAt(i);
                    }
                }

                var aggregate = DynastyPoliticalEventUtility.BuildAggregate(buffer, inWorldDays);
                EnsureAggregate(entityManager, entity, aggregate);
            }
        }

        static void ProcessDivineRightFailures(
            EntityManager entityManager,
            in DualClockComponent dualClock,
            float inWorldDays)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationDivineRightComponent>());
            using var operationEntities = query.ToEntityArray(Allocator.Temp);
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);

            for (int i = 0; i < operationEntities.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active || operation.OperationKind != DynastyOperationKind.DivineRight)
                {
                    continue;
                }

                var factionEntity = FindFactionEntity(entityManager, operation.SourceFactionId);
                if (factionEntity == Entity.Null ||
                    !entityManager.HasComponent<FaithStateComponent>(factionEntity))
                {
                    DeactivateDivineRightOperation(entityManager, operationEntities[i], operation);
                    continue;
                }

                var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
                bool declarationFailed =
                    faithState.SelectedFaith == CovenantId.None ||
                    faithState.Intensity < PlayerDivineRightDeclarationSystem.DivineRightIntensityThreshold ||
                    faithState.Level < PlayerDivineRightDeclarationSystem.DivineRightLevelThreshold;

                if (!declarationFailed)
                {
                    continue;
                }

                DeactivateDivineRightOperation(entityManager, operationEntities[i], operation);
                DynastyPoliticalEventUtility.AddOrRefreshEvent(
                    entityManager,
                    factionEntity,
                    DynastyPoliticalEventTypes.DivineRightFailedCooldown,
                    inWorldDays + DynastyPoliticalEventUtility.ResolveDivineRightFailedCooldownInWorldDays(dualClock));
            }
        }

        static void DeactivateDivineRightOperation(
            EntityManager entityManager,
            Entity operationEntity,
            DynastyOperationComponent operation)
        {
            operation.Active = false;
            entityManager.SetComponentData(operationEntity, operation);
        }

        static void EnsureAggregate(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyPoliticalEventAggregateComponent aggregate)
        {
            if (entityManager.HasComponent<DynastyPoliticalEventAggregateComponent>(factionEntity))
            {
                entityManager.SetComponentData(factionEntity, aggregate);
            }
            else
            {
                entityManager.AddComponentData(factionEntity, aggregate);
            }
        }

        static Entity FindFactionEntity(EntityManager entityManager, FixedString32Bytes factionId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
