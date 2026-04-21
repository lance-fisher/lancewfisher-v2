using System.Collections.Generic;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Siege;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Unity-native defender breach recovery loop.
    /// Settlements with open breaches can spend stone plus one idle worker's time
    /// to close breaches over the in-world timeline. Destroyed wall, gate, tower,
    /// and keep counters stay unchanged in this slice; only OpenBreachCount
    /// recovers.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DualClockDeclarationSystem))]
    [UpdateAfter(typeof(FortificationDestructionResolutionSystem))]
    [UpdateBefore(typeof(BreachAssaultPressureSystem))]
    public partial struct BreachSealingSystem : ISystem
    {
        private float tickAccumulatorSeconds;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float tickIntervalSeconds = 1f / FortificationCanon.BreachSealingTickRateHz;
            tickAccumulatorSeconds += SystemAPI.Time.DeltaTime;
            if (tickAccumulatorSeconds + 0.0001f < tickIntervalSeconds)
            {
                return;
            }

            float processedSeconds = 0f;
            while (tickAccumulatorSeconds >= tickIntervalSeconds)
            {
                tickAccumulatorSeconds -= tickIntervalSeconds;
                processedSeconds += tickIntervalSeconds;
            }

            if (processedSeconds <= 0f)
            {
                return;
            }

            var entityManager = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            float currentInWorldDays = clock.InWorldDays;
            float fallbackLastTickInWorldDays = math.max(
                0f,
                currentInWorldDays - (processedSeconds * math.max(0f, clock.DaysPerRealSecond)));

            var stockpiles = BuildFactionStockpileRecords(entityManager);
            var controlPoints = BuildControlPointRecords(entityManager);
            var idleWorkers = BuildFactionIdleWorkerRecords(entityManager, controlPoints);

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var fortifications = settlementQuery.ToComponentDataArray<FortificationComponent>(Allocator.Temp);
            using var factions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var positions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            settlementQuery.Dispose();

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                var settlementEntity = settlementEntities[i];
                var fortification = fortifications[i];
                bool hasProgress = entityManager.HasComponent<BreachSealingProgressComponent>(settlementEntity);
                var progress = hasProgress
                    ? entityManager.GetComponentData<BreachSealingProgressComponent>(settlementEntity)
                    : new BreachSealingProgressComponent
                    {
                        AccumulatedWorkerHours = 0f,
                        StoneReservedForCurrentBreach = 0f,
                        LastTickInWorldDays = fallbackLastTickInWorldDays,
                    };

                if (fortification.OpenBreachCount <= 0)
                {
                    if (hasProgress &&
                        (progress.AccumulatedWorkerHours > 0f ||
                         progress.StoneReservedForCurrentBreach > 0f))
                    {
                        progress.AccumulatedWorkerHours = 0f;
                        progress.StoneReservedForCurrentBreach = 0f;
                        progress.LastTickInWorldDays = currentInWorldDays;
                        entityManager.SetComponentData(settlementEntity, progress);
                    }

                    continue;
                }

                float elapsedInWorldDays = math.max(0f, currentInWorldDays - progress.LastTickInWorldDays);
                progress.LastTickInWorldDays = currentInWorldDays;
                float requiredStone = FortificationCanon.ResolveBreachSealingStoneCostPerBreach(fortification.Tier);
                float requiredWorkerHours = FortificationCanon.ResolveBreachSealingWorkerHoursPerBreach(fortification.Tier);

                if (elapsedInWorldDays > 0f)
                {
                    int settlementControlPointIndex = FindNearestOwnedControlPointIndex(
                        controlPoints,
                        factions[i].FactionId,
                        positions[i].Value);
                    int laborIndex = settlementControlPointIndex >= 0
                        ? FindIdleWorkerIndex(
                            idleWorkers,
                            factions[i].FactionId,
                            controlPoints[settlementControlPointIndex].Entity)
                        : -1;
                    if (laborIndex >= 0 && idleWorkers[laborIndex].AvailableIdleWorkers > 0)
                    {
                        int stockpileIndex = FindStockpileIndex(stockpiles, factions[i].FactionId);
                        bool canFundCurrentBreach =
                            progress.StoneReservedForCurrentBreach >= requiredStone;
                        if (!canFundCurrentBreach &&
                            stockpileIndex >= 0 &&
                            TryReserveStoneForCurrentBreach(
                                ref stockpiles,
                                stockpileIndex,
                                ref progress,
                                requiredStone))
                        {
                            canFundCurrentBreach = true;
                        }

                        if (canFundCurrentBreach)
                        {
                            var labor = idleWorkers[laborIndex];
                            labor.AvailableIdleWorkers--;
                            idleWorkers[laborIndex] = labor;

                            AccumulateSealingProgress(
                                entityManager,
                                ref stockpiles,
                                stockpileIndex,
                                ref fortification,
                                ref progress,
                                elapsedInWorldDays,
                                requiredStone,
                                requiredWorkerHours,
                                factions[i].FactionId,
                                fortification.SettlementId);
                        }
                    }
                }

                if (hasProgress)
                {
                    entityManager.SetComponentData(settlementEntity, progress);
                }
                else
                {
                    entityManager.AddComponentData(settlementEntity, progress);
                }

                if (fortification.OpenBreachCount != fortifications[i].OpenBreachCount)
                {
                    entityManager.SetComponentData(settlementEntity, fortification);
                }
            }

            for (int i = 0; i < stockpiles.Count; i++)
            {
                entityManager.SetComponentData(stockpiles[i].Entity, stockpiles[i].Stockpile);
            }
        }

        private static List<FactionStockpileRecord> BuildFactionStockpileRecords(EntityManager entityManager)
        {
            var stockpiles = new List<FactionStockpileRecord>(4);
            var stockpileQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceStockpileComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var entities = stockpileQuery.ToEntityArray(Allocator.Temp);
            using var stockpileComponents = stockpileQuery.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            using var factions = stockpileQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            stockpileQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                stockpiles.Add(new FactionStockpileRecord
                {
                    Entity = entities[i],
                    FactionId = factions[i].FactionId,
                    Stockpile = stockpileComponents[i],
                });
            }

            return stockpiles;
        }

        private static List<ControlPointRecord> BuildControlPointRecords(EntityManager entityManager)
        {
            var controlPoints = new List<ControlPointRecord>(8);
            var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var entities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var controlPointComponents = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var positions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            controlPointQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                controlPoints.Add(new ControlPointRecord
                {
                    Entity = entities[i],
                    OwnerFactionId = controlPointComponents[i].OwnerFactionId,
                    Position = positions[i].Value,
                });
            }

            return controlPoints;
        }

        private static List<FactionIdleWorkerRecord> BuildFactionIdleWorkerRecords(
            EntityManager entityManager,
            List<ControlPointRecord> controlPoints)
        {
            var idleWorkerRecords = new List<FactionIdleWorkerRecord>(4);
            var workerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<WorkerGatherComponent>());

            using var workerFactions = workerQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var workerPositions = workerQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var workerTypes = workerQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var workerHealth = workerQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var workerGathers = workerQuery.ToComponentDataArray<WorkerGatherComponent>(Allocator.Temp);
            workerQuery.Dispose();

            for (int i = 0; i < workerFactions.Length; i++)
            {
                if (workerTypes[i].Role != UnitRole.Worker ||
                    workerHealth[i].Current <= 0f ||
                    workerGathers[i].Phase != WorkerGatherPhase.Idle)
                {
                    continue;
                }

                int nearestControlPointIndex = FindNearestControlPointIndex(controlPoints, workerPositions[i].Value);
                if (nearestControlPointIndex < 0)
                {
                    continue;
                }

                var nearestControlPoint = controlPoints[nearestControlPointIndex];
                if (!nearestControlPoint.OwnerFactionId.Equals(workerFactions[i].FactionId))
                {
                    continue;
                }

                int recordIndex = FindIdleWorkerIndex(
                    idleWorkerRecords,
                    workerFactions[i].FactionId,
                    nearestControlPoint.Entity);
                if (recordIndex >= 0)
                {
                    var record = idleWorkerRecords[recordIndex];
                    record.AvailableIdleWorkers++;
                    idleWorkerRecords[recordIndex] = record;
                }
                else
                {
                    idleWorkerRecords.Add(new FactionIdleWorkerRecord
                    {
                        FactionId = workerFactions[i].FactionId,
                        ControlPointEntity = nearestControlPoint.Entity,
                        AvailableIdleWorkers = 1,
                    });
                }
            }

            return idleWorkerRecords;
        }

        private static void AccumulateSealingProgress(
            EntityManager entityManager,
            ref List<FactionStockpileRecord> stockpiles,
            int stockpileIndex,
            ref FortificationComponent fortification,
            ref BreachSealingProgressComponent progress,
            float elapsedInWorldDays,
            float requiredStone,
            float requiredWorkerHours,
            FixedString32Bytes factionId,
            FixedString64Bytes settlementId)
        {
            float remainingWorkerHours = math.max(0f, elapsedInWorldDays * 24f);
            while (remainingWorkerHours > 0f && fortification.OpenBreachCount > 0)
            {
                if (progress.StoneReservedForCurrentBreach < requiredStone)
                {
                    if (stockpileIndex < 0 ||
                        !TryReserveStoneForCurrentBreach(
                            ref stockpiles,
                            stockpileIndex,
                            ref progress,
                            requiredStone))
                    {
                        break;
                    }
                }

                float neededWorkerHours = requiredWorkerHours - progress.AccumulatedWorkerHours;
                float appliedWorkerHours = math.min(remainingWorkerHours, neededWorkerHours);
                progress.AccumulatedWorkerHours += appliedWorkerHours;
                remainingWorkerHours -= appliedWorkerHours;

                if (progress.AccumulatedWorkerHours + 0.0001f < requiredWorkerHours)
                {
                    break;
                }

                fortification.OpenBreachCount = math.max(0, fortification.OpenBreachCount - 1);
                progress.AccumulatedWorkerHours = 0f;
                progress.StoneReservedForCurrentBreach = 0f;
                PushBreachClosedMessage(entityManager, factionId, settlementId);
            }
        }

        private static void PushBreachClosedMessage(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString64Bytes settlementId)
        {
            var message = new FixedString128Bytes();
            message.Append(factionId);
            message.Append((FixedString64Bytes)"'s masons seal a breach at ");
            message.Append(settlementId);
            message.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(entityManager, message, NarrativeMessageTone.Info);
        }

        private static bool TryReserveStoneForCurrentBreach(
            ref List<FactionStockpileRecord> stockpiles,
            int stockpileIndex,
            ref BreachSealingProgressComponent progress,
            float requiredStone)
        {
            if (stockpileIndex < 0)
            {
                return false;
            }

            var stockpileRecord = stockpiles[stockpileIndex];
            float remainingStoneToReserve = requiredStone - progress.StoneReservedForCurrentBreach;
            if (remainingStoneToReserve <= 0f)
            {
                progress.StoneReservedForCurrentBreach = requiredStone;
                return true;
            }

            if (stockpileRecord.Stockpile.Stone < remainingStoneToReserve)
            {
                return false;
            }

            stockpileRecord.Stockpile.Stone -= remainingStoneToReserve;
            stockpiles[stockpileIndex] = stockpileRecord;
            progress.StoneReservedForCurrentBreach = requiredStone;
            return true;
        }

        private static int FindStockpileIndex(
            List<FactionStockpileRecord> stockpiles,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < stockpiles.Count; i++)
            {
                if (stockpiles[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindIdleWorkerIndex(
            List<FactionIdleWorkerRecord> idleWorkers,
            FixedString32Bytes factionId,
            Entity controlPointEntity)
        {
            for (int i = 0; i < idleWorkers.Count; i++)
            {
                if (idleWorkers[i].FactionId.Equals(factionId) &&
                    idleWorkers[i].ControlPointEntity == controlPointEntity)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindNearestOwnedControlPointIndex(
            List<ControlPointRecord> controlPoints,
            FixedString32Bytes factionId,
            float3 position)
        {
            float bestDistanceSq = float.MaxValue;
            int bestIndex = -1;

            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(position, controlPoints[i].Position);
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static int FindNearestControlPointIndex(
            List<ControlPointRecord> controlPoints,
            float3 position)
        {
            float bestDistanceSq = float.MaxValue;
            int bestIndex = -1;

            for (int i = 0; i < controlPoints.Count; i++)
            {
                float distanceSq = math.distancesq(position, controlPoints[i].Position);
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private struct ControlPointRecord
        {
            public Entity Entity;
            public FixedString32Bytes OwnerFactionId;
            public float3 Position;
        }

        private struct FactionStockpileRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public ResourceStockpileComponent Stockpile;
        }

        private struct FactionIdleWorkerRecord
        {
            public FixedString32Bytes FactionId;
            public Entity ControlPointEntity;
            public int AvailableIdleWorkers;
        }
    }
}
