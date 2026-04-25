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
        public ResourceTrickleFields resourceTrickle;

        [Header("Build Tier (Early Game Foundation 2026-04-25)")]
        // Matches buildings.json buildTier field.
        // 0 = pre-deploy / environment-placed, 1 = first post-Keep tier,
        // 2 = small-farm tier, 3+ = full build tree.
        public int buildTier;

        [Header("Worker Slots (Early Game Foundation 2026-04-25)")]
        // >0 on buildings that use assigned worker slots (Woodcutter Camp, Forager Camp, Small Farm).
        // 0 means no slot model (trickle-based or gather-based instead).
        public int maxWorkerSlots;
        // Per-worker output per second. Resolved at runtime into WorkerSlotBuildingComponent.
        public ResourceTrickleFields workerOutputPerSecond;

        [Header("Water Capacity (Early Game Foundation 2026-04-25)")]
        // Population supported by this building's water infrastructure.
        // Only relevant for well-type buildings. 0 means no water support.
        public int waterPopulationSupport;

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
