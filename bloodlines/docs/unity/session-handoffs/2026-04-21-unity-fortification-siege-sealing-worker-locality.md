# 2026-04-21 Unity Fortification Siege Sealing Worker Locality

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-sealing-worker-locality`.

This slice started from `origin/master` `dad7952e` at contract revision `47`
after sub-slice 11 sealing-cost tier scaling had landed. The goal was to stop
breach sealing from poaching idle workers across same-faction settlements by
gating labor to the settlement's own control-point footprint.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/BreachSealingSystem.cs`
  - breach sealing now resolves each settlement's nearest same-owner
    `ControlPointComponent` from the settlement anchor position
  - the idle-worker scan now requires `PositionComponent`, resolves each
    worker's nearest control point, and only counts the worker when that
    nearest control point is owned by the worker's faction
  - settlement sealing labor is now drawn only from workers whose nearest
    control point matches the settlement's own control-point anchor
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingWorkerLocalitySmokeValidation.cs`
  - new dedicated validator with four phases:
    local worker seals, other-settlement worker does not seal, no workers
    blocks, and non-idle workers block even when present
- `scripts/Invoke-BloodlinesUnityBreachSealingWorkerLocalitySmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Design Notes

- The runtime still does not introduce a stored settlement-to-control-point
  foreign key. It uses the same position-based world model already used by
  the bootstrap and fortification link systems:
  settlement home control point = nearest same-owner control point to the
  settlement anchor.
- Worker locality is intentionally stricter than the old faction-wide labor
  bucket. A worker near another player-owned settlement no longer advances a
  breach at this settlement even when both settlements share the same faction.
- The worker scan ignores idle workers whose nearest control point is neutral
  or enemy-owned. That preserves the prompt's owner-match requirement on the
  worker's nearest `ControlPointComponent`.
- The first dedicated smoke rerun after script compilation exited immediately
  with return code `1` before the validator executed. After a single 10-second
  retry on a clear runway, the validator reached the batch method and passed
  cleanly.

## Validation

The slice is green on `D:\BLF12\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke via a worktree-safe wrapper copy
4. combat smoke
5. bootstrap scene-shell validation via a worktree-safe wrapper copy
6. gameplay scene-shell validation via a worktree-safe wrapper copy
7. fortification smoke
8. siege smoke
9. `node tests/data-validation.mjs`
10. `node tests/runtime-bridge.mjs`
11. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
12. breach sealing worker locality smoke

Dedicated worker-locality smoke phases:

- Phase 1 PASS: local worker fully seals the breached settlement and leaves
  the Tier 2 cost-correct stockpile at `30`
- Phase 2 PASS: worker nearest another same-faction settlement's control point
  does not contribute to this settlement's breach closure
- Phase 3 PASS: no workers blocks sealing
- Phase 4 PASS: non-idle workers block sealing even when positioned locally

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-sealing-worker-locality-smoke.log`

## Current Readiness

This slice is complete and ready for push, merge-temp coordination, and
continuation onto sub-slice 13 fortification repair narrative.

## Next Action

1. Push `codex/unity-fortification-sealing-worker-locality`.
2. Merge it to `master` via the normal merge-temp ceremony.
3. After the merge lands, branch from the refreshed `origin/master` and start
   sub-slice 13 on `codex/unity-fortification-repair-narrative`.
