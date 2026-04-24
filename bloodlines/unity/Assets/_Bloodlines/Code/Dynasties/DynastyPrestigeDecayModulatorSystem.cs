using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Adjusts DynastyRenownComponent.RenownDecayRate each update so it reflects
    /// the dynasty's current political health. The accumulation system applies
    /// the rate as-written; this system makes it drift rather than stay fixed.
    ///
    /// Drift rules:
    ///   - Base rate: 0.45 per day (canonical default)
    ///   - Interregnum active: +50% of base (ruling void accelerates prestige loss)
    ///   - Active succession crisis (RecoveryProgressPct less than 1.0): +25% of base
    ///   - ConvictionBand ApexCruel: +15% of base (moral bankruptcy speeds decline)
    ///   - ConvictionBand ApexMoral: -15% of base (moral clarity slows decline)
    ///   - Floor: 0.10 per day (prestige decay never halts entirely)
    ///
    /// Browser equivalent: absent -- no dynasty-level prestige decay exists in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(DynastyRenownAccumulationSystem))]
    public partial struct DynastyPrestigeDecayModulatorSystem : ISystem
    {
        public const float BaseDecayRatePerDay    = 0.45f;
        public const float MinDecayRatePerDay     = 0.10f;
        public const float InterregnumPenalty     = 0.225f;  // +50% of base
        public const float CrisisPenalty          = 0.1125f; // +25% of base
        public const float ApexCruelPenalty       = 0.0675f; // +15% of base
        public const float ApexMoralDiscount      = 0.0675f; // -15% of base

        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            using var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<DynastyRenownComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity e = factionEntities[i];
                if (!em.HasComponent<DynastyRenownComponent>(e))
                    continue;

                var renown = em.GetComponentData<DynastyRenownComponent>(e);
                float rate = BaseDecayRatePerDay;

                if (em.HasComponent<DynastyStateComponent>(e))
                {
                    var dynasty = em.GetComponentData<DynastyStateComponent>(e);
                    if (dynasty.Interregnum)
                        rate += InterregnumPenalty;
                }

                if (em.HasComponent<SuccessionCrisisComponent>(e))
                {
                    var crisis = em.GetComponentData<SuccessionCrisisComponent>(e);
                    if (crisis.RecoveryProgressPct < 1f)
                        rate += CrisisPenalty;
                }

                if (em.HasComponent<ConvictionComponent>(e))
                {
                    var conviction = em.GetComponentData<ConvictionComponent>(e);
                    switch (conviction.Band)
                    {
                        case ConvictionBand.ApexCruel:
                            rate += ApexCruelPenalty;
                            break;
                        case ConvictionBand.ApexMoral:
                            rate -= ApexMoralDiscount;
                            break;
                    }
                }

                renown.RenownDecayRate = math.max(MinDecayRatePerDay, rate);
                em.SetComponentData(e, renown);
            }
        }
    }
}
