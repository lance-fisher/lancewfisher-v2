using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Active counter-intelligence watch state on the faction entity. Browser parity
    /// keeps a short watch list; Unity stores the live watch directly because the
    /// player lane only needs one active court watch at a time.
    /// </summary>
    public struct PlayerCounterIntelligenceComponent : IComponentData
    {
        public FixedString64Bytes WatchId;
        public float ActivatedAtInWorldDays;
        public float ExpiresAtInWorldDays;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float WatchStrength;
        public FixedString64Bytes WardLabel;
        public FixedString128Bytes GuardedRoles;
        public float AverageLoyalty;
        public float WeakestLoyalty;
        public int Interceptions;
        public int FoiledEspionage;
        public int FoiledAssassinations;
        public float LastInterceptAtInWorldDays;
        public FixedString32Bytes LastInterceptType;
        public FixedString32Bytes LastSourceFactionId;
        public FixedString64Bytes LastTargetMemberId;
        public int LastSourceInterceptions;
        public int LastSourceFoiledEspionage;
        public int LastSourceFoiledAssassinations;
        public float LastDossierAtInWorldDays;
    }
}
