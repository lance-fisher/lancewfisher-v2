using Bloodlines.Components;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Collects early-game state from the player faction each frame and writes it
    /// to the EarlyGameHUDComponent singleton for the UI layer to read.
    ///
    /// Only the "player" faction is surfaced (FactionId == "player").
    /// The singleton entity is created if it does not exist.
    ///
    /// Canon: early-game-foundation prompt 2026-04-25 UI requirements.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct EarlyGameHUDSystem : ISystem
    {
        static readonly FixedString32Bytes PlayerFactionId = new("player");

        Entity _hudEntity;

        public void OnCreate(ref SystemState state)
        {
            _hudEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(_hudEntity, new EarlyGameHUDComponent());
        }

        public void OnUpdate(ref SystemState state)
        {
            ref var hud = ref SystemAPI.GetComponentRW<EarlyGameHUDComponent>(_hudEntity).ValueRW;

            hud.TotalSquads      = 0;
            hud.ReserveSquads    = 0;
            hud.ActiveDutySquads = 0;

            // --- Pass 1: core faction stats (population, resources, draft, productivity) ---
            foreach (var (faction, pop, resources, draft, productivity, entity) in
                SystemAPI.Query<
                    RefRO<FactionComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<ResourceStockpileComponent>,
                    RefRO<MilitaryDraftComponent>,
                    RefRO<PopulationProductivityComponent>>()
                    .WithEntityAccess())
            {
                if (faction.ValueRO.FactionId != PlayerFactionId) continue;

                hud.PopTotal    = pop.ValueRO.Total;
                hud.PopCap      = pop.ValueRO.Cap;
                hud.PopAvailable = pop.ValueRO.Available;

                hud.DraftRatePct       = draft.ValueRO.DraftRatePct;
                hud.DraftPool          = draft.ValueRO.DraftPool;
                hud.TrainedMilitary    = draft.ValueRO.TrainedMilitary;
                hud.UntrainedDrafted   = draft.ValueRO.UntrainedDrafted;
                hud.ReserveMilitary    = draft.ValueRO.ReserveMilitary;
                hud.ActiveDutyMilitary = draft.ValueRO.ActiveDutyMilitary;
                hud.OverDrafted        = draft.ValueRO.OverDraftedFlag;

                hud.BaseProductivity      = productivity.ValueRO.BaseProductivity;
                hud.EffectiveProductivity = productivity.ValueRO.EffectiveProductivity;
                hud.FoodAdequate          = productivity.ValueRO.FoodAdequate;
                hud.WaterAdequate         = productivity.ValueRO.WaterAdequate;
                hud.HousingAdequate       = productivity.ValueRO.HousingAdequate;

                hud.Food  = resources.ValueRO.Food;
                hud.Water = resources.ValueRO.Water;
                hud.Wood  = resources.ValueRO.Wood;

                // Pass 2: overflow components fetched via EntityManager to stay under query limit.
                if (state.EntityManager.HasComponent<WaterCapacityComponent>(entity))
                    hud.WaterCapacity = state.EntityManager.GetComponentData<WaterCapacityComponent>(entity).MaxSupportedByWater;

                if (state.EntityManager.HasComponent<FoundingRetinueComponent>(entity))
                {
                    var retinue = state.EntityManager.GetComponentData<FoundingRetinueComponent>(entity);
                    hud.KeepDeployed = retinue.IsDeployed;
                }

                if (state.EntityManager.HasComponent<BuildTierComponent>(entity))
                {
                    var buildTier = state.EntityManager.GetComponentData<BuildTierComponent>(entity);
                    hud.BuildTier       = buildTier.CurrentTier;
                    hud.HasHousing      = buildTier.HasHousing;
                    hud.HasWater        = buildTier.HasWater;
                    hud.HasFoodSource   = buildTier.HasFoodSource;
                    hud.HasTrainingYard = buildTier.HasTrainingYard;
                }

                break;
            }

            // --- Squad tally for player squads ---
            foreach (var (faction, squad) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<MilitiaSquadComponent>>())
            {
                if (faction.ValueRO.FactionId != PlayerFactionId) continue;
                hud.TotalSquads++;
                if (squad.ValueRO.DutyState == DutyState.Reserve)
                    hud.ReserveSquads++;
                else
                    hud.ActiveDutySquads++;
            }
        }
    }
}
