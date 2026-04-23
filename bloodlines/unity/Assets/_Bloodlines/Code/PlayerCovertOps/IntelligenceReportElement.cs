using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Per-faction intelligence report ledger. Mirrors the browser dynasty
    /// intelligence report shape closely enough for dossier follow-through while
    /// remaining lane-local on the faction entity.
    /// </summary>
    [InternalBufferCapacity(6)]
    public struct IntelligenceReportElement : IBufferElementData
    {
        public FixedString64Bytes ReportId;
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
        public FixedString32Bytes SourceType;
        public FixedString64Bytes ReportLabel;
        public FixedString32Bytes InterceptType;
        public int InterceptCount;
        public float CreatedAtInWorldDays;
        public float ExpiresAtInWorldDays;
        public int TargetLegitimacy;
        public int TargetActiveOperations;
        public int TargetCaptiveCount;
        public int TargetLesserHouseCount;
        public FixedString128Bytes TargetResourceSummary;
        public FixedString128Bytes TargetBuildingSummary;
        public FixedString512Bytes MemberSummary;
    }
}
