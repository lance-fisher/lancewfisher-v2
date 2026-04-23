# Unity Player Covenant Test Dispatch Landing

- Date: 2026-04-23
- Lane: `faith-covenant-test`
- Branch: `codex/unity-player-covenant-test-dispatch`
- Status: merged to canonical `master` and revalidated

## Goal

Land the validated player covenant-test dispatch slice onto canonical
`master`, rerun the governed validation chain on the merged result, and leave
the next Codex pickup pointed at Priority 19
`codex/unity-contested-territory-pressure`.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestDispatchStateComponent.cs`
  and `PlayerCovenantTestDispatchSystem.cs` now live on canonical `master`,
  so the player faction root exposes covenant-test rite availability,
  affordability, action label/detail, queued state, pending request state,
  and covenant-specific cost display before `CovenantTestResolutionSystem`
  consumes the same request shape.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs`
  now exposes `TryDebugQueueCovenantTestDispatch(...)` and
  `TryDebugGetCovenantTestDispatchState(...)` on the merged line while
  preserving the older direct trigger path used by the legacy
  covenant-test validator.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovenantTestDispatchSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCovenantTestDispatchSmokeValidation.ps1`
  now live on canonical `master` as the dedicated proof surface.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`
  now retain the additive compile includes for the new runtime/editor files
  and the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer roots
  after the landing validation pass rewrote them to the detached worktree
  path.

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
- Dedicated smoke on merge result:
  - `Player covenant-test dispatch smoke validation passed.`

## Exact Next Action

1. Start the next fresh `codex/unity-contested-territory-pressure` branch
   from the updated canonical `master`.
2. Read the territorial pressure seam in `src/game/core/simulation.js` and
   the current Unity `WorldPressure/**` surfaces.
3. Port the next additive non-AI slice with its own dedicated smoke
   validator and matching PowerShell wrapper.
