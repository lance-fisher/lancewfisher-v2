# Unity Trueborn Rise Recognized Pressure Landing

- Date: 2026-04-23
- Lane: `world-trueborn-rise`
- Merge Result: `71c19cde`
- Status: merged to canonical `master` and revalidated

## Goal

Land the recognized-pressure parity follow-up onto canonical `master`, rerun
the governed validation chain on the merge result, and leave the Trueborn lane
ready for the next remaining non-AI follow-up.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRiseArcSystem.cs` now
  matches the browser Session 95 `recognizedPressureMultiplier` behavior:
  recognized kingdoms take only `0.25x` of the base Trueborn rise loyalty and
  legitimacy pressure instead of full exemption, while unrecognized kingdoms
  still take full pressure.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTruebornRiseArcSmokeValidation.cs`
  now proves that quarter-strength recognized pressure directly while
  preserving the duplicate-recognition no-op assertion.

## Validation Proof On Merge Result

- Runtime build:
  - `Build succeeded.`
- Editor build:
  - `Build succeeded.` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Combat smoke validation passed.`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Siege smoke validation passed.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness on landing state:
  - `STALENESS CHECK PASSED: Contract revision=111, last-updated=2026-04-23 is current.`
- Dedicated smoke on merge result:
  - `BLOODLINES_TRUEBORN_RISE_ARC_SMOKE PASS`

## Exact Next Action

1. Start the next fresh `codex/unity-world-trueborn-rise-*` branch from the
   updated canonical `master`.
2. The next clean follow-up is the remaining non-AI Trueborn trust /
   contribution-history or coalition-response seam described in
   `11_MATCHFLOW/MATCH_STRUCTURE.md`, unless a newer directive supersedes it.
