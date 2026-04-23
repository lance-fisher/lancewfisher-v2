# 2026-04-22 Unity Territory Governor Specialization

## Slice Summary

- Lane: `territory-governor-specialization`
- Branch: `codex/unity-territory-governor-specialization`
- Browser reference searches used:
  - `DEFAULT_GOVERNOR_SPECIALIZATION`
  - `GOVERNOR_SPECIALIZATION_PROFILES`
  - `getGovernorSpecializationIdForSettlementClass`
  - `getGovernanceSeatMembers`
  - `getGovernanceSeatMemberScore`
  - `syncGovernorAssignments`
  - `getGovernorProfileForControlPoint`

## What Landed

- Added `GovernorSeatAssignmentComponent`, `GovernorSpecializationComponent`, `GovernorSpecializationCanon`, and `GovernorSpecializationSystem` under `unity/Assets/_Bloodlines/Code/TerritoryGovernance/` so dynasty governance members now auto-assign once per in-world day to owned control points and settlements using browser-aligned role priority plus specialization scoring.
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Governance.cs` so control-point specialization state and faction assignment summaries are queryable without widening AI-owned paths.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGovernorSpecializationSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityGovernorSpecializationSmokeValidation.ps1` to prove border assignment, live consumer gains, and ungoverned cleanup.
- Narrow shared-file seams now consume governor specialization effects:
  - `ControlPointResourceTrickleSystem.cs` applies the canonical governor trickle base bonus plus specialization multiplier.
  - `ControlPointCaptureSystem.cs` applies governor stabilization, capture resistance, and loyalty-protection on hostile capture drain.
  - `FortificationReserveSystem.cs` applies keep-governor reserve muster and heal multipliers to settlement-linked reserves.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now explicitly include the new runtime/editor files for this slice.

## Validation

- Dedicated governor specialization smoke: PASS
  - `Governor specialization smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build: PASS
  - `Build succeeded.`
- Governed 10-gate chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Fortification smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=93, last-updated=2026-04-22 is current.`

## Deferred / Notes

- The specialization lane intentionally consumed loyalty protection on the control-point capture drain path rather than reopening unrelated faction-loyalty or AI-owned systems.
- Unity still emits pre-existing ordering warnings for some older system attributes during batch validation. They did not block the governor slice and were not widened in this pass beyond the new territory-governance lane.
- The clean short-path worktree needed a local `unity/Library` junction to the canonical root so generated Unity references resolved during unattended validation. This is an environment aid, not a source change.

## Exact Next Action

- Claim Priority 5 `combat-commander-aura` from `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`, branch from updated `master`, and wire commander aura buffs plus dedicated smoke validation.
