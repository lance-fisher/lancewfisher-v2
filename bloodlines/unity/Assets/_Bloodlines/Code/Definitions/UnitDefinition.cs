using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Unit_", menuName = "Bloodlines/Definitions/Unit")]
    public class UnitDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public int stage;
        public string role;
        public string siegeClass;
        public bool prototypeEnabled;
        public int populationCost;
        public float health;
        public float speed;
        public float attackDamage;
        public float attackRange;
        public float attackCooldown;
        public float sight;
        public float projectileSpeed;
        public string movementDomain;
        public string vesselClass;
        public int transportCapacity;
        public bool oneUseSacrifice;
        public float carryCapacity;
        public float gatherRate;
        public float buildRate;
        public float structuralDamageMultiplier;
        public float antiUnitDamageMultiplier;
        public ResourceAmountFields cost;
        public string house;
        public string faithId;
        public string doctrinePath;
        public int ironmarkBloodPrice;
        public float bloodProductionLoadDelta;
        public string[] notes;
        public float separationRadius;

        [Header("Unity Only")]
        public GameObject runtimePrefab;
        public Sprite icon;
        public AudioClip selectClip;
        public AudioClip deathClip;
    }
}
