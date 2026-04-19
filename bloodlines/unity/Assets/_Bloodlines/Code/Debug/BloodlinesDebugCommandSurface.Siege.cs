using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the siege logistics / field-water slice.
    /// Exposes the minimal governed inspection seam needed by isolated validators
    /// and later operational HUD work without touching the base input surface.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetFieldWaterState(Entity unitEntity, out FieldWaterComponent fieldWater)
        {
            fieldWater = default;
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity) ||
                !entityManager.HasComponent<FieldWaterComponent>(unitEntity))
            {
                return false;
            }

            fieldWater = entityManager.GetComponentData<FieldWaterComponent>(unitEntity);
            return true;
        }

        public bool TryDebugGetSiegeSupportState(Entity unitEntity, out SiegeSupportComponent siegeSupport)
        {
            siegeSupport = default;
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity) ||
                !entityManager.HasComponent<SiegeSupportComponent>(unitEntity))
            {
                return false;
            }

            siegeSupport = entityManager.GetComponentData<SiegeSupportComponent>(unitEntity);
            return true;
        }

        public bool TryDebugGetSiegeSupplyTrainState(Entity unitEntity, out SiegeSupplyTrainComponent supplyTrain)
        {
            supplyTrain = default;
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(unitEntity) ||
                !entityManager.HasComponent<SiegeSupplyTrainComponent>(unitEntity))
            {
                return false;
            }

            supplyTrain = entityManager.GetComponentData<SiegeSupplyTrainComponent>(unitEntity);
            return true;
        }

        public bool TryDebugGetSiegeSupplyCampState(Entity buildingEntity, out Bloodlines.Siege.SiegeSupplyCampComponent supplyCamp)
        {
            supplyCamp = default;
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(buildingEntity) ||
                !entityManager.HasComponent<Bloodlines.Siege.SiegeSupplyCampComponent>(buildingEntity))
            {
                return false;
            }

            supplyCamp = entityManager.GetComponentData<Bloodlines.Siege.SiegeSupplyCampComponent>(buildingEntity);
            return true;
        }

        public bool TryDebugGetFactionStockpile(
            FixedString32Bytes factionId,
            out ResourceStockpileComponent stockpile)
        {
            stockpile = default;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ResourceStockpileComponent>());

            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            for (int i = 0; i < factions.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                stockpile = stockpiles[i];
                return true;
            }

            return false;
        }
    }
}
