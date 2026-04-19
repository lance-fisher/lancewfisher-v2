using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Explicit gather-work order issued by the AI command-dispatch layer.
    /// Tracks the node the worker has been ordered to gather from after the
    /// strategic gather chooser selects a valid target.
    /// </summary>
    public struct WorkerGatherOrderComponent : IComponentData
    {
        public Entity TargetNode;
        public FixedString32Bytes ResourceId;
    }
}
