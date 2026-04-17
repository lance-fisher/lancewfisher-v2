using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction conviction ledger. Canonical four-bucket behavioral morality spectrum
    /// INDEPENDENT of faith. Browser runtime equivalent: faction.conviction.
    ///
    /// Buckets accrue from player action. Score is derived. Band is resolved from
    /// the score against data/conviction-states.json bands.
    ///
    /// Master doctrine XX insists conviction must not be collapsed into faith. The ECS
    /// shape preserves that: FaithStateComponent and ConvictionComponent are separate
    /// components on the faction entity.
    /// </summary>
    public struct ConvictionComponent : IComponentData
    {
        public float Ruthlessness;
        public float Stewardship;
        public float Oathkeeping;
        public float Desecration;
        public float Score;
        public ConvictionBand Band;
    }

    public enum ConvictionBand : byte
    {
        ApexMoral = 0,
        Moral = 1,
        Neutral = 2,
        Cruel = 3,
        ApexCruel = 4,
    }
}
