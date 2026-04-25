using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Resolves disembark orders. For each transport carrying a
    /// <see cref="DisembarkOrderComponent"/>, scans the eight surrounding tiles
    /// for the first non-water tile, then drops every passenger from the
    /// transport's <see cref="PassengerBufferElement"/> onto that tile in a 3x3
    /// position offset grid.
    ///
    /// Browser parity: simulation.js disembarkTransport (~7576-7623). On
    /// success: passenger MoveCommand is cleared (not active) and the embarked
    /// tag + transport link are removed. Buffer is cleared on the transport.
    /// On failure (empty transport or surrounded by water): order is dropped
    /// silently; passengers remain embarked.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DisembarkSystem : ISystem
    {
        private const float OffsetSpacingWorldUnits = 10f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DisembarkOrderComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            float tileSize = ResolveTileSize(entityManager);
            using var waterPatches = ResolveWaterPatches(entityManager, Allocator.Temp);

            var orderQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DisembarkOrderComponent>(),
                ComponentType.ReadOnly<NavalVesselComponent>());

            using var transports = orderQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < transports.Length; i++)
            {
                var transport = transports[i];
                ecb.RemoveComponent<DisembarkOrderComponent>(transport);

                if (!entityManager.HasBuffer<PassengerBufferElement>(transport)) continue;
                if (!entityManager.HasComponent<PositionComponent>(transport)) continue;

                var transportPos = entityManager.GetComponentData<PositionComponent>(transport).Value;
                int tx = (int)math.floor(transportPos.x / math.max(0.0001f, tileSize));
                int ty = (int)math.floor(transportPos.z / math.max(0.0001f, tileSize));

                if (!TryFindAdjacentLandTile(waterPatches, tx, ty, out int landTileX, out int landTileY))
                {
                    continue;
                }

                float landWorldX = (landTileX + 0.5f) * tileSize;
                float landWorldZ = (landTileY + 0.5f) * tileSize;

                // Snapshot passenger entities and clear the transport buffer in one pass.
                // Both the snapshot read and the buffer Clear happen before any ecb-queued
                // structural changes, so the buffer handle remains valid throughout.
                int passengerCount;
                Entity[] passengers;
                {
                    var buf = entityManager.GetBuffer<PassengerBufferElement>(transport);
                    passengerCount = buf.Length;
                    if (passengerCount == 0) continue;
                    passengers = new Entity[passengerCount];
                    for (int p = 0; p < passengerCount; p++) passengers[p] = buf[p].Passenger;
                    buf.Clear();
                }

                for (int p = 0; p < passengerCount; p++)
                {
                    var passenger = passengers[p];
                    if (passenger == Entity.Null || !entityManager.Exists(passenger)) continue;

                    float offsetX = ((p % 3) - 1) * OffsetSpacingWorldUnits;
                    float offsetZ = (math.floor(p / 3f) - 1f) * OffsetSpacingWorldUnits;
                    float3 dropPos = new float3(landWorldX + offsetX, transportPos.y, landWorldZ + offsetZ);

                    if (entityManager.HasComponent<PositionComponent>(passenger))
                    {
                        ecb.SetComponent(passenger, new PositionComponent { Value = dropPos });
                    }
                    if (entityManager.HasComponent<LocalTransform>(passenger))
                    {
                        var lt = entityManager.GetComponentData<LocalTransform>(passenger);
                        lt.Position = dropPos;
                        ecb.SetComponent(passenger, lt);
                    }
                    if (entityManager.HasComponent<MoveCommandComponent>(passenger))
                    {
                        var move = entityManager.GetComponentData<MoveCommandComponent>(passenger);
                        move.Destination = dropPos;
                        move.IsActive = false;
                        ecb.SetComponent(passenger, move);
                    }

                    if (entityManager.HasComponent<EmbarkedPassengerTag>(passenger))
                        ecb.RemoveComponent<EmbarkedPassengerTag>(passenger);
                    if (entityManager.HasComponent<PassengerTransportLinkComponent>(passenger))
                        ecb.RemoveComponent<PassengerTransportLinkComponent>(passenger);
                }
            }
        }

        private static bool TryFindAdjacentLandTile(
            NativeArray<MapWaterTilePatchSeedElement> waterPatches,
            int tx, int ty, out int landTileX, out int landTileY)
        {
            // Browser parity: scan dx in [-1, 1], dy in [-1, 1] excluding (0,0); first
            // non-water tile wins. Iteration order matches the browser ring scan.
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int candX = tx + dx;
                    int candY = ty + dy;
                    if (!IsWaterTile(waterPatches, candX, candY))
                    {
                        landTileX = candX;
                        landTileY = candY;
                        return true;
                    }
                }
            }
            landTileX = 0;
            landTileY = 0;
            return false;
        }

        private static bool IsWaterTile(NativeArray<MapWaterTilePatchSeedElement> patches, int tileX, int tileY)
        {
            for (int i = 0; i < patches.Length; i++)
            {
                var p = patches[i];
                if (tileX >= p.X && tileX < p.X + p.Width &&
                    tileY >= p.Y && tileY < p.Y + p.Height)
                {
                    return true;
                }
            }
            return false;
        }

        private static NativeArray<MapWaterTilePatchSeedElement> ResolveWaterPatches(EntityManager em, Allocator allocator)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<MapWaterTilePatchSeedElement>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            int total = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (em.HasBuffer<MapWaterTilePatchSeedElement>(entities[i]))
                    total += em.GetBuffer<MapWaterTilePatchSeedElement>(entities[i]).Length;
            }
            var result = new NativeArray<MapWaterTilePatchSeedElement>(total, allocator);
            int idx = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!em.HasBuffer<MapWaterTilePatchSeedElement>(entities[i])) continue;
                var buf = em.GetBuffer<MapWaterTilePatchSeedElement>(entities[i]);
                for (int j = 0; j < buf.Length; j++)
                {
                    result[idx++] = buf[j];
                }
            }
            query.Dispose();
            return result;
        }

        private static float ResolveTileSize(EntityManager em)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<MapBootstrapConfigComponent>());
            if (query.CalculateEntityCount() == 0)
            {
                query.Dispose();
                return NavalCanon.ValidationDefaultTileSize;
            }
            using var configs = query.ToComponentDataArray<MapBootstrapConfigComponent>(Allocator.Temp);
            float ts = configs[0].TileSize;
            query.Dispose();
            return ts > 0f ? ts : NavalCanon.ValidationDefaultTileSize;
        }
    }
}
