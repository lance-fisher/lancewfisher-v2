using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "BloodlinePath_", menuName = "Bloodlines/Definitions/Bloodline Path")]
    public class BloodlinePathDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        [TextArea]
        public string summary;
    }
}
