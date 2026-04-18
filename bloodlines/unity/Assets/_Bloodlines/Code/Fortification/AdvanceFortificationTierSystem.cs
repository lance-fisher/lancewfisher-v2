using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Deterministically recomputes settlement fortification tier from linked,
    /// completed building contributions.
    /// Browser reference: simulation.js advanceFortificationTier (11227).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AdvanceFortificationTierSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationBuildingContributionComponent>(),
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var buildingEntities = buildingQuery.ToEntityArray(Allocator.Temp);
            using var contributions = buildingQuery.ToComponentDataArray<FortificationBuildingContributionComponent>(Allocator.Temp);
            using var links = buildingQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var healths = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            foreach (var (fortificationRw, settlementEntity) in
                SystemAPI.Query<RefRW<FortificationComponent>>().WithEntityAccess())
            {
                int tier = 0;
                for (int i = 0; i < buildingEntities.Length; i++)
                {
                    if (links[i].SettlementEntity != settlementEntity ||
                        healths[i].Current <= 0f ||
                        entityManager.HasComponent<ConstructionStateComponent>(buildingEntities[i]))
                    {
                        continue;
                    }

                    tier += math.max(0, contributions[i].TierContribution);
                }

                var fortification = fortificationRw.ValueRO;
                fortification.Tier = math.clamp(tier, 0, math.max(0, fortification.Ceiling));
                fortificationRw.ValueRW = fortification;

                if (entityManager.HasComponent<SettlementComponent>(settlementEntity))
                {
                    var settlement = entityManager.GetComponentData<SettlementComponent>(settlementEntity);
                    settlement.FortificationTier = fortification.Tier;
                    settlement.FortificationCeiling = fortification.Ceiling;
                    entityManager.SetComponentData(settlementEntity, settlement);
                }
            }
        }
    }
}
