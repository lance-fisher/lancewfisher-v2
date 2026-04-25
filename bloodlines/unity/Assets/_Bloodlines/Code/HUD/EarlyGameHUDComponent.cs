using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton HUD snapshot for the early-game foundation panel.
    /// Written by EarlyGameHUDSystem each frame for the player faction.
    ///
    /// Surfaces: Keep deployment state, build tier, population breakdown,
    /// draft state, water/food/housing adequacy, worker slot fill, squad counts.
    ///
    /// Canon: early-game-foundation prompt 2026-04-25 UI requirements.
    /// </summary>
    public struct EarlyGameHUDComponent : IComponentData
    {
        // Keep / founding state
        public bool KeepDeployed;
        public byte BuildTier;
        public bool HasHousing;
        public bool HasWater;
        public bool HasFoodSource;
        public bool HasTrainingYard;

        // Population
        public int PopTotal;
        public int PopCap;
        public int PopAvailable;
        public int WaterCapacity;   // max population water infrastructure supports

        // Draft
        public int DraftRatePct;
        public int DraftPool;
        public int TrainedMilitary;
        public int UntrainedDrafted;
        public int ReserveMilitary;
        public int ActiveDutyMilitary;
        public bool OverDrafted;

        // Productivity
        public float BaseProductivity;
        public float EffectiveProductivity;
        public bool FoodAdequate;
        public bool WaterAdequate;
        public bool HousingAdequate;

        // Resources (for context)
        public float Food;
        public float Water;
        public float Wood;

        // Squad counts (all factions squads tallied for player)
        public int TotalSquads;
        public int ReserveSquads;
        public int ActiveDutySquads;
    }
}
