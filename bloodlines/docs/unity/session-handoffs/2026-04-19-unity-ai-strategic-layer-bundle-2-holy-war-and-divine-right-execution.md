# Unity AI Strategic Layer Bundle 2: Holy War and Divine Right Execution

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-holy-war-and-divine-right-execution`
**Lane:** ai-strategic-layer
**Contract Revision:** 38
**Master Base:** `0b2fd111` (after Bundle 1 captive state + missionary execution landed at revision 37)

---

## Goal

Bundle 2 of the AI mechanical campaign. Two faith-driven dispatch
consumers ship as one commit, one merge, and one contract revision
bump on top of Bundle 1's missionary execution baseline.

Sub-slice 21 ports the holy war declaration execution path as the
second production consumer of the sub-slice 18 dynasty-operations
foundation. The system mirrors sub-slice 20 missionary execution's
shape: gate on the simplified browser parity terms, deduct cost on
success, attach a per-kind component to the entity created by
`DynastyOperationLimits.BeginOperation`, push the canonical narrative
line, and clear LastFiredOp to None unconditionally (one-shot pattern).

Sub-slice 22 ports the divine right declaration execution path as the
third production consumer. Divine right is the most architecturally
distinctive of the dispatch family because the browser does not gate
it on `DYNASTY_OPERATION_ACTIVE_LIMIT`; Unity routes it through the
shared limits surface anyway for consistency with the other dispatch
consumers, while preserving the browser per-faction one-active-at-a-time
semantic via an explicit existing-declaration gate.

Per-kind resolution (declaration completion / war-tick effects / apex
faith claim or failure cooldown) is intentionally deferred to a future
slice for both sub-slices.

---

## Work Completed

### Sub-Slice 21: Holy War Declaration Execution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationHolyWarComponent.cs`
  Per-kind component attached to the entity created by
  `DynastyOperationLimits.BeginOperation`:
  - `ResolveAtInWorldDays` (float) - declaration window end (current + 18f)
  - `WarExpiresAtInWorldDays` (float) - full holy-war duration end (current + 180f)
  - `OperatorMemberId` (FixedString64Bytes)
  - `OperatorTitle` (FixedString64Bytes)
  - `IntensityPulse` (float) - browser doctrine bias 1.2 dark / 0.9 light
  - `LoyaltyPulse` (float) - browser doctrine bias 1.8 dark / 1.2 light
  - `CompatibilityLabel` (FixedString64Bytes) - simplified Unity-side derivation
  - `IntensityCost` (float) - 18f
  - `EscrowCost` (DynastyOperationEscrowCost) - Influence = 24f

- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarExecutionSystem.cs`
  `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]`
  ISystem consuming `AICovertOpsComponent.LastFiredOp ==
  CovertOpKind.HolyWar`. Gates on:
  1. Source != target.
  2. Source has committed faith (FaithStateComponent.SelectedFaith != None).
  3. Target faction exists with committed faith.
  4. Simplified harmonious-tier check via identical (faith, doctrine).
  5. Source has a faith operator on the dynasty roster
     (IdeologicalLeader / Spymaster / HeadOfBloodline / Diplomat with
     non-Fallen, non-Captured status).
  6. Source ResourceStockpileComponent.Influence >= 24.
  7. Source FaithStateComponent.Intensity >= 18.
  8. DynastyOperationLimits.HasCapacity for source.

  On success deducts cost, computes per-kind doctrine-bias pulses,
  calls BeginOperation with DynastyOperationKind.HolyWar + player
  target, attaches the per-kind component, pushes
  `<source> sends a holy war declaration toward <target>.` with
  Warn tone when source or target is player.

#### Browser parity notes (sub-slice 21)

Browser execution body (simulation.js:10565-10602): full
startHolyWarDeclaration including terms gate, cost deduction,
operation construction, dispatch capacity check, and pushMessage.

Browser dispatch hook (ai.js:2512-2551): hardcoded source "enemy" /
target "player".

Browser tone routing (simulation.js:10599):
```js
factionId === "player" ? "warn" : targetFactionId === "player" ? "warn" : "info"
```
Unity matches: Warn when either source or target is player, Info
otherwise. Browser uses warn either way because holy war is an
escalation regardless of perspective.

Browser doctrine-bias pulses (simulation.js:10468-10469):
```js
intensityPulse: faction.faith.doctrinePath === "dark" ? 1.2 : 0.9,
loyaltyPulse:   faction.faith.doctrinePath === "dark" ? 1.8 : 1.2,
```
Unity matches exactly.

Browser duration translation: `HOLY_WAR_DECLARATION_DURATION_SECONDS = 18`
(simulation.js:9771) and `HOLY_WAR_DURATION_SECONDS = 180`
(simulation.js:9775) are real seconds. Unity treats both as in-world-day
numeric values directly (`HolyWarDeclarationDurationInWorldDays = 18f`,
`HolyWarDurationInWorldDays = 180f`), matching the sub-slice 20
missionary-duration convention. Future resolution slice can re-translate
at runtime if the canonical clock rate ever shifts.

**Compatibility-tier simplification.** Browser uses
`getMarriageFaithCompatibilityProfile` (simulation.js:596-730) which
ladders through a tier system based on covenant-name covariance. Unity
collapses this to identical-(faith, doctrine) equality for the
harmonious gate. The simplified gate is strictly looser than browser
(it lets some "neutral" tiers through that browser would block);
future tightening lands when the covenant compatibility surface ports.

**Faith operator role gate.** Reuses sub-slice 20's pattern. Browser
`getFaithOperatorMember` (simulation.js:690) accepts `["ideological_leader",
"sorcerer", "head_of_bloodline", "diplomat"]`. Unity has no Sorcerer
DynastyRole; Spymaster stands in pending a canonical addition.

### Sub-Slice 22: Divine Right Declaration Execution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationDivineRightComponent.cs`
  Per-kind component:
  - `ResolveAtInWorldDays` (float) - declaration window end (current + 180f)
  - `SourceFaithId` (FixedString64Bytes) - CovenantId mapped to canonical string
  - `DoctrinePath` (DoctrinePath enum) - captured at declaration
  - `RecognitionShare` (float) - default 0; recognition system not yet ported
  - `RecognitionSharePct` (float) - default 0; same deferral
  - `ActiveApexStructureId` (FixedString64Bytes) - default empty; apex structure system not yet ported
  - `ActiveApexStructureName` (FixedString64Bytes) - default empty; same deferral

  No EscrowCost or IntensityCost: browser divine right does NOT
  deduct intensity or charge an escrow cost. The cost is the
  covenant-test prerequisite that gates getDivineRightDeclarationTerms.

- `unity/Assets/_Bloodlines/Code/AI/AIDivineRightExecutionSystem.cs`
  Same group/order as the holy-war system. Consumes
  `AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight`.
  Gates on the simplified Unity-side parity set:
  1. Source has committed faith (FaithStateComponent.SelectedFaith != None).
  2. Source FaithStateComponent.Intensity >= 80
     (DIVINE_RIGHT_INTENSITY_THRESHOLD).
  3. Source FaithStateComponent.Level >= 5.
  4. No existing active DivineRight DynastyOperationComponent for this
     faction (preserves browser per-faction one-active-at-a-time
     semantic).
  5. DynastyOperationLimits.HasCapacity for source.

  On success calls BeginOperation with DynastyOperationKind.DivineRight
  and default target (divine right has no specific target faction;
  affected factions are derived at resolution time), attaches the
  per-kind component, pushes
  `<source> declares Divine Right under <faith>. The spread window opens for 180 in-world days.`
  with Warn tone (browser always-warn).

#### Browser parity notes (sub-slice 22)

Browser execution body (simulation.js:10784-10835): full
startDivineRightDeclaration including terms gate, declaration object
construction, mutual hostility application, AI timer cap propagation,
conviction event recording, and pushMessage.

Browser dispatch hook (ai.js:2553-2564): hardcoded source "enemy" with
no target argument; divine right is unilateral.

Browser tone routing (simulation.js:10832):
```js
factionId === "player" ? "warn" : "warn"
```
Always warn regardless of source. Unity matches.

**Notable Unity-side departure.** Browser does NOT route divine right
through `DYNASTY_OPERATION_ACTIVE_LIMIT` (startDivineRightDeclaration
never calls getDynastyOperationsState; declaration writes directly to
faction.faith.divineRightDeclaration). Unity DOES create a
`DynastyOperationComponent` entity with Kind=DivineRight so future
intel-report and resolution-system queries can use one shape for all
dispatch consumers. The browser per-faction one-active-at-a-time
semantic is preserved by the explicit existing-declaration gate
before the capacity check.

**Gates intentionally deferred** (Unity has no canonical surface yet):
- Covenant Test passed gate (browser
  `ensureFaithCovenantTestCompletionFromLegacyState` +
  `faithState.covenantTestPassed` at simulation.js:10606/10614).
- Cooldown gate (browser `profile.cooldownRemaining` at :10626).
- Stage-ready gate (browser `profile.stageReady` Final Convergence
  threshold at :10629).
- Active apex structure gate (browser `profile.activeApexStructureReady`
  at :10635).
- Recognition share gate (browser `profile.recognitionReady` at :10638).
- Faction kind == kingdom gate (at :10607). Defensible to add via
  FactionKindComponent in a hardening pass; omitted because the AI
  dispatch hook only fires for the enemy faction which is canonically
  a kingdom.

**Effects intentionally deferred** to a future resolution slice:
- Mutual hostility application against all non-same-faith kingdoms
  (browser `ensureMutualHostility` at simulation.js:10819).
- AI timer cap propagation to candidate factions (browser
  `attackTimer / territoryTimer / raidTimer / missionaryTimer /
  holyWarTimer` caps at :10822-10826).
- Conviction event recording (browser `recordConvictionEvent` at
  :10806; oathkeeping for light, desecration for dark, +3 either way).
- Resolution at ResolveAtInWorldDays (apex faith claim on success vs
  `failDivineRightDeclaration` cooldown + legitimacy penalty on
  failure at :10691).

### Shared-File Narrow Edits (lane-shared files)

- `unity/Assembly-CSharp.csproj` - added
  `DynastyOperationHolyWarComponent.cs`,
  `AIHolyWarExecutionSystem.cs`,
  `DynastyOperationDivineRightComponent.cs`,
  `AIDivineRightExecutionSystem.cs`. The csproj on disk already
  carried Bundle 1's four new C# files plus Codex sub-slice 7's
  entries from the prior branch state, so no Bundle 1 / Codex 7
  re-registration was needed.
- `unity/Assembly-CSharp-Editor.csproj` - added
  `BloodlinesHolyWarAndDivineRightExecutionSmokeValidation.cs`.

### Cross-Lane Reads (no writes)

- Reads `FactionComponent.FactionId` to find target faction entity by
  id (sub-slice 21). No mutations.
- Reads `FaithStateComponent.SelectedFaith`,
  `FaithStateComponent.DoctrinePath`,
  `FaithStateComponent.Intensity`,
  `FaithStateComponent.Level` on the source faction entity. Mutates
  Intensity on the source via in-place SetComponentData when the
  holy-war dispatch fires (sub-slice 21 only; sub-slice 22 does not
  deduct intensity).
- Reads `FaithStateComponent.SelectedFaith` and DoctrinePath on the
  target faction entity for the holy-war compatibility check. No
  mutations.
- Reads `ResourceStockpileComponent.Influence` on the source faction
  entity (sub-slice 21 only). Mutates Influence on the source via
  in-place SetComponentData when the holy-war dispatch fires.
- Reads `DynastyMemberComponent` (Role, Status, MemberId, Title) and
  `DynastyMemberRef` buffer to resolve faith operator on the source
  dynasty roster (sub-slice 21 only). No mutations.
- Reads `DualClockComponent.InWorldDays` to stamp ResolveAtInWorldDays
  and WarExpiresAtInWorldDays. No mutations.
- Reads existing `DynastyOperationComponent` entries to enforce the
  divine-right existing-declaration gate (sub-slice 22).

### New dedicated smoke validator

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarAndDivineRightExecutionSmokeValidation.cs`
  10-phase validator covering both sub-slices:

  **Sub-slice 21 phases**:
  - **PhaseHolyWarDispatchSuccess**: enemy with faith, intensity 30,
    influence 50, faith operator on roster; target=player has different
    committed faith. Tick world. Verify dispatch cleared, one HolyWar
    op created with player target, per-kind attached with ResolveAt=68
    (50+18), WarExpiresAt=230 (50+180), IntensityCost=18,
    EscrowCost.Influence=24, OperatorMemberId resolved, light-doctrine
    pulses 0.9/1.2, source Influence 50->26, source Intensity 30->12,
    narrative +1 Warn (target=player).
  - **PhaseHolyWarHarmoniousFaithBlocks**: identical (faith, doctrine)
    blocks; resources untouched; dispatch cleared.
  - **PhaseHolyWarTargetNoFaithBlocks**: target CovenantId.None
    blocks; resources untouched.
  - **PhaseHolyWarInsufficientIntensityBlocks**: intensity 12 < 18
    blocks; intensity / influence untouched.
  - **PhaseHolyWarInsufficientResourcesBlocks**: influence 20 < 24
    blocks; influence untouched.
  - **PhaseHolyWarDarkPathPulses**: dark-doctrine source produces
    intensityPulse 1.2 and loyaltyPulse 1.8 per browser parity.

  **Sub-slice 22 phases**:
  - **PhaseDivineRightDispatchSuccess**: enemy with faith, intensity 90,
    level 5. Tick world. Verify dispatch cleared, one DivineRight op
    created with default target, per-kind attached with ResolveAt=280
    (100+180), SourceFaithId="the_order", DoctrinePath=Light,
    recognition/structure placeholders default, narrative +1 Warn.
  - **PhaseDivineRightInsufficientIntensityBlocks**: intensity 50 <
    80 threshold blocks; dispatch cleared.
  - **PhaseDivineRightInsufficientLevelBlocks**: level 4 < 5 blocks;
    dispatch cleared.
  - **PhaseDivineRightExistingDeclarationBlocks**: pre-seed an active
    DivineRight op for enemy; new dispatch fails (existing-declaration
    gate); only 1 active op total; dispatch still cleared.

  Artifact: `../artifacts/unity-holy-war-and-divine-right-execution-smoke.log`
  Marker: `BLOODLINES_HOLY_WAR_AND_DIVINE_RIGHT_EXECUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityHolyWarAndDivineRightExecutionSmokeValidation.ps1`
  Standard PowerShell runner mirroring the Bundle 1 wrapper.

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
10. Contract staleness check - revision 38 current

Bundle 2 dedicated smoke
(`Invoke-BloodlinesUnityHolyWarAndDivineRightExecutionSmokeValidation.ps1`):
all 10 phases PASS.

No Bundle 1 / sub-slice 8-18 regression expected because Bundle 2 is
pure net-new surface. Sub-slice 18 helpers (DynastyOperationLimits.HasCapacity
and BeginOperation) gain their second and third production callers
without shape change. Sub-slice 16 NarrativeMessageBridge gains a
fifth and sixth production caller without shape change.

---

## Key Design Notes

**Bundle structure.** Two faith-driven dispatch consumers ship as
one bundle because both are direct extensions of the sub-slice 20
missionary execution pattern: same dispatch-consumer shape, same
one-shot pattern, same per-kind component attachment to the entity
created by BeginOperation. Bundling reduces ceremony (one merge,
one contract bump) without coupling the sub-slice files. Sub-slice
21 and 22 each have separate phases in the smoke validator and can
be reviewed independently.

**Divine right Unity-side cap routing.** Browser does not gate divine
right on `DYNASTY_OPERATION_ACTIVE_LIMIT`. Unity routes through
DynastyOperationLimits anyway because:
- Surface consistency: future intel-report queries iterate
  DynastyOperationComponent entities. Excluding divine right from
  that surface would force a separate query path.
- Resolution-system uniformity: a future per-kind resolution system
  can walk all DynastyOperationKind values uniformly without special
  casing divine right.
- Browser parity preservation: the per-faction one-active-at-a-time
  semantic is enforced by the explicit existing-declaration gate
  before the capacity check, so Unity's behavior matches browser
  even with the extra surface routing.

The trade-off: a faction at cap on missionary + holy war could be
blocked from declaring divine right when browser would allow it. In
practice this is a rare configuration (cap is 6, divine right
prerequisites are stringent) and the cap blockage surfaces as an
explicit failure rather than silent under-cap divergence.

**Compatibility-tier simplification (sub-slice 21).** Browser
`getMarriageFaithCompatibilityProfile` ladders through a tier system
(harmonious / aligned / neutral / fractured / discordant) based on
covenant-name covariance with cross-faith pairings. Unity collapses
to identical-(faith, doctrine) -> harmonious vs everything-else ->
allowed. The Unity gate is strictly looser than browser, so a future
covenant compatibility port that adds tier ladder cases can tighten
the gate without re-shaping the system. This trade is acceptable
because the browser also allows holy war for any non-harmonious tier
(it doesn't block on intermediate tiers); the looser Unity gate just
admits a few cases the browser also admits but with different tier
labels.

**Per-kind resolution deferred for both sub-slices.** Following the
sub-slice 20 missionary pattern, Bundle 2 ships only the dispatch
creation path. A future resolution slice walks expired
`DynastyOperationHolyWarComponent` and
`DynastyOperationDivineRightComponent` entries at their respective
ResolveAtInWorldDays, applies the per-kind effects (holy-war
materialization, war-tick pulses, divine-right success/failure
resolution), and flips Active=false. Bundling resolution into Bundle
2 would couple two distinct concerns (dispatch creation vs effect
application) and inflate the slice; separating them keeps each
reviewable change focused.

**Divine right effect deferral.** Browser
startDivineRightDeclaration applies several immediate side effects
beyond writing the declaration object: mutual hostility against
non-same-faith kingdoms, AI timer caps on candidate factions,
conviction event recording. Unity defers all three. The mutual
hostility application is the most game-impactful and should land in
the resolution slice that ports the divine-right effects; AI timer
caps require AICovertOpsComponent / EnemyAIStrategyComponent
mutation surfaces to be available; conviction event recording can
land independently when the conviction event surface is ready.

**No cost / no intensity for divine right.** Browser divine right
charges no escrow cost and no intensity deduction. The cost is the
covenant-test prerequisite that gates
`getDivineRightDeclarationTerms`. Unity's
DynastyOperationDivineRightComponent has no EscrowCost or
IntensityCost fields by design.

**Browser duration unit reinterpretation.** Continues the sub-slice
20 convention. HOLY_WAR_DECLARATION_DURATION_SECONDS = 18,
HOLY_WAR_DURATION_SECONDS = 180, and
DIVINE_RIGHT_DECLARATION_DURATION_SECONDS = 180 are reinterpreted as
in-world-day numeric values directly rather than translated through
DualClock.DaysPerRealSecond. A future resolution slice can either
preserve the direct-numeric port or translate at resolution time;
the data shape stays the same.

---

## Current Readiness

Branch `claude/unity-ai-holy-war-and-divine-right-execution` is
ready to merge. All gates green, contract at revision 38, continuity
files updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Bundle 3 candidates (next session's scope):
   - **Captive rescue execution + captive ransom execution** (sub-slices
     23 + 24): consume `CovertOpKind.CaptiveRescue` /
     `CovertOpKind.CaptiveRansom`, produce per-kind components
     (`DynastyOperationCaptiveRescueComponent`,
     `DynastyOperationCaptiveRansomComponent`), and mutate the
     sub-slice 19 `CapturedMemberElement` buffer (rescue removes Held
     captives via Released status flip; ransom flips to RansomOffered
     then Released with gold cost on the home faction). Bundle as
     Bundle 3 following the same ceremony.
   - **Per-kind resolution system** (the first multi-kind resolution
     consumer): walk expired `DynastyOperationMissionaryComponent`,
     `DynastyOperationHolyWarComponent`, and
     `DynastyOperationDivineRightComponent` entries at their
     ResolveAtInWorldDays / WarExpiresAtInWorldDays boundaries, apply
     per-kind effects, flip Active=false. Could ship as a single
     resolution slice or split per-kind.
3. Codex fortification-siege lane: breach sealing / recovery
   (sub-slice 8) is the next recommended Codex slice on a fresh
   `codex/unity-fortification-*` branch. Master-base contract
   revision target bumps to 39 when that lands.
