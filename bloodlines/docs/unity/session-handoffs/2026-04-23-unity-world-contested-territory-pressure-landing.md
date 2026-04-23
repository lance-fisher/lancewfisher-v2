# Unity World Contested Territory Pressure Landing

- Date: 2026-04-23
- Lane: `world-contested-territory-pressure`
- Merge Result: `ea359daf`
- Status: merged onto canonical `master` and revalidated

## Goal

Land the contested-territory pressure rerun on canonical `master` so the
browser-faithful contested-territory read-model, debug readout, and dedicated
smoke proof all live on the canonical Unity line without reviving the stale
older branch's loyalty-drain behavior.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureComponent.cs`
  now gives kingdom faction roots a dedicated contested-territory pressure
  surface for:
  - external contested territories
  - owned contested marches
  - weakest owned contested loyalty
  - governance-contest blocking
- `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureEvaluationSystem.cs`
  now projects the browser's contested-territory seams every frame without
  creating a new world-pressure score source or a new direct loyalty-drain
  mechanic.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs`
  now exposes `TryDebugGetTerritorialPressureState(...)` and additive
  contested / hold-ready fields on the governance readout.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesContestedTerritoryPressureSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityContestedTerritoryPressureSmokeValidation.ps1`
  now provide dedicated proof for realm-condition counting, governance
  blocking, and contest-clear recovery.
- The committed `unity/Assembly-CSharp*.csproj` files retain the contested
  territory compile includes while preserving the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer roots.

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
- Dedicated contested-territory pressure smoke on the merged result:
  - `Contested territory pressure smoke validation passed.`

## Exact Next Action

1. Start from the refreshed canonical `master`.
2. Claim the next unblocked non-AI Codex lane from
   `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`.
3. Do not reopen the stale historical
   `codex/unity-contested-territory-pressure` branch.
