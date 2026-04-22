using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Singleton HUD read-model for the canonical match-progression legibility block.
    /// Mirrors the live MatchProgressionComponent plus the dominant faction's
    /// world-pressure state without mutating the underlying simulation state.
    /// </summary>
    public struct MatchProgressionHUDComponent : IComponentData
    {
        public int StageNumber;
        public FixedString32Bytes StageId;
        public FixedString64Bytes StageLabel;

        public FixedString32Bytes PhaseId;
        public FixedString32Bytes PhaseLabel;

        public float StageReadiness;
        public FixedString32Bytes NextStageId;
        public FixedString64Bytes NextStageLabel;

        public float InWorldDays;
        public float InWorldYears;
        public int DeclarationCount;

        public int WorldPressureLevel;
        public FixedString32Bytes WorldPressureLabel;
        public int WorldPressureScore;
        public bool WorldPressureTargeted;

        public FixedString32Bytes DominantLeaderFactionId;
        public float DominantLeaderTerritoryShare;

        public bool GreatReckoningActive;
        public FixedString32Bytes GreatReckoningTargetFactionId;
        public float GreatReckoningShare;
    }
}
