using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Calculates per-faction effective productivity each frame.
    ///
    /// Two-stage calculation:
    ///
    ///   Stage 1 — BaseProductivity: weighted average of the four population states.
    ///     Civilian (not drafted):        100%
    ///     Drafted but untrained:          75%
    ///     Trained Reserve:                50%
    ///     Trained Active Duty:             5%
    ///
    ///   Stage 2 — Settlement condition modifiers (multiplicative, applied in order):
    ///     Food shortage:    × 0.70
    ///     Water shortage:   × 0.65
    ///     Housing at cap:   × 0.85
    ///
    /// Shortage accumulator tracking triggers hooks for future sickness / desertion /
    /// instability events. The accumulators are reset when the condition clears.
    ///
    /// EffectiveProductivity is consumed by WorkerSlotProductionSystem.
    ///
    /// Canon: RESOURCE_SYSTEM.md "Production scaling by population allocation",
    ///        early-game-foundation prompt 2026-04-25 productivity model.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WaterCapacitySystem))]
    [UpdateBefore(typeof(WorkerSlotProductionSystem))]
    public partial struct PopulationProductivitySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (productivityRw, population, resources, draft, waterCap) in
                SystemAPI.Query<
                    RefRW<PopulationProductivityComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<ResourceStockpileComponent>,
                    RefRO<MilitaryDraftComponent>,
                    RefRO<WaterCapacityComponent>>())
            {
                ref var prod = ref productivityRw.ValueRW;
                var pop      = population.ValueRO;
                var res      = resources.ValueRO;
                var draftData    = draft.ValueRO;
                var waterCapData = waterCap.ValueRO;

                // --- Stage 1: Base productivity from population state mix ---

                int total = math.max(1, pop.Total);

                int activeDuty  = draftData.ActiveDutyMilitary;
                int reserve     = draftData.ReserveMilitary;
                int untrained   = draftData.UntrainedDrafted;
                int civilian    = math.max(0, total - activeDuty - reserve - untrained);

                float baseProductivity =
                    (civilian  * EarlyGameConstants.ProductivityCivilian          +
                     untrained * EarlyGameConstants.ProductivityDraftedUntrained  +
                     reserve   * EarlyGameConstants.ProductivityTrainedReserve    +
                     activeDuty* EarlyGameConstants.ProductivityTrainedActiveDuty)
                    / total;

                prod.BaseProductivity = math.clamp(baseProductivity, 0f, 1f);

                // --- Stage 2: Settlement condition modifiers ---

                // Food: needs some positive stockpile as a proxy for adequacy.
                bool foodOk    = res.Food > 0f;
                bool waterOk   = pop.Total <= waterCapData.MaxSupportedByWater;
                bool housingOk = pop.Total < pop.Cap;

                prod.FoodAdequate    = foodOk;
                prod.WaterAdequate   = waterOk;
                prod.HousingAdequate = housingOk;

                // Accumulate shortage durations for future escalation hooks.
                prod.FoodShortageAccumulator  = foodOk  ? 0f : prod.FoodShortageAccumulator  + dt;
                prod.WaterShortageAccumulator = waterOk ? 0f : prod.WaterShortageAccumulator + dt;

                float effective = prod.BaseProductivity;
                if (!foodOk)    effective *= EarlyGameConstants.FoodShortageModifier;
                if (!waterOk)   effective *= EarlyGameConstants.WaterShortageModifier;
                if (!housingOk) effective *= EarlyGameConstants.HousingShortageModifier;

                prod.EffectiveProductivity = math.clamp(effective, 0f, 1f);
            }
        }
    }
}
