using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Current and maximum health for units and buildings.
    /// Canonical browser runtime equivalent: entity.health (current) and unitDef.health /
    /// buildingDef.health (maximum).
    /// Damage systems read Max from the definition cache and subtract from Current here.
    /// </summary>
    public struct HealthComponent : IComponentData
    {
        public float Current;
        public float Max;
    }

    /// <summary>
    /// Added to entities with zero health on the current tick so finalize systems
    /// can process deaths (succession cascade, captive/fallen ledger updates) without
    /// racing the damage pipeline. Canonical browser runtime equivalent: deathFinalized flag.
    /// </summary>
    public struct DeadTag : IComponentData
    {
    }
}
