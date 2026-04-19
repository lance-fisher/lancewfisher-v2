# Unity AI Strategic Layer Bundle 1: Captive State and Missionary Execution

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-captive-state-and-missionary-execution`
**Lane:** ai-strategic-layer
**Contract Revision:** 37
**Master Base:** `ef58ec4f` (after Codex fortification-siege sub-slice 7 breach legibility readout landed at revision 36)

---

## Goal

Bundle 1 of the AI mechanical campaign per the 2026-04-19 roadmap
decision. Two logical sub-slices ship as one commit, one merge, and
one contract revision bump.

Sub-slice 19 ports the canonical browser captive-member state surface
as a Unity buffer on the captor faction entity. No consumer system
ships in this slice; the data shape lands in advance of the
captive rescue (sub-slice 23) and captive ransom (sub-slice 24)
dispatch slices that will consume it.

Sub-slice 20 ports the missionary execution path as the first
production consumer of the sub-slice 18 dynasty operations
foundation. The system gates on the same conditions the browser
`getMissionaryTerms` enforces, deducts cost on success, computes the
per-kind operation fields, calls
`DynastyOperationLimits.BeginOperation`, attaches a
`DynastyOperationMissionaryComponent` carrying the per-kind data, and
pushes the canonical narrative line. Per-kind resolution (applying
exposure / intensity erosion / loyalty pressure to the target faction
when `ResolveAtInWorldDays` elapses) is intentionally deferred to a
future slice.

---

## Work Completed

### Sub-Slice 19: Captive Member State

#### New files

- `unity/Assets/_Bloodlines/Code/AI/CapturedMemberElement.cs`
  New `CapturedMemberStatus` enum (Held / RansomOffered / Released /
  Executed) carries explicit captive lifecycle state because Unity
  retains the buffer entry for audit even after release while the
  browser splices removed entries out of the array.

  New `CapturedMemberElement` IBufferElementData with
  `[InternalBufferCapacity(8)]` attaches to the captor faction entity
  and carries:
  - `MemberId` (FixedString64Bytes) — browser member.id
  - `MemberTitle` (FixedString64Bytes) — browser member.title
  - `OriginFactionId` (FixedString32Bytes) — browser
    victimFactionId from the captor's perspective; the captive's
    home faction
  - `CapturedAtInWorldDays` (float) — DualClock InWorldDays at
    capture (browser uses elapsed simulation seconds on the same
    clock surface)
  - `RansomCost` (float) — default 0; sub-slice 24 ransom dispatch
    sets the canonical value
  - `Status` (CapturedMemberStatus) — defaults to Held for newly
    captured members

- `unity/Assets/_Bloodlines/Code/AI/CapturedMemberHelpers.cs`
  Static helper class:
  - `CaptureMember(em, captorFactionId, memberId, memberTitle,
    originFactionId)` — finds the captor faction entity by id,
    lazy-creates the CapturedMemberElement buffer if missing,
    appends one Held entry with the DualClock timestamp, returns
    the new entry's buffer index. Returns -1 when the captor
    faction is not found, mirroring browser's early-return when
    captorFaction.dynasty is missing.
  - `TryGetCaptive(em, captorFactionId, memberId, out element,
    out index)` — walks the buffer for a member-id match, returns
    the entry plus its index. Returns false when the captor entity
    or buffer or matching entry is absent.
  - `ReleaseCaptive(em, captorFactionId, memberId, newStatus)` —
    mutates status in place. Buffer length unchanged (audit
    retention).

#### Browser parity notes (sub-slice 19)

Browser create site (simulation.js:4429-4441):
```js
const captive = {
  id: `captive-${member.id}-${Math.floor(elapsed * 10)}`,
  memberId: member.id,
  sourceFactionId: victimFactionId,
  sourceFactionName: getFactionDisplayName(state, victimFactionId),
  title: member.title,
  roleId: member.roleId,
  pathId: member.pathId,
  renown: member.renown,
  age: member.age,
  capturedAt: elapsed,
  reason,
};
captorFaction.dynasty.captives = [captive, ...captorFaction.dynasty.captives].slice(0, CAPTIVE_LEDGER_LIMIT);
```

Unity port preserves the fields the rescue/ransom dispatch slices
will need (memberId, memberTitle, originFactionId, capturedAt). The
extended browser fields (roleId, pathId, renown, age,
sourceFactionName, reason) are intentionally deferred to the
matching dispatch slice that needs them.

`CAPTIVE_LEDGER_LIMIT = 16` (simulation.js:14) enforces a 16-entry
cap on the browser side via `slice(0, CAPTIVE_LEDGER_LIMIT)`. Unity
does not silently trim because `[InternalBufferCapacity(8)]` is the
inline storage hint, not a hard cap; ECS buffers grow to the heap
on demand. A future retention/eviction pass can enforce a hard cap
when one is needed, similar to how DynastyOperationLimits enforces
the operations cap at the call site.

### Sub-Slice 20: Missionary Execution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationMissionaryComponent.cs`
  New `DynastyOperationEscrowCost` struct mirrors
  `ResourceStockpileComponent` (Gold / Food / Water / Wood / Stone /
  Iron / Influence) so future per-kind component slices can carry
  the same cost surface without reshape. Only the relevant resource
  is non-zero per operation; missionary work sets only Influence.

  New `DynastyOperationMissionaryComponent` per-kind component
  attached to the entity created by
  `DynastyOperationLimits.BeginOperation`:
  - `ResolveAtInWorldDays` (float) — current + 32f
  - `OperatorMemberId` (FixedString64Bytes) — resolved from the
    source faction's DynastyMemberRef roster
  - `OperatorTitle` (FixedString64Bytes)
  - `SourceFaithId` (FixedString64Bytes) — CovenantId enum mapped
    to canonical string token
  - `ExposureGain` (float)
  - `IntensityErosion` (float)
  - `LoyaltyPressure` (float)
  - `SuccessScore` (float)
  - `ProjectedChance` (float)
  - `IntensityCost` (float) — 12f
  - `EscrowCost` (DynastyOperationEscrowCost) — Influence = 14f

- `unity/Assets/_Bloodlines/Code/AI/AIMissionaryExecutionSystem.cs`
  `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]`
  ISystem. Per faction with `AICovertOpsComponent.LastFiredOp ==
  CovertOpKind.Missionary`:
  - Gates on getMissionaryTerms parity (source != target, source
    has committed faith, target faction exists, target has at
    least one ControlPointComponent owned by player, source has a
    faith operator on the dynasty roster, source Influence >= 14,
    source Intensity >= 12) plus DynastyOperationLimits.HasCapacity.
  - On success: deducts 14 Influence and 12 Intensity, computes
    per-kind terms from the simplified browser parity formula,
    calls BeginOperation with DynastyOperationKind.Missionary,
    attaches the per-kind component, pushes the narrative message.
  - Always clears LastFiredOp to None regardless of outcome
    (one-shot pattern shared with sub-slices 8/9/12/13/14).

#### Browser parity notes (sub-slice 20)

Browser dispatch hook (ai.js:2469-2496):
```js
if (enemy.ai.missionaryTimer <= 0) {
  ...
  const canPressureFaith = Boolean(enemyFaithId) &&
    enemyFaithId !== playerFaithId &&
    (enemy.faith?.intensity ?? 0) >= 12;
  if (canPressureFaith && !liveMissionary) {
    const result = startMissionaryOperation(state, "enemy", "player");
    ...
  }
}
```

Browser execution body (simulation.js:10523-10563): full
startMissionaryOperation including terms gate, cost deduction,
operation construction, dispatch capacity check, and pushMessage.

Unity port maps tone routing identically: source==player -> Info,
target==player -> Warn, else Info. The browser uses "warn" when the
player is the missionary target so the player UI sees hostile
incoming missionary work; Unity matches.

Browser per-kind formula (simulation.js:10395-10405):
```js
const offenseScore = (operator.renown ?? 10) + sourceIntensity * 0.65 + (faction.faith.doctrinePath === "dark" ? 8 : 4);
const defenseScore = (targetOperator?.renown ?? 0) * 0.6 + targetIntensity * 0.55 + (wardProfile.id !== "none" ? 10 : 0);
const successScore = offenseScore - defenseScore;
const projectedChance = Math.max(0.1, Math.min(0.9, 0.5 + successScore / 100));
const exposureGain = Math.max(12, Math.round(14 + Math.max(0, successScore) * 0.22));
const intensityErosion = ... Math.max(4, Math.round(5 + Math.max(0, sourceIntensity - targetIntensity) / 12) + (faction.faith.doctrinePath === "dark" ? 2 : 0)) : 0;
const loyaltyPressure = ... (faction.faith.doctrinePath === "dark" ? 4 : 2) : 1;
```

Unity port omits the target-operator renown contribution and the
ward-profile bonus from the defense score (browser
`getFortificationFaithWardProfile` lookup not yet ported). The result
is a slight power adjustment vs browser; per-kind effects are
deferred to a future resolution slice that can revisit the formula
when the ward-profile readout lands.

**Browser duration translation.** `MISSIONARY_DURATION_SECONDS = 32`
at simulation.js:9770 is in real seconds. Unity uses the in-world
day clock for `ResolveAtInWorldDays`. The port treats the browser
numeric value as the canonical duration on the in-world timeline
directly (`MissionaryDurationInWorldDays = 32f`) rather than
translating through `DualClock.DaysPerRealSecond`. A future
resolution slice can re-translate at runtime if the canonical clock
rate ever shifts; the data shape stays the same.

**Faith operator role gate.** Browser `getFaithOperatorMember`
(simulation.js:690) accepts `["ideological_leader", "sorcerer",
"head_of_bloodline", "diplomat"]`. Unity has no Sorcerer role;
Spymaster stands in as the closest covert-faith equivalent for
parity pending a canonical Sorcerer DynastyRole addition. Status
gate excludes Fallen and Captured members.

**Per-kind resolution deferred.** The system creates the
DynastyOperationComponent entity with Active=true and attaches the
DynastyOperationMissionaryComponent with the per-kind fields. A
future resolution slice will walk expired entries at
`ResolveAtInWorldDays`, apply `ExposureGain` to target's
FaithExposureElement buffer, apply `IntensityErosion` to target's
FaithStateComponent.Intensity, apply `LoyaltyPressure` to the
lowest-loyalty ControlPointComponent owned by the target, and flip
Active=false. That's a separate slice.

### Shared-File Narrow Edits (lane-shared files)

- `unity/Assembly-CSharp.csproj` — added
  `CapturedMemberElement.cs`, `CapturedMemberHelpers.cs`,
  `DynastyOperationMissionaryComponent.cs`,
  `AIMissionaryExecutionSystem.cs`. Also added Codex sub-slice 7
  entry (`BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`)
  that was missing from the gitignored csproj after branching from
  origin/master ahead of Unity's csproj regeneration.
- `unity/Assembly-CSharp-Editor.csproj` — added
  `BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation.cs`.
  Also added Codex sub-slice 7 entry
  (`BloodlinesBreachLegibilityReadoutSmokeValidation.cs`) for the
  same csproj-regeneration reason.

### Cross-Lane Reads (no writes)

- Reads `FactionComponent.FactionId` to find captor / source / target
  faction entities by id. No mutations to FactionComponent.
- Reads `FaithStateComponent.SelectedFaith`,
  `FaithStateComponent.DoctrinePath`, and
  `FaithStateComponent.Intensity` on the source faction entity
  (sub-slice 20). Mutates Intensity on the source via in-place
  SetComponentData when the missionary dispatch fires.
- Reads `FaithStateComponent.SelectedFaith` and Intensity on the
  target faction entity for the per-kind formula. No mutations.
- Reads `ResourceStockpileComponent.Influence` on the source faction
  entity (sub-slice 20). Mutates Influence on the source via in-place
  SetComponentData when the missionary dispatch fires.
- Reads `ControlPointComponent.OwnerFactionId` to gate on target
  having at least one territory. No mutations.
- Reads `DynastyMemberComponent` (Role, Status, Renown, MemberId,
  Title) and `DynastyMemberRef` buffer to resolve faith operator on
  the source dynasty roster. No mutations.
- Reads `DualClockComponent.InWorldDays` to stamp
  CapturedAtInWorldDays and ResolveAtInWorldDays. No mutations.

### New dedicated smoke validator

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation.cs`
  Eight-phase validator covering both sub-slices:
  - **PhaseCaptureMember**: capture a single member; verify the
    buffer gets one Held entry with correct fields and a DualClock
    timestamp.
  - **PhaseMultipleCaptivesPerFaction**: capture 3 members from 3
    different origin factions; verify buffer length=3 and per-member
    lookup via TryGetCaptive.
  - **PhaseReleaseCaptive**: capture, then ReleaseCaptive(Released);
    verify status flipped and entry retained on the buffer (audit).
  - **PhaseMissionaryDispatchSuccess**: enemy with faith committed,
    intensity 30, influence 50, target=player has one
    ControlPointComponent and a different committed faith;
    LastFiredOp=Missionary; tick world; verify dispatch cleared,
    one Missionary DynastyOperationComponent created with player
    target, DynastyOperationMissionaryComponent attached with
    ResolveAt=72 (40+32), OperatorMemberId resolved, IntensityCost=12,
    EscrowCost.Influence=14, source Influence 50->36, source
    Intensity 30->18, narrative pushed (+1).
  - **PhaseMissionaryCapBlocks**: 6 active missionary operations
    pre-seeded for enemy; dispatch fails (HasCapacity=false);
    resources untouched; dispatch still cleared (one-shot).
  - **PhaseMissionaryNoFaithBlocks**: enemy CovenantId.None blocks;
    no deductions; dispatch cleared.
  - **PhaseMissionaryInsufficientIntensityBlocks**: intensity 8 <
    12 blocks; no deductions.
  - **PhaseMissionaryInsufficientResourcesBlocks**: influence 10 <
    14 blocks; no deductions.

  Artifact: `../artifacts/unity-captive-state-and-missionary-execution-smoke.log`
  Marker: `BLOODLINES_CAPTIVE_STATE_AND_MISSIONARY_EXECUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityCaptiveStateAndMissionaryExecutionSmokeValidation.ps1`
  Standard PowerShell runner mirroring the dynasty-operations wrapper.
  Uses the canonical `D:\ProjectsHome\Bloodlines` project path.

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
10. Contract staleness check — revision 37 current

Bundle 1 dedicated smoke
(`Invoke-BloodlinesUnityCaptiveStateAndMissionaryExecutionSmokeValidation.ps1`):
all 8 phases PASS.

No sub-slice 8-18 regression expected because Bundle 1 is pure
net-new surface. Sub-slice 18 helpers (DynastyOperationLimits.HasCapacity
and BeginOperation) gain their first production caller without
shape change. Sub-slice 16 NarrativeMessageBridge gains a fourth
production caller (after marriage accept, lesser-house promotion,
pact entry from sub-slice 17) without shape change.

---

## Key Design Notes

**Bundle structure.** Two logical sub-slices ship as one commit
because sub-slice 19 is a pure data-shape port with no consumer in
Bundle 1, and sub-slice 20 is the first dispatch consumer of the
sub-slice 18 foundation. Bundling reduces ceremony (one merge, one
contract bump) without coupling the captive surface to the
missionary execution path: each sub-slice's files are independent and
have separate smoke phases. Sub-slice 23 captive rescue and
sub-slice 24 captive ransom can land later as their own slices and
will consume the captive buffer without revisiting it.

**Captive status enum.** Browser tracks captive status implicitly
through array membership: a captive in `faction.dynasty.captives` is
held; a rescued or ransomed captive is spliced out of the array. Unity
adds an explicit `CapturedMemberStatus` enum (Held, RansomOffered,
Released, Executed) so audit-retained entries can stay on the buffer
without being mistaken for currently-held captives. Future intel
report consumers will read the full buffer; the status enum lets them
distinguish active captives from historical records.

**Captor-faction lookup pattern.** CapturedMemberHelpers walks the
FactionComponent query and matches by FactionId rather than taking
the captor entity directly. This mirrors the browser's
`getFaction(state, factionId)` lookup pattern and lets future
multiplayer / netcode code call CaptureMember without coordinating
on entity references. Performance-wise, a small number of factions
per match makes the linear scan acceptable; if a future profile
shows this in hot paths a faction-id index is the upgrade path.

**Missionary one-shot pattern.** AIMissionaryExecutionSystem clears
LastFiredOp to None unconditionally after processing (regardless of
whether the dispatch fired). This matches the one-shot pattern from
sub-slices 8/9/12/13/14: one tick of dispatch produces at most one
execution attempt. Without the unconditional clear, a dispatch that
fails its terms gates would re-fire every tick until the next
AICovertOpsSystem reset.

**Per-kind component pattern.** DynastyOperationMissionaryComponent
is added to the same entity DynastyOperationLimits.BeginOperation
creates. This keeps per-operation data co-located on a single entity
while letting each dispatch slice attach its own per-kind shape
without reshaping DynastyOperationComponent. Future holy war,
divine right, captive rescue, captive ransom, and counter-intel
slices will follow the same pattern (DynastyOperationHolyWarComponent,
DynastyOperationDivineRightComponent, etc.).

**Defense score simplification.** Unity's per-kind formula omits the
target-operator renown contribution (browser `targetOperator?.renown
* 0.6`) and the ward-profile bonus (browser `+10` when wardProfile.id
!== "none") from the defense score. The result is a slight power
adjustment vs browser parity. The omissions are intentional for this
slice: the target-operator gate has no Unity equivalent yet (faith
operator selection uses a single source-side helper); the ward
profile readout requires `getFortificationFaithWardProfile` parity
which is on the fortification lane's roadmap. A future resolution
slice that ports per-kind effects can revisit the formula when both
inputs land.

**Resource cost struct shape.** DynastyOperationEscrowCost mirrors
all seven ResourceStockpileComponent fields even though missionary
cost only sets Influence. This lets future per-kind components
(holy war's gold cost, captive ransom's gold cost) carry the same
shape without changing the struct definition. The cost is one
surface for all dispatch slices.

**Browser duration unit reinterpretation.** Browser
MISSIONARY_DURATION_SECONDS = 32 (real seconds) is reinterpreted as
MissionaryDurationInWorldDays = 32f directly on the in-world
timeline. The alternative (translate via DualClock.DaysPerRealSecond)
couples the constant to clock-state at dispatch time, which would
make per-operation resolution timing non-deterministic if the clock
rate ever shifts. A future resolution slice can either preserve the
direct-numeric port or translate at resolution time; the data shape
stays the same.

**Stale csproj entries from branch switches.** Per HANDOFF.md the
csproj files are gitignored and leak across branch switches. After
branching from origin/master (ef58ec4f) the csproj on disk reflected
the prior branch's state and was missing the Codex sub-slice 7
entries (`BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`
and `BloodlinesBreachLegibilityReadoutSmokeValidation.cs`); those
files are present on the new base from disk and required csproj
re-registration for `dotnet build` to compile them. Unity's csproj
auto-regeneration restores entries when the editor reloads but
the dotnet-build gate runs first.

---

## Current Readiness

Branch `claude/unity-ai-captive-state-and-missionary-execution` is
ready to merge. All gates green, contract at revision 37, continuity
files updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Bundle 2 candidates (next session's scope per the 2026-04-19
   roadmap):
   - **Holy war declaration execution** (sub-slice 21): port
     `startHolyWarDeclaration` at simulation.js:10565 with a
     DynastyOperationHolyWarComponent attached to the entity created
     by BeginOperation. Consumes
     `AICovertOpsComponent.LastFiredOp == CovertOpKind.HolyWar`.
   - **Divine right declaration execution** (sub-slice 22): port
     the divine right declaration flow with a
     DynastyOperationDivineRightComponent. Consumes
     `AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight`.
3. Codex fortification-siege lane: breach sealing / recovery
   (sub-slice 8) is the next recommended Codex slice on a fresh
   `codex/unity-fortification-*` branch. Master-base contract
   revision target bumps to 38 when that lands.
