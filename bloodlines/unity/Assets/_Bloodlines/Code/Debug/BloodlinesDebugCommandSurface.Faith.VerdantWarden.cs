using Bloodlines.Components;
using Bloodlines.Faith;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetVerdantWardenCoverage(string controlPointId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(controlPointId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.ToString() != controlPointId)
                {
                    continue;
                }

                VerdantWardenCoverageProfile support =
                    VerdantWardenRules.ResolveControlPointSupport(controlPoints[i]);
                readout =
                    "VerdantWardenCoverage|ControlPointId=" + controlPointId +
                    "|Count=" + support.Count +
                    "|CappedCount=" + support.CappedCount +
                    "|LoyaltyBonusPerTick=" + support.LoyaltyBonusPerTick +
                    "|LoyaltyGainMultiplier=" + support.LoyaltyGainMultiplier +
                    "|LoyaltyProtectionMultiplier=" + support.LoyaltyProtectionMultiplier +
                    "|StabilizationMultiplier=" + support.StabilizationMultiplier;
                return true;
            }

            return false;
        }
    }
}
