using Bloodlines.Components;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Converts AI gather decisions into concrete worker orders.
    ///
    /// Browser reference: ai.js updateEnemyAi idle-worker dispatch loop,
    /// immediately after chooseGatherNode -> issueGatherCommand.
    ///
    /// The existing AIWorkerGatherSystem chooses the node and writes Seeking onto
    /// WorkerGatherComponent. This bridge adds an explicit WorkerGatherOrderComponent,
    /// issues movement toward the node, and promotes the worker into Gathering so the
    /// same assignment is not re-dispatched every frame.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    [UpdateBefore(typeof(WorkerGatherSystem))]
    public partial struct AIWorkerCommandSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorkerGatherComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var workerQuery = entityManager.CreateEntityQuery(typeof(WorkerGatherComponent));
            using var workerEntities = workerQuery.ToEntityArray(Allocator.Temp);
            using var gatherComponents = workerQuery.ToComponentDataArray<WorkerGatherComponent>(Allocator.Temp);
            workerQuery.Dispose();

            for (int i = 0; i < workerEntities.Length; i++)
            {
                var entity = workerEntities[i];
                var gather = gatherComponents[i];
                if (gather.Phase != WorkerGatherPhase.Seeking ||
                    gather.AssignedNode == Entity.Null ||
                    !entityManager.Exists(gather.AssignedNode) ||
                    !entityManager.HasComponent<PositionComponent>(gather.AssignedNode))
                {
                    continue;
                }

                var order = new WorkerGatherOrderComponent
                {
                    TargetNode = gather.AssignedNode,
                    ResourceId = gather.AssignedResourceId,
                };

                if (entityManager.HasComponent<WorkerGatherOrderComponent>(entity))
                {
                    entityManager.SetComponentData(entity, order);
                }
                else
                {
                    entityManager.AddComponentData(entity, order);
                }

                float3 nodePosition = entityManager.GetComponentData<PositionComponent>(gather.AssignedNode).Value;
                var moveCommand = new MoveCommandComponent
                {
                    Destination = nodePosition,
                    StoppingDistance = math.max(0.2f, gather.GatherRadius * 0.9f),
                    IsActive = true,
                };

                if (entityManager.HasComponent<MoveCommandComponent>(entity))
                {
                    entityManager.SetComponentData(entity, moveCommand);
                }
                else
                {
                    entityManager.AddComponentData(entity, moveCommand);
                }

                gather.Phase = WorkerGatherPhase.Gathering;
                entityManager.SetComponentData(entity, gather);
            }
        }
    }
}
