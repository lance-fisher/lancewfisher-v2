# Unity AI Strategic Layer Sub-Slice 9: Marriage Inbox Accept

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-marriage-inbox-accept`
**Lane:** ai-strategic-layer
**Contract Revision:** 21

---

## Goal

Port the browser `tryAiAcceptIncomingMarriage` execution body (ai.js
~2880-2895) to Unity DOTS/ECS. Sub-slice 6 writes
`AICovertOpsComponent.LastFiredOp = CovertOpKind.MarriageInboxAccept` when
the covert-ops inbox timer fires but does not touch the inbox. This slice
consumes that signal, finds the first pending `MarriageProposalComponent`
from player to this AI faction, flips it to Accepted, and creates primary +
mirror `MarriageComponent` entities. `MarriageGestationSystem` then
schedules child generation at 60 in-world days (filters on `IsPrimary`).

With sub-slice 9 landed, the marriage loop is mechanically end-to-end:
proposal dispatch (sub-slice 6), proposal execution (sub-slice 8), accept
dispatch (sub-slice 6), accept execution (this slice), 30-day expiration
(existing `MarriageProposalExpirationSystem`), and 60-day gestation
(existing `MarriageGestationSystem`).

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageInboxAcceptSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateAfter(typeof(AIMarriageProposalExecutionSystem))]`. Per update:
  1. Pulls `InWorldDays` from the `DualClockComponent` singleton.
  2. Iterates every faction entity with `FactionComponent` and
     `AICovertOpsComponent`. Skips any whose `LastFiredOp` is not
     `MarriageInboxAccept`.
  3. For each AI faction dispatching accept, scans
     `MarriageProposalComponent` entities for the first match where
     `Status == Pending`, `SourceFactionId == "player"`, and
     `TargetFactionId == AI faction id`. Mirrors the browser's
     `inbox.find((p) => p.status === "pending" && p.sourceFactionId === "player")`
     exactly.
  4. If found: flips proposal `Status` to `Accepted`, creates a primary
     `MarriageComponent` entity (`IsPrimary = true`, head = proposal
     source side) and a mirror `MarriageComponent` entity
     (`IsPrimary = false`, head = proposal target side). Both share
     `MarriageId` and timestamps. `MarriageGestationSystem` filters on
     `IsPrimary` so only one child spawns.
  5. Always clears `LastFiredOp` to `CovertOpKind.None` after processing,
     regardless of whether a proposal was found. One dispatch produces
     at most one execution attempt.
  Source faction hardcoded to `"player"` matching the browser convention
  where the AI only accepts player-initiated proposals.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageInboxAcceptSmokeValidation.cs`
  Four-phase smoke validator. Each phase spins up an isolated World with
  `SimulationSystemGroup`, seeds `MatchProgressionComponent`,
  `DualClockComponent` at 20 in-world days, player faction with canonical
  dynasty, and an enemy faction with `AICovertOpsComponent` seeded with a
  specific `LastFiredOp`.
  - Phase 1: dispatch + pending player->enemy proposal -> proposal flipped
    to Accepted; exactly two `MarriageComponent` records created with
    matching MarriageId; exactly one of them has `IsPrimary = true`;
    `LastFiredOp` cleared to `None`.
  - Phase 2: dispatch + no pending proposal -> no marriage records
    created; `LastFiredOp` cleared.
  - Phase 3: dispatch + only an expired proposal in the system ->
    expired proposal stays `Expired`; no marriage records created;
    `LastFiredOp` cleared.
  - Phase 4: no dispatch (`LastFiredOp = None`) + pending proposal present
    -> proposal stays `Pending`; no marriage records; `LastFiredOp` stays
    `None`. Proves the system is a strict signal consumer.
  Artifact: `../artifacts/unity-ai-marriage-inbox-accept-smoke.log`
  Marker: `BLOODLINES_AI_MARRIAGE_INBOX_ACCEPT_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIMarriageInboxAcceptSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIMarriageInboxAcceptSmokeValidation.RunBatchAIMarriageInboxAcceptSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added `<Compile Include="..." />` entry
  for `AIMarriageInboxAcceptSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added `<Compile Include="..." />`
  entry for `BloodlinesAIMarriageInboxAcceptSmokeValidation.cs`.

### Cross-Lane Reads (no writes to another lane's code)

- Reads `MarriageProposalComponent` / `MarriageProposalStatus` and writes
  new `MarriageComponent` entities, mutates existing
  `MarriageProposalComponent.Status` pending->accepted. Component types
  owned by the retired tier2-batch-dynasty-systems lane. No modification
  to the struct layout or any existing dynasty system code.
- Reads `MarriageGestationSystem.GestationInWorldDays` constant so
  expected-child timestamps stay synchronized with the canonical gestation
  window. If the canon shifts, both systems update together.
- Reads `DualClockComponent.InWorldDays` from `Bloodlines.GameTime`.

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors PASS
3. Bootstrap runtime smoke -- PASS (required one retry after a transient
   bee_backend state-file rename conflict; passed cleanly on second run)
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS (required one retry after transient Library
   write-contention from back-to-back Unity launches)
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision=20 current at gate time; bumped
    to 21 post-gate as part of handoff

AI marriage inbox accept smoke: all 4 phases PASS
- Phase 1 PASS: proposal accepted, 2 marriages created (1 primary), LastFiredOp cleared
- Phase 2 PASS: no pending proposal, no marriage, LastFiredOp cleared
- Phase 3 PASS: expired proposal skipped, no marriage, LastFiredOp cleared
- Phase 4 PASS: no dispatch, proposal stays Pending, no marriage

---

## Key Design Notes

**Dual-record marriage creation.** The browser stores marriages on both
source and target faction arrays (`source.dynasty.marriages.unshift(sourceRecord)`
and `target.dynasty.marriages.unshift(targetRecord)`). This slice mirrors
that by creating two `MarriageComponent` entities with the same
`MarriageId`. The primary record (source side) has `IsPrimary = true`
and is the canonical child-spawn record. The mirror (target side) has
`IsPrimary = false` and exists for symmetric enumeration only.
`MarriageGestationSystem` already filters on `IsPrimary` so only one
child spawns at 60 in-world days. No changes required to the gestation
system.

**Mechanical record creation only.** The browser's `acceptMarriage` also
performs governance authority cost on the target's legitimacy, drops
hostility between factions, records oathkeeping conviction events on both
sides, increments legitimacy by 2 on both sides, calls
`declareInWorldTime(state, 30, ...)` to advance the world clock by 30
in-world days, and pushes a narrative message. None of these effects are
ported in this slice. They remain browser-only until a dedicated
ported-effects slice. The mechanical loop (proposal -> accept -> marriage
-> gestation -> child) is sufficient for this slice's gameplay value.

**Source gating matches browser.** The browser uses `p.sourceFactionId
=== "player"` in the inbox filter. This slice hardcodes the same literal
`"player"` check. Multi-faction extension is reserved for a later slice
alongside the proposal-side multi-faction extension (sub-slice 8 also
has hardcoded `"player"` as target).

**One-shot consumption.** Same pattern as sub-slice 8: unconditional clear
of `LastFiredOp` after processing. Prevents the system from re-firing on
the same dispatch signal across frames.

**`MarriageId` derived from proposal id.** `BuildMarriageId` prefixes
`"marriage-"` and copies proposal id characters (bounded to avoid
`FixedString64Bytes` overflow). Correlation between accepted marriages
and their source proposals is preserved for debugging and future save/load
serialization without needing a separate id generator.

**Proposal entity is mutated in place, not deleted.** The browser sets
`proposal.status = "accepted"` and keeps the proposal record. This slice
uses `EntityManager.SetComponentData` on the matched entity rather than
destroying it, matching the browser's semantic. A future cleanup slice
can remove stale accepted/expired proposal entities if memory pressure
becomes a concern; the expiration system already flags expired proposals
but does not destroy them.

---

## Current Readiness

Branch `claude/unity-ai-marriage-inbox-accept` is ready to merge. All
gates green, contract at revision 21, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-marriage-inbox-accept` into master.
2. Claim sub-slice 10 (marriage strategic profile) on a new branch
   `claude/unity-ai-marriage-strategic-profile`. Port
   `getAiMarriageStrategicProfile` (~2730-2857) so both proposal and
   accept can gate on faith-aware legitimacy repair vs. hardened refusal,
   population deficit, hostility, and succession signals via
   `AICovertOpsComponent.MarriageStrategicProfileWilling`.
3. Codex still owes the `codex/unity-ai-command-dispatch` and
   `codex/unity-fortification-siege` rebases. Master is now at revision
   21; Codex bumps to 22 and 23 on rebase.
