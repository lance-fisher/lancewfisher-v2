# Bloodlines: Open Questions

**Purpose:** A structured catalog of every unresolved design question identified during the consolidation review. Organized by system area, with severity labels indicating how much each gap blocks downstream work.

**Severity Labels:**
- **BLOCKING** = Cannot prototype or build without answering this
- **IMPORTANT** = Significantly affects design quality and system coherence
- **ENRICHING** = Adds depth and polish but does not block core work
- **FUTURE** = Can be deferred to a later design phase

**Last Updated:** 2026-03-15

---

## Map and World Structure

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| W1 | What map type? Hex grid, province, continuous terrain, or hybrid? | BLOCKING | Q10.1 |
| W2 | What terrain types exist and how do they affect gameplay? | BLOCKING | Q10.3 |
| W3 | How are territory boundaries drawn and contested? | BLOCKING | -- |
| W4 | What is the size of a typical map (number of territories, playable area)? | IMPORTANT | -- |
| W5 | How does random world generation distribute resources and terrain? | IMPORTANT | -- |
| W6 | Are there neutral territories, contested zones, or sacred sites? | IMPORTANT | -- |
| W7 | What types of water features exist (rivers, lakes, ocean, straits)? | ENRICHING | -- |

---

## Match Flow and Pacing

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| M1 | What are the player's starting conditions (units, buildings, resources)? | BLOCKING | Q2.1 |
| M2 | What does the first 10 minutes of a match look like step by step? | BLOCKING | Q2.2 |
| M3 | What triggers the transition from Stage 1 to Stage 2? | BLOCKING | Q2.5 |
| M4 | What triggers transitions for all subsequent stages? | BLOCKING | Q2.5 |
| M5 | How does the faith selection moment work in practice? | IMPORTANT | Q2.4 |
| M6 | When does the player first encounter another faction or tribe? | IMPORTANT | Q2.3 |
| M7 | What specific match options exist in the lobby (match length, victory types, map size)? | ENRICHING | -- |

---

## Combat and Military

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| C1 | What is the combat resolution model (real-time direct, auto-resolve, hybrid)? | BLOCKING | -- |
| C2 | What units exist at Levels 2, 3, 4, and 5? | BLOCKING | Q9.1 |
| C3 | When does cavalry appear and what roles does it serve? | BLOCKING | Q9.3 |
| C4 | How does siege warfare work mechanically? | IMPORTANT | Q9.2 |
| C5 | How does terrain affect combat? | IMPORTANT | -- |
| C6 | How do bloodline commanders affect battlefield outcomes? | IMPORTANT | -- |
| C7 | How do morale, conviction, and faith affect combat performance? | IMPORTANT | -- |
| C8 | What naval units exist and how do naval/land forces interact? | ENRICHING | Q9.4 |
| C9 | What faith-specific military units or abilities exist? | ENRICHING | -- |
| C10 | What house-specific military units or abilities exist? | ENRICHING | -- |

---

## Faction Asymmetry

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| F1 | What degree of mechanical asymmetry exists between houses? | IMPORTANT | Q3.1 |
| F2 | Do houses have different starting conditions? | IMPORTANT | Q3.2 |
| F3 | Do houses have natural affinities for certain faiths? | IMPORTANT | Q3.3 |
| F4 | How does a house feel mechanically different before unique content is unlocked? | IMPORTANT | Q3.4 |
| F5 | Do houses have unique buildings, units, or technologies? | ENRICHING | Q3.1 |

---

## Faith Mechanics

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| FA1 | What exclusive content (buildings, units, abilities) does each faith unlock? | IMPORTANT | Q4.1 |
| FA2 | Can a dynasty change its faith mid-match? | IMPORTANT | Q4.2 |
| FA3 | Can multiple faiths coexist in one territory? | IMPORTANT | Q4.3 |
| FA4 | How does shared faith affect diplomatic relations? | IMPORTANT | Q4.4 |
| FA5 | What are the specific grand faith structures for each covenant? | IMPORTANT | -- |
| FA6 | What specific faith manifestation mechanics exist at extreme intensity? | ENRICHING | -- |
| FA7 | What specific resource costs does faith intensity maintenance require? | ENRICHING | -- |

---

## Conviction Mechanics

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| CV1 | Is conviction tracked per-dynasty, per-territory, per-army, or per-member? | BLOCKING | Q5.1 |
| CV2 | Is conviction visible to the player? In what form? | IMPORTANT | Q5.2 |
| CV3 | Is conviction a single axis or multiple axes? | IMPORTANT | Q5.3 |
| CV4 | What specific actions change conviction, and by how much? | IMPORTANT | Q5.4 |
| CV5 | What conviction thresholds exist and what do they unlock or lock? | IMPORTANT | -- |
| CV6 | How does conviction interact with faith alignment mechanically? | IMPORTANT | -- |

---

## Population and Economy

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| P1 | How do workers gather resources (worker-assigned or building-automated)? | BLOCKING | Q6.3 |
| P2 | What are the specific population growth rates? | IMPORTANT | Q6.2 |
| P3 | What happens when a civilian is converted to a soldier? Can they return? | IMPORTANT | Q6.1 |
| P4 | Does inter-dynasty trade exist? What can be traded? | ENRICHING | Q6.4 |
| P5 | What is the Influence secondary resource and how does it work? | ENRICHING | -- |

---

## Dynastic System

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| D1 | How does succession work when the leader dies? | IMPORTANT | Q7.1 |
| D2 | What determines which members become dormant at the 20 cap? | IMPORTANT | Q7.2 |
| D3 | Do bloodline members have inheritable character traits? | IMPORTANT | Q7.3 |
| D4 | Do bloodline members have agency, opinions, or loyalty levels? | ENRICHING | Q7.4 |
| D5 | How do bloodline members age and die? | IMPORTANT | Q7.5 |
| D6 | Can bloodline members be sacrificed through Born of Sacrifice? | ENRICHING | -- |

---

## Diplomacy

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| DI1 | What formal diplomatic states exist between dynasties? | IMPORTANT | Q8.1 |
| DI2 | How does the player interact with lesser houses? | IMPORTANT | Q8.2 |
| DI3 | What specific actions can loyalty enable that ownership alone cannot? | IMPORTANT | Q8.3 |
| DI4 | What is the diplomatic UI? | ENRICHING | -- |
| DI5 | How does AI diplomatic behavior work? | ENRICHING | -- |

---

## Technology and Progression

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| T1 | What is researched vs unlocked by level progression? | BLOCKING | -- |
| T2 | What is the building prerequisite chain? | BLOCKING | -- |
| T3 | Do faith-specific technologies or abilities exist? | IMPORTANT | -- |
| T4 | What Level 4 divergence choices exist concretely? | IMPORTANT | -- |
| T5 | What does Level 5 apex gameplay look like? | ENRICHING | -- |

---

## Victory Conditions

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| V1 | What specific threshold triggers each victory path? | IMPORTANT | Q11.1 |
| V2 | What are the defeat/elimination conditions? | IMPORTANT | Q11.2 |
| V3 | How do players know someone is approaching victory? | IMPORTANT | Q11.3 |
| V4 | How does currency dominance work mechanically? | ENRICHING | -- |
| V5 | How do prestige dispute wars work mechanically? | ENRICHING | -- |

---

## AI and Multiplayer

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| A1 | What AI behavior models/personalities exist? | ENRICHING | -- |
| A2 | How does AI prioritize faith, expansion, economy, and diplomacy? | ENRICHING | -- |
| A3 | What multiplayer lobby options exist? | FUTURE | -- |
| A4 | What network architecture supports multiplayer? | FUTURE | -- |

---

## UI/UX

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| U1 | What information is always visible on the main game screen? | IMPORTANT | Q12.1 |
| U2 | How does the player interact with the family tree? | IMPORTANT | Q12.2 |
| U3 | What is the camera model (fixed isometric, rotatable 3D, etc.)? | IMPORTANT | Q1.3 |

---

## Technical Implementation

| # | Question | Severity | Workbook Ref |
|---|----------|----------|--------------|
| TE1 | What is the target platform? | IMPORTANT | Q13.2 |
| TE2 | What engine/technology stack? | IMPORTANT | Q13.3 |
| TE3 | What is the minimum viable prototype scope? | IMPORTANT | Q14.1 |
| TE4 | What question must the first prototype answer? | IMPORTANT | Q14.2 |

---

## Resolution Tracking

When an open question is answered, record it here:

| # | Answer Date | Resolution | Applied To |
|---|------------|------------|------------|
| _(none yet)_ | | | |

---

*New questions are appended as they are discovered. Resolved questions are moved to the Resolution Tracking table but their original entry remains in the catalog.*
