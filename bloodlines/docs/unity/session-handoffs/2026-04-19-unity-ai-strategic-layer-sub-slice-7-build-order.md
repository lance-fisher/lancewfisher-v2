# Unity AI Strategic Layer Sub-Slice 7: Build Order Priority Chain

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-build-timer-chain`
**Lane:** ai-strategic-layer
**Contract Revision:** 19

---

## Goal

Port the AI building build-order priority evaluation and per-building timer chain
from `src/game/core/ai.js` `updateEnemyAi` buildTimer block (~lines 1377-1573) to
Unity DOTS/ECS. This is the 13-step if-else chain that drives when the AI queues
construction: barracks, wayshrine, quarry, iron mine, siege workshop, covenant hall,
grand sanctuary, apex covenant, supply camp, stable, dwelling, farm, well. Timer
resets to 4s when the player keep is fortified (aggressive pressure) or 6s otherwise.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIBuildOrderComponent.cs`
  Context struct holding every gate condition needed by the 13-step priority chain:
  worker availability (`HasBuilder`), building existence flags (barracks, wayshrine,
  quarry, iron mine, siege workshop, covenant hall, grand sanctuary, apex covenant,
  supply camp, stable), completion flags required for dependent branches
  (`WayshrineCompleted`, `SiegeWorkshopCompleted`, `CovenantHallCompleted`,
  `GrandSanctuaryCompleted`, `SupplyCampCompleted`), affordability flags precomputed
  by a future context provider (`CanAffordBarracks` etc.), faith context
  (`EnemyHasFaith`, `CovenantTestPassed`, `FaithIntensity`, `ActiveCovenantTest`,
  `PlayerCovenantActive`), military support (`HasEngineerCorps`, `HasSupplyWagon`),
  dwelling/farm/well counts (`HouseCount`, `FarmCount`, `WellCount`), population and
  stockpile data (`PopulationCapAvailable`, `PopulationTotal`, `FoodStock`,
  `WaterStock`), and the dispatch result `NextBuildOp`. BuildOrderKind 14-value enum
  (None, Barracks, Wayshrine, Quarry, IronMine, SiegeWorkshop, CovenantHall,
  GrandSanctuary, ApexCovenant, SupplyCamp, Stable, Dwelling, Farm, Well).

- `unity/Assets/_Bloodlines/Code/AI/AIBuildOrderSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateAfter(typeof(AICovertOpsSystem))]`. Per entity per frame:
  1. Read `AIStrategyComponent.BuildTimer` (decremented by
     `AIStrategicPressureSystem`). If `> 0`, clear `NextBuildOp = None` and skip.
  2. If `HasBuilder` is false, write `None`. Otherwise call `EvaluateBuildPriority`
     which walks the 13-step if-else chain in exact ai.js order and returns the
     first match.
  3. Reset `BuildTimer` to 4s when `PlayerKeepFortified` is true or 6s otherwise,
     matching the browser reset values at ai.js line 1573.
  Faith intensity thresholds: 26 (covenant hall), 48 (grand sanctuary), 80 (apex
  covenant). Grand sanctuary urgency: intensity >= 48 OR active covenant test OR
  player covenant active OR player divine right active.
  Actual `attemptPlaceBuilding` is deferred to a later integration pass; this slice
  ports scheduling and decision logic only.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIBuildOrderSmokeValidation.cs`
  Five-phase smoke validator. Each phase creates an isolated World, seeds
  `MatchProgressionComponent`, a player faction, and an AI faction with
  `AIBuildOrderComponent` plus `AIStrategyComponent.BuildTimer = -1f` so the timer
  is already expired at first `world.Update()` (test worlds run with
  DeltaTime=0 for the first update; `AIStrategicPressureSystem` applies a 0.016f
  floor that takes `-1f` to `-1.016f`, still `<= 0`).
  - Phase 1: `HasBuilder + !HasBarracks + CanAffordBarracks` -> Barracks
  - Phase 2: `HasBarracks + EnemyHasFaith + !HasWayshrine + CanAffordWayshrine`
    -> Wayshrine
  - Phase 3: `playerKeepFortified + HasBarracks + HasQuarry + HasIronMine +
    !HasSiegeWorkshop + CanAffordSiegeWorkshop` -> SiegeWorkshop
  - Phase 4: `PopulationCapAvailable <= 1 + HouseCount < 4` -> Dwelling
  - Phase 5: `FoodStock < PopulationTotal + 4 + FarmCount < 3` -> Farm
  Artifact: `../artifacts/unity-ai-build-order-smoke.log`
  Marker: `BLOODLINES_AI_BUILD_ORDER_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIBuildOrderSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIBuildOrderSmokeValidation.RunBatchAIBuildOrderSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added `<Compile Include="..." />` entries for
  `AIBuildOrderComponent.cs` and `AIBuildOrderSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added `<Compile Include="..." />` entry
  for `BloodlinesAIBuildOrderSmokeValidation.cs`.

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors (112 unrelated CS0649 warnings) PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision=19 current PASS

AI build order smoke: all 5 phases PASS
- Phase 1 PASS: NextBuildOp=Barracks (no barracks -> barracks)
- Phase 2 PASS: NextBuildOp=Wayshrine (faith+barracks -> wayshrine)
- Phase 3 PASS: NextBuildOp=SiegeWorkshop (barracks+quarry+iron+fortified -> siege workshop)
- Phase 4 PASS: NextBuildOp=Dwelling (pop cap full -> dwelling)
- Phase 5 PASS: NextBuildOp=Farm (low food -> farm)

---

## Key Design Notes

**Timer-fire pattern in test worlds.** Same pattern as the covert-ops slice.
Unity ECS test worlds run `world.Update()` with DeltaTime=0. Timers seeded at
`0.001f` never expire because `0.001f - 0f = 0.001f > 0`. The canonical fix for
AI timer smokes is seeding firing timers at `-1f` so they are already expired at
first update. `AIStrategicPressureSystem` applies a `math.max(dt, 0.016f)` floor
which takes `-1f` to `-1.016f` on the first decrement, still `<= 0`, so
`AIBuildOrderSystem` sees an expired timer and fires.

**Priority chain preserves ai.js order exactly.** The 13 branches are tested in
the order browser `updateEnemyAi` tests them. Order matters: branch 2 (wayshrine)
gates on `HasBarracks` because the browser chain does. Branch 5 (siege workshop)
requires both quarry and iron mine present because both are prerequisites in the
browser. Branch 10 (stable) requires engineer corps and supply wagon production
capability because the browser reads `enemyArmy.some(u => u.type === 'engineer')`
and `enemyArmy.some(u => u.type === 'supplyWagon')` at that branch.

**Timer reset mirrors browser.** `BuildTimer = playerKeepFortified ? 4f : 6f`
matches ai.js line 1573 exactly. Fortified-keep pressure shortens the build
review interval because the AI needs to react faster when the player is
entrenched.

**Deferred execution.** `AIBuildOrderSystem` writes `NextBuildOp` and resets
`BuildTimer`. It does not yet call an equivalent of `attemptPlaceBuilding`; the
economy/placement side of the port will consume `NextBuildOp` in a future slice.
This matches how sub-slice 6 also defers `startAssassinationOperation` etc.

**`AIBuildOrderComponent` is additive-only on the AI lane.** The file sits
alongside existing AI components in `unity/Assets/_Bloodlines/Code/AI/`. No
shared files were modified beyond csproj registration. No bootstrap probe edits
were needed because the build order is verified by its own dedicated smoke.

---

## Current Readiness

Branch `claude/unity-ai-build-timer-chain` is ready to merge. All gates green,
contract at revision 19, continuity files updated. Merge will advance
`origin/master` past `c4dd5875` (sub-slice 6), which unblocks Codex's
`codex/unity-ai-command-dispatch` rebase onto revision 20.

---

## Next Action

1. Merge `claude/unity-ai-build-timer-chain` into master.
2. Notify Codex that master is now at revision 19 so
   `codex/unity-ai-command-dispatch` (currently at `80569a9f`, revision 19 on
   its own branch) can rebase and bump to revision 20.
3. Fetch and merge `codex/unity-fortification-siege` (fortification-siege
   sub-slice 3 imminent-engagement warnings). That branch is based on an older
   revision; contract conflict resolution is required during the merge.
4. Claim sub-slice 8 (marriage-proposal execution wiring) on a new branch, or
   move to other unclaimed Tier 1 lanes. Browser reference: `src/game/core/ai.js`
   marriage proposal execution (~lines 2580-2620) plus simulation-side
   `proposeMarriage` (~7340) and `acceptMarriage` (~7388).
