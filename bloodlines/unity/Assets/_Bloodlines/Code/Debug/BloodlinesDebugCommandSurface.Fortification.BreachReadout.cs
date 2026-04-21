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

    /// <summary>
    /// Extended fortification telemetry for smoke validation and future debug HUD work.
    /// Keeps the player-facing breach readout intact while exposing sealing and
    /// destroyed-counter recovery progress with their live costs.
    /// </summary>
    public struct SettlementBreachTelemetry
    {
        public SettlementBreachReadout Readout;
        public bool SealingEligible;
        public bool SealingTracked;
        public float SealingAccumulatedWorkerHours;
        public float SealingRequiredWorkerHours;
        public float SealingReservedStone;
        public float SealingRequiredStone;
        public float SealingProgress01;
        public bool RecoveryEligible;
        public bool RecoveryTracked;
        public DestroyedCounterKind RecoveryTargetCounter;
        public float RecoveryAccumulatedWorkerHours;
        public float RecoveryRequiredWorkerHours;
        public float RecoveryReservedStone;
        public float RecoveryRequiredStone;
        public float RecoveryProgress01;
    }

    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSettlementBreachReadout(
            FixedString32Bytes settlementId,
            out SettlementBreachReadout readout)
        {
            readout = default;
            if (!TryDebugGetSettlementBreachTelemetry(settlementId, out var telemetry))
            {
                return false;
            }

            readout = telemetry.Readout;
            return true;
        }

        public bool TryDebugGetSettlementBreachTelemetry(
            FixedString32Bytes settlementId,
            out SettlementBreachTelemetry telemetry)
        {
            telemetry = default;
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
            ResolveSealingTelemetry(
                entityManager,
                settlementEntity,
                fortification,
                out bool sealingEligible,
                out bool sealingTracked,
                out float sealingAccumulatedWorkerHours,
                out float sealingRequiredWorkerHours,
                out float sealingReservedStone,
                out float sealingRequiredStone,
                out float sealingProgress01);
            ResolveRecoveryTelemetry(
                entityManager,
                settlementEntity,
                fortification,
                out bool recoveryEligible,
                out bool recoveryTracked,
                out DestroyedCounterKind recoveryTargetCounter,
                out float recoveryAccumulatedWorkerHours,
                out float recoveryRequiredWorkerHours,
                out float recoveryReservedStone,
                out float recoveryRequiredStone,
                out float recoveryProgress01);

            var readout = new SettlementBreachReadout
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

            telemetry = new SettlementBreachTelemetry
            {
                Readout = readout,
                SealingEligible = sealingEligible,
                SealingTracked = sealingTracked,
                SealingAccumulatedWorkerHours = sealingAccumulatedWorkerHours,
                SealingRequiredWorkerHours = sealingRequiredWorkerHours,
                SealingReservedStone = sealingReservedStone,
                SealingRequiredStone = sealingRequiredStone,
                SealingProgress01 = sealingProgress01,
                RecoveryEligible = recoveryEligible,
                RecoveryTracked = recoveryTracked,
                RecoveryTargetCounter = recoveryTargetCounter,
                RecoveryAccumulatedWorkerHours = recoveryAccumulatedWorkerHours,
                RecoveryRequiredWorkerHours = recoveryRequiredWorkerHours,
                RecoveryReservedStone = recoveryReservedStone,
                RecoveryRequiredStone = recoveryRequiredStone,
                RecoveryProgress01 = recoveryProgress01,
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

        private static void ResolveSealingTelemetry(
            EntityManager entityManager,
            Entity settlementEntity,
            in FortificationComponent fortification,
            out bool sealingEligible,
            out bool sealingTracked,
            out float accumulatedWorkerHours,
            out float requiredWorkerHours,
            out float reservedStone,
            out float requiredStone,
            out float progress01)
        {
            sealingEligible = fortification.OpenBreachCount > 0;
            sealingTracked = entityManager.HasComponent<BreachSealingProgressComponent>(settlementEntity);
            accumulatedWorkerHours = 0f;
            requiredWorkerHours = sealingEligible
                ? FortificationCanon.ResolveBreachSealingWorkerHoursPerBreach(fortification.Tier)
                : 0f;
            reservedStone = 0f;
            requiredStone = sealingEligible
                ? FortificationCanon.ResolveBreachSealingStoneCostPerBreach(fortification.Tier)
                : 0f;
            progress01 = 0f;

            if (!sealingTracked)
            {
                return;
            }

            var progress = entityManager.GetComponentData<BreachSealingProgressComponent>(settlementEntity);
            accumulatedWorkerHours = progress.AccumulatedWorkerHours;
            reservedStone = progress.StoneReservedForCurrentBreach;
            if (requiredWorkerHours > 0f)
            {
                progress01 = math.clamp(accumulatedWorkerHours / requiredWorkerHours, 0f, 1f);
            }
        }

        private static void ResolveRecoveryTelemetry(
            EntityManager entityManager,
            Entity settlementEntity,
            in FortificationComponent fortification,
            out bool recoveryEligible,
            out bool recoveryTracked,
            out DestroyedCounterKind targetCounter,
            out float accumulatedWorkerHours,
            out float requiredWorkerHours,
            out float reservedStone,
            out float requiredStone,
            out float progress01)
        {
            recoveryEligible = fortification.OpenBreachCount <= 0 &&
                               fortification.DestroyedWallSegmentCount +
                               fortification.DestroyedTowerCount +
                               fortification.DestroyedGateCount +
                               fortification.DestroyedKeepCount > 0;
            recoveryTracked = entityManager.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlementEntity);
            targetCounter = DestroyedCounterKind.None;
            accumulatedWorkerHours = 0f;
            requiredWorkerHours = 0f;
            reservedStone = 0f;
            requiredStone = 0f;
            progress01 = 0f;

            if (!recoveryTracked)
            {
                return;
            }

            var progress = entityManager.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementEntity);
            targetCounter = progress.TargetCounter;
            accumulatedWorkerHours = progress.AccumulatedWorkerHours;
            reservedStone = progress.StoneReservedForCurrentSegment;
            if (targetCounter == DestroyedCounterKind.None)
            {
                return;
            }

            float multiplier = targetCounter == DestroyedCounterKind.Keep
                ? FortificationCanon.DestroyedCounterRecoveryKeepMultiplier
                : 1f;
            requiredWorkerHours = FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment * multiplier;
            requiredStone = FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment * multiplier;
            if (requiredWorkerHours > 0f)
            {
                progress01 = math.clamp(accumulatedWorkerHours / requiredWorkerHours, 0f, 1f);
            }
        }
    }
}
