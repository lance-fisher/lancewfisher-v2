# Unity Player Diplomacy Slice: Non-Aggression Pact Proposal And Break

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-pact-proposal`
- Status: validated on branch

## Goal

Port the browser player-side non-aggression pact seam under
`unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` without widening the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove both pact
creation and pact break penalties with a dedicated smoke validator plus the
full governed validation chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `NON_AGGRESSION_PACT_INFLUENCE_COST` (5126)
  - `NON_AGGRESSION_PACT_GOLD_COST` (5127)
  - `NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS` (5128)
  - `NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST` (5129)
  - `getNonAggressionPactTerms` (5150-5183)
  - `proposeNonAggressionPact` (5185-5222)
  - `breakNonAggressionPact` (5224-5257)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerPactProposalRequestComponent.cs`
  and
  `PlayerPactBreakRequestComponent.cs`
  now define the player-owned request surfaces for pact creation and explicit
  pact dissolution.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerPactUtility.cs`
  now centralizes the browser pact constants, active-pact lookup, hostility
  removal/restoration, pact-id construction, legitimacy plus oathkeeping
  break penalties, and narrative emission so the player lane stays narrow and
  self-contained.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerPactProposalSystem.cs`
  now ports the browser pact-terms and proposal path for player issuance:
  kingdom-only validation, self-target rejection, hostility gate, existing
  pact rejection, canonical `influence=50` / `gold=80` deduction, hostility
  removal, AI-owned `PactComponent` creation, and narrative emission.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerPactBreakSystem.cs`
  now ports the browser pact-break seam for explicit player action: active
  pact lookup, `Broken=true` plus `BrokenByFactionId`, hostility restoration
  both ways, dynasty legitimacy `-8`, conviction oathkeeping `-2`, and the
  browser-parity early-break narrative suffix.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes:
  - `TryDebugIssuePlayerPactProposal(...)`
  - `TryDebugIssuePlayerPactBreak(...)`
  - `TryDebugGetPlayerPacts(...)`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerPactSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerPactSmokeValidation.ps1`
  now prove four phases:
  1. proposal success creates the pact, deducts canonical influence and gold,
     and clears hostility
  2. an existing active pact blocks duplicate creation and preserves stockpile
  3. early break marks the pact broken, restores hostility, and applies
     legitimacy/oathkeeping penalties
  4. insufficient influence/gold blocks creation
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
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Scene shells:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
- Siege smoke:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=74, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player pact smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice reuses the AI-owned single-entity `PactComponent` representation
  rather than mirroring the browser's two faction-local pact records.
- The browser holy-war pact gate remains deferred here just as it remains
  deferred in the AI pact-execution slice; the player slice matches the live
  Unity pact surface rather than introducing a one-off divergence.
- Player pact issuance and break currently flow through the debug command
  surface only; no gameplay HUD action has been added yet.

## Exact Next Action

1. Stage the player pact slice files plus contract and continuity updates,
   commit them on `codex/unity-player-pact-proposal`, and push to `origin`.
2. Merge the validated branch to canonical `master` through the governed merge
   flow, then rerun the full 10-gate validation chain on merged `master`.
3. After the landing continuity pass, return to the lower-priority Codex HUD
   backlog, with fortification or victory-distance readout follow-up work next.
