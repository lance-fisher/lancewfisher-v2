# Unity AI Strategic Layer Bundle 4: Missionary Resolution

**Date:** 2026-04-20
**Branch:** `claude/unity-ai-missionary-resolution`
**Lane:** ai-strategic-layer
**Contract Revision:** 41
**Master Base:** `e73933e4` (after Bundle 3 captive rescue + ransom execution landed at revision 40)

---

## Goal

Bundle 4 of the AI mechanical campaign: the first production
per-kind resolution consumer. Walks expired Missionary
DynastyOperationComponent entries and applies the canonical browser
missionary resolution effects (exposure gain, intensity erosion,
loyalty pressure) to the target faction.

Bundle 4 ships sub-slice 25 alone. Per-kind resolution systems differ
substantively across operation kinds (missionary, holy war, divine
right, captive rescue, captive ransom), so bundling them would couple
distinct effect logic. Missionary is the simplest and fully-portable
resolution (all input surfaces already exist in Unity); later bundles
ship holy war + divine right resolution and captive rescue + ransom
resolution separately.

This is the first consumer of the DynastyOperation* per-kind
components that reads them rather than producing them. Sub-slice 18's
DynastyOperationComponent gains a new consumer shape (the resolution
walker) that does not call BeginOperation or HasCapacity but rather
reads the DynastyOperationComponent entity query as a surface to find
expired ops.

---

## Work Completed

### Sub-Slice 25: Missionary Resolution

#### New files

- `unity/Assets/_Bloodlines/Code/AI/AIMissionaryResolutionSystem.cs`
  `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AIMissionaryExecutionSystem)]`
  ISystem. OnUpdate:
  1. Query all (DynastyOperationComponent, DynastyOperationMissionaryComponent) entities.
  2. Filter by Active==true and OperationKind==Missionary and
     ResolveAtInWorldDays <= current DualClock.InWorldDays.
  3. For each expired op: resolve effects (success or failure), flip Active=false, push narrative.

  Success gate: `mission.SuccessScore >= 0`. Deterministic, matching
  browser gate at simulation.js:5527.

  Success effects (from applyMissionaryEffect at simulation.js:10473-10503):
  - Apply ExposureGain to target's FaithExposureElement buffer: find
    entry where Faith matches the parsed source faith (via
    ParseFaithId mapping the FixedString64Bytes back to CovenantId),
    increment Exposure clamped to [0, 100], set Discovered=true. If
    no entry, append a new one.
  - Apply IntensityErosion to target's FaithStateComponent.Intensity
    (clamped >= 0) only when target has a different committed faith
    than source (browser gate at simulation.js:10485-10488).
  - Apply LoyaltyPressure to the lowest-loyalty ControlPointComponent
    owned by target, clamped to [0, 100]. Walks every ControlPointComponent
    entity, matches by OwnerFactionId, tracks the lowest-loyalty match.

  Failure effects:
  - Target FaithStateComponent.Intensity += 2 (FailureIntensityReinforcement)
    when target has a committed faith. Mirrors browser
    simulation.js:5563-5566.

  Void path (target faction entity missing): flips Active=false and
  pushes a void narrative without applying effects.

  Always flips Active=false regardless of outcome so the per-faction
  operation cap (sub-slice 18) releases the slot.

  Narrative push via NarrativeMessageBridge.Push with three variants:
  - Success: `"<faith> missionaries breach <target> with pressure dispatched by <source>."`
    Tone Good when source=player, Warn when target=player, else Info.
  - Failure: `"<target> repelled missionaries of <faith> from <source>."`
    Tone Warn when source=player, Good when target=player, else Info.
  - Void: `"<source> missionary mission expired without a valid target at <target>."`
    Tone Info always.

#### Browser parity notes

Browser resolution dispatcher (simulation.js:5453 tickDynastyOperations):
Iterates every faction's operations.active buffer, filters for
resolveAt <= elapsed, and switches on op.type. The missionary branch
(simulation.js:5517-5588) handles the success / failure / void paths.

Browser applyMissionaryEffect (simulation.js:10473-10503):
```js
noteFaithDiscovery(targetFaction, sourceFaithId);
const previousExposure = targetFaction.faith.exposure[sourceFaithId] ?? 0;
targetFaction.faith.exposure[sourceFaithId] = clamp(previousExposure + exposureGain, 0, 100);

if (targetFaction.faith.selectedFaithId && targetFaction.faith.selectedFaithId !== sourceFaithId) {
  targetFaction.faith.intensity = Math.max(0, intensity - intensityErosion);
  syncFaithIntensityState(targetFaction);
}

const pressurePoint = getLowestLoyaltyControlPoint(state, targetFactionId);
if (pressurePoint) {
  pressurePoint.loyalty = clamp(pressurePoint.loyalty - loyaltyPressure, 0, 100);
} else {
  adjustLegitimacy(targetFaction, -Math.max(1, Math.round(loyaltyPressure * 0.5)));
}
```

Unity matches the exposure, intensity, and loyalty-pressure paths.
The legitimacy fallback (when no control points exist) is deferred
because it requires mutating DynastyStateComponent.Legitimacy, a
dynasty-core lane surface; Unity falls back to a no-op when target
owns zero control points.

Browser failure branch (simulation.js:5562-5576):
```js
if (targetFaction.faith?.selectedFaithId) {
  targetFaction.faith.intensity = intensity + 2;
}
if (wardProfile.id !== "none" || targetFaith === "the_order") {
  ensureMutualHostility(state, sourceFactionId, targetFactionId);
}
recordConvictionEvent(state, targetFactionId, "stewardship", 1, "Repelled missionaries...");
```

Unity ports the +2 intensity reinforcement. The ward-profile-
triggered mutual hostility is deferred until the faith ward surface
ports. Conviction event recording is deferred until the conviction-
scoring lane exposes a mutation seam.

**Unity-side simplifications (deferred)**:
- Ward-profile-triggered mutual hostility on failure (browser
  simulation.js:5567): requires the faith ward profile surface which
  is not yet ported. On failure Unity applies the +2 intensity bump
  but omits the ward-triggered hostility.
- Conviction event recording (browser
  oathkeeping/desecration/stewardship +1): requires ConvictionComponent
  mutation owned by the conviction-scoring lane. Deferred.
- Legitimacy-penalty fallback on success when no control points
  exist (browser adjustLegitimacy at simulation.js:10493-10495):
  Unity DynastyStateComponent.Legitimacy mutation is owned by the
  dynasty-core lane. Deferred. Unity falls back to a no-op when
  target owns zero control points.
- Exposure-threshold crossing narrative (browser simulation.js:5544-5549):
  emit a narrative line when the exposure crosses 100
  (FAITH_EXPOSURE_THRESHOLD). Constant is declared on
  AIMissionaryResolutionSystem but the crossing narrative is deferred
  to a follow-up cosmetic slice since it does not block the core
  resolution logic.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` - added `AIMissionaryResolutionSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` - added
  `BloodlinesMissionaryResolutionSmokeValidation.cs`.

### Cross-Lane Reads (reads + writes)

- Reads `DynastyOperationComponent` + `DynastyOperationMissionaryComponent`
  entities as the operation expiry query. Mutates parent
  `DynastyOperationComponent.Active` field in place when flipping to
  false.
- Reads `DualClockComponent.InWorldDays` for expiry comparison. No
  mutations.
- Reads `FactionComponent.FactionId` to resolve target faction entity.
- Reads `FaithStateComponent.SelectedFaith` on target to gate
  intensity erosion. Mutates `FaithStateComponent.Intensity` on success
  (clamped >= 0) and failure (+2 reinforcement).
- Reads `FaithExposureElement` buffer on target. Mutates existing
  entries (update Exposure + Discovered) or appends new entries.
- Reads `ControlPointComponent.OwnerFactionId` and `.Loyalty` to
  pick the lowest-loyalty CP. Mutates Loyalty in place (clamped
  [0, 100]).

### New dedicated smoke validator

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMissionaryResolutionSmokeValidation.cs`
  8-phase validator:
  - **PhaseNotExpiredSkips**: ResolveAt > current DualClock;
    Active remains true, no effects, no narrative.
  - **PhaseExpiredSuccessApplies**: SuccessScore=10, ResolveAt<=current;
    Active->false; exposure 10->30; intensity 50->45; loyalty 80->77;
    narrative +1.
  - **PhaseExpiredFailureStrengthens**: SuccessScore=-5; Active->false;
    target intensity 50->52 (+2 reinforcement); exposure and loyalty
    unchanged.
  - **PhaseVoidOnMissingTarget**: target faction entity absent;
    Active->false; void narrative pushed.
  - **PhaseExposureAppendsNewEntry**: no prior exposure entry; new
    entry appended with Discovered=true and Exposure=ExposureGain.
  - **PhaseExposureClampsAt100**: existing exposure 95 + gain 20 ->
    clamped to 100.
  - **PhaseLowestLoyaltyControlPointTargeted**: target owns two CPs
    at 80 and 50; only the 50-loyalty CP reduced by LoyaltyPressure
    (50->47), high-loyalty CP (80) unchanged.
  - **PhaseIntensityErosionSkippedWhenSameFaith**: target and source
    both committed to same CovenantId; IntensityErosion NOT applied;
    intensity remains 50.

  Artifact: `../artifacts/unity-missionary-resolution-smoke.log`
  Marker: `BLOODLINES_MISSIONARY_RESOLUTION_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityMissionaryResolutionSmokeValidation.ps1`
  Standard PowerShell runner.

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
10. Contract staleness check - revision 41 current

Bundle 4 dedicated smoke
(`Invoke-BloodlinesUnityMissionaryResolutionSmokeValidation.ps1`):
all 8 phases PASS.

No Bundle 1/2/3 or sub-slice 8-18 regression expected because Bundle
4 is additive-only. Sub-slice 20's DynastyOperationMissionaryComponent
gains its first consumer. Sub-slice 18's DynastyOperationComponent
gains a new consumer shape that reads (not writes) via Active mutation.

---

## Key Design Notes

**Single sub-slice bundle rationale.** Per-kind resolution systems
differ substantively across operation kinds:
- Missionary: exposure + intensity + loyalty effects on target.
- Holy war: materialize activeHolyWars + war-tick pulses over 180 in-world days.
- Divine right: apex faith claim vs failure cooldown + legitimacy penalty.
- Captive rescue: roll against ProjectedChance + captive status flip.
- Captive ransom: unconditional status flip + gold deduction.

Bundling them would couple distinct effect logic without saving
code. Bundle 4 ships missionary alone because it has the fewest
deferred dependencies (all input surfaces already exist in Unity).
Bundle 5 could ship holy war + divine right resolution; Bundle 6
captive rescue + ransom resolution; these bundles ship later when
their respective per-kind resolution logic lands.

**Resolution walker pattern.** AIMissionaryResolutionSystem establishes
a pattern future per-kind resolution systems can reuse:
1. `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(PerKindExecutionSystem)]`
2. Query (DynastyOperationComponent, DynastyOperationXComponent) entities.
3. Filter by Active==true + OperationKind==X + ResolveAtInWorldDays <= current.
4. For each match: resolve, flip Active=false, push narrative.

The UpdateAfter relationship ensures a dispatched operation cannot
resolve in the same tick (there would be a data-race between
execution creating the entity and resolution reading it).

**Deterministic resolution.** Browser missionary uses
`successScore >= 0` as the success gate (simulation.js:5527). Unity
matches. No randomness is introduced because the SuccessScore is
computed deterministically at dispatch time (sub-slice 20) from
operator renown + source intensity + doctrine bias + target intensity.

**Intensity erosion gate.** Browser applyMissionaryEffect only erodes
target intensity when target has a different committed faith
(simulation.js:10485-10488: `if (targetFaction.faith.selectedFaithId
&& targetFaction.faith.selectedFaithId !== sourceFaithId)`). Unity
matches via the `targetFaith.SelectedFaith != CovenantId.None &&
targetFaith.SelectedFaith != faithId` gate. This prevents missionaries
of the same faith as the target from eroding the target's intensity,
which would make no narrative sense.

**Lowest-loyalty control point pick.** Browser picks by iterating
`state.world.controlPoints` for the target and tracking the lowest
`loyalty` (simulation.js getLowestLoyaltyControlPoint). Unity walks
every ControlPointComponent entity, matches by OwnerFactionId, and
tracks the lowest Loyalty. Runs in a single pass.

**Faith id round-trip.** DynastyOperationMissionaryComponent carries
SourceFaithId as FixedString64Bytes (the string form, e.g. "old_light"),
mapped from the CovenantId enum at dispatch time (sub-slice 20's
SelectedFaithIdString). Resolution parses it back to CovenantId via
ParseFaithId. The string round-trip is retained because the
FixedString64Bytes form is the canonical serialization across the
dynasty operation surface; all per-kind components use it.

**Narrative tone routing.** Matches sub-slice 20's dispatch
pattern: Good when source is player and success, Warn when target is
player (either success or failure), Info otherwise. Sensible
defaults: player learns they had a successful offensive operation
(Good), or that a hostile missionary just breached their court
(Warn).

---

## Current Readiness

Branch `claude/unity-ai-missionary-resolution` is ready to merge.
All gates green, contract at revision 41, continuity files updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Bundle 5 candidates (next session's scope):
   - **Holy war + divine right resolution** (sub-slices 26 + 27):
     holy war resolution materializes an active holy war entity
     (Unity equivalent of faction.faith.activeHolyWars), applies
     war-tick pulses until WarExpiresAtInWorldDays, and flips
     Active=false. Divine right resolution evaluates recognition
     share at ResolveAtInWorldDays (or simpler Unity-side flag for
     now since recognition share surface is not ported) and triggers
     apex-faith-claim success or failure cooldown + legitimacy
     penalty.
   - **Captive rescue + ransom resolution** (sub-slices 28 + 29):
     rescue rolls against ProjectedChance and on success flips
     captive status to Released via CapturedMemberHelpers.ReleaseCaptive
     plus source-faction member roster restoration. Ransom
     unconditionally flips to Released (ProjectedChance=1.0) plus
     returns member to source roster.
3. Codex fortification-siege lane: sub-slice 9 destroyed-counter
   recovery remains Codex's recommended next slice.
