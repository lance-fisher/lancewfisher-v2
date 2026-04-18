# 2026-04-18 Unity Fortification Siege Sub-slice 3 Imminent Engagement

## Goal

Port `tickImminentEngagementWarnings` from the browser runtime into Unity 6.3
DOTS/ECS for fortified settlements only:

- canonical imminent-engagement warning constants
- settlement warning-window runtime state
- hostile scan, warning-window activation, posture selection, and expiry
- counterstroke reserve sortie behavior for primary keeps

## Files Created

- `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementCanon.cs`
- `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementComponent.cs`
- `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementWarningSystem.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementSmokeValidation.cs`
- `scripts/Invoke-BloodlinesUnityImminentEngagementSmokeValidation.ps1`

## Files Changed

- `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - now resolves project and artifact paths relative to the active repo root so
    gate 6 can run against `D:\BLFS\bloodlines` instead of the dirty canonical
    checkout
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md`
  - revision `13 -> 14`, imminent-engagement files added to the fortification
    lane, stale unclaimed sub-slice-3 candidate removed, last-slice handoff
    updated
- local-only, gitignored build registration:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`
  - both were updated on disk only so `dotnet build` includes the new imminent
    engagement files and the already-merged AI/world-pressure files missing from
    the local project files

## Smoke Test Results

- Dedicated imminent-engagement smoke:
  `Imminent engagement smoke validation passed.`
  - Phase 1: tier-0 settlement did not activate
  - Phase 2: tier-1 settlement without nearby hostiles stayed inactive
  - Phase 3: tier-1 settlement with three hostile units inside warning radius
    activated with canonical clamped warning seconds
  - Phase 4: expired warning window set `WindowConsumed=True` and wrote
    `EngagedAt`
- Preserved fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True,
  tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
- Preserved siege smoke:
  wrapper exited `0`; governed pass marker remained green in
  `artifacts/unity-siege-smoke.log`

## Validation Gate Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
   - PASS: `Build succeeded. 0 Warning(s), 0 Error(s).`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
   - PASS: `Build succeeded. 111 Warning(s), 0 Error(s).`
3. Bootstrap runtime smoke
   - PASS: `Bootstrap runtime smoke validation passed.`
4. Combat smoke
   - PASS: wrapper exited `0`
5. Canonical scene-shell validation
   - PASS: bootstrap and gameplay scene shell validators both passed
6. Fortification smoke
   - PASS: `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
7. Siege smoke
   - PASS: wrapper exited `0`
8. `node tests/data-validation.mjs`
   - PASS: `Bloodlines data validation passed.`
9. `node tests/runtime-bridge.mjs`
   - PASS: `Bloodlines runtime bridge validation passed.`
10. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
   - PASS: contract revision `14` is current

## Current Readiness

Sub-slice 3 is complete on `codex/unity-fortification-siege` after rebasing onto
current `origin/master` and verifying merge-base
`7671cc32aa886e7f0cb02b13cf4ad44711293d60`. The fortification lane now has
canonical imminent-engagement warning windows, AI posture selection
(`Steady/Brace/Counterstroke`), hostile-count observability, and keep-only
counterstroke reserve sortie behavior in isolated ECS validation. The branch is
ready for merge coordination back to `master`.

## Next Action

1. Merge `codex/unity-fortification-siege` to `master`.
2. Retire or refresh `fortification-siege-imminent-engagement` ownership in
   `docs/unity/CONCURRENT_SESSION_CONTRACT.md` and `continuity/PROJECT_STATE.json`
   after the merge lands.
3. Do not widen this branch into bootstrap integration, siege lifecycle
   progression, or HUD sortie readouts before merge coordination completes.

## Browser Reference

- `src/game/core/simulation.js:199-205` - imminent-engagement constants block
- `src/game/core/simulation.js:271` - imminent-engagement postures
- `src/game/core/simulation.js:11509` - `getSettlementImminentEngagementProfile`
- `src/game/core/simulation.js:11720` - `chooseAiImminentEngagementResponse`
- `src/game/core/simulation.js:11745` - `tickImminentEngagementWarnings`

## Canon Reference

- `04_SYSTEMS/TERRITORY_SYSTEM.md`
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`

## Validation Notes

- Because the canonical checkout at `D:\ProjectsHome\Bloodlines` was dirty on a
  different branch, gates 3 and 5 were run through temporary BLFS-local copies
  of the shared bootstrap and scene-shell wrappers so Unity validated
  `D:\BLFS\bloodlines`. Those temporary wrapper copies are not committed.
