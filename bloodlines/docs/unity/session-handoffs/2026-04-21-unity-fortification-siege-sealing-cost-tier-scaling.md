# 2026-04-21 Unity Fortification Siege Sealing Cost Tier Scaling

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-sealing-cost-tier-scaling`.

This slice started from `origin/master` `7cc7bed6` at contract revision `46`
after sub-slice 10 breach-depth telemetry and the dynasty parity follow-ups had
already landed. The goal was to replace the flat breach-sealing economy with
the tier-aware browser-style scaling: Tier 1 stays at `60` stone and `8`
worker-hours, Tier 2 scales to `90` stone and `12` worker-hours, and Tier 3
scales to `135` stone and `18` worker-hours.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/FortificationCanon.cs`
  - added tier-aware breach-sealing cost helpers:
    `ResolveBreachSealingStoneCostPerBreach` and
    `ResolveBreachSealingWorkerHoursPerBreach`
  - preserved the existing Tier 1 constants and added Tier 2 and Tier 3
    canon values
- `unity/Assets/_Bloodlines/Code/Fortification/BreachSealingSystem.cs`
  - now resolves required stone and required worker-hours from the settlement's
    live fortification tier before reserving stone, checking funding, and
    accumulating progress
  - sealing completion still clears one open breach at a time and leaves the
    downstream destroyed-counter rebuild slice unchanged
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`
  - sealing telemetry now reports the tier-aware required worker-hours and
    required stone instead of the old flat values
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachLegibilityReadoutSmokeValidation.cs`
  - phase 6 now derives expected sealing cost and labor from the fortification
    tier so the existing breach telemetry smoke stays aligned with the runtime
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingTierScalingSmokeValidation.cs`
  - new dedicated validator with four phases:
    Tier 1 baseline, Tier 2 scale-up, Tier 3 scale-up, and mixed-tier
    portfolio
- `scripts/Invoke-BloodlinesUnityBreachSealingTierScalingSmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Design Notes

- The ECS surface uses `FortificationComponent.Tier` as the canonical source of
  truth for the settlement's current fortification tier. The prompt's
  `CurrentTier` wording maps to that live field.
- The tier scaling is intentionally sealing-only. Destroyed-counter recovery
  stays on the existing `90` stone / `14` worker-hour baseline with the keep
  multiplier from sub-slice 9.
- Mixed-tier validation asserts that simultaneous reservations use the sum of
  the tier-specific costs: `60 + 90 + 135 = 285`, leaving `115` stone from a
  `400`-stone stockpile.
- The checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `D:\ProjectsHome\Bloodlines\unity`, so this slice re-used worktree-safe
  temporary copies for those gates while preserving the same execute methods and
  pass markers.
- The worktree did not initially include `Assembly-CSharp*.csproj`. The first
  Unity open generated local gitignored project files and the validator `.meta`
  file before the governed `dotnet build` gates ran.

## Validation

The slice is green on `D:\BLF11\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke via a worktree-safe wrapper copy
4. combat smoke
5. bootstrap scene-shell validation via a worktree-safe wrapper copy
6. gameplay scene-shell validation via a worktree-safe wrapper copy
7. fortification smoke
8. siege smoke
9. `node tests/data-validation.mjs`
10. `node tests/runtime-bridge.mjs`
11. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
12. breach sealing tier scaling smoke

Dedicated tier-scaling smoke phases:

- Phase 1 PASS: Tier 1 sealing still reserves `60` stone, requires `8`
  worker-hours, and reaches `0.5` progress after `4` hours
- Phase 2 PASS: Tier 2 sealing reserves `90` stone, requires `12`
  worker-hours, and reaches `0.5` progress after `6` hours
- Phase 3 PASS: Tier 3 sealing reserves `135` stone, requires `18`
  worker-hours, and reaches `0.5` progress after `9` hours
- Phase 4 PASS: mixed Tier 1, Tier 2, and Tier 3 settlements each reserve the
  correct amount in parallel and leave the shared stockpile at `115`

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-sealing-tier-scaling-smoke.log`

## Current Readiness

This slice is complete and ready for push, merge-temp coordination, and
continuation onto sub-slice 12 worker-locality gating.

## Next Action

1. Push `codex/unity-fortification-sealing-cost-tier-scaling`.
2. Merge it to `master` via the normal merge-temp ceremony.
3. After the merge lands, branch from the refreshed `origin/master` and start
   sub-slice 12 on `codex/unity-fortification-sealing-worker-locality`.
