# Session Handoff: Sub-slice 36 -- AI Succession Crisis Context Flag Refresher

## Goal

Write the four succession-crisis context flags into `AIStrategyComponent` from live game state each frame. `AIStrategicPressureSystem` reads these flags to apply timer clamps (ai.js lines 1161-1185) but previously had no system setting them from real `SuccessionCrisisComponent` data.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisContextSystem.cs` -- reads `SuccessionCrisisComponent` on all factions, writes `PlayerSuccessionCrisisActive`, `PlayerSuccessionCrisisHigh`, `EnemySuccessionCrisisActive`, `EnemySuccessionCrisisSevere` into `AIStrategyComponent` on each AI faction. Severity >= 3 (Major/Catastrophic) triggers the high/severe flags.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SuccessionCrisisContext.cs` -- debug surface exposing `TryDebugGetSuccessionCrisisContextFlags`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisContextSmokeValidation.cs` -- 3-phase smoke validator (PlayerCrisisHighDetected, EnemyCrisisSevereDetected, NoCrisis). Marker: `BLOODLINES_SUCCESSION_CRISIS_CONTEXT_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnitySuccessionCrisisContextSmokeValidation.ps1` -- PS1 wrapper.

### Modified Files

- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries.
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 141.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 141 confirmed current.

## Current Readiness

Branch `claude/unity-ai-succession-crisis-context` is ready to merge to master.

## Next Action

Sub-slice 37: AI Holy War Context Flag Refresher. Scans active `DynastyOperationKind.HolyWar` operations and writes `AIStrategyComponent.HolyWarActive` for each involved AI faction. Feeds `AIStrategicPressureSystem` (ai.js lines 1129-1132).

## Browser Reference

- `src/game/core/ai.js` updateEnemyAi lines 1161-1185 (player and enemy succession crisis clamp blocks).
