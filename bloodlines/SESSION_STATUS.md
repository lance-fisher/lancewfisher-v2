# BLOODLINES — SESSION STATUS
## Session: 2026-04-07 Full Session (Two-Pass)
## Status: COMPLETE

---

## First Pass — Integration Session

### Design Bible v3.2 Produced (3,526 lines from v3.1's 2,942)

Five changes from v3.1:

1. **Document Authority updated, Source Governance Framework added** — The three-layer source model (Active Canon Snapshot, Historical Design Archive, Open Design Reservoir) is now in the bible's front matter as a permanent governance reminder for all AI sessions.

2. **Section 6 Ironmark stat corrected** — Swordsman entry was `6/5` (incorrect) and is now `5/5` (canonical per Ninth Ingestion lock). Ironmark's asymmetry is carried by the Axeman, not the Swordsman.

3. **Section 45 replaced with five-stage system** — The four stages (Founding, Consolidation, Ideological Expansion, Irreversible Divergence) replaced by the five stages per Lance Fisher direction: Founding, Expansion and Identity, Encounter/Establishment/Neutral City, War and Turning of Tides, Final Convergence.

4. **Section 62 added** — Multi-Speed Time Model: three simultaneous time speeds (Battlefield, Campaign, Dynastic), notification and queue system, generational arc by match length.

5. **Parts XII and XIII added (Sections 70-75)** — Command and Zoom Architecture, Continental World Map Architecture, Naval Doctrine as Complete Playstyle, Match Phase Architecture, Water Strategic Depth, Open Design Reservoir.

### House Starting Relationships Canonicalized (First Pass)

Seven canonical pairs locked from BLOODLINES_ADDITIVE_INTEGRATION2.md: Goldgrave vs Whitehall, Ironmark vs Stonehelm, Highborne vs Trueborn, Hartvale + Stonehelm, Oldcrest vs Westland, Trueborn vs Blood Dominion, Whitehall with all.

---

## Second Pass — Lance Design Decisions (12 Decisions)

Source: Lance Fisher direct decisions delivered at session end.

### 1. Hartvale Unique Unit — LOCKED

**Verdant Warden** (CB002 option A). Off 4 / Def 5. Hybrid guardian. Settlement defense + population loyalty bonuses. CB004 Hearthmasters voided. Section 59 updated with full spec.

### 2. Dynastic Prestige — ABANDONED as Victory Path

Prestige is now a civilizational modifier system, not a win condition. Accumulating prestige provides: population loyalty advantages, diplomatic reception improvements, reduced friction, attraction of allies and lesser houses, faith declaration legitimacy, currency dominance acceleration. Does not itself win the game.

### 3. Five Victory Paths — LOCKED (was six)

Military Conquest, Economic Dominance (Currency), Faith Divine Right, Territorial Governance, Alliance Victory. Dynastic Prestige eliminated as standalone path. All "six paths" references updated throughout the bible.

### 4. CB004 House Profiles — VOIDED (8 non-Ironmark houses)

All CB004 strategic design content for eight non-Ironmark houses removed from active canon. Only settled elements remain: hair color, color theme. Ironmark remains fully locked as the sole exception. CB004 files archived to `19_ARCHIVE/CB004_VOIDED_2026-04-07/`. Sections 53-54, 56-61 stripped to stubs.

### 5. Phase Entry Mode Variant — LOCKED

Players can enter at any of the five match phases. Phase 3 entry = Phase 3 conditions with accelerated build-up rate. Same five-phase architecture throughout, different entry point. Section 76 added.

### 6. Alliance Trigger Conditions — LOCKED

Two restrictions: (1) Alliances cannot form between opposite faith extremes. (2) If one dynasty is steamrolling, other players must have counter-coalition option. Section 77 added.

### 7. Currency Dominance — PROPOSED (redesigned)

Custom dynasty currency displaces gold as world trade standard. Mechanism: introduce custom currency, invest in devaluing gold through inflation, build adoption. When Trueborn City and other dynasties adopt the custom currency, dominant dynasty gains near-unlimited resources. Counter: Resolution Battle + coalition.

### 8. Territorial Governance — ~90% threshold (data-layer tuning, not locked)

### 9. Wild Faith Mounts — PROPOSED (expanded)

At sufficient Wild faith intensity, animals beyond bears available as cavalry mounts with higher bonuses than horse cavalry. Specific animals and thresholds to be designed. Section 79 added.

### 10. Spiritual Manifestations — Do not lock, do not remove. Remain PROPOSED.

### 11. Dynastic Prestige Modifier System — SETTLED

Section 51 completely rewritten as prestige modifier system with accumulation sources and strategic benefits.

### 12. All references propagated throughout Design Bible v3.2

Prestige Wars, six-paths references, and all related content updated. AI archetypes revised. Stage Five terminal conditions revised. Political events revised.

---

## Files Modified This Session

| File | Change |
|------|--------|
| CANONICAL_RULES.md | Twelfth Session Canon table added (13 rows) |
| CANON_LOCK.md | Hartvale LOCKED, 8 houses VOIDED, 5 paths, Mode Variants section added, Superseded entries added |
| BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md | Sections 51, 53-54, 56-61, 59 rewritten; Part XIV (76-79) added; six→five paths throughout; prestige references updated |
| BLOODLINES_COMPLETE_DESIGN_BIBLE.md | Synced to v3.2 |
| FOUNDING_HOUSES.md | Additive status note appended |
| 19_ARCHIVE/CB004_VOIDED_2026-04-07/ | CB004 files archived here |

---

## Grand Unified Document Regenerated

`18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md` — **15,091 lines** (up from 14,692)

Now includes all v3.2 changes: Part XIV, updated Section 51, house stubs for 8 non-Ironmark houses, Canon Lock v2, Part B2.

`D:\BLOODLINES_COMPLETE_UNIFIED_v1.0.txt` — Standing rule copy updated.

**Use this file to initialize any new AI session on Bloodlines. It contains everything.**

---

## Website Deployed

All updated files synced to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\` via sync-to-web.sh.

---

## Outstanding Questions — Decisions Required From Lance

### 1. 8 Non-Ironmark House Strategic Profiles

CB004 has been voided. Hair color and color theme are the only settled elements. Full house strategic design (unique unit, squad trait, stat deviations, victory path affinities, narrative identity) is pending. This is the largest open design gap.

### 2. Victory Condition Thresholds

Three remain unspecified:
- **Faith Divine Right:** Global faith share % to trigger declaration window (working assumption: 40-50%)
- **Currency Dominance:** Adoption metric (% of dynasties or % of trade volume)
- **Territorial Governance:** Exact loyalty % (working assumption: ~90%)

These can be locked or explicitly deferred to data-layer tuning.

### 3. Proposed → Settled Candidates

Two systems fully designed, ready for elevation:
- Spiritual Manifestations (FAITH_SYSTEM.md) — angels, demons, cursed armies at extreme thresholds
- Currency Dominance full mechanics (RESOURCE_SYSTEM.md)

### 4. Multiplayer Architecture

Four open questions:
- Diplomatic interactions between human players in real time
- Formal alliances with custom in-session terms
- Simultaneous conflicting player actions in same territory
- Team/co-op modes vs. free-for-all only

---

## Recommended Next Steps

1. **DESIGN: 8 non-Ironmark house profiles** — hair color + color theme are the only settled elements; full strategic design pending
2. **DESIGN: Bloodline population/family scope** — managing hundreds of family members across generations
3. **DESIGN: Personality Trait System** — what traits do Bloodline Members have; how do they express in events
4. **DESIGN: Wild faith animal mounts** — specific animals, thresholds, bonuses beyond bears
5. **SETTLE: Currency Dominance mechanics** — full mechanics need design and lock
6. **SETTLE: Spiritual Manifestations** — fully designed, should move from PROPOSED to SETTLED
7. **SPECIFY: Victory thresholds** — lock or explicitly defer to data layer

---

## File Inventory (Key Files)

| File | Location | Lines | Purpose |
|------|----------|-------|---------|
| BLOODLINES_COMPLETE_UNIFIED_v1.0.md | 18_EXPORTS/ | 15,091 | **USE THIS FOR NEW AI SESSIONS** |
| BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md | 18_EXPORTS/ | ~3,600+ | Current authoritative bible (v3.2 + Part XIV) |
| BLOODLINES_COMPLETE_DESIGN_BIBLE.md | 18_EXPORTS/ | same | Synced to v3.2, displayed on website |
| CANONICAL_RULES.md | 01_CANON/ | 360+ | All settled/proposed/open decisions |
| CANON_LOCK.md | 01_CANON/ | 143 | High-level lock summary |
| BLOODLINES_MASTER_MEMORY.md | 01_CANON/ | 2,807 | Cumulative session history (91 sections) |
| FOUR_ANCIENT_FAITHS.md | 07_FAITHS/ | 1,778 | Detailed faith content |
| POLITICAL_EVENTS.md | 11_MATCHFLOW/ | 980 | All 28+ political events |
| FAITH_SYSTEM.md | 04_SYSTEMS/ | 749 | Faith mechanics deep-dive |

---

*Session complete. Next session can start from BLOODLINES_COMPLETE_UNIFIED_v1.0.md for full context.*
