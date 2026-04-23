# Unity HUD Political State Panels Rerun Master Refresh

- Date: 2026-04-23
- Lane: `player-hud-realm-condition-legibility`
- Merge Result: `db79f248`
- Status: merged forward over current canonical `master` lineage and revalidated

## Goal

Forward-port the already-landed HUD political-state panel rerun over a
canonical `master` line that now already includes the player covenant-test
dispatch landing, without changing HUD behavior or widening ownership into
`unity/Assets/_Bloodlines/Code/AI/**`.

## What Landed On Master

- No new player-facing HUD behavior was introduced in this refresh. Canonical
  `master` still carries:
  - `SuccessionCrisisHUDSystem`
  - `PoliticalEventsTrayHUDSystem`
  - `CovenantTestProgressHUDSystem`
  - `TruebornRiseHUDSystem`
  - `BloodlinesDebugCommandSurface.HUD` political-state snapshot helpers
  - `BloodlinesPoliticalStateHUDSmokeValidation` plus
    `scripts/Invoke-BloodlinesUnityPoliticalStateHUDSmokeValidation.ps1`
- This refresh proves that the same HUD slice still validates on top of the
  newer master ancestry that now also includes:
  - `PlayerCovenantTestDispatchStateComponent`
  - `PlayerCovenantTestDispatchSystem`
  - `BloodlinesDebugCommandSurface.Faith` covenant-dispatch helpers
  - `BloodlinesPlayerCovenantTestDispatchSmokeValidation` plus
    `scripts/Invoke-BloodlinesUnityPlayerCovenantTestDispatchSmokeValidation.ps1`
- The committed `unity/Assembly-CSharp*.csproj` files continue to preserve the
  canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer
  roots after the landing worktree validation pass rewrote them locally.

## Validation Proof On Merge Result

- Runtime build:
  - `Build succeeded.`
- Editor build:
  - `Build succeeded.` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Combat smoke validation passed.`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Siege smoke validation passed.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Dedicated smokes on the refreshed merge result:
  - `Political state HUD smoke validation passed.`
  - `Player covenant-test dispatch smoke validation passed.`
- Final staleness recheck after landing bookkeeping:
  - `STALENESS CHECK PASSED: Contract revision=115, last-updated=2026-04-23 is current.`

## Exact Next Action

1. Start Priority 19 `codex/unity-contested-territory-pressure` from the
   updated canonical `master`.
2. Read the territorial pressure seam in `src/game/core/simulation.js` plus
   the current Unity `WorldPressure/**` surfaces.
3. Port the next additive non-AI slice with its own dedicated smoke validator
   and matching PowerShell wrapper.
