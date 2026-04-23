# 2026-04-23 Unity Trueborn Rise Recognized Pressure Follow-Up

## Slice Summary

- Lane: `world-trueborn-rise`
- Branch: `codex/unity-world-trueborn-recognized-pressure`
- Browser references / search terms used:
  - `tickTruebornRiseArc`
  - `recognizedPressureMultiplier`
  - `recognizeTruebornClaim`
  - `getTruebornRecognitionTerms`
- Canon references:
  - `11_MATCHFLOW/MATCH_STRUCTURE.md`
  - `08_MECHANICS/DIPLOMACY_SYSTEM.md`

## What Landed

- `TruebornRiseArcSystem` now matches the browser Session 95 rise-pressure
  nuance: recognized kingdoms no longer receive full exemption from base
  Trueborn rise pressure and instead take the intended `0.25x` loyalty and
  legitimacy pressure multiplier while unrecognized kingdoms still take full
  pressure.
- `BloodlinesTruebornRiseArcSmokeValidation` now proves the reduced-pressure
  behavior directly: the recognized kingdom keeps taking only quarter-strength
  stage-2 pressure, the unrecognized rival still takes full pressure, and
  duplicate recognition requests remain no-ops.

## Validation

- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.` with existing repo-wide warnings only
- Governed full validation chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=110, last-updated=2026-04-23 is current.`
- Dedicated Trueborn rise smoke: PASS
  - `BLOODLINES_TRUEBORN_RISE_ARC_SMOKE PASS`

## Exact Next Action

1. Commit and push `codex/unity-world-trueborn-recognized-pressure`.
2. Merge the branch back onto canonical `master` with `--no-ff`, rerun the
   governed chain on the merged result, and then continue to the next
   remaining non-AI Trueborn follow-up.
