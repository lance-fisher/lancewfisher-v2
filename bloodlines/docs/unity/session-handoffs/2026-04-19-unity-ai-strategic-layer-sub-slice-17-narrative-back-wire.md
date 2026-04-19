# Unity AI Strategic Layer Sub-Slice 17: Narrative Back-Wire

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-narrative-back-wire`
**Lane:** ai-strategic-layer
**Contract Revision:** 33
**Master Base:** `8826d855` (after sub-slice 16 narrative message bridge landed at revision 32)

---

## Goal

Sub-slice 16 landed the `NarrativeMessageBridge.Push` producer surface
and wired `PactBreakSystem` (sub-slice 15) as the first consumer.
Sub-slices 11, 12, 13, and 14 all deferred their ceremonial pushes
because the bridge did not exist yet. Sub-slice 17 back-wires the
three remaining AI systems that produce browser-visible narrative
lines, so the producer surface carries the full browser parity set
at parity with the canonical ceremonial-message surface:

1. `AIMarriageAcceptEffectsSystem` (sub-slice 11+12) — marriage accept
   ceremonial line with authority-mode suffix.
2. `AILesserHousePromotionSystem` (sub-slice 13) — founding line.
3. `AIPactProposalExecutionSystem` (sub-slice 14) — non-aggression
   pact entry line.

Each wire-up is a small additive edit following the one-line push
pattern established by `PactBreakSystem` in sub-slice 16.

---

## Work Completed

### Shared-File Narrow Edits (AI lane-owned files)

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageAcceptEffectsSystem.cs`
  After all mechanical effects are applied and before the pending tag
  is removed, call `PushMarriageAcceptMessage(em, marriageEntity,
  marriage)`. The helper:
  - Resolves head and spouse titles via `DynastyMemberRef` buffer on
    the respective faction entities. Falls back to the raw `MemberId`
    when no roster is seeded (smoke fixtures without full dynasty
    seeding still produce a readable line).
  - Reads the `MarriageAcceptanceTermsComponent` (attached by
    `AIMarriageInboxAcceptSystem` in sub-slice 12) to build the
    authority suffix: HeadDirect → "under head approval", HeirRegency
    → "under heir regency (legitimacy -1)", EnvoyRegency → "under
    envoy regency (legitimacy -2)".
  - Tones Good when `HeadFactionId == "player"`, else Info. Browser
    hardcodes "good" at simulation.js:7466; sub-slice 17 applies the
    same source/target tone routing rule used by the other
    back-wired systems so routine AI-to-AI marriages do not flood the
    HUD with Good-toned notifications.

- `unity/Assets/_Bloodlines/Code/AI/AILesserHousePromotionSystem.cs`
  After the stewardship conviction bonus is applied, call
  `PushPromotionMessage(em, factionEntity, founderTitle)`. The helper
  reads the last-appended `LesserHouseElement` name (the one just
  created) and composes the browser line: `"<factionId> founds
  <lesserHouseName>, honoring <founderTitle>."` Tones Good when the
  faction is "player", else Info (matches browser simulation.js:7254
  tone ternary).

- `unity/Assets/_Bloodlines/Code/AI/AIPactProposalExecutionSystem.cs`
  After the `PactComponent` entity is created, call
  `PushPactEnteredMessage(em, sourceFactionId, targetFactionId)`. The
  helper composes: `"<source> and <target> enter a non-aggression
  pact. Hostility ceases for at least 180 in-world days."` Tones Good
  when source is "player", else Info. The dispatch hardcodes target
  to "player" so in production the Good branch is unreachable; it is
  kept defensively for future extensions that route player-source
  pact proposals through this system.

### New Files

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAINarrativeBackWireSmokeValidation.cs`
  Six-phase smoke validator:
  - **PhaseMarriageAcceptPlayerHeadDirect**: player→enemy marriage
    with HeadDirect terms; buffer gains exactly 1 message containing
    "weds" and "under head approval"; tone = Good.
  - **PhaseMarriageAcceptEnemyHeirRegency**: enemy→player marriage
    with HeirRegency terms (cost 1); message contains "heir regency"
    and "legitimacy -1"; tone = Info.
  - **PhaseMarriageAcceptEnemyEnvoyRegency**: enemy→player marriage
    with EnvoyRegency terms (cost 2); message contains "envoy
    regency" and "legitimacy -2"; tone = Info.
  - **PhaseLesserHousePromotionPlayer**: player faction with Governor
    candidate at Renown 40; buffer gains 1 message containing "founds"
    and "honoring"; tone = Good.
  - **PhaseLesserHousePromotionEnemy**: enemy faction same scenario;
    tone = Info.
  - **PhasePactProposalEnemy**: enemy source, player target (matches
    the browser dispatch convention); message contains "enter a
    non-aggression pact" and "180 in-world days"; tone = Info. The
    player-source case is intentionally not tested because
    `AIPactProposalExecutionSystem.OnUpdate` hardcodes the target to
    "player", so source="player" collides with target="player" and
    is filtered out by the same-faction guard. Keeping this phase as
    the only pact phase reflects the system's actual dispatch
    surface.

  Artifact: `../artifacts/unity-ai-narrative-back-wire-smoke.log`
  Marker: `BLOODLINES_AI_NARRATIVE_BACK_WIRE_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAINarrativeBackWireSmokeValidation.ps1`
  Standard PowerShell runner mirroring the sub-slice 16 wrapper.

### Shared-File Narrow Edits (lane-shared files)

- `unity/Assembly-CSharp-Editor.csproj` — added
  `BloodlinesAINarrativeBackWireSmokeValidation.cs`.

### Cross-Lane Reads (no new writes)

- Reads `DynastyMemberComponent.Title` via `DynastyMemberRef` buffer
  on faction entities during marriage title resolution. No
  mutations.
- Reads `MarriageAcceptanceTermsComponent.AuthorityMode` and
  `.LegitimacyCost` on primary marriage entities. No mutations.

---

## Browser Parity Notes

**Marriage accept ceremonial line** (simulation.js:7460-7467):

```js
const approvalText = terms.targetAuthority?.mode === "head_direct"
  ? `under ${terms.targetAuthority?.title ?? "household"} approval`
  : `under ${terms.targetAuthority?.title ?? "acting authority"} (${terms.targetAuthority?.label ?? "regency"}, legitimacy -${terms.legitimacyCost})`;
pushMessage(
  state,
  `${sourceMember.title} of ${source.displayName} weds ${targetMember.title} of ${target.displayName} ${approvalText}.`,
  "good",
);
```

Unity differences:
- Authority title strings ("head", "heir", "envoy" in browser) are
  not tracked in Unity's authority evaluator; the suffix uses
  canonical role words as stand-ins ("head approval", "heir regency",
  "envoy regency").
- `getFactionDisplayName(state, factionId)` has no Unity equivalent;
  `FactionComponent.FactionId` substitutes. A future display-name
  component can plug into the same call site.
- Browser tone is hardcoded "good"; Unity routes source=player → Good,
  else Info, matching the source/target tone rule applied across the
  other sub-slice 17 wire-ups.

**Lesser-house founding line** (simulation.js:7251-7255):

```js
pushMessage(
  state,
  `${faction.displayName} founds ${lesserHouse.name}, honoring ${member.title}.`,
  faction.id === "player" ? "good" : "info",
);
```

Unity preserves this shape exactly except for the displayName
substitution.

**Pact entry line** (simulation.js:5216-5220):

```js
pushMessage(
  state,
  `${getFactionDisplayName(state, factionId)} and ${getFactionDisplayName(state, targetFactionId)} enter a non-aggression pact. Hostility ceases for at least ${NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS} in-world days.`,
  factionId === "player" ? "good" : "info",
);
```

Unity preserves this shape exactly except for the displayName
substitution. `NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS`
reads from the system's `MinimumDurationInWorldDays` constant (180).

---

## Verification Results

All 10 canonical validation gates green, serialized per Unity's
project lock:

1. `dotnet build Assembly-CSharp.csproj -nologo` — 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` — 0 errors PASS
3. Bootstrap runtime smoke — PASS
4. Combat smoke — all 8 phases PASS
5. Canonical scene shells (Bootstrap + Gameplay) — both PASS
6. Fortification smoke — PASS
7. Siege smoke — PASS
8. `node tests/data-validation.mjs` — PASS
9. `node tests/runtime-bridge.mjs` — PASS
10. Contract staleness check — revision 33 current

Sub-slice 17 dedicated smoke
(`Invoke-BloodlinesUnityAINarrativeBackWireSmokeValidation.ps1`):
all 6 phases PASS.

Regression smokes for the four systems the wire-ups touch or depend
on, all re-run green to confirm the wire-ups are purely additive:

- `Invoke-BloodlinesUnityAIMarriageAcceptEffectsSmokeValidation.ps1` — PASS
- `Invoke-BloodlinesUnityAILesserHousePromotionSmokeValidation.ps1` — PASS
- `Invoke-BloodlinesUnityAIPactProposalExecutionSmokeValidation.ps1` — PASS
- `Invoke-BloodlinesUnityPactBreakSmokeValidation.ps1` — PASS
- `Invoke-BloodlinesUnityNarrativeMessageBridgeSmokeValidation.ps1` — PASS

Example back-wire smoke output (player head-direct marriage):

```
PhaseMarriageAcceptPlayerHeadDirect PASS:
  'player-heir of player weds enemy-heir of enemy under head approval.'
  tone=Good
```

---

## Key Design Notes

**Member title fallback.** When the faction roster has no matching
`DynastyMemberComponent` entity (synthetic smoke fixtures without
full dynasty seeding), `ResolveMemberTitle` returns the raw
`MemberId` FixedString64Bytes directly rather than walking byte-by-
byte. `FixedString64Bytes.Append(byte)` promotes through the
`Append(int)` overload and writes the byte as a decimal number, so
the naive fallback produced garbage output (`"112108..." instead of
"pl..."`). Direct assignment of the FixedString preserves the UTF-8
bytes verbatim.

**Pact proposal target hardcoded.** `AIPactProposalExecutionSystem`
hardcodes `targetFactionId = "player"` per the browser ai.js
dispatch (`proposeNonAggressionPact(state, "enemy", "player")`). As
a result, the source-faction guard filters out any source="player"
dispatch, and the Good-tone branch is defensively included for
future extensions that route player-initiated pact proposals through
this system. The back-wire smoke only exercises the enemy→player
path because that is the only reachable production path in the
current system.

**Authority suffix simplification.** The browser authority suffix
reads titles from `terms.targetAuthority.title` and a label from
`terms.targetAuthority.label`; neither is stored in Unity's
`MarriageAcceptanceTermsComponent` (which only carries mode and
cost). The suffix uses canonical role words ("head", "heir",
"envoy") as stand-ins. If a future slice adds authority labels to
the terms component, the suffix builder can pull them without
reshaping the push path.

**Tone routing rule.** Unity applies the browser source/target tone
rule uniformly across the four sub-slice 17 wire-ups: the
player-involved party promotes the message to Good, else Info. The
PactBreakSystem wire-up from sub-slice 16 uses `Warn` for
player-breaker because breaking an oath carries negative valence;
the sub-slice 17 pushes are neutral/positive events so Good/Info
covers the valence space.

---

## Current Readiness

Branch `claude/unity-ai-narrative-back-wire` is ready to merge. All
gates green, contract at revision 33, continuity files updated.

---

## Next Action

1. Merge via the merge-temp ceremony.
2. Next candidates:
   - **Dynasty operations foundation** (sub-slice 18):
     `DynastyOperationComponent` entity shape + the
     `DYNASTY_OPERATION_ACTIVE_LIMIT` gate. Unblocks missionary, holy
     war, divine right, captive rescue, and captive ransom execution
     slices.
   - **Captive member state** (sub-slice 19): port the browser
     `captives` array on faction and the `CapturedMemberRecord`
     shape as a Unity buffer. Needed for captive rescue/ransom
     execution.
   - **Narrative TTL eviction system**: walk the
     `NarrativeMessageElement` buffer each tick and remove entries
     whose `CreatedAtInWorldDays + Ttl` is past current. Keeps the
     buffer bounded without a UI consumer yet. Small, self-contained.
3. Codex fortification-siege lane: breach assault pressure
   (sub-slice 6) is in flight on
   `codex/unity-fortification-breach-assault-pressure` locally but
   has not yet reached origin/master.
