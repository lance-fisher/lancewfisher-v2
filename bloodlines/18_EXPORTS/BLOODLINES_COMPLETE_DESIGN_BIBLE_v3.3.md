# BLOODLINES — COMPLETE DESIGN BIBLE
**Version 3.2 — Full Integration — 2026-04-07**
**Authoritative reference document. Incorporates all content through Session 11 (2026-04-07).**

---

> **Document Authority:** This bible supersedes all prior design bible versions. It incorporates settled canon through the Ninth Ingestion (2026-03-19), creative branch content from CB001 through CB003, the Operations System design from Session 10 (2026-03-21), and the Session 11/12 additions from 2026-04-07. Where CB003 supersedes CB002, CB003 is used. The Frost Elder system is excluded in full — it was abandoned as of 2026-03-19. Faith doctrine content uses the CB003 adult-tone rewrite exclusively. CB004 strategic profiles for the 8 non-Ironmark houses are voided (2026-04-07) — only hair color, color theme, and Ironmark's full profile remain settled. Victory conditions reflect the five-path system (Dynastic Prestige eliminated as standalone path 2026-04-07). The Operations System (Sections 63-68) introduces covert, faith, and military operations with the complete timing system.
>
> **v3.2 Changes (2026-04-07 — first pass):** Section 45 revised — the four-stage match system is replaced by the five-stage system per Lance Fisher direction. Section 62 filled with the Multi-Speed Time Model (NEW-A). Section 6 Ironmark Swordsmen stat corrected to 5/5 per canonical lock. New Part XII added: Scale, Time, Command, and Geography (Sections 70-74). Source Governance Framework inserted into front matter. Open Design Reservoir appended as Part XIII.
>
> **v3.2 Changes (2026-04-07 — second pass):** Dynastic Prestige eliminated as a victory path; Section 51 rewritten as Dynastic Prestige Modifier System; victory path count updated from six to five throughout. CB004 house profiles for 8 non-Ironmark houses voided; Sections 53-54 and 56-61 stripped to settled-elements-only stubs. Hartvale Verdant Warden locked (Section 59). Phase Entry mode variant added. Alliance trigger conditions added. Currency Dominance updated to custom-currency-devalues-gold model. All Dynastic Prestige victory path references updated.

---

### SOURCE GOVERNANCE AND HISTORY PRESERVATION

**This section governs how Bloodlines source material must be managed across all working sessions, regardless of which AI model or tool is being used.**

#### The Three-Layer Source Model

Bloodlines source material exists across three simultaneous layers. These layers coexist at all times. One does not replace another. A newer document does not retroactively deprecate the contents of an older one unless Lance has explicitly ordered the removal of specific content.

**Layer A — Active Canon Snapshot:** The current best organized state of the project. This is the working reference for active design sessions. It is not the total picture. It is not permission to erase what preceded it.

**Layer B — Historical Design Archive:** Everything discussed across the full history of Bloodlines work: earlier bible versions, experimental branches, alternative formulations, mechanics raised once and never fully integrated but never explicitly killed, lore fragments, discarded systems that remain potentially valuable, unresolved design tensions. Unless Lance has explicitly ordered removal of something, it belongs in this layer and must remain recoverable.

**Layer C — Open Design Reservoir:** A living pool of ideas that have been discussed, floated, or partially developed without being definitively canonized or definitively removed. These are concepts in orbit, available for revival, not dead.

#### Governing Rules for All Future Sessions

Never condense prior systems into fewer categories for neatness. If the project has seven types of a thing, document all seven. Never reduce victory routes into a smaller number for manageability. The breadth of viable paths is a feature. Never omit previously raised mechanics because they complicate the architecture. Never assume an older concept is deprecated because a later document does not include it. Never wipe alternatives. If two viable approaches were discussed, preserve both. Preserve tensions and unresolved choices explicitly. Do not resolve them silently. If material from prior sessions is missing from the current context, flag the gap explicitly. Do not silently omit and do not fill with invention. Only Lance determines what is canon, deprecated, or removed.

---

# PART I — FOUNDATION

---

## Section 1: What Bloodlines Is

### Game Identity

Bloodlines is a massively large-scale medieval real-time strategy game. It is not Age of Empires. It is not Total War. Its lineage is Command and Conquer — base building, resource accumulation, strategic freedom, direct unit control — but stretched across a canvas a full order of magnitude larger and draped with the machinery of dynasties, faiths, and moral consequence.

The mechanical core is Warcraft 3-aligned: a lower unit count than mass-scale RTS, meaningful hero-equivalent anchors in the form of Bloodline Members, a premium placed on micro-management and unit positioning over numbers alone. A skilled Bloodlines player is managing armies, yes — but they are also managing a family. They are governing a faith. They are building an identity for a civilization across multiple generations.

The game is built for long sessions. Two to ten hours is the intended match window. The design target for balance is a six to ten hour match. The map is roughly ten times the scale of a typical RTS map, with no ceiling imposed on map generation scope. A full match supports up to ten players with AI kingdoms and minor tribes filling out the political world.

### Design Philosophy

Every system the player touches must feel like it has been thought through five levels deeper than they can see. Not procedural randomness — designed depth. A player who spends a hundred hours with Bloodlines should still be discovering interactions they did not know existed. The game does not owe the player a clean experience. It owes the player a real one.

The design philosophy prioritizes clarity over accessibility, consequence over convenience, and long-term strategic depth over immediate gratification. Bloodlines is a game that rewards people who think about second and third-order effects of their decisions, who understand that the army they train today is the population they will not have tomorrow, who recognize that the faith they choose at the end of Level 1 is not a bonus selection but an identity commitment that will shape everything their civilization builds.

There is no scope ceiling on this project. Every system that can go deeper must go deeper. The risk is building too little, not too much.

### Tone

Bloodlines is not a game for children. The world it depicts is not cleaned up or sanitized. Four ancient faiths emerged from the wreckage of a civilization that nearly ended, and all four carry in their doctrine the memory of that near-ending and the full weight of what it cost. Blood is shed in their names. Populations suffer under their authorities. Players who commit to dark doctrine paths will do things that are genuinely disturbing, institutionally rationalized, and available as legitimate strategic choices. This is not edginess. This is what a serious game about power, faith, and consequence looks like when it takes its own subject matter seriously.

The dynasty that the player builds lives across multiple generations. The choices of the grandfather shape the options available to the granddaughter. Bad succession decisions trigger revolts that kill people who were loyal. Marriages are contracts and breaking them costs something real. The game does not need to punish the player. The game is designed so that the world responds.

---

## Section 2: Design Pillars — Dynasty, Faith, Conviction, and Territory

Every civilization in Bloodlines is defined by four pillars. These are not categories of mechanics. They are the four fundamental questions that every dynasty in the game is being forced to answer by the conditions of the world they inherited.

### The Three Primary Pillars

**Dynasty** is the answer to: who are you, and who comes after you?

The bloodline is the emotional core of Bloodlines. Not the armies, not the territory, not the faith mechanics. The family is the picture. Every other system registers its consequences through the family: a famine culls your children, a bad succession triggers a revolt, a political marriage opens a border, a member lost in battle leaves a functional hole you feel for the rest of the match. The dynasty is the player's stake in the outcome — not abstract territory on a map, but named people with histories, relationships, and futures that the player has invested in.

**Faith** is the answer to: why does power exist, and what is it for?

The four ancient covenants are complete theologies that emerged from the wreckage of the world and have endured because they answer the questions the survivors needed answered. Faith is not a bonus system. It is a commitment to a specific account of how the world works and what the civilization that follows it must do. Selecting a faith at the end of Level 1 is not choosing a faction bonus. It is committing to a worldview that will shape what units your civilization can build, what rituals it can perform, what populations will trust it, and what powers it can access at the cost of things it cannot take back.

**Conviction** is the answer to: what kind of ruler are you?

Conviction is the behavioral record of the dynasty's leadership over the course of the match. Every significant action the player takes generates Conviction Points — positive (Moral) or negative (Cruel). The pattern amplifies over time: the same action costs more conviction weight the second time, and more again the third. Conviction shapes population loyalty, diplomatic reception, army morale, succession stability, and faith access. It is not a morality meter that tells the player who the good guys are. A dynasty at maximum Cruel conviction can win. A dynasty at maximum Moral conviction can win. The difference is not which path is available but which systems are costly and which are efficient along each axis.

Faith and Conviction are independent systems. They interact but are never forced into alignment. A dynasty can follow dark faith doctrine with high Moral conviction. A dynasty can follow light faith doctrine while accumulating Cruel conviction. The combinations produce different civilizations with different strengths and vulnerabilities. This independence is settled canon and must never be compromised in design.

### Territory

Territory is the fourth pillar — the physical substrate that everything else operates on. Territory is not merely map space. It is population, resource production, loyalty, stability, and the physical anchor of a dynasty's claim to legitimacy. Controlling territory requires both military presence and population loyalty. A region held by garrison force alone without population acceptance is a liability that will eventually revolt. True control is earned, not taken. This is the fundamental design statement of the territory system, and it applies across every victory path.

---

## Section 3: World History — The Four Eras

The world of Bloodlines carries the memory of what happened to it. That memory shapes every institution, every faith, every political structure, and every decision the player makes. Understanding the world's history is not background reading. It is strategic context.

### The Age Before

The Age Before is the civilization that preceded the catastrophe. It was not a golden age — the sources are fragmentary and the founding houses do not romanticize it — but it was a functioning world with political structures, economic networks, cultural traditions, and populations large enough to sustain them. The Trueborn ruled this world, or at least anchored it: a bloodline that held a position of recognized authority over the other lineages, not through constant warfare but through the accumulated legitimacy of being the center everyone navigated around.

What ended the Age Before was not a single event. It was the Great Frost, and the Frost did not arrive suddenly. The world had been declining — bonds between people cheapening, obligations going unpaid, the collective accounting of civilization running a deficit that nobody was formally acknowledging. The Frost completed what had already begun internally.

### The Great Frost

The Great Frost lasted approximately 170 years. It was not merely weather. The cold was severe enough to kill agriculture across vast regions, collapse trade networks, sever the political relationships that depended on those networks, and reduce populations to scattered survival communities. Entire cultures disappeared. Knowledge was lost. The Trueborn's authority did not survive the Frost as a political institution — but the Trueborn themselves survived, as they always have, by holding the one neutral point that everyone needed: the city that became the post-Frost Trueborn City.

Iron ore was exposed during the glacial retreat of the Great Frost's peak. The iron deposits that would fuel the military and economic development of the Age of Reclamation were a direct consequence of the geological disruption the Frost caused. The curse that nearly ended civilization left behind the tool that rebuilt it.

Each of the four ancient faiths emerged from the Frost with a specific theology of what it meant and what it demanded of survivors. These theologies differ at the level of first principles. They are not positions on the same spectrum. They are genuinely different answers to the same catastrophe.

### The Age of Survival

Following the Frost's worst period, the scattered survivor communities began the process of consolidation. The nine founding houses emerged during this era — not because they were the most powerful communities, but because they were the ones that built lasting political structures: walls, inherited governance, resource extraction networks, and enough consolidated military power to claim and hold territory through the years of chaos.

The founding houses do not all agree on when the Age of Survival ended and the Age of Reclamation began. The official match calendar uses Year 0 of the Reclamation as an agreed-upon anchor date, but the transition was gradual and contested.

Minor tribes — the communities that survived without building founding-house-level political structures — emerged from this era with their own identities. Some descend from pre-Frost civilizations. Some coalesced during the Frost from necessity. Some are the remnant populations of what could have been a tenth founding house but was not. They are not afterthoughts. They are the majority of the surviving world's population, organized outside the nine-house framework.

### The Age of Reclamation

The match begins in Year 80 of the Reclamation. Eighty years of rebuilding, consolidation, and political development have produced the nine founding houses as recognizable powers with distinct identities, military capabilities, and strategic positions. The world is not stable — it has never been stable since the Frost — but it has achieved a kind of contested equilibrium that has lasted long enough for the founding houses to develop meaningfully different characters.

Year 80 is chosen deliberately. It is early enough in the Reclamation that iron infrastructure is not yet mature — the early match is still partially a survival-era economy — but late enough that political structures, faith institutions, and dynastic identities are established rather than nascent. The founding houses know who they are. They are not yet certain who will inherit the world.

---

## Section 4: The Sovereignty Seat — What Every Bloodline Is Fighting For

Every victory path in Bloodlines is a route to the same destination: the Sovereignty Seat.

The Sovereignty Seat is the recognized position of supreme authority over all lands — one bloodline, or one coalition with an agreed supreme ruler, recognized by the population of the world as the legitimate governing power. It is not a throne in a specific location. It is a status — the condition of being the power that all others have accepted as primary, whether through conquest, economic dominion, divine mandate, administrative excellence, dynastic legitimacy, or coalition agreement.

This is the Iron Throne equivalent of the game's world (not named that in-game). The founding houses call it different things. Every system in the game is ultimately in service of this destination.

The design core of the Sovereignty Seat is that reaching it is genuinely difficult. Not hard in the sense of requiring mechanical skill alone, but hard in the way that absolute, sustained, recognized authority over other human beings is actually difficult in a world that actively resists being dominated. Most matches will not produce a clean solo winner. The design expectation is that the majority of matches trend toward Alliance Victory, a Trueborn-pressure-forced resolution, or a prolonged stalemate where multiple contenders exhaust each other. A bloodline that actually claims the Sovereignty Seat — through any of the five paths — has done something that the world's history will record as exceptional.

---

# PART II — CIVILIZATION

---

## Section 5: The Nine Founding Houses — Overview

The nine founding houses are the primary political and military powers of the Age of Reclamation. Each is a distinct civilization with its own strategic identity, cultural character, and inherited advantages. All nine are descended from common pre-Frost ancestry — the founding lineages share ancient blood, which is why cross-dynasty marriages are politically significant rather than foreign-relations curiosities.

**The canonical nine:** Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest.

All nine houses share a complete tech tree and unit roster. No house is locked out of any building category or unit progression. The differences between houses are intentionally subtle at match start: per-unit base stat variations on shared units (Off/Def ratings using 5/5 as the neutral baseline) and one unique unit accessible only to that house. No house is dominant at match start.

Visual identity is a core design commitment. Each house is defined by a specific hair color carried by all its bloodline members — a biological marker of common ancestry differentiated over generations. Houses are also defined by color palette, symbol family, architectural language, and material tone.

**Hair color assignments (settled canon):**
- Trueborn: Silver
- Highborne: Darker gold with highlights
- Ironmark: Black
- Goldgrave: Bright gold
- Stonehelm: Brown
- Westland: Vivid red (Westgrave lineage)
- Hartvale: Orange (Hartborne lineage)
- Whitehall: Neutral brown
- Oldcrest: Black/gray/pepper

Lesser houses — subsidiary political structures created through title grants and land allotments — may be AI-controlled or player-controlled. Lesser house names must include the parent bloodline's root as a visible prefix or suffix.

---

## Section 6: House Strategic Profiles — All Nine

### House Trueborn

**Hair color:** Silver

Strategic design pending.

---

### House Highborne

**Hair color:** Darker gold with highlights

Strategic design pending.

---

### House Ironmark

**Hair color:** Black
**Strategic archetype:** Industrial military

Ironmark smells like charcoal and heated metal. The forge culture does not separate the labor of making things from the capability to break them. Their armies look slightly smaller than they should for the resources spent on them, and then they hit an enemy formation and the numbers stop mattering.

**Level 1 unit stat deviations:**
- Militia: 6/5 (forge culture — physical labor and fighting are not far apart)
- Swordsmen: 5/5 (baseline — asymmetry is carried by the Axeman, not the Swordsman; per canonical lock)
- Spearmen: 5/5
- Hunters: 5/5
- Bowmen: 5/5

**Unique unit: The Axeman**
Heavy two-handed axe fighter drawn from Ironmark's forge labor class. Slower than Swordsmen but hits harder on each strike. Excels at breaking armored formations and destroying structures. Almost impossible to route quickly. Poor against fast cavalry and in open-field pursuit. Inside chokepoints, against walls, and against heavy infantry: exceptional.

**Opening tendency:** Production infrastructure before military numbers. Iron access prioritized early.

**Strongest victory paths:** Military Conquest (production advantage overwhelming in the iron era), Economic Dominance (forge infrastructure generates trade goods when not devoted to weapons).

**Conviction natural tendency:** Neutral leaning Cruel.

---

### House Goldgrave

**Hair color:** Bright gold

Strategic design pending.

---

### House Stonehelm

**Hair color:** Brown

Strategic design pending.

---

### House Westland

**Hair color:** Vivid red (Westgrave lineage)

Strategic design pending.

---

### House Hartvale

**Hair color:** Orange (Hartborne lineage)

**Unique unit: The Verdant Warden** — LOCKED
Off 4 / Def 5. Hybrid guardian. Can fight but is primarily a defender. Provides settlement defense bonuses and population loyalty bonuses in its zone. Represents Hartvale as protectors of their people rather than conquerors.

Full strategic design pending.

---

### House Whitehall

**Hair color:** Neutral brown

Strategic design pending.

---

### House Oldcrest

**Hair color:** Black/gray/pepper

Strategic design pending.

---

## Section 7: The Trueborn City — History, Role, and The Rise Arc

### History

The Trueborn City predates the Great Frost. It is the oldest continuously inhabited political structure in the game world. When the Frost came and every other political institution collapsed or contracted into survival mode, the Trueborn City survived by maintaining absolute neutrality — all factions knew that attacking it destroyed the only universal trade point. This precedent calcified into institutional identity over 170 years of Frost survival. The city is not neutral because its rulers say so. It is neutral because the alternative has been clearly demonstrated to result in universal collapse, and everyone who survived the Frost knows that memory.

The Trueborn bloodline is historically descended from the lineage that held authority in the Age Before. The connection between the founding house Trueborn and the Trueborn City is real but complicated — the city is an institution that transcends any one bloodline, but the historical resonance is significant.

### Role in the Match

The Trueborn City is the primary conduit for global information. Trade routes passing through the city carry news from across the world — population sentiment, faith distribution, political events, and faction movements. Dynasties with active trade relationships with the city receive accurate, timely world-state information. Dynasties diplomatically isolated from the city receive degraded or delayed information.

The city also functions as the world's diplomatic center of gravity. Killing or capturing a dynasty representative at Trueborn City neutral ground triggers an automatic and immediate Trueborn Summons against the perpetrator — a mobilization of all dynasties with any relationship to the city into a temporary coalition against the perpetrator.

Conquering the Trueborn City is not a victory condition. It is a strategic objective that accelerates pursuit of whichever victory condition the conquering bloodline is pursuing. Holding the city provides different bonuses to each path: Military Conquest gains stability bonuses in occupied territories; Economic Dominance gains historical trade network legitimacy; Faith Divine Right resolves a key Synod challenge; Territorial Governance gains diplomatic neutralization of the city's loyalty drain; Alliance Victory gains the city's endorsement of coalition arrangements. In all cases, holding the city increases prestige, which accelerates pursuit of each path through the prestige modifier system.

### The Rise Arc

If no bloodline has conquered or meaningfully challenged the Trueborn City by a defined threshold point in the match, the Trueborn City activates. The Trueborn — who held authority over all lands before the Great Frost — raise their banners and begin actively asserting themselves as the legitimate authority. They are no longer a neutral institution. They are a major NPC faction pursuing their own reconquest of the land.

The active Trueborn faction has access to the city's full accumulated resources, its historical legitimacy generating passive loyalty pressure in territories across the map, and the coalition infrastructure that kept the city alive for 250 years. Populations that were passively compliant under a founding house's rule begin questioning whether the Trueborn's claim is more legitimate. Legitimacy doubt degrades territory stability across the map with effects proportional to proximity to the city.

The forced pressure is clear: the city cannot be ignored indefinitely. A bloodline that conquers the city demonstrates the capacity to rule all others. A bloodline that lets the city rise must contend with a powerful, historically legitimate rival pursuing the Sovereignty Seat through mechanics no founding house has available.

---

## Section 8: Minor Tribes — System Overview and Ten Archetypes

### What Minor Tribes Are

Minor tribes are the forgotten survivors. The nine founding houses represent the communities that built lasting political structures through the Great Frost. Minor tribes are everyone else who survived. Some descend from pre-Frost civilizations. Some coalesced during the Frost from shared suffering. Some are the remnant populations of what could have been a tenth founding house but was not. A small number are nomadic by choice.

A typical match contains 8 to 14 minor tribes, distributed across terrain zones that match their archetype. No two tribes of the same archetype occupy the same match. They occupy buffer zones between founding house territories, deep wilderness sections, strategic resource nodes, and high-defense terrain.

Minor tribes are not static. A tribe left alone grows modestly. A tribe actively engaged through trade, tribute, or faith alignment develops faster and becomes a genuine military and economic asset. A tribe repeatedly raided or subjected to faith persecution can radicalize — contacting neighboring houses with offers of military cooperation, or appearing in the late game as part of a coalition force. Tribes remember how houses treat them.

### The Ten Tribal Archetypes

**The Remnant Faith:** Descendants of pre-Frost religious communities preserving practices that predate the four covenants. Strong faith alignment and resistance to conversion away from it. Valuable for faith-path players who share their covenant.

**The Frost Covenant:** Tribes that coalesced during the Great Frost itself, formed by shared suffering. Their founding myth is the Frost. Resilient under pressure, relatively indifferent to luxury incentives. Cannot be bought easily; must be earned.

**The Near-House:** Remnant populations of dynasties that nearly became founding houses but did not. Carry resentment toward specific founding houses that displaced their ancestors. Have names, histories, and in some cases written records of the leadership lines that died in the Frost.

**The Nomadic Warband:** Tribes that survived the Frost by moving. Cannot be threatened with siege or economically pressured through territory control. They move the moment a relationship becomes unfavorable. Interact with founding houses most unpredictably. Offer exceptional military capability if relationship is maintained.

**The River Stewards:** Communities organized around water resource management. Control the river valley crossings. Economically focused but militarily competent at defending water infrastructure. Trade-responsive and often the first tribes to accept formal agreements.

**The Highland Keepers:** Mountain communities with extraordinary defensive capability in their home terrain. Nearly impossible to dislodge militarily once terrain advantage is engaged. Their territories contain stone and iron deposits.

**The Forest Clans:** Deep wilderness communities with exceptional ambush, stealth, and terrain navigation capability. Often aligned with The Wild covenant. Their territories contain ancient forests with faith significance for Wild-following dynasties.

**The Iron Workers:** Communities that developed iron-working skills during the Frost. Produce iron more efficiently than standard mines and can be integrated as a permanent economic asset.

**The Sacred Ground Guardians:** Tribes positioned on or near Sacred Ground terrain. They do not control the Sacred Ground as a political claim but are its traditional stewards. Attacking them on or near Sacred Ground carries faith consequences regardless of the attacking dynasty's covenant.

**The Trade Network:** Cosmopolitan communities at the intersection of trade routes, with connections to multiple other tribes and factions. Modest military but significant intelligence and diplomatic value. A dynasty that allies with the Trade Network gains access to an information network that rivals Trueborn City connections.

---

## Section 9: AI Kingdoms — Eight Personality Archetypes and Five Named Opponents

### AI Design Philosophy

AI kingdoms in Bloodlines serve three simultaneous roles: genuine challenge across all victory paths, variety that makes every match feel distinct, and narrative role as dynasties with legible agendas. AI does not cheat — no fog-of-war immunity, no resource bonuses, no information the human player cannot access. Difficulty scales through build order precision, economic efficiency, threat recognition speed, and adaptation rate. The personality is fixed regardless of difficulty setting.

AI opponents are legible. A player who has invested time in the game should be able to look at an AI's early build order and correctly infer its personality archetype. The player who watches what the AI is doing should gain a genuine advantage.

### The Eight Personality Archetypes

**Archetype 1: The Patience of Stone**
Victory priority: Territorial Governance, with Economic Dominance as fallback. Methodical, conservative economy. Defensive military posture until ready to strike with overwhelming force. Never attacks first. Cordial with everyone during Founding. Watches, waits, picks its moment — always claiming territory the moment two other players go to war. Gets progressively harder to dislodge the longer it is left alone. Conviction: Neutral, trending Moral. Counter-strategy: attack early before fortifications mature.

**Archetype 2: The Burning Covenant**
Victory priority: Faith Divine Right, exclusively. Does not pursue fallback paths. Faith infrastructure before military. Will die trying before pursuing an alternative. Conviction determined entirely by covenant — Old Light Burning Covenant is rigidly Moral, Blood Dominion Burning Covenant is firmly Cruel. Counter-strategy: disrupt Covenant Tests during the recovery windows between them.

**Archetype 3: The Iron Merchant**
Victory priority: Economic Dominance, with Alliance Victory as fallback. Most economically sophisticated AI. Offers resource trades to every player including those technically hostile — always structured to benefit Iron Merchant more than the recipient. Will switch covenants if economic alignment shifts. Conviction: Neutral, maintained with discipline. Counter-strategy: economic isolation — cut off trade routes and deny resource-rich territories.

**Archetype 4: The Iron Tide**
Victory priority: Military Conquest. Most aggressive AI. Fields the largest army it can sustain and uses it continuously. Never fully disbands its army. Comfortable losing military engagements — rebuilds and attacks again. Conviction: Cruel, with no apology. Counter-strategy: economic denial combined with defensive attrition.

**Archetype 5: The Spider's Web**
Victory priority: Alliance Victory, with Economic Dominance as fallback. Treats every other player as a piece to be positioned and eventually used. Maintains alliances or Cordial relationships with every player it can. Manufactures conflicts between other players by selectively sharing information. Builds prestige through political maneuver to attract coalition partners. At high difficulty: genuinely frightening political opponent. Counter-strategy: diplomatic isolation — identify early, warn other players, build a counter-coalition that excludes it.

**Archetype 6: The Walled Cathedral**
Victory priority: Faith Divine Right exclusively. Compact, self-sufficient economy within deliberately limited territorial footprint. Develops existing territory to extraordinary depth. Does not want your land — wants to be left alone to build something magnificent and will defend that with absolute ferocity. Conviction: aligned with its chosen covenant's deepest doctrine.

**Archetype 7: The Reluctant Conqueror**
Victory priority: Military Conquest with a guilty conscience. Fights when it must, governs what it takes more carefully than any other military-path AI. Builds occupation infrastructure immediately after conquest. Honors prisoners, rebuilds conquered civilian infrastructure, avoids civilian harm when militarily possible. Conviction: Moral, maintained under genuine pressure. Counter-strategy: economic pressure — a Moral military AI is expensive to run.

**Archetype 8: The Dynastic Calculator**
Victory priority: Alliance Victory, with Military Conquest as secondary. Every decision evaluated through a Bloodline Member lifecycle model. Operates with a longer planning horizon than any other AI. Accumulates prestige relentlessly as a strategic modifier — uses it to attract coalition partners and accelerate Alliance Victory. Conviction: Neutral, mathematically maintained. Counter-strategy: force it into Defining Moment gambles before it has prepared adequately.

### Five Named Opponent Dynasties

**Valdrik of House Ironmark ("The Grinding Wall")**
Ironmark. Blood Dominion, dark doctrine. Cruel conviction. Iron Tide archetype with Walled Cathedral secondary. Fights two wars simultaneously — one with armies and one with attrition inflicted on enemies' populations through sustained pressure. Does not believe in decisive battles. Believes in a world where enemies are always slightly poorer, slightly hungrier, and slightly more afraid than last week. Has never lost a siege he initiated. Has never held conquered territory for less than two match-phases.

**Sera of House Highborne ("The Anointed Voice")**
Highborne. Old Light, light doctrine. Moral conviction. Burning Covenant archetype with Patience of Stone secondary. The most dangerous faith player in the named roster not because she is aggressive but because she is patient. Has completed the Divine Right Declaration in a higher percentage of matches than any other named opponent. Has never started a war. Has finished several.

**Goras of House Westland ("The Red Debt")**
Westland. The Wild, dark doctrine. Cruel conviction. Iron Tide, full expression. His Outriders are on the map before opposing players have finished their second structure. Conviction record: catastrophic. Military record: exceptional. Has won through Military Conquest more times than any other named opponent. Has triggered more Great Reckoning coalitions than any other named opponent. Fast, brutal, ultimately controllable if you can survive the first hour.

**Yssel of House Goldgrave ("The Patient Hand")**
Goldgrave. Old Light, light doctrine. Neutral conviction. Iron Merchant, advanced expression. Has never personally led a military campaign. Her armies have fought on her behalf, funded by loans extended to allies. By the time opponents understand her economic position, she typically holds financial leverage over three or four kingdoms. Nobody can agree on exactly when Yssel won any given match, but everyone agrees she won.

**Caerith of House Oldcrest ("The Weight of Years")**
Oldcrest. The Order, light doctrine. Moral conviction. Dynastic Calculator with Patience of Stone secondary. His Bloodline Members have depth of development that other houses' equivalents simply have not had time to accumulate. Accumulates prestige faster than any other named opponent — uses it to attract coalition partners rather than as an end in itself. The most reliable Alliance Victory achiever in the named roster. Has never been eliminated in the early match.

---

# PART III — THE BLOODLINE

---

## Section 10: The Living Bloodline — Family Structure and Generational Depth

The bloodline is not an abstract roster of units. It is a family. A real family, with a patriarch or matriarch at its center, a spouse, children who grow up during the match, political marriages arranged for strategic gain, grandchildren born to members who were themselves children when the match began. Over a 2-10 hour session, the player watches a dynasty live across multiple generations.

This is the emotional core of Bloodlines. The family is the picture. Every system in the game ultimately registers its consequences through the family: a famine culls your children, a bad succession triggers a revolt, a political marriage opens a border, a member lost in battle leaves a functional hole you feel for the rest of the match.

### How a Dynasty Begins

At match start, the player controls a bloodline that already exists. The head of bloodline is established: a man or woman in their late thirties to mid-forties, past their youth, not yet in decline. Their best years as a direct combatant or administrator may be behind them. Their best years as a strategist, political operator, and parent are now.

Three starting configurations are available:

**Configuration 1 — The Established Dynasty**
The head has a spouse. Two or three children exist, ranging from young adolescent to early adult. One child may already be committed to a role. The family is established but not fully formed. The most common starting point.

**Configuration 2 — The New Union**
The head is recently married. No children yet, or one infant. The dynasty is fresh — the family will grow entirely during the match. Rewards long-game thinking and patient building.

**Configuration 3 — The Widowed Patriarch/Matriarch**
The head lost their spouse. Children exist from that marriage. The head may remarry during the match, producing a second generation with a different bloodline inheritance. Creates natural half-sibling political dynamics within the family. The most complex starting configuration.

The player names the head of bloodline. Family names are fixed by house. Given names are player-customizable. Children inherit the family name.

### Generational Overlap

A long match produces generational overlap. The founding head of bloodline may still be alive — aged, reduced in some capacities, elevated in others — when grandchildren are born and begin to develop.

An aged head of bloodline carries the accumulated weight of decades of match history. Their conviction shaped by every choice made under their leadership. Their faith intensity may have deepened into Fervent or Apex territory. Their presence radiates gravity that younger members do not yet have. But they are also slowing. The dynasty will have to transition to a new generation. The player has been watching that next generation grow.

The grandfather who watches his grandson take his first command role is a moment the game is designed to produce. It should feel like something.

---

## Section 11: Marriage, Children, and Offspring

### Marriage as a Strategic Transaction

Marriage in Bloodlines is a strategic transaction with personal consequences. It is never purely one or the other.

**Who can marry whom:**
Any bloodline member of appropriate age can be offered in marriage to a member of another player's bloodline (direct diplomatic negotiation), a member of a lesser house or vassal family (internal alliance-building), a member of an AI kingdom's ruling family (foreign diplomatic play), or a commoner of exceptional attribute (rare, lower political value, possible stigma from higher houses).

**Arranging a marriage:**
The player initiates marriage offers through the diplomacy interface or through a Diplomatic Envoy committed member. Marriage offers include proposed terms — what the offering family provides and what they expect in return. The other party negotiates. A marriage is a contract. Breaking it has conviction consequences.

**Cross-bloodline marriages and children:**
A child born of two different bloodlines has attribute inheritance drawing from both parents. Their faith inheritance can pull toward either parent's faith alignment — a source of family tension and meaningful design space. Their political identity is contested.

Children of cross-dynasty marriages who survive to adulthood and accumulate sufficient political standing can found their own dynasty as a separate player faction if the parent player chooses to split the bloodline, or if an event forces the issue. This is rare, consequential, and represents a late-game branching outcome.

**Political marriages within lesser houses:**
A bloodline member married into a lesser house elevates that house's loyalty and binds it more tightly to the bloodline. The married member functions as a governor within that house, blending the Defense commitment role with a familial anchor. If that member dies or is recalled, the lesser house's loyalty may fracture.

### How Children Develop

**Birth:**
A child is born based on match time elapsed, faith bonuses to fertility, and dynasty health conditions. Harsh winters, famine, illness, and the mother's health all modulate fertility.

**Childhood stage (not yet active):**
A child born early in the match is not available as an active member for several match years. During childhood, the player assigns the child's birth path — a declaration of intent, not a guarantee. The child trains along that path. The player can see the child in the Keep, growing visibly over time.

**Adolescent stage (limited availability):**
Transitions to a recognizable person in the Keep, beginning to develop as a visible member. Can be placed in limited supporting roles — accompanying a War General on campaign to gain experience, traveling with a Trade Anchor to learn the routes. Cannot yet lead independently.

**Adult transition:**
Becomes eligible for full active commitment. If a slot exists among the 20 active members, they can be committed immediately. If the roster is full, they enter dormancy — present, tracked, aging, but not deployed.

---

## Section 12: Bloodline Member Functional Roles — All Eight

Commitment roles are distinct from birth paths. A member's birth path shapes what they are capable of and inclined toward. Their commitment role is what they are actually doing in the match.

Birth path determines the ceiling. Commitment role determines the current function. Members do not need to physically occupy buildings or spaces. Functional roles are commitments that generate effects across the dynasty's systems.

### Role 1: War General

Commands armies in the field. Provides a command anchor — every unit under direct command benefits from morale bonuses, coordination bonuses, and reduced rout risk. A large army without a War General fights as a collection of units. A large army with a strong War General fights as a single instrument.

**Active ability:** Vanguard Surge — personally leads a concentrated push against a specific enemy formation. High-risk direct action. Success breaks the targeted formation and generates army-wide morale surge. If wounded or killed during the Surge, army suffers immediate morale collapse. Long cooldown — should not be used carelessly.

**Passive presence:** Persistent command radius providing baseline command bonuses to all units within it. Strength scales with the General's experience and conviction alignment.

**Absence consequence:** Every army they commanded loses its command anchor immediately. If the dead General was also head of bloodline, succession triggers simultaneously — the dynasty has no military leader and no political certainty at the same moment. Enemies who track this will attack.

**Development:** Campaign Experience accumulates through battles won, battles survived, and specific feat events. Milestones improve command radius and Vanguard Surge effectiveness. Faith intensity deepens naturally through the witness of death and the weight of command. Conviction is shaped by how the player conducts wars.

### Role 2: Trade Anchor

Commits to a trade route or caravan operation. Transforms a basic commercial route into a high-value exchange. Without a Trade Anchor, commerce functions but access to luxury goods, high-tier resources, and cross-bloodline economic diplomacy is limited.

**Active ability:** Strategic Negotiation — brokers a specific deal that could not be executed through standard trade mechanics.

**Passive presence:** All trade routes with the Anchor committed provide a passive income multiplier and a loyalty bonus to lesser houses along the route.

**Absence consequence:** All active trade in the route pauses or degrades. Merchant partners lose confidence. Lesser houses along the route see a loyalty dip.

**Development:** Merchant Network attribute grows through successful negotiations and route expansions. Faith and conviction shape negotiation style.

### Role 3: Territory Governor

Commits to a specific claimed region as the dynasty's permanent presence. Does not roam — anchors. Suppresses local revolt risk, generates loyalty bonus, ensures the dynasty's claim has a human face attached.

**Active ability:** Governor's Proclamation — can suppress an emerging revolt before it fires, offer a one-time loyalty surge, or formally recognize a lesser house's service.

**Passive presence:** Persistent stability field across the territory. Revolt thresholds raised. Taxation efficiency improved.

**Absence consequence:** Territory enters an instability window immediately. Revolt risk rises sharply.

**Development:** Accumulated presence represented as a Roots attribute that grows slowly and cannot be transferred. If the Governor dies, the Roots go with them. A replacement Governor starts from scratch. Conviction shapes how the population develops under them.

### Role 4: Faith Leader (Old Light / Light Doctrine Path)

The dynasty's spiritual representative within a light faith tradition. Performs rites, leads observances, blesses armies, consecrates claimed lands. A living argument that the dynasty deserves what it has.

**Active ability:** Grand Rite — a significant formal ceremony producing a major faith-based effect (consecrating a territory, blessing an army before a decisive campaign, performing a rite of passage for a bloodline heir).

**Passive presence:** Raises the baseline faith intensity of the dynasty's population over time.

**Absence consequence:** Faith intensity does not drop immediately, but without renewal it erodes. Consecrations decay.

**Development:** Each Grand Rite pushes faith intensity one step higher. At Fervent faith they have become something the population looks at differently. At Apex faith they are the most spiritually potent individual in the dynasty — and they are burning. Their body shows it. They cannot perform rites back-to-back without cost.

### Role 5: Covenant Officiant (Blood Dominion / Dark Doctrine Path)

The dynasty's instrument within darker doctrinal paths. Does not perform blessings. Performs bindings. Covenants with forces that demand payment. The Officiant is the person who knows the terms and executes the act. The population knows what the Officiant does, or suspects. The dynasty that employs a Covenant Officiant is broadcasting something about itself — that it will pay prices others will not.

**Active ability:** Covenant Binding — a formal act of dark doctrine extracting a significant benefit in exchange for a real, irreversible cost. Under Blood Dominion: may sacrifice a lesser member of the bloodline (the player chooses the sacrifice from eligible dormant or peripheral members) to produce a permanent, dynasty-wide power bonus. The sacrifice is permanent. The member is gone. The power is real.

**Passive presence:** Fear Aura covering the dynasty's political footprint. Lesser houses less likely to revolt — not because they are loyal, but because they are afraid.

**Absence consequence:** Active Covenants enter an unstable state. Some Covenants, if their Officiant dies mid-process, produce active backlash from the doctrine's demands.

**Development:** Develops by completing Bindings and surviving consequences. Their appearance changes with each major Binding. The Keep knows what the Officiant does. Other family members do not look at them directly. Their children do not play near the Officiant's space.

### Role 6: Spymaster

Builds and operates the dynasty's covert intelligence network. Does not appear on battlefields. Does not govern territory publicly. Knows things — about rival bloodlines, about the loyalty of lesser houses, about the personal vulnerabilities of enemy leaders.

**Active ability:** Intelligence Operation — targeted assassination of a rival bloodline member (high-risk, catastrophic if traced back), loyalty subversion of a lesser house, intelligence extraction, or counter-intelligence purge of compromised members from the dynasty's own administrative structure.

**Passive presence:** Passive intelligence coverage across a radius scaled to their network's development. Within coverage, the player sees hidden loyalty values of lesser houses, receives early warning of rival diplomatic negotiations, and gets notification when a rival bloodline member moves into a position of unusual significance.

**Absence consequence:** The intelligence network goes dark. Not immediately — agents and contacts exist independently — but without a central coordinator the network fragments over time.

**Development:** Agent Web attribute maps coverage area and operational capacity. A senior Spymaster with a decade of match time has agents embedded in rival courts and lesser houses across the map. Nearly impossible to fully neutralize without killing them directly.

### Role 7: Diplomatic Envoy

The dynasty's formal representative in relations with other bloodlines, foreign kingdoms, and the Trueborn City. Travels. Negotiates. Carries the dynasty's reputation in their person. Critical negotiations, alliance formations, and marriage arrangements between bloodlines all require an Envoy's direct participation for optimal outcomes.

**Active ability:** Grand Embassy — a formal diplomatic initiative where the Envoy leads a significant delegation to a foreign party. Can ratify a major alliance, dissolve a long-standing conflict through formal negotiation, open a new trade corridor, or negotiate the release of a captured bloodline member.

**Passive presence:** Diplomatic Presence bonus that slowly improves the dynasty's standing with all foreign parties.

**Absence consequence:** Diplomatic relationships enter a review period. Alliances do not immediately break but their terms become renegotiable.

**Development:** Reputation attribute compounds through successful negotiations and crisis mediations. Develops personal relationships with specific foreign leaders that the player can leverage.

### Role 8: Heir Designate

The formally named successor to the head of bloodline. Not yet in power. May hold a secondary commitment role simultaneously. Their formal designation smooths the succession event — when the head dies, the interface resolves faster, revolt risk is lower, and the Heir accesses the head's full attribute bonuses more quickly.

**Active ability:** Shadow Governance — the Heir begins assuming certain administrative functions while the current head is still alive. Accelerates development and creates a visible continuity signal to the broader world.

**Passive presence:** Succession Stability bonus that reduces the dynasty's overall revolt risk during any succession event. The longer they have been formally designated, the stronger this bonus.

**Absence consequence:** If a designated Heir dies, an immediate Political Instability event fires. Lesser houses that had aligned loyalty to the dead Heir recalibrate. Foreign parties renegotiate their diplomatic assumptions.

**Development:** Develops a distinct political identity — their own conviction posture, faith alignment, relationships with lesser houses. A dynasty where the Heir has been in Shadow Governance for years has effectively pre-installed their next leader.

---

## Section 13: Succession — Player Choice, Projected Impact, Revolt Consequences

When the primary bloodline member dies, the player chooses their successor.

The succession interface shows each eligible candidate's attributes with visible projected impact on the bloodline as a whole: military standing, economic output, conviction trajectory, faith alignment, population loyalty. The choice is consequential. Some successor selections cause uprising events — lesser houses or territories that were loyal to the previous head revolt and must be reconquered or reclaimed.

The succession system is designed to make succession feel real. The world has been organized around a specific person. When that person is gone, the world reorganizes — and it does not reorganize instantly or without friction. The Heir Designate role exists precisely to smooth this transition. A formally designated Heir who has been serving in Shadow Governance for years may transition with minimal disruption. An unexpected succession to a surprise candidate may produce widespread instability.

Conviction, faith, and bloodline reputation all affect which successors are more and less disruptive. A deeply Moral dynasty with deep faith investment and broad population loyalty absorbs succession more gracefully than a Cruel dynasty whose loyalty structures were built on fear of the specific person who just died.

---

## Section 14: The Keep — Visual Representation of Dynasty State

The Keep is the player's visual home base — the UI anchor that represents the dynasty's state not through text interfaces alone but through the visual condition of a physical place inhabited by real people.

The head of bloodline is present. Their spouse or consort is visible. Children occupy the Keep in proportion to their developmental stage — an infant is seen in a cradle, a child appears in the corridors, an adolescent is training or reading, an adult is present and purposeful. Key household members are visible in their roles.

The Faith Leader has a dedicated devotional space that grows more elaborate as faith intensity deepens. At Fervent: the space is visibly consuming — the Faith Leader's bearing has changed, their presence fills the area. At Apex: they look like a person who has been partly consumed by something larger than themselves. The Spymaster is always slightly peripheral — present but never at the center, their space containing correspondence that is carefully organized with nothing identifying left visible. The Covenant Officiant occupies a distinct space that other members give a wider berth, extraordinarily calm, carrying below the surface what other people carry visibly.

The Keep's physical condition reflects the dynasty's health. A prosperous dynasty in Moral conviction with deep faith investment has a keep that looks and feels like it is inhabited by people who believe in something. A Cruel dynasty that has been at war continuously may have a keep that looks depleted. A dynasty that has lost several members in rapid succession will have visible gaps — spaces that used to be inhabited.

Character visual style across the Keep uses realistic proportions and natural anatomy. Oversized limbs and stylized exaggeration are explicitly rejected.

---

## Section 15: Bloodline Active Cap and Generational Scale

The bloodline active cap is 20 members. Beyond 20, members enter dormancy — present, tracked, aging, producing children, affected by events, capable of being elevated to active status if a slot opens. There is no cap on total family members. A bloodline can have 50 living members across three generations if conditions favor it.

The cap governs deployment and active contribution. A dormant member is not lost — they are waiting. The cap creates a perpetual management tension: every death opens a slot. Every death also means a person is gone. Opening a slot by losing an active member is never a clean gain.

Children do not consume an active slot until they reach adulthood. They grow through childhood and adolescence outside the cap. When they reach adulthood, the player must decide whether to activate them or let them remain dormant.

Generational scale — the mechanics for managing a very large number of living family members across generations — is a high-priority open design area. Environmental events (long winters, famine, disease outbreaks) function as natural thinning mechanics that operate on the broader family without requiring the player to manually manage population decay. These events are the world's pressure on the dynasty. How the dynasty survives them is a meaningful design space.

The 20-active-member cap is the discipline that forces real choices: you cannot keep everyone useful. You must make decisions about who rises, who waits in dormancy, who marries into another bloodline, who you are willing to sacrifice.

---

# PART II — WORLD

---

## Section 16: Ten Terrain Types — Properties, Resources, and Strategic Role

The world of Bloodlines is not a flat plain interrupted by decorative scenery. Terrain is a system. Every square kilometer of land a dynasty controls, contests, or crosses through has a physical character that shapes what can be built there, what units fight well there, which faith regards it as significant, and what resources it yields over time. Ten canonical terrain types cover the full range of the game world. No map is generated without all ten present in sufficient proportion.

### Reclaimed Plains

Reclaimed Plains are the match's economic foundation. These are the flatlands that agriculture depends on — glacial retreat has cleared them of ice, the soil has recovered enough to support crops, and they are wide enough to sustain the kind of settlement density that a founding house requires. Almost every dynasty begins on the edge of a Reclaimed Plains zone because this is where population can grow.

Resource profile: Highest food yield per tile of any terrain type. Modest wood from scattered stands. No stone or iron without specialized infrastructure. Gold trade routes move easily across plains.

Military properties: Open ground that favors cavalry and massed infantry formations. No natural cover. Defensive structures must be built — the terrain provides none. Archers have exceptional sight lines. Flanking attacks are easy to execute and difficult to prevent without deliberate defensive planning.

Faith affinity: Neutral across all four covenants, with slight preference for Old Light, whose theology of remembrance and cultivation frames the recovered earth as evidence of the world's survival.

Strategic value: Control of substantial Reclaimed Plains territory is the baseline requirement for any economic strategy. The dynasty that controls no plains depends entirely on tribute and trade. Plains also permit the fastest army movement across the map.

Visual description: Rolling fields of pale green and gold, dotted with farmsteads and granary structures. At match start, some plains still show the scars of the Frost — patches of gravel and grey soil that have not yet recovered. By mid-match, a well-managed plains territory should look like a civilization in process.

### Ancient Forest

Ancient Forest zones are the world's memory. These are the forests that survived the Great Frost intact, their root systems deep enough that even 170 years of extreme cold could not kill them. They are old in a way that players can feel through the visual register: the canopy closes overhead, the undergrowth is dense, and the silence under the trees is not an absence of life but a density of it that has stopped announcing itself.

Resource profile: High wood yield, continuously replenishing at a rate that scales with forest integrity. Food from foraging is available but modest compared to plains. No stone or iron without destructive clearing — which reduces wood yield permanently. The choice to mine an Ancient Forest is irreversible.

Military properties: Severely restricted cavalry and formation movement. Infantry fights at close quarters. Ambush attacks deal amplified damage. Units in Ancient Forest are significantly harder to detect without ranger-type scouts. Non-ranger cavalry is nearly useless here.

Faith affinity: Strong Wild faith significance. Ancient Forests are the domain of The Wild covenant — their sacred groves and spiritual lodges are found here, and Wild-following dynasties receive passive faith intensity bonuses from controlling Ancient Forest zones. Old Light regards them with respect. Blood Dominion and The Order regard them as resources to be managed or cleared.

Strategic value: Control of Ancient Forest provides a sustained wood economy that does not require replenishment investment. Wild faith players gain military advantage here that other faith paths cannot replicate. The ambush capacity creates asymmetric defense value far exceeding the territory's surface area.

Visual description: Towering canopy with trunks wide enough that three adults could not circle them with linked hands. Shafts of light pierce the canopy at irregular intervals. The floor is layers of old growth — roots, moss, fallen wood, new growth pushing up through everything that fell before it. The deep forest has areas of absolute shadow at midday.

### Stone Highlands

Stone Highlands are defensive territory of the highest order. These are elevated rocky zones where geology dominates everything — stone is everywhere, visibility is long from the high ground, and movement for anything that is not native to the terrain is slow and costly. The dynasty that fortifies Stone Highlands territory correctly makes it nearly impregnable.

Resource profile: Exceptional stone yield — the match's highest per tile. Modest iron deposits accessible without the full iron-era infrastructure required in lower terrain. No meaningful food or wood. Water must be managed carefully at elevation.

Military properties: Overwhelming defensive advantage for garrisons. Attacking up the highland slope imposes severe offensive penalties on every unit type. Cavalry is nearly useless in highland terrain. Infantry movement is slow but armor and formation bonuses for defensive infantry are at their highest. Stone walls built in Highland terrain cost less and provide more defensive value per resource spent than walls built anywhere else.

Faith affinity: Modest Order preference. The Order covenant's theology of structure and mandate resonates with the highlands' physical permanence. Old Light also regards highlands as places where the sky is closer and rites performed at altitude carry additional weight.

Strategic value: A dynasty controlling Stone Highlands territory holds a defensive anchor that can sustain a force much smaller than the attacking army. This is Stonehelm's natural terrain and the source of their institutional identity. Any dynasty fortifying highlands buys time against superior numbers.

Visual description: Grey and brown stone faces, exposed ridgelines, narrow passes between elevated formations. Hardy scrub vegetation in the cracks of older rock. Wind is constant at the upper levels. Structures built in highland terrain use the stone itself as their primary material — walls blend with the natural rock faces they are built against.

### Iron Ridges

Iron Ridges are the economic consequence of the Great Frost. The glacial movement that nearly ended civilization also exposed the iron deposits that rebuilt it. Iron Ridges are the terrain type where the ore is closest to the surface, easiest to access, and highest in yield. Every dynasty needs iron from the mid-game onward. The ridge territories that contain it are among the most contested zones on any map.

Resource profile: The match's highest iron concentration per zone. Moderate stone as a secondary. No food. No wood of significance. Water is scarce — rivers rarely originate in ridge formations.

Military properties: Contested terrain with no strong advantage to either side. Ridgelines provide limited high-ground advantage over open plains but are not as defensible as Stone Highlands. Enough rough terrain to disrupt cavalry charges without fully negating them. Siege engines and heavy infantry are the dominant forces here.

Faith affinity: Blood Dominion holds specific ritual significance for Iron Ridges — their theology frames iron as a material that has been tested and survived, a physical covenant between material and purpose. Blood Dominion dynasties receive modest iron extraction bonuses in ridge territory.

Strategic value: Whoever controls the ridge territory controls the iron economy. In the late game, when cavalry and heavy infantry dominate, the dynasty processing the most iron fields the best-equipped army. Iron Ridges are always worth fighting over.

Visual description: Exposed rust-red and dark grey rock faces where iron veins are visible to the naked eye. Rough terrain, short scrub vegetation, exposed ore faces where extraction has begun. Mine structures cling to the ridgelines. The land looks like it has been turned inside out by geological force — because it has.

### River Valleys

River Valleys are the connective tissue of the match world. They carry water to surrounding regions, enable agricultural development in otherwise dry areas, and create the natural movement corridors that trade caravans and armies follow. They are also the most politically contested terrain at match start, because every founding house needs water access and the rivers that provide it flow through territory that multiple houses can reach.

Resource profile: Significant food yield — river valley agriculture is the second-most productive food source after Reclaimed Plains. Permanent water access eliminates one of the early game's primary infrastructure costs. Modest wood from riverbank growth. Gold trade routes naturally follow river valley corridors.

Military properties: Rivers themselves are defensive barriers — crossing them under fire is costly and slow. River valley terrain channels army movement into predictable corridors, which favors players who can control the chokepoints. Cavalry moves quickly along valley floors.

Faith affinity: The Wild covenant holds river valleys significant as places where water and land meet in sustained relationship. Old Light views river valleys as evidence of the world's restoration — water returning to where water belongs. All four faiths perform water rituals when faith infrastructure is established in river valley terrain.

Strategic value: Early control of a river valley secures food and water simultaneously. Late control of multiple valley connections enables troop movement at speed that landlocked dynasties cannot match. Cutting an enemy's river access is a genuine economic attack.

Visual description: Flowing water cutting through varied terrain, green banks sustained by proximity to the river, broader agricultural development on the flat valley floor. Bridges where roads cross. Defensive positions often sited at river bends or narrows where the water does the defensive work.

### Coastal Zones

Coastal Zones are the match's naval threshold. Where land meets sea, the world opens in a direction that landlocked dynasties cannot access. Coastal control means trade routes that do not follow road networks, supply chains immune to land blockades, and the capacity to project military force across water to enemies who do not expect attack from that direction.

Resource profile: Significant food from fishing operations — consistent and not weather-dependent. Access to salt, which has trade value and functions as a preservation resource extending supply range for armies in the field. Gold trade routes through port infrastructure can become the highest-volume exchanges in the match.

Military properties: Naval combat occurs here. Land forces defending coastal terrain need to account for amphibious assault vectors. Coastal fortifications specialize differently from inland fortifications — harbor-defense positioning and naval artillery coverage matter. Control of a harbor allows loading and deploying troop transport capacity.

Faith affinity: All faiths regard the sea with spiritual weight. Old Light sees the horizon as the direction of the world's future. The Wild regards the deep ocean as the one domain its covenant has not fully named. Blood Dominion's dark doctrine has specific ritual associations with the sea's indifference to human suffering.

Strategic value: Coastal control is essential for naval victory strategies and meaningfully useful for every other path. A dynasty without any coastal access depends entirely on land-based supply chains. A dynasty with developed coastal infrastructure has trade and military supply options that its opponents must work much harder to disrupt.

Visual description: Rocky headlands, sandy beaches, harbor towns where boats are visible at all stages of preparation and use. The sea itself visible from elevated positions throughout the zone. Coastal fortifications integrate naturally with harbor structures. The atmosphere shifts with match season — summer coastal zones look and feel different from winter ones.

### Frost Ruins

Frost Ruins are the match's most haunted terrain. These are the sites of pre-Frost civilization that 170 years of ice preserved rather than buried. The ruins are intact in the way that things preserved in cold are intact — not deteriorated by ordinary decay, but frozen into a specific moment. Coming into a Frost Ruin zone is coming into the world as it was before the catastrophe.

Resource profile: Significant stone recoverable from structures without quarrying. Occasional iron artifacts with salvage value. No food, no wood from the ruins themselves, but ruins are typically sited in locations that had food and water access in the pre-Frost era, and those resources may still be present nearby.

Military properties: Dense structural terrain that severely limits formation fighting. Close-quarters infantry dominates. Cavalry is nearly useless in intact ruin structures. The ruins provide exceptional cover and ambush capacity. Defensive positions in ruin terrain are highly favorable to the smaller force.

Faith affinity: Old Light holds Frost Ruins as sacred historical sites — places where the memory they are charged to preserve is physically present. Old Light dynasties receive significant faith intensity bonuses for controlling Frost Ruin zones and can perform specific memorial rites there. Blood Dominion regards the ruins with complex theology — the Frost was a kind of sacrifice, and the ruins are the permanent scar.

Strategic value: Frost Ruins provide defensive strongpoints, material salvage, and faith significance for Old Light players. The ruin events system generates match events specific to each ruin zone — discoveries, historical revelations, and encounters with remnants of pre-Frost culture that affect diplomacy and conviction.

Visual description: Stone buildings preserved in states of frozen moment — not crumbled but interrupted. Doors that were open remain open. Personal objects visible through windows. Ice sometimes still present in sheltered interior spaces despite the broader Reclamation. The Old Light builds its shrines here, at the edges and thresholds of preserved spaces.

### Badlands

Badlands are the places the Frost left broken. Extreme temperature cycling fractured the geology, killed the vegetation, and produced a terrain type that is difficult to inhabit, difficult to cross, and difficult to exploit — but not without value to those willing to work for it.

Resource profile: Sparse food. No wood. Modest stone. The primary resource is iron exposed by geological fracturing — not as concentrated as Iron Ridges but accessible without the same infrastructure investment. Some gold from specific mineral deposits.

Military properties: Slow movement for all unit types. Units in sustained Badlands operations develop attrition at an accelerated rate without proper supply chain investment. Ambush capacity is moderate. No strong defensive advantage or disadvantage for conventional fortification.

Faith affinity: Blood Dominion regards Badlands as proving grounds — the theology of endurance and cost-bearing is felt most clearly in terrain that asks the most from the people moving through it. The Wild regards Badlands as territory that resists human order, which is not threatening to their worldview.

Strategic value: Badlands function as buffer zones between more valuable territories. A dynasty that fortifies Badlands effectively creates a costly obstacle between itself and adversaries. The iron deposits make sustained Badlands occupation worthwhile for resource-focused players who have the logistics to support it.

Visual description: Fractured rock faces, sparse dry vegetation, dust. Sky is wide because there is nothing tall enough to interrupt it. The terrain looks like the aftermath of something — which it is. Colors are ochre, grey, and washed-out brown at every time of day.

### Sacred Ground

Sacred Ground is the most unusual terrain type in Bloodlines. It is not defined by geology or climate but by history and spiritual accumulation — these are places where the concentration of significant events, ritual practice, and faith investment across generations has produced ground that all four covenants acknowledge as different. They do not agree on why it is different. They do not agree on what it means. But no covenant leader who has stood on Sacred Ground has been able to claim it is ordinary.

Resource profile: Modest food. No stone or iron. No wood of significance. The resource Sacred Ground provides is faith intensity — passive generation requiring no building investment, simply the act of controlling the zone.

Military properties: No fortification of any kind can be built on Sacred Ground. This is the single hardest terrain constraint in the game, and it does not bend. Sacred Ground cannot be fortified. Military structures on its borders can defend the approaches. The ground itself is defended only by the armies standing on it. This creates an inherent vulnerability: the most spiritually significant zones are the most exposed to direct military contest.

Faith affinity: All four covenants respond to Sacred Ground, each in their own way. Old Light builds memorial structures at the edges. Blood Dominion performs specific rites unique to Sacred Ground presence. The Order establishes watch-posts of formal mandate. The Wild simply inhabits the ground as it is, regarding it as the purest expression of their covenant's relationship with the world.

Strategic value: Faith intensity generation without building cost. A key ritual site for Covenant Tests. The unfortifiable nature creates a dynamic where Sacred Ground is always contested by whichever factions most need the faith it provides.

Visual description: Ground that looks, on close inspection, slightly different from the terrain around it. Soil that is darker or lighter than what surrounds it. Rock formations arranged with a regularity that suggests intention without clear evidence of it. Ancient markers — stone, wood, carved earth — placed by hands that are centuries gone. No new construction stands here. The new structures are always at the edge, looking in.

### Tundra

Tundra is the match's peripheral terrain — the edge of the world where the Frost still lives in the ground. The permafrost never fully retreated. The Frost's worst decades are preserved here in the soil itself, a geological record of what the world endured.

Resource profile: Very limited food — cold-resistant crops only, at reduced yield. Occasional stone accessible through permafrost mining. Some iron accessible with significant infrastructure investment. Water in frozen form is everywhere; liquid water requires active infrastructure. Rich fur trade if population is positioned and sustained here.

Military properties: Severe cold weather attrition for units not equipped or trained for tundra conditions. Cold-adapted units operate normally. Movement is slow for all unprepared forces. Supply chains are extraordinarily costly to maintain at tundra distance. Any protracted military campaign in Tundra without appropriate logistics will lose its army to attrition before it finds a fight worth having.

Faith affinity: The Wild holds Tundra as a reminder of the world's extremes — The Wild covenant does not fear the cold but reads it as honest. Old Light regards Tundra as the last territory the Frost has not yet yielded, a frontier of the restoration work that remains incomplete.

Strategic value: Tundra is peripheral by design. It is not where matches are decided. It serves as a buffer zone at map edges, a source of specific resources for dynasties that develop cold-weather logistics capability, and terrain that can be used to force pursuing armies to pay attrition costs in the chase.

Visual description: Flat, treeless, pale. The sky dominates completely. Snow cover for much of the match year. What little vegetation exists is low to the ground and adapted to endure there. The permafrost gives the soil a particular quality — it looks solid but does not behave as solid ground under sustained pressure or weight.

---

## Section 17: Map Structure — Three-Layer Architecture and Generation Principles

### The Three Layers

Every Bloodlines map is built from three distinct layers that stack and interact continuously without the player needing to manage them separately.

The base layer is the terrain itself. This is the ground that units walk on, the rivers that block or enable movement, the forests that conceal and slow, the highlands that confer elevation advantage. Every location on the map has a terrain type drawn from the ten canonical types. This layer is what the player sees and what determines unit behavior, resource availability, and military dynamics at the squad level.

The province layer sits above the terrain. Provinces are the political and economic units of territorial governance. Each province has a defined boundary, a loyalty value, a population count, and a set of active resources being worked. Provinces can shift loyalty, be contested, be formally claimed through governance acts, or revolt. The province layer is the unit of political control. When a player says they hold a territory, they mean they hold provinces — the terrain under them does not change, but who governs the people on that terrain does.

The calculation layer underlies both. This is the hex or tile grid that the engine uses to compute resource yields, movement costs, attack vectors, and event triggers. The player never sees this layer directly. It exists to ensure that the resource production of a given terrain tile is calculated consistently, that unit movement across varied terrain costs the correct action points, and that event triggers fire at the right moments. It is invisible and foundational.

### Scale

The Bloodlines map is roughly ten times the size of a typical RTS map. This is not a stylistic choice. It is a functional requirement: the match duration is 2-10 hours, and the world needs enough geographic scope to sustain meaningful strategic decisions across that entire span. A small map at ten hours becomes fully explored and contested within the first hour. The world must feel large enough that a dynasty can hold a coherent strategic position without being in simultaneous contact with every other dynasty.

There is no ceiling on map generation scope. The ten-times-standard scale is a floor, not a ceiling. Larger maps support more players and more minor tribes, which deepens the political world. No match size is defined as too large.

### Sacred Ground Cannot Be Fortified

This rule applies universally across the map and in all three layers. The terrain base layer marks Sacred Ground zones. The province layer tracks political control of them. The calculation layer enforces the no-fortification constraint absolutely. No building type classified as a fortification structure may be placed within a Sacred Ground province. Defensive perimeter structures may be placed immediately adjacent to Sacred Ground provinces — controlling the approaches without building on the ground itself. The constraint is architectural and cannot be overridden by any player action or bloodline ability.

### Province Generation

Provinces are generated at map creation with boundaries that follow natural terrain features — river edges, forest lines, highland ridgelines, and coastal boundaries serve as natural province borders wherever possible. Provinces that span terrain types have mixed resource profiles. The number of provinces scales with map size. A standard map contains approximately 200 to 400 provinces, ensuring that no founding house starts with more than 5 to 10 percent of the map under direct control. Most of the world is unclaimed, contested, or held by minor tribes at match start.

---

## Section 18: Minor Tribes — Diplomacy, Development, and Relationship Memory

### The Political World Outside the Nine Houses

Minor tribes are the majority of the world's surviving population organized outside the nine-house framework. The nine founding houses built lasting political structures, but the people who survived the Great Frost without becoming founding house subjects did not disappear. They organized themselves into communities scaled to what they could sustain. Some are hundreds of people. Some are in the thousands. Collectively they represent the human population that exists between the founding houses' territories, within the deep wilderness and high terrain that founding houses have not yet reached, and at the strategic nodes that multiple dynasties want to control.

A typical match contains 8 to 14 minor tribes depending on map size. They are distributed according to archetype alignment with terrain type: the Forest Clans occupy Ancient Forest, the Highland Keepers occupy Stone Highlands, the River Stewards occupy River Valley zones. Minor tribes do not randomly spawn in terrain that is inconsistent with their archetype.

### The Ten Tribe Archetypes

**The Remnant Faith** descends from pre-Frost religious communities that preserved doctrinal practices predating the four covenants. Their faith identity is specific and not easily converted — they have been practicing it longer than the four covenants have existed in their post-Frost forms. A dynasty that shares their covenant and approaches them respectfully gains a faith alignment bonus that is difficult to obtain any other way. A dynasty that attempts forced conversion faces determined resistance and a conviction cost that reflects what the act actually is.

**The Frost Covenant** coalesced during the Great Frost itself. These communities were formed entirely by shared survival under extreme conditions. Their founding myth is the Frost, which means they are more psychologically resilient under pressure than any other archetype and significantly less responsive to economic incentives. They cannot be purchased. Relationships with them must be built through demonstrated shared values and sustained presence over time.

**The Near-House** carries the memory of a dynasty that nearly became a founding house and did not. They have records, oral histories, and in some cases written documentation of the lineage that died in the Frost. They carry resentment toward specific founding houses that displaced their ancestors or denied their claims. This resentment is named and specific — a Near-House will communicate to a player exactly why they distrust a specific founding house. This information functions as intelligence.

**The Nomadic Warband** survived the Frost by moving and never stopped. They cannot be pressured through territorial siege — they leave if a relationship becomes unfavorable. The only leverage a founding house has with a Nomadic Warband is the value of the relationship itself. Maintained well, they offer military capability that founding house armies cannot replicate: extreme speed, terrain knowledge, unconventional tactics. Maintained poorly, they disappear and reappear fighting for someone who treated them better.

**The River Stewards** organized around water resource management and hold the crossing infrastructure that founded their communities. Economically focused, militarily competent within their specific domain, and reliably trade-responsive. They are the easiest archetype to establish productive relationships with and the first to accept formal alliance or vassal arrangements.

**The Highland Keepers** are the most defensively potent archetype in their home terrain. Displacing them militarily requires an investment that makes the territory barely worth the cost of taking it. Their territories contain stone and iron. The productive approach is economic integration — they will sell the resources, they will allow infrastructure development, but they will not be conquered without demonstrating that the cost was not worth it.

**The Forest Clans** live in Ancient Forest and know it completely. They fight with ambush, concealment, and terrain exploitation in ways that direct military combat cannot easily counter. They are frequently aligned with The Wild covenant. For Wild-faith dynasties, the Forest Clans are natural allies whose terrain expertise amplifies the dynasty's existing advantages. For other faith paths, they are a persistent threat and a potential source of intelligence about wilderness movement.

**The Iron Workers** developed iron-working capability during the Frost out of necessity and have refined it since. They produce iron more efficiently than standard mine infrastructure and can be integrated as a permanent economic partnership. A dynasty that establishes a productive relationship with the Iron Workers gains iron production capacity that compounds through the mid-game period when iron demand is highest.

**The Sacred Ground Guardians** are positioned on or near Sacred Ground terrain and have served as its traditional stewards since before the founding houses existed. They do not claim political sovereignty over the Sacred Ground — they protect it. Attacking them on or near Sacred Ground carries faith consequences regardless of which covenant the attacking dynasty follows. Every faith recognizes that these people have been performing a custodial function, and attacking them while they are doing it says something specific about the attacking dynasty's character.

**The Trade Network** operates at the intersection of multiple trade routes with connections to several other tribes and factions. Modest military, significant intelligence and diplomatic value. A dynasty that builds a strong relationship with the Trade Network gains an information flow that rivals the Trueborn City's intelligence reach, covering the tribal tier rather than the founding house tier.

### Tribe Relationship Memory

Tribes remember. Every action a founding house takes that affects a tribe is logged and affects the relationship score. Raiding a tribe's settlements for resources creates a wound in the relationship that simple positive actions cannot quickly heal. Offering tribute during a famine builds goodwill that persists across the match. Protecting a tribe from an attacking rival builds trust that survives political turbulence. Forcing a faith conversion on a tribe with strong existing faith identity creates an adversary that will resurface at the worst possible moment.

Radicalized tribes — those whose relationship with a specific founding house has deteriorated past a defined threshold — will approach adversary dynasties with offers of military cooperation. The founding house that has mistreated multiple tribes finds itself in a world that remembers exactly what it did and has organized around that memory.

### Tribe Integration and the Trueborn City

Minor tribes exist outside the nine-house framework but not outside the Trueborn City's network. The Trade Network tribe has direct Trueborn City connections. Other tribes interact with the city through these channels. A founding house that destroys or severely damages a tribe with Trueborn City ties triggers a diplomatic consequence with the city — the city is aware of what happened to the people it traded with.

---

## Section 19: The Trueborn City — Architecture, Diplomacy, and The Rise Arc

### Origins and Survival Through the Frost

The Trueborn City predates the Great Frost by centuries. It is the oldest continuously inhabited political structure in the game world. When the Frost came and every other political institution collapsed or contracted into survival mode, the city survived because of a calculation that proved correct: all factions facing starvation and desperation needed at least one neutral point of exchange. Attacking the city destroyed the one place where enemies could buy food from each other without having to stop fighting.

The Trueborn City's neutrality is not a treaty. It is an institutional memory of what happened when neutrality was violated during the Frost's middle century — a brief period when a desperate faction seized the city, collapsed the trade network, and triggered a cascade of famines that historians of the Age of Reclamation consider the worst decade of the entire Frost period. The city was retaken within eight years but the decade of collapse produced generational trauma that no founding house has forgotten. The memory functions as the city's most durable defense.

### The Trueborn City's Function

The Trueborn City is the primary conduit for global information. Trade routes passing through the city carry news from across the world — population sentiment, faith distribution, political events, and faction movements. Dynasties with active trade relationships with the city receive accurate, timely world-state information. Dynasties diplomatically isolated from the city receive degraded or delayed information, which makes every strategic decision more costly.

The city also functions as the world's diplomatic center of gravity. Its Exchange Hall — the primary trade and diplomatic forum — extends a guarantee of protection to all registered representatives conducting business under the city's auspices. This guarantee is the foundation of its value. Killing or capturing a registered representative on Trueborn City neutral ground triggers an automatic Trueborn Summons against the perpetrator — a mobilization of every dynasty with an active relationship with the city into a temporary punitive coalition. The coalition does not expand or claim territory. It exists to punish the violation and force diplomatic resolution.

A dynasty that triggers the Summons faces a coalition whose individual members may be otherwise hostile to each other but are institutionally aligned in this specific moment. The coalition dissolves when the perpetrating dynasty pays a defined reparative cost or is militarily defeated.

### Conquest and the Sovereignty Anchor

Conquering the Trueborn City is the hardest single military objective in any match. The city's defenses incorporate centuries of accumulated military architecture, its garrison is supported by the full weight of the city's economic resources, and attacking it triggers immediate diplomatic pressure from every faction with city ties. The conquest window requires either prior diplomatic isolation of the city from its external supporters or overwhelming force applied faster than the political response can mobilize.

The reward for successful conquest is the Sovereignty Anchor — a permanent legitimacy bonus affecting every province in the match world. The Sovereignty Anchor does not win the game, but it accelerates pursuit of any victory path by shifting the global population's baseline trust toward the conquering dynasty. The world pays attention to who holds the city.

### The Trueborn Rise Arc — Three Stages

If no founding house has conquered or meaningfully challenged the Trueborn City by a match-specific threshold, the city activates. The Trueborn raise their banners and begin actively asserting themselves as the legitimate successor to pre-Frost political authority. This is not aggressive conquest. It is the city pursuing what it believes is rightfully and historically its own — not domination, but Restoration.

Stage One begins with the city establishing formal governance claims over territories within its immediate sphere. These claims are delivered through diplomatic channels as requests for recognition. Founding house territories adjacent to the city face gentle but persistent legitimacy pressure — the city's historical claim generates passive doubt in the local population's loyalty to the occupying dynasty.

Stage Two escalates when no founding house has responded constructively to Stage One claims. The city's passive legitimacy pressure becomes active — missions of recognition travel to every founding house with an active relationship. Founding houses that recognize the Trueborn claim gain significant diplomatic standing with the city and modest economic bonuses. Founding houses that refuse face accelerating loyalty erosion in their peripheral territories.

Stage Three is the full assertion. The Trueborn City fields military forces and pursues territorial reconquest — not of the entire world but of what it regards as its ancestral administrative zones. It has access to accumulated resources across centuries of neutral trade, historical legitimacy that generates passive loyalty conversion in contested territories, and the coalition infrastructure that sustained the city through the Frost. A founding house that has ignored the Rise Arc to this stage faces a competitor pursuing the Sovereignty Seat through mechanics no founding house has available. The Trueborn are not pursuing power for its own sake. They are pursuing Restoration.

---

## Section 20: Random World Generation and Starting Conditions

### Procedural Generation Philosophy

Every Bloodlines map is generated procedurally within constraints that ensure playability, diversity, and strategic depth. The generation is not purely random — it follows rules that guarantee meaningful distribution of terrain types, resource access, and starting positions. A bad procedural roll cannot produce a map where one founding house starts with all the iron and another starts with no water. The constraints exist to ensure that all starting positions are viable, not equal.

Terrain distribution starts with a continental shape determined by a noise function, then applies biome rules that ensure each terrain type appears in proportions within defined ranges. Stone Highlands appear only on elevated landmasses. River Valleys are generated by water flow simulation from highland origins to coastal or interior endpoints. Ancient Forests cluster around valleys and lowland areas with sufficient water. Frost Ruins are placed at locations where pre-Frost settlement density would have been highest — near river confluences and coastal zones. Sacred Ground is placed at a defined number of locations per map, never randomly distributed but positioned at points where multiple terrain boundaries meet, because those are the places where human activity concentrated over time.

### Regional Culture and Starting Identity

The region where a dynasty starts shapes its economic baseline and its default military composition, even though all tech trees and unit rosters are available to all houses. A dynasty starting in a heavily forested region with Wild faith tribal neighbors will develop different early game habits than a dynasty starting on open plains with Order faith ruins nearby. This regional character is not a faction lock — it is starting pressure that creates natural initial divergence which the player is free to move away from if they choose.

Regional culture also determines which minor tribes are present in the starting area. A dynasty starting near a Sacred Ground zone will have Sacred Ground Guardians as immediate neighbors. A dynasty starting in highland terrain will have Highland Keepers nearby. The player's first diplomatic decisions are shaped by who their immediate neighbors are.

### Starting Position Implications by House Type

The generation algorithm attempts to give each house type a starting position that aligns with their strategic identity without making it mandatory. Stonehelm placed in open plains loses nothing except the extreme defensive amplification Stone Highlands provide — their building bonuses and defensive unit capabilities still function, just without terrain enhancement. Houses do not require their natural terrain to be competitive.

However, houses placed in their natural terrain express their full potential faster. Ironmark starting near Iron Ridges develops iron infrastructure two to three match years ahead of schedule. Hartvale starting on rich Reclaimed Plains with a River Valley nearby produces a population expansion in the early game that other houses require additional investment to match. Westland starting at the frontier edge of the map where wilderness meets contested territory produces an Outrider force that covers the early map before opponents have organized a response.

### Year 80 Reclamation Starting Context

The match begins in Year 80 of the Age of Reclamation. This is not an arbitrary number. It is chosen to produce a specific starting condition: iron infrastructure is emerging but not yet mature, faith institutions are established enough to define cultural identity but not yet fully built out, and each founding house has a recognizable character that developed over eighty years of post-Frost consolidation.

At Year 80, the nine founding houses know who they are. They have solved the survival problem. They are beginning to think about more than survival. The match question is not who will live through the next decade — everyone has solved that problem already — but who will inherit the world that the Reclamation is producing. Year 80 is the moment that question becomes an active contest rather than a distant abstraction.

Starting population for a standard match is approximately 80 households per founding house, each household representing the base unit of population accounting. The broader world contains minor tribe populations that equal or exceed founding house populations at match start. The founding houses are powerful relative to individual political actors but not yet overwhelming relative to the world's total population. The world is still to be won.

---

# PART IV — FAITH SYSTEM

---

## Section 21: The Four Ancient Faiths — Deep Overview

### The Theological Problem

The Great Frost raised a question that every surviving community had to answer: what does this mean? A civilization was nearly destroyed. 170 years of cold killed agriculture, collapsed trade, severed political bonds, and reduced populations to scattered survival communities. Something that organized and functional came apart. The question of why — and what survivors were supposed to do with the answer — produced four genuinely different theologies that emerged from the Frost period and have endured through the Reclamation because they each answer the question convincingly to their adherents.

These are not game bonus systems dressed in theological language. They are complete worldviews. Each covenant makes specific claims about the nature of the world, the reasons for the Frost, the obligations of survivors, and the path forward. A player who commits to a faith covenant at the end of Level 1 is not choosing a stat bonus. They are committing their civilization to a specific account of why they exist and what they are supposed to do with the power they accumulate. That commitment shapes unit types, ritual options, population relationships, diplomatic reception, and the kind of player they become.

### The Old Light

The Old Light remembers. This is the complete summary of their theology, and it is not a simplification — remembrance is the covenant's first and deepest obligation. The Old Light holds that the Great Frost was the consequence of forgetting: forgetting obligations, forgetting the bonds between people, forgetting that civilization is an ongoing act of maintenance rather than a stable condition. When communities stopped investing in the relationships that held the world together, the world fell apart. The Frost was not punishment from outside. It was the natural consequence of internal collapse that had been happening long before the cold came.

The survivors' obligation is therefore to remember. Remember the world as it was. Remember the people who were lost. Remember how it happened so it cannot happen again. The Old Light builds shrines to specific events and named dead. Their holy sites are Frost Ruins — places where the pre-Frost world is physically preserved. Their rituals are memorial rituals. Their architecture is archival.

The Light Path of the Old Light is vigil-keeping, documentation, and the cultivation of a community that does not let memory decay. The Faith Leader performs rites that reconnect the living to the historical record. The dynasty that follows the Light Path of Old Light tends toward Moral conviction naturally — the theology frames cruelty as the failure mode that caused the Frost.

The Dark Path of the Old Light is the Purge Templar system. When the covenant's analysis identifies communities, practices, or political formations that recreate the conditions that caused the Frost — cheapened obligations, broken bonds, the systematic forgetting of consequence — the Dark Path demands intervention. Purge Templars are not inquisitors in the religious persecution sense. They are agents of historical enforcement, tasked with preventing the recurrence of the conditions that produced the Frost. In practice, the distinction between historical enforcement and religious persecution is a line that Dark Path Old Light dynasties cross when they believe the stakes are high enough. The Purge Templar system is the mechanism through which a dynasty can do genuinely terrible things while maintaining the sincere conviction that they are preventing something worse.

What the Old Light demands from its followers: sustained investment in remembrance infrastructure, regular ritual observance, protection of historical sites, and the moral seriousness to confront forgetting wherever it is found.

What the Old Light offers in return: deep faith intensity from Frost Ruin control, memorial bonuses that generate conviction stability over time, Purge Templar units with unique enforcement capabilities on the Dark Path, and the diplomatic trust of communities that share their theology of consequence.

### Blood Dominion

Blood Dominion demands. This is the theological core: the covenant holds that the world was not nearly destroyed by neglect but by insufficiency — that the pre-Frost civilization failed because it was not strong enough, not committed enough, not willing to pay the prices that survival and power actually require. The Frost was a test. The survivors are the ones who paid enough to pass it. The covenant now demands that its followers continue to pay, because the world's demands do not stop.

Blood Dominion's theology is transactional at its foundation. Power is real. Power requires cost. Anyone claiming power without paying its true cost is running a deficit that will eventually come due. The covenant formalizes the cost — makes it explicit, ritualized, and managed rather than hidden. Blood Dominion dynasties are not hiding from the price of power. They are building a religion around the honest acknowledgment of it.

The Light Path of Blood Dominion is the path of earned covenant. The follower pays costs — blood in formal ritual, resources in sustained tribute, personal sacrifice in documented, voluntary form — and receives in return the covenant's power in proportion to what was paid. The Covenant Priest manages these exchanges. The light doctrine holds that voluntary, consensual, formally witnessed sacrifice is the purest expression of the covenant's theology. No one is compelled. Everyone chooses what they offer.

The Dark Path is where the covenant's logic extends to its furthest conclusion. If power requires cost, and if the dynasty needs power now, then the question becomes whose cost is acceptable. The Dark Path of Blood Dominion authorizes sacrifice that is neither voluntary nor consensual in the strictest sense — including, at the extreme end, the sacrifice of lesser bloodline members to produce dynasty-wide power bonuses. The Covenant Officiant is the role that manages this path. The population knows or suspects what the Officiant does. The Keep's other members give the Officiant a wider berth. The power that results from the Binding is real.

What Blood Dominion demands from its followers: formal, witnessed acts of sacrifice and covenant-keeping, sustained ritual maintenance, and the willingness to pay prices that other faiths frame as unacceptable.

What Blood Dominion offers in return: the most direct conversion of cost into power of any covenant, unique ritual capabilities tied to sacrifice mechanics, iron extraction bonuses in Blood Dominion's theologically significant terrain, and access to the Covenant Binding system whose outputs exceed what any other faith produces at equivalent investment.

### The Order

The Order enforces. The covenant's theology holds that the Great Frost was the product of insufficient structure — not the forgetting that Old Light identifies, but the absence of systems strong enough to hold human behavior to account. Individual moral failure is not the problem. Individual moral failure is inevitable. The problem is the absence of frameworks that detect it, correct it, and prevent it from cascading. The Frost happened because there were no institutions with sufficient authority to force compliance with the obligations that hold civilization together.

The Order's response is institutional. Build the systems. Establish the mandates. Create the enforcement mechanisms. The covenant does not expect moral perfection from its followers — it expects compliance with articulated, publicly stated rules. The Order documents obligations, monitors adherence, and enforces consequences. The theology is that a civilization that has clear rules and enforces them is more stable than a civilization that relies on virtue alone.

The Light Path of The Order is administrative governance. The Faith Leader in an Order dynasty is a bureaucratic official as much as a spiritual one — they document, witness, adjudicate, and maintain the record. Order dynasties on the Light Path tend toward territorial stability that significantly exceeds what other faith paths produce, because the population understands the rules and the rules are enforced consistently.

The Dark Path of The Order is Dominion enforcement — the point at which the covenant's institutional logic extends to populations that have not consented to The Order's framework. Dominion Arbiters are deployed to establish Order mandate in territories whose populations follow different faiths or no faith. The theological justification is that insufficient structure caused the Frost, and insufficient structure anywhere is a threat to the stability The Order is trying to build. The practical consequence is that Order dynasties on the Dark Path impose compliance on populations who did not choose it, administered by officials who have been given the authority to do so by a covenant whose own rules authorize the expansion.

What The Order demands from its followers: formal acknowledgment of mandate, documented compliance with articulated rules, and the institutional infrastructure to monitor and enforce.

What The Order offers in return: the match's highest territorial stability bonuses, unique administrative efficiency improvements that no other faith path produces, Dominion enforcement capabilities that pacify conquered territories faster than military occupation alone, and diplomatic trust from communities that value predictable governance over personal virtue.

### The Wild

The Wild endures. The covenant's theology is the simplest of the four in statement and the most complex in implication: the world existed before human civilization and it will exist after. The Great Frost was not a moral failure, not a test, not a structural problem. It was the world asserting its own terms against a civilization that had stopped paying attention to them. The Frost was not punishment. It was the world being itself.

The Wild's covenant asks its followers to build their civilization in relationship with the world rather than in conquest of it. Not passivity — the Wild covenant does not ask its followers to stop building or fighting. It asks them to understand what they are building within and to factor the world's terms into their plans. A dynasty that burns every Ancient Forest for wood has made a calculation. The Wild holds that the calculation does not end when the wood runs out.

The Light Path of The Wild is integration. Wild-faith dynasties on the Light Path develop genuine relationships with the terrain they inhabit. Their forests are managed rather than consumed. Their rivers are channeled rather than blocked. Their relationship with Wild-aligned minor tribes — the Forest Clans, the River Stewards — is reciprocal rather than extractive. The faith bonuses from Ancient Forest control are highest for Light Path Wild dynasties because the theology is being expressed in practice.

The Dark Path of The Wild is Bonebreaker shamanism — the strand of the covenant that does not negotiate with the world but calls on its most extreme capacities. Bonebreaker shamans access powers that other Wild practitioners regard as dangerous to channel: forces that operate below the level of cultivation and relationship, older and less discriminating. The Dark Path Wild dynasty can bring things onto a battlefield that are not strictly military. The cost is that these forces are not reliably controlled, and the shamans who channel them are changed by the experience in ways that are not always reversible.

What The Wild demands from its followers: genuine relationship with the terrain they inhabit, sustained respect for the world's independent terms, and the honesty to acknowledge when they are taking from the world rather than exchanging with it.

What The Wild offers in return: exceptional terrain-specific military advantages — highest of any faith in Ancient Forest — bear rider cavalry that requires no iron infrastructure, passive faith intensity from Ancient Forest control, bear and animal-mounted unit options available to no other covenant, and the strategic depth of a faith that makes your terrain itself a tactical asset.

---

## Section 22: Faith Intensity System — Five Tiers and the Mechanics of Maintenance

### What Faith Intensity Measures

Faith intensity is not a measure of sincerity. It is a measure of investment — the accumulated weight of buildings built, rituals performed, territory held, and bloodline members committed to faith practice. A dynasty with high faith intensity has been consistently acting in accordance with their covenant's demands. A dynasty with low faith intensity has not. The world's response to each dynasty's faith investment is calibrated to this accumulated record.

Faith intensity is tracked as a percentage from 0 to 100, divided into five tiers. Movement between tiers unlocks new capabilities and changes how the world responds to the dynasty. Faith intensity decays without maintenance — the covenant's demands do not pause.

### The Five Tiers

**Latent (0-20%):** The dynasty has declared a faith affiliation but has invested little in it. Level 1 faith units are accessible. Basic faith buildings are available. The population knows what faith the dynasty nominally follows but does not yet organize their lives around it. No meaningful diplomatic signal is generated by Latent faith intensity.

**Active (20-40%):** The dynasty has built basic faith infrastructure and is performing regular ritual observance. Level 2 faith-influenced units become more efficient. The population begins to internalize the covenant's framework — loyalty is modestly improved for populations that share the covenant. Other dynasties recognize the faith commitment as genuine. The Faith Leader role becomes actively valuable at this tier.

**Devout (40-60%):** The dynasty's faith commitment is visible and significant. Level 3 faith units become accessible — the point at which faith doctrine begins to differentiate the military. The Covenant Test fires for the first time at Devout entry. The population has genuinely organized its understanding of the world around the covenant's framework. Faith-based diplomatic relationships with same-covenant tribes and minor kingdoms become available.

**Fervent (60-80%):** The dynasty's faith investment has reached a level that changes how it is perceived across the entire match world. Level 4 faith units unlock — all units at this tier are faith-specific, representing the irreversible commitment of the dynasty's military identity to its covenant. The Faith Leader's physical appearance has changed. Their bearing carries weight that earlier stages did not. The population does not merely follow the covenant — they believe it. Same-covenant minor tribes actively seek alliance. Opposing-covenant dynasties begin passive diplomatic pressure.

**Apex (80-100%):** The dynasty has committed everything. The Level 5 apex unit becomes eligible — one per faith, one or two instances per match across the entire game world. The Faith Leader looks like a person who has been partly consumed by something larger than themselves. The dynasty's territory is visually and functionally transformed by faith saturation — buildings carry faith iconography, population behavior reflects covenant imperatives, and the spiritual character of the civilization is unmistakable. This is the tier at which Faith Divine Right victory path requirements begin to resolve.

### How Faith Intensity Is Generated

Faith intensity is produced by a combination of sources, none of which is passive. Faith buildings generate intensity proportional to their tier — a basic Wayshrine generates modest intensity; an Eternal Cathedral generates significant intensity continuously. Ritual performance by a committed Faith Leader generates bursts of intensity at defined intervals. Territory control generates intensity for terrain with faith affinity — Ancient Forest for The Wild, Frost Ruins for Old Light, Iron Ridges for Blood Dominion, developed province networks for The Order.

Bloodline member faith alignment contributes to intensity — members with high personal faith conviction generate passive intensity simply by being active. Population composition matters: a population with high same-covenant faith among its households generates intensity from its daily practice, separate from any building or official structure.

### How Faith Intensity Decays

Faith intensity decays continuously without active maintenance. The rate of decay scales with current intensity tier — it is harder to maintain Apex than to maintain Active. Without a Faith Leader committed to ritual function, the decay accelerates. Without faith buildings operating at their required maintenance cost, the generation drops. Military defeat that destroys faith buildings triggers a significant intensity loss. The death of a high-development Faith Leader produces a major intensity drop.

Enemy faith structures in contested territory generate counter-intensity that specifically works against the occupying dynasty's faith level in that region. A dynasty that conquers Blood Dominion territory still populated by Blood Dominion believers and filled with Blood Dominion altars faces active faith decay in their territory until they dismantle the opposing infrastructure or convert the population — neither of which is quick.

---

## Section 23: Faith Buildings — Covenant-Specific Architecture and Function

### How Faith Buildings Work

Each covenant has its own building progression with three tiers. The buildings are distinct in name, appearance, and function. They share a common structural logic: first-tier buildings establish faith presence, second-tier buildings generate sustained intensity and ritual capacity, third-tier grand structures saturate the territory and unlock apex mechanics. Only one grand faith structure per covenant can exist anywhere on the map — not per player, but per match world. The third-tier grand structure is a significant diplomatic signal and a contested objective.

Building costs in resources scale with tier and are not trivial. Faith infrastructure competes with economic and military infrastructure for the same resource pools. The player who invests heavily in faith buildings has made a real tradeoff. The payoff is faith intensity generation and ritual access that military investment alone cannot produce.

### Old Light: Memory Architecture

**Wayshrine** (Tier 1): A simple stone structure marking a location of historical significance. Can be placed on any terrain. Generates low faith intensity. Enables basic memorial ritual events. Visual character: unadorned stone with the house's iconographic carving at the top, designed to be permanent and to survive weather without maintenance.

**Hall of Remembrance** (Tier 2): A substantial building containing documented records — names of the dead, accounts of significant events, copies of pre-Frost documentation recovered from Frost Ruins. Requires Wayshrine presence in the territory. Generates moderate faith intensity. Enables the Grand Rite of Documentation and allows the Faith Leader to perform the Memorial Vigil — a ritual that generates a significant conviction stability bonus for the dynasty. Visual character: stone library aesthetics, multiple chambers, scribes visible through the windows.

**The Eternal Cathedral** (Tier 3, unique per match): The grand structure of Old Light is not a cathedral in the traditional religious sense. It is an archive that doubles as a ceremonial space — the largest repository of pre-Frost documentation in the current world. Its construction requires stone at a significant level, a Hall of Remembrance in the territory, and at least Fervent faith intensity. Generates high faith intensity continuously. Enables the Restoration Declaration — the formal act initiating the Faith Divine Right victory path for Old Light dynasties. Visual character: vast stone structure with deep internal archives and a formal ceremonial chamber. The building looks like it is meant to last until the next Frost.

### Blood Dominion: The Architecture of Cost

**Blood-Altar** (Tier 1): A formal ritual site for the performance of covenant exchanges. Placement requires a source of stone at minimum. Generates low faith intensity. Enables the Covenant Offering ritual — a small, voluntary sacrifice ritual that generates a modest, temporary production bonus. Visual character: dark stone altar construction, efficient and undecorated, designed for use rather than display.

**Covenant Hall** (Tier 2): The administrative and ritual center of Blood Dominion practice in a territory. Requires a Blood-Altar. Generates moderate faith intensity. Enables the Binding Ritual — a significant covenant exchange producing meaningful sustained bonuses. Enables the Covenant Priest role to perform the Grand Binding, the highest-value light-doctrine ritual available before Tier 3. Visual character: substantial construction with interior ritual chambers, the exterior deliberately understated.

**The Wound** (Tier 3, unique per match): The grand structure of Blood Dominion is named for what it represents — the covenant's honest acknowledgment that power requires cost, and that the place where that exchange happens most fully is a wound in the normal order of things. The Wound's construction requires significant resources across multiple types, a Covenant Hall in the territory, and Fervent faith intensity. Generates high faith intensity continuously. The dark-doctrine ritual of maximum covenant binding is only available at a Wound. It is also the site where the Faith Divine Right declaration fires for Blood Dominion dynasties. Visual character: the only grand structure in the game that is visually dark in an honest sense — not theatrical darkness but the specific quality of a place where consequential things happen.

### The Order: Institutional Architecture

**Lawpost** (Tier 1): A formal administrative station marking the presence of Order mandate in a territory. Generates low faith intensity. Enables the Mandate Declaration — a formal statement of Order law applying to the territory, which modestly increases the province's loyalty baseline over time. Visual character: a functional administrative structure, stone with clean lines, designed to communicate permanence and legitimacy.

**Hall of Mandate** (Tier 2): The regional administrative center for Order governance. Requires Lawpost presence. Generates moderate faith intensity. Enables the Order Tribunal — a significant administrative ritual that can resolve a local revolt or dispute through formal process rather than military force. Also enables the Iron Legionary to be recruited locally rather than from the capital. Visual character: substantial institutional architecture, stone construction with formal entrance.

**The Iron Spire** (Tier 3, unique per match): The grand structure of The Order is a literal tower of administration — the highest visible structure in any territory it is built in, designed to be seen from distance. Its presence asserts that The Order's mandate extends to the horizon. Requires substantial stone investment, a Hall of Mandate, and Fervent faith intensity. Generates high faith intensity continuously. The Dominion Declaration initiating the Faith Divine Right victory path for Order dynasties fires from the Iron Spire. Visual character: a tall, squared stone tower with administrative chambers at every level and a beacon at the peak that is visible at night.

### The Wild: Living Architecture

**Grove Marker** (Tier 1): Not a constructed building but a formal designation of a grove or natural feature as covenant-significant. Requires Ancient Forest or River Valley terrain. Generates low faith intensity. Enables basic spirit acknowledgment rituals. Visual character: carved wooden markers placed at the boundary of the designated area, integrated with the existing terrain rather than replacing it.

**Spirit Lodge** (Tier 2): A structure built within or at the edge of Wild-significant terrain, serving as the formal center of Wild practice. Requires a Grove Marker in the territory. Generates moderate faith intensity. Enables the Spirit Communion — a ritual that strengthens the passive combat bonuses Wild faith provides in the surrounding terrain. Also enables the Thornrider and Bear Rider units to be recruited locally. Visual character: wooden construction, partially open to the outside, designed to exist within the forest rather than impose on it.

**The First Grove** (Tier 3, unique per match): The grand structure of The Wild is the most unusual grand structure in the match — it is defined as the oldest living grove, not a constructed building at all, but a designated area of Ancient Forest that has been formally recognized as the covenant's highest sacred site. It requires the player to control an Ancient Forest zone at Fervent faith intensity and to have committed sufficient Faith Leader ritual practice to the location. It does not need to be built so much as recognized and maintained. Generates high faith intensity continuously. The Covenant of the Wild declaration initiating the Faith Divine Right victory path fires from the First Grove. Visual character: an ancient section of forest that is visually distinct from surrounding forest — older, denser, with the quality of light and silence that Sacred Ground has.

---

## Section 24: Faith Doctrine Paths — Light and Dark

### The Doctrine Architecture

Every covenant has two doctrine paths: Light and Dark. These are not moral labels — they are directional labels describing the nature of the covenant's expression. Light doctrine paths express the covenant's theology through affirmative action: building, maintaining, performing, cultivating. Dark doctrine paths express the covenant's theology through its more demanding, more extractive, more consequential capacities. A dynasty can follow Light doctrine with Cruel conviction. A dynasty can follow Dark doctrine with Moral conviction. The doctrine axis and the conviction axis are independent.

Doctrine path is determined by the specific rituals and unit types the player develops. There is no single moment of doctrine declaration — doctrine emerges from pattern. A player who builds Purge Templar units and performs inquisition rituals is on Old Light Dark doctrine, regardless of what other labels they claim. The world responds to what the dynasty actually does, not what it says it does.

### Old Light Doctrine Paths

**Light Path — The Path of Remembrance:** The dynasty invests in documentation, memorial architecture, and sustained ritual observance. The Faith Leader performs vigils at Frost Ruins and Halls of Remembrance. The population is cultivated to maintain active historical memory as a community practice. Military units on this path are Flame Wardens — soldiers who carry the Old Light's defensive mission into combat, protecting historical sites and civilian populations rather than pressing aggressive campaigns. The Flame Warden's abilities reflect this: strong defensive bonuses, ability to consecrate held territory against desecration, reduced rout risk in defensive engagements.

The Light Path generates conviction stability over time — the sustained practice of remembrance tends to keep the dynasty's moral record legible. Diplomatically, Old Light Light Path dynasties are trusted by communities that share their theology of consequence and by non-covenant communities that respect the dynasty's visible moral consistency.

**Dark Path — The Purge Templar System:** The dynasty has determined that the prevention of the conditions that caused the Frost requires active enforcement. The Purge Templar is the instrument of that enforcement: a unit class that operates on the covenant's analysis of what poses a threat to civilization's stability. Purge Templars are deployed against communities, practices, or political formations that Old Light analysis identifies as recreating the Frost's preconditions.

The dark-path rituals of Old Light are inquisition processes — formal investigations with the power to condemn, exile, or eliminate identified threats. These rituals are theologically rigorous within Old Light's own framework. The dynasty conducting them does not regard the targets as innocent. The conviction cost of Purge Templar operations reflects the weight of the actions, not their illegitimacy within the covenant's terms. A dynasty on Dark Path Old Light accumulates Cruel conviction if Purge Templar operations are extensive — but the conviction cost is precisely calibrated to reflect reality, not to punish the player for following their faith's logic.

### Blood Dominion Doctrine Paths

**Light Path — The Path of Earned Covenant:** The dynasty performs formal, witnessed, voluntary covenant rituals. Every exchange is documented. The Covenant Priest oversees the process with full transparency. The population understands what is being offered and what is being received. The light-path ritual suite generates power bonuses through a clear, maintained system of cost and return that feels — within the covenant's theology — honest.

The Covenant Scion unit is the military expression of the Light Path: soldiers who have voluntarily entered into a formal Blood Dominion covenant that enhances their combat capabilities in exchange for explicit obligations they have accepted. Their effectiveness reflects the covenant's thesis — committed sacrifice produces genuine power.

**Dark Path — Covenant Binding Beyond Consent:** The dynasty has concluded that the covenant's demands and the stakes of power justify extending the exchange beyond the willing. The Covenant Officiant performs Bindings that involve parties who did not fully consent, including at the extreme end the sacrifice of lesser bloodline members chosen by the player from the active or dormant roster. The sacrifice is permanent. The Bloodbound unit is the dark-path military expression: soldiers who have entered into covenant relationships whose terms they did not fully understand at signing and cannot exit. Their combat effectiveness reflects what has been extracted from them.

The dark-path ritual of maximum covenant binding — available only at The Wound — is the most powerful single ritual in the match. Its cost is commensurate. The player must make a specific sacrifice that the game asks them to name. The bonus that results is persistent and real. The action is recorded in the match history.

### The Order Doctrine Paths

**Light Path — The Path of Legitimate Mandate:** The dynasty establishes Order governance through formal process — Mandate Declarations, Order Tribunals, documented rules publicly stated before enforcement begins. The population governed by Light Path Order knows what the rules are and what happens when they are broken. The Iron Legionary is the military expression of this path: disciplined professional soldiers operating under clear rules of engagement, known for what they will and will not do.

The Light Path generates the highest territorial stability of any faith path combination in the game. Populations under formal, consistent, transparent governance develop deep loyalty over time that is resistant to disruption. The Order's theology is validated by the outcome.

**Dark Path — Dominion Enforcement:** The dynasty has extended The Order's framework to populations who did not consent to it and enforces compliance through the Warden-Enforcer and Dominion Arbiter units. The theological justification is that insufficient structure is a danger regardless of whether the population being structured desires it. The Dominion Arbiter has formal authority to establish compliance in conquered territories through means that go beyond what Light Path enforcement permits.

The dark-path rituals of The Order are enforcement instruments — procedures for establishing Dominion mandate in territories whose populations resist it. They work. The territory stabilizes under enforcement faster than it would under Light Path governance in the short term. The long-term loyalty depth produced by Dominion enforcement is lower than what Light Path governance produces, which creates a structural fragility that dark-path Order dynasties must manage continuously.

### The Wild Doctrine Paths

**Light Path — The Path of Integration:** The dynasty builds its civilization in genuine relationship with the terrain and the forces the covenant acknowledges. Ancient Forests are managed rather than consumed. River systems are worked with rather than against. The Thornrider unit is the military expression of this path — cavalry that moves through forest terrain that neutralizes conventional cavalry, operating in partnership with the terrain rather than being restricted by it.

Light Path Wild dynasties produce the match's best terrain-advantage multipliers. Their units in Ancient Forest are demonstrably different from other units in Ancient Forest. Their relationship with Wild-aligned minor tribes generates military and intelligence assets that compound over time. The covenant's promise is that genuine relationship with the world produces genuine advantage within it.

**Dark Path — Bonebreaker Shamanism:** The dynasty has reached into capacities that even Wild practitioners regard as dangerous to channel. Bonebreaker shamans access forces that are older than cultivation and less discriminating in their expression. The Bear Rider on the dark path is not a light-path beast cavalry unit — it is something that has been drawn into a darker compact. The Bonebreaker unit itself is a shaman whose combat role is to introduce forces onto a battlefield that neither side fully controls.

The dark-path rituals of The Wild involve invoking capacities that The Wild's light practitioners regard as a violation of the relationship the covenant is built on — taking from the world's extreme capacities rather than exchanging with them. These rituals generate real military effects. They also change the shamans who perform them. Bonebreaker shamans in the Keep's visual record look different from other faith leaders. Other family members keep a respectful distance. The Wild's dark path is not corrupt — it is the honest acknowledgment that the world's terms include things that are not gentle.

---

## Section 25: Faith Units — Level 3 Through Level 5

### The Unit Progression Architecture

Faith does not affect military units until Level 3. The first two unit tiers are shared across all houses with house-specific stat variations, reflecting the survival era and early iron transition. At Level 3, faith begins to shape military options. At Level 4, all units are faith-specific and the divergence is irreversible. At Level 5, one apex unit per faith exists, with one or two instances accessible per match across the entire game world.

The doctrine path adds a second dimension: at Level 3 and Level 4, each faith has one light-doctrine unit and one dark-doctrine unit. A dynasty that has been developing primarily along the Light Path will find its Level 3 and Level 4 units reflect that path. A dynasty that has committed to the Dark Path accesses the dark-doctrine units instead. Switching doctrine paths after Level 3 divergence is possible but expensive — faith intensity loss and a political signal that the dynasty is doctrinally unstable.

### Old Light Units

**Level 3 — Flame Warden (Light Path):** Infantry unit developed from Old Light's theology of protection and preservation. Specializes in defensive engagements and the protection of historically significant territory. Consecration ability: the Flame Warden can designate held territory as consecrated, making it more costly for enemy units to desecrate or occupy without triggering a covenant response. Strong against dark-doctrine units in direct combat due to faith-strengthened conviction.

**Level 3 — Inquisitor (Dark Path):** The Purge Templar system's field operative. Functions as a combined investigator and enforcer. The Inquisitor's abilities are oriented toward identifying and eliminating designated threats within territories — effectively a specialized infantry unit with additional ability to trigger investigation events against enemy bloodline members who enter their operational area. Weaker in open field combat than the Flame Warden but significantly more effective in targeted enforcement operations.

**Level 4 — Sunblade Champion (Light Path):** The elite infantry of Old Light's light doctrine. A veteran soldier whose faith investment has reached the point where their combat effectiveness is augmented by accumulated conviction. The Sunblade Champion fights with greater effectiveness when protecting a territory that has been consecrated and when fighting alongside a Faith Leader or high-faith bloodline member. In open field combat against non-light opponents: strong but not dominant. In defense of consecrated territory: among the most effective defensive infantry in the match.

**Level 4 — Purge Templar (Dark Path):** The military expression of Old Light's dark doctrine at full development. A Purge Templar leads operations rather than participating in them — they are the command authority for enforcement actions, the individual whose doctrinal authority authorizes the action and whose presence in the field makes it official. Their combat effectiveness is secondary to their ability to authorize investigation rituals against specific targets and to provide the legal framework for dark-doctrine enforcement operations within The Order's and Blood Dominion's territories.

**Level 5 — The Unbroken (Apex):** One instance per match, globally. The Unbroken is a bloodline member who has passed through a specific historical trauma — loss, betrayal, near-death — and emerged with their faith deepened rather than destroyed. The Unbroken is not a military unit in the conventional sense. They are a historical anchor: their presence on a battlefield removes rout risk from all nearby units, their presence in the Keep generates conviction stability that cannot be produced any other way, and their existence generates passive faith intensity across the dynasty's entire territory. The Unbroken cannot be replaced. The Unbroken cannot be replicated.

### Blood Dominion Units

**Level 3 — Covenant Priest (Light Path):** A military chaplain integrated into the Blood Dominion's field operations. The Covenant Priest does not fight directly — they perform battlefield rituals that restore small amounts of health to squads in their radius, generate temporary combat bonuses from formal covenant invocations, and manage the voluntary covenant agreements that light-doctrine Blood Dominion forces operate under. Fragile. High-value support role.

**Level 3 — Blood Rider (Dark Path):** A cavalry unit that has entered into a combat covenant with Blood Dominion doctrine. The Blood Rider fights with amplified effectiveness early in an engagement when fresh covenant energy is active and degrades in effectiveness if the engagement extends beyond the covenant's expected duration. Fast, powerful in the first shock of contact, increasingly vulnerable in sustained engagement. Requires iron infrastructure. The dark-doctrine cavalry solution.

**Level 4 — Covenant Scion (Light Path):** Elite infantry who have formally entered into light-doctrine Blood Dominion covenant. Their combat effectiveness scales with the intensity of the covenant — Scions who have served in multiple engagements and renewed their covenant agreements after each are significantly stronger than freshly covenanted recruits. The Covenant Scion system rewards maintaining the same units rather than refreshing them, which creates a strategic tension with the Born of Sacrifice recycling system.

**Level 4 — Bloodbound (Dark Path):** Soldiers who entered into covenant relationships whose terms they did not fully understand. The Bloodbound is not a willing soldier in the conventional sense — they are an effective one. Combat effectiveness is high and does not require the sustained personal investment that Covenant Scions require, because the terms of the Bloodbound's covenant were set by someone else. They fight without the emotional complexity of voluntary commitment. They fight without the cost of choice. They are very dangerous.

**Level 5 — The Sacrificed (Apex):** The entity produced by the complete enactment of the maximum covenant binding ritual at The Wound. One per match, globally. The Sacrificed is the cost made manifest — whatever was offered at the covenant's deepest level has been transformed into something that is no longer fully human. Their presence on a battlefield is not a combat unit presence in the conventional sense. They are a force. Enemies in their radius experience conviction degradation — not fear as a mechanical penalty, but a genuine processing of what they are looking at. The Sacrificed cannot be replicated. Losing them is a loss of something that was paid for once and will not be paid for again.

### The Order Units

**Level 3 — Iron Legionary (Light Path):** The professional soldier of The Order's light doctrine. Disciplined, consistent, and reliable under sustained pressure in ways that less institutionalized soldiers are not. The Iron Legionary's strength is formation coherence — they do not rout easily, they maintain formation structure under pressure better than equivalent units, and they fight at a consistent level across the full duration of an engagement rather than peaking and degrading. Not spectacular. Reliable in the way that institutions are reliable: predictably, at scale.

**Level 3 — Warden-Enforcer (Dark Path):** The Dominion enforcement presence in a territory. Not primarily a field combat unit — a governance presence. The Warden-Enforcer maintains compliance in occupied territories through visible authority rather than through building trust. Their presence suppresses revolt risk and reduces civilian resistance more quickly than peaceful governance, at the cost of the deep loyalty that peaceful governance eventually generates.

**Level 4 — Iron Sentinel (Light Path):** The elite formation soldier of The Order's light doctrine. The Iron Sentinel is a direct upgrade of the Iron Legionary's core qualities — more resilient, more formation-coherent, more effective in the sustained engagement scenarios where The Order's military doctrine operates. In siege defense, the Iron Sentinel is the best defensive infantry in the match per unit cost. In offensive operations, they are slower and less dramatic than equivalent faith path units, but they hold what they take and they do not break.

**Level 4 — Dominion Arbiter (Dark Path):** The Dominion enforcement authority at its full development. The Dominion Arbiter has been granted formal Order mandate to establish compliance in territories through whatever means the doctrine authorizes. Their presence in a conquered territory accelerates the stabilization process significantly compared to conventional occupation, because they are not waiting for population trust to develop — they are establishing the conditions for compliance through formal enforcement. The deep loyalty that Light Path governance produces does not follow from Dominion Arbiter-established compliance. The territory holds. It does not bond.

**Level 5 — The Mandate (Apex):** The single individual who embodies The Order's complete institutional authority in physical form. One per match. The Mandate does not have a name in the way that other apex units do — the name is the role. Their presence in any territory immediately and permanently resolves all revolt risk for the duration of their presence. Other dynasties' representatives interacting with The Mandate experience a form of diplomatic pressure that is not hostility but is also not negotiation — they are being addressed by a recognized authority. The Mandate cannot be replaced. The institution that produced them persists, but the individual expression of it at that level is singular.

### The Wild Units

**Level 3 — Thornrider (Light Path):** Cavalry designed for Ancient Forest terrain. Where conventional cavalry is neutralized by forest density, the Thornrider moves through it at speed. Not as powerful as conventional cavalry in open terrain — the Thornrider trades open-field effectiveness for terrain adaptability. In mixed terrain with forest components: among the most mobile cavalry options in the match. The light-path expression of The Wild's thesis that integration with terrain produces advantages within it.

**Level 3 — Bear Rider (Dark Path):** The alternative cavalry that requires no iron infrastructure. A trained bear with a rider, accessed through The Wild's relationship with the world's animal populations. Bear Riders are slower than horse cavalry in open terrain but significantly more powerful in direct combat — a bear is not a horse and does not behave like one. In confined terrain where cavalry charges are less effective, the Bear Rider's strength-over-speed profile gives it an advantage that horse cavalry does not have.

**Level 4 — Forest Sovereign (Light Path):** The elite Wild military unit of the light path — a commander whose understanding of terrain has reached a level where they function as a force multiplier for every Wild unit in their proximity. The Forest Sovereign's abilities are oriented toward terrain exploitation: they enhance ambush effectiveness for nearby units, extend the forest movement bonuses of nearby Thornriders, and can call environmental effects — sudden weather changes, terrain obstruction — that are available only to Fervent or higher Wild faith dynasties.

**Level 4 — Bonebreaker (Dark Path):** A shaman who has accessed the Wild's more extreme capacities and channels them in combat. The Bonebreaker's abilities do not map neatly to damage or defense ratings. They introduce forces onto the battlefield that affect both sides — fear in enemy formations, behavioral disruption in enemy units, environmental effects that do not discriminate between friend and enemy. A Bonebreaker is an unreliable weapon by conventional standards. In the right context, with the right preparation, they are the most disruptive single unit in the match.

**Level 5 — The First Wild (Apex):** One per match. Not an individual in the conventional sense — a manifestation. Something that the Wild covenant's darkest and deepest practices have been calling toward through the accumulated ritual investment of the dynasty's match history. The First Wild's presence on a battlefield changes the field itself. The terrain responds. Animal life in the area responds. Enemy units in proximity to The First Wild experience conviction degradation and behavioral disruption. Their specific capabilities resist precise specification because they emerge from the doctrine and the match history of the dynasty that produced them — but they are real, they are singular, and losing them means something that cannot be undone.

---

## Section 26: The Covenant Test — Faith Demanding Action

### What the Covenant Test Is

The Covenant Test fires when any dynasty reaches Level 4 unit progression — the moment when faith doctrine becomes irreversible in the military sense. At this threshold, the covenant makes a demand. The theology is simple: a dynasty that has committed this deeply to the covenant's framework is being asked to demonstrate that the commitment is real, not merely developmental. The test is not punitive. It is the covenant expressing what it actually requires.

The test is different per faith and per doctrine path. It is always a grand act — something that requires meaningful investment and cannot be trivially completed. It is always thematically consistent with the covenant's theology. It is always real in its consequences if failed.

### Old Light Covenant Tests

**Light Path:** The covenant demands a formal Act of Memorial — the construction or completion of a Hall of Remembrance or Eternal Cathedral in a territory that was previously contested and has now been stabilized. The act must be witnessed by the Faith Leader performing the Memorial Grand Rite at the completed structure. The theological content of the test: the covenant demands that the dynasty demonstrate its capacity to build lasting memory in the places where contest and struggle have occurred. Victory over territory is not enough. The Old Light demands what comes after victory.

Failure: faith intensity drops by one full tier. The test can be retried at the next appropriate opportunity, at higher cost.

**Dark Path:** The covenant demands a formal Purge operation against a designated threat — a specific community, practice, or political formation that the Old Light's dark doctrine has identified as recreating the conditions of the Frost. The operation must be documented in full by the Faith Leader, with evidence of the threat's nature and the action taken. The theological content: the dark path demands that the dynasty demonstrate it has the conviction to act on its analysis rather than merely articulate it.

Failure: Purge Templar effectiveness is permanently reduced until the test is completed. The test carries a significant conviction cost when completed — the action is registered by the conviction system at full weight.

### Blood Dominion Covenant Tests

**Light Path:** The covenant demands a Grand Covenant Ceremony — a formal, witnessed, large-scale voluntary covenant exchange involving a significant portion of the dynasty's active military forces. Every soldier who participates formally enters a Blood Dominion covenant. The ceremony requires a Covenant Hall or better, the Faith Leader in full officiant role, and sufficient covenant priests to process the volume. The theological content: the light path's thesis is that voluntary, formal, witnessed covenant is the purest expression of the doctrine. The test demands the dynasty demonstrate this at scale.

Failure: faith intensity drops and the Covenant Scion units already in service experience a morale degradation event.

**Dark Path:** The covenant demands a Binding of significant cost — a specific sacrifice that the game presents to the player with full information about what is being asked and what will result. The test does not specify what must be sacrificed beyond "something real." The player may choose from eligible options — including lesser bloodline members — but the choice must be made and the sacrifice must be real. The theological content: the dark path thesis is that power requires genuine cost. The test demands the dynasty demonstrate it can identify what it values and that it can spend it.

Failure: the Wound's covenant binding rituals are locked until the test is completed. The test, when completed, generates a permanent dynasty-wide power bonus proportional to the cost of what was sacrificed.

### The Order Covenant Tests

**Light Path:** The covenant demands a Mandate Codification — the production of a formal written governance document covering all territories under Order control, distributed to all lesser houses and vassal populations within the dynasty's sphere. Every rule must be articulated. Every enforcement mechanism must be named. The Order Tribunal must be in active function in every controlled province before the test resolves. The theological content: the light path thesis is that legitimate mandate requires transparency and completeness. The test demands that the dynasty govern the way it claims it governs.

Failure: territorial stability bonuses are suspended until the test is completed. The test, when completed, generates a permanent stability boost in all controlled territories.

**Dark Path:** The covenant demands a Dominion Expansion event — the formal imposition of Order mandate on a territory or community that has not accepted it. The expansion must be accompanied by the deployment of Warden-Enforcers and the formal issuance of Dominion mandate, and the territory must remain stable under enforcement for a defined period before the test resolves. The theological content: the dark path thesis is that insufficient structure is a danger regardless of consent. The test demands the dynasty demonstrate it can establish and maintain Dominion in a resistant environment.

Failure: Dominion Arbiter effectiveness is reduced until the test is completed. Completion generates a significant population compliance bonus across all Dominion-governed territories.

### The Wild Covenant Tests

**Light Path:** The covenant demands a Grove Renewal — the formal designation and restoration of an Ancient Forest zone that has been damaged or diminished during the match. If no such zone exists in the dynasty's territory, the test demands the expansion of Wild faith infrastructure into a new Ancient Forest zone that had not previously been formally committed. The Faith Leader must perform the Grove Renewal ritual at the Spirit Lodge or First Grove. The theological content: the light path thesis is that relationship with the world means sustaining it, not merely using it. The test demands the dynasty demonstrate it can give back as well as take.

Failure: Ancient Forest combat bonuses are reduced for Wild units until the test is completed.

**Dark Path:** The covenant demands a Summoning — the formal invocation of a Bonebreaker shaman's deepest capabilities in a real military engagement rather than a ritual context. The test requires that a Bonebreaker unit be deployed in a significant engagement and that the forces they channel have a demonstrable effect on the battle's outcome. The theological content: the dark path thesis is that accessing the world's extreme capacities requires demonstration, not declaration. The test demands the dynasty prove the Bonebreaker is what it claims to be.

Failure: Bonebreaker abilities are reduced until the test is completed. Completion unlocks access to the First Wild apex unit eligibility.

---

## Section 27: Faith Spread and Global Faith Tracking

### How Faith Spreads

Faith is not contained within a single dynasty's territory. The covenants are missionary faiths — not all equally or with the same methods, but all interested in the expansion of their theological framework beyond their originating dynasty.

Cultural influence spreads faith passively across shared borders. A dynasty with Devout faith intensity and a large population sharing that faith with a neighboring province generates slow, passive faith conversion among the neighboring population over time. The rate scales with the intensity differential, the population size on both sides, and the presence of faith buildings near the border.

Conquest spreads faith actively when the conquering dynasty has sufficient faith intensity and chooses to invest in conversion infrastructure. Leaving conquered populations to their existing faith is simpler in the short term — they resist less immediately. Converting them produces deeper long-term loyalty if successful, at significant short-term investment cost and with an extended resistance window.

Missionary operations by a Faith Leader or committed Bloodline Member with high faith attributes can accelerate faith spread in specific territories through direct community engagement. The Wild covenant does this least formally of the four. The Order does it most systematically.

Shared trade routes carry faith as a cultural byproduct. Dynasties conducting significant trade with a high-intensity faith dynasty find their populations gradually exposed to covenant frameworks through contact with traders, goods with religious iconography, and the general cultural presence of a deeply faith-invested trading partner.

### Global Faith Distribution

The Trueborn City's trade network makes global faith distribution visible. Dynasties with active Trueborn City relationships can access information about what percentage of the world's population follows each covenant and at what intensity. This information is tactically and strategically significant — a world in which Blood Dominion has grown to majority global faith distribution is a different match environment than one where all four covenants remain roughly equal.

Global faith percentage affects match-wide events. When any single covenant crosses a significant global distribution threshold, covenant-specific match events become available: Holy War declarations, Ecumenical Councils, faith-triggered coalitions, and the emergence of covenant-wide diplomatic alignments that cut across political loyalties.

### Holy War Triggers

When a dominant covenant's global faith distribution exceeds 50 percent, the Holy War trigger becomes available to any dynasty following that covenant with Fervent or higher faith intensity. A Holy War declaration changes the diplomatic character of the match — it frames the conflict between covenant dynasties and non-covenant territories as formally sanctioned, generating faith intensity bonuses for participating covenant dynasties, legitimizing military expansion into non-covenant territories on a theological rather than political basis, and generating counter-coalition pressure from non-covenant dynasties who recognize what a Holy War means for their long-term survival.

Holy War is not automatically a good option for the declaring dynasty. It generates enemies. It also generates allies among same-covenant dynasties who were waiting for a formal framework to participate. The declaring dynasty is gambling that the alliance it triggers outweighs the coalition it provokes. Most Holy Wars in Bloodlines matches end in negotiated resolution rather than total victory, which means the declaring dynasty spends conviction, faith intensity, and military resources on an outcome that a less dramatic diplomatic approach might have reached for less cost. Most declaring dynasties would dispute this analysis.

---

# PART V — CONVICTION SYSTEM

---

## Section 28: The Conviction Axis — Independence, Scale, and Foundational Principles

### Two Axes, Not One

Conviction and Faith Doctrine are independent systems. This is settled canon and cannot be compromised in design. A dynasty can follow Old Light dark doctrine while accumulating Moral conviction. A dynasty can follow Wild light doctrine while accumulating Cruel conviction. The two axes interact — they are not isolated — but neither determines the other. No position on the faith doctrine axis implies any position on the conviction axis.

The reason this independence is non-negotiable is that it reflects how power actually works. Religious people commit cruelties. Irreligious people demonstrate extraordinary compassion. A dynasty that follows Blood Dominion dark doctrine is not automatically Cruel — they have chosen a demanding theology, not an immoral one. A dynasty that follows Old Light light doctrine is not automatically Moral — they remember, but memory does not prevent atrocity. The game must be honest about this because the players building dynasties with genuine strategic seriousness need a system that is honest about it.

### The Conviction Scale

The conviction axis runs from Apex Moral through five bands to Apex Cruel. The player does not choose a position on this axis. They are placed by the accumulation of their actions. Every significant action the player takes generates Conviction Points — positive (Moral) or negative (Cruel). The pattern amplifies over time through the Pattern Amplification system. The current position on the axis determines which milestones are active, which diplomatic responses the dynasty receives, and which systems operate efficiently or at cost.

The five bands: Apex Moral, Moral, Neutral, Cruel, Apex Cruel. Movement between bands requires consistent action in the relevant direction. Movement away from an extreme requires sustained behavior in the opposite direction — the dynasty is not merely stopping the behavior that produced the extreme, it is demonstrating sustained commitment to the opposite pattern.

### Conviction Points and Pattern Amplification

Conviction Points are generated by specific, defined actions. The weighting reflects the actual moral significance of the action as understood within the game's world — not as calculated by an external moral framework, but as registered by the populations, institutions, and systems that respond to the dynasty's behavior.

Highest-weight positive actions: providing food aid to starving populations (+6 CP), honoring a disadvantageous peace treaty that the dynasty could have broken without consequence (+5 CP), voluntarily releasing captured enemy bloodline members rather than executing or holding them for ransom (+4 CP), rebuilding civilian infrastructure in conquered territory without extracted tribute (+4 CP).

Highest-weight negative actions: sacrificing unbeliever populations in Blood Dominion dark covenant (-15 CP, extreme threshold), executing captured bloodline members of defeated dynasties (-8 CP), using forced labor for infrastructure construction (-6 CP), deliberately targeting civilian infrastructure for destruction as a military strategy (-7 CP), breaking a formal treaty when the treaty could have been maintained (-5 CP).

Pattern amplification is the mechanism that makes conviction meaningful over time. The first time a dynasty provides food aid to starving populations, it generates 1x the base CP value. The second time the same dynasty provides food aid in comparable circumstances, it generates 1.5x. The third and subsequent times: 2x. The pattern is recognized. The dynasty is not performing an individual act — it is demonstrating a behavioral commitment, which is what conviction actually means.

The same amplification applies to negative actions. The first forced labor deployment generates 1x the negative CP. The second generates 1.5x. The third and beyond: 2x. A dynasty that consistently extracts labor through force is not committing isolated acts — it is building an institutional pattern that the world registers accordingly.

---

## Section 29: Conviction Milestones — Both Directions Have Genuine Value

### The Design Principle

Both conviction extremes are genuine strategic assets. The Sovereignty Seat can be reached from Apex Moral conviction. The Sovereignty Seat can be reached from Apex Cruel conviction. The game does not declare a winner by conviction posture. The two extremes produce different civilizations with different strengths, different vulnerabilities, and different paths to the match's final objective. This is not a moral relativism statement. It is a recognition that the game is modeling power, and power is not inherently aligned with virtue or with cruelty — it is aligned with whatever sustains it.

### Moral Milestones

**Volunteer Recruitment:** At Moral conviction, military recruitment generates volunteers who join at lower gold cost than standard recruits. The dynasty's reputation for Moral governance creates a population that is willing to serve without full mercenary compensation. Volunteer recruits have baseline morale that is slightly higher than standard recruits because they chose to be here.

**Reduced Succession Revolt Risk:** Moral dynasties face significantly lower revolt probability during succession events. The population that has been governed well over time has loyalty that is tied to the dynasty's demonstrated record, not to the specific individual who just died. The record survives the individual.

**Diplomatic Trust Bonuses:** Same-conviction and broadly Moral dynasties offer more favorable initial diplomatic terms to Moral dynasties. Trade deals are more accessible. Alliance terms are less demanding. The diplomatic trust that Moral conviction generates is not charity — it is the calculation that a Moral dynasty is more likely to honor agreements, which makes agreements with them worth more.

**Population Loyalty Depth:** Moral dynasty populations develop deeper loyalty over time that is harder to disrupt through enemy propaganda, faith pressure, or economic hardship. A Moral dynasty that loses a major battle does not immediately face population revolt — the population has reasons to hold beyond the military outcome.

**Apex Moral — The Recognized Protector:** At Apex Moral conviction, the dynasty is recognized across the match world as a protector rather than merely a power. Minor tribes approach voluntarily. Neutral communities offer tribute without coercion. Coalition requests from threatened smaller powers generate genuine military partnership offers rather than instrumental agreements. The Trueborn City offers trade terms unavailable to any other conviction posture.

### Cruel Milestones

**Fear Compliance:** At Cruel conviction, populations in controlled territory comply with taxation and military levies at a rate that exceeds what Moral governance produces in the short term. Fear is efficient. The population pays because not paying has visible and credible consequences. The efficiency is real. It does not compound the way loyalty compounds, but it does not require the sustained investment that loyalty requires either.

**Cheaper Forced Labor:** Cruel dynasties access forced labor for infrastructure construction at significantly reduced cost compared to hired labor. The productivity loss from involuntary labor is partially offset by the population cost. Infrastructure gets built faster than it would under Moral governance's investment-based approach.

**Tribute Extraction:** Cruel dynasties can extract tribute from vassal houses and conquered populations at higher rates than Moral dynasties. The vassal structure operates on fear rather than loyalty, which allows the dynasty to take more than willing partners would provide.

**Apex Cruel — The Fear Standard:** At Apex Cruel conviction, the dynasty has established a reputation that functions as a passive strategic deterrent. Other dynasties calculate the cost of engaging with an Apex Cruel power differently than they calculate engaging with a Moral one. Cowardly AI archetypes avoid direct conflict. Even aggressive AI archetypes approach Apex Cruel powers with additional preparation. The fear is not just a flavor descriptor — it modifies the match's diplomatic and military calculations.

### Extreme Milestones — The Trueborn Response

At either conviction extreme, if the Trueborn City has not been conquered or neutralized, a specific response becomes available to the city and to sufficiently threatened other dynasties.

For extreme Cruel dynasties: when Cruel conviction reaches Apex and stays there beyond a defined threshold, the Trueborn City is authorized to issue a formal Coalition Call against the dynasty. This is not a Holy War — it has no faith component. It is a political and moral statement from the world's diplomatic center that the dynasty's pattern of behavior constitutes a threat to collective stability. The Coalition Call does not guarantee a military response, but it authorizes one and provides the diplomatic framework for it to organize.

For extreme Moral dynasties: the dynamic is more subtle and more positive. Extreme Moral dynasties do not trigger a Trueborn threat response. Instead, they receive the Trueborn's endorsement for specific victory path declarations, which accelerates the path's resolution mechanics. The world recognizes that an Apex Moral dynasty making a claim on the Sovereignty Seat is a different kind of proposition than an Apex Cruel dynasty making the same claim.

---

## Section 30: Conviction and Population

### How Population Responds to Conviction

The population is not an undifferentiated mass that responds uniformly to the dynasty's conviction posture. Different population segments respond differently based on their own composition, faith alignment, and historical relationship with the governing dynasty.

A Moral dynasty governing a population with a strong match history of being treated well has deep loyalty that is resistant to disruption. A Cruel dynasty governing a population with a history of extraction and fear has compliance that is efficient in the short term and brittle under sustained pressure.

Migration responds to conviction posture. Moral dynasties attract voluntary migrants from neighboring territories — people who have heard about the governance quality and want to live under it. This is not a mechanic that fires instantly. It is a slow accumulation over many match years. But a Moral dynasty that has maintained its posture for a long time will find its population growing faster than its military investment accounts for, because the world is choosing to join them.

Desertion is more common in Cruel dynasties during periods of military failure. The fear that maintains compliance during success does not sustain loyalty during defeat. When a Cruel dynasty faces a significant military reversal, the compliance structure can unravel faster than Moral dynasty equivalent. Population that stayed because it was afraid has less reason to stay when the thing it feared is losing.

### Population Conviction Divergence

The population's own conviction posture can diverge from the dynasty's official posture. A Moral dynasty that governs a population with deep Cruel cultural traditions — through conquest, for instance — will find that the population's conviction posture is Cruel even as the dynasty's is Moral. The dynasty's governance eventually shifts the population's posture, but the shift takes time and requires consistent demonstration rather than declaration.

The reverse is also true. A Cruel dynasty inheriting a Moral population — through dynastic succession, for instance — initially faces a population whose internal standards are different from the governance being applied. The Cruel governance gradually shifts the population's posture, or the population revolts.

### Extreme Cruel and Irreversible Population Decline

Extreme Cruel conviction maintained over extended match time produces irreversible population decline. The mechanisms are specific: desertion accumulates without full recovery, birth rates decline in fear-maintained communities, voluntary migration reverses into voluntary exodus, and disease events that would be manageable under other governance conditions become more severe because fear-maintained populations have lower investment in collective health infrastructure.

The decline is not immediate and not guaranteed at Cruel conviction — it is a characteristic of extreme Cruel posture maintained over time without counter-investment in population welfare. A Cruel dynasty that maintains its military and extraction efficiency while also investing in food, water, and housing infrastructure can offset the decline. The investment cost to offset Apex Cruel's population decline is high enough to make it a genuine strategic tension.

---

## Section 31: Conviction and Other Systems

### Conviction and Faith Independence

Conviction and faith interact without determining each other. A dynasty with Moral conviction following Blood Dominion dark doctrine will find that certain actions that the doctrine encourages — formal covenant exchanges, sacrifice rituals, covenant bindings — carry conviction costs that the doctrine does not cancel. The conviction system registers what the action actually is. The doctrine provides theological framing. The two systems respond independently.

The interaction creates genuinely different civilizational characters. A Blood Dominion dark doctrine dynasty with Moral conviction is conducting a theology of cost-bearing with genuine internal moral standards — they perform the Bindings, they pay the prices, and they experience the conviction weight of each one as real. A Blood Dominion dark doctrine dynasty with Cruel conviction has abandoned the internal moral accounting. Both are following the same doctrine. They are doing it from different positions on the conviction axis.

### Conviction and Diplomacy

Other dynasties respond to conviction posture in their diplomatic calculations. Moral dynasties receive more favorable initial terms from similarly-positioned dynasties. Cruel dynasties are treated with wariness and calculation rather than trust. Alliance agreements with Cruel dynasties typically include enforcement mechanisms that Moral dynasties do not require, because the counterparty has calculated that the Cruel dynasty is more likely to defect when defection becomes advantageous.

Neutral conviction is the diplomatic baseline — no positive or negative diplomatic modifier from conviction posture alone. Neutral dynasties are treated as uncommitted, which is neither an advantage nor a disadvantage in most diplomatic contexts.

### Conviction and Succession

Highly Cruel dynasties have more unstable successions. The fear-maintained loyalty structures that function during the reign of an established Cruel leader do not automatically transfer to an untested successor. The population that complied because it feared the specific person who just died must recalculate whether it fears the successor — and during that recalculation window, revolt events are more likely to fire.

Moral dynasties' succession events are smoother — not easy, but smoother. The loyalty to the dynasty's record rather than to the specific individual means the record transfers more gracefully. A Moral dynasty that loses a beloved leader still faces a succession challenge, but the loyalty base that exists provides more time for the successor to establish themselves before revolt becomes probable.

### Conviction and the Trueborn City

Moral dynasties receive trade preference from the Trueborn City. The city's trade network is built on relationships of trust and sustained mutual benefit, and it allocates its most favorable terms to dynasties whose governance record suggests they will maintain those relationships. Moral dynasties access more of the city's intelligence network, receive better commodity pricing, and find the city's diplomatic channels more open.

Cruel dynasties face a progressively more hostile Trueborn City response as Cruel conviction approaches the extreme. At a defined threshold, the city begins diplomatic pressure. Beyond the threshold, the Coalition Call option becomes available. A Cruel dynasty that conquers the Trueborn City before this mechanism fires seizes the city's resources but loses the intelligence and diplomatic functions that the city's neutrality provided — the city's trade network requires the city's neutrality guarantee to function. Conquering the city destroys some of what made it valuable. The Sovereignty Anchor persists. The information network does not.

---

# PART VI — MILITARY SYSTEMS

---

## Section 32: The Squad System — Core Mechanics and Visual Identity

### One Selection, Five Individuals

The squad is the fundamental unit of military management in Bloodlines. A single player selection clicks not one soldier but five. These five share a health pool — the squad's total health is tracked as an aggregate, and individual soldiers within the squad are not individually selectable. When the squad has taken enough damage, a soldier falls. The squad degrades visually — from five, to four, to three, to two, to one. The final survivor fights at a different quality than the squad fought at full strength.

This system produces military engagements where numbers are legible. A player can look at a squad and know its current health from its visual composition. A squad of five is fresh. A squad of two has been in a bad fight. A single survivor carrying the squad's history into the next engagement reads immediately as a unit in crisis.

The shared health pool is not split equally across soldiers for visual purposes. The degradation order within the squad reflects the engagement — the soldiers at the exposed edge fall first. A squad fighting in formation loses from the front. A squad that was flanked loses from the side. The visual storytelling of squad degradation is directionally honest.

### Squad History and Veteran Status

Squads are tracked. Their match history accumulates. A squad that has fought in multiple engagements, been rebuilt from survivors, and carried veterans into new campaigns has a history that the game registers. Veteran squads have visual distinctions from freshly recruited squads — worn equipment, different bearing, the specific character that extended service produces.

The veteran distinction is not only aesthetic. Veteran squads perform at a level that reflects their history — higher morale baseline, faster recovery from rout conditions, slightly better coordination when fighting alongside known allied squads. The difference is not so large that veteran squads are dramatically superior to fresh squads in comparable equipment tiers. It is large enough that the distinction matters in close engagements.

The house-specific squad trait modifies the base squad system in a direction consistent with each house's strategic identity. Ironmark squads have slightly larger effective health pools at equivalent population cost. Hartvale squads rebuild from survivors faster after an engagement. Westland squads move faster when not in formation. Stonehelm squads hold formation against charges better than any other house. The modifications are subtle but consistent with each house's designed character.

---

## Section 33: Level 1 and Level 2 Units — Survival Era and Iron Transition

### Level 1 — The Survival Era

Level 1 units represent the military reality of Year 80 of the Reclamation: armies built from what the survival era produced. Equipment is functional but not uniform. Tactics are effective but not refined. The soldiers are competent fighters who have been fighting most of their lives, which is not the same as trained soldiers who have been fighting in an organized military tradition.

**Militia:** The foundational unit. Minimum equipment — agricultural tool adapted for combat, or basic club and shield. 5/5 offense/defense at the neutral baseline, modified by house. Fast to recruit, low cost, low effectiveness per individual. Value is in numbers and in absorbing damage that more expensive units cannot afford to take.

**Swordsmen:** The first dedicated military unit. Iron sword, basic iron shield. 5/5 neutral baseline, modified by house. More effective than Militia in direct combat, more expensive, slower to recruit. The backbone of early game direct engagements.

**Spearmen:** Anti-cavalry specialist. Long spear braced against charge. 5/5 neutral baseline with the specific capability of significantly reducing cavalry attack effectiveness when properly formed. The defensive answer to early cavalry threats.

**Hunters:** Ranged scouts adapted for combat. Light bow, minimal equipment. 5/5 neutral baseline with extended vision range relative to other Level 1 units. The early game's primary scouting and harassment unit. Less effective in direct combat than Swordsmen. High movement speed relative to infantry.

**Bowmen:** Dedicated ranged unit. Better bow than Hunters, slightly better armor. 5/5 neutral baseline. Positioned behind formations to deal ranged damage. Fragile if exposed to direct engagement. The primary damage source in engagements where the dynasty controls the terms.

### Level 2 — Iron Transition

Level 2 units are the military expression of the iron era's early development. Equipment has standardized. Iron is more available than at match start. The armies look different — more uniform, better equipped, more recognizable as a professional military rather than an armed population.

Nine confirmed Level 2 units span the transition period, representing the diversification of military roles as iron infrastructure matures. The units at this tier remain universal — no faith specialization yet. House stat deviations persist at Level 2, becoming more pronounced as the specific character of each house's military tradition expresses itself through better equipment and more developed training.

The Level 2 tier is the last tier where all dynasties on all faith paths field identical unit types. Beginning at Level 3, faith begins to differentiate the military. The Level 2 window is therefore the period of maximum tactical flexibility — any formation works, any doctrine is available, any tactical approach can be tested without commitment.

---

## Section 34: Cavalry System — Infrastructure, Bear Riders, and Tactical Role

### The Horse Cavalry Requirement

War horses require two infrastructure investments that must be developed before cavalry is accessible: iron production sufficient to supply mounted equipment, and animal husbandry infrastructure that specifically supports military horse development. A dynasty that has neither cannot field horse cavalry regardless of how much gold it has accumulated. The two requirements reflect the genuine logistical reality of mounted warfare — horses are expensive to maintain, require specialized care, and need equipment that does not exist without iron production.

This requirement creates a meaningful mid-game decision. The player who develops cavalry infrastructure early gains a significant tactical advantage in the period when opponents have not yet responded. The player who delays cavalry development in favor of economic or faith infrastructure faces a window of mounted vulnerability. The timing of cavalry development is a strategic choice, not an automatic progression.

Cavalry effectiveness is a major mid-to-late game tactical factor. In open terrain — Reclaimed Plains, River Valley floors, Badlands — properly employed cavalry determines engagements. The ability to flank, to pursue routed infantry, to threaten supply lines, and to concentrate force faster than infantry can respond makes cavalry an army multiplier rather than merely an additional unit type.

### Cavalry Counter-Play

Spearmen in formation are the primary counter to cavalry charges. A properly braced Spearman formation reduces cavalry charge damage significantly and prevents the breakthrough that cavalry charges are designed to produce. Tight terrain — Ancient Forest, Stone Highlands, Frost Ruins — neutralizes cavalry advantage by removing the space in which cavalry operates effectively.

The relationship between cavalry and anti-cavalry infrastructure is one of the fundamental tactical dynamics of the mid-game. Dynasties that develop cavalry early force opponents to develop Spearman formations earlier than they might prefer. Dynasties that invest deeply in Spearman counter-play create windows for more expensive unit types to operate without cavalry threat.

### The Wild Alternative — Bear Riders

Wild covenant dynasties on the dark doctrine path access Bear Rider cavalry units that require no iron or horse infrastructure. Bears do not need the same maintenance infrastructure that horses require, and they are domesticated through Wild covenant practices rather than through the conventional animal husbandry building chain.

Bear Rider cavalry is slower than horse cavalry in open terrain but significantly more powerful per unit in direct combat — a war bear engaging infantry is a different kind of engagement than a cavalry horse engaging infantry. Bear Riders also operate in terrain that neutralizes horse cavalry. A Bear Rider can move through Ancient Forest at reduced penalty, making them useful in exactly the terrain where conventional cavalry cannot go.

The tradeoff is access path. Bear Riders require Wild covenant dark doctrine development, which means the dynasty has committed to a specific faith and a specific doctrine path. Horse cavalry is accessible across all covenant paths to any dynasty with the infrastructure. Bear Riders are accessible only to Wild dark path players. The exclusivity is a design feature, not a limitation — it gives the Wild dark path a military option that makes it distinct.

---

## Section 35: Born of Sacrifice — The Population-Constrained Army Lifecycle

### What Born of Sacrifice Is

Born of Sacrifice is the name for the system that governs the lifecycle of military units — not their creation and death in the individual sense, but the institutional relationship between the army and the population that produces it. The name reflects the covenant recognition across all four faiths that military service is a form of sacrifice — not always literal, not always fatal, but always a drawing from the population's store of human capacity.

Born of Sacrifice is not a one-time ritual that produces a superunit. It is a structural system governing how armies are built, maintained, and periodically returned to the population pool from which they came.

### The Population-Army Relationship

All military units are recruited from the population pool. Each unit recruited represents population that is no longer available for economic production. The population pool is unified — the worker in the field and the soldier in the army draw from the same pool of people. A dynasty that maintains a large standing army for an extended period is paying a continuous economic cost in reduced worker availability.

This relationship creates a genuine strategic tension between military strength and economic health that cannot be resolved by simply accumulating enough gold. A dynasty that fields an army large enough to win any engagement it chooses will eventually face population pressure — fewer workers means slower resource extraction, slower economic growth, and slower population recovery. The army that wins every battle but exhausts the population producing it is building toward a different kind of failure.

### Veteran Army Recycling

The veteran army recycling component of Born of Sacrifice is the system's most unusual mechanic. Veteran armies — squads that have accumulated significant battle history and veteran status — are periodically recycled back into the population pool. The player does not lose the veterans permanently. The veterans return to the population as experienced adults with military history, where they contribute to population quality metrics and, critically, to the next generation of military recruitment.

When the recycled veterans' children reach military age, they enter the recruitment pool not as ordinary recruits but as individuals with a specific military inheritance. The recruit drawn from a veteran's family knows more than a recruit drawn from a civilian family. Their baseline attributes are modestly higher. Their morale baseline is more stable. Their faith alignment is more developed if their veteran parent had faith investment.

The recycling system means that a dynasty's military quality compounds across generations through the population in a way that direct unit training alone cannot produce. The army trained in Year 100 fights at a baseline level. The army trained in Year 200 from a population that includes multiple generations of veteran recycling fights at a meaningfully higher baseline — not through equipment or magic, but through the accumulated institutional depth of a society that has been producing competent soldiers for generations.

### Elite Quality Through Institutional Depth

The Born of Sacrifice system's ultimate output is institutional depth — a civilization that has been producing and cycling military capability through its population for long enough that military competence is distributed through the society rather than concentrated in a standing professional army.

A dynasty that understands and works with this system does not try to keep every veteran in service until they die. It cycles them deliberately, returns them to the population at the right time, and manages the generational compound across the full match. The outcome is an army that is qualitatively superior to what any equal-resource dynasty without this practice can field — not through individual unit power, but through the aggregate quality of a population that has been shaped by cycles of military service and civilian integration.

---

## Section 36: Siege and Naval Warfare

### Naval Vessels — Six Types

Naval capability is unlocked through harbor infrastructure and scales with harbor tier. Six vessel types cover the full range of naval military and economic function.

**Fishing Vessel:** Not a military unit. Produces food through offshore fishing operations. Accessible at the first harbor tier. Valuable in coastal economies where land-based food production is limited.

**Scout:** Light naval vessel with extended vision range and high speed. Not combat-capable against larger vessels. Essential for coastal map knowledge and early naval intelligence. Accessible at first harbor tier.

**War Galley:** The primary combat vessel of the match's mid-game naval period. Oar-propelled, crewed, armed with naval weapons. Engages other War Galleys and smaller vessels. Cannot threaten a Capital Ship in direct combat but is significantly cheaper and faster to replace. The workhorse of naval conflict.

**Troop Transport:** A vessel designed for carrying infantry across water. No combat capability — it moves infantry, it does not fight. The enabling vessel for amphibious operations. Without Troop Transports, a naval power cannot project force onto land beyond coastal artillery range.

**Fire Ship:** A specialized offensive vessel — a War Galley equipped with incendiary weapons. Devastatingly effective against enemy harbor infrastructure and against dense naval formations where the incendiary effect propagates. Fragile. Single-use in the extreme — a Fire Ship committed to a harbor raid is not retrievable. The tactical gamble that changes a naval engagement.

**Capital Ship:** The largest naval vessel. Expensive, slow, powerful. The Capital Ship is the anchor of a naval battle formation — the most durable vessel available, capable of sustaining and inflicting damage that no War Galley can match. Requires the highest harbor tier to build. One or two Capital Ships in a naval force changes the tactical calculus for any opponent.

### Harbor Tiers and Infrastructure

Harbor development follows a three-tier progression parallel to the broader building system. First-tier harbors enable basic fishing and Scout production. Second-tier harbors enable War Galley and Troop Transport production. Third-tier grand harbors enable Capital Ship production and unlock the full naval tactical suite. Harbor construction requires coastal terrain — inland dynasties with no coastal access cannot develop naval capability without territorial expansion.

### Amphibious Operations

Troop Transport-enabled amphibious operations allow a coastal dynasty to project force against any territory accessible from the sea, regardless of land route availability. Amphibious assault has a specific mechanic — landing infantry on a defended shore is costly, and units in transit on Troop Transports are vulnerable to naval interdiction. A dynasty attempting an amphibious assault on a prepared coastal defense is paying a significant attacker's penalty. A dynasty landing troops on an undefended coast faces no such penalty.

The strategic value of amphibious capability is the threat itself — a dynasty with Troop Transport capacity forces coastal opponents to garrison coastal provinces that a pure land power could leave undefended.

### Ironmark Axemen and Siege

Ironmark's Axeman unit has specific effectiveness against fortifications that no other Level 1-2 unit can match. The two-handed axe weapon that makes Axemen powerful against armored infantry translates directly to effectiveness against wooden and early-stone fortification. An Ironmark force with strong Axeman composition can breach first-tier fortifications faster than any other founding house at equivalent unit cost.

Siege warfare broadly requires either dedicated siege equipment — available in the mid-game as a separate infrastructure investment — or unit types with specific fortification-breaking capabilities. A force without siege capability can blockade a fortress indefinitely but cannot breach it without incurring catastrophic assault casualties.

---

## Section 37: The Bloodline Member in Combat

### The Hero-Equivalent Anchor

Bloodline Members are not standard military units. They are the player's representatives in the world — named individuals with histories, relationships, and developmental arcs. When a Bloodline Member enters combat, the engagement changes in ways that reflect both their specific capabilities and the weight of their presence.

A War General committed to a military role provides a command anchor that affects every unit in their operational radius. The mechanics are documented in the Role system. The phenomenology matters too: a Bloodline Member in combat is the player's skin in the game. Their death is not a unit loss. It is a named person dying — someone who had children, a political position, relationships with other members. The match history registers their death.

### War Commitment and Field Presence

The War Commitment role has a specific field presence mechanic. A War General on the battlefield is not just a buffed unit among units. Their presence raises army morale in a radius, reduces rout risk for committed units, and enables the Vanguard Surge ability. But their presence also signals to the opponent — a matching AI or player who recognizes a Bloodline Member in the field knows that the army is operating at a higher commitment level and that destroying the member destroys the army's coherence.

Other Bloodline Member roles have combat implications even when not directly committed to the War role. A Territory Governor present in a province during a defensive engagement provides stability bonuses. A Faith Leader who moves with an army provides faith intensity during the campaign period.

### Narrative Weight — Presence and Death

The design intention is that a Bloodline Member's presence in a battle should feel different from a standard unit's presence. The Keep's visual record shows them as a person with a room, a family, a face. When they move onto the battlefield, they carry that weight with them.

Their death is registered differently. A standard unit squad degrading to zero is a military event. A Bloodline Member dying is a family event — the Keep records the death, the dynasty's other members respond, succession mechanics may trigger, foreign parties recalibrate their assumptions about the dynasty's leadership. The match continues, but something has changed that cannot be changed back.

Enemies who track Bloodline Member positions know this. An AI opponent that identifies a Bloodline Member in the field and successfully kills them has inflicted a wound that is not purely military. Players who do not understand this play without protecting their members. Players who understand it play differently.

---

## Section 38: Army Composition and the Recruitment Slider System

### Sliders, Not Locks

Army composition in Bloodlines is controlled through an adjustable slider system rather than a fixed doctrine selection. The player can move sliders to determine the proportional composition of recruited forces — how much of the next recruitment cycle goes to infantry versus cavalry versus siege versus faith units versus naval units. No position on any slider is locked. Any dynasty can recruit any unit type in the tech tree that it has unlocked.

The sliders express the player's current priorities. A dynasty preparing for a siege campaign moves sliders toward heavy infantry and siege equipment. A dynasty preparing for a naval expansion moves sliders toward harbor-produced vessels. A dynasty that has just committed to a cavalry strategy moves sliders toward mounted units while maintaining a Spearman counter in reserve.

Faith, Conviction, and Bloodline identity all influence which slider positions are most efficient. A Wild covenant dynasty with high Ancient Forest control finds that Thornrider and Forest Sovereign sliders produce units faster and at lower cost than equivalent cavalry sliders for horse cavalry. A Blood Dominion dark dynasty finds that Bloodbound sliders produce units with specific capabilities that no other slider position accesses.

The slider system does not prevent strategic adaptation. It does not force a dynasty to commit to a single military identity. What it does is reflect the reality that military infrastructure specialization exists — a dynasty that has invested deeply in harbor infrastructure produces naval units more efficiently than one that has not, regardless of where the slider is pointed.

### Composite Strategies and Counter-Play

The slider system enables composite army compositions that pure doctrine systems cannot produce. A dynasty can maintain a Spearman-heavy defensive component while developing cavalry offense while also investing in faith units that amplify the entire force. These compositions require more management than single-doctrine armies but produce greater tactical flexibility.

Counter-play in army composition is straightforward in principle and complex in practice: observe what the opponent is developing, develop what counters it, and maintain enough composition flexibility to respond when the opponent adapts. A dynasty that has locked itself into a single composition is easier to counter than a dynasty that maintains slider flexibility.

---

## Section 39: Military Events — Key Event System Components

### How Military Events Work

Military events are triggered by specific match conditions — territorial control, conviction levels, faith intensity, bloodline member actions, and inter-dynasty relationships. They fire through the same event system as political and economic events, but they have direct military outputs: new unit types become available, specific engagements become mandatory, coalitions form, and supply chain disruptions require military response.

Key military-trigger events from the canonical event system:

**The Iron Rush:** Fires when any dynasty achieves full Iron Ridge control in a zone while another dynasty is contesting the same zone. Produces a mandatory military engagement window — diplomacy is suspended for the zone until the military contest resolves.

**The Warband Call:** Fires when a Nomadic Warband tribe reaches radicalization against a specific founding house. The Warband begins approaching opposing houses with military cooperation offers. Houses that accept gain a temporary cavalry and harassment force supplement. The originating house faces a guerrilla threat that formal military engagement cannot fully resolve.

**The Holy War Mobilization:** Fires when a Holy War is declared. All same-covenant dynasties receive a mobilization option — joining the Holy War generates faith intensity bonuses but also commits military resources to a campaign defined by the declaring dynasty's objectives.

**The Great Reckoning:** Fires when an Apex Cruel dynasty has triggered the Trueborn Coalition Call and at least three other dynasties have accepted. The Great Reckoning is the match's largest potential coalition engagement — multiple dynasties coordinating military action against the perpetrator. The Apex Cruel dynasty faces simultaneous threats on multiple fronts. Surviving the Great Reckoning without losing the Sovereignty Seat pursuit is among the most difficult match challenges.

**The Siege of Memory:** Fires when a dynasty with Fervent Old Light faith attacks a Frost Ruin zone held by an opposing dynasty. The event generates a specific engagement with additional defensive bonuses for the defending Old Light faith units in the ruin terrain.

---

# PART VII — ECONOMIC SYSTEMS

---

## Section 40: The Six Resources — Role, Production, and Strategic Importance

### The Resource Architecture

Six resources govern everything the dynasty builds, trains, recruits, and sustains. No system in the game operates without resource cost. The resources are not equal — they do not all matter at the same time, and their relative importance shifts across the match's stages. Understanding which resources matter when, and building to have those resources available when they are needed, is the economic game within the broader strategic game.

### Gold

Gold is the universal exchange medium. It is the only resource that can substitute for any other in trade transactions. It is required for mercenary recruitment, diplomatic agreements, alliance maintenance, tribute payments, and trade deals with the Trueborn City. Gold is not the most important resource at any specific match stage — but it is always useful and never useless. A dynasty with abundant gold and resource constraints can trade for what it needs. A dynasty with abundant resources and no gold cannot make its diplomatic relationships work.

Gold is produced through trade routes, market infrastructure, Goldgrave's house-specific bonuses, and the Coin Sergeant's mercenary coordination effects. Mining gold is possible but not the primary production method — the match's gold economy is a trade economy rather than an extraction economy. The dynasty that controls trade routes produces more gold than the dynasty that controls gold deposits.

### Food

Food is the foundational population resource. Population cannot grow without sufficient food. Population stability degrades with food scarcity. Military campaigns that outrun their food supply chains degrade in effectiveness. Food is the most immediately consequential resource in the early game and remains significant throughout the match.

Food is produced by agricultural buildings on Reclaimed Plains, River Valley farming, Hartvale's Verdant Warden bonuses, and coastal fishing operations. Food production scales with the land being worked and the infrastructure supporting it. A dynasty controlling large Reclaimed Plains with developed agricultural infrastructure produces food that no other terrain type can match.

Food scarcity events — droughts, hard winters, crop failures — are among the match's highest-impact events. A dynasty with no food surplus when a famine event fires faces population loss that takes years to recover. Food reserves are the most consequential strategic buffer a dynasty can maintain.

### Water

Water is the second foundational population resource. Population growth requires both food and water in sufficient supply. Water scarcity produces a harder constraint than food scarcity in the short term — humans can survive longer without food than without water, but in the match's abstracted economy, water deprivation produces immediate and severe population stress.

Water is produced by wells in settled areas, river access in River Valley terrain, and water infrastructure buildings that expand the accessible water supply. Coastal access provides salt water that requires desalination infrastructure to convert to population supply. The water system creates a specific early-game priority: before agriculture can expand, water access must be established.

The Housing system that Water supports is the game's replacement for conventional game mechanics around population and production capacity. Where other RTS games use power plants, Bloodlines uses housing — population in households, each household consuming food and water to sustain itself and contribute its labor to the economy and its soldiers to the military.

### Wood

Wood is the primary building material of the early match. Every first and second tier building requires wood in significant quantity. Wood's importance decreases as iron and stone infrastructure matures and replace wood as the dominant building material. In the late match, wood is primarily used for naval construction and for specific special structures. In the early match, a dynasty without reliable wood access cannot build the infrastructure that everything else requires.

Wood is produced from forest terrain. Ancient Forest provides the highest sustained yield. Reclaimed Plains with scattered woodland provides modest yield. Stone Highlands and Iron Ridges provide almost none. A dynasty starting in terrain-poor-for-wood regions must either establish trade for wood or quickly expand into forested territory.

### Stone

Stone is the material of permanent infrastructure. Fortifications, higher-tier buildings, and grand structures all require stone in significant quantity. Stone's importance scales with the match's progression — early game, stone is less critical than wood; mid-to-late game, every major strategic investment requires stone.

Stone is produced from Stone Highlands quarrying, Frost Ruins salvage, and dedicated quarry buildings on appropriate terrain. Iron Ridges also yield moderate stone as a secondary resource. Stonehelm's house bonuses specifically enhance stone production and reduce stone consumption in fortification construction.

### Iron

Iron is the critical mid-to-late game resource and the material that the Great Frost made available. Every cavalry unit requires iron. Higher-tier military units require iron equipment. Grand structures require iron components. Naval vessels in the War Galley tier and above require iron.

Iron is produced by Iron Mines — a specific building type that must be constructed on Iron Ridge terrain or, in reduced efficiency, on other terrain types with iron deposits. The Iron Mine building is not available without the iron-era infrastructure investment. Before the iron era develops, the dynasty operates with what was salvaged and what was traded. When the iron era matures, the dynasty that has secured the most Iron Ridge territory has the strongest foundation for the mid-to-late game.

Iron is not infinitely available. Iron deposits can be exhausted if mined at maximum rate for long enough — a mechanic that creates genuine long-term resource pressure. A dynasty that burns through its iron deposits in the mid-game faces a late game where iron must be obtained through trade or conquest of new deposits.

---

## Section 41: Population as Resource — The Unified Pool and the 90-Second Cycle

### One Pool, Everything Draws From It

Population is not a separate system from the military or the economy. It is the single pool from which all labor and all soldiers are drawn. A dynasty with 500 people in its households has 500 people. How those 500 people are allocated between farming, mining, building, and soldiering is the dynasty's most fundamental economic decision. Putting more into the army means fewer workers. Putting more into the fields means fewer soldiers. The constraint is real and does not yield to optimization tricks.

Starting population is approximately 80 households per founding house at Year 80 of the Reclamation. Each household represents a family unit — adults and children treated as a single economic entity for production purposes. As the match progresses, the dynasty that has managed food, water, and housing infrastructure effectively will see its household count grow. Scaling targets for a mature match are 3,000 to 4,000 households per founding house — a 40x increase over the match's arc.

The population growth rate is governed by a 90-second game cycle — the base rhythm at which the population counter updates and events are assessed. Every 90 seconds, food and water consumption is calculated against production, births and deaths are processed, migration events are assessed, and the net population change is applied. The 90-second cycle is the heartbeat of the economic game.

### Housing — The Population Capacity System

Population cannot exceed housing capacity. Housing buildings establish the upper limit of the population that can be stably sustained in a given territory. Building housing ahead of population growth is an investment in capacity. Allowing population to grow into existing housing maximizes efficiency until housing becomes the constraint — at which point growth stops until additional housing is constructed.

The housing system replaces the power plant mechanic from Command and Conquer's framework. Where C&C uses energy production as the constraint on building and unit production, Bloodlines uses housing as the constraint on population — and through population, on everything that population produces and constitutes. Building housing is investing in the dynasty's future capacity across every system.

### Population Quality and Composition

Population is not homogeneous. The quality of a population pool affects the quality of what it produces. A population with significant veteran military history in its composition produces better-quality military recruits than a population without that history. A population with deep faith practice integrated into daily life generates passive faith intensity. A population that has lived under Moral governance for extended periods has a different baseline loyalty than a population that has lived under Cruel governance.

Population composition also affects economic output in specific ways. A population with significant Iron Worker tribe integration produces iron more efficiently. A population with Trueborn City trade network exposure has more developed commercial instincts that modestly improve gold production from trade routes. Population quality is not a visible stat — it is a composite that emerges from the match's history and expresses itself through every system that draws from the population pool.

---

## Section 42: Economic Buildings and the Tech Tree

### Building Categories

Economic buildings fall into five categories that collectively cover the full range of the dynasty's development: Civic, Economic, Military, Faith, and Special. Each category has its own progression logic. Buildings within a category typically form a prerequisite chain — you cannot build a Hall of Mandate without a Lawpost, you cannot build a Covenant Hall without a Blood-Altar. The prerequisite chain ensures that tech tree progression is orderly and that players invest in foundations before superstructures.

**Civic buildings** establish population and governance infrastructure: houses, wells, granaries, administrative halls, courts. These are the buildings that sustain the population and maintain territorial stability. They produce no military or economic output directly but are the prerequisite for population growth and governance function.

**Economic buildings** produce resources or amplify resource production: mines, farms, markets, trade posts, caravan depots, ports. These are the buildings that generate the resources that everything else requires. Economic building investment is the highest-priority development choice in the early and mid-game.

**Military buildings** produce and support military units: barracks, stables, archery ranges, forge structures for Ironmark, specific faction-appropriate production facilities. Military building investment is the prerequisite for accessing higher-tier units. Faith-specific military units require both military buildings and faith buildings.

**Faith buildings** are documented in the Faith System sections. They generate faith intensity, enable rituals, and unlock faith-specific units and events.

**Special buildings** are unique structures with specific match functions: the Grand Trade Exchange, the Grand War Foundry, the Trueborn Embassy, the Observatory. Special buildings typically have one-of-a-kind effects and significant resource costs.

### The Grand Structure Lockout System

Four pairs of grand structures cannot coexist within the same dynasty. Building one grand structure locks out its opposing pair. This is not a flavor mechanic — it is a fundamental strategic commitment.

**The Grand Trade Exchange locks out war citadels.** A dynasty that commits to the Grand Trade Exchange has declared itself an economic power and sacrificed the ability to develop the highest-tier military fortification infrastructure.

**The Grand War Foundry locks out trade structures at the highest tier.** A dynasty that commits to the Grand War Foundry has declared itself a military power and sacrificed the ability to develop the highest-tier trade economy.

**The Eternal Cathedral / The Wound / The Iron Spire / The First Grove locks out competing grand faith structures.** Only one covenant's grand faith structure can exist in a dynasty's territory. Faith is exclusive at the grand tier.

**The Grand Population Archive locks out the Grand Forced Labor Camp.** The population systems at the highest tier commit in one direction or the other — deep welfare investment or maximum extraction efficiency.

The lockout system forces real choices. A dynasty cannot be everything simultaneously. The grand structures express what the dynasty has decided to be.

---

## Section 43: Trade, Caravans, and the Trueborn City Economy

### Trade Agreement Mechanics

Trade agreements between dynasties are formal contracts with specific terms: which resources are exchanged, at what ratio, for how long, and what happens when terms are violated. Trade agreements are negotiated through the diplomatic interface, typically with a Trade Anchor committed member present for optimal terms. Unsigned agreements at baseline terms are possible but represent an information disadvantage for the player who accepts them without negotiation.

Active trade agreements generate ongoing resource flows that supplement production. A dynasty with three active trade agreements for resources it produces in surplus can fund infrastructure investment in resources it needs without dedicating territory to producing them. The trade network is a genuine alternative to self-sufficiency, and the Goldgrave house's entire strategic identity is built around leveraging that alternative more efficiently than anyone else.

Trade agreement violations are tracked by the conviction system. Breaking a trade agreement when continued compliance was possible generates negative conviction. The diplomatic system registers the violation with every dynasty that has active relationships with both parties — word travels through the Trueborn City trade network.

### Caravan System

Trade agreements generate trade flow, but that flow travels through caravans — actual physical convoys moving across the map with actual vulnerability to interdiction. Caravans are not theoretical resource transfers. They are visible, stoppable, and valuable.

A Trade Anchor committed member can optimize caravan routes, security, and negotiation at waypoints. Caravans moving without a Trade Anchor presence follow standard routes at standard efficiency. Caravans moving with a Trade Anchor presence find better routes, negotiate better terms at waypoints, and are more likely to survive interdiction attempts.

Caravan interdiction — attacking or capturing an opponent's caravan — is a military economic action with conviction cost. It is a standard strategic option for militarily aggressive dynasties and a persistent threat for economically focused dynasties. Protecting caravan routes requires either military escort investment or diplomatic arrangements with powers whose territory the caravans cross.

### The Trueborn City as Economic Hub

The Trueborn City functions as the match's highest-volume trade node. Dynasties with active trade relationships with the city access commodity markets that exceed what any bilateral agreement can provide. The city's merchant network connects every faction that trades with it, which means a dynasty with strong Trueborn City relationships can access resources produced anywhere in the match world that another city-connected dynasty is willing to sell.

Goldgrave's advantage in this space is structural. Their entire house identity is built around economic leverage, and the Trueborn City's merchant network is where economic leverage expresses itself most fully. A mature Goldgrave dynasty with strong Trueborn City relationships is operating in a different economy than the one other houses see — they are not producing all their own resources, they are purchasing them efficiently through a trade network that they have specifically invested in.

---

## Section 44: Economic Victory Path — Currency Dominance

### The Mechanism

Currency Dominance is the economic victory path. The objective is to get the world to adopt the dynasty's currency standard — not as a single dramatic event but as a gradual process of currency network capture that reaches a threshold where the network effects become self-reinforcing.

The currency standard develops through trade. A dynasty conducting enough trade volume in its own currency, across enough dynasties and minor tribes, gradually makes its currency the most useful medium of exchange in the match world. When enough parties have accepted it as their primary trading medium, the dynasty holding the standard gains access to the Currency Dominance mechanics.

### What Currency Dominance Enables

At the dominance threshold, the controlling dynasty can purchase enemy armies' loyalty — not by bribing individual soldiers but by leveraging the economic dependency that the currency standard has created. Armies that are sustained by economic systems dependent on the dominant currency find that their financial infrastructure is exposed to pressure that the currency-controlling dynasty can apply.

The dynasty can withhold currency access — effectively cutting off a rival's access to the trade network — which produces economic starvation in dynasties that have become dependent on trade for resources they no longer produce independently.

The most powerful expression of Currency Dominance is forcing kingdoms into economic dependency — a situation where the dependent kingdom cannot sustain its military or population at current levels without continued access to the dominant currency's trade network. A kingdom in dependency cannot easily choose war with the currency-controlling dynasty without collapsing its own economy first.

### Counter-Play

Currency Dominance is not invulnerable. The counter-play is the Resolution Battle — a mechanic that triggers when any dynasty determines that the Currency Dominance threshold has been reached and chooses to declare a military challenge rather than accept dependency. The Resolution Battle initiates a formal military engagement against the currency-controlling dynasty with the objective of breaking the economic network through military force.

If the Resolution Battle is won by the challenger, the currency standard is disrupted and the Dominance threshold resets. If won by the currency controller, the Dominance victory path advances toward its resolution.

### Conviction Requirement

Currency Dominance requires Moral or Neutral conviction to function at full efficiency. The trade network trust that the currency standard depends on is produced by the dynasty's reputation as a reliable economic partner. A Cruel dynasty attempting Currency Dominance faces the specific problem that trade partners do not trust them to maintain the currency standard's terms without exploiting the dependency relationship in ways that damage the network. The trust deficit is not absolute — Cruel dynasties can pursue Currency Dominance — but they pay a friction cost that Moral and Neutral dynasties do not.

This is one of the clearest mechanical expressions of the conviction system's thesis: Moral and Cruel conviction are not good and evil paths but are differently efficient along different strategic lanes. Moral conviction is optimized for economic victory. Cruel conviction is optimized for fear-based compliance. A Cruel dynasty attempting an economic victory is working against its own efficiency optimization. The dynasty that understands this is playing with full information.

---

---

# PART VIII — MATCH STRUCTURE AND VICTORY

## Section 45: The Five Match Stages
*[REVISED 2026-04-07 — Supersedes the four-stage system. The four stages (Founding, Consolidation, Ideological Expansion, Irreversible Divergence) are replaced with five stages per Lance Fisher direction.]*

### Why Five Stages

A Bloodlines match is an arc, not a sequence of unlocks. The four-stage system previously documented captured the technological and faith progression of a match but did not fully represent the experiential and strategic shape of how a match actually develops: how a player moves from the intimate work of founding a dynasty in a specific place, to the expansive work of competing with other civilizations across an entire world, to the final convergence of everything built toward a single resolution. Five stages more accurately reflect that shape.

The stages are not governed by timers. They are governed by conditions — the state of the match, the player's position within it, and the structural changes that the game's systems produce as the match matures. A stage does not end on schedule. It ends when the conditions of the next stage have been met. Players in different matches, and different players within the same match, may progress through the stages at different rates depending on their strategic choices, their geography, and the behavior of their rivals.

Everything in a Bloodlines match is designed to feel large. The armies are large. The decisions are consequential. The world does not shrink to accommodate the player's preferences. Each stage is bigger than the one before it in scope, in consequence, and in the scale of what is at stake.

---

### STAGE ONE: FOUNDING

Stage One is the most personal stage in the match. The player is not yet competing with the world. They are building the foundation from which everything else will eventually reach outward. The match begins at the location of the head of the bloodline — the patriarch or matriarch whose choices will shape every generation that follows. This location is the dynasty's anchor. It will become, if properly developed, the most fortified and most personally significant territory the dynasty holds for the remainder of the match.

The primary work of Stage One is threefold: establishing the home base and the immediate surrounding territory, understanding the environment the dynasty has been placed in, and making first contact with the world that exists around it.

The home base is not arbitrary. Its placement reflects the geography of the map — the dynasty starts where the map says a house of this type would have established itself, and the player's job is to understand what that placement means. What terrain surrounds the starting position? Where is the nearest water? Where is the nearest iron deposit? Which direction does the coast lie? How far is it? What does the canopy of the nearby forest suggest about what lives in it? A player who spends Stage One understanding the geography of their starting position will make better decisions in every subsequent stage. A player who rushes past this understanding will pay for that gap later.

Expansion in Stage One is local. The dynasty is not reaching across continents or contesting major strategic objectives. It is establishing a defensible perimeter around its home settlement, extending to the nearest resource nodes, and beginning the population and infrastructure development that everything else requires. The primary base — the seat of the head of the bloodline — should be developed with seriousness. This is where the Keep will be built. This is the physical and political heart of the dynasty. The walls around it, the wells and granaries inside it, the faith marker that first signals what the dynasty is becoming — all of this begins here, and the investment made here in Stage One pays returns across every stage that follows.

Local tribes are the first external relationship the player must navigate. Every tribe in the starting area has an identity, a history, and a disposition that the player must read and respond to. Stage One is the window in which those first impressions are formed. A tribe that receives a respectful trade offer from a dynasty in Stage One has a different relationship with that dynasty than one that watches the same dynasty expand militarily through its territory without contact. These impressions persist. The game remembers them when those tribes reappear as military complications, diplomatic partners, or radicalized adversaries in later stages.

Faith is present in Stage One but has not yet demanded commitment. The player may build early faith infrastructure, may observe which covenants are present in nearby tribes and ruins, and may begin developing an intuition for which covenant fits this dynasty in this match. The formal faith commitment arrives at the boundary between Stage One and Stage Two, or in the early portion of Stage Two. It should not be rushed. A player who understands the tribal landscape, the terrain's faith affinities, and the neighboring dynasties' likely orientations before committing to a covenant is making a more informed decision than one who commits in the first minutes of play.

Stage One ends when the dynasty has a stable home base with defended borders, a resource infrastructure that exceeds immediate survival requirements, and enough knowledge of the surrounding environment to make the strategic decisions that Stage Two demands.

---

### STAGE TWO: EXPANSION AND IDENTITY

Stage Two is where the dynasty begins to become what it is going to be. The survival question has been answered. The geography is understood. The work now is expansion — pushing the dynasty's footprint outward from its home base — and identity formation: committing to the faith covenant, accumulating the early conviction weight that defines the dynasty's moral character, and developing the specific capabilities that the player's strategic direction requires.

Expansion in Stage Two is more ambitious than Stage One but still primarily within the home continent. The dynasty is pushing beyond its immediate starting area, contesting resource nodes that are farther away, establishing forward positions that extend its territorial reach, and beginning to encounter the tribes that lie beyond its initial neighborhood. These tribes are stronger and more established than the ones immediately surrounding the starting position. They have had time to develop their own internal structures, and approaching them requires more sophistication than the first-contact diplomacy of Stage One. How the player handles this stronger tribal opposition begins shaping the dynasty's conviction with real weight. A dynasty that responds to resistance with measured force and subsequent integration behaves differently from one that suppresses and extracts, and the conviction system registers the difference with Pattern Amplification applied from here forward.

The faith commitment arrives in Stage Two. This is one of the most consequential decisions in the match, and it is made with substantially more information than would have been available in Stage One. The player now knows which tribes are nearby and which covenants those tribes already follow. They know whether Sacred Ground exists in their expanding territory and which covenant it aligns with. They know, from the behavior of other dynasties glimpsed at the edges of explored territory, what faith orientations their rivals appear to be developing. All of this informs the choice. Faith is not merely a bonus selection. It is a commitment to a worldview that will shape what the dynasty builds, what populations will trust it, and what it will be permitted to do for the remainder of the match.

Economy development becomes serious work in Stage Two. Trade routes, not yet extended to other dynasties, begin connecting the dynasty's own territories to each other. Gold income begins to compound. Population growth is producing a labor pool substantial enough to support both military recruitment and continued economic expansion. Iron infrastructure, if the player has reached it, is transforming what the military can field. By the end of Stage Two, the dynasty's economic foundation should be producing a genuine surplus that funds the Stage Three buildup rather than merely sustaining current operations.

At the edges of Stage Two's expanded territory, the player begins making first contact with other dynasties. These contacts may be scouts encountered in contested wilderness, a rival house's forward settlement appearing at the map's explored edge, or a diplomatic communication arriving from a neighbor who has been watching the expansion. These contacts are not yet the sustained conflict of Stage Three — but they are signals. The player is no longer alone in the world. Other civilizations are developing, and the relative positions established now will shape the contest that follows.

Stage Two ends when the dynasty has committed to a faith covenant, expanded its territorial footprint meaningfully across its home continent geography, established the economic foundation that larger conflict will require, and made initial contact with the other major powers that will define the Stage Three competitive landscape.

---

### STAGE THREE: ENCOUNTER, ESTABLISHMENT, AND THE NEUTRAL CITY

Stage Three is the stage where the match stops being a personal development exercise and becomes an actual contest between civilizations. Other players are now encountered not as distant signals at the edge of explored territory but as real presences with armies, positions, and intentions that conflict with the player's own. The diplomatic landscape crystallizes from tentative first contact into defined relationships. The match's first major military engagements happen here.

Conflict in Stage Three may begin as border skirmishes over contested resource nodes, as rival scouting forces meeting in territory both dynasties intend to claim, or as a deliberate military initiative by a player or AI dynasty that has identified vulnerability in a rival's exposed position. These are not the war-defining campaigns of Stage Four — they are the engagements that establish who controls what at the moment when the map's contested middle territory is being sorted out. Forward bases appear in Stage Three: military and logistical positions in contested or newly claimed territory, designed to support operations at greater range from the home base. A dynasty without forward bases in Stage Three is fighting from its home position, which constrains both offensive range and defensive response time throughout Stage Four.

Trade routes extend beyond the dynasty's internal territories in Stage Three. Formal trade agreements with other dynasties, or with the Trueborn City, begin generating cross-dynasty economic relationships that carry diplomatic weight beyond their resource value. Army development in Stage Three is serious. The forces adequate for tribal engagements in Stages One and Two are now being evaluated against the organized armies of other founded dynasties, and that comparison reveals gaps that must be closed before Stage Four's major escalation.

The Trueborn City makes its active presence felt in Stage Three. As the match world expands and the political landscape becomes complex enough to require a neutral center of gravity, the city's function as diplomatic hub and information node becomes genuinely valuable. Dynasties that establish trade relationships with the city in Stage Three gain intelligence advantages that inform their Stage Four decisions. More importantly, Stage Three is the stage in which the city's fate becomes a strategic question the player must actively consider rather than background context. The player must now decide what the city represents to their victory path: a diplomatic resource to cultivate, a military objective to contest in Stage Four or Five, or a threat to monitor.

The city will not remain passive indefinitely. Every match produces one of two outcomes: either the city is contested and conquered during Stage Four or Stage Five by a dominant power willing to absorb the political cost of taking it, or the city remains unconquered and begins the transition from neutral institution to active historical claimant. If Stage Three concludes without meaningful engagement from the dominant powers, the Trueborn Rise Arc advances toward Stage Two activation, and what was a dormant institution in Stage One becomes an increasingly purposeful actor preparing its own bid for the Sovereignty Seat. Stage Three is the last stage in which engaging with the city can be characterized as deliberate strategic cultivation rather than emergency response to an escalating situation.

Stage Three ends when the major civilizations of the match have made significant military contact with each other, the political map has developed enough definition that alliance structures and rivalries are legible, trade networks are operating across dynasty lines, forward military positions are established, the Trueborn City has been meaningfully engaged or deliberately avoided as a strategic choice, and the stakes of the Stage Four contest are becoming clear.

---

### STAGE FOUR: WAR AND THE TURNING OF TIDES

Stage Four is where the match earns the weight it has been building toward. Everything accumulated across Stages One through Three — armies, faith intensity, conviction records, bloodline member development, economic networks, diplomatic relationships, territorial positions — is now deployed against opposition doing the same. Stage Four is the stage of major war. Not border skirmishes and contested resource nodes but sustained campaigns that determine which dynasties will be positioned to pursue a victory path and which will be fighting for survival.

The scale of military engagement in Stage Four is categorically larger than what preceded it. Armies are larger. The bloodline members commanding them are more developed and carry more strategic weight. The battles are fought over territories and objectives that have match-defining consequences. A dynasty that loses a critical Stage Four engagement does not simply cede a resource node — it cedes a position that shapes everything that follows in Stage Five.

Tides turn in Stage Four. A dynasty that entered the stage with apparent advantages may find those advantages challenged or reversed by sustained military pressure, by the political realignment of neutral parties, by the economic consequences of disrupted trade routes, or by the internal pressures that prolonged war places on bloodline stability and population loyalty. The converse is equally true: a dynasty that has been methodical and patient in the earlier stages may find that Stage Four is the moment when accumulated depth becomes decisive — when the veteran armies produced by cycles of Born of Sacrifice recycling prove more resilient than rivals who recruited quickly and fought constantly, when deep population loyalty built through Moral governance proves more stable than the fear-compliance that sustained a Cruel rival's rapid expansion.

Faith divergence in Stage Four produces fully visible military consequences. All units from Stage Four onward are faith-specific and irreversible in doctrine. Dynasties that committed to a covenant early and developed it consistently arrive at Stage Four with military forces that express that covenant's doctrine in every unit on the field. Dynasties that treated faith as secondary infrastructure arrive at Stage Four with a meaningful military disadvantage they cannot quickly close.

The Great Reckoning can fire in Stage Four. If any dynasty has reached 70% of total match territory, the structural immune response activates — every remaining dynasty is notified, the Trueborn City is authorized to mobilize coalition infrastructure, and the political conditions for a broad response against the dominant power are formalized. A dynasty that has advanced rapidly through military conquest in earlier stages may find Stage Four defined not by its own offensive operations but by its management of a coordinated response from every other power that remains.

The Trueborn City's fate is typically decided in Stage Four. A dominant dynasty that chooses to contest the city does so here, when its military capacity is at its highest development and the city's conquest produces the Sovereignty Anchor that accelerates every remaining victory path. A dominant dynasty that does not contest the city in Stage Four risks entering Stage Five against a Trueborn reemergence that has had the full developmental arc of the match to prepare its response. The city does not wait. If Stage Four concludes without meaningful challenge to it, the Trueborn Rise Arc advances into Stage Three activation — the point at which the historical bloodline is actively pursuing the Sovereignty Seat against the match's most dominant power.

Stage Four ends when the major military contests have produced a discernible shift in the match's power distribution — when it is clear which dynasties are positioned to pursue a victory path, which are fighting for survival, and what the Stage Five contest will actually look like.

---

### STAGE FIVE: FINAL CONVERGENCE

Stage Five is where the match accelerates toward its resolution. Everything becomes larger, more consequential, and faster in its impact. The match world is fully developed. Every system that has been building across Stages One through Four is now operating at maximum expression. The distance between the remaining competitive dynasties and the Sovereignty Seat is measured in the final decisions they make and the final battles they fight.

Faith commitment reaches its deepest expression in Stage Five. This is the stage of the final doctrinal decisions — not the initial covenant selection that happened in Stage Two, but the deepest commitments that determine which apex units become accessible, how the dynasty's faith intensity interacts with the political landscape of the late match, and whether the Faith Divine Right victory path has been developed sufficiently to pursue a declaration. A dynasty that has been at Apex faith intensity through Stage Four and enters Stage Five with the grand faith structure operational and the global faith share approaching the declaration threshold is making the decisive choice: declare now, or accept that this path will not resolve in this match. The spread window, once opened, is visible to every dynasty. The coalition response it triggers is the final test of whether a faith path was genuinely built or merely maintained as a contingency.

Conviction's accumulated record arrives in Stage Five as a fully formed civilizational identity that cannot be easily shifted. The moral record of every decision made across Stages One through Four is legible to the world, and the world responds accordingly in Stage Five's final negotiations, final population compliance windows, and final diplomatic arrangements. A dynasty at Apex Moral conviction entering Stage Five finds that the world's neutral populations, minor tribes, and unconsolidated territories respond to its claims with a different disposition than they bring to a dynasty at Apex Cruel. These differences are most consequential in Stage Five because Stage Five is when those populations must decide whether to accept or resist what is being asked of them.

High-end technological capability is concentrated in Stage Five. The apex faith units — one per covenant, one or two instances globally per match — become accessible for dynasties that have developed sufficiently. Capital Ships define the naval engagements of Stage Five. The most developed bloodline members carry their maximum capability into the engagements where that capability is most consequential. A Legendary bloodline member who has been present since Stage One and has accumulated the full depth of development across four prior stages is a qualitatively different force in Stage Five than anything the earlier stages could produce.

The Trueborn reemergence arrives in Stage Five in full force if the city has not been conquered or meaningfully neutralized. By Stage Five, if the Rise Arc has been allowed to advance without intervention, the Trueborn City is in Stage Three of its activation — fielding military forces, pursuing territorial reconquest of its ancestral administrative zones, using accumulated resources and historical legitimacy to challenge the match's most dominant power directly. A dynasty that ignored the city through Stages Three and Four now faces an opponent in Stage Five that cannot be ignored. The reemergence is not simply a military challenge. It is a legitimacy challenge that forces every dynasty to make a decision about where they stand relative to the city's historical claim, and those decisions further reshape the Stage Five landscape.

The final battles of Stage Five are the match's most consequential engagements. Military campaigns that determine whether a Faith Divine Right declaration survives its spread window. Naval engagements contesting secondary continent territory required to reach Military Conquest thresholds. Final coalition negotiations determining whether an Alliance Victory can be ratified. The currency dominance threshold that triggers the Resolution Battle. Every victory path reaches its decisive moment in Stage Five.

The match does not end quietly. Bloodlines is designed so that Stage Five produces the moments players will describe afterward — the named figures who fell, the alliances that held or broke, the declarations that succeeded or failed, the moment the world recognized a dynasty's sovereignty or refused to. Stage Five is where the story the match has been generating since the head of bloodline first stood on the ground of the starting position finds its conclusion.

---

### STAGE ARCHITECTURE AND THE PHASE FRAMEWORK

The five stages describe what is structurally available and what the match's conditions are at each point in its arc. Three phases overlay the five stages and describe what the player is primarily focused on doing: Phase One Emergence (Stage One through early Stage Two), Phase Two Commitment (late Stage Two through Stage Four), Phase Three Resolution (Stage Five). Stages track the match. Phases track the player. Both frameworks are necessary to fully describe the arc of a Bloodlines match. See Section 73 (Match Phase Architecture) for the full phase specification.

---

## Section 46: Match Scale, Pacing, and Recovery Mechanics

### The Session Design Philosophy

Bloodlines is designed for 2-to-10-plus-hour sessions. This is not a caveat or a disclaimer — it is the design target. The game is built for players who want to sit down with a match and live inside it for an evening, and possibly continue across multiple sessions if the match remains unresolved. The systems are tuned for endurance. A Bloodlines match at full scale, with the full player count, is expected to generate enough events, decisions, and dramatic reversals that five hours in, the match is still surprising.

This scale commitment has practical implications. The game cannot be won by accident. No early-game decision is so dominant that it determines the outcome by hour two. The systems are specifically tuned to resist runaway leader effects — not by handicapping successful players but by ensuring that every position of strength generates new opposition, new counter-play, and new costs that must be managed.

Up to ten players can participate in a match, alongside AI-controlled dynasties and minor tribal factions operating as environmental pressure. The minor tribes are not neutral — they have preferences, grievances, and the capacity to ally with or resist human dynasties based on how those dynasties have treated them. A match with ten human players and four AI dynasties and a dozen minor tribes is a living political environment, not a map with colored regions.

### The 90-Second Cycle

The heartbeat of a Bloodlines match is the 90-second game cycle. Every 90 seconds, the match processes: resource accumulation, population changes, army supply consumption, trade route yields, faith intensity shifts, conviction drift, and event triggers. The cycle is not hidden from players — they can see the clock. They know when the next processing moment arrives.

The 90-second cycle is fast enough to create urgency and slow enough to allow genuine decision-making. A player who needs to redirect a caravan, reassign garrison troops, issue a diplomatic communique, and check on a faith ritual in progress can do all of that within a cycle if they are paying attention. A player who is managing a military engagement on one border while monitoring a succession crisis on another is using the cycle to triage.

The cycle accumulates into match rhythm. Early-stage cycles are relatively quiet — the events being processed are small in scale. Late-stage cycles are dense. A Stage Four cycle might process a major battle result, a faith intensity threshold crossing, two trade route disruptions, and a political event trigger simultaneously. The cycle is the same 90 seconds it has always been. The world it is processing has become vastly more complex.

### Recovery Mechanics

Bloodlines is designed to preserve dynasties through disasters. The game state called extinction — a dynasty removed from the board entirely — requires sustained, deliberate targeting over time. A single catastrophic event does not kill a dynasty. It damages it. What happens next determines whether the dynasty survives.

Famine recovery operates through population compression. A dynasty experiencing famine contracts into its most sustainable territory, concentrating population where food production is reliable. The military shrinks. The political footprint shrinks. But the bloodline survives, and with it the Bloodline Members whose capabilities define what the dynasty can become again. A dynasty that survives famine with its Bloodline Member structure intact has lost an act but not the match.

Military defeat recovery follows the same compression logic plus the exile mechanic. A dynasty that loses its primary settlements in military engagements enters a state of exile — the bloodline retreats to whatever territory remains, potentially as little as a single defensible holdout. The exile state is mechanically defined: reduced resource access, reduced diplomatic standing, reduced military capacity. But the dynasty persists. The bloodline's Conviction axis remains intact. The Bloodline Members remain alive. Exile is the game saying: you have lost this phase. It is not saying you have lost the match.

Succession crises are recoverable through the Bloodline Member system. A dynasty struck by a Succession Crisis — the monarch has died without a clear heir, or multiple claimants are contesting the seat — enters a period of reduced institutional cohesion. Production drops. Military morale drops. Diplomatic credibility weakens. The resolution mechanics involve designating a new ruler from available Bloodline Members, which requires navigating rival claims and conviction costs. A dynasty that resolves a Succession Crisis cleanly emerges from it. A dynasty that handles it badly may fragment — but fragmentation is recoverable. Extinction requires far more than a failed succession.

Trueborn Summons recovery is different from the others because the Trueborn Summons is the game's response to a dynasty attempting illegitimate victory. A dynasty that has triggered a Trueborn Summons is not recovering from a disaster that happened to them — they created the situation. Recovery means surviving the coordinated response, which requires military, diplomatic, and faith resources simultaneously. Dynasties that survive a Trueborn Summons without collapse tend to emerge with significantly altered political positions, often unable to pursue the same victory path that triggered the Summons.

### Late-Game Pressure Mechanics

The game is specifically designed to prevent stalemate from feeling like safety. Three late-game pressure mechanics ensure that dynasties in strong but unresolved positions face mounting costs over time.

The Great Exhaustion fires when a match has been in Stage Three or Stage Four for a defined number of cycles without any dynasty making meaningful victory progress. All armies begin accumulating an Exhaustion penalty that reduces combat effectiveness, increases supply costs, and generates morale decay. The penalty is cumulative and does not reset without active recovery measures. The Great Exhaustion is the game telling the players: the world cannot sustain this indefinitely. Resolve something or begin paying the cost of irresolution.

The Succession Crisis is more targeted. Dynasties that have operated for an extended period without a formal succession framework — no designated heir, no marriage alliance securing the next generation, no faith covenant defining inheritance — begin generating Succession Risk. When the risk threshold is crossed, a crisis event fires. The timing is not random but it is not telegraphed with specific precision. Dynasties that maintain succession infrastructure avoid the crisis entirely. Dynasties that neglect it in favor of military and economic priorities will face it at an inopportune moment.

Legitimacy Erosion applies to dynasties holding large amounts of territory without the governance infrastructure to administer it. A dynasty that has conquered extensively but invested insufficiently in integration, housing, temple infrastructure, and garrison stability finds that the territory it holds begins generating Legitimacy Erosion — a slow degradation of population loyalty that compounds over time. Legitimacy Erosion cannot be stopped by military means. It can only be resolved by governance investment. A dynasty suffering severe Legitimacy Erosion in Stage Four is paying, in accumulated form, for every corner it cut in Consolidation.

### The Design Intent

The core design intent underlying all of this is that matches should trend toward negotiated resolution, Alliance Victory, or Trueborn-pressure resolution rather than clean solo conquest. Individual dominance is the hardest thing to achieve in Bloodlines. The systems are not designed to make it impossible — they are designed to make it genuinely difficult, requiring sustained excellence across every dimension of play simultaneously. Most matches do not end with one dynasty standing triumphant over all others. They end with something more complicated, more politically charged, and more narratively interesting than that.

---

## Section 47: Political Events System

### How the System Works

The political events system is not scripted. It does not fire events on a predetermined sequence or timeline. Events fire when the game state meets their trigger conditions. A match in which no dynasty ever reaches a high faith intensity level will never see the faith-crisis events that require high faith intensity. A match in which all dynasties maintain stable successions will never see the Succession War events that require contested claims. The system is a library of conditional consequences, and the match state determines which consequences become relevant.

This has a significant design implication: the events feel authored because they respond to what the players have actually done. A Dynastic Betrayal event that fires because two dynasties that have been Allies for three stages suddenly declare war lands with narrative weight that a scripted event in the same position would not. The players built the context. The system recognized what the context meant and named it.

Events are visible to all players when they fire. The notification system announces what happened and which dynasty or dynasties are affected. Some events are public in their full detail — The Great Reckoning, for instance, is announced with complete information about what it is and why it triggered. Others are announced by name and effect without revealing the full internal state of the affected dynasty. A Succession Crisis is publicly visible as a destabilization event. The precise nature of the competing claims is private information.

### Faith Events

Faith events fire based on faith intensity levels, faith spread, doctrinal conflicts between adjacent dynasties, and the behavior of dynasties toward faith infrastructure.

The Covenant Test is the Level 4 faith progression trigger. When a dynasty's faith intensity has reached the threshold for Stage Four progression, the Covenant Test fires. It is a single high-stakes event that requires the dynasty to demonstrate their covenant's full commitment — the specific form this takes depends on which faith is being practiced, but the mechanical requirement is always significant. Passing the Covenant Test opens Stage Four faith access. Failing it resets faith intensity to the Stage Three threshold and imposes a conviction penalty. A dynasty that has built its entire strategy around Stage Four faith access and fails the Covenant Test at a critical match moment is facing a genuine crisis.

The Faith Schism fires when two dynasties practicing the same faith adopt doctrinal interpretations that have diverged beyond a compatibility threshold. The Schism event does not end the shared faith — it creates a formal doctrinal split that reduces the faith-alignment bonuses both dynasties receive from their shared covenant and introduces friction into any cooperation attempts between them. Schisms are visible to all dynasties and often trigger opportunistic behavior from dynasties that had been held back by the faith-allied coalition's cohesion.

The Faith Suppression event fires when a dynasty's population has a significant existing faith allegiance and the ruling dynasty is actively suppressing it. Extended suppression produces underground faith practice, which increases conviction instability and creates sympathy populations within the dynasty's territory who will receive competing dynasties' faith missionaries with unusual openness.

The Covenant Collapse fires when a dynasty's faith intensity drops catastrophically — typically from military defeat destroying faith infrastructure, from a Trueborn Summons response targeting faith buildings, or from a failed Covenant Test at high intensity with cascading penalties. A Covenant Collapse is a major event that reshapes the match's faith landscape. The dynasty experiencing it loses access to all Stage Three and Stage Four faith mechanics. Recovery requires beginning the faith intensity climb again from Stage Two levels.

### Dynastic Events

Dynastic events fire based on succession states, Bloodline Member status, marriage alliance networks, and long-term diplomatic relationships.

The Legendary Member Emergence fires when a Bloodline Member has accumulated enough development — through battles, political decisions, diplomatic engagements, and faith practice — to cross into Legendary status. The event is announced publicly. All dynasties now know that a Legendary Member exists within the affected dynasty. This information matters: Legendary Members are priority targets in any engagement, they generate significant conviction bonuses for their dynasty, they accelerate prestige accumulation, and their death in combat produces narrative consequences that reverberate through the match.

The Succession Declaration fires when a ruling Bloodline Member is aging or damaged and has made a formal declaration of their intended heir. The declaration is public. Other dynasties can now plan around the succession rather than speculate about it. If the intended heir is a minor, the declaration also announces the regency arrangement, which creates a window of relative dynastic vulnerability that rivals may exploit.

The Alliance Betrayal fires when a dynasty in the Allied diplomatic state declares war on their ally, skipping the Hostile state entirely. This is the most serious dynastic event in the game. All dynasties receive immediate notification. The betraying dynasty absorbs a major conviction penalty. The political standing reduction is severe — the trust deficit generated by an Alliance Betrayal persists for an extended match period and affects every diplomatic relationship the betraying dynasty holds, not just the one with the former ally. An Alliance Betrayal is an event that reshapes the match's political landscape. The dynasty that does it must have weighed the cost carefully, because the cost is real and lasting.

The Succession War fires when a ruling dynasty head has died without a designated heir or when two Bloodline Members have public claim to the succession. The event creates a fork: the dynasty must resolve the succession through internal political mechanics before external pressure compounds the crisis. Neighboring dynasties can intervene diplomatically or militarily in support of one claimant, which draws them into the succession's resolution and creates obligations that may outlast the crisis itself.

### Economic Events

Economic events fire based on trade volume, currency network status, caravan disruption, resource production levels, and dynasty-to-dynasty dependency ratios.

The Trade Collapse fires when a major trade network — multiple connected routes between multiple dynasties — experiences simultaneous disruption from military conflict, diplomatic breakdown, or infrastructure damage. A Trade Collapse reduces economic output for all affected dynasties proportionally to their dependency on the collapsed network. Goldgrave experiences Trade Collapses more severely than other houses due to network dependency. Whitehall experiences them less severely due to infrastructure redundancy.

The Currency Crisis fires when a dynasty's currency standard has been adopted broadly enough to create dependency but has not yet reached the Currency Dominance threshold. A competitor dynasty can trigger a Currency Crisis by deliberately flooding the market with their own currency in contested trade zones, destabilizing the incumbent standard. The dynasty experiencing the Crisis must either defend their network aggressively or accept a threshold reset.

The Caravan Heist is a targeted economic event fired by a player action — specifically, a military operation directed at an enemy trade caravan rather than at a military target. Successful Caravan Heists reduce trade yield for the targeted dynasty and generate resources for the attacking dynasty. They also generate diplomatic heat — even dynasties not directly involved in the Heist register it as a destabilizing act. Westland and Goldgrave are the primary executors and targets of Caravan Heist events.

### Military Events

Military events fire based on army size ratios, garrison stability, border tensions, and prolonged conflict states.

The Veteran Legion fires when a specific squad has participated in enough engagements, with enough survivals, against enough different enemy types, that it crosses a veteran threshold. The squad's combat values increase and it gains a distinct designation visible to the dynasty controlling it. Veteran Legions are not lost lightly — their death in combat generates morale consequences that go beyond the unit's raw combat value. Ironmark, with its Ferocity Under Loss squad trait, produces Veteran Legions more readily than other houses.

The Military Collapse fires when a dynasty's total military capacity drops below the threshold required to defend its current territory holdings. The event is an internal notification — other dynasties do not automatically know it has fired, though aggressive probing of the dynasty's borders may reveal the weakness. A Military Collapse begins a clock: the dynasty must rebuild above the defense threshold within a defined window or begin losing border territories to attritional pressure without formal military engagement.

The March of the Exhausted fires during The Great Exhaustion when a dynasty's armies have accumulated maximum Exhaustion penalty. The event imposes the full penalty visibly and publicly — other dynasties can see that the affected army is compromised. This is the game's clearest signal that the match needs to resolve.

### Diplomatic Events

Diplomatic events fire based on treaty networks, long-standing relationships, broken agreements, and the accumulation of perceived slights in dynastic interactions.

The Diplomatic Summit fires when three or more dynasties in Cordial or Allied state with each other have maintained that positive diplomatic relationship for an extended period. The Summit event creates a formal opportunity for the allied parties to establish a multi-dynasty agreement — the closest thing the game has to a formal alliance bloc. Summit agreements have more binding terms than standard bilateral agreements and generate shared legitimacy bonuses for all participants.

The War of Grievances fires when a dynasty has accumulated three or more unresolved Diplomatic Grievances against a single rival — each Grievance being a recognized act of aggression, treaty violation, or disrespect that was never formally resolved. The War of Grievances event converts the accumulated Grievances into a formal casus belli that is visible to all dynasties. The declaring dynasty receives a conviction bonus for the declaration, and the defending dynasty faces the legitimacy cost of having generated three formal Grievances without resolution.

The Non-Aligned Bloc fires when multiple dynasties that have maintained strict neutrality toward each other have avoided entanglement in the major power conflicts of the match. The Bloc event recognizes them as a collective neutral party with shared interests in preventing any single dynasty from achieving Military Conquest victory. The Bloc is not a military alliance — it does not obligate military coordination. It is a diplomatic acknowledgment that creates the foundation for one if the match develops in that direction.

### World Events

World events fire based on global game state conditions that transcend individual dynasty behavior.

The Great Reckoning fires when any dynasty controls 70% or more of the match's total territory. It is the game's response to near-total military dominance. The Great Reckoning triggers the Trueborn Summons regardless of whether the Trueborn Summons' normal diplomatic conditions have been met. Every dynasty not under the hegemon's control is notified and offered Trueborn coordination. The hegemon faces a coordinated response from all remaining independent parties. The Great Reckoning is the match telling the hegemon: you have reached the limit of what individual conquest can achieve without the world recognizing you as a tyrant. What happens next determines whether you are a conqueror or a sovereign.

The Age of Heroes fires when three or more Legendary Bloodline Members are active simultaneously across different dynasties. The Age of Heroes event amplifies the narrative weight of all Bloodline Member actions during its duration — prestige accumulation accelerates for all dynasties with active Legendary Members, diplomatic achievements carry greater legitimacy bonuses, and faith covenant milestones are reached faster by dynasties with active Legendary Members. The Age of Heroes is the match saying: this is the moment the world will remember.

The World Forgetting fires when a match has been in late-stage for an extended period without resolution and The Great Exhaustion has been in effect for multiple cycles. The World Forgetting is the most extreme late-game pressure event. It imposes accelerating decay penalties on all dynasties simultaneously. Population declines. Trade yields drop. Faith intensity begins eroding across all covenants. The World Forgetting cannot be stopped — it can only be survived long enough to reach a resolution. It is the game's final mechanism for forcing a match toward its end. The dynasty that navigates The World Forgetting most effectively — that has built the most resilient systems and preserved the most capable Bloodline Members — will likely determine what kind of resolution arrives.

---

## Section 48: Diplomatic System

### The Five Diplomatic States

Every pair of dynasties in a Bloodlines match occupies one of five diplomatic states at all times. The state governs what actions are available between them, what trust accumulation is possible, and how other dynasties read their relationship.

War is the state of active military conflict. Armies can engage freely. Trade is suspended. Diplomatic communication continues at severe penalty — dynasties at war can still send envoys, but reception is structured by the active conflict. War can end through negotiated peace, through military victory by one party, or through external intervention that forces both parties to disengage. A dynasty that declares war and then cannot follow through with military pressure occupies an embarrassing diplomatic position — the declaration without the substance damages reputation more than maintained Hostility would have.

Hostile is the state of formal diplomatic deterioration below Neutral. Trade is possible but strained. Military deployments near the border are read as threatening. Diplomatic agreements reached at Hostile state carry less binding weight than those reached at Neutral or above — the distrust is structurally acknowledged. Moving from Hostile to War requires a formal war declaration plus grounds. Moving from Hostile toward Neutral requires active diplomatic investment that the other party must accept.

Neutral is the baseline state of dynasties that have not developed a significant relationship in either direction. Trade proceeds at standard yields. Diplomatic agreements are available. Neither party has claims on the other's behavior. Neutral is not a stable equilibrium — over time, dynasties that interact extensively will drift toward Cordial or Hostile depending on the nature of those interactions.

Cordial is the state of positive diplomatic relationship established through trade, formal agreements, marriage alliances, or sustained cooperative behavior. Trade yields increase slightly between Cordial parties. Diplomatic agreements carry greater binding weight. Military deployments near a Cordial dynasty's border are read as reassuring rather than threatening. Cordial is not Allied — it does not obligate military coordination — but it creates the foundation for alliance and generates genuine political goodwill that other dynasties can observe and factor into their calculations.

Allied is the state of formal military and diplomatic partnership. Allied dynasties are obligated to coordinate on military threats to either party. Trade yields are maximized. Diplomatic representation is shared in some contexts. Allied state is the strongest diplomatic position available and carries the heaviest obligations. Breaking from Allied state to War — the Alliance Betrayal — is the most politically costly single act in the diplomatic system. Moving from Allied to Hostile without Betrayal — a formal downgrade through diplomatic process — is painful but legitimate. The Alliance Betrayal skips the process entirely and pays the maximum cost.

### Agreement Types

Seven agreement types govern what dynasties can formalize between each other. Each has minimum diplomatic state requirements, conviction implications, and binding terms.

Trade Agreements establish formal trade routes between dynasties with defined yield terms. Available from Neutral state. Trade Agreements generate economic benefit for both parties and slowly accumulate Cordial drift if maintained without disruption.

Non-Aggression Pacts formalize the absence of military intent for a defined period. Available from Neutral state. A Non-Aggression Pact is not an alliance — it is a mutual acknowledgment that both parties are better served by stability than conflict in the current match moment. Breaking a Non-Aggression Pact generates a Diplomatic Grievance against the breaking party.

Military Alliances obligate both parties to coordinate against military threats. Available from Cordial state. Military Alliances are the formal version of the Allied diplomatic state's military implications. A dynasty that fails to honor a Military Alliance obligation generates a major Diplomatic Grievance and absorbs a conviction penalty.

Faith Alliances establish a shared covenant framework between two dynasties practicing compatible faith doctrines. Available from Cordial state, with faith intensity requirements. Faith Alliances generate faith-alignment bonuses for joint operations and create the groundwork for combined Stage Four faith mechanics. They are resistant to dissolution — the faith covenant binding is treated as more fundamental than political calculation by the populations involved, and breaking a Faith Alliance carries a heavier conviction cost than breaking a standard Military Alliance.

Marriage Agreements establish a dynastic family connection between two bloodlines. Available from Cordial state. Marriage Agreements create succession implications — a child of the union has potential claim to both bloodlines — and generate long-term legitimacy benefits that compound over generations. They are the most durable diplomatic instrument in the game. Dissolving a Marriage Agreement requires formal repudiation, which generates conviction cost and Diplomatic Grievance.

Tributary Arrangements formalize an unequal relationship where one dynasty provides resources or strategic compliance to another in exchange for protection or recognition. Available when one party holds significant military or economic leverage over the other. Tributary Arrangements are stable but brittle — the subjugated dynasty will pursue exit from the arrangement whenever the opportunity presents itself. Cruel dynasties that impose Tributary Arrangements face higher exit attempt rates than Moral dynasties that negotiate more favorable terms.

Trade Route Establishments are the formal establishment of a specific physical trade route between two settlements, distinct from the general Trade Agreement. Route Establishments define the specific goods, the specific path, and the specific security obligations of both parties. They generate higher yields than general Trade Agreements but require specific infrastructure and are vulnerable to physical disruption along the route.

### War Declaration Process

War cannot be declared arbitrarily without diplomatic consequence. The declaration process requires grounds. Six recognized grounds for war exist in Bloodlines diplomatic law.

Territorial Claim is the assertion that a specific territory rightfully belongs to the declaring dynasty by historical right, inheritance, or conquest reconquest. The claim must have been formally registered at least one cycle prior to the declaration.

Treaty Violation is the assertion that the opposing dynasty has broken the terms of a formal agreement. The violation must be documented in the diplomatic record — it cannot be a general accusation. Specific agreements and specific terms must be cited.

Faith Aggression is the assertion that the opposing dynasty has actively suppressed, damaged, or militarily targeted the declaring dynasty's faith infrastructure or practitioners. Faith Aggression grounds are available only when faith infrastructure damage is documented.

Dynastic Insult is the assertion that the opposing dynasty's leaders have publicly dishonored the declaring dynasty in ways that require satisfaction. Dynastic Insult is the most subjective grounds and carries the least legitimacy bonus — other dynasties recognize it as a weaker justification, and the conviction benefit is correspondingly reduced.

Defense of Tributary is the assertion that the opposing dynasty has attacked a dynasty under the declaring dynasty's protection. Defense of Tributary requires an active Tributary Arrangement to be in effect.

Trueborn Summons Response is not a grounds for declaration in the standard sense — it is the grounds generated automatically when the Trueborn Summons system activates. A dynasty responding to a Trueborn Summons declaration has automatically recognized grounds for their military engagement.

### The Trueborn City Diplomatic Guarantee

The Trueborn Summons City holds a unique diplomatic status. When a dynasty sends a representative — any formal diplomatic envoy, not just Bloodline Members — to the Trueborn City for negotiation, that representative is under the city's diplomatic protection. Attacking a representative at or in transit to and from the Trueborn City triggers an automatic Trueborn Summons regardless of any other match conditions.

This is the most serious diplomatic violation in the game. The Trueborn Summons triggered by representative attack is unconditional — it cannot be argued, negotiated, or politically managed. It fires. Every dynasty in the match receives notification. The attacking dynasty faces immediate, full-scale Trueborn Summons response without the normal lead time that other Trueborn triggers provide.

The guarantee exists because the Trueborn Summons system requires a neutral coordination mechanism that all parties can rely on. If the Trueborn City's neutrality could be violated with military force, the Summons mechanism loses its anchor. The unconditional nature of the representative protection is not a game balance choice — it is a worldbuilding necessity. The world of Bloodlines requires one place where the rules hold absolutely. The Trueborn City is that place.

### Conviction and Diplomatic Reception

Conviction is not merely an internal resource. It has an external face. A dynasty's conviction axis is known to the match world — not in its specific numerical value but in its behavioral reputation. Dynasties that have operated Cruelly have a reputation that precedes them. Dynasties that have operated Morally have a different reputation. Neutral dynasties are known for pragmatic calculation rather than principle.

Cruel dynasties face structural trust deficits in negotiation. This manifests mechanically as a reduced probability of agreement acceptance at equivalent offer values — a Cruel dynasty must offer more to achieve the same agreement outcome than a Moral dynasty would need to offer. Trade Agreements with Cruel dynasties require higher yield terms to attract partners. Tributary Arrangements imposed by Cruel dynasties generate higher exit attempt rates. Military Alliances with Cruel dynasties are viewed with more skepticism by third parties who calculate the probability of Alliance Betrayal.

This is not a punishment for playing Cruel. It is an accurate representation of how political reputation functions. A dynasty that has demonstrated willingness to use fear, execute prisoners, and suppress populations has told the world something true about how it operates. The world factors that information into its dealings. The Cruel dynasty that understands this plays its diplomacy accordingly — seeking Tributary power over willing partnership, using economic leverage rather than trust as its diplomatic instrument, and accepting that its alliance network will be smaller and more mercenary than what a Moral dynasty can build.

---

## Section 49: Victory Conditions Overview

### The Sovereignty Seat

The sovereignty seat is the match's terminal condition. It is not named the Iron Throne. It is not derived from any external property. It is the concept indigenous to Bloodlines: one bloodline, or one coalition with an agreed head ruler, recognized by the world as the supreme authority over all lands. Recognition is the operative word. It is not enough to hold the territory. The world must accept — through population compliance, diplomatic acknowledgment, or faith covenant ratification — that this dynasty rules. Taking land is military work. Being recognized as sovereign is political, economic, and spiritual work simultaneously.

### The Design Philosophy

Individual dynasty victory is designed to be very, very, very difficult. This is a non-negotiable design commitment, not a difficulty slider setting. The game's systems are constructed so that every position of strength generates new pressure from multiple directions simultaneously. A dynasty approaching Military Conquest triggers the Great Reckoning and Trueborn Summons. A dynasty approaching Faith Divine Right triggers coalition formation among non-faith-aligned parties. A dynasty approaching Currency Dominance triggers the Resolution Battle mechanic. A dynasty approaching Territorial Governance completion faces constant revolts and population resistance. Every path to the sovereignty seat has a structural immune response built into the game.

This is intentional. The game is modeling something true about power: legitimate authority is not taken by force alone. It is granted by the world's acquiescence, and the world does not acquiesce easily to those who demand it. The dynasties that have come closest to individual victory in the design conception of the game are the ones that built legitimacy slowly, in multiple dimensions simultaneously, so that when the moment of potential victory arrived, the world had less cause to resist than it would have had against a less sophisticated dynasty.

The asymmetry between paths matters. Military Conquest is the most legible path — everyone can see a dynasty accumulating territory — and consequently the one with the broadest and most automatic resistance. Faith Divine Right is more resistant to military counter-play but requires a longer development arc and faces coalition pressure during the declaration window. Currency Dominance is structurally subtle until it approaches threshold, at which point the Resolution Battle mechanic provides a hard military counter. Territorial Governance is the highest-difficulty path, nearly impossible to complete while facing active military disruption. Alliance Victory requires managing both external opposition and internal coalition tensions simultaneously.

### The Expected Resolution Shape

Most matches do not end with one dynasty achieving the sovereignty seat alone. The design expectation — not a fallback, not a consolation, but the designed primary resolution shape — is that matches end through Alliance Victory, through Trueborn-pressure resolution that forces a negotiated settlement, or through prolonged stalemate that slowly resolves into a negotiated outcome. The game is not designed to reward the player who conquers fastest. It is designed to reward the player who is still standing when everyone else is ready to deal.

Exile is a real game state, not a soft death. A dynasty in exile has lost its dominant position but retains its bloodline, its Bloodline Members, and its conviction axis. An exiled dynasty can re-enter the match's competitive landscape given time and the right circumstances. A dynasty that drove a rival into exile and then overextended in the pursuit of another rival has handed the exiled dynasty an opportunity. The game knows this. The exile mechanic exists specifically to prevent dynasty elimination from being simple.

### Path Asymmetries and Counter-Play

Each victory path has a different resistance profile, a different window of vulnerability, and a different counter-play toolset.

Military Conquest is most vulnerable during Consolidation, when the garrison math of held territory has not yet been proven. A dynasty that has taken too much territory too fast will discover that it cannot hold all of it — garrison costs, loyalty decay, and Legitimacy Erosion will force contraction. The counter-play to Military Conquest is available to every dynasty: refuse engagements, deny supply lines, target faith infrastructure, and wait for the Legitimacy Erosion mechanics to compound against the overextended aggressor.

Faith Divine Right is most vulnerable during the declaration window. The pre-declaration requirements are public knowledge. The declaration clock, once started, is visible. The counter-play is coalition formation before the declaration reaches its resolution, specifically targeting faith infrastructure and the grand structures that are prerequisites for the declaration. A dynasty with strong faith intensity and strong military does not guarantee Faith Divine Right victory — it guarantees a contested declaration that requires political as well as military management.

Currency Dominance is most vulnerable at the moment it approaches the threshold, when the Resolution Battle mechanic becomes available to challengers. Before that threshold, the path is structurally quiet — trade network building does not generate the same alarm as military expansion. The counter-play is the Resolution Battle itself, which is a specifically designed military option for breaking the economic network before it reaches its terminal condition.

Alliance Victory is most vulnerable to coalition defection — the structural temptation for any coalition partner to break the agreement and pursue individual victory once the coalition has accumulated enough combined strength that one party might be able to win alone. Managing the internal dynamics of an Alliance Victory coalition is as complex as managing the external opposition.

---

## Section 50: Military Conquest and Territorial Governance

### Military Conquest

Military Conquest is the path that every dynasty can understand and every dynasty will resist. Its clarity is its double-edged quality — the most legible victory path is also the most universally opposed.

The prerequisites for Military Conquest are staged across the match. A dynasty cannot simply win enough battles and declare victory. The path requires not just territorial control but demonstrated governance capacity — the ability to administer what has been taken, not just hold it militarily. A dynasty approaching the Military Conquest threshold of 75% total territory with a 60% stability floor must demonstrate that it has governed its held territory into something resembling functioning administration. A dynasty with 75% territory and 40% stability has not achieved Military Conquest — it has achieved overextension, and the Legitimacy Erosion mechanics will reflect that.

The 75% territory threshold and 60% stability floor must both be held simultaneously for a scaled time window that adjusts based on total match size. In a full ten-player match, the time window is longer than in a four-player match — the world at scale requires more sustained demonstration of governance. During the hold window, all other dynasties can see the threshold being approached and the time remaining. Opacity is rejected here for the same reason it is rejected elsewhere: the drama is the race, not the reveal.

Garrison math is the tactical reality of Military Conquest that separates dynasties who understand the path from dynasties who simply accumulate territory. Every territory holding requires garrison capacity proportional to its size, population, and loyalty level. Low-loyalty territories require heavier garrison. Territories with active faith suppression require even heavier garrison — suppressed populations do not simply comply. At modest territorial holdings, garrison math is manageable. At the scale required for Military Conquest, garrison math is a constant pressure that drains military capacity that cannot be used for further expansion while still being spent on holding what exists.

The Great Reckoning counter is the structural immune response at 70% territory. It fires before the 75% threshold, giving the coalition response time to form before the victory condition is actually claimable. A dynasty that reaches 70% territory has done extraordinary work and will now be tested by the full weight of everyone remaining.

The Cruel conquest variant trades stability for speed. Fear-compliance — executing resistors, burning settlements that refuse to capitulate, using brutal suppression to break loyalty — generates faster territory compliance than governance investment. The stability floor it produces is lower and more volatile. A Cruel dynasty pursuing Military Conquest can potentially reach the 75% threshold faster than a Moral dynasty using governance investment, but it will struggle to meet the 60% stability requirement because fear-compliance produces a compliance that exists only as long as the fear does. When the army moves on to the next territory, the compliance at the previous territory begins eroding unless garrison force is heavy enough to sustain the fear environment. Cruel Military Conquest is a path that demands constant military attention to every held territory simultaneously, which is ultimately incompatible with the expansion required to keep moving toward the threshold.

Mutiny and defection are the mechanical expression of what happens when reckless conquest exceeds the dynasty's administrative and moral capacity. Armies that have been ordered to execute civilians, burn settlements, and suppress faith practitioners for extended periods begin generating morale decay that cannot be addressed with resources alone. The soldiers are people. They carry what they have done. A dynasty that pushes its armies through extended Cruel conquest without faith covenant support — the faith frameworks that contextually justify what is being done — will find that the armies begin to lose cohesion at the edges. Defection is individual. Mutiny is collective. Both are catastrophic at the scale Military Conquest requires.

### Territorial Governance Victory

Territorial Governance victory is stated plainly: it is the highest-difficulty path in the game. The requirement is not simply controlling territory but developing every piece of owned territory — including tribal lands absorbed through the match — into genuine willing acceptance of the dynasty's rule. Every corner of the land governed into functioning administration with population loyalty above a sustained threshold.

The practical obstacles are relentless. Territory acquired through conquest has loyalty deficits from the conquest process itself. Territory acquired through tribal absorption has cultural identity resistance that does not respond quickly to governance investment. Territory acquired through economic leverage has dependencies that the population resents even when they comply with. Every territory type presents a different governance challenge, and the path requires solving all of them simultaneously.

Constant revolts are the rhythm of this path. A dynasty pursuing Territorial Governance victory is always dealing with active unrest somewhere in its holdings. The question is never whether unrest exists — it always does — but whether the dynasty has the governance infrastructure to manage and reduce it faster than new unrest emerges. This requires not just military garrison but housing investment, faith infrastructure calibrated to local populations, trade access that gives the population a material interest in stability, and Bloodline Members deployed in administrative rather than military roles.

What it means to achieve Territorial Governance victory is the most total expression of dynastic ambition in the game. It means a dynasty has made every population in the world — every culture, every faith allegiance, every regional identity — into a functioning part of a governed whole that accepts the dynasty's authority not out of fear and not out of dependency but out of something closer to genuine legitimacy. It is the rarest outcome in the game's expected resolution space. A dynasty that achieves it has done something that the game's systems were designed to make almost impossible, and the sovereignty seat it earns is the most complete expression of what the seat represents.

---

## Section 51: Dynastic Prestige — The Modifier System

### What Prestige Is

Dynastic Prestige is not a victory path. It is a civilizational modifier that accelerates pursuit of every other victory path and shapes how the world responds to the dynasty pursuing it. High prestige does not win the game. It makes the dynasty that holds it significantly harder to stop, easier to ally with, and more capable of converting strategic positions into recognized authority.

Accumulating prestige is not optional. Every major action the player takes generates or costs prestige. Winning significant battles, developing named Bloodline Members, completing political marriages, surviving coalition attacks, and achieving major diplomatic milestones all generate prestige. Being publicly humiliated, losing Legendary Members, having declared claims fail, and breaking formal agreements all cost it.

The world tracks prestige. Every dynasty can see the prestige standings of every other dynasty at all times. There is no obscuring a prestige lead.

### Prestige Accumulation Sources

**Bloodline depth:** The number of generations of developed, named Bloodline Members who have lived consequential lives within the match. A dynasty maintaining continuity across four or five generations has done something that cannot be purchased or replicated quickly. This is the foundational prestige category.

**Political marriages:** Each strategic marriage builds the dynasty into the world's aristocratic fabric. Marriages to other founding houses, to lesser houses, and to captured or allied foreign bloodlines all contribute. Marriage prestige compounds across generations.

**Famous battle outcomes:** Victories against numerically superior forces, survival against Trueborn Summons, defeat of a rival's Legendary Member — the prestige system weights battles by difficulty and stakes, not just by outcome.

**Faith alignment:** Deep alignment with the faith practiced by a majority of the world population generates legitimacy that crosses political boundaries. Populations that owe no political loyalty to a dynasty may recognize its prestige through shared covenant.

**Legendary Member legacy:** Each Legendary Bloodline Member alive and active contributes living prestige that compounds with their development arc. Upon death, this converts to permanent historical prestige that cannot be eroded. The dynasty that raised legendary figures has told the world something that territory alone cannot say.

**Major diplomatic achievements:** Brokering Alliance Victory negotiations, mediating Succession Wars, resolving crises through intervention — these contribute prestige through demonstrated political stature rather than military or economic dominance.

### What Prestige Does

High prestige generates concrete strategic advantages across all five victory paths:

**Population loyalty advantage:** Populations throughout the map respond to high-prestige dynasties with reduced loyalty resistance. Territories that would normally require heavy garrison investment accept the dynasty's presence with less friction.

**Diplomatic reception improvement:** High-prestige dynasties receive more favorable agreement terms from other dynasties. The trust deficit that Cruel dynasties face in negotiation is partially offset by prestige; the trust advantage that Moral dynasties enjoy is amplified.

**Lesser house attraction:** Ambitious lesser houses and promoted commanders are more likely to seek vassalage with high-prestige dynasties than with equally powerful but lower-prestige rivals. This matters most in Stage Four and Five when the match's power structure is sorting out.

**Alliance formation advantage:** Coalition formation is easier for high-prestige dynasties. Partners willing to accept junior roles in an Alliance Victory arrangement are more numerous. The alliance trigger conditions are easier to meet when prestige legitimizes the partnership.

**Faith declaration legitimacy:** A high-prestige dynasty declaring Faith Divine Right receives less automatic skepticism from neutral populations. The claim that this bloodline descends from divine authority is more credible when the bloodline has a demonstrated history of exceptional quality.

### Prestige Does Not Win the Match

Opponents must be conquered, neutralized, or otherwise defeated through the five remaining victory paths. A dynasty at maximum prestige with no victory path in progress is a prestigious dynasty that has not won. Prestige is the modifier. The paths are the method. High prestige means the dynasty pursuing Military Conquest faces less population resistance. High prestige means the Currency Dominance adoption curve is faster. High prestige means the Alliance Victory coalition is easier to hold together. None of these make victory automatic. They make it less impossible.

---

## Section 52: Faith Divine Right and Alliance Victory

### Faith Divine Right

The Declaration of Divine Right is the formal initiation of the Faith Divine Right victory path. When a dynasty has met all pre-declaration requirements — faith intensity at the Stage Four threshold, global faith share at or above the covenant's required percentage of world population, the covenant's specified grand structure built and operational — the dynasty can make the declaration.

The declaration is public from the moment it is made. The spread window opens immediately and is visible to all players: a countdown showing exactly how much time remains before the declaration reaches the threshold at which the victory condition can be claimed. The declaration clock is not hidden. The milestone requirements that preceded the declaration were not hidden. Nothing in the Faith Divine Right path depends on secrecy. The drama is entirely in what the rest of the world does in response.

The spread window is the period during which the dynasty's claim that its bloodline descends from the covenant's highest power must propagate through the world's faith population sufficiently to constitute recognition. This is not a simple counter — the spread encounters resistance. Populations practicing different faiths do not accept the claim. Dynasties with competing faith interests actively work against the spread. The grand structures of rival faiths generate counter-narrative that slows the propagation in regions they influence. The dynasty making the declaration must simultaneously defend its spread progress militarily, maintain its faith infrastructure against targeted disruption, and manage the diplomatic fallout of having declared the most audacious claim in the match.

Coalition response during the spread window is the structural immune response of the match to Faith Divine Right. The declaration notifies all dynasties simultaneously. Every dynasty that is not faith-aligned with the declaring covenant has an immediate and legible interest in preventing the victory. Every dynasty that is faith-aligned with the covenant must decide whether their shared faith creates solidarity with the declaring dynasty or whether they see the declaration as the declaring dynasty claiming sovereignty over a covenant that belongs to all its practitioners. Faith alliances forged in Stage Two become strained when one party to the alliance has declared itself the covenant's divine heir and the other party has not.

Success requires that the declaration spread window completes without the spread being broken. If faith infrastructure is destroyed on a scale that drops the global faith share below the required percentage, the declaration fails. If the grand structure is destroyed, the declaration fails. If the declaring dynasty's faith intensity drops below the threshold due to successful enemy targeting, the declaration fails and resets to the pre-declaration state. The window is long enough to be achievable. It is not so long that the coalition response cannot organize.

Failure consequences are significant. A dynasty that has invested the entire late-game arc into Faith Divine Right and fails the declaration enters Stage Four without a viable victory path in progress. The pre-declaration prerequisites must be rebuilt. The coalition that formed to stop the declaration is now battle-hardened and politically organized. The dynasty must either find a path to victory through means it has not developed or accept that this match will not end in its sovereignty.

### Alliance Victory

Two or more bloodlines formally agreeing on terms of shared rulership — with one agreed ultimate ruler — constitutes the framework of Alliance Victory. The mechanism is simple to state and genuinely difficult to execute. Every part of the process is contested.

The formal agreement itself requires that the coalition partners negotiate and ratify the terms: who holds the sovereignty seat, what obligations the sovereignty holder has to the coalition partners, how territory and resources are distributed under the shared rulership arrangement. These negotiations happen under match conditions — other dynasties are watching, military situations are evolving, internal conviction dynamics are operating on all parties. A coalition that cannot agree on terms within the window the match provides has not achieved Alliance Victory — they have failed to negotiate one.

Once terms are ratified, the population acceptance clock begins. Sixty-five percent of the world's total population must come to accept the coalition's authority before the victory condition is recognized. This is not automatic. Populations that have been under different rulers, different faiths, different economic systems do not simply comply because two dynasties have agreed to share the sovereignty seat. The acceptance process requires active governance work: faith infrastructure deployment in resistant territories, economic investment in populations that have no material interest in the coalition's success, military presence in territories that are actively revolting against the new arrangement.

Revolt continues throughout the acceptance clock. A coalition that has strong governance capacity but is experiencing targeted military disruption by dynasties that are not party to the agreement must manage the revolt while simultaneously managing the acceptance accumulation. The two pressures compound: military disruption slows acceptance accumulation, and slow acceptance accumulation extends the window during which military disruption can operate.

Coalition partners have competing interests. The dynasty that holds the sovereignty seat under the Alliance Victory arrangement has more to gain from the victory condition being reached quickly. The dynasty that is the junior coalition partner has the strategic option of defecting once the combined coalition strength is sufficient that the junior partner might be able to pursue individual victory — using the coalition as a vehicle to reach a position from which the coalition itself becomes unnecessary. Managing the internal dynamics of an Alliance Victory coalition is a political game within the political game, requiring constant investment in maintaining the alliance's cohesion against the very same forces that make individual victory so difficult.

The difficulty of Alliance Victory is treated with the same standard as every other victory path. It is not a consolation for players who could not win individually. It is genuinely, structurally difficult — a complex coordination problem under adversarial conditions with competing incentives pulling at the coalition from within and without simultaneously. A match that ends in Alliance Victory has produced a resolution that required extraordinary political sophistication from both coalition partners over an extended arc. That is the appropriate difficulty standard for the sovereignty seat.

### Late-Game Escalation

The three late-game escalation mechanics that were introduced in Section 46 take on specific character in the context of the victory paths.

The Great Exhaustion accelerates the pressure on all dynasties simultaneously, but it does not accelerate equally. Dynasties that have invested in economic self-sufficiency — Goldgrave's trade network, Whitehall's infrastructure efficiency, Hartvale's population resilience — experience The Great Exhaustion less severely than dynasties that have been on sustained military offense. Military-dominant dynasties in late-stage matches are therefore structurally disadvantaged by The Great Exhaustion relative to economically sophisticated dynasties. This is a designed consequence: the path of sustained conquest is the path most punished by endurance pressure.

The Succession Crisis in endgame conditions is a match-altering event. A dynasty approaching Faith Divine Right declaration threshold that is simultaneously hit by a Succession Crisis faces a window of institutional weakness at the worst possible moment. The Succession Crisis is not random — dynasties that maintain succession infrastructure avoid it. But maintaining succession infrastructure in Stage Four requires continuous investment that is competing with every other late-game priority. The dynasties that let it slip will feel the cost.

Legitimacy Erosion at match endgame becomes the defining test of Military Conquest viability. A dynasty at 60% territory but with 45% stability is not approaching Military Conquest — it is approaching collapse. The endgame arc of a military-dominant dynasty is defined by whether governance investment can outpace the Legitimacy Erosion being generated by the scale of what has been taken.

The Trueborn rise arc reaches its fullest expression in the match endgame. The Trueborn Summons, if it fires in late-stage, is the most consequential event in the match. A dynasty that has survived to Stage Four with a position strong enough to trigger the Summons has done something extraordinary. The Summons itself, if the Trueborn player has been positioned well throughout the match, arrives as the expression of everything the Trueborn house was designed to be: the institutional response of the world to the claim that one bloodline deserves to rule all others. Whether the Summons succeeds or fails, it generates the match's most significant narrative moment.

---

---

# PART IX — THE NINE FOUNDING HOUSES

## Section 53: House Trueborn

**Hair color:** Silver

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 54: House Highborne

**Hair color:** Darker gold with highlights

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 55: House Ironmark

### Historical Identity

Ironmark did not survive The Great Frost by preserving anything. It survived by cutting loose everything that could not be kept and holding onto the one thing that could not be taken: the knowledge of how to make iron work. Not the politics of iron, not the trade of iron, not the institutional frameworks governing who could mine and smelt and forge — those were the trappings of a world that the Frost ended. The knowledge itself. The physical, embodied, practiced knowledge that lives in hands that have done the work ten thousand times and do not need to think about it anymore. When the world collapsed, Ironmark's smiths kept forging. They forged through the Frost. They forged in the aftermath. They forged because it was what they knew how to do and because the people who needed what they made were willing to pay for it in food, in protection, in labor, in whatever was necessary.

The house that emerged from the Frost was not the same house that entered it. The pre-Frost Ironmark had been powerful in a world where institutional frameworks mediated the relationship between iron production and political authority. The post-Frost Ironmark was powerful in a way that required no mediation: it made what the world needed, and the world understood that directly. The politics came later, as the world restabilized enough for politics to resume. But by then, Ironmark had already established the pattern of its post-Frost existence — take what is needed, pay in blood if necessary, waste nothing, and build the next capability before the current one is tested to failure. The forge culture is not brutality for its own sake. It is the culture of people who learned during the worst catastrophe the world had experienced that resources are real and abstractions are not, and that the people who survive are the ones who can make things that matter out of materials that exist.

The post-Frost Ironmark carries a specific kind of pride that other houses sometimes misread as arrogance. It is not arrogance. Arrogance is claiming more than has been earned. Ironmark claims exactly what has been earned, and what has been earned is considerable. The forge-house reached productive capacity before any other house in the post-Frost period. It fielded capable armies while other houses were still negotiating the terms of their survival. It established territorial control in the chaos between the Frost's end and the restabilization of political norms. It did all of this through work — physical, exhausting, costly work — and it has not forgotten that the position it holds now was built in that period. The pride is not in what Ironmark is. It is in what Ironmark made itself into.

### Identity Statement

Ironmark reaches power first and pays for it in blood.

### Mechanical Identity

Ironmark's societal advantage is early iron access, available 20 to 30% faster than the standard progression curve for any other house. This is the foundational advantage of the house — everything else derives from it. Iron access determines unit quality. Unit quality determines military capacity. Military capacity determines territorial control in the match's early stages. A dynasty with better armies earlier has options that other dynasties do not have, and the ability to exercise those options before the match's political structure has hardened around them is the advantage that the early iron access provides.

The required disadvantage is Blood Production: Ironmark's forge culture generates increased combat casualties from its aggressive fighting posture, combined with reinforcement inefficiency from the permanent competition between forge labor and military labor for the same population pool. Ironmark armies fight with controlled aggression that produces results — and produces a casualty rate that other houses' armies, fighting more defensively or more cautiously, do not generate. At small scales, this is manageable. At the scale that late-game Military Conquest requires, the Blood Production penalty is a grinding pressure that Ironmark must consciously manage or it will find that victories are consuming the army faster than reinforcement can replace it.

Unit values: Swordsmen 5/5 are baseline. Militia 6/5 represents Ironmark's characteristic advantage — its common infantry is materially better than every other house's common infantry because they are equipped from better-quality forge output. The Axeman is Ironmark's exclusive unit, at Offensive 6 / Defensive 4, designed for maximum offensive pressure with acknowledged defensive fragility. Axemen are exceptional against armored opponents and against structures — they can assault fortifications with effectiveness that other house's standard units cannot match. Against fast, mobile units, the Axeman's slower engagement pace and lower defensive value creates real vulnerability.

The Ferocity Under Loss squad trait is the house's defining mechanic. As squad size decreases toward one, the surviving members gain progressive combat bonuses: increased attack speed, reduced routing tendency, and a fear aura that suppresses enemy morale in proximity. The Axeman variant of this trait is the most pronounced in the game — a single surviving Axeman with full Ferocity accumulation is a substantially different threat than the same Axeman at the start of the engagement. The tactical consequence is that Ironmark squads are most dangerous when they appear most defeated. Other houses learn this or pay for it.

### Strategic Narrative

The Ironmark early game is as aggressive as the forge-culture advantage allows. The iron access advantage is the most front-loaded temporal advantage in the game — its value is highest in Stage One and Stage Two and diminishes over time as other houses reach comparable quality through their own development paths. Ironmark players who understand this treat the early stages as a window that must be exploited before it closes. Territory taken in Stage One costs less than territory taken in Stage Three because the armies resisting the taking are less capable. The early iron advantage is the tool for taking that territory.

The mid-game Ironmark challenge is managing the Blood Production penalty against the demands of the territorial control already established. A dynasty that has taken significant territory in Stage One and Two now has garrison costs, loyalty management requirements, and an ongoing reinforcement drain from the blood cost of the aggressive fighting culture. The mid-game is where Ironmark players discover whether they over-committed in the early stages — whether the territory taken can be held sustainably or whether contraction is necessary to restore population balance.

Late-game Ironmark in Stage Four is the match's most capable sustained military force, at the cost of being one of the match's most vulnerable to extended attritional pressure. The Ferocity Under Loss trait means Ironmark armies are nearly impossible to completely destroy in battle — the survivors will fight as though they were a full squad, which other houses find deeply unnerving on the battlefield. But the Blood Production penalty means Ironmark arrives at late-stage with a smaller total population base than houses that fought more conservatively. The numbers are there. The depth is not. Ironmark endgame is a race against attrition that the house did not build itself to win.

### Victory Path Affinities

Military Conquest is Ironmark's strongest path by design. The early territorial advantage, the superior infantry, the Axeman's assault capability, and the Ferocity Under Loss resilience are all aligned with what Military Conquest requires in Stages One through Three. Ironmark is the house most likely to trigger The Great Reckoning simply by being better at military expansion than every other house in the game.

The weakest path is Currency Dominance, which requires exactly the kind of patient network-building and population management that the Blood Production disadvantage makes structurally expensive. A depleted population base cannot sustain the trade network that Currency Dominance requires, and the forge-culture disposition toward direct acquisition over negotiated arrangement makes the trade relationship management feel like wasted effort.

### Conviction Axis

A Moral Ironmark playthrough channels the forge-culture aggression through a code of conduct — the honor of acknowledged combat, the respect for worthy enemies, the care for soldiers who have bled in the house's service. The Moral conviction axis creates a fighting culture that is brutal by many measures but internally coherent: Ironmark soldiers know what they are fighting for, they know how they are expected to fight, and the faith covenant that Moral Ironmark practices gives the blood cost meaning. A Moral Ironmark Legendary Member is the match's most respected warrior-figure — feared by enemies, valued by allies, recognized by all parties as someone who has earned their position through the most direct possible demonstration.

A Cruel Ironmark playthrough removes the code of conduct and replaces it with pure efficiency. The brutal elements of the forge culture — the aggressive fighting posture, the willingness to take casualties — are divorced from any framework of honor and become instruments of fear. Cruel Ironmark is harder to ally with because the calculus of what happens to defeated parties is known. It generates stronger fear-compliance in governed territories than any other house, which compensates partially for the Legitimacy Erosion that Military Conquest always generates. The Cruel conviction axis with Ironmark's military capability produces a dynasty that other parties are right to fear, and that they will organize against with corresponding urgency.

### Key Rivalries and Relationships

Ironmark's most fraught relationship is with Trueborn. The administrative house's insistence on formal process, legitimate grounds, and precedent-based governance is a direct negation of the forge-house's practical orientation toward results. When Trueborn attempts to hold Ironmark to diplomatic conventions that Ironmark does not recognize as binding, the conflict is not between two houses who disagree about a specific policy — it is between two houses who have fundamentally different theories of what makes authority legitimate.

Ironmark's most productive relationship tends to be with Hartvale, which is counterintuitive given how different the houses are. The explanation is practical: Hartvale's population resilience and Ironmark's forge capacity are natural complements. Ironmark produces the military output; Hartvale produces the population base that sustains the military over time. The relationship is never easy — Hartvale's care for its people makes it uncomfortable with the Blood Production costs that alliance with Ironmark generates — but the structural complementarity is real enough to make the alliance worth the friction.

### Legendary Member Archetype

The Ironmark Legendary Member is typically the Forge-General: a figure who came up through the smithing culture, who commanded at every level before reaching the top, and whose physical bearing is visibly shaped by decades of hard work and harder combat. Not a political figure. Not an administrative figure. A person who knows what their soldiers are experiencing because they experienced it themselves, who makes tactical decisions with the instinctive speed of someone who has processed thousands of engagements and whose pattern recognition operates faster than conscious deliberation. Other dynasties' Legendary Members are impressive in the political and diplomatic space. The Forge-General is impressive in the field, where being impressive is the most direct expression of power. Enemies who have faced the Forge-General and survived carry the memory of it. Those who haven't heard the accounts second-hand and adjust their behavior accordingly.

### Flavor Paragraph

Ironmark smells like iron and char. Not metaphorically — the house's people carry the smell of the forge on their skin, in their hair, on the wool of their clothing. It is the smell of productive labor pushed to its limits, of heat that does not dissipate between sessions because the forge does not stop. They are compact and physical in the way of people who have done manual work long enough that the work has shaped the body, and they move with the economy of motion that comes from years of doing things that had to be done right or someone got hurt. In a room of political figures making arguments, Ironmark stands slightly apart — not hostile, not imperious, but present in a way that is different from the others, as though they are in the room because someone needed to be here and no one else volunteered. They are not comfortable in the diplomatic register. They are not pretending to be. The Ironmark representative at a summit is thinking about what happens after the summit ends, when the words have to be made real by something other than words.

---

## Section 56: House Goldgrave

**Hair color:** Bright gold

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 57: House Stonehelm

**Hair color:** Brown

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 58: House Westland

**Hair color:** Vivid red (Westgrave lineage)

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 59: House Hartvale

**Hair color:** Orange (Hartborne lineage)

**Unique unit: The Verdant Warden** — LOCKED
Off 4 / Def 5. Hybrid guardian. Can fight but is primarily a defender. Provides settlement defense bonuses and population loyalty bonuses in its zone. Represents Hartvale as protectors of their people rather than conquerors.

Full strategic design pending.

*(CB004 content voided 2026-04-07. Only the Verdant Warden spec is settled for Hartvale. Full house design to be developed in a future session.)*

---

## Section 60: House Whitehall

**Hair color:** Neutral brown

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Section 61: House Oldcrest

**Hair color:** Black/gray/pepper

Strategic design pending.

*(CB004 content voided 2026-04-07. Settled elements: hair color, color theme. Full house design to be developed in a future session.)*

---

## Part IX Addendum: House Relationships at Match Start

*Canonicalized 2026-04-07 per Lance Fisher direction. These are the default political tensions and natural affinities between founding houses at the moment the match begins. They are tendencies, not locks — player action can alter all of them. But they define the political landscape the player enters and generate the natural first-act story beats of every match.*

| Pair | Default Relationship | Notes |
|------|---------------------|-------|
| Goldgrave ↔ Whitehall | Economic rivalry | Both want control of money flow and information access. Different methods — Goldgrave through currency, Whitehall through relationships — aimed at the same strategic dominance. |
| Ironmark ↔ Stonehelm | Military respect and wariness | Two military cultures with completely different doctrines. Ironmark is aggressive and mobile. Stonehelm is defensive and positional. Mutual recognition of capability, mutual suspicion of intent. |
| Highborne ↔ Trueborn | Prestige competition | Both houses believe in the quality of blood and the legitimacy of lineage. Highborne resents the moral gravity the Trueborn name carries — a gravity it cannot claim and cannot fully counter. |
| Hartvale ↔ Stonehelm | Natural partnership | Stonehelm wants food security; Hartvale wants military protection. The two houses offer each other exactly what the other lacks. Natural alliance unless a third party drives a wedge. |
| Oldcrest ↔ Westland | Cultural friction | Oldcrest's conservatism and long memory versus Westland's frontier impulse and aggressive expansion. Oldcrest has seen what happens when houses push too far too fast. Westland does not particularly care what Oldcrest has seen. |
| Trueborn ↔ Blood Dominion dynasties | Ideological tension | The Trueborn name carries Old Light resonance — the city, the Compact, the legitimacy of the match's neutral center all invoke the covenant of remembrance and protection. Blood Dominion dynasties know this and respond to it. |
| Whitehall ↔ All houses | Default diplomatic access | Whitehall maintains a functional relationship with every house by design. This is not warmth — it is infrastructure. Every house understands the value of keeping Whitehall close. |

These starting relationships shape the natural first-act diplomatic landscape of every match. A player who understands them can read the board faster. A player who ignores them discovers them through consequences.

---

## Section 62: The Multi-Speed Time Model

### The Central Design Problem

Bloodlines asks two things of its player simultaneously that appear to be in tension. It asks them to manage live real-time battlefield engagements with the immediacy and responsiveness of Command and Conquer-style play. And it asks them to steward a dynasty across multiple generations, managing births, marriages, development arcs, succession, and the slow accumulation of dynastic identity. These two demands operate at completely different time scales.

The game does not resolve this tension by choosing one mode over the other. It resolves it by running multiple time speeds simultaneously and giving the player the tools to navigate between them.

### Three Simultaneous Time Speeds

**Battlefield Time** is the fastest layer. It is real-time. When the player is on a battlefield map in direct command, units move, fire, and die in real time. Tactical decisions resolve in seconds. A battlefield engagement that the player participates in directly might last 15 to 40 minutes of actual session time. Battlefield Time does not affect the other speeds — while the player is on the battlefield, campaign events and dynastic events continue to progress in the background.

**Campaign Time** is the medium layer. It governs regional army movement, supply route operation, territory consolidation, trade route yields, diplomatic exchange windows, and faith spread. Events that take days or weeks in the world of Bloodlines unfold over minutes of session time at Campaign speed. The 90-second cycle documented in Section 46 is the campaign time heartbeat. Every 90 seconds, Campaign Time processes resource accumulation, population changes, army supply, trade yields, faith intensity shifts, conviction drift, and event triggers.

**Dynastic Time** is the longest layer. It governs the full generational arc: births, childhood development, adolescent training, adult maturation, productive career, aging, and natural death. A character moving from birth to full adult capability covers what in the game world would be roughly 16-22 years of elapsed time. The match's campaign-time duration determines how much dynastic time elapses. In a two-hour session with appropriate compression, one generation can complete. In a six-to-ten-hour match, multiple generations are achievable.

### How the Three Speeds Coexist

The three speeds are not sequential. They run simultaneously. The player's attention determines which one is experientially primary at any moment.

When the player is engaged in direct battlefield command, Battlefield Time is primary. The player's decisions are tactical and resolve in seconds. Campaign Time continues ticking in the background — regional armies continue their assigned movements, trade caravans continue their routes, enemy dynasties continue building. Dynastic Time continues as well — an heir who was developing before the battle is still developing during it.

When the player zooms out to regional or sovereign command, Campaign Time becomes primary. Battlefield engagements at this level that the player is not directing personally resolve under AI direction. The player sees the results as they become available. They can zoom back into any of these battles at any time and take direct control.

Dynastic events surface across all time speeds through the notification and queue system described below.

### The Notification and Queue System

Dynastic events do not pause live warfare. They do not demand immediate response in the middle of a battlefield engagement. They enter a management queue that the player can address at natural pauses — between engagements, during zoom-out periods, or during lower-intensity match phases.

Events in the queue are tagged by time sensitivity:

**Immediate** — Events that require response within a small number of campaign cycles. A succession crisis triggered by an unexpected death. A Covenant Test window opening with a finite decision period. These events are flagged prominently and carry a countdown visible at any zoom level.

**Soon** — Events with a longer response window but meaningful consequences if ignored. A marriage proposal with a defined acceptance window. A bloodline member reaching adulthood and requiring role assignment. A territorial loyalty threshold approaching revolt. These surface as persistent notifications that escalate if unaddressed.

**Persistent** — Events that benefit from attention but do not have hard deadlines. A bloodline member's development milestone reached. A trade agreement renewal opportunity. A tribal relationship crossing a positive threshold. These accumulate in the queue without escalation.

The player who regularly zooms out and processes their queue manages the dynasty proactively. The player who remains immersed in battlefield command for extended periods accumulates queue events. Some of those events will have progressed toward worse outcomes by the time they are addressed. This is not a punishment for playing the game directly — it is the natural consequence of command burden and delegated oversight, which is one of the game's core design commitments.

### Births, Childhood, Adolescence, and Maturation During Live Play

When a marriage produces a child, the child enters the bloodline record immediately as a future asset and present vulnerability. They do not become active or consume an active roster slot. They progress through developmental stages that are tracked in Campaign Time compression.

**Infancy** surfaces as a bloodline record entry and a Keep presence. Minor early-environment decisions can be assigned — who raises the child, what foundational values they are exposed to — but no active role is available.

**Childhood** generates a development milestone notification. The player assigns training direction: military, governance, faith, or operations. This decision has consequences but is not irrevocable — the training path can be adjusted at cost during adolescence.

**Adolescence** generates a supporting-role eligibility notification. The developing member can be assigned to accompany an active member in their role, gaining experience at a junior level. They cannot lead independently. Their development progress is visible in the Keep.

**Adulthood** generates an activation eligibility notification. If an active roster slot exists among the 20-member cap, the player can activate them immediately with a role assignment. If the roster is full, they enter dormancy — tracked, aging, developing further relationships and characteristics, but not deployed.

Each developmental transition surfaces in the queue at the appropriate time sensitivity level. None of them interrupt a battlefield engagement. All of them reward the player who checks the queue regularly.

### Generational Change Without Breaking Real-Time Identity

The critical design principle is that the battlefield layer is always real-time and always available. Generational change happens in Campaign and Dynastic time in the background. The player who wants to play a predominantly tactical game can do so — the dynastic layer will continue developing, notifications will accumulate, and the match will be influenced by generational events they may not have fully managed. The player who wants to engage fully with the dynastic layer must regularly zoom out to manage it.

The game does not become turn-based to accommodate dynasty. Dynasty operates in parallel with real-time play. The player navigates between the speeds by zooming. When they zoom in, they are in real-time. When they zoom out, they are in campaign or dynastic time. The design challenge is making this transition feel seamless rather than jarring — the player should always know which speed they are operating in and what is happening at the other speeds.

### Match Length and the Generational Arc

**Two-Hour Match** — Dynastic time is compressed such that approximately one full generation matures during the session. A bloodline member born in the first major campaign phase can reach full adult capability and begin contributing before the match resolves. One succession event is achievable for most dynasties. The match is a single-generation story with the beginning of the next visible at its conclusion.

**Six-Hour Match** — Two full generations are achievable. A dynasty that invested in strategic marriages early in the match may find its second generation has a bloodline composition — traits, capabilities, faith alignment — that reflects those marriage choices in ways the first generation could not predict. The match is a two-generation story with genuine dynastic arc.

**Ten-Hour Match** — Three or more generations are achievable. The founding generation that opened the match may still be alive — aged, reduced in some capabilities, elevated in others — when grandchildren reach adulthood. The full generational depth of the dynasty system is accessible. The founding decisions of generation one have compounded into the bloodline characteristics of generation three in ways that are legible to anyone examining the bloodline record.

---

---

# PART X — OPERATIONS SYSTEM

---

## Section 63: Operations System Architecture

### The Three Categories

Every operation in Bloodlines follows the same core loop: **Commit → Navigate → Execute → Consequence**. The player assigns an operative (or army), physically controls or directs them through the world, meets the operation's execution conditions, and then deals with the outcome — both the immediate effect and the systemic ripple (conviction, diplomacy, faith intensity, population reaction).

The system introduces three distinct operation categories that give the player strategic verbs beyond "build" and "fight":

**Covert Operations** are executed by Rogue-class units and Spymaster-committed bloodline members. Intelligence, sabotage, theft, assassination. The player physically navigates the operative through enemy territory in real-time. The C&C engineer infiltration parallel — sneaking a fast unit past defenses for a high-value objective — is the core gameplay feel. Rogues reward player micro-skill and patience.

**Faith Operations** are executed by Mystic-class units (covenant-specific) and Priest/Priestess bloodline members. Offensive rituals, defensive wards, utility divination. Some require cosmic timing (moon phase, solstice), some require gathered components, some require positional alignment. Faith operations consume faith intensity as fuel — the resource the player spent infrastructure building up. Powerful operations require higher intensity thresholds to attempt, and reckless casting weakens the covenant's foundation. Mystics reward strategic planning and calendar awareness.

**Military Operations** are selected when committing an army to assault a territory. The player chooses an operation doctrine — Conquer, Plunder, Pillage, Raze, Siege, or Massacre — that governs how the army behaves during and after engagement. This affects targeting priority, resource yield, conviction consequence, and post-battle territory state. The army fights regardless; the doctrine determines what they do with their victory.

Operations are not abstract click-and-wait actions. The player controls the operative unit on the map in real-time, navigating real terrain, real defenses, real risk. Every operation has execution conditions, failure states with real consequences, and interactions with conviction, faith, and diplomacy. There is no consequence-free action.

### Design Origin

This system draws from Utopia's thievery/mystic/attack operation categories, translated from abstract percentage-roll mechanics into Bloodlines' real-time tactical execution model. The addition of timing as a universal strategic layer — where the *when* matters as much as the *what* and *where* — deepens the system beyond its source inspiration.

---

## Section 64: Covert Operations

The covert roster contains 10 named operations organized into three subcategories: Intelligence (4), Sabotage (3), and Direct Action (3). All are executed by Rogue-class units (squad size 3, Off 3/Def 2, fastest non-cavalry infantry, invisible unless detected) or by Spymaster bloodline members personally (higher success potential, catastrophic risk if captured).

**Intelligence Operations** provide the information that makes everything else possible. Scout Defenses reveals garrison strength and fortification layout. Infiltrate Court exposes diplomatic agreements and pending betrayals. Map Network reveals trade routes and resource flow. Identify Bloodline reveals the locations, roles, and traits of a rival dynasty's active bloodline members — the most dangerous intelligence operation because it requires reaching the target's Keep.

**Sabotage Operations** degrade the enemy's capacity without direct military confrontation. Arson destroys specific buildings. Poison Supply contaminates food or water, reducing population health and morale. Sabotage Infrastructure disables military or economic buildings for multiple cycles. Each requires the rogue to reach the target building and complete a timed interaction — discovery mid-operation means failure with consequences.

**Direct Action Operations** are the highest-risk, highest-reward covert plays. Assassinate targets a specific bloodline member — the rogue must reach the target's physical location and execute a timed strike. Kidnap requires extracting a bloodline member alive back to friendly territory. Incite Revolt destabilizes a territory through sustained population agitation over 3+ cycles, requiring the rogue to operate deep in enemy territory for an extended period.

Every covert operation has a timing dimension: fog and night improve concealment; festivals provide crowd cover; harvest season maximizes sabotage impact on full granaries; winter contamination compounds starvation pressure.

---

## Section 65: Faith Operations — The Four Covenants

Each covenant has 10 faith operations: 3 offensive, 3 defensive/utility, 2 light doctrine-specific, and 2 dark doctrine-specific. Mystic-class units are covenant-specific — an Old Light Mystic cannot cast Blood Dominion operations.

### Old Light Operations

Old Light's offensive operations channel fire, light, and ancestral memory. Pillar of Dawn summons a column of burning light at actual dawn (blocked by overcast weather). Ancestral Judgment breaks enemy morale through visions of ancestral wrath. Searing Brand reduces rival faith intensity in a territory for 5 cycles.

Defensively, Beacon of Memory provides complete vision of all movement in a large radius. Veil of the Ancestors conceals a friendly army from detection. Restoration of the Hearth heals population health and morale.

Light doctrine unlocks Chronicle Ward (permanent territory protection against covert operations and faith erosion) and Light of Revelation (exposes all hidden enemy operatives in a territory). Dark doctrine unlocks The Inquisition (systematic purge of rival faith and disloyal population — major Cruel conviction shift) and Pyre of Purification (destroys a rival faith building with holy fire — severe consequences).

### Blood Dominion Operations

Blood Dominion operations use blood, sacrifice, and life-force as currency. Blood Tide curses a target army so wounds resist healing. Sanguine Pact binds two enemy units so damage transfers between them. Hemorrhage drains resources from a territory over time.

Defensively, Bloodward creates invisible perimeter alerts. Covenant Shield absorbs damage at the cost of a squad member. Blood Sight reveals tactical intelligence through self-bleeding ritual.

Light doctrine unlocks Rite of Willing Sacrifice (volunteers empower an army with supernatural vitality — population loyalty increases because they chose to give) and Oath of Blood Brotherhood (permanently bonds two allied armies with shared healing). Dark doctrine unlocks Exsanguination (drains captive population to restore your army — extreme Cruel shift) and Blood Curse (lasting health degradation on a rival dynasty's bloodline members).

### The Order Operations

The Order channels structure, law, and enforcement. Edict of Condemnation reduces a target dynasty's diplomatic standing through formal legal pronouncement. Iron Shackles slows a target army and prevents retreat. Mandate of Seizure legitimizes territorial conquest, reducing conviction costs if the justification is genuine (but doubling them if fabricated and exposed).

Defensively, Codex of Law permanently raises a territory's loyalty floor. Adjudicator's Eye reveals a target dynasty's true conviction and hidden diplomatic agreements. Garrison Discipline permanently improves garrison defensive effectiveness.

Light doctrine unlocks Shield of the Just (enemies attacking a shielded defensive army suffer conviction costs) and Writ of Sanctuary (territory becomes formally protected neutral ground). Dark doctrine unlocks Martial Law (absolute control eliminating dissent at the cost of economic output and growth) and Trial by Ordeal (public execution of accused — loyalty through fear).

### The Wild Operations

The Wild channels nature, predation, and the cycles of life and death. Call of the Pack summons predatory animals to attack a target (uncontrollable once summoned). Rot and Ruin accelerates agricultural decay through Blight Spores. Feral Terror overwhelms a territory's population with primal fear through howling rituals.

Defensively, Verdant Regrowth restores depleted food and wood production. Nature's Veil conceals armies moving through forest terrain. Beast Communion provides passive intelligence on all movement through animal populations.

Light doctrine unlocks Harmony of Seasons (season-long ritual permanently increasing food production by 25%) and Guardian Spirit (permanent territorial patrol unit that detects covert operatives). Dark doctrine unlocks Blight Plague (devastating agricultural plague across multiple territories — severe Cruel shift) and Apex Predator (transforms a military unit into an uncontrollable feral monster with +4 Off/+2 Def that attacks everything for 3 cycles before dying).

---

## Section 66: Military Operations

Six military operation doctrines define what an army does with its victory:

**Conquer** seizes and holds territory with infrastructure intact but low starting loyalty. The standard military expansion action. Dawn assault grants morale bonus.

**Plunder** strips portable resources and withdraws — no territory claim. Harvest season maximizes yield. Minor Cruel conviction shift.

**Pillage** systematically destroys economic and civic infrastructure, leaving the territory devastated but unclaimed. Dry season amplifies fire damage. Moderate Cruel shift.

**Raze** destroys everything — all buildings, all fortifications. Territory becomes unclaimed wasteland. Severe Cruel shift with near-universal condemnation.

**Siege** encircles and starves a fortified position into surrender. Yields intact territory with higher starting loyalty than Conquer. Winter siege is devastating. Moral if surrender is accepted fairly; Cruel if civilians are deliberately starved.

**Massacre** eliminates military and civilian population. Territory claimed but nearly empty. Extreme Cruel shift — the single most condemnable military action. Automatic Trueborn coalition trigger. Your own population loyalty drops across ALL territories. A Moral conviction dynasty's army physically refuses to execute Massacre — this is the only hard-lock in the operations system.

Operation doctrine is chosen before engagement and defenders do not know which doctrine the attacker has chosen until behavior becomes apparent.

---

## Section 67: The Timing System

Time is a universal strategic layer. Every operation is affected by when it happens.

### Day/Night Cycle
A full cycle takes ~20 minutes real-time: Dawn (~2 min), Day (~8 min), Dusk (~2 min), Night (~8 min). A 6-hour match covers roughly 18 in-game days. Dawn grants morale bonuses for assaults. Night improves covert operations but imposes terrain penalties on attackers. Dark faith operations are amplified at night.

### Lunar Cycle
Four phases rotate every 4-5 in-game days: New Moon (maximum darkness, covert operations peak, Blood Dominion curses amplified), Waxing Moon (growing power, Blood Dominion vitality), Full Moon (maximum night visibility, Old Light operations amplified, The Wild predator aggression peaks), Waning Moon (The Order operations strengthened, defensive faith at peak).

### Seasonal Cycle
Four seasons rotate every 4-5 in-game days: Spring (planting, mud slows cavalry, The Wild operations amplified), Summer (peak food, dry ground favors cavalry, fire spreads fastest), Autumn (harvest fills granaries maximizing Plunder, Blood Dominion sacrifice rituals most potent), Winter (food consumption up, siege devastating, The Order strengthened, snow tracks expose covert operatives).

### Celestial Events
Rare events occurring 2-3 times per match, visible to all players in advance: Solar Eclipse (faith operations cost 50% less, Old Light weakened), Blood Moon (Blood Dominion intensity regenerates 3x, The Wild animals frenzied), Solstice (summer extends daylight and Old Light power; winter extends night and covert/Order power), Comet Sighting (random, conviction event for all dynasties, double duration on all faith operations during visibility).

### Dishonorable Timing Combinations
Cruel conviction dynasties gain access to devastating operation sequences with compounding conviction costs. Night Burning + Dawn Assault disrupts defenders before the main attack. Poison Supply + Winter Siege halves siege duration. Massacre + Feral Terror empties a territory completely. The most sophisticated play — Incite Revolt + Conquer — destabilizes from within and arrives as a liberator, achieving similar outcomes at near-zero conviction cost if the revolt is not traced back.

The design principle: the cruelest actions are the most obvious. The most sophisticated plays minimize conviction cost while achieving similar outcomes. A dynasty that burns housing and massacres populations will win battles but lose the political war. A dynasty that destabilizes from within and arrives as a liberator wins both.

---

## Section 68: Faith Switching

Players can switch their dynasty's covenant faith after the initial selection at the end of Stage 1, but at enormous cost. Faith intensity resets to zero. All faith buildings must be re-consecrated. A percentage of the population that followed the previous covenant revolts or emigrates. Bloodline members in faith roles must convert (which may fail). Former faith allies may break alliances. All faith-specific military units (Level 3+) become unavailable until new faith intensity reaches required thresholds.

Faith switching is designed to be possible because the choice must exist. But the cost is designed to make it genuinely a last resort — a decision made in desperation or as part of a radical strategic pivot, not a casual optimization.

---

# PART XI — CLOSING

## Section 69: Design Philosophy — What Bloodlines Must Always Be

### The Question

Every civilization in history has been organized around the same four questions, whether it acknowledged them or not. Who controls the land and what can be extracted from it. Who feeds and houses the people and what those people will do in return. What the people believe and what that belief demands of them. Who leads and whether that leadership is legitimate enough to survive being challenged. Territory. Population. Faith. Dynasty.

These are not four themes that Bloodlines incorporated because they seemed interesting. They are four things that are actually true about how human civilization functions. The game exists because the intersection of those four questions — the way they push against each other, reinforce each other, corrupt each other, and occasionally produce moments of genuine coherence — is the most interesting set of dynamics that political strategy as a genre has not yet fully explored. Bloodlines is the attempt to explore it fully.

To rule something worth ruling means to have a position on all four questions simultaneously. Territory that cannot be fed into population is a desert. Population without faith is a crowd without a reason. Faith without territory is a dream. Dynasty without all three is a name. The sovereignty seat is not the seat of the dynasty with the best army or the most gold or the purest faith. It is the seat of the dynasty that found the right answer to all four questions at the same time and held that answer together long enough for the world to recognize it.

### No Ceiling

Bloodlines has no ceiling. There is no point at which the project is too big, too detailed, too deep, or too complex. Conventional game development scope constraints — feature cuts, feasibility trims, complexity reductions, scope management in service of shipping on a timeline — do not govern this project. They are appropriate tools for products that must be finished by a specific date for commercial reasons. This is not that product yet. Right now, Bloodlines is a design, and the design must go as deep as the subject demands.

The subject demands very deep treatment. Political power, religious conviction, dynastic legacy, population stewardship — these are not simple subjects. They have been generating literature, philosophy, history, and conflict for the entirety of recorded human experience. A game that takes them seriously cannot be shallow. A game that takes them seriously and has no ceiling can become as deep as the designer is willing to make it. The risk is not building too much. The risk is building too little and calling it done.

Every session should make the design richer. When a new system is described, the question is not whether it fits within current scope — the scope expands to contain what the design requires. When a house identity generates a new question about how that house would interact with a mechanic, the answer goes into the design, not into a parking lot for later. The parking lot does not exist. What exists is the design in its current state and the direction of growth from that state.

### The Register

This is not a game for children. Not because it contains content inappropriate for children — though it does — but because the subjects it addresses require the emotional and intellectual capacity to engage with genuine complexity. Political power is not clean. The faith doctrines that demand sacrifice demand real sacrifice. Dynasty politics produces the full range of human behavior, including the worst of it. The conviction system tracks the moral character of a civilization, which means it must represent both moral poles accurately and without sanitization.

Cruel conviction is not the villain's path. It is a real orientation that real civilizations have practiced, that produced real outcomes — some of them historically significant, some of them catastrophic — and that the game must represent at the level of seriousness it deserves. Moral conviction is not the hero's path. It is a different real orientation with different real tradeoffs, different real costs, different real advantages. The game is not structured to tell the player which axis is correct. It is structured to show what each axis produces and to make the player live with what they chose.

The faith doctrines are adult in their demands and their consequences. They ask for things that are difficult to give. The Born of Sacrifice system — the most extreme expression of faith in the game — produces power through cost that is genuinely costly. The rituals mean something in the game world. They produce measurable effects. They are not decoration.

The political violence is consequence-bearing. Military defeat kills people the dynasty has named, developed, and invested in. Succession crises fracture families. Betrayal produces lasting damage that does not reset at the end of the cycle. The game keeps a record. The Bloodline Members who die in battle are not respawning units — they are concluded lives, and the match world changes because they concluded.

This is the register. The Game of Thrones register, as a reference point for tone and seriousness and the willingness to let consequences be real. Not derived from that property. Not a copy of it. Operating at that level of political and human seriousness, because that is the level at which this subject matter genuinely operates.

### The Story Generator

Every match of Bloodlines should produce a story worth retelling. Not a score summary — a story. Something with characters, with choices that mattered, with moments of genuine drama that emerged from the systems rather than from scripted events. The systems must generate narrative moments that feel authored even though they are emergent. When a player finishes a match and describes what happened to someone who was not there, the description should be a story with a beginning, middle, and end, with named figures and their fates, with turning points that changed the match's direction, with a resolution that felt, in retrospect, like the consequence of everything that came before it.

This is the test. Not: did the systems function correctly? Not: did the player achieve a victory condition? The test is: does the player have a story to tell? A great Bloodlines match is remembered the way great fiction is remembered — not as a sequence of events but as a narrative with emotional shape, with moments that mattered, with figures whose choices created consequences that the player is still thinking about days later.

The dynasty is the vehicle for this. The Bloodline Members are the named figures whose choices drive the narrative. The faith covenant is the framework that gives those choices moral weight. The conviction axis is the character of the civilization they built. These four things together — dynasty, member, faith, conviction — produce a story every time, in a different configuration every time, that no one could have scripted because it emerges from the specific combination of decisions, events, and responses that constituted this particular match.

### Why the Seat Is Denied

The sovereignty seat is designed to be hard to reach. Genuinely, structurally, mechanically hard — not hard as a matter of difficulty setting but hard as a design commitment about what the seat represents. The game resists clean solo conquest because clean solo conquest is not what legitimate authority looks like in the world Bloodlines is modeling. Legitimate authority is contested, negotiated, earned across multiple domains simultaneously, and recognized by parties who were not compelled to recognize it. The systems that resist individual dominance — the Great Reckoning, the Trueborn Summons, the Alliance Victory requirement for genuine coalition formation, the Territorial Governance requirement for genuine population administration — are not obstacles between the player and a reward. They are the definition of what the reward is.

If the sovereignty seat could be taken by superior military force alone, it would be a throne, not a seat of sovereignty. The game's distinction between those two things is the distinction between the player who has the biggest army and the player who has built something that the world recognizes as legitimate. The army can hold the throne. Only the civilization can hold the seat.

No bloodline wins by being second. The match's design intent ensures that — of the paths available, none produce a meaningful outcome short of the sovereignty seat. Exile is not second place. Alliance Victory junior partner is not second place. These are states within the match, not endpoints. The match ends when the seat is recognized. Everything before that recognition is the story.

### The Final Statement

Bloodlines is a game about what it costs to build something that outlasts you.

The dynasty is the emotional core. The military is the expression. The faith is the framework. The conviction is the character. These four things together produce a story, every time, that no one could have scripted.

---

# PART XII — COMMAND, WORLD, AND STRATEGIC DOCTRINE

*This part contains sections added in the 2026-04-07 revision. All content is additive and supplements existing sections without replacing them. Cross-references to existing sections are noted where relevant.*

---

## Section 70: Command and Zoom Architecture

*Supplements Section 62 (Multi-Speed Time Model). Together these two sections fulfill the governing prompt's Part IV and Part V requirements for a complete time and command model.*

### The Zoom Layer Model

Bloodlines is organized around three command layers that the player navigates by zooming in and out. These are not separate game modes — they are perspectives on the same continuous world, with different information density and different decision scales appropriate to each perspective.

The experience should feel like Command and Conquer-style maps strung together into a larger war picture, with the ability to zoom out to see the whole theater and issue orders at that scale.

### Layer One: Local Battlefield Control

At the most zoomed-in level, the player exercises complete direct control over a single battlefield engagement. This is Command and Conquer-style play in feel and responsiveness. Every unit can be individually directed. Bloodline heroes can be micro-managed. Resource gathering on the battlefield map can be optimized. Defensive positions can be adjusted. Production queues can be managed.

The player is never forced to use less granularity than they want. If they want to micro every unit in a five-squad engagement, they can. If they want to select all units and issue a group attack order, they can. Both are valid approaches.

**What is available at Local Battlefield:**
- Individual unit selection and direction
- Squad-level formation commands
- Bloodline hero ability activation and micro-management
- Battlefield resource gathering management
- Base building and defensive installation placement
- Production queue management
- Tactical retreat and advance orders

**What is not available at Local Battlefield:**
- Regional army movement orders
- Diplomatic actions
- Trade route management
- Bloodline role reassignment for non-present members
- Faith ritual activation for non-battlefield rituals

**Transitions:** Zooming out from Local Battlefield moves to Regional Command. The transition is smooth — the camera pulls back and the tactical view gives way to the regional map. The engagement the player was just directing either continues under AI direction (the army following its last assigned posture and doctrine) or pauses at a natural moment if the player chooses to manually pause before transitioning.

### Layer Two: Regional Command

At the regional level, the player sees multiple battlefield zones simultaneously in a strategic view. Active battles appear as engagement indicators with status summaries. Armies not currently engaged show their positions, movements, and assigned postures. Supply routes and their status are visible. Minor tribe territories and their relationship status are displayed.

The regional layer is where the Command and Conquer-maps-strung-together feeling is most explicit. A player who has been fighting across a sequence of adjacent territorial zones sees those zones from the regional level as a connected campaign — advances, holding positions, contested areas, and the supply infrastructure connecting them all.

**What is available at Regional Command:**
- Army movement orders between zones
- Posture assignment (Attack, Defend, Reinforce, Patrol, Siege)
- Bloodline commander assignment to armies
- Supply route monitoring and rerouting
- Minor tribe diplomatic contact initiation
- Trade caravan direction
- Observation of AI-directed battle outcomes as they resolve

**Army Direction and Posture System:**

When the player assigns an army to a posture at the regional level, they define the operational parameters the army follows when acting under AI direction:

*Attack posture* — the army advances toward the assigned objective and engages any resistance it encounters. Doctrine settings define whether the army conserves strength (pauses on heavy resistance, waits for reinforcement) or accepts attrition to achieve the objective.

*Defend posture* — the army holds its current position and engages any forces that move toward it. Retreat threshold defines at what force ratio the army withdraws rather than fights a losing engagement.

*Reinforce posture* — the army moves toward a designated engaged army and integrates into that engagement on arrival. Useful for feeding reserves into active battles.

*Patrol posture* — the army moves along a defined route, engaging threats encountered along the route and returning to the route after each engagement. Primarily used for supply route protection and border security.

*Siege posture* — the army encircles a designated fortified position and applies sustained pressure. No direct assault unless the player switches to Attack posture or personally directs the assault.

**Bloodline Commanders:**

Armies assigned bloodline commanders fight AI-directed battles at a quality level that reflects the commander's development and capabilities. An experienced War General commanding a regional army produces better AI-directed outcomes than the same army without a bloodline commander. The player can see bloodline commander status at the regional level and can reassign commanders between armies.

A bloodline commander present in an AI-directed battle that results in a defeat faces the same death probability that the same commander would face in a player-directed battle with equivalent outcomes. The player must decide whether to personally attend battles where bloodline members of significance are deployed.

**Zooming into an Active Battle:**

From the regional level, the player can zoom into any active battle at any time. The transition moves them directly into Local Battlefield control of that engagement. If the AI had been directing the battle, the army's current state — positions, health, ongoing actions — is what the player inherits when they take direct control. They do not start from the beginning of the engagement. They join it in progress.

This transition should be immediate and seamless. The player should never feel that they are loading into a separate mode. They are zooming into a different perspective on the same continuous world.

### Layer Three: Sovereign and World Command

At the most zoomed-out level, the player sees the full game world: all active dynasties, their territorial control, major cities including the Trueborn City, faith spread by region, trade route networks, and the positions of all known major actors. The sovereign layer is where the player thinks about the match as a whole.

**What is available at Sovereign Command:**
- Bloodline role assignments across the full roster
- Diplomatic actions with any dynasty or the Trueborn City
- Major strategic orders that cascade to regional level
- Victory path progress monitoring
- Faith spread and global faith distribution review
- Currency network status review
- Bloodline member queue management (addressing all pending dynastic notifications)
- Alliance negotiations

**Macro Orders:**

From the sovereign level, the player can issue macro orders that cascade downward. Declaring war on a dynasty from the sovereign level automatically adjusts the posture of all armies adjacent to that dynasty's territory to Attack or Defend depending on their current positioning. Entering a trade agreement from the sovereign level automatically establishes the trade routes between the relevant settlements.

The player does not need to go back to the regional level to execute the consequences of sovereign decisions — the consequences flow downward automatically. The player can then zoom to the regional or battlefield level to monitor execution if they choose.

### How the Layers Interact During Active Warfare

The most complex situation is when the player is managing an active battle at the battlefield layer while significant events are occurring at the regional and sovereign layers simultaneously. This is the game's highest-intensity state.

The notification system handles this by surfacing regional and sovereign events as non-intrusive alerts at the edge of the battlefield view. These alerts are categorized by urgency (using the Immediate / Soon / Persistent taxonomy from Section 62). Immediate alerts generate a distinct visual and audio signal. Soon and Persistent alerts are visible but do not interrupt.

The player can choose to zoom out mid-battle, leaving the battlefield engagement under AI direction with the army's last assigned posture, address the regional or sovereign event, and zoom back in. The battle will have continued in their absence. If the battle was going well, it may have progressed further. If it was going poorly, the AI may have made decisions the player would not have made.

This is a real tension and a real design choice. The player is always trading completeness at one layer against completeness at another. The game does not resolve this tension. It sustains it as the source of the match's ongoing decision pressure.

### Bloodline Heroes Across Zoom Layers

A bloodline hero with a military role exists simultaneously in the game world regardless of which layer the player is viewing. Their physical position is tracked. Their health, development, and current combat status are tracked.

At the Local Battlefield layer: bloodline heroes can be directly controlled, their abilities can be manually activated, and they can be micro-managed through engagements.

At the Regional layer: bloodline heroes appear as command indicators on armies. Their status (healthy, wounded, engaged) is visible. They can be reassigned to different armies.

At the Sovereign layer: all active bloodline members are visible in the roster view. Their current assignments, development status, and pending role changes are manageable.

The risk of a bloodline member dying in an AI-directed battle is always present when they are deployed in a military role. A player who has zoomed to the sovereign layer to manage a diplomatic crisis and whose bloodline commander was involved in an AI-directed battle that went badly will see the death notification in their queue. The death is real. The notification does not prevent it.

### AI-Directed Battle Resolution

When a battle resolves under AI direction (the player is not present at the Local Battlefield layer), the resolution follows these principles:

The outcome is determined by: army composition, total strength and quality of forces on each side, bloodline commander capabilities and doctrine, posture assignments, terrain type and associated modifier, supply status, and faith intensity effects on the army.

The AI is not given information the player does not have. The AI opponent does not have fog-of-war immunity or resource advantages. The AI-directed friendly army is not given perfect tactical execution. The resolution reflects realistic probability based on the factors above, with a variance band that occasionally produces unexpected outcomes in either direction.

The player who has invested in strong bloodline commanders, well-supplied armies, and thoughtful posture assignments will see better AI-directed outcomes than the player who has neglected these factors. AI-directed battles are not coin flips — they are probability-weighted resolutions whose probability the player can meaningfully influence through preparation.

---

## Section 71: Continental World Map Architecture

*Supplements Section 20 (World Generation) and Section 9 (Minor Tribes). The continental structure is canonical for all matches regardless of generation seed.*

### The World Is Not One Landmass

The Bloodlines world consists of multiple continents separated by ocean. This is structural, not decorative. The continental architecture has direct consequences for every victory path, for which dynasties can access which resources, for what naval investment actually unlocks, and for whether a maritime or land-dominant playstyle is viable at match scale.

The exact number of continents and their configurations are procedurally influenced at map generation, but the following structural rules are canonical and apply to every match regardless of generation seed.

### The Home Continent

The Home Continent is the primary landmass. All nine founding houses begin here. It is the largest continent, the one where the majority of the match's early conflict plays out, and the location of the Trueborn City. The Home Continent contains representatives of all ten terrain types in proportions that ensure all founding houses have viable starting positions. Its coastal zones are accessible to multiple founding houses. Its interior contains the contested resources — Iron Ridges, Sacred Ground, Ancient Forests — that generate mid-game conflict between houses.

The Home Continent is the arena of the Founding and Consolidation stages. Most dynasties spend the majority of their first two stages here, which is appropriate. The Home Continent is large enough to sustain a full match without secondary continent engagement if a dynasty chooses a pure land path — but that path carries structural limitations in the late game that are described below.

### Secondary Continents

At least two secondary landmasses exist in every generated match. Their size, terrain composition, and distance from the Home Continent vary by generation, but several structural properties are canonical:

Secondary continents contain resources not present in sufficient quantity on the Home Continent, or resources that have been depleted on the Home Continent by mid-to-late match extraction. A dynasty that reaches the late game with exhausted Iron Ridge deposits on the Home Continent and no naval access to secondary continent iron is in a structural resource crisis.

Secondary continents carry minor tribe populations with archetypes that may not appear on the Home Continent, deepening the diplomatic landscape. The Forest Clans on a secondary continent have no prior relationship with the founding houses. The Trade Network on a secondary continent has connections that the Home Continent's Trade Network does not have. Secondary continent tribes are not the same entities as Home Continent tribes — they are distinct populations with distinct histories, and approaching them requires relationship-building from scratch.

Some secondary continent terrain features carry unique match significance: Sacred Ground with no founding house historical claim, which creates faith path opportunities without the territorial baggage of Home Continent Sacred Ground; Frost Ruins preserving pre-Frost civilization remnants distinct from Home Continent ruins; and in some generations, major harbor islands positioned between continents that function as critical naval chokepoints.

Secondary continents are not empty territory waiting to be claimed. They have established ecological and tribal political structures. A founding house arriving on a secondary continent is a foreign power making first contact, and the local populations respond accordingly.

### Ocean Zones

The ocean between continents is not empty space. It contains:

**Trade route corridors** — established navigation paths between coastlines that naval dynasties can control, tax, or contest. A dynasty controlling the primary corridor between the Home Continent and a resource-rich secondary continent controls a toll road that every trade-dependent rival must negotiate.

**Fishing grounds** — offshore resource nodes producing food independent of land-based agricultural infrastructure. A coastal dynasty with developed fishing capacity has food security that is immune to land-based siege and drought events.

**Naval chokepoints** — island formations and narrow ocean passages that force all ocean traffic through confined geography. Fortifying a chokepoint island gives a naval dynasty leverage over commerce and military movement that no land-based fortification can match.

**Open ocean hazards** — storm events, navigational difficulty zones, and seasonal passage windows that affect fleet movement timing. Ocean is not a highway. It is a domain that demands specific investment to use reliably.

### Why Continental Architecture Matters for Every Victory Path

**Military Conquest** — The 75% territory threshold is global. It includes secondary continent territory. A land-only dynasty that never develops naval capacity cannot reach the Military Conquest threshold because it cannot claim what it cannot reach. This is not a soft disadvantage. It is a hard structural ceiling. Military Conquest at full match scale requires either naval capacity to project force, or an alliance partner who performs the maritime projection while the land-dominant dynasty claims the territory.

**Territorial Governance** — The governance requirement applies globally. Secondary continent populations have distinct cultural, faith, and economic profiles that create governance challenges different from Home Continent populations. The path's difficulty compounds with each continent added to the governance portfolio.

**Faith Divine Right** — Global faith share requirements include secondary continent populations. A dynasty achieving 50% faith dominance on the Home Continent may fall significantly short of the global threshold if secondary continent populations follow different covenants. Naval missionary reach, or proxy relationships with dynasties already present on secondary continents, is a meaningful faith strategy consideration.

**Currency Dominance** — A currency that circulates only on the Home Continent is not a global standard. True Currency Dominance requires trade routes connecting all landmasses. Secondary continent resources, flowing through a dynasty's controlled trade corridors in its own currency, is what the reserve currency analogy actually describes. Naval trade infrastructure is a prerequisite for full Currency Dominance at match scale.

**Dynastic Prestige and Alliance Victory** — Secondary continent tribes can be integrated into diplomatic frameworks, married into, and incorporated into coalition structures. A prestige player who has achieved notable accomplishments across continents accesses a category of diplomatic achievement that a purely Home Continent player cannot replicate.

---

## Section 72: Naval Doctrine as a Complete Playstyle

*Supplements Section 36 (Naval Warfare). The maritime path is a fully supported strategic orientation, not a supplement to the land game.*

### The Maritime Dynasty Is Not a Subset of the Land Dynasty

A dynasty can win Bloodlines as a primarily maritime power. This is a fully supported, mechanically coherent path to the sovereignty seat. It is not a hybrid strategy bolted onto a land game. It is a distinct civilization orientation with its own infrastructure logic, its own strategic tempo, and its own path to each of the five victory conditions.

The maritime path is harder to establish than the land path in the early match. The Founding stage begins on land. Harbor infrastructure requires coastal terrain, wood, stone, and time that could be spent on other priorities. Naval units are more expensive per combat value than land units at equivalent development tiers. A dynasty that commits to maritime power in the Founding stage is accepting short-term land vulnerability in exchange for long-term positional advantages that compound through the mid and late game.

A dynasty that reaches the late game as the match's dominant naval power is a dynasty that other dynasties cannot easily destroy, cannot easily isolate economically, and cannot reach on the secondary continents where it has established footholds. The investment pays.

### Harbor Infrastructure and Naval Development

Harbor development follows the three-tier progression established in Section 36, with the following additions:

**Naval Dockyards** — a specialized production building available at Tier 2 harbor that accelerates vessel production speed and unlocks hull-upgrade options that improve vessel combat values beyond their base ratings. A dynasty committed to maritime power invests in Naval Dockyards at multiple coastal settlements, creating a distributed production network rather than a single harbor dependency.

**Coastal Fortifications** — defensive installations designed specifically for harbor defense rather than land perimeter defense. Coastal fortifications carry sea-facing artillery that damages approaching hostile fleets, elevated observation positions that extend naval fog-of-war visibility, and chain-barrier infrastructure that can temporarily block harbor mouth access. A heavily fortified coastal settlement is as difficult to assault from the sea as a fortified inland position is to assault from land.

**Supply Depots at Sea** — island or coastal outposts positioned along major ocean trade routes that allow fleets to resupply without returning to a home harbor. Supply depots extend fleet operational range dramatically. A maritime dynasty that has built a supply depot network along its primary ocean corridors can sustain extended operations at ranges that a dynasty with only home harbors cannot.

### The Maritime Progression by Match Stage

**Founding Stage (Maritime)** — The maritime dynasty establishes coastal settlements with harbor access as its first priority rather than inland expansion. It accepts a smaller land territory footprint than land-focused rivals in exchange for early harbor infrastructure. It begins fishing operations to establish food security not dependent on inland agriculture. It scouts the coastline and ocean approaches to map the paths to secondary continents before those paths are needed. Relationships with River Stewards and coastal minor tribes are prioritized.

**Consolidation Stage (Maritime)** — Tier 2 harbors come online, enabling War Galley and Troop Transport production. The dynasty begins establishing its first footholds on secondary continent coastlines — not full conquest, but supply depot positions, trade contact with local tribes, and small garrison presences. Trade route corridors between continents begin generating income. The maritime dynasty is often slightly economically ahead of equivalent land dynasties at this stage because ocean trade routes generate without the ongoing vulnerability that land caravans face.

**Ideological Expansion Stage (Maritime)** — Full naval military capacity is online. Troop Transports enable genuine force projection onto secondary continents. The maritime dynasty begins the sustained campaigns that convert foothold positions into territorial control. The fleet protects trade corridors. Naval chokepoints are fortified or controlled. Faith infrastructure extends to secondary continent populations through missionary vessels carrying Faith Leaders. The maritime dynasty's economic lead from trade route control is compounding.

**Irreversible Divergence Stage (Maritime)** — The match's late stage reveals the maritime dynasty's structural advantages fully. It has resources from secondary continents that land-only dynasties cannot access. Its trade routes are difficult to disrupt because ocean corridors require naval force to contest. Its population base includes secondary continent populations that land-only dynasties cannot reach. Every victory path benefits from the continental reach the maritime orientation has built.

### Naval Fleet Doctrines

The six vessel types established in Section 36 support the following fleet doctrines:

**Commerce Raiding** — A doctrine emphasizing War Galley and Scout deployment along rival trade corridors. Commerce Raiders do not seek fleet-on-fleet engagements. They target unescorted trade caravans and Troop Transports. A commerce raiding doctrine is the naval equivalent of the operational harassment that Westland's Outriders apply to land supply lines.

**Power Projection** — A doctrine built around Capital Ships and Troop Transport capacity. Power Projection fleets are designed to deliver force to secondary continent coastlines and hold the sea lanes during amphibious operations. They are slower than Commerce Raiders and more expensive but can accomplish what no other naval doctrine can: the delivery of a full military ground force to a hostile shore.

**Chokepoint Control** — A doctrine emphasizing the fortification of critical ocean passages and the deployment of a concentrated fleet at those passages. A Chokepoint Control fleet does not range widely. It holds the narrow places that ocean traffic must pass through and taxes or denies passage accordingly.

**Trade Escort** — A doctrine that prioritizes the protection of friendly trade caravans and the suppression of rival commerce raiding. Trade Escort fleets are not designed to win fleet battles — they are designed to make fleet battles happen on terms unfavorable to the attacker, deterring commerce raiding through the credible threat of escort response.

Fleet doctrines are not formal selections. Like military operation doctrines, they emerge from the composition of the fleet and the posture assignments the player makes.

### Sea-Land Footholds

A foothold is a coastal position on a secondary continent that is less than full territorial control but more than a diplomatic contact. It is a physical presence: a small garrison, a supply depot, a trading post, and the beginning of a relationship with local tribes.

**Establishing a Foothold** requires: a Troop Transport delivering garrison forces to a landing site; a coastal terrain position that can be held by a modest force; initial construction of a supply depot or trading post; and the beginning of diplomatic contact with any tribes in proximity.

**Foothold Vulnerabilities** — A foothold garrison is isolated from Home Continent reinforcement by the ocean crossing time. A rival naval dynasty that contests the sea lane supplying a foothold can effectively besiege it from the water. Footholds surrounded by hostile tribes are unsustainable without either military clearing of adjacent territory or diplomatic resolution with those tribes.

**Foothold Development** — A foothold that is sustained and invested in becomes a colonial settlement, then a proper province with governance infrastructure, then a fully integrated territory with population loyalty, faith coverage, and contribution to the dynasty's resource base. This development takes match time that the dynasty must invest deliberately. A foothold that is established and then neglected will not develop on its own.

**Tribal Relationships on Secondary Continents** — Local tribes on secondary continents have no prior relationship with the founding houses. The reputation the player's dynasty has built on the Home Continent does not automatically transfer. A dynasty's conviction axis does express itself in how it conducts first contact — and local tribes read conduct, not history. The mechanics are the same as on the Home Continent. The history is shorter.

---

## Section 73: Match Phase Architecture

*This section defines the Phase framework, which complements and does not replace the stage framework in Section 45. Stages describe what becomes structurally available. Phases describe what the player is primarily doing with what is available.*

### Phases vs. Stages: The Distinction

Section 45 establishes the five match stages: Founding, Expansion and Identity, Encounter/Establishment/Neutral City, War and the Turning of Tides, Final Convergence. These stages remain canonical and describe the technological, military, and faith development arc of a match.

This section adds the Phase framework, which describes the player's experiential and strategic focus within and across those stages. Three phases are defined. A phase does not end with a timer. It ends when the player's situation has transformed enough that their primary strategic focus shifts to the next phase's concerns.

### Phase One: Emergence

**What it is:** The opening phase of every match. The player is finding their footing — establishing a stable resource base, exploring the immediate geography, making first contact with neighboring dynasties and local tribes, and beginning to understand what kind of dynasty they are going to become. Phase One corresponds primarily to the Founding stage and early Expansion and Identity.

**The strategic priority of Phase One is positioning, not victory.** A player trying to win in Phase One is playing the wrong game. The correct priority is establishing a position from which multiple strategic paths remain viable. A player who overcommits to military conquest in Phase One and loses significant population to attritional border skirmishes has narrowed their options before they understood what those options were.

**What Phase One requires from the player:**

*Exploration of the immediate geography.* Where are the Iron Ridges relative to the starting position? Where does the nearest river valley run? Which direction is the coast, and how far? Which minor tribes are within reach, and what archetypes are they? A player who reaches Phase Two without knowing the answers to these questions is playing with incomplete information.

*Establishing food and water security.* These are not optional. They are the prerequisite for everything else. Phase One is not complete until food and water production consistently exceeds consumption with a buffer for adverse events. The buffer matters — a famine event hitting a dynasty with zero food reserve is an emergency. The same event hitting a dynasty with two cycles of reserve is a managed setback.

*First contact with local tribes.* Every tribe within reach is either a future ally, a future complication, or a future enemy. Phase One is the window to establish those relationships before other dynasties do.

*Choosing a directional lean.* Phase One is where the player begins to understand what kind of dynasty they are going to be. Not a final decision — Phase Two is where commitments harden — but an initial lean. Is this dynasty going to be maritime or land-dominant? Faith-focused or militarily aggressive? The early choices about what to build first, which direction to expand in, which tribe to approach first — these are not commitments, but they are signals.

*Identifying naval or land orientation.* Does the starting position have coastal access? If yes, is the player going to develop it in Phase One or treat it as a later-stage asset? A player who knows they want to be a maritime power should begin in Phase One.

**What Phase One does NOT require:** Faith commitment (should not be rushed), military dominance, or diplomatic formalization. Relationships begun in Phase One do not need to be locked into formal agreements in Phase One. Establishing cordial contact is enough.

**Phase One ends** when the player has stable food and water, a defended home territory boundary, at least preliminary contact with neighboring dynasties and local tribes, and a directional lean that informs their Phase Two investments. This is a readiness condition, not a timer condition.

### Phase Two: Commitment

**What it is:** The phase where the player begins hardening their strategic identity. The Expansion and Identity and Encounter/Establishment stages are dominant here. The player has enough information from Phase One to make meaningful commitments: which faith covenant to follow, which victory path to orient toward, which diplomatic relationships to formalize, and whether naval and continental engagement will be primary or secondary.

**The strategic priority of Phase Two is investment and differentiation.** Resources committed to faith infrastructure in Phase Two are not available for military expansion. Resources committed to harbor development are not available for inland territory consolidation. Phase Two is where the costs of strategy become real because the choices have real opportunity costs. A player who tries to do everything in Phase Two does nothing well.

Phase Two's completion corresponds roughly to the end of Encounter/Establishment. By the end of Phase Two, the player's strategic identity should be legible — to themselves and, through their behavior, to rival dynasties.

### Phase Three: Resolution

**What it is:** The phase where the match's outcome is determined. The War and Turning of Tides and Final Convergence stages are dominant here. The player is executing their established strategy against active opposition, managing the late-game pressure mechanics, and navigating toward a resolution condition.

Phase Three is where the consequences of Phase One and Phase Two decisions fully materialize. A dynasty that built well in both preceding phases enters Phase Three with compounding advantages. A dynasty that made structural errors — overextension in Phase One, wrong commitment in Phase Two, neglected succession infrastructure — faces those errors as compounding costs in Phase Three.

### Stage-Phase Correspondence

Stage One (Founding) corresponds to Phase One in its entirety. Stage Two (Expansion and Identity) spans the late portion of Phase One and the opening of Phase Two. Stages Three (Encounter) and Four (War) correspond to Phase Two and the beginning of Phase Three. Stage Five (Final Convergence) is Phase Three in full expression.

The stage framework tracks what is structurally available in the match. The phase framework tracks what the player is primarily doing with it. Both are necessary to fully describe the arc of a Bloodlines match. Neither is sufficient alone.

---

## Section 74: Water — Strategic Depth Beyond Resource

*Supplements Section 40 (Resource System) and Section 36 (Naval Warfare). Water operates in three distinct strategic roles that deserve explicit design treatment.*

### Water Is Not a Generic Resource

The existing design establishes water as existential — present alongside food as the foundational population resource, mentioned as a target for denial warfare, included in the resource table. This section makes water's strategic architecture explicit across all three of its roles.

### Water's Three Roles

**Water as Population Sustenance** — The most fundamental role, established in Section 40. Population cannot grow without water. Population stability degrades with water scarcity. A settlement without water infrastructure has a population ceiling significantly below a settlement with full water access. The ceiling is hard, not soft. Population cannot grow past what water can support.

**Water as Strategic Geography** — Water defines movement, borders, and access across the map in ways that gold and stone do not. Rivers determine where armies can cross quickly and where they cannot. Coastal terrain determines where harbors can be built and where they cannot. Ocean separates continents and creates the domain that requires specific investment to traverse. Lake and river valley geography creates the natural province boundaries described in Section 17.

A river running between two founding house starting positions is simultaneously an economic asset (River Valley terrain with food and water production), a diplomatic boundary (a natural province line that both parties understand), and a military obstacle (crossing under fire is costly). The same water feature serves all three roles simultaneously.

Water denial — controlling the upstream water sources that feed an enemy's population centers — is one of the cleanest strategic tools in the game for a dynasty in geographic position to use it. Unlike food denial, which requires controlling agricultural territory, water denial in the right geographic configuration requires controlling the source, which may be upstream from the enemy's territory and within easier reach.

**Water as Naval Domain** — The ocean is water at its largest scale, and it introduces a strategic dimension that has no land equivalent. Sections 36 and 72 elaborate what becomes possible when a dynasty treats water as a domain to be mastered rather than an obstacle to be avoided. A dynasty that masters the ocean domain has: supply routes that land-based armies cannot interdict, access to secondary continent resources, the ability to project force where rivals cannot quickly respond, and trade network control that extends to every coastal civilization in the match world.

The ocean is not simply a larger river. It is a domain with its own infrastructure requirements, its own hazards, its own tactical dynamics, and its own victory path implications.

### Water Infrastructure Types

**Wells** — The basic water access building for inland settlements. Provides baseline water access for a settlement's population up to a capacity ceiling. Multiple wells raise the ceiling but do not eliminate the geographic dependency on groundwater availability, which varies by terrain type.

**Aqueducts** — A mid-to-late infrastructure investment that channels water from a river or lake source to a settlement that lacks natural direct access. Aqueducts are expensive in stone and labor but remove the geographic constraint on settlement positioning for water purposes.

**Irrigation Networks** — Agricultural infrastructure that channels water onto Reclaimed Plains terrain, increasing food production yield per tile beyond what rain-fed agriculture produces. Irrigation Networks require water infrastructure to build and operate, and produce food as their output. The most agriculturally productive territory in the match is Reclaimed Plains with full irrigation coverage fed by River Valley water sources.

**Cisterns** — Storage infrastructure that accumulates water during wet periods for use during drought events. A dynasty with cistern infrastructure weathers drought events as managed setbacks. A dynasty without cisterns weathers them as emergencies.

**Harbor Infrastructure** — As established in Section 36 and expanded in Section 72. The harbor is the building that converts coastal access into naval capability. From a water-infrastructure perspective, it is the building that converts the ocean from an obstacle into a resource domain.

**Desalination Infrastructure** — Available at coastal settlements as a late-tier investment. Converts salt water access into population-usable fresh water supply, removing the dependence on inland groundwater or river access for coastal population centers. A fully developed coastal settlement with desalination infrastructure has water security that no inland siege or upstream denial can reach.

### Water Denial as a Strategic Path

Water denial is a distinct strategic option that operates faster and more completely than food denial in the right geographic configuration because there is no substitute for water the way that trade can substitute for food.

**Upstream control** — A dynasty controlling the headwaters of a river that feeds rival settlements controls a denial option. Damming, diverting, or poisoning upstream water (through the Poison Supply covert operation in Section 64) reduces water availability downstream.

**Well and aqueduct targeting** — The Sabotage Infrastructure covert operation (Section 64) can target water infrastructure specifically. Destroying a rival's wells or aqueduct connections forces the target settlement into emergency water rationing, degrading population health, reducing military morale, and triggering sustained water scarcity consequences.

**Siege-based denial** — A siege operation encircling a fortified settlement cuts off external supply, including water resupply if the settlement is not self-sufficient. The Siege military doctrine (Section 66) is the formal mechanism for this. A settlement that has not invested in cisterns and internal wells is more vulnerable to siege-based water denial.

**Naval blockade** — A maritime dynasty that controls the sea approaches to a coastal rival can deny that rival the fishing grounds and desalination access that its water security depends on. Naval blockade as water denial is available only to maritime-capable dynasties and only effective against rivals with coastal water dependencies — but when those conditions are met, it is one of the cleanest denial tools in the game.

### The Choice: Land-Dominant or Sea-Dominant

Every dynasty in Bloodlines implicitly makes a choice about its relationship to water as a domain. This choice emerges from the sum of infrastructure investments, expansion decisions, and doctrinal commitments made across Phase One and Phase Two.

**Land-dominant dynasties** treat water as a resource and an obstacle. They build wells and irrigation. They bridge rivers rather than sail them. They view the ocean as the edge of the map rather than as a strategic domain. Their vulnerability is continental reach.

**Maritime-dominant dynasties** treat water as a domain and a highway. They invest in harbor infrastructure early. They view the ocean as the most controllable strategic space on the map. Their vulnerability is land depth — a maritime dynasty with a thin Home Continent foothold is exposed to land-based pressure. The answer is not to abandon the maritime orientation but to maintain sufficient land-based military posture to deter Home Continent aggression while the maritime advantages compound.

**Hybrid dynasties** exist on a spectrum between the two poles. Partial naval investment does not produce the full advantages of maritime dominance, and partial land investment may not produce the security against Home Continent pressure that a committed land dynasty achieves.

The game never signals that one orientation is correct. Both are complete strategic identities. Neither should be mandatory. Neither should be unrewarded.

---

# PART XIII — OPEN DESIGN RESERVOIR

*This part is a living document. Items listed here are concepts that have been raised, discussed, or partially developed without being definitively canonized or definitively removed. They are in orbit — available for revival, potentially canonizable, not dead. Only Lance Fisher determines when an item moves from Open Design Reservoir to canon.*

---

## Section 75: Open Design Items

### Item 1: Victory Condition Thresholds — Partially Resolved

Three victory paths require specific numerical threshold decisions before the victory system is fully specified:

**Faith Divine Right** — The global faith share percentage required to open the declaration window is not specified. Working design assumption: higher than originally assumed — significant conversion investment required. Exact percentage to be determined.

**Currency Dominance** — REVISED 2026-04-07: The mechanism is a dynasty's custom currency displacing gold as the world standard. When the Trueborn City and other dynasties adopt the custom currency for trade/economy, the dominant dynasty gains effectively unlimited resources. The specific adoption threshold (what level of world adoption triggers this advantage) is not yet specified.

**Territorial Governance** — PARTIALLY RESOLVED 2026-04-07: Approximately 90% population loyalty required — essentially the remaining opposition is only minor tribes the winning dynasty is allowing to exist by choice. Not locked — fine-tune through data layer.

Military Conquest threshold remains at 75% territory / 60% stability floor. Dynastic Prestige no longer applies (eliminated as a victory path 2026-04-07).

These threshold decisions should be specified before the victory system is considered complete.

### Item 2: Hartvale Unique Unit — RESOLVED 2026-04-07

The Verdant Warden (CB002) was selected. Off 4/Def 5. Hybrid guardian. See Section 59. This item is closed.

### Item 3: Mode Variants — PARTIALLY RESOLVED 2026-04-07

The Phase Entry system is settled (see Section 76). Players can enter the match at any of the five stages. Entering at Stage 3 means starting with Stage 3 conditions at an accelerated build-up rate. This covers the primary shorter-session use case. Other distinct mode variants (campaign modes, scenario modes) remain open.

### Item 4: Multiplayer Interaction Architecture — Open Questions

The bible establishes that up to 10 players can participate. The following interactions in human-to-human multiplayer are open design questions:

How do human-to-human diplomatic interactions work in real time? Whether human players can form formal alliances with custom terms in session? How does the match handle simultaneous player actions in the same territory (two players both issuing orders that conflict with each other in the same turn)? Whether there are team or co-op modes versus free-for-all only?

### Item 5: House Starting Relationships — RESOLVED 2026-04-07

Seven canonical starting relationships locked. See Part IX Addendum (Section 62.1). Full pairwise mapping for remaining pairs (beyond the seven canonical ones) remains open.

### Item 6: Bloodline Member Personality Traits — Design Gap

The Eighth Ingestion Canon established that each Bloodline Member has personality traits affecting event interactions, NPC responses, and player nudges. The trait system has been established as canonical intent but the specific trait list, the mechanics by which traits express in events, and the visual and behavioral signals that communicate a member's trait to the player are not designed.

### Item 7: Secondary Continent Tribe Profiles — Not Designed

Section 71 (this document) establishes that secondary continents have minor tribe populations distinct from Home Continent tribes. The ten archetype system established in Section 8 applies as the base framework, but secondary continent variants — which archetypes appear, what their distinct histories and dispositions are, how their resources and terrain differ from Home Continent equivalents — are not specified. This is a design gap with direct impact on the late-game diplomatic and expansion experience.

### Item 8: Generational Scale Management — High Priority Open Design Gap

The Seventh Ingestion Canon (2026-03-19) identified managing a very large number of living family members across generations — births, childhood, maturity, death, marriage, offspring — as a high-priority open design gap. A system for managing generational scale (potentially using environmental events like long winters or famine as natural population thinning mechanics) has been considered but not designed. Section 62 of this document establishes the time model that frames this system. The mechanics of managing it at scale remain open.

---

# PART XIV — SESSION 12 ADDITIONS (2026-04-07)

---

## Section 76: Phase Entry System — Mode Variant

*Settled 2026-04-07 per Lance Fisher direction.*

### The Core Concept

The five-stage match structure is the complete arc of every Bloodlines game. Phase Entry is a mode variant that changes where in that arc a player begins — not what the arc is.

A player who enters at Stage 1 (Founding) begins from scratch: no infrastructure, no armies, no established position, no history. A player who enters at Stage 3 (Encounter/Establishment/Neutral City) begins in Stage 3 conditions, with infrastructure, armies, and position appropriate to Stage 3 — built at an accelerated rate during a brief start-up period rather than developed organically through Stages 1 and 2.

The five-stage structure is unchanged. The match rules are unchanged. The victory conditions are unchanged. What changes is the speed at which each player reaches a baseline appropriate to their entry stage, and the depth of history their dynasty carries at entry.

### Why This Exists

The grand match format (2-10+ hours) is the design target and primary format. Phase Entry does not replace it. It enables players who want more immediate, direct action — who want to arrive at the contest phase without building through two full stages of foundation — to enter a match in a state that supports that experience.

A Stage 3 entry starts with what a well-developed Stage 3 dynasty looks like: forward bases, established trade routes, a committed faith covenant, a developing army, active diplomatic relationships. The entry is compressed, not skipped — the player makes key foundational choices during a brief accelerated setup window before full match conditions apply.

### Entry Stage Specifications

**Stage 1 Entry (Standard):** Full grand match. No accelerated build-up. Dynasty begins from founding conditions.

**Stage 2 Entry:** Brief accelerated setup window. Player makes: faith commitment, starting territory footprint, initial diplomatic posture, bloodline member role assignment. Enters with basic faith infrastructure and modest territory base.

**Stage 3 Entry:** Moderate accelerated setup window. Player makes: all Stage 2 decisions plus army composition, trade route configuration, major alliance or rivalry status. Enters with mid-match conditions: developed faith infrastructure, active trade networks, established position.

**Stage 4 Entry:** Extended accelerated setup window. Player enters with a late-stage civilization already in full configuration. Most decisions pre-committed. Recommended for experienced players who want to focus on endgame mechanics.

**Stage 5 Entry:** Maximum accelerated setup. Enters at Final Convergence conditions. Highest intensity, least developmental arc.

### Design Constraints

Phase Entry does not change the time pressure of the match. All players in the same match should be at comparable development levels for the entry stage. The match is not restructured for the phase entrant — the match clock and other players continue normally.

The accelerated build-up period is not a period of immunity. The player is making real choices quickly, not deferring them.

---

## Section 77: Alliance System — Trigger Conditions and Constraints

*Settled 2026-04-07 per Lance Fisher direction.*

### Alliances Are Available But Conditional

Alliances between players are a designed and supported feature of the match. They are not freely available at all times. Two restrictions govern when alliances can be formed:

**Restriction 1 — Faith Extreme Incompatibility**

Alliances cannot be formed between dynasties that are at opposite faith extremes. A dynasty at Apex-level Old Light practice cannot form a formal alliance with a dynasty at Apex-level Blood Dominion practice. The faith systems are ideologically incompatible at full intensity — the populations and institutions of each would reject the alliance as theologically illegitimate. This restriction does not prevent Cordial relationships or trade agreements between faith-extreme dynasties. It prevents formal military alliances with shared victory obligations.

The restriction activates at high faith intensity thresholds, not at any faith divergence. A Light Doctrine and Dark Doctrine dynasty at moderate intensity can still ally. The restriction bites when both parties have committed deeply enough that the covenant incompatibility becomes structurally defining.

**Restriction 2 — Steamroll Counter-Coalition**

If one dynasty is clearly dominating the match — accumulating territory, victory progress, or military advantage at a rate that makes individual resistance by remaining players effectively hopeless — the game must provide the other players with the option to form a counter-coalition.

This is a game-state-triggered option, not a permanent feature. When the steamroll threshold is crossed (specific metrics to be designed), players who would otherwise be restricted from allying with each other — including players at faith extremes — receive the counter-coalition option. The restriction on faith-extreme alliances is suspended for counter-coalition purposes.

The counter-coalition mechanic is the structural immune response to runaway leader scenarios at the alliance level, parallel to what the Great Reckoning provides at the military level.

### Alliance Victory vs Standard Alliance

A standard alliance (Cordial → Allied diplomatic state) is the operational structure for military cooperation. An Alliance Victory is the formal commitment to shared sovereignty with agreed terms. The latter requires the former but the former does not imply the latter.

---

## Section 78: Dynastic Prestige as Strategic Modifier — Summary Reference

*See Section 51 for full specification. This section provides quick-reference.*

Prestige is not a victory path. It is a civilizational modifier. High prestige:

- Reduces population loyalty resistance in controlled and contested territories
- Improves diplomatic agreement terms with other dynasties
- Attracts lesser houses and promoted commanders seeking alliance
- Eases alliance formation and coalition building
- Adds legitimacy to Faith Divine Right declarations
- Accelerates Currency Dominance adoption

Prestige is accumulated through: bloodline depth, political marriages, famous battle outcomes, faith alignment with world-dominant covenant, legendary Bloodline Member legacy, and major diplomatic achievements.

Prestige is a visible public resource. All dynasties see all prestige standings at all times.

Opponents must still be conquered, neutralized, or otherwise addressed through one of the five victory paths. Prestige makes the path less impossible, not automatic.

---

## Section 79: Wild Faith Animal Mounts — Design Direction

*PROPOSED 2026-04-07. Full design pending.*

At sufficient Wild faith intensity (threshold to be determined), players following The Wild covenant can deploy additional animals beyond bears as cavalry mounts. These additional animals provide higher combat bonuses than standard horse cavalry, reflecting the deepest expression of The Wild's covenant relationship with the natural world.

Bears are the established anchor (canonical from Third Ingestion). The expanded roster of possible mounts beyond bears — what other animals, what intensity thresholds, what specific bonuses, what visual expression — is to be designed in a dedicated future session.

The design direction: the wider the player has opened The Wild's animal covenant, the more powerful and varied the mounts available. This is the Wild faith's equivalent of the other covenants' apex unit access at maximum faith intensity.


======================================================================
# PART XV -- THE DUAL-CLOCK ARCHITECTURE AND EMPIRE COMMAND
## Sections 80-81 -- Addendum A Integration -- 2026-04-07
======================================================================

---

## Section 80: The Declared Time Model

*SETTLED 2026-04-07. This section supersedes all prior time model content in BLOODLINES_ADDITIVE_INTEGRATION.md and the Multi-Speed Time Model placeholder. This is the canonical architecture for Bloodlines' time system.*

---

### THE FUNDAMENTAL DESIGN INSIGHT

The time problem in Bloodlines -- how a live real-time strategy game coexists with a generational dynasty simulation across a 6-10 hour session -- resolves through a principle called declared time.

A battle takes 20-40 minutes of the player's actual session time. When that battle concludes, the game declares that a defined amount of in-world time has elapsed. The player experienced a fast, tactical, real-time engagement with all the immediacy and responsiveness of Command and Conquer-style play. The world experienced a campaign of several months or several years. Both are simultaneously true and neither contradicts the other, because the player never had to hold both frames in their head at the same time.

This is not a compromise or a trick. It is how all historical narrative works at every level of the form. A chapter takes twenty minutes to read and covers three years of a character's life. The reader never feels the contradiction because the narrative manages the frame. Bloodlines manages the frame the same way -- through what it tells the player when the engagement concludes.

The alternative approaches that do not work for Bloodlines are worth naming explicitly so they can be discarded cleanly:

A single universal year-to-hour ratio cannot satisfy both games simultaneously. If one real hour equals ten in-world years, a single 30-minute battle covers five in-world years, which is absurd as a tactical engagement but reasonable as a dynasty clock. If one real hour equals one in-world year, a battle lasts a few months of in-world time but the dynasty barely ages across the full match. No single ratio works because the RTS layer and the dynasty layer operate at genuinely different time scales that cannot be collapsed into one number without breaking one or both.

Pausing dynasty time during battles is an admission of failure -- it acknowledges the two systems cannot actually coexist and imposes a mechanical separation that the design philosophy explicitly rejects.

Making dynasty time primary and abstracting battles loses the Command and Conquer immediacy that is the game's foundational RTS identity.

Making battle time primary and treating dynasty as a notification layer produces a dynasty simulation that feels thin and bolted-on rather than genuinely generational.

Declared time solves all of these by not trying to reconcile the two clocks into one. They are different clocks. The player experiences the battle clock during battles and the dynasty clock during the strategic layer between battles. The declaration at battle's end is the seam that joins them. Done correctly, the seam is invisible.

---

### HOW DECLARED TIME WORKS IN PRACTICE

When a battle concludes -- whether the player fought it directly or it resolved under Commander direction -- the game presents a brief declaration screen or overlay that announces the elapsed in-world time. This is not a loading screen. It is a narrative beat. The world has moved. Here is how much.

The declaration is not arbitrary. It is calculated from the engagement type, size, duration, and outcome. A small skirmish at a frontier resource node declares that three to six months have passed. A significant pitched battle between two standing armies declares one to two in-world years. A sustained siege campaign fought across multiple connected engagements declares three to five years. Stage One engagements, which are small and contained by design, declare modest amounts -- a few months each -- because the dynasty is young and the world is still being discovered, and time should feel slow at the beginning. By Stage Four, a single major engagement might declare three years elapsed, and the player feels the weight of that.

The specific declaration table, which maps engagement type and stage to in-world time elapsed, is a primary design lever for controlling dynasty pacing. Adjusting these numbers adjusts how many generations mature across the match's full arc. The design target -- two to three generations achievable in a full 6-10 hour session -- requires that by the match's mid-stages, major engagements are declaring enough time that a child born in Stage One can be a senior military figure by Stage Four or Five. This requires major engagements in Stages Three and Four to declare roughly two to four years each.

The declaration is presented as a brief narrative statement in the voice of the match world, not as a game-system notification. Not "elapsed time: 2 years." Rather, something that reflects what the battle represented in the world's history. The format serves both the time declaration purpose and the story-generator purpose that the design philosophy identifies as the game's ultimate test.

---

### THE STRATEGIC LAYER: WHERE THE DYNASTY GAME LIVES

Following the declaration, the player enters the strategic layer. This is not a menu or a pause screen. It is the regional and sovereign view of the game -- the player looking out at the full match state and making the decisions that determine what happens next. The strategic layer is where the empire is actually run. The battles are the dramatic foreground. The strategic layer between them is the substance of the dynasty game.

The strategic layer has two primary components: the events queue and the commitment phase.

The events queue surfaces everything that accumulated during the elapsed in-world time. Dynasty events: a bloodline member has reached a development milestone, an heir has matured into an adult role, a spouse is expecting a child, a senior member is showing the first signs of age-related decline. Diplomatic events: a trade agreement has reached its renewal threshold, a previously neutral dynasty has made a territorial claim adjacent to a contested province, an ally has sent a delegation requesting military coordination. Operational events: a rogue operative has advanced to a new position, a mystic's calendar window has opened, an operation in progress has encountered a complication. Faith events: intensity in a recently conquered territory has risen to the threshold for the second-tier building, a rival covenant's spread is approaching the boundary of a region the player considers theirs. Conviction events: the Pattern Amplification system has registered a behavioral milestone, a population segment's loyalty has crossed a threshold in either direction.

The player works through these events. Some require immediate decisions. A succession event triggered by an unexpected death cannot wait -- the player must designate a replacement and manage the political consequences before moving on. A trade agreement renewal can be deferred to a later strategic window. Most events sit between these extremes: they benefit from attention but do not demand it immediately, and the player's judgment about prioritization is itself a meaningful strategic decision.

The commitment phase is where the player defines what happens next across every active front. Where do armies move? Which battles does the player intend to personally attend, and which will be delegated to Commanders? Which operations are being initiated, advanced, or paused? What diplomatic communications are being sent? Where are bloodline members being reassigned? Which faith rituals are being ordered?

These commitments take real planning time in the session. They also take in-world time to execute: an army moving three provinces over requires several months of in-world transit, declared as elapsed time when the player next surfaces from a battle. The player must commit to timelines before returning to direct engagement, accepting that those timelines will run while they are focused elsewhere.

When the commitment phase is complete, the player drops into their next direct engagement. The match continues on every front simultaneously, with Commanders executing their directives, operations advancing, caravans moving, tribes being influenced, faith spreading. The player surfaces again when their direct engagement concludes, finds that the world has moved by however much the next declaration states, and works through a new events queue shaped by everything that happened while they were fighting.

This cycle -- battle, declaration, events queue, commitment phase, battle -- is the rhythm of a Bloodlines session. The battles are what the player directly experiences as the RTS game. The strategic layer is where the dynasty simulation lives. The declared time is the seam that makes both feel real simultaneously.

---

### DYNASTY EVENTS DURING LIVE BATTLE

Dynasty events and operational events do not interrupt live battles. This is absolute. The player in the middle of a Command and Conquer-style engagement is not pulled out of it by a popup announcing that their heir has reached adolescence. The tactical layer has the player's full attention. The dynasty layer queues everything and waits.

Events that accumulate during a battle are held in the queue and surfaced at the declaration screen following the battle's conclusion.

The exception is events of immediate existential consequence to the active battle: the Commander of the army being fought is killed or routed, a supply line to the active battlefield has been cut and attrition is beginning, a bloodline member present in the engagement has been wounded. These are surfaced as in-battle notifications because they affect decisions the player is making right now. They are presented as tactical intelligence, not as dynasty management prompts.

Everything else queues. The player trusts that the world is running and focuses on the battle in front of them. When the battle ends, they find out what happened.

---

### STAGE-BY-STAGE TIME FEEL

Stage One: time declarations are modest -- three to six months per encounter. The dynasty grows slowly and deliberately. A child born early in Stage One has not yet reached childhood by Stage One's end. The founding head is still in their prime.

Stage Two: engagements grow in scale. Time declarations increase: six months to a year per major engagement. The dynasty begins to feel it -- bloodline members' ages are visibly advancing, children are reaching adolescence, the first succession questions are appearing in the events queue as distant concerns.

Stage Three: full-scale conflicts between founding houses. Time declarations of one to two years per major engagement. By Stage Three's end, a founding head who began in their late 30s is now in their 50s. The heir designated in Stage One or early Stage Two is now an adult figure with their own established capability.

Stage Four: heavy combat. Major battles declare two to three years. Sustained siege campaigns declare three to five. The founding head, if still alive, is aging into their 60s or 70s and their decline is becoming mechanically visible -- reduced command radius, slower physical recovery from wounds, elevated succession urgency. The dynasty has genuine multi-generational depth.

Stage Five: the match's culmination. Time declarations are significant but the match is approaching resolution. Stage Five is the harvest of everything planted in Stages One through Four.

---

### THE ROGUE AND MYSTIC TIME MODEL

Covert and faith operations find their most natural mechanical home under the declared time model.

A rogue being navigated into enemy territory runs in the background across multiple battle-strategic cycles. The player initiates it during the commitment phase -- setting the operative's objective, their route approach, the patience threshold, and the military support directive for any nearby army. The operative then advances across subsequent battle-declaration cycles, each one moving them closer to the objective or reporting complications that require course correction.

A rogue operation that takes several in-world years to complete does not occupy two years of real session time. It runs across multiple cycles while the player is fighting battles and managing other commitments. In real session time it resolves over an hour or two. In in-world time it has been a multi-year covert campaign. Both are simultaneously true.

The calendar timing of mystic operations -- the lunar events, solstices, and celestial events established in the Operations System -- gains mechanical depth under the declared time model. The player can see from the strategic layer that a Blood Moon is three in-world months away. That means one or two battle cycles before the window opens. The timing is not a passive countdown. It is a strategic constraint that the player incorporates into their planning. Missing the window means waiting for the next occurrence, and the match does not wait.

---

### MULTIPLAYER: THE SHARED MATCH CALENDAR

In a multiplayer match, the declared time model extends to a shared framework that prevents different players' dynasties from aging at different rates.

The match runs on a shared calendar -- a single in-world timeline that all players and all AI dynasties operate against. This calendar advances based on the match's aggregate activity rather than any individual player's battle frequency.

Individual players' battle conclusions declare time drawn from the shared calendar. A player concluding a battle during a period when the shared calendar has advanced six months receives a six-month declaration. Another player concluding a battle in the same shared period receives the same declaration. The world has moved six months for everyone simultaneously.

The match does not pause for individual players' strategic layer windows. A player who spends a long time in the strategic layer is allowing more real-world time to pass before their next personal engagement. This is the reality of a large empire -- time keeps moving whether you are watching it or not. A player with well-constructed Commanders and clear directives on all fronts can afford a long strategic window. A player with vague directives and weak Commanders cannot.

Diplomatic interactions between human players in real time are handled through the strategic layer interface, which functions as a persistent communication channel available to all players simultaneously regardless of their current state. Communication happens in in-world time -- messages take in-world travel time to arrive, calculated automatically based on the diplomatic channel and current calendar position.

---

### CROSS-REFERENCES

- Section 38 (Army Composition and the Recruitment Slider System): the Commander system (Section 81) provides the delegation infrastructure that operates during declared time.
- Sections 63-68 (Operations System): the rogue and mystic time model above extends the operations content.
- Section 46 (Match Scale, Pacing, and Recovery Mechanics): the 90-second cycle example is the tactical layer within a single declared-time engagement.
- Section 12 (Bloodline Member Functional Roles): the War General role operates as a bloodline Commander as described in Section 81.
- Section 47 (Political Events System): the Legendary Member Emergence event is achievable through Commander development as described in Section 81.

---

## Section 81: The Commander System

*SETTLED 2026-04-07. New canonical content -- no equivalent in prior bible versions.*

---

### THE FOUNDATIONAL PRINCIPLE

The Commander system exists to solve a specific problem without using the design solution that Bloodlines explicitly rejects.

The problem: a match-scale Bloodlines session produces an empire with multiple active military fronts, simultaneous operations, contested trade routes, and diplomatic situations that all require attention simultaneously. The player has one body and one field of focus. They cannot personally command everything at once.

The rejected solution: impose a numerical cap. The player can only directly control X armies. Beyond X, armies cannot be activated or committed. This is the conventional RTS approach to preventing scope overload. It is explicitly not Bloodlines' approach. The design philosophy states that the player must always be permitted to try to control everything personally. The limitation must be human, not mechanical.

The Commander system is the infrastructure that makes this true. Every army in the match is always commandable by the player. The player never reaches a wall that says "you cannot control this." What the player reaches, if they overextend, is the natural consequence of having distributed their attention too thin -- Commanders without sufficient capability executing directives without sufficient specificity on fronts the player is not watching. Those fronts produce worse outcomes than the player would have produced personally. The match shows the player what overextension costs. It does not prevent them from overextending.

The Commander slot on every army is always occupied by one of two things: a player-designated Commander operating under player-set directives, or the player themselves in direct control. Switching between these two states is immediate and available at any time from the strategic layer.

---

### THE TWO COMMANDER TYPES

#### Bloodline Member Commander

Any bloodline member with sufficient military development can be assigned as Commander of a specific army. This is the premier option. A bloodline member commanding an army brings everything they are to that army -- their accumulated military experience measured as a developed attribute, their conviction posture which shapes how they execute under ambiguous orders, their faith alignment which affects the morale of troops sharing that alignment, their personal tactical history which the Tactical Memory system records and compounds, and their personal relationships with units and sub-commanders who have served under them across multiple engagements.

An army under a veteran bloodline Commander is a qualitatively different thing from an army under an AI Field Commander. The veteran Commander executes the directive as someone with a reputation among the troops, a history of making good calls under pressure, and a presence that the units respond to emotionally as well as functionally.

Every bloodline Commander has a command capacity -- the scale of force they can effectively lead while maintaining full command effectiveness. This is not a hard cap on army size. It is a quality threshold. A Commander assigned an army within their capacity leads it at full effectiveness. A Commander assigned an army that exceeds their capacity leads everything within their capacity at full effectiveness and everything beyond it at degraded effectiveness. The player who assigns their newly committed War General to lead the match's largest army is not blocked from doing so. They are simply doing it badly.

Command capacity grows with development. A War General who has led armies across multiple stages has a larger effective capacity than one who was committed in Stage Three and has fought one major engagement.

A bloodline Commander given a directive executes it according to two simultaneous influences: the specific directive content, and their personal tactical profile built from their match history. A Commander who has spent multiple stages in aggressive offensive operations executes even a Hold directive with more aggressive margins than a Commander whose history is defensive. A Commander at Fervent faith intensity will prioritize protection of faith infrastructure within their operational area even when the directive does not specify it, because that is who they are.

A bloodline Commander can be set to full directive-free autonomy, in which case they operate entirely on their own tactical judgment within their understood strategic context. A Legendary Member War General operating with full autonomy is a genuine strategic asset. A recently promoted Commander with minimal engagement history operating with full autonomy in a complex situation is a liability.

A bloodline Commander can be recalled from any army during the strategic layer and reassigned elsewhere. The recall takes in-world transit time. During that transit period, the army they left operates under AI Field Commander direction with the last active directive.

#### AI Field Commander

When no bloodline member is available or the player chooses not to commit one, an AI Field Commander occupies the slot. AI Field Commanders are competent. They execute directives reliably. They do not make catastrophically bad decisions under normal conditions. What they do not bring is anything personal -- no command aura, no conviction weight, no accumulated relationship with the troops, no tactical profile shaped by real engagement history, no faith alignment that amplifies morale.

An army under an AI Field Commander performs at its baseline capability. Nothing amplifies it beyond raw values. AI Field Commanders do not develop. They will not be the subject of assassination or capture operations by a sophisticated rival. They are reliable, generic, and replaceable.

---

### THE DIRECTIVE SYSTEM

When an army is operating under any Commander rather than the player's direct control, it operates under a Directive. The Directive is the standing order -- the mission the Commander pursues until the player changes it, the objective is achieved, or the conditions make it impossible.

Directives are set during the strategic layer commitment phase. They persist across battle cycles until the player changes them.

#### Territorial Directives

**Hold This Line** -- The army maintains its current position and engages anything that enters a defined perimeter threshold. Does not advance beyond defensive necessity. The player can specify the pursuit threshold.

**Advance to This Position** -- The army moves toward a specified map location and secures it, engaging resistance along the route as necessary. When the position is reached, the army defaults to Hold This Line unless a follow-on directive is pre-set. The player can chain directives.

**Secure This Region** -- Broader than Advance to Position. The army pacifies and establishes control across a defined territory. The Commander handles resistance as it finds it, establishes garrison sub-units at key locations, and reports mission complete when the region's overall stability threshold is met.

**Push This Front** -- An aggressive advance directive without a specified terminus. The Commander's tactical profile determines how aggressively they interpret the directive.

**Fall Back to This Position** -- The army withdraws to a specified location, managing the retreat as orderly as the operational situation permits.

**Contain and Watch** -- The army maintains a position at a defined distance from an opponent's territory, observing and reporting without initiating engagement unless the opponent crosses a defined threshold.

#### Combat Directives

**Eliminate This Opponent** -- The army prioritizes engagements with a specified enemy dynasty's forces above all other operational considerations. Sub-objectives can be set: eliminate their field armies while leaving infrastructure intact, target specifically the forces commanded by a named bloodline Commander.

**Suppress This Area** -- The army maintains combat pressure on a defined territory without committing to full conquest. Degrades the enemy's capacity to use that area productively without the full governance and supply cost of claiming and holding it.

**Intercept and Destroy** -- The army positions at a defined corridor or approach and engages enemy forces moving through it. Reactive rather than aggressive.

**Protect This Asset** -- The army positions around a specified settlement, resource node, trade route section, or bloodline member location and treats its defense as the primary mission.

#### Operational Directives

**Secure This Resource** -- The army moves to claim and hold a specified resource node, handling whatever opposition it finds and establishing garrison infrastructure.

**Establish Forward Position** -- The army advances to a specified location and constructs a forward base there, including initial fortification, supply depot, and garrison structure.

**Clear This Route** -- The army patrols a specified supply or trade route, engaging hostile forces encountered along it and returning to the route after each engagement.

**Support This Operation** -- The army positions to provide military cover for a covert or faith operation in progress within a defined area. The player sets both the support radius and the engagement trigger.

#### Diplomatic Directives

**Demonstrate Force** -- The army moves to the border of a specified dynasty's territory and establishes a visible presence without initiating engagement. A political signal with military expression.

**Monitor and Report** -- The army maintains observation of a specified area and generates regular intelligence reports without engaging.

**Enforce Boundary** -- The army intercepts and engages any force crossing a defined territorial line without standing authorization. The player specifies the line and the exception list.

#### Siege Directives

**Encircle and Starve** -- The army encircles a target settlement and maintains the encirclement, denying resupply without initiating direct assault unless explicitly ordered otherwise. The player is given an estimated resolution window at directive set and updated as conditions change.

**Assault When Ready** -- The army prepares a siege and initiates the assault when the Commander judges conditions favorable. The player can override the Commander's judgment at any time by switching to direct control.

**Siege and Negotiate** -- The army encircles the target but the Commander is authorized to receive and evaluate surrender offers within player-specified acceptable parameters. Offers below the parameters are rejected without escalation. Offers at or above the parameters are accepted autonomously.

---

### ATTITUDE MODIFIERS

Applied on top of any directive, attitude modifiers shape how the Commander interprets ambiguous situations.

**Aggressive** -- When in doubt, engage. Press advantages as they appear. Accept higher casualty rates in exchange for faster objective completion.

**Measured** -- Execute the directive at a sustainable pace. Engage when conditions favor engagement. The middle position appropriate for most standard operational situations.

**Cautious** -- Minimize risk. Set retreat thresholds conservatively. Prioritize force preservation over objective completion speed.

**Economical** -- Minimize resource expenditure alongside objective completion. Avoid costly sieges that can be bypassed. The Goldgrave Commander disposition.

**Relentless** -- Do not stop. Do not allow routed enemies to recover before re-engaging. Generates higher army attrition over time but produces faster objective completion.

**Disciplined** -- Honor all standing conviction constraints and diplomatic agreements even when violating them would benefit the immediate objective.

**Opportunistic** -- Execute the primary directive but act on significant opportunities that emerge within the operational area even if outside the directive's strict scope. High-autonomy modifier requiring strong trust in the Commander's judgment.

Attitude modifiers stack with the Commander's personal profile. An AI Field Commander under any modifier executes that modifier more literally and less adaptively than a bloodline Commander.

---

### THE DIRECT CONTROL TOGGLE

At any point during the match, the player can switch any army from Commander direction to direct player control. The toggle is immediate. The transition is seamless.

If the player switches to direct control of an army mid-engagement, they enter it in its current state. A battle going badly under Commander direction can be rescued by the player. When the player releases direct control, the Commander resumes authority under the last active directive.

The Commander does not disappear during the player's direct control window. They remain present in the battle as a passive morale anchor and command aura provider. The Commander's passive bonuses apply throughout.

---

### COMMAND CAPACITY AND THE OVERWHELM MECHANIC

There is no numerical cap on how many armies the player can have active simultaneously. No hard wall exists.

What exists is a different kind of constraint. The player's personal attention is finite. When they are in direct control of a battle, every other army in the match is operating under Commander direction. The bloodline member Commander investment is what makes large-scale empire management viable.

The player who attempts to personally command everything in a large empire will not be stopped. They will simply discover, through failures on the fronts they are not attending, that attention is the scarcest resource in the game. The empire that grows large enough to be genuinely difficult to manage is the empire that is playing Bloodlines at the scale it was designed for. The overwhelm is the point. The Commander system is the infrastructure that lets a player manage that overwhelm intelligently rather than by simply having less.

---

### COMMANDER DEVELOPMENT OVER THE MATCH

Bloodline member Commanders develop in ways that AI Field Commanders do not.

A bloodline Commander gains campaign experience from each engagement they lead -- not a simple counter but a qualitative record of what kind of engagements they have led, what outcomes they produced, what tactical situations they navigated. A Commander who has never led a siege and is now assigned an Encircle and Starve directive is operating outside their experience, and the directive execution will reflect it.

Faith intensity deepens naturally for bloodline Commanders through sustained military service. High-faith bloodline Commanders carry that intensity into the armies they lead -- affecting the morale of faith-aligned troops around them and generating passive faith intensity in their operational area.

Conviction shapes and is shaped by the Commander's tactical record. A bloodline Commander who has consistently honored prisoner status and operated within the dynasty's established conviction posture develops a Moral conviction record that is their own. The Commander's conviction history affects how their troops respond to them.

The Legendary Member emergence event can occur to bloodline Commanders who have accumulated sufficient development across all these dimensions. A Commander whose engagement history, faith depth, conviction coherence, and notable achievements cross the Legendary threshold becomes a match-world significant figure. Other dynasties know who they are. They become assassination targets for sophisticated rival operations. Their death in battle is registered as a major match event. The investment in developing a capable bloodline Commander to Legendary status is among the highest-return strategic investments a player can make.

---

### HOW THE COMMANDER SYSTEM AND THE TIME MODEL INTERLOCK

The Commander system and the declared time model are two halves of the same solution to the same problem.

The declared time model makes the dynasty game real by advancing in-world time meaningfully between battle engagements. The Commander system makes the military game manageable across a large empire by giving the player genuine delegates rather than abstract AI behavior. Together they produce the specific experience Bloodlines is designed to create: a player fighting a real battle in the foreground while a real empire runs -- imperfectly, according to the quality of the Commanders and directives the player has established -- in the background.

During the strategic layer commitment phase, the player reviews their Commanders and directives. The events queue has told them what changed during the last batch of declared time. The player adjusts directives that no longer match the situation, reassigns Commanders whose positions have changed, initiates operations that the elapsed time has made feasible, and commits movement orders for armies that need to reposition before the next major personal engagement.

This is the game. These are the decisions that matter. The battle is dramatic. The strategic layer is where those battles are won and lost before they happen.

---

### CROSS-REFERENCES

- Section 36 (Siege and Naval Warfare): siege mechanics should cross-reference the Siege Directives section above.
- Section 37 (Bloodline Member in Combat): the Commander system extends this section's treatment of bloodline member battlefield presence.
- Section 38 (Army Composition and the Recruitment Slider System): command capacity interaction with army composition.
- Sections 63-68 (Operations System): the Support This Operation directive integrates directly with the covert operations layer.
- Section 80 (Declared Time Model): the Commander system and declared time are the dual-clock architecture. Read both together.

---

*End of Part XV*
*Sections 80-81 integrate content from BLOODLINES_ADDENDUM_A_2026-04-07.md*

---

*End of Part XV*

---

## PART XVI — SESSION 14 INTEGRATION (2026-04-14)

*This part is appended per Bloodlines additive archival rules. It does not supersede or reduce any prior content. It integrates the Dynasty Consequence Cascade runtime expansion (2026-04-14) and the Defensive Fortification Doctrine (2026-04-14) canonically locked during the 2026-04-14 session.*

### Section 82: Dynasty Consequence Cascade — Runtime Integration (2026-04-14)

The Dynasty Consequence Cascade is the first implementation-level expansion that makes the bloodline roster consequential in live simulation. It was designed and implemented 2026-04-14 in the canonical browser runtime (`src/game/core/simulation.js`) and the bridge UI (`src/game/main.js`, `src/game/core/renderer.js`). This section documents the canonical behavior for all future sessions, implementations, and bible versions.

**Commander capture versus kill.** When a unit carrying a commander bloodline member takes lethal damage, the simulation resolves the member's fate by checking for hostile kingdom-faction combat units within a canonical capture proximity radius at the moment of death. If a hostile combatant is in range, the member is captured by that faction. Otherwise the member is killed (marked fallen).

**Captive ledger.** A captured member is transferred to the captor faction's `dynasty.captives` list. The captive carries the original title, role, path, age, renown, and the time of capture. The source faction's `dynasty.attachments.capturedMembers` map records which faction holds each captive.

**Fallen ledger.** Every member loss (killed or captured) is appended to the source faction's `dynasty.attachments.fallenMembers` log with disposition (fallen / captured / displaced), attacker or captor faction id where relevant, and the time of the event. The ledger is capped at twelve entries, oldest overwritten.

**Role succession cascade.** When the head of bloodline is lost (killed or captured), the heir designate is promoted to head of bloodline with a +6 renown increase and a status transition to "ruling." Legitimacy takes an 18-point hit at the moment of loss and recovers by 7 when succession succeeds. If no heir is available, the dynasty enters interregnum with an additional 14-point legitimacy shock and a message log event. The heir slot is then back-filled from the canonical succession chain (commander, governor, diplomat, ideological leader, merchant, spymaster) via role promotion.

**Governor loss on territory flip.** When a control point changes ownership, the previous governor member's fate is resolved the same way as a commander: hostile combatants at the flip location cause capture, otherwise the governor is displaced. A displaced governor is restored to "active" status once the realm again holds at least one territory. Territory loss produces a 5-point legitimacy hit on the previous owner plus a one-point stewardship penalty on the conviction ledger.

**Ransom influence trickle.** A faction holding captives accrues influence over time: a base trickle per captive plus a renown-weighted bonus. This produces a canonical ransom economy that makes holding captives strategically valuable and sets the foundation for the future explicit ransom / rescue operations layer.

**Conviction ledger integration.** Capture events register to the captor's ruthlessness and stewardship buckets. Kill events register to the killer's ruthlessness bucket. Stabilization of captured territory registers to the owner's oathkeeping and stewardship buckets. Dark-doctrine commitment registers desecration, light-doctrine commitment registers oathkeeping. The conviction band label (Apex Moral through Apex Cruel) is derived continuously from the four-bucket scoring.

**Unit-death finalization.** A refactor of the damage pipeline centralizes death effects in a deferred finalization step. `applyDamage` now only reduces health and records the killer faction. A `finalizeUnitDeaths` pass runs at the end of each tick and handles commander fall, population decrement, and message log entries. Command Hall destruction uses the same pattern via `finalizeBuildingDeaths`. This makes the commander cascade fire for any lethal path (damage, starvation, apocalyptic, future disease), not just explicit damage.

**UI integration.** The dynasty panel now surfaces: legitimacy, active member count, interregnum flag, current commander attachment, heir line, governor assignment, held captives (with source faction and renown), fallen and captured history, and living members. The debug overlay now surfaces: dynasty legitimacy, heir label, captive count, fallen count, and the four-bucket conviction ledger.

**Verification.** The expansion is validated by `tests/runtime-bridge.mjs`, which boots a simulation, forces a commander unit to die adjacent to an enemy combat unit, asserts the capture path, asserts the captor's ledger increments, asserts the succession replacement, asserts ransom influence accrues over simulated time, and in a second scenario asserts the head-of-bloodline succession cascade promotes a new head when no captor is nearby. `tests/data-validation.mjs` continues to pass with the extended doctrine-shape assertions. All runtime files pass `node --check`. Live browser boot shows the updated dynasty panel and debug overlay rendering correctly with zero console or network errors.

**Canonical integration.** The Dynasty Consequence Cascade is the concrete expression in runtime of the canonical dynastic design as documented in Section 10 through Section 15 of this bible. It formalizes: the rarity of aging-out-natural death (canon 2026-03-18), the player-choice succession model (canon 2026-03-19), the captured bloodline handling (canon 2026-03-18 and 2026-03-26), and the bloodline-as-living-family depth (canon 2026-03-19). It sets the foundation for the Captured Heir Homeland Penalty, the ransom economy, rescue operations, and generational succession mechanics that later sessions will expand.

---

### Section 83: Defensive Fortification Doctrine — Full Canonical Text (2026-04-14)

The Defensive Fortification Doctrine was canonically locked on 2026-04-14 by Lance W. Fisher direction. It is a canonical strategic pillar, not optional flavor. The full doctrine is reproduced verbatim below, followed by interpretive depth and implications.

#### Purpose

Bloodlines must treat defensive fortification as a major and fully viable strategic path, especially around primary keeps, bloodline seats, and high-value dynastic strongholds. The design goal is not for defense to be cosmetic, symbolic, or merely delaying. The design goal is for meaningful investment into fortification to create real security, real deterrence, and real military difficulty for the attacker. A player who makes significant and deliberate investment into defending their realm, especially the main keep where the head of the bloodline resides, must feel that the investment matters and cannot be casually erased by repeated wave attacks, suicide assaults, or late-game mass rushing.

A heavily fortified bloodline seat must not behave like a normal structure with a larger health pool. It must function as the center of an integrated defensive network. The keep and its surrounding systems should form a layered fortress complex that is difficult to breach quickly and costly to assault recklessly.

#### The Ten Doctrine Pillars (Verbatim)

**1. Fortification must be layered, not singular.** A defended stronghold should be capable of having meaningful outer defenses, inner defenses, and final defensive cores. The fall of one defensive ring must not automatically render the entire base tactically meaningless. Breaching the perimeter should be an achievement, not the whole battle.

**2. Defensive systems must support one another.** Walls, gates, towers, defensive emplacements, garrisons, chokepoints, kill zones, signal systems, reserve mustering points, and inner fallback positions must function as a connected defensive ecosystem rather than isolated health-bar objects.

**3. Deep investment in defense must create real defensive leverage.** A player who commits heavily to fortification should gain more than time. That investment should meaningfully increase the attacker's losses, slow assault tempo, increase siege requirements, create higher operational risk, and strengthen the defender's ability to stabilize and recover the position.

**4. Bloodline keeps and capital seats must be especially resilient if properly fortified.** A primary keep housing key bloodline figures, succession lines, or dynastic command should be among the hardest targets in the game when fully developed. The attacker should need a real siege plan, not just repeated unit spam.

**5. Wave-spam and suicide erosion must not be a dominant answer to fortification.** The game must not allow heavily defended strongholds to be reliably broken by mindless repetition alone. Failed assaults should matter. Repeated reckless attacks should result in real military waste, tempo loss, morale or cohesion penalties where appropriate, and increased defender advantage rather than automatically guaranteeing eventual structural collapse.

**6. Siege should be an earned military operation.** To break elite fortifications, attackers should often require real preparation such as siege engines, engineers, logistics, supply continuity, scouting, breach planning, elite units, faith powers, sabotage, coordinated multi-front timing, isolation of the target, or similar serious commitments. Assaulting a major fortress should feel like a campaign objective, not a casual click action.

**7. Late-game defense must remain relevant.** Do not allow offense to scale upward so completely that defensive investment becomes meaningless in the late game. Bloodlines must preserve the viability of fortress-centered strategy all the way through longer and larger matches. High-tier defensive development should unlock stronger and more specialized forms of fortification, recovery, and layered resistance.

**8. Defensive commitment should have tradeoffs, but the reward must be real.** A player who heavily invests in fortification may accept opportunity costs in army size, expansion speed, economy tempo, or other areas. That is acceptable and desirable. But once that investment is made, the resulting fortress must provide authentic strategic value and not merely superficial delay.

**9. Different settlement classes should have different defensive expectations.** Border settlements, military forts, trade towns, regional strongholds, and primary dynastic keeps should not all defend the same way or reach the same defensive ceiling. Major bloodline seats and intentionally developed fortress-citadels should represent the highest expression of defensive power in the realm.

**10. The player should feel the difference between an unfortified realm and a fortified one.** A neglected seat of power should be vulnerable. A disciplined, well-fortified capital should command respect.

#### Implications (Interpretive Depth)

**Layered architecture.** Fortified settlements are composed of outer works (first contact surface: earthen berm, palisade, abatis, forward towers, gatehouse with killing bay), inner ring (primary curtain walls, flanking towers, main gatehouse with murder holes and machicolations, shielded firing platforms, garrison barracks, cisterns, reserve mustering yards), and final defensive core (inner keep or great hall, bloodline seat, throne chamber, deep supply stores, faith inner sanctum, emergency reserve garrison, house-specific features such as Ironmark blood reservoir or Old Light pyre chamber). A defender who invests only in one layer has built a shell, not a fortress.

**Defensive ecosystem.** Walls must be covered by towers. Gatehouses must channel attackers into killing bays. Garrisons must cycle through reserve yards. Signal infrastructure must alert inner sections in time. Chokepoints must channel attackers through planned fire corridors. Inner fallback lines must exist between a breach and the final core. Supply must be part of the defensive system, not a separate economy concern. Cutting any connection weakens the whole.

**Defensive leverage forms.** Attritional cost multiplier (attacker losses exceed defender losses proportional to fortification depth), tempo drag (time spent besieging is time not spent expanding), siege requirement escalation (line infantry cannot take an elite fortress), operational risk imposition (besiegers face sorties, relief armies, coalition intervention, faith counter-powers, home unrest), defender recovery (repulsed assaults produce morale swings, reinforcement arrival windows, fortification repair under cover).

**Wave-spam denial mathematics.** A failed assault against an intact fortified position produces unit loss scaling with fortification depth, morale and cohesion degradation requiring reset time, supply drain on the besieging force, a repair window for the defender, and possible reinforcement arrival. Repeated reckless assaults compound these penalties. The simulation refuses to reward wave-spam as a reliable path to structural collapse.

**Bloodline presence bonuses.** Dynasty members in the keep produce measurable defensive bonuses by role: head of bloodline confers garrison cohesion and loyalty surge, heir designate confers succession-intact resolve, commander confers defensive aura extending into garrison coordination, governor confers supply-under-siege bonus and repair tempo, ideological leader confers faith intensity and covenant defensive operations access, diplomat reduces coalition-forming difficulty for relief, spymaster confers counter-sabotage bonus and siege-preparation visibility, merchant confers supply acquisition bonus, sorcerer (mysticism path) confers specialist faith or mystic defensive operations.

**Settlement class hierarchy.** Border settlement (low defensive ceiling, warn and retreat), military fort (mid ceiling, hold against raids and modest sieges), trade town (mid ceiling, protect economic core), regional stronghold (high ceiling, force a real siege), primary dynastic keep (apex ceiling, bloodline seat), fortress-citadel (apex+ ceiling, actively-invested primary keep, the hardest target in the realm). Investment deepens within-class; class promotion requires major material and political commitment.

**Faith integration.** Faith buildings inside fortified enceintes alter the defensive equation. Old Light pyre wards project vigilance and counter-sabotage visibility. Blood Dominion blood-altar reserves fuel defensive rites with morale surge at sacrifice cost. The Order edict wards enforce garrison discipline and reserve cohesion. The Wild root wards entangle approach and produce defensive terrain advantage. Doctrine path modifies behavior. Apex-tier faith buildings inside a keep produce canonically powerful defensive effects.

**Tradeoffs and reward.** A fortress-path realm accepts slower map expansion, smaller field armies, economy tempo reduction, stone-and-iron-biased resource flow, and commander commitment to defensive governance. In exchange, the realm gets canonically apex defensive position, measurable siege-denial against ordinary attackers, coalition-defiance capability, and dynasty preservation under siege. Both paths are canonically viable; neither is dominant.

**Felt difference.** The fortified realm is visibly different (walls, towers, banners, garrisons visible, supply trains moving, signal fires lit when threatened), auditorily different when audio lands (horns, bells, drums, faith chants), behaviorally different (NPCs, population, and AI treat a fortified realm with more caution), numerically different (measurable resistance, attacker cost, and recovery in combat logs), and narratively different (political weight; a fortified keep commands respect; a neglected one invites opportunism).

**Doctrine non-negotiability.** The Defensive Fortification Doctrine is a canonical strategic pillar. No session may reduce, dilute, or contradict the doctrine's ten pillars without explicit authorization from Lance.

---

### Section 84: Fortification System — Defender Specification (2026-04-14)

The Fortification System specification is preserved in full at `04_SYSTEMS/FORTIFICATION_SYSTEM.md` (created 2026-04-14). Essential summary for bible integration:

**Settlement class defensive ceilings.** Border settlement, military fort, trade town, regional stronghold, primary dynastic keep, fortress-citadel — each with distinct defensive ceilings. Promotion between classes requires major material and political commitment.

**Layered defense architecture.** Three concentric layers: outer works, inner ring, final defensive core. Each layer is a separate construction target with its own progression. Each breach is a separate control event.

**Defensive ecosystem components.** Walls, gates, towers, emplacements, garrisons, chokepoints, kill zones, signal systems, reserve mustering yards, inner fallback positions, granaries, cisterns, arsenals, and house-specific reservoirs.

**Defensive leverage mathematical structure.** Attritional cost multiplier, tempo drag, siege requirement escalation, operational risk imposition, accelerated defender recovery, assault failure penalties.

**Failure penalty model.** Unit loss scaling with fortification depth, morale and cohesion degradation, supply drain, repair window, possible reinforcement arrival. Repeated reckless assault compounds penalties.

**Bloodline presence bonuses.** Nine canonical role-based bonuses (see Section 83 Implications).

**Faith integration.** Covenant-specific defensive expressions per faith doctrine. Apex-tier faith buildings inside the keep produce canonical defensive effects.

**Specialist population categories.** Garrison troops, engineers and sappers, signal keepers, wall wardens, tower artillerists (late-game), keep guard. Distinct from field army populations.

**Recovery and repair.** Partial damage at structural segment granularity. Repair rate driven by engineer population, material supply, and section control. Post-assault repair window between assaults.

**Late-game apex fortification.** Grand wall projects, citadel keeps, shielded inner cores, reinforced gatehouses, hardened towers, specialized garrison barracks, faith-warded inner sanctums.

**Economy tradeoffs.** Smaller field army, slower expansion, economy tempo reduction, stone/iron/wood bias, commander commitment to defensive governance. In exchange: canonically apex defensive position, coalition-defiance capability, dynasty preservation.

**Implementation milestones.** Phase F1-F8 roadmap from control-point fortification metadata through wall/tower/gate structural entities, reserve cycling, bloodline presence bonuses, faith-warded inner sanctums, assault failure penalty math, late-game apex unlocks, and fortress-path AI specialization.

---

### Section 85: Siege System — Attacker Specification (2026-04-14)

The Siege System specification is preserved in full at `04_SYSTEMS/SIEGE_SYSTEM.md` (created 2026-04-14). Essential summary for bible integration:

**Canonical posture.** Siege is a campaign-scale commitment. Line infantry alone cannot reduce an elite fortress. An attacker who fails to prepare loses units and tempo with little to show for it.

**Required commitment categories.** Plural commitments are required for serious siege: siege engines (rams, siege towers, trebuchets, ballistae, mantlets, late-game bombards), engineer specialists (dedicated specialist population for engine operation, earthworks, mining, counter-mining, ram escort), logistics and supply continuity (supply camps, wagons, foraging, rear-area security), scouting and intelligence (weak sections, garrison patterns, reserve locations, faith and bloodline presence, supply status), breach planning (target selection, approach preparation, supporting fires, breach assault force, flooding reserves, exploitation plan), elite units (heavy assault infantry, faith-blessed units at L3+ intensity, sworn companions, apex units at L5 faith intensity, Born of Sacrifice assault-consecrated elites), faith powers (covenant-sanctioned siege operations per doctrine path), sabotage (gate-opening, fire-raising, supply poisoning, defector turning, counter-mining sabotage, spymaster operations), coordinated multi-front timing (diversions, feints, simultaneous multi-gate assaults, dynastic-political timing, weather or nightfall exploitation), isolation (road interdiction, reinforcement interception, depot destruction, political isolation, faith isolation).

**Assault model.** Seven canonical phases: preparation, initial bombardment, breach formation, assault launch, exploitation, reserve commitment, resolution.

**Assault failure consequences.** Explicit attacker penalties: elite unit loss at high rates, morale and cohesion degradation, supply drain, tempo penalty, visibility penalty (defender intelligence gain from repulsed attacks), political penalty (coalition shift, unrest, conviction consequences).

**Siege tempo.** No instant assault after arrival; preparation phases are real. Assault cooldown between attempts. Siege duration scales with fortification class. Relief windows exist throughout a siege. Siege operations anchor stage 4 and stage 5 of canonical match structure.

**Counter-operations by defender.** Sorties, counter-mining, faith counter-powers, signal-triggered relief, dynastic appeals, spymaster counter-operations.

**AI behavior requirements.** Attacking AI must identify target fortification class, refuse insufficient-force attacks on elite fortifications, plan siege preparation, protect supply lines, time assaults for defender weakness windows, recognize repulsed assaults as losing branches, coordinate multi-front and isolation operations, seek faith and sabotage force multipliers before committing to assault.

**Conviction consequences.** Committing a serious siege adds ruthlessness; massacre after successful breach adds strong ruthlessness and often desecration; sparing the defender adds oathkeeping and stewardship; wave-spam failure adds minor desecration (dishonor of the fallen by reckless waste); victory through sabotage alone adds desecration under most doctrines; victory through honorable siege adds oathkeeping. Siege outcomes feed the four-bucket conviction ledger implemented 2026-04-14.

**Late-game siege scaling.** Bombards and heavy trebuchets, multiple coordinated engineer teams, faith apex rites, dynastic commander commitment to siege command role, coalition siege with multiple allied dynasties, extended campaign logistics.

**Implementation milestones.** Phase S1-S10 roadmap from siege engine unit class and build pipeline through engineer specialist population, wall-segment damage model, assault phase state machine, siege supply line model, scouting surface, multi-front coordination, faith-siege integration, sabotage operations, and AI siege planning behavior.

---

### Part XVI Cross-References

- Section 16 (Ten Terrain Types): fortification interacts with terrain defensibility.
- Section 18 (Minor Tribes): tribal raiders face fortified and unfortified settlements differently.
- Section 36 (Siege and Naval Warfare): extends and elaborates prior siege content with the canonical doctrine.
- Section 37 (Bloodline Member in Combat): commander capture and defensive aura mechanics formalized.
- Section 51 (Dynastic Prestige Modifier System): keeps and bloodline seats are a central expression.
- Sections 63-68 (Operations System): sabotage and covert operations integrate as siege enablers.
- Section 80 (Declared Time Model): multi-year siege campaigns use declared time.
- Section 81 (Commander System): defensive governance Commanders anchor the keep.
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`: canonical source of the doctrine.
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md`: defender specification.
- `04_SYSTEMS/SIEGE_SYSTEM.md`: attacker specification.
- `04_SYSTEMS/TERRITORY_SYSTEM.md` (2026-04-14 addendum): territorial integration.
- `04_SYSTEMS/DYNASTIC_SYSTEM.md` (2026-04-14 addendum): keep-as-bloodline-seat integration.
- `11_MATCHFLOW/MATCH_STRUCTURE.md` (2026-04-14 addendum): fortress pacing across the five stages.
- `10_UNITS/UNIT_INDEX.md` (2026-04-14 addendum): fortification and siege unit class architecture.
- `08_MECHANICS/MECHANICS_INDEX.md` (2026-04-14 addendum): mechanics-layer cross-interactions.

---

*End of Part XVI*
*Sections 82-85 integrate content from the 2026-04-14 session: Dynasty Consequence Cascade runtime expansion and Defensive Fortification Doctrine canonical lock.*

---

*End of Design Bible v3.3*
*This document incorporates all design decisions through Session 14 (2026-04-14).*
*Version 3.3 supersedes Version 3.2 for all active design purposes.*
*Version 3.2 and prior are preserved in 18_EXPORTS/ and 19_ARCHIVE/ per additive-only archival rules.*
*Lance Fisher is the sole authority on canon, deprecation, and removal.*

---
