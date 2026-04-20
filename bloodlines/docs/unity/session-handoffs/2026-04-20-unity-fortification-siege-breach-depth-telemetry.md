# 2026-04-20 Unity Fortification Siege Breach Depth Telemetry

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-breach-depth-telemetry`.

This slice started from the revision-42 master-equivalent checkout at
`575b824f` and closes the fortification lane's immediate observability follow-up
after sub-slice 9 destroyed-counter recovery. The goal was to expose
settlement-level breach telemetry for both sealing and destroyed-counter
recovery without changing the existing player-facing breach readout shape.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/FortificationCanon.cs`
  - centralized the sealing and destroyed-counter recovery constants so runtime
    systems and telemetry now read the same values
  - added shared canon values for tick rate, stone cost, worker-hours, and keep
    multiplier
- `unity/Assets/_Bloodlines/Code/Fortification/BreachSealingSystem.cs`
  - replaced private hardcoded constants with the shared
    `FortificationCanon.BreachSealing*` values
- `unity/Assets/_Bloodlines/Code/Fortification/DestroyedCounterRecoverySystem.cs`
  - replaced private hardcoded constants with the shared
    `FortificationCanon.DestroyedCounterRecovery*` values
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`
  - preserved the existing `SettlementBreachReadout` surface
  - added `SettlementBreachTelemetry` for structured sealing and recovery state:
    eligibility flags, progress flags, reserved stone, required stone,
    accumulated worker-hours, required worker-hours, normalized progress, and
    current `DestroyedCounterKind` target
  - added `TryDebugGetSettlementBreachTelemetry`; the existing
    `TryDebugGetSettlementBreachReadout` now delegates to it so current callers
    remain stable
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachLegibilityReadoutSmokeValidation.cs`
  - extended the existing validator from 5 phases to 7 phases
  - phase 6 proves half-window breach sealing telemetry
  - phase 7 proves half-window destroyed-counter recovery telemetry with keep
    priority and keep-cost multiplier

## Design Notes

- No production HUD ships in this slice. The surface is intentionally debug-only
  so smoke validation and future UI consumers can read one settlement object
  instead of scanning multiple fortification and recovery components.
- The pre-existing `SettlementBreachReadout` shape was left intact. Telemetry is
  additive so current observability consumers do not break.
- Sealing and recovery costs are now sourced from one canon file. This removes
  the risk of the debug seam drifting away from the actual runtime systems.
- Recovery telemetry only activates when `OpenBreachCount == 0`, which keeps the
  sealing-then-rebuild ordering from sub-slices 8 and 9 explicit in the data
  surface as well as the runtime.
- Repo reality note: the current multi-day directive's Priority 2 description is
  stale. Marriage, gestation, proposal expiration, lesser-house loyalty drift,
  and minor-house levy systems already exist under the retired
  `tier2-batch-dynasty-systems` lane. A future dynasty follow-up should harden
  or extend that surface rather than re-porting it from zero.

## Validation

The slice is green on `D:\BLBDT\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene shells: Bootstrap + Gameplay
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. breach legibility readout smoke

Breach legibility readout smoke phases:

- Phase 1 PASS: intact fortified settlement baseline still returns tier, reserve
  frontage, and zero-breach state
- Phase 2 PASS: single-breach pressure state still returns the expected attack
  and speed multipliers
- Phase 3 PASS: three-breach scaling still caps correctly
- Phase 4 PASS: missing settlement still returns false with a default readout
- Phase 5 PASS: mixed partial destruction still reports the expected destroyed
  wall and gate counts
- Phase 6 PASS: sealing telemetry reports `4/8` worker-hours,
  `60/60` reserved stone, and `0.5` normalized progress with recovery inactive
- Phase 7 PASS: recovery telemetry reports `Keep` target,
  `14/28` worker-hours, `180/180` reserved stone, and `0.5` normalized progress
  with sealing inactive

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-legibility-readout-smoke.log`

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean merge worktree, those gates were run through temporary
  worktree-safe wrapper copies that preserved the same execute methods and pass
  markers while targeting `D:\BLBDT\bloodlines\unity`

Local csproj refresh note:

- this clean worktree did not initially include Unity-generated
  `Assembly-CSharp*.csproj` files
- before the governed `dotnet build` gates, local gitignored copies were
  refreshed so they included the already-landed fortification and AI files plus
  the new breach telemetry validation edits
- both csproj files remain gitignored and are not part of the commit

## Current Readiness

This slice is complete and the fortification lane's immediate observability
follow-up is closed. The branch is ready for push and merge-temp coordination.

## Next Action

1. Push `codex/unity-fortification-breach-depth-telemetry`.
2. Merge it to `master` via the normal merge-temp ceremony.
3. If Lance still wants fortification tuning after merge, use a fresh
   `codex/unity-fortification-*` branch for the optional sealing-cost balance
   pass.
4. Do not open a duplicate zero-code marriages lane. Reconcile any dynasty
   follow-up against the already-landed `MarriageComponents`,
   `MarriageGestationSystem`, `MarriageProposalExpirationSystem`,
   `LesserHouseLoyaltyDriftSystem`, and `MinorHouseLevySystem` surfaces first.
