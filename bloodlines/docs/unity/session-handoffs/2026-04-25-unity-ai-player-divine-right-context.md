# Session Handoff: Sub-slice 38 -- AI Player Divine Right Context Flag Refresher

## Goal

Write `PlayerDivineRightActive` into both `AIStrategyComponent` and `AICovertOpsComponent` on each AI faction when the player has an active `DynastyOperationKind.DivineRight` operation. `AIStrategicPressureSystem` uses the strategy flag for timer clamps (ai.js lines 1156-1160); `AICovertOpsComponent` uses it to block AI re-declaration while the player has one in flight.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIPlayerDivineRightContextSystem.cs` -- scans all `DynastyOperationComponent` entities for `Kind == DivineRight && SourceFactionId == "player" && Active`. Writes the boolean into both component types on all AI factions.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDivineRightContext.cs` -- debug surface exposing `TryDebugGetPlayerDivineRightFlags`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerDivineRightContextSmokeValidation.cs` -- 3-phase smoke validator (Detected, NoDivineRight, InactiveOpIgnored). Marker: `BLOODLINES_PLAYER_DIVINE_RIGHT_CONTEXT_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnityPlayerDivineRightContextSmokeValidation.ps1` -- PS1 wrapper.

### Modified Files

- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries.
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 143.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 143 confirmed current.

## Current Readiness

Branch `claude/unity-ai-player-divine-right-context` is ready to merge to master.

## Next Action

The four sub-slices of the local model work queue (35-38) are now complete. Next candidates from the AI strategic layer are deferred (Covenant Test Action Dispatch requires CovenantTestComponent which does not yet exist; Scout Raid Dispatch requires unit-command integration; Governance Recognition Context requires TerritorialGovernanceComponent not yet defined). The next productive lane is likely: AI CovertOps context flag updater for the remaining AIStrategyComponent fields (EnemyCovenantActive, EnemyGovernanceActive), which requires defining those supporting components first.

## Browser Reference

- `src/game/core/ai.js` updateEnemyAi lines 1156-1160 (player divine right clamp block).
