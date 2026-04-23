using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction contested-territory pressure read-model.
    /// Browser references:
    ///   - getRealmConditionSnapshot contestedTerritories
    ///   - territorial-governance contested-territory conflict pressure
    /// </summary>
    public struct TerritorialPressureComponent : IComponentData
    {
        // Browser realm-condition snapshot: non-owned control points with captureProgress > 0.
        public int ExternalContestedTerritoryCount;
        // Governance-recognition conflict surface: owned control points that are contested or mid-capture.
        public int OwnedContestedTerritoryCount;
        public FixedString32Bytes WeakestOwnedContestedControlPointId;
        public float WeakestOwnedContestedLoyalty;
        public bool GovernanceContestBlockingActive;
    }
}
