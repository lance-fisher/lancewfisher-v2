# 2026-04-20 Unity Fortification Siege Destroyed Counter Recovery

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-destroyed-counter-recovery`.

This slice started from `origin/master` `7821d74a` after Codex's sub-slice 8
breach sealing / recovery landed at contract revision `39`. While the slice was
in flight, Claude's ai-strategic-layer Bundle 3 captive rescue + captive ransom
merge advanced `origin/master` to `e73933e4` and contract revision `40`, so
this branch fast-forwarded onto that head before close and lands at revision
`41`.

Sub-slice 9 extends the sub-slice 8 sealing pattern into slow physical rebuild.
The browser still treats destroyed fortification segments as permanent for the
rest of a match. Unity now adds a forward-extension foundation: once
`FortificationComponent.OpenBreachCount` has been driven to zero by the sealing
system, defenders can spend stone plus idle worker-hours over in-world time to
restore destroyed wall, tower, gate, and keep counters toward zero.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Components/DestroyedCounterRecoveryProgressComponent.cs`
  - new per-fortification progress payload with
    `AccumulatedWorkerHours`, `StoneReservedForCurrentSegment`,
    `LastTickInWorldDays`, and `TargetCounter`
  - new `DestroyedCounterKind` enum with `None`, `Wall`, `Tower`, `Gate`, and
    `Keep`
  - attached lazily only after breaches are sealed and at least one destroyed
    counter remains
- `unity/Assets/_Bloodlines/Code/Fortification/DestroyedCounterRecoverySystem.cs`
  - new runtime rebuild loop in `SimulationSystemGroup`
  - `[UpdateAfter(typeof(BreachSealingSystem))]` so breach sealing resolves
    first
  - gates on `OpenBreachCount == 0` and total destroyed counters greater than
    zero
  - scans the owning faction's live idle workers
    (`UnitRole.Worker`, positive health, `WorkerGatherPhase.Idle`) and the
    faction stockpile for rebuild capacity
  - rebuild priority is strict `Keep > Gate > Wall > Tower`
  - standard rebuild cost is `90` stone plus `14` worker-hours at `1 Hz`
  - keep rebuilds apply a `2x` cost and `2x` worker-hour multiplier
  - on completion decrements the selected destroyed counter and restores the
    linked destroyed structure's `HealthComponent.Current` to max so
    `FortificationDestructionResolutionSystem` remains authoritative on the next
    tick
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDestroyedCounterRecoverySmokeValidation.cs`
  - dedicated 6-phase validator covering progress accumulation, first wall
    rebuild completion, keep-over-gate priority, open-breach block, all-zero
    no-op, and full keep->gate->wall->tower rebuild order
  - artifact marker:
    `BLOODLINES_DESTROYED_COUNTER_RECOVERY_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnityDestroyedCounterRecoverySmokeValidation.ps1`
  - dedicated wrapper for the new validator

## Design Notes

- The ordering is deliberate. Sub-slice 8 handles immediate breach closure on
  `OpenBreachCount`; sub-slice 9 only starts once that live breach count
  reaches zero. A wall or gate segment is not rebuilt while the fortification
  still reports an open breach in that class.
- Worker availability reuses the narrow live idle-worker scan from sub-slice 8
  rather than widening `FortificationReserveComponent` or borrowing AI-owned
  worker caches.
- Rebuild progress holds reserved stone on the current target segment until the
  segment completes or the fortification falls out of the eligible state.
- The highest-priority destroyed counter is re-evaluated each tick. Keeps are
  first because the dynasty seat loss is existential, then gates as the
  controlled-access chokepoint, then walls, then towers.
- The runtime system heals the linked destroyed structure back to full health
  when a rebuild completes. That keeps the existing fortification destruction
  resolution loop authoritative instead of trying to shadow those counters in a
  second bookkeeping surface.

## Browser And Canon References

- `src/game/core/simulation.js`
  - fortification destruction is currently permanent in the browser runtime for
    the rest of a match
  - Unity destroyed-counter recovery is an intentional forward extension, not a
    browser parity port
- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-wall-segment-destruction-resolution.md`
  - sub-slice 5 owns the destroyed-counter increment side that this slice now
    reverses over time
- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-sealing-recovery.md`
  - sub-slice 8 owns the live `OpenBreachCount` closure that now gates this
    rebuild loop

## Validation

The slice is green on `D:\BLDCR\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene shells: Bootstrap + Gameplay
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. dedicated destroyed-counter-recovery smoke

Dedicated destroyed-counter-recovery smoke phases:

- Phase 1 PASS: half-window progress accumulates on a sealed fortification,
  `TargetCounter=Wall`, and `DestroyedWallCount` stays `2`
- Phase 2 PASS: first wall rebuild completes, `DestroyedWallCount` drops
  `2 -> 1`, progress resets, and stone falls by `90`
- Phase 3 PASS: keep beats gate on the first tick and uses `180` stone plus
  `28` worker-hours
- Phase 4 PASS: `OpenBreachCount = 1` blocks all destroyed-counter recovery,
  leaves `TargetCounter=None`, and spends no stone
- Phase 5 PASS: all destroyed counters at zero remain a no-op with no progress
  component attached
- Phase 6 PASS: keep, gate, wall, and tower all rebuild in strict priority
  order and total stone spent is `450`

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-destroyed-counter-recovery-smoke.log`

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean merge worktree, those gates were run through temporary
  worktree-safe wrapper copies that preserved the same execute methods and pass
  markers while targeting `D:\BLDCR\bloodlines\unity`

Local csproj refresh note:

- this clean worktree needed local Unity-generated `.csproj` refresh before the
  governed `dotnet build` gates
- before running the build gates, the local gitignored
  `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`
  were verified to include all already-landed Claude Bundle 3 files plus:
  - `DestroyedCounterRecoveryProgressComponent.cs`
  - `DestroyedCounterRecoverySystem.cs`
  - `BloodlinesDestroyedCounterRecoverySmokeValidation.cs`
- both csproj files remain gitignored and are not part of the commit

## Branch State

- Branch: `codex/unity-fortification-destroyed-counter-recovery`
- Master base at close: `e73933e4`
- Contract revision at handoff close: `41`
- Branch status: ready for push and merge-temp coordination after the revision
  `41` destroyed-counter recovery delivery

## Next Action

1. Push `codex/unity-fortification-destroyed-counter-recovery`.
2. Merge it to `master` via the merge-temp ceremony.
3. Stop this lane here. The next fortification follow-up should be either the
   sealing-cost balance pass or breach-depth telemetry on a fresh
   `codex/unity-fortification-*` branch.
