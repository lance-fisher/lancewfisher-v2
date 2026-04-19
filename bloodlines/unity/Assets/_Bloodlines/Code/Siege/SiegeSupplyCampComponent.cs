using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Supply-camp operational reserve for the first governed interdiction slice.
    /// The browser runtime models the camp as a live siege anchor whose disruption
    /// cascades into convoy interdiction, recovering screens, and assault delays.
    /// </summary>
    public struct SiegeSupplyCampComponent : IComponentData
    {
        public float Stockpile;
        public float MaxStockpile;
        public float OperationalThreshold;
        public int NearbyRaiderCount;
        public double LastInterdictedAt;
        public double LastRecoveredAt;
        public FixedString32Bytes InterdictedByFactionId;
    }
}
