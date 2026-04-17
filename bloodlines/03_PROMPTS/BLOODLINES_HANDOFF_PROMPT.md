# Bloodlines — Complete Project Handoff Prompt

Use this prompt to onboard any AI system, design tool, or collaborator into the Bloodlines project. Paste the full text below into your session. Do not shorten it.

---

## BEGIN PROMPT

---

# BLOODLINES — Game Design Project Briefing

You are being brought into an active, large-scale game design project called **Bloodlines**. This document explains what the project is, what has been built so far, what remains to be designed, and the rules that govern how work is done on this project. Read this entire document before responding or taking any action.

---

## What Bloodlines Is

Bloodlines is a dynasty-driven, large-scale medieval real-time strategy game where faith, sacrifice, and bloodline shape the fate of civilizations. The design draws from Command & Conquer-style RTS mechanics (base building, resource gathering, army composition) combined with Warcraft 3-aligned gameplay (lower unit count, micro-management emphasis, hero-equivalent anchors) and layered with deep political, dynastic, and faith systems inspired by the narrative weight of Game of Thrones.

The game is not a simple RTS with a story mode bolted on. It is an RTS where the dynasty IS the game. Your bloodline members are named characters with personality traits, functional roles, marriages, children, and deaths that reshape your civilization. Political maneuvering, marriages, succession crises, betrayal, and assassination carry equal strategic weight to military action. Every match should generate a story worth retelling.

**Core design pillars:** Territory, Population, Faith, Dynasty.

**Seven core game systems:** Conviction (behavioral morality spectrum), Faith (four ancient covenants with light/dark doctrine paths), Population (unified realm-wide pool), Resource (six primary resources: gold, food, water, wood, stone, iron), Territory (military control + population loyalty), Dynastic (bloodline family management across generations), Born of Sacrifice (elite army lifecycle and veteran emergence).

**Nine founding houses:** Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest. Each house has a distinct visual identity (hair color, color palette, symbol family, architectural language), subtle base stat variations on shared units, and one or more unique units exclusive to that house.

**Four ancient faiths (covenants):** Old Light, Blood Dominion, The Order, The Wild. Each faith has a light doctrine path and a dark doctrine path. Faith is selected at the end of Stage 1 of a match and permanently shapes the civilization's military roster, rituals, buildings, and world interactions from Level 3 onward.

**Six victory paths:** Military Conquest, Economic Dominance (currency standard), Faith Divine Right (declared, not passive), Territorial Governance (near-impossible difficulty), Dynastic Prestige, Alliance Victory. All victory paths are designed to be extremely difficult. The game naturally trends toward stalemate, coalition formation, and political resolution rather than clean solo conquest.

**Match scale:** 2-10+ hours, up to 10 players with AI kingdoms and minor tribes. Four match stages: Founding, Consolidation, Ideological Expansion, Irreversible Divergence. Five-level military progression where Level 4 is an irreversible divergence point and Level 5 produces apex units (one per faith, only one or two can exist per match).

**Tonal anchor:** The world must feel like a living, breathing version of the world behind Game of Thrones. Named heroes with personal stakes. Betrayal, marriage alliances, succession crises, assassination. No house is purely good or evil. Every powerful character is complex. Faith content is serious, adult, and R-rated in design sensibility. Ritual design reflects genuine cost, darkness, and consequence.

---

## Scope Philosophy — Read This Before Doing Anything

**Bloodlines has no size or scope ceiling.** There is no point at which the project is too big. Conventional game development scope constraints (feature cuts, scope reduction, feasibility trimming) do not apply here. Depth is the goal. Every session should make the design richer, not smaller. If something can go deeper, it goes deeper. The risk is building too little, not too much.

**Every system the player touches must feel like it has been thought through five levels deeper than they can see.** Inexhaustible designed depth is the target, not procedural randomness.

**Do not suggest scope reduction, feature cuts, or "MVP" thinking.** Do not suggest simplifying systems to make them more "achievable." Do not apply conventional game dev project management constraints to this project. If you find yourself thinking "this is too complex," that thought is wrong in this context. Complexity and depth are the deliverable.

---

## What Has Been Built — Project Corpus Inventory

The project lives in a structured 20-folder archive system (folders numbered 00 through 19). The total design corpus exceeds **15,000 lines of original design documentation** across 50+ files. Nine design ingestion sessions have been completed. Four Creative Branch explorations have been produced. The project is in Phase 1: Design Content Population (pre-prototype, documentation-first).

### Canon Files (01_CANON/)

| File | Lines | Content |
|---|---|---|
| BLOODLINES_MASTER_MEMORY.md | 2,769 | Cumulative design memory, 90 sections spanning all 9 ingestion sessions. Every design decision, system expansion, and creative exploration is recorded here. This is the living memory of the project. |
| BLOODLINES_DESIGN_BIBLE.md | 289 | Original 15-section design bible scaffold from the first ingestion. Serves as the structural template. |
| CANONICAL_RULES.md | 280 | Tracks which design elements are SETTLED (locked canon), PROPOSED (awaiting confirmation), or OPEN (still being explored). Contains 9 ingestion tables with 200+ canonical decisions. This is the source of truth for what has been decided. |

### Complete Design Bible (18_EXPORTS/)

| File | Lines | Content |
|---|---|---|
| BLOODLINES_COMPLETE_DESIGN_BIBLE.md | 2,813 | The comprehensive design bible, version 3.0. All 62 sections complete. Sections 1-44 cover Sessions 1-8 content. Sections 45-62 cover Session 9 content including all match stages, diplomatic systems, victory paths, all nine houses at full depth, and a closing design manifesto. This is the single most important reference document in the project. |

### Core System Files (04_SYSTEMS/)

All seven systems plus the dynastic system have dedicated files with substantive design content:

| File | Lines | Content |
|---|---|---|
| SYSTEM_INDEX.md | 77 (11,492 chars) | System depth summary table and full cross-system interdependency map |
| POPULATION_SYSTEM.md | 126 | 90-second growth cycle, housing, pool allocation, conviction mirror effects |
| RESOURCE_SYSTEM.md | 228 | Six resources, trade network, production chains, caravans, house-specific economic notes |
| TERRITORY_SYSTEM.md | 113 | Three-layer map (continuous terrain + province overlay + tile grid), control clock, faith diffusion, governance victory mechanics |
| CONVICTION_SYSTEM.md | 334 | CP weight system with pattern amplification (1x/1.5x/2x), five moral bands (Apex Moral through Apex Cruel), 10 dated content strata, milestone powers for both moral and cruel paths |
| FAITH_SYSTEM.md | 749 | All four covenants with theology, doctrine paths, faith intensity thresholds (5-tier: Latent through Apex), maintenance/decay mechanics, Level 3-5 faith-specific unit roster |
| BORN_OF_SACRIFICE_SYSTEM.md | 313 | Population-constrained army lifecycle, champion emergence mechanics, faith consecration paths per covenant |
| DYNASTIC_SYSTEM.md | 292 | Three-tier family management model (committed/active/dormant), 8 commitment roles, succession mechanics with uprising events, marriage system, generational scale |

### Faith Files (07_FAITHS/)

| File | Lines | Content |
|---|---|---|
| FOUR_ANCIENT_FAITHS.md | 1,778 | Three content layers: original covenant descriptions (lines 1-327), Session 9 theological expansion (lines 328-907), CB003 adult-tone faith doctrine mechanics (lines 908-1778). All four faiths with full doctrine path design. |

### Faction Files (06_FACTIONS/)

| File | Lines | Content |
|---|---|---|
| FOUNDING_HOUSES.md | 360 | All nine founding houses with profiles, hair color assignments, naming history, conflict resolutions |
| FACTION_INDEX.md | 14 | Stub — not yet populated |

### Unit Files (10_UNITS/)

| File | Lines | Content |
|---|---|---|
| UNIT_INDEX.md | 466 | Level 1-5 military progression. Level 1: faith-neutral survival units. Level 2: iron transition, cavalry. Level 3-4: faith-specific units (two per faith per level, one light doctrine, one dark). Level 5: one apex unit per faith. Six naval vessel types. Global squad model (1 selection = 1 squad = 5 individual unit-characters with shared health pool and visual degradation). |

### Match Flow Files (11_MATCHFLOW/)

| File | Lines | Content |
|---|---|---|
| MATCH_STRUCTURE.md | 252 | Four match stages, six victory paths expanded, sovereignty seat concept, Trueborn Rise Arc timing, world events reference table |
| POLITICAL_EVENTS.md | 980 | 34 named political events across 6 categories (Faith, Dynastic, Economic, Military, Diplomatic, World) plus Victory Path Triggers and Late-Game Escalation. Each event has trigger conditions, recipients, effects, resolution paths, and counter-play. |

### World, UI, Audio-Visual

| File | Lines | Content |
|---|---|---|
| WORLD_INDEX.md (09_WORLD/) | 449 | Full procedural world generation parameters, ten canonical terrain types, regional culture, diplomatic starting conditions, Trueborn City fixed-point rule |
| UI_NOTES.md (12_UI_UX/) | 275 | Keep/Home UI system, all match screens A through K |
| AUDIO_VISUAL_DIRECTION.md (13_AUDIO_VISUAL/) | 194 | Sound design direction, music direction, art style, visual identity framework |

### Lore (05_LORE/)

| File | Lines | Content |
|---|---|---|
| TIMELINE.md | 169 | World chronology: Age Before, The Great Frost (~170 years), Age of Survival, Age of Reclamation. Match begins Year 80 of Reclamation. Iron deposits exposed by glacial retreat. Trueborn City predates the Frost. |

### Creative Branch Files (14_ASSETS/)

Four Creative Branch explorations have been completed. These are exploratory design documents where ideas are proposed and developed in depth before being canonized:

| Branch | Files | Lines | Content |
|---|---|---|---|
| CB001 | 1 | 435 | Unit progression, house identities, terrain types, conviction weights, political events |
| CB002 | 10 | 6,337 | House profiles with Off/Def stat deviations, political events (28), buildings and Covenant Tests, population and territory, minor tribes (10 archetypes), AI kingdoms, bloodline members, faith doctrine, victory conditions |
| CB003 | 5 | 3,222 | Bloodline family system, faith doctrine rewrite (adult tone), victory conditions (6 paths), house lore and pre-Frost histories and rivalries, sample match narrative |
| CB004 | 4+index | 1,885 | All 8 remaining bloodlines at Ironmark depth (13-element profile: identity statement, primary/accent colors, dominant symbol with variants, societal advantage, required disadvantage, squad trait, unit stat deviations, unique unit, victory path affinity, faith tendency, political style, flavor text) |

### Session Ingestion Archive (02_SESSION_INGESTIONS/)

| File | Lines | Content |
|---|---|---|
| SESSION_2026-03-15_player-manual-raw-input.md | 1,158 | Verbatim preservation of the 60-part Player Manual raw input. Never modified. |
| SESSION_2026-03-15_player-manual-ingestion.md | 155 | Processed ingestion summary |
| SESSION_2026-03-15_first-substantive-ingestion.md | 105 | First 4-session consolidation |

### Legacy Archive (19_ARCHIVE/)

Three pre-project design files archived from the original `bloodlines game` working folder:

| File | Size | Content |
|---|---|---|
| LEGACY_AI_Copy_Prompt_pre-project.txt | ~78KB | Original project bible and AI memory injection prompt from pre-structure sessions |
| LEGACY_bible_pre-project.txt | ~63KB | Earlier design bible draft |
| LEGACY_master_prompt_bloodline_details_pre-project.txt | ~30KB | Original canonical design memory. Contains unique content including legitimacy/perception layer, recruitment slider system, covert action/assassination mechanics, captured bloodline member handling options |

---

## What Has Been Decided — Key Locked Decisions

These are non-negotiable design decisions that have been marked SETTLED and LOCKED in the canonical rules. Do not contradict, modify, or reinterpret any of these:

- **Nine founding houses** with specific names, hair colors, and visual identity systems
- **Four ancient faiths** with light/dark doctrine paths
- **Six primary resources:** gold, food, water, wood, stone, iron
- **Five-level military progression** with faith-neutral L1, iron-transition L2, faith-influenced L3, faith-specific L4 (irreversible divergence), and apex L5
- **Global squad model:** 1 selection = 5-member squad with shared health pool and visual degradation
- **Ironmark bloodline fully locked:** Charcoal Iron primary color, Ember Red accent, Axe symbol (5 variants), early iron access advantage, Blood Production disadvantage, Ferocity Under Loss squad trait, Axeman unique unit (Off 6 / Def 4)
- **Born of Sacrifice redesigned:** population-constrained army lifecycle, NOT one-time ritual sacrifice for superunits
- **Warcraft 3-aligned RTS gameplay:** lower unit count, micro-management emphasis, bloodline members as hero-equivalent anchors
- **Conviction terminology:** Moral/Cruel axis (not High/Low)
- **Faith and Conviction are independent systems** that interact but must never be forced into alignment
- **All victory paths are extremely difficult.** Individual dynasty victory is designed to be rare.
- **Faith doctrine is serious, adult, R-rated.** Sanitized ritual design is explicitly rejected.
- **Game of Thrones tonal anchor** for world feel
- **Bloodline family depth is a significant core mechanic** — marriages, children, generational offspring, political marriages, family management are deep and central
- **Political conflict equals military conflict** in strategic weight
- **Narrative generation is a design goal** — every match should produce a story worth retelling
- **Project scope has no ceiling** — depth is always the goal

---

## What Still Needs Design Work

These are the known design gaps, unresolved conflicts, and areas that need expansion to bring the game design to completion. They are listed roughly in priority order:

### Unresolved Design Conflicts (Require Decision)

1. **Hartvale unique unit conflict:** Creative Branch 002 proposed "Verdant Warden" (hybrid fighter-healer), Creative Branch 004 proposed "Hearthmasters" (non-combat logistics unit). Only one can be canonical. A decision is needed.

2. **CB004 canonicalization:** All eight remaining bloodline profiles (Highborne, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest, Trueborn) were produced at Ironmark depth in CB004 but are still marked PROPOSED. Each profile needs review and formal SETTLED status to match Ironmark's canonical lock.

### Systems That Need Deeper Design

3. **Bloodline population/family scope mechanics:** The OPEN design question of how to manage potentially hundreds of living family members across generations. Natural thinning mechanics (long winters, famine, plague as environmental events) have been considered but not designed. This is the highest-priority open system design gap.

4. **Bloodline Member personality traits:** SETTLED as a concept (each member has traits affecting decisions, NPC reactions, player choices) but the actual trait taxonomy, trait effects, and trait interactions have not been designed. A Cruel Warlord and a Moral Warlord should feel completely different to play — the system for that difference does not yet exist.

5. **House historical rivalries and starting relationships:** SETTLED as a concept (named rivalries, territorial disputes, ideological incompatibilities that predate the match) but the full rivalry and alliance mapping between all nine houses has not been designed.

6. **House lore depth:** Each house needs fully designed pre-Frost history, what they lost, what they believe, and why they fight. CB003 has house lore and rivalries (~681 lines) but this has not been formalized into canonical house identity documents. Some houses are deeper than others.

7. **Faction asymmetry — full stat assignments:** The hybrid asymmetry model is SETTLED (shared tech tree + subtle stat deviations + unique units per house), but the full per-house Off/Def stat tables for all unit types have not been formally locked. CB002 has proposed values; CB004 has proposed values. Neither set is canonical.

8. **Level 2 unit roster:** Partially defined in CB002 but not at the same depth as Level 1 or Levels 3-5. The iron-transition tier needs full unit definitions, stat tables, and house-specific variations.

9. **Unique unit assignments — full mapping:** Each house should have one or more exclusive units. Ironmark's Axeman is fully locked. The other eight houses have proposed unique units in CB002 and CB004 but these are not finalized.

### Systems That Could Go Deeper

10. **Naval combat mechanics:** Six vessel types are defined. Harbor tiers exist. Amphibious operations are designed. But tactical naval combat (ship-to-ship engagement, boarding actions, naval formation, wind/current effects) has not been detailed.

11. **Siege warfare mechanics:** Siege equipment is referenced throughout but a dedicated siege system (siege engines, wall breach mechanics, sally-forth mechanics, starvation siege timing) has not been written as a standalone design document.

12. **Lesser house system:** The concept is SETTLED (created through titles/land grants, optional AI or player-controlled, naming must include parent bloodline root). But lesser house creation mechanics, lesser house revolt mechanics, lesser house diplomatic behavior, and lesser house personality have not been designed in depth.

13. **Minor tribe depth:** 10 archetypes are designed in CB002 with diplomacy and military units. Could go deeper into individual tribe lore, territory behavior, tribute mechanics, and integration/conquest consequences.

14. **AI Kingdom behavior:** Documented in CB002 but the AI decision-making model (how AI dynasties choose faith, pursue victory paths, form alliances, handle succession, manage conviction) has not been designed as a standalone system.

15. **Building tech tree per house:** Five building categories exist. Faith-specific building names are defined. Covenant Tests are designed. But house-specific building variations, unlock conditions, upgrade paths, and economic building chains could go deeper.

16. **Trade route system:** Caravans, trade networks, Trueborn City as hub, and trade agreements are documented. But physical trade route mechanics (caravan pathing, route protection, trade interdiction, trade route discovery) could be a standalone system.

17. **Covert operations system:** Assassination, espionage, sabotage, and captured bloodline member handling are referenced in the legacy master prompt and in various Creative Branch files, but no dedicated covert operations system document exists. The mechanics for dynasty-conducted covert actions (assassination attempts, spy networks, sabotage operations, intelligence gathering) need their own system design.

18. **Legitimacy and perception layer:** Referenced in the legacy master prompt: "Military success, wealth, cultural dominance, and religious infrastructure can make harsh actions seem justified to populations. Legitimacy affects loyalty and sabotage." This concept exists but has not been formalized into a system. It connects to conviction, population loyalty, and diplomatic reputation but is not documented as its own mechanic.

19. **Recruitment slider system:** SETTLED as a concept (adjustable sliders, not hard-locked doctrines). Three components (family obligation, paid professional soldiers, faith militancy volunteers) are referenced. The detailed slider mechanics, tradeoffs per component, and house-specific recruitment tendencies have not been fully designed.

20. **World events depth:** The world events table in MATCH_STRUCTURE.md references Great Reckoning, Covenant Test, Trueborn Rise Arc, Dark Extremes, Succession Crisis, and Trueborn Summons. POLITICAL_EVENTS.md has 34 events. But world-scale environmental events (Great Plague, continental famine, volcanic winter, resource vein discovery) and their systemic effects could be designed.

21. **End-of-match progression system:** PROPOSED as an XP rewards system for all placements, providing progression and replayability. Not yet designed.

22. **Map design — specific content:** Procedural generation parameters are complete (449 lines). But specific named regions, landmarks, chokepoints, and strategic terrain features for reference maps have not been designed.

---

## Archival Rules — Non-Negotiable

These rules govern how all content in the Bloodlines project is handled:

1. **Nothing may be deleted without explicit authorization.**
2. **Nothing may be summarized or shortened.** If content exists, it stays at full length.
3. **New information is always appended, never replaced.** Old content is preserved alongside new additions. Conflicting ideas are both kept.
4. **Project memory grows continuously.** Historical context remains intact.
5. **Design content files use additive-only dating.** Each addition is marked with a dated section header. Old sections are never overwritten.
6. **The only files that may be updated in place** are operational tracking files (PROJECT_STATUS.md current status fields, HANDOFF.md session handoff content). Design content is never replaced.

---

## How to Work on This Project

1. Before proposing new design work, read the relevant existing files. The content is deep. Do not assume you know what has been designed — verify by reading the source.
2. New content is always additive. If you are expanding a system, append to the existing document with a dated section header. Do not rewrite what exists.
3. Proposed changes to SETTLED/LOCKED canon require explicit authorization. Do not silently contradict locked decisions.
4. Every design addition should make the game richer, not simpler. If you find yourself reducing scope, stop. That is not the goal.
5. The Design Bible at `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE.md` (2,813 lines, 62 sections) is the best single-document overview of the full design. Start there if you need to understand the whole picture.
6. `01_CANON/CANONICAL_RULES.md` is the source of truth for what has been decided. Check it before proposing anything that might contradict a settled decision.
7. `01_CANON/BLOODLINES_MASTER_MEMORY.md` (2,769 lines, 90 sections) is the cumulative session memory. It contains everything that has ever been discussed, decided, or explored.

---

## What "Done" Means for This Project

The game design is "complete" when:

- Every system has a dedicated design document at the depth of CONVICTION_SYSTEM.md (334 lines) or FAITH_SYSTEM.md (749 lines) — not as a scaffold, but as a fully realized mechanical specification
- Every founding house has a fully locked canonical profile at Ironmark depth (13-element template)
- Every design gap listed above has been resolved with original, substantive design content
- The Design Bible reflects all canonical decisions and can serve as a complete reference for a development team to build from
- A developer could read the documentation and implement any system without needing to ask the designer clarifying questions about intent, edge cases, or interactions

The project is currently approximately 60-65% of the way to that standard. The foundation and architecture are complete. The seven core systems are designed. The nine houses exist. The four faiths are detailed. What remains is filling in the gaps listed above, locking proposed content into canon, and ensuring every system reaches the depth standard.

---

## END PROMPT

---
