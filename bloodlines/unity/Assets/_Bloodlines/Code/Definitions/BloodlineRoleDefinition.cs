using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "BloodlineRole_", menuName = "Bloodlines/Definitions/Bloodline Role")]
    public class BloodlineRoleDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public bool prototypeEnabled;
        public string pathAffinity;
    }
}
