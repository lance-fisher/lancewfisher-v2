using UnityEngine;

namespace Bloodlines.DataDefinitions
{
    [CreateAssetMenu(fileName = "Map_", menuName = "Bloodlines/Definitions/Map")]
    public class MapDefinition : ScriptableObject
    {
        [Header("Synced From JSON")]
        public string id;
        public string displayName;
        public int tileSize;
        public int width;
        public int height;
        public Float2Data cameraStart;
        public TerrainPatchData[] terrainPatches;
        public ResourceNodeData[] resourceNodes;
        public ControlPointData[] controlPoints;
        public SettlementSeedData[] settlements;
        public SacredSiteData[] sacredSites;
        public FactionSeedData[] factions;
    }
}
