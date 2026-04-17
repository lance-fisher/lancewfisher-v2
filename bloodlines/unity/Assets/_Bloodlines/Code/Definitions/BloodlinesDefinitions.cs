using System;

namespace Bloodlines.DataDefinitions
{
    [Serializable]
    public class ResourceAmountFields
    {
        public int gold;
        public int food;
        public int water;
        public int wood;
        public int stone;
        public int iron;
        public int influence;
    }

    [Serializable]
    public class Float2Data
    {
        public float x;
        public float y;
    }

    [Serializable]
    public class Int2Data
    {
        public int w;
        public int h;
    }

    [Serializable]
    public class TerrainPatchData
    {
        public string type;
        public int x;
        public int y;
        public int w;
        public int h;
        public bool isCoastal;
        public string continentDivide;
    }

    [Serializable]
    public class ResourceTrickleFields
    {
        public float gold;
        public float food;
        public float water;
        public float wood;
        public float stone;
        public float iron;
        public float influence;
    }

    [Serializable]
    public class ResourceNodeData
    {
        public string id;
        public string type;
        public float x;
        public float y;
        public int amount;
    }

    [Serializable]
    public class ControlPointData
    {
        public string id;
        public string name;
        public string settlementClass;
        public string continentId;
        public float x;
        public float y;
        public float radiusTiles;
        public float captureTime;
        public ResourceTrickleFields resourceTrickle;
    }

    [Serializable]
    public class SacredSiteData
    {
        public string id;
        public string name;
        public string faithId;
        public float x;
        public float y;
        public float radiusTiles;
        public float exposureRate;
    }

    [Serializable]
    public class SettlementSeedData
    {
        public string id;
        public string name;
        public string factionId;
        public string settlementClass;
        public string anchorBuildingId;
        public float x;
        public float y;
    }

    [Serializable]
    public class PopulationSeedData
    {
        public int total;
        public int cap;
        public int reserved;
    }

    [Serializable]
    public class BuildingSeedData
    {
        public string id;
        public string typeId;
        public int x;
        public int y;
        public bool completed;
    }

    [Serializable]
    public class UnitSeedData
    {
        public string id;
        public string typeId;
        public float x;
        public float y;
    }

    [Serializable]
    public class PresentationData
    {
        public string primaryColor;
        public string accentColor;
    }

    [Serializable]
    public class DoctrineEffectData
    {
        public string label;
        public float auraAttackMultiplier;
        public float auraRadiusBonus;
        public float auraSightBonus;
        public float stabilizationMultiplier;
        public float captureMultiplier;
        public float populationGrowthMultiplier;
    }

    [Serializable]
    public class DoctrineEffectSetData
    {
        public DoctrineEffectData light;
        public DoctrineEffectData dark;
    }

    [Serializable]
    public class FactionSeedData
    {
        public string id;
        public string houseId;
        public string displayName;
        public string kind;
        public string[] hostileTo;
        public string aiProfile;
        public PresentationData presentation;
        public ResourceAmountFields startingResources;
        public PopulationSeedData population;
        public BuildingSeedData[] buildings;
        public UnitSeedData[] units;
    }

    [Serializable]
    public class RealmConditionThresholdsData
    {
        public float foodStrainRatio;
        public int foodFamineConsecutiveCycles;
        public float waterStrainRatio;
        public int waterCrisisConsecutiveCycles;
        public float populationCapPressureRatio;
    }

    [Serializable]
    public class RealmConditionFamineData
    {
        public int populationDeclinePerCycle;
        public int loyaltyDeltaPerCycle;
        public float unitMoraleMultiplier;
    }

    [Serializable]
    public class RealmConditionWaterCrisisData
    {
        public int outmigrationPerCycle;
        public int loyaltyDeltaPerCycle;
        public float territoryAgricultureMultiplier;
    }

    [Serializable]
    public class RealmConditionCapPressureData
    {
        public int loyaltyDeltaPerCycle;
    }

    [Serializable]
    public class RealmConditionEffectsData
    {
        public RealmConditionFamineData famine;
        public RealmConditionWaterCrisisData waterCrisis;
        public RealmConditionCapPressureData capPressure;
    }

    [Serializable]
    public class RealmConditionLegibilityData
    {
        public float foodGreenRatio;
        public float foodYellowRatio;
        public float waterGreenRatio;
        public float waterYellowRatio;
        public float populationGreenCapRatio;
        public float populationYellowCapRatio;
        public int loyaltyGreenFloor;
        public int loyaltyYellowFloor;
    }
}
