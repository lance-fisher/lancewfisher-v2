using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the fortification lane.
    /// Lets isolated smoke validators and later tooling inspect fortification
    /// tier and reserve state without depending on the live bootstrap scene.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetFortificationTier(string settlementId, out int tier, out int ceiling)
        {
            tier = 0;
            ceiling = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<FortificationComponent>(settlementEntity))
            {
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlementEntity);
            tier = fortification.Tier;
            ceiling = fortification.Ceiling;
            return true;
        }

        public bool TryDebugGetReserveCount(string settlementId, out int readyReserveCount)
        {
            readyReserveCount = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null)
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<FortificationReserveAssignmentComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var links = query.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (links[i].SettlementEntity != settlementEntity)
                {
                    continue;
                }

                var health = entityManager.GetComponentData<HealthComponent>(entities[i]);
                if (health.Current <= 0f)
                {
                    continue;
                }

                var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(entities[i]);
                if (assignment.Duty == ReserveDutyState.Ready)
                {
                    readyReserveCount++;
                }
            }

            return true;
        }

        public bool TryDebugGetFortificationBreachState(
            string settlementId,
            out int openBreachCount,
            out int destroyedWalls,
            out int destroyedTowers,
            out int destroyedGates,
            out int destroyedKeeps)
        {
            openBreachCount = 0;
            destroyedWalls = 0;
            destroyedTowers = 0;
            destroyedGates = 0;
            destroyedKeeps = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<FortificationComponent>(settlementEntity))
            {
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlementEntity);
            openBreachCount = fortification.OpenBreachCount;
            destroyedWalls = fortification.DestroyedWallSegmentCount;
            destroyedTowers = fortification.DestroyedTowerCount;
            destroyedGates = fortification.DestroyedGateCount;
            destroyedKeeps = fortification.DestroyedKeepCount;
            return true;
        }

        public bool TryDebugForceMuster(string settlementId, out int committedCount)
        {
            committedCount = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<FortificationComponent>(settlementEntity) ||
                !entityManager.HasComponent<FortificationReserveComponent>(settlementEntity) ||
                !entityManager.HasComponent<FactionComponent>(settlementEntity) ||
                !entityManager.HasComponent<PositionComponent>(settlementEntity))
            {
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlementEntity);
            var reserve = entityManager.GetComponentData<FortificationReserveComponent>(settlementEntity);
            var settlementPosition = entityManager.GetComponentData<PositionComponent>(settlementEntity).Value;
            var faction = entityManager.GetComponentData<FactionComponent>(settlementEntity);

            var hostileQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationCombatantTag>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            int hostileCount = 0;
            float3 hostileCenter = settlementPosition;
            using (var hostilePositions = hostileQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp))
            using (var hostileHealths = hostileQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp))
            using (var hostileFactions = hostileQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp))
            {
                float3 total = float3.zero;
                for (int i = 0; i < hostilePositions.Length; i++)
                {
                    if (hostileHealths[i].Current <= 0f ||
                        hostileFactions[i].FactionId.Equals(faction.FactionId) ||
                        math.distance(hostilePositions[i].Value, settlementPosition) > fortification.ThreatRadiusTiles)
                    {
                        continue;
                    }

                    hostileCount++;
                    total += hostilePositions[i].Value;
                }

                if (hostileCount > 0)
                {
                    hostileCenter = total / hostileCount;
                }
            }

            if (hostileCount == 0)
            {
                return false;
            }

            var linkedQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<FortificationReserveAssignmentComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var linkedEntities = linkedQuery.ToEntityArray(Allocator.Temp);
            using var links = linkedQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            int engagedCount = 0;
            int activeCount = 0;
            for (int i = 0; i < linkedEntities.Length; i++)
            {
                if (links[i].SettlementEntity != settlementEntity)
                {
                    continue;
                }

                var health = entityManager.GetComponentData<HealthComponent>(linkedEntities[i]);
                if (health.Current <= 0f)
                {
                    continue;
                }

                activeCount++;
                var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedEntities[i]);
                if (assignment.Duty == ReserveDutyState.Engaged || assignment.Duty == ReserveDutyState.Muster)
                {
                    engagedCount++;
                }
            }

            int desiredFrontline = math.min(activeCount, math.max(1, 1 + fortification.Tier));
            int neededReserves = math.max(1, desiredFrontline - engagedCount);

            for (int i = 0; i < linkedEntities.Length && committedCount < neededReserves; i++)
            {
                if (links[i].SettlementEntity != settlementEntity)
                {
                    continue;
                }

                var health = entityManager.GetComponentData<HealthComponent>(linkedEntities[i]);
                if (health.Current <= 0f || health.Max <= 0f)
                {
                    continue;
                }

                var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedEntities[i]);
                if (assignment.Duty != ReserveDutyState.Ready)
                {
                    continue;
                }

                float3 unitPosition = entityManager.GetComponentData<PositionComponent>(linkedEntities[i]).Value;
                float healthRatio = health.Current / health.Max;
                if (healthRatio < reserve.RecoveryHealthRatio ||
                    math.distance(unitPosition, settlementPosition) > fortification.ReserveRadiusTiles * 0.55f ||
                    math.distance(unitPosition, hostileCenter) <= fortification.ThreatRadiusTiles * 0.55f)
                {
                    continue;
                }

                assignment.Duty = ReserveDutyState.Muster;
                entityManager.SetComponentData(linkedEntities[i], assignment);
                committedCount++;
            }

            if (committedCount <= 0)
            {
                return false;
            }

            reserve.LastCommitAt = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            reserve.LastCommittedCount = committedCount;
            entityManager.SetComponentData(settlementEntity, reserve);
            return true;
        }

        private static Entity FindSettlementEntity(EntityManager entityManager, string settlementId)
        {
            if (string.IsNullOrWhiteSpace(settlementId))
            {
                return Entity.Null;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<FortificationComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (settlements[i].SettlementId.ToString() == settlementId)
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
