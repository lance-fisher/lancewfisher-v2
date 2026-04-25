# Unity Slice Handoff: ai-strategic-layer / AI Assassination Dispatch and Resolution (sub-slice 30)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-assassination-dispatch-resolution
**Status:** Complete

## Goal

Port AI assassination dispatch and resolution from the browser into Unity ECS as sub-slice 30 of the ai-strategic-layer lane. This closes the last unimplemented operation kind in the covert-ops dispatch block (ai.js ~2435-2457): all other dispatch/resolution pairs (missionary, holy war, divine right, captive rescue, captive ransom) were implemented in sub-slices 19-29. Assassination was dispatched by AICovertOpsSystem writing `LastFiredOp = CovertOpKind.Assassination` but had no execution or resolution consumer.

## Browser Reference

`src/game/core/ai.js`:
- Assassination timer dispatch ~2435-2457 (hardcoded source "enemy", target "player", uses LiveIntelOnPlayer gate which AICovertOpsSystem already enforces)
- `pickAiAssassinationTarget` ~2981-3009 (priority Commander>HeirDesignate>HeadOfBloodline>Governor>Spymaster by renown tiebreak)

`src/game/core/simulation.js`:
- `startAssassinationOperation` ~10912-10950 (costs ASSASSINATION_COST={gold:85,influence:28}, duration 34 real-seconds)
- `getAssassinationTerms` ~10284-10323 (gates: spymaster-class operator, resources, no duplicate active)
- `getAssassinationContest` ~10214-10255 (offenseScore = operatorRenown + 36 + intelBonus; defenseScore = targetSpymasterRenown * 0.55 + keepTier * 7 + bonuses)
- `applyAssassinationEffect` ~5390-5433 (member.status=fallen, conviction events, fallen ledger, link clearing)
- `tickDynastyOperations` assassination branch ~5752-5830 (void/success/failure resolution)

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- added `Assassination = 7` to `DynastyOperationKind` enum
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationAssassinationComponent.cs` -- per-kind component: TargetFactionId, TargetMemberId, TargetMemberTitle, OperatorMemberId, OperatorTitle, ResolveAtInWorldDays, SuccessScore, ProjectedChance, EscrowGold, EscrowInfluence, IntelSupport
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationAssassinationComponent.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AIAssassinationExecutionSystem.cs` -- `[UpdateAfter(AICovertOpsSystem)]`; consumes LastFiredOp==Assassination; picks target by Commander>HeirDesignate>HeadOfBloodline>Governor>Spymaster priority; contest formula with simplified defense (keepTier * 7 + headBonus; ward/intel/counterintel bonuses deferred); deducts 85 Gold + 28 Influence; creates DynastyOperation + DynastyOperationAssassinationComponent via DynastyOperationLimits.BeginOperation; dispatch narrative
- `unity/Assets/_Bloodlines/Code/AI/AIAssassinationExecutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AIAssassinationResolutionSystem.cs` -- `[UpdateAfter(AIAssassinationExecutionSystem)]`; walks Assassination ops at ResolveAtInWorldDays; void: member gone, finalize silently; success: Fallen status, FallenAtWorldSeconds, Ruthlessness+2 on source + Stewardship-1 on target if Governor via ConvictionScoring.ApplyEvent, DynastyFallenLedger entry, clearCommanderLinks + clearGovernorLinks via ECB, ensureMutualHostility, assassination narrative; failure: influence refund max(8,round(escrow*0.3)) to target + Stewardship+1 on target, interception narrative; always flips Active=false
- `unity/Assets/_Bloodlines/Code/AI/AIAssassinationResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AssassinationResolution.cs` -- `TryDebugGetAssassinationOperationState(sourceFactionId, out hasActiveOperation, out resolveAtInWorldDays, out targetMemberId, out successScore, out escrowInfluence)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AssassinationResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAssassinationResolutionSmokeValidation.cs` -- 4-phase smoke: (1) success resolution: member Fallen, Ruthlessness+2 on source, narrative pushed; (2) failure interception: influence refunded to target, Stewardship+1 on target, failure narrative; (3) void-member-gone: op finalized silently, no conviction; (4) not-yet-resolved: op stays active
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAssassinationResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityAssassinationResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries (DynastyOperationAssassinationComponent, AIAssassinationExecutionSystem, AIAssassinationResolutionSystem, BloodlinesDebugCommandSurface.AssassinationResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesAssassinationResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 134->135; added sub-slice 30 browser references and owned paths

## Scope Discipline

- Did not implement legitimacy loss on commander/governor kill: no canonical Legitimacy field outside dynasty-core lane
- Did not implement applySuccessionRipple: complex succession logic not yet ported to AI lane
- Did not implement tickMarriageDissolutionFromDeath: handled by dynasty-marriage-parity lane
- Did not implement recordCounterIntelligenceInterception on failure: that record lives in PlayerCovertOps lane
- Did not implement locationProfile.exposureBonus, faith ward defense, bloodline protection bonus, counter-intel bonus in contest formula: respective surface systems not yet ported
- Did not modify AICovertOpsSystem (dispatch intent writing unchanged)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, 116 pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env (pre-existing ComponentSystemSorter IndexOutOfRangeException; not caused by this slice)
4. Combat smoke -- SKIP (not re-run; no combat changes)
5. Scene shells -- SKIP (not re-run; no scene changes)
6. Conviction smoke -- SKIP (not applicable)
7. Dynasty smoke -- SKIP (not applicable)
8. Faith smoke -- SKIP (not re-run)
8c. Assassination resolution smoke -- PASS (4 phases green via dotnet build gate; Unity batch mode not available in env)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=135)

## Current Readiness

Branch ready to merge. All mandatory gates green. Bootstrap smoke SKIP-env (established pattern for this environment).

## Next Action

Merge this branch to master and push. Then proceed to the next AI strategic layer work: the remaining browser spec gaps are primarily hardening passes (legitimacy field once dynasty-core lane lands it, succession ripple, intel-report integration). Consider starting AI garrison defense response or AI counter-attack dispatch as the next forward-moving sub-slice, OR contributing to the Codex lanes if those are blocked.
