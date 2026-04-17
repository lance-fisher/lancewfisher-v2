using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Per-faction faith state. Canonical four-covenant tracking.
    /// Browser runtime equivalent: faction.faith.
    ///
    /// Exposure accrues passively while dynasty units are near sacred sites.
    /// At the canonical threshold (100 in browser runtime) the player may commit to
    /// a covenant via FaithCommitment. Doctrine path and tier effects scale from there.
    /// </summary>
    public struct FaithStateComponent : IComponentData
    {
        public CovenantId SelectedFaith;
        public DoctrinePath DoctrinePath;
        public float Intensity;
        public int Level;
    }

    /// <summary>
    /// Exposure-track element. Added per discovered faith for a faction.
    /// </summary>
    public struct FaithExposureElement : IBufferElementData
    {
        public CovenantId Faith;
        public float Exposure;
        public bool Discovered;
    }

    public enum CovenantId : byte
    {
        None = 0,
        OldLight = 1,
        BloodDominion = 2,
        TheOrder = 3,
        TheWild = 4,
    }

    public enum DoctrinePath : byte
    {
        Unassigned = 0,
        Light = 1,
        Dark = 2,
    }

    /// <summary>
    /// Tag marking a settlement as faith-warded by the faction's committed covenant.
    /// Ward profile (sight bonus, defender attack multiplier, heal multiplier, muster
    /// multiplier, loyalty protection multiplier, surge state) is resolved at the
    /// system layer by reading FaithStateComponent on the owning faction entity.
    /// </summary>
    public struct FaithWardedSettlementTag : IComponentData
    {
    }
}
