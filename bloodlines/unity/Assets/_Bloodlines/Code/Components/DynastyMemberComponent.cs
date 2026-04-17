using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical dynasty role identifiers. Mirror of the browser runtime's
    /// role template set (simulation.js createDynastyState templates at line 4267).
    /// Order matters for succession: head_of_bloodline is the ruling seat;
    /// heir_designate is the eldest heir; remaining roles are active council
    /// positions.
    /// </summary>
    public enum DynastyRole : byte
    {
        HeadOfBloodline = 0,
        HeirDesignate = 1,
        Commander = 2,
        Governor = 3,
        Diplomat = 4,
        IdeologicalLeader = 5,
        Merchant = 6,
        Spymaster = 7,
    }

    /// <summary>
    /// Canonical dynasty path identifiers. A member commits to a path early in life.
    /// Mirrors the canonical six-path set from simulation.js createDynastyState.
    /// </summary>
    public enum DynastyPath : byte
    {
        Governance = 0,
        MilitaryCommand = 1,
        Diplomacy = 2,
        ReligiousLeadership = 3,
        EconomicStewardshipTrade = 4,
        CovertOperations = 5,
    }

    /// <summary>
    /// Canonical dynasty member status. Derived from the browser runtime's
    /// member.status string set ("ruling", "active", "dormant", "fallen", "captured").
    /// </summary>
    public enum DynastyMemberStatus : byte
    {
        Active = 0,
        Ruling = 1,
        Dormant = 2,
        Fallen = 3,
        Captured = 4,
    }

    /// <summary>
    /// Per-member dynasty state. One entity per member, parented to the owning
    /// faction entity via FactionComponent. Browser reference: simulation.js
    /// createDynastyState template objects at line 4267-4275 and the resulting
    /// member records at line 4318.
    /// </summary>
    public struct DynastyMemberComponent : IComponentData
    {
        public FixedString64Bytes MemberId;
        public FixedString64Bytes Title;
        public DynastyRole Role;
        public DynastyPath Path;
        public float AgeYears;
        public DynastyMemberStatus Status;
        public float Renown;
        public int Order;
        public float FallenAtWorldSeconds;
    }

    /// <summary>
    /// Per-faction dynasty state. Mirrors the top-level fields of the browser's
    /// createDynastyState return object (simulation.js:4278). Member list lives
    /// on separate member entities linked via DynastyMemberRef buffer.
    /// </summary>
    public struct DynastyStateComponent : IComponentData
    {
        public int ActiveMemberCap;
        public int DormantReserve;
        public float Legitimacy;
        public float LoyaltyPressure;
        public bool Interregnum;
    }

    /// <summary>
    /// Buffer on the faction entity linking to its member entities.
    /// </summary>
    public struct DynastyMemberRef : IBufferElementData
    {
        public Entity Member;
    }

    /// <summary>
    /// Buffer on the faction entity storing the fallen-member ledger in order
    /// of death. Browser reference: attachments.fallenMembers.
    /// </summary>
    public struct DynastyFallenLedger : IBufferElementData
    {
        public FixedString64Bytes MemberId;
        public FixedString64Bytes Title;
        public DynastyRole Role;
        public float FallenAtWorldSeconds;
    }
}
