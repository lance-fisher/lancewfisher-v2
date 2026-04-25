using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Reads the currently selected building each frame and writes its worker-slot
    /// state to WorkerSlotHUDComponent for the UI panel to consume.
    ///
    /// Selection is detected via SelectedTag on the building entity.
    /// If no selected entity carries WorkerSlotBuildingComponent, IsSlotBuilding
    /// is set false and the panel hides itself.
    ///
    /// Only one selected slot-building is surfaced at a time (first match wins).
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct WorkerSlotHUDSystem : ISystem
    {
        private static readonly FixedString32Bytes FoodLabel = new("food");
        private static readonly FixedString32Bytes WoodLabel = new("wood");

        private Entity _hudEntity;

        public void OnCreate(ref SystemState state)
        {
            _hudEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(_hudEntity, new WorkerSlotHUDComponent());
        }

        public void OnUpdate(ref SystemState state)
        {
            ref var hud = ref SystemAPI.GetComponentRW<WorkerSlotHUDComponent>(_hudEntity).ValueRW;

            // Default: no slot building selected.
            hud.IsSlotBuilding = false;
            hud.BuildingEntity = Entity.Null;
            hud.AssignedWorkers = 0;
            hud.MaxWorkerSlots = 0;
            hud.FillRatio = 0f;
            hud.FoodOutputPerWorkerPerSecond = 0f;
            hud.WoodOutputPerWorkerPerSecond = 0f;
            hud.ResourceLabel = default;

            // Find first selected entity that carries WorkerSlotBuildingComponent.
            foreach (var (slot, entity) in
                SystemAPI.Query<RefRO<WorkerSlotBuildingComponent>>()
                    .WithAll<SelectedTag>()
                    .WithEntityAccess())
            {
                hud.IsSlotBuilding = true;
                hud.BuildingEntity = entity;
                hud.AssignedWorkers = slot.ValueRO.AssignedWorkers;
                hud.MaxWorkerSlots = slot.ValueRO.MaxWorkerSlots;
                hud.FillRatio = slot.ValueRO.FillRatio;
                hud.FoodOutputPerWorkerPerSecond = slot.ValueRO.FoodOutputPerWorkerPerSecond;
                hud.WoodOutputPerWorkerPerSecond = slot.ValueRO.WoodOutputPerWorkerPerSecond;
                hud.ResourceLabel = slot.ValueRO.FoodOutputPerWorkerPerSecond > 0f ? FoodLabel : WoodLabel;
                break;
            }
        }
    }
}
