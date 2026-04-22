# 2026-04-21 Unity Player HUD Realm-Condition Legibility

## Goal

Land the first player-HUD slice for Priority 5 by creating a per-faction realm-condition HUD read-model, a parseable debug seam, and a dedicated smoke validator that proves the player can read cycle, population, food, water, loyalty, conviction, and faith state from Unity ECS without touching Claude's AI-owned lane.

## Browser Reference

- `src/game/core/simulation.js`
  - `getRealmConditionSnapshot` lines `14291-14590`
    - cycle block
    - population / food / water / loyalty legibility bands
    - conviction band label and color semantics
    - faith covenant + intensity band semantics
- `tests/runtime-bridge.mjs`
  - lines `1344-1364`
    - canonical snapshot block assertions for `cycle`, `population`, `food`, `water`, `loyalty`, `faith`, and `conviction`
- Canon / UI doctrine
  - `12_UI_UX/UI_NOTES.md` lines `95-113`
  - `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` lines `378-392`
  - `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` lines `460-468`

## Work Completed

- Added the lane-owned runtime read-model under `unity/Assets/_Bloodlines/Code/HUD/`:
  - `RealmConditionHUDComponent.cs`
  - `RealmConditionHUDSystem.cs`
- `RealmConditionHUDSystem` now:
  - attaches `RealmConditionHUDComponent` to eligible faction-root entities
  - projects cycle progress, population pressure, food/water ratios and strain streaks, loyalty band, conviction band/label/color, and faith covenant / doctrine / tier / band from the existing simulation components
  - stays additive and read-only relative to the source simulation components
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs` with:
  - `TryDebugGetRealmConditionHUDSnapshot(string factionId, out string readout)`
  - structured output shaped as a single parseable `RealmHUD|Key=Value|...` string
- Added dedicated validation surfaces:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityRealmConditionHUDSmokeValidation.ps1`
- The dedicated HUD smoke now proves four phases:
  1. stable baseline
  2. realm strain
  3. conviction shift
  4. faith commitment
- Narrow shared-file edits applied:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Dedicated HUD smoke:
  - `BLOODLINES_REALM_CONDITION_HUD_SMOKE PASS`
  - `Phase 1 PASS: stable baseline surfaces green realm bands, neutral conviction, red uncommitted faith, CycleProgress=0.500.`
  - `Phase 2 PASS: cap pressure, food strain, water strain, and loyalty distress all surface red with visible strain streaks.`
  - `Phase 3 PASS: ConvictionBand=ApexMoral, ConvictionLabel=Apex Moral, ConvictionScore=80.0.`
  - `Phase 4 PASS: FaithId=OldLight, DoctrinePath=Light, FaithLevel=4, FaithTier=Fervent, FaithBand=green.`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat smoke:
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
- Siege smoke:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. Strain: status=Strained, strain=6.25, attackMultiplier=0.88, speedMultiplier=0.90. Recovery: supported=True, strain=3.88, supportRefreshCount=51. Support: engineerSupport=True, supplySupport=True, refreshCount=3, suppliedUntil=11.20.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`

## Unity-Side Simplifications Deferred

- This slice creates the HUD read-model and proof seam only; it does not yet render a new on-screen player HUD panel.
- Loyalty currently surfaces `FactionLoyaltyComponent.Current` rather than the browser snapshot's territory-average / territory-min loyalty pair.
- The broader browser snapshot blocks remain deferred to later slices in this lane:
  - match progression
  - fortification legibility
  - world pressure
  - victory readout
  - logistics readout

## Exact Next Action

After landing this slice on `master`, start the next HUD slice on a fresh Codex branch by implementing `MatchProgressionHUDComponent`, `MatchProgressionHUDSystem`, and `TryDebugGetMatchHUDSnapshot` under the same `player-hud-realm-condition-legibility` lane.
