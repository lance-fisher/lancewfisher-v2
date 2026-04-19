# 2026-04-19 Unity Fortification Siege Breach Sealing Recovery

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-breach-sealing-recovery`.

This slice started from `origin/master` `0b2fd111` after Claude's
ai-strategic-layer Bundle 1 captive-state and missionary-execution merge moved
the concurrent-session contract to revision `37`. While the slice was in
flight, Claude's Bundle 2 holy-war and divine-right execution merge advanced
`origin/master` to `cec33509` and the contract to revision `38`, so this branch
rebased before close and lands at revision `39`.

Sub-slice 8 ports the first defender-side breach recovery loop. The browser has
no explicit sealing system. Open breaches remain open until other siege
resolution paths overwrite state. Unity now adds a forward-extension foundation:
defenders at a fortified settlement can spend stone plus idle worker time to
restore `FortificationComponent.OpenBreachCount` toward zero over in-world time.
Destroyed wall, tower, gate, and keep counters remain untouched in this slice.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Components/BreachSealingProgressComponent.cs`
  - new per-settlement sealing progress payload with
    `AccumulatedWorkerHours`, `StoneReservedForCurrentBreach`, and
    `LastTickInWorldDays`
  - attached lazily the first time a fortified settlement enters the sealing
    loop
- `unity/Assets/_Bloodlines/Code/Fortification/BreachSealingSystem.cs`
  - new runtime sealing loop in `SimulationSystemGroup`
  - ticks at `1 Hz`
  - reserves `60` stone per breach and spends `8` in-world worker-hours per
    breach before reducing `OpenBreachCount`
  - resolves worker availability by scanning live idle workers
    (`UnitRole.Worker`, positive health, `WorkerGatherPhase.Idle`) for the
    owning faction instead of widening `FortificationReserveComponent`
- `unity/Assets/_Bloodlines/Code/Fortification/FortificationStructureResolutionSystem.cs`
  - destruction accounting still refreshes destroyed wall, tower, gate, and
    keep counters
  - `OpenBreachCount` is no longer recomputed directly from destroyed walls plus
    destroyed gates every frame
  - the system now preserves previously sealed closures and only adds
    newly-created breaches back into the live open-breach total
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingRecoverySmokeValidation.cs`
  - dedicated 6-phase validator covering proportional progress, first-breach
    seal, insufficient-stone block, zero-idle-worker block, intact-settlement
    no-op, and full multi-breach sealing
  - artifact marker:
    `BLOODLINES_BREACH_SEALING_RECOVERY_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnityBreachSealingRecoverySmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Design Notes

- Worker availability uses a direct idle-worker scan, not a new cached field on
  `FortificationReserveComponent`. The existing AI worker surfaces are
  AI-lane-owned and faction-oriented, while sealing needs a narrow shared rule
  that works for both AI and player factions without broadening the reserve
  schema.
- Stone is reserved up front for the active breach being sealed. If a settlement
  starts sealing and then runs out of idle workers, the already-reserved stone
  remains committed to that breach until work resumes or the breach closes.
- `FortificationDestructionResolutionSystem` had to change for sealing to be
  real. Without that narrow edit, the next destruction-resolution tick would
  overwrite any sealed breach by re-deriving `OpenBreachCount` from destroyed
  walls plus gates.

## Browser And Canon References

- `src/game/core/simulation.js`
  - search terms: `openBreaches`, `breachSealing`, `sealBreach`
  - browser parity note: no explicit sealing loop currently ships in the
    browser runtime, so Unity's recovery path is an intentional forward
    extension
- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-assault-pressure.md`
  - sub-slice 6 owns the live assault attack and speed surface that now
    reverses automatically when `OpenBreachCount` falls
- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-legibility-readout.md`
  - sub-slice 7 exposed the settlement-side breach counts and aggregate breach
    state the new sealing loop now changes

## Validation

The slice is green on `D:\BLBSR\bloodlines`:

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
11. dedicated breach-sealing-recovery smoke

Dedicated breach-sealing-recovery smoke phases:

- Phase 1 PASS: half-window sealing progress accumulates to `4.00` worker-hours
  while `OpenBreachCount` stays `2`
- Phase 2 PASS: first breach seals cleanly, `OpenBreachCount` drops `2 -> 1`,
  reserved progress resets, and faction stone falls `200 -> 140`
- Phase 3 PASS: stone below `60` blocks sealing progress completely
- Phase 4 PASS: zero idle workers block sealing progress completely
- Phase 5 PASS: `OpenBreachCount = 0` leaves the settlement untouched with no
  `BreachSealingProgressComponent` attached
- Phase 6 PASS: three full sealing windows clear all breaches and consume
  `180` stone total

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-sealing-recovery-smoke.log`

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean worktree, those three gates were run directly against
  `D:\BLBSR\bloodlines\unity` using the same execute methods and success markers
  as the governed wrappers

Local csproj refresh note:

- this worktree started without trustworthy Unity-generated `.csproj` files
- before the governed `dotnet build` gates ran, the local
  `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` were
  refreshed so they included:
  - Claude Bundle 1 files already present on `master`
  - the new `BreachSealingProgressComponent.cs`
  - the new `BreachSealingSystem.cs`
  - the new `BloodlinesBreachSealingRecoverySmokeValidation.cs`
- both csproj files remain gitignored and are not part of the commit

## Branch State

- Branch: `codex/unity-fortification-breach-sealing-recovery`
- Master base at close: `cec33509`
- Contract revision at handoff close: `39`
- Branch status: ready for push and merge coordination after rebased
  revision-39 breach sealing / recovery delivery

## Next Action

1. Push `codex/unity-fortification-breach-sealing-recovery`.
2. Merge it to `master` via the merge-temp ceremony.
3. Follow with the next fortification follow-up on a fresh
   `codex/unity-fortification-*` branch:
   - destroyed-counter recovery if the owner wants walls or gates themselves to
     heal after sealing
   - sealing-cost or throughput balance if the owner wants the new recovery loop
     tuned before more consumers land
