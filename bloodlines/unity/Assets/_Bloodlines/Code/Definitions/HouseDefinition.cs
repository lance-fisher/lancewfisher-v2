using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "House_", menuName = "Bloodlines/Definitions/House")]
    public class HouseDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public string status;
        public string hairColor;
        public string primaryColor;
        public string accentColor;
        public string symbol;
        public string societalAdvantage;
        public string requiredDisadvantage;
        public bool prototypePlayable;
        public string[] notes;

        [Header("House Mechanics (browser data/houses.json mechanics block)")]
        public float fortificationCostMultiplier;
        public float fortificationBuildSpeedMultiplier;
    }
}
