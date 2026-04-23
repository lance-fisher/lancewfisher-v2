using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Live contested-territory pressure on a control point.
    /// Applies when two or more hostile factions project claim pressure into the
    /// same owned control point, even if no single claimant is about to capture it.
    /// </summary>
    public struct ContestedTerritoryComponent : IComponentData
    {
        public byte ContestingFactionCount;
        public float StabilityPenaltyPerDay;
        public float LoyaltyVolatilityMultiplier;
        public float ContestStartedAtInWorldDays;
    }
}
