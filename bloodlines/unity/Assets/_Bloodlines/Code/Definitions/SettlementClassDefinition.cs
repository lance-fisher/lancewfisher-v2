using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "SettlementClass_", menuName = "Bloodlines/Definitions/Settlement Class")]
    public class SettlementClassDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public int defensiveCeiling;
        [TextArea]
        public string description;
    }
}
