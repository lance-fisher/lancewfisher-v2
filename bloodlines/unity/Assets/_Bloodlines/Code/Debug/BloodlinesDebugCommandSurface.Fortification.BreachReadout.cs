using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Plain-data breach readout for settlement-focused fortification HUD work.
    /// </summary>
    public struct SettlementBreachReadout
    {
        public FixedString64Bytes SettlementId;
        public FixedString32Bytes OwnerFactionId;
        public int OpenBreachCount;
        public int DestroyedWallSegmentCount;
        public int DestroyedTowerCount;
        public int DestroyedGateCount;
        public int DestroyedKeepCount;
        public int CurrentTier;
        public int ReserveFrontage;
        public bool BreachAssaultAdvantageActive;
        public float AggregateAttackMultiplier;
        public float AggregateSpeedMultiplier;
    }

    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSettlementBreachReadout(
            FixedString32Bytes settlementId,
            out SettlementBreachReadout readout)
        {
            readout = default;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId.ToString());
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<FortificationComponent>(settlementEntity) ||
                !entityManager.HasComponent<FactionComponent>(settlementEntity))
            {
                return false;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(settlementEntity);
            var ownerFaction = entityManager.GetComponentData<FactionComponent>(settlementEntity);
            ResolveAggregateBreachAssaultState(
                entityManager,
                fortification.SettlementId,
                out bool breachAdvantageActive,
                out float aggregateAttackMultiplier,
                out float aggregateSpeedMultiplier);

            readout = new SettlementBreachReadout
            {
                SettlementId = fortification.SettlementId,
                OwnerFactionId = ownerFaction.FactionId,
                OpenBreachCount = fortification.OpenBreachCount,
                DestroyedWallSegmentCount = fortification.DestroyedWallSegmentCount,
                DestroyedTowerCount = fortification.DestroyedTowerCount,
                DestroyedGateCount = fortification.DestroyedGateCount,
                DestroyedKeepCount = fortification.DestroyedKeepCount,
                CurrentTier = fortification.Tier,
                ReserveFrontage = ResolveReserveFrontage(
                    entityManager,
                    settlementEntity,
                    ownerFaction.FactionId,
                    fortification),
                BreachAssaultAdvantageActive = breachAdvantageActive,
                AggregateAttackMultiplier = aggregateAttackMultiplier,
                AggregateSpeedMultiplier = aggregateSpeedMultiplier,
            };

            return true;
        }

        private static int ResolveReserveFrontage(
            EntityManager entityManager,
            Entity settlementEntity,
            FixedString32Bytes ownerFactionId,
            in FortificationComponent fortification)
        {
            int desiredFrontline = math.max(1, 1 + fortification.Tier);
            if (!entityManager.HasComponent<PositionComponent>(settlementEntity))
            {
                return desiredFrontline;
            }

            float3 settlementCenter = entityManager.GetComponentData<PositionComponent>(settlementEntity).Value;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationCombatantTag>(),
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var links = query.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var healths = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            int activeDefenderCount = 0;
            for (int i = 0; i < links.Length; i++)
            {
                if (links[i].SettlementEntity != settlementEntity ||
                    !factions[i].FactionId.Equals(ownerFactionId) ||
                    healths[i].Current <= 0f ||
                    math.distance(positions[i].Value, settlementCenter) > fortification.ReserveRadiusTiles)
                {
                    continue;
                }

                activeDefenderCount++;
            }

            return math.min(activeDefenderCount, desiredFrontline);
        }

        private static void ResolveAggregateBreachAssaultState(
            EntityManager entityManager,
            FixedString64Bytes settlementId,
            out bool breachAssaultAdvantageActive,
            out float aggregateAttackMultiplier,
            out float aggregateSpeedMultiplier)
        {
            breachAssaultAdvantageActive = false;
            aggregateAttackMultiplier = 1f;
            aggregateSpeedMultiplier = 1f;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FieldWaterComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var fieldWaters = query.ToComponentDataArray<FieldWaterComponent>(Allocator.Temp);
            using var healths = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < fieldWaters.Length; i++)
            {
                if (healths[i].Current <= 0f ||
                    !fieldWaters[i].BreachAssaultAdvantageActive ||
                    !fieldWaters[i].BreachTargetSettlementId.Equals(settlementId))
                {
                    continue;
                }

                breachAssaultAdvantageActive = true;
                aggregateAttackMultiplier = math.max(
                    aggregateAttackMultiplier,
                    fieldWaters[i].BreachAssaultAttackMultiplier);
                aggregateSpeedMultiplier = math.max(
                    aggregateSpeedMultiplier,
                    fieldWaters[i].BreachAssaultSpeedMultiplier);
            }
        }
    }
}
