# Unity AI Strategic Layer Sub-Slice 12: Marriage Acceptance Terms

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-marriage-acceptance-terms`
**Lane:** ai-strategic-layer
**Contract Revision:** 26

---

## Goal

Port `getMarriageAcceptanceTerms` from `src/game/core/simulation.js` (~6327)
together with the governance-authority legitimacy cost that
`acceptMarriage` (~7449) applies on the target side via
`applyMarriageGovernanceLegitimacyCost` (~6232). Sub-slice 11 explicitly
deferred this. The acceptance-terms layer makes the AI marriage accept
path browser-accurate end-to-end at the legitimacy/conviction level:

1. Resolve the target (AI) faction's authority before record creation.
   Head-direct (head_of_bloodline + Ruling) costs 0. Heir regency
   (heir_designate present) costs 1. Envoy regency (diplomat present)
   costs 2. No authority path means the accept is rejected and the
   dispatch is cleared without creating records.
2. Capture the resolved AuthorityMode + LegitimacyCost on the primary
   marriage entity as a durable provenance marker.
3. Apply the cost to the spouse legitimacy clamped [0, 100] before the
   sub-slice 11 +2 bonus, matching the browser order. Record a
   Stewardship -cost conviction event on the spouse so reckless regency
   marriages stain the stewardship ledger.

The narrative message push (`pushMessage` ceremonial line at
simulation.js:7463) remains deferred until an AI-to-UI message bridge
exists.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/MarriageAcceptanceTermsComponent.cs`
  Defines `MarriageAuthorityMode { None, HeadDirect, HeirRegency, EnvoyRegency }`
  and `MarriageAcceptanceTermsComponent { AuthorityMode, LegitimacyCost }`.
  Attached by `AIMarriageInboxAcceptSystem` to the primary marriage
  entity alongside `MarriageAcceptEffectsPendingTag`. Persists after
  effects apply as a provenance marker for downstream systems and HUD
  surfaces that may want to show "Married under regency" badging.

- `unity/Assets/_Bloodlines/Code/AI/MarriageAuthorityEvaluator.cs`
  Static helper. Simplified port of simulation.js `getMarriageAuthorityProfile`
  (~6134). Walks the target faction's `DynastyMemberRef` buffer. Selects
  in browser priority order: HeadDirect (head_of_bloodline + Ruling),
  HeirRegency (heir_designate available), EnvoyRegency (diplomat
  available). Returns `false` (reject) only when the roster exists but
  yields no head-direct, heir, or envoy. Backward-compatibility default:
  if the faction has no `DynastyMemberRef` buffer at all, returns
  HeadDirect with cost 0; this preserves sub-slice 11's synthetic test
  fixtures which never seed dynasty members.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageAcceptanceTermsSmokeValidation.cs`
  Five-phase dedicated smoke validator:
  - PhaseHeadDirect: roster head=Ruling, heir, envoy. Cost 0. Player
    legitimacy 80 -> 82, enemy 75 -> 77. Stewardship unchanged at 4.
  - PhaseHeirRegency: head=Fallen, heir=Active, envoy=Active. Cost 1.
    Enemy legitimacy 75 -> 76 (-1 cost + 2 bonus). Stewardship 4 -> 3.
  - PhaseEnvoyRegency: head=Fallen, no heir, diplomat=Active. Cost 2.
    Enemy legitimacy 75 -> 75 (-2 cost + 2 bonus). Stewardship 4 -> 2.
  - PhaseNoAuthority: head=Fallen, no heir, no diplomat. Accept
    rejected. No marriage record created. Proposal stays pending.
    Dispatch cleared. Enemy legitimacy untouched at 75.
  - PhaseTermsPersisted: head-direct accept retains
    `MarriageAcceptanceTermsComponent { HeadDirect, 0 }` on the primary
    marriage entity after `MarriageAcceptEffectsPendingTag` is removed.
  Artifact: `../artifacts/unity-ai-marriage-acceptance-terms-smoke.log`
  Marker: `BLOODLINES_AI_MARRIAGE_ACCEPTANCE_TERMS_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIMarriageAcceptanceTermsSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIMarriageAcceptanceTermsSmokeValidation.RunBatchAIMarriageAcceptanceTermsSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageInboxAcceptSystem.cs`
  - Calls `MarriageAuthorityEvaluator.TryResolve` before
    `TryAcceptIncoming`. If false, clears the dispatch and skips.
  - Adds `MarriageAcceptanceTermsComponent` to the primary marriage
    entity with the resolved AuthorityMode + LegitimacyCost.
  - Updated XML doc to describe sub-slice 12 layering.

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageAcceptEffectsSystem.cs`
  - Added `LegitimacyFloor = 0f`.
  - Reordered `OnUpdate` per-marriage to match browser order: cost
    first, hostility drop, oathkeeping +2, legitimacy +2, declare time.
  - New helper `ApplyAuthorityLegitimacyCost`: reads
    `MarriageAcceptanceTermsComponent` on the marriage entity, applies
    `Legitimacy = clamp(Legitimacy - cost, 0, 100)` on the spouse
    dynasty, calls `ConvictionScoring.ApplyEvent(..., Stewardship, -cost)`
    on the spouse conviction. Skips silently when terms component is
    absent (covers sub-slice 11 phase-3 untagged synthetic marriage).
  - Pending tag removed at end of update; terms component retained.

- `unity/Assembly-CSharp.csproj` -- added entries for
  `MarriageAcceptanceTermsComponent.cs` and `MarriageAuthorityEvaluator.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added entry for
  `BloodlinesAIMarriageAcceptanceTermsSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Reads `DynastyMemberRef` buffer and `DynastyMemberComponent` (Role,
  Status) on dynasty member entities of the target faction
  (dynasty-core lane, retired). Read-only iteration; no mutation.
- Mutates `DynastyStateComponent.Legitimacy` on the spouse faction
  (dynasty-core lane, retired). Field update only, clamped [0, 100].
- Mutates `ConvictionComponent.Stewardship` and Score/Band on the spouse
  faction via `ConvictionScoring.ApplyEvent` (conviction-scoring lane,
  retired). The helper clamps the bucket at 0 and refreshes the score.
- All other reads/writes are unchanged from sub-slice 11 (HostilityComponent
  buffer entries removed both ways, ConvictionComponent.Oathkeeping
  +2 both sides, DeclareInWorldTimeRequest pushed onto DualClock
  singleton buffer, DynastyStateComponent.Legitimacy +2 clamped to 100
  on both sides).

---

## Browser Parity Notes

Order of legitimacy mutations on the target dynasty matches
`acceptMarriage` (simulation.js ~7449-7458):

1. `applyMarriageGovernanceLegitimacyCost`: `target.dynasty.legitimacy = clamp(L - cost, 0, 100)`
   plus `recordConvictionEvent("stewardship", -penalty)`.
2. `target.dynasty.legitimacy = min(100, L + 2)`.

For target legitimacy `L` and authority cost `c`:

- Net target legitimacy after both steps: `min(100, max(0, L - c) + 2)`.
- For L=80, c=2: 80 -> 78 -> 80 (net 0).
- For L=80, c=1: 80 -> 79 -> 81 (net +1).
- For L=80, c=0: 80 -> 80 -> 82 (net +2; matches sub-slice 11 baseline).
- For L=99, c=2: 99 -> 97 -> 99 (cost cancels bonus).
- For L=100, c=2: 100 -> 98 -> 100 (already at ceiling).

Source dynasty mutations are unchanged (browser does not assess a
governance cost on the proposing side in `acceptMarriage`). Stewardship
events fire only when cost > 0; head-direct accepts skip the conviction
write entirely.

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
10. Contract staleness check -- revision=26 current, latest handoff
    date matches.

Sub-slice 11 regression (`Invoke-BloodlinesUnityAIMarriageAcceptEffectsSmokeValidation.ps1`):
all 6 phases PASS. The new code paths default to HeadDirect cost 0
when the synthetic test fixtures omit `DynastyMemberRef`, so the
sub-slice 11 expected legitimacy and oathkeeping values are unchanged.

Sub-slice 12 dedicated smoke
(`Invoke-BloodlinesUnityAIMarriageAcceptanceTermsSmokeValidation.ps1`):
all 5 phases PASS.

---

## Key Design Notes

**Backward-compatibility default for missing rosters.** The browser
`getMarriageAcceptanceTerms` rejects when authority is unavailable.
Unity's evaluator preserves that for factions that have a roster but
no qualifying members, but defaults to HeadDirect when the roster
buffer is absent. This is the cleanest way to preserve the sub-slice 11
tests (which never seeded `DynastyMemberRef`) while keeping the strict
path active in real bootstrapped matches. `DynastyBootstrap.AttachDynasty`
always seeds the canonical 8-member set with head_of_bloodline = Ruling,
so real factions take the strict head-direct path on accept.

**Cost-before-bonus ordering.** Browser `acceptMarriage` calls
`applyMarriageGovernanceLegitimacyCost` before the +2 legitimacy bonus
block, so the cost can be partially absorbed by the bonus when the
spouse is below the ceiling. Unity preserves this ordering inside
`AIMarriageAcceptEffectsSystem.OnUpdate`. The hostility drop and
oathkeeping events sit between the two legitimacy mutations exactly as
the browser does.

**Terms persisted as provenance.** The pending tag is removed after
effects apply, but `MarriageAcceptanceTermsComponent` stays attached to
the primary marriage entity. Future HUD or AI systems that want to
surface "married under regency" or audit the legitimacy debit can read
this without rerunning the evaluator. Dissolution systems can also use
the mode to decide whether dissolving a regency marriage produces a
different penalty.

**Single attach point for terms.** Both the pending tag and the terms
component are attached only to the primary marriage entity (IsPrimary
== true), matching the sub-slice 11 pattern that mirrored the browser's
single source-side `acceptedByAuthority` field at simulation.js:7407.
The mirror record stays clean.

**No regency-state component yet.** Browser regency machinery (full
regent appointment, regency duration, regent legitimacy bleed, etc.)
is not in Unity. Sub-slice 12 derives the authority mode from the
existing `DynastyMemberComponent.Role` + `Status` enum alone. When a
proper regency state component lands later, the evaluator can extend
to consult it without changing the cost numbers or the effects
pipeline.

---

## Current Readiness

Branch `claude/unity-ai-marriage-acceptance-terms` is ready to merge.
All gates green, contract at revision 26, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-marriage-acceptance-terms` into master via
   the documented merge-temp ceremony (D:\BLAICD holds master in a
   separate worktree; do not try to check out master in the main shared
   worktree).
2. Sub-slice 13 candidate: AI-to-UI narrative message bridge so the
   `pushMessage` ceremonial lines from `acceptMarriage` (and other AI
   marriage paths) can be surfaced to the player. Best handled as a
   broader narrative message channel because the same channel will
   serve proposal-sent, expiration, child-birth, and related events.
3. Codex lane fortification-siege remains open; next Codex slice is
   sub-slice 4 of that lane (unclaimed at handoff time).
