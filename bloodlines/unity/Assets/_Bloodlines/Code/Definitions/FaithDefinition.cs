using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Faith_", menuName = "Bloodlines/Definitions/Faith")]
    public class FaithDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public string covenantName;
        public bool prototypeEnabled;
        public string[] alignmentPaths;
        public DoctrineEffectSetData prototypeEffects;
    }
}
