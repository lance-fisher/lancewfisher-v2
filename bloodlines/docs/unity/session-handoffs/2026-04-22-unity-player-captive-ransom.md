# Unity Player Diplomacy Slice: Captive Ransom Dispatch

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-captive-ransom-dispatch`
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
  - captive recovery dispatch block (~2576-2595)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRansomRequestComponent.cs`
  now defines the player-owned request surface for captive ransom issuance by
  source faction id, captive member id, resolved target faction id, and the
  player-requested ransom gold amount.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveDispatchUtility.cs`
  now centralizes player ransom operation id construction and the browser-parity
  ransom narrative seam alongside the already-landed rescue helpers.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRansomDispatchSystem.cs`
  now ports the browser ransom-dispatch seam for player issuance:
  kingdom-only validation, captured-member verification, held-captive lookup,
  duplicate-active-operation rejection, diplomat/merchant operator selection
  using the AI-owned role-priority helpers, hostility gating against the captor,
  canonical `gold=70` / `influence=18` deduction, AI-owned
  `DynastyOperationComponent` plus
  `DynastyOperationCaptiveRansomComponent` creation, and player-visible
  narrative emission.
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
  3. hostility to the captor blocks dispatch and preserves stockpile
- Narrow shared-file edits were applied to:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`
  so the new runtime and editor files compile in the initialized local
  project, and the generated `Library` reference paths now point at the
  canonical root's Unity cache because this worktree has no local `unity/Library`.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)` with existing unresolved-reference warnings only
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
  - `STALENESS CHECK PASSED: Contract revision=82, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player captive ransom smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice reuses the AI-owned
  `DynastyOperationCaptiveRansomComponent` payload and
  `CapturedMemberElement` holding model rather than introducing a parallel
  player-owned captive store.
- The player request surface carries `RansomGoldAmount` for future UI parity,
  but the dispatch still enforces the currently-landed canonical Unity base
  cost constants from `AICaptiveRansomExecutionSystem` because the captor-envoy
  premium and renown-scaled browser terms are not yet ported onto the captive
  holding surface.
- Ransom requests currently flow through the debug command surface only; no
  gameplay HUD button or panel wiring has been added yet.

## Exact Next Action

1. Stage the player captive ransom slice files plus contract and continuity
   updates, commit them on `codex/unity-player-captive-ransom-dispatch`, and
   push to `origin`.
2. Continue with the next unblocked Codex priority after landing: renown /
   prestige scoring or the HUD victory leaderboard follow-up.
