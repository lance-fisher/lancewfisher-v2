# MASTER_BLOODLINES_CONTEXT

This file gives a full-spectrum context restore for future sessions without replacing the actual source materials.

## Project Identity

Bloodlines is a dynasty-driven strategy game built at epic scale. The project combines:

- a preserved design corpus and additive canon archive
- a frozen browser RTS behavioral specification
- a Unity shipping lane
- deep long-form lore, systems, and faction design
- prompts, continuity material, planning docs, and preserved historical variants

The design direction is explicitly not MVP-first. Scope reduction is not the goal. Depth, preservation, and continuity are the goal.

## 2026-04-16 Owner Direction Override

The active non-negotiable direction is recorded at:

- `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`

This direction hardens the following:

- Bloodlines ships as the full canonical no-compromise realization of the design bible
- Unity 6.3 LTS with DOTS / ECS is the shipping engine
- `unity/` is the only active Unity work target
- the browser runtime is frozen as behavioral specification only
- MVP, phased-release reduction, and scope-cut recommendations are stale and forbidden
- if a task is broad, the correct continuation move is the next concrete Unity implementation step, not scope reduction

## Canonical Root

- Canonical folder to target: `D:\ProjectsHome\Bloodlines`
- Filesystem backing path today: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Compatibility-only copy preserved in-root: `archive_preserved_sources/2026-04-14_deploy-bloodlines-compatibility-copy/`

Future sessions should enter `D:\ProjectsHome\Bloodlines` and stay there.

## What Lives Here Now

### Design and canon

The numbered directories `00_ADMIN` through `19_ARCHIVE` preserve the design corpus, lore, systems, prompts, exports, assets, research, and historical variants.

### Browser runtime

The current browser prototype lives in:

- `play.html`
- `src/`
- `data/`
- `tests/`

This runtime is now frozen as behavioral specification only. It remains useful for feel, sequencing, pressure interaction, and AI-behavior reference, but it is no longer an active lane for new system additions.

Recent live implementation direction includes:

- dynasty consequence cascade
- commander capture versus death
- succession cascade and heir handling
- captive ledger and fallen ledger
- doctrine path effects
- conviction bucket ledger
- occupation versus stabilized territory states
- doctrine hardening around population, water, logistics, world scale, and UI legibility

### Unity lane

Unity migration and continuation work lives in:

- `unity/`
- `docs/unity/`

Unity 6.3 LTS with DOTS / ECS is the shipping lane.

### Preservation and provenance

Outside source roots that mattered have been imported into:

- `archive_preserved_sources/`
- `governance/`

The `deploy/bloodlines` compatibility copy is not just archived now. Its deploy-only delta has also been reconciled into the active canonical tree so forward work can continue from this root without needing to consult deploy for latest files.

The project now also contains a preserved external doctrine ingestion:

- preserved DOCX artifacts under `archive_preserved_sources/2026-04-14_downloads_bloodlines_design_doctrine_docx/`
- raw extraction and appendix under `02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/`
- canonical readable doctrine source at `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`

### Continuity

Cross-session continuation now lives in:

- `AGENTS.md`
- `README.md`
- `CLAUDE.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `SOURCE_PROVENANCE_MAP.md`
- `CONSOLIDATION_REPORT.md`
- `continuity/`

## Current Design Priorities

From the active plan, current canon state, and active owner direction:

- Unity 6.3 LTS with DOTS / ECS in `unity/` is the only shipping lane.
- The full design bible remains in scope, including all nine founding houses, full canonical rosters, late-game arcs, Wwise audio polish, and Netcode for Entities multiplayer unless Lance later removes it.
- Fortification and siege doctrine is canonically locked and implementation is the next major runtime lane.
- The 2026-04-14 master doctrine now hardens Bloodlines' continuity around water, population, supply, commanders, continents, naval identity, and player legibility.
- Dynasty consequences are already partially live and now need expansion into rescue, ransom, governance specialization, and keep-level defensive leverage.
- Faith doctrine and conviction are no longer just documents, but their runtime depth still needs broadening.
- Bloodlines remains a design-rich archive with far more material than the current runtime implements.

## Most Important Current Source Files

### Governance and continuity

- `AGENTS.md`
- `README.md`
- `CLAUDE.md`
- `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `SOURCE_PROVENANCE_MAP.md`
- `CONSOLIDATION_REPORT.md`

### Project operations

- `00_ADMIN/PROJECT_STATUS.md`
- `00_ADMIN/CHANGE_LOG.md`
- `tasks/todo.md`
- `tasks/lessons.md`
- `continuity/CURRENT_STATUS_AND_NEXT_STEPS.md`

### Canon

- `01_CANON/CANONICAL_RULES.md`
- `01_CANON/CANON_LOCK.md`
- `01_CANON/BLOODLINES_MASTER_MEMORY.md`
- `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
- `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
- `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md`
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
- `docs/BLOODLINES_SYSTEM_CROSSWALK_2026-04-14_DOCTRINE_INGESTION.md`

### Analysis and roadmap

- `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`
- `docs/DEFINITIVE_DECISIONS_REGISTER.md`
- `docs/DEVELOPMENT_REALITY_REPORT.md`
- `docs/IMPLEMENTATION_ROADMAP.md`
- `docs/COMPLETION_STAGE_GATES.md`

## Known Continuity Risks

### Historical root confusion

Older docs and prior sessions referred to different canonical locations:

- `D:\ProjectsHome\Bloodlines`
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines`

This is now resolved by governance: only `D:\ProjectsHome\Bloodlines` is the canonical session target.

### Preserved contradictory statements

Some older files still state that `deploy/bloodlines` was canonical. Those statements are preserved historically. They are not deleted. They are superseded by the 2026-04-14 governance layer.

### Preserved competing versions

The project intentionally preserves parallel variants, including:

- early prompt bundles
- preconsolidation snapshots
- `frontier-bastion` predecessor material
- deploy compatibility copy
- archived CB004 house material

These are not errors. They are preserved context and must stay traceable.

## How To Continue Safely

1. Read the root governance and continuity files first.
2. Work from `D:\ProjectsHome\Bloodlines`, not from the physical backing path or deploy copy.
3. Treat `archive_preserved_sources/` as preserved history, not as the default edit target.
4. Put new canon in the numbered canon folders.
5. Put new shipping implementation in `unity/` and related implementation docs. Touch `src/`, `data/`, and `tests/` only to preserve, validate, or clarify the frozen browser reference simulation when Unity parity work needs it.
6. If you find outside Bloodlines material, import it and update the provenance map.
7. Before ending, update project state and handoff files.

## What Not To Do

- Do not create a new random Bloodlines root.
- Do not flatten the design into a smaller game because the runtime is behind the canon.
- Do not add new systems to the browser reference simulation.
- Do not work in any Unity project other than `unity/` inside the Bloodlines root.
- Do not treat preserved alternates as disposable.
- Do not overwrite richer design files with thinner summaries.
- Do not silently change root identity again.

## Current Working Truth

Bloodlines is already both a preserved design archive and an active implementation project. The correct continuation model is additive convergence:

- preserve all prior material
- keep provenance visible
- extend live implementation toward canon through Unity
- keep future sessions anchored to one root
- keep the 2026-04-14 master doctrine in view when making design or implementation decisions

---

## 2026-04-14 Session 9 Addendum — Full-Realization Continuation

This addendum is additive. It does not replace any prior section of this file. It extends the master context to record the session 9 full-project state analysis, the system gap analysis, the continuation plan, and the next-phase execution roadmap produced under the creator's full-realization continuation directive.

### Session 9 Context

Session 9 was launched under a full-project state analysis and continuation directive that reaffirms the 2026-04-14 Master Design Doctrine preservation posture. The directive explicitly prohibits scope reduction, MVP framing, compression, and thinning of Bloodlines into a smaller or more generic substitute. The expected outputs were:

1. A Bloodlines State Analysis document.
2. A Full Project Continuation Plan.
3. A System Gap Analysis.
4. A Next-Phase Execution Roadmap.
5. An updated master continuation reference.
6. Continuation of implementation work after the analysis documents were produced.

All six were produced. The corresponding files are:

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md`
- `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
- `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`
- `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`
- this addendum (inside `MASTER_BLOODLINES_CONTEXT.md`)
- the session 9 implementation (ballista + mantlet siege-support classes and 11-state realm dashboard) recorded in `00_ADMIN/CHANGE_LOG.md`, `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json`.

### Protected Scope Reaffirmed

Session 9 reaffirms the full protected canonical scope of Bloodlines. The six growth vectors are:

1. Civilizational depth (population, water, food, land, forestry, supply, logistics).
2. Dynastic depth (bloodline members as real actors, marriage, succession, lesser houses, captivity consequence).
3. Military depth (land and naval doctrine axes, commanders and generals, delegated and direct command, fortification, siege).
4. Faith and conviction depth (four covenants with Light/Dark paths and 5-tier ladders, independent conviction spectrum, irreversible late-game divergence).
5. World depth (continental geography, five-stage match, multi-speed time with declared dual-clock, world pressure, Trueborn late return, up to 10 players plus AI kingdoms plus minor tribes).
6. Legibility depth (theatre-of-war strategic zoom, 11-state realm dashboard, ward iconography, governor iconography, naval-and-continental visual awareness, keep interior as core UI anchor).

These six vectors are the growth frame. No vector may lag indefinitely. No vector may be reduced to fit runtime constraints.

### Session 9 Implementation Wave Summary

Session 9 advanced Vector 6 (Legibility) and Vector 3 (Military) in the browser reference simulation lane:

- Full 11-state realm-condition dashboard: HUD expanded from 6 pills to 11 pills, aligned with `getRealmConditionSnapshot` and covering cycle, population, food, water, loyalty, fortification, army, faith, conviction, logistics, and world-pressure.
- Ballista and mantlet siege-support classes added: ranged anti-personnel (ballista) and mobile cover (mantlet). Trained at `siege_workshop`. Participate in siege supply continuity. Renderer gives them distinct silhouettes. Stonehelm AI siege-line preparation extended to include both.

Tests remained green. Data validation passed. Runtime bridge passed. All simulation modules passed `node --check`.

### Canonical Next Directions After Session 9

Per the execution roadmap, the next-session priorities are:

- Session 10: sabotage operation type added to `dynasty.operations`; commander keep-presence expanded.
- Session 11: Stonehelm becomes second playable house with minimal house-select seam; Hartvale Verdant Warden entered into data.
- Session 12: longer-siege AI adaptation (relief-window awareness, repeated-assault windows, post-repulse adjustment).
- Session 13: Unity version decision landed (recommended 6.3 LTS); Unity JSON content sync; first ECS components.

The Unity production lane remains structurally prepared and awaits the version alignment decision. The browser reference simulation lane continues as working spec.

### Preservation Statement

Nothing has been reduced. The design corpus remains additive. The nine founding houses, four faiths, six vessel classes, continental architecture, five-stage match, dual-clock, multi-speed time, Decision 21 fortification doctrine, and all other canonical pillars remain protected.

Future sessions should continue this discipline. The project is being built toward the full intended grandeur, not trimmed into convenience.

---

## 2026-04-15 Sessions 46-47 Addendum

This addendum is additive. It records the continuation wave after the Session 45 minor-house founder-unit layer.

### Session 46

Session 46 completed the unfinished civilizational feedback seam inside the canonical 90-second realm cycle. Food and water abundance no longer matter only by their absence. When a faction materially exceeds both surplus thresholds and is not under cap pressure, owned marches gain loyalty through the same territorial pathway used by famine, water crisis, and cap-pressure erosion. This means civilizational health now has a positive governance expression, not only a punitive one.

### Session 47

Session 47 made breakaway lesser houses territorial actors. A defected branch already had faction identity and a founder militia from Sessions 44 and 45. It now also spawns a stabilized border march with first-layer resource trickle, visible ownership in the world-control layer, and persistence through snapshot export and restore. This closes the gap where a breakaway house existed in state but not as a durable territorial claimant.

### Immediate Continuation Direction

The next highest-leverage action after Sessions 46 and 47 is minor-house AI activation and territorial defense. The breakaway branch now has identity, founder, and territory. It still needs autonomous behavior, territorial reaction, and local defensive intent so it stops being a passive consequence and becomes an operational world actor.

---

## 2026-04-15 Sessions 48-49 Addendum

This addendum is additive. It records the continuation wave after the minor-house territorial foothold became live.

### Session 48

Session 48 made breakaway cadet branches operational. A `minor_house` faction with a founder and claimed march no longer sits passively on the map. Minor-house AI now detects nearby hostile pressure, attacks to defend the claimed march, retakes the march if dispossessed, and regroups toward the claim after pressure clears. The dynasty panel exposes retinue count and posture so the player can read whether the breakaway branch is merely holding, actively defending, or fighting to recover its foothold.

### Session 49

Session 49 hardened long-form continuity for the restore lane. The snapshot restore path had been rebuilding an obsolete `entityIdCounter`, but live runtime creation had already moved to prefix-based `state.counters`. Restore now rebuilds those live counters for buildings and dynasty-side ids. This matters because future continuation waves, especially minor-house retinue growth and other dynamic spawning, now need post-restore id minting to stay collision-safe.

### Immediate Continuation Direction

The next highest-leverage action after Sessions 48 and 49 is breakaway-march territorial levy and local retinue growth. The minor house can now hold, defend, and restore its claim. The next canonical layer is to let the held march raise or attract additional local force so the polity stops being a one-unit actor and starts behaving like a fragile but growing border realm.

---

## 2026-04-15 Session 50 Addendum

This addendum is additive. It records the first territorial levy layer for breakaway cadet branches.

### Session 50

Session 50 made the held breakaway march a manpower source rather than only a map marker plus trickle node. A `minor_house` that still controls its claimed march, maintains sufficient local loyalty, and accumulates enough food and influence can now raise additional retinue directly from that territory. Levying is not free abstraction. It consumes food and influence, reduces local loyalty, stops when the march is contested or too unsettled, and expands into a real battlefield combat unit with save and restore continuity.

### Immediate Continuation Direction

The next highest-leverage action after Session 50 is mixed-bloodline defection weighting inside the lesser-house instability pipeline. The branch can now exist, defect, claim, defend, and grow. The next canonical layer is to make bloodline mixture itself matter to how loyal or unstable cadet branches become under dynastic pressure.

---

## 2026-04-15 Session 51 Addendum

This addendum is additive. It records the first live mixed-bloodline instability layer.

### Session 51

Session 51 connected marriage-born bloodline mixture to cadet-branch instability. Mixed-bloodline children already existed as metadata from the marriage system. They now carry that provenance into lesser-house promotion, and the resulting cadet branch no longer drifts like a same-house branch. Active marriage ties to the outside house soften the instability pull, while renewed hostility toward that house worsens it. This makes marriage, child generation, diplomacy, and lesser-house instability interact as one dynastic system instead of four disconnected records.

### Immediate Continuation Direction

The next highest-leverage action after Session 51 is faith-compatibility weighting in AI marriage proposal and acceptance logic. Marriage reciprocity is live, mixed-bloodline consequences are live, and the next canonical pressure layer is covenant and doctrine fit.

---

## 2026-04-15 Session 52 Addendum

This addendum is additive. It records the first live covenant-fit layer inside AI dynastic diplomacy.

### Session 52

Session 52 made AI marriage diplomacy faith-aware. Covenant and doctrine identity no longer live only in faith commitment, ward behavior, and conviction text. AI marriage proposal and acceptance now read a shared compatibility profile derived from real selected covenant and doctrine path. Harmonious or sectarian matches can now open a legitimacy-repair marriage path when the enemy house is dynastically weak enough. Fractured matches now block weak one-signal diplomacy and force stronger strategic pressure before the union is considered acceptable. The dynasty panel now exposes covenant stance and enemy-court posture for pending offers so this pressure is readable.

### Immediate Continuation Direction

The next highest-leverage action after Session 52 is marriage death and dissolution with legitimacy and dynastic consequence. Covenant filtering is now live at the front door of the marriage system. The next canonical layer is to let bloodline death dissolve a union, alter legitimacy and conviction, and remove the buffering effects that an active marriage can provide elsewhere in the dynasty.

---

## 2026-04-15 Session 53 Addendum

This addendum is additive. It records the first live death-consequence layer inside the marriage system.

### Session 53

Session 53 ended the previous immortal-marriage state. A cross-dynasty union no longer survives indefinitely once a spouse dies in runtime. Real bloodline death now dissolves the existing marriage record pair, applies legitimacy loss to both houses, records an oathkeeping mourning response in the conviction ledger, halts further gestation on the dissolved union, and surfaces the death-ended marriage in the dynasty panel. This also means active-marriage buffering for mixed-bloodline cadet drift now falls away automatically when the union is broken by death, because that buffer already keyed off `!dissolvedAt`.

### Immediate Continuation Direction

The next highest-leverage action after Session 53 is water-infrastructure tier 1 and field-army water sustainment beyond the siege-engine chain. The dynastic lane now has a real mortality consequence layer. The next civilizational and military pressure seam is to make water infrastructure and campaigning armies interact directly in live runtime.

---

## 2026-04-15 Session 54 Addendum

This addendum is additive. It records the first live campaign-water sustainment layer.

### Session 54

Session 54 made water matter to armies in motion. Water was already live as a realm stockpile, civilizational pressure source, and settlement-facing crisis trigger. It now also exists as a local sustainment network for campaigning land forces. Owned marches, owned settlements, wells, supply camps, and camp-linked wagons now provide real hydration support. Armies that outrun that network accumulate dehydration strain, move slower, strike weaker, surface that failure in the logistics dashboard, and can force Stonehelm to delay an assault until the column regroups around a water anchor.

### Immediate Continuation Direction

The next highest-leverage action after Session 54 is additional covert-operation depth, specifically assassination and espionage. The water lane now has a real campaign consequence layer. The next adjacent dynastic and military seam is to let covert operations threaten bloodline safety, command structure, legitimacy, and AI response through the already-live `dynasty.operations` architecture.

---

## 2026-04-15 Session 55 Addendum

This addendum is additive. It records the first live bloodline-targeted covert-pressure layer.

### Session 55

Session 55 extended `dynasty.operations` from captivity and building sabotage into rival-court espionage and assassination. Espionage now creates live intelligence reports on enemy dynasties, including legitimacy, captives, lesser houses, and exposed bloodline members with location labels. Assassination now kills real bloodline members in runtime, triggers legitimacy and succession consequence, clears commander or governor links when relevant, forces mutual hostility, survives restore, and is surfaced through the dynasty panel instead of remaining hidden in raw state. Stonehelm now uses that same covert lane against the player, so the system is world-reactive rather than player-only.

### Immediate Continuation Direction

The next highest-leverage action after Session 55 is faith operations, specifically missionary conversion and holy war declaration. The covert lane now has bloodline consequence and AI reciprocity. The next adjacent seam is to let covenant commitment create real operational conversion pressure, hostility shifts, and declared faith-war escalation through the already-live faith and dynasty architecture.

---

## 2026-04-15 Session 56 Addendum

This addendum is additive. It records the first live operational faith-pressure layer.

### Session 56

Session 56 made covenant commitment operational. Missionary pressure and holy war declaration no longer sit only in doctrine documents or future-roadmap text. They now consume real faith intensity, pressure rival territorial loyalty or dynastic legitimacy, alter hostility, persist as live holy-war state, surface through the faith panel, survive restore, and trigger Stonehelm reaction. This closes the gap where faith identity mattered to exposure, wards, and marriage compatibility but not yet to direct inter-faction operations.

### Immediate Continuation Direction

The next highest-leverage action after Session 56 is marriage-governance controls by head of household. Marriage proposal and acceptance are already live, faith compatibility is already live, mixed-bloodline consequence is already live, and death-driven dissolution is already live. The next dynastic authority seam is to route marriage power through the head of bloodline so unions become governed political acts with real authority, not free-floating bilateral toggles.

---

## 2026-04-15 Session 57 Addendum

This addendum is additive. It records the first live household-authority layer inside the marriage system.

### Session 57

Session 57 made marriage governance explicit. Cross-dynasty marriage no longer behaves like a free bilateral toggle between two eligible members. The offering house now needs both live household authority and a diplomatic envoy, and proposal or approval can fall to heir or envoy regency only when the head is unavailable. That fallback is not decorative. It now costs legitimacy and stewardship, persists on the proposal and marriage records, and is visible in the dynasty panel. This connects marriage directly to role loss, capture pressure, succession disruption, and dynastic legitimacy.

### Immediate Continuation Direction

The next highest-leverage action after Session 57 is field-water attrition and desertion. Water sustainment already slows and weakens armies operating too far from support. The next canonical layer is to let prolonged dehydration start costing armies bodies, cohesion, and field presence so water pressure reshapes wars materially over longer campaigns.

---

## 2026-04-15 Session 58 Addendum

This addendum is additive. It records the first live dehydration-collapse layer inside the field-water system.

### Session 58

Session 58 turned field-water pressure into body loss and line breakage. Campaign dehydration no longer stops at speed and attack penalties. Units that remain critically dry now accumulate collapse time, begin taking real attrition, and can reach desertion risk if the line stays unsupported. Command presence matters to that outcome, because commanded units now hold together longer and bleed more slowly than uncommanded ones. The logistics dashboard and debug overlay now expose attrition and breaking-line pressure directly, and Stonehelm reacts to that collapse by pulling back before its assault column deserts away. Verification also hardened active holy war so it now sustains legitimacy pressure alongside territorial pressure.

### Immediate Continuation Direction

The next highest-leverage action after Session 58 is covert counter-intelligence and bloodline-targeting defense. Espionage and assassination are already live offensive tools. The next canonical layer is to let rival courts actively blunt, expose, or intercept those operations through real defensive covert state instead of only suffering or launching attacks.

---

## 2026-04-15 Session 59 Addendum

This addendum is additive. It records the first live defensive court-watch layer inside the covert system.

### Session 59

Session 59 ended the offense-only covert state. Espionage and assassination no longer push into rival courts that only answer with passive spymaster numbers. Courts can now raise a real counter-intelligence watch through the existing dynasty-operations lane, and that watch materially changes covert outcomes. Watch strength reads bloodline operator renown, keep depth, ward backing, court loyalty, and dynastic legitimacy. Guarded roles inside the bloodline now gain extra assassination protection. Foiled covert actions now record interceptions and reinforce defending legitimacy. The dynasty panel exposes both player-side watch state and rival-court protection, and Stonehelm now raises its own watch when the player already holds live hostile intelligence.

### Immediate Continuation Direction

The next highest-leverage action after Session 59 is broader world pressure and late-stage escalation. The covert lane now has both offensive and defensive depth. The next canonical layer is to make the world answer consolidation more aggressively through live rival escalation, destabilization pressure, and legible late-stage world reaction.

---

## 2026-04-15 Session 60 Addendum

This addendum is additive. It records the first live late-stage world-pressure escalation layer.

### Session 60

Session 60 made world pressure materially answer dominance. Territorial breadth, off-home holdings, holy war, captives, hostile operations, and dark-extremes descent now accumulate a live pressure score on kingdom factions. Realm cycles promote a dominant leader through `Watchful`, `Severe`, and `Convergence` escalation. That leader now suffers the first real late-stage penalties: the weakest march loses loyalty, severe pressure drains legitimacy, Stonehelm accelerates military and covert pressure against the target, tribes redirect raids toward the pressured kingdom, and the world pill exposes score, streak, and penalties instead of generic signal count alone.

### Immediate Continuation Direction

The next highest-leverage action after Session 60 is deeper counter-intelligence consequence and retaliation. World pressure now accelerates hostile covert tempo against the dominant realm. The next adjacent canonical layer is to turn successful interception into actionable retaliatory knowledge instead of passive defense only.

---

## 2026-04-15 Session 61 Addendum

This addendum is additive. It records the first live interception-dossier and retaliatory-reuse layer inside the covert-defense system.

### Session 61

Session 61 turned successful counter-intelligence into more than a defensive stop. A foiled hostile web now leaves behind actionable knowledge about the attacker. Active watch state now records hostile-source interception history and can issue a `Counter-intelligence dossier` on the attacking court, including the intercepted operation type and accumulated network hits. That dossier survives restore, surfaces in the dynasty panel, and Stonehelm now treats it as enough live knowledge to retaliate without reopening redundant espionage first.

### Immediate Continuation Direction

The next highest-leverage action after Session 61 is marriage follow-up, specifically lesser-house marital anchoring and controlled mixed-bloodline branch pressure. The covert lane now has offense, defense, and actionable retaliation. The next adjacent dynastic layer is to let active or dissolved marriages materially stabilize or destabilize cadet-house loyalty and branch identity.

---

## 2026-04-15 Session 62 Addendum

This addendum is additive. It records the first live cadet-house marital-anchor layer inside the marriage and lesser-house systems.

### Session 62

Session 62 turned mixed-bloodline cadet drift into explicit branch-state. A mixed-bloodline lesser house no longer carries only a hidden negative pull from outside blood. It now reads whether the parent house still has an active marriage anchor into that outside house, whether that anchor has dissolved, and whether renewed hostility has fractured the branch harder. Existing branch children now strengthen an active anchor and deepen a dissolved one. The dynasty panel exposes that state directly, and restore preserves it.

### Immediate Continuation Direction

The next highest-leverage action after Session 62 is house-layer follow-up, specifically Hartvale playability and house-gated unique-unit enablement. Stonehelm is already playable and Hartvale's Verdant Warden already exists in data. The missing layer is honest runtime access control and UI legibility so a third house can become playable without decorative dead data.

---

## 2026-04-15 Session 63 Addendum

This addendum is additive. It records the first live Hartvale house-expansion layer.

### Session 63

Session 63 moved Hartvale out of the near-option state. Hartvale is now prototype-playable through the existing house-select seam, Verdant Warden is now prototype-enabled, and the shared barracks roster no longer leaks house-specific training across factions because simulation now filters trainable units by house ownership before both UI rendering and queue resolution. This means the house layer now advances through real runtime access instead of decorative data presence, and additional unique-unit lanes can follow the same seam without later demolition.

### Immediate Continuation Direction

The next highest-leverage action after Session 63 is world-pressure follow-up, specifically internal dynastic destabilization under overextension. Dominant realms already lose march loyalty and legitimacy. The next canonical layer is to make lesser-house loyalty also worsen under sustained world pressure so late-stage dominance strains the dynasty from within.

---

## 2026-04-15 Session 64 Addendum

This addendum is additive. It records the next internal-consequence layer inside the world-pressure system.

### Session 64

Session 64 made overextension hit the dynasty from within. Dominant realms already lost frontier loyalty and legitimacy under sustained world pressure. They now also pressure active lesser houses directly. Cadet branches record world-pressure severity and a branch-specific loyalty penalty, the dynasty panel surfaces that burden per branch, the world pill exposes cadet drift and pressured-branch count, and restore preserves the new internal-pressure state.

### Immediate Continuation Direction

The next highest-leverage action after Session 64 is world-pressure follow-up through minor-house opportunism. Internal cadet branches now feel the strain. The next canonical layer is to let hostile splinter factions capitalize on the same overextension through faster levy growth, sharper retake behavior, or equivalent territorial opportunism.

---

## 2026-04-15 Session 65 Addendum

This addendum is additive. It records the next external-consequence layer inside the world-pressure system.

### Session 65

Session 65 turned overextension into splinter momentum. Dominant world pressure no longer strains only frontier marches, legitimacy, and cadet-branch loyalty. Hostile breakaway minor houses now read that same parent pressure and capitalize on it directly. Severe overextension accelerates their levy growth, raises their local retinue ceiling, sharpens retake cadence, and becomes visible in both the dynasty panel and world-pressure surface. Restore preserves that splinter-opportunism state.

### Immediate Continuation Direction

The next highest-leverage action after Session 65 is post-dossier covert follow-up, specifically smarter sabotage retaliation and court-counterplay. Counter-intelligence dossiers already create actionable hostile-court knowledge and already feed retaliation assassination. The next canonical layer is to let that same dossier state drive infrastructure-directed sabotage and sharper covert response instead of stopping at bloodline killing alone.

---

## 2026-04-15 Session 66 Addendum

This addendum is additive. It records the next retaliatory-consequence layer inside the covert-defense system.

### Session 66

Session 66 turned counter-intelligence dossiers into sabotage knowledge, not only assassination knowledge. Intercepted hostile covert pressure already produced actionable dossiers and already let Stonehelm retaliate without reopening espionage. Dossiers now also choose sabotage target and subtype, carry a real sabotage-support bonus into covert math, and surface that recommendation in the dynasty panel. Restore preserves the new dossier-backed sabotage state.

### Immediate Continuation Direction

The next highest-leverage action after Session 66 is player-facing dossier-backed sabotage action from the rival-court panel. The recommendation and sabotage support are now live. The next canonical layer is to let the player actually spend the existing covert resources and launch that sabotage through the same shared profile instead of leaving dossier-backed sabotage as an AI-only privilege.

---

## 2026-04-15 Session 67 Addendum

This addendum is additive. It records the first player-facing dossier-actionability layer inside the covert-defense and sabotage systems.

### Session 67

Session 67 ended the AI-only privilege on dossier-backed sabotage. Counter-intelligence dossiers already carried sabotage target, sabotage subtype, and sabotage-support recommendation, but only Stonehelm could spend that knowledge directly. The rival-court panel now exposes a real dossier sabotage action that uses the same shared sabotage profile, the same sabotage-term math, the same dynasty-operation lane, and the same restore-safe dossier provenance. The player can now turn intercepted hostile espionage into live infrastructure-directed covert response instead of only reading recommendation text.

### Immediate Continuation Direction

The next highest-leverage action after Session 67 is convergence-tier world-pressure escalation. World pressure already damages frontier loyalty, legitimacy, cadet stability, and splinter containment, and it already sharpens first-pass hostile tempo. The next canonical layer is to make `Convergence` visibly and materially intensify rival-kingdom covert or faith tempo and tribal raid cadence through a shared escalation profile rather than treating all non-zero pressure above `Severe` as nearly the same.

---

## 2026-04-15 Session 68 Addendum

This addendum is additive. It records the first convergence-specific escalation layer inside the world-pressure system.

### Session 68

Session 68 gave `Convergence` its own runtime teeth. World pressure already punished overextension through frontier loyalty loss, legitimacy pressure, cadet strain, splinter opportunism, rival timer compression, and tribal retargeting. `Convergence` now has a shared escalation profile that sharpens rival military, covert, and faith tempo beyond `Severe`, drives harsher tribal raid cadence, and exposes those caps through the world surface. The late-stage pressure lane now distinguishes its top tier materially instead of only by label.

### Immediate Continuation Direction

The next highest-leverage action after Session 68 is house-vector follow-up through Ironmark's dormant `axeman` lane. Hartvale already has a live honest unique-unit seam, but the house vector has lagged since Session 63. The next canonical layer is to make Ironmark's disabled unique unit live through simulation-side gating, command-surface honesty, and blood-production consequence instead of leaving that identity in inactive data.

---

## 2026-04-15 Session 69 Addendum

This addendum is additive. It records the first live Ironmark unique-unit lane inside the house and blood-production systems.

### Session 69

Session 69 turned Ironmark's `axeman` from dormant content into a real house weapon. The shared Barracks roster now includes the unit, but simulation-side house gating remains authoritative, so only Ironmark can actually surface and queue it. Axeman training now reads a heavier unit-specific blood levy and blood-production load than standard infantry, the command panel exposes that burden honestly, and restore preserves both the queued unit and the resulting civilizational cost.

### Immediate Continuation Direction

The next highest-leverage action after Session 69 is AI awareness of the same house lane. The unique unit is now honest for the player. The next canonical layer is to make Ironmark-controlled AI recruit `axeman` through the same runtime seam and weigh that choice against live blood-production pressure instead of leaving the new house identity as a player-only path.

---

## 2026-04-15 Session 70 Addendum

This addendum is additive. It records the first AI-aware continuation layer for Ironmark's live unique-unit lane.

### Session 70

Session 70 ended the player-only state of Ironmark's `axeman`. AI-controlled Ironmark courts now recruit Axemen through the same shared barracks gate the player already uses. That choice now reads live blood-production burden: when the load is still manageable, Ironmark spends blood to raise Axemen; when the burden is already high, it reins in that heavier levy and falls back to Swordsmen instead. The message log exposes both outcomes, and restore preserves the queued AI Axeman.

### Immediate Continuation Direction

The next highest-leverage action after Session 70 is deeper world-pressure composition and source-aware targeting. World pressure already reacts to dominance, covert pressure, holy war, cadet strain, splinter opportunism, and convergence tempo. The next canonical layer is to expose why a realm is under pressure and make continental overextension matter directly when it is the dominant source.

---

## 2026-04-15 Session 71 Addendum

This addendum is additive. It records the first source-aware world-pressure composition layer.

### Session 71

Session 71 made world pressure name its cause and react to it. The pressure lane no longer exposes only score, streak, and level. It now resolves through a shared source breakdown covering territorial breadth, off-home holdings, holy war, captives, hostile operations, and dark extremes. That source identity is live in the world snapshot and the world pill, and tribes now attack off-home marches directly when continental overextension is the dominant pressure source instead of only converging on the pressured realm generically.

### Immediate Continuation Direction

The next highest-leverage action after Session 71 is source-aware rival-kingdom response. The world itself now recognizes off-home overextension explicitly. The next canonical layer is to make rival kingdoms contest the dominant source directly, starting with off-home holdings when continental breadth is the leading pressure driver.

---

## 2026-04-15 Session 72 Addendum

This addendum is additive. It records the first source-aware rival-kingdom response layer inside the world-pressure system.

### Session 72

Session 72 carried source-aware world pressure from neutral tribes into rival-kingdom action. Stonehelm no longer reacts to a pressured player only through faster generic territorial timers. It now reads the same world-pressure source breakdown and redirects live territorial movement onto off-home marches when continental overextension is the leading source. The message log surfaces that redirect explicitly, and restore preserves the conditions for the same counter-pressure after reload.

### Immediate Continuation Direction

The next highest-leverage action after Session 72 is source-aware faith backlash. Territorial and tribal counter-pressure now react to off-home overextension directly. The next canonical layer is to make enemy missionary and holy-war behavior react more sharply when active holy war is the leading source of world pressure.

---

## 2026-04-15 Session 73 Addendum

This addendum is additive. It records the first source-aware faith-response layer inside the world-pressure system.

### Session 73

Session 73 carried source-aware world pressure into the faith lane. Stonehelm already reacted more aggressively to pressured realms in general, but its missionary and holy-war timing still treated all pressure causes almost the same. It now reads when active holy war is the dominant source and compresses faith backlash more sharply, surfacing renewed missionary pressure and counter-holy-war declaration through the message log. Restore preserves that timing behavior.

### Immediate Continuation Direction

The next highest-leverage action after Session 73 is source-aware covert backlash. Territorial and faith responses now read specific pressure causes. The next canonical layer is to make counter-intelligence and covert retaliation react more sharply when hostile operations are the leading source of world pressure.

---

## 2026-04-15 Session 74 Addendum

This addendum is additive. It records the first source-aware covert-response layer inside the world-pressure system.

### Session 74

Session 74 carried source-aware world pressure into the covert lane. Stonehelm already raised counter-intelligence and retaliation sabotage under ordinary covert pressure, but it now reacts more sharply when hostile operations are the dominant source of world pressure. Counter-intelligence and sabotage timing compress harder, source-aware backlash is surfaced through the message log, and restore preserves that timing behavior.

### Immediate Continuation Direction

The next highest-leverage action after Session 74 is source-aware dark-extremes backlash. Territorial, faith, and covert responses now read specific pressure causes. The next canonical layer is to make world or rival reaction sharpen when dark extremes are the dominant source of world pressure.

---

## 2026-04-15 Session 75 Addendum

This addendum is additive. It records the first source-aware dark-extremes-response layer inside the world-pressure system.

### Session 75

Session 75 carried source-aware world pressure into the dark-extremes lane. Stonehelm already reacted to pressured realms through generic timer compression, but it now reads when `darkExtremes` is the dominant cause and answers through punitive war behavior. Territorial targeting now biases toward the weakest player-held marches, assassination timing compresses when live court intelligence exists, the message log surfaces the backlash explicitly, and restore preserves both the timing and the punitive target selection.

### Immediate Continuation Direction

The next highest-leverage action after Session 75 is source-aware captive backlash. Territorial, faith, covert, and dark-extremes responses now read specific pressure causes. The next canonical layer is to make rival reaction sharpen when held captives are the dominant source of world pressure.

---

## 2026-04-15 Session 76 Addendum

This addendum is additive. It records the first source-aware captive-response layer inside the world-pressure system.

### Session 76

Session 76 carried source-aware world pressure into the captive lane. Stonehelm already lived inside the captivity system through hostility and generic dynasty consequence, but it did not use captive-led pressure as a direct recovery trigger. It now reads when `captives` is the dominant source of player world pressure, compresses captive-recovery timing, prioritizes strategically critical captured members, and launches live rescue or ransom backlash through the existing dynasty-operations architecture. The message log surfaces that response, and restore preserves enough captive state for the rival to relaunch recovery after reload.

### Immediate Continuation Direction

The next highest-leverage action after Session 76 is source-aware territory-expansion backlash. Off-home overextension, holy war, hostile operations, dark extremes, and captives now all drive specific source-aware reactions. The next canonical layer is to make world or rival reaction sharpen when broad territorial expansion, not off-home holdings specifically, is the dominant source of world pressure.

---

## 2026-04-15 Session 77 Addendum

This addendum is additive. It records the first source-aware territory-expansion-response layer inside the world-pressure system.

### Session 77

Session 77 carried source-aware world pressure into broad territorial breadth. Off-home holdings were already a live source with dedicated tribal and rival backlash, but generic territorial expansion still collapsed back into undifferentiated pressure. Tribes and Stonehelm now read `territoryExpansion` directly and punish the weakest stretched marches when territorial breadth itself is the dominant source. The world pill also now exposes territorial-breadth contribution explicitly, so the player can read both the cause and the operational consequence.

### Immediate Continuation Direction

The next highest-leverage action after Session 77 is to return to the house vector and deepen an already-live unique-unit lane. Hartvale's `verdant_warden` is live as a house-gated unit, but its canonical support role is still only implied by stats. The next canonical layer is to make Verdant Warden provide real settlement-defense and local loyalty support through existing runtime and legibility seams.

---

## 2026-04-15 Session 78 Addendum

This addendum is additive. It records the first live Hartvale support-expression layer inside the house and settlement-defense systems.

### Session 78

Session 78 made Hartvale's `verdant_warden` a real protector instead of only an available roster icon. Local Warden presence now strengthens keep-defense attack output, accelerates reserve healing and muster, improves march stabilization, strengthens loyalty recovery, softens loyalty erosion, and surfaces that support through the existing Loyalty and Fort dashboard surfaces. Stonehelm also now recognizes Warden-backed keeps as harder targets and delays assault until escort mass is heavier.

### Immediate Continuation Direction

Do not invent another non-settled house-specific unit lane. Hartvale is the only additional non-Ironmark house whose unique-unit seam is already locked by canon. The next highest-leverage unblocked action is to advance the standard military lane through `Scout Rider` as the first honest stage-2 cavalry and raiding unit, then wire it into logistics disruption, territorial harassment, AI behavior, and restore continuity.

---

## 2026-04-15 Session 79 Addendum

This addendum is additive. It records the first live Scout Rider cavalry-expression layer inside the military, logistics, and territorial-pressure systems.

### Session 79

Session 79 made `Scout Rider` a real stage-2 cavalry unit instead of a dormant roster stub. Stables now train Scout Riders, the player and Stonehelm can issue or launch real raid orders, and those raids disable soft hostile infrastructure for a live window instead of only dealing cosmetic damage. Raids now strip stores from hostile food, wood, stone, iron, and water reserves, shock local march loyalty, suspend raided supply camps as siege anchors, cut raided well support out of field-water sustainment, remove raided dropoff structures from logistics service, and surface all of that through the command surface, logistics dashboard, renderer overlay, and message log. Restore now preserves both active raid-disable windows and cavalry cooldown state.

### Immediate Continuation Direction

The next highest-leverage action after Session 79 is to carry the cavalry lane into direct worker and resource-node harassment, then add the first honest counter-raid response. Infrastructure raiding is now live. The next canonical layer is to let Scout Riders suppress gathering throughput and force defensive reaction around active economic nodes instead of only disabling buildings.

---

## 2026-04-15 Session 80 Addendum

This addendum is additive. It records the first live Scout Rider seam-harassment and counter-raid layer inside the military, logistics, and territorial-pressure systems.

### Session 80

Session 80 carried Scout Rider pressure from fixed infrastructure into live economic seams. Worked hostile resource nodes can now be harried directly, nearby workers are routed to refuge, local hostile loyalty around the seam is shaken, and the logistics dashboard now exposes harried seams together with routed workers. Stonehelm also now recognizes active harried seams as local pressure sites and throws nearby defenders into a real counter-raid response through the same battlefield AI lane already used for other live military action. Restore preserves the new seam-harassment state.

### Immediate Continuation Direction

The next highest-leverage action after Session 80 is moving-logistics interception. Fixed infrastructure raids and worked-seam harassment are now both live. The next canonical cavalry layer is to let Scout Riders strike `supply_wagon` and other moving sustainment carriers so stage-2 cavalry can materially disrupt live siege supply and field-water continuity in motion, not only at static targets.

---

## 2026-04-15 Session 81 Addendum

This addendum is additive. It records the first live moving-logistics interception layer inside the military, logistics, and territorial-pressure systems.

### Session 81

Session 81 carried Scout Rider pressure into moving convoy warfare. `supply_wagon` is no longer only a passive sustainment object once it leaves camp range. Scout Riders can now strike hostile wagons in motion, strip live stores, force convoy retreat, cut formal siege readiness and field-water sustainment, shake nearby hostile march loyalty, and trigger both Stonehelm convoy targeting and local counter-screen response. The logistics surface now names convoy cuts directly, and restore preserves active convoy interdiction with faction provenance.

### Immediate Continuation Direction

The next highest-leverage action after Session 81 is convoy escort discipline and post-interdiction reconsolidation. Moving-logistics interception is now live. The next canonical layer is to make escort screens bind more explicitly to wagons, make recovering convoys reform and re-enter assault timing honestly, and surface that reconsolidation through existing runtime legibility instead of leaving convoy defense as loose ambient patrol behavior.

---

## 2026-04-15 Session 83 Addendum

This addendum is additive. It records the first live runtime layer of the Session 82 match-structure, time-system, and multiplayer doctrine ingestion.

### Session 83

Session 83 carried the newly ingested match doctrine out of canon-only status and into live browser runtime. Match progression is no longer a dead future note. The simulation now computes a readiness-gated five-stage state from food and water stability, founding buildout, covenant commitment, territorial reach, field-army mass, rival contact, sustained war pressure, convergence pressure, and dynastic time. The existing Cycle pill and debug surfaces now expose stage, phase, year, declaration count, and next-stage shortfall so the player can read where the match actually stands. Stonehelm also now respects early-war stage gates for territorial rivalry and Scout Rider tempo unless already-live world-pressure, dark-extremes, convergence, holy-war, or hostile-operations backlash has honestly escalated the conflict.

### Immediate Continuation Direction

The next highest-leverage action after Session 83 is to deepen the new match-progression spine rather than reopen doctrine ingestion. The next canonical layer is either stage-specific declaration or event pressure, especially the first Stage 4 to Stage 5 escalation seam, or the first live imminent-engagement warning layer from the now-ingested multiplayer and time doctrine.

---

## 2026-04-15 Session 84 Addendum

This addendum is additive. It records the first live imminent-engagement warning lane and the first live Stage 5 Divine Right declaration window inside the browser reference simulation.

### Session 84

Session 84 carried the newly live match-progression spine into direct late-game and battle-start command pressure. Threatened dynastic keeps now open imminent-engagement countdowns that expose hostile composition, reserve state, commander presence, and bloodline risk, while the player can answer through reinforcements, posture shifts, commander recall, and emergency bloodline guard. The same session made Stage 5 Divine Right a real public declaration window: Final Convergence kingdoms can now declare through live covenant, apex-structure, and recognition-share gates; rival AI responds through faster coalition tempo; the faith panel and cycle surfaces expose active or incoming declarations; restore preserves the window; and the declaration now resolves honestly into failure or victory instead of stalling after expiry.

### Immediate Continuation Direction

The next highest-leverage action after Session 84 is the first live political-event architecture on top of the now-live progression plus declaration plus warning spine, starting with Covenant Test or Succession Crisis. After that, deepen Stage 5 victory follow-up through territorial-governance or alliance-threshold pressure and stronger coalition counterplay. If those lanes block, continue multi-kingdom pressure, neutral-power stage presence, or naval-world integration.

---

## 2026-04-15 Session 85 Addendum

This addendum is additive. It records the first live political-event architecture inside the browser reference simulation.

### Session 85

Session 85 turned the political-event lane into runtime reality through `Succession Crisis`. Ruling bloodline death no longer stops at legitimacy loss and role succession. When a succession is weak, disputed, underage, or absent, the dynasty can now enter an active crisis with real severity, claimant pressure, timed escalation, immediate territorial loyalty shock, ongoing legitimacy and loyalty drain, weaker economic yield, slower stabilization, slower population growth, and reduced battlefield attack power. The dynasty panel now exposes that pressure directly and gives the player a real consolidation response, while Stonehelm reads the same instability as an opportunity and enemy courts also have to consolidate their own crises through the shared event seam. Restore continuity preserves both active and historical political events.

### Immediate Continuation Direction

The next highest-leverage action after Session 85 is `Covenant Test` on top of the now-live political-event spine. Succession collapse is now real. The next canonical layer is to let faith commitment and conviction extremity trigger an equally live political event that pressures loyalty, unrest, AI timing, and late-stage declaration pacing from the covenant side rather than the dynasty side. After that, continue the Stage 5 victory follow-up through territorial-governance or alliance-threshold pressure and stronger coalition counterplay.

---

## 2026-04-15 Session 86 Addendum

This addendum is additive. It records the second live political-event family and the first live territorial-governance follow-up on the browser reference simulation's Stage 5 spine.

### Session 86

Session 86 made `Covenant Test` live across the four covenants and both doctrine paths. Covenant structures now feed live intensity growth, active tests now impose real runtime strain, direct rites now resolve with real cost where canon supports them, passed tests now gate Apex Covenant access plus late faith-unit access, and Stonehelm now climbs the covenant ladder and performs its own rite when actionable. The same session also made the first `Territorial Governance Recognition` layer live: Final Convergence kingdoms can now enter and sustain a recognition state from loyal stabilized holdings and real territory share, world pressure now reads that recognition as a coalition trigger, Stonehelm redirects pressure toward the weakest governed frontier, and the dynasty panel plus cycle header plus debug lane now surface that state.

### Immediate Continuation Direction

The next highest-leverage action after Session 86 is to deepen `Territorial Governance Recognition` into the first honest Territorial Governance victory seam by adding the missing governor-seat or Governor's House coverage, anti-revolt validation, stronger no-war enforcement, and final resolution logic. If that lane blocks cleanly, the next Stage 5 sovereignty follow-up is alliance-threshold pressure and population-acceptance buildup. After that, continue broader political-event families, multi-kingdom pressure, neutral-power stage presence, or naval-world integration.

---

## 2026-04-15 Session 87 Addendum

This addendum is additive. It records the first honest Territorial Governance sovereignty-resolution layer inside the browser reference simulation's Stage 5 victory family.

### Session 87

Session 87 carried Territorial Governance past recognition-only status and into real sovereignty resolution. Dynastic governorship no longer collapses into one symbolic assignment. Multiple bloodline authorities now seat across governed keeps, cities, and frontier marches, and Territorial Governance now reads that seat coverage together with court loyalty, lesser-house anti-revolt pressure, incoming holy-war pressure, succession fragility, and 90-plus-loyalty integrated marches. Recognition can now mature into held governance, then into a final sovereignty-hold countdown, and finally into a real win state with restore-safe victory metadata. Stonehelm also now reads imminent governance victory as an emergency and compresses war or raid or covert or faith tempo more sharply to break the hold before sovereignty settles.

### Immediate Continuation Direction

The next highest-leverage action after Session 87 is alliance-threshold pressure and population-acceptance buildup on top of the now-live Territorial Governance sovereignty seam. Recognition, seat coverage, revolt resistance, and final resolution are now honest runtime systems. The next canonical layer is to make wider kingdom acceptance and population consent materially determine whether late-game sovereignty can consolidate or whether coalition backlash should intensify instead.
