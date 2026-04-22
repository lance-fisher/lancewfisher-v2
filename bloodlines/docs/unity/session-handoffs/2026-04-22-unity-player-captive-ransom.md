# Unity Player Diplomacy Slice: Captive Ransom Dispatch

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-captive-ransom-followup`
- Status: validated on branch

## Goal

Port the browser player-side captive ransom dispatch seam under
`unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` without widening the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove successful
dispatch plus the main rejection paths with a dedicated smoke validator and
the full governed validation chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `getCapturedMemberRansomTerms` (~4929-4966)
  - `startRansomNegotiation` (~11026-11065)
- `src/game/core/ai.js`
  - captive recovery dispatch preference / hostile-player bias (~2566-2608)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRansomRequestComponent.cs`
  now defines the player-owned request surface for captive ransom issuance by
  source faction id, captive member id, resolved captor faction id, and offered
  gold amount.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRansomDispatchSystem.cs`
  now ports the browser ransom-dispatch seam for player issuance:
  kingdom-only validation, captured-member verification, held-captive lookup,
  captor resolution, hostile-captor rejection, active-operation rejection,
  operator selection using the canonical diplomat/merchant fallback priorities,
  minimum ransom enforcement, canonical gold plus fixed influence deduction,
  and AI-owned `DynastyOperationComponent` plus
  `DynastyOperationCaptiveRansomComponent` creation.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveDispatchUtility.cs`
  now also builds player ransom operation ids and emits the canonical ransom
  narrative line so captive-dispatch narrative formatting stays centralized.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes `TryDebugDispatchCaptiveRansom(...)` so the player-facing ransom
  path can be issued from the existing diplomacy command surface.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRansomSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCaptiveRansomSmokeValidation.ps1`
  now prove three phases:
  1. ransom success creates the AI-owned operation, deducts canonical gold and
     influence, and schedules the canonical duration
  2. insufficient gold blocks dispatch and preserves stockpile
  3. hostile captor state blocks dispatch and preserves stockpile
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
  - `STALENESS CHECK PASSED: Contract revision=81, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player captive ransom smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice reuses the AI-owned `DynastyOperationCaptiveRansomComponent`
  payload and `CapturedMemberElement` holding model rather than introducing a
  parallel player-owned captive store.
- Player ransom currently flows through the debug command surface only; no
  gameplay HUD button or panel wiring has been added yet.
- Browser ransom scaling from captive renown, envoy discount, and captor premium
  remains deferred because the Unity captive ledger does not yet carry the full
  browser renown/role detail. This slice enforces the canonical base floor and
  the player-requested gold amount instead.
- The hostile-captor rejection is a Unity-side product rule added by the active
  Codex directive; the browser `getCapturedMemberRansomTerms` path did not gate
  on hostility.

## Exact Next Action

1. Stage the player captive ransom slice files plus contract and continuity
   updates, commit them on `codex/unity-player-captive-ransom-followup`, and
   push to `origin`.
2. Merge the validated branch onto `master` when session capacity allows, then
   continue with the next directive item: the renown and prestige scoring
   surface.
