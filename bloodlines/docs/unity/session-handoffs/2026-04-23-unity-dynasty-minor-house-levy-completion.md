# 2026-04-23 Unity Dynasty Minor-House Levy Completion

## Scope

Follow-up completion pass on `codex/unity-dynasty-minor-house-levy-complete`.

This slice revalidated the already-landed minor-house levy runtime against the
browser decay, claim-gate, and loyalty-tier rules, then closed the remaining
observability and proof gaps without reopening `AI/**`.

## Browser References

- `src/game/core/simulation.js`
  - `ensureMinorHouseLevyState`
  - `pickMinorHouseLevyProfile`
  - `tickMinorHouseTerritorialLevies`
  - `MINOR_HOUSE_LEVY_PROFILES`
  - `MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND`
- `src/game/core/ai.js`
  - read-only confirmation that no AI-owned execution surface needed changes

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs`
  - adds `TryDebugGetMinorHouseLevyState(factionId)` so a live minor-house levy
    readout can be inspected without relying on dynasty-core state
  - the summary reports the current claim id, levy status, accumulator,
    seconds-required, selected unit id, retinue count/cap, levies issued, last
    levy unit, and parent pressure context
- `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - minor-house breakaway spawning now explicitly initializes
    `LastLevyUnitId = default` so levy state is fully seeded on the first frame
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMinorHouseLevyParitySmokeValidation.cs`
  - expands the dedicated validator from 4 phases to 5 phases
  - adds an explicit unsettled claim-gate proof where a stabilized claim at
    loyalty `47` stays `Unsettled`, decays levy progress, emits no levy, and
    reports the same blocked state through the new debug readout
- `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1`
  - now waits for explicit PASS/FAIL markers, treats `errored` / `timed out`
    markers as failures, reruns once if the first Unity batch exits before the
    log flushes, and returns success when the PASS marker is present even if
    Unity exits noisily

## Design Notes

- The browser-shaped levy state machine itself was already effectively aligned
  on this master line, so this pass deliberately avoided unnecessary runtime
  rewrites.
- The main gaps were proof completion and debug visibility: a low-loyalty claim
  needed an explicit smoke phase, and the validator needed a lightweight way to
  inspect the live levy state summary.
- No new `.cs` files were introduced, so no `Assembly-CSharp*.csproj` updates
  were required in this pass.

## Validation

The slice is green on
`C:\Users\lance\.codex\worktrees\6c7f\lancewfisher-v2\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
4. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
5. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
6. `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
7. `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1`

Minor-house levy smoke phases:

- Phase 1 PASS: breakaway spawning still seeds a live minor faction claim with
  levy provenance
- Phase 2 PASS: landless minor houses decay levy progress instead of spawning
  phantom troops
- Phase 3 PASS: low-loyalty stabilized claims stay `Unsettled`, decay progress,
  and surface the blocked state through the debug readout
- Phase 4 PASS: pressured high-loyalty claims raise the correct levy profile and
  spend food / influence / loyalty correctly
- Phase 5 PASS: retinue cap blocks over-mustering and clears residual progress

## Current Readiness

This follow-up is complete and master-compatible.

## Next Action

1. Claim Priority 14 `world-trueborn-rise`.
2. Open `codex/unity-world-trueborn-rise-arc-1` from updated `master`.
3. Implement `TruebornRiseArcComponent`, `TruebornRiseArcSystem`, and the
   dedicated smoke validator before rerunning the governed 10-gate chain.
