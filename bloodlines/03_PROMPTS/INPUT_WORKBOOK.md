# Bloodlines: Input Workbook

**Purpose:** A structured workbook designed for filling in the design gaps identified during the consolidation review. Each section targets a specific area of the game where answers are needed to advance the design. Questions are project-specific and intended to meaningfully move the design forward, not fill space.

**How to Use This Workbook:**
1. Read the section introduction to understand why the area matters
2. Answer as many questions as you can in the answer spaces provided
3. Partial answers, rough thoughts, and directional instincts are all valuable
4. Conflicting ideas within a single answer are fine, note both
5. When finished, copy your answers to `docs/INPUT_TO_APPLY.md` or leave them here
6. A future Claude Code session will integrate your answers into the appropriate project files

**How Answers Get Applied:**
See the Update Workflow section at the end of this document for the complete process.

**Last Updated:** 2026-03-15

---

## Section 1: Core Identity Clarification

**Why this matters:** The project identity shapes every downstream design decision. These questions help sharpen the edges of what Bloodlines is and is not.

### Q1.1: Competitive vs. Narrative Focus

The game supports both multiplayer competition and long-form narrative play. Where does the balance sit?

- (a) Primarily competitive RTS with narrative flavor
- (b) Primarily narrative strategy with competitive viability
- (c) Two distinct modes with different balance targets
- (d) Other

**Your answer:**
> _[Write your answer here]_

### Q1.2: Accessibility vs. Depth

Classic RTS games often force a choice between accessibility for new players and depth for experienced ones. Where should Bloodlines sit?

- How steep should the learning curve be?
- Should the dynastic and faith systems be immediately visible or gradually revealed?
- Is the target audience experienced RTS players, strategy game enthusiasts broadly, or both?

**Your answer:**
> _[Write your answer here]_

### Q1.3: Visual Benchmark

The design references Command and Conquer Generals as a visual benchmark. Should Bloodlines aim for:

- (a) Isometric 2D (classic RTS)
- (b) 3D with fixed camera angle (C&C Generals style)
- (c) 3D with rotatable camera
- (d) Top-down strategic map view
- (e) Other

**Your answer:**
> _[Write your answer here]_

### Q1.4: Mod and Custom Map Support

Should the game be designed with modding and custom map creation in mind from the start, or is this a post-launch consideration?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `docs/USER_GUIDE.md` Section 1-3 and `01_CANON/BLOODLINES_DESIGN_BIBLE.md` Section 1.

---

## Section 2: Match Flow Clarification

**Why this matters:** The first 10 minutes of a match determine the player's first impression and set the pace for everything that follows. Without a concrete step-by-step picture, nothing can be prototyped.

### Q2.1: Starting Conditions

When a match begins, what does the player have?

- How many workers/villagers?
- What starting buildings (Settlement Hall only, or more)?
- What starting resources (how much gold, food, water, wood, stone)?
- Any starting military units?
- Any starting bloodline members beyond the leader?

**Your answer:**
> _[Write your answer here]_

### Q2.2: First 60 Seconds

What should the player do in the first minute of a match? Walk through the actions:

- Place buildings? Assign workers? Scout? All three?
- Is there an immediate threat or is the opening peaceful?
- How does the player learn the immediate surroundings?

**Your answer:**
> _[Write your answer here]_

### Q2.3: First Contact

When does the player first encounter another faction, tribe, or world element?

- Is it a guaranteed event at a set time, or random based on exploration?
- What happens at first contact? (diplomacy prompt, automatic hostility, trade opportunity, neutral encounter)
- How do encounters with minor tribes differ from encounters with AI kingdoms?

**Your answer:**
> _[Write your answer here]_

### Q2.4: Faith Selection Moment

The design says faith is selected at the end of Stage 1. What does this moment look like?

- Does the player encounter all four covenants and choose, or only the ones near their territory?
- Is it a menu selection, a narrative event, or an in-world interaction (visiting a sacred site)?
- Can the player delay faith selection?
- What happens if two players try to select the same faith?

**Your answer:**
> _[Write your answer here]_

### Q2.5: Stage Transition Triggers

What causes the transition from Stage 1 to Stage 2, and from each subsequent stage?

- Time-based?
- Achievement-based (reach X population, build Y structure)?
- Player-initiated (choose to advance)?
- Different for each stage?

**Your answer:**
> _[Write your answer here]_

### Q2.6: Early Game Pacing Feel

What should the early game feel like?

- Urgent and tense (immediate threats, limited time)?
- Exploratory and deliberate (room to build and discover)?
- Variable (depends on map generation and nearby factions)?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section create new content in `11_MATCHFLOW/MATCH_STRUCTURE.md` and update `docs/USER_GUIDE.md` Sections 5-6.

---

## Section 3: House Design Clarification

**Why this matters:** The nine houses have names and themes but no mechanical identity. Without knowing how Ironmark plays differently from Goldgrave, house selection is cosmetic.

### Q3.1: Degree of Asymmetry

How different should houses feel from each other?

- (a) Purely cosmetic (same mechanics, different art/lore)
- (b) Minor stat bonuses (small starting advantages)
- (c) Unique passive abilities (each house has one special trait)
- (d) Unique units or buildings (houses unlock different content)
- (e) Fully asymmetric (houses play fundamentally differently, like Starcraft races)
- (f) Other

**Your answer:**
> _[Write your answer here]_

### Q3.2: Starting Differences

Do houses start with different resources, buildings, or positions?

- Does Goldgrave start with more gold?
- Does Ironmark start with a military advantage?
- Does Stonehelm start with better fortifications?
- Or do all houses start identically?

**Your answer:**
> _[Write your answer here]_

### Q3.3: House-Faith Affinity

Do certain houses have natural affinities for certain faiths?

- Example: Does Whitehall naturally align with The Order?
- If so, is this a starting bonus, a lore suggestion, or a mechanical constraint?
- Can any house choose any faith?

**Your answer:**
> _[Write your answer here]_

### Q3.4: What Should Make a House Feel Different Before Unique Units Appear?

In the first few minutes, before any unique content is unlocked, how does a player know they are playing Ironmark vs Hartvale? What feels different?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `06_FACTIONS/FOUNDING_HOUSES.md` and create new content in `01_CANON/CANONICAL_RULES.md` under Faction and House Canon.

---

## Section 4: Faith Design Clarification

**Why this matters:** The four faiths are thematically rich but mechanically underspecified. The difference between a faith being a flavor label and a faith being a strategic axis depends on what it actually does to gameplay.

### Q4.1: Exclusive Content per Faith

Does each faith unlock exclusive buildings, units, abilities, or technologies?

- If yes, how many exclusive elements per faith?
- Are exclusives powerful enough to be build-defining or are they supplementary?

**Your answer:**
> _[Write your answer here]_

### Q4.2: Faith Change

Can a dynasty change its faith mid-match?

- If yes, at what cost?
- If no, is this an intentional design constraint (commitment matters)?

**Your answer:**
> _[Write your answer here]_

### Q4.3: Multiple Faiths in One Territory

Can different populations within a dynasty's territory practice different faiths?

- If conquered population follows a different faith, what happens?
- Does forced conversion exist? At what cost?
- Can religious tolerance be a governance strategy?

**Your answer:**
> _[Write your answer here]_

### Q4.4: Faith and Diplomacy

How does shared faith affect diplomatic relations?

- Is same-faith easier to ally with?
- Does faith alignment difference within the same covenant create schism risk?
- Can faith difference be a casus belli (justification for war)?

**Your answer:**
> _[Write your answer here]_

### Q4.5: Does Faith Function More Like...

Which metaphor best describes the role of faith in gameplay? (Pick one or describe your own.)

- (a) Doctrine (a set of rules that constrain and empower)
- (b) Covenant (a pact with a higher power that grants abilities at a price)
- (c) Law (a governance framework imposed on the population)
- (d) Spiritual power (a magic system with religious flavor)
- (e) State identity (a civilization-defining ideology, like communism vs capitalism)
- (f) All of the above, layered
- (g) Other

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `04_SYSTEMS/FAITH_SYSTEM.md` and `07_FAITHS/FOUR_ANCIENT_FAITHS.md`.

---

## Section 5: Conviction Design Clarification

**Why this matters:** Conviction is one of the most original design concepts in Bloodlines, but it is the least mechanically specified of the core systems. Without measurable triggers and visible effects, conviction cannot be implemented.

### Q5.1: Conviction Scope

Is conviction tracked at which level?

- (a) Per-dynasty (one conviction value for the entire civilization)
- (b) Per-territory (different regions develop different moral character)
- (c) Per-army (armies that commit atrocities carry that mark)
- (d) Per-bloodline-member (individual moral records)
- (e) Multiple levels simultaneously

**Your answer:**
> _[Write your answer here]_

### Q5.2: Conviction Visibility

Should the player see their conviction?

- (a) Visible as a numeric score or bar
- (b) Visible through effects and indicators but not as a direct number
- (c) Partially hidden, with general direction visible but exact state inferred
- (d) Fully hidden, experienced only through consequences

**Your answer:**
> _[Write your answer here]_

### Q5.3: Conviction Axes

Is conviction a single light-to-dark spectrum, or multiple independent axes?

- Example single axis: Merciful <-------> Ruthless
- Example multiple axes: Militaristic, Mercantile, Devout, Expansionist (each tracked separately)
- What conviction directions have you imagined?

**Your answer:**
> _[Write your answer here]_

### Q5.4: Specific Action Examples

For the following actions, what should happen to conviction?

| Action | Conviction Effect? |
|--------|--------------------|
| Protecting civilian populations during war | |
| Burning a captured city to the ground | |
| Ransoming a captured bloodline member | |
| Executing a captured bloodline member | |
| Enslaving a conquered population | |
| Building a temple in conquered territory | |
| Sacrificing an army through Born of Sacrifice | |
| Trading food to a starving rival | |

**Your answer:**
> _[Fill in the table above]_

### Q5.5: What Should Make a Late-Game Divergence Feel Irreversible and Dramatic?

At Level 4, conviction and faith combine to create an irreversible divergence. What should this feel like? Not just statistically different, but dramatically different. What changes in the game's tone, visuals, music, or mechanics at this point?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `04_SYSTEMS/CONVICTION_SYSTEM.md` and create entries in `01_CANON/CANONICAL_RULES.md`.

---

## Section 6: Population and Economy Clarification

**Why this matters:** Population is described as the central living resource, but conversion rates, growth formulas, and the specific economic loop are undefined.

### Q6.1: Population Conversion

When a civilian is converted to a soldier, what exactly happens?

- Does the population number decrease by 1?
- Can soldiers return to civilian status?
- Is there a training time, or is conversion instant?
- What is the maximum percentage of population that can be military before the economy collapses?

**Your answer:**
> _[Write your answer here]_

### Q6.2: Population Growth Rate

How fast should population grow?

- Should a comfortable economy double population in 5 minutes? 15 minutes? 30 minutes?
- What is the target population size for a healthy mid-game civilization?
- Is there a natural cap beyond housing (food, water, morale)?

**Your answer:**
> _[Write your answer here]_

### Q6.3: Resource Gathering Workers

Do workers gather resources like in Age of Empires (assigned to specific tasks) or like in Command and Conquer (automated from buildings)?

- If worker-assigned: How many workers per resource node?
- If building-automated: Does population size affect production rate?

**Your answer:**
> _[Write your answer here]_

### Q6.4: Trade Between Dynasties

Does inter-dynasty trade exist?

- If yes: What can be traded? Resources? Population? Technology? Bloodline members?
- How is trade initiated? (diplomatic interface, trade building, market mechanic)
- Can trade be blockaded?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `04_SYSTEMS/POPULATION_SYSTEM.md` and `04_SYSTEMS/RESOURCE_SYSTEM.md`.

---

## Section 7: Bloodline Clarification

**Why this matters:** The bloodline system is the emotional and strategic core of the game. Several key mechanics are undefined.

### Q7.1: Succession

When the dynasty leader dies, what happens?

- Does the eldest heir automatically succeed?
- Can the player choose an heir?
- Can succession be contested by other bloodline members?
- What happens if there is no heir? (Game over? Regency? Election?)

**Your answer:**
> _[Write your answer here]_

### Q7.2: Dormancy Mechanics

When bloodline members exceed the active cap of 20, what determines which become dormant?

- Player choice?
- Automatic based on age, skill, or role?
- How does the player experience the transition to dormancy?
- Can dormant members be reactivated? Under what conditions?

**Your answer:**
> _[Write your answer here]_

### Q7.3: Character Traits

Do bloodline members have inheritable traits?

- If yes: Are traits genetic (inherited from parents), trained (from training path), or both?
- What kinds of traits exist? (Military prowess, diplomatic skill, faith devotion, economic talent, negative traits like cruelty or cowardice)
- Can traits be improved through experience?

**Your answer:**
> _[Write your answer here]_

### Q7.4: Bloodline Member Agency

Do bloodline members have their own opinions, goals, or loyalty levels?

- Can a bloodline member rebel?
- Can a bloodline member refuse an order?
- Do members have relationships with each other (rivalries, alliances)?

**Your answer:**
> _[Write your answer here]_

### Q7.5: Lifespan Model

How do bloodline members age and die?

- Fixed lifespan in game ticks/minutes?
- Age-based decay with increasing death probability?
- Event-driven death only (battle, assassination, sacrifice)?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers from this section update `04_SYSTEMS/DYNASTIC_SYSTEM.md`.

---

## Section 8: Diplomacy and Control Clarification

**Why this matters:** Dynastic interactions are richly imagined but the diplomatic framework is undefined. Without formal diplomatic states and treaty types, AI kingdoms cannot be programmed to behave diplomatically.

### Q8.1: Diplomatic States

What formal diplomatic states exist between dynasties?

- War, Peace, Alliance, Non-Aggression Pact, Vassal, Trade Partner, Tributary?
- Others?

**Your answer:**
> _[Write your answer here]_

### Q8.2: Lesser House Control

The design says lesser houses can be AI-autonomous or player-controlled. How does the player interact with their lesser houses?

- Direct command (give orders, they obey)?
- Request-based (suggest actions, they may comply)?
- Autonomous with influence (they act independently but the player can shift priorities)?
- Adjustable control level?

**Your answer:**
> _[Write your answer here]_

### Q8.3: What Should Loyalty Allow That Territory Ownership Alone Does Not?

This is a key design question. If you own a territory militarily but the population is disloyal, what specifically cannot you do that a loyal territory allows?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers create new content in `08_MECHANICS/MECHANICS_INDEX.md` and `04_SYSTEMS/TERRITORY_SYSTEM.md`.

---

## Section 9: Unit, Structure, and Military Doctrine Clarification

**Why this matters:** Level 1 units are defined but nothing exists beyond them. The military system needs to span 5 levels and support multiple army compositions.

### Q9.1: Level 2 Units

What new unit types should appear at Level 2?

- Cavalry? (Referenced in spearmen description as a counter target)
- Siege equipment?
- Upgraded versions of Level 1 units?
- Faith-specific units?
- What role does each new unit fill that Level 1 does not?

**Your answer:**
> _[Write your answer here]_

### Q9.2: Siege Warfare

How should siege warfare work?

- Do armies automatically attack walls, or do they need siege equipment?
- What siege equipment exists? (Rams, catapults, trebuchets, siege towers)
- How do defenders interact with siege (archers on walls, oil, sorties)?
- How long should a typical siege last?

**Your answer:**
> _[Write your answer here]_

### Q9.3: Cavalry

Given that spearmen are described as "strong against cavalry," cavalry must exist. Define cavalry:

- When does cavalry become available (which level)?
- What roles does cavalry serve? (Shock, flanking, pursuit, raiding, scouting)
- Is cavalry drawn from population like infantry?
- Does cavalry require special resources or buildings?

**Your answer:**
> _[Write your answer here]_

### Q9.4: Naval Units

Naval warfare exists in the design. What naval units should exist?

- Warships, transport ships, trade ships, fishing vessels?
- How do naval and land forces interact?
- Can naval units bombard coastal positions?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers update `10_UNITS/UNIT_INDEX.md` and create new content for Levels 2-5.

---

## Section 10: World and Lore Clarification

**Why this matters:** The world history and geography files are empty scaffolds. The world's past shapes its present, and geography shapes gameplay.

### Q10.1: Map Structure

What map type best fits Bloodlines?

- (a) Hex grid (clear territory boundaries, turn-based feel)
- (b) Province/region map (like Crusader Kings or Europa Universalis)
- (c) Continuous terrain (like Command and Conquer, no grid)
- (d) Hybrid (continuous terrain with province overlay)
- (e) Other

**Your answer:**
> _[Write your answer here]_

### Q10.2: World History

Do you have a vision for the world's history? Key questions:

- How old is this world?
- Were the four faiths always separate, or did they diverge from one?
- What happened to the Trueborn dynasty that they became a neutral city rather than a ruling power?
- Are there ancient ruins, and if so, what civilization built them?

**Your answer:**
> _[Write your answer here]_

### Q10.3: Geography

What types of terrain and biomes should the map contain?

- Plains, forests, mountains, deserts, tundra, swamps, coasts, islands?
- Do terrain types affect which resources are available?
- Do terrain types affect military combat?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers populate `05_LORE/WORLD_HISTORY.md`, `05_LORE/TIMELINE.md`, and `09_WORLD/WORLD_INDEX.md`.

---

## Section 11: Victory and Failure Condition Clarification

**Why this matters:** Five victory paths are identified but only military conquest has any mechanical definition. The other four need concrete win conditions.

### Q11.1: Victory Thresholds

For each victory path, what is the specific win condition?

| Victory Path | What Triggers Victory? |
|--------------|----------------------|
| Military Conquest | |
| Currency Dominance | |
| Faith Divine Right | |
| Territorial Governance | |
| Dynastic Prestige | |

**Your answer:**
> _[Fill in the table above]_

### Q11.2: Defeat Conditions

When is a player eliminated?

- When their last settlement is destroyed?
- When their bloodline has no living members?
- When their population reaches zero?
- Can a defeated dynasty be absorbed rather than eliminated?

**Your answer:**
> _[Write your answer here]_

### Q11.3: Victory Visibility

How do players know someone is approaching victory?

- Public announcements?
- Visible victory progress bars?
- Hidden until threshold is crossed?
- Should nearing victory trigger the Trueborn coalition?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers update `11_MATCHFLOW/MATCH_STRUCTURE.md` and `01_CANON/CANONICAL_RULES.md` Victory Condition Canon.

---

## Section 12: UI/UX and Player Readability Clarification

**Why this matters:** Complex systems require clear presentation. The UI must make seven interconnected systems readable without overwhelming the player.

### Q12.1: Information Priority

When the player looks at the main game screen, what information is always visible?

- Resources?
- Population count?
- Faith intensity?
- Conviction indicator?
- Minimap?
- Family tree shortcut?
- Current stage/level?

**Your answer:**
> _[Write your answer here]_

### Q12.2: Family Tree Interaction

The family tree is described as a "central gameplay element." How does the player interact with it?

- Fullscreen overlay? Side panel? Separate screen?
- Can the player issue commands from the family tree (assign roles, arrange marriages)?
- How does the player learn about dormant members?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers populate `12_UI_UX/UI_NOTES.md`.

---

## Section 13: Technical Implementation Priority Clarification

**Why this matters:** A deterministic RTS architecture prompt already exists. These questions help prioritize what gets built first.

### Q13.1: First System to Prototype

Which system should be prototyped first?

- (a) Resource gathering and building construction
- (b) Population growth and housing
- (c) Military recruitment and Level 1 combat
- (d) Territory control and loyalty
- (e) The economic loop (resources to buildings to population to more resources)

**Your answer:**
> _[Write your answer here]_

### Q13.2: Target Platform

What platform(s) is the game being built for?

- PC only?
- PC + Console?
- Browser-based prototype first?
- Mobile ever?

**Your answer:**
> _[Write your answer here]_

### Q13.3: Engine/Technology

Is there a preferred engine or technology stack?

- Unity?
- Unreal?
- Custom engine?
- Web-based (the bootstrap prompt suggests TypeScript)?
- Godot?

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers inform `15_PROTOTYPE/` and `01_CANON/BLOODLINES_MASTER_MEMORY.md` Section 48.

---

## Section 14: Prototype Priority Clarification

**Why this matters:** Knowing what to build first prevents wasted effort. The first playable slice should demonstrate the core experience with minimal scope.

### Q14.1: Minimum Viable Slice

What is the smallest playable experience that would feel like Bloodlines?

- Just resource gathering and building?
- Resource gathering + military + one enemy?
- Resource gathering + population + territory + one AI opponent?

**Your answer:**
> _[Write your answer here]_

### Q14.2: What Must the Prototype Prove?

What question should the first prototype answer?

- "Does the economic loop feel right?"
- "Does territory loyalty add meaningful gameplay?"
- "Does the population-as-resource model create interesting tradeoffs?"
- "Does combat feel like a C&C-style RTS?"

**Your answer:**
> _[Write your answer here]_

**Application note:** Answers create a new prototype spec document in `15_PROTOTYPE/`.

---

## Update Workflow

When you have completed some or all of the answers in this workbook, follow this process:

### Step 1: Record Your Answers
Leave answers in this file, or copy them to `docs/INPUT_TO_APPLY.md` with the section number and question number for each.

### Step 2: Review Session
In a future Claude Code session, provide this instruction:
> "Review `docs/INPUT_WORKBOOK.md` (or `docs/INPUT_TO_APPLY.md`) for new answers. Apply them to the appropriate project files following the application notes in each section."

### Step 3: Application Process
The AI session will:
1. Read the new answers
2. Identify which project files need updating (using the Application Note in each section)
3. Append new content to the appropriate files with date stamps
4. Update `01_CANON/CANONICAL_RULES.md` if any answer settles an open question
5. Preserve prior content in all files (append, never replace)
6. Update `docs/CHANGE_LOG.md` with what was applied

### Step 4: Preservation Rules
- If an answer contradicts prior design content, both the answer and the prior content are preserved
- If an answer refines prior content, the refinement is added alongside the original
- If an answer settles an OPEN DESIGN QUESTION, the question is moved to SETTLED in `01_CANON/CANONICAL_RULES.md`
- Historical answers are never deleted, even if superseded by later answers

---

*New sections and questions are added to this workbook as the design progresses. Completed sections are marked as applied but their content remains.*
