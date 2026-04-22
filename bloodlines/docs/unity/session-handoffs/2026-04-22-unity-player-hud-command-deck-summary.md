# Unity Player HUD Command Deck Summary

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-command-deck-summary`
- Status: validated on branch

## Goal

Add the next consolidated player-facing HUD consumer on top of the already-landed
match, victory, fortification, and dynasty renown read-models: a command-deck
summary snapshot that gives one faction-scoped view of stage pressure, leading
victory momentum, dynasty prestige, realm condition bands, and the primary
player alert without widening `AI/**`.

## Browser Reference

- `src/game/core/simulation.js`
  - `getMatchProgressionSnapshot` (13650-13658)
  - `getRealmConditionSnapshot` (14291-14764)
- Browser divergence:
  - the browser runtime exposes match progression and realm-condition snapshots
    separately, but it does not ship a single consolidated player command-deck
    HUD surface; this slice is an additive Unity HUD consumer of already-landed
    ECS read-model seams.

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/HUD/PlayerCommandDeckHUDComponent.cs`
  and
  `PlayerCommandDeckHUDSystem.cs`
  now add a faction-scoped command-deck HUD snapshot under `HUD/**` that
  consumes the existing match progression, victory leaderboard, dynasty renown,
  and fortification HUD surfaces, derives realm-condition bands from canonical
  faction state, and resolves a single `PrimaryAlertLabel` with ordered
  precedence across Great Reckoning, fortification threat, loyalty crisis,
  victory pressure, and world pressure.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetPlayerCommandDeckHUDSnapshot()` with parseable
  `PlayerCommandDeckHUD|FactionId=...|StageLabel=...|LeadingVictoryProgressPct=...|PrimaryAlertLabel=...`
  output for later panel binding.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCommandDeckHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCommandDeckHUDSmokeValidation.ps1`
  now prove summary projection, Great Reckoning alert precedence, fortification
  threat alerting, and victory-imminent alerting in a dedicated ECS validation
  world.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  now include the new runtime/editor files so governed builds compile the slice.

## Validation Proof

- Dedicated smoke:
  - `Player command-deck HUD smoke validation passed.`
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
  - `STALENESS CHECK PASSED: Contract revision=87, last-updated=2026-04-22 is current.`

## Unity-Side Simplifications Deferred

- This slice stops at the HUD-owned read-model and debug seam. It does not yet
  add an on-screen UITK/UGUI command-deck widget.
- Realm population, loyalty, and faith remain coarse command-deck bands for
  legibility. The slice does not yet expose every underlying scalar already
  available inside the separate HUD subsystems.
- Alert precedence is intentionally single-channel. It does not yet emit a full
  ordered alert stack or inbox surface.

## Exact Next Action

1. Stage the command-deck slice files plus contract and continuity updates,
   commit them on `codex/unity-player-hud-command-deck-summary`, and push to
   `origin`.
2. Merge the branch to canonical `master`, rerun the full governed 10-gate
   chain on the merge result, and then continue the next actual on-screen HUD
   binding or adjacent consolidated player-facing HUD surface.
