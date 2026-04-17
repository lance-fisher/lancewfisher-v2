# 2026-04-16 Unity First Construction Slice

## Goal

Land the first governed Unity construction slice so the Bootstrap shell can place a supported building with a controlled worker, progress it through ECS construction state, and prove the result through the governed runtime-smoke lane without regressing the stronger production shell.

## Work Completed

- Added the first construction runtime state in:
  - `unity/Assets/_Bloodlines/Code/Components/ConstructionComponents.cs`
  - `unity/Assets/_Bloodlines/Code/Systems/ConstructionSystem.cs`
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` so it now supports:
  - worker-aware construction panel visibility
  - pending build-placement mode
  - right-click placement for supported buildings
  - obstruction feedback for invalid sites
  - governed debug construction start for batch runtime smoke
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs` so under-construction buildings render distinctly.
- Updated `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs` so under-construction buildings do not act as live production seats.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now proves:
  - the existing two-deep `command_hall -> villager` queue issue, rear-entry cancel-and-refund, and surviving front-entry completion
  - `dwelling` placement near a controlled worker
  - building-count increase from `9` to `10`
  - construction-site completion
  - final population-cap increase from `18` to `24`
- Added the missing Unity metadata files for the new construction runtime files and synchronized the current generated `unity/Assembly-CSharp.csproj` include list so isolated `dotnet build` sees the new construction code until the next Unity project regeneration.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` warnings and `0` errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` errors. Existing editor/importer `CS0649` warnings remain; this session surfaced `110`.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- `artifacts/unity-bootstrap-runtime-smoke.log` now proves:
  - factions `3`
  - buildings `10`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
  - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion
  - `dwelling` construction completion with `constructionSites=0` and `populationCap=24`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

- The Unity first-shell lane now has governed proof for selection, movement, production, queue cancellation, and first construction completion.
- Manual in-editor feel verification is still the next real gate for:
  - worker construction-panel clarity
  - pending-placement readability
  - obstruction feedback feel
  - right-click placement feel
  - build-completion readability and pacing

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
   - queue-row visibility plus rear-entry cancel-button feel
   - worker construction-panel visibility and pending-placement readability
   - `dwelling` obstruction response, right-click placement, completion timing, and final population-cap increase feel
3. After that, continue into broader construction-roster verification, construction progress UI, deeper build-placement UX, broader production-roster verification, or other command-state work. Do not prioritize attack-move until a real combat lane exists.
