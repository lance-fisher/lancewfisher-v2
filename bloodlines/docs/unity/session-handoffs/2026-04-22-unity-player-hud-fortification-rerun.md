# Unity Player HUD Slice: Fortification Legibility Readout Rerun

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-fortification-legibility-rerun`
- Status: validated on rerun branch

## Goal

Carry the preserved fortification legibility slice forward onto the current
canonical `origin/master`, revalidate it in this worktree, and keep the HUD lane
continuity append-only while preserving the original branch handoff.

## Browser Reference

- `src/game/core/simulation.js`
  - `getRealmConditionSnapshot` (14291-14764)
  - fortification block (14568-14629)
- Supporting parity checks:
  - `tests/runtime-bridge.mjs` fortification/readout assertions (1438-1444)

## What Reran On This Branch

- `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDComponent.cs`
  and
  `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDSystem.cs`
  were replayed unchanged from the preserved fortification HUD slice onto the
  current master baseline.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  still exposes the parseable
  `FortificationHUD|SettlementId=...|...` readout for HUD smoke and command
  surface inspection.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`
  still prove intact baseline, threat-active reserve mustering, tier-scaled
  sealing progress, and destroyed-counter recovery progress.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were regenerated locally by a Unity compile pass because this worktree still
  carried stale analyzer/package-cache references to another checkout before the
  rerun.

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
  - `STALENESS CHECK PASSED: Contract revision=78, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Fortification HUD smoke validation passed.`

## Rerun Notes

- The only slice-specific repair during rerun was the local Unity metadata
  refresh needed to repoint `Assembly-CSharp*.csproj` back to this worktree's
  `unity/Library/PackageCache`.
- Local wrapper copies under `artifacts/local-wrappers/` were used only for the
  still-root-pinned bootstrap-runtime and canonical scene-shell validators; the
  checked-in wrapper behavior was otherwise preserved.

## Unity-Side Simplifications Deferred

- This slice still ports only the fortification read-model and debug/smoke seam.
  It does not render an on-screen fortification panel yet.
- Sortie timers, commander/governor labels, ward labels, and imminent-engagement
  detail remain outside this narrow HUD slice.
- Victory-distance readout remains the next HUD follow-up.

## Exact Next Action

1. Stage the fortification HUD rerun slice files plus continuity and contract
   updates, commit them on
   `codex/unity-player-hud-fortification-legibility-rerun`, and push to
   `origin`.
2. Continue the HUD lane with the victory-distance readout slice on a fresh
   Codex branch once this rerun branch is preserved.
