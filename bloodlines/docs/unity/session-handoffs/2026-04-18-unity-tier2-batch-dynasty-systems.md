# Unity Slice Handoff: Tier 2 Batch Dynasty Systems

- Date: 2026-04-18
- Session: 130
- Branch: codex/unity-fortification-siege
- Lane: tier2-batch-dynasty-systems

## Goal

Port the Tier 2 batch dynasty systems from the browser spec (simulation.js) to Unity DOTS/ECS:
renown award routing, marriage proposal expiration, marriage gestation child generation,
lesser house loyalty drift with defection spawning, and minor house levy unit spawning.

## Work Completed

### New Components

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - `MarriageProposalStatus` enum (Pending/Accepted/Declined/Expired)
  - `MarriageProposalComponent` (IComponentData)
  - `MarriageComponent` (IComponentData, IsPrimary/ChildGenerated/Dissolved)
  - `LesserHouseElement` (IBufferElementData, includes LastDriftAppliedInWorldDays for DualClock gating)
  - `MinorHouseLevyComponent` (IComponentData)

- `unity/Assets/_Bloodlines/Code/Dynasties/RenownAwardRequestComponent.cs`
  - `RenownAwardRequestComponent` (IComponentData, FactionId/Amount/Consumed)

### New Systems

- `unity/Assets/_Bloodlines/Code/Dynasties/RenownAwardSystem.cs`
  - Priority routing: Commander > HeadOfBloodline > MilitaryCommand path > any available
  - RENOWN_CAP = 100f. Marks request Consumed = true after processing.

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs`
  - ExpirationInWorldDays = 30f (public const). Uses DualClockComponent.InWorldDays.

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs`
  - GestationInWorldDays = 60f. Fires on IsPrimary marriage when inWorldDays >= ExpectedChildAtInWorldDays.
  - Creates child DynastyMemberComponent (HeirDesignate, Age=0, Renown=4), adds to head faction buffer.

- `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - Uses DualClockComponent.InWorldDays + LastDriftAppliedInWorldDays tracking per element.
  - Applies floor(daysSinceLast) days of drift per update. BaseDailyDelta = -0.5f.
  - Structural change safety: collects defecting houses in NativeList<LesserHouseElement>,
    completes ALL buffer[j]=lh writes, THEN spawns minor faction entities.
  - SpawnDefectedMinorFaction: FactionId = "minor-{HouseId}", FactionKind.MinorHouse,
    AIEconomyControllerComponent, MinorHouseLevyComponent.

- `unity/Assets/_Bloodlines/Code/Dynasties/MinorHouseLevySystem.cs`
  - Filters FactionKind.MinorHouse entities. LevyAccumulator += dt.
  - Interval floor = 0.001f (smoke-test friendly). Spawns levy_militia UnitTypeComponent.

### New Smoke Validator

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTier2BatchSmokeValidation.cs`
  - Phase 1: RenownAwardSystem routes +5 renown to Commander (15 total), Consumed=true.
  - Phase 2: MarriageProposalExpirationSystem expires proposal at day 35 (expires at day 30).
  - Phase 3: MarriageGestationSystem generates child at day 70 (expected at day 60), MemberCount 0->1.
  - Phase 4: LesserHouseLoyaltyDriftSystem defects house at day 5 (Loyalty=2, delta=-1/day), spawns minor-lh-1.
  - Phase 5: MinorHouseLevySystem fires levy on first tick with interval=0f, unit count increases.

- `scripts/Invoke-BloodlinesUnityTier2BatchSmokeValidation.ps1`
  - Batch wrapper; artifact at `artifacts/unity-tier2-batch-smoke.log`.
  - Looks for `BLOODLINES_TIER2_BATCH_SMOKE PASS/FAIL`.

### Shared-File Edits

- `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs`
  - Added `MinorHouse = 3` to `FactionKind` enum.
- `unity/Assembly-CSharp.csproj` -- all 7 Dynasties files registered.
- `unity/Assembly-CSharp-Editor.csproj` -- BloodlinesTier2BatchSmokeValidation.cs registered.

## Bugs Fixed During Development

1. **Phase 4 ObjectDisposedException** -- `SpawnDefectedMinorFaction` called `em.CreateEntity()` while
   DynamicBuffer<LesserHouseElement> was still held inside the loop. Fixed by collecting defections in
   NativeList first, then spawning after all buffer writes complete.

2. **Phase 5 levy not firing** -- `MinorHouseLevySystem` used `> 0.001f ? interval : 60f` so interval=0f
   fell back to 60f instead of the smoke-test floor. Fixed to `interval < 0.001f ? 0.001f : interval`.

3. **Initial LesserHouseLoyaltyDriftSystem design** used real-time accumulator (SecondsPerDay=432f).
   This could never fire in single-tick smoke tests. Redesigned to use DualClock in-world days.

## Verification Results

All 8 gates green:
1. dotnet build Assembly-CSharp.csproj: 0 errors
2. dotnet build Assembly-CSharp-Editor.csproj: 0 errors
3. Bootstrap runtime smoke: PASS
4. Combat smoke: all 8 phases PASS
5. Canonical scene shells: PASS
6. data-validation.mjs: exit 0
7. runtime-bridge.mjs: exit 0
8. Contract staleness check: exit 0 (revision 11)

Tier 2 batch smoke: PASS (all 5 phases)

## Current Readiness

All 5 Tier 2 batch dynasty systems are implemented, tested, and validated.
This is a complete slice. No follow-up items are required before committing.

## Next Action

Item 3: Victory Conditions System.
- VictoryStateComponent singleton
- VictoryConditionEvaluationSystem with per-condition evaluators:
  - Military conquest (all enemy factions destroyed or surrendered)
  - Territorial governance (80% loyalty hold for 120 in-world seconds)
  - Faith divine right (Level 5 divine-right supremacy)
- Smoke validator + contract lane + state file updates
- Browser reference: simulation.js evaluateVictoryConditions (search ~checkVictoryConditions)
