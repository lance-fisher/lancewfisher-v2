# 2026-04-23 Unity Fortification Postures

## Slice Summary

- Lane: `fortification-postures`
- Branch: `codex/unity-fortification-postures`
- Browser reference searches used:
  - `IMMINENT_ENGAGEMENT_POSTURES`
  - `tickImminentEngagementWarnings`

## What Landed

- Added `ImminentEngagementPostureComponent`, `PlayerImminentEngagementPostureRequestComponent`, and `ImminentEngagementPostureSystem` under `unity/Assets/_Bloodlines/Code/Fortification/` so active imminent-engagement windows now materialize explicit brace / steady / counterstroke posture data on each settlement.
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.Posture.cs` so posture selection can be forced by settlement id without reopening the older fortification debug file.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementPostureSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityImminentEngagementPostureSmokeValidation.ps1` to prove brace healing, counterstroke frontline damage, player request cleanup, and posture removal after the warning resolves.
- Narrow shared-file seams now consume posture effects:
  - `ImminentEngagementWarningSystem.cs` preserves player-selected posture ids while still auto-resolving AI choices.
  - `FortificationReserveSystem.cs` applies posture-driven reserve healing, muster tempo, desired frontline count, and retreat-threshold adjustment.
  - `AttackResolutionSystem.cs` applies the live counterstroke frontline bonus to fortification-linked defenders on the active line.
  - `CombatStanceResolutionSystem.cs` applies the live brace / counterstroke retreat-threshold adjustment on the defender retreat seam.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now explicitly include the new runtime and editor files for this slice.
- This worktree now links `unity/Library` to the canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so the governed .NET build gates resolve Unity `ScriptAssemblies` and `PackageCache` correctly.

## Validation

- Dedicated imminent-engagement posture smoke: PASS
  - `Imminent engagement posture smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed 10-gate chain:
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation returned exit code 0.`
  - `Canonical scene shell validation passed.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation returned exit code 0.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=96, last-updated=2026-04-23 is current.`

## Deferred / Notes

- The browser posture definitions expose additive retreat-threshold bonuses (`+0.05` brace, `-0.02` counterstroke), while the directive asked for a `RetreatThresholdMultiplier` field. `ImminentEngagementPostureComponent` stores the requested multiplier field and also preserves the canonical additive offset so the live retreat seam stays browser-faithful.
- The worktree-local `unity/Library` junction is not a repo asset; it is an environment repair so detached worktrees can reuse the canonical Unity import state instead of failing the governed build gates on missing `ScriptAssemblies`.

## Exact Next Action

- Claim Priority 7 `faith-verdant-warden` from `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`, branch from updated `master`, and port Verdant Warden faith support with a dedicated smoke validator.
