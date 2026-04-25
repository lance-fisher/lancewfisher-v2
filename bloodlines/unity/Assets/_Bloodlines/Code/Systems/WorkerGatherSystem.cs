using Bloodlines.Components;
using Bloodlines.Raids;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First ECS worker gather-deposit primary loop.
    ///
    /// Advances a controlled worker through seek -> gather -> return -> deposit
    /// against a canonical resource node, then resumes gather if the node still
    /// has resources. Mirrors the browser runtime's primary-economy gather loop.
    ///
    /// Scope of this first slice:
    /// - Any resource type can be assigned; deposits accrue into the faction's
    ///   ResourceStockpileComponent field that matches the carry resource.
    /// - Drop-off is the nearest alive, fully-built, faction-owned structure that
    ///   accepts the carried resource and is not under active scout raid.
    /// - Movement is driven through the existing MoveCommandComponent, keeping
    ///   the pattern consistent with player-issued moves.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UnitProductionSystem))]
    public partial struct WorkerGatherSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorkerGatherComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                return;
            }

            var entityManager = state.EntityManager;
            double elapsed = SystemAPI.Time.ElapsedTime;

            using var workerEntities = CollectWorkerEntities(entityManager);
            foreach (var workerEntity in workerEntities)
            {
                if (!entityManager.HasComponent<WorkerGatherComponent>(workerEntity))
                {
                    continue;
                }

                var gather = entityManager.GetComponentData<WorkerGatherComponent>(workerEntity);
                if (gather.Phase == WorkerGatherPhase.Idle)
                {
                    continue;
                }

                if (!entityManager.HasComponent<PositionComponent>(workerEntity) ||
                    !entityManager.HasComponent<FactionComponent>(workerEntity))
                {
                    continue;
                }

                var workerPosition = entityManager.GetComponentData<PositionComponent>(workerEntity).Value;
                var workerFaction = entityManager.GetComponentData<FactionComponent>(workerEntity).FactionId;

                bool nodeAlive = gather.AssignedNode != Entity.Null &&
                                 entityManager.Exists(gather.AssignedNode) &&
                                 entityManager.HasComponent<ResourceNodeComponent>(gather.AssignedNode);
                if (!nodeAlive && gather.Phase != WorkerGatherPhase.Returning && gather.Phase != WorkerGatherPhase.Depositing)
                {
                    if (gather.CarryAmount > 0f)
                    {
                        gather.Phase = WorkerGatherPhase.Returning;
                    }
                    else
                    {
                        gather.Phase = WorkerGatherPhase.Idle;
                        gather.AssignedNode = Entity.Null;
                        entityManager.SetComponentData(workerEntity, gather);
                        continue;
                    }
                }

                switch (gather.Phase)
                {
                    case WorkerGatherPhase.Seeking:
                        AdvanceSeeking(entityManager, workerEntity, workerPosition, ref gather);
                        break;
                    case WorkerGatherPhase.Gathering:
                        AdvanceGathering(entityManager, workerEntity, workerPosition, dt, ref gather);
                        break;
                    case WorkerGatherPhase.Returning:
                        AdvanceReturning(entityManager, workerEntity, workerPosition, workerFaction, elapsed, ref gather);
                        break;
                    case WorkerGatherPhase.Depositing:
                        AdvanceDepositing(entityManager, workerEntity, workerPosition, workerFaction, elapsed, ref gather);
                        break;
                }

                entityManager.SetComponentData(workerEntity, gather);
            }
        }

        private static void AdvanceSeeking(
            EntityManager entityManager,
            Entity workerEntity,
            float3 workerPosition,
            ref WorkerGatherComponent gather)
        {
            var nodePosition = entityManager.GetComponentData<PositionComponent>(gather.AssignedNode).Value;
            float distanceSq = math.distancesq(workerPosition, nodePosition);
            float radiusSq = gather.GatherRadius * gather.GatherRadius;
            if (distanceSq <= radiusSq)
            {
                gather.Phase = WorkerGatherPhase.Gathering;
                StopMoveCommand(entityManager, workerEntity);
                return;
            }

            IssueMoveCommand(entityManager, workerEntity, nodePosition, math.max(0.2f, gather.GatherRadius * 0.9f));
        }

        private static void AdvanceGathering(
            EntityManager entityManager,
            Entity workerEntity,
            float3 workerPosition,
            float dt,
            ref WorkerGatherComponent gather)
        {
            var nodePosition = entityManager.GetComponentData<PositionComponent>(gather.AssignedNode).Value;
            float distanceSq = math.distancesq(workerPosition, nodePosition);
            float radiusSq = gather.GatherRadius * gather.GatherRadius;
            if (distanceSq > radiusSq)
            {
                IssueMoveCommand(entityManager, workerEntity, nodePosition, math.max(0.2f, gather.GatherRadius * 0.9f));
                return;
            }

            StopMoveCommand(entityManager, workerEntity);

            var node = entityManager.GetComponentData<ResourceNodeComponent>(gather.AssignedNode);
            if (node.Amount <= 0f)
            {
                gather.Phase = gather.CarryAmount > 0f ? WorkerGatherPhase.Returning : WorkerGatherPhase.Idle;
                return;
            }

            float capacity = math.max(1f, gather.CarryCapacity);
            float remainingCapacity = capacity - gather.CarryAmount;
            if (remainingCapacity <= 0f)
            {
                gather.Phase = WorkerGatherPhase.Returning;
                return;
            }

            float rate = math.max(0f, gather.GatherRate);
            float increment = math.min(rate * dt, math.min(remainingCapacity, node.Amount));
            if (increment > 0f)
            {
                gather.CarryAmount += increment;
                if (gather.CarryResourceId.Length == 0)
                {
                    gather.CarryResourceId = gather.AssignedResourceId;
                }
                node.Amount = math.max(0f, node.Amount - increment);
                entityManager.SetComponentData(gather.AssignedNode, node);
            }

            if (gather.CarryAmount >= capacity - 0.001f || node.Amount <= 0f)
            {
                gather.Phase = WorkerGatherPhase.Returning;
            }
        }

        private static void AdvanceReturning(
            EntityManager entityManager,
            Entity workerEntity,
            float3 workerPosition,
            FixedString32Bytes workerFaction,
            double elapsed,
            ref WorkerGatherComponent gather)
        {
            FixedString32Bytes resourceId = gather.CarryResourceId.Length > 0
                ? gather.CarryResourceId
                : gather.AssignedResourceId;

            if (!TryFindNearestDropOff(entityManager, workerFaction, resourceId, workerPosition, elapsed, out var dropOffPosition, out _))
            {
                return;
            }

            float distanceSq = math.distancesq(workerPosition, dropOffPosition);
            float radiusSq = gather.DepositRadius * gather.DepositRadius;
            if (distanceSq <= radiusSq)
            {
                gather.Phase = WorkerGatherPhase.Depositing;
                StopMoveCommand(entityManager, workerEntity);
                return;
            }

            IssueMoveCommand(entityManager, workerEntity, dropOffPosition, math.max(0.2f, gather.DepositRadius * 0.9f));
        }

        private static void AdvanceDepositing(
            EntityManager entityManager,
            Entity workerEntity,
            float3 workerPosition,
            FixedString32Bytes workerFaction,
            double elapsed,
            ref WorkerGatherComponent gather)
        {
            FixedString32Bytes resourceId = gather.CarryResourceId.Length > 0
                ? gather.CarryResourceId
                : gather.AssignedResourceId;

            // Re-find the nearest drop-off (the worker may have moved relative to
            // alternatives, but for the smelting fuel check we only need the
            // currently-targeted building's smelting params). Returns the entity
            // so we can read SmeltingComponent if present.
            Entity dropOffEntity = Entity.Null;
            if (gather.CarryAmount > 0f)
            {
                TryFindNearestDropOff(entityManager, workerFaction, resourceId, workerPosition, elapsed, out _, out dropOffEntity);
            }

            // Browser parity (simulation.js ~8454-8481): if the drop-off building has
            // a smelting fuel requirement, deduct fuel from the faction stockpile or
            // return the carried ore to the source node and stall.
            bool smeltingStalled = false;
            if (gather.CarryAmount > 0f && dropOffEntity != Entity.Null &&
                entityManager.HasComponent<SmeltingComponent>(dropOffEntity))
            {
                var smelting = entityManager.GetComponentData<SmeltingComponent>(dropOffEntity);
                if (smelting.FuelRatio > 0f && smelting.FuelResourceId.Length > 0)
                {
                    float fuelNeeded = gather.CarryAmount * smelting.FuelRatio;
                    if (!TryConsumeFactionResource(entityManager, workerFaction, smelting.FuelResourceId, fuelNeeded))
                    {
                        // Insufficient fuel: return ore to source node, drop carry,
                        // re-seek so the worker re-attempts after fuel arrives.
                        if (gather.AssignedNode != Entity.Null &&
                            entityManager.Exists(gather.AssignedNode) &&
                            entityManager.HasComponent<ResourceNodeComponent>(gather.AssignedNode))
                        {
                            var node = entityManager.GetComponentData<ResourceNodeComponent>(gather.AssignedNode);
                            node.Amount += gather.CarryAmount;
                            entityManager.SetComponentData(gather.AssignedNode, node);
                        }
                        gather.CarryAmount = 0f;
                        gather.CarryResourceId = default;
                        smeltingStalled = true;
                    }
                }
            }

            if (!smeltingStalled && gather.CarryAmount > 0f &&
                TryDepositCarry(entityManager, workerFaction, gather.CarryResourceId, gather.CarryAmount))
            {
                gather.CarryAmount = 0f;
            }

            bool nodeStillProductive = gather.AssignedNode != Entity.Null &&
                                       entityManager.Exists(gather.AssignedNode) &&
                                       entityManager.HasComponent<ResourceNodeComponent>(gather.AssignedNode) &&
                                       entityManager.GetComponentData<ResourceNodeComponent>(gather.AssignedNode).Amount > 0f;

            if (nodeStillProductive)
            {
                gather.Phase = WorkerGatherPhase.Seeking;
            }
            else
            {
                gather.Phase = WorkerGatherPhase.Idle;
                gather.AssignedNode = Entity.Null;
                gather.CarryResourceId = default;
            }
        }

        private static NativeArray<Entity> CollectWorkerEntities(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<WorkerGatherComponent>());
            return query.ToEntityArray(Allocator.Temp);
        }

        private static void IssueMoveCommand(
            EntityManager entityManager,
            Entity workerEntity,
            float3 destination,
            float stoppingDistance)
        {
            if (!entityManager.HasComponent<MoveCommandComponent>(workerEntity))
            {
                return;
            }

            entityManager.SetComponentData(workerEntity, new MoveCommandComponent
            {
                Destination = destination,
                StoppingDistance = math.max(0.05f, stoppingDistance),
                IsActive = true,
            });
        }

        private static void StopMoveCommand(EntityManager entityManager, Entity workerEntity)
        {
            if (!entityManager.HasComponent<MoveCommandComponent>(workerEntity))
            {
                return;
            }

            var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(workerEntity);
            moveCommand.IsActive = false;
            entityManager.SetComponentData(workerEntity, moveCommand);
        }

        private static bool TryFindNearestDropOff(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString32Bytes resourceId,
            float3 workerPosition,
            double elapsed,
            out float3 dropOffPosition,
            out Entity dropOffEntity)
        {
            dropOffPosition = default;
            dropOffEntity = Entity.Null;
            float bestDistanceSq = float.MaxValue;
            bool found = false;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    !factions[i].FactionId.Equals(factionId) ||
                    entityManager.HasComponent<ConstructionStateComponent>(entities[i]))
                {
                    continue;
                }

                if (entityManager.HasComponent<BuildingRaidStateComponent>(entities[i]) &&
                    ScoutRaidCanon.IsBuildingRaided(
                        entityManager.GetComponentData<BuildingRaidStateComponent>(entities[i]),
                        elapsed))
                {
                    continue;
                }

                if (!ScoutRaidCanon.CanBuildingDropOff(buildingTypes[i].TypeId, resourceId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(workerPosition, positions[i].Value);
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    dropOffPosition = positions[i].Value;
                    dropOffEntity = entities[i];
                    found = true;
                }
            }

            return found;
        }

        private static bool TryConsumeFactionResource(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString32Bytes resourceId,
            float amount)
        {
            if (amount <= 0f)
            {
                return true;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(ResourceStockpileComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                var stockpile = stockpiles[i];
                float available = ReadStockpile(in stockpile, resourceId);
                if (available < amount)
                {
                    return false;
                }
                WriteStockpile(ref stockpile, resourceId, available - amount);
                entityManager.SetComponentData(entities[i], stockpile);
                return true;
            }

            return false;
        }

        private static float ReadStockpile(in ResourceStockpileComponent stockpile, FixedString32Bytes resourceId)
        {
            if (resourceId.Equals(new FixedString32Bytes("gold"))) return stockpile.Gold;
            if (resourceId.Equals(new FixedString32Bytes("wood"))) return stockpile.Wood;
            if (resourceId.Equals(new FixedString32Bytes("stone"))) return stockpile.Stone;
            if (resourceId.Equals(new FixedString32Bytes("iron"))) return stockpile.Iron;
            if (resourceId.Equals(new FixedString32Bytes("food"))) return stockpile.Food;
            if (resourceId.Equals(new FixedString32Bytes("water"))) return stockpile.Water;
            if (resourceId.Equals(new FixedString32Bytes("influence"))) return stockpile.Influence;
            return 0f;
        }

        private static void WriteStockpile(ref ResourceStockpileComponent stockpile, FixedString32Bytes resourceId, float value)
        {
            if (resourceId.Equals(new FixedString32Bytes("gold"))) { stockpile.Gold = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("wood"))) { stockpile.Wood = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("stone"))) { stockpile.Stone = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("iron"))) { stockpile.Iron = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("food"))) { stockpile.Food = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("water"))) { stockpile.Water = value; return; }
            if (resourceId.Equals(new FixedString32Bytes("influence"))) { stockpile.Influence = value; return; }
        }

        private static bool TryDepositCarry(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString32Bytes resourceId,
            float amount)
        {
            if (amount <= 0f)
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(ResourceStockpileComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                var stockpile = stockpiles[i];
                ApplyCarryToStockpile(ref stockpile, resourceId, amount);
                entityManager.SetComponentData(entities[i], stockpile);
                return true;
            }

            return false;
        }

        private static void ApplyCarryToStockpile(
            ref ResourceStockpileComponent stockpile,
            FixedString32Bytes resourceId,
            float amount)
        {
            if (resourceId.Equals(new FixedString32Bytes("gold")))
            {
                stockpile.Gold += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("wood")))
            {
                stockpile.Wood += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("stone")))
            {
                stockpile.Stone += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("iron")))
            {
                stockpile.Iron += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("food")))
            {
                stockpile.Food += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("water")))
            {
                stockpile.Water += amount;
                return;
            }

            if (resourceId.Equals(new FixedString32Bytes("influence")))
            {
                stockpile.Influence += amount;
                return;
            }

            stockpile.Gold += amount;
        }
    }
}
