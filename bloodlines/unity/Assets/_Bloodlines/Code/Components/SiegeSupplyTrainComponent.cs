using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Supply-wagon linkage state for the first governed siege logistics slice.
    /// A wagon must stay linked to a live allied supply camp before it can extend
    /// siege-engine supply windows or field-water support.
    /// </summary>
    public struct SiegeSupplyTrainComponent : IComponentData
    {
        public Entity LinkedCampEntity;
        public double LastSupplyTransferAt;
    }
}
