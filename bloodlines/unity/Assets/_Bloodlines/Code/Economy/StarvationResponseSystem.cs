using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical famine and water-crisis population response.
    ///
    /// Browser runtime equivalent: realm-condition effects applied at cycle close
    /// (data/realm-conditions.json).
    ///
    /// Runs once per realm cycle close, after RealmConditionCycleSystem has updated
    /// the streaks and advanced CycleCount. If the faction has reached the canonical
    /// famine or water-crisis threshold, applies population decline / outmigration
    /// to its PopulationComponent exactly once for that cycle. Population never
    /// drops below zero. PopulationComponent.Available is clamped alongside Total.
    ///
    /// Loyalty deltas, morale multipliers, and agriculture multipliers remain
    /// out of scope for this first starvation-response slice and will be layered
    /// in as those systems come online.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RealmConditionCycleSystem))]
    public partial struct StarvationResponseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RealmCycleConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out RealmCycleConfig cfg))
            {
                return;
            }

            int famineThreshold = math.max(1, cfg.FoodFamineConsecutiveCycles);
            int waterCrisisThreshold = math.max(1, cfg.WaterCrisisConsecutiveCycles);
            int faminePopulationDecline = math.max(0, cfg.FaminePopulationDeclinePerCycle);
            int waterCrisisOutmigration = math.max(0, cfg.WaterCrisisOutmigrationPerCycle);
            float famineLoyaltyDelta = cfg.FamineLoyaltyDeltaPerCycle;
            float waterCrisisLoyaltyDelta = cfg.WaterCrisisLoyaltyDeltaPerCycle;

            foreach (var (realmRw, populationRw, loyaltyRw) in
                SystemAPI.Query<
                    RefRW<RealmConditionComponent>,
                    RefRW<PopulationComponent>,
                    RefRW<FactionLoyaltyComponent>>())
            {
                ref var realm = ref realmRw.ValueRW;

                if (realm.CycleCount <= realm.LastStarvationResponseCycle)
                {
                    continue;
                }

                bool famineActive = realm.FoodStrainStreak >= famineThreshold;
                bool waterCrisisActive = realm.WaterStrainStreak >= waterCrisisThreshold;

                int totalPopulationDecline = 0;
                float totalLoyaltyDelta = 0f;

                if (famineActive)
                {
                    totalPopulationDecline += faminePopulationDecline;
                    totalLoyaltyDelta += famineLoyaltyDelta;
                }

                if (waterCrisisActive)
                {
                    totalPopulationDecline += waterCrisisOutmigration;
                    totalLoyaltyDelta += waterCrisisLoyaltyDelta;
                }

                realm.LastStarvationResponseCycle = realm.CycleCount;

                if (totalPopulationDecline > 0)
                {
                    ref var population = ref populationRw.ValueRW;
                    int newTotal = math.max(0, population.Total - totalPopulationDecline);
                    int appliedDecline = population.Total - newTotal;
                    population.Total = newTotal;
                    population.Available = math.max(0, population.Available - appliedDecline);
                }

                if (totalLoyaltyDelta != 0f)
                {
                    ref var loyalty = ref loyaltyRw.ValueRW;
                    float newLoyalty = loyalty.Current + totalLoyaltyDelta;
                    float max = loyalty.Max > 0f ? loyalty.Max : 100f;
                    loyalty.Current = math.clamp(newLoyalty, loyalty.Floor, max);
                }
            }
        }
    }
}
