# Unity Player Command-Deck Overlay

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-command-deck-overlay`
- Status: validated on branch and ready to stage/commit/push

## Goal

Bind the already-landed `PlayerCommandDeckHUDComponent` into the live
`BloodlinesDebugCommandSurface` battlefield shell so the player sees a real
on-screen command-deck overlay instead of only parseable debug readouts.

## Browser And Canon Reference

- Browser reference:
  - `src/game/core/simulation.js` `getRealmConditionSnapshot` (14291-14764)
  - `src/game/core/simulation.js` `getMatchProgressionSnapshot` (13650-13658)
  - `tests/runtime-bridge.mjs` realm-condition + match-progression assertions
    (1344-1364, 7521, 7773-7871, 7923-7975, 8133, 8185)
- Unity seam consumed:
  - `unity/Assets/_Bloodlines/Code/HUD/PlayerCommandDeckHUDComponent.cs`
  - `unity/Assets/_Bloodlines/Code/HUD/PlayerCommandDeckHUDSystem.cs`
- Divergence:
  - the browser spec exposes the underlying realm and match snapshots but does
    not ship a single on-screen command-deck overlay; this slice is a Unity
    player-legibility binding that consumes already-landed HUD read-models.

## What Changed

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  now grows the battlefield shell height when a command-deck snapshot is
  present and appends a dedicated command-deck block under the existing
  faction/resources/selection/control-point summary without disturbing the
  construction or production panels.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now builds a shared battlefield overlay snapshot from
  `PlayerCommandDeckHUDComponent`, exposes
  `TryDebugGetBattlefieldCommandDeckOverlay()` for parseable validation, and
  formats visible stage, alert, victory, dynasty, and pressure lines for the
  live IMGUI shell.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCommandDeckOverlaySmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCommandDeckOverlaySmokeValidation.ps1`
  now prove overlay summary rendering, Great Reckoning alert rendering,
  fortification-threat alert rendering, and pressure-band rendering from the
  already-settled command-deck snapshot.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`
  had stale analyzer/source-generator paths repaired from the dead `ba3c`
  worktree back to this live `c7fc` worktree's `unity/Library/PackageCache`,
  and this worktree's `unity/Library` junction was restored to
  `D:\ProjectsHome\Bloodlines\unity\Library` so governed .NET validation works
  again here.

## Validation Proof

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
- Dedicated smoke:
  - `Player command-deck overlay smoke validation passed.`

## Exact Next Action

1. Stage the overlay slice files plus contract and continuity updates, commit
   them on `codex/unity-player-hud-command-deck-overlay`, and push to `origin`.
2. Merge the branch to `master`, rerun the full governed 10-gate chain on the
   merge result, and then continue the next player-facing follow-up from the
   now-live command-deck overlay.

## Context Notes

- `unity/Assets/_Bloodlines/Code/AI/**` remained untouched.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
