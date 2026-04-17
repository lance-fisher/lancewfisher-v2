using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Tracks per-building production facility runtime state.
    /// </summary>
    public struct ProductionFacilityComponent : IComponentData
    {
        public int SpawnSequence;
    }

    /// <summary>
    /// First ECS unit-production queue item. The command surface resolves the canonical
    /// definition data up front, then the runtime system only needs the queued payload.
    /// </summary>
    public struct ProductionQueueItemElement : IBufferElementData
    {
        public FixedString64Bytes UnitId;
        public FixedString64Bytes DisplayName;
        public float RemainingSeconds;
        public float TotalSeconds;
        public int PopulationCost;
        public int BloodPrice;
        public float BloodLoadDelta;
        public float MaxHealth;
        public float MaxSpeed;
        public UnitRole Role;
        public SiegeClass SiegeClass;
        public int Stage;
        public int GoldCost;
        public int FoodCost;
        public int WaterCost;
        public int WoodCost;
        public int StoneCost;
        public int IronCost;
        public int InfluenceCost;
    }
}
