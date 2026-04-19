using System;
using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// 1.25 second cadence siege logistics refresh. Engineers refresh short-lived
    /// repair / support windows, while supply wagons extend siege-engine supply
    /// continuity only when they remain linked to a live allied supply camp.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SiegeComponentInitializationSystem))]
    [UpdateBefore(typeof(FieldWaterSupportScanSystem))]
    public partial struct SiegeSupportRefreshSystem : ISystem
    {
        private float refreshAccumulator;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SiegeSupportComponent>();
            state.RequireForUpdate<SiegeSupplyCampComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            refreshAccumulator += SystemAPI.Time.DeltaTime;
            if (refreshAccumulator + 0.0001f < SiegeSupportCanon.SiegeSupportRefreshSeconds)
            {
                return;
            }

            while (refreshAccumulator >= SiegeSupportCanon.SiegeSupportRefreshSeconds)
            {
                refreshAccumulator -= SiegeSupportCanon.SiegeSupportRefreshSeconds;
            }

            var entityManager = state.EntityManager;
            double elapsed = SystemAPI.Time.ElapsedTime;
            float refreshWindow = SiegeSupportCanon.SiegeSupportRefreshSeconds;

            var engines = new List<SiegeUnitRecord>(16);
            var engineers = new List<SiegeUnitRecord>(8);
            var wagons = new List<SiegeUnitRecord>(8);
            var supplyCamps = new List<SupplyCampRecord>(8);
            var stockpiles = new List<FactionStockpileRecord>(4);

            foreach (var (support, faction, position, health, entity) in
                SystemAPI.Query<
                    RefRO<SiegeSupportComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                var record = new SiegeUnitRecord
                {
                    Entity = entity,
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    Health = health.ValueRO,
                    Support = support.ValueRO,
                };

                if (record.Support.IsSiegeEngine)
                {
                    engines.Add(record);
                }
                else if (record.Support.IsEngineer)
                {
                    engineers.Add(record);
                }
                else if (record.Support.IsSupplyWagon)
                {
                    wagons.Add(record);
                }
            }

            foreach (var (buildingType, faction, position, health, supportCamp, entity) in
                SystemAPI.Query<
                    RefRO<BuildingTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>,
                    RefRO<SiegeSupplyCampComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current <= 0f || !buildingType.ValueRO.SupportsSiegeLogistics)
                {
                    continue;
                }

                supplyCamps.Add(new SupplyCampRecord
                {
                    Entity = entity,
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    IsOperational = SiegeSupplyInterdictionCanon.IsCampOperational(supportCamp.ValueRO),
                });
            }

            foreach (var (stockpile, faction, entity) in
                SystemAPI.Query<RefRO<ResourceStockpileComponent>, RefRO<FactionComponent>>()
                .WithEntityAccess())
            {
                stockpiles.Add(new FactionStockpileRecord
                {
                    Entity = entity,
                    FactionId = faction.ValueRO.FactionId,
                    Stockpile = stockpile.ValueRO,
                });
            }

            for (int i = 0; i < engineers.Count; i++)
            {
                var engineer = engineers[i];
                var support = engineer.Support;
                support.LastRefreshAt = elapsed;
                support.Status = SiegeSupportStatus.Idle;

                bool supportingAnyEngine = false;
                bool repairedAnyEngine = false;
                for (int engineIndex = 0; engineIndex < engines.Count; engineIndex++)
                {
                    if (!engines[engineIndex].FactionId.Equals(engineer.FactionId) ||
                        Distance2D(engineer.Position, engines[engineIndex].Position) > SiegeSupportCanon.EngineerSupportRadius)
                    {
                        continue;
                    }

                    supportingAnyEngine = true;
                    var engine = engines[engineIndex];
                    engine.Support.EngineerSupportUntil = Math.Max(
                        engine.Support.EngineerSupportUntil,
                        elapsed + SiegeSupportCanon.SiegeSupportRefreshSeconds);
                    engine.Support.LastRefreshAt = elapsed;
                    engine.Support.RefreshCount++;

                    if (engine.Health.Current < engine.Health.Max)
                    {
                        engine.Health.Current = math.min(
                            engine.Health.Max,
                            engine.Health.Current + SiegeSupportCanon.EngineerRepairPerSecond * refreshWindow);
                        repairedAnyEngine = true;
                    }

                    engines[engineIndex] = engine;
                }

                if (supportingAnyEngine)
                {
                    support.RefreshCount++;
                    support.Status = repairedAnyEngine
                        ? SiegeSupportStatus.Repair
                        : SiegeSupportStatus.Supporting;
                }

                engineer.Support = support;
                engineers[i] = engineer;
            }

            for (int i = 0; i < wagons.Count; i++)
            {
                var wagon = wagons[i];
                var support = wagon.Support;
                support.LastRefreshAt = elapsed;
                support.Status = SiegeSupportStatus.Idle;
                support.HasSupplyTrainSupport = false;

                var supplyTrain = entityManager.GetComponentData<SiegeSupplyTrainComponent>(wagon.Entity);
                bool interdicted = supplyTrain.LogisticsInterdictedUntil > elapsed;
                bool recovering = !interdicted && supplyTrain.ConvoyRecoveryUntil > elapsed;
                int linkedCampIndex = FindNearestSupplyCampIndex(supplyCamps, wagon.FactionId, wagon.Position);
                if (linkedCampIndex < 0)
                {
                    support.HasLinkedSupplyCamp = false;
                    support.Status = SiegeSupportStatus.CutOff;
                    supplyTrain.LinkedCampEntity = Entity.Null;
                    wagon.Support = support;
                    entityManager.SetComponentData(wagon.Entity, wagon.Support);
                    entityManager.SetComponentData(wagon.Entity, supplyTrain);
                    continue;
                }

                support.HasLinkedSupplyCamp = true;
                supplyTrain.LinkedCampEntity = supplyCamps[linkedCampIndex].Entity;

                if (interdicted)
                {
                    support.HasLinkedSupplyCamp = false;
                    support.Status = SiegeSupportStatus.Interdicted;
                    supplyTrain.LinkedCampEntity = Entity.Null;
                    wagon.Support = support;
                    entityManager.SetComponentData(wagon.Entity, wagon.Support);
                    entityManager.SetComponentData(wagon.Entity, supplyTrain);
                    continue;
                }

                int nearbyEngineCount = 0;
                var nearbyEngineIndices = new List<int>(4);
                for (int engineIndex = 0; engineIndex < engines.Count; engineIndex++)
                {
                    if (!engines[engineIndex].FactionId.Equals(wagon.FactionId) ||
                        Distance2D(wagon.Position, engines[engineIndex].Position) > SiegeSupportCanon.SupplyWagonRadius)
                    {
                        continue;
                    }

                    nearbyEngineIndices.Add(engineIndex);
                    nearbyEngineCount++;
                }

                if (recovering)
                {
                    support.Status = supplyTrain.EscortScreened
                        ? SiegeSupportStatus.RecoveringScreened
                        : SiegeSupportStatus.RecoveringUnscreened;
                    wagon.Support = support;
                    entityManager.SetComponentData(wagon.Entity, wagon.Support);
                    entityManager.SetComponentData(wagon.Entity, supplyTrain);
                    continue;
                }

                if (nearbyEngineCount <= 0)
                {
                    wagon.Support = support;
                    entityManager.SetComponentData(wagon.Entity, wagon.Support);
                    entityManager.SetComponentData(wagon.Entity, supplyTrain);
                    continue;
                }

                support.RefreshCount++;
                bool transferReady =
                    (elapsed - supplyTrain.LastSupplyTransferAt) >= SiegeSupportCanon.SupplyTransferSeconds;
                if (transferReady)
                {
                    float transferScale = SiegeSupportCanon.ResolveSupplyTransferScale(nearbyEngineCount);
                    int stockpileIndex = FindStockpileIndex(stockpiles, wagon.FactionId);
                    if (stockpileIndex >= 0 &&
                        TrySpendSiegeTransfer(ref stockpiles, stockpileIndex, transferScale))
                    {
                        for (int j = 0; j < nearbyEngineIndices.Count; j++)
                        {
                            int engineIndex = nearbyEngineIndices[j];
                            var engine = engines[engineIndex];
                            engine.Support.SuppliedUntil = Math.Max(
                                engine.Support.SuppliedUntil,
                                elapsed + SiegeSupportCanon.SupplyDurationSeconds);
                            engine.Support.LastRefreshAt = elapsed;
                            engine.Support.RefreshCount++;
                            engines[engineIndex] = engine;
                        }

                        supplyTrain.LastSupplyTransferAt = elapsed;
                        support.Status = SiegeSupportStatus.Supporting;
                    }
                    else
                    {
                        support.Status = SiegeSupportStatus.Starved;
                    }
                }
                else
                {
                    support.Status = SiegeSupportStatus.Supporting;
                }

                wagon.Support = support;
                entityManager.SetComponentData(wagon.Entity, wagon.Support);
                entityManager.SetComponentData(wagon.Entity, supplyTrain);
            }

            for (int i = 0; i < engines.Count; i++)
            {
                entityManager.SetComponentData(engines[i].Entity, engines[i].Support);
                entityManager.SetComponentData(engines[i].Entity, engines[i].Health);
            }

            for (int i = 0; i < engineers.Count; i++)
            {
                entityManager.SetComponentData(engineers[i].Entity, engineers[i].Support);
            }

            for (int i = 0; i < stockpiles.Count; i++)
            {
                entityManager.SetComponentData(stockpiles[i].Entity, stockpiles[i].Stockpile);
            }
        }

        private static int FindNearestSupplyCampIndex(
            List<SupplyCampRecord> supplyCamps,
            FixedString32Bytes factionId,
            float3 wagonPosition)
        {
            int bestIndex = -1;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < supplyCamps.Count; i++)
            {
                if (!supplyCamps[i].FactionId.Equals(factionId) ||
                    !supplyCamps[i].IsOperational)
                {
                    continue;
                }

                float distance = Distance2D(wagonPosition, supplyCamps[i].Position);
                if (distance > SiegeSupportCanon.SupplyCampLinkRadius || distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestIndex = i;
            }

            return bestIndex;
        }

        private static bool TrySpendSiegeTransfer(
            ref List<FactionStockpileRecord> stockpiles,
            int stockpileIndex,
            float transferScale)
        {
            var record = stockpiles[stockpileIndex];
            float foodCost = SiegeSupportCanon.SupplyTransferFoodCost * transferScale;
            float waterCost = SiegeSupportCanon.SupplyTransferWaterCost * transferScale;
            float woodCost = SiegeSupportCanon.SupplyTransferWoodCost * transferScale;

            if (record.Stockpile.Food < foodCost ||
                record.Stockpile.Water < waterCost ||
                record.Stockpile.Wood < woodCost)
            {
                return false;
            }

            record.Stockpile.Food -= foodCost;
            record.Stockpile.Water -= waterCost;
            record.Stockpile.Wood -= woodCost;
            stockpiles[stockpileIndex] = record;
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

        private static float Distance2D(float3 a, float3 b)
        {
            return math.distance(a.xz, b.xz);
        }

        private struct SiegeUnitRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public HealthComponent Health;
            public SiegeSupportComponent Support;
        }

        private struct SupplyCampRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public bool IsOperational;
        }

        private struct FactionStockpileRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public ResourceStockpileComponent Stockpile;
        }
    }
}
