using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Singleton tracking the schema version of the most recently captured or
    /// restored snapshot. Version 1 covers faction-scoped simulation state
    /// (factions, loyalty, resources, realm conditions, conviction, dynasty,
    /// faith, population). Later versions extend the payload incrementally.
    ///
    /// Browser reference: simulation.js:13822 exportStateSnapshot (version field).
    /// </summary>
    public struct SnapshotVersionComponent : IComponentData
    {
        public int Version;
    }
}
