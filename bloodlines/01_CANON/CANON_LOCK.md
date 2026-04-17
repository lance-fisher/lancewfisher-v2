# Bloodlines — Canon Lock Reference
**Created:** 2026-03-31 (Phase 5 ecosystem audit)
**Purpose:** Quick-reference for which major design decisions are SETTLED (locked) vs OPEN (undecided). This complements CANONICAL_RULES.md, which has the full granular list. This file focuses on the highest-level decisions that shape the entire game.

---

## How to Use This File

- **LOCKED** means the decision is final. Do not contradict it in any session without explicit revisitation and authorization from Lance.
- **OPEN** means the decision is unresolved. Multiple ideas may coexist. Future sessions may freely propose alternatives.
- **PROPOSED → REVIEW** means the concept has been fully designed in system files but not formally elevated to SETTLED in CANONICAL_RULES.md. These should be reviewed for settlement.

---

## Core Identity

| Decision | Status | Summary |
|----------|--------|---------|
| Game title | LOCKED | Bloodlines |
| Genre | LOCKED | Massively large-scale medieval RTS, Warcraft 3-aligned (lower unit count, micro emphasis) |
| Scope directive | LOCKED | No scope ceiling. Depth is the goal. Every session makes the design richer, not smaller. |
| Tone | LOCKED | Serious, adult, R-rated design sensibility. Sanitized rituals explicitly rejected. |
| Art style | LOCKED | Realistic proportions, natural anatomy, non-cartoon |
| Three pillars | LOCKED | Dynasty (lineage/political), Faith (worldview/cultural), Conviction (moral character) |

## Factions

| Decision | Status | Summary |
|----------|--------|---------|
| 9 founding houses | LOCKED | Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest |
| Hair colors (all 9) | LOCKED | Silver, Dark gold, Black, Bright gold, Brown, Vivid red, Orange, Neutral brown, Black/gray/pepper |
| Asymmetry model | LOCKED | Hybrid: shared tech tree + subtle stat variations + unique units per house |
| Ironmark blood profile | LOCKED | CB004 canonicalized 2026-03-19 |
| Remaining 8 house profiles | VOIDED | CB004 strategic designs for all 8 non-Ironmark houses voided 2026-04-07 per Lance Fisher direction. Settled elements only: hair color and color theme. Strategic design pending future work. |
| Hartvale unique unit | LOCKED | Verdant Warden. Off 4/Def 5. Hybrid guardian. Settlement defense and population loyalty bonuses. CB002 design selected over CB004 Hearthmasters. Set 2026-04-07. |
| House starting relationships | LOCKED | 7 canonical pairs: Goldgrave vs Whitehall (economic rivalry), Ironmark vs Stonehelm (military respect/wariness), Highborne vs Trueborn (prestige competition), Hartvale + Stonehelm (natural partnership), Oldcrest vs Westland (cultural friction), Trueborn vs Blood Dominion (ideological tension), Whitehall vs All (universal diplomatic access). Tendencies, not locks — player can change them. Set 2026-04-07. |

## Systems (7 Core)

| Decision | Status | Summary |
|----------|--------|---------|
| Conviction system | LOCKED | Behavioral morality spectrum. Axis: Moral/Neutral/Cruel. Independent from Faith. CP weight system with pattern amplification. |
| Faith system | LOCKED | 4 covenants, each with Light/Dark Doctrine Paths. 5-tier intensity (Latent→Apex). Faith selected end of Level 1. |
| Population system | LOCKED | Unified pool, realm-wide, growth by food+water. 90-second cycle. Loyalty mechanic. |
| Resource system | LOCKED | 6 resources: gold, food, water, wood, stone, iron. Iron added 2026-03-18. |
| Territory system | LOCKED | Military control + population loyalty. Three-layer map (terrain/province/boundary). |
| Dynastic system | LOCKED | Bloodline active cap 20. Succession by player choice. Deep family management. Mysticism path, sorcerer role. |
| Born of Sacrifice | LOCKED | Population-constrained army lifecycle. Veterans recycled. Elite status through institutional depth. Not a one-time ritual. |

## Faith Specifics

| Decision | Status | Summary |
|----------|--------|---------|
| 4 covenants | LOCKED | Old Light, Blood Dominion, The Order, The Wild |
| Doctrine Path terminology | LOCKED | "Doctrine Path" canonical. Light Doctrine / Dark Doctrine. |
| Faith building names (all 4) | LOCKED | Unique per covenant (Wayshrine, Blood-Altar, Lawpost, Grove Marker, etc.) |
| Faith units L3-L5 | LOCKED | 2 per faith at L3, 2 at L4, 1 apex at L5. All named. |
| Wild faith animal cavalry | LOCKED | Bears and other animals as mounts |

## Victory

| Decision | Status | Summary |
|----------|--------|---------|
| 5 victory paths | LOCKED | Military Conquest, Economic Dominance (Currency), Faith Divine Right, Territorial Governance, Alliance Victory. Dynastic Prestige eliminated as standalone path — reframed as modifier/bonus system (2026-04-07). |
| Victory difficulty | LOCKED | All paths VERY difficult. Solo victory VERY VERY VERY difficult. Game trends toward stalemate/coalition. |
| Sovereignty seat | LOCKED | All paths lead to one bloodline (or coalition head) recognized as supreme authority |
| Divine Right declaration | LOCKED | Declared, not passive. Fully transparent milestones. Coalition can challenge during spread window. |
| Currency dominance | PROPOSED | Custom dynasty currency devalues gold, adopted by Trueborn City and other dynasties for trade/economy, creates resource dominance approaching unstoppable. Counter: Resolution Battle + alliance. Full mechanics to be refined. |
| Territorial governance threshold | PROPOSED | Approximately 90% population loyalty. Fine-tune later in data layer. |

## World

| Decision | Status | Summary |
|----------|--------|---------|
| 4 historical eras | LOCKED | Age Before, The Great Frost (~170yr), Age of Survival, Age of Reclamation (match starts Year 80) |
| 10 terrain types | LOCKED | Reclaimed Plains, Ancient Forest, Stone Highlands, Iron Ridges, River Valleys, Coastal Zones, Frost Ruins, Badlands, Sacred Ground, Tundra |
| Trueborn City | LOCKED | Pre-Frost neutral trade hub. Activates if not conquered. Conquering is strategic, not victory. |
| Map scale | LOCKED | ~10x typical RTS. Three-layer structure. No upper ceiling. |
| 5 diplomatic states | LOCKED | War, Hostile, Neutral, Cordial, Allied |
| Historical timeline details | OPEN | Era structure settled, specific events and dates open |

## Military

| Decision | Status | Summary |
|----------|--------|---------|
| 5-tier unit progression | LOCKED | L1 faith-neutral → L2 iron/cavalry → L3 faith-influenced → L4 faith-specific → L5 apex |
| L1 units | LOCKED | Militia, Swordsmen, Spearmen, Hunters, Bowmen |
| Cavalry | LOCKED | Major military component requiring iron + animal husbandry |
| 6 naval vessel types | LOCKED | Fishing, Scout, War Galley, Transport, Fire Ship, Capital Ship |
| Unit promotion pipeline | LOCKED | 5-tier: combat unit → commander → lesser house (added 2026-03-26) |

## Dynastic

| Decision | Status | Summary |
|----------|--------|---------|
| Marriage control | LOCKED | Children cannot freely declare loyalty. Head of household controls. (Corrected 2026-03-26) |
| Polygamy | LOCKED | Faith-specific: Blood Dominion + The Wild only |
| Mixed bloodline defection | LOCKED | Slider mechanic for loyalty in cross-dynasty marriages |
| Mysticism training path | LOCKED | Added 2026-03-26 |
| Sorcerer role | LOCKED | Added 2026-03-26 |
| Bloodline population/family scope | OPEN | Managing hundreds of members across generations — system not yet designed |

## Match Structure

| Decision | Status | Summary |
|----------|--------|---------|
| Five match stages | LOCKED | Founding, Expansion and Identity, Encounter/Establishment/Neutral City, War and Turning of Tides, Final Convergence. Set 2026-04-07. Replaces four-stage system. |
| Three match phases | LOCKED | Phase 1: Emergence, Phase 2: Commitment, Phase 3: Resolution. Overlays the five stages. Set 2026-04-07. |
| Multi-speed time model | LOCKED | Three simultaneous time speeds: Battlefield Time (real-time), Campaign Time (90-second cycle), Dynastic Time (generational). Set 2026-04-07. |
| Command and zoom layers | LOCKED | Three layers: Local Battlefield, Regional Command, Sovereign/World Command. Set 2026-04-07. |
| Continental world architecture | LOCKED | Multiple continents separated by ocean. Home Continent (all nine houses + Trueborn City) + at least two secondary continents. Secondary continent territory counts toward all victory thresholds. Set 2026-04-07. |
| Naval doctrine as full playstyle | LOCKED | Maritime path is a complete strategic orientation, not a supplement. All six victory paths accessible from maritime orientation. Set 2026-04-07. |

## Mode Variants

| Decision | Status | Summary |
|----------|--------|---------|
| Phase Entry system | LOCKED | Players can enter the match at any of the five phases. Entering at Phase 3 starts with Phase 3 conditions at an accelerated build-up rate. Same five-phase architecture throughout — different entry point only. Allows shorter, more immediately active sessions. Set 2026-04-07. |
| Alliance trigger conditions | LOCKED | Alliances available but conditional: (1) Cannot form between opposite faith extremes. (2) If one dynasty is steamrolling, other players must have the option to counter-ally. Set 2026-04-07. |
| Wild faith animal mounts | PROPOSED | At sufficient Wild faith intensity, other animals beyond bears available as cavalry mounts with higher bonuses than horses. Specific animals and thresholds to be designed. |

## Superseded Decisions

| Decision | Status | Date |
|----------|--------|------|
| Four match stages | SUPERSEDED | 2026-04-07. Founding, Consolidation, Ideological Expansion, Irreversible Divergence replaced by five-stage system per Lance Fisher direction. |
| Frost Elder mechanic | ABANDONED | 2026-03-19. Starting leaders no longer carry mandatory death mechanics. |
| Creative Branch 002 faith doctrines | SUPERSEDED | 2026-03-19. Full rewrite required under adult tone standard. |
| Dynastic Prestige as victory path | ABANDONED | 2026-04-07. Reframed as a modifier/bonus system. High prestige accelerates pursuit of the five remaining victory paths but is not itself a win condition. |
| CB004 house profiles (8 houses) | VOIDED | 2026-04-07. All strategic design content for 8 non-Ironmark houses removed from active canon. Ironmark remains fully locked. |

---

## Dual-Clock Architecture (Addendum A — 2026-04-07)

| Decision | Status | Summary |
|----------|--------|---------|
| Declared time model | LOCKED | Battles fought in real-time (20-40 real min). On battle conclusion, game declares elapsed in-world time (3-6 months for skirmishes, 1-2 years for major battles, 3-5 years for sieges). No universal real-to-in-world ratio. Two different clocks: battle clock during battles, dynasty clock during strategic layer. Declaration is the seam. |
| Battle-strategic cycle | LOCKED | Battle → Declaration → Events Queue → Commitment Phase → Battle. This is the rhythm of a Bloodlines session. |
| No dynasty interrupts during battle | LOCKED | All dynasty/operational events queue during live battle. Surface at declaration screen post-battle. Exception: existential in-battle events only (Commander killed, supply line cut, bloodline member wounded). |
| Strategic layer | LOCKED | Post-declaration events queue + commitment phase. Not a menu — it is the game. |
| Shared multiplayer calendar | LOCKED | Single in-world timeline for all players. Calendar advances on aggregate match activity, not individual player's battle frequency. |
| Commander system | LOCKED | No hard cap on armies. Every army always commandable by player. Limitation is human attention. Two commander types: Bloodline Member Commander (full personal history, development, faith, conviction — amplifies above raw unit values) and AI Field Commander (competent, generic, does not develop). |
| Directive system | LOCKED | 14 directives across 5 categories: Territorial, Combat, Operational, Diplomatic, Siege. Persists across battle cycles. Adjusted during commitment phase. |
| Attitude modifiers | LOCKED | 7 modifiers: Aggressive, Measured, Cautious, Economical, Relentless, Disciplined, Opportunistic. Stack with Commander's personal profile. |
| Command capacity | LOCKED | Quality threshold — Commander leads their capacity at full effectiveness, excess at degraded effectiveness. Grows with development. No hard cap prevents the player from overassigning. |
| Direct control toggle | LOCKED | Player can switch any army to direct control at any time, including mid-engagement. Commander passive bonuses (morale aura, faith presence) continue during direct player control. |
| Declaration table reference ranges | LOCKED | Canonical ranges locked 2026-04-15: skirmish 3-6 months, major battle 1-2 years, sustained siege 3-5 years. Later stages produce larger declarations. Design target: 2-3 visible generations per full match. Source: Canonical Prompt Insert v1.0. |

## Multiplayer Engagement Doctrine (Session 82 Ingestion, 2026-04-15)

| Decision | Status | Summary |
|----------|--------|---------|
| No individual multiplayer pause | LOCKED | No single player may pause the shared match or alter global speed. Permitted forms: unanimous, majority vote, host/admin in private lobby, emergency tokens (limited, non-chainable). Source: Canonical Prompt Insert v1.0. |
| Imminent engagement warning | LOCKED | Hostile engagement becoming inevitable triggers an Immediate alert with a short live countdown (10-30 seconds by type). Pre-battle command panel surfaces forces, commanders, posture options. World does not pause. Countdown is not a planning chamber. Source: Canonical Prompt Insert v1.0. |
| Warning modifier system | LOCKED | Watchtowers, patrols, scouting, fortifications, loyalty, vigilant commanders increase warning time. Ambush terrain, fog, deception, low scouting decrease it. Source: Canonical Prompt Insert v1.0. |
| No battle restart on late-join | LOCKED | Players entering an active battle inherit current battlefield state. No restart from beginning. Source: Canonical Prompt Insert v1.0. |
| Stage advance by readiness | LOCKED | Stages advance on readiness conditions, never timers. Canonical conditions per stage specified in raw ingestion. Source: Canonical Prompt Insert v1.0. |
| Faith commitment in Stage Two | LOCKED | Faith covenant commitment belongs to Stage Two. Should not be rushed during Stage One. Supersedes older "end of Stage 1" placement. Source: Canonical Prompt Insert v1.0. |
| Internal processing heartbeat | LOCKED | ~90 seconds of game time per cycle. Confirmed alignment with existing `REALM_CYCLE_DEFAULT_SECONDS = 90`. Source: Canonical Prompt Insert v1.0. |
| Live strategic layer | LOCKED | World remains active when zoomed out. Armies move as icons. Date display prominent. Strategic layer is a core play layer, not a pause menu. Source: Canonical Prompt Insert v1.0. |

## Recommendations for Next Canon Session

1. **DESIGN: 8 non-Ironmark house profiles** — hair color and color theme are the only settled elements; full house strategic design pending
2. **DESIGN: Bloodline population/family scope** — high-priority open gap for managing hundreds of members across generations
3. **DESIGN: Personality Trait System** — what traits do Bloodline Members have; how do they express in events
4. **DESIGN: Wild faith animal mounts** — specific animals, thresholds, bonuses beyond bears
5. ~~**SPECIFY: Declaration table**~~ RESOLVED 2026-04-15: canonical reference ranges locked (skirmish 3-6 months, major battle 1-2 years, siege 3-5 years). Exact per-engagement tuning remains data-layer work.
6. **SETTLE: Currency Dominance mechanics** — custom currency model defined, full mechanics need design
7. **SETTLE: Spiritual Manifestations** — fully designed in FAITH_SYSTEM, should move from PROPOSED to SETTLED
8. **SPECIFY: Victory thresholds** — lock specific numbers or explicitly categorize as data-layer tuning

---

## Defensive Fortification — 2026-04-14

Source: Lance W. Fisher direction, session of 2026-04-14. Full doctrine: `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`. Defender-side: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker-side: `04_SYSTEMS/SIEGE_SYSTEM.md`.

| Decision | Status | Summary |
|----------|--------|---------|
| Defensive fortification as strategic pillar | LOCKED | Canonical major strategic path. Fortification is not cosmetic or merely delaying. Deep investment produces real security, real deterrence, and real military difficulty for the attacker. |
| Layered fortification architecture | LOCKED | Outer works, inner ring, final defensive core. Each breach is a separate achievement. The fall of one layer does not render the whole base meaningless. |
| Defensive ecosystem | LOCKED | Walls, gates, towers, emplacements, garrisons, chokepoints, kill zones, signal systems, reserve mustering, inner fallback — all interconnected, not isolated health-bar objects. |
| Defensive leverage | LOCKED | Investment produces attritional cost multiplier, tempo drag, siege requirement escalation, operational risk imposition, accelerated defender recovery. |
| Bloodline keep as apex structure | LOCKED | Primary dynastic keeps are canonically the hardest targets in the game when fully developed. Bloodline presence confers defensive bonuses. |
| Wave-spam denial | LOCKED | Repeated reckless assault is a canonically losing branch against developed fortifications. Failed assaults cost the attacker real units, morale, cohesion, supply, tempo, and political standing. |
| Siege as earned operation | LOCKED | Siege requires plural commitments: engines, engineers, logistics, scouting, breach planning, elites, faith powers, sabotage, multi-front timing, isolation. |
| Late-game defensive relevance | LOCKED | Apex fortifications match apex offense. Offensive scaling does not invalidate fortress-path strategy in the late game. |
| Fortification tradeoffs | LOCKED | Real opportunity cost (smaller army, slower expansion, resource bias, commander commitment) in exchange for real strategic reward. Both offensive and fortress paths are canonically viable. |
| Settlement class hierarchy | LOCKED | Border settlement, military fort, trade town, regional stronghold, primary dynastic keep, fortress-citadel. Each has a distinct defensive ceiling. |
| Felt difference | LOCKED | Fortified and unfortified realms must be distinguishable visually, mechanically, numerically, and narratively. |
| AI siege rejection logic | LOCKED | Attacking AI must recognize that elite fortifications require siege preparation. Throwing line infantry at a fortress-citadel must be an explicit losing branch, reinforced by simulation penalties. |
| Fortification specialist populations | LOCKED | Garrison, engineers, signal keepers, wall wardens, tower artillerists — distinct from field army populations with their own pipelines. |
| Faith integration in fortifications | LOCKED | Covenant-specific defensive expressions: Old Light pyre wards, Blood Dominion blood-altar reserves, The Order edict wards, The Wild root wards. |
| Siege conviction consequences | LOCKED | Siege conduct feeds the four-bucket conviction ledger. Honorable siege rewards oathkeeping; wave-spam failure and sabotage-only victories add desecration; breach massacres add ruthlessness and desecration. |
| Fortification doctrine non-negotiability | LOCKED | No session may reduce, dilute, or contradict the doctrine's ten pillars without explicit authorization from Lance. |

---

## Master Design Doctrine - 2026-04-14

Source: `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`

| Decision | Status | Summary |
|----------|--------|---------|
| Bloodlines identity | LOCKED | Grand dynastic civilizational war game. The player governs a dynasty, not just a base. |
| C&C relationship | LOCKED | Clarity and replayability inspiration, not a scope ceiling. |
| Visual discipline | LOCKED | Graphics serve readability, house identity, atmosphere, and strategic communication. |
| Population centrality | LOCKED | People are army, labor, infrastructure, faith body, recovery base, and continuity. |
| Water centrality | LOCKED | Water is a defining strategic pillar: visible, contestable, defendable, and attackable. |
| Food and forestry stewardship | LOCKED | Food growth, land transformation, and managed forestry are canonical civilizational systems. |
| Supply disruption and desertion | LOCKED | Strategic strangulation is viable. Severe sustainment failure must eventually produce desertion. |
| Commander significance | LOCKED | Commanders and bloodline military leaders materially affect delegated and direct warfare. |
| Timing terminology resolution | LOCKED | Doctrine phrasing about "determined time" maps to the already-settled declared-time dual-clock model. |
| Continental and naval architecture | LOCKED | Multi-continent world structure and real naval strategy remain canonical. |
| Houses as civilizations | LOCKED | Houses are true dynastic civilizations, not cosmetic faction skins. |
| Faith and conviction distinction | LOCKED | Faith is covenantal; conviction is behavioral morality. They remain separate systems. |
| UI legibility requirement | LOCKED | Deep systems must be communicated clearly, not simplified away. |
| Anti-reduction posture | LOCKED | Future sessions may stage implementation but may not shrink the intended scale of Bloodlines by default. |
