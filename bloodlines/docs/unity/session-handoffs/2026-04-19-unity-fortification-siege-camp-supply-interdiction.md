# 2026-04-19 Unity Fortification Siege Camp Supply Interdiction

## Scope

Prompt-accurate Unity ECS port of the browser-side siege camp supply
interdiction seam on `codex/unity-fortification-siege-camp-supply-interdiction`,
started from `origin/master`
`c16d5450f1dc2887727db41575e9f9f824f63793` (revision 26).

This slice chose the siege camp supply interdiction candidate because it is
bounded to Codex-owned fortification and siege surfaces while still changing
real siege behavior immediately: hostile raiders can now degrade camp
stockpiles, interdict wagons, force convoy recovery windows, and push AI siege
orchestration into the already-ported `SupplyInterdicted`,
`SupplyRecoveringUnscreened`, and `SupplyRecoveringScreened` phases.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupplyCampComponent.cs`
  - new camp-state component for live siege logistics stockpile, raider count,
    interdiction timestamps, and attacker tracking
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupplyInterdictionCanon.cs`
  - browser-derived constants and role filters for raiders, escorts, camp
    stockpile drain and recovery, convoy screen thresholds, and one-shot wagon
    resource loss
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupplyInterdictionSystem.cs`
  - new runtime system that:
    - scans live raider and escort coverage
    - drains siege camp stockpiles while contested
    - marks wagons interdicted or recovering
    - records escort-screen state and convoy reconsolidation
    - applies direct stockpile loss on wagon hits
    - feeds runtime counts into `AISiegeOrchestrationComponent` without editing
      Claude-owned AI systems
- `unity/Assets/_Bloodlines/Code/Siege/SiegeComponentInitializationSystem.cs`
  - now seeds `SiegeSupplyCampComponent` on logistics-support buildings and
    initializes the new supply-train state fields
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportRefreshSystem.cs`
  - treats only operational camps as valid wagon anchors and preserves
    `Interdicted`, `RecoveringUnscreened`, and `RecoveringScreened` wagon
    states during the supply refresh cadence
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterSupportScanSystem.cs`
  - supply camps and wagons now stop contributing field-water support while
    interdicted, recovering, or linked to a non-operational camp
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterStrainSystem.cs`
  - preserves interdiction and recovery wagon statuses instead of collapsing
    them back to `CutOff`
- `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
  - additive runtime fields for logistics interdiction window, convoy recovery
    window, reconsolidation timestamp, escort counts, and attacker tracking
- `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  - additive `SiegeSupportStatus` states:
    `Interdicted`, `RecoveringUnscreened`, `RecoveringScreened`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs`
  - new siege debug helpers for wagon and camp logistics state snapshots
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSupplyInterdictionSmokeValidation.cs`
  - dedicated 5-phase validator covering baseline readiness, live interdiction,
    unscreened recovery, screened recovery, and camp stockpile restoration
  - artifact marker:
    `BLOODLINES_SIEGE_SUPPLY_INTERDICTION_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnitySiegeSupplyInterdictionSmokeValidation.ps1`
  - dedicated batch wrapper for the new interdiction validator

## Browser References Ported

- `src/game/core/simulation.js:218-220`
  - convoy recovery duration, escort screen radius, and minimum escort count
- `src/game/core/simulation.js:1999-2018`
  - supply wagon interdicted and recovering state helpers
- `src/game/core/simulation.js:2492-2579`
  - supply wagon interdiction impact profile and scout logistics interdiction
- `src/game/core/simulation.js:2613-2657`
  - faction siege-state logistics snapshot fields consumed by the AI layer
- `src/game/core/simulation.js:12420-12523`
  - wagon interdiction and convoy recovery ticking

## Validation

The required serial gate chain is green on `D:\BLFS\bloodlines`:

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
11. dedicated siege supply interdiction smoke

Dedicated interdiction smoke phases:

- Phase 1 PASS: baseline camp stays at full stockpile and AI remains
  `Assaulting`
- Phase 2 PASS: hostile raider presence drains camp stockpile, interdicts the
  wagon, and pushes AI to `SupplyInterdicted`
- Phase 3 PASS: recovering convoy without escorts holds
  `SupplyRecoveringUnscreened`
- Phase 4 PASS: screened recovery records convoy reconsolidation and advances to
  `SupplyRecoveringScreened`
- Phase 5 PASS: drained camp stockpile recovers above operational threshold and
  clears attacker ownership

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-batch-rev27-codex.log`
- `artifacts/unity-gameplay-scene-batch-rev27-codex.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-siege-supply-interdiction-smoke.log`

Because the checked-in bootstrap-runtime and scene-shell wrappers are still
path-pinned to `D:\ProjectsHome\Bloodlines`, those gates were executed against
`D:\BLFS\bloodlines` through worktree-local direct execute-method validation
under the Unity wrapper lock. No shared wrapper files were committed for that
temporary adaptation.

## Branch State

- Branch: `codex/unity-fortification-siege-camp-supply-interdiction`
- Contract revision at handoff close: `27`
- Branch status: ready for push and merge coordination after revision-27
  supply-line interdiction delivery

## Next Action

1. Push `codex/unity-fortification-siege-camp-supply-interdiction`.
2. Merge it to `master` and push `master`.
3. For the next fortification lane slice, prefer wall-segment destruction
   resolution. It has the highest direct gameplay payoff left on this lane
   because it ties siege engine pressure to fortification tier drops and to the
   already-landed reserve-duty and morale systems.
