using Unity.Entities;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Per-faction resolved doctrine effects written by FaithDoctrineSyncSystem from the
    /// faction's committed FaithStateComponent. Downstream systems read this component
    /// rather than doing covenant+doctrine lookups per frame.
    ///
    /// Values match faiths.json prototypeEffects keyed by covenant+doctrinePath.
    /// Defaults (no commitment) are identity multipliers: 1.0 for multipliers, 0 for bonuses.
    ///
    /// Browser reference: DOCTRINE_DEFAULTS / getFaithDoctrineEffects (simulation.js ~170, ~581)
    /// </summary>
    public struct FaithDoctrineEffectsComponent : IComponentData
    {
        /// Commander aura attack multiplier (applied in CommanderAuraCanon).
        public float AuraAttackMultiplier;
        /// Commander aura radius bonus in tiles.
        public float AuraRadiusBonus;
        /// Commander aura sight bonus.
        public float AuraSightBonus;
        /// Control point passive and active stabilization multiplier.
        public float StabilizationMultiplier;
        /// Control point capture rate multiplier.
        public float CaptureMultiplier;
        /// Population growth multiplier (reserved for future population system).
        public float PopulationGrowthMultiplier;

        public static FaithDoctrineEffectsComponent Defaults() => new FaithDoctrineEffectsComponent
        {
            AuraAttackMultiplier = 1f,
            AuraRadiusBonus = 0f,
            AuraSightBonus = 0f,
            StabilizationMultiplier = 1f,
            CaptureMultiplier = 1f,
            PopulationGrowthMultiplier = 1f,
        };
    }
}
