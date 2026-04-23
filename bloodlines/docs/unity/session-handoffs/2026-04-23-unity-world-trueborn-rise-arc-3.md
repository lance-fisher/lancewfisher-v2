# 2026-04-23 Unity Trueborn Rise Arc (Sub-Slice 3)

## Slice Summary

- Lane: `world-trueborn-rise`
- Branch: `codex/unity-world-trueborn-rise-arc-3`
- Browser references / search terms used:
  - `tickTruebornRiseArc`
  - `getTruebornRecognitionTerms`
  - `recognizedPressureMultiplier`
  - `TRUEBORN_RISE_STAGE_*`
- Canon references:
  - `11_MATCHFLOW/MATCH_STRUCTURE.md`
  - `08_MECHANICS/DIPLOMACY_SYSTEM.md`

## What Landed

- `TruebornRiseArcComponent` now carries the singleton ultimatum state for the
  stage-4/5 diplomatic escalation seam: target faction id, issued/deadline
  timestamps, per-day loyalty and legitimacy pressure, and the stage number
  that issued the ultimatum.
- `TruebornRecognitionUtility` now exposes shared helpers for active ultimatum
  detection, ultimatum clearing, and `FactionId` to entity lookup so the
  escalation system and existing recognition-resolution slice share one
  authority surface.
- `TruebornDiplomaticEscalationSystem` now ports the late-stage Trueborn
  ultimatum loop into `WorldPressure/**` without touching `AI/**`: once the
  rise arc reaches stage 4 or 5, the dominant kingdom receives a timed
  recognition ultimatum; pre-deadline recognition clears it, while an expired
  ultimatum applies extra loyalty pressure to the target's weakest march plus
  dynasty legitimacy strain on whole in-world days.
- `BloodlinesDebugCommandSurface.WorldPressure` now extends the existing
  rise-arc readout with ultimatum target/stage/deadline metadata and adds
  `TryDebugGetTruebornUltimatumState(...)` for focused live inspection.
- `BloodlinesTruebornDiplomaticEscalationSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityTruebornDiplomaticEscalationSmokeValidation.ps1`
  now prove stage-4 ultimatum issuance, pre-deadline recognition clearance,
  and stage-5 expiry fallout on the current master-compatible runtime.
- The worktree-local `unity/Library` surface was restored as a junction to
  `D:\ProjectsHome\Bloodlines\unity\Library`, and stale
  `Assembly-CSharp*.csproj` analyzer roots were canonicalized back to
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` so the governed
  `dotnet build` gates resolve Unity assemblies again from this checkout.

## Validation

- Dedicated Trueborn diplomatic escalation smoke: PASS
  - `BLOODLINES_TRUEBORN_DIPLOMATIC_ESCALATION_SMOKE PASS`
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
  - `STALENESS CHECK PASSED: Contract revision=107, last-updated=2026-04-23 is current.`
- Post-append contract staleness recheck: PASS
  - `STALENESS CHECK PASSED: Contract revision=108, last-updated=2026-04-23 is current.`

## Notes

- This slice is a canon lift for the stage-4/5 Trueborn diplomatic escalation;
  it uses the existing rise-arc and recognition surfaces but does not reopen
  `unity/Assets/_Bloodlines/Code/AI/**`.
- Keep
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  unstaged. Unity dirties it during validation and it is unrelated to this
  slice.

## Exact Next Action

1. Commit and push `codex/unity-world-trueborn-rise-arc-3`, then merge it to
   canonical `master` with `--no-ff`.
2. On merged `master`, rerun the governed validation chain plus the dedicated
   Trueborn diplomatic escalation smoke, then start the next non-AI Trueborn
   follow-up from a fresh branch if session capacity remains.
