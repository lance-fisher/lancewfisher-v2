# 2026-04-23 Unity Player Covert Ops Resolution Effects Follow-Up

## Slice Summary

- Lane: `player-covert-ops`
- Branch: `codex/unity-player-covert-ops-resolution-effects-followup`
- Browser references / search terms used:
  - `createDynastyIntelligenceReport`
  - `recordCounterIntelligenceInterception`
  - `applyAssassinationEffect`
  - `tickBuildingStatusEffects`
  - `SABOTAGE_DURATIONS`
  - `LEGITIMACY_LOSS_COMMANDER_KILL`
  - `LEGITIMACY_LOSS_GOVERNOR_LOSS`

## What Landed

- Replayed the validated covert-ops runtime onto current `master` as an
  additive follow-up instead of rewriting the already-landed historical
  `codex/unity-player-covert-ops-resolution-effects` branch.
- `EspionageResolutionSystem`,
  `AssassinationResolutionSystem`,
  and
  `SabotageResolutionSystem`
  now consume the stored dispatch-time `SuccessScore` directly, so resolve-time
  outcomes match the projected operation window rather than recomputing the
  contest later against drifted live state.
- Added `PlayerSabotageEffectComponent` as the active sabotage-effect carrier
  for gate-opening and fire-raising timers while preserving the prior
  `PlayerSabotageStatusComponent` file as a compatibility surface already
  landed on master.
- `SabotageResolutionSystem` now:
  - promotes gate-opening breaches in the same resolve frame
  - applies elapsed-time burn damage
  - writes `BuildingRaidStateComponent` poison windows for supply poisoning
  - raises `RealmConditionComponent.WaterStrainStreak` on successful well
    poisoning
- `AssassinationResolutionSystem` now clears commander/governor attachments via
  narrow lane-local hooks instead of widening foreign runtime systems, and
  `EspionageResolutionSystem` plus `IntelligenceReportElement` now emit tighter
  building/resource dossier summaries for the debug surface.
- `BloodlinesDebugCommandSurface.PlayerCovertOps` now reports the richer
  dossier fields plus the live sabotage-effect / raid state readout, and
  `BloodlinesPlayerCovertOpsResolutionSmokeValidation` now proves enriched
  espionage, commander/governor fallout, same-frame gate-opening breach
  exposure, and the fire / supply / well sabotage variants.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include `PlayerSabotageEffectComponent` and point Unity.Entities
  analyzer roots back at the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` surface.

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
  - `STALENESS CHECK PASSED: Contract revision=103, last-updated=2026-04-23 is current.`

## Notes

- The branch-local worktree still relies on the local `unity/Library` junction
  to the canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so the
  governed `.csproj` builds can resolve `Library\ScriptAssemblies`.
- Keep the existing
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  modification unstaged. Unity dirties it during validation and it is
  unrelated to this follow-up.

## Exact Next Action

1. Claim Priority 12 `governance-coalition-pressure` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`.
2. Open a fresh `codex/unity-governance-coalition-pressure` branch from
   updated `master`.
3. Port the coalition-pressure seam with a dedicated smoke validator and rerun
   the full governed 10-gate chain.
