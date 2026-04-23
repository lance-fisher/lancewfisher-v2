using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Per-faction HUD read-model for the active succession-crisis badge.
    /// </summary>
    public struct SuccessionCrisisHUDComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public bool CrisisActive;
        public SuccessionCrisisSeverity CrisisSeverity;
        public FixedString32Bytes SeverityLabel;
        public FixedString32Bytes SeverityColor;
        public float RecoveryProgressPct;
        public float ResourceTrickleFactor;
        public float LegitimacyDrainRatePerDay;
    }
}
