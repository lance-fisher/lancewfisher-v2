---
lane: ai-strategic-layer
sub-slice: 2
date: 2026-04-18
session: claude-ai-strategic-pressure-2026-04-18
branch: claude/unity-ai-strategic-pressure
---

# AI Strategic Layer Sub-Slice 2: Strategic Pressure System

## Goal

Port the timer-clamp and stage-gate block from ai.js lines 1127-1241 (updateEnemyAi) into Unity ECS. This block governs how world pressure escalates AI aggression by clamping timers downward, and how stage progression gates rivalry/raid behaviors with hard timer floors.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIStrategicPressureSystem.cs`
  Ports the timer clamp/floor block. Updates `AIStrategyComponent` per AI faction entity each frame. Reads `MatchProgressionComponent` singleton for stage number and the player `WorldPressureComponent` for wpLevel and GreatReckoningScore.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIStrategicPressureSmokeValidation.cs`
  Four-phase smoke validator (static batch-mode entry point pattern). Phases:
  - Phase 1: stage 1, no world pressure. RivalryUnlocked=false, TerritoryTimer floored >=24.
  - Phase 2: stage 3, wpLevel 0. RivalryUnlocked=true, RaidPressureUnlocked=true, AttackTimer<20.
  - Phase 3: stage 3, wpLevel 2. AttackTimer<=8, TerritoryTimer<=6.
  - Phase 4: stage 3, wpLevel 2, GreatReckoningScore=4. AttackTimer<=6, TerritoryTimer<=4, RaidTimer<=8.

- `scripts/Invoke-BloodlinesUnityAIStrategicPressureSmokeValidation.ps1`
  Self-contained PowerShell runner. Calls RunBatchAIStrategicPressureSmokeValidation via -executeMethod. Artifact: `artifacts/unity-ai-strategic-pressure-smoke.log`. Marker: `BLOODLINES_AI_STRATEGIC_PRESSURE_SMOKE PASS/FAIL`.

### Modified Files

- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs`
  Added stage-gate bool fields: `RivalryUnlocked`, `RaidPressureUnlocked`.

- `unity/Assembly-CSharp.csproj`
  Added compile entry for `AIStrategicPressureSystem.cs`.

- `unity/Assembly-CSharp-Editor.csproj`
  Added compile entry for `BloodlinesAIStrategicPressureSmokeValidation.cs`.

- `docs/unity/CONCURRENT_SESSION_CONTRACT.md`
  Bumped to revision 14. Updated ai-strategic-layer lane owned paths and sub-slice 2 record. Retired victory-conditions and tier2-batch-dynasty-systems lanes.

## Verification Results

All 10 validation gates green:

1. `dotnet build unity/Assembly-CSharp.csproj` -- 0 errors
2. `dotnet build unity/Assembly-CSharp-Editor.csproj` -- 0 errors
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 (melee and projectile green)
5. Scene shells -- Bootstrap + Gameplay green
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision 14)

AI strategic pressure smoke: all 4 phases PASS.

## Constants Ported (ai.js lines 1127-1241)

| Constant | Value | Governs |
|---|---|---|
| TerritoryTimerRivalryFloor | 24 | min TerritoryTimer when rivalry locked |
| TerritoryTimerStage2Floor | 16 | min TerritoryTimer at stage 2 |
| RaidTimerNoPressureFloor | 20 | min RaidTimer when no world pressure |
| AttackTimerPressureLevel1Cap | 14 | max AttackTimer at wpLevel 1 |
| AttackTimerPressureLevel2Cap | 8 | max AttackTimer at wpLevel 2+ |
| TerritoryTimerPressureLevel1Cap | 10 | max TerritoryTimer at wpLevel 1 |
| TerritoryTimerPressureLevel2Cap | 6 | max TerritoryTimer at wpLevel 2+ |
| AttackTimerGreatReckoningCap | 6 | max AttackTimer under Great Reckoning |
| TerritoryTimerGreatReckoningCap | 4 | max TerritoryTimer under Great Reckoning |
| RaidTimerGreatReckoningCap | 8 | max RaidTimer under Great Reckoning |
| GreatReckoningPressureScore | 4 | GreatReckoningScore threshold |

## Current Readiness

Sub-slice 2 complete. Branch `claude/unity-ai-strategic-pressure` is ready to merge to master after this commit.

## Next Action

Sub-slice 3 (imminent-engagement warnings) is the next unblocked candidate on this lane. A Codex prompt for that slice was written during this session. Alternatively, the `fortification-siege` lane has sub-slice 3 (same topic) also queued. Either can proceed independently.

After merging this branch, the next claimer of this lane should read this handoff and the updated contract at revision 14.
