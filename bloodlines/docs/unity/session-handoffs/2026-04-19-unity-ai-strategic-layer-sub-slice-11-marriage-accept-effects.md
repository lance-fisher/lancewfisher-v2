# Unity AI Strategic Layer Sub-Slice 11: Marriage Accept Effects

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-marriage-accept-effects`
**Lane:** ai-strategic-layer
**Contract Revision:** 25

---

## Goal

Port the browser-side effects on marriage accept that sub-slice 9
explicitly deferred (simulation.js `acceptMarriage` ~7388-7469,
post-record block). Apply four effects exactly once per accepted
marriage via a tag-driven pipeline:

1. Legitimacy +2 on both HeadFaction and SpouseFaction dynasties,
   clamped to 100.
2. Hostility drop: remove `HostilityComponent` buffer entries pointing
   from head to spouse and from spouse to head.
3. 30-day `DeclareInWorldTimeRequest` push onto DualClock singleton so
   match-progression clock advances to reflect the wedding ceremony.
4. Oathkeeping conviction +2 on both factions via
   `ConvictionScoring.ApplyEvent` (score + band refreshed in place).

With sub-slice 11 landed, the browser marriage-accept block is
mechanically complete except for the governance-authority legitimacy
cost (requires a port of `getMarriageAcceptanceTerms`) and the narrative
message push (no AI-to-UI message component yet).

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/MarriageAcceptEffectsPendingTag.cs`
  Zero-size `IComponentData` tag. Attached in sub-slice 9's
  `AIMarriageInboxAcceptSystem` to the primary `MarriageComponent`
  entity at accept time so effects apply exactly once. The mirror
  record is not tagged to prevent double application.

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageAcceptEffectsSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateAfter(typeof(AIMarriageInboxAcceptSystem))]`. Per update:
  1. Queries `MarriageComponent + MarriageAcceptEffectsPendingTag`.
  2. For each matched marriage entity:
     - `ApplyLegitimacyBonus(HeadFactionId)` and
       `ApplyLegitimacyBonus(SpouseFactionId)` update
       `DynastyStateComponent.Legitimacy` to `min(100, legitimacy + 2)`.
     - `DropHostility(HeadFactionId, SpouseFactionId)` and reverse
       scan the source's `HostilityComponent` buffer and remove entries
       where `HostileFactionId.Equals(target)`.
     - `RecordOathkeepingBonus(HeadFactionId)` and spouse: call
       `ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, +2f)`
       on `ConvictionComponent`. The helper refreshes Score + Band.
     - `EnqueueMarriageTimeDeclaration(MarriageId)` locates the
       DualClock singleton, pushes a `DeclareInWorldTimeRequest` with
       `DaysDelta = 30f` and reason prefix `"Marriage "` plus the
       marriage id suffix.
     - Removes the pending tag.
  3. Skips any faction without the required component (DynastyStateComponent
     for legitimacy, ConvictionComponent for oathkeeping, HostilityComponent
     buffer for hostility). Safe on skirmish bootstraps that may not seed
     every component.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageAcceptEffectsSmokeValidation.cs`
  Six-phase smoke validator. Shared helpers seed DualClockComponent +
  DeclareInWorldTimeRequest buffer, player and enemy factions with
  dynasties + hostility + conviction, and pending MarriageProposalComponent.
  - Phase 1: Full accept pipeline with starting legitimacy 80 (player)
    and 75 (enemy) plus hostility both ways -> legitimacy 82/77, hostility
    dropped, tag removed.
  - Phase 2: Legitimacy ceiling. Both start at 99 -> both clamp to 100.
  - Phase 3: Synthetic MarriageComponent created WITHOUT the tag ->
    effects system must not touch it. Legitimacy stays 50/50, hostility
    unchanged.
  - Phase 4: Full pipeline end-to-end starting at legitimacy 70/65 ->
    72/67, hostility dropped, tag cleared.
  - Phase 5: Verifies the DualClock singleton buffer contains exactly
    one `DeclareInWorldTimeRequest` with `DaysDelta = 30f` after the
    update.
  - Phase 6: Starts oathkeeping at 10 (player) and 5 (enemy) + full
    accept flow -> player oathkeeping 12, enemy oathkeeping 7,
    ConvictionScoring.ApplyEvent has refreshed Score.
  Artifact: `../artifacts/unity-ai-marriage-accept-effects-smoke.log`
  Marker: `BLOODLINES_AI_MARRIAGE_ACCEPT_EFFECTS_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIMarriageAcceptEffectsSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIMarriageAcceptEffectsSmokeValidation.RunBatchAIMarriageAcceptEffectsSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageInboxAcceptSystem.cs` --
  adds `em.AddComponent<MarriageAcceptEffectsPendingTag>(primary)` on
  the primary marriage entity after `SetComponentData`. Mirror record is
  not tagged. This is the only behavioral change to sub-slice 9's file;
  structure and existing effects are untouched.
- `unity/Assembly-CSharp.csproj` -- added entries for
  `MarriageAcceptEffectsPendingTag.cs` and `AIMarriageAcceptEffectsSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added entry for
  `BloodlinesAIMarriageAcceptEffectsSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Mutates `DynastyStateComponent.Legitimacy` on both dynasties (dynasty-core
  lane, retired). Field update only, no schema change.
- Mutates `HostilityComponent` buffer on both factions (combat-and-projectile
  lane, retired). Removes entries by `HostileFactionId` match. No buffer
  element schema change.
- Mutates `ConvictionComponent` on both factions via
  `ConvictionScoring.ApplyEvent` helper (conviction-scoring lane, retired).
  The helper is designed for external callers.
- Pushes `DeclareInWorldTimeRequest` onto the DualClock singleton buffer
  (dual-clock-match-progression lane, retired). `DualClockDeclarationSystem`
  drains the buffer and applies the jump.
- Reads only: `MarriageComponent` fields on the tagged primary entity.

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
10. Contract staleness check -- revision=24 current at gate time; bumped
    to 25 post-gate as part of handoff

AI marriage accept effects smoke: all 6 phases PASS
- Phase 1 PASS: legitimacy +2 both sides (player=82, enemy=77), hostility dropped, tag removed
- Phase 2 PASS: legitimacy clamped to 100 both sides
- Phase 3 PASS: untagged marriage ignored, state unchanged
- Phase 4 PASS: full pipeline leaves legitimacy=72/67, hostility dropped, tag cleared
- Phase 5 PASS: 30-day DeclareInWorldTimeRequest enqueued on DualClock singleton
- Phase 6 PASS: oathkeeping +2 applied both sides (player=12, enemy=7), score refreshed

---

## Key Design Notes

**Tag-driven one-shot application.** The browser applies accept effects
inline inside `acceptMarriage`. Unity splits the creation (sub-slice 9)
from the effects (sub-slice 11) because they belong to different stages
of the simulation pipeline. The tag ensures the effects system runs
exactly once per marriage. Removing the tag after the first pass prevents
double application on subsequent frames.

**Primary record only.** Only the primary (IsPrimary = true) marriage
record is tagged. The mirror record exists for symmetric enumeration but
does not drive effects. `MarriageGestationSystem` already filters on
IsPrimary for child spawn; this slice follows the same pattern.

**ConvictionScoring helper reuse.** Rather than reimplementing the
bucket-update + score-refresh logic inline, the system calls
`ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, +2f)`
from the retired conviction-scoring lane. The helper is pure and
designed for external callers. If the bucket-score formula changes in
the conviction lane, this system picks up the update automatically.

**Time declaration via buffer push, not direct mutation.** The browser
calls `declareInWorldTime(state, 30, ...)` which mutates `state.dualClock`
directly. Unity's equivalent is the `DeclareInWorldTimeRequest` buffer
on the DualClock singleton, drained each frame by `DualClockDeclarationSystem`.
This slice pushes a request rather than advancing `InWorldDays` directly,
matching Unity's established declaration pipeline.

**Hostility buffer back-to-front iteration.** Removing entries from a
`DynamicBuffer` during forward iteration invalidates indices. The system
iterates back-to-front so removals don't skip entries. Matches the
pattern used in `MarriageProposalExpirationSystem`.

**Null-safe skip paths.** Each effect helper checks component existence
before reading/writing (HasComponent for DynastyState/Conviction,
HasBuffer for Hostility). If a bootstrap skips any of these on a faction,
the effects system silently skips that effect for that faction rather
than throwing.

---

## Current Readiness

Branch `claude/unity-ai-marriage-accept-effects` is ready to merge.
All gates green, contract at revision 25, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-marriage-accept-effects` into master.
2. Claim sub-slice 12 (marriage acceptance terms) on a new branch to
   port `getMarriageAcceptanceTerms` and apply the governance-authority
   legitimacy cost on accept. With that and the narrative message push
   deferred, sub-slice 11 covers the material mechanical effects of the
   browser accept block.
3. Codex lane fortification-siege remains open; next Codex slice is
   sub-slice 4 of that lane (unclaimed).
