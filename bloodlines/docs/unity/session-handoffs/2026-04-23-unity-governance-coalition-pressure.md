# 2026-04-23 Unity Governance Coalition Pressure

## Slice Summary

- Lane: `world-governance-coalition`
- Branch: `codex/unity-governance-coalition-pressure`
- Browser references / search terms used:
  - `getTerritorialGovernanceAcceptanceProfile`
  - `shouldIssueTerritorialGovernanceRecognition`
  - `tickTerritorialGovernanceRecognition`
  - `getTerritorialGovernanceWorldPressureContribution`
  - `governanceAlliancePressureActive`
  - `GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE`
  - `GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE`

## What Landed

- Added `TerritorialGovernanceRecognitionComponent` as the live kingdom-root
  state surface for territorial-governance recognition, including acceptance
  seed/target/rise/fall fields, sustain/victory timers, coalition drag, weakest
  frontier tracking, and cached world-pressure contribution.
- Added `GovernanceCoalitionPressureSystem` to port the browser
  territorial-governance recognition loop into Unity ECS: kingdom faction roots
  now auto-seed recognition state, evaluate stage/share/seat/war/loyalty
  readiness, grow or decay acceptance toward a browser-grounded target, and
  fire alliance-threshold coalition pressure every 90 seconds against the
  weakest governed march plus faction legitimacy when hostile kingdoms are
  aligned against the leader.
- `WorldPressureComponent` and `WorldPressureEscalationSystem` now carry and
  consume a dedicated `TerritorialGovernanceRecognitionScore`, so active,
  recognized, threshold-crossing, and victory-ready governance states now feed
  the broader world-pressure score instead of remaining a reserved source.
- `VictoryConditionEvaluationSystem` now resolves Territorial Governance from
  the live recognition component first and mirrors the active victory hold into
  `VictoryStateComponent`, while preserving the prior loyalty-only fallback if
  the new component surface is absent.
- `BloodlinesDebugCommandSurface.WorldPressure` now exposes
  `TryDebugGetGovernanceCoalitionState`, and the dedicated
  `BloodlinesGovernanceCoalitionPressureSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityGovernanceCoalitionPressureSmokeValidation.ps1`
  prove bootstrap recognition issuance, alliance-threshold loyalty/legitimacy
  strain, below-threshold non-triggering, and completed-recognition victory
  with enemy governance observability.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new governance coalition runtime and validator files.

## Validation

- Dedicated governance coalition smoke: PASS
  - `BLOODLINES_GOVERNANCE_COALITION_PRESSURE_SMOKE PASS phase1Active=True,acceptance=40.0,recScore=6; phase2Loyalty=79.0,legitimacy=88.4,cycles=1; phase3NoPressureLoyalty=82.0,wpScore=5; phase4Victory=Won,enemyGovernanceVictory=True`
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
  - `STALENESS CHECK PASSED: Contract revision=104, last-updated=2026-04-23 is current.`

## Notes

- The branch-local worktree still relies on the local `unity/Library` junction
  to the canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so the
  governed `.csproj` builds can resolve `Library\ScriptAssemblies`.
- The dedicated smoke validator still emits Unity.Entities ordering warnings
  inside its minimal validation world because
  `GovernorSpecializationSystem` and `MatchProgressionEvaluationSystem` are not
  added to that test-only world. The live runtime ordering remains intentional;
  this warning cleanup can be folded into a later validator hygiene pass.
- Keep the existing
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  modification unstaged. Unity dirties it during validation and it is
  unrelated to this slice.
- Unity-side simplification intentionally kept:
  - the acceptance seed/target/rise/fall curve is browser-grounded but still a
    compact ECS heuristic rather than a verbatim lift of every browser
    intermediate variable; the thresholds, hold timers, coalition cadence, and
    pressure values are the canonical pieces preserved here.

## Exact Next Action

1. Claim Priority 13 `dynasty-minor-house-levy` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`.
2. Open a fresh `codex/unity-dynasty-minor-house-levy-complete` branch from
   updated canonical `master`.
3. Validate the existing minor-house levy runtime against the browser spec,
   fill any decay / loyalty-tier / claim-gate gaps, add or tighten the smoke
   proof, and rerun the full governed 10-gate chain.
