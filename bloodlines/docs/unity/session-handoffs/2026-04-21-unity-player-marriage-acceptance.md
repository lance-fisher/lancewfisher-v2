# Unity Player Marriage Diplomacy Sub-Slice 2B: Acceptance And Effects

- Date: 2026-04-21
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-marriage-acceptance`
- Status: complete on branch, pending merge to `master`

## Goal

Port the browser's player-facing `acceptMarriage` and `getMarriageAcceptanceTerms` flow into ECS under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`, staying out of Claude's AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane.

## Browser Reference

- `src/game/core/simulation.js`
  - `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (6091-6095)
  - `getMarriageAuthorityProfile` (6134-6190)
  - `getMarriageEnvoyProfile` (6192-6215)
  - `buildMarriageGovernanceStatus` (6217-6230)
  - `applyMarriageGovernanceLegitimacyCost` (6232-6245)
  - `getMarriageAcceptanceTerms` (6327-6361)
  - `acceptMarriage` (7388-7469)
- `tests/runtime-bridge.mjs`
  - marriage baseline and acceptance assertions (2072-2113, 2240-2308)

## Work Completed

- Added `PlayerMarriageAcceptRequestComponent` and `PlayerMarriageAcceptSystem` under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
- Ported the acceptance gate chain around pending-proposal lookup, source/target dynasty presence, source/target member availability, and target-side authority resolution through the player-owned `PlayerMarriageAuthorityEvaluator`.
- On success, the acceptance system now:
  - flips `MarriageProposalComponent.Status` to `Accepted`
  - creates primary + mirror `MarriageComponent` records with canonical gestation timing
  - applies target-side governance legitimacy cost before the +2 legitimacy bonus
  - records oathkeeping +2 on both factions through `ConvictionScoring.ApplyEvent`
  - removes hostility both ways
  - enqueues the 30-day `DeclareInWorldTimeRequest`
- Extended `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` with:
  - `TryDebugIssuePlayerMarriageAccept(proposalEntityIndex)`
  - proposal readout entity indices so the smoke validator can target the exact pending proposal
- Added `BloodlinesPlayerMarriageAcceptanceSmokeValidation` plus `scripts/Invoke-BloodlinesUnityPlayerMarriageAcceptanceSmokeValidation.ps1`.
- Added Unity `.meta` files for the new PlayerDiplomacy and Editor scripts.
- Updated the lane contract to claim the acceptance validator and wrapper and to mark sub-slice 2B complete on the feature branch.

## Validation Proof Lines

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)`
- Bootstrap runtime smoke: `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat smoke: `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell: `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell: `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke: `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke: `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Dedicated smoke marker: `BLOODLINES_PLAYER_MARRIAGE_ACCEPTANCE_SMOKE PASS`
- Dedicated smoke success proof:
  - `Phase 2 PASS: proposal accepted, marriageCount=2, legitimacy=82/72, oathkeeping=5/3, dualClockDays=50`
  - `Phase 4 PASS: heir-regency cost applied, legitimacy=81, stewardship=2`

## Unity-Side Simplifications Deferred

- Unity reuses the existing `MarriageProposalComponent` shape, so accepted-authority provenance and accepted-at timestamps that the browser stores on the proposal are not persisted yet.
- The browser pushes a narrative message on accept; this slice does not emit a player-side narrative bridge entry yet because the directive prioritized the mechanical acceptance effects and the AI narrative path remains in a separate lane.
- The debug accept seam uses `proposalEntityIndex` from the structured pending-proposal readout instead of a browser-style proposal id lookup.

## Exact Next Action

1. Merge `codex/unity-player-marriage-acceptance` to `master`.
2. Rerun the full governed validation gate on merged `master`.
3. Start sub-slice 2C on fresh branch `codex/unity-player-marriage-dissolution` and port death-driven marriage dissolution without touching `unity/Assets/_Bloodlines/Code/AI/**`.
