# Unity AI Strategic Layer Bundle 3: Captive Rescue and Ransom Execution

**Date:** 2026-04-20
**Branch:** `claude/unity-ai-captive-rescue-and-ransom-execution`
**Lane:** ai-strategic-layer
**Contract Revision:** 40
**Master Base:** `7821d74a` (rebased from original cec33509 base after Codex fortification-siege sub-slice 8 breach sealing / recovery landed at revision 39 concurrently with Bundle 3's original revision-39 claim)

---

## Goal

Bundle 3 of the AI mechanical campaign. Two captive-lifecycle
dispatch consumers ship as one commit, one merge, and one contract
revision bump on top of Bundle 2's faith-driven dispatch baseline.

Sub-slice 23 ports the captive rescue declaration path as the fourth
production consumer of the sub-slice 18 dynasty-operations foundation
and the first production reader of the sub-slice 19
CapturedMemberElement buffer. The system walks every faction's
captive buffer to find a Held captive belonging to the source
faction, gates on operative availability + cost, attaches a per-kind
component carrying rescue-specific timing + success probability +
cost, and pushes the canonical narrative line.

Sub-slice 24 ports the captive ransom negotiation path as the fifth
production consumer of the foundation and the second production
reader of the captive buffer. Mirrors the rescue system's captive
picker + dispatch shape but differs in operative roles (Diplomat +
Merchant), cost (higher gold, lower influence), duration, and
ProjectedChance (hardcoded 1.0 because ransom is a paid transaction,
not a roll).

Per-kind resolution (rescue success roll, captive release via
CapturedMemberHelpers.ReleaseCaptive, conviction event recording) is
intentionally deferred to a future slice for both sub-slices.

---

## Work Completed

### Sub-Slice 23: Captive Rescue Execution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCaptiveRescueComponent.cs`
  Per-kind component attached to the entity created by
  `DynastyOperationLimits.BeginOperation`:
  - `ResolveAtInWorldDays` (float) - current + 20f for rescue duration
  - `CaptiveMemberId` (FixedString64Bytes) - captured member id
  - `CaptiveMemberTitle` (FixedString64Bytes) - carried for narrative replay
  - `CaptorFactionId` (FixedString32Bytes) - faction holding the captive
  - `SpymasterMemberId` (FixedString64Bytes) - resolved by priority
    [Spymaster, Diplomat, Merchant]
  - `DiplomatMemberId` (FixedString64Bytes) - resolved by priority
    [Diplomat, Merchant, HeirDesignate]
  - `HoldingSettlementId` (FixedString64Bytes) - default empty (holding-settlement port deferred)
  - `KeepTier` (int) - default 0
  - `WardId` (FixedString32Bytes) - default empty (ward-profile surface deferred)
  - `SuccessScore` (float) - power - difficulty from simplified parity
  - `ProjectedChance` (float) - clamp(0.5 + successScore/45, 0.12, 0.88)
  - `IntensityCost` (float) - 0f (rescue does not deduct intensity)
  - `EscrowCost` (DynastyOperationEscrowCost) - Gold=42f, Influence=26f

- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRescueExecutionSystem.cs`
  `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]`
  ISystem consuming `AICovertOpsComponent.LastFiredOp ==
  CovertOpKind.CaptiveRescue`. Gates on:
  1. Source faction exists with FactionComponent.
  2. Captive picker finds a Held captive belonging to source across
     any faction's CapturedMemberElement buffer.
  3. No existing active dynasty operation for this captive member id.
  4. Source has Spymaster-equivalent member on roster (priority
     [Spymaster, Diplomat, Merchant]) with non-Fallen, non-Captured status.
  5. Source has Diplomat-equivalent member (priority [Diplomat,
     Merchant, HeirDesignate]).
  6. Source ResourceStockpileComponent.Gold >= 42.
  7. Source ResourceStockpileComponent.Influence >= 26.
  8. DynastyOperationLimits.HasCapacity for source.

  On success deducts 42 Gold + 26 Influence, computes simplified
  parity formula, calls BeginOperation with DynastyOperationKind.CaptiveRescue +
  captor faction id as target + captive member id, attaches the
  per-kind component, and pushes
  `<source> dispatches covert agents to recover <captive> from <captor>.`
  with Info tone when source is player and Good otherwise.

  Exposes three `internal static` helpers for reuse by
  AICaptiveRansomExecutionSystem:
  - `TryPickCaptive` - captive picker (first Held matching source)
  - `HasActiveOperationForMember` - existing-operation gate check
  - `TryGetMemberByRolePriority` - general role-priority lookup

#### Browser parity notes (sub-slice 23)

Browser execution body (simulation.js:11067-11111): full
startRescueOperation including terms gate, cost deduction, operation
construction, dispatch capacity check, and pushMessage.

Browser dispatch hook (ai.js:2566-2607): hardcoded source "enemy"
with a chosen captive from pickAiCaptiveRecoveryTarget (ai.js:3011).
The "rescue or ransom" decision happens inside AICovertOpsSystem's
TryFireOps based on HighPriorityCaptive / EnemyIsHostileToPlayer
flags; by the time this system runs the choice is already made.

Browser tone routing (simulation.js:11108):
```js
factionId === "player" ? "info" : "good"
```

Unity matches: Info when source is player, Good otherwise. Browser
uses good for non-player rescue dispatch because the rescue is a
positive faction action (recovering a captured dynasty member).

Browser duration translation: `RESCUE_BASE_DURATION_SECONDS = 20` at
simulation.js:29 is real seconds. Unity treats the numeric value as
in-world-day directly (`RescueBaseDurationInWorldDays = 20f`),
matching the sub-slice 20/21/22 duration convention. Renown-scaled
duration adjustment (browser RESCUE_DURATION_RENOWN_MULTIPLIER = 0.7
at simulation.js:30) deferred until CapturedMemberElement carries
renown.

**Captive picker simplification.** Browser
pickAiCaptiveRecoveryTarget (ai.js:3011) filters enemy.dynasty.members
for captured status, sorts by role priority
(head_of_bloodline < heir_designate < commander < governor <
spymaster < steward < diplomat < merchant) then by renown descending,
returns the first. Unity port walks every faction's
CapturedMemberElement buffer and returns the first entry where
OriginFactionId matches source and Status == Held. This is sufficient
for this slice because CapturedMemberElement does not yet carry
roleId or renown; a future slice that extends the captive element
shape can swap in a sophisticated picker without reshaping this
system.

**Cost calculation simplification.** Browser cost
(simulation.js:4999-5002) scales
`RESCUE_BASE_GOLD_COST * (1 + keepTier*0.06) + member.renown*4`
and
`RESCUE_BASE_INFLUENCE_COST * roleMultiplier + member.renown*1.4`.
Unity uses the canonical base (Gold=42, Influence=26) without
scaling. A future slice that extends CapturedMemberElement with
renown/role will tighten the calculation.

**Parity formula simplification.** Browser power/difficulty at
simulation.js:4987-4997:
```js
const power = 12 + spymaster.renown*0.95 + envoy.renown*0.35 + commander.renown*0.22;
const difficulty = 16 + member.renown*roleMultiplier*0.65 + keepTier*4.8 + wardDifficulty + captorSpymaster.renown*0.42;
```
Unity port uses only `BasePower + spymaster.renown*0.95 +
diplomat.renown*0.35` and `BaseDifficulty` constants. The commander
contribution to power and the member / keepTier / ward / captorSpymaster
contributions to difficulty are deferred because those inputs are not
yet available on CapturedMemberElement or on a fortification ward
surface. Result is a slight power adjustment vs browser; a future
slice can restore full formula parity when the inputs port.

**Faith operator role priority reuse.** Sub-slice 20 / 21's
`TryGetFaithOperator` pattern is extended to
`TryGetMemberByRolePriority` which takes an ordered DynastyRole[]
array and returns the first non-Fallen / non-Captured member matching
any role in declared priority order. The helper is `internal static`
on AICaptiveRescueExecutionSystem so AICaptiveRansomExecutionSystem
can reuse it for its Diplomat + Merchant priorities without
duplicating the walker.

### Sub-Slice 24: Captive Ransom Execution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCaptiveRansomComponent.cs`
  Per-kind component:
  - `ResolveAtInWorldDays` (float) - current + 16f for ransom duration
  - `CaptiveMemberId` (FixedString64Bytes)
  - `CaptiveMemberTitle` (FixedString64Bytes)
  - `CaptorFactionId` (FixedString32Bytes)
  - `DiplomatMemberId` (FixedString64Bytes) - priority
    [Diplomat, Merchant, HeirDesignate, HeadOfBloodline]
  - `MerchantMemberId` (FixedString64Bytes) - priority
    [Merchant, Governor, HeadOfBloodline]
  - `ProjectedChance` (float) - hardcoded 1.0 (ransom is a paid
    transaction, not a roll, per browser simulation.js:4964)
  - `IntensityCost` (float) - 0f
  - `EscrowCost` (DynastyOperationEscrowCost) - Gold=70f, Influence=18f

- `unity/Assets/_Bloodlines/Code/AI/AICaptiveRansomExecutionSystem.cs`
  Same group/order as the rescue system. Reuses the sub-slice 23
  `TryPickCaptive`, `HasActiveOperationForMember`, and
  `TryGetMemberByRolePriority` helpers directly (no duplication).
  Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRansom`.

  Gates:
  1-3 identical to rescue (source exists, captive found, no existing
  operation for member).
  4. Source has Diplomat-equivalent member (priority [Diplomat,
     Merchant, HeirDesignate, HeadOfBloodline]).
  5. Source has Merchant-equivalent member (priority [Merchant,
     Governor, HeadOfBloodline]).
  6. Source ResourceStockpileComponent.Gold >= 70.
  7. Source ResourceStockpileComponent.Influence >= 18.
  8. DynastyOperationLimits.HasCapacity.

  On success deducts 70 Gold + 18 Influence, calls BeginOperation with
  DynastyOperationKind.CaptiveRansom + captor faction id as target +
  captive member id, attaches the per-kind component with
  ProjectedChance=1.0, and pushes
  `<source> opens ransom terms for <captive> with <captor>.`
  with Info tone when source is player and Good otherwise.

#### Browser parity notes (sub-slice 24)

Browser execution body (simulation.js:11026-11065): full
startRansomNegotiation.

Browser tone routing (simulation.js:11062): identical to rescue
(Info when source is player, Good otherwise).

Browser duration translation: `RANSOM_BASE_DURATION_SECONDS = 16` at
simulation.js:27 reinterpreted as `RansomBaseDurationInWorldDays = 16f`.
Renown-scaled adjustment (browser RANSOM_DURATION_RENOWN_MULTIPLIER =
0.55 at simulation.js:28) deferred.

**Cost calculation simplification.** Browser cost
(simulation.js:4947-4950) scales the base by member.renown,
roleMultiplier, envoyDiscount, captorPremium. Unity uses the
canonical base (Gold=70, Influence=18) without scaling. Same
rationale as sub-slice 23.

**Captor-faction envoy premium deferred.** Browser scales cost by
captorEnvoy.renown premium (simulation.js:4946). Captor-side
renown surface not yet ported on captives; Unity port omits this
factor.

**Resolution semantics** (deferred): the browser rescue/ransom
operations resolve at `resolveAt` timestamps via the per-kind
operation tick. Successful ransom flips the captive's status
(Held -> RansomOffered -> Released), returns the member to the
source faction roster, deducts cost from the home faction (already
done at dispatch time in Unity's port), records stewardship +1
conviction on captor, and oathkeeping +1 on source per browser
simulation.js:11136-11138. Unity defers all resolution-time effects
to a future resolution slice; the per-kind component sits on the
entity with Active=true until resolution.

### Shared-File Narrow Edits (lane-shared files)

- `unity/Assembly-CSharp.csproj` - added
  `DynastyOperationCaptiveRescueComponent.cs`,
  `AICaptiveRescueExecutionSystem.cs`,
  `DynastyOperationCaptiveRansomComponent.cs`,
  `AICaptiveRansomExecutionSystem.cs`. Bundle 1/2 + Codex 7 entries
  already present on disk from prior branch state.
- `unity/Assembly-CSharp-Editor.csproj` - added
  `BloodlinesCaptiveRescueAndRansomExecutionSmokeValidation.cs`.

### Cross-Lane Reads (no writes)

- Reads `FactionComponent.FactionId` and walks
  `CapturedMemberElement` buffers across all factions via the shared
  captive picker. No mutations.
- Reads `ResourceStockpileComponent.Gold` and `.Influence` on the
  source faction entity. Mutates both fields via in-place
  SetComponentData when either dispatch fires.
- Reads `DynastyMemberComponent` (Role, Status, Renown, MemberId,
  Title) and `DynastyMemberRef` buffer to resolve operatives.
  No mutations.
- Reads `DualClockComponent.InWorldDays` to stamp ResolveAtInWorldDays.
  No mutations.
- Reads existing `DynastyOperationComponent` entries to enforce the
  per-member existing-operation gate (both sub-slices share the
  member id).

### New dedicated smoke validator

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveRescueAndRansomExecutionSmokeValidation.cs`
  9-phase validator covering both sub-slices:

  **Sub-slice 23 phases**:
  - **PhaseRescueDispatchSuccess**: enemy faction with full
    Spymaster + Diplomat roster, gold 100, influence 100; player
    faction holds a Held captive belonging to enemy. Tick world.
    Verify dispatch cleared, one CaptiveRescue op created with player
    target + captive member id, per-kind attached with ResolveAt=80
    (60+20), EscrowCost.Gold=42, EscrowCost.Influence=26,
    OperatorMemberIds resolved, source Gold 100->58, source Influence
    100->74, narrative +1 Good (source != player).
  - **PhaseRescueNoCaptiveBlocks**: bare player (no Held captives);
    dispatch fails (picker returns null); resources untouched.
  - **PhaseRescueNoSpymasterBlocks**: enemy roster has only a
    Commander (no Spymaster / Diplomat / Merchant); dispatch fails.
  - **PhaseRescueInsufficientGoldBlocks**: enemy gold 30 < 42 cost;
    dispatch fails.
  - **PhaseRescueInsufficientInfluenceBlocks**: enemy influence 20 <
    26 cost; dispatch fails.
  - **PhaseRescueExistingOperationBlocks**: pre-seed an active
    DynastyOperationComponent (a Ransom op) targeting the same member
    id; new rescue dispatch fails (existing-operation gate); only
    the preexisting ransom remains.

  **Sub-slice 24 phases**:
  - **PhaseRansomDispatchSuccess**: enemy with full Diplomat +
    Merchant roster, gold 100, influence 50; player holds a Held
    captive belonging to enemy. Tick world. Verify dispatch cleared,
    one CaptiveRansom op created, per-kind attached with
    ResolveAt=96 (80+16), EscrowCost.Gold=70, EscrowCost.Influence=18,
    ProjectedChance=1.0, source Gold 100->30, source Influence
    50->32, narrative +1 Good.
  - **PhaseRansomInsufficientGoldBlocks**: enemy gold 50 < 70; dispatch fails.
  - **PhaseRansomNoMerchantBlocks**: enemy roster has only a
    Diplomat (no Merchant / Governor / HeadOfBloodline); dispatch
    fails.

  Artifact: `../artifacts/unity-captive-rescue-and-ransom-execution-smoke.log`
  Marker: `BLOODLINES_CAPTIVE_RESCUE_AND_RANSOM_EXECUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityCaptiveRescueAndRansomExecutionSmokeValidation.ps1`
  Standard PowerShell runner mirroring the Bundle 1/2 wrappers.

---

## Verification Results

All 10 canonical validation gates green, serialized per Unity's
project lock:

1. `dotnet build Assembly-CSharp.csproj -nologo` - 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` - 0 errors PASS
3. Bootstrap runtime smoke - PASS
4. Combat smoke - all phases PASS
5. Canonical scene shells (Bootstrap + Gameplay) - both PASS
6. Fortification smoke - PASS
7. Siege smoke - PASS
8. `node tests/data-validation.mjs` - PASS
9. `node tests/runtime-bridge.mjs` - PASS
10. Contract staleness check - revision 40 current

Bundle 3 dedicated smoke
(`Invoke-BloodlinesUnityCaptiveRescueAndRansomExecutionSmokeValidation.ps1`):
all 9 phases PASS.

No Bundle 1 / Bundle 2 / sub-slice 8-18 regression expected because
Bundle 3 is pure net-new surface. Sub-slice 18 helpers
(DynastyOperationLimits.HasCapacity and BeginOperation) gain their
fourth and fifth production callers without shape change.
Sub-slice 19 CapturedMemberElement buffer gains its first two
production readers without shape change. Sub-slice 16
NarrativeMessageBridge gains a seventh and eighth production caller
without shape change.

---

## Key Design Notes

**Bundle structure.** Two captive-lifecycle consumers ship as one
bundle because both are mirror-image dispatch pathways sharing the
captive picker, the existing-operation gate, the role-priority
helper, and the narrative-tone routing. Bundling reduces ceremony
(one merge, one contract bump) without coupling the sub-slice files.
The `internal static` helper sharing on AICaptiveRescueExecutionSystem
explicitly shared with AICaptiveRansomExecutionSystem is an in-bundle
pattern; a future refactor can promote the helpers to a shared
CaptiveHelpers static class if a third consumer lands.

**Sub-slice 23/24 cost-simplification rationale.** Browser cost
scales by member.renown, roleMultiplier, keepTier, ward difficulty,
and captor premium. Unity uses canonical base constants because
CapturedMemberElement does not carry renown/roleId, and the
holding-settlement/ward surfaces are not yet ported. Delivering a
simplified but deterministic cost is better than blocking both
sub-slices on those surface ports; a future captive-element
extension can tighten cost calculation without reshaping the
systems.

**Captive picker simplification.** Browser picks by role priority
then renown; Unity picks the first match. Browser picker lives on a
per-faction-owned member list (enemy.dynasty.members filtered for
captured status); Unity picker walks every faction's captive buffer
because the captive-side source-of-truth moved to the captor's
buffer in sub-slice 19. The simplification is acceptable for this
slice because the dispatch test surface exercises single-captive
scenarios; multi-captive priority selection is a polish concern that
lands with the captive-element extension.

**Helper sharing pattern.** AICaptiveRescueExecutionSystem exposes
`TryPickCaptive`, `HasActiveOperationForMember`, and
`TryGetMemberByRolePriority` as `internal static` methods.
AICaptiveRansomExecutionSystem calls them directly. This keeps the
two systems in sync without introducing a separate helper class. If
a fourth captive-lifecycle consumer (captive execution, captive
exchange) lands, promote the helpers to a `CapturedMemberHelpers`
partial or a new `CaptiveOperationHelpers` static class.

**Per-kind resolution deferred for both sub-slices.** Following the
sub-slice 20/21/22 pattern, Bundle 3 ships only the dispatch creation
path. A future resolution slice walks expired entries at
ResolveAtInWorldDays, rolls against ProjectedChance for rescue (or
unconditionally succeeds for ransom), applies effects (captive
release via CapturedMemberHelpers.ReleaseCaptive with Status flip to
Released, conviction event recording, source-faction member roster
return if a member-restoration surface ports), and flips Active=false.

**Resource cost asymmetry vs dispatch source.** Browser's ransom
flow has a peculiar asymmetry: `demandCaptiveRansom` (simulation.js:11113)
charges the captive's *home* faction when the *captor* demands
ransom, but `startRansomNegotiation` (simulation.js:11026) charges
the home faction when the *home faction* negotiates ransom. The AI
dispatch path calls startRansomNegotiation with the home faction as
the source (`startRansomNegotiation(state, "enemy", captiveTarget.id)`
at ai.js:2578 where "enemy" is the AI faction holding captured
members of its own dynasty). Unity matches: the source faction
(which is the captive's home) pays the ransom cost. Browser's
captor-driven `demandCaptiveRansom` path is a separate surface not
yet ported; a future slice can add it as a parallel dispatch
consumer if the canonical AI wants to run the captor side too.

**Existing-operation gate shared across sub-slices.** Both systems
call `HasActiveOperationForMember` which walks every DynastyOperationComponent
entity and checks TargetMemberId. A preexisting Rescue op blocks
a new Ransom op (and vice versa) for the same member. This matches
the browser `getActiveDynastyOperationForMember` check at
simulation.js:4935/4974 which treats any active operation for the
member as blocking regardless of operation type.

---

## Current Readiness

Branch `claude/unity-ai-captive-rescue-and-ransom-execution` is
ready to merge. All gates green, contract at revision 40, continuity
files updated. Rebased onto `7821d74a` (Codex fortification-siege
sub-slice 8) after the parallel-revision resolution.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Bundle 4 candidates (next session's scope):
   - **Per-kind resolution system** (the first multi-kind resolution
     consumer): walk expired per-kind components at their resolve
     boundaries, apply effects, flip Active=false. Missionary is the
     simplest (exposure + intensity + loyalty); holy war requires
     faction.faith.activeHolyWars materialization + war-tick pulses;
     divine right requires apex faith claim evaluation; captive
     rescue/ransom require captive status flips plus member roster
     restoration. Ship as one walker or split per-kind.
   - **CapturedMemberElement extension** (roleId + renown fields):
     adds the canonical browser fields to enable renown-scaled cost,
     role-priority captive picker, and simplifed parity formula
     tightening.
   - **Divine right side-effect resolution** (sub-slice 22 deferrals):
     mutual hostility application, AI timer cap propagation,
     conviction event recording.
3. Codex fortification-siege lane: breach sealing / recovery
   (sub-slice 8) landed on master at `7821d74a` at revision 39 in
   parallel with this Bundle 3 work. The next Codex slice is
   sub-slice 9 destroyed-counter recovery on a fresh
   `codex/unity-fortification-*` branch. Master-base contract
   revision target bumps to 41 when that lands.
