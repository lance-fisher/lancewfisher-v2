using System;
using System.Collections.Generic;
using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Ports the browser convoy interdiction seam into Unity ECS:
    ///
    /// - raider presence near a wagon or linked camp interdicts the convoy
    /// - camps lose operational stockpile while the supply line is contested
    /// - recovering wagons require escort screen before reconsolidation
    /// - runtime counts feed AISiegeOrchestrationComponent without editing the AI lane
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SiegeComponentInitializationSystem))]
    [UpdateBefore(typeof(SiegeSupportRefreshSystem))]
    [UpdateBefore(typeof(AISiegeOrchestrationSystem))]
    public partial struct SiegeSupplyInterdictionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SiegeSupplyTrainComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            double elapsed = SystemAPI.Time.ElapsedTime;
            float dt = SystemAPI.Time.DeltaTime;

            var raiders = new List<RaiderRecord>(8);
            var camps = new List<CampRecord>(8);
            var wagons = new List<WagonRecord>(8);
            var escorts = new List<EscortRecord>(16);
            var stockpiles = new List<FactionStockpileRecord>(4);
            var snapshots = new List<FactionSiegeSnapshot>(4);

            foreach (var (unitType, faction, position, health) in
                SystemAPI.Query<
                    RefRO<UnitTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                if (SiegeSupplyInterdictionCanon.IsRaider(unitType.ValueRO))
                {
                    raiders.Add(new RaiderRecord
                    {
                        FactionId = faction.ValueRO.FactionId,
                        Position = position.ValueRO.Value,
                    });
                }

                if (SiegeSupplyInterdictionCanon.IsEscortEligible(unitType.ValueRO))
                {
                    escorts.Add(new EscortRecord
                    {
                        FactionId = faction.ValueRO.FactionId,
                        Position = position.ValueRO.Value,
                    });
                }
            }

            foreach (var (campState, buildingType, faction, position, health, entity) in
                SystemAPI.Query<
                    RefRO<SiegeSupplyCampComponent>,
                    RefRO<BuildingTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current <= 0f || !buildingType.ValueRO.SupportsSiegeLogistics)
                {
                    continue;
                }

                camps.Add(new CampRecord
                {
                    Entity = entity,
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    State = campState.ValueRO,
                });

                int snapshotIndex = GetOrCreateSnapshotIndex(snapshots, faction.ValueRO.FactionId);
                var snapshot = snapshots[snapshotIndex];
                snapshot.CampCount++;
                snapshots[snapshotIndex] = snapshot;
            }

            foreach (var (support, supplyTrain, faction, position, health, entity) in
                SystemAPI.Query<
                    RefRO<SiegeSupportComponent>,
                    RefRO<SiegeSupplyTrainComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (!support.ValueRO.IsSupplyWagon || health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                wagons.Add(new WagonRecord
                {
                    Entity = entity,
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    Train = supplyTrain.ValueRO,
                });

                int snapshotIndex = GetOrCreateSnapshotIndex(snapshots, faction.ValueRO.FactionId);
                var snapshot = snapshots[snapshotIndex];
                snapshot.WagonCount++;
                snapshots[snapshotIndex] = snapshot;
            }

            foreach (var (support, faction, health) in
                SystemAPI.Query<
                    RefRO<SiegeSupportComponent>,
                    RefRO<FactionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                int snapshotIndex = GetOrCreateSnapshotIndex(snapshots, faction.ValueRO.FactionId);
                var snapshot = snapshots[snapshotIndex];
                if (support.ValueRO.IsSiegeEngine && support.ValueRO.SuppliedUntil > elapsed)
                {
                    snapshot.SuppliedEngineCount++;
                }
                snapshots[snapshotIndex] = snapshot;
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

            for (int i = 0; i < camps.Count; i++)
            {
                var camp = camps[i];
                UpdateCampState(ref camp, raiders, elapsed, dt);
                camps[i] = camp;
                entityManager.SetComponentData(camp.Entity, camp.State);

                if (SiegeSupplyInterdictionCanon.IsCampOperational(camp.State))
                {
                    int snapshotIndex = GetOrCreateSnapshotIndex(snapshots, camp.FactionId);
                    var snapshot = snapshots[snapshotIndex];
                    snapshot.OperationalCampCount++;
                    snapshots[snapshotIndex] = snapshot;
                }
            }

            for (int i = 0; i < wagons.Count; i++)
            {
                var wagon = wagons[i];
                UpdateWagonState(ref wagon, camps, raiders, escorts, ref stockpiles, elapsed);
                wagons[i] = wagon;
                entityManager.SetComponentData(wagon.Entity, wagon.Train);

                int snapshotIndex = GetOrCreateSnapshotIndex(snapshots, wagon.FactionId);
                var snapshot = snapshots[snapshotIndex];
                if (wagon.Train.LogisticsInterdictedUntil > elapsed)
                {
                    snapshot.InterdictedWagonCount++;
                }
                else if (wagon.Train.ConvoyRecoveryUntil > elapsed)
                {
                    snapshot.RecoveringWagonCount++;
                    if (!wagon.Train.EscortScreened)
                    {
                        snapshot.ConvoyRecoveringUnscreenedCount++;
                    }
                }

                snapshots[snapshotIndex] = snapshot;
            }

            for (int i = 0; i < stockpiles.Count; i++)
            {
                entityManager.SetComponentData(stockpiles[i].Entity, stockpiles[i].Stockpile);
            }

            foreach (var (siegeRw, faction) in
                SystemAPI.Query<RefRW<AISiegeOrchestrationComponent>, RefRO<FactionComponent>>())
            {
                if (!TryGetSnapshotIndex(snapshots, faction.ValueRO.FactionId, out int snapshotIndex))
                {
                    siegeRw.ValueRW.SupplyCampCompleted = false;
                    siegeRw.ValueRW.SupplyLineReady = false;
                    siegeRw.ValueRW.InterdictedWagonCount = 0;
                    siegeRw.ValueRW.RecoveringWagonCount = 0;
                    siegeRw.ValueRW.ConvoyRecoveringUnscreenedCount = 0;
                    siegeRw.ValueRW.SuppliedSiegeReady = false;
                    continue;
                }

                var snapshot = snapshots[snapshotIndex];
                siegeRw.ValueRW.SupplyCampCompleted = snapshot.CampCount > 0;
                siegeRw.ValueRW.SupplyLineReady = snapshot.CampCount > 0 && snapshot.WagonCount > 0;
                siegeRw.ValueRW.InterdictedWagonCount = snapshot.InterdictedWagonCount;
                siegeRw.ValueRW.RecoveringWagonCount = snapshot.RecoveringWagonCount;
                siegeRw.ValueRW.ConvoyRecoveringUnscreenedCount = snapshot.ConvoyRecoveringUnscreenedCount;
                siegeRw.ValueRW.SuppliedSiegeReady = snapshot.SuppliedEngineCount > 0;
            }
        }

        private static void UpdateCampState(
            ref CampRecord camp,
            List<RaiderRecord> raiders,
            double elapsed,
            float dt)
        {
            camp.State.NearbyRaiderCount = 0;
            FixedString32Bytes attackerId = default;
            float raidRadiusSq = SiegeSupplyInterdictionCanon.SupplyLineRaidRadius *
                                 SiegeSupplyInterdictionCanon.SupplyLineRaidRadius;

            for (int i = 0; i < raiders.Count; i++)
            {
                if (raiders[i].FactionId.Equals(camp.FactionId) ||
                    Distance2DSq(camp.Position, raiders[i].Position) > raidRadiusSq)
                {
                    continue;
                }

                camp.State.NearbyRaiderCount++;
                if (attackerId.Length == 0)
                {
                    attackerId = raiders[i].FactionId;
                }
            }

            if (camp.State.NearbyRaiderCount > 0)
            {
                camp.State.Stockpile = math.max(
                    0f,
                    camp.State.Stockpile - (SiegeSupplyInterdictionCanon.SupplyCampRaidDrainPerSecond *
                                            camp.State.NearbyRaiderCount * dt));
                camp.State.LastInterdictedAt = elapsed;
                camp.State.InterdictedByFactionId = attackerId;
                return;
            }

            if (camp.State.Stockpile < camp.State.MaxStockpile)
            {
                camp.State.Stockpile = math.min(
                    camp.State.MaxStockpile,
                    camp.State.Stockpile + (SiegeSupplyInterdictionCanon.SupplyCampRecoveryPerSecond * dt));
                camp.State.LastRecoveredAt = elapsed;
            }

            if (camp.State.Stockpile >= camp.State.OperationalThreshold)
            {
                camp.State.InterdictedByFactionId = default;
            }
        }

        private static void UpdateWagonState(
            ref WagonRecord wagon,
            List<CampRecord> camps,
            List<RaiderRecord> raiders,
            List<EscortRecord> escorts,
            ref List<FactionStockpileRecord> stockpiles,
            double elapsed)
        {
            if (wagon.Train.LogisticsInterdictedUntil > 0d &&
                elapsed >= wagon.Train.LogisticsInterdictedUntil)
            {
                wagon.Train.LogisticsInterdictedUntil = 0d;
                wagon.Train.InterdictedByFactionId = default;
            }

            if (wagon.Train.ConvoyRecoveryUntil > 0d &&
                elapsed >= wagon.Train.ConvoyRecoveryUntil)
            {
                wagon.Train.ConvoyRecoveryUntil = 0d;
                wagon.Train.ConvoyReconsolidatedAt = 0d;
            }

            int nearestCampIndex = FindNearestCampIndex(camps, wagon.FactionId, wagon.Position, operationalOnly: false);
            int operationalCampIndex = FindNearestCampIndex(camps, wagon.FactionId, wagon.Position, operationalOnly: true);
            wagon.Train.LinkedCampEntity = operationalCampIndex >= 0
                ? camps[operationalCampIndex].Entity
                : Entity.Null;

            FixedString32Bytes wagonAttackerId = default;
            FixedString32Bytes campAttackerId = default;
            bool raiderNearWagon = TryFindHostileRaider(
                raiders,
                wagon.FactionId,
                wagon.Position,
                SiegeSupplyInterdictionCanon.ConvoyEscortScreenRadius,
                out wagonAttackerId);
            bool raiderNearCamp = nearestCampIndex >= 0 && TryFindHostileRaider(
                raiders,
                wagon.FactionId,
                camps[nearestCampIndex].Position,
                SiegeSupplyInterdictionCanon.SupplyLineRaidRadius,
                out campAttackerId);

            if (wagon.Train.LogisticsInterdictedUntil <= elapsed &&
                (raiderNearWagon || raiderNearCamp))
            {
                wagon.Train.LogisticsInterdictedUntil = elapsed +
                    SiegeSupplyInterdictionCanon.WagonInterdictionDurationSeconds;
                wagon.Train.ConvoyRecoveryUntil = math.max(
                    wagon.Train.ConvoyRecoveryUntil,
                    wagon.Train.LogisticsInterdictedUntil +
                    SiegeSupplyInterdictionCanon.ConvoyRecoveryDurationSeconds);
                wagon.Train.InterdictedByFactionId = wagonAttackerId.Length > 0
                    ? wagonAttackerId
                    : campAttackerId;
                wagon.Train.ConvoyReconsolidatedAt = 0d;

                if (raiderNearWagon)
                {
                    ApplyInterdictionLoss(ref stockpiles, wagon.FactionId);
                }
            }

            bool recovering = wagon.Train.LogisticsInterdictedUntil <= elapsed &&
                              wagon.Train.ConvoyRecoveryUntil > elapsed;
            wagon.Train.EscortCount = CountEscorts(escorts, wagon.FactionId, wagon.Position);
            wagon.Train.RequiredEscortCount = recovering
                ? SiegeSupplyInterdictionCanon.ConvoyEscortMinEscorts
                : 1;
            wagon.Train.EscortScreened = wagon.Train.EscortCount >= wagon.Train.RequiredEscortCount;

            if (recovering)
            {
                wagon.Train.ConvoyReconsolidatedAt = wagon.Train.EscortScreened &&
                                                     wagon.Train.ConvoyReconsolidatedAt <= 0d
                    ? elapsed
                    : wagon.Train.EscortScreened
                        ? wagon.Train.ConvoyReconsolidatedAt
                        : 0d;
            }
            else
            {
                wagon.Train.ConvoyReconsolidatedAt = 0d;
            }
        }

        private static void ApplyInterdictionLoss(
            ref List<FactionStockpileRecord> stockpiles,
            FixedString32Bytes factionId)
        {
            if (!TryFindStockpileIndex(stockpiles, factionId, out int stockpileIndex))
            {
                return;
            }

            var stockpile = stockpiles[stockpileIndex];
            stockpile.Stockpile.Food = math.max(
                0f,
                stockpile.Stockpile.Food - SiegeSupplyInterdictionCanon.InterdictionFoodLoss);
            stockpile.Stockpile.Water = math.max(
                0f,
                stockpile.Stockpile.Water - SiegeSupplyInterdictionCanon.InterdictionWaterLoss);
            stockpile.Stockpile.Wood = math.max(
                0f,
                stockpile.Stockpile.Wood - SiegeSupplyInterdictionCanon.InterdictionWoodLoss);
            stockpiles[stockpileIndex] = stockpile;
        }

        private static int CountEscorts(
            List<EscortRecord> escorts,
            FixedString32Bytes factionId,
            float3 wagonPosition)
        {
            int escortCount = 0;
            float escortRadiusSq = SiegeSupplyInterdictionCanon.ConvoyEscortScreenRadius *
                                   SiegeSupplyInterdictionCanon.ConvoyEscortScreenRadius;

            for (int i = 0; i < escorts.Count; i++)
            {
                if (!escorts[i].FactionId.Equals(factionId) ||
                    Distance2DSq(wagonPosition, escorts[i].Position) > escortRadiusSq)
                {
                    continue;
                }

                escortCount++;
            }

            return escortCount;
        }

        private static bool TryFindHostileRaider(
            List<RaiderRecord> raiders,
            FixedString32Bytes factionId,
            float3 position,
            float radius,
            out FixedString32Bytes attackerId)
        {
            attackerId = default;
            float radiusSq = radius * radius;
            for (int i = 0; i < raiders.Count; i++)
            {
                if (raiders[i].FactionId.Equals(factionId) ||
                    Distance2DSq(position, raiders[i].Position) > radiusSq)
                {
                    continue;
                }

                attackerId = raiders[i].FactionId;
                return true;
            }

            return false;
        }

        private static int FindNearestCampIndex(
            List<CampRecord> camps,
            FixedString32Bytes factionId,
            float3 wagonPosition,
            bool operationalOnly)
        {
            int bestIndex = -1;
            float bestDistanceSq = float.MaxValue;
            float linkRadiusSq = SiegeSupportCanon.SupplyCampLinkRadius * SiegeSupportCanon.SupplyCampLinkRadius;

            for (int i = 0; i < camps.Count; i++)
            {
                if (!camps[i].FactionId.Equals(factionId) ||
                    (operationalOnly && !SiegeSupplyInterdictionCanon.IsCampOperational(camps[i].State)))
                {
                    continue;
                }

                float distanceSq = Distance2DSq(wagonPosition, camps[i].Position);
                if (distanceSq > linkRadiusSq || distanceSq >= bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                bestIndex = i;
            }

            return bestIndex;
        }

        private static int GetOrCreateSnapshotIndex(
            List<FactionSiegeSnapshot> snapshots,
            FixedString32Bytes factionId)
        {
            if (TryGetSnapshotIndex(snapshots, factionId, out int existingIndex))
            {
                return existingIndex;
            }

            snapshots.Add(new FactionSiegeSnapshot
            {
                FactionId = factionId,
            });
            return snapshots.Count - 1;
        }

        private static bool TryGetSnapshotIndex(
            List<FactionSiegeSnapshot> snapshots,
            FixedString32Bytes factionId,
            out int index)
        {
            for (int i = 0; i < snapshots.Count; i++)
            {
                if (!snapshots[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                index = i;
                return true;
            }

            index = -1;
            return false;
        }

        private static bool TryFindStockpileIndex(
            List<FactionStockpileRecord> stockpiles,
            FixedString32Bytes factionId,
            out int index)
        {
            for (int i = 0; i < stockpiles.Count; i++)
            {
                if (!stockpiles[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                index = i;
                return true;
            }

            index = -1;
            return false;
        }

        private static float Distance2DSq(float3 a, float3 b)
        {
            return math.distancesq(a.xz, b.xz);
        }

        private struct RaiderRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
        }

        private struct CampRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public SiegeSupplyCampComponent State;
        }

        private struct EscortRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
        }

        private struct WagonRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public SiegeSupplyTrainComponent Train;
        }

        private struct FactionStockpileRecord
        {
            public Entity Entity;
            public FixedString32Bytes FactionId;
            public ResourceStockpileComponent Stockpile;
        }

        private struct FactionSiegeSnapshot
        {
            public FixedString32Bytes FactionId;
            public int CampCount;
            public int OperationalCampCount;
            public int WagonCount;
            public int InterdictedWagonCount;
            public int RecoveringWagonCount;
            public int ConvoyRecoveringUnscreenedCount;
            public int SuppliedEngineCount;
        }
    }
}
