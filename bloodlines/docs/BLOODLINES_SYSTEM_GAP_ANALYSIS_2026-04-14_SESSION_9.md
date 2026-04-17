# BLOODLINES SYSTEM GAP ANALYSIS

Date: 2026-04-14
Session: 9
Canonical root: `D:\ProjectsHome\Bloodlines`

## Purpose

This document is the system-by-system gap analysis demanded by the 2026-04-14 Master Design Doctrine and by the session 9 full-realization continuation directive. It classifies every canonical Bloodlines system against the current live state.

This is not a cut list. It is a build list.

No entry here is recommended for deletion, reduction, or substitution. Where a system is marked `MISSING` or `DOCUMENTED ONLY`, the correct interpretation is "implementation debt owed to canon", not "scope that is too big". The whole point of the preservation-first posture is that the canonical surface stays constant while the runtime catches up.

## Status Legend

- **LIVE** — implemented in the simulation and visible to the player
- **LIVE (CODE) / NOT EXPOSED** — implemented in simulation but not surfaced to the player yet
- **PARTIAL** — scaffolded in code or data but not fully integrated or surfaced
- **DATA-ONLY** — present in JSON or design docs but not wired into simulation
- **DOCUMENTED** — present in canon only, no runtime or data artifact
- **CANON-LOCKED / IMPLEMENTATION PENDING** — locked by canon (e.g., Decision 21) and not yet implemented
- **VOIDED** — formally rescinded by the creator and archived
- **CANON-OPEN** — intentionally unresolved, pending creator decision

## A. Civilizational Core

| System | Status | Evidence / Notes |
|---|---|---|
| Dynasty as a living system, not a military factory | LIVE (foundation) | Dynasty consequence cascade, captive and fallen ledgers, succession chain, dynasty.operations, governor specialization, faith wards are all live. Depth still accumulating. |
| Population management gated by food and water | LIVE | `tickPopulationGrowth` consumes 1 food + 1 water per growth tick at 18s interval. `recalculatePopulationCaps` reads `populationCapBonus`. |
| Population strategic tension (military vs labor vs faith vs infrastructure) | PARTIAL | Housing cap, food/water gating, available-pop display are live. Explicit allocation categories (agricultural, craft, military, faith, governance) from `04_SYSTEMS/POPULATION_SYSTEM.md` are not yet reflected as distinct allocation pools. |
| Population loss, displacement, growth, support capacity | PARTIAL | Growth is live. Displacement happens via control-point flip (captured vs displaced governor). Explicit population displacement mechanics tied to raids, burned settlements, and occupation have not been fully wired. |
| Water as developable, defendable, attackable, contestable pillar | PARTIAL | Water is a primary resource. Wells produce water trickle. Water crisis fires from `tickRealmConditionCycle`. Water denial is possible by control-point capture but not yet via targeted infrastructure sabotage. Advanced water tiers (reserves, routed systems, aqueducts) are DOCUMENTED ONLY. |
| Water mastery progression (primitive well → reserves → routed → running water → aqueducts) | DOCUMENTED | Canon in master doctrine section V. No tiered water infrastructure exists in data or code. |
| Water for armies in motion | DOCUMENTED | Siege supply wagons carry food, water, and wood-part throughput, so supply-wagon water to engines is LIVE. Generic army water sustainment outside the siege-supply chain is DOCUMENTED ONLY. |
| Water denial as emergent strategy | PARTIAL | Cutting off supply-camp-wagon chain interdicts siege engines. Scout Rider raids can now disable hostile wells for a live window, and Scout Rider convoy interception can now cut active `supply_wagon` sustainment in motion, stripping field-water support and siege continuity from live moving logistics. Broader water denial against settlements and armies (aqueduct cutting, coastal blockade) is still DOCUMENTED ONLY. |
| Food production, security, storage, transport, protection | PARTIAL | Food production from farms is LIVE. Storage and transport beyond the supply-camp-wagon chain are DOCUMENTED ONLY. |
| Land transformation (forests → farmland across generations) | DOCUMENTED | Canon in master doctrine section VI. No land-use transition mechanic exists. |
| Forestry as managed cycle (replant, cultivate, master) | DOCUMENTED | Canon in master doctrine section VII. Wood is currently one-time depletion from patches. |
| Supply disruption → negative upkeep → desertion | PARTIAL | Unsupplied siege engines lose operational efficiency (0.84x attack, 0.88x speed) and the runtime messages this. Army-wide upkeep-driven desertion is DOCUMENTED ONLY. |
| Raid, blockade, encirclement, infrastructure sabotage as viable tools | PARTIAL | Tribe raiding and contested territory movement are LIVE. Scout Rider infrastructure raids and moving-convoy interception are now LIVE as the first directed military raid tools, disabling soft hostile logistics buildings, cutting active `supply_wagon` sustainment, stripping stores, and shocking local loyalty. Blockade, encirclement depth, and the remaining sabotage families are still PARTIAL to MISSING. |

## B. Military and Strategic Core

| System | Status | Evidence / Notes |
|---|---|---|
| Multi-dimensional land doctrine (defensive / balanced / offensive) | DOCUMENTED | Canon in master doctrine section IX. Doctrine-path effects apply to Light/Dark faith commitment, but the land offensive/defensive/balanced axis as a doctrine posture with material consequences is not a player choice. |
| Multi-dimensional naval doctrine (defensive / balanced / offensive) | DOCUMENTED | Canon in master doctrine section IX. Naval runtime does not exist. |
| Defensive fortification doctrine (Decision 21) | LIVE (core), CANON-LOCKED / IMPLEMENTATION PENDING (depth) | Layered defense foundation (wall, tower, gatehouse, keep_tier_1) with structural multipliers is LIVE. Reserve cycling, triage, fallback are LIVE. Anti-wave-spam penalty is LIVE. Apex tiers, signal networks, bloodline-presence-at-keep depth beyond current cascade, fortification-specialist populations (engineers, signal keepers, wall wardens, tower artillerists, keep guard) remain PARTIAL to DOCUMENTED. |
| Siege as earned operation | LIVE (core), PARTIAL (depth) | Siege workshop, 3 engines, engineers, supply camps, supply wagons, unsupplied penalty, AI preparation and sustainment are LIVE. Next support classes (`ballista`, `mantlet`) canonically settled and NOT YET LIVE. Sabotage, coordinated multi-front timing, isolation (starvation), relief-window awareness are MISSING. |
| Delegated strategic resolution (Utopia-style commitment) | DOCUMENTED | Canon in master doctrine section XI. Runtime has direct-command only. |
| Direct player command path | LIVE | All combat is currently direct player command. |
| Dual-path balance (auto-resolve credible, manual rewarding) | DOCUMENTED | Cannot exist until delegated resolution exists. |
| Commanders with battlefield presence, aura, killable, capturable | LIVE | Commander unit exists, aura applies, capture-vs-kill resolution works based on hostile proximity. |
| Generals as separate from commanders | DOCUMENTED | Canon distinguishes commanders and generals. Runtime has only a single commander role. |
| Commander and general effect on delegated outcomes | DOCUMENTED | Requires delegated resolution path. |
| Commander and general effect on battlefield performance | PARTIAL | Commander aura is live. Generals as separate category and their morale/operational effect are DOCUMENTED. |
| Naval strategy (6 vessel classes, harbor tiers, trade routes, blockade, crossing) | DOCUMENTED | Canon in `11_MATCHFLOW/NAVAL_SYSTEM.md`. Vessels in `10_UNITS/UNIT_INDEX.md`. Current map has no water, no harbor, no vessel, no naval combat. |
| Continental world architecture (home + 2+ secondary continents, separation by water) | DOCUMENTED | Canon in master doctrine section XIV and `09_WORLD/`. Current map is single-continent. |

## C. Fortification and Siege Detail (Decision 21)

| System | Status | Notes |
|---|---|---|
| Fortification tier metadata on control points and settlements | LIVE | Classes and ceilings live, tier advances on building completion. |
| Outer works, inner ring, final core layering | PARTIAL | Wall segments + towers + gatehouses + keep are separate entities. Logical layering (outer must fall before inner matters) is implicit through positioning, not structural. |
| Walls with structural multipliers | LIVE | `wall_segment` structural 0.2, others tuned. |
| Gates with passable state | LIVE | `gatehouse` 0.3 structural, passable. |
| Towers with sight/attack aura | LIVE | `watch_tower` 0.15 structural plus aura. |
| Keep as bloodline seat | LIVE | `keep_tier_1` canonical; dynasty ties live via governor specialization and reserve cycling. |
| Kill zones / chokepoints | DOCUMENTED | No explicit kill-zone entity. Defensive leverage emerges from positioning. |
| Signal horns and rally points | DOCUMENTED | Nothing in code yet. |
| Fallback positions | LIVE | Reserve cycling (fallback → triage → muster) is live. |
| Garrison / reserve mustering | LIVE | `tickFortificationReserves` governs this. |
| Fortification specialist populations (garrison, engineers, signal keepers, wall wardens, tower artillerists, keep guard) | PARTIAL | Engineers LIVE. Signal keepers, wall wardens, tower artillerists, keep guard DOCUMENTED ONLY. |
| Boiling oil / anti-breach systems | DOCUMENTED | No runtime artifact. |
| Keep-linked garrisons | PARTIAL | Reserve cycle treats the keep as triage hub. Dedicated garrison unit class is not separate from militia/swordsman. |
| Siege-resistant gates | DOCUMENTED | Gate tier beyond current `gatehouse` is unbuilt. |
| Emergency musters | LIVE | Blood Dominion sacrificial reserve surge is the clearest emergency-muster expression. |
| Defender command bonuses while at home seat | PARTIAL | Faith wards and keep presence alter defensive behavior. Explicit command-at-home-seat bonuses beyond this are DOCUMENTED. |
| Protected fallback courtyards | DOCUMENTED | No inner-courtyard entity. |
| Stronger structural regeneration when not under active siege | DOCUMENTED | Repair only happens through engineer action on siege engines. Structural regeneration for walls is not present. |
| Anti-swarm deterrents | PARTIAL | Assault cohesion strain is the canonical wave-spam denial. |
| Internal defensive districts that survive partial breach | DOCUMENTED | No district entity. |
| Battering ram (engine) | LIVE | Structural ×1.6. |
| Siege tower (support) | LIVE | Extends allied structural pressure. |
| Trebuchet (bombardment) | LIVE | Range ≥200, wall pressure. |
| Ballista (future support) | LIVE | Live in data, simulation, renderer, AI, and tests as ranged anti-personnel siege support. |
| Mantlet (future support) | LIVE | Live in data, simulation, renderer, AI, and tests as mobile ranged-cover siege support. |
| Siege engineer | LIVE | Breach support, repair throughput. |
| Supply camp | LIVE | Forward logistics anchor. |
| Supply wagon | LIVE | Sustainment, linked to camp. |
| Mining / counter-mining | DOCUMENTED | Canon reference, no code. |
| Saps and berms | DOCUMENTED | Canon reference, no code. |
| Breach planning | DOCUMENTED | No breach-planning operation. |
| Coordinated multi-front timing | DOCUMENTED | No multi-front coordination AI or operation. |
| Isolation / starvation | DOCUMENTED | No isolation operation. Siege interdiction is supply-chain break only. |
| Faith powers as siege accelerant | PARTIAL | Faith wards affect defense. Faith-powered offensive siege support is DOCUMENTED. |
| Sabotage (gate opening, fire raising, supply poisoning) | DOCUMENTED | Missing operation type. `dynasty.operations` architecture is ready. |
| Relief-window adaptation in AI | DOCUMENTED | Stonehelm does not yet respond to relief armies. |
| Repeated-assault windows | DOCUMENTED | No multi-wave siege adaptation yet. |
| Post-repulse AI adjustment | DOCUMENTED | No learning after failed assault. |
| Late-game apex fortifications | DOCUMENTED | `fortress_citadel` tier 5 exists in settlement classes; no apex-specific structures. |

## D. Dynasty and Identity Core

| System | Status | Notes |
|---|---|---|
| Nine canonical founding houses | DATA-ONLY (8 of 9), LIVE (1 of 9) | Ironmark fully locked and `prototypePlayable`. Hartvale `partially-locked`. Seven others `settled-visual-only` with CB004 voided 2026-04-07. |
| House asymmetry (unique units, mechanics, aesthetics) | PARTIAL | Ironmark Blood Production loop is live enough to support unit-specific levy burden, Hartvale Verdant Warden and Ironmark Axeman are now live through house-gated runtime training, Ironmark AI now reads the Axeman lane with blood-load-aware fallback, and the remaining house-specific asymmetry still remains implementation debt. |
| House-select / lobby | DOCUMENTED | Skirmish hardcoded Ironmark vs Stonehelm. |
| Trueborn as oldest house | DOCUMENTED | Canon lock. No data. No runtime. |
| Trueborn neutral city + late-game return arc | DOCUMENTED | Canon in master doctrine section XXIII. No implementation. |
| Bloodline members as dynastic actors | LIVE (roster, roles, succession), PARTIAL (battlefield, UI) | Roster exists, roles resolve, succession cascades. Battlefield attachment is live for commander role. Other roles are state-only, not battlefield-actor. |
| Ruler / head of bloodline | LIVE | Top of succession chain. |
| Heir / heir designate | LIVE | Second in succession. |
| Commander | LIVE | Battlefield unit with aura. |
| Governor | LIVE | Territorial role with specialization. |
| Diplomat | DOCUMENTED | Role exists in state. No diplomacy system in runtime. |
| Ideological leader | DOCUMENTED | Role exists. No ideological-leader mechanics. |
| Merchant | DOCUMENTED | Role exists. No trade system in runtime. |
| Sorcerer (Mysticism, locked 2026-03-26) | DATA-ONLY | Role exists. No runtime effect. |
| Spymaster | PARTIAL | Role exists. Spymaster contributes to rescue difficulty formula. Full covert-ops pool is DOCUMENTED. |
| Marriage control by head of household | DOCUMENTED | Canon in `04_SYSTEMS/DYNASTIC_SYSTEM.md`. No code. |
| Polygamy restricted to Blood Dominion and Wild | LIVE (foundation) | `proposeMarriage` now enforces covenant-specific polygamy restriction in runtime. |
| Faith-compatibility weighting in AI marriage diplomacy | LIVE (first layer) | AI marriage proposal and acceptance now classify covenant and doctrine fit, opening legitimacy-repair matches for compatible courts and blocking weak fractured matches. |
| Marriage death and dissolution from live bloodline death | LIVE (first layer) | Real spouse death now dissolves the shared marriage record, applies legitimacy loss and oathkeeping mourning, halts gestation, and survives snapshot restore. |
| Cross-dynasty children | LIVE (foundation) | Marriage gestation produces children with mixed-bloodline metadata in the head faction. |
| Mixed-bloodline defection slider | LIVE (first layer) | Mixed-bloodline lesser houses now carry outside-house pressure in daily loyalty drift, buffered by active marriage ties and worsened by renewed hostility. |
| World-pressure-driven internal dynastic destabilization | LIVE (first layer) | Dominant world-pressure targets now also worsen active lesser-house loyalty drift, surface cadet-pressure severity in dynasty and world legibility, and preserve that state through restore. |
| Succession interface with 5 impact metrics | PARTIAL | Succession works in simulation, and the dynasty panel now surfaces active succession-crisis severity, claim pressure, runtime modifiers, hostile-court crisis state, and consolidation terms. A dedicated full succession interface is still not present. |
| Born of Sacrifice (population-constrained champion lifecycle) | DOCUMENTED | Full spec in `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md` (52KB). No runtime. |
| Lesser houses from war heroes | LIVE (foundation) | Promotion pipeline, loyalty drift, defection, minor-house spawn, territorial claim, defense AI, and first territorial levy layer are live. |
| Breakaway minor-house territorial levy and local retinue growth | LIVE (first layer) | Held breakaway march can now spend food and influence, consume local loyalty, and raise real retinue units that survive save/restore. |
| World-pressure-driven splinter opportunism against hostile parent realms | LIVE (first layer) | Hostile breakaway minor houses now read parent world pressure, accelerate levy tempo, gain retinue-cap escalation under severe overextension, sharpen territorial retake cadence, surface the new state in dynasty and world legibility, and preserve it through restore. |

## E. Faith and Conviction Core

| System | Status | Notes |
|---|---|---|
| Four ancient faiths chosen by map interaction | LIVE (discovery, commitment) | Exposure accrual at sacred sites, commitment at threshold 100. |
| Light / Dark doctrine paths | LIVE (choice + doctrine effects), PARTIAL (depth) | Doctrine path stored, effects applied to capture, stabilization, growth, aura, and ward profiles. Full canonical asymmetry between Light and Dark at each tier is PARTIAL. |
| Faith intensity 5-tier ladder (Latent, Active, Devout, Fervent, Apex) | PARTIAL | Tier label renders. Per-tier mechanical consequences are not yet distinct. |
| Faith building progression (Wayshrine → Hall → Cathedral-equivalent) | DOCUMENTED | Not in data (`faiths.json` has all four covenants `prototypeEnabled: false`). Not in code. |
| L3, L4 faith-specific units (2 per faith per path) | DOCUMENTED | Canonical roster of 16 units. None in `units.json`. |
| L5 apex units (one per faith) | DOCUMENTED | Named apex units (The Unbroken, The Sacrificed, The Mandate, The First Wild). None in data. |
| Apex faith structures (one per faith, tied to victory path) | DOCUMENTED | No apex structure artifact. |
| Covenant Test mechanics | DOCUMENTED | No implementation. Session 85 established the shared dynasty political-event spine that this event can now reuse. |
| Tithes, ceremonies, priests, priestesses | DOCUMENTED | No implementation. |
| Faith intensity growth over time with aura | LIVE | `updateFaithExposure` increments intensity while in aura. |
| Faith-integrated fortification wards (Pyre / Altar / Edict / Root) | LIVE | All four covenants have light and dark ward profiles. Behavioral. |
| Blood Dominion sacrificial reserve surge | LIVE | Population burn for defensive surge. |
| Conviction as independent behavioral spectrum | LIVE | Four-bucket ledger (Ruthlessness, Stewardship, Oathkeeping, Desecration), derived band. |
| Conviction milestone powers per band | DOCUMENTED | Band label renders; per-band mechanical unlocks are not wired. |
| Conviction CP weighting (1x / 1.5x / 2x pattern amplification) | DOCUMENTED | Flat bucket increments currently. |
| Population loyalty response to conviction | DOCUMENTED | Loyalty mechanics do not yet read conviction band. |
| Dark-extremes world pressure trigger | LIVE (first layer) | Sustained `Apex Cruel` now feeds world-pressure score directly, exposes `darkExtremesActive`, and drives source-aware punitive territorial backlash plus assassination-timing escalation when dark extremes are the dominant pressure source. |
| Irreversible divergence at late-game commitment | DOCUMENTED | No late-game lock-in yet. |

## F. Pacing and World Core

| System | Status | Notes |
|---|---|---|
| Five-stage match structure | PARTIAL | Readiness-gated stage progression, phase state, stage legibility, stage-aware AI restraint, imminent-engagement timing, and Divine Right declaration pressure are now live on the current skirmish slice. Full continental structure, multi-front stage presence, and the complete event queue remain unfinished. |
| Three overlay phases (Emergence, Commitment, Resolution) | DOCUMENTED | No phase overlay. |
| Multi-speed time model (battlefield / campaign / dynastic) | PARTIAL | Canonical 90-second campaign cycle is LIVE via `tickRealmConditionCycle`. Battlefield clock is the single runtime clock. Dynastic clock is DOCUMENTED. |
| Determined time system (solved, canonical, single timing doctrine) | LIVE (within current slice) | Current runtime uses a single consistent real-time clock. Dual-clock declaration seam is DOCUMENTED. |
| Dual-clock architecture (Declaration + Events Queue + Commitment) | PARTIAL | Declaration-side runtime is now live through Divine Right windows and succession-consolidation declarations, with restore continuity and UI exposure. Events Queue and Commitment-phase sequencing remain unfinished. |
| Phase Entry (accelerated entry at any stage) | DOCUMENTED | No stage model in runtime. |
| Strategic zoom (theatre-of-war view) | DOCUMENTED | Only battlefield view exists. |
| Fog of war | PARTIAL | Sight-radius unit logic is LIVE. Zoom-scaled fog is DOCUMENTED. |
| World pressure (tribes) | LIVE | Neutral tribes raid, contest territory, and now target off-home holdings or stretched weak marches specifically when those are the dominant sources of world pressure. Rival kingdoms now also redirect territorial pressure toward those same pressure-driving marches. |
| Neutral hub / trade center | DOCUMENTED | No neutral hub exists in data or code. |
| Trueborn late-game return | DOCUMENTED | No implementation. |
| Dark-extremes pressure | LIVE (first layer) | Dominant dark-extremes pressure now sharpens rival punitive territorial response toward the weakest marches and accelerates assassination backlash when court intelligence is already live. |
| Up to 10 players + AI kingdoms + minor tribes | DOCUMENTED | 1 player + 1 AI + 1 tribes faction in current slice. |
| Ten canonical terrain types | DOCUMENTED | Current map uses simplified terrain grid. Ten-terrain system not yet enforced. |
| Four canonical historical eras | DOCUMENTED | Not represented in runtime. |

## G. Operations System

| System | Status | Notes |
|---|---|---|
| `dynasty.operations` state and active/history tracks | LIVE | Architecture ready. |
| Negotiated ransom operation | LIVE | Live and tested. Rival AI now also uses ransom as source-aware captive backlash when held captives are the dominant pressure source and rescue is not prioritized. |
| Covert rescue operation | LIVE | Live and tested. Rival AI now also uses rescue as source-aware captive backlash when held captives are the dominant pressure source and the captive is strategically critical. |
| Captor-side ransom demand | LIVE | Live and tested. |
| Covert operations: assassination, sabotage, espionage | LIVE (foundation) | Espionage, assassination, and sabotage are live. World-pressure source-aware AI backlash now sharpens counter-intelligence and sabotage response when hostile operations are the dominant pressure source. |
| Faith operations: missionary, conversion, holy war | LIVE (first layer) | Missionary pressure and holy war declaration are live. World-pressure source-aware AI backlash now sharpens missionary and holy-war response when holy war is the dominant pressure source. |
| Military operations: raid, ambush, supply-line strike | PARTIAL | Tribe raids are ambient, and Scout Rider now provides the first structured player and AI raid lane against hostile civilian logistics both at fixed infrastructure and against moving `supply_wagon` sustainment. Ambush and broader supply-line strike operations remain MISSING. |
| Detection and counterplay for covert ops | DOCUMENTED | Missing. |
| Sabotage sub-operations (gate opening, fire raising, supply poisoning, well poisoning) | DOCUMENTED | Missing. |

## H. Legibility (UI, HUD, Dashboards)

| System | Status | Notes |
|---|---|---|
| Battle HUD with resources | LIVE | 10-pill resource bar. |
| Realm condition bar | LIVE | Full 11-state HUD is live and driven by `getRealmConditionSnapshot`. |
| Message log with tone | LIVE | Limit 6, TTL decay. |
| Dynasty panel | LIVE | Legitimacy, roster, roles, paths, captives, fallen, live operation actions. |
| Faith panel | LIVE (foundation) | Exposure progress, doctrine commit, ward state partial. |
| Keep interior UI as core anchor | DOCUMENTED | `12_UI_UX/UI_NOTES.md` (60KB) specifies a visual-first keep interior. No implementation. |
| Territory overlay (provinces, loyalty, control) | PARTIAL | Control-point aura and loyalty ring are visible. Province layer is DOCUMENTED. |
| Strategic-zoom theatre view | DOCUMENTED | Not implemented. |
| Continental awareness visual | DOCUMENTED | Not implemented. |
| Ward iconography | DOCUMENTED | No icon layer for ward state yet. |
| Governor specialization iconography | DOCUMENTED | No icon. |
| Reserve state iconography | LIVE (renderer) | Reserve-duty markers live. |
| Siege line visualization | PARTIAL | Siege engines, workshop, camp, wagon, engineer all render. Explicit "siege line" aggregate is not a distinct visual. |
| Onboarding / tutorial | DOCUMENTED | Launch-card copy only. |
| Match setup / lobby / house select | PARTIAL | URL-driven house-select seam exists for prototype-playable houses, but there is still no full lobby or explicit in-shell setup surface. |
| Audio layer | DOCUMENTED | No audio in prototype. |

## I. Victory Conditions

| Path | Status | Notes |
|---|---|---|
| Military Conquest | LIVE (player win, enemy command hall destroyed) | Simple skirmish. |
| Economic Dominance / Currency Dominance | DOCUMENTED, CANON-OPEN on details (Decision 13) | Not enabled in data. |
| Faith Divine Right declaration window | LIVE | Stage 5 Divine Right is now a real public declaration window with recognition-share and apex-structure gating, coalition response, UI legibility, restore continuity, and real failure or victory resolution. |
| Territorial Governance (~90% threshold) | DOCUMENTED | Not enabled in data. |
| Alliance Victory | DOCUMENTED | Not enabled in data. |
| Dynastic Prestige as victory path | VOIDED 2026-04-07, reframed as modifier | Preserved as `preserved-historical-non-victory`. |

## J. Houses — Playable Depth

| House | Data | Prototype | Unique Unit | Unique Mechanic | Status |
|---|---|---|---|---|---|
| Ironmark | fully-locked | playable | Axeman (`prototypeEnabled: false`) | Blood Production (PARTIAL) | Playable house with current blood-production identity |
| Stonehelm | settled-visual-only | playable | none | fortification discount (LIVE) | Playable house and AI opponent |
| Highborne | settled-visual-only | no | none | none | Lore only |
| Goldgrave | settled-visual-only | no | none | none | Lore only |
| Westland | settled-visual-only | no | none | none | Lore only |
| Hartvale | partially-locked | playable | Verdant Warden (LIVE, house-gated) | none live | Playable house with first live unique-unit access layer |
| Whitehall | settled-visual-only | no | none | none | Lore only |
| Oldcrest | settled-visual-only | no | none | none | Lore only |
| Trueborn | canonical oldest, neutral city tied | no | none | Late-game return arc (DOCUMENTED) | Canon-load-bearing, no runtime |

## K. Infrastructure

| System | Status | Notes |
|---|---|---|
| Data-driven JSON content pipeline | LIVE | `data-loader.js` loads all definition files. |
| Validation tests | LIVE | `tests/data-validation.mjs`, 45 assertions, passes. |
| Runtime bridge tests | LIVE | `tests/runtime-bridge.mjs`, 70+ blocks, 1,070 lines, passes. |
| Syntax check | LIVE | `node --check` on every simulation module. |
| Pathing (grid + waypoint) | DOCUMENTED | Current movement is steering only. Dense groups can snag. |
| Formation / stance / patrol / hold-ground | DOCUMENTED | Not implemented. |
| Morale / rout | DOCUMENTED | No morale scalar. |
| Save / resume | DOCUMENTED | Decision 16 locked. No save system. |
| Replay / deterministic seed | DOCUMENTED | Not implemented. |
| Profiling | DOCUMENTED | Not implemented. |

## L. Unity Production Lane

| Item | Status | Notes |
|---|---|---|
| Unity project at `unity/` | LIVE (structurally) | 6.4 target, awaits decision to align to 6.3 LTS. |
| `_Bloodlines/` baseline tree | LIVE | Art, Audio, Code, Data, Prefabs, Scenes, Materials, Shaders, Animation, Docs. |
| DOTS/ECS package stack | INSTALLED | Entities 6.4.0, Burst, Collections, Mathematics, Entities.Graphics, URP, Input System, Addressables. |
| `BloodlinesDefinitions.cs` | LIVE (canon fields extended) | Fortification, siege, settlement, realm-condition fields extended. |
| `JsonContentImporter.cs` | LIVE | Now imports settlement-classes and realm-conditions. |
| Generated ScriptableObject assets | PENDING | Requires Unity open + `Bloodlines → Import → Sync JSON Content` run. |
| ECS Components (Position, Faction, Health, UnitType, BuildingType, ResourceNode, ControlPoint, Settlement, Commander, Governor, Bloodline status, Faith, Conviction, AssaultCohesion) | PENDING | Awaits version decision. |
| ECS Systems (ResourceAccumulation, PopulationGrowth, RealmConditionCycle, UnitMovement, UnitGather, SmeltingFuel, BuildingProduction, Construction, CombatTargeting, CombatDamage, AssaultCohesion, TerritoryCapture, FaithExposure, DynastyCascade) | PENDING | Awaits version decision. |
| Bootstrap scene | PENDING | |
| Ironmark Frontier gameplay scene | PENDING | |
| Battlefield camera with pan/zoom | PENDING | |
| Input System action map | PENDING | |
| Persistent top-bloodline-members HUD panel | PENDING | |
| 11-state realm-condition dashboard (Unity) | PENDING | |
| Strategic-zoom theatre transition | PENDING | |
| Fortification-phase Unity work (U17-U22) | PENDING | |

## M. Preservation Check (Non-Reduction Audit)

The following items must NOT be reduced, substituted, or sidelined. If any future session proposes to narrow them, that proposal should be rejected on the grounds of master doctrine preservation.

1. Nine founding houses with asymmetric identity.
2. Trueborn and the neutral city late-game return.
3. Four faiths with Light and Dark doctrine paths, 5-tier ladders, faith-specific unit rosters, apex structures.
4. Conviction as independent four-bucket spectrum with five bands.
5. Water as central civilizational pillar with progression from primitive access to engineered mastery.
6. Food with land transformation (forests → farmland).
7. Forestry as managed cycle.
8. Supply disruption → negative upkeep → desertion.
9. Multi-dimensional land and naval doctrine axes.
10. Defensive fortification doctrine (Decision 21, ten pillars).
11. Delegated resolution plus direct command dual path.
12. Commanders and generals with real impact.
13. Five-stage match structure with three overlay phases.
14. Multi-speed time model with determined time canonical and dual-clock architecture.
15. Continental world architecture with 2+ secondary continents.
16. Six naval vessel classes with harbor tiers and naval trade routes.
17. Strategic zoom to theatre-of-war view.
18. Strategic legibility as first-class responsibility.
19. Ten players + AI kingdoms + minor tribes scale.
20. Replayability through systemic depth, not cosmetics.

## N. Implementation Debt Priority (Non-Reductive)

The following priority order is determined by what unlocks the most downstream work, not by what is easiest.

1. **Full 11-state realm dashboard** — unlocks legibility for every other system. `getRealmConditionSnapshot` already exports the data. This is near-immediate.
2. **Ballista + mantlet siege-support classes** — canonically next; roster-expansion-class; extends live siege architecture; small but visible.
3. **Sabotage operation type** — opens the covert operations seam using the live `dynasty.operations` architecture. Enables gate opening, fire raising, supply poisoning as dynasty operations.
4. **Commander keep-presence expansion** — deepens fortification + dynasty + faith ward integration. No new architecture needed.
5. **Longer-siege AI adaptation** — relief-window awareness, repeated assault windows, post-repulse adjustment. Extends live Stonehelm AI.
6. **Second playable house (Stonehelm or Highborne)** — breaks the single-house-playable situation. Unblocks house asymmetry verification.
7. **Hartvale Verdant Warden in data + simulation** — closes a canon-to-data gap.
8. **Unity version decision, JSON sync, ECS foundation** — unblocks the production lane. User-decision-blocked today.
9. **Faith prototype enablement** — flip `faiths.json` prototypeEnabled flags; wire doctrine-specific per-tier effects; add L3 / L4 / L5 unit rosters.
10. **Ironmark Blood Production cost loop deepening** — the defining Ironmark identity mechanic is PARTIAL; make it strategically visible.
11. **Dual-clock declaration seam (minimal)** — even a post-battle Declaration summary with declared elapsed time proves the rhythm.
12. **Naval foundation on current map** — add a water body, a harbor, one vessel class. Proves the architectural seam.
13. **Continental architecture on current map** — add a secondary landmass, even if empty.
14. **Operations system expansion** — covert, faith, and military operations in `dynasty.operations`.
15. **Political events cascade minimal** — stage-triggered events firing from realm condition.
16. **Marriage / succession interface panel** — surface what already happens in simulation through a dedicated UI.
17. **Born of Sacrifice champion lifecycle scaffold** — start the canonical lifecycle at the most basic tier.
18. **Lesser houses from war heroes** — promotion pipeline from unit to lesser-house AI.
19. **Apex faith structures (one per covenant)** — late-game lock-in artifact.
20. **Save / resume** — Decision 16 is locked.

## O. Risk of Session-Level Drift

The following are the specific drift risks an undisciplined next session could introduce. Call these out if they appear.

- Adding more HUD pills in a way that breaks the 11-state alignment (do not add a 12th unless canon changes).
- Adding a sabotage operation that is not registered as a member of `dynasty.operations` schema.
- Implementing marriage in a way that collapses Blood Dominion / Wild polygamy into a single family-tree rule.
- Implementing a second playable house with no unique unit or unique mechanic (just a reskin of Ironmark).
- Starting ECS code in `unity/` before the version decision is locked (would risk re-resolving packages later).
- Adding water to the current map without designing it as a naval seam (treating it as cosmetic only).
- Compressing the 5-stage match structure to 3 stages or less to "simplify" runtime.
- Treating apex faith structures as optional "nice to have" instead of as victory-path-tied commitments.
- Letting the Unity stub project at `Bloodlines/` be opened as the primary Unity project (it must stay stub).
- Deleting `archive_preserved_sources/` material even if it appears redundant.

## P. Session 9 Actions

Actual session 9 implementation work is tracked alongside this analysis. See:

- `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md` for the continuation plan.
- `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` for the execution roadmap.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` for the state report.
- `00_ADMIN/CHANGE_LOG.md` for the change log entry.

The state of each item here will evolve as future sessions work it forward. Reclassification from `DOCUMENTED` to `PARTIAL`, `PARTIAL` to `LIVE`, and so on is the correct direction of motion. Reclassification in the reverse direction should not happen except via explicit creator authorization.

## 2026-04-15 Sessions 46-47 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Minor-house territory | DOCUMENTED | LIVE | Session 47 made defected cadet branches territorial actors by spawning a stabilized border march with live ownership, food and influence trickle, dynasty-panel legibility, and snapshot round-trip persistence. |

Session 46 did not flip a distinct matrix row fully to `LIVE`, but it materially deepened several already-`PARTIAL` civilizational rows by adding positive loyalty reinforcement from food and water abundance under the realm-condition cycle.

## 2026-04-15 Sessions 48-49 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Minor-house operational AI | DOCUMENTED | LIVE | Session 48 made breakaway cadet branches react to hostile pressure, defend claimed territory, retake it if seized, regroup after pressure clears, and surface posture in the dynasty panel. |
| Save / resume | DOCUMENTED | PARTIAL | Session 49 deepened the live restore lane by rebuilding prefix-based runtime counters after snapshot restore, preventing post-restore id collisions for later dynamic creation. Player-facing save UX and full match persistence remain unfinished. |

## 2026-04-15 Session 54 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Water-infrastructure tier 1 and field-army water sustainment | DOCUMENTED | LIVE | Session 54 made owned marches, settlements, wells, supply camps, and linked wagons act as live hydration anchors for field armies. Dehydration now slows armies, weakens attacks, surfaces in the logistics dashboard, survives snapshot restore, and can delay Stonehelm assaults. |

## 2026-04-15 Session 55 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Additional covert-operation depth, assassination and espionage | DOCUMENTED | LIVE | Session 55 extended `dynasty.operations` into rival-court intelligence reports and bloodline-targeted assassination. The new covert lane now touches legitimacy, succession, command and governor links, hostility, dynasty-panel legibility, AI reciprocity, and restore continuity. |

## 2026-04-15 Session 56 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Faith operations, missionary conversion and holy war declaration | DOCUMENTED | LIVE | Session 56 extended the live faith lane into timed operations fueled by faith intensity. Missionary work now raises rival exposure, erodes incompatible covenant intensity, and pressures territory or legitimacy. Holy war declaration now creates persistent faith-war state with hostility consequence, ongoing zeal and loyalty pressure, AI reactivity, faith-panel legibility, and restore continuity. |

## 2026-04-15 Session 57 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Marriage control by head of household | DOCUMENTED | LIVE | Session 57 routed marriage proposal and acceptance through live household authority, required an offering envoy, applied regency legitimacy strain when the head is unavailable, surfaced governance through the dynasty panel and faction snapshot, and preserved governance provenance through restore. |

## 2026-04-15 Session 58 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Longer-run field-water attrition and desertion | DOCUMENTED | LIVE | Session 58 extended field-water sustainment into critical-duration tracking, real health attrition, desertion risk, commander-buffered discipline, logistics and debug legibility, AI retreat behavior, and restore continuity. |

## 2026-04-15 Session 59 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Covert counter-intelligence and bloodline-targeting defense | DOCUMENTED | LIVE | Session 59 added live dynasty counter-intelligence watch state, guarded bloodline-role protection, interception tracking, legitimacy reinforcement on successful defense, dynasty-panel legibility, AI reciprocity, and restore-safe `dynastyCounter` continuity. |

## 2026-04-15 Session 60 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Broader world pressure and late-stage escalation | DOCUMENTED | LIVE | Session 60 added realm-cycle world-pressure scoring, dominant-leader escalation levels, weakest-frontier loyalty pressure, legitimacy pressure, Stonehelm timer compression, tribal retargeting, dashboard legibility, and snapshot continuity. |

## 2026-04-15 Session 61 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Counter-intelligence interception-network consequence and retaliation | DOCUMENTED | LIVE | Session 61 turned successful covert interception into a live dossier and retaliation layer with source-scoped watch history, AI reuse of interception-driven knowledge, dynasty-panel legibility, and restore continuity. |

## 2026-04-15 Session 62 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Lesser-house marital anchoring and controlled mixed-bloodline branch pressure | DOCUMENTED | LIVE | Session 62 added active, dissolved, strained, and fractured cadet-house marriage anchors, child-backed branch support, dynasty-panel legibility, and restore continuity for the new branch-state fields. |

## 2026-04-15 Session 66 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Dossier-backed sabotage retaliation and covert court-counterplay | PARTIAL | LIVE | Session 66 extended counter-intelligence dossiers into sabotage target selection, sabotage subtype selection, sabotage-support bonus, dynasty-panel legibility, AI retaliation reuse, and restore continuity. |

## 2026-04-15 Session 67 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Player-facing dossier-backed sabotage actionability | PARTIAL | LIVE | Session 67 exposed dossier-backed sabotage as a real rival-court action using shared sabotage terms, live dynasty-operation launch, dynasty-panel legibility, and restore continuity instead of leaving dossier sabotage as an AI-only privilege. |

## 2026-04-15 Session 68 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Convergence-tier world-pressure rival tempo and tribal escalation | PARTIAL | LIVE | Session 68 added a shared convergence profile, sharper rival military, covert, and faith tempo, harsher tribal raid cadence, world-pill legibility, and restore continuity for the top world-pressure tier. |

## 2026-04-15 Session 77 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Source-aware territory-expansion backlash under world pressure | PARTIAL | LIVE | Session 77 made tribes and rival kingdoms read `territoryExpansion` directly, punish stretched weak marches when territorial breadth is the dominant source, expose breadth contribution through the world pill, and preserve the new backlash behavior through restore. |

## 2026-04-15 Session 78 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Hartvale Verdant Warden settlement-defense and loyalty support | PARTIAL | LIVE | Session 78 turned Hartvale's settled unique unit into a real support system. Verdant Wardens now strengthen keep-defense attack output, reserve healing, reserve muster, local loyalty protection, loyalty recovery, and march stabilization, surface that support in the existing dashboard, sharpen Stonehelm assault caution, and preserve the behavior through restore. |

## 2026-04-15 Session 80 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

No full matrix row moved to `LIVE` in Session 80. The session materially deepened existing `PARTIAL` military and logistics rows by making Scout Rider harassment reach worked seams, routed workers, local counter-raids, logistics-pill legibility, and restore continuity. Ambush and moving-logistics interception still remain open work.

## 2026-04-15 Session 81 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

No full matrix row moved to `LIVE` in Session 81. The session materially deepened existing `PARTIAL` military and civilizational rows by making Scout Rider harassment reach moving `supply_wagon`, active siege-supply lapse, field-water interruption, convoy-linked loyalty shock, AI convoy targeting, local counter-screen response, and restore continuity. Escort discipline and post-interdiction reconsolidation remain open work.

## 2026-04-15 Session 83 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Five-stage match progression, declared dual-clock legibility, and first runtime stage gating | DOCUMENTED | PARTIAL | Session 83 converted the newly ingested match doctrine into live browser runtime. Match stage now resolves from real founding, faith, territorial, military, rival-contact, and war-pressure conditions; stage and phase now surface in the existing dashboard; `dualClock` and `matchProgression` survive restore; Stonehelm now respects stage-aware early-war territorial and cavalry restraint. Stage-specific declaration windows, Great Reckoning logic, and imminent-engagement warning remain unfinished. |

## 2026-04-15 Session 84 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Dual-clock architecture (Declaration + Events Queue + Commitment) | DOCUMENTED | PARTIAL | Session 84 made the first declaration-side runtime layer live. Stage 5 Divine Right windows now open as public timed declarations, survive restore, expose active and incoming state in UI, drive rival AI coalition tempo, and resolve into real failure or victory. Events Queue and Commitment-phase event sequencing remain unfinished. |
| Faith Divine Right declaration window | DOCUMENTED | LIVE | Session 84 added recognition-share gating, apex covenant structure requirement, public countdown state, failure and completion resolution, world-pressure contribution, AI declaration behavior, faith-panel legibility, and restore continuity. |

The broader five-stage progression row remains `PARTIAL`, but Session 84 materially deepened it by making stage-specific declaration pressure and imminent-engagement timing live on top of the Session 83 spine.

## 2026-04-15 Session 85 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Political-event architecture on the match-progression and dual-clock spine | DOCUMENTED | PARTIAL | Session 85 added live dynasty political-event state with active and historical tracking, the first runtime event through `Succession Crisis`, severity escalation, real economy and legitimacy and loyalty and combat penalties, AI pressure response, player consolidation, and restore continuity. Covenant Test, alliance fractures, and broader event-family coverage remain unfinished. |
| Succession interface and succession-pressure legibility | DOCUMENTED | PARTIAL | Session 85 surfaced active succession-crisis severity, trigger reason, claimant count, escalation timing, runtime modifiers, and consolidation terms through the dynasty panel together with hostile-court crisis state. A dedicated full five-impact succession panel still remains unfinished. |

## 2026-04-15 Session 86 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Faith covenant tests and covenant-ascent gating | DOCUMENTED | LIVE | Session 86 made `Covenant Test` live across the four covenants and both doctrine paths. Covenant structures now feed live intensity growth, active tests apply runtime strain, direct rites resolve with real cost where canon supports them, Apex Covenant and late faith-unit access now require a passed test, Stonehelm climbs the covenant ladder and performs actionable rites, UI surfaces the event family, and restore continuity preserves both active and historical state. |
| Territorial-governance recognition and coalition backlash | DOCUMENTED | PARTIAL | Session 86 added the first Stage 5 territorial-governance runtime layer. Final Convergence kingdoms can now trigger and sustain a live recognition state from loyal stabilized holdings and territory share, world pressure reads that recognition as a coalition trigger, Stonehelm reacts against the weakest governed frontier, UI surfaces the state, and restore continuity preserves establishment and collapse. The full Territorial Governance victory seam remains unfinished because governor-seat or Governor's House coverage, anti-revolt validation, stronger no-war enforcement, and final resolution are not yet live. |

## 2026-04-15 Session 87 Reclassification Addendum

This addendum preserves the original Session 9 matrix and records later reclassifications additively.

| System | Previous | Current | Notes |
|---|---|---|---|
| Territorial-governance sovereignty-hold victory resolution | DOCUMENTED | PARTIAL | Session 87 turned the first Territorial Governance runtime layer into the first honest sovereignty seam. Multi-seat dynastic authorities now seat across governed keeps or cities or frontier marches, recognition now requires seat coverage plus court-loyalty and anti-revolt stability plus no-incoming-holy-war pressure, world pressure escalates across recognition or held governance or imminent sovereignty victory, Stonehelm answers with emergency anti-sovereignty tempo, UI surfaces the new hold states, and restore continuity now preserves completed territorial-governance victory. The broader canonical victory family remains unfinished because alliance-threshold pressure, population-acceptance buildup, and the full ~90 percent sovereignty doctrine are still not yet live. |
