using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// One-shot request entity for a player worker-slot assignment change.
    /// UI creates a new entity carrying this component when the player clicks
    /// the + or - button on the worker-slot panel.
    ///
    /// Delta is typically +1 or -1. WorkerSlotAssignmentSystem processes and
    /// destroys the request entity in the same frame.
    ///
    /// BuildingEntity must carry WorkerSlotBuildingComponent; requests targeting
    /// an entity without it are silently discarded.
    /// </summary>
    public struct WorkerSlotAssignmentRequestComponent : IComponentData
    {
        public Entity BuildingEntity;
        public int Delta; // +1 to assign, -1 to unassign
    }
}
