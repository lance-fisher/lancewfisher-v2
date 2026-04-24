using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Siege;
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

            foreach (var (realmRw, populationRw, loyaltyRw, conviction, entity) in
                SystemAPI.Query<
                    RefRW<RealmConditionComponent>,
                    RefRW<PopulationComponent>,
                    RefRW<FactionLoyaltyComponent>,
                    RefRO<ConvictionComponent>>()
                .WithEntityAccess())
            {
                ref var realm = ref realmRw.ValueRW;
                float siegeStarvationMultiplier = SystemAPI.HasComponent<FactionSiegeEscalationStateComponent>(entity)
                    ? SystemAPI.GetComponent<FactionSiegeEscalationStateComponent>(entity).StarvationMultiplier
                    : 1.0f;

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
                    int scaledDecline = (int)math.ceil(totalPopulationDecline * siegeStarvationMultiplier);
                    int adjustedDecline = ApplyPopulationProtection(
                        scaledDecline,
                        conviction.ValueRO.Band);
                    int newTotal = math.max(0, population.Total - adjustedDecline);
                    int appliedDecline = population.Total - newTotal;
                    population.Total = newTotal;
                    population.Available = math.max(0, population.Available - appliedDecline);
                }

                if (totalLoyaltyDelta != 0f)
                {
                    ref var loyalty = ref loyaltyRw.ValueRW;
                    float adjustedDelta = ApplyLoyaltyProtection(totalLoyaltyDelta, conviction.ValueRO.Band);
                    float newLoyalty = loyalty.Current + adjustedDelta;
                    float max = loyalty.Max > 0f ? loyalty.Max : 100f;
                    loyalty.Current = math.clamp(newLoyalty, loyalty.Floor, max);
                }
            }
        }

        private static int ApplyPopulationProtection(int totalPopulationDecline, ConvictionBand band)
        {
            if (totalPopulationDecline <= 0)
            {
                return 0;
            }

            float protection = math.max(1f, ConvictionBandEffects.ForBand(band).PopulationGrowthMultiplier);
            int adjustedDecline = (int)math.floor(totalPopulationDecline / protection);
            return math.clamp(adjustedDecline, 1, totalPopulationDecline);
        }

        private static float ApplyLoyaltyProtection(float loyaltyDelta, ConvictionBand band)
        {
            if (loyaltyDelta >= 0f)
            {
                return loyaltyDelta;
            }

            float protection = math.max(1f, ConvictionBandEffects.ForBand(band).LoyaltyProtectionMultiplier);
            return loyaltyDelta / protection;
        }
    }
}
