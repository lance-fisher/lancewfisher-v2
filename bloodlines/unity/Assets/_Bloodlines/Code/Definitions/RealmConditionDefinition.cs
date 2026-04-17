using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "RealmConditions_", menuName = "Bloodlines/Definitions/Realm Conditions")]
    public class RealmConditionDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public float cycleSeconds;
        public RealmConditionThresholdsData thresholds;
        public RealmConditionEffectsData effects;
        public RealmConditionLegibilityData legibility;
    }
}
