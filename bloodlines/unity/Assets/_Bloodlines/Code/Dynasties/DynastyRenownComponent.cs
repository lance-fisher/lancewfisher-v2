using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Public per-faction prestige surface derived from live dynasty, territory,
    /// faith, and victory state. This is additive to the existing per-member
    /// RenownAwardSystem: member renown still tracks heroic individuals, while
    /// this component tracks the dynasty-level renown/prestige score that
    /// downstream diplomacy and victory-legibility systems can read.
    /// </summary>
    public struct DynastyRenownComponent : IComponentData
    {
        public float RenownScore;
        public float LastRenownUpdateInWorldDays;
        public float RenownDecayRate;
        public float PeakRenown;
        public FixedString64Bytes LastRulingMemberId;
        public byte Initialized;
    }
}
