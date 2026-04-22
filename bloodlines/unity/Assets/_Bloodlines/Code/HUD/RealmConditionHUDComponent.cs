using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD snapshot for the canonical realm-condition legibility block.
    /// This is an additive read-model owned by the player HUD lane; it does not
    /// replace the underlying simulation components.
    /// </summary>
    public struct RealmConditionHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;

        public int CycleCount;
        public float CycleProgress;

        public int Population;
        public int PopulationCap;
        public float PopulationRatio;
        public FixedString32Bytes PopulationBand;

        public float FoodStock;
        public float FoodNeed;
        public float FoodRatio;
        public FixedString32Bytes FoodBand;
        public int FoodStrainStreak;

        public float WaterStock;
        public float WaterNeed;
        public float WaterRatio;
        public FixedString32Bytes WaterBand;
        public int WaterStrainStreak;

        public float LoyaltyCurrent;
        public float LoyaltyMax;
        public float LoyaltyFloor;
        public FixedString32Bytes LoyaltyBand;

        public float ConvictionScore;
        public ConvictionBand ConvictionBand;
        public FixedString32Bytes ConvictionBandLabel;
        public FixedString32Bytes ConvictionBandColor;

        public CovenantId FaithId;
        public DoctrinePath DoctrinePath;
        public float FaithIntensity;
        public int FaithLevel;
        public FixedString32Bytes FaithLabel;
        public FixedString32Bytes FaithTierLabel;
        public FixedString32Bytes DoctrineLabel;
        public FixedString32Bytes FaithBand;
        public bool FaithCommitted;
    }
}
