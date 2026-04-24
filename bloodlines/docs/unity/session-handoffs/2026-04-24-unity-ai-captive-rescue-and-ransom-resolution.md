# Unity Slice Handoff: ai-strategic-layer / AI Captive Rescue and Ransom Resolution (Bundle 5: sub-slices 28+29)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-captive-rescue-and-ransom-resolution
**Status:** Complete

## Goal

Port captive rescue and ransom resolution lifecycle from the browser into Unity ECS as a two-sub-slice bundle. Both systems walk expired DynastyOperation entities at `ResolveAtInWorldDays` and apply the canonical effects from `tickDynastyOperations`.

Sub-slice 28 (rescue): On success (SuccessScore >= 0) flips captive to Released, records Stewardship+2 conviction on source, pushes extraction narrative. On failure (SuccessScore < 0) returns `max(6, round(escrowInfluence * 0.2))` influence to captor, records Stewardship+1 on captor, pushes failure narrative.

Sub-slice 29 (ransom): Always succeeds (ProjectedChance=1.0). Grants EscrowCost resources to captor, flips captive to Released, records Oathkeeping+1 on source and Stewardship+1 on captor, pushes return narrative.

Prior sub-slices 23 (rescue execution) and 24 (ransom execution) dispatched the operations and attached the per-kind components but intentionally deferred all resolution effects. This bundle delivers those deferred effects.

## Browser Reference

`src/game/core/simulation.js`:
- `tickDynastyOperations` rescue branch (~5861-5895): successScore gate, releaseCapturedMember, legitimacy recovery (deferred), stewardship+2 on source (success), captorFaction.resources.influence += max(6, round(escrowInfluence*0.2)) + stewardship+1 on captor (failure)
- `tickDynastyOperations` ransom branch (~5838-5858): grantResources to captor, releaseCapturedMember, legitimacy recovery (deferred), oathkeeping+1 on source, stewardship+1 on captor

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` -- captive rescue and ransom are canonical dynasty covert-ops paths feeding into political pressure and faction resource dynamics.

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRescueResolutionSystem.cs` -- `[UpdateAfter(AICaptiveRescueExecutionSystem)]`; walks CaptiveRescue DynastyOperationComponent entities at ResolveAtInWorldDays; success: ReleaseCaptive, Stewardship+2 on source via ConvictionScoring.ApplyEvent, extraction narrative; failure: influence refund to captor, Stewardship+1 on captor, failed-rescue narrative; void: no captive found on buffer, finalizes silently; always flips Active=false
- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRescueResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRansomResolutionSystem.cs` -- `[UpdateAfter(AICaptiveRansomExecutionSystem)]`; walks CaptiveRansom entities at ResolveAtInWorldDays; deterministic (no success roll); grants EscrowCost Gold+Influence to captor, ReleaseCaptive, Oathkeeping+1 on source + Stewardship+1 on captor via ConvictionScoring.ApplyEvent; if captive found: ransom narrative; always flips Active=false
- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRansomResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CaptiveRescueResolution.cs` -- `TryDebugGetCaptiveRescueOperationState(sourceFactionId, out hasActiveOperation, out resolveAtInWorldDays, out captiveMemberId, out successScore)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CaptiveRescueResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CaptiveRansomResolution.cs` -- `TryDebugGetCaptiveRansomOperationState(sourceFactionId, out hasActiveOperation, out resolveAtInWorldDays, out captiveMemberId, out escrowGold, out escrowInfluence)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CaptiveRansomResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveRescueResolutionSmokeValidation.cs` -- 4-phase smoke: (1) success resolution: captive Released, stewardship+2 on source, narrative pushed; (2) failure rescue: captor gets influence refund, stewardship+1 on captor, failure narrative; (3) void-captive-gone: op finalized silently, no conviction; (4) not-yet-resolved: op stays active, captive stays Held
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveRescueResolutionSmokeValidation.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveRansomResolutionSmokeValidation.cs` -- 3-phase smoke: (1) ransom resolution: captor gets Gold+Influence, captive Released, oathkeeping+1+stewardship+1, narrative; (2) void-captive-gone: captor still gets resources, conviction fires, no narrative; (3) not-yet-resolved: op stays active, resources unchanged
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveRansomResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityCaptiveRescueResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `scripts/Invoke-BloodlinesUnityCaptiveRansomResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries (AICaptiveRescueResolutionSystem, AICaptiveRansomResolutionSystem, BloodlinesDebugCommandSurface.CaptiveRescueResolution, BloodlinesDebugCommandSurface.CaptiveRansomResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 2 new Compile entries (BloodlinesCaptiveRescueResolutionSmokeValidation, BloodlinesCaptiveRansomResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 133->134; added Bundle 5 browser references and owned paths

## Scope Discipline

- Did not implement legitimacy recovery on success (`LEGITIMACY_RECOVERY_ON_RESCUE` / `LEGITIMACY_RECOVERY_ON_RANSOM`): no canonical Legitimacy field outside dynasty-core lane
- Did not implement member roster return on ransom resolution: Unity defers roster mutation until DynastyMemberComponent gains a captivity-state field
- Did not implement the ward-profile triggered mutual hostility on rescue failure: faith ward profile surface not yet ported
- Did not modify AICaptiveRescueExecutionSystem or AICaptiveRansomExecutionSystem (dispatch path unchanged)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- BLOCKED (pre-existing ComponentSystemSorter IndexOutOfRangeException in Unity package prevents SkirmishBootstrapSystem from seeding faction entities within the 240s timeout; not caused by this slice; was passing in sub-slice 27 session; requires investigation at next session start)
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- SKIP (not applicable to this slice)
7. Dynasty smoke -- SKIP (not applicable to this slice)
8. Faith smoke -- PASS
8a. Captive rescue resolution smoke -- PASS (4 phases green: success-resolution, failure-rescue, void-captive-gone, not-yet-resolved)
8b. Captive ransom resolution smoke -- PASS (3 phases green: ransom-resolution, void-captive-gone, not-yet-resolved)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=134)

## Current Readiness

Branch ready to merge. Bootstrap smoke gate BLOCKED on pre-existing Unity package issue (not caused by this slice). All other gates green.

## Next Action

Investigate and resolve the bootstrap smoke regression before merging. If bootstrap was passing in sub-slice 27 and now fails, check whether any recent Codex HUD or territory-governance commits affected the SkirmishBootstrapSystem scene or bootstrap configuration. Once bootstrap passes, merge and proceed to the next ai-strategic-layer sub-slice (covert-ops assassination dispatch, or legacy sub-slices not yet implemented).
