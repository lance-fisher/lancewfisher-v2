# 2026-04-20 Unity Scout Raids And Logistics Interdiction

## Scope

Prompt-accurate continuation on the new Codex-owned scout raid and logistics
interdiction lane at `codex/unity-scout-raids-logistics-interdiction`.

This slice starts from the post-dynasty-parity branch state at `7cc7bed6`. The
goal was to port the browser's non-AI execution seams for scout building raids
and supply-wagon interdiction without widening the AI strategic layer, then
wire the affected runtime consumers so active raids actually suppress resource
trickle, field-water support, and worker drop-off behavior.

## Browser References

- `src/game/core/simulation.js`
  - `SCOUT_RAID_TARGET_RANGE` (35)
  - `SCOUT_RAID_RETREAT_DISTANCE` (36)
  - `SCOUT_RAID_LOYALTY_RADIUS` (37)
  - `isBuildingUnderScoutRaid` (2046)
  - `getRaidRetreatCommand` (2349)
  - `executeScoutRaid` (2362)
  - `executeScoutLogisticsInterdiction` (2515)
- `src/game/core/ai.js`
  - read-only confirmation only; this slice does not widen AI dispatch

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Raids/ScoutRaidComponents.cs`
  - adds `ScoutRaidTargetKind`
  - adds `ScoutRaidCommandComponent`
  - adds `BuildingRaidStateComponent`
- `unity/Assets/_Bloodlines/Code/Raids/ScoutRaidCanon.cs`
  - ports raid constants and code-native canon profiles for:
    - `scout_rider`
    - raidable buildings (`farm`, `well`, `lumber_camp`, `mine_works`,
      `quarry`, `iron_mine`, `supply_camp`)
    - `supply_wagon`
  - adds helpers for raid-state checks, drop-off eligibility, resource loss,
    and convoy recovery timing
- `unity/Assets/_Bloodlines/Code/Raids/ScoutRaidResolutionSystem.cs`
  - ports explicit building raid resolution:
    - hostile-target validation
    - raid range gate
    - stockpile loss
    - nearby control-point loyalty shock
    - retreat command
  - ports explicit supply-wagon interdiction:
    - `LogisticsInterdictedUntil`
    - `ConvoyRecoveryUntil`
    - support status flip to `Interdicted`
    - wagon retreat toward the nearest allied operational, non-raided supply
      camp
    - convoy stockpile loss and local loyalty shock
- `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingSystem.cs`
  - active `BuildingRaidStateComponent` now suppresses passive trickle from
    raided structures
- `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs`
  - returning workers now choose the nearest alive, completed, faction-owned
    drop-off that both:
    - accepts the carried resource
    - is not under active scout raid
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterSupportScanSystem.cs`
  - raided wells and supply camps no longer count as active field-water support
    sources until raid expiry
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportRefreshSystem.cs`
  - raided supply camps no longer count as operational logistics anchors for
    wagon linkage
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
  - newly spawned raidable buildings now receive `BuildingRaidStateComponent`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  - debug-placed construction buildings now also receive
    `BuildingRaidStateComponent`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesScoutRaidAndInterdictionSmokeValidation.cs`
  - adds a dedicated 4-phase validator
  - the smoke harness now adds `EndSimulationEntityCommandBufferSystem` to the
    local simulation group so raid-order removal actually plays back during the
    test world
- `scripts/Invoke-BloodlinesUnityScoutRaidAndInterdictionSmokeValidation.ps1`
  - batch wrapper with pass marker
    `BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE PASS`

## Design Notes

- This slice is runtime-side execution only. It does not touch the
  `ai-strategic-layer` lane under `unity/Assets/_Bloodlines/Code/AI/**`.
- Raid state is modeled as a reusable building-side component instead of a
  one-off boolean so downstream systems can read raid expiry, last raid time,
  and raider faction consistently.
- The worker reroute widening deliberately keeps the drop-off search narrow:
  resource-specific acceptance is still hardcoded to the buildings the current
  Unity economy loop understands.
- The dedicated smoke initially failed for a valid reason in the harness rather
  than the runtime: the well-support phase seeded too little post-raid water and
  the local test world was not replaying the end-simulation ECB. Both issues are
  now fixed in the validator.

## Validation

The slice is green on `D:\BLAICD\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke via worktree-local wrapper under the Unity lock
4. combat smoke
5. canonical scene shells via worktree-local wrappers under the Unity lock
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. scout raid and interdiction smoke

Scout raid and interdiction smoke phases:

- Phase 1 PASS: farm raid drains food and influence, preserves stabilized
  loyalty at the threshold floor, suppresses same-frame trickle, and issues a
  retreat move
- Phase 2 PASS: a raided well stops field-water refresh until expiry, then
  support resumes and consumes water from the surviving stockpile
- Phase 3 PASS: a returning worker reroutes away from a raided `lumber_camp`
  toward a valid clear `command_hall`
- Phase 4 PASS: supply-wagon interdiction applies stockpile loss, interdiction
  and recovery timers, wagon retreat routing, and raider retreat

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-scout-raid-and-interdiction-smoke.log`

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean worktree, those gates were run through temporary worktree-safe
  wrapper copies that preserved the same pass markers while targeting
  `D:\BLAICD\bloodlines\unity`

## Current Readiness

This slice is complete and green. The branch is ready for continuity updates,
push, and merge coordination.

## Next Action

1. Push `codex/unity-scout-raids-logistics-interdiction`.
2. Merge to `master` if the rebased tree is still clean.
3. If Lance wants another non-AI runtime slice next, covert-operations
   hardening is now the cleanest remaining high-leverage target after the
   already-landed fortification and dynasty parity work.
