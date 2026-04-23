using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// One-shot request consumed by the Trueborn recognition resolution seam.
    /// Player-facing dispatch and later AI-facing request producers both write
    /// the same payload so the resolution system can stay outside AI-owned code.
    /// </summary>
    public struct PlayerTruebornRecognitionRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
    }
}
