# Session Handoff: Sub-slice 37 -- AI Holy War Context Flag Refresher

## Goal

Write `AIStrategyComponent.HolyWarActive` for each AI faction based on whether any active `DynastyOperationKind.HolyWar` operation involves that faction. `AIStrategicPressureSystem` uses this flag to cap `attackTimer <= 10` and `territoryTimer <= 8` (ai.js lines 1129-1132).

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarContextSystem.cs` -- scans all active `DynastyOperationComponent` entities for `Kind == HolyWar`, collects all involved faction IDs into a `NativeHashSet`, then writes `HolyWarActive` into each AI faction's `AIStrategyComponent`.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HolyWarContext.cs` -- debug surface exposing `TryDebugGetHolyWarContextFlag`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarContextSmokeValidation.cs` -- 3-phase smoke validator (HolyWarDetectedAsSource, HolyWarDetectedAsTarget, NoHolyWar). Marker: `BLOODLINES_HOLY_WAR_CONTEXT_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnityHolyWarContextSmokeValidation.ps1` -- PS1 wrapper.

### Modified Files

- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries.
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 142.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 142 confirmed current.

## Current Readiness

Branch `claude/unity-ai-holy-war-context` is ready to merge to master.

## Next Action

Sub-slice 38: AI Player Divine Right Context Flag Refresher. Detects an active `DynastyOperationKind.DivineRight` on the player faction and writes `PlayerDivineRightActive` into both `AIStrategyComponent` and `AICovertOpsComponent`. Feeds `AIStrategicPressureSystem` clamps (ai.js lines 1156-1160) and blocks AI re-declaration while player has one in flight.

## Browser Reference

- `src/game/core/ai.js` updateEnemyAi lines 1129-1132 (holy war timer clamp block).
