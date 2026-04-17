using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical stability-surplus loyalty restoration.
    ///
    /// Browser runtime equivalent: realm-condition stabilitySurplus effect applied
    /// at cycle close when food ratio >= StabilitySurplusFoodRatio (1.75) AND
    /// water ratio >= StabilitySurplusWaterRatio (1.75). Restores
    /// StabilitySurplusLoyaltyDeltaPerCycle (+1) to FactionLoyaltyComponent,
    /// capped at StabilitySurplusMaxLoyaltyToApply (95).
    ///
    /// Runs once per realm cycle close via its own LastStabilitySurplusResponseCycle
    /// marker so it cannot race or double-apply alongside StarvationResponseSystem
    /// or CapPressureResponseSystem. Completes the canonical realm-condition
    /// effect set (famine, water crisis, cap pressure, stability surplus).
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RealmConditionCycleSystem))]
    public partial struct StabilitySurplusResponseSystem : ISystem
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

            float foodThreshold = cfg.StabilitySurplusFoodRatio > 0f ? cfg.StabilitySurplusFoodRatio : 1.75f;
            float waterThreshold = cfg.StabilitySurplusWaterRatio > 0f ? cfg.StabilitySurplusWaterRatio : 1.75f;
            float loyaltyDelta = cfg.StabilitySurplusLoyaltyDeltaPerCycle;
            float cap = cfg.StabilitySurplusMaxLoyaltyToApply > 0f ? cfg.StabilitySurplusMaxLoyaltyToApply : 95f;

            foreach (var (realmRw, populationRo, resourcesRo, loyaltyRw) in
                SystemAPI.Query<
                    RefRW<RealmConditionComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<ResourceStockpileComponent>,
                    RefRW<FactionLoyaltyComponent>>())
            {
                ref var realm = ref realmRw.ValueRW;

                if (realm.CycleCount <= realm.LastStabilitySurplusResponseCycle)
                {
                    continue;
                }

                realm.LastStabilitySurplusResponseCycle = realm.CycleCount;

                int pop = populationRo.ValueRO.Total;
                if (pop <= 0)
                {
                    continue;
                }

                float foodRatio = resourcesRo.ValueRO.Food / pop;
                float waterRatio = resourcesRo.ValueRO.Water / pop;
                if (foodRatio < foodThreshold || waterRatio < waterThreshold)
                {
                    continue;
                }

                if (loyaltyDelta == 0f)
                {
                    continue;
                }

                ref var loyalty = ref loyaltyRw.ValueRW;
                if (loyalty.Current >= cap)
                {
                    continue;
                }

                float max = loyalty.Max > 0f ? loyalty.Max : 100f;
                float effectiveCap = math.min(cap, max);
                loyalty.Current = math.clamp(loyalty.Current + loyaltyDelta, loyalty.Floor, effectiveCap);
            }
        }
    }
}
