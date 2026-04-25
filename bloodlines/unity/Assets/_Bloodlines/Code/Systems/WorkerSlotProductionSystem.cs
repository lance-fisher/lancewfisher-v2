using Bloodlines.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Produces resources from slot-based buildings (Woodcutter Camp, Forager Camp,
    /// Small Farm) scaled by assigned workers and faction effective productivity.
    ///
    /// This is NOT the trickle model. Slot-based buildings produce:
    ///   output = BaseRatePerWorker × AssignedWorkers × EffectiveProductivity × dt
    ///
    /// The ResourceTrickleBuildingSystem continues to handle passive-trickle buildings
    /// (well → water, farm → food). These two production models are complementary.
    ///
    /// Requires: PopulationProductivitySystem runs first (for EffectiveProductivity).
    ///
    /// Canon: RESOURCE_SYSTEM.md "Production scaling by population allocation".
    ///        early-game-foundation prompt 2026-04-25 worker-slot model.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PopulationProductivitySystem))]
    public partial struct WorkerSlotProductionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            // Build faction productivity lookup (factionId hash → effective productivity).
            using var productivityMap = new NativeHashMap<int, float>(16, Allocator.Temp);

            foreach (var (faction, productivity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<PopulationProductivityComponent>>())
            {
                int pKey = faction.ValueRO.FactionId.GetHashCode();
                productivityMap.Remove(pKey);
                productivityMap.Add(pKey, productivity.ValueRO.EffectiveProductivity);
            }

            // Build faction stockpile entity map (factionId hash → Entity).
            // We write to the ResourceStockpileComponent via EntityManager to avoid
            // structural changes; use a separate pass.
            using var stockpileMap = new NativeHashMap<int, Entity>(16, Allocator.Temp);

            foreach (var (faction, _, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<ResourceStockpileComponent>>()
                    .WithEntityAccess())
            {
                int sKey = faction.ValueRO.FactionId.GetHashCode();
                stockpileMap.Remove(sKey);
                stockpileMap.Add(sKey, entity);
            }

            // Accumulate output per faction.
            using var foodDelta = new NativeHashMap<int, float>(16, Allocator.Temp);
            using var woodDelta = new NativeHashMap<int, float>(16, Allocator.Temp);

            foreach (var (workerSlotRw, faction) in
                SystemAPI.Query<RefRW<WorkerSlotBuildingComponent>, RefRO<FactionComponent>>())
            {
                ref var slot = ref workerSlotRw.ValueRW;
                if (slot.AssignedWorkers <= 0) continue;

                int key = faction.ValueRO.FactionId.GetHashCode();
                productivityMap.TryGetValue(key, out float productivity);

                slot.FillRatio = slot.MaxWorkerSlots > 0
                    ? (float)slot.AssignedWorkers / slot.MaxWorkerSlots
                    : 0f;

                float effectiveWorkers = slot.AssignedWorkers * productivity;

                if (slot.FoodOutputPerWorkerPerSecond > 0f)
                {
                    foodDelta.TryGetValue(key, out float f);
                    foodDelta.Remove(key);
                    foodDelta.Add(key, f + slot.FoodOutputPerWorkerPerSecond * effectiveWorkers * dt);
                }

                if (slot.WoodOutputPerWorkerPerSecond > 0f)
                {
                    woodDelta.TryGetValue(key, out float w);
                    woodDelta.Remove(key);
                    woodDelta.Add(key, w + slot.WoodOutputPerWorkerPerSecond * effectiveWorkers * dt);
                }
            }

            // Apply accumulated deltas to faction stockpiles.
            foreach (var (stockpileRw, faction) in
                SystemAPI.Query<RefRW<ResourceStockpileComponent>, RefRO<FactionComponent>>())
            {
                int key = faction.ValueRO.FactionId.GetHashCode();
                if (foodDelta.TryGetValue(key, out float food))
                    stockpileRw.ValueRW.Food += food;
                if (woodDelta.TryGetValue(key, out float wood))
                    stockpileRw.ValueRW.Wood += wood;
            }
        }
    }
}
