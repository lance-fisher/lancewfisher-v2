using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
     /// Supply-wagon linkage state for the first governed siege logistics slice.
     /// A wagon must stay linked to a live allied supply camp before it can extend
     /// siege-engine supply windows or field-water support. The interdiction and
     /// recovery fields mirror the browser convoy state surfaced by
     /// `isSupplyWagonInterdicted`, `isSupplyWagonRecovering`, and
     /// `getSupplyWagonEscortCoverage`.
     /// </summary>
    public struct SiegeSupplyTrainComponent : IComponentData
    {
        public Entity LinkedCampEntity;
        public double LastSupplyTransferAt;
        public double LogisticsInterdictedUntil;
        public double ConvoyRecoveryUntil;
        public double ConvoyReconsolidatedAt;
        public FixedString32Bytes InterdictedByFactionId;
        public int EscortCount;
        public int RequiredEscortCount;
        public bool EscortScreened;
    }
}
