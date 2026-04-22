# Unity Player HUD Command Deck Summary

Date: 2026-04-22
Lane: `player-hud-realm-condition-legibility`
Branch: `codex/unity-player-hud-command-deck-summary-followup`

## Goal

Extend the existing Unity battlefield shell so the player-facing command deck consumes the already-landed match, realm-condition, faith/conviction, and victory HUD read-models instead of leaving them stranded as debug-only ECS readouts.

## Browser Reference

- `src/game/main.js`
  - search `matchDebugLine`
  - search `realmSnapshot`
  - search `Great Reckoning`
- `src/game/core/ai.js`
  - search `getRealmConditionSnapshot`

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` so the battlefield shell now renders additional player-facing command-deck lines for:
  - live match stage / phase / year / declarations / world pressure / Great Reckoning status
  - realm-condition band legibility across population, food, water, and loyalty
  - conviction plus faith commitment/tier summary
  - player victory rank versus the current overall leader
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs` with `TryDebugGetBattlefieldCommandDeckSummary(...)`, a parseable `CommandDeckSummary|...` seam for editor smoke validation and future UI consumers.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBattlefieldCommandDeckSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityBattlefieldCommandDeckSmokeValidation.ps1`.
- The dedicated smoke proves:
  - a baseline command deck summary with rising pressure, stable realm bands, Apex Moral conviction, Old Light Fervent faith, and player victory rank 1
  - a Great Reckoning follow-up where convergence pressure targets the leader, the player realm shows yellow/red strain, and the player surfaces as victory rank 2 behind the enemy

## Validation Proof

- Dedicated smoke:
  - `Battlefield command deck smoke validation passed.`
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
  - `STALENESS CHECK PASSED: Contract revision=85, last-updated=2026-04-22 is current.`

## Exact Next Action

Keep the same player-HUD lane and continue the next concrete follow-up on top of this unified command deck, with the cleanest additive pickup being an on-screen dynasty-renown/prestige consumer or another command-surface summary block that reuses existing ECS HUD read-models.
