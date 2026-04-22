using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects live settlement fortification, reserve, sealing, and recovery state into
    /// a stable HUD read-model for the player-facing fortification block.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct FortificationHUDSystem : ISystem
    {
        private EntityQuery settlementQuery;
        private EntityQuery combatantQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            settlementQuery = SystemAPI.QueryBuilder()
                .WithAll<SettlementComponent, FortificationComponent, FactionComponent>()
                .Build();
            combatantQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    FortificationCombatantTag,
                    FortificationSettlementLinkComponent,
                    PositionComponent,
                    HealthComponent,
                    FactionComponent>()
                .Build();

            state.RequireForUpdate(settlementQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (settlement, faction, entity) in SystemAPI.Query<
                         RefRO<SettlementComponent>,
                         RefRO<FactionComponent>>()
                     .WithAll<FortificationComponent>()
                     .WithNone<FortificationHUDComponent>()
                     .WithEntityAccess())
            {
                ecb.AddComponent(entity, new FortificationHUDComponent
                {
                    SettlementId = settlement.ValueRO.SettlementId,
                    SettlementClassId = settlement.ValueRO.SettlementClassId,
                    OwnerFactionId = faction.ValueRO.FactionId,
                });
            }

            ecb.Playback(entityManager);

            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlements = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            using var fortifications = settlementQuery.ToComponentDataArray<FortificationComponent>(Allocator.Temp);
            using var factions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var combatantLinks = combatantQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var combatantPositions = combatantQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var combatantHealths = combatantQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var combatantFactions = combatantQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                var settlementEntity = settlementEntities[i];
                var settlement = settlements[i];
                var fortification = fortifications[i];
                var faction = factions[i];
                var reserve = entityManager.HasComponent<FortificationReserveComponent>(settlementEntity)
                    ? entityManager.GetComponentData<FortificationReserveComponent>(settlementEntity)
                    : default;

                ResolveDefenderCounts(
                    entityManager,
                    settlementEntity,
                    faction.FactionId,
                    fortification.ReserveRadiusTiles,
                    fortification.Tier,
                    combatantLinks,
                    combatantPositions,
                    combatantHealths,
                    combatantFactions,
                    out int reserveFrontage,
                    out int musteredDefenders);
                ResolveSealingTelemetry(
                    entityManager,
                    settlementEntity,
                    fortification,
                    out bool sealingEligible,
                    out bool sealingTracked,
                    out float sealingProgress01,
                    out float sealingAccumulatedWorkerHours,
                    out float sealingRequiredWorkerHours,
                    out float sealingReservedStone,
                    out float sealingRequiredStone);
                ResolveRecoveryTelemetry(
                    entityManager,
                    settlementEntity,
                    fortification,
                    out bool recoveryEligible,
                    out bool recoveryTracked,
                    out DestroyedCounterKind recoveryTargetCounter,
                    out float recoveryProgress01,
                    out float recoveryAccumulatedWorkerHours,
                    out float recoveryRequiredWorkerHours,
                    out float recoveryReservedStone,
                    out float recoveryRequiredStone);

                entityManager.SetComponentData(settlementEntity, new FortificationHUDComponent
                {
                    SettlementId = settlement.SettlementId,
                    SettlementClassId = settlement.SettlementClassId,
                    OwnerFactionId = faction.FactionId,
                    IsPrimaryKeep = entityManager.HasComponent<PrimaryKeepTag>(settlementEntity),
                    Tier = fortification.Tier,
                    Ceiling = fortification.Ceiling,
                    OpenBreachCount = fortification.OpenBreachCount,
                    DestroyedWallSegmentCount = fortification.DestroyedWallSegmentCount,
                    DestroyedTowerCount = fortification.DestroyedTowerCount,
                    DestroyedGateCount = fortification.DestroyedGateCount,
                    DestroyedKeepCount = fortification.DestroyedKeepCount,
                    ReserveFrontage = reserveFrontage,
                    MusteredDefenders = musteredDefenders,
                    ReadyReserveCount = reserve.ReadyReserveCount,
                    MusteringReserveCount = reserve.MusteringReserveCount,
                    RecoveringReserveCount = reserve.RecoveringReserveCount,
                    FallbackReserveCount = reserve.FallbackReserveCount,
                    LastCommittedCount = reserve.LastCommittedCount,
                    ThreatActive = reserve.ThreatActive,
                    SealingEligible = sealingEligible,
                    SealingTracked = sealingTracked,
                    SealingProgress01 = sealingProgress01,
                    SealingAccumulatedWorkerHours = sealingAccumulatedWorkerHours,
                    SealingRequiredWorkerHours = sealingRequiredWorkerHours,
                    SealingReservedStone = sealingReservedStone,
                    SealingRequiredStone = sealingRequiredStone,
                    RecoveryEligible = recoveryEligible,
                    RecoveryTracked = recoveryTracked,
                    RecoveryTargetCounter = recoveryTargetCounter,
                    RecoveryProgress01 = recoveryProgress01,
                    RecoveryAccumulatedWorkerHours = recoveryAccumulatedWorkerHours,
                    RecoveryRequiredWorkerHours = recoveryRequiredWorkerHours,
                    RecoveryReservedStone = recoveryReservedStone,
                    RecoveryRequiredStone = recoveryRequiredStone,
                });
            }
        }

        private static void ResolveDefenderCounts(
            EntityManager entityManager,
            Entity settlementEntity,
            FixedString32Bytes ownerFactionId,
            float reserveRadiusTiles,
            int fortificationTier,
            NativeArray<FortificationSettlementLinkComponent> combatantLinks,
            NativeArray<PositionComponent> combatantPositions,
            NativeArray<HealthComponent> combatantHealths,
            NativeArray<FactionComponent> combatantFactions,
            out int reserveFrontage,
            out int musteredDefenders)
        {
            musteredDefenders = 0;
            int desiredFrontline = math.max(1, 1 + fortificationTier);
            bool hasSettlementPosition = entityManager.HasComponent<PositionComponent>(settlementEntity);
            float3 settlementCenter = hasSettlementPosition
                ? entityManager.GetComponentData<PositionComponent>(settlementEntity).Value
                : float3.zero;

            for (int i = 0; i < combatantLinks.Length; i++)
            {
                if (combatantLinks[i].SettlementEntity != settlementEntity ||
                    !combatantFactions[i].FactionId.Equals(ownerFactionId) ||
                    combatantHealths[i].Current <= 0f)
                {
                    continue;
                }

                if (hasSettlementPosition &&
                    math.distance(combatantPositions[i].Value, settlementCenter) > reserveRadiusTiles)
                {
                    continue;
                }

                musteredDefenders++;
            }

            reserveFrontage = math.min(musteredDefenders, desiredFrontline);
        }

        private static void ResolveSealingTelemetry(
            EntityManager entityManager,
            Entity settlementEntity,
            in FortificationComponent fortification,
            out bool sealingEligible,
            out bool sealingTracked,
            out float progress01,
            out float accumulatedWorkerHours,
            out float requiredWorkerHours,
            out float reservedStone,
            out float requiredStone)
        {
            sealingEligible = fortification.OpenBreachCount > 0;
            sealingTracked = entityManager.HasComponent<BreachSealingProgressComponent>(settlementEntity);
            requiredWorkerHours = sealingEligible
                ? FortificationCanon.ResolveBreachSealingWorkerHoursPerBreach(fortification.Tier)
                : 0f;
            requiredStone = sealingEligible
                ? FortificationCanon.ResolveBreachSealingStoneCostPerBreach(fortification.Tier)
                : 0f;
            accumulatedWorkerHours = 0f;
            reservedStone = 0f;
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
            out DestroyedCounterKind recoveryTargetCounter,
            out float progress01,
            out float accumulatedWorkerHours,
            out float requiredWorkerHours,
            out float reservedStone,
            out float requiredStone)
        {
            recoveryEligible = fortification.OpenBreachCount <= 0 &&
                               fortification.DestroyedWallSegmentCount +
                               fortification.DestroyedTowerCount +
                               fortification.DestroyedGateCount +
                               fortification.DestroyedKeepCount > 0;
            recoveryTracked = entityManager.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlementEntity);
            recoveryTargetCounter = DestroyedCounterKind.None;
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
            recoveryTargetCounter = progress.TargetCounter;
            accumulatedWorkerHours = progress.AccumulatedWorkerHours;
            reservedStone = progress.StoneReservedForCurrentSegment;
            if (recoveryTargetCounter == DestroyedCounterKind.None)
            {
                return;
            }

            float keepMultiplier = recoveryTargetCounter == DestroyedCounterKind.Keep
                ? FortificationCanon.DestroyedCounterRecoveryKeepMultiplier
                : 1f;
            requiredWorkerHours = FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment * keepMultiplier;
            requiredStone = FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment * keepMultiplier;
            if (requiredWorkerHours > 0f)
            {
                progress01 = math.clamp(accumulatedWorkerHours / requiredWorkerHours, 0f, 1f);
            }
        }
    }
}
