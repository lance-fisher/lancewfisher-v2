using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Resource_", menuName = "Bloodlines/Definitions/Resource")]
    public class ResourceDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public string category;
        public bool enabledInPrototype;
        [TextArea]
        public string description;
    }
}
