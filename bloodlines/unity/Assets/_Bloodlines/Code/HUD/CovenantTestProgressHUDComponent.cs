using Bloodlines.Components;
using Bloodlines.Faith;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD progress read-model for covenant-test qualification and
    /// completion state.
    /// </summary>
    public struct CovenantTestProgressHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float LastRefreshInWorldDays;
        public CovenantId FaithId;
        public CovenantTestPhase TestPhase;
        public FixedString32Bytes PhaseLabel;
        public float QualifyingDays;
        public float RequiredDays;
        public float ProgressPct;
        public float CooldownRemainingInWorldDays;
        public int SuccessCount;
    }
}
