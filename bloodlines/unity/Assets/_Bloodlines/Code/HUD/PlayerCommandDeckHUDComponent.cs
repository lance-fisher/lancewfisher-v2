using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Consolidated player-facing command-deck snapshot that folds the existing
    /// realm, match, victory, renown, and fortification HUD seams into one
    /// compact summary on the player faction root.
    /// </summary>
    public struct PlayerCommandDeckHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float LastRefreshInWorldDays;

        public FixedString64Bytes StageLabel;
        public FixedString32Bytes PhaseLabel;
        public FixedString32Bytes WorldPressureLabel;
        public int WorldPressureLevel;
        public bool GreatReckoningActive;

        public FixedString32Bytes LeadingVictoryConditionId;
        public float LeadingVictoryProgressPct;
        public float LeadingVictoryEtaInWorldDays;
        public int VictoryRank;
        public FixedString32Bytes VictoryLeaderFactionId;

        public int RenownRank;
        public float RenownScore;
        public FixedString32Bytes RenownBandLabel;

        public FixedString32Bytes PopulationBand;
        public FixedString32Bytes LoyaltyBand;
        public FixedString32Bytes FaithBand;
        public bool FortificationThreatActive;

        public FixedString32Bytes PrimaryAlertLabel;
    }
}
