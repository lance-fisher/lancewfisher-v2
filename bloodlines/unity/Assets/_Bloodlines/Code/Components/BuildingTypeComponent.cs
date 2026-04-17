using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Canonical building type identity for a building entity.
    /// Maps to an entry in _Bloodlines/Data/Buildings/*.asset. Browser runtime equivalent:
    /// building.typeId.
    ///
    /// FortificationRole and StructuralDamageMultiplier are the Decision 21 canonical fields
    /// driving layered-defense damage math. Session 9 closed seven-layer siege maturity;
    /// these fields support that path.
    /// </summary>
    public struct BuildingTypeComponent : IComponentData
    {
        public FixedString64Bytes TypeId;
        public FortificationRole FortificationRole;
        public float StructuralDamageMultiplier;
        public int PopulationCapBonus;
        public bool BlocksPassage;
        public bool SupportsSiegePreparation;
        public bool SupportsSiegeLogistics;
    }

    public enum FortificationRole : byte
    {
        None = 0,
        Wall = 1,
        Tower = 2,
        Gate = 3,
        Keep = 4,
    }
}
