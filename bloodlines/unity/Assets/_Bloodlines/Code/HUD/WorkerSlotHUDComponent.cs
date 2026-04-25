using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton snapshot for the worker-slot assignment panel.
    /// Written by WorkerSlotHUDSystem each frame from the currently selected building.
    ///
    /// IsSlotBuilding drives panel visibility: hide the panel when false.
    /// ResourceLabel is "food" or "wood" for the output label.
    ///
    /// UI writes WorkerSlotAssignmentRequestComponent one-shot entities to mutate
    /// AssignedWorkers; WorkerSlotAssignmentSystem applies and clamps the delta.
    ///
    /// Canon: early-game-foundation prompt 2026-04-25 worker-slot model.
    /// buildings.json tooltip: "Output = workerOutputPerSecond × assignedWorkers × effectiveProductivity"
    /// </summary>
    public struct WorkerSlotHUDComponent : IComponentData
    {
        public bool IsSlotBuilding;
        public Entity BuildingEntity;

        public int AssignedWorkers;
        public int MaxWorkerSlots;
        public float FillRatio;

        // Only one of these will be non-zero per building type.
        public float FoodOutputPerWorkerPerSecond;
        public float WoodOutputPerWorkerPerSecond;

        // "food" or "wood" — drive the output-rate label.
        public FixedString32Bytes ResourceLabel;
    }
}
