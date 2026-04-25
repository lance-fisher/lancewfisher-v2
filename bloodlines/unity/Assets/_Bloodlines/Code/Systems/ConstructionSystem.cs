using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First ECS construction resolver.
    /// Construction sites progress over time, gain health as they complete, and apply
    /// their population-cap bonus to the owning faction when they finish.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ConstructionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConstructionStateComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (constructionRw, healthRw, buildingType, faction, entity) in
                SystemAPI.Query<
                    RefRW<ConstructionStateComponent>,
                    RefRW<HealthComponent>,
                    RefRO<BuildingTypeComponent>,
                    RefRO<FactionComponent>>()
                .WithEntityAccess())
            {
                var construction = constructionRw.ValueRO;
                // Browser parity (data/houses.json mechanics block + advanceFortificationTier
                // call sites): apply HouseMechanics.FortificationBuildSpeedMultiplier when the
                // building under construction is a fortification (Wall/Tower/Gate/Keep).
                // Stonehelm = 1.2; all other houses = 1.0 by default.
                float effectiveDt = dt;
                if (buildingType.ValueRO.FortificationRole != FortificationRole.None)
                {
                    float speedMultiplier = ResolveFortBuildSpeedMultiplier(entityManager, faction.ValueRO.FactionId);
                    effectiveDt = dt * speedMultiplier;
                }
                construction.RemainingSeconds = math.max(0f, construction.RemainingSeconds - effectiveDt);

                float totalSeconds = math.max(0.1f, construction.TotalSeconds);
                float progress = math.saturate(1f - (construction.RemainingSeconds / totalSeconds));

                var health = healthRw.ValueRO;
                health.Max = math.max(1f, health.Max);
                health.Current = math.clamp(
                    math.lerp(construction.StartingHealth, health.Max, progress),
                    construction.StartingHealth,
                    health.Max);
                healthRw.ValueRW = health;

                if (construction.RemainingSeconds > 0f)
                {
                    constructionRw.ValueRW = construction;
                    continue;
                }

                if (buildingType.ValueRO.PopulationCapBonus > 0 &&
                    TryFindFactionPopulation(entityManager, faction.ValueRO.FactionId, out var population, out var populationEntity))
                {
                    population.CapBonus += buildingType.ValueRO.PopulationCapBonus;
                    population.Cap = population.BaseCap + population.CapBonus;
                    entityManager.SetComponentData(populationEntity, population);
                }

                ecb.RemoveComponent<ConstructionStateComponent>(entity);
            }
        }

        private static float ResolveFortBuildSpeedMultiplier(EntityManager entityManager, FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HouseMechanicsComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var mechanics = query.ToComponentDataArray<HouseMechanicsComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    float m = mechanics[i].FortificationBuildSpeedMultiplier;
                    return m > 0f ? m : 1f;
                }
            }
            return 1f;
        }

        private static bool TryFindFactionPopulation(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out PopulationComponent population,
            out Entity populationEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(PopulationComponent));

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
                populationEntity = entities[i];
                return true;
            }

            population = default;
            populationEntity = Entity.Null;
            return false;
        }
    }
}
