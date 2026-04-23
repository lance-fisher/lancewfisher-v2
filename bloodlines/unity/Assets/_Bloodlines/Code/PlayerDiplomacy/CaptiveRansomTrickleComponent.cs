using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Debug-observable tracker for the passive captive ransom trickle seam.
    /// Stores the current per-second rates plus the last whole-day deltas that
    /// the runtime applied to the captor faction.
    /// </summary>
    public struct CaptiveRansomTrickleComponent : IComponentData
    {
        public float LastAppliedInWorldDays;
        public float CurrentInfluenceRatePerSecond;
        public float CurrentDynastyRenownRatePerSecond;
        public float LastAppliedInfluenceDelta;
        public float LastAppliedDynastyRenownDelta;
        public float HighestCaptiveRenown;
        public int HeldCaptiveCount;
        public byte Initialized;
    }

    internal static class CaptiveRansomTrickleRules
    {
        internal const float BaseInfluenceTricklePerSecond = 0.022f;
        internal const float CaptiveRenownWeight = 0.0014f;
        internal const float DefaultCaptiveRenown = 6f;

        internal static float ResolveInfluenceRatePerSecond(float captiveRenown)
        {
            return BaseInfluenceTricklePerSecond +
                   math.max(0f, captiveRenown) * CaptiveRenownWeight;
        }

        internal static float ResolveDynastyRenownRatePerSecond(float captiveRenown)
        {
            // The browser only pays passive influence here; Unity also folds the
            // same renown-weighted signal into the already-landed dynasty renown
            // ledger so notable captives create visible prestige pressure.
            return math.max(0f, captiveRenown) * CaptiveRenownWeight;
        }

        internal static float ResolveElapsedRealSeconds(int elapsedWholeDays, float daysPerRealSecond)
        {
            if (elapsedWholeDays <= 0)
            {
                return 0f;
            }

            return elapsedWholeDays / math.max(0.0001f, daysPerRealSecond);
        }
    }
}
