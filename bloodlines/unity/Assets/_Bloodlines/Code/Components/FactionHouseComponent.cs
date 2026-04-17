using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical faction house identity. This preserves house-gated prototype rules such
    /// as Ironmark- or Hartvale-exclusive training in the Unity production lane.
    /// </summary>
    public struct FactionHouseComponent : IComponentData
    {
        public FixedString32Bytes HouseId;
    }
}
