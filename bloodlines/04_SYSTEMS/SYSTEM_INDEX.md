# Bloodlines — System Index

Overview of the core game systems that define the Bloodlines experience. Each system has a dedicated file with detailed mechanical documentation.

## Core Systems

| System | File | Description |
|--------|------|-------------|
| Conviction | `CONVICTION_SYSTEM.md` | Belief, morale, and ideological commitment as a gameplay mechanic |
| Faith | `FAITH_SYSTEM.md` | The four ancient faiths and their mechanical influence |
| Population | `POPULATION_SYSTEM.md` | Population growth, consumption, migration, and military manpower |
| Resource | `RESOURCE_SYSTEM.md` | Economic resources, production, trade, and scarcity |
| Territory | `TERRITORY_SYSTEM.md` | Land control, borders, fortification, and strategic geography |
| Dynastic | `DYNASTIC_SYSTEM.md` | Bloodline traits, succession, marriage, inheritance, generational arcs |
| Born of Sacrifice | `BORN_OF_SACRIFICE_SYSTEM.md` | Sacrifice-driven creation of elite units and extraordinary power |
| Fortification | `FORTIFICATION_SYSTEM.md` | Defender-side mechanics for layered fortifications, keep-as-bloodline-seat, and defensive leverage (canon locked 2026-04-14) |
| Siege | `SIEGE_SYSTEM.md` | Attacker-side mechanics for earned siege operations, plural commitments, and failure-penalty simulation (canon locked 2026-04-14) |
| Match Structure | `11_MATCHFLOW/MATCH_STRUCTURE.md` | Five-stage condition-gated match progression with three overlay phases (canon locked 2026-04-07, deepened 2026-04-15) |
| Dual-Clock Time | `01_CANON/CANON_LOCK.md` (Addendum A) | Declared Time Model: battle clock + dynasty clock joined by declaration seam (canon locked 2026-04-07, ranges locked 2026-04-15) |
| Multiplayer Engagement | `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine_RAW.md` | Shared calendar, no individual pause, imminent engagement warning with live countdown (canon locked 2026-04-15) |

## System Interdependencies

These systems are not isolated. They interact in meaningful ways:

- **Population** feeds **Resource** production and **Army** composition
- **Conviction** is influenced by **Faith** and affects **Population** loyalty
- **Territory** determines **Resource** access and **Population** capacity
- **Dynastic** traits can modify all other systems through bloodline effects
- **Born of Sacrifice** consumes **Population** and **Conviction** to produce elite power

*Detailed interaction maps will be added as systems are developed.*

---

### System Depth Summary (Updated 2026-03-19 — Session 9)

| System | File | Lines | Status | Core Content Documented |
|--------|------|-------|--------|------------------------|
| Conviction | `CONVICTION_SYSTEM.md` | 334 | SOLID | CP weight system, pattern amplification (1x/1.5x/2x), five bands (Apex Moral/Moral/Neutral/Cruel/Apex Cruel), milestone powers, conviction-as-governance-posture, population loyalty response per band, dark extremes world pressure trigger |
| Faith | `FAITH_SYSTEM.md` | 749 | EXPANDED (Session 9) | All four covenant theologies at adult/serious tone (Old Light, Blood Dominion, The Order, The Wild), full light/dark doctrine path mechanics per covenant, unit roster L3-L5 with canonical names, building progressions with canonical names, Covenant Test mechanics, faith intensity five-tier system (Latent/Active/Devout/Fervent/Apex), global faith tracking, conviction/faith independence, doctrine tone standard LOCKED |
| Population | `POPULATION_SYSTEM.md` | 126 | EXPANDED (Session 9) | 90-second game cycle, housing as core mechanic (replaces RTS power plant), population pool allocation categories (agricultural/craft/military/faith/governance), faith distribution per population segment, population as conviction mirror (Moral/Neutral/Cruel behavioral effects), dark extremes world pressure coalition trigger, house-specific notes (Hartvale 25% faster recovery, Goldgrave trade-route military cap) |
| Resource | `RESOURCE_SYSTEM.md` | 138 | FOUNDATIONAL | Six primary resources (gold, food, water, wood, stone, iron), building production chains, iron as sixth resource and mid-to-late game strategic differentiator, resource consumption by army tier, Iron Ridges as terrain type tied to resource, caravan and trade network economics, resource-to-military conversion chains |
| Territory | `TERRITORY_SYSTEM.md` | 113 | EXPANDED (Session 9) | Three-layer map architecture (terrain base/province overlay/boundary calculation layer), territory control clock mechanics with variable factors, contested province behavior, faith influence diffusion through territory, province-level conviction posture mechanics, Territorial Governance victory path full specification, house-specific territory notes (Stonehelm 20% faster fortification, Whitehall 25% faster integration, Hartvale 25% faster recovery, Oldcrest 20% reduced loyalty decay) |
| Dynastic | `DYNASTIC_SYSTEM.md` | 291 | EXPANDED (Session 9) | Three-tier family management (Tier 1: 8-12 committed/fully managed; Tier 2: up to 20 active/individually tracked; Tier 3: dormant/branch management), eight commitment roles (War, Trade/Economic, Defense, Governance, Faith, Scholarly/Advisory, Covert Operations, Diplomatic), succession interface with five impact metrics, marriage and cross-dynasty mechanics, generational scale management, Nested Family Unit model for large bloodlines |
| Born of Sacrifice | `BORN_OF_SACRIFICE_SYSTEM.md` | 313 | EXPANDED (Session 9) | Full lifecycle walkthrough (five stages: recruitment → veteran service → recycling decision → ceremony → outcome), champion emergence mechanics (three paths: military command/lesser house creation/exceptional service unit), faith consecration per covenant (Old Light Flame Warden blessing; Blood Dominion blood rite light/dark variants; The Order mandate blessing; The Wild beast bond), population calculus and decision framework, narrative layer — the weight of what soldiers cost |

---

## Full System Interdependency Map (Session 9)

The seven systems form a closed loop. No system operates independently of the others. Understanding cross-system interactions is essential to understanding the strategic depth of any match arc.

**Population → Resource:** Population is the labor force behind all resource production. Agricultural labor drives food and water output. Craft labor drives gold, wood, stone, and iron output. Population decline reduces resource production before any other indicator of crisis appears. The 90-second cycle connects population state to production output every cycle.

**Population → Military:** Military units are drawn from the population pool. Five people leave the productive civilian population for every recruited squad. Dead soldiers are gone from the population permanently. The Born of Sacrifice system creates the strategic pressure: veteran armies represent significant population investment, and the decision to recycle versus retain them is a population management decision as much as a military one.

**Territory → Population:** Territory determines housing capacity, and housing is the hard ceiling on population size. A dynasty that has not built Housing Districts cannot grow its population regardless of food and water surplus. Territory type also affects population growth rates — Reclaimed Plains support dense population, Tundra and Frost Ruins support little.

**Territory → Resource:** Resource nodes are located in specific terrain types. Iron Ridges contain iron deposits. River Valleys have water. Ancient Forest yields wood. The distribution of resources across the map is a territory distribution problem — controlling resource-relevant terrain is the economic expression of territorial control.

**Conviction → Population:** The population reads conviction and responds to it. Moral conviction generates civic engagement events, volunteer recruitment bonuses, loyalty that exceeds what prosperity alone would justify. Neutral conviction produces functional performance. Cruel conviction generates fear-compliance with short-term output parity and catastrophic long-term loyalty collapse risk when the fear mechanism weakens.

**Conviction → Faith:** The two axes are independent, but they interact. Faith doctrine and conviction combine to produce specific unlocks and restrictions. Dark doctrine at Cruel conviction unlocks unbeliever sacrifice. Light doctrine at Moral conviction unlocks Bloodline Member devotion paths. The interaction is not automatic — it is the product of two independent systems reaching specific thresholds simultaneously.

**Faith → Territory:** Faith spreads through territory like a slow-moving resource. Sacred Ground amplifies faith intensity for any shrine placed within it. Faith presence in a province can generate loyalty friction if the ruling dynasty's faith differs from the population's faith. Province loyalty is partially a function of faith alignment between ruler and governed.

**Faith → Population:** Population segments track which covenant they follow. Conquered populations carry their prior covenant affiliation. Faith distribution within the population affects faith intensity generation — a dynasty can have faith buildings but if its population has low faith engagement, intensity will not climb above the Active tier. The global faith distribution tracked through the Trueborn City information network aggregates individual population faith states.

**Dynastic → All Systems:** Bloodline Members act as force multipliers across every system. A War commitment member improves military output. A Defense commitment member improves loyalty stability in a province. A Trade/Economic commitment member improves resource generation on a caravan route. A Governance commitment member improves territory integration rate. A Faith commitment member improves faith intensity generation. A Scholarly commitment member provides research and event-response advantages. The Dynastic system is the mechanic through which player identity — the specific people in the bloodline and their traits — modifies every other system's behavior.

**Born of Sacrifice → Conviction:** The decision to perform a Born of Sacrifice ceremony is itself a conviction event. The magnitude of the conviction change depends on the scale of the sacrifice and the conviction posture under which it is performed. Light-doctrine faith consecrations generate positive conviction. Dark-doctrine ceremonies at extreme thresholds generate negative conviction. A dynasty that cycles armies through Born of Sacrifice repeatedly is accumulating conviction momentum — in either direction, depending on how the ceremonies are conducted.

**Born of Sacrifice → Population:** Every sacrifice ceremony removes population from the realm permanently. The veterans who have been recycled back into the population pool are not sacrificed — they return to productive civilian life. The sacrifice element refers to specific faith consecration mechanics that consume population as part of the ceremony. This is the highest-cost conviction action in the game and the one with the most significant population-side consequences.

**Resource → Military:** Military capability scales with resource availability. Iron is required for Swordsmen, cavalry equipment, fortification, and siege equipment. Gold funds recruitment, training, and upkeep. Food sustains armies in the field — an army without food supply begins experiencing attrition. The resource-to-military conversion chain is the primary mechanism through which economic development translates into military capacity.

**Diplomatic and Political Events (cross-system):** Every political event is a cross-system interaction. The Great Reckoning fires on territory threshold and triggers conviction effects across all dynasties. The Covenant Test fires on faith intensity threshold and demands resource and population expenditure. The Succession Crisis fires on dynastic health indicators and disrupts all system outputs simultaneously. The Dark Extremes world pressure fires on population suffering scale and conviction depth, producing a coalition event that is primarily diplomatic but driven by faith and conviction inputs.

---

*This index is updated at the end of each major design session. System files are append-only and grow continuously.*

---

### System Depth Update (2026-04-25 — Early-Game Foundation Session)

| System | File | Previous Lines | Current Lines | New Additions |
|--------|------|---------------|--------------|---------------|
| Population | `POPULATION_SYSTEM.md` | 126 | ~250 | Population productivity states (Civilian 100%/Untrained 75%/Reserve 50%/ActiveDuty 5%), effective productivity calculation (shortage modifiers: food 0.70, water 0.65, housing 0.85), military draft system (0-100% step-5, DraftPool/TrainedMilitary/UntrainedDrafted), squad system (5-person canonical squads, Reserve/ActiveDuty duty states, 9 assignment types), productivity-military tradeoff design intent |
| Resource | `RESOURCE_SYSTEM.md` | 228 | ~310 | Build tier system (Tier 0 pre-deploy / Tier 1 Keep placed / Tier 2 four prerequisites met / Tier 3+ full tree), three production models (passive trickle / active gather / worker slot), worker slot formula (baseRate × assignedWorkers × effectiveProductivity × dt), worker slot building roster with capacities and rates, water capacity infrastructure model (Keep 15 base + Well 50 per building = MaxSupportedByWater hard population cap) |

**New inter-system wiring documented in this session:**

- Population productivity states wire directly into the worker slot production output — EffectiveProductivity is a per-faction multiplier applied to all slot-based building output. An army in the field is not just a military fact; it is an economic penalty on every active worker slot.

- Water capacity creates a hard population growth gate separate from the housing cap. Both housing cap (PopCap) and water cap (MaxSupportedByWater) must be above Total Population for growth to occur. Water infrastructure (Well buildings) must be built proportionally to population target — one Well supports 50 people beyond the Keep's 15-person base.

- The draft slider interacts with both systems simultaneously: higher draft rates increase UntrainedDrafted (reducing BaseProductivity) while the resulting squads in Reserve further reduce it (50%), and squads on Active Duty nearly eliminate it (5%). Economic cost of military is continuous, not one-time.

- Build tier prerequisites enforce the design intent that a sustainable settlement requires all four infrastructure pillars (housing, water, food, training) before advancing. Factions that rush military (Training Yard first) without investing in housing and water will hit the Tier 2 gate incomplete and cannot access Small Farm food scaling.
