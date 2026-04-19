# Unity AI Strategic Layer Sub-Slice 16: Narrative Message Bridge

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-narrative-message-bridge`
**Lane:** ai-strategic-layer
**Contract Revision:** 32
**Master Base:** `40e80e03` (after Codex's fortification-siege sub-slice 5 wall-segment destruction landed at revision 31 on top of my sub-slice 15)

---

## Goal

Port the browser `pushMessage` surface (simulation.js, many call
sites) to Unity. Sub-slices 11, 12, 13, 14, and 15 all deferred
their narrative ceremonial lines because no AI-to-UI message
channel existed in Unity. Sub-slice 16 ports the minimal channel
and wires the first consumer (PactBreakSystem from sub-slice 15)
end-to-end so future slices can push messages with a one-line
call.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/NarrativeMessageComponents.cs`
  Three types:
  - `NarrativeMessageTone { Info, Good, Warn }`: matches the
    browser `pushMessage` tone parameter exactly.
  - `NarrativeMessageSingleton : IComponentData`: tag on the entity
    that owns the global buffer.
  - `NarrativeMessageElement : IBufferElementData`: carries Text
    (FixedString128Bytes), Tone, CreatedAtInWorldDays, Ttl. Shape
    mirrors the browser `{ id, text, tone, ttl }` record; Unity
    derives a stable id from buffer index + timestamp instead of
    storing one.

- `unity/Assets/_Bloodlines/Code/AI/NarrativeMessageBridge.cs`
  Static helper class:
  - `Push(em, text, tone, ttlSeconds = 7f)`: lazy-creates the
    singleton on first call; reads CreatedAtInWorldDays from
    DualClockComponent if seeded (stamps 0 otherwise); appends a
    new NarrativeMessageElement to the buffer.
  - `Count(em)`: returns the total message count (0 when no
    singleton exists).
  Designed for simple producer ergonomics: any system can push
  without needing to coordinate bootstrap or locate the singleton
  entity. The bridge lives in the AI lane because the first
  batch of producers is AI-side (PactBreakSystem here, plus future
  wire-ups for marriage/pact/lesser-house paths that deferred
  narrative pushes).

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNarrativeMessageBridgeSmokeValidation.cs`
  Six-phase smoke validator:
  - **PhaseLazySingleton**: Push on an empty world;
    singleton is created, buffer has 1 entry with matching Text
    and Tone.
  - **PhaseMultiplePushes**: three Pushes accumulate three
    buffer entries in append order with correct tone sequence
    (Info, Good, Warn).
  - **PhaseCreatedAtInWorldDays**: DualClock seeded at day 42;
    pushed entry carries CreatedAtInWorldDays = 42.
  - **PhasePactBreakEarlyMessage**: PactBreakSystem breaks a
    pact before MinimumExpiresAtInWorldDays; narrative buffer
    carries a message containing "early breach" with Warn tone
    because the breaker is "player".
  - **PhasePactBreakLateMessage**: PactBreakSystem breaks past
    the minimum-expiry threshold; message contains "Hostility
    resumes" with Info tone because the breaker is "enemy".
  - **PhaseNoMessageOnBadPact**: PactBreakSystem short-circuits
    on missing PactComponent; no message is pushed.
  Artifact: `../artifacts/unity-narrative-message-bridge-smoke.log`
  Marker: `BLOODLINES_NARRATIVE_MESSAGE_BRIDGE_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityNarrativeMessageBridgeSmokeValidation.ps1`
  Standard PowerShell runner.

### Shared-File Narrow Edits

- `unity/Assets/_Bloodlines/Code/AI/PactBreakSystem.cs` --
  additive: after the mechanical break effects, call
  `PushBreakMessage(em, breakerId, otherId, pact.MinimumExpiresAtInWorldDays)`.
  The helper computes `earlyBreak` from `currentInWorldDays <
  minimumExpiresAtInWorldDays`, builds the full
  FixedString128Bytes ceremonial line, routes tone via
  `breakerId == "player" ? Warn : Info`, and calls
  `NarrativeMessageBridge.Push`. No changes to the existing
  penalty/hostility code paths.
- `unity/Assembly-CSharp.csproj` -- added `NarrativeMessageComponents.cs`
  and `NarrativeMessageBridge.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added
  `BloodlinesNarrativeMessageBridgeSmokeValidation.cs`.

### Cross-Lane Reads + Mutations (no structural edits)

- Reads `DualClockComponent.InWorldDays` (dual-clock-match-progression
  lane, retired) for the CreatedAtInWorldDays stamp. Read-only.
- Creates the NarrativeMessageSingleton entity on first push.
  Entity and buffer are owned by the ai-strategic-layer lane.

---

## Browser Parity Notes

Browser `pushMessage` (simulation.js, called from every AI action
that produces player-visible text):

```js
state.messages.unshift({ id, text, tone, ttl });
```

Unity preserves the shape but collapses to a single entity with a
DynamicBuffer. Append order is the natural buffer order; the
browser uses `unshift` to put newest-first. Consumers in Unity
will read the buffer and decide presentation order.

- **Text.** FixedString128Bytes caps individual messages at ~128
  bytes. This is adequate for the browser's ceremonial lines which
  fit comfortably. Longer texts need truncation upstream.
- **Tone.** `NarrativeMessageTone { Info, Good, Warn }` maps
  directly to browser tone strings.
- **Ttl.** Stored but not yet consumed. A future slice can add a
  TTL-based eviction system (walk the buffer per-tick, remove
  entries whose CreatedAtInWorldDays + Ttl is past current).
- **CreatedAtInWorldDays.** Unity addition. The browser does not
  explicitly stamp messages with a creation day because the
  message queue is a short-lived rolling ledger. Unity uses
  in-world day stamping so consumer systems can render messages
  chronologically, not just by queue order.

**PactBreakSystem wire-up** mirrors simulation.js:5251 exactly:

- Text: `"<breaker> breaks the non-aggression pact with <other>.
  <suffix>"` where suffix is "The early breach damages legitimacy
  and conviction." when `currentInWorldDays <
  minimumExpiresAtInWorldDays`, else "Hostility resumes."
- Tone: `Warn` when the breaker is the player (matches browser's
  `factionId === "player" ? "warn" : "info"`).

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS (first run hit a transient
   bee_backend cache-rebuild error with a stale reference to a
   missing FortificationStructureResolutionSystem.cs in Unity's
   Library cache; retried once after 15-second wait and passed
   cleanly per the 10-gate retry protocol)
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision 31 current

Sub-slice 16 dedicated smoke
(`Invoke-BloodlinesUnityNarrativeMessageBridgeSmokeValidation.ps1`):
all 6 phases PASS.

Sub-slice 15 regression smoke
(`Invoke-BloodlinesUnityPactBreakSmokeValidation.ps1`): all 5
phases PASS. The narrative push added to PactBreakSystem does not
affect the mechanical penalty numbers or the idempotency gates.

---

## Key Design Notes

**Lazy singleton creation.** Callers do not need to coordinate
bootstrap. The first Push lazy-creates the singleton with its
buffer attached; subsequent Pushes find it via an EntityQuery.
This matches the browser's `state.messages` which is always
available as soon as state exists.

**Append-order buffer.** Unity uses append; browser uses unshift.
Consumers can choose presentation order by reading forward or
backward through the buffer. For a chronological HUD log, forward
iteration is the natural choice (oldest-first). For
newest-first UI like a notification toast feed, reverse
iteration.

**No consumer yet.** Sub-slice 16 ships the production surface
only. A future consumer (HUD log panel, notification toast, etc.)
can read the buffer and render messages according to its own UX
needs. A TTL-based eviction system can also land as a separate
slice without reshaping the producer API.

**Global, not per-faction.** Browser is global; Unity sub-slice 16
matches. A future multiplayer slice may introduce per-faction
buffers so each player sees only their own messages. The bridge
API's current shape (`Push(em, text, tone)`) can evolve into a
faction-aware helper (`Push(em, faction, text, tone)`) without
breaking the global case: the existing default is "global
narrative", new callers that want per-faction routing opt in.

**PactBreakSystem as first consumer.** The break path is the
highest-value first wire-up because it has a conditional suffix
(early-breach vs hostility-resumes) that exercises the tone and
text routing logic. Future wire-ups for marriage accept (sub-slice
11), marriage acceptance terms (sub-slice 12), lesser-house
founding (sub-slice 13), and pact proposal (sub-slice 14) each
become trivial one-line adds: compose the FixedString128Bytes,
pick the tone, call Push.

---

## Current Readiness

Branch `claude/unity-ai-narrative-message-bridge` is ready to
merge. All gates green, contract at revision 31, continuity files
updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Next candidates:
   - **Back-wire existing AI paths** (sub-slice 17): add
     `NarrativeMessageBridge.Push` calls to AIMarriageAcceptEffectsSystem
     (marriage accept ceremonial line), AIMarriageInboxAcceptSystem
     (marriage acceptance terms line with authority label),
     AILesserHousePromotionSystem (lesser-house founded line), and
     AIPactProposalExecutionSystem (pact entered line). Each is a
     small additive edit following the pattern from sub-slice 16.
   - **Dynasty operations infrastructure** (sub-slice 18):
     `DynastyOperationComponent` entity shape + the
     `DYNASTY_OPERATION_ACTIVE_LIMIT` gate. Unblocks missionary,
     holy war, divine right, rescue, and ransom execution sub-slices.
   - **Captive member state** (sub-slice 19): port the browser
     `captives` array on faction and the `CapturedMemberRecord`
     shape as a Unity buffer. Needed for captive rescue/ransom
     execution.
3. Codex fortification-siege lane: wall-segment destruction
   resolution remains the recommended next Codex slice.
