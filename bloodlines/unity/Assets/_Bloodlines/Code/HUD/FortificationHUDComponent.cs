using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Settlement-scoped fortification read-model for the player HUD lane.
    /// Mirrors the fortification block surfaced from the browser realm-condition
    /// snapshot, narrowed to the live ECS fortification, reserve, and repair seams.
    /// </summary>
    public struct FortificationHUDComponent : IComponentData
    {
        public FixedString64Bytes SettlementId;
        public FixedString32Bytes OwnerFactionId;
        public int Tier;
        public int OpenBreachCount;
        public int ReserveFrontage;
        public int MusteredDefenderCount;
        public int ReadyReserveCount;
        public int RecoveringReserveCount;
        public bool ThreatActive;
        public float SealingProgress01;
        public float RecoveryProgress01;
    }
}
