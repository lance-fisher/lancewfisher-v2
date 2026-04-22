# Codex Unity Dynasty Renown Prestige Slice

## Date

- 2026-04-22

## Branch

- `codex/unity-dynasty-renown-prestige`

## Scope

- Port the next dynasty-facing prestige surface from the active Codex directive
  as a Unity ECS read-model/runtime seam without widening `AI/**`.

## What Landed

- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyRenownComponent.cs`
  introduces a faction-level renown/prestige state carrier for dynasty-facing
  score, peak score, decay rate, last ruling-member tracking, and day-stamped
  updates.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyRenownAccumulationSystem.cs`
  adds a pure ECS `ISystem` that projects dynasty renown from live strategic
  state already present in Unity: territorial advantage above map average,
  committed-faith intensity, victory momentum, and legitimate succession, then
  applies configurable daily decay and peak tracking.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyRenown.cs`
  adds a parseable dynasty readout seam:
  `DynastyRenown|FactionId=...|Score=...|PeakRenown=...|DecayRate=...|LastUpdatedInWorldDays=...`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyRenownSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityDynastyRenownSmokeValidation.ps1`
  provide a dedicated four-phase validator for accumulation, decay, territory
  scaling, and peak tracking.

## Browser Reference

- Read-only parity source inspected:
  - `src/game/core/simulation.js`
  - The browser currently tracks per-member `Renown`; it does not expose a
    dynasty-level prestige ledger, so this slice lifts the design-bible dynasty
    prestige surface onto existing Unity ECS state rather than cloning a
    missing browser subsystem 1:1.

## Validation Proof

- Dedicated smoke:
  - `Dynasty renown smoke validation passed.`
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
  - `STALENESS CHECK PASSED`

## Notes

- The validator now primes the world at day 0 before advancing elapsed time so
  the system's lazy component attach does not mask day-based accumulation in the
  first update.
- Invalid cross-group `UpdateAfter(...)` attributes were removed from the
  runtime system because the referenced systems do not live in the same update
  group instance in the current Unity world graph.

## Immediate Next Action

1. Stage the dynasty renown slice files plus contract and continuity updates,
   commit them on `codex/unity-dynasty-renown-prestige`, and push to `origin`.
2. Continue the next directive item after renown/prestige: likely a downstream
   dynasty-facing read surface or the next HUD/player seam that consumes this
   readout.
