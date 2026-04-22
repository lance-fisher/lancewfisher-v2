# Unity Player HUD Command-Deck Summary Landing

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Merge Result: `b8fc7589`
- Status: merged to canonical `master` and revalidated

## Goal

Land the validated player command-deck HUD summary slice onto canonical
`master`, rerun the governed validation chain on the merge result, and leave
the HUD lane ready for the next actual on-screen binding or adjacent
player-facing consolidated read surface.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/HUD/PlayerCommandDeckHUDComponent.cs`
  and
  `PlayerCommandDeckHUDSystem.cs`
  now live on canonical `master`, providing a faction-scoped command-deck
  summary that consumes the already-landed match progression, victory
  leaderboard, dynasty renown, and fortification HUD seams.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetPlayerCommandDeckHUDSnapshot()` on canonical
  `master`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCommandDeckHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCommandDeckHUDSmokeValidation.ps1`
  now live on canonical `master` as the dedicated validator/wrapper pair.

## Validation Proof On Merge Result

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
- Contract staleness before landing contract bump:
  - `STALENESS CHECK PASSED: Contract revision=88, last-updated=2026-04-22 is current.`
- Dedicated smoke on merge result:
  - `Player command-deck HUD smoke validation passed.`

## Exact Next Action

1. Claim the next fresh HUD/player-facing follow-up from canonical `master`.
2. The cleanest pickup is an actual on-screen HUD binding that consumes the
   new command-deck snapshot, or another consolidated player command surface
   that reuses the same read-model seams.
