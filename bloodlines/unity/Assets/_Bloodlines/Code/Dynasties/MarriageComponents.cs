using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    public enum MarriageProposalStatus : byte
    {
        Pending  = 0,
        Accepted = 1,
        Declined = 2,
        Expired  = 3,
    }

    /// <summary>
    /// One entity per proposal. Created by the propose action; expired by
    /// MarriageProposalExpirationSystem; accepted by the accept action.
    /// Browser reference: simulation.js proposeMarriage (~7340),
    /// tickMarriageProposalExpiration (~7274).
    /// MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 30 (browser default).
    /// </summary>
    public struct MarriageProposalComponent : IComponentData
    {
        public FixedString64Bytes ProposalId;
        public FixedString32Bytes SourceFactionId;
        public FixedString64Bytes SourceMemberId;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public MarriageProposalStatus Status;
        public float ProposedAtInWorldDays;
        public float ExpiresAtInWorldDays;
    }

    /// <summary>
    /// One entity per accepted marriage. The primary record (IsPrimary = true)
    /// is the canonical side for child generation. Browser reference:
    /// simulation.js acceptMarriage (~7388), tickMarriageGestation (~7496),
    /// tickMarriageDissolutionFromDeath (~7471).
    /// MARRIAGE_GESTATION_IN_WORLD_DAYS = 60 (browser default).
    /// </summary>
    public struct MarriageComponent : IComponentData
    {
        public FixedString64Bytes MarriageId;
        public FixedString32Bytes HeadFactionId;
        public FixedString64Bytes HeadMemberId;
        public FixedString32Bytes SpouseFactionId;
        public FixedString64Bytes SpouseMemberId;
        public float MarriedAtInWorldDays;
        public float ExpectedChildAtInWorldDays;
        public bool IsPrimary;
        public bool ChildGenerated;
        public bool Dissolved;
    }

    /// <summary>
    /// Buffer on the faction entity tracking all lesser house sub-dynasties.
    /// Loyalty drifts daily; defection triggers when Loyalty reaches zero.
    /// LastDriftAppliedInWorldDays tracks when drift was last applied so the
    /// system can apply per-in-world-day deltas exactly once per day.
    /// Browser reference: simulation.js tickLesserHouseLoyaltyDrift (~6631),
    /// spawnDefectedMinorFaction (~6851).
    /// </summary>
    public struct LesserHouseElement : IBufferElementData
    {
        public FixedString32Bytes HouseId;
        public FixedString64Bytes HouseName;
        public FixedString64Bytes FounderMemberId;
        public float Loyalty;
        public float DailyLoyaltyDelta;
        public float LastDriftAppliedInWorldDays;
        public bool Defected;
    }

    /// <summary>
    /// Per-minor-house faction levy state. Advances levy tempo each tick;
    /// spawns a combat unit when accumulator fires.
    /// Browser reference: simulation.js tickMinorHouseTerritorialLevies (~7060).
    /// </summary>
    public struct MinorHouseLevyComponent : IComponentData
    {
        public float LevyAccumulator;
        public float LevyIntervalSeconds;
        public int LeviesIssued;
    }
}
