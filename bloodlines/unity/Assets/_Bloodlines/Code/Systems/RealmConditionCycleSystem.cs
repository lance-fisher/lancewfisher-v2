using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical 90-second realm cycle. Drives famine, water crisis, and cap pressure
    /// events at the end of each cycle. Master doctrine section IV (population), V
    /// (water), VI (food) all lean on this cycle being canonical and not drifting.
    ///
    /// Browser runtime equivalent: tickRealmConditionCycle in simulation.js.
    ///
    /// This is the reference ECS system: it shows the pattern every other faction-scoped
    /// system in the Bloodlines Unity lane should follow:
    ///   - SystemBase or ISystem (ISystem preferred for Burst compatibility)
    ///   - OnUpdate reads deltaTime from SystemAPI.Time.DeltaTime
    ///   - Faction entities carry the singleton-like RealmConditionComponent
    ///   - Results remain additive: strain streaks increment or reset, they never silently
    ///     disappear mid-cycle
    /// </summary>
    [BurstCompile]
    public partial struct RealmConditionCycleSystem : ISystem
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

            float dt = SystemAPI.Time.DeltaTime;
            float cycleSeconds = cfg.CycleSeconds > 0f ? cfg.CycleSeconds : 90f;

            // Advance each faction's realm cycle accumulator. At cycle end, evaluate
            // food/water ratios against population and adjust famine/water-crisis streaks.
            foreach (var (realmRw, population, resources) in
                SystemAPI.Query<
                    RefRW<RealmConditionComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<ResourceStockpileComponent>>())
            {
                ref var realm = ref realmRw.ValueRW;
                realm.CycleAccumulator += dt;

                if (realm.CycleAccumulator < cycleSeconds)
                {
                    continue;
                }

                // Close one cycle, reset accumulator, increment cycle count.
                realm.CycleAccumulator = 0f;
                realm.CycleCount += 1;

                float pop = population.ValueRO.Total > 0 ? population.ValueRO.Total : 1f;
                float foodRatio = resources.ValueRO.Food / pop;
                float waterRatio = resources.ValueRO.Water / pop;

                float foodYellow = cfg.FoodYellowRatio > 0f ? cfg.FoodYellowRatio : 1.05f;
                float waterYellow = cfg.WaterYellowRatio > 0f ? cfg.WaterYellowRatio : 1.05f;

                // Canonical famine: consecutive sub-food cycles accrue streak. Reset on recovery.
                if (foodRatio < foodYellow)
                {
                    realm.FoodStrainStreak += 1;
                }
                else
                {
                    realm.FoodStrainStreak = 0;
                }

                // Canonical water crisis: one sub-water cycle is enough to trigger.
                if (waterRatio < waterYellow)
                {
                    realm.WaterStrainStreak += 1;
                }
                else
                {
                    realm.WaterStrainStreak = 0;
                }

                // Assault cohesion strain decays on cycle close as additional recovery hook
                // (browser runtime decays per-tick; ECS path uses cycle as a stable decay
                // point). Browser lane remains the working spec; this is parity work.
                if (realm.AssaultFailureStrain > 0f)
                {
                    realm.AssaultFailureStrain = realm.AssaultFailureStrain * 0.7f;
                    if (realm.AssaultFailureStrain < 0.05f)
                    {
                        realm.AssaultFailureStrain = 0f;
                    }
                }
            }
        }
    }
}
