using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugSetRallyPoint(Entity buildingEntity, float3 targetPosition)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(buildingEntity) ||
                !entityManager.HasComponent<BuildingTypeComponent>(buildingEntity))
            {
                return false;
            }

            var request = new PlayerRallyPointSetRequestComponent
            {
                TargetPosition = targetPosition,
                IsActive = true,
            };

            if (entityManager.HasComponent<PlayerRallyPointSetRequestComponent>(buildingEntity))
            {
                entityManager.SetComponentData(buildingEntity, request);
            }
            else
            {
                entityManager.AddComponentData(buildingEntity, request);
            }

            return true;
        }

        public bool TryDebugClearRallyPoint(Entity buildingEntity)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(buildingEntity) ||
                !entityManager.HasComponent<BuildingTypeComponent>(buildingEntity))
            {
                return false;
            }

            var request = new PlayerRallyPointSetRequestComponent
            {
                TargetPosition = float3.zero,
                IsActive = false,
            };

            if (entityManager.HasComponent<PlayerRallyPointSetRequestComponent>(buildingEntity))
            {
                entityManager.SetComponentData(buildingEntity, request);
            }
            else
            {
                entityManager.AddComponentData(buildingEntity, request);
            }

            return true;
        }

        public bool TryDebugGetRallyPoint(Entity buildingEntity, out float3 targetPosition, out bool isActive)
        {
            targetPosition = float3.zero;
            isActive = false;

            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(buildingEntity) ||
                !entityManager.HasComponent<RallyPointComponent>(buildingEntity))
            {
                return false;
            }

            var rally = entityManager.GetComponentData<RallyPointComponent>(buildingEntity);
            targetPosition = rally.TargetPosition;
            isActive = rally.IsActive;
            return true;
        }
    }
}
