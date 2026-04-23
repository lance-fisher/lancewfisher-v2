using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD read-model for the shared Trueborn rise arc and each
    /// kingdom's recognition state.
    /// </summary>
    public struct TruebornRiseHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float LastRefreshInWorldDays;
        public byte CurrentStage;
        public FixedString64Bytes StageLabel;
        public bool RiseActive;
        public bool Recognized;
        public int RecognizedFactionCount;
        public int ChallengeLevel;
        public float GlobalPressurePerDay;
        public float LoyaltyErosionPerDay;
    }
}
