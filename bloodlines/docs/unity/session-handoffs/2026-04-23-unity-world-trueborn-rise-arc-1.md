# 2026-04-23 Unity Trueborn Rise Arc (Sub-Slice 1)

## Slice Summary

- Lane: `world-trueborn-rise`
- Branch: `codex/unity-world-trueborn-rise-arc-1`
- Browser references / search terms used:
  - `tickTruebornRiseArc`
  - `getTruebornChallengeLevel`
  - `TRUEBORN_RISE_UNCHALLENGED_THRESHOLD_DAYS`
  - `TRUEBORN_RISE_STAGE_2_DELAY_DAYS`
  - `TRUEBORN_RISE_STAGE_3_DELAY_DAYS`
  - `recognizedPressureMultiplier`
  - `getTruebornRecognitionTerms`
  - `recognizeTruebornClaim`

## What Landed

- Added `TruebornRiseArcComponent` plus
  `TruebornRiseFactionRecognitionSlotElement` as the singleton ECS surface for
  the browser Trueborn-city rise arc, including the staged pressure profile,
  challenge tracking, unchallenged-cycle counter, and a stable append-only
  kingdom-slot registry for the recognition bitmask.
- Added `TruebornRiseArcSystem` to port the browser late-game arc timing into
  Unity ECS: the Trueborn city now activates after the canonical 8-year dormant
  threshold plus three unchallenged cycles, escalates again after the 2-year
  and 3-year follow-up windows, and applies stage-scaled loyalty erosion plus
  dynasty legitimacy strain to non-recognizing kingdoms on whole-day cadence.
- `BloodlinesDebugCommandSurface.WorldPressure` now exposes
  `TryDebugGetTruebornRiseArc(...)` for live stage / pressure / recognition
  readout and `TryDebugSetTruebornRecognition(...)` so later recognition slices
  and smoke phases can flip kingdoms into the exemption bitmask without
  widening the player-diplomacy lane yet.
- Added `BloodlinesTruebornRiseArcSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityTruebornRiseArcSmokeValidation.ps1` to prove
  canonical stage advancement and the current sub-slice recognition exemption.
  The validator now uses a GameObject-backed debug surface and a separate
  summary artifact path so batchmode log locking no longer throws local noise.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new Trueborn rise runtime and validator files.

## Validation

- Dedicated Trueborn rise smoke: PASS
  - `BLOODLINES_TRUEBORN_RISE_ARC_SMOKE PASS phase1Stage=3,challenge=0,cycles=3,readout=Stage=3|StageStarted=4745|GlobalPressure=1.4|LoyaltyErosion=3.2|Challenge=0|UnchallengedCycles=3|RecognizedCount=0|RecognizedMask=0; phase2PlayerLoyalty=90.0,enemyLoyalty=88.2,enemyLegitimacy=79.4`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed full validation chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=106, last-updated=2026-04-23 is current.`

## Notes

- Unity-side simplifications intentionally kept in sub-slice 1:
  - browser challenge level also counts Trueborn trade-relationship standing;
    Unity does not yet have an equivalent standing surface on the current
    master line, so the challenge score currently uses kingdom territory count
    (`>= 3`) plus hostility toward `trueborn_city`
  - the current directive explicitly says recognized kingdoms should be
    skipped; Unity therefore grants full pressure exemption here rather than
    mirroring the browser's reduced `0.25x` recognized-pressure multiplier
- The dedicated smoke still emits inherited ordering warnings inside its
  minimal validation world because `GovernanceCoalitionPressureSystem` and
  `WorldPressureEscalationSystem` retain their own upstream ordering targets
  (`GovernorSpecializationSystem`, `VictoryConditionEvaluationSystem`,
  `MatchProgressionEvaluationSystem`) that are not added to the test-only
  world. Live runtime ordering is unchanged; the warnings are validator-local.
- The branch-local worktree still relies on the local `unity/Library` junction
  to the canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so the
  governed `.csproj` builds can resolve `Library\ScriptAssemblies`.
- Keep the existing
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  modification unstaged. Unity dirties it during validation and it is
  unrelated to this slice.

## Exact Next Action

1. Open a fresh follow-up branch for the Trueborn recognition / diplomatic
   exemption action once the next directive window authorizes sub-slice 2.
2. Reuse `TruebornRiseFactionRecognitionSlotElement` and
   `TryDebugSetTruebornRecognition(...)` instead of inventing a second
   recognition-index surface.
3. Decide in that follow-up whether Unity should keep the current full-exemption
   rule or widen toward the browser's `0.25x` recognized-pressure multiplier.
