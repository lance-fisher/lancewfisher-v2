using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Dispatches idle AI workers to the nearest suitable resource node each
    /// gather interval, porting the idle-worker dispatch loop from ai.js
    /// updateEnemyAi lines 1243-1251.
    ///
    /// Browser equivalents:
    ///   idleWorkers.forEach dispatch loop   (ai.js lines 1243-1251)
    ///   getEnemyGatherPriorities            (ai.js lines 885-922)
    ///   chooseGatherNode                    (ai.js lines 924-933)
    ///
    /// Gather priority order (simplified from browser; full context-aware
    /// version deferred until building-state tracking is comprehensive):
    ///   gold > wood > stone > iron.
    /// Workers are rotated across the priority list so they spread across
    /// resource types (mirrors the browser index-modulo rotation).
    ///
    /// Sets WorkerGatherComponent.Phase = Seeking and writes AssignedNode /
    /// AssignedResourceId. The existing worker-gather execution system carries
    /// the worker from Seeking → Gathering → Returning → Depositing → Idle.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EnemyAIStrategySystem))]
    public partial struct AIWorkerGatherSystem : ISystem
    {
        // Canonical gather priority resource IDs (ai.js gold/wood/stone/iron order).
        private static readonly FixedString32Bytes ResourceGold  = "gold";
        private static readonly FixedString32Bytes ResourceWood  = "wood";
        private static readonly FixedString32Bytes ResourceStone = "stone";
        private static readonly FixedString32Bytes ResourceIron  = "iron";

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
                dt = 0.016f;

            // Snapshot all resource nodes (read-only, amortised).
            var nodeQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            var nodeEntities = nodeQuery.ToEntityArray(Allocator.Temp);
            var nodeComps    = nodeQuery.ToComponentDataArray<ResourceNodeComponent>(Allocator.Temp);
            var nodePos      = nodeQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            nodeQuery.Dispose();

            // Query AI faction strategy components.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factions        = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var economics       = factionQuery.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);
            var strategies      = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // Query all worker units that can be dispatched.
            var workerQuery     = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                typeof(WorkerGatherComponent));
            var workerEntities  = workerQuery.ToEntityArray(Allocator.Temp);
            var workerFactions  = workerQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var workerTypes     = workerQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            var workerHealth    = workerQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            var workerPos       = workerQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var workerGathers   = workerQuery.ToComponentDataArray<WorkerGatherComponent>(Allocator.Temp);
            workerQuery.Dispose();

            for (int fi = 0; fi < factionEntities.Length; fi++)
            {
                if (!economics[fi].Enabled)
                    continue;

                var s = strategies[fi];
                float interval = math.max(0.001f, s.WorkerGatherIntervalSeconds);
                s.WorkerGatherAccumulator += dt;
                if (s.WorkerGatherAccumulator < interval)
                {
                    strategies[fi] = s;
                    em.SetComponentData(factionEntities[fi], s);
                    continue;
                }
                s.WorkerGatherAccumulator = 0f;

                var factionKey = factions[fi].FactionId;

                // Collect idle workers for this faction.
                using var idleWorkers = new NativeList<int>(Allocator.Temp);
                for (int wi = 0; wi < workerEntities.Length; wi++)
                {
                    if (!workerFactions[wi].FactionId.Equals(factionKey))
                        continue;
                    if (workerHealth[wi].Current <= 0f)
                        continue;
                    if (workerTypes[wi].Role != UnitRole.Worker)
                        continue;
                    if (workerGathers[wi].Phase != WorkerGatherPhase.Idle)
                        continue;
                    idleWorkers.Add(wi);
                }

                s.IdleWorkerCount = idleWorkers.Length;

                // Dispatch each idle worker to the nearest suitable node.
                for (int idx = 0; idx < idleWorkers.Length; idx++)
                {
                    int wi = idleWorkers[idx];
                    // Rotate gather priority by worker index (browser: index % priorities.length).
                    FixedString32Bytes chosenResource = ChooseResource(
                        idx, s.PlayerKeepFortified,
                        workerPos[wi].Value, nodeEntities, nodeComps, nodePos,
                        out Entity chosenNode);

                    if (chosenNode == Entity.Null)
                        continue;

                    var gather = workerGathers[wi];
                    gather.Phase              = WorkerGatherPhase.Seeking;
                    gather.AssignedNode       = chosenNode;
                    gather.AssignedResourceId = chosenResource;
                    gather.CarryResourceId    = chosenResource;
                    gather.CarryAmount        = 0f;
                    em.SetComponentData(workerEntities[wi], gather);
                    s.WorkersDispatched++;
                }

                strategies[fi] = s;
                em.SetComponentData(factionEntities[fi], s);
            }

            nodeEntities.Dispose();
            nodeComps.Dispose();
            nodePos.Dispose();
            factionEntities.Dispose();
            factions.Dispose();
            economics.Dispose();
            strategies.Dispose();
            workerEntities.Dispose();
            workerFactions.Dispose();
            workerTypes.Dispose();
            workerHealth.Dispose();
            workerPos.Dispose();
            workerGathers.Dispose();
        }

        // --------------------------------------------------------------------- //
        //  Gather priority selection                                             //
        // --------------------------------------------------------------------- //

        /// <summary>
        /// Chooses a resource type and nearest non-depleted node for the given
        /// worker. Mirrors browser chooseGatherNode + rotated priority list.
        ///
        /// Priority order: gold > wood > stone > iron.
        /// Worker index rotates the starting offset so workers spread across types.
        /// </summary>
        private static FixedString32Bytes ChooseResource(
            int workerIndex,
            bool playerKeepFortified,
            float3 workerPosition,
            NativeArray<Entity> nodeEntities,
            NativeArray<ResourceNodeComponent> nodeComps,
            NativeArray<PositionComponent> nodePos,
            out Entity chosenNode)
        {
            // Build priority rotation (simplified: gold/wood/stone/iron always in order).
            // Full context-aware priorities (building shortfall detection) are deferred;
            // the rotation ensures workers spread across types even without full context.
            const int priorityCount = 4;
            int startOffset = workerIndex % priorityCount;

            for (int p = 0; p < priorityCount; p++)
            {
                int slot = (startOffset + p) % priorityCount;
                FixedString32Bytes resourceId = slot switch
                {
                    0 => ResourceGold,
                    1 => ResourceWood,
                    2 => ResourceStone,
                    _ => ResourceIron,
                };

                if (TryFindNearestNode(resourceId, workerPosition, nodeEntities, nodeComps, nodePos, out chosenNode))
                    return resourceId;
            }

            chosenNode = Entity.Null;
            return ResourceGold;
        }

        private static bool TryFindNearestNode(
            FixedString32Bytes resourceId,
            float3 workerPosition,
            NativeArray<Entity> nodeEntities,
            NativeArray<ResourceNodeComponent> nodeComps,
            NativeArray<PositionComponent> nodePos,
            out Entity nearest)
        {
            nearest       = Entity.Null;
            float bestSq  = float.MaxValue;

            for (int i = 0; i < nodeEntities.Length; i++)
            {
                if (!nodeComps[i].ResourceId.Equals(resourceId))
                    continue;
                if (nodeComps[i].Amount <= 0f)
                    continue;

                float sq = math.distancesq(workerPosition, nodePos[i].Value);
                if (sq < bestSq)
                {
                    bestSq  = sq;
                    nearest = nodeEntities[i];
                }
            }

            return nearest != Entity.Null;
        }
    }
}
