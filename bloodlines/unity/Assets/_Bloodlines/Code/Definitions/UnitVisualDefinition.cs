using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    /// <summary>
    /// Visual-layer pairing for a UnitDefinition. Additive, does NOT modify
    /// the existing gameplay UnitDefinition contract.
    ///
    /// Keyed by the same string id as the matching UnitDefinition so the
    /// runtime visual binding system can look up a unit's mesh and material
    /// from its UnitTypeComponent.TypeId without round-tripping through the
    /// gameplay definition layer.
    ///
    /// The placeholder meshes land in unity/Assets/_Bloodlines/Art/Placeholders/
    /// via BloodlinesPlaceholderMeshGenerator. When commercial art arrives,
    /// only the Mesh and Material references in these assets need to flip
    /// over; no gameplay code has to change.
    /// </summary>
    [CreateAssetMenu(fileName = "UnitVisual_", menuName = "Bloodlines/Graphics/Unit Visual")]
    public sealed class UnitVisualDefinition : ScriptableObject
    {
        [Header("Pairing")]
        [Tooltip("Must match the gameplay UnitDefinition.id.")]
        public string id;

        [Header("Mesh")]
        public Mesh mesh;
        public float silhouetteScale = 1f;

        [Header("Material")]
        public Material material;

        [Header("Presentation")]
        [Tooltip("Vertical offset applied to the visual proxy transform so capsule-class silhouettes sit above the ground plane.")]
        public float verticalOffset = 0.9f;

        [Tooltip("Shadow casting mode hint for future renderers. Placeholder meshes default to off.")]
        public bool castsShadows = false;
    }
}
