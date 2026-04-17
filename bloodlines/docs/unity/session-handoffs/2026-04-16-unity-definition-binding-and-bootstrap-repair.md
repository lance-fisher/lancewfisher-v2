# 2026-04-16 Unity Definition Binding And Bootstrap Repair

## Scope

Repair the broken Unity definition-asset lane so the canonical `MapDefinition` can resolve correctly in editor automation, then restore the Bootstrap scene's canonical map assignment.

## What Changed

- Confirmed the real blocker behind failed Bootstrap repair: all 119 generated definition assets under `unity/Assets/_Bloodlines/Data/` were serialized with `m_Script: {fileID: 0}`.
- Corrected the Unity definition layout by moving every ScriptableObject type into its own correctly named file under `unity/Assets/_Bloodlines/Code/Definitions/`.
- Kept shared serializable helper data types in `BloodlinesDefinitions.cs`.
- Updated `JsonContentImporter.cs` so broken generated definition assets are:
  - backed up outside `Assets/`
  - recreated in place
  - repopulated from canonical JSON
- Added `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` as the governed batch wrapper for the JSON sync path.
- Preserved the broken pre-repair generated definition assets under `reports/unity-definition-binding-repair/2026-04-16-095158/`.
- Re-ran the JSON sync path and confirmed:
  - 119 definition assets now have valid `m_Script` bindings
  - 0 definition assets remain in the old broken state
- Updated `BloodlinesGameplaySceneBootstrap.cs` so Bootstrap repair assigns the canonical map by persistent asset-path reference instead of relying on the earlier ambiguous object resolution.
- Repaired `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` so it now stores:
  - `map: {fileID: 11400000, guid: d1ad0843071350c45aa1a54a2bad5b84, type: 2}`

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings.
- Governed batch Bootstrap repair now logs:
  - `Bootstrap scene canonical map assignment repaired using asset: Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`

## Immediate Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Enter Play Mode and verify:
   - live entity spawning from the Bootstrap authoring path
   - debug presentation visibility
   - battlefield camera behavior
   - single-select, drag-box select, select-all, clear-selection, and formation-aware move
   - control-point capture and uncontested trickle
3. After that first live shell is confirmed, continue into attack-move or richer command-state UX.
