# Unity AI Strategic Layer Sub-Slice 14: Non-Aggression Pact Proposal Execution

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-pact-proposal-execution`
**Lane:** ai-strategic-layer
**Contract Revision:** 29
**Master Base:** `2b6df571` (after sub-slice 13 landed)

---

## Goal

Port the AI non-aggression pact proposal pipeline. Browser dispatch
at ai.js ~2643-2666 evaluates three strategic conditions
(succession crisis, low army count, or player governance near
victory) and fires `proposeNonAggressionPact` when pact terms pass.
The simulation-side sink at `simulation.js:5185-5222` validates
terms via `getNonAggressionPactTerms` (~5150), deducts 50 Influence
+ 80 Gold from the proposing faction, removes mutual hostility,
and creates symmetric `faction.diplomacy.nonAggressionPacts`
records on both sides with a 180 in-world-day minimum duration
marker.

Sub-slice 6 already dispatches `CovertOpKind.PactProposal` into
`AICovertOpsComponent.LastFiredOp`. Sub-slice 14 consumes that
dispatch and executes the browser-side pact semantics at the
mechanical level Unity tracks.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/PactComponent.cs`
  `IComponentData` carrying the active pact record:
  `PactId`, `FactionAId`, `FactionBId`, `StartedAtInWorldDays`,
  `MinimumExpiresAtInWorldDays`, `Broken`, `BrokenByFactionId`.
  One entity per pact. Unlike the marriage primary+mirror pattern
  from sub-slice 9, pacts are collapsed to a single symmetric
  entity because both sides carry identical fields and no
  asymmetric downstream system analogous to `MarriageGestationSystem`
  exists.

- `unity/Assets/_Bloodlines/Code/AI/AIPactProposalExecutionSystem.cs`
  ISystem, `[UpdateInGroup(SimulationSystemGroup)]`,
  `[UpdateAfter(AICovertOpsSystem)]`. Per update:
  1. Iterates faction entities with `AICovertOpsComponent`.
  2. Skips any whose `LastFiredOp != CovertOpKind.PactProposal`.
  3. For matched factions, calls `TryProposePact` against the
     hardcoded target "player" (browser ai.js ~2658 passes
     "enemy", "player"), then unconditionally clears `LastFiredOp`
     to `None` (one-shot pattern shared with sub-slices 8, 9, 12, 13).
  4. `TryProposePact` gates on:
     - Source != target.
     - Both factions have `FactionKindComponent.Kind == Kingdom`
       (browser kingdom-only gate).
     - Source hostile to target (browser `areHostile` gate; scans
       source's `HostilityComponent` buffer for a matching entry).
     - No existing `PactComponent` entity already links the two
       factions (either direction).
     - Source `ResourceStockpileComponent.Influence >= 50` and
       `Gold >= 80` (canonical cost).
  5. On success:
     - Deduct 50 Influence and 80 Gold from source
       `ResourceStockpileComponent`.
     - Scan `HostilityComponent` buffer on source (back-to-front)
       and remove all entries pointing to target. Do the same for
       the target-side buffer pointing to source.
     - Create one `PactComponent` entity with `FactionAId = source`,
       `FactionBId = target`, `StartedAtInWorldDays = currentInWorldDays`,
       `MinimumExpiresAtInWorldDays = current + 180`.

  Constants exposed as `public const`:
  - `InfluenceCost = 50f` (NON_AGGRESSION_PACT_INFLUENCE_COST)
  - `GoldCost = 80f` (NON_AGGRESSION_PACT_GOLD_COST)
  - `MinimumDurationInWorldDays = 180f` (NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS)

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIPactProposalExecutionSmokeValidation.cs`
  Six-phase smoke validator:
  - **PhaseSuccessfulPact**: both kingdoms, mutual hostility,
    source has 100 Influence + 200 Gold; after one update there
    is exactly 1 PactComponent, resources drop to 50/120,
    hostility clears both ways, dispatch cleared, expiry = 210
    (starting day 30 + 180).
  - **PhaseHostilityRequired**: no hostility on either side;
    pact request rejected per browser `areHostile` gate; resources
    untouched.
  - **PhaseAlreadyPactedRejected**: existing PactComponent
    pre-seeded between the two factions; new pact request
    short-circuits; still exactly 1 pact, resources untouched.
  - **PhaseInsufficientInfluence**: source has 30 Influence (below
    the 50 threshold); pact rejected; gold untouched.
  - **PhaseInsufficientGold**: source has 60 Gold (below the 80
    threshold); pact rejected; influence untouched.
  - **PhaseTribeRejected**: source `FactionKind.Tribes` (not
    Kingdom); pact rejected per browser canon requiring both
    parties to be kingdoms.
  Artifact: `../artifacts/unity-ai-pact-proposal-execution-smoke.log`
  Marker: `BLOODLINES_AI_PACT_PROPOSAL_EXECUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIPactProposalExecutionSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIPactProposalExecutionSmokeValidation.RunBatchAIPactProposalExecutionSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added entries for
  `PactComponent.cs` and `AIPactProposalExecutionSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added entry for
  `BloodlinesAIPactProposalExecutionSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Reads `AICovertOpsComponent.LastFiredOp` and writes
  `CovertOpKind.None` when consuming the dispatch (lane-owned).
- Reads `FactionKindComponent.Kind` on both factions (retired
  lanes).
- Reads + writes `HostilityComponent` buffer on both factions
  (combat-and-projectile lane, retired). Removes entries by
  `HostileFactionId` match. No buffer element schema change.
- Reads + writes `ResourceStockpileComponent.Influence` and
  `Gold` on the source faction (retired lanes). Field
  mutation only.

---

## Browser Parity Notes

Browser `getNonAggressionPactTerms` gates (simulation.js:5150-5183):
- Both kingdoms: ported.
- Source != target: ported.
- `areHostile(state, source, target)`: ported via
  `HostilityComponent` buffer scan.
- No active pact already (`getActivePact(faction, targetFactionId)`):
  ported via PactComponent existence scan.
- No active or incoming holy war: **deferred**. Unity has no
  holy-war system yet; the gate can be added when the holy-war
  lane lands without reshaping this system.
- `canAffordCost(faction.resources, cost)` with cost `{ influence: 50,
  gold: 80 }`: ported via `ResourceStockpileComponent` read.

Browser `proposeNonAggressionPact` effects (simulation.js:5185-5222):
- `applyCost(faction.resources, terms.cost)`: ported via
  `ResourceStockpileComponent` field mutation.
- `removeMutualHostility(state, factionId, targetFactionId)`:
  ported via `DropHostility` helper on both sides.
- Create pact record on source and target `faction.diplomacy.nonAggressionPacts`:
  Unity collapses both records into a single `PactComponent`
  entity because both sides carry identical fields and no
  downstream system filters on side-of-record. Queries looking
  for a faction's pacts scan all `PactComponent` entities and
  match either `FactionAId` or `FactionBId`.
- `pushMessage(...)`: deferred until the AI-to-UI message bridge
  lands (same blocker as sub-slices 11, 12, 13).

Browser also calls `proposeNonAggressionPact` unilaterally when
the AI decides the terms are favorable. There is no separate
acceptance gate on the target side. Unity preserves that
semantic: the system establishes the pact in one update when
gates pass, no accept tag or second stage required.

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
10. Contract staleness check -- revision 29 current

Sub-slice 14 dedicated smoke
(`Invoke-BloodlinesUnityAIPactProposalExecutionSmokeValidation.ps1`):
all 6 phases PASS.

---

## Key Design Notes

**Single-entity-per-pact.** Marriage uses primary+mirror records
because `MarriageGestationSystem` must spawn exactly one child per
marriage. Pacts have no asymmetric downstream system, so a single
`PactComponent` with both faction IDs is cleaner. Queries that
need per-faction pact enumeration scan all PactComponents and
match either FactionAId or FactionBId. This avoids the cost of
maintaining two mirrored entities that always carry identical
fields.

**One-shot dispatch consumption.** The dispatch is unconditionally
cleared after gate evaluation, matching the sub-slice 8, 9, 12,
13 pattern. A faction that fails any gate consumes its dispatch
and waits for the next AICovertOpsSystem fire; timer cadence and
context-flag computation already live in sub-slices 2/3/6.

**Back-to-front hostility buffer iteration.** Removing entries
from a DynamicBuffer during forward iteration invalidates indices.
The system iterates back-to-front so removals do not skip entries.
This matches the pattern from sub-slice 11's `DropHostility` and
the existing `MarriageProposalExpirationSystem`.

**Hostility required gate.** The browser pacts are explicitly
peace treaties: `areHostile` must be true before a pact can form
(the canon rejects "Already at peace"). Unity mirrors this. An
edge case worth flagging: if a pact has been broken and hostility
was re-established by some future break-and-penalty system, a new
pact can form. The PactComponent's `Broken` flag means the
existing-pact scan correctly ignores broken pacts, so a new
PactComponent can be created after a break.

**Holy-war gate deferred.** The browser rejects pacts during
active or incoming holy wars. Unity has no holy-war component
yet. When the holy-war lane lands, the gate plugs into
`TryProposePact` with a HasComponent + query pattern.

**Tribes cannot pact.** Browser canon: "Both parties must be
kingdoms." Unity enforces this via `FactionKindComponent.Kind ==
Kingdom` on both sides. Minor houses (added by sub-slice
tier2-batch-dynasty-systems) also cannot pact; only kingdoms
can.

---

## Current Readiness

Branch `claude/unity-ai-pact-proposal-execution` is ready to
merge. All gates green, contract at revision 29, continuity
files updated (this handoff plus state-file appends).

---

## Next Action

1. Merge `claude/unity-ai-pact-proposal-execution` into master
   via the merge-temp ceremony.
2. Next candidates:
   - **Pact break and expiration** (sub-slice 15): port
     `breakNonAggressionPact` (simulation.js ~5224) plus the
     early-break legitimacy penalty. Extends the existing
     `PactComponent.Broken` and `BrokenByFactionId` fields.
     Mechanical port following the same pattern.
   - **Narrative message bridge**: still deferred; unblocks all
     three open `pushMessage` paths from sub-slices 11, 12, 13,
     plus the new pact-creation message from sub-slice 14.
   - Other `CovertOpKind` execution sub-slices: assassination,
     missionary, holy war, divine right, captive recovery /
     ransom. Each follows the established pattern.
3. Codex lane fortification-siege remains active; recommended
   next Codex slice is wall-segment destruction resolution per
   Codex's own sub-slice 4 handoff.
