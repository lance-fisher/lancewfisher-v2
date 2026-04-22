# Unity Player HUD Slice: Victory Distance Readout

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-victory-readout`
- Status: validated on branch

## Goal

Extend the player HUD lane with a narrow victory-distance readout that mirrors
the browser's command-hall, territorial-governance, and divine-right progress
signals without widening the retired `victory-conditions` lane or mutating the
source victory systems.

## Browser Reference

- `src/game/core/simulation.js`
  - territorial-governance profile helpers (1207-1416)
  - realm snapshot victory / faith legibility block (14658-14764)
- Supporting parity checks:
  - `tests/runtime-bridge.mjs` victory / legibility assertions already exercised
    through the canonical bridge suite

## What Landed On This Branch

- `unity/Assets/_Bloodlines/Code/HUD/VictoryReadoutComponent.cs`
  defines the faction-scoped HUD summary plus per-condition dynamic buffer used
  for command-surface inspection and future presentation binding.
- `unity/Assets/_Bloodlines/Code/HUD/VictoryReadoutSystem.cs`
  now projects three victory conditions into the HUD-owned read-model:
  command-hall fall distance, territorial-governance integration plus
  hold-countdown state, and divine-right readiness based on faith level and
  intensity thresholds.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes a parseable `VictoryReadout|FactionId=...|ConditionId=...|...`
  seam for automation and local inspection.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryReadoutHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityVictoryReadoutHUDSmokeValidation.ps1`
  prove four deterministic ECS phases: command-hall distance while an enemy hall
  remains, territorial-governance readiness with a hold countdown converted to
  in-world days, partial divine-right readiness, and a completed
  territorial-governance win state.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were updated only to include the new runtime and editor validation files in
  this worktree's generated compile surfaces.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)` with existing repo-wide warnings only
- Bootstrap runtime smoke:
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
  - `STALENESS CHECK PASSED: Contract revision=79, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Victory readout HUD smoke validation passed.`

## Implementation Notes

- Territorial-governance progress is intentionally split into two halves:
  integration coverage first, then sovereignty-hold progress once every held
  march meets the loyalty threshold.
- Remaining territorial-governance hold time is converted from seconds to
  in-world days by reading `DualClockComponent.DaysPerRealSecond`, preserving
  the player-facing time scale already established in the match-progression HUD.
- Divine-right readiness remains a narrow threshold readout here; declaration
  issuance and outcome logic stay inside the already-landed player-faith and
  victory systems.

## Exact Next Action

1. Stage the victory readout HUD slice files plus continuity and contract
   updates, commit them on `codex/unity-player-hud-victory-readout`, and push to
   `origin`.
2. Continue the HUD lane with the next unread victory / realm-legibility follow
   up only after this branch is preserved.
