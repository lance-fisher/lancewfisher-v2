using Bloodlines.Components;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Applies player worker-slot assignment changes issued via
    /// WorkerSlotAssignmentRequestComponent one-shot entities.
    ///
    /// For each request:
    ///   1. Validates the target building carries WorkerSlotBuildingComponent.
    ///   2. Clamps the resulting AssignedWorkers to [0, MaxWorkerSlots].
    ///   3. Writes AssignedWorkers back. WorkerSlotProductionSystem picks it up
    ///      next frame automatically.
    ///   4. Destroys the request entity.
    ///
    /// Runs in SimulationSystemGroup before WorkerSlotProductionSystem so the
    /// new assignment is reflected in the same-frame output tick.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(WorkerSlotProductionSystem))]
    public partial struct WorkerSlotAssignmentSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorkerSlotAssignmentRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            using var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<WorkerSlotAssignmentRequestComponent>());
            using NativeArray<Entity> requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<WorkerSlotAssignmentRequestComponent> requests =
                requestQuery.ToComponentDataArray<WorkerSlotAssignmentRequestComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < requestEntities.Length; i++)
            {
                var req = requests[i];

                if (req.BuildingEntity == Entity.Null ||
                    !entityManager.Exists(req.BuildingEntity) ||
                    !entityManager.HasComponent<WorkerSlotBuildingComponent>(req.BuildingEntity))
                {
                    ecb.DestroyEntity(requestEntities[i]);
                    continue;
                }

                var slot = entityManager.GetComponentData<WorkerSlotBuildingComponent>(req.BuildingEntity);
                int next = slot.AssignedWorkers + req.Delta;
                slot.AssignedWorkers = next < 0 ? 0 : next > slot.MaxWorkerSlots ? slot.MaxWorkerSlots : next;
                entityManager.SetComponentData(req.BuildingEntity, slot);

                ecb.DestroyEntity(requestEntities[i]);
            }

            ecb.Playback(entityManager);
        }
    }
}
