using System.Collections.Generic;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Restores destroyed fortification structures after open breaches have been
    /// sealed. Recovery is intentionally slower and more expensive than sealing.
    /// Each completed rebuild restores one linked destroyed structure so the
    /// existing destruction accounting remains the authoritative source.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BreachSealingSystem))]
    public partial struct DestroyedCounterRecoverySystem : ISystem
    {
        private const float WorkerHourCompletionEpsilon = 0.001f;

        private float tickAccumulatorSeconds;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float tickIntervalSeconds = 1f / FortificationCanon.DestroyedCounterRecoveryTickRateHz;
            tickAccumulatorSeconds += SystemAPI.Time.DeltaTime;
            int processedTickCount = (int)math.floor((tickAccumulatorSeconds + 0.0001f) / tickIntervalSeconds);
            if (processedTickCount <= 0)
            {
                return;
            }

            float processedSeconds = processedTickCount * tickIntervalSeconds;
            tickAccumulatorSeconds = math.max(0f, tickAccumulatorSeconds - processedSeconds);

            var entityManager = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            float currentInWorldDays = clock.InWorldDays;
            float fallbackLastTickInWorldDays = math.max(
                0f,
                currentInWorldDays - (processedSeconds * math.max(0f, clock.DaysPerRealSecond)));

            var stockpiles = BuildFactionStockpileRecords(entityManager);
            var idleWorkers = BuildFactionIdleWorkerRecords(entityManager);
            var fortificationStructures = BuildFortificationStructureRecords(entityManager);

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var fortifications = settlementQuery.ToComponentDataArray<FortificationComponent>(Allocator.Temp);
            using var factions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            settlementQuery.Dispose();

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                var settlementEntity = settlementEntities[i];
                var fortification = fortifications[i];
                int totalDestroyedCount = GetTotalDestroyedCount(fortification);
                bool canRecover = fortification.OpenBreachCount <= 0 && totalDestroyedCount > 0;
                bool hasProgress = entityManager.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlementEntity);
                var progress = hasProgress
                    ? entityManager.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementEntity)
                    : new DestroyedCounterRecoveryProgressComponent
                    {
                        AccumulatedWorkerHours = 0f,
                        StoneReservedForCurrentSegment = 0f,
                        LastTickInWorldDays = fallbackLastTickInWorldDays,
                        TargetCounter = DestroyedCounterKind.None,
                    };

                if (!canRecover)
                {
                    if (hasProgress)
                    {
                        progress.LastTickInWorldDays = currentInWorldDays;
                        if (totalDestroyedCount <= 0)
                        {
                            progress.AccumulatedWorkerHours = 0f;
                            progress.StoneReservedForCurrentSegment = 0f;
                            progress.TargetCounter = DestroyedCounterKind.None;
                        }

                        entityManager.SetComponentData(settlementEntity, progress);
                    }

                    continue;
                }

                float elapsedInWorldDays = math.max(0f, currentInWorldDays - progress.LastTickInWorldDays);
                progress.LastTickInWorldDays = currentInWorldDays;

                if (!IsTargetValid(fortification, progress.TargetCounter))
                {
                    progress.AccumulatedWorkerHours = 0f;
                    progress.StoneReservedForCurrentSegment = 0f;
                    progress.TargetCounter = ResolveHighestPriorityTarget(fortification);
                }
                else if (progress.TargetCounter == DestroyedCounterKind.None)
                {
                    progress.TargetCounter = ResolveHighestPriorityTarget(fortification);
                }

                if (elapsedInWorldDays > 0f && progress.TargetCounter != DestroyedCounterKind.None)
                {
                    int laborIndex = FindIdleWorkerIndex(idleWorkers, factions[i].FactionId);
                    if (laborIndex >= 0 && idleWorkers[laborIndex].AvailableIdleWorkers > 0)
                    {
                        int stockpileIndex = FindStockpileIndex(stockpiles, factions[i].FactionId);
                        var requirements = ResolveTargetRequirements(progress.TargetCounter);
                        bool canFundCurrentSegment =
                            progress.StoneReservedForCurrentSegment >= requirements.StoneCost;
                        if (!canFundCurrentSegment &&
                            stockpileIndex >= 0 &&
                            TryReserveStoneForCurrentSegment(
                                ref stockpiles,
                                stockpileIndex,
                                ref progress,
                                requirements.StoneCost))
                        {
                            canFundCurrentSegment = true;
                        }

                        if (canFundCurrentSegment)
                        {
                            var labor = idleWorkers[laborIndex];
                            labor.AvailableIdleWorkers--;
                            idleWorkers[laborIndex] = labor;

                            AccumulateRecoveryProgress(
                                entityManager,
                                ref stockpiles,
                                ref fortificationStructures,
                                stockpileIndex,
                                settlementEntity,
                                ref fortification,
                                ref progress,
                                elapsedInWorldDays,
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

                if (!fortification.Equals(fortifications[i]))
                {
                    entityManager.SetComponentData(settlementEntity, fortification);
                }
            }

            for (int i = 0; i < stockpiles.Count; i++)
            {
                entityManager.SetComponentData(stockpiles[i].Entity, stockpiles[i].Stockpile);
            }
        }

        private static void AccumulateRecoveryProgress(
            EntityManager entityManager,
            ref List<FactionStockpileRecord> stockpiles,
            ref List<FortificationStructureRecord> fortificationStructures,
            int stockpileIndex,
            Entity settlementEntity,
            ref FortificationComponent fortification,
            ref DestroyedCounterRecoveryProgressComponent progress,
            float elapsedInWorldDays,
            FixedString32Bytes factionId,
            FixedString64Bytes settlementId)
        {
            float remainingWorkerHours = math.max(0f, elapsedInWorldDays * 24f);
            while (remainingWorkerHours > 0f && GetTotalDestroyedCount(fortification) > 0)
            {
                if (remainingWorkerHours <= 0.0001f)
                {
                    break;
                }

                if (!IsTargetValid(fortification, progress.TargetCounter))
                {
                    progress.AccumulatedWorkerHours = 0f;
                    progress.StoneReservedForCurrentSegment = 0f;
                    progress.TargetCounter = ResolveHighestPriorityTarget(fortification);
                    if (progress.TargetCounter == DestroyedCounterKind.None)
                    {
                        break;
                    }
                }

                var requirements = ResolveTargetRequirements(progress.TargetCounter);
                if (progress.StoneReservedForCurrentSegment < requirements.StoneCost)
                {
                    if (stockpileIndex < 0 ||
                        !TryReserveStoneForCurrentSegment(
                            ref stockpiles,
                            stockpileIndex,
                            ref progress,
                            requirements.StoneCost))
                    {
                        break;
                    }
                }

                float neededWorkerHours = math.max(0f, requirements.WorkerHours - progress.AccumulatedWorkerHours);
                if (neededWorkerHours > WorkerHourCompletionEpsilon)
                {
                    float appliedWorkerHours = math.min(remainingWorkerHours, neededWorkerHours);
                    progress.AccumulatedWorkerHours += appliedWorkerHours;
                    remainingWorkerHours -= appliedWorkerHours;
                }

                if (requirements.WorkerHours - progress.AccumulatedWorkerHours <= WorkerHourCompletionEpsilon)
                {
                    progress.AccumulatedWorkerHours = requirements.WorkerHours;
                }

                if (remainingWorkerHours <= 0.0001f)
                {
                    remainingWorkerHours = 0f;
                }

                if (progress.AccumulatedWorkerHours + WorkerHourCompletionEpsilon < requirements.WorkerHours)
                {
                    break;
                }

                if (!TryCompleteCurrentRebuild(
                        entityManager,
                        ref fortificationStructures,
                        settlementEntity,
                        ref fortification,
                        progress.TargetCounter))
                {
                    progress.AccumulatedWorkerHours = 0f;
                    progress.StoneReservedForCurrentSegment = 0f;
                    progress.TargetCounter = DestroyedCounterKind.None;
                    break;
                }

                PushRebuildMessage(entityManager, factionId, settlementId, progress.TargetCounter);
                progress.AccumulatedWorkerHours = 0f;
                progress.StoneReservedForCurrentSegment = 0f;
                progress.TargetCounter = ResolveHighestPriorityTarget(fortification);
            }

            if (GetTotalDestroyedCount(fortification) <= 0)
            {
                progress.TargetCounter = DestroyedCounterKind.None;
            }
        }

        private static void PushRebuildMessage(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString64Bytes settlementId,
            DestroyedCounterKind targetCounter)
        {
            var message = new FixedString128Bytes();
            message.Append(factionId);
            message.Append((FixedString32Bytes)" rebuilds a ");
            message.Append(ResolveRebuildLabel(targetCounter));
            message.Append((FixedString32Bytes)" at ");
            message.Append(settlementId);
            message.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(entityManager, message, NarrativeMessageTone.Info);
        }

        private static bool TryCompleteCurrentRebuild(
            EntityManager entityManager,
            ref List<FortificationStructureRecord> fortificationStructures,
            Entity settlementEntity,
            ref FortificationComponent fortification,
            DestroyedCounterKind targetCounter)
        {
            int structureIndex = FindDestroyedStructureIndex(
                fortificationStructures,
                settlementEntity,
                ResolveFortificationRole(targetCounter));
            if (structureIndex < 0)
            {
                return false;
            }

            switch (targetCounter)
            {
                case DestroyedCounterKind.Wall:
                    fortification.DestroyedWallSegmentCount = math.max(0, fortification.DestroyedWallSegmentCount - 1);
                    break;
                case DestroyedCounterKind.Tower:
                    fortification.DestroyedTowerCount = math.max(0, fortification.DestroyedTowerCount - 1);
                    break;
                case DestroyedCounterKind.Gate:
                    fortification.DestroyedGateCount = math.max(0, fortification.DestroyedGateCount - 1);
                    break;
                case DestroyedCounterKind.Keep:
                    fortification.DestroyedKeepCount = math.max(0, fortification.DestroyedKeepCount - 1);
                    break;
                default:
                    return false;
            }

            var structure = fortificationStructures[structureIndex];
            structure.Health.Current = structure.Health.Max;
            entityManager.SetComponentData(structure.Entity, structure.Health);
            fortificationStructures[structureIndex] = structure;
            return true;
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

        private static List<FactionIdleWorkerRecord> BuildFactionIdleWorkerRecords(EntityManager entityManager)
        {
            var idleWorkerRecords = new List<FactionIdleWorkerRecord>(4);
            var workerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<WorkerGatherComponent>());

            using var workerFactions = workerQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
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

                int recordIndex = FindIdleWorkerIndex(idleWorkerRecords, workerFactions[i].FactionId);
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
                        AvailableIdleWorkers = 1,
                    });
                }
            }

            return idleWorkerRecords;
        }

        private static List<FortificationStructureRecord> BuildFortificationStructureRecords(EntityManager entityManager)
        {
            var structures = new List<FortificationStructureRecord>(16);
            var structureQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = structureQuery.ToEntityArray(Allocator.Temp);
            using var buildingTypes = structureQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var links = structureQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var healths = structureQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            structureQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (buildingTypes[i].FortificationRole == FortificationRole.None)
                {
                    continue;
                }

                structures.Add(new FortificationStructureRecord
                {
                    Entity = entities[i],
                    SettlementEntity = links[i].SettlementEntity,
                    Role = buildingTypes[i].FortificationRole,
                    Health = healths[i],
                });
            }

            return structures;
        }

        private static DestroyedCounterKind ResolveHighestPriorityTarget(FortificationComponent fortification)
        {
            if (fortification.DestroyedKeepCount > 0)
            {
                return DestroyedCounterKind.Keep;
            }

            if (fortification.DestroyedGateCount > 0)
            {
                return DestroyedCounterKind.Gate;
            }

            if (fortification.DestroyedWallSegmentCount > 0)
            {
                return DestroyedCounterKind.Wall;
            }

            if (fortification.DestroyedTowerCount > 0)
            {
                return DestroyedCounterKind.Tower;
            }

            return DestroyedCounterKind.None;
        }

        private static bool IsTargetValid(FortificationComponent fortification, DestroyedCounterKind targetCounter)
        {
            return targetCounter switch
            {
                DestroyedCounterKind.Wall => fortification.DestroyedWallSegmentCount > 0,
                DestroyedCounterKind.Tower => fortification.DestroyedTowerCount > 0,
                DestroyedCounterKind.Gate => fortification.DestroyedGateCount > 0,
                DestroyedCounterKind.Keep => fortification.DestroyedKeepCount > 0,
                _ => false,
            };
        }

        private static TargetRequirements ResolveTargetRequirements(DestroyedCounterKind targetCounter)
        {
            float multiplier = targetCounter == DestroyedCounterKind.Keep
                ? FortificationCanon.DestroyedCounterRecoveryKeepMultiplier
                : 1f;
            return new TargetRequirements
            {
                StoneCost = FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment * multiplier,
                WorkerHours = FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment * multiplier,
            };
        }

        private static int GetTotalDestroyedCount(FortificationComponent fortification)
        {
            return fortification.DestroyedWallSegmentCount +
                   fortification.DestroyedTowerCount +
                   fortification.DestroyedGateCount +
                   fortification.DestroyedKeepCount;
        }

        private static bool TryReserveStoneForCurrentSegment(
            ref List<FactionStockpileRecord> stockpiles,
            int stockpileIndex,
            ref DestroyedCounterRecoveryProgressComponent progress,
            float requiredStoneForTarget)
        {
            if (stockpileIndex < 0)
            {
                return false;
            }

            var stockpileRecord = stockpiles[stockpileIndex];
            float requiredStone = requiredStoneForTarget - progress.StoneReservedForCurrentSegment;
            if (requiredStone <= 0f)
            {
                progress.StoneReservedForCurrentSegment = requiredStoneForTarget;
                return true;
            }

            if (stockpileRecord.Stockpile.Stone < requiredStone)
            {
                return false;
            }

            stockpileRecord.Stockpile.Stone -= requiredStone;
            stockpiles[stockpileIndex] = stockpileRecord;
            progress.StoneReservedForCurrentSegment = requiredStoneForTarget;
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
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < idleWorkers.Count; i++)
            {
                if (idleWorkers[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindDestroyedStructureIndex(
            List<FortificationStructureRecord> fortificationStructures,
            Entity settlementEntity,
            FortificationRole role)
        {
            for (int i = 0; i < fortificationStructures.Count; i++)
            {
                if (fortificationStructures[i].SettlementEntity == settlementEntity &&
                    fortificationStructures[i].Role == role &&
                    fortificationStructures[i].Health.Current <= 0f)
                {
                    return i;
                }
            }

            return -1;
        }

        private static FortificationRole ResolveFortificationRole(DestroyedCounterKind targetCounter)
        {
            return targetCounter switch
            {
                DestroyedCounterKind.Wall => FortificationRole.Wall,
                DestroyedCounterKind.Tower => FortificationRole.Tower,
                DestroyedCounterKind.Gate => FortificationRole.Gate,
                DestroyedCounterKind.Keep => FortificationRole.Keep,
                _ => FortificationRole.None,
            };
        }

        private static FixedString32Bytes ResolveRebuildLabel(DestroyedCounterKind targetCounter)
        {
            return targetCounter switch
            {
                DestroyedCounterKind.Wall => new FixedString32Bytes("wall"),
                DestroyedCounterKind.Tower => new FixedString32Bytes("tower"),
                DestroyedCounterKind.Gate => new FixedString32Bytes("gate"),
                DestroyedCounterKind.Keep => new FixedString32Bytes("keep"),
                _ => new FixedString32Bytes("fortification"),
            };
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
            public int AvailableIdleWorkers;
        }

        private struct FortificationStructureRecord
        {
            public Entity Entity;
            public Entity SettlementEntity;
            public FortificationRole Role;
            public HealthComponent Health;
        }

        private struct TargetRequirements
        {
            public float StoneCost;
            public float WorkerHours;
        }
    }
}
