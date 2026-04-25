using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Resolves embark orders into transport-passenger links. Reads candidates
    /// carrying <see cref="EmbarkOrderComponent"/>, validates faction parity,
    /// adjacency, capacity, and that the candidate is itself a land unit, then
    /// either commits the embarkation (passenger added to the transport buffer
    /// and tagged inactive) or silently drops the order.
    ///
    /// Browser runtime equivalent: simulation.js `embarkUnitsOnTransport`
    /// (line 7539). Capacity reject and faction-mismatch reject are silent in
    /// the browser; this system mirrors that behavior. Cross-vessel embarkation
    /// is rejected: vessels do not board other vessels.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EmbarkSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EmbarkOrderComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var orderQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<EmbarkOrderComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var passengerEntities = orderQuery.ToEntityArray(Allocator.Temp);
            using var passengerOrders = orderQuery.ToComponentDataArray<EmbarkOrderComponent>(Allocator.Temp);
            using var passengerFactions = orderQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var passengerPositions = orderQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var passengerHealth = orderQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            float tileSize = ResolveTileSize(entityManager);
            float embarkRadius = NavalCanon.EmbarkRadiusTileMultiplier * tileSize;
            float embarkRadiusSq = embarkRadius * embarkRadius;

            // ECB writes are deferred to playback; track in-flight admissions per
            // transport so a single OnUpdate cannot overshoot a vessel's capacity
            // when multiple orders fire on the same tick.
            var pendingAdmissions = new Dictionary<Entity, int>();

            for (int i = 0; i < passengerEntities.Length; i++)
            {
                var passengerEntity = passengerEntities[i];
                var order = passengerOrders[i];

                ecb.RemoveComponent<EmbarkOrderComponent>(passengerEntity);

                if (order.TargetTransport == Entity.Null ||
                    !entityManager.Exists(order.TargetTransport))
                {
                    continue;
                }

                if (entityManager.HasComponent<DeadTag>(passengerEntity) ||
                    passengerHealth[i].Current <= 0f)
                {
                    continue;
                }

                if (entityManager.HasComponent<EmbarkedPassengerTag>(passengerEntity) ||
                    entityManager.HasComponent<PassengerTransportLinkComponent>(passengerEntity))
                {
                    continue;
                }

                if (entityManager.HasComponent<NavalVesselComponent>(passengerEntity))
                {
                    continue;
                }

                if (!entityManager.HasComponent<NavalVesselComponent>(order.TargetTransport) ||
                    !entityManager.HasComponent<FactionComponent>(order.TargetTransport) ||
                    !entityManager.HasComponent<PositionComponent>(order.TargetTransport) ||
                    !entityManager.HasComponent<HealthComponent>(order.TargetTransport))
                {
                    continue;
                }

                if (entityManager.HasComponent<DeadTag>(order.TargetTransport))
                {
                    continue;
                }

                var transportFaction = entityManager.GetComponentData<FactionComponent>(order.TargetTransport);
                if (!transportFaction.FactionId.Equals(passengerFactions[i].FactionId))
                {
                    continue;
                }

                var transportHealth = entityManager.GetComponentData<HealthComponent>(order.TargetTransport);
                if (transportHealth.Current <= 0f)
                {
                    continue;
                }

                var transportVessel = entityManager.GetComponentData<NavalVesselComponent>(order.TargetTransport);
                if (transportVessel.TransportCapacity <= 0)
                {
                    continue;
                }

                var transportPosition = entityManager.GetComponentData<PositionComponent>(order.TargetTransport);
                float distanceSq = math.distancesq(passengerPositions[i].Value, transportPosition.Value);
                if (distanceSq > embarkRadiusSq)
                {
                    continue;
                }

                if (!entityManager.HasBuffer<PassengerBufferElement>(order.TargetTransport))
                {
                    ecb.AddBuffer<PassengerBufferElement>(order.TargetTransport);
                }

                var passengerBuffer = entityManager.HasBuffer<PassengerBufferElement>(order.TargetTransport)
                    ? entityManager.GetBuffer<PassengerBufferElement>(order.TargetTransport)
                    : default;

                int currentCount = passengerBuffer.IsCreated ? passengerBuffer.Length : 0;
                int pendingForTransport = pendingAdmissions.TryGetValue(order.TargetTransport, out int alreadyAdmitted)
                    ? alreadyAdmitted
                    : 0;
                if (currentCount + pendingForTransport >= transportVessel.TransportCapacity)
                {
                    continue;
                }

                FixedString64Bytes passengerTypeId = default;
                if (entityManager.HasComponent<UnitTypeComponent>(passengerEntity))
                {
                    passengerTypeId = entityManager.GetComponentData<UnitTypeComponent>(passengerEntity).TypeId;
                }

                ecb.AppendToBuffer(order.TargetTransport, new PassengerBufferElement
                {
                    Passenger = passengerEntity,
                    PassengerTypeId = passengerTypeId,
                });

                pendingAdmissions[order.TargetTransport] = pendingForTransport + 1;

                ecb.AddComponent(passengerEntity, new EmbarkedPassengerTag());
                ecb.AddComponent(passengerEntity, new PassengerTransportLinkComponent
                {
                    Transport = order.TargetTransport,
                });

                if (entityManager.HasComponent<MoveCommandComponent>(passengerEntity))
                {
                    var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(passengerEntity);
                    moveCommand.IsActive = false;
                    ecb.SetComponent(passengerEntity, moveCommand);
                }

                if (entityManager.HasComponent<WorkerGatherOrderComponent>(passengerEntity))
                {
                    ecb.RemoveComponent<WorkerGatherOrderComponent>(passengerEntity);
                }
            }
        }

        private static float ResolveTileSize(EntityManager entityManager)
        {
            var configQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MapBootstrapConfigComponent>());
            if (configQuery.CalculateEntityCount() == 0)
            {
                return NavalCanon.ValidationDefaultTileSize;
            }

            using var configs = configQuery.ToComponentDataArray<MapBootstrapConfigComponent>(Allocator.Temp);
            float tileSize = configs[0].TileSize;
            return tileSize > 0f ? tileSize : NavalCanon.ValidationDefaultTileSize;
        }
    }
}
