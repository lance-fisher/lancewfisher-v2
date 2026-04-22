# Unity Player Diplomacy Slice: Missionary Dispatch

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-missionary-dispatch`
- Status: validated on branch

## Goal

Port the player-side missionary dispatch seam under
`unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` without widening the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove the slice with
a dedicated editor smoke validator plus the full governed validation chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `getMissionaryTerms` (~10362-10421)
  - `startMissionaryOperation` (~10523-10563)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerMissionaryDispatchRequestComponent.cs`
  now defines the player-owned request surface for issuing a missionary
  operation from the debug bridge or future UI surfaces.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerMissionaryDispatchSystem.cs`
  now ports the browser missionary gate chain for player issuance: source and
  target faction validation, committed-faith validation, same-faith rejection,
  duplicate-active-op rejection, faith-operator lookup, canonical
  `influence=14` / `intensity=12` deduction, `DynastyOperationLimits`
  enforcement, AI-owned `DynastyOperationComponent` plus
  `DynastyOperationMissionaryComponent` creation, and narrative emission
  through `NarrativeMessageBridge`.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerFaithDeclarationUtility.cs`
  now exposes a renown-returning faith-operator lookup overload and a
  player-side missionary narrative helper so the missionary slice stays within
  the existing `PlayerDiplomacy/**` utility surface.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes:
  - `TryDebugIssuePlayerMissionaryDispatch(...)`
  - missionary payload fields in `TryDebugGetPlayerFaithDeclarationOperations(...)`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMissionaryDispatchSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerMissionaryDispatchSmokeValidation.ps1`
  now prove three phases:
  1. missionary success creates the AI-owned operation payload and deducts
     canonical influence/intensity
  2. insufficient influence blocks creation and preserves stockpile
  3. active-operation capacity at six blocks dispatch
- Narrow shared-file edits were applied to:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`
  so the new runtime and editor files compile in the initialized local
  project.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)`
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
  - `STALENESS CHECK PASSED: Contract revision=72, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player missionary dispatch smoke validation passed.`

## Unity-Side Simplifications Deferred

- Missionary defense continues to use the same simplified port already used by
- the AI missionary dispatch slice: target-operator renown and faith-ward
- bonuses remain deferred, so defense currently uses target intensity only.
- The slice only dispatches missionary operations; it does not widen the
  missionary resolution surface already owned by the AI lane.
- Player missionary readout currently reuses the existing faith-declaration
  debug surface rather than creating a separate player-diplomacy summary API.

## Exact Next Action

1. Stage the player missionary dispatch slice files plus contract and
   continuity updates, commit them on `codex/unity-player-missionary-dispatch`,
   and push to `origin`.
2. Merge the branch to canonical `master` through the governed merge flow, then
   rerun the full 10-gate validation chain on merged `master`.
3. After the landing continuity pass, continue the next directive item from the
   current player-facing priority stack.
