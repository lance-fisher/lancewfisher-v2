# 2026-04-16 Unity Constructed Barracks Production Continuity

## Goal

Extend the governed Bootstrap runtime-smoke lane so the Unity first shell proves not just construction completion, but production continuity from a newly completed worker-built production structure.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` beyond the Session 103 `dwelling` proof.
- The governed runtime-smoke lane now proves:
  - worker-led `barracks` placement after the existing `dwelling` slice
  - barracks construction completion
  - controlled selection of the completed barracks
  - `militia` queue issuance from that newly completed building
  - queue drain and final spawned-unit growth from the constructed production seat
- Raised the runtime-smoke timeout from `45s` to `75s` so the longer governed sequence can finish in batch mode.
- Fixed the phase boundary inside the validator so the older `command_hall -> villager` exact-count checkpoint no longer trips once the later `barracks -> militia` phase legitimately raises the live unit count.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors. Existing editor/importer `CS0649` debt remains at 110 warnings.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- The latest `artifacts/unity-bootstrap-runtime-smoke.log` success line now ends with:
  - `factions=3`
  - `buildings=11`
  - `units=18`
  - `resourceNodes=13`
  - `controlPoints=4`
  - `settlements=2`
  - `controlledUnits=8`
  - `populationCap=24`
  - `constructedProductionBuildingType='barracks'`
  - `constructedProductionUnitType='militia'`

## Current Readiness

- The governed Unity first shell now proves:
  - command-shell selection and control-group basics
  - two-deep `command_hall -> villager` queue issue, rear-entry cancel, refund, and surviving front-entry completion
  - worker-led `dwelling` construction completion with final population-cap gain
  - worker-led `barracks` construction completion with immediate post-completion `militia` production continuity
- The remaining real gate is still manual in-editor feel verification in `Bootstrap.unity`.

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - `dwelling` placement feel, obstruction response, completion timing, and final cap increase
   - `barracks` completion, selection, `militia` queue visibility, training completion, and controlled-unit growth feel
   - existing command-shell behaviors: selection, drag-box, control groups, framing, move feel, production cancel feel, camera feel, and control-point capture behavior
3. Continue into broader construction-roster verification, construction progress UI, deeper build-placement UX, production from additional newly completed buildings, or broader production-roster work after the in-editor shell is confirmed.
