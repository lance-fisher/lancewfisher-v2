# Unity Player Diplomacy Slice: Captive Rescue Dispatch

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-captive-rescue`
- Status: validated on branch

## Goal

Port the browser player-side captive rescue dispatch seam under
`unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` without widening the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove successful
dispatch plus the main rejection paths with a dedicated smoke validator and
the full governed validation chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `getCapturedMemberRescueTerms` (~11153-11234)
  - `startRescueOperation` (~11236-11341)
- `src/game/core/ai.js`
  - captive recovery contest / operator selection logic (~2550-2760)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRescueRequestComponent.cs`
  now defines the player-owned request surface for captive rescue issuance by
  source faction id plus captive member id.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveDispatchUtility.cs`
  now centralizes captured-member lookup, held-captive resolution against the
  AI-owned `CapturedMemberElement` buffers, rescue operation id construction,
  and rescue narrative emission so the player slice stays narrow and additive.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRescueDispatchSystem.cs`
  now ports the browser rescue-dispatch seam for player issuance:
  kingdom-only validation, captured-member verification, held-captive lookup,
  active-operation rejection, operator selection using the canonical
  spymaster/diplomat fallback priorities, canonical `gold=42` /
  `influence=26` deduction, AI-owned `DynastyOperationComponent` plus
  `DynastyOperationCaptiveRescueComponent` creation, and player-visible
  narrative emission.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes `TryDebugDispatchCaptiveRescue(...)` so the player-facing rescue
  path can be issued from the existing diplomacy command surface.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRescueSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCaptiveRescueSmokeValidation.ps1`
  now prove three phases:
  1. rescue success creates the AI-owned operation, deducts canonical gold and
     influence, and schedules the canonical duration
  2. missing eligible rescue operators blocks dispatch and preserves stockpile
  3. a missing captive member id blocks dispatch and preserves stockpile
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
  - `0 Error(s)` with existing repo-wide editor warnings only
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
  - `STALENESS CHECK PASSED: Contract revision=80, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player captive rescue smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice reuses the AI-owned `DynastyOperationCaptiveRescueComponent`
  payload and `CapturedMemberElement` holding model rather than introducing a
  parallel player-owned captive store.
- Rescue requests currently flow through the debug command surface only; no
  gameplay HUD button or panel wiring has been added yet.
- Member ids remain string-backed `FixedString64Bytes` identifiers in Unity so
  they stay compatible with the already-landed dynasty roster surfaces instead
  of introducing a one-off numeric id path.

## Exact Next Action

1. Stage the player captive rescue slice files plus contract and continuity
   updates, commit them on `codex/unity-player-captive-rescue`, and push to
   `origin`.
2. Continue the next player-diplomacy captive sub-slice on a fresh branch:
   player captive ransom dispatch.
