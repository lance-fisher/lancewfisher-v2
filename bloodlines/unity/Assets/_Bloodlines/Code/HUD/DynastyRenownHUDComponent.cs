using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD snapshot for the dynasty renown/prestige surface.
    /// This stays read-only relative to the source dynasty and renown systems.
    /// </summary>
    public struct DynastyRenownHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float RenownScore;
        public float PeakRenown;
        public float ScoreToPeakRatio;
        public int RenownRank;
        public bool IsLeadingDynasty;
        public FixedString64Bytes RulerMemberId;
        public FixedString64Bytes RulerTitle;
        public float Legitimacy;
        public bool Interregnum;
        public FixedString32Bytes StatusLabel;
        public FixedString32Bytes RenownBandLabel;
        public FixedString32Bytes RenownBandColor;
    }

    /// <summary>
    /// Tracks when the HUD snapshot was last refreshed so the presentation layer
    /// does not rewrite the read-model every frame.
    /// </summary>
    public struct DynastyRenownHUDRefreshComponent : IComponentData
    {
        public float LastRefreshInWorldDays;
    }
}
