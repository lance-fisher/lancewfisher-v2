# Unity AI Strategic Layer Sub-Slice 18: Dynasty Operations Foundation

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-dynasty-operations-foundation`
**Lane:** ai-strategic-layer
**Contract Revision:** 35
**Master Base:** `dfec72f5` (after sub-slice 17 narrative back-wire landed at revision 33); rebased onto `a2f5e6cd` after Codex fortification-siege sub-slice 6 breach assault pressure landed at revision 34

---

## Goal

Sub-slices 10 through 17 ported the AI marriage loop end-to-end, the
lesser-house promotion execution, the non-aggression pact proposal and
break, the narrative message bridge, and the back-wires of the three AI
systems that produce browser-visible ceremonial lines. The AI loop is
now browser-accurate at parity for the social/diplomatic surface.

Sub-slice 18 turns attention to the covert and faith-driven dispatch
surface. Every browser dispatch site in `simulation.js` that starts a
missionary, a holy war, a counter-intelligence sweep, an espionage
web, an assassination cell, a sabotage run, a captive rescue, or a
captive ransom routes through one shared gate:

```js
const DYNASTY_OPERATION_ACTIVE_LIMIT = 6;  // simulation.js:17
...
const operations = getDynastyOperationsState(faction);
if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
  return { ok: false, reason: "Too many dynasty operations are already active." };
}
...
operations.active.unshift(operation);
operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);
```

Unity has no canonical operation surface yet. Every future covert-op
execution slice (missionary dispatch, holy war dispatch, divine right
declaration, captive rescue execution, captive ransom execution) needs
a canonical way to create and gate against. Sub-slice 18 ports that
foundation so the later slices plug in without reshaping.

No system ships in this slice. The foundation ships standalone; the
first consumer arrives in a later slice.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs`
  New `DynastyOperationKind` enum mirroring the browser dispatch
  surface: None, Missionary, HolyWar, DivineRight, CaptiveRescue,
  CaptiveRansom, LesserHousePromotion. None is the default so
  default-constructed components do not match any specific kind by
  accident. LesserHousePromotion is reserved for enum completeness
  since the sub-slice 13 promotion system currently runs without a
  dynasty-operation slot (browser `promoteMemberToLesserHouse` does
  not route through `getDynastyOperationsState`); a future slice may
  promote promotion to a gated operation and this enum value is
  ready.

  New `DynastyOperationComponent` struct carries one entity per active
  operation. Fields: `OperationId` (FixedString64Bytes),
  `SourceFactionId` (FixedString32Bytes), `OperationKind`
  (DynastyOperationKind), `StartedAtInWorldDays` (float, stamped from
  DualClock), `TargetFactionId` (FixedString32Bytes, default when
  N/A), `TargetMemberId` (FixedString64Bytes, default when N/A),
  `Active` (bool). `Active=false` retains the entity for audit while
  excluding it from capacity counts; a future resolution system can
  flip the flag when an operation completes without destroying the
  entity.

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationLimits.cs`
  Static helper class exposing:
  - `DYNASTY_OPERATION_ACTIVE_LIMIT = 6` (browser simulation.js:17).
  - `HasCapacity(em, factionId)` — counts active entities matching
    SourceFactionId via a full DynastyOperationComponent scan, returns
    true strictly below cap.
  - `CountActiveForFaction(em, factionId)` — exposes the count for
    smoke assertions and future consumers (intelligence reports,
    queue HUD).
  - `BeginOperation(em, operationId, sourceFactionId, kind,
    targetFactionId, targetMemberId)` — creates one
    `DynastyOperationComponent` entity with `Active=true` and
    `StartedAtInWorldDays` stamped from the DualClock singleton
    (0 if no clock is seeded). Returns the entity so callers can
    attach per-kind component data in the same tick.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyOperationsSmokeValidation.cs`
  Five-phase smoke validator:
  - **PhaseBeginOperation**: call `BeginOperation` for source "enemy"
    with kind Missionary and target "player"; verify one entity
    exists with correct OperationId, SourceFactionId, OperationKind,
    TargetFactionId, Active=true, and `StartedAtInWorldDays = 12`
    (DualClock seeded at day 12).
  - **PhaseMultipleOperationsUnderCap**: call `BeginOperation` four
    times for "enemy" with four different kinds; verify four entities
    exist and HasCapacity still returns true (4 < 6).
  - **PhaseCapReached**: seed `DYNASTY_OPERATION_ACTIVE_LIMIT` (6)
    active entities for "enemy"; verify HasCapacity returns false.
  - **PhasePerFactionCap**: seed cap-minus-one (5) active for "enemy"
    and one active for "player"; verify HasCapacity("player") returns
    true and CountActiveForFaction("enemy") returns 5 (cap is
    per-faction, not global).
  - **PhaseInactiveExcluded**: seed cap worth of entities with
    Active=false; verify HasCapacity returns true (inactive
    operations do not consume capacity).

  Artifact: `../artifacts/unity-dynasty-operations-smoke.log`
  Marker: `BLOODLINES_DYNASTY_OPERATIONS_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityDynastyOperationsSmokeValidation.ps1`
  Standard PowerShell runner mirroring the narrative-bridge wrapper.
  Uses the canonical `D:\ProjectsHome\Bloodlines` project path.

### Shared-File Narrow Edits (lane-shared files)

- `unity/Assembly-CSharp.csproj` — added
  `DynastyOperationComponent.cs` and `DynastyOperationLimits.cs`.
- `unity/Assembly-CSharp-Editor.csproj` — added
  `BloodlinesDynastyOperationsSmokeValidation.cs`.

Initial build on the `dfec72f5` base had to remove two stale csproj
entries that had leaked from Codex's in-flight fortification-siege
sub-slice 6 work (`BreachAssaultPressureSystem.cs` and
`BloodlinesBreachAssaultPressureSmokeValidation.cs`); those files
did not exist on the `dfec72f5` branch, and per HANDOFF.md `csproj`
files are not tracked by git and auto-regenerate. After rebasing
onto `a2f5e6cd` (which includes Codex's merged fortification
sub-slice 6), those files are present on disk and Unity's csproj
regeneration restores the entries; the rebased build runs cleanly.

### Cross-Lane Reads (no writes)

- Reads `DualClockComponent.InWorldDays` on the dual-clock singleton
  to stamp `StartedAtInWorldDays`. No mutations.

---

## Browser Parity Notes

**Cap constant** (simulation.js:17):

```js
const DYNASTY_OPERATION_ACTIVE_LIMIT = 6;
```

Unity mirrors exactly as `DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT`.

**Dispatch gate** (repeated seven times across simulation.js, example
from `startMissionaryOperation` at ~10530):

```js
const operations = getDynastyOperationsState(faction);
if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
  return { ok: false, reason: "Too many dynasty operations are already active." };
}
```

Unity port (call-site pattern for later slices):

```csharp
if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;
var entity = DynastyOperationLimits.BeginOperation(em, ...);
```

**Operation shape** (example, `startMissionaryOperation` ~10536-10554):

```js
const operation = {
  id: createEntityId(state, "dynastyOperation"),
  type: "missionary",
  sourceFactionId,
  targetFactionId,
  sourceFaithId: terms.sourceFaithId,
  sourceFaithName: terms.sourceFaithName,
  startedAt: state.meta.elapsed,
  resolveAt: state.meta.elapsed + terms.duration,
  operatorId: terms.operatorId,
  operatorTitle: terms.operatorTitle,
  exposureGain: terms.exposureGain,
  intensityErosion: terms.intensityErosion,
  loyaltyPressure: terms.loyaltyPressure,
  successScore: terms.successScore,
  projectedChance: terms.projectedChance,
  intensityCost: terms.intensityCost,
  escrowCost: terms.cost,
};
```

Unity port (sub-slice 18 fields only; per-kind fields belong on
future per-kind component structs):

```csharp
struct DynastyOperationComponent : IComponentData {
    FixedString64Bytes OperationId;         // id
    FixedString32Bytes SourceFactionId;     // sourceFactionId
    DynastyOperationKind OperationKind;     // type (enum)
    float StartedAtInWorldDays;             // startedAt (in-world days)
    FixedString32Bytes TargetFactionId;     // targetFactionId (default for N/A)
    FixedString64Bytes TargetMemberId;      // memberId for assassination/captive ops
    bool Active;                            // implicit in browser (trimmed when done)
}
```

Per-kind resolution fields (resolveAt, operatorId, successScore,
sourceFaithId, exposureGain, etc.) are intentionally deferred. Each
future dispatch slice attaches its own per-kind component struct to
the entity created here.

**Browser silent-trim departure.** After unshift, the browser trims
the array to the cap:

```js
operations.active.unshift(operation);
operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);
```

Unity does not replicate the silent trim. An entity-per-operation
model cannot drop entries without orphaning per-kind component data,
and a silent drop at the producer would mask over-cap bugs rather
than surfacing them. Instead, the gate is strict at the call site:
callers must check `HasCapacity` before `BeginOperation`. Callers
that skip the check may exceed the cap; that is intentionally left as
a caller contract so over-cap creation shows up as a bug.

---

## Verification Results

All 10 canonical validation gates green, serialized per Unity's
project lock:

1. `dotnet build Assembly-CSharp.csproj -nologo` — 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` — 0 errors PASS
3. Bootstrap runtime smoke — PASS
4. Combat smoke — all phases PASS
5. Canonical scene shells (Bootstrap + Gameplay) — both PASS
6. Fortification smoke — PASS
7. Siege smoke — PASS
8. `node tests/data-validation.mjs` — PASS
9. `node tests/runtime-bridge.mjs` — PASS
10. Contract staleness check — revision 35 current

Sub-slice 18 dedicated smoke
(`Invoke-BloodlinesUnityDynastyOperationsSmokeValidation.ps1`):
all 5 phases PASS.

Phase output:

```
BLOODLINES_DYNASTY_OPERATIONS_SMOKE PASS
PhaseBeginOperation PASS: entity created with correct fields, HasCapacity=true at count 1
PhaseMultipleOperationsUnderCap PASS: 4 entities created, HasCapacity=true (4 < 6)
PhaseCapReached PASS: 6 entities seeded, HasCapacity=false at cap
PhasePerFactionCap PASS: enemy=5, player=1, both below cap, per-faction scoping holds
PhaseInactiveExcluded PASS: cap-worth of inactive entities excluded, HasCapacity=true
```

No sub-slice 8-17 regressions expected because sub-slice 18 is pure
net-new surface; no existing system was modified.

---

## Key Design Notes

**No consumer this slice.** The foundation ships without a system
that produces operations. This keeps the slice focused on the shared
surface and lets each later dispatch slice (missionary, holy war,
divine right, captive rescue, captive ransom) land as its own
reviewable change. The alternative would be to bundle one dispatch
site with the foundation, which couples the foundation's shape to
that site's specifics and makes later slices harder to review.

**Enum completeness over strict minimalism.** DynastyOperationKind
includes LesserHousePromotion even though the sub-slice 13 promotion
system does not currently gate through `DYNASTY_OPERATION_ACTIVE_LIMIT`
in the browser. Including it avoids re-extending the enum when a
future slice decides to gate promotion as a dynasty operation. The
alternative (a stricter minimal enum) would require changing the
type to add a value later, which is a surface change visible to
every serializer, debug tool, and downstream query.

**Active=false preserved for audit.** Flipping Active=false rather
than destroying the entity retains operation records for future
intelligence-report consumers, retrospective scoring, and history
queries. The HasCapacity gate excludes inactive entities explicitly
so audit retention does not block new operation creation. Future
resolution systems mark Active=false at completion; a separate TTL
or retention policy (if one lands later) can evict truly ancient
records.

**Per-faction scan vs. per-faction index.** HasCapacity scans all
DynastyOperationComponent entities and filters by SourceFactionId.
Per-faction indexing would be faster but is deferred until profiling
shows it matters: a typical match holds a small count of operations
(cap 6 per faction, small number of factions), so linear scan over
a few dozen entities is fine. If a future profile shows this gate
in hot paths, a per-faction pinned-entity index or a SharedComponent
partition is the upgrade path.

**Browser trim departure.** Browser `operations.active.slice(0,
DYNASTY_OPERATION_ACTIVE_LIMIT)` silently drops entries past the cap
after unshift. Unity does not silently trim because:
(a) entity-per-operation means silent drop orphans per-kind data,
(b) silent drop at the producer masks over-cap bugs. The Unity
contract is explicit: callers must check `HasCapacity` first.
`BeginOperation` creates unconditionally. Over-cap creation at the
call site is a bug, not a quiet no-op.

**Stale csproj leaks.** Per HANDOFF.md the csproj files are not
tracked by git and leak across branch switches. This branch had to
remove `BreachAssaultPressureSystem.cs` and
`BloodlinesBreachAssaultPressureSmokeValidation.cs` entries left over
from Codex's in-flight fortification-siege sub-slice 6 work; those
files do not exist on this branch. The removal is a csproj-only
change; Unity auto-regenerates the entries on Codex's branch when
those files are present.

---

## Current Readiness

Branch `claude/unity-ai-dynasty-operations-foundation` is ready to
merge. All gates green, contract at revision 34, continuity files
updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Next candidates:
   - **Captive member state** (sub-slice 19): port the browser
     `captives` array on faction and the `CapturedMemberRecord` shape
     as a Unity buffer on the faction entity. Together with
     sub-slice 18 this unblocks the captive rescue execution and
     captive ransom execution slices.
   - **Narrative TTL eviction system**: walk the
     `NarrativeMessageElement` buffer each tick and remove entries
     whose `CreatedAtInWorldDays + Ttl` is past current. Small,
     self-contained. Keeps the buffer bounded without a UI consumer
     yet.
   - **Missionary execution** (the first consumer of the dynasty
     operations foundation): port `startMissionaryOperation` at
     simulation.js:10523 with its per-kind resolveAt, operatorId,
     sourceFaithId, exposureGain, and loyaltyPressure fields on a
     new `DynastyOperationMissionaryComponent` struct attached to
     the entity created by `BeginOperation`.
3. Codex fortification-siege lane: breach assault pressure
   (sub-slice 6) remains in flight on
   `codex/unity-fortification-breach-assault-pressure` locally.
   Master-base contract revision target bumps to 35 when that lands.
