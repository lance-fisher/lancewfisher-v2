using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Faith
{
    public struct PlayerCovenantTestDispatchStateComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public CovenantId FaithId;
        public DoctrinePath DoctrinePath;
        public CovenantTestPhase TestPhase;
        public bool ActionAvailable;
        public bool CanAfford;
        public bool RequestQueued;
        public bool RequestPending;
        public float FoodCost;
        public float InfluenceCost;
        public int PopulationCost;
        public float LegitimacyCost;
        public FixedString64Bytes ActionLabel;
        public FixedString128Bytes ActionDetail;
        public FixedString128Bytes BlockReason;

        public static PlayerCovenantTestDispatchStateComponent CreateDefault(FixedString32Bytes sourceFactionId)
        {
            return new PlayerCovenantTestDispatchStateComponent
            {
                SourceFactionId = sourceFactionId,
                FaithId = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                TestPhase = CovenantTestPhase.Inactive,
                ActionAvailable = false,
                CanAfford = false,
                RequestQueued = false,
                RequestPending = false,
                FoodCost = 0f,
                InfluenceCost = 0f,
                PopulationCost = 0,
                LegitimacyCost = 0f,
                ActionLabel = default,
                ActionDetail = default,
                BlockReason = default,
            };
        }
    }
}
