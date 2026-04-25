# Session Handoff: Sub-slice 35 -- AI Faith Commitment Auto-Selection

## Goal

Implement automatic covenant faith selection for AI factions that have accrued sufficient exposure but have not yet committed to a faith. When an AI faction has `FaithStateComponent.SelectedFaith == None` and its `FaithExposureElement` buffer contains at least one entry with `Exposure >= 100f` and `Discovered == true`, the system commits to the highest-exposure qualifying faith.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIFaithCommitmentSystem.cs` -- one-shot auto-commit system. Scans AI factions (AIEconomyControllerComponent gate), picks highest-exposure faith at threshold, sets SelectedFaith/DoctrinePath.Light/Intensity=20/Level=1, applies Oathkeeping+2, pushes Info narrative.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.FaithCommitment.cs` -- debug surface exposing `TryDebugGetFaithState`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithCommitmentSmokeValidation.cs` -- 3-phase smoke validator (CommitsWhenAvailable, SkipsWhenExposureLow, SkipsWhenAlreadyCommitted). Marker: `BLOODLINES_FAITH_COMMITMENT_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnityFaithCommitmentSmokeValidation.ps1` -- PS1 wrapper.

### Modified Files

- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries (system + debug surface).
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (smoke validator).
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 140.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 140 confirmed current.

## Current Readiness

Branch `claude/unity-ai-faith-commitment` is ready to merge to master.

## Next Action

Sub-slice 36: AI Succession Crisis Context Flag Refresher. Writes `PlayerSuccessionCrisisActive/High` and `EnemySuccessionCrisisActive/Severe` flags into `AIStrategyComponent` from live `SuccessionCrisisComponent` data. These flags are consumed by `AIStrategicPressureSystem` (ai.js lines 1161-1185) but currently have no system writing them.

## Browser Reference

- `src/game/core/ai.js` updateEnemyAi faith selection block (~1253-1260).
- `src/game/core/simulation.js` `chooseFaithCommitment` (~9694-9724).
