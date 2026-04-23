using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// One-shot player request to switch a settlement's imminent-engagement posture.
    /// </summary>
    public struct PlayerImminentEngagementPostureRequestComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public byte PostureId;
    }
}
