using System;
using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Per-unit field-water support scan. This mirrors the browser sustainment logic:
    /// support availability is checked continuously, but the water-cost refresh only
    /// fires on the canonical four-second transfer interval.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SiegeSupportRefreshSystem))]
    [UpdateBefore(typeof(FieldWaterStrainSystem))]
    public partial struct FieldWaterSupportScanSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FieldWaterComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            double elapsed = SystemAPI.Time.ElapsedTime;
            float tileSize = 0f;
            if (SystemAPI.TryGetSingleton<MapBootstrapConfigComponent>(out var mapConfig))
            {
                tileSize = mapConfig.TileSize;
            }

            var stockpiles = new List<FactionStockpileRecord>(4);
            var controlPoints = new List<ControlPointSupportRecord>(8);
            var settlements = new List<SettlementSupportRecord>(8);
            var buildingSources = new List<BuildingSupportRecord>(8);
            var wagonSources = new List<WagonSupportRecord>(8);

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

            foreach (var (controlPoint, position) in
                SystemAPI.Query<RefRO<ControlPointComponent>, RefRO<PositionComponent>>())
            {
                if (controlPoint.ValueRO.OwnerFactionId.Length == 0)
                {
                    continue;
                }

                controlPoints.Add(new ControlPointSupportRecord
                {
                    FactionId = controlPoint.ValueRO.OwnerFactionId,
                    Position = position.ValueRO.Value,
                    Radius = math.max(
                        FieldWaterCanon.FieldWaterLocalSupportRadius,
                        controlPoint.ValueRO.RadiusTiles * math.max(0f, tileSize) * 0.7f),
                });
            }

            foreach (var (settlement, faction, position) in
                SystemAPI.Query<
                    RefRO<SettlementComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>>())
            {
                settlements.Add(new SettlementSupportRecord
                {
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                });
            }

            foreach (var (buildingType, faction, position, health, entity) in
                SystemAPI.Query<
                    RefRO<BuildingTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current <= 0f ||
                    !FieldWaterCanon.TryGetBuildingSupportProfile(
                        buildingType.ValueRO.TypeId,
                        out float supportRadius,
                        out float supportDurationSeconds))
                {
                    continue;
                }

                if (buildingType.ValueRO.SupportsSiegeLogistics &&
                    entityManager.HasComponent<SiegeSupplyCampComponent>(entity))
                {
                    var camp = entityManager.GetComponentData<SiegeSupplyCampComponent>(entity);
                    if (!SiegeSupplyInterdictionCanon.IsCampOperational(camp))
                    {
                        continue;
                    }
                }

                buildingSources.Add(new BuildingSupportRecord
                {
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    Radius = supportRadius,
                    DurationSeconds = supportDurationSeconds,
                });
            }

            foreach (var (support, unitType, faction, position, health, supplyTrain) in
                SystemAPI.Query<
                    RefRO<SiegeSupportComponent>,
                    RefRO<UnitTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>,
                    RefRO<SiegeSupplyTrainComponent>>()
                .WithNone<DeadTag>())
            {
                if (!support.ValueRO.IsSupplyWagon ||
                    !support.ValueRO.HasLinkedSupplyCamp ||
                    supplyTrain.ValueRO.LinkedCampEntity == Entity.Null ||
                    (supplyTrain.ValueRO.LogisticsInterdictedUntil > elapsed) ||
                    (supplyTrain.ValueRO.ConvoyRecoveryUntil > elapsed) ||
                    !entityManager.Exists(supplyTrain.ValueRO.LinkedCampEntity) ||
                    !entityManager.HasComponent<HealthComponent>(supplyTrain.ValueRO.LinkedCampEntity) ||
                    !entityManager.HasComponent<SiegeSupplyCampComponent>(supplyTrain.ValueRO.LinkedCampEntity) ||
                    !SiegeSupplyInterdictionCanon.IsCampOperational(
                        entityManager.GetComponentData<SiegeSupplyCampComponent>(supplyTrain.ValueRO.LinkedCampEntity)) ||
                    entityManager.GetComponentData<HealthComponent>(supplyTrain.ValueRO.LinkedCampEntity).Current <= 0f ||
                    health.ValueRO.Current <= 0f ||
                    !FieldWaterCanon.TryGetSupplyWagonSupportProfile(
                        unitType.ValueRO.TypeId,
                        out float supportRadius,
                        out float supportDurationSeconds))
                {
                    continue;
                }

                wagonSources.Add(new WagonSupportRecord
                {
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    Radius = supportRadius,
                    DurationSeconds = supportDurationSeconds,
                });
            }

            foreach (var (fieldWaterRw, faction, position, health) in
                SystemAPI.Query<
                    RefRW<FieldWaterComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                float supportDurationSeconds = 0f;
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    if (!controlPoints[i].FactionId.Equals(faction.ValueRO.FactionId) ||
                        Distance2D(controlPoints[i].Position, position.ValueRO.Value) > controlPoints[i].Radius)
                    {
                        continue;
                    }

                    supportDurationSeconds = math.max(
                        supportDurationSeconds,
                        FieldWaterCanon.FieldWaterSupportDurationSeconds);
                    break;
                }

                for (int i = 0; i < settlements.Count; i++)
                {
                    if (!settlements[i].FactionId.Equals(faction.ValueRO.FactionId) ||
                        Distance2D(settlements[i].Position, position.ValueRO.Value) > FieldWaterCanon.FieldWaterSettlementSupportRadius)
                    {
                        continue;
                    }

                    supportDurationSeconds = math.max(
                        supportDurationSeconds,
                        FieldWaterCanon.FieldWaterSupportDurationSeconds);
                    break;
                }

                for (int i = 0; i < buildingSources.Count; i++)
                {
                    if (!buildingSources[i].FactionId.Equals(faction.ValueRO.FactionId) ||
                        Distance2D(buildingSources[i].Position, position.ValueRO.Value) > buildingSources[i].Radius)
                    {
                        continue;
                    }

                    supportDurationSeconds = math.max(
                        supportDurationSeconds,
                        buildingSources[i].DurationSeconds);
                }

                for (int i = 0; i < wagonSources.Count; i++)
                {
                    if (!wagonSources[i].FactionId.Equals(faction.ValueRO.FactionId) ||
                        Distance2D(wagonSources[i].Position, position.ValueRO.Value) > wagonSources[i].Radius)
                    {
                        continue;
                    }

                    supportDurationSeconds = math.max(
                        supportDurationSeconds,
                        wagonSources[i].DurationSeconds);
                }

                var fieldWater = fieldWaterRw.ValueRO;
                if (supportDurationSeconds > 0f)
                {
                    TryRefreshSupport(
                        ref stockpiles,
                        faction.ValueRO.FactionId,
                        elapsed,
                        supportDurationSeconds,
                        ref fieldWater);
                }

                fieldWaterRw.ValueRW = fieldWater;
            }

            for (int i = 0; i < stockpiles.Count; i++)
            {
                entityManager.SetComponentData(stockpiles[i].Entity, stockpiles[i].Stockpile);
            }
        }

        private static bool TryRefreshSupport(
            ref List<FactionStockpileRecord> stockpiles,
            FixedString32Bytes factionId,
            double elapsed,
            float sourceDurationSeconds,
            ref FieldWaterComponent fieldWater)
        {
            if ((elapsed - fieldWater.LastTransferAt) >= FieldWaterCanon.FieldWaterTransferIntervalSeconds)
            {
                int stockpileIndex = FindStockpileIndex(stockpiles, factionId);
                if (stockpileIndex < 0)
                {
                    return false;
                }

                var record = stockpiles[stockpileIndex];
                if (record.Stockpile.Water < FieldWaterCanon.FieldWaterTransferCost)
                {
                    return false;
                }

                record.Stockpile.Water -= FieldWaterCanon.FieldWaterTransferCost;
                stockpiles[stockpileIndex] = record;
                fieldWater.LastTransferAt = elapsed;
            }

            fieldWater.SuppliedUntil = Math.Max(
                fieldWater.SuppliedUntil,
                elapsed + math.max(
                    FieldWaterCanon.FieldWaterSupportDurationSeconds,
                    sourceDurationSeconds));
            fieldWater.LastSupportRefreshAt = elapsed;
            fieldWater.SupportRefreshCount++;
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

        private struct FactionStockpileRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public ResourceStockpileComponent Stockpile;
        }

        private struct ControlPointSupportRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
            public float Radius;
        }

        private struct SettlementSupportRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
        }

        private struct BuildingSupportRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
            public float Radius;
            public float DurationSeconds;
        }

        private struct WagonSupportRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
            public float Radius;
            public float DurationSeconds;
        }
    }
}
