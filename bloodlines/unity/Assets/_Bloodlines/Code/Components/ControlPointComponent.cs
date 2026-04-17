using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Control point on the battlefield. Canonical territorial atom.
    /// Browser runtime equivalent: world.controlPoints entries.
    ///
    /// OwnerFactionId is the currently-holding faction. Loyalty accrues toward
    /// stabilization at the canonical threshold, at which point ControlState transitions
    /// from Occupied to Stabilized and the governor specialization may rotate.
    /// CaptureFactionId tracks the active claimant while CaptureProgress rises.
    /// IsContested is kept separate from ControlState so an occupied or stabilized point
    /// can be contested temporarily without losing its underlying territorial identity.
    /// </summary>
    public struct ControlPointComponent : IComponentData
    {
        public FixedString32Bytes ControlPointId;
        public FixedString32Bytes OwnerFactionId;
        public FixedString32Bytes CaptureFactionId;
        public FixedString32Bytes ContinentId;
        public ControlState ControlState;
        public bool IsContested;
        public float Loyalty;
        public float CaptureProgress;
        public FixedString32Bytes SettlementClassId;
        public int FortificationTier;
        public float RadiusTiles;
        public float CaptureTimeSeconds;
        public float GoldTrickle;
        public float FoodTrickle;
        public float WaterTrickle;
        public float WoodTrickle;
        public float StoneTrickle;
        public float IronTrickle;
        public float InfluenceTrickle;
    }

    public enum ControlState : byte
    {
        Neutral = 0,
        Occupied = 1,
        Stabilized = 2,
        Contested = 3,
    }
}
