using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    /// <summary>
    /// Visual-layer pairing for a BuildingDefinition. Additive, does NOT
    /// modify the existing gameplay BuildingDefinition contract.
    ///
    /// Keyed by the same string id as the matching BuildingDefinition so the
    /// runtime visual binding system can look up a building's mesh and
    /// material from its BuildingTypeComponent.TypeId. Placeholder meshes
    /// land in unity/Assets/_Bloodlines/Art/Placeholders/ via
    /// BloodlinesPlaceholderMeshGenerator. When commercial art arrives,
    /// only the Mesh and Material references flip; no gameplay touches.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildingVisual_", menuName = "Bloodlines/Graphics/Building Visual")]
    public sealed class BuildingVisualDefinition : ScriptableObject
    {
        [Header("Pairing")]
        [Tooltip("Must match the gameplay BuildingDefinition.id.")]
        public string id;

        [Header("Mesh")]
        public Mesh mesh;
        public float silhouetteScale = 1f;

        [Header("Material")]
        public Material material;

        [Header("Presentation")]
        [Tooltip("Vertical offset applied to the visual proxy transform so cube-class silhouettes sit with their base at the ground plane.")]
        public float verticalOffset = 0.6f;

        [Tooltip("Shadow casting mode hint. Placeholder meshes default to off.")]
        public bool castsShadows = false;
    }
}
