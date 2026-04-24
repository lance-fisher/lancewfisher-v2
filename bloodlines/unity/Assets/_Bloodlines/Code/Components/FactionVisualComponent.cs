using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction canonical visual identity. Written once at match startup by
    /// FactionVisualAssignmentSystem and treated as read-only by all other systems.
    /// PrimaryTint is resolved from FactionTintPalette for the owning faction's
    /// canonical house color. EmblemId is the asset-registry key used by UI and
    /// rendering systems to load the dynasty emblem sprite or texture.
    ///
    /// Browser equivalent: absent -- implemented from canonical faction visual design.
    /// </summary>
    public struct FactionVisualComponent : IComponentData
    {
        public float4 PrimaryTint;
        public FixedString64Bytes EmblemId;
        public bool IsAssigned;
    }

    /// <summary>
    /// Per-unit faction color reference. Written at unit spawn time or retroactively
    /// by FactionVisualAssignmentSystem. Allows downstream systems to identify which
    /// faction colored this unit without traversing faction entity queries.
    /// Tint mirrors the owning faction's FactionVisualComponent.PrimaryTint.
    /// </summary>
    public struct UnitFactionColorComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float4 Tint;
    }
}
