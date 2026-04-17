using System;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    /// <summary>
    /// AI economic loop driver. Runs per-frame against every faction entity
    /// that carries an AIEconomyControllerComponent. Assigns idle workers to
    /// gather, queues villagers, and queues militia against the same canonical
    /// rules and costs the player uses. Reuses the existing definition caches
    /// on the debug command surface so that one source of truth covers both
    /// player and AI training.
    ///
    /// Scope for the first AI slice: economic base. Construction, combat
    /// orders, expansion, and diplomacy come in later AI slices.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        private void TickAIFactions(EntityManager entityManager, float dt)
        {
            if (dt <= 0f)
            {
                return;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(AIEconomyControllerComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controllers = query.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var controller = controllers[i];
                if (!controller.Enabled)
                {
                    continue;
                }

                string factionId = factions[i].FactionId.ToString();
                RefreshAIStatsCache(entityManager, factionId, ref controller);

                controller.GatherAssignmentAccumulator += dt;
                controller.ProductionAccumulator += dt;
                controller.ConstructionAccumulator += dt;
                controller.MilitaryPostureAccumulator += dt;

                float gatherInterval = math.max(0.5f, controller.GatherAssignmentIntervalSeconds);
                float productionInterval = math.max(0.5f, controller.ProductionIntervalSeconds);
                float constructionInterval = math.max(0.5f, controller.ConstructionIntervalSeconds);
                float militaryInterval = math.max(0.5f, controller.MilitaryPostureIntervalSeconds);

                if (controller.GatherAssignmentAccumulator >= gatherInterval)
                {
                    controller.GatherAssignmentAccumulator = 0f;
                    AIAssignIdleWorkersToGather(entityManager, factionId, ref controller);
                }

                if (controller.ProductionAccumulator >= productionInterval)
                {
                    controller.ProductionAccumulator = 0f;
                    AIRunProductionPlan(entityManager, factionId, ref controller);
                }

                if (controller.ConstructionAccumulator >= constructionInterval)
                {
                    controller.ConstructionAccumulator = 0f;
                    AIRunConstructionPlan(entityManager, factionId, ref controller);
                }

                if (controller.MilitaryPostureAccumulator >= militaryInterval)
                {
                    controller.MilitaryPostureAccumulator = 0f;
                    AIRunMilitaryPosture(entityManager, factionId, ref controller);
                }

                entityManager.SetComponentData(entities[i], controller);
            }
        }

        private void RefreshAIStatsCache(EntityManager entityManager, string factionId, ref AIEconomyControllerComponent controller)
        {
            var factionKey = new FixedString32Bytes(factionId);
            controller.ControlledWorkerCountCached = 0;
            controller.ControlledMilitiaCountCached = 0;
            controller.IdleWorkerCountCached = 0;
            controller.ProductionQueueCountCached = 0;

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using (var unitEntities = unitQuery.ToEntityArray(Allocator.Temp))
            using (var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp))
            using (var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp))
            using (var unitHealth = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp))
            {
                for (int i = 0; i < unitEntities.Length; i++)
                {
                    if (!unitFactions[i].FactionId.Equals(factionKey) || unitHealth[i].Current <= 0f)
                    {
                        continue;
                    }

                    if (unitTypes[i].Role == UnitRole.Worker)
                    {
                        controller.ControlledWorkerCountCached++;
                        if (!entityManager.HasComponent<WorkerGatherComponent>(unitEntities[i]) ||
                            entityManager.GetComponentData<WorkerGatherComponent>(unitEntities[i]).Phase == WorkerGatherPhase.Idle)
                        {
                            controller.IdleWorkerCountCached++;
                        }
                    }
                    else if (unitTypes[i].TypeId.Equals(new FixedString64Bytes("militia")))
                    {
                        controller.ControlledMilitiaCountCached++;
                    }
                }
            }

            controller.ControlledDwellingCountCached = 0;
            controller.ControlledFarmCountCached = 0;
            controller.ControlledWellCountCached = 0;
            controller.ControlledBarracksCountCached = 0;

            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using (var buildingEntities = buildingQuery.ToEntityArray(Allocator.Temp))
            using (var buildingFactions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp))
            using (var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp))
            using (var buildingHealth = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp))
            {
                for (int i = 0; i < buildingEntities.Length; i++)
                {
                    if (!buildingFactions[i].FactionId.Equals(factionKey) || buildingHealth[i].Current <= 0f)
                    {
                        continue;
                    }

                    if (entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntities[i]))
                    {
                        controller.ProductionQueueCountCached +=
                            entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntities[i]).Length;
                    }

                    var typeId = buildingTypes[i].TypeId;
                    if (typeId.Equals(new FixedString64Bytes("dwelling")))
                    {
                        controller.ControlledDwellingCountCached++;
                    }
                    else if (typeId.Equals(new FixedString64Bytes("farm")))
                    {
                        controller.ControlledFarmCountCached++;
                    }
                    else if (typeId.Equals(new FixedString64Bytes("well")))
                    {
                        controller.ControlledWellCountCached++;
                    }
                    else if (typeId.Equals(new FixedString64Bytes("barracks")))
                    {
                        controller.ControlledBarracksCountCached++;
                    }
                }
            }
        }

        private void AIRunConstructionPlan(EntityManager entityManager, string factionId, ref AIEconomyControllerComponent controller)
        {
            string targetBuildingId = null;

            if (controller.ControlledDwellingCountCached < controller.TargetDwellingCount)
            {
                targetBuildingId = "dwelling";
            }
            else if (controller.ControlledBarracksCountCached < controller.TargetBarracksCount)
            {
                targetBuildingId = "barracks";
            }
            else if (controller.ControlledFarmCountCached < controller.TargetFarmCount)
            {
                targetBuildingId = "farm";
            }
            else if (controller.ControlledWellCountCached < controller.TargetWellCount)
            {
                targetBuildingId = "well";
            }

            if (targetBuildingId == null)
            {
                return;
            }

            if (!TryResolveBuildingDefinition(targetBuildingId, out var buildingDefinition))
            {
                return;
            }

            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var factionSnapshot))
            {
                return;
            }

            if (!CanAffordCost(factionSnapshot.Resources, buildingDefinition.cost))
            {
                return;
            }

            if (!TryFindIdleAIWorker(entityManager, factionId, out Entity workerEntity, out float3 workerPosition))
            {
                return;
            }

            if (!TryGetNearestOwnedCommandHallPosition(entityManager, factionId, out float3 hallPosition))
            {
                return;
            }

            controller.ConstructionPlacementsAttempted++;

            if (TryPickAIBuildPosition(
                entityManager,
                buildingDefinition,
                hallPosition,
                controller.ConstructionPlacementsAttempted,
                out float3 buildPosition))
            {
                if (TryPlaceConstruction(entityManager, workerEntity, targetBuildingId, buildPosition, out _))
                {
                    controller.ConstructionPlacementsSucceeded++;
                }
            }
        }

        private void AIRunMilitaryPosture(EntityManager entityManager, string factionId, ref AIEconomyControllerComponent controller)
        {
            if (controller.ControlledMilitiaCountCached < controller.MilitaryPostureMinimumMilitiaCount)
            {
                return;
            }

            if (!TryFindHostileControlPointAnchor(entityManager, factionId, out float3 anchorPosition))
            {
                return;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var militiaTypeKey = new FixedString64Bytes("militia");

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            float approachRadius = math.max(0.25f, controller.MilitaryPostureApproachRadius);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey) ||
                    !unitTypes[i].TypeId.Equals(militiaTypeKey) ||
                    health[i].Current <= 0f)
                {
                    continue;
                }

                float distanceSq = math.distancesq(positions[i].Value, anchorPosition);
                if (distanceSq <= approachRadius * approachRadius)
                {
                    continue;
                }

                if (!entityManager.HasComponent<MoveCommandComponent>(entities[i]))
                {
                    continue;
                }

                var existing = entityManager.GetComponentData<MoveCommandComponent>(entities[i]);
                if (existing.IsActive &&
                    math.distancesq(existing.Destination, anchorPosition) <= approachRadius * approachRadius)
                {
                    continue;
                }

                entityManager.SetComponentData(entities[i], new MoveCommandComponent
                {
                    Destination = anchorPosition,
                    StoppingDistance = approachRadius,
                    IsActive = true,
                });

                controller.MilitaryPostureOrdersIssued++;
            }
        }

        private static bool TryFindHostileControlPointAnchor(
            EntityManager entityManager,
            string factionId,
            out float3 position)
        {
            position = default;
            var factionKey = new FixedString32Bytes(factionId);

            if (!TryGetNearestOwnedCommandHallPosition(entityManager, factionId, out float3 hallPosition))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            float bestDistanceSq = float.MaxValue;
            bool found = false;
            FixedString32Bytes neutralOwner = default;

            for (int i = 0; i < controlPoints.Length; i++)
            {
                var cp = controlPoints[i];
                if (cp.OwnerFactionId.Equals(factionKey))
                {
                    continue;
                }

                bool isNeutral = cp.OwnerFactionId.Equals(neutralOwner);
                if (!isNeutral)
                {
                    float distanceSq = math.distancesq(hallPosition, positions[i].Value);
                    if (distanceSq < bestDistanceSq)
                    {
                        bestDistanceSq = distanceSq;
                        position = positions[i].Value;
                        found = true;
                    }
                }
            }

            if (found)
            {
                return true;
            }

            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionKey))
                {
                    float distanceSq = math.distancesq(hallPosition, positions[i].Value);
                    if (distanceSq < bestDistanceSq)
                    {
                        bestDistanceSq = distanceSq;
                        position = positions[i].Value;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static bool TryFindIdleAIWorker(
            EntityManager entityManager,
            string factionId,
            out Entity workerEntity,
            out float3 workerPosition)
        {
            workerEntity = Entity.Null;
            workerPosition = default;

            var factionKey = new FixedString32Bytes(factionId);
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            Entity fallbackWorker = Entity.Null;
            float3 fallbackPosition = default;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey) ||
                    unitTypes[i].Role != UnitRole.Worker ||
                    health[i].Current <= 0f)
                {
                    continue;
                }

                if (!entityManager.HasComponent<WorkerGatherComponent>(entities[i]) ||
                    entityManager.GetComponentData<WorkerGatherComponent>(entities[i]).Phase == WorkerGatherPhase.Idle)
                {
                    workerEntity = entities[i];
                    workerPosition = positions[i].Value;
                    return true;
                }

                if (fallbackWorker == Entity.Null)
                {
                    fallbackWorker = entities[i];
                    fallbackPosition = positions[i].Value;
                }
            }

            if (fallbackWorker != Entity.Null)
            {
                workerEntity = fallbackWorker;
                workerPosition = fallbackPosition;
                return true;
            }

            return false;
        }

        private bool TryPickAIBuildPosition(
            EntityManager entityManager,
            BuildingDefinition buildingDefinition,
            float3 hallPosition,
            int attemptIndex,
            out float3 buildPosition)
        {
            const float baseRadius = 4f;
            const int maxRings = 3;
            const int perRing = 8;

            for (int ring = 1; ring <= maxRings; ring++)
            {
                float radius = baseRadius * ring;
                int attemptsInRing = perRing * ring;
                for (int step = 0; step < attemptsInRing; step++)
                {
                    int stepIndex = (step + attemptIndex) % attemptsInRing;
                    float angle = (stepIndex / (float)attemptsInRing) * math.PI * 2f;
                    float x = hallPosition.x + math.cos(angle) * radius;
                    float z = hallPosition.z + math.sin(angle) * radius;
                    var candidate = new float3(x, hallPosition.y, z);

                    if (CanPlaceConstructionAt(entityManager, buildingDefinition, candidate, out _))
                    {
                        buildPosition = candidate;
                        return true;
                    }
                }
            }

            buildPosition = default;
            return false;
        }

        private void AIAssignIdleWorkersToGather(EntityManager entityManager, string factionId, ref AIEconomyControllerComponent controller)
        {
            if (controller.IdleWorkerCountCached <= 0)
            {
                return;
            }

            string resourceId = controller.PrimaryGatherResourceId.Length > 0
                ? controller.PrimaryGatherResourceId.ToString()
                : "gold";

            if (!TryFindNearestResourceNodeForFaction(entityManager, factionId, resourceId, out Entity nodeEntity, out _))
            {
                return;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var resourceKey = new FixedString32Bytes(resourceId);

            var workerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = workerQuery.ToEntityArray(Allocator.Temp);
            using var factions = workerQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = workerQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var health = workerQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey) ||
                    unitTypes[i].Role != UnitRole.Worker ||
                    health[i].Current <= 0f)
                {
                    continue;
                }

                if (entityManager.HasComponent<WorkerGatherComponent>(entities[i]))
                {
                    var gather = entityManager.GetComponentData<WorkerGatherComponent>(entities[i]);
                    if (gather.Phase != WorkerGatherPhase.Idle)
                    {
                        continue;
                    }
                }

                string typeIdString = unitTypes[i].TypeId.ToString();
                if (!TryResolveUnitDefinition(typeIdString, out var unitDefinition))
                {
                    continue;
                }

                float capacity = math.max(1f, unitDefinition.carryCapacity > 0 ? unitDefinition.carryCapacity : 10f);
                float rate = math.max(0.1f, unitDefinition.gatherRate > 0f ? unitDefinition.gatherRate : 1f);

                var newGather = new WorkerGatherComponent
                {
                    AssignedNode = nodeEntity,
                    AssignedResourceId = resourceKey,
                    CarryResourceId = default,
                    CarryAmount = 0f,
                    CarryCapacity = capacity,
                    GatherRate = rate,
                    Phase = WorkerGatherPhase.Seeking,
                    GatherRadius = 1.25f,
                    DepositRadius = 1.75f,
                };

                if (entityManager.HasComponent<WorkerGatherComponent>(entities[i]))
                {
                    entityManager.SetComponentData(entities[i], newGather);
                }
                else
                {
                    entityManager.AddComponentData(entities[i], newGather);
                }
            }
        }

        private void AIRunProductionPlan(EntityManager entityManager, string factionId, ref AIEconomyControllerComponent controller)
        {
            bool wantsVillager = controller.ControlledWorkerCountCached < controller.TargetWorkerCount;
            bool wantsMilitia =
                !wantsVillager &&
                controller.ControlledWorkerCountCached >= controller.TargetWorkerCount &&
                controller.ControlledMilitiaCountCached < controller.TargetMilitiaCount;

            if (!wantsVillager && !wantsMilitia)
            {
                return;
            }

            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var factionSnapshot))
            {
                return;
            }

            if (wantsVillager)
            {
                AITryQueueUnitAtBuilding(entityManager, "command_hall", "villager", in factionSnapshot);
            }
            else if (wantsMilitia)
            {
                AITryQueueUnitAtBuilding(entityManager, "barracks", "militia", in factionSnapshot);
            }
        }

        private bool AITryQueueUnitAtBuilding(
            EntityManager entityManager,
            string buildingTypeId,
            string unitId,
            in FactionRuntimeSnapshot factionSnapshot)
        {
            if (!TryResolveUnitDefinition(unitId, out var unitDefinition))
            {
                return false;
            }

            if (!CanAffordCost(factionSnapshot.Resources, unitDefinition.cost))
            {
                return false;
            }

            int requiredPop = unitDefinition.populationCost;
            if (factionSnapshot.Population.Available < requiredPop)
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionSnapshot.FactionId);
            var buildingKey = new FixedString64Bytes(buildingTypeId);
            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = buildingQuery.ToEntityArray(Allocator.Temp);
            using var factions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var health = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey) ||
                    !buildingTypes[i].TypeId.Equals(buildingKey) ||
                    health[i].Current <= 0f)
                {
                    continue;
                }

                if (entityManager.HasComponent<ConstructionStateComponent>(entities[i]))
                {
                    continue;
                }

                if (!entityManager.HasBuffer<ProductionQueueItemElement>(entities[i]))
                {
                    continue;
                }

                var queue = entityManager.GetBuffer<ProductionQueueItemElement>(entities[i]);
                if (queue.Length >= 1)
                {
                    return false;
                }

                var queueItem = BuildAIQueueItem(unitId, unitDefinition);
                queue.Add(queueItem);

                var resources = factionSnapshot.Resources;
                SpendCost(ref resources, unitDefinition.cost);
                entityManager.SetComponentData(factionSnapshot.Entity, resources);

                var population = factionSnapshot.Population;
                population.Available = math.max(0, population.Available - requiredPop);
                entityManager.SetComponentData(factionSnapshot.Entity, population);

                return true;
            }

            return false;
        }

        private static ProductionQueueItemElement BuildAIQueueItem(string unitId, UnitDefinition unitDefinition)
        {
            float durationSeconds = GetProductionDurationSeconds(unitDefinition);
            UnitRole role = ResolveUnitRole(unitDefinition.role);
            SiegeClass siegeClass = ResolveSiegeClass(unitDefinition.siegeClass);

            return new ProductionQueueItemElement
            {
                UnitId = new FixedString64Bytes(unitId),
                DisplayName = new FixedString64Bytes(string.IsNullOrWhiteSpace(unitDefinition.displayName) ? unitId : unitDefinition.displayName),
                RemainingSeconds = durationSeconds,
                TotalSeconds = durationSeconds,
                PopulationCost = unitDefinition.populationCost,
                BloodPrice = 0,
                BloodLoadDelta = 0f,
                MaxHealth = unitDefinition.health,
                MaxSpeed = unitDefinition.speed,
                Role = role,
                SiegeClass = siegeClass,
                Stage = unitDefinition.stage,
                GoldCost = unitDefinition.cost?.gold ?? 0,
                FoodCost = unitDefinition.cost?.food ?? 0,
                WaterCost = unitDefinition.cost?.water ?? 0,
                WoodCost = unitDefinition.cost?.wood ?? 0,
                StoneCost = unitDefinition.cost?.stone ?? 0,
                IronCost = unitDefinition.cost?.iron ?? 0,
                InfluenceCost = unitDefinition.cost?.influence ?? 0,
            };
        }

        private static bool TryFindNearestResourceNodeForFaction(
            EntityManager entityManager,
            string factionId,
            string resourceId,
            out Entity nodeEntity,
            out float3 nodePosition)
        {
            nodeEntity = Entity.Null;
            nodePosition = default;

            if (!TryGetNearestOwnedCommandHallPosition(entityManager, factionId, out float3 reference))
            {
                return false;
            }

            var resourceKey = new FixedString32Bytes(resourceId);

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var nodes = query.ToComponentDataArray<ResourceNodeComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            float bestDistanceSq = float.MaxValue;
            bool found = false;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!nodes[i].ResourceId.Equals(resourceKey) || nodes[i].Amount <= 0f)
                {
                    continue;
                }

                float distanceSq = math.distancesq(reference, positions[i].Value);
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    nodeEntity = entities[i];
                    nodePosition = positions[i].Value;
                    found = true;
                }
            }

            return found;
        }

        private static bool TryGetNearestOwnedCommandHallPosition(
            EntityManager entityManager,
            string factionId,
            out float3 position)
        {
            position = default;
            var factionKey = new FixedString32Bytes(factionId);
            var buildingKey = new FixedString64Bytes("command_hall");

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey) ||
                    !buildingTypes[i].TypeId.Equals(buildingKey) ||
                    health[i].Current <= 0f)
                {
                    continue;
                }

                if (entityManager.HasComponent<ConstructionStateComponent>(entities[i]))
                {
                    continue;
                }

                position = positions[i].Value;
                return true;
            }

            return false;
        }

        public bool TryDebugEnableAIForFaction(string factionId, bool enabled)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(AIEconomyControllerComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controllers = query.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                var controller = controllers[i];
                controller.Enabled = enabled;
                controller.GatherAssignmentAccumulator = 0f;
                controller.ProductionAccumulator = 0f;
                entityManager.SetComponentData(entities[i], controller);
                return true;
            }

            return false;
        }

        public bool TryDebugGetAIBuildingCounts(
            string factionId,
            out int dwellings,
            out int farms,
            out int wells,
            out int barracks)
        {
            dwellings = 0;
            farms = 0;
            wells = 0;
            barracks = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controllers = query.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                dwellings = controllers[i].ControlledDwellingCountCached;
                farms = controllers[i].ControlledFarmCountCached;
                wells = controllers[i].ControlledWellCountCached;
                barracks = controllers[i].ControlledBarracksCountCached;
                return true;
            }

            return false;
        }

        public bool TryDebugGetAIMilitaryOrdersIssued(string factionId, out int ordersIssued)
        {
            ordersIssued = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controllers = query.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                ordersIssued = controllers[i].MilitaryPostureOrdersIssued;
                return true;
            }

            return false;
        }

        public bool TryDebugGetAIEconomyStats(
            string factionId,
            out bool aiEnabled,
            out int controlledWorkerCount,
            out int idleWorkerCount,
            out int controlledMilitiaCount,
            out int productionQueueCount,
            out int targetWorkerCount,
            out int targetMilitiaCount)
        {
            aiEnabled = false;
            controlledWorkerCount = 0;
            idleWorkerCount = 0;
            controlledMilitiaCount = 0;
            productionQueueCount = 0;
            targetWorkerCount = 0;
            targetMilitiaCount = 0;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controllers = query.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                aiEnabled = controllers[i].Enabled;
                controlledWorkerCount = controllers[i].ControlledWorkerCountCached;
                idleWorkerCount = controllers[i].IdleWorkerCountCached;
                controlledMilitiaCount = controllers[i].ControlledMilitiaCountCached;
                productionQueueCount = controllers[i].ProductionQueueCountCached;
                targetWorkerCount = controllers[i].TargetWorkerCount;
                targetMilitiaCount = controllers[i].TargetMilitiaCount;
                return true;
            }

            return false;
        }

        public int CountFactionBuildings(string factionId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            int count = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key) || health[i].Current <= 0f)
                {
                    continue;
                }
                count++;
            }

            return count;
        }

        public int CountFactionUnits(string factionId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            int count = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key) || health[i].Current <= 0f)
                {
                    continue;
                }
                count++;
            }

            return count;
        }
    }
}
