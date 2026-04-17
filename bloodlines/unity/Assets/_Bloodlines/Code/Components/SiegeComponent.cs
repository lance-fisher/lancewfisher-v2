using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Session 9 siege sustainment state. Attached to siege-engine units so the
    /// supply-chain-aware damage and speed modifiers can resolve per-frame.
    /// Browser runtime equivalent: unit.siegeSuppliedUntil, unit.engineerSupportUntil.
    /// </summary>
    public struct SiegeEngineStateComponent : IComponentData
    {
        public double SuppliedUntil;
        public double EngineerSupportUntil;
    }

    /// <summary>
    /// Mantlet cover profile. Reduces inbound ranged damage to friendly units inside
    /// CoverRadius by InboundRangedMultiplier. Session 9 siege-support class.
    /// Browser runtime equivalent: unitDef.coverRadius + unitDef.coverInboundRangedMultiplier.
    /// </summary>
    public struct MantletCoverComponent : IComponentData
    {
        public float CoverRadius;
        public float InboundRangedMultiplier;
    }
}
