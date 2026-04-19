# 2026-04-19 Unity Fortification Siege Wall Segment Destruction Resolution

## Scope

Prompt-accurate Unity ECS port of the browser-side fortification wall-segment
destruction resolution seam on
`codex/unity-fortification-wall-segment-destruction`.

This slice started from the prompt's expected `origin/master` head
`2b6df571` (contract revision 28), then rebased cleanly onto
`origin/master` `879f1647` on 2026-04-19 after AI strategic-layer
sub-slice 14 landed and moved the contract to revision 29, and finally
restacked onto `origin/master` `0a0e122f` after AI strategic-layer
sub-slice 15 landed and moved the contract to revision 30.

The port closes the missing runtime seam identified in the prior fortification
handoff: fortification-role buildings now become live contributors in normal
world state, destroyed wall segments and gatehouses resolve into explicit breach
state on the owning settlement, and the already-landed reserve frontage logic
consumes the post-destruction fortification tier on the same update chain.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/FortificationStructureResolutionSystem.cs`
  - new runtime fortification seam with two systems:
    - `FortificationStructureLinkSystem` links wall/tower/gate/keep buildings to
      the nearest same-faction settlement inside the fortification ecosystem
      radius and materializes `FortificationBuildingContributionComponent`
    - `FortificationDestructionResolutionSystem` counts destroyed linked walls,
      towers, gates, and keeps and writes those counts plus `OpenBreachCount`
      back onto the owning `FortificationComponent`
- `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs`
  - additive breach-state fields:
    `DestroyedWallSegmentCount`, `DestroyedTowerCount`, `DestroyedGateCount`,
    `DestroyedKeepCount`, and `OpenBreachCount`
- `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs`
  - update ordering tightened so reserve resolution runs after destruction
    accounting and therefore consumes breached-wall tier drops immediately
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs`
  - new `TryDebugGetFortificationBreachState` helper for wall, tower, gate,
    keep, and open-breach inspection without widening the base debug surface
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesWallSegmentDestructionSmokeValidation.cs`
  - dedicated 4-phase validator covering linked-wall baseline, destroyed-wall
    breach resolution, intact-wall reserve frontage, and breached-wall reserve
    frontage
  - artifact marker:
    `BLOODLINES_WALL_SEGMENT_DESTRUCTION_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnityWallSegmentDestructionSmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Browser References Ported

- `src/game/core/simulation.js:7694-7755`
  - structural building kill path inside `applyDamage`
- `src/game/core/simulation.js:11182-11213`
  - `nearestSettlementForBuilding`
- `src/game/core/simulation.js:11227-11245`
  - `advanceFortificationTier`
- `data/buildings.json`
  - fortification-role and tier-contribution metadata for `wall_segment`,
    `watch_tower`, `gatehouse`, and `keep_tier_1`

## Validation

The final rebased branch state is green on `D:\BLFWD\bloodlines`:

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
11. imminent engagement smoke
12. siege supply interdiction smoke
13. dedicated wall-segment destruction smoke

Dedicated wall-segment destruction smoke phases:

- Phase 1 PASS: linked wall contributes a clean tier-1 baseline with no breaches
- Phase 2 PASS: destroyed wall resolves to tier 0 with one open breach and one
  destroyed wall segment
- Phase 3 PASS: intact wall plus gate preserves tier 3 and commits 3 reserves
- Phase 4 PASS: breached wall drops frontage to tier 2 and commits only 2 reserves

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-imminent-engagement-smoke.log`
- `artifacts/unity-siege-supply-interdiction-smoke.log`
- `artifacts/unity-wall-segment-destruction-smoke.log`

Because the checked-in bootstrap-runtime and scene-shell validators are still
path-pinned to `D:\ProjectsHome\Bloodlines`, those gates were executed against
`D:\BLFWD\bloodlines` through worktree-local direct execute-method wrappers under
`scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`. No shared wrapper files were
committed for that temporary adaptation.

## Branch State

- Branch: `codex/unity-fortification-wall-segment-destruction`
- Rebased master base at close: `0a0e122f`
- Contract revision at handoff close: `31`
- Branch status: ready for push and merge coordination after revision-31 wall
  destruction delivery

## Next Action

1. Push `codex/unity-fortification-wall-segment-destruction`.
2. Merge it to `master` via the merge-temp ceremony.
3. For the next fortification follow-up, consume `OpenBreachCount` in a
   breach-aware assault, pathing, or legibility slice so the new breach state is
   visible beyond reserve frontage and tier loss.
