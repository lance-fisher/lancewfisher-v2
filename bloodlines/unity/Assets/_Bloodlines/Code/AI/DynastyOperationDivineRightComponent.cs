using Unity.Collections;
using Unity.Entities;
using Bloodlines.Components;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to the DynastyOperationComponent
    /// entity that AIDivineRightExecutionSystem creates via
    /// DynastyOperationLimits.BeginOperation. Carries the per-operation
    /// fields the browser stores on the declaration object at
    /// simulation.js:10790-10803 inside startDivineRightDeclaration.
    ///
    /// Field mapping browser -> Unity:
    ///   resolveAt          -> ResolveAtInWorldDays
    ///                          (DIVINE_RIGHT_DECLARATION_DURATION_SECONDS
    ///                          = 180 at simulation.js:9779)
    ///   faithId            -> SourceFaithId (FixedString64Bytes;
    ///                          CovenantId enum mapped to canonical
    ///                          string token)
    ///   doctrinePath       -> DoctrinePath (DoctrinePath enum)
    ///   recognitionShare   -> RecognitionShare (default 0; full
    ///                          recognition surface depends on the
    ///                          global apex-faith share calculator
    ///                          which is not yet ported. Future
    ///                          recognition lane writes the canonical
    ///                          value when it lands.)
    ///   recognitionSharePct-> RecognitionSharePct (default 0; same
    ///                          deferral)
    ///   structureId        -> ActiveApexStructureId (FixedString64Bytes;
    ///                          default empty; depends on the apex
    ///                          structure system not yet ported)
    ///   structureName      -> ActiveApexStructureName (FixedString64Bytes;
    ///                          default empty; same deferral)
    ///
    /// Browser parity notes:
    /// - The browser does NOT route divine right through the
    ///   DYNASTY_OPERATION_ACTIVE_LIMIT cap (startDivineRightDeclaration
    ///   never calls getDynastyOperationsState). Divine right writes
    ///   directly to faction.faith.divineRightDeclaration. Unity DOES
    ///   route through DynastyOperationLimits.BeginOperation for
    ///   surface consistency with the missionary, holy-war, and
    ///   captive-rescue/ransom dispatch consumers; this is a deliberate
    ///   Unity-side departure that simplifies later intel-report and
    ///   resolution-system queries (one shape for all dispatch
    ///   consumers). The browser parity gate "no existing active divine
    ///   right declaration for this faction" is enforced before the
    ///   capacity check so the per-faction one-active-at-a-time browser
    ///   semantic holds even with the Unity-side cap routing.
    /// - The browser also has NO escrow cost or intensity deduction for
    ///   divine right (the cost is the covenant-test prerequisite that
    ///   gates getDivineRightDeclarationTerms). Unity matches: no
    ///   EscrowCost, no IntensityCost. The component carries only the
    ///   declaration-window fields plus the apex-context placeholders.
    /// - The browser declaration also triggers immediate side effects
    ///   on other factions (mutual hostility, AI timer caps; lines
    ///   10813-10828). Unity defers those to a future resolution
    ///   slice; this component captures only the declaration data.
    ///
    /// Browser duration translation: DIVINE_RIGHT_DECLARATION_DURATION_SECONDS
    /// = 180 is real seconds. Unity stores ResolveAtInWorldDays =
    /// current + 180 (using browser numeric value directly on the
    /// in-world timeline), matching the sub-slice 20 missionary and
    /// sub-slice 21 holy-war duration convention.
    ///
    /// No resolution system ships in this slice. The DynastyOperationComponent
    /// remains Active=true with this per-kind component attached; a
    /// future resolution slice walks expired entries at
    /// ResolveAtInWorldDays, evaluates whether recognition share met
    /// the threshold during the window, and either succeeds (apex
    /// faith claim, Divine Right victory progression) or fails (cooldown
    /// + legitimacy penalty per browser failDivineRightDeclaration at
    /// simulation.js:10691).
    /// </summary>
    public struct DynastyOperationDivineRightComponent : IComponentData
    {
        public float                ResolveAtInWorldDays;
        public FixedString64Bytes   SourceFaithId;
        public DoctrinePath         DoctrinePath;
        public float                RecognitionShare;
        public float                RecognitionSharePct;
        public FixedString64Bytes   ActiveApexStructureId;
        public FixedString64Bytes   ActiveApexStructureName;
    }
}
