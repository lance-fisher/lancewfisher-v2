# Unity AI Strategic Layer Sub-Slice 8: Marriage Proposal Execution

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-marriage-proposal-execution`
**Lane:** ai-strategic-layer
**Contract Revision:** 20

---

## Goal

Port the browser `tryAiMarriageProposal` execution body (ai.js ~2897-2944)
to Unity DOTS/ECS. Sub-slice 6 writes `AICovertOpsComponent.LastFiredOp =
CovertOpKind.MarriageProposal` when the covert-ops dispatch fires but does
not create the proposal entity. This slice consumes that signal, runs the
four browser abort gates (already married, already pending, no source
candidate, no target candidate), picks a non-head active/ruling member on
each side, and creates a `MarriageProposalComponent` entity that the
existing `MarriageProposalExpirationSystem` ages out at 30 in-world days.

Proposal execution is now end-to-end: sub-slice 6 decides when to fire,
sub-slice 8 creates the entity, sub-slice 9 (next) will accept on the
inbox side.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageProposalExecutionSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateAfter(typeof(AICovertOpsSystem))]`. Per update:
  1. Pulls `InWorldDays` from the `DualClockComponent` singleton.
  2. Iterates every faction entity with both `FactionComponent` and
     `AICovertOpsComponent`. Skips any whose `LastFiredOp` is not
     `MarriageProposal`.
  3. Runs the browser abort chain in order: already-married between source
     and target factions (`MarriageComponent.Dissolved == false` match
     either direction); already-pending from source to target
     (`MarriageProposalComponent.Status == Pending` match); source-member
     candidate from source faction's `DynastyMemberRef` buffer (first
     non-head, active/ruling); target-member candidate from target
     faction's `DynastyMemberRef` buffer (first non-head, active/ruling).
  4. If all gates pass, creates a `MarriageProposalComponent` entity with
     `ProposalId = "marriage-proposal-<src>-to-<tgt>-d<days>"`,
     `ProposedAtInWorldDays = now`,
     `ExpiresAtInWorldDays = now + 30` (pulls `ExpirationInWorldDays`
     directly from `MarriageProposalExpirationSystem` so the window can
     never drift out of sync).
  5. Always clears `LastFiredOp` to `CovertOpKind.None` after processing,
     regardless of whether a proposal was created. One dispatch produces
     at most one execution attempt, matching the browser single-fire
     semantic.
  Target faction is hardcoded to `"player"` to mirror the browser
  convention where `tryAiMarriageProposal` only proposes player-ward.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageProposalExecutionSmokeValidation.cs`
  Five-phase smoke validator. Each phase spins up an isolated World with
  `SimulationSystemGroup`, seeds `MatchProgressionComponent`,
  `DualClockComponent` at 10 in-world days, player faction with canonical
  dynasty via `DynastyBootstrap.AttachDynasty`, and an enemy faction with
  `AICovertOpsComponent` seeded with the specific `LastFiredOp` under
  test.
  - Phase 1: clean state + `LastFiredOp = MarriageProposal` -> exactly one
    proposal entity created from enemy to player; `LastFiredOp` cleared to
    `None`.
  - Phase 2: prior `MarriageComponent` seeded with enemy as head and
    player as spouse -> no proposal created; `LastFiredOp` cleared.
  - Phase 3: prior `MarriageProposalComponent` with `Status = Pending`
    from enemy to player -> no second proposal (count stays 1);
    `LastFiredOp` cleared.
  - Phase 4: target faction seeded without dynasty (withMembers: false)
    -> no proposal created; `LastFiredOp` cleared.
  - Phase 5: `LastFiredOp = None` (no dispatch signal) -> no proposal
    created; `LastFiredOp` stays `None`. Proves the system is a strict
    signal consumer and does not fire spontaneously.
  Artifact: `../artifacts/unity-ai-marriage-proposal-execution-smoke.log`
  Marker: `BLOODLINES_AI_MARRIAGE_PROPOSAL_EXECUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIMarriageProposalExecutionSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIMarriageProposalExecutionSmokeValidation.RunBatchAIMarriageProposalExecutionSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added `<Compile Include="..." />` entry
  for `AIMarriageProposalExecutionSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added `<Compile Include="..." />`
  entry for `BloodlinesAIMarriageProposalExecutionSmokeValidation.cs`.

### Cross-Lane Reads (no writes to another lane's code)

- Reads `MarriageComponent` and `MarriageProposalComponent` /
  `MarriageProposalStatus` from `Bloodlines.Dynasties` (tier2-batch-dynasty-systems
  lane, retired). Creates new `MarriageProposalComponent` entities; does
  not modify the struct layout or any existing dynasty system code.
- Reads `DynastyMemberComponent`, `DynastyRole`, `DynastyMemberStatus`,
  and `DynastyMemberRef` from `Bloodlines.Components` (dynasty-core lane,
  retired).
- Reads `DualClockComponent.InWorldDays` from `Bloodlines.GameTime`
  (dual-clock-match-progression lane, retired).

All cross-lane interactions are read-only except for the proposal entity
creation, which is a write to a component type owned by the retired
tier2-batch-dynasty-systems lane. The browser's `proposeMarriage` in
`simulation.js` uses exactly the same struct shape, and the write is
additive: no modification to existing proposals, no schema change, no
touch of the dynasty systems themselves.

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors (112 unrelated CS0649 warnings) PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision=19 current (bumped to 20 post-gate as part of handoff)

AI marriage proposal execution smoke: all 5 phases PASS
- Phase 1 PASS: proposal created, LastFiredOp cleared to None
- Phase 2 PASS: blocked by prior marriage, LastFiredOp cleared
- Phase 3 PASS: blocked by pending proposal, no duplicate, LastFiredOp cleared
- Phase 4 PASS: no target dynasty members, no proposal, LastFiredOp cleared
- Phase 5 PASS: no dispatch signal, no proposal, LastFiredOp stays None

---

## Key Design Notes

**One-shot dispatch consumption.** The browser's `updateEnemyAi` calls
`tryAiMarriageProposal` exactly once per timer fire. In Unity, sub-slice 6
writes `LastFiredOp` but doesn't clear it; this slice clears it. The clear
is unconditional, matching the browser's "skip or proposed then reset the
timer and return" semantic. Without the clear, subsequent frames would
re-run the proposal gates every tick until a different op fired.

**Partial port of `tryAiMarriageProposal`.** The browser version also
calls `getAiMarriageStrategicProfile` to check faith hostility,
population deficit, legitimacy distress, succession crises, and
faith-harmony blocks. Sub-slice 6 already encodes a
`MarriageStrategicProfileWilling` context flag path in
`AICovertOpsComponent` that can gate the dispatch on the covert-ops side.
This slice does not duplicate the strategic-profile evaluation; the
abort gates here are structural (already-married, already-pending, no
candidate) rather than strategic. Strategic gating is therefore
responsibility-split: sub-slice 6 gates the dispatch, sub-slice 8 gates
the execution structurally. Additional strategic ports can layer onto
sub-slice 6 in a future slice without touching this system.

**Candidate selection order is deterministic.** The member buffer is
iterated in insertion order (`DynastyBootstrap` adds canonical members in
template order). This matches the browser's
`enemy.dynasty.members.find(...)` which walks the array in declaration
order. Both sides land on the first non-head, active/ruling member.

**No writes through structural change inside a query iteration.** The
system uses `EntityManager.CreateEntityQuery` + `ToEntityArray(Allocator.Temp)`
to snapshot faction entities, then iterates the snapshot and mutates via
`SetComponentData` / `CreateEntity` outside the query. This is the same
pattern as `MarriageProposalExpirationSystem`.

**Expiration window not hardcoded.** `ProposalExpirationInWorldDays`
pulls directly from `MarriageProposalExpirationSystem.ExpirationInWorldDays`.
If the expiration window canon changes, both systems update together.

**Target faction is `"player"` for now.** The browser's
`tryAiMarriageProposal` always proposes to player. In a future
multi-faction extension, target selection can become
`AICovertOpsComponent.MarriageProposalTargetFactionId` or similar.
That extension can layer on without reshaping the current system.

---

## Current Readiness

Branch `claude/unity-ai-marriage-proposal-execution` is ready to merge.
All gates green, contract at revision 20, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-marriage-proposal-execution` into master.
2. Claim sub-slice 9 (marriage inbox accept execution) on a new branch
   `claude/unity-ai-marriage-inbox-accept`. Browser reference:
   `src/game/core/ai.js` `tryAiAcceptIncomingMarriage` (~2880-2895);
   simulation sink `acceptMarriage` (~7388). That system consumes
   `LastFiredOp == MarriageInboxAccept`, finds a pending incoming proposal
   from the player, flips it to Accepted, creates a primary
   `MarriageComponent` entity.
3. Codex still owes the `codex/unity-ai-command-dispatch` rebase onto
   master (now at revision 20 after this merge) and the
   `codex/unity-fortification-siege` rebase. Both are Codex-owned; Codex
   handles them.
