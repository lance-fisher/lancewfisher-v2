# Unity Player HUD Dynasty Renown Panel

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-hud-dynasty-renown-panel`
- Status: validated on branch

## Goal

Add the next dynasty-facing HUD consumer on top of the already-landed
`DynastyRenownHUDComponent` seam: a singleton leaderboard/panel read-model that
consolidates renown pressure across factions into one parseable ordered surface
for later on-screen UI binding.

## Browser Reference

- `src/game/core/simulation.js`
  - search `getMatchSummary`
  - search `getLeaderboard`
  - search `renown`
  - search `prestige`
- Browser divergence:
  - the browser runtime exposes member-level renown and broader summary surfaces
    but does not ship a dynasty-level prestige panel; this slice is an additive
    Unity HUD consumer of the already-landed renown runtime

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/HUD/DynastyRenownLeaderboardHUDComponent.cs`
  and
  `DynastyRenownLeaderboardHUDSystem.cs`
  now add a singleton ordered dynasty leaderboard under `HUD/**` that consumes
  the per-faction `DynastyRenownHUDComponent` snapshots, sorts factions by
  renown score with peak-renown tie-breaking, carries player/interregnum state,
  and projects ruler identity and renown-band labels into one consolidated panel
  surface.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetDynastyRenownLeaderboard()` with parseable multi-line
  `DynastyRenownLeaderboard|Rank=...|FactionId=...|Score=...|PeakRenown=...|...`
  output.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyRenownLeaderboardHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityDynastyRenownLeaderboardHUDSmokeValidation.ps1`
  now prove row population, human-player/interregnum projection, and
  peak-aware ordering.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  now include the new runtime/editor files, restore this worktree's
  `unity/Library` via a junction to the canonical root, and retarget the stale
  source-generator analyzer paths away from the dead `c946` worktree to
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`.

## Validation Proof

- Dedicated smoke:
  - `Dynasty renown leaderboard HUD smoke validation passed.`
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
  - `STALENESS CHECK PASSED: Contract revision=85, last-updated=2026-04-22 is current.`

## Unity-Side Simplifications Deferred

- This slice stops at the HUD-owned data model and debug seam. It does not add
  an on-screen UGUI/UITK panel yet.
- Sorting prefers `RenownScore`, then `PeakRenown`, then the existing HUD rank
  and player flag. It does not yet fold in separate diplomatic or world-pressure
  weighting.
- The browser has no direct dynasty prestige panel to clone, so this remains a
  Unity-specific read surface built on canonical renown state already landed in
  ECS.

## Exact Next Action

1. Stage the dynasty-renown panel slice files plus contract and continuity
   updates, commit them on `codex/unity-hud-dynasty-renown-panel`, and push to
   `origin`.
2. Merge the branch to canonical `master`, rerun the full governed 10-gate
   chain on the merge result, and then continue the next player-facing consumer
   or on-screen HUD binding that uses the consolidated dynasty leaderboard.
