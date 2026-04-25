# Session Handoff: Sub-slice 33 -- AI Espionage Dispatch and Resolution

## Goal

Implement the AI espionage operation dispatch and resolution pipeline: `AIEspionageExecutionSystem` consumes the `CovertOpKind.Espionage` intent written by `AICovertOpsSystem` and creates a `DynastyOperationEspionageComponent`; `AIEspionageResolutionSystem` matures that operation into an `IntelligenceReportElement` (success) or a counter-intelligence interception (failure).

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationEspionageComponent.cs` -- per-operation data struct (SourceFactionId, TargetFactionId, OperatorMemberId, OperatorTitle, ResolveAtInWorldDays, ReportExpiresAtInWorldDays, SuccessScore, ProjectedChance, EscrowGold, EscrowInfluence).
- `unity/Assets/_Bloodlines/Code/AI/AIEspionageExecutionSystem.cs` -- consumes `LastFiredOp == Espionage`; gates: different faction, target exists, no active espionage on target, operator found (Spymaster/Diplomat/Merchant), resources >= (gold 45, influence 16), capacity. Contest: `offenseScore = operatorRenown + 32`; `defenseScore = targetSpymasterRenown * 0.55f + fortTier * 6f`; `projectedChance = clamp(0.5f + successScore/100f, 0.08f, 0.92f)`. Duration 30 world-days; report expiry 120 world-days.
- `unity/Assets/_Bloodlines/Code/AI/AIEspionageResolutionSystem.cs` -- resolves at `ResolveAtInWorldDays <= inWorldDays`. Success (SuccessScore >= 0): `TryCreateIntelligenceReport` + `StoreIntelligenceReport` on source faction; Stewardship+1 on source. Failure (SuccessScore < 0): `RecordCounterIntelligenceInterception`; `EnsureMutualHostility`; Stewardship+1 on target. Both paths: ECB deactivates op. Void: target faction missing, deactivate silently.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.EspionageResolution.cs` -- debug surface with `TryDebugGetAIEspionageOperationState` and `RunBatchEspionageResolutionSmokeValidation`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesEspionageResolutionSmokeValidation.cs` -- 4-phase smoke validator (success-report-created, void-target-gone, failure-interception, not-yet-resolved). Marker: `BLOODLINES_ESPIONAGE_RESOLUTION_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnityEspionageResolutionSmokeValidation.ps1` -- PS1 wrapper.

### Modified Files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- `DynastyOperationKind.Espionage = 10` added.
- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs` -- `EspionageTimer`, `HasActiveEspionageOnPlayer`, `ConvergenceEspionageTimerCap`, `CovertOpKind.Espionage = 13` added.
- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsSystem.cs` -- `EspionageTimer` tick, espionage pressure caps, espionage fire block with gate and conditional retry (90s/30s blocked, 150s on fire).
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries.
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 138, espionage paths added to owned paths and lane authority docs.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 138 confirmed current.

Bootstrap, combat, canonical-scene-shells, and espionage-resolution Unity batch validators require a live Unity editor and are deferred to the next Unity Editor session.

## Current Readiness

Branch `claude/unity-ai-espionage-dispatch` is ready to merge to master.

## Next Action

Merge sub-slice 33 to master. Next sub-slice candidates in the ai-strategic-layer lane include further AI covert operation resolution systems or the next unimplemented browser-spec path.

## Browser Reference

- `src/game/core/ai.js` espionage timer block ~2399-2417 (gate, pressure caps, fire).
- `src/game/core/simulation.js` `startEspionageOperation` ~10876-10910 (dispatch).
- `src/game/core/simulation.js` `resolveEspionageOperation` ~10920-10970 (resolution).
- `getEspionageContest` ~10187-10212 (offense/defense formula).
