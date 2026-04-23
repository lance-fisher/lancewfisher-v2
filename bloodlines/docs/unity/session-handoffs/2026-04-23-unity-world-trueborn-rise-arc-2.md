# 2026-04-23 Unity Trueborn Rise Arc (Sub-Slice 2)

## Slice Summary

- Lane: `world-trueborn-rise`
- Branch: `codex/unity-world-trueborn-rise-arc-2`
- Browser references / search terms used:
  - `getTruebornRecognitionTerms`
  - `recognizeTruebornClaim`
  - `tickTruebornRiseArc`
  - `TRUEBORN_RISE_STAGE_*`
  - `recognizedPressureMultiplier`

## What Landed

- Added `PlayerTruebornRecognitionRequestComponent`,
  `TruebornRecognitionUtility`, and `TruebornRecognitionResolutionSystem` as
  the shared recognition-dispatch seam for the existing Trueborn rise arc.
  Eligible kingdoms now spend the browser-aligned recognition costs, set their
  stable recognition-slot bit, gain the standing-to-renown bonus, and clear
  the Covenant Test / Divine Right cooldown events when recognition resolves.
- `TruebornRiseArcSystem` now reuses `TruebornRecognitionUtility` for shared
  recognition-slot lookups instead of carrying a second private slot helper,
  keeping the rise-pressure and recognition-resolution slices on one bitmask
  surface.
- `BloodlinesDebugCommandSurface.WorldPressure` now exposes
  `TryDebugRecognizeTrueborn(...)` and
  `TryDebugGetTruebornRecognitionState(...)`; the existing
  `TryDebugGetTruebornRiseArc(...)` and `TryDebugSetTruebornRecognition(...)`
  paths now read/write through the shared recognition utility.
- `BloodlinesTruebornRiseArcSmokeValidation` now proves three phases on the
  current master-compatible runtime: stage progression, recognition-cost
  resolution with renown/cooldown fallout, and recognized-faction pressure
  exemption while duplicate requests remain no-ops.
- `unity/Assembly-CSharp.csproj` now explicitly includes the new Trueborn
  recognition runtime files. `unity/Assembly-CSharp-Editor.csproj` retains the
  dedicated Trueborn smoke validator registration.

## Validation

- Dedicated Trueborn rise smoke: PASS
  - `BLOODLINES_TRUEBORN_RISE_ARC_SMOKE PASS phase1Stage=3,challenge=0,cycles=3,readout=Stage=3|StageStarted=4745|GlobalPressure=1.4|LoyaltyErosion=3.2|Challenge=0|UnchallengedCycles=3|RecognizedCount=0|RecognizedMask=0; phase2Influence=60.0,gold=40.0,legitimacy=75.0,renown=16.0; phase3PlayerLoyalty=90.0,enemyLoyalty=86.4,enemyLegitimacy=78.8`
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
  - `STALENESS CHECK PASSED: Contract revision=107, last-updated=2026-04-23 is current.`

## Notes

- The shared `PlayerTruebornRecognitionRequestComponent` is ready for later AI
  producers, but this slice intentionally did not modify
  `unity/Assets/_Bloodlines/Code/AI/**` because that path remains exclusive to
  the `ai-strategic-layer` lane. The current runtime therefore proves player
  dispatch plus an AI-compatible resolution seam rather than introducing an
  AI-owned request writer here.
- The branch-local worktree still relies on the local `unity/Library` junction
  to the canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so the
  governed `.csproj` build gates can resolve `Library\ScriptAssemblies`.
- The governed Unity wrappers still split between canonical-root and worktree
  project paths; both surfaces remained green for this slice.
- Keep the existing
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  modification unstaged. Unity dirties it during validation and it is
  unrelated to this slice.

## Exact Next Action

1. Open `codex/unity-world-trueborn-rise-arc-3` from updated canonical
   `master`.
2. Implement Priority 16 from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`:
   add the Trueborn ultimatum fields and diplomatic-escalation system for
   stage-4/5 pressure.
3. Reuse `PlayerTruebornRecognitionRequestComponent` and
   `TruebornRecognitionUtility` for any pre-deadline recognition path, and
   keep `unity/Assets/_Bloodlines/Code/AI/**` untouched unless the
   `ai-strategic-layer` lane explicitly picks up AI request emission.
