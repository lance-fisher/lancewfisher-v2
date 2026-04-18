using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Explicit ECS link between a fortified settlement and its buildings or defenders.
    /// </summary>
    public struct FortificationSettlementLinkComponent : IComponentData
    {
        public Entity SettlementEntity;
        public FixedString64Bytes SettlementId;
    }
}
