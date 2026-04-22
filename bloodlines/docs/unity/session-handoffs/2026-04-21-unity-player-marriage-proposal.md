# Unity Player Marriage Proposal (Sub-Slice 2A)

Date: 2026-04-21  
Lane: `player-marriage-diplomacy`  
Branch: `codex/unity-player-marriage-proposal`

## Goal

Port the browser's player-facing `proposeMarriage` path into Unity ECS under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`, reusing the existing `MarriageProposalComponent` shape and adding a dedicated player debug surface plus a dedicated smoke validator.

## Browser Reference Line Numbers

- `src/game/core/simulation.js`
  - `MARRIAGE_REGENCY_LEGITIMACY_COSTS` lines `6091-6095`
  - `getMarriageAuthorityProfile` lines `6134-6190`
  - `getMarriageEnvoyProfile` lines `6192-6215`
  - `buildMarriageGovernanceStatus` lines `6217-6230`
  - `applyMarriageGovernanceLegitimacyCost` lines `6232-6245`
  - `getMarriageProposalContext` lines `6247-6274`
  - `getMarriageProposalTerms` lines `6296-6325`
  - `factionAllowsPolygamy` lines `6431-6433`
  - `memberHasActiveMarriage` lines `7260-7264`
  - `proposeMarriage` lines `7340-7386`
- `tests/runtime-bridge.mjs`
  - marriage proposal / acceptance proof block lines `2072-2113`

## Work Completed

- Added `PlayerMarriageProposalRequestComponent` as the one-shot request seam for player-issued proposals.
- Added `PlayerMarriageAuthorityEvaluator` under the new lane instead of writing into Claude's AI-owned `MarriageAuthorityEvaluator.cs`.
- Added `PlayerMarriageProposalSystem` to port the browser proposal gates:
  - source/target factions must both carry dynasties
  - same-faction proposals are rejected
  - source/target members must be `Active` or `Ruling`
  - active-marriage + non-polygamy blocks proposal creation on either side
  - source governance must have both an authority chain and an envoy path
  - exact duplicate pending proposal pairs are rejected
  - proposal expiration is seeded from `MarriageProposalExpirationSystem.ExpirationInWorldDays`
  - source-side regency legitimacy cost is applied on proposal creation
- Added `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` with:
  - `TryDebugIssuePlayerMarriageProposal(sourceFactionId, targetMemberId)`
  - `TryDebugGetPlayerMarriageProposals(factionId, out readout)`
- Added dedicated validator `BloodlinesPlayerMarriageProposalSmokeValidation` with the required 4 phases:
  - clean baseline
  - valid proposal creation
  - duplicate block
  - already-married block
- Added wrapper `scripts/Invoke-BloodlinesUnityPlayerMarriageProposalSmokeValidation.ps1`.
- Ran the full governed 10-step validation gate in the worktree. The root-pinned bootstrap-runtime and scene-shell wrappers were executed through temporary worktree-local copies under the Unity wrapper lock so they validated this branch rather than the canonical-root alias.

## Validation Proof Lines

- `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `113 Warning(s)`
  - `0 Error(s)`
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ... productionProgressAdvancementVerified=True ... constructionProgressAdvancementVerified=True ... gatherDepositObserved=True ... aiConstructionObserved=True ...`
- `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. ...`
- `node tests/data-validation.mjs`
  - `Bloodlines data validation passed.`
- `node tests/runtime-bridge.mjs`
  - `Bloodlines runtime bridge validation passed.`
- `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
  - `STALENESS CHECK PASSED: Contract revision=52, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated slice validator
  - `BLOODLINES_PLAYER_MARRIAGE_PROPOSAL_SMOKE PASS`
  - `Phase 1 PASS: PendingProposalCount=0`
  - `Phase 2 PASS: PendingProposalCount=1, source=player-bloodline-heir, target=enemy-bloodline-heir, legitimacy=57`
  - `Phase 3 PASS: duplicate request preserved a single pending proposal`
  - `Phase 4 PASS: active marriage blocked proposal creation`

## Unity-Side Simplifications Deferred

- The ECS proposal entity still reuses the existing `MarriageProposalComponent` shape only; the richer browser-side governance preview object (`sourceAuthority`, `sourceEnvoy`, `targetAuthorityPreview`) is not yet stored on proposal entities.
- The duplicate guard currently blocks the exact same source-member / target-member pending pair. It does not yet implement a richer per-member or per-faction inbox dedupe policy.
- The browser's narrative message push from `proposeMarriage` is deferred for a later player-diplomacy / narrative follow-up. Sub-slice 2A only required the simulation action, debug surface, and validator.

## Exact Next Action

Start sub-slice 2B on a fresh `codex/unity-player-marriage-acceptance` branch and port `acceptMarriage` plus `getMarriageAcceptanceTerms` into `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`, reusing the just-landed request/debug lane surfaces.
