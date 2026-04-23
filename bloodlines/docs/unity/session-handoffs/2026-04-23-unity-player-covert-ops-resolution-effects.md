# 2026-04-23 Unity Player Covert Ops Resolution Effects

## Slice Summary

- Lane: `player-covert-ops`
- Branch: `codex/unity-player-covert-ops-resolution-effects`
- Browser references / search terms used:
  - `createDynastyIntelligenceReport`
  - `recordCounterIntelligenceInterception`
  - `getEspionageContest`
  - `getAssassinationContest`
  - `applyAssassinationEffect`
  - `tickBuildingStatusEffects`
  - `SABOTAGE_DURATIONS`
  - `LEGITIMACY_LOSS_HEAD_FALL`
  - `LEGITIMACY_LOSS_COMMANDER_KILL`
  - `LEGITIMACY_LOSS_GOVERNOR_LOSS`
  - `LEGITIMACY_LOSS_INTERREGNUM`
  - `LEGITIMACY_RECOVERY_ON_SUCCESSION`

## What Landed

- Split ready covert-op resolution out of
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCounterIntelligenceSystem.cs`
  into `EspionageResolutionSystem`, `AssassinationResolutionSystem`, and
  `SabotageResolutionSystem` so dispatch stays lane-local while resolve-time
  fallout can run against live dynasty, death, and production state without
  editing `unity/Assets/_Bloodlines/Code/AI/**`.
- Added `PlayerSabotageStatusComponent` so successful sabotage now persists
  building-local effect timers, production halts, gate exposure, burn damage,
  and subtype-specific health floors. `SabotageResolutionSystem` compensates
  queue time after `UnitProductionSystem` so sabotage can freeze production
  without mutating foreign runtime systems.
- `AssassinationResolutionSystem` now kills the targeted dynasty member, writes
  `DynastyFallenLedger`, applies browser-aligned legitimacy and conviction
  fallout by role, clears governor assignments when needed, kills linked
  commander units, and leaves ruler deaths visible to same-frame
  `DynastySuccessionSystem` and `SuccessionCrisisEvaluationSystem`.
- `EspionageResolutionSystem` now writes richer dossiers on success and applies
  hostility, legitimacy, stewardship, and attacking-watch penalties on failure.
- `IntelligenceReportElement` and
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  now expose building/resource dossier summaries plus sabotage status readouts.
- Added `BloodlinesPlayerCovertOpsResolutionSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityPlayerCovertOpsResolutionSmokeValidation.ps1`
  to prove assassination succession fallout, sabotage production freeze,
  dossier content, and failure penalties.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime and editor files.

## Validation

- Dedicated covert-ops resolution smoke: PASS
  - `Player covert-ops resolution smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed full validation chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=102, last-updated=2026-04-23 is current.`

## Notes

- The dedicated smoke initially surfaced a weak espionage-failure fixture. The
  validator now zeroes the player faction's fallback diplomat/merchant renown
  and raises the defender watch before resolution so the failure branch is
  deterministic instead of depending on default dynasty bootstrap roles.
- The existing
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  modification and
  `unity/Library.pre-junction-20260423-player-captive-ransom-trickle/`
  directory were preserved and must remain unstaged.
- This worktree continues to rely on the local `unity/Library` junction to the
  canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so governed
  `.csproj` builds can resolve `Library\ScriptAssemblies`.

## Exact Next Action

1. Claim Priority 12 `governance-coalition-pressure` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open a fresh branch from updated `master`.
3. Port the coalition-pressure seam with a dedicated smoke validator and rerun
   the full governed 10-gate chain.
