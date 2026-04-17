using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-entity faction tint value fed into the BloodlinesFactionColor
    /// shader via MaterialPropertyBlock / DOTS instance override.
    ///
    /// Resolved at spawn time from the entity's FactionComponent via
    /// FactionTintPalette. Stored as a float4 so the shader can treat it
    /// as a color without conversion. Alpha channel reserved for future
    /// legibility toggles (selection highlight intensity, fog-of-war
    /// dimming, etc.) and currently defaults to 1.
    /// </summary>
    public struct FactionTintComponent : IComponentData
    {
        public float4 Tint;
    }
}
