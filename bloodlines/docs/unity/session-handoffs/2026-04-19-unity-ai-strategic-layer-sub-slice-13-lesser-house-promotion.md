# Unity AI Strategic Layer Sub-Slice 13: Lesser-House Promotion Execution

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-lesser-house-promotion`
**Lane:** ai-strategic-layer
**Contract Revision:** 28
**Master Base:** `cda757c6` (after Codex sub-slice 4 + Claude governance update)

---

## Goal

Port `tryAiPromoteLesserHouse` (ai.js ~2784-2801) and the mechanical
core of `promoteMemberToLesserHouse` (simulation.js ~7184-7258).
Sub-slice 6 already dispatches `CovertOpKind.LesserHousePromotion`
into `AICovertOpsComponent.LastFiredOp`; sub-slice 13 consumes that
dispatch and executes the strategic gate plus the founding effect.

With sub-slice 13 landed, the lesser-house pipeline is mechanically
functional in Unity: AI dynasties below the consolidation ceiling
can promote eligible members to cadet branches, the new
`LesserHouseElement` enters the daily loyalty drift loop immediately,
and dynasty legitimacy + Stewardship conviction reflect the
governance act exactly as the browser canon dictates.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AILesserHousePromotionSystem.cs`
  ISystem, `[UpdateInGroup(SimulationSystemGroup)]`,
  `[UpdateAfter(AICovertOpsSystem)]`. Per update:
  1. Iterates faction entities with `AICovertOpsComponent`.
  2. Skips any whose `LastFiredOp != CovertOpKind.LesserHousePromotion`.
  3. For matched factions, calls `TryPromote` then unconditionally
     clears `LastFiredOp` to `None` (one-shot pattern shared with
     sub-slices 8/9/12).
  4. `TryPromote` gates on:
     - `DynastyStateComponent.Legitimacy < 90` (browser
       LESSER_HOUSE_LEGITIMACY_CEILING_BLOCK; consolidated houses
       do not need cadet branches).
     - `LesserHouseElement` buffer count `< 3` (browser
       LESSER_HOUSE_PROMOTION_CAP).
     - `DynastyMemberRef` buffer is present.
  5. Walks the member roster in order; selects the first eligible
     candidate that meets:
     - `Status == Active || Ruling`.
     - `Role != HeadOfBloodline` (head continues the main line).
     - `Path` is one of `Governance`, `MilitaryCommand`,
       `CovertOperations` (browser LESSER_HOUSE_QUALIFYING_PATHS).
     - `Renown >= 30` (browser LESSER_HOUSE_RENOWN_THRESHOLD).
     - `MemberId` is not already a `FounderMemberId` of an existing
       `LesserHouseElement` on this faction (cross-reference because
       Unity's `DynastyMemberComponent` lacks a `FoundedLesserHouseId`
       field; the browser writes `member.foundedLesserHouseId` as a
       per-member field).
  6. On match, appends a new `LesserHouseElement` with:
     - `HouseId = "lh-<founderMemberId>"`
     - `HouseName = "Cadet of <founderTitle>"`
     - `FounderMemberId = founder.MemberId`
     - `Loyalty = 75` (browser LESSER_HOUSE_INITIAL_LOYALTY)
     - `DailyLoyaltyDelta = 0` (the existing
       `LesserHouseLoyaltyDriftSystem` recomputes this each in-world
       day based on parent dynasty legitimacy and other inputs)
     - `LastDriftAppliedInWorldDays = inWorldDays` so the next
       in-world day boundary triggers a drift application
     - `Defected = false`
  7. Applies dynasty legitimacy `+3` clamped to `100` (browser
     LESSER_HOUSE_LEGITIMACY_BONUS).
  8. Records a Stewardship conviction event `+2` via
     `ConvictionScoring.ApplyEvent` (browser
     `recordConvictionEvent("stewardship", 2, "lesser_house_founded")`).

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAILesserHousePromotionSmokeValidation.cs`
  Six-phase smoke validator:
  - **PhaseSuccessfulPromotion**: legitimacy 60, no existing lesser
    houses, eligible Governor (Governance, Active, Renown 35); after
    one update there is exactly 1 LesserHouseElement, legitimacy is
    63, stewardship is 2, dispatch cleared.
  - **PhaseHighLegitimacyBlocks**: same eligible roster but legitimacy
    is at the 90 ceiling; promotion does not fire, dispatch still
    clears.
  - **PhaseCapBlocks**: 3 pre-existing lesser houses block promotion
    even with low legitimacy and an eligible Commander.
  - **PhaseHeadOfBloodlineRejected**: only the head meets the renown
    threshold; head is excluded from the candidate pool; promotion
    does not fire.
  - **PhaseNonQualifyingPathRejected**: only Diplomat (Diplomacy) and
    Merchant (EconomicStewardshipTrade) meet the renown threshold;
    both are non-qualifying paths; promotion does not fire.
  - **PhaseNearCeilingPromotion**: legitimacy starts at 89 (just
    below the consolidation ceiling) with an eligible CovertOperations
    Spymaster; +3 bonus lands at 92. The clamp-to-100 logic is wired
    but unreachable in practice because the legitimacy ceiling block
    at 90 fires first; this phase proves the near-ceiling path
    executes cleanly.
  Artifact: `../artifacts/unity-ai-lesser-house-promotion-smoke.log`
  Marker: `BLOODLINES_AI_LESSER_HOUSE_PROMOTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAILesserHousePromotionSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAILesserHousePromotionSmokeValidation.RunBatchAILesserHousePromotionSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added entries for
  `AILesserHousePromotionSystem.cs` and (because they were missing
  from the local csproj after the master-base switch) Codex's
  sub-slice 4 files `SiegeSupplyCampComponent.cs`,
  `SiegeSupplyInterdictionCanon.cs`, `SiegeSupplyInterdictionSystem.cs`.
  The csproj is gitignored and these were missing because Unity had
  not regenerated the project file after Codex's lane changes
  reached this worktree.
- `unity/Assembly-CSharp-Editor.csproj` -- added entries for
  `BloodlinesAILesserHousePromotionSmokeValidation.cs` and (same
  reason as above) `BloodlinesSiegeSupplyInterdictionSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Reads `AICovertOpsComponent.LastFiredOp` and writes
  `CovertOpKind.None` when consuming the dispatch (this lane owns
  the AI components).
- Reads `DynastyStateComponent.Legitimacy` and writes the +3 clamp
  (dynasty-core lane, retired). Field update only, no schema change.
- Reads `DynastyMemberRef` buffer and `DynastyMemberComponent` fields
  (Role, Path, Status, Renown, MemberId, Title) (dynasty-core lane,
  retired). Read-only iteration.
- Reads + writes `LesserHouseElement` buffer (tier2-batch-dynasty-systems
  lane, retired). Append only; the buffer element schema is not
  modified. New entries carry `LastDriftAppliedInWorldDays` so the
  existing `LesserHouseLoyaltyDriftSystem` picks them up at the next
  in-world-day boundary.
- Reads + writes `ConvictionComponent.Stewardship` via
  `ConvictionScoring.ApplyEvent` helper (conviction-scoring lane,
  retired). The helper refreshes Score and Band in place.
- Reads `DualClockComponent.InWorldDays` (dual-clock-match-progression
  lane, retired). Read-only.

---

## Browser Parity Notes

The browser's `tryAiPromoteLesserHouse` is short and gates on three
strategic conditions before delegating to `promoteMemberToLesserHouse`:

```
const candidates = enemy.dynasty.lesserHouseCandidates ?? [];
if (candidates.length === 0) return false;
if (legitimacy >= 90) return false;
if ((enemy.dynasty.lesserHouses ?? []).length >= 3) return false;
return promoteMemberToLesserHouse(state, "enemy", candidates[0]).ok;
```

Unity inlines the candidate-selection step (browser pre-builds
`lesserHouseCandidates` via a separate ticker; Unity walks the roster
each dispatch instead). The simplification is:

- The browser's `tickLesserHouseCandidates` (~6481-6507) maintains a
  per-faction `lesserHouseCandidates: string[]` and announces newly-
  eligible members. Unity does not yet maintain this list as a
  separate buffer because there is no UI surface that needs the
  announcement. When such a UI lands, a `LesserHouseCandidateBuffer`
  with the announcement state can be added without changing the
  promotion gate logic.
- The browser tracks `member.promotionHistory.length >= 1` as a
  gate (LESSER_HOUSE_MIN_PROMOTIONS). Unity has no promotion-history
  field on DynastyMemberComponent; this gate is intentionally
  deferred. Renown >= 30 is a strong proxy because canonical
  bootstrap members start at 7-22 renown and only growth-driven
  members reach 30+.

The mechanical effects on success match exactly:

- Append lesser-house record with Loyalty=75. The browser builds a
  rich record with marital-anchor, world-pressure, and deriveLesserHouseName
  fields; Unity uses the canonical `LesserHouseElement` struct that
  the tier2-batch-dynasty-systems lane already shipped, which carries
  the loyalty-drift-relevant fields and leaves marital-anchor /
  world-pressure profile state for a future slice.
- Mark founder so they are not re-nominated. Browser writes
  `member.foundedLesserHouseId`. Unity skips this field write
  (DynastyMemberComponent has no such field) and instead detects
  re-nomination by cross-referencing existing
  `LesserHouseElement.FounderMemberId` values. Functionally equivalent
  for the promotion gate; the founder's UI badge is deferred until
  the field exists.
- `adjustLegitimacy(faction, +3)`. Browser clamps low at 0 and high
  at 100. Unity uses `min(100, L + 3)` because +3 cannot push
  legitimacy below zero; the clamp at 0 is unreachable in this code
  path.
- `recordConvictionEvent("stewardship", +2, "lesser_house_founded")`.
  Unity calls `ConvictionScoring.ApplyEvent(ref c, Stewardship, +2)`.
  The helper refreshes Score and Band in place.
- `pushMessage("...founds <house>...")`. Deferred until the AI->UI
  message bridge exists (sub-slice 14 candidate).

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision 28 current

Sub-slice 13 dedicated smoke
(`Invoke-BloodlinesUnityAILesserHousePromotionSmokeValidation.ps1`):
all 6 phases PASS.

Note on csproj refresh: the local `Assembly-CSharp.csproj` was
missing entries for Codex's sub-slice 4 files
(`SiegeSupplyCampComponent.cs`, `SiegeSupplyInterdictionCanon.cs`,
`SiegeSupplyInterdictionSystem.cs`) and the editor csproj was missing
`BloodlinesSiegeSupplyInterdictionSmokeValidation.cs` because Unity
had not regenerated the project files in this worktree after the
master-base switch. Both csproj files were updated as part of this
slice. csproj files remain gitignored and are not part of the commit.

---

## Key Design Notes

**One-shot dispatch consumption.** The dispatch is cleared
unconditionally after the gate is evaluated, matching the browser's
single-fire-per-timer pattern shared with sub-slices 8 and 9. A
faction that fails any gate consumes its dispatch and waits for the
next AICovertOpsSystem fire to retry; the timer cadence and
context-flag computation already live in sub-slices 2/3/6.

**Cross-reference for foundedLesserHouseId.** Browser writes
`member.foundedLesserHouseId` so subsequent `tryAiPromoteLesserHouse`
calls do not re-nominate the same member. Unity instead scans the
existing `LesserHouseElement` buffer for the candidate's MemberId
and skips matches. Functionally equivalent; the cross-reference is
O(rosters * lesser_houses) per dispatch, which is fine at the cap of
3 lesser houses.

**Buffer recompute through the existing drift system.** The new
LesserHouseElement enters the buffer with `DailyLoyaltyDelta = 0` and
`LastDriftAppliedInWorldDays = currentInWorldDays`. The existing
`LesserHouseLoyaltyDriftSystem` runs each tick and only applies
drift when `daysSinceLast >= 1`, so the new house gets exactly one
drift application at the next in-world-day boundary. This matches
the browser pattern where new houses join the daily drift loop
immediately.

**Renown threshold as promotion-history proxy.** The browser also
gates on `member.promotionHistory.length >= 1` to ensure the member
"climbed" through prior roles before founding a cadet branch. Unity
has no promotion-history tracking yet. Renown >= 30 covers the
intent because canonical bootstrap members start at 7-22 renown and
only members who participated in the renown-growth pipeline (combat,
diplomacy events, etc.) reach 30. When promotion-history is added
to DynastyMemberComponent, the explicit gate can be ported.

**Why no `MarriageAcceptanceTermsComponent`-style provenance tag.**
Sub-slice 12 attached a durable provenance tag to record marriage
authority. For lesser-house founding the LesserHouseElement itself
is the durable record (FounderMemberId, HouseId, foundedAt via
LastDriftAppliedInWorldDays initialization). No separate tag is
needed; the buffer entry IS the provenance.

---

## Current Readiness

Branch `claude/unity-ai-lesser-house-promotion` is ready to merge.
All gates green, contract at revision 28, continuity files
(this handoff plus state-file appends) updated.

---

## Next Action

1. Merge `claude/unity-ai-lesser-house-promotion` into master via the
   merge-temp ceremony (D:\BLAICD holds master in a separate
   worktree).
2. Open candidates for sub-slice 14 / 15:
   - **Narrative message bridge** (sub-slice 14): unblocks the
     deferred `pushMessage` calls from sub-slices 11, 12, and 13.
     Needs a design decision on per-faction vs global message buffer
     and a UI consumer.
   - **Pact proposal execution** (sub-slice 15): mirrors the
     marriage-proposal pattern from sub-slice 8. Smaller scope, no
     new design decisions. Ports `proposeNonAggressionPact` from the
     pact dispatch block at ai.js ~2643-2665.
   - **Other CovertOpKind execution sub-slices**: assassination,
     missionary, holy war, divine right, captive recovery / ransom.
     Each follows the established pattern.
3. Codex lane fortification-siege remains active; recommended next
   Codex slice is wall-segment destruction resolution (per Codex's
   own sub-slice 4 handoff).
