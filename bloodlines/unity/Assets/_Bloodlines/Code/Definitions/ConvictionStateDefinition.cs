using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Conviction_", menuName = "Bloodlines/Definitions/Conviction State")]
    public class ConvictionStateDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string label;
        public int minScore;
    }
}
