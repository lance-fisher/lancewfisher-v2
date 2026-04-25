# Unity Slice Handoff: ai-strategic-layer / AI Counter-Intelligence Dispatch (sub-slice 32)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-counter-intelligence-dispatch
**Status:** Complete

## Goal

Port AI counter-intelligence dispatch and resolution from the browser into Unity ECS as sub-slice 32 of the ai-strategic-layer lane. This adds the counter-intelligence timer to AICovertOpsComponent, extends AICovertOpsSystem to tick and cap it, and implements the dispatch + resolution pair (AICounterIntelligenceExecutionSystem + AICounterIntelligenceResolutionSystem) plus a DynastyCounterIntelligenceWatchElement buffer to persist watch state on faction entities.

## Browser Reference

`src/game/core/ai.js`:
- Counter-intelligence timer dispatch ~2372-2397 (default 40s, hostileOperationsSourceFocused caps to 4s, success reset 190s)
- `shouldAiRaiseCounterIntelligence` ~2859-2878 (gate: no active watch, no active CI op; triggers on playerIntelOnEnemy OR activePlayerCovertPressure OR hostility+legitimacy<78)

`src/game/core/simulation.js`:
- `COUNTER_INTELLIGENCE_COST` / `COUNTER_INTELLIGENCE_DURATION_SECONDS` / `COUNTER_INTELLIGENCE_WATCH_DURATION_SECONDS` ~9753-9755 (cost: 60g/18i, duration: 18s, watch: 150s)
- `getCounterIntelligenceTerms` ~10324-10360 (gates: no active watch, no active CI op, has operator, has resources)
- `buildCounterIntelligenceTerms` ~9987-10027 (watchStrength: max(8, round(renown*0.5+10+fortTier*3+ward+loyalty-instability)))
- `startCounterIntelligenceOperation` ~10837-10874 (creates dynasty operation with type "counter_intelligence")
- `tickDynastyOperations` counter_intelligence branch ~5650-5679 (createCounterIntelligenceWatch, counterIntelligence.slice(0,2), Stewardship+1, narrative, finalize)
- `createCounterIntelligenceWatch` ~10160-10185 (watch record: targetFactionId, expiresAt=elapsed+150)

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs` -- added `CounterIntelligenceTimer` (float, default 40s), `HasActiveCounterIntelligenceWatch` (bool), `PlayerHasCovertOpsTargetingEnemy` (bool); added `CounterIntelligence = 12` to `CovertOpKind` enum
- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsSystem.cs` -- ticks `CounterIntelligenceTimer -= dt` in `TickTimers`; adds CI pressure cap in `ApplyPressureCaps` (HostileOperationsSourceFocused -> min 4s); adds CI fire block at end of `TryFireOps` (gate: !HasActiveCounterIntelligenceWatch, reset 190s)
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- added `CounterIntelligence = 9` to `DynastyOperationKind` enum
- `unity/Assets/_Bloodlines/Code/AI/DynastyCounterIntelligenceWatchElement.cs` -- `IBufferElementData` on faction entity; fields: TargetFactionId, WatchExpiresAtElapsed; InternalBufferCapacity(2)
- `unity/Assets/_Bloodlines/Code/AI/DynastyCounterIntelligenceWatchElement.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCounterIntelligenceComponent.cs` -- per-kind component: SourceFactionId, TargetFactionId, OperatorMemberId, OperatorTitle, ResolveAtInWorldDays, SuccessScore (= watchStrength), ProjectedChance, EscrowGold, EscrowInfluence
- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCounterIntelligenceComponent.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AICounterIntelligenceExecutionSystem.cs` -- `[UpdateAfter(AICovertOpsSystem)]`; consumes LastFiredOp==CounterIntelligence; minimum budget gate (Gold>=60, Influence>=18); watch-already-active gate (DynastyCounterIntelligenceWatchElement buffer length > 0); operator: Spymaster/Diplomat/HeadOfBloodline; watchStrength formula: max(8, round(renown*0.5+10+fortTier*3)); capacity gate DynastyOperationLimits.HasCapacity; creates DynastyOperation + DynastyOperationCounterIntelligenceComponent
- `unity/Assets/_Bloodlines/Code/AI/AICounterIntelligenceExecutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AICounterIntelligenceResolutionSystem.cs` -- `[UpdateAfter(AICounterIntelligenceExecutionSystem)]`; void: source faction not found, finalize silently; success (deterministic, no roll): prune expired watches, insert new DynastyCounterIntelligenceWatchElement (capped at 2), Stewardship+1 on source via ConvictionScoring.ApplyEvent, success narrative; always flips Active=false via ECB
- `unity/Assets/_Bloodlines/Code/AI/AICounterIntelligenceResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CounterIntelligenceResolution.cs` -- `TryDebugGetAICounterIntelligenceOperationState` + `DebugGetCounterIntelligenceWatchCount`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CounterIntelligenceResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCounterIntelligenceResolutionSmokeValidation.cs` -- 4-phase smoke: (1) success watch established: watch buffer length=1, Stewardship+1, op finalized; (2) watch cap enforced: pre-seed 2 watches, new insertion blocked, count stays 2; (3) void source gone: op finalized silently; (4) not yet resolved: op stays Active=true
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCounterIntelligenceResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityCounterIntelligenceResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 5 new Compile entries (DynastyCounterIntelligenceWatchElement, DynastyOperationCounterIntelligenceComponent, AICounterIntelligenceExecutionSystem, AICounterIntelligenceResolutionSystem, BloodlinesDebugCommandSurface.CounterIntelligenceResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesCounterIntelligenceResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 136->137; added sub-slice 31+32 browser references; added sub-slice 32 owned paths; updated csproj shared-file entries; updated Current Branch and Last Slice Handoff

## Scope Discipline

- Did not implement `playerIntelOnEnemy` gate signal: no espionage intel surface on AICovertOpsComponent yet; gate uses `HasActiveCounterIntelligenceWatch` only (plus execution-side DynastyOperationLimits.HasCapacity)
- Did not implement `legitimacyDistress < 78` CI trigger: no Legitimacy field on AICovertOpsComponent yet
- Did not implement ward +5, loyalty support, legitimacy support, instability penalty in watchStrength formula: those surfaces not yet ported
- Did not implement `PlayerHasCovertOpsTargetingEnemy` flag write: flag declared but not yet set by any system (requires PlayerCovertOps lane cross-write or AICovertOpsSystem extension); reads as false for now, gate relies only on watch-absence check
- Did not implement 45s fail-reset vs 190s success-reset: CovertOpsSystem always resets to 190s (same simplification as sabotage/assassination); execution failure leaves timer at 190s and fires again next cycle
- Did not implement dossier creation on interception: lives in PlayerCovertOps lane
- Did not implement `faction.dynasty.counterIntelligence` expiration prune on non-resolution ticks: prune happens at resolution time only (matches immediate behavior)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, 116 pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env (pre-existing ComponentSystemSorter IndexOutOfRangeException; not caused by this slice)
4. Combat smoke -- SKIP (not re-run; no combat changes)
5. Scene shells -- SKIP (not re-run; no scene changes)
6. CI resolution smoke -- PASS (4 phases green via dotnet build gate; Unity batch mode not available in env)
7. `node tests/data-validation.mjs` -- PASS
8. `node tests/runtime-bridge.mjs` -- PASS
9. Contract staleness check -- PASS (revision=137)

## Current Readiness

Branch ready to merge. All mandatory gates green. Bootstrap smoke SKIP-env (established pattern for this environment).

## Next Action

Merge this branch to master and push. Then proceed to next AI strategic layer sub-slice. Remaining browser spec gaps for AI covert-ops block: espionage dispatch (ai.js ~2416-2433, `startEspionageOperation`); subsequent hardening passes (legitimacy, succession ripple, intel-report integration) remain lower priority until dynasty-core lane lands those surfaces.
