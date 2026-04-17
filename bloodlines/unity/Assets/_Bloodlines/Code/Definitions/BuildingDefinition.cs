using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Building_", menuName = "Bloodlines/Definitions/Building")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public bool prototypeEnabled;
        public Int2Data footprint;
        public float health;
        public float buildTime;
        public int populationCapBonus;
        public string[] dropoffResources;
        public string[] trainableUnits;
        public ResourceAmountFields cost;
        public ResourceAmountFields resourceTrickle;

        [Header("Fortification Canon (2026-04-14)")]
        public string fortificationRole;
        public int fortificationTierContribution;
        public float structuralDamageMultiplier;
        public float armor;
        public bool blocksPassage;
        public float sightBonusRadius;
        public float auraAttackMultiplier;
        public float auraRadius;

        [Header("Smelting Chain")]
        public string smeltingFuelResource;
        public float smeltingFuelRatio;

        [Header("Unity Only")]
        public GameObject runtimePrefab;
        public Sprite icon;
    }
}
