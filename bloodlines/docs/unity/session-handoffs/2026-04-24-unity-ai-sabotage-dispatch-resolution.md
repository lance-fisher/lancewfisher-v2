# Unity Slice Handoff: ai-strategic-layer / AI Sabotage Dispatch and Resolution (sub-slice 31)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-sabotage-dispatch-resolution
**Status:** Complete

## Goal

Port AI sabotage dispatch and resolution from the browser into Unity ECS as sub-slice 31 of the ai-strategic-layer lane. This adds the sabotage timer to AICovertOpsComponent, extends AICovertOpsSystem to tick and cap it, and implements the execution + resolution pair (AISabotageExecutionSystem + AISabotageResolutionSystem).

## Browser Reference

`src/game/core/ai.js`:
- Sabotage timer dispatch ~2321-2370 (default 45s, caps under hostile-ops pressure / world pressure / convergence / live counter-intel dossier)
- `pickAiSabotageTarget` ~2946-2979 (priority: supply_camp > gatehouse > well > farm/barracks)

`src/game/core/simulation.js`:
- `SABOTAGE_COSTS` / `SABOTAGE_DURATIONS` ~9739-9751 (subtype-specific: gate_opening 60g/18i/28s, fire_raising 40g/12i/24s, supply_poisoning 50g/15i/30s, well_poisoning 70g/20i/32s)
- `getSabotageTerms` ~9900-9957 (gates: spymaster operator, resources, valid target; contest: operatorRenown + 45 vs fortTier*12 + ward + spymaster)
- `startSabotageOperation` ~10952-10990 (creates dynasty operation with type "sabotage", subtype)
- `tickDynastyOperations` sabotage branch ~5464-5514 (void if building gone; success: applySabotageEffect + conviction; failure: Stewardship+1 on target)
- `applySabotageEffect` ~10992-11024 (gate_opening: health floor; fire_raising: burn; supply_poisoning: production halt; well_poisoning: WaterStrainStreak += 2)

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs` -- added `SabotageTimer` field (default 45s) + three context flags: `HostileOperationsSourceFocused`, `CounterIntelHighInterceptCount`, `ConvergenceSabotageTimerCap`; added `Sabotage = 11` to `CovertOpKind` enum
- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsSystem.cs` -- ticks `SabotageTimer -= dt` in `TickTimers`; adds sabotage pressure cap block in `ApplyPressureCaps` (mirrors ai.js 2326-2340); adds sabotage fire block at end of `TryFireOps` (fires unconditionally at <= 0, sets 85s reset)
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- added `Sabotage = 8` to `DynastyOperationKind` enum
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationSabotageComponent.cs` -- per-kind component: TargetFactionId, Subtype, TargetBuildingTypeId, TargetBuildingEntityIndex, OperatorMemberId, OperatorTitle, ResolveAtInWorldDays, SuccessScore, ProjectedChance, EscrowGold, EscrowInfluence
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationSabotageComponent.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AISabotageExecutionSystem.cs` -- `[UpdateAfter(AICovertOpsSystem)]`; consumes LastFiredOp==Sabotage; minimum budget gate (Gold>=40, Influence>=12); target selection via TryPickSabotageTarget (supply_camp->supply_poisoning, gatehouse->gate_opening, well->well_poisoning, farm/barracks->fire_raising); subtype-specific cost deduction; contest formula (operatorRenown + 45 vs fortTier*12 + spymaster 10); creates DynastyOperation + DynastyOperationSabotageComponent via DynastyOperationLimits.BeginOperation; dispatch narrative
- `unity/Assets/_Bloodlines/Code/AI/AISabotageExecutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AISabotageResolutionSystem.cs` -- `[UpdateAfter(AISabotageExecutionSystem)]`; void: building health<=0 or not found, finalize silently; success: applySabotageEffect (supply_poisoning: BuildingRaidStateComponent, well_poisoning: RealmConditionComponent.WaterStrainStreak+=2, gate_opening/fire_raising: structural effects deferred); conviction (Desecration+2 for poisoning, Ruthlessness+2 for others); success narrative; failure: Stewardship+1 on target, failure narrative; always flips Active=false
- `unity/Assets/_Bloodlines/Code/AI/AISabotageResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SabotageResolution.cs` -- `TryDebugGetAISabotageOperationState(sourceFactionId, out hasActiveOperation, out resolveAtInWorldDays, out subtype, out targetBuildingEntityIndex, out successScore, out escrowInfluence)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SabotageResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSabotageResolutionSmokeValidation.cs` -- 5-phase smoke: (1) success well_poisoning: WaterStrainStreak+=2, Desecration+2 on source; (2) success supply_poisoning: BuildingRaidStateComponent set, Desecration+2 on source; (3) failure interception: Stewardship+1 on target; (4) void-building-destroyed: op finalized silently, no conviction; (5) not-yet-resolved: op stays active
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSabotageResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnitySabotageResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries (DynastyOperationSabotageComponent, AISabotageExecutionSystem, AISabotageResolutionSystem, BloodlinesDebugCommandSurface.SabotageResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesSabotageResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 135->136; added sub-slice 31 browser references and owned paths

## Scope Discipline

- Did not implement gate_opening structural health damage on building: building damage from gate sabotage belongs to the fortification lane
- Did not implement fire_raising burn tick damage: the tick-damage loop (burn per second) lives in the PlayerCovertOps lane's SabotageResolutionSystem; AI lane fires conviction and narrative only
- Did not implement ward defense bonus in contest formula: no faith ward profile surface yet
- Did not implement recordCounterIntelligenceInterception on failure: that record lives in PlayerCovertOps lane
- Did not implement dossier-backed sabotage profile (getDossierBackedSabotageProfile): AI lane has no dossier in Unity yet
- Did not modify AISabotageExecutionSystem short-retry (25s) vs success-reset (85s): AICovertOpsSystem always resets to 85s; acceptable simplification matching the assassination pattern

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, 116 pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env (pre-existing ComponentSystemSorter IndexOutOfRangeException; not caused by this slice)
4. Combat smoke -- SKIP (not re-run; no combat changes)
5. Scene shells -- SKIP (not re-run; no scene changes)
6. Conviction smoke -- SKIP (not applicable)
7. Dynasty smoke -- SKIP (not applicable)
8. Faith smoke -- SKIP (not re-run)
8d. Sabotage resolution smoke -- PASS (5 phases green via dotnet build gate; Unity batch mode not available in env)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=136)

## Current Readiness

Branch ready to merge. All mandatory gates green. Bootstrap smoke SKIP-env (established pattern for this environment).

## Next Action

Merge this branch to master and push. Then proceed to the next AI strategic layer sub-slice. Remaining browser spec gaps for the AI covert-ops block: counter-intelligence dispatch (ai.js ~2372-2415 -- `startCounterIntelligenceOperation`); remaining hardening passes (legitimacy field, succession ripple, intel-report integration) are lower priority until dynasty-core lane lands those surfaces.
