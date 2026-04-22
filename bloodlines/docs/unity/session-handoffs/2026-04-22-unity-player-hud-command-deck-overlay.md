# Unity Player Command-Deck Overlay

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-command-deck-overlay-followup`
- Status: validated on branch

## Goal

Bind the already-landed `PlayerCommandDeckHUDComponent` into the existing
on-screen Unity IMGUI shell so the command-deck summary is no longer only a
debug-string seam and becomes a live battlefield overlay.

## Browser Reference

- No exact browser-side overlay equivalent exists. This slice binds the
  already-landed HUD read models into the current Unity shell.
- Read-only source references used to keep the binding aligned with existing
  player-facing summary exports:
  - `src/game/core/simulation.js` `getFactionSnapshot` (9471)
  - `src/game/core/simulation.js` `getMatchProgressionSnapshot` (13650)
  - `src/game/core/simulation.js` `getRealmConditionSnapshot` (14291)

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/HUD/PlayerCommandDeckOverlayPresenter.cs`
  as the pure formatter for the command-deck panel title/body and alert key.
- Added
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CommandDeckOverlay.cs`
  so the existing IMGUI debug shell now renders a top-right command-deck panel
  with alert-color accenting from the live `PlayerCommandDeckHUDComponent`.
- Narrowly updated
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  to invoke the new overlay draw path without changing the existing panel
  order or command behaviors.
- Added
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCommandDeckOverlaySmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCommandDeckOverlaySmokeValidation.ps1`
  to prove stable summary rendering, victory ETA rendering, and Great
  Reckoning alert preservation.
- Repaired this worktree's generated `Assembly-CSharp*.csproj` analyzer paths
  back to `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` and restored
  a local `unity/Library` junction so the governed .NET build gates could run.

## Validation Proof

- Dedicated overlay smoke:
  - `Player command-deck overlay smoke validation passed.`
  - `BLOODLINES_PLAYER_COMMAND_DECK_OVERLAY_SMOKE PASS`
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
- Contract staleness before contract bump:
  - `STALENESS CHECK PASSED: Contract revision=89, last-updated=2026-04-22 is current.`

## Unity-Side Simplifications Deferred

- The overlay is still IMGUI-based and debug-shell-hosted; no scene/UI Toolkit
  panel asset was introduced.
- The overlay currently consumes the player command-deck summary only; it does
  not yet expose deeper faction-by-faction drill-down rows.
- Alert coloring is intentionally coarse (`stable`, warning, critical) and does
  not yet carry faction-color or covenant-specific styling.

## Exact Next Action

1. Stage the command-deck overlay slice files plus continuity/contract updates,
   commit them on `codex/unity-player-hud-command-deck-overlay-followup`, and
   push to `origin`.
2. Continue the HUD lane with the next actual player-facing follow-up, with the
   cleanest pickup being either a narrative/inbox overlay consumer or a richer
   on-screen faction leaderboard that reuses the same read-model seams.
