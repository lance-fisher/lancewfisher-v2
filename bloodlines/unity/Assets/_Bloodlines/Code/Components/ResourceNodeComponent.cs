using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical resource node on the map: gold, wood, stone, iron.
    /// Food and water are production-building resources, not nodes.
    /// Browser runtime equivalent: world.resourceNodes entries.
    ///
    /// Amount decreases as workers gather; when it hits zero the node is depleted.
    /// For iron nodes, the smelting chain (wood as fuel at 0.5 ratio) is enforced at
    /// the Economy system layer, not here, so the node shape stays uniform.
    /// </summary>
    public struct ResourceNodeComponent : IComponentData
    {
        public FixedString32Bytes ResourceId;
        public float Amount;
        public float InitialAmount;
    }
}
