using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canonical population-cap pressure response.
    ///
    /// Browser runtime equivalent: realm-condition cap-pressure effect applied at
    /// cycle close when population.total / population.cap >= populationCapPressureRatio.
    ///
    /// Runs once per realm cycle close, after RealmConditionCycleSystem has
    /// advanced CycleCount. Applies the canonical
    /// capPressure.loyaltyDeltaPerCycle (-1) whenever population density crosses
    /// the canonical threshold (default 0.95). Loyalty is clamped to
    /// [FactionLoyaltyComponent.Floor, FactionLoyaltyComponent.Max].
    ///
    /// Runs independently from StarvationResponseSystem via a separate
    /// LastCapPressureResponseCycle marker on the realm component so the two
    /// cycle-close responders do not race or double-apply.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RealmConditionCycleSystem))]
    public partial struct CapPressureResponseSystem : ISystem
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

            float threshold = cfg.PopulationCapPressureRatio > 0f ? cfg.PopulationCapPressureRatio : 0.95f;
            float loyaltyDelta = cfg.CapPressureLoyaltyDeltaPerCycle;

            foreach (var (realmRw, populationRo, loyaltyRw, conviction) in
                SystemAPI.Query<
                    RefRW<RealmConditionComponent>,
                    RefRO<PopulationComponent>,
                    RefRW<FactionLoyaltyComponent>,
                    RefRO<ConvictionComponent>>())
            {
                ref var realm = ref realmRw.ValueRW;

                if (realm.CycleCount <= realm.LastCapPressureResponseCycle)
                {
                    continue;
                }

                realm.LastCapPressureResponseCycle = realm.CycleCount;

                int cap = populationRo.ValueRO.Cap;
                int total = populationRo.ValueRO.Total;
                if (cap <= 0 || total <= 0)
                {
                    continue;
                }

                float ratio = (float)total / cap;
                if (ratio < threshold)
                {
                    continue;
                }

                if (loyaltyDelta == 0f)
                {
                    continue;
                }

                ref var loyalty = ref loyaltyRw.ValueRW;
                float max = loyalty.Max > 0f ? loyalty.Max : 100f;
                float adjustedDelta = ApplyLoyaltyProtection(loyaltyDelta, conviction.ValueRO.Band);
                loyalty.Current = math.clamp(loyalty.Current + adjustedDelta, loyalty.Floor, max);
            }
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
