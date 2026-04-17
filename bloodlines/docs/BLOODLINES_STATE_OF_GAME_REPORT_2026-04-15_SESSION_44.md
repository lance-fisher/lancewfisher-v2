# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 44
Author: Claude

## Scope

Defected branch spawns a real minor faction on the world register. Converts the Session 43 defection event from an internal bookkeeping status flip into a visible consequence at faction scope: when a lesser house defects, a new entry appears in `state.factions` with `kind: "minor_house"`, hostile to the parent, carrying the founder forward as its head of bloodline, inheriting the parent's faith commitment, and recording full provenance back to the lesser-house record that bore it. Survives save/restore round-trip. First canonical layer; territory, units, AI, marriage candidacy, and economy remain explicitly documented follow-ups.

## Changes Landed

### `spawnDefectedMinorFaction(state, lesserHouse, parentFaction)` (simulation.js)

Called from the defection block inside `tickLesserHouseLoyaltyDrift` when a lesser house crosses the grace window. Creates a full faction entry conforming to the same shape as `createSimulation`'s map-seeded factions:

- `id`: `minor-${lesserHouse.id}`, deterministic.
- `kind`: `"minor_house"` (new category, gracefully skipped by all kingdom-only iteration paths: `tickSiegeSupportLogistics`, `tickRealmConditionCycle`).
- `hostileTo`: `[parentFactionId]` (canonical: departed with grievance).
- `presentation`: copies parent's primary/accent colors so the minor reads as visually related.
- `resources`: zero baseline across all ten resource pills.
- `dynasty`: minimal but full-shape (marriages, marriageProposalsIn/Out, lesserHouses, lesserHouseCandidates arrays all present so existing ticks don't crash). Seeded with a COPY of the founder member tagged with `originFactionId` and `originMemberId` back-references. Copy semantics (not move) preserves the parent's dynasty register, renown ledger, and any outstanding commander attachments.
- `faith`: inherits parent's `selectedFaithId` and `doctrinePath` at intensity 20 (canonical: founder worshipped under parent's covenant, now establishing independently).
- `conviction`: neutral band, empty buckets, seed ledger entry `minor_house_founded_from_defection`.
- `population`: total 0, cap 0 (no units on the map yet; future layer).
- `ai`: null (future layer).
- New provenance fields: `originFactionId`, `originLesserHouseId`, `foundedAtInWorldDays`.

Mirror back-reference: `lesserHouse.defectedAsFactionId = minorId` so UI and snapshot can reconstruct the lineage.

Idempotency guard: if the minor id already exists on `state.factions`, return early without re-spawning.

### Defection hook updated

Single new line inside the grace-window-elapsed branch of `tickLesserHouseLoyaltyDrift`: `spawnDefectedMinorFaction(state, lh, f);`. All prior effects (status flip, dual-clock timestamp, legitimacy -6, ruthlessness +1, canonical message) preserved.

### Snapshot / restore compatibility

- `exportStateSnapshot` now captures `kind`, `originFactionId`, `originLesserHouseId`, `foundedAtInWorldDays` per faction so minor-house identity persists.
- `restoreStateSnapshot` handles the case of a snapshot faction that `createSimulation` did not rebuild: if `state.factions[factionId]` is missing, a minimal shell is constructed on the fly, then all fields are applied. Without this, minor houses would silently evaporate on reload. Existing factions still restored field-by-field as before.

### Legibility (main.js, Dynasty panel)

- Active lesser-house rows now show `DEFECTED` status explicitly when `lh.status === "defected"` instead of the loyalty readout (which is now meaningless).
- New section "Rival minor houses (N)" iterates `state.factions` for `kind === "minor_house"` entries whose `originFactionId === "player"`. Lists each with display name + head of bloodline if present. Surfaces for the player the direct consequence: their neglect of the cadet branch has produced a hostile actor.

### Test coverage (runtime-bridge.mjs Session 44 block)

- Engineer candidate + promotion, force loyalty collapse with weak parent, push past grace window.
- Assert: `lh.status === "defected"`, minor faction exists at `state.factions[minor-<lh.id>]`, `kind === "minor_house"`, origin fields correct, hostile to parent, dynasty head is `head_of_bloodline`.
- Assert founder member is COPIED (parent's original still present).
- Assert `lh.defectedAsFactionId` equals the minor id.
- Assert idempotency: further ticks do not double-spawn.
- Assert save/restore round-trip preserves the minor faction including kind, origin, hostility.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full S44 block + all S33-S43 tests still green).
- `node --check` on `main.js`, `simulation.js`, `renderer.js`, `ai.js`, `data-loader.js`, `utils.js` — all passed.

## Canonical Interdependency Check

Defected-branch-as-minor-faction now connects to:

1. **Lesser-house system (S35-S43)**: defection event is the trigger; `lh.defectedAsFactionId` provides back-reference.
2. **Faction register**: new `state.factions` entry; all iterators (marriage, lesser-house candidate detection, save/restore) see it.
3. **Hostility tracking**: `hostileTo: [parent]` makes the minor immediately a hostile actor recognized by every existing hostility-sensitive code path.
4. **Faith system**: minor inherits parent's faith commitment (Vector 4).
5. **Conviction system**: minor has fresh conviction state with founding-event ledger entry.
6. **Dynasty system**: founder is present in both parent (original) and minor (copy-with-provenance); marriage / renown / promotion history invariants preserved on both sides.
7. **Dual-clock**: `foundedAtInWorldDays` timestamps the spawn.
8. **Save/resume**: full round-trip verified.
9. **Legibility (Vector 6)**: Dynasty panel surfaces both the defected lesser-house row AND the rival-minor-house list.

Satisfies Canonical Interdependency Mandate dramatically (≥8 live systems touched).

## Vectors Advanced This Session

- **Vector 2 (Dynastic)**: minor-house provenance connects cadet-branch lifecycle to faction-scope consequences.
- **Vector 5 (World)**: new actors appear on the faction register.
- **Vector 6 (Legibility)**: rival-minor-house section surfaces defection impact.
- **Vector 3 (Military) FOUNDATION**: minor faction is recognized as hostile by existing hostility queries, though it has no units yet (explicitly deferred).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Defected-branch as new minor faction | DOCUMENTED | LIVE (registry spawn + hostility + founder copy + faith inheritance + save/restore + legibility) |
| Minor-house presence on world register | NOT-STARTED | FIRST-LAYER LIVE |
| Minor-house territory, units, AI, economy, diplomacy | DOCUMENTED | DOCUMENTED (explicit follow-up layers for future sessions) |

## Lesser-House System Status After S35-S44

| Layer | Status |
|---|---|
| Candidate detection | LIVE (S35) |
| Player promotion | LIVE (S35) |
| AI promotion | LIVE (S38) |
| Battlefield-driven renown accumulation | LIVE (S40) |
| Loyalty drift | LIVE (S42) |
| Defection event hook | LIVE (S43) |
| Defected-as-minor-faction registry entry | LIVE (S44) |
| Minor-house territory | DOCUMENTED |
| Minor-house units | DOCUMENTED |
| Minor-house AI | DOCUMENTED |
| Minor-house economy | DOCUMENTED |
| Lesser/minor-house marriage candidacy | DOCUMENTED |

## Performance and Stability Gate

- No new tick-per-frame work; minor-faction iteration only hits during existing faction-loop ticks. Single `if (!f.dynasty?...)` or kingdom-kind guard skips unopened paths.
- No new listeners, no new render subscriptions, no unbounded arrays.
- Snapshot deep-clones each faction individually — size grows linearly with defection count, bounded by the number of lesser houses which is itself bounded by the LESSER_HOUSE_QUALIFYING_PATHS filter on dynasty members.

## Session 45 Next Action

- Minor-house territorial footprint: on spawn, claim the tile region around the founder's last known location OR a specific map region seeded for splinter settlements. Vector 5 continues.
- Or: Minor-house founder unit spawn (the founder appears as a commander unit on the map with the minor's colors). Vector 3 advance.
- Or: Mixed-bloodline defection slider runtime (S33 children carry `mixedBloodline` metadata; runtime effect still DOCUMENTED).
- Or: Faith-compatibility weighting in AI marriage decisions. Vector 4 advance.
- Or: Marriage death/dissolution mechanics.
- Or: Vector 1 Civilizational advance (water/food infrastructure decay or local development feedback).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. 1 item moved NOT-STARTED → FIRST-LAYER LIVE. Tests green. Defection consequence is now visible across the full simulation: registry, hostility graph, dynasty legibility, save/restore. The vassal-politics loop is complete on its first canonical layer.
