using Bloodlines.Components;
using Bloodlines.Pathing;
using Bloodlines.Siege;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Raids
{
    /// <summary>
    /// Resolves explicit scout raid commands against buildings and moving
    /// logistics carriers.
    ///
    /// This ports the direct execution branch from `simulation.js` without
    /// widening the AI strategic layer. Dispatch remains a separate concern;
    /// this system only validates a live raid order, applies the mechanical
    /// consequences, then pushes the raider onto a retreat move.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UnitMovementSystem))]
    [UpdateBefore(typeof(WorkerGatherSystem))]
    [UpdateBefore(typeof(ResourceTrickleBuildingSystem))]
    [UpdateBefore(typeof(SiegeSupportRefreshSystem))]
    [UpdateBefore(typeof(FieldWaterSupportScanSystem))]
    public partial struct ScoutRaidResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScoutRaidCommandComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            double elapsed = SystemAPI.Time.ElapsedTime;
            float maxWorldX = -1f;
            float maxWorldZ = -1f;
            if (SystemAPI.TryGetSingleton<MapBootstrapConfigComponent>(out var mapConfig))
            {
                float tileSize = math.max(1f, mapConfig.TileSize);
                maxWorldX = math.max(0f, mapConfig.Width * tileSize);
                maxWorldZ = math.max(0f, mapConfig.Height * tileSize);
            }

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (raidCommand, unitType, faction, position, moveCommand, entity) in
                SystemAPI.Query<
                    RefRO<ScoutRaidCommandComponent>,
                    RefRO<UnitTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRW<MoveCommandComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (!ScoutRaidCanon.TryGetRaiderProfile(unitType.ValueRO, out var raiderProfile))
                {
                    ecb.RemoveComponent<ScoutRaidCommandComponent>(entity);
                    continue;
                }

                if (!entityManager.Exists(raidCommand.ValueRO.TargetEntity))
                {
                    ecb.RemoveComponent<ScoutRaidCommandComponent>(entity);
                    continue;
                }

                switch (raidCommand.ValueRO.TargetKind)
                {
                    case ScoutRaidTargetKind.Building:
                        ResolveBuildingRaid(
                            entityManager,
                            elapsed,
                            entity,
                            faction.ValueRO.FactionId,
                            position.ValueRO.Value,
                            ref moveCommand.ValueRW,
                            raidCommand.ValueRO.TargetEntity,
                            in raiderProfile,
                            maxWorldX,
                            maxWorldZ,
                            ecb);
                        break;

                    case ScoutRaidTargetKind.LogisticsCarrier:
                        ResolveLogisticsInterdiction(
                            entityManager,
                            elapsed,
                            entity,
                            faction.ValueRO.FactionId,
                            position.ValueRO.Value,
                            ref moveCommand.ValueRW,
                            raidCommand.ValueRO.TargetEntity,
                            in raiderProfile,
                            maxWorldX,
                            maxWorldZ,
                            ecb);
                        break;

                    default:
                        ecb.RemoveComponent<ScoutRaidCommandComponent>(entity);
                        break;
                }
            }
        }

        private static void ResolveBuildingRaid(
            EntityManager entityManager,
            double elapsed,
            Entity raiderEntity,
            FixedString32Bytes raiderFactionId,
            float3 raiderPosition,
            ref MoveCommandComponent raiderMoveCommand,
            Entity targetEntity,
            in ScoutRaiderProfile raiderProfile,
            float maxWorldX,
            float maxWorldZ,
            EntityCommandBuffer ecb)
        {
            if (!entityManager.HasComponent<BuildingTypeComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity) ||
                !entityManager.HasComponent<PositionComponent>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetHealth = entityManager.GetComponentData<HealthComponent>(targetEntity);
            if (targetHealth.Current <= 0f || entityManager.HasComponent<ConstructionStateComponent>(targetEntity))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetBuildingType = entityManager.GetComponentData<BuildingTypeComponent>(targetEntity);
            if (!ScoutRaidCanon.TryGetBuildingRaidProfile(targetBuildingType.TypeId, out var raidImpact))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetFaction = entityManager.GetComponentData<FactionComponent>(targetEntity).FactionId;
            if (!IsFactionHostile(entityManager, raiderFactionId, targetFaction))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            float3 targetPosition = entityManager.GetComponentData<PositionComponent>(targetEntity).Value;
            if (!AdvanceTowardTarget(ref raiderMoveCommand, raiderPosition, targetPosition))
            {
                return;
            }

            if (!entityManager.HasComponent<BuildingRaidStateComponent>(targetEntity))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var raidState = entityManager.GetComponentData<BuildingRaidStateComponent>(targetEntity);
            raidState.RaidedUntil = math.max(
                raidState.RaidedUntil,
                elapsed + math.max(8f, raiderProfile.RaidDurationSeconds));
            raidState.LastRaidedAt = elapsed;
            raidState.RaidedByFactionId = raiderFactionId;
            entityManager.SetComponentData(targetEntity, raidState);

            ApplyResourceLossToFaction(entityManager, targetFaction, in raidImpact);
            ApplyLocalLoyaltyShock(
                entityManager,
                targetFaction,
                targetPosition,
                raiderProfile.RaidLoyaltyDamage);
            ClearAttackIntent(entityManager, raiderEntity);
            raiderMoveCommand = CreateRetreatCommand(raiderPosition, targetPosition, maxWorldX, maxWorldZ);
            ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
        }

        private static void ResolveLogisticsInterdiction(
            EntityManager entityManager,
            double elapsed,
            Entity raiderEntity,
            FixedString32Bytes raiderFactionId,
            float3 raiderPosition,
            ref MoveCommandComponent raiderMoveCommand,
            Entity targetEntity,
            in ScoutRaiderProfile raiderProfile,
            float maxWorldX,
            float maxWorldZ,
            EntityCommandBuffer ecb)
        {
            if (!entityManager.HasComponent<UnitTypeComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity) ||
                !entityManager.HasComponent<PositionComponent>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity) ||
                !entityManager.HasComponent<SiegeSupplyTrainComponent>(targetEntity) ||
                !entityManager.HasComponent<MoveCommandComponent>(targetEntity))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetHealth = entityManager.GetComponentData<HealthComponent>(targetEntity);
            if (targetHealth.Current <= 0f)
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetUnitType = entityManager.GetComponentData<UnitTypeComponent>(targetEntity);
            if (!ScoutRaidCanon.TryGetLogisticsCarrierProfile(targetUnitType.TypeId, out var logisticsProfile))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            var targetFaction = entityManager.GetComponentData<FactionComponent>(targetEntity).FactionId;
            if (!IsFactionHostile(entityManager, raiderFactionId, targetFaction))
            {
                ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
                return;
            }

            float3 targetPosition = entityManager.GetComponentData<PositionComponent>(targetEntity).Value;
            if (!AdvanceTowardTarget(ref raiderMoveCommand, raiderPosition, targetPosition))
            {
                return;
            }

            var supplyTrain = entityManager.GetComponentData<SiegeSupplyTrainComponent>(targetEntity);
            float interdictionDuration = ScoutRaidCanon.ResolveInterdictionDuration(
                raiderProfile,
                logisticsProfile);

            supplyTrain.LogisticsInterdictedUntil = math.max(
                supplyTrain.LogisticsInterdictedUntil,
                elapsed + interdictionDuration);
            supplyTrain.ConvoyRecoveryUntil = math.max(
                supplyTrain.ConvoyRecoveryUntil,
                supplyTrain.LogisticsInterdictedUntil + ScoutRaidCanon.ConvoyRecoveryDurationSeconds);
            supplyTrain.InterdictedByFactionId = raiderFactionId;
            entityManager.SetComponentData(targetEntity, supplyTrain);

            if (entityManager.HasComponent<SiegeSupportComponent>(targetEntity))
            {
                var support = entityManager.GetComponentData<SiegeSupportComponent>(targetEntity);
                support.HasLinkedSupplyCamp = false;
                support.HasSupplyTrainSupport = false;
                support.Status = SiegeSupportStatus.Interdicted;
                entityManager.SetComponentData(targetEntity, support);
            }

            var targetMove = entityManager.GetComponentData<MoveCommandComponent>(targetEntity);
            targetMove.Destination = ResolveWagonRetreatPosition(entityManager, targetEntity, targetPosition, elapsed);
            targetMove.StoppingDistance = 0.6f;
            targetMove.IsActive = true;
            entityManager.SetComponentData(targetEntity, targetMove);

            ApplyResourceLossToFaction(entityManager, targetFaction, logisticsProfile.ResourceLoss);
            ApplyLocalLoyaltyShock(
                entityManager,
                targetFaction,
                targetPosition,
                math.max(3f, math.round(raiderProfile.RaidLoyaltyDamage * 0.5f)));
            ClearAttackIntent(entityManager, raiderEntity);
            raiderMoveCommand = CreateRetreatCommand(raiderPosition, targetPosition, maxWorldX, maxWorldZ);
            ecb.RemoveComponent<ScoutRaidCommandComponent>(raiderEntity);
        }

        private static bool AdvanceTowardTarget(
            ref MoveCommandComponent moveCommand,
            float3 attackerPosition,
            float3 targetPosition)
        {
            if (math.distance(attackerPosition.xz, targetPosition.xz) > ScoutRaidCanon.TargetRange)
            {
                moveCommand.Destination = targetPosition;
                moveCommand.StoppingDistance = ScoutRaidCanon.TargetRange;
                moveCommand.IsActive = true;
                return false;
            }

            return true;
        }

        private static MoveCommandComponent CreateRetreatCommand(
            float3 raiderPosition,
            float3 targetPosition,
            float maxWorldX,
            float maxWorldZ)
        {
            float3 delta = raiderPosition - targetPosition;
            float length = math.length(delta.xz);
            if (length <= 0.001f)
            {
                delta = new float3(ScoutRaidCanon.RetreatDistance, 0f, 0f);
                length = ScoutRaidCanon.RetreatDistance;
            }

            float2 retreat2D = raiderPosition.xz + ((delta.xz / math.max(0.001f, length)) * ScoutRaidCanon.RetreatDistance);
            if (maxWorldX > 0f && maxWorldZ > 0f)
            {
                retreat2D.x = math.clamp(retreat2D.x, 0f, maxWorldX);
                retreat2D.y = math.clamp(retreat2D.y, 0f, maxWorldZ);
            }

            return new MoveCommandComponent
            {
                Destination = new float3(retreat2D.x, raiderPosition.y, retreat2D.y),
                StoppingDistance = 0.6f,
                IsActive = true,
            };
        }

        private static void ApplyResourceLossToFaction(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            in RaidImpactProfile loss)
        {
            if (!loss.HasAnyLoss())
            {
                return;
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
                ScoutRaidCanon.ApplyResourceLoss(ref stockpile, loss);
                entityManager.SetComponentData(entities[i], stockpile);
                return;
            }
        }

        private static void ApplyLocalLoyaltyShock(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            float3 targetPosition,
            float loyaltyDamage)
        {
            if (factionId.Length == 0 || loyaltyDamage <= 0f)
            {
                return;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            int bestIndex = -1;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                float distance = math.distance(targetPosition.xz, positions[i].Value.xz);
                if (distance > ScoutRaidCanon.LoyaltyRadius || distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                return;
            }

            var controlPoint = controlPoints[bestIndex];
            controlPoint.Loyalty = math.clamp(controlPoint.Loyalty - loyaltyDamage, 0f, 100f);
            if (controlPoint.ControlState == ControlState.Stabilized &&
                controlPoint.Loyalty < ScoutRaidCanon.StabilizedLoyaltyThreshold)
            {
                controlPoint.ControlState = ControlState.Occupied;
            }

            entityManager.SetComponentData(entities[bestIndex], controlPoint);
        }

        private static float3 ResolveWagonRetreatPosition(
            EntityManager entityManager,
            Entity wagonEntity,
            float3 threatPosition,
            double elapsed)
        {
            var wagonFaction = entityManager.GetComponentData<FactionComponent>(wagonEntity).FactionId;
            var wagonPosition = entityManager.GetComponentData<PositionComponent>(wagonEntity).Value;

            var campQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<SiegeSupplyCampComponent>());

            using var campEntities = campQuery.ToEntityArray(Allocator.Temp);
            using var campTypes = campQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var campFactions = campQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var campPositions = campQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var campHealth = campQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var campStates = campQuery.ToComponentDataArray<SiegeSupplyCampComponent>(Allocator.Temp);

            float bestDistance = float.MaxValue;
            float3 bestPosition = CreateRetreatCommand(wagonPosition, threatPosition, -1f, -1f).Destination;

            for (int i = 0; i < campEntities.Length; i++)
            {
                if (campHealth[i].Current <= 0f ||
                    !campFactions[i].FactionId.Equals(wagonFaction) ||
                    !campTypes[i].SupportsSiegeLogistics)
                {
                    continue;
                }

                if (entityManager.HasComponent<BuildingRaidStateComponent>(campEntities[i]))
                {
                    var raidState = entityManager.GetComponentData<BuildingRaidStateComponent>(campEntities[i]);
                    if (ScoutRaidCanon.IsBuildingRaided(raidState, elapsed))
                    {
                        continue;
                    }
                }

                if (!SiegeSupplyInterdictionCanon.IsCampOperational(campStates[i]))
                {
                    continue;
                }

                float distance = math.distance(wagonPosition.xz, campPositions[i].Value.xz);
                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestPosition = campPositions[i].Value;
            }

            return bestPosition;
        }

        private static bool IsFactionHostile(
            EntityManager entityManager,
            FixedString32Bytes attackerFactionId,
            FixedString32Bytes targetFactionId)
        {
            if (attackerFactionId.Length == 0 ||
                targetFactionId.Length == 0 ||
                attackerFactionId.Equals(targetFactionId))
            {
                return false;
            }

            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HostilityComponent>());

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factionIds[i].FactionId.Equals(attackerFactionId))
                {
                    continue;
                }

                var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntities[i]);
                for (int j = 0; j < hostilityBuffer.Length; j++)
                {
                    if (hostilityBuffer[j].HostileFactionId.Equals(targetFactionId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void ClearAttackIntent(EntityManager entityManager, Entity unitEntity)
        {
            if (entityManager.HasComponent<AttackTargetComponent>(unitEntity))
            {
                entityManager.SetComponentData(unitEntity, new AttackTargetComponent
                {
                    TargetEntity = Entity.Null,
                    EngagementRange = 0f,
                });
            }
        }
    }
}
