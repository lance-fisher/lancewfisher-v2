using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Victory_", menuName = "Bloodlines/Definitions/Victory Condition")]
    public class VictoryConditionDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public string status;
        public bool prototypeEnabled;
    }
}
