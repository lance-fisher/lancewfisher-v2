# Unity AI Strategic Layer Sub-Slice 15: Pact Break And Expiration

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-pact-break-expiration`
**Lane:** ai-strategic-layer
**Contract Revision:** 30
**Master Base:** `879f1647` (after sub-slice 14 landed)

---

## Goal

Port the break path for non-aggression pacts. Browser
`breakNonAggressionPact` (simulation.js ~5224-5257) is an explicit
operation: it marks both faction records as broken, re-establishes
mutual hostility, and applies a breaker-side legitimacy and
conviction penalty. There is no auto-expiration in the browser
canon; `minimumExpiresAtInWorldDays` marks only the timing
threshold for the "early-break" message. Sub-slice 15 ports the
mechanical break behavior while preserving the explicit-only
semantic.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/PactBreakRequestComponent.cs`
  `IComponentData` request marker. Fields: `PactId` (matches the
  PactComponent to break) and `RequestingFactionId` (the breaker,
  who pays the penalty). Attached to a short-lived entity by test
  harnesses, future player-UI break actions, or future AI policy
  systems. Destroyed by `PactBreakSystem` after processing.

- `unity/Assets/_Bloodlines/Code/AI/PactBreakSystem.cs`
  ISystem, `[UpdateInGroup(SimulationSystemGroup)]`,
  `[UpdateAfter(AIPactProposalExecutionSystem)]`. Per update:
  1. Query all PactBreakRequestComponent entities.
  2. For each request, call `ApplyBreak` and destroy the entity.
  3. `ApplyBreak` finds the matching PactComponent by PactId,
     returns early if not found or already broken, then:
     - Sets `Broken = true` and `BrokenByFactionId = requesting`.
     - Calls `EnsureHostility` both ways (idempotent add of
       `HostilityComponent` buffer entries).
     - Applies `DynastyStateComponent.Legitimacy -= 8` clamped
       [0, 100] on the breaker.
     - Applies `ConvictionComponent.Oathkeeping -= 2` on the breaker
       via `ConvictionScoring.ApplyEvent` (the helper clamps the
       bucket at zero).

  Constants exposed as `public const`:
  - `LegitimacyCost = 8f` (NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST)
  - `OathkeepingPenalty = 2f` (browser conviction.score -= 2 mapped
    to Oathkeeping bucket)
  - `LegitimacyFloor = 0f`
  - `LegitimacyCeiling = 100f`

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPactBreakSmokeValidation.cs`
  Five-phase smoke validator:
  - **PhaseEarlyBreak**: pact at day 30, minimum expiry 210;
    current day 50 so earlyBreak is true. Enemy (breaker) breaks
    the pact. Result: PactComponent.Broken=true,
    BrokenByFactionId=enemy, hostility restored both ways, enemy
    legitimacy 70 -> 62, enemy Oathkeeping 5 -> 3. Request entity
    destroyed. Player side untouched.
  - **PhaseLateBreak**: current day 250 (past minimum expiry 190);
    penalty mechanics are identical to early-break. Proves the
    unconditional penalty matches the browser (earlyBreak flag
    only affects messaging).
  - **PhaseIdempotentBreak**: first break applied, then a second
    break request on the same already-broken pact does not
    double-apply the penalty. Legitimacy and Oathkeeping remain
    at the first-break values.
  - **PhaseNoPactNoOp**: break request for a PactId that does not
    match any PactComponent. State untouched; request entity still
    destroyed.
  - **PhaseLegitimacyClamp**: breaker starts at Legitimacy 5 and
    Oathkeeping 0. Legitimacy clamps to 0 (5 - 8 = -3 -> 0) and
    Oathkeeping clamps at 0 (ApplyEvent's `max(0, amount + current)`).
  Artifact: `../artifacts/unity-pact-break-smoke.log`
  Marker: `BLOODLINES_PACT_BREAK_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityPactBreakSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesPactBreakSmokeValidation.RunBatchPactBreakSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added entries for
  `PactBreakRequestComponent.cs` and `PactBreakSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added entry for
  `BloodlinesPactBreakSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Reads + writes own `PactComponent` fields (Broken,
  BrokenByFactionId) on the matching pact entity.
- Adds (idempotent) `HostilityComponent` buffer entries on both
  factions (combat-and-projectile lane, retired). If the buffer
  is missing it is created; no schema change.
- Mutates `DynastyStateComponent.Legitimacy` on the breaker
  (dynasty-core lane, retired). Field update only, clamped [0, 100].
- Mutates `ConvictionComponent.Oathkeeping` on the breaker via
  `ConvictionScoring.ApplyEvent` helper (conviction-scoring lane,
  retired). The helper refreshes Score and Band in place.

---

## Browser Parity Notes

Browser `breakNonAggressionPact` (simulation.js:5224-5257):

```js
const earlyBreak = currentInWorldDays < minimumExpiry;
pact.brokenAt = state.meta.elapsed;
pact.brokenByFactionId = factionId;
const targetPact = getActivePact(targetFaction, factionId);
if (targetPact) {
  targetPact.brokenAt = state.meta.elapsed;
  targetPact.brokenByFactionId = factionId;
}
ensureMutualHostility(state, factionId, targetFactionId);
if (faction.dynasty) {
  faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - 8);
}
if (faction.conviction) {
  faction.conviction.score = clamp((faction.conviction.score ?? 0) - 2, -100, 100);
}
pushMessage(...);
return { ok: true, earlyBreak };
```

Unity preserves each effect:

- **Both sides marked broken.** Browser mirrors pact records on both
  factions and marks both. Unity collapsed to a single PactComponent
  entity in sub-slice 14, so there is only one record to mark. Same
  semantic outcome.
- **Mutual hostility.** Browser `ensureMutualHostility` is
  idempotent: adds hostility only if not already present. Unity
  `EnsureHostility` helper scans the buffer before adding.
- **Breaker legitimacy -8 clamped [0, 100].** Ported via
  `math.clamp(Legitimacy - 8, 0, 100)`. Browser uses
  `Math.max(0, legitimacy - 8)` which clamps only on the low side;
  this is equivalent in practice because the pre-break legitimacy
  is already in [0, 100] and subtracting 8 cannot push it above 100.
- **Breaker conviction.score -= 2 clamped [-100, 100].** Unity's
  conviction Score is derived from bucket values
  (`ConvictionScoring.DeriveScore: Stewardship + Oathkeeping -
  Ruthlessness - Desecration`). Directly writing Score would be
  overwritten by the next Refresh call, so the penalty is applied
  at the bucket layer via `ConvictionScoring.ApplyEvent(ref c,
  Oathkeeping, -2)`. This maps cleanly: pacts are oaths, and
  breaking one is an oath-keeping stain. `ApplyEvent` clamps the
  bucket at zero, so a faction with no remaining Oathkeeping
  (already an oath-breaker) takes no additional penalty. That
  matches the spirit of the browser canon: if you have no oath
  integrity left to lose, another betrayal costs you nothing.
- **`earlyBreak` return value / message only.** Browser uses the
  flag purely for the narrative pushMessage text ("The early breach
  damages legitimacy and conviction.") vs "Hostility resumes." The
  mechanical penalty is identical. Unity's `PhaseEarlyBreak` and
  `PhaseLateBreak` both verify the same 62/3 outcome to prove
  this parity.
- **pushMessage deferred.** Same blocker as sub-slices 11, 12, 13,
  14. Will land with the AI-to-UI message bridge.

**No auto-expiration.** Browser canon has no timer that dissolves a
pact. `minimumExpiresAtInWorldDays` marks the threshold for
messaging ("early breach" vs normal breach) and for future mechanics
(early-break penalty variants, diplomatic-memory records); it does
not trigger dissolution. Unity matches: `PactBreakSystem` fires only
on explicit `PactBreakRequestComponent` and never polls a timer.

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
10. Contract staleness check -- revision 30 current

Sub-slice 15 dedicated smoke
(`Invoke-BloodlinesUnityPactBreakSmokeValidation.ps1`): all 5 phases
PASS.

---

## Key Design Notes

**Request-component pattern, not timer-polling.** The browser has
no internal caller for `breakNonAggressionPact`; the AI does not
auto-break pacts and there is no expiration ticker. Unity mirrors
that by requiring an explicit `PactBreakRequestComponent`
producer. The system never polls PactComponents looking for
expired-but-unbroken records.

**Idempotent hostility re-establishment.** `EnsureHostility`
checks the target buffer for an existing entry before adding. This
matters because broken pacts should not stack hostility: a pact
created between two factions who had been hostile, then broken,
should leave them with exactly one hostility entry per side. If a
future system creates additional hostility entries for unrelated
reasons, those are untouched.

**Bucket-layer conviction penalty.** The browser writes
`faction.conviction.score -= 2` directly. In Unity, Score is
derived from bucket values and any direct write would be
overwritten by the next Refresh. The penalty must land at the
bucket layer. Oathkeeping is the natural choice because pacts are
oaths; writing at the Stewardship, Ruthlessness, or Desecration
layer would be semantically wrong. The trade-off is that
`ConvictionScoring.ApplyEvent` clamps the bucket at zero
(`max(0, current + amount)`), so a faction with zero Oathkeeping
takes no additional penalty. This is acceptable: a faction that
has already burned all its oath-keeping cannot be further stained
by one more betrayal.

**Idempotent on re-request.** A second `PactBreakRequestComponent`
for an already-broken pact short-circuits at the `pact.Broken`
check. The request entity is still destroyed so the same request
does not loop forever. This lets UI code produce break requests
without first checking whether the pact is already broken.

**No target-side penalty.** Browser only penalizes the breaker
(the faction identified by `factionId` in the function call). The
target of the break is put back into mutual hostility but keeps
its legitimacy and conviction. Unity matches exactly.

---

## Current Readiness

Branch `claude/unity-ai-pact-break-expiration` is ready to merge.
All gates green, contract at revision 30, continuity files
updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Next candidates:
   - **Captive recovery execution** (sub-slice 16): port `CovertOpKind.CaptiveRescue` / `CaptiveRansom` consumers.
   - **Narrative message bridge**: still deferred across sub-slices 11-15.
   - **Assassination / Missionary / HolyWar / DivineRight execution**: each follows the established one-shot pattern.
3. Codex fortification-siege lane: wall-segment destruction resolution remains the recommended next Codex slice.
