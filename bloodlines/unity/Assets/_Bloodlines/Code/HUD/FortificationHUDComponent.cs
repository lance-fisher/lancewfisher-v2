using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Settlement-level HUD read-model for fortification legibility.
    /// Mirrors the fortification block of the browser realm-condition snapshot without
    /// mutating the underlying siege or reserve state.
    /// </summary>
    public struct FortificationHUDComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public FixedString32Bytes SettlementClassId;
        public FixedString32Bytes OwnerFactionId;

        public bool IsPrimaryKeep;
        public int Tier;
        public int Ceiling;

        public int OpenBreachCount;
        public int DestroyedWallSegmentCount;
        public int DestroyedTowerCount;
        public int DestroyedGateCount;
        public int DestroyedKeepCount;

        public int ReserveFrontage;
        public int MusteredDefenders;
        public int ReadyReserveCount;
        public int MusteringReserveCount;
        public int RecoveringReserveCount;
        public int FallbackReserveCount;
        public int LastCommittedCount;
        public bool ThreatActive;

        public bool SealingEligible;
        public bool SealingTracked;
        public float SealingProgress01;
        public float SealingAccumulatedWorkerHours;
        public float SealingRequiredWorkerHours;
        public float SealingReservedStone;
        public float SealingRequiredStone;

        public bool RecoveryEligible;
        public bool RecoveryTracked;
        public DestroyedCounterKind RecoveryTargetCounter;
        public float RecoveryProgress01;
        public float RecoveryAccumulatedWorkerHours;
        public float RecoveryRequiredWorkerHours;
        public float RecoveryReservedStone;
        public float RecoveryRequiredStone;
    }
}
