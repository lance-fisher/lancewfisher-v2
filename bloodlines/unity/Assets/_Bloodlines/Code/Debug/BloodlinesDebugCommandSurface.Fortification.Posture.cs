using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugSetImminentEngagementPosture(string settlementId, int postureId)
        {
            if (!TryGetEntityManager(out var entityManager) ||
                string.IsNullOrWhiteSpace(settlementId))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<ImminentEngagementComponent>(settlementEntity) ||
                !ImminentEngagementPostureUtility.IsValidPostureId((byte)postureId))
            {
                return false;
            }

            byte normalizedPostureId = (byte)postureId;
            var engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlementEntity);
            engagement.SelectedResponseId = ImminentEngagementPostureUtility.ResolveResponseId(normalizedPostureId);
            engagement.SelectedResponseLabel = ImminentEngagementPostureUtility.ResolveResponseLabel(normalizedPostureId);
            entityManager.SetComponentData(settlementEntity, engagement);

            if (engagement.Active)
            {
                var posture = ImminentEngagementPostureUtility.ResolveComponent(normalizedPostureId);
                if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
                {
                    entityManager.SetComponentData(settlementEntity, posture);
                }
                else
                {
                    entityManager.AddComponentData(settlementEntity, posture);
                }
            }
            else if (entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
            {
                entityManager.RemoveComponent<ImminentEngagementPostureComponent>(settlementEntity);
            }

            return true;
        }
    }
}
