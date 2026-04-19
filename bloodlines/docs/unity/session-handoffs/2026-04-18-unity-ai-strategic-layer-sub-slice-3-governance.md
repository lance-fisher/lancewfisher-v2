---
lane: ai-strategic-layer
sub-slice: 3
date: 2026-04-18
session: claude-ai-governance-pressure-2026-04-18
branch: claude/unity-ai-governance-pressure
---

# AI Strategic Layer Sub-Slice 3: Governance and Event-Context Pressure

## Goal

Port the governance/event-context timer clamp block from ai.js updateEnemyAi lines 1129-1215 into Unity ECS. This block applies a second layer of timer pressure driven by dynasty, faith, and governance system states: holy war, player territorial governance recognition, player covenant test, player divine right, player/enemy succession crisis, enemy covenant test, and enemy governance recognition.

## Work Completed

### Modified Files

- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs`
  Added 20 governance/event-context flag fields and `BuildTimer`. All flags are written by external dynasty/faith/governance systems (or seeded directly in tests); AIStrategicPressureSystem reads them to apply clamps.

  New fields: `BuildTimer`, `HolyWarActive`, `PlayerGovernanceActive`, `PlayerGovernanceVictoryPressure`, `PlayerGovernanceAlliancePressure`, `PlayerCovenantActive`, `PlayerCovenantTargetsEnemy`, `PlayerDivineRightActive`, `PlayerSuccessionCrisisActive`, `PlayerSuccessionCrisisHigh`, `EnemySuccessionCrisisActive`, `EnemySuccessionCrisisSevere`, `EnemySuccessionCrisisAccumulator`, `EnemyCovenantActive`, `EnemyCovenantTargetsPlayer`, `EnemyCovenantDarkPurgeMandate`, `EnemyCovenantHasTargetPoint`, `EnemyGovernanceActive`, `EnemyGovernanceVictoryPressure`, `EnemyGovernanceAlliancePressure`, `EnemyGovernanceHasTargetPoint`.

- `unity/Assets/_Bloodlines/Code/AI/AIStrategicPressureSystem.cs`
  Extended `OnUpdate` to call `ApplyGovernancePressure` after the existing world-pressure clamp block. `ApplyGovernancePressure` is a new static method that ports lines 1129-1215 verbatim. `BuildTimer` is now decremented alongside the other countdown timers. 37 named constants added for all canonical cap/floor values.

### New Files

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIGovernancePressureSmokeValidation.cs`
  Four-phase smoke validator (static batch-mode entry point pattern):
  - Phase 1: HolyWarActive=true -- AttackTimer<=10, TerritoryTimer<=8.
  - Phase 2: PlayerGovernanceActive + VictoryPressure -- AttackTimer<=3, TerritoryTimer<=2, RaidTimer<=4.
  - Phase 3: PlayerSuccessionCrisisActive + High -- AttackTimer<=5, TerritoryTimer<=4, MarriageProposalTimer<=18.
  - Phase 4: EnemyGovernanceActive + VictoryPressure -- AttackTimer floored >=14, HolyWarTimer floored >=24.

- `scripts/Invoke-BloodlinesUnityAIGovernancePressureSmokeValidation.ps1`
  Self-contained PowerShell runner. Marker: `BLOODLINES_AI_GOVERNANCE_PRESSURE_SMOKE PASS/FAIL`. Artifact: `artifacts/unity-ai-governance-pressure-smoke.log`.

## Verification Results

All validation gates green:

1. `dotnet build unity/Assembly-CSharp.csproj` -- 0 errors
2. `dotnet build unity/Assembly-CSharp-Editor.csproj` -- 0 errors
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0
5. Scene shells -- Bootstrap + Gameplay green
6. AI strategic pressure smoke -- PASS (prior sub-slice 2 still green)
7. AI governance pressure smoke -- PASS (all 4 phases)
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision 15)

## Constants Ported (ai.js lines 1129-1215)

| Condition | Timer | Cap/Floor | Value |
|---|---|---|---|
| Holy war active | AttackTimer | cap | 10 |
| Holy war active | TerritoryTimer | cap | 8 |
| Player gov. victory pressure | AttackTimer | cap | 3 |
| Player gov. victory pressure | TerritoryTimer | cap | 2 |
| Player gov. victory pressure | RaidTimer | cap | 4 |
| Player gov. alliance pressure | AttackTimer | cap | 4 |
| Player gov. alliance pressure | TerritoryTimer | cap | 3 |
| Player gov. active | AttackTimer | cap | 5 |
| Player covenant targets enemy | AttackTimer | cap | 4 |
| Player covenant active | AttackTimer | cap | 6 |
| Player divine right active | AttackTimer | cap | 5 |
| Player succession crisis high | AttackTimer | cap | 5 |
| Player succession crisis norm | AttackTimer | cap | 7 |
| Player succession crisis | MarriageProposalTimer | cap | 18 |
| Enemy succession crisis severe | AttackTimer | floor | 16 |
| Enemy succession crisis norm | AttackTimer | floor | 12 |
| Enemy succession crisis severe | TerritoryTimer | floor | 14 |
| Enemy succession crisis norm | TerritoryTimer | floor | 10 |
| Enemy covenant has target point | TerritoryTimer | cap | 5 |
| Enemy governance victory pressure | AttackTimer | floor | 14 |
| Enemy governance victory pressure | HolyWarTimer | floor | 24 |
| Enemy governance active | AttackTimer | floor | 12 |

## Current Readiness

Sub-slice 3 complete. Branch `claude/unity-ai-governance-pressure` ready to merge to master.

## Next Action

Sub-slices pending on this lane: siege staging, dynasty-aware covert ops (ai.js ~2681). The governance flags added this slice will be written by real dynasty/faith/governance systems as those ports land; for now they default to false and are safe to ship.
