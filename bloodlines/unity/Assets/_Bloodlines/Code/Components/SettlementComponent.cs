using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Settlement entity: the canonical fortification anchor.
    /// Browser runtime equivalent: world.settlements entries.
    ///
    /// SettlementClass determines the fortification tier ceiling. Class values map to
    /// the six canonical settlement classes loaded from data/settlement-classes.json:
    /// border_settlement, military_fort, trade_town, regional_stronghold,
    /// primary_dynastic_keep, fortress_citadel.
    /// </summary>
    public struct SettlementComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public FixedString32Bytes SettlementClassId;
        public int FortificationTier;
        public int FortificationCeiling;
    }

    /// <summary>
    /// Tag marking a settlement as the primary dynastic keep for its faction.
    /// The bloodline head of household is canonically seated here.
    /// Session 9 snapshot's fortification block reads from this anchor.
    /// </summary>
    public struct PrimaryKeepTag : IComponentData
    {
    }
}
