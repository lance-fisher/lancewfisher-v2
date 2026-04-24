# Unity Slice Handoff: ai-strategic-layer / AI Divine Right Resolution (sub-slice 27)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-divine-right-resolution
**Status:** Complete

## Goal

Port the divine right resolution lifecycle from the browser into Unity ECS. Fires at `DynastyOperationDivineRightComponent.ResolveAtInWorldDays` or earlier if the source faction loses its faith commitment during the declaration window. On success (window closes with faith intact) applies conviction event and completion narrative. On failure (faith lost mid-window) drains intensity by 18, applies conviction event, and pushes failure narrative. Also fires the conviction event (+3 oathkeeping or desecration) that `AIDivineRightExecutionSystem` (sub-slice 22) intentionally deferred from dispatch time.

Prior sub-slice 22 dispatched the divine right declaration and attached `DynastyOperationDivineRightComponent` but intentionally deferred all resolution effects. This slice delivers those deferred effects.

## Browser Reference

`src/game/core/simulation.js`:
- `tickFaithDivineRightDeclarations` (~10747-10782): per-tick failure and success checks
- `failDivineRightDeclaration` (~10691-10713): failure effects (intensity drain, legitimacy penalty deferred, cooldown deferred, narrative)
- `completeDivineRightDeclaration` (~10715-10741): success effects (legitimacy deferred, game-over deferred, narrative)
- `startDivineRightDeclaration` conviction event (~10806-10812): deferred dispatch-time +3 (dark=desecration / light=oathkeeping)
- Constants: `DIVINE_RIGHT_FAILURE_INTENSITY_LOSS = 18` (line 9783), `DIVINE_RIGHT_INTENSITY_THRESHOLD = 80` (line 9782)

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` -- divine right resolution is a canonical victory path; faith intensity pressure and conviction events feed directly into multi-faction political pressure dynamics.

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/AIDivineRightResolutionSystem.cs` -- `[UpdateAfter(AIDivineRightExecutionSystem)]`; walks `DynastyOperationDivineRightComponent` entities (Active=true); failure path (source faith lost or intensity below 80): drains intensity by 18, fires conviction +3, pushes failure narrative, flips Active=false; success path (inWorldDays >= ResolveAtInWorldDays with faith intact): fires conviction +3, pushes completion narrative, flips Active=false; void path (source entity missing): silently finalizes
- `unity/Assets/_Bloodlines/Code/AI/AIDivineRightResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DivineRightResolution.cs` -- `TryDebugGetDivineRightOperationState(factionId, out hasActiveOperation, out resolveAtInWorldDays, out sourceFaithId, out darkDoctrine)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DivineRightResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDivineRightResolutionSmokeValidation.cs` -- 4-phase smoke: (1) success resolution: op finalized, oathkeeping +3, completion narrative; (2) failure-faith-lost: op finalized, intensity 85->67, desecration +3, failure narrative; (3) failure-intensity-dropped: op finalized, intensity 40->22, oathkeeping +3, failure narrative; (4) void-source-gone: op finalized silently
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDivineRightResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityDivineRightResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries (AIDivineRightResolutionSystem, BloodlinesDebugCommandSurface.DivineRightResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesDivineRightResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 132->133; added sub-slice 27 browser reference and owned paths

## Scope Discipline

- Did not modify `AIDivineRightExecutionSystem` (dispatch path unchanged)
- Did not implement legitimacy adjustment (+12 on success / -10 on failure): no canonical Legitimacy field outside dynasty-core lane; deferred
- Did not implement game-over signaling: no game-status component in this lane; deferred
- Did not implement cooldown surface after failure: deferred per sub-slice 22 scope note
- Did not implement recognition share check: global apex-faith share calculator not yet ported; resolution treats inWorldDays >= ResolveAt as sole success condition
- Did not implement apex structure check: apex structure surface not yet ported
- Did not implement ensureMutualHostility at declaration time: hostility component pair not yet ported

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Divine right resolution smoke -- PASS (4 phases green: success resolution, failure-faith-lost, failure-intensity-dropped, void-source-gone)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=133)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to sub-slice 28: missionary resolution effects -- fires at `DynastyOperationMissionaryComponent.ResolveAtInWorldDays`; on success applies faith exposure gain to target faction; on failure applies faith intensity erosion to target; records conviction events per browser tickDynastyOperations missionary branch (~5520-5590).
