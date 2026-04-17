# 2026-04-16 Unity Two-Deep Production Queue Tail-Cancel Validation

## Goal

Strengthen the first governed Unity production slice by proving a deeper queue-depth path: issue two queued villagers, cancel the rear entry safely with refund verification, keep the front entry alive, and confirm its final completion through the governed Bootstrap runtime-smoke lane.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now proves:
  - controlled `command_hall` selection
  - first `villager` queue issuance
  - second `villager` queue issuance
  - rear-entry cancellation at queue index `1`
  - refund verification against the stored queue payload while the front entry remains alive
  - queue-depth reduction without canceling the surviving front entry
  - final surviving front-entry completion and post-production unit-count verification
- Preserved the stronger queue-depth result in:
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `HANDOFF.md`
  - `continuity/PROJECT_STATE.json`
  - `unity/README.md`
  - `unity/Assets/_Bloodlines/Code/README.md`

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` warnings and `0` errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` errors. Existing editor/importer `CS0649` warnings remain unchanged.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- `artifacts/unity-bootstrap-runtime-smoke.log` remains green with:
  - `productionCancelVerified=True`
  - factions `3`
  - buildings `9`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

- The Unity first production shell is now stronger than the earlier single-entry cancel slice because the governed runtime lane proves two-deep queue semantics instead of only one queued cancel path.
- Manual in-editor feel verification is still the next real gate:
  - queue-row clarity
  - rear-entry cancel-button feel
  - refund legibility
  - surviving front-entry completion feel

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - unit select and building select
   - drag-box select
   - `1` select-all
   - `Ctrl+2-5` save
   - `2-5` recall
   - `F` frame selection
   - formation-aware move issuance
   - queue-row visibility and rear-entry cancel-button feel
   - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion feel
3. After that, continue into construction placement, broader production-roster verification, deeper production-queue depth beyond the current two-deep rear-entry tail-cancel proof, or other command-state work. Do not prioritize attack-move until a real combat lane exists.
