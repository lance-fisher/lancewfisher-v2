using Unity.Entities;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Runtime tier contribution for completed buildings linked to a fortified
    /// settlement. This keeps the sub-slice 1 validator isolated from the live
    /// bootstrap authoring flow.
    /// </summary>
    public struct FortificationBuildingContributionComponent : IComponentData
    {
        public int TierContribution;
    }
}
