using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Per-tick fishing gather. For each entity carrying both
    /// <see cref="FishingVesselComponent"/> and <see cref="NavalVesselComponent"/>,
    /// if the vessel is idle (no active MoveCommand) and its tile is in a water
    /// patch, increment the owning faction's food stockpile by
    /// FoodPerSecond * dt.
    ///
    /// Browser parity: simulation.js updateVessel (~8782-8801). The browser
    /// fishes only when `unit.command` is null/idle. Unity uses
    /// MoveCommandComponent.IsActive == false as the equivalent of "idle".
    ///
    /// Dead vessels (DeadTag or HealthComponent.Current &lt;= 0) skip.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FishingGatherSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FishingVesselComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f) return;

            var em = state.EntityManager;
            int tileSize = ResolveTileSizeInt(em);
            using var waterPatches = ResolveWaterPatches(em, Allocator.Temp);

            // Snapshot fishing vessel entities to avoid handle invalidation if any
            // other system mutates structurally during this OnUpdate.
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FishingVesselComponent>(),
                ComponentType.ReadOnly<NavalVesselComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (em.HasComponent<DeadTag>(entity)) continue;
                if (em.HasComponent<HealthComponent>(entity) &&
                    em.GetComponentData<HealthComponent>(entity).Current <= 0f) continue;

                // Idle check: either no MoveCommandComponent, or the existing one is inactive.
                if (em.HasComponent<MoveCommandComponent>(entity))
                {
                    var move = em.GetComponentData<MoveCommandComponent>(entity);
                    if (move.IsActive) continue;
                }

                var pos = em.GetComponentData<PositionComponent>(entity).Value;
                int tx = (int)math.floor(pos.x / math.max(0.0001f, tileSize));
                int ty = (int)math.floor(pos.z / math.max(0.0001f, tileSize));
                if (!IsWaterTile(waterPatches, tx, ty)) continue;

                var fishing = em.GetComponentData<FishingVesselComponent>(entity);
                float yieldAmount = math.max(0f, fishing.FoodPerSecond) * dt;
                if (yieldAmount <= 0f) continue;

                var faction = em.GetComponentData<FactionComponent>(entity);
                AddFoodToFaction(em, faction.FactionId, yieldAmount);
            }

            query.Dispose();
        }

        private static void AddFoodToFaction(EntityManager em, FixedString32Bytes factionId, float amount)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(ResourceStockpileComponent));
            using var factionEntities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId)) continue;
                var s = stockpiles[i];
                s.Food += amount;
                em.SetComponentData(factionEntities[i], s);
                break;
            }
            query.Dispose();
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

        private static int ResolveTileSizeInt(EntityManager em)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<MapBootstrapConfigComponent>());
            if (query.CalculateEntityCount() == 0)
            {
                query.Dispose();
                return 1;
            }
            using var configs = query.ToComponentDataArray<MapBootstrapConfigComponent>(Allocator.Temp);
            int ts = configs[0].TileSize;
            query.Dispose();
            return ts > 0 ? ts : 1;
        }
    }
}
