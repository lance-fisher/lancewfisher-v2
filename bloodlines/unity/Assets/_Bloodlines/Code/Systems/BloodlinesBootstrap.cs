using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// One-shot bootstrap that seeds the singleton RealmCycleConfig with canonical
    /// defaults if it has not already been created by the JSON content importer.
    /// Browser runtime equivalent: state.realmConditions (loaded from data/realm-conditions.json).
    ///
    /// Runs once on world initialization. Does not touch faction-level state.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BloodlinesBootstrapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Create the RealmCycleConfig singleton if it does not exist.
            // JsonContentImporter will overwrite with canon values from data/realm-conditions.json.
            if (!SystemAPI.HasSingleton<RealmCycleConfig>())
            {
                var configEntity = EntityManager.CreateEntity(typeof(RealmCycleConfig));
                EntityManager.SetName(configEntity, "RealmCycleConfig");
                EntityManager.SetComponentData(configEntity, new RealmCycleConfig
                {
                    CycleSeconds = 90f,
                    FoodGreenRatio = 1.35f,
                    FoodYellowRatio = 1.05f,
                    WaterGreenRatio = 1.35f,
                    WaterYellowRatio = 1.05f,
                    LoyaltyGreenFloor = 62f,
                    LoyaltyYellowFloor = 32f,
                    PopulationGreenCapRatio = 0.75f,
                    PopulationYellowCapRatio = 0.92f,
                    FoodFamineConsecutiveCycles = 2,
                    WaterCrisisConsecutiveCycles = 1,
                    FaminePopulationDeclinePerCycle = 1,
                    WaterCrisisOutmigrationPerCycle = 1,
                    FamineLoyaltyDeltaPerCycle = -4,
                    WaterCrisisLoyaltyDeltaPerCycle = -6,
                });
            }
        }

        protected override void OnUpdate()
        {
            // One-shot: disable after first update.
            Enabled = false;
        }
    }
}
