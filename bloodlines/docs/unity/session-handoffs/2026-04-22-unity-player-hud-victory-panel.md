# Unity Player HUD Victory Leaderboard Panel

Date: 2026-04-22
Lane: `player-hud-realm-condition-legibility`
Branch: `codex/unity-hud-victory-panel`

## Goal

Add the next player-facing victory HUD surface on top of the existing per-faction victory-distance readout: a single ordered leaderboard that exposes the top victory path for each faction without widening the retired `Victory/**` lane.

## Browser Reference

- `src/game/core/simulation.js`
  - search `getMatchSummary`
  - search `getLeaderboard`
  - search `TerritorialGovernance`
  - search `DivineRight`
  - search `CommandHallFall`
- `tests/runtime-bridge.mjs`
  - search `victory`

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/HUD/VictoryLeaderboardHUDComponent.cs` with a singleton refresh component plus ordered buffer entries carrying `FactionId`, `LeadingConditionId`, `ProgressPct`, and `IsHumanPlayer`.
- Added `unity/Assets/_Bloodlines/Code/HUD/VictoryLeaderboardHUDSystem.cs` as a presentation-group `ISystem` that reads `VictoryConditionReadoutComponent` buffers, resolves each faction's leading condition, sorts the rows by highest progress, and writes a singleton ordered leaderboard buffer.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs` with `TryDebugGetVictoryLeaderboard()` returning parseable multi-line `VictoryLeaderboard|Rank=...|FactionId=...|LeadingConditionId=...|ProgressPct=...|IsHumanPlayer=...` output.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryLeaderboardHUDSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityVictoryLeaderboardHUDSmokeValidation.ps1`.
- The dedicated smoke now proves:
  - two-faction leaderboard population with the correct leading condition per faction
  - correct human-player flagging for the player row
  - descending sort order by highest victory progress
- Added compile registrations for the new HUD runtime/editor files in the shared generated `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`.

## Validation Proof

- Dedicated smoke:
  - `Victory leaderboard HUD smoke validation passed.`
- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Unity exited with code 0`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Unity exited with code 0`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=84, last-updated=2026-04-22 is current.`

## Unity-Side Simplifications Deferred

- The leaderboard currently consumes the already-landed `VictoryConditionReadoutComponent` buffers rather than reconstructing a separate browser-style full match summary object.
- Tie-breaking is stable by progress first, then human-player preference, then existing faction query order. No additional canon tie-break policy was found in the browser references searched for this slice.

## Exact Next Action

Merge `codex/unity-hud-victory-panel` after the full 10-gate rerun, then either add a HUD consumer for dynasty renown/prestige or resume the next unfinished player-facing directive item.
