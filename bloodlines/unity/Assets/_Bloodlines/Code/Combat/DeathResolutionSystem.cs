using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First death finalization pass. Marks dead entities, clears hostile references, and
    /// releases the faction population slot consumed by dead units.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AttackResolutionSystem))]
    public partial struct DeathResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var attackTargetQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AttackTargetComponent>());

            using var attackTargetEntities = attackTargetQuery.ToEntityArray(Allocator.Temp);
            using var attackTargets = attackTargetQuery.ToComponentDataArray<AttackTargetComponent>(Allocator.Temp);

            foreach (var (health, entity) in SystemAPI.Query<RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current > 0f)
                {
                    continue;
                }

                ecb.AddComponent<DeadTag>(entity);
                if (entityManager.HasComponent<AttackTargetComponent>(entity))
                {
                    ecb.RemoveComponent<AttackTargetComponent>(entity);
                }

                if (entityManager.HasComponent<MoveCommandComponent>(entity))
                {
                    var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(entity);
                    moveCommand.IsActive = false;
                    entityManager.SetComponentData(entity, moveCommand);
                }

                for (int i = 0; i < attackTargets.Length; i++)
                {
                    if (attackTargets[i].TargetEntity == entity)
                    {
                        ecb.RemoveComponent<AttackTargetComponent>(attackTargetEntities[i]);
                    }
                }

                if (entityManager.HasComponent<UnitTypeComponent>(entity) &&
                    entityManager.HasComponent<FactionComponent>(entity))
                {
                    var unitType = entityManager.GetComponentData<UnitTypeComponent>(entity);
                    if (unitType.PopulationCost > 0 &&
                        TryFindFactionPopulation(
                            entityManager,
                            entityManager.GetComponentData<FactionComponent>(entity).FactionId,
                            out var population,
                            out var factionEntity))
                    {
                        population.Available = math.min(population.Cap, population.Available + unitType.PopulationCost);
                        entityManager.SetComponentData(factionEntity, population);
                    }
                }
            }
        }

        private static bool TryFindFactionPopulation(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out PopulationComponent population,
            out Entity factionEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PopulationComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                population = populations[i];
                factionEntity = entities[i];
                return true;
            }

            population = default;
            factionEntity = Entity.Null;
            return false;
        }
    }
}
