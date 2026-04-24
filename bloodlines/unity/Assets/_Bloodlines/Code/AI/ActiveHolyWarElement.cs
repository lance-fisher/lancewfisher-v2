using Unity.Collections;
using Unity.Entities;
using Bloodlines.Components;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-faction dynamic buffer element tracking one active holy war
    /// declared by this faction. Stored on the source faction entity.
    /// Browser equivalent: an entry in faction.faith.activeHolyWars
    /// (simulation.js createHolyWarEntry at ~10505).
    ///
    /// Field mapping browser -> Unity:
    ///   id            -> Id
    ///   targetFactionId -> TargetFactionId
    ///   faithId       -> FaithId (CovenantId enum)
    ///   doctrinePath  -> DocPath (DoctrinePath enum)
    ///   declaredAt    -> DeclaredAtInWorldDays
    ///   lastPulseAt   -> LastPulseAtInWorldDays
    ///   expiresAt     -> ExpiresAtInWorldDays
    ///   intensityPulse -> IntensityPulse
    ///   loyaltyPulse  -> LoyaltyPulse
    ///
    /// Browser bounds: faction.faith.activeHolyWars is capped at 6
    /// entries (simulation.js:4211, :5610). AIHolyWarResolutionSystem
    /// enforces the same cap when inserting entries.
    ///
    /// Populated by AIHolyWarResolutionSystem Phase A at declaration
    /// resolution (when InWorldDays >= DynastyOperationHolyWarComponent
    /// .ResolveAtInWorldDays). Sustained effects and expiration are
    /// processed by AIHolyWarResolutionSystem Phase B each tick.
    ///
    /// Duration convention: browser expiresAt is stored in real elapsed
    /// seconds (state.meta.elapsed + HOLY_WAR_DURATION_SECONDS = 180).
    /// Unity stores ExpiresAtInWorldDays using the same numeric value
    /// directly on the in-world timeline (current + 180), matching the
    /// convention established in sub-slice 21
    /// (AIHolyWarExecutionSystem.HolyWarDurationInWorldDays = 180f).
    ///
    /// Pulse interval convention: browser HOLY_WAR_PULSE_INTERVAL_SECONDS
    /// = 30. Unity converts to in-world days using the canonical
    /// DaysPerRealSecond = 2: PulseIntervalInWorldDays = 60f. Stored in
    /// LastPulseAtInWorldDays and compared against InWorldDays.
    /// </summary>
    public struct ActiveHolyWarElement : IBufferElementData
    {
        public FixedString64Bytes Id;
        public FixedString32Bytes TargetFactionId;
        public CovenantId         FaithId;
        public DoctrinePath       DocPath;
        public float              DeclaredAtInWorldDays;
        public float              LastPulseAtInWorldDays;
        public float              ExpiresAtInWorldDays;
        public float              IntensityPulse;
        public float              LoyaltyPulse;
    }
}
