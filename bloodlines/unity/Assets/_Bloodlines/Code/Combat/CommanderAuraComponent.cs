using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Live commander aura profile resolved from commander renown, owning faith doctrine,
    /// and faction conviction band. Stored on the commander entity for debug/readout access.
    /// AttackBonus and SpeedBonus are additive percentages over the current runtime stats.
    /// MoraleBonus reduces the low-health retreat threshold for aura recipients.
    /// </summary>
    public struct CommanderAuraComponent : IComponentData
    {
        public float AuraRadius;
        public float AttackBonus;
        public float SightBonus;
        public float SpeedBonus;
        public float MoraleBonus;
        public float ConvictionBandMultiplier;
    }

    /// <summary>
    /// Runtime record of commander aura application on a friendly unit so the system can
    /// cleanly restore the unit's current combat and movement stats before re-evaluating
    /// proximity on the next frame.
    /// </summary>
    public struct CommanderAuraRecipientComponent : IComponentData
    {
        public Entity SourceCommander;
        public float AppliedAttackMultiplier;
        public float AppliedSightBonus;
        public float AppliedSpeedDelta;
        public float MoraleBonus;
    }
}
