# Unity Player HUD Dynasty Renown Readout

Date: 2026-04-22
Lane: `player-hud-realm-condition-legibility`
Branch: `codex/unity-hud-dynasty-renown-readout`

## Goal

Add the next player-facing HUD consumer on top of the already-landed dynasty
renown/prestige runtime seam: a faction-scoped HUD read-model that surfaces
renown score, rank, ruler identity, legitimacy, and succession status without
widening `AI/**`.

## Browser Reference

- `src/game/core/simulation.js`
  - search `renown`
  - search `prestige`
  - search `getMatchSummary`
  - search `getLeaderboard`
- Browser divergence:
  - the browser currently exposes per-member renown only, primarily around
    `awardRenownToFaction` and the per-member dynasty records; it does not ship
    a dynasty-level HUD or prestige leaderboard surface
- Canon references:
  - `01_CANON/BLOODLINES_DESIGN_BIBLE.md` (`75`, `238`, `277`)
  - `04_SYSTEMS/DYNASTIC_SYSTEM.md`

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/HUD/DynastyRenownHUDComponent.cs` with a
  faction-scoped HUD read-model carrying renown score, peak renown, score-to-peak
  ratio, dynasty rank, leading flag, current ruler identity, legitimacy,
  interregnum status, and a HUD-only renown band label/color.
- Added `unity/Assets/_Bloodlines/Code/HUD/DynastyRenownHUDSystem.cs` as a
  simulation-group `ISystem` that reads the existing
  `DynastyRenownComponent`, `DynastyStateComponent`, and dynasty roster buffer,
  computes cross-faction rank ordering, projects ruler identity, and throttles
  refreshes on a quarter-day cadence.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  with `TryDebugGetDynastyRenownHUDSnapshot(...)`, returning parseable
  `DynastyRenownHUD|FactionId=...|Score=...|PeakRenown=...|Rank=...|...`
  output for smoke and later panel wiring.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyRenownHUDSmokeValidation.cs`
  plus `scripts/Invoke-BloodlinesUnityDynastyRenownHUDSmokeValidation.ps1`.
- Added the shared generated compile registrations for the new HUD runtime and
  editor files in `unity/Assembly-CSharp.csproj` and
  `unity/Assembly-CSharp-Editor.csproj`.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
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
- Contract staleness after contract bump:
  - `STALENESS CHECK PASSED: Contract revision=85, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Dynasty renown HUD smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice projects the already-landed dynasty renown score into HUD state; it
  does not invent a separate browser-style prestige victory condition or dispute
  war flow.
- The renown band labels (`obscure`, `rising`, `ascendant`, `legendary`) are a
  HUD-only readability layer because the browser has no dynasty-level prestige
  bands to clone.
- The read surface is debug/HUD-owned only. No on-screen panel prefab or UGUI
  wiring is added in this slice.

## Exact Next Action

Merge `codex/unity-hud-dynasty-renown-readout`, rerun the governed validation
chain on the merge result, then continue the HUD lane with the next dynasty-facing
consumer such as a leaderboard/panel readout that consolidates renown pressure
across factions.
