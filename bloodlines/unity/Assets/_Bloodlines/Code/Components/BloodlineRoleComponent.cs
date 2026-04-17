using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Bloodline member state. Canonical roster element backing the dynasty panel,
    /// succession chain, captive ledger, and fallen ledger.
    /// Browser runtime equivalent: faction.dynasty.members entries.
    /// </summary>
    public struct BloodlineMemberComponent : IComponentData
    {
        public FixedString64Bytes MemberId;
        public FixedString32Bytes FactionId;
        public BloodlineRole Role;
        public BloodlineStatus Status;
        public BloodlinePath Path;
        public int Age;
        public float Renown;
    }

    /// <summary>
    /// Canonical roles. Order matters: SUCCESSION_ROLE_CHAIN in the browser runtime
    /// walks from HeadOfBloodline to Heir to Commander to Governor to Diplomat to
    /// IdeologicalLeader to Merchant to Spymaster. Sorcerer (Mysticism path, locked
    /// 2026-03-26) sits alongside but does not participate in the succession chain.
    /// </summary>
    public enum BloodlineRole : byte
    {
        HeadOfBloodline = 0,
        Heir = 1,
        Commander = 2,
        Governor = 3,
        Diplomat = 4,
        IdeologicalLeader = 5,
        Merchant = 6,
        Spymaster = 7,
        Sorcerer = 8,
    }

    public enum BloodlineStatus : byte
    {
        Active = 0,
        Captured = 1,
        Fallen = 2,
        Interregnum = 3,
    }

    /// <summary>
    /// Canonical training paths. Matches data/bloodline-paths.json.
    /// </summary>
    public enum BloodlinePath : byte
    {
        Unassigned = 0,
        MilitaryCommand = 1,
        Governance = 2,
        ReligiousLeadership = 3,
        Diplomacy = 4,
        CovertOperations = 5,
        EconomicStewardshipTrade = 6,
        Mysticism = 7,
    }
}
