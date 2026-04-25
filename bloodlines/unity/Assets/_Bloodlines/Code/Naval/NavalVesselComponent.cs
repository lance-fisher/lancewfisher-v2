using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Identifies an entity as a naval vessel and carries the canonical vessel-class
    /// payload shared by every naval system in the layer.
    ///
    /// Browser runtime equivalent: a unit whose UnitDefinition has
    /// movementDomain == "water" and a vesselClass string. Stored on the entity so
    /// downstream systems (embark, disembark, fire-ship detonation, naval combat,
    /// fishing gather) can filter without rereading the unit definition table.
    /// </summary>
    public struct NavalVesselComponent : IComponentData
    {
        public VesselClass Class;

        /// <summary>
        /// Maximum land-unit passengers this vessel can ferry. Non-zero only for
        /// transport-class vessels (canonical capacity 6 for transport_ship per
        /// data/units.json). Other vessel classes carry zero.
        /// </summary>
        public int TransportCapacity;

        /// <summary>
        /// True when the vessel detonates on its first hostile strike and is
        /// destroyed alongside the target. Canonical for fire_ship per master
        /// doctrine section XV. Consumed by the fire-ship detonation system in S3.
        /// </summary>
        public bool OneUseSacrifice;
    }
}
