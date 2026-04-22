# Unity Player Diplomacy Slice: Holy War And Divine Right Declarations

- Date: 2026-04-22
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-holy-war-divine-right`
- Status: validated on branch

## Goal

Port the player-side holy war and divine-right declaration dispatch seams under
`unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` without widening the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove the slice with
a dedicated editor smoke validator plus the full governed 10-gate validation
chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `getHolyWarDeclarationTerms` (~10424-10471)
  - `startHolyWarDeclaration` (~10565-10602)
  - `getDivineRightDeclarationTerms` (~10604-10653)
  - `startDivineRightDeclaration` (~10784-10835)

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerHolyWarDeclarationRequestComponent.cs`
  and
  `PlayerDivineRightDeclarationRequestComponent.cs`
  now define the player-owned request surfaces for issuing holy-war and
  divine-right declarations from the debug bridge or future UI surfaces.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerFaithDeclarationUtility.cs`
  centralizes faction-root resolution, kingdom gating, doctrine compatibility,
  faith-operator selection, active-operation scans, and narrative helpers so
  the declaration systems stay additive and lane-local.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerHolyWarDeclarationSystem.cs`
  now ports the browser holy-war gate chain for player issuance: source/target
  kingdom validation, committed-faith validation, same-faith/same-doctrine
  harmony rejection, duplicate-active-op rejection, operator lookup, canonical
  influence and intensity deduction, `DynastyOperationLimits` capacity
  enforcement, AI-owned `DynastyOperationComponent` plus
  `DynastyOperationHolyWarComponent` creation, and narrative emission through
  `NarrativeMessageBridge`.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerDivineRightDeclarationSystem.cs`
  now ports the player-side divine-right gate chain: committed faith,
  intensity >= 80, level >= 5, no existing active declaration for the same
  faction, capacity enforcement, AI-owned `DynastyOperationComponent` plus
  `DynastyOperationDivineRightComponent` creation, and narrative emission.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes:
  - `TryDebugIssuePlayerHolyWarDeclaration(...)`
  - `TryDebugIssuePlayerDivineRightDeclaration(...)`
  - `TryDebugGetPlayerFaithDeclarationOperations(...)`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerHolyWarDivineRightSmokeValidation.ps1`
  now prove four phases:
  1. holy-war success against an incompatible-faith kingdom
  2. holy-war rejection for harmonious same-faith/same-doctrine targets
  3. divine-right success at intensity 85 and faith level 5
  4. divine-right rejection below the canonical intensity threshold
- Narrow shared-file edits were applied to:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`
  so the new runtime and editor files compile in the generated local project.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Warning(s)`
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
  - `STALENESS CHECK PASSED: Contract revision=71, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player holy war/divine right smoke validation passed.`

## Unity-Side Simplifications Deferred

- Holy-war compatibility still uses identical `(faith, doctrine)` equality
  rather than the browser's fuller compatibility ladder because the covenant
  covariance surface is not yet ported.
- Divine-right side effects still defer the broader browser follow-up surface:
  rival-hostility expansion, recognition-share calculation, apex-structure
  gating, and conviction-event recording remain outside this player-side
  dispatch slice.
- The new player declaration systems reuse AI-owned dynasty-operation payload
  shapes read-only; they do not widen the `AI/**` lane or move per-kind
  resolution out of the existing dynasty-operation model.

## Exact Next Action

1. Stage the player-faith declaration slice files plus continuity and contract
   updates, commit them on `codex/unity-player-holy-war-divine-right`, and push
   to `origin`.
2. Merge the branch to canonical `master` through the normal governed merge
   flow, then rerun the full 10-gate validation chain on merged `master`.
3. After the landing continuity pass, continue the next directive item from the
   current priority stack on a fresh Codex branch.
