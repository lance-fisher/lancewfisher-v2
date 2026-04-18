# 2026-04-18 Unity Fortification Siege Siege Support And Field Water

## Goal

Second sub-slice of the `fortification-siege-imminent-engagement` Tier 1 lane.
This pass ports the browser siege-support logistics and field-water sustainment
loop into Unity 6.3 DOTS/ECS so besieging and fielded armies no longer behave
like infinite-supply camps during stage 3 and stage 4 pressure.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  adds per-unit siege-support state: siege-engine / engineer / wagon role flags,
  short-lived engineer and supply support windows, operational multipliers, and
  refresh observability.
- `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  adds per-unit field-water runtime state: support expiry, transfer timestamps,
  strain / critical timers, attrition and desertion flags, stored base combat and
  movement values, and live operational multipliers.
- `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
  adds supply-train linkage state for wagons, including linked-camp entity and
  last transfer timestamp.
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportCanon.cs`
  ports the canonical 1.25-second siege refresh cadence and keeps the existing
  authored engineer and wagon logistics values in one governed static canon
  surface.
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterCanon.cs`
  ports the canonical field-water constants block verbatim and keeps the current
  authored support-source values in the same governed canon surface.
- `unity/Assets/_Bloodlines/Code/Siege/SiegeComponentInitializationSystem.cs`
  additively attaches the new siege and field-water components at runtime so this
  slice stays out of shared bootstrap files.
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportRefreshSystem.cs`
  runs on the canonical 1.25-second cadence, links wagons to live supply camps,
  refreshes engineer support windows, repairs nearby siege engines, and spends
  faction stockpile food / water / wood only when support transfers actually fire.
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterSupportScanSystem.cs`
  evaluates per-unit support from allied control points, settlements, wells,
  supply camps, and linked supply wagons, applies the 14-second lingering support
  window, and charges 0.2 water on the canonical 4-second transfer interval.
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterStrainSystem.cs`
  applies per-delta strain accumulation and recovery, commander discipline
  buffering, critical attrition and desertion thresholds, and the canonical
  attack and speed multipliers by mutating runtime combat and movement stats from
  stored base values.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs`
  adds governed debug APIs for reading field-water state, siege-support state,
  and faction stockpiles.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs`
  adds the dedicated four-phase isolated ECS validator for baseline, strain,
  recovery, and siege-support cadence proof.
- `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  adds the lane-owned wrapper that writes `artifacts/unity-siege-smoke.log` and
  hard-fails if the dedicated pass marker is missing.

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
   - pass excerpt:
     `Build succeeded. 0 Warning(s), 0 Error(s).`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
   - pass excerpt:
     `Build succeeded. 111 Warning(s), 0 Error(s).`
3. `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
   - pass excerpt:
     `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier.`
4. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
   - pass excerpt:
     `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
5. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
   - pass excerpt:
     `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
   - pass excerpt:
     `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
6. `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
   - pass excerpt:
     `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
7. `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
   - pass excerpt:
     `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True.`
8. `node tests/data-validation.mjs`
   - pass excerpt:
     `Bloodlines data validation passed.`
9. `node tests/runtime-bridge.mjs`
   - pass excerpt:
     `Bloodlines runtime bridge validation passed.`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
   - pass excerpt:
     `STALENESS CHECK PASSED: Contract revision=10, last-updated=2026-04-18 is current. Latest handoff: 2026-04-18-unity-fortification-siege-siege-support-and-field-water.md (2026-04-18).`

## Behavioral Parity Targets

1. Confirmed: siege support refresh now runs on a 1.25-second cadence in `SiegeSupportRefreshSystem`, not per frame.
2. Confirmed: field-water support resolves from allied mobile water support at 132 units and allied settlements at 156 units, with lingering support expiry tracked in `FieldWaterComponent.SuppliedUntil`.
3. Confirmed: strain accumulates at `0.85/s` outside support, recovers at `1.25/s` inside support, and clamps at zero.
4. Confirmed: strain and critical thresholds apply the canonical attack and speed multipliers, and critical recovery decays at `2.1/s`.
5. Confirmed: critical strain starts attrition after 4 seconds, desertion risk after 10 seconds, and commander aura extends both thresholds by 4 seconds while reducing attrition damage by the canonical `0.6x`.
6. Confirmed: water transfer from support sources consumes `0.2` water every 4 seconds only when stock exists and only when the unit remains inside an active support source.

## Current Readiness

The lane is green for sub-slice 2 on `codex/unity-fortification-siege`. Siege
support logistics and field-water strain are now isolated-ECS validated and all
shared runtime gates remain preserved green. Bootstrap-scene integration,
imminent-engagement warnings, siege lifecycle progression, and HUD strain
readouts remain intentionally deferred.

## Next Action

Proceed to fortification lane sub-slice 3 on the same branch:

1. port imminent-engagement warnings only
2. keep bootstrap runtime smoke integration out of scope until that later slice
3. continue to avoid siege-operation lifecycle work and HUD work
4. preserve isolated ECS validation for new siege surfaces until bootstrap
   integration is explicitly claimed

## Browser Reference

- `src/game/core/simulation.js:213-241` - field-water constants block
- `src/game/core/simulation.js:2824` - `refreshFieldWaterSupport(...)`
- `src/game/core/simulation.js:12437` - `tickSiegeSupportLogistics(...)`
- `src/game/core/simulation.js:12565` - `tickFieldWaterLogistics(...)`

## Canon Reference

- `04_SYSTEMS/TERRITORY_SYSTEM.md` - siege logistics and field-water sections
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` - siege camp, supply-train, and defensive sustainment behavior
