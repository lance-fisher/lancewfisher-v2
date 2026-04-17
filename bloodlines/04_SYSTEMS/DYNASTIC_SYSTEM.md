# Bloodlines — Dynastic System

## Overview
The Dynastic system is the heart of Bloodlines. It models the ruling bloodline of each faction across generations. Leaders are born, grow, rule, and die. Their traits, marriages, children, and legacies shape the faction's capabilities and identity over the course of a match. The dynasty is what makes each playthrough unique.

## Design Intent
The dynasty creates personal stakes in a strategy game. Players do not just manage abstract resources. They shepherd a family through triumph and tragedy. Succession crises, brilliant heirs, disastrous marriages, and sacrificed children all create emergent narrative. The dynasty is the source of the game's title and its emotional core.

## Open Design Questions
- How are bloodline traits inherited? (Genetic model, random, player-influenced?)
- How does succession work? (Primogeniture, elective, merit-based, faith-determined?)
- What happens when the dynasty has no heir?
- How do marriages between houses affect gameplay?
- Can dynasty members be sacrificed through the Born of Sacrifice system?
- How do leader traits affect faction-wide bonuses?
- What is the lifespan model? (Fixed turns, event-driven, age-based decay?)
- Can players groom specific heirs or is succession a managed risk?

---

*Detailed mechanics will be appended here as design sessions explore this system.*

### Design Content (Added 2026-03-15)

The dynastic system is one of the four foundational design pillars alongside territory, population, and faith.

**Bloodline Members:**
- 20 active members maximum, beyond dormant
- Born into the dynasty over time
- Paths chosen at birth, trained from childhood
- Training paths: military command, governance, religious leadership, diplomacy, covert operations, economic stewardship/trade, and mysticism (less focused on expanding and increasing faith, more focused on the offensive/defensive abilities of faith in war)
- Become active leaders upon maturity
- Possible roles: commanders, governors, diplomats, ideological leader (priest, priestess), merchant, sorcerer

**Family Tree UI:**
The family tree is a central gameplay element. Player must always know:
- Who leads the house
- Who the heirs are
- Who belongs to the dynasty
- Their specialization
- Their heritage
- Their reputation with the population

**Dynasty Interactions:**
- Capture of rival bloodline members
- Marriage alliances (cross-dynasty)
- Ransom exchanges
- Enslavement
- Execution
- Assassination and covert warfare

**Captured Bloodline Handling:**
- Enslavement as workers
- Execution
- Ransom
- Marriage into the dynasty (if polygamy practiced — Blood Dominion and The Wild only)

**Polygamy Rules (Faith-Specific):**
Polygamy is permitted only under Blood Dominion and The Wild. In other faiths, members born into the bloodline can be withheld from marriage UNTIL a suitable enslavement or capture happens. Marriage does not happen naturally in Bloodlines. It is highly controlled and determined by the head of the household/player.

**Marriage and Hybrid Heirs:**
Cross-dynasty marriages create new dynastic branches. Children CANNOT freely declare loyalty to either bloodline or form a new dynasty. Marriage and dynastic branching are highly controlled and determined by the head of the household/player. Branching can be PROPOSED in-game with a set of advantages or disadvantages presented to the player, but is ultimately determined by the player.

**War Heroes and Lesser Houses:**
Exceptional commanders earn titles and lands, creating lesser houses under the main dynasty. Lesser houses remain loyal but become their own branches.

**Unit Promotion System (Combat-to-Dynasty Pipeline):**
In battle, a swordsman or other combat unit not directly affiliated with the bloodline gets ranked up through combat across multiple stages (5 promotion tiers). Each promotion tier creates cumulative advantages for the player. For example, an army of 5 promoted swordsmen would likely beat an army of 5 regular class axemen, despite situations where that edge would be given to the axemen if they were equally matched otherwise. Beyond 5 promotions, the unit may be offered a promotion to command an army in a role similar to a general or commander. After multiple successful campaigns, they may be offered lands and titles, becoming a lesser house affiliated with the bloodline dynasty.

**Recruitment:**
Adjustable sliders (not hard-locked doctrines). Mix of family obligations, paid soldiers, faith volunteers. Including daughters produces negative attack buffer but positive healing/sustainment buffer.

**Starting Leader Options:**
- Father (born during The Frost) / 30-40 years old
- Eldest Son / 10-20 years old
- Second Eldest / less than 10 years old
- Brother of the King (uncle) / 25-35 years old (slightly younger than father)
- Brother of the King's first eldest / 12-16 years old

**Lesser Houses:**
Optional AI-autonomous or player-controlled. Can act independently as AI entities or be directly controlled by a player.

---

### Player Manual Expansion (Added 2026-03-15)

**Bloodline Members on Battlefield:** When a bloodline member leads an army, the army gains dynastic weight, not merely a combat buff. Their presence affects morale, legitimacy, reputation, succession security, and political meaning of the war. Best players learn not only when to risk a member, but which member, for what purpose, in what theater, with what political backup.

**Captured Heir Consequences:** A captured heir is a structural wound. If the next rightful leader is in bondage, the homeland suffers: confidence weakens, stability cracks, rival claimants gain ground, external powers sense vulnerability, internal factions imagine alternative futures.

**War Heroes Expanded:** Military success generates political and dynastic capital. A victory that creates a hero may change the structure of the realm more than the land seized. War heroes become sources of loyalty, legitimacy, fear, and dynastic narrative.

**Mixed Bloodline Dynasties:** If a bloodline repeatedly takes wives of another bloodline, children create complex inheritance and legitimacy dynamics. Creates emergent dynastic webs, rival claims, legitimacy disputes. Late game allows newly formed dynasties to emerge from royalty of older houses to make claims over current leaders. However, all branching and loyalty declarations are controlled by the head of the household/player, not freely chosen by children. The game provides ample notification about the status of bloodline members or children born seeking to branch off, and the head of the bloodline makes the final decision.

**Bloodline Loyalty Slider:** A slider tracks the intentions of children and members regarding defection:
- Loyal to the head of bloodline (vassal)
- Neutral (NPC-like behavior)
- Seeking to join a different bloodline entirely
Based on the decision of the head of the family, this could result in forced combat between them. On the reverse side, members of other bloodlines may seek to join yours, which requires a successful diplomatic action to initiate.

**Starting Leader Expansion:** Each starting choice has unique advantages, playstyles, and disadvantages with lore justifications (oldest unable to bear children, father old or sick, brother being incredible war general). Hidden synergies exist for replayability. A random factor can be determined at match start and disclosed to Houses.

**Lesser Houses Fracture:** Lesser houses can fracture under extreme conditions. This is a new mechanical concept.

---

### Full System Design — CB003 + Session 9 Expansion (Added 2026-03-19)

**STATUS: PROPOSED / CB003 + SESSION 9 EXPANSION — AWAITING CANONICALIZATION**

---

## Section 1: System Identity

The Dynastic System is the emotional core of Bloodlines. Not one element of the emotional core, not the emotional layer sitting beside the mechanical layer — the core. Everything else in the game eventually registers its consequences through the dynasty: a famine is a famine that culls your children, a border dispute is a war that puts your son in the field, a succession is a death in the family followed immediately by a question about who runs the realm while enemies are watching.

What the Dynastic System is not is a stat-management layer. It does not ask the player to optimize a set of attribute numbers. It asks the player to shepherd a family through triumph and tragedy across a match that can run two hours or ten, and to make decisions about real people — people the player has named, watched grow, committed to roles, and invested in — with real consequences that cannot be undone. When a War General dies in a bad campaign, the player loses a committed member with years of development, an active bonus structure, a role that now needs to be filled, and a person whose portrait will go on the gallery wall. All of those things happen simultaneously.

This is where Bloodlines separates itself from the rest of the strategy genre. In a conventional RTS, the player manages abstract units — HP bars, attack ratings, production queues. The death of a unit is a resource transaction. In Bloodlines, the dynasty makes the deaths legible as losses. The player has a reason to not want them to happen beyond efficiency. They have watched the War General grow from an adolescent committed to the role, watched them develop across campaigns, watched them age in the Keep. Losing them is not a resource event. It is a narrative event.

This places Bloodlines in a tradition that Crusader Kings II established and that no real-time strategy game has attempted at full fidelity: personal stakes embedded in a strategic framework. In Crusader Kings II, the personal stakes exist within a turn-based or slow-time political simulation. Bloodlines takes that same emotional architecture and places it inside a real-time engine — with all the time pressure and irreversibility that real-time implies. The player cannot pause and deliberate over a succession. The world does not stop when the head of bloodline dies. The armies keep moving. The enemies keep calculating. The succession happens in a live match, and the decisions the player makes about who leads next will determine whether the dynasty survives the next twenty minutes.

The dynasty is also what makes every match unique in a way that map layout and starting conditions cannot fully achieve. Two players can start from the same faction on the same map and produce entirely different dynasties, entirely different family stories, entirely different sequences of triumph and tragedy. The emergent narrative comes from the family. The army positions are tactical. The family is personal. When the match is over, what the player remembers is the family.

The Dynastic System does not cap the depth of this narrative. It amplifies it at every scale. A short two-hour match produces one complete generation. A ten-hour match produces three, with all the overlap and transition and grief and inheritance that three generations of a ruling family living through war and expansion and crisis implies.

---

## Section 2: The Three-Tier Family Model — Full Specification

The three-tier model is the structural solution to the generational scale problem. A match long enough to produce forty or fifty living family members cannot require the player to individually manage each one without destroying the management experience. The three-tier model solves this by defining three zones of player attention, each with its own management depth and its own relationship to player agency.

---

### Tier 1 — Committed Members (8-12 Maximum)

Tier 1 is the player's inner circle. These are the members who are deployed in specific functional commitment roles — the eight canonical roles that define how a bloodline member contributes to the dynasty's strategic operations. The player manages these members individually and directly. They have portraits in the Keep. The player knows their names, their traits, their conviction posture, their faith intensity. When one of them dies, the player feels it.

The hard cap of eight to twelve committed members is a design discipline. It establishes the maximum management surface: the player is never responsible for more than twelve individually tracked deployments at once. Eight is the floor because eight roles exist in the commitment system. Twelve is the ceiling because it allows the player to carry a few committed members in secondary supporting capacities — an Heir Designate who is also functioning in a limited War General role, for example — without expanding the surface beyond what the interface can reasonably present.

Commitment is an assignment, not a permanent state, but it is not trivially reversible. Switching a member from one committed role to another carries a transition cost measured in match time and reduced effectiveness during the transition period. A member who has spent years developing in the War General role does not immediately produce full Diplomatic Envoy output the day they are reassigned. The transition represents the real cognitive and practical cost of a human being changing their entire professional function. Some traits make transitions easier — a Cunning member adapts faster than a Stalwart one. Some role combinations are particularly painful to transition between — pulling a deeply committed Faith Leader out of their role and into Trade Anchor creates a long period of faith intensity disruption that the player must manage.

Certain roles have trait requirements to maximize their value. The War General role produces its full potential only in a member with martial traits (Fierce, Brilliant, Stalwart, Reckless) or with a Warlord birth path. A Scholar birth path member committed to War General can execute the role — armies still respond to their command anchor — but their ceiling is lower, their development rate is slower, and certain elite development options within the role are locked behind trait combinations they cannot satisfy. This creates meaningful decisions at the point of commitment: the player is choosing not just what function this member fills but what ceiling that commitment can reach.

**The Eight Commitment Roles:**

The War General commands armies in the field. Their presence provides a command anchor — a persistent radius within which every unit benefits from morale bonuses, coordination bonuses, and reduced rout risk. An army without a War General is a collection of units operating on base behavioral parameters. An army with a strong War General is a single instrument. The General's personal attributes modulate which specific bonuses apply: a Fervent faith intensity General fighting a defensive campaign under Moral conviction produces a distinct effect signature from a Neutral conviction General running an aggressive expansion. The active ability is the Vanguard Surge — a high-risk personal commitment in which the General leads a concentrated push against a specific enemy formation. Success breaks the formation and generates a temporary army-wide morale surge; if the General is wounded or killed in the Surge, the army suffers immediate morale collapse. The Vanguard Surge has a multi-minute cooldown and demands respect from the player who uses it. When a War General dies or is captured, every army they commanded loses its anchor immediately. Morale penalties cascade. If the dead General was also the head of bloodline, succession triggers simultaneously, producing the most dangerous window the dynasty will face — no military leader and no political certainty at the same moment, in a live match.

The Trade Anchor commits to a trade route or caravan operation, traveling with or stationed along an economic corridor. Their presence transforms a basic commercial route into a high-value exchange: merchant partners increase engagement, diplomatic credibility opens higher-tier goods, and the Anchor's personal reputation functions as the dynasty's guarantee. A dynasty without a Trade Anchor can conduct commerce but cannot access luxury goods, high-tier resources, or cross-bloodline economic diplomacy at full potential. The active ability is the Strategic Negotiation — a brokered deal that standard trade mechanics cannot execute, opening a new resource connection, extracting a treaty term through commercial framing, or securing a short-term advantage during a rival's crisis. Strategic Negotiations require time, carry failure risk, and on failure may damage the relationship with the negotiating party. The Trade Anchor's Merchant Network attribute expands over time, quietly increasing the number of routes and partners available to the dynasty. When a Trade Anchor dies, all active trade in their corridor pauses or degrades.

The Territory Governor commits to a specific claimed region as the dynasty's permanent presence there. They do not roam. They anchor. Their commitment suppresses local revolt risk, generates a loyalty bonus to the territory's population, and ensures the dynasty's claim has a human face attached to it. A territory governed by a bloodline member is qualitatively more stable than one held only by administrative appointment. The active ability is the Governor's Proclamation — a formal act that can suppress an emerging revolt before it fires, offer a one-time loyalty surge to lesser houses in the region, or formally recognize a lesser house's service in a way that binds that house more tightly to the bloodline. The Proclamation's effectiveness scales with the Governor's established Roots — a Governor who has held a territory for many match years carries far more weight than one newly assigned. When a Territory Governor dies, their territory enters an immediate instability window. Revolt risk rises sharply. The Roots they built cannot be transferred to a replacement, and the gap is real.

The Faith Leader serves the dynasty's spiritual representation within an Old Light faith tradition. They perform rites, bless armies, consecrate claimed lands, and function as the living argument that the dynasty's power has moral and sacred weight. The active ability is the Grand Rite — a significant formal ceremony producing a major faith effect: consecrating a territory to raise its permanent loyalty ceiling, blessing an army before a decisive campaign, or performing a rite of passage that accelerates an heir's development. Grand Rites require preparation time, consume significant resources, and age the Faith Leader measurably. A Faith Leader who performs rites too frequently and too quickly deepens toward Fervent and then Apex intensity — states where their effects become more powerful and their physical cost more visible and accelerating. The Faith Leader's commitment is the most self-consuming of the eight roles; the player must decide how hard to push them.

The Covenant Officiant operates within the darker doctrinal paths — Blood Dominion and equivalent faith systems that extract power through deliberate, irreversible cost. They perform bindings: formal acts of covenant that extract significant benefits in exchange for permanent sacrifice. Under Blood Dominion, a Covenant Binding may sacrifice a peripheral member of the bloodline to produce a dynasty-wide permanent power bonus. The sacrifice is permanent. The member is gone. The power is real. The population knows, or suspects, what the Officiant does. Their presence generates a Fear Aura across the dynasty's political footprint — lesser houses comply through fear rather than loyalty, and some potential allies will refuse contact with a dynasty known to practice dark doctrine. When a Covenant Officiant dies, active Covenants enter unstable states. Bindings requiring ongoing maintenance begin to unravel, and some Covenants, if their Officiant dies mid-process, produce active backlash from whatever was being held in the binding.

The Spymaster builds and operates the dynasty's covert intelligence network. They are never on battlefields, never governing territory publicly. They are the person who knows things: about rival bloodlines, about lesser house loyalty, about the personal vulnerabilities of enemy leaders. A dynasty with a committed Spymaster operates with significantly more information than one without — they detect revolts before they fire, identify traitors, and track rival bloodline members. The active ability is the Intelligence Operation: targeted assassination of a rival bloodline member, loyalty subversion of a lesser house within rival territory, intelligence extraction, or counter-intelligence that purges compromised elements from the dynasty's own structure. Each operation consumes network resources and carries exposure risk. The Spymaster's Agent Web grows through successful operations and accumulated contacts. A senior Spymaster with a decade of match time has agents embedded across the map and is nearly impossible to neutralize without killing them directly.

The Diplomatic Envoy serves as the dynasty's formal representative in relations with other bloodlines, foreign kingdoms, and the broader political world. They travel, negotiate, and carry the dynasty's reputation in their person. A dynasty without a committed Envoy can still conduct diplomacy, but critical negotiations, alliance formations, and marriage arrangements all require an Envoy to achieve optimal outcomes. The active ability is the Grand Embassy — a significant diplomatic initiative that can ratify a major alliance, dissolve a conflict through formal negotiation, open a new trade corridor at the diplomatic level, or negotiate the release of a captured bloodline member. The Envoy's Reputation attribute compounds over time: a senior Envoy is welcomed and trusted differently from a new one, with personal relationships to specific foreign leaders that the player can leverage. Marriage Agreements between bloodlines always require a committed Diplomatic Envoy from the offering dynasty.

The Heir Designate is the formally named successor to the head of bloodline. They may simultaneously hold a secondary commitment role — an Heir Designate functioning as a War General is common — but their formal designation changes how the world treats them. Foreign parties negotiate with them as future heads. Lesser houses calibrate loyalty to include their expected future leadership. The succession event resolves faster, revolt risk is lower, and the new head accesses their full bonus structure more quickly when a formal Heir is already designated. The active ability is Shadow Governance — a period where the Heir begins assuming certain administrative functions of the current head, accelerating their development and creating visible continuity. The longer the Heir has been formally designated, the more established their position, and the stronger their Succession Stability bonus. When a designated Heir dies, the gap in formal succession planning fires an immediate Political Instability event.

The commitment interface presents the eight roles clearly. When assigning a member, the player sees the role requirements, their member's fit to those requirements (strong, adequate, or mismatched), and a projection of what the commitment will produce over the first match phase. The commitment confirmation is a deliberate act — the game marks it. This person is now committed. They are in the center of the story.

---

### Tier 2 — Active Roster, Uncommitted (Up to 20 Minus Committed Members)

Tier 2 is the visible family. These are members who are individually tracked, who appear in the Keep, who the player can see aging and developing over match time — but who are not currently deployed in a commitment role. They exist in the interface without demanding constant decisions. The player can check on them, can read their developing traits and birth path progression, can note that someone is growing into something interesting. But they are not asking the player for direct management. They are available.

Event-Driven Culling applies more heavily to Tier 2 than Tier 1. Committed members are somewhat protected by their role context — a War General on campaign is exposed to combat risk, but the player is aware of and managing that risk. Tier 2 members are exposed to the full ambient pressure of match conditions: long winters kill the youngest and oldest first, plague moves through the general population non-discriminatorily, civil conflicts catch people who are not safely positioned in role contexts. A dynasty in perpetual crisis will see its Tier 2 roster thin in ways that committed members are largely shielded from.

Tier 2 members develop over time without direct player intervention. Their birth path produces slow ongoing growth in the relevant attribute domains. Their traits express in small ambient behaviors visible in the Keep — a Fierce Tier 2 member in the Keep during a period of military crisis looks different from a Cautious one. Relationships between Tier 2 members develop: siblings establish bonds or rivalries, a Tier 2 member who admires a committed War General develops toward martial traits faster than baseline, a Tier 2 member married into another bloodline develops a dual loyalty complexity that the player must eventually address.

When a Tier 1 committed member dies, the succession interface for that role opens and presents the Tier 2 roster as candidates. The player selects a replacement. This is the primary function of Tier 2 as a mechanical pool: it is the source of continuity when Tier 1 members are lost. A dynasty with a rich, developed Tier 2 has options when a committed member dies. A dynasty whose Tier 2 has been thinned by culling events, by marriages out of the bloodline, by succession choices that pulled the best candidates into commitment — that dynasty feels the loss of a committed member acutely, because the replacement options are limited or mismatched.

What makes Tier 2 members distinguishable from each other is the same thing that makes Tier 1 members memorable: developing traits, birth path specializations, personal history within the match, relationships with other members. A Tier 2 member who witnessed the death of a parent in a succession crisis has a trait interaction with events involving loyalty and betrayal that a Tier 2 member raised in peacetime prosperity does not. The individual histories accumulate. The player who has been watching the Tier 2 roster across a long match knows its members better at hour six than at hour one.

---

### Tier 3 — Dormant Family (Beyond the 20-Active Cap)

Tier 3 is the living family beyond the management surface. These members exist — they are alive, aging, producing children at a background rate, dying of natural causes and illness and old age and match events that find them. The player does not directly manage individual Tier 3 members. The player manages the branches they form.

The Nested Branch system organizes Tier 3 by family branch. A branch is a subdivision of the extended family centered on a Branch Head — the eldest or most capable member of that sub-family, presented as the player's interface to everything in that branch. The player interacts with the branch as a unit: they can see the branch's general health and trajectory, can deploy the branch to a territory context as a strategic function, can manage the political relationship between branches and the inner family, can receive notifications when a branch member distinguishes themselves.

What the player can do with a branch as a unit includes sending an entire branch to anchor a distant territory — where the Branch Head serves a rough Governor function and the rest of the branch provides supporting presence — or assigning a branch to a specific strategic context (a cadet branch built toward trade routes, a cadet branch embedded in a lesser house through marriage). These are not precise commitment role deployments. They are broader strategic uses that take advantage of the branch's collective presence without requiring individual management.

Event-Driven Culling governs Tier 3 growth. The extended family cannot grow indefinitely. Match conditions — disease, harsh winters, territorial conflict — act as organic limiters. A dynasty that maintains good conditions for its people sustains a larger extended family. A dynasty in ongoing crisis sees the extended family thin. This is the system working as designed: the size of the dynasty is a reflection of the player's match performance, not an arbitrary cap.

The exceptional surfacing mechanic is the primary way individual Tier 3 members become visible. When a dormant member crosses a development threshold — a birth path produces something statistically significant, an event occurs that brings them to prominence within the branch, they reach an age where they are clearly more capable than some active Tier 2 members — the player receives a notification and a brief portrait of the member. The player then decides whether to surface them to Tier 2 status (if a slot is available) or acknowledge the exceptional quality and leave them dormant for now. This notification is the moment Tier 3 becomes personal. A dormant grandchild who surfaces as a potential Brilliant commander is a character the player suddenly cares about.

---

### Tier Transitions

Movement between tiers follows defined mechanics and is intentionally weighted toward significance.

When a Tier 3 member surfaces to Tier 2, the game marks it with a brief notification event — not a full succession-level interface, but enough to register as meaningful. The player sees who this person is, what their birth path has produced, what their developing traits suggest. They have emerged from the background into the visible family. They are now individually tracked.

When a Tier 2 member is elevated to Tier 1 commitment, the transition is the most significant tier movement in the system. The commitment interface presents the member in full detail — their attributes, their traits, their birth path specialization, the projected impact of their commitment to the specific role being filled. The player confirms the commitment. The game marks this confirmation. This person has joined the center of the story. They appear with full visual presence in the Keep's commitment hierarchy. Other family members in the Keep visually acknowledge the change — the family dynamic has shifted. Someone new is at the center.

The reverse transition — a Tier 1 member moving back to Tier 2 because they have been stripped of their role — is possible but costly. A committed member removed from their role does not smoothly reintegrate into Tier 2 as a neutral candidate. They carry the memory of their commitment: other family members and the world at large know they were committed and are now not. Some traits handle this gracefully. Proud members handle it poorly. The political and interpersonal dynamics of de-committing a member are as real as the mechanics.

The design principle governing all transitions is simple: the player should feel each one. Moving someone into commitment is a choice about the story of this match, and the story should register it.

---

## Section 3: Bloodline Member Development

Members of the bloodline are not born with fixed capabilities. They develop across match time through the combination of their birth path, their natural traits, and the specific history of what happens to them in the match.

Birth paths are chosen at birth and represent the direction in which a member is trained during childhood and adolescence. The available paths are Military Command, Governance, Religious Leadership, Diplomacy, Covert Operations, Economic Stewardship/Trade, Mysticism (focused on offensive/defensive faith abilities in war rather than faith expansion), and Scholarly. A birth path is a declaration of intent: this is what the dynasty intends for this child. It is not a guarantee. The path shapes the development ceiling and the rate of growth in specific attribute domains. A child on the Military Command path will, under normal conditions, grow into someone who performs better in martial commitment roles than a child on the Scholarly path. But the path is the starting trajectory, not the final destination.

The interaction between birth path and natural personality traits is where individual character emerges. A member born on the Military Command path with the Brilliant trait produces something rare and recognizable: a natural commander whose strategic processing outpaces the decisions events normally allow for. Option trees open for a Brilliant commander that are invisible to other members — choices that require the capacity to see three steps ahead in tactical situations. The same Military Command path member born with the Cautious trait produces something entirely different: a defensive specialist who survives wars that aggressive commanders lose, who accumulates experience across many campaigns precisely because they do not take the risks that kill more spectacular generals. These are not stat variations. They are character types. The player comes to know them as individuals.

The personality traits are organized across five categories. Members typically carry two to three traits. Trait combinations produce compound behavioral tendencies in events: a Cunning and Ambitious member in a diplomatic event behaves differently from a Cunning and Loyal one, even though both are Cunning.

**Martial Traits** govern how members think about and engage in conflict. Fierce members push for aggressive action in military events and are difficult to restrain once they have committed to a course. Their presence on campaign reduces unit hesitation and improves early-engagement outcomes. Cautious members pull toward defensive and information-gathering choices; their campaigns have lower catastrophic loss rates and lower spectacular victory rates — they are the reason a dynasty survives wars it should have lost. Brilliant commanders process tactical situations faster than events normally allow, producing option trees invisible to other members; they are rare, and the world knows when one is in the field. Reckless members choose high-risk options with approximately equal frequency of impossible victories and unrecoverable disasters; they are the ones other bloodline members tell stories about regardless of outcome. Stalwart members do not yield — not the best strategists, not the most aggressive, but the ones who hold when every other option is retreat.

**Political Traits** govern how members operate in the landscape of power and relationship. Cunning members understand that what people say they want and what they actually want are different, and they work both simultaneously; diplomatic events in their presence frequently have options only visible through this trait. Honorable members hold the word given at personal cost; other dynasties respond with a specific trust that takes years to build through any other trait and cannot be manufactured. Ambitious members push events toward choices that increase power and positioning, always calculating whether the current arrangement could be better for them and for the house. Loyal members will not betray the dynasty under any pressure; events offering personal gain at the house's expense are simply not considered by them.

**Social Traits** govern how members move through the human world around them. Charismatic members change the room — they improve relations with minor factions, common populations, and other dynasties through the ordinary operations of presence. Cold members do not warm to people and do not perform as though they do; they are respected, carefully kept at a distance, and efficient in ways that exclude warmth. Beloved members are the rarest social designation — they cannot be declared, only earned through specific combinations of sustained action and circumstance across match time, and their death produces political reverberations that other deaths do not. Feared members have made the world understand that crossing them has a predictable cost; they suppress external aggression and produce compliance rather than loyalty, and when they die, some of what was being suppressed resurfaces immediately.

**Faith Traits** govern how members relate to the sacred and to covenant practice. Devout members treat faith as a reality rather than a tool; they amplify all faith-related events and provide stability to faith-aligned populations. Apostate members have examined the house's faith and rejected it from considered position — not indifference, but conclusion; they create friction with covenant relationships and provide advantages where religious independence is useful. Seeker members are in active inquiry without settled conclusions; their openness builds unusual diplomatic relationships with faith communities the house is not formally aligned with, and they may settle over time or remain in productive uncertainty. Zealot members treat faith as the only lens through which the world is legible; they are maximum instruments for faith-based expansion and maximum liabilities for multi-faith diplomacy.

**Personal Traits** govern the individual character underneath the roles. Proud members cannot separate their sense of worth from the house's standing, performing brilliantly when the house is ascendant and making the most expensive mistakes when it is not. Humble members have a realistic sense of what they and the house are, providing the clearest analysis of events because ego does not filter their assessment. Vengeful members track grievances across decades of game time and activate those entries in future related events. Merciful members bend outcomes toward the least possible harm when they have power over them, producing expensive results in the short term and durable alliances in the long term. Melancholic members carry something heavier than the current situation explains; they see clearly and see the costs, their performance uneven between situations requiring endurance and those requiring forward enthusiasm. Wrathful members have anger close to the surface that occasionally escapes it; they win confrontations that Cautious members would have avoided and cause confrontations that Diplomatic members would have resolved.

Traits express in gameplay not merely as stat modifiers but as behavioral tendencies visible in event choices and in how the world responds to these people. A Feared member governing a territory produces different population behavior than a Beloved member governing the same territory. The traits make individuals legible as individuals. The player who has been watching them for five hours of match time knows them.

---

## Section 4: Succession Mechanics

Succession is the most important individual system moment in the game. It is the event where the dynasty's accumulated history — every marriage negotiated, every heir developed, every political relationship cultivated — produces its consequences all at once, in a live match, while enemies are watching.

When the head of bloodline dies, the game does not pause. The world continues. Armies fight. Trade routes move. Lesser houses calculate their loyalty. The succession event fires into a live match. This is intentional. The dynasty's vulnerability at the moment of transition is real, not abstracted.

Succession triggers through three conditions: primary death of the head of bloodline in battle or from illness or assassination; capture of the head with no active ransom agreement (the dynasty operates in a state of effective headlessness); or incapacitating illness that prevents the head from exercising their role. In all three cases, the succession interface opens, and the world continues outside it.

The succession interface is designed around a specific information architecture. The left panel shows the current state of the dynasty: active territories, alliances, military commitments, resource levels, faith state across the population. This is the inheritance — the player is about to hand all of this to someone new. The center panel shows the eligible successors, each with their portrait, current commitment role, age, and five projected impact readings. The right panel shows the live world map, moving in real time. The world is not waiting.

The five projected impact metrics are the succession interface's analytical core. Military Standing projects whether the chosen candidate will strengthen or weaken the dynasty's armies and command structure — a candidate with strong martial birth path and War General experience produces a high Military Standing projection; a candidate with Scholarly birth path and no martial history produces a low one, even if they are otherwise capable. Economic Output projects how the candidate's trade and economic affinity will affect realm income. Conviction Trajectory projects whether the candidate's personality tendency pushes toward Moral or Cruel over time, and the downstream effects on population loyalty and diplomatic standing that trajectory implies. Faith Alignment projects how the candidate's covenant devotion aligns with or conflicts with the existing faith infrastructure — a candidate whose personal faith alignment differs sharply from the dominant faith of the dynasty's most important territories is flagging a governance challenge. Population Loyalty projects how the population's pre-existing awareness of this candidate affects their initial acceptance as head. The Heir Designate, if one was formally named, scores highest on this metric. An unknown cadet branch member scores lowest.

These projections are presented as ranges, not certainties. The interface is honest that it is projecting. The variables that determine outcomes are visible and understandable. The player is not choosing from a black box.

The Revolt Risk Indicator shows which specific territories and lesser houses are at elevated risk if each candidate is chosen, with mouseover detail explaining the relationship: a house that was personally loyal to the dead head and does not yet know this candidate; a territory governed by a rival claimant; a lesser house whose daughter was passed over for a marriage that would have bound them more tightly to the bloodline. The specificity is intentional. The player can see exactly what they are risking with each choice and why.

There is no hard decision timer. But the live world map in the right panel shows enemy armies moving. If a rival bloodline is taking advantage of the transition window, the player sees it in real time. The pressure is visible and genuine. The temptation to rush a bad decision to end the succession event is real. The correct decision is to make the right choice, not the fast one — but the match continues while the player decides.

Succession eligibility flows through three tiers. Primary eligibility is direct children of the deceased head. A formally designated Heir Designate has the highest Political Legitimacy score but is not mechanically forced — the player chooses freely. Secondary eligibility are siblings of the deceased head, accessible if no living adult children exist. Extended eligibility covers nephews, nieces, and cousins, accessible through explicit prior designation via Shadow Governance or if both primary and secondary tiers are unavailable. Non-family succession is an emergency option only — a legendary War General without bloodline can be proposed when the family has been devastated, triggering a severe revolt from any surviving blood candidates and permanently locking certain bloodline-specific abilities.

The Regency scenario fires when the head dies with children who exist but are not yet adult by match terms. A regent chosen from the secondary or extended eligibility tier governs until the designated heir reaches adulthood. The Regency period has its own political dynamics: the regent may or may not be loyal to the young heir's future, and managing that relationship while the heir develops is its own strategic challenge.

Contested succession fires when the chosen candidate's Political Legitimacy score falls below the threshold required for smooth transition. A succession without revolt is earned. It means the player has managed the family, the political relationships, and the designation process well enough that the transition is accepted. When revolt fires, its severity is determined by the number of lesser houses loyal personally to the dead head rather than institutionally to the bloodline, the legitimacy gap between the chosen candidate and the most legitimate alternative, the chosen candidate's conviction alignment versus the political character of the revolting entities, and whether any rival bloodline is actively offering support to the revolting parties. Minor revolts take one or two lesser houses in a single territory. Major revolts can cut the dynasty's effective territorial control by a third during the most vulnerable post-succession window. Rivals who track dynasty state will attack during succession. This is not unfair. It is the system working.

Revolts that fire after succession can be addressed through war, but they can also be addressed through tribute offers, marriage proposals to the revolting house, faith mediation through a committed Faith Leader, or a credible show of force that makes the revolting party calculate the cost of continued resistance. Not all revolts require reconquest. Some require acknowledging a grievance and paying a material cost to resolve it. A player who thinks about succession in advance — who has managed the Heir Designate's designation period, who has cultivated political marriages with key lesser houses, who has deployed Governors with deep Roots in volatile territories — will navigate the succession event with damage contained. A player who has not thought about it faces a crisis.

The Transition Window immediately following a confirmed succession choice runs approximately five to ten match minutes. During it, all bonus effects from the previous head lapse, the new head's bonuses activate at reduced level and scale up as the window closes, diplomatic relationships enter an acknowledged-but-pending status, and any active revolt events fire. The window is the designed vulnerability. It is the cost of succession. Managing the window — having military forces available, having diplomatic relationships that can absorb the uncertainty, having a Transition-prepared heir — is the reward for planning.

---

## Section 5: Marriage, Cross-Dynasty Relations, and Hybrid Lineages

Marriage in Bloodlines is a strategic transaction with personal consequences that compound over time. It is never one without the other. The player who negotiates a marriage purely for its immediate diplomatic value and ignores the long-term personal and lineage consequences is making a decision that will produce complications at match hour six that seemed irrelevant at match hour two.

Arranging a marriage requires a committed Diplomatic Envoy from the offering dynasty. Marriage is a Marriage Agreement — a specific agreement type within the diplomatic system — and like all significant agreements, it requires formal diplomatic investment to open and execute. The terms of a marriage arrangement include what the offering family provides (dowry in resources, territory access, trade route commitment, military non-aggression period) and what they expect in return. The other party negotiates. A marriage is a contract. Breaking it is possible and has consequences proportional to the relationship's depth at the time of breaking.

Who can be offered in marriage is broad. Any bloodline member of appropriate age — late adolescence onward by match timeline standards — can be proposed. The other party may be a member of another player's bloodline (direct diplomatic negotiation), a member of a lesser house or vassal family (internal alliance-building), a member of an AI kingdom's ruling family (foreign diplomatic play), or a commoner of exceptional attribute (rare, lower political value, some stigma from higher houses). Each context produces different strategic value: a marriage to another player bloodline creates a personal bond between dynasties that diplomatic agreements without marriage cannot replicate; a marriage into a lesser house binds that house through familial anchor in ways that economic incentive alone cannot achieve.

Cross-bloodline marriages produce children who carry inheritance from both parents. Attribute inheritance draws from both. Faith inheritance can pull toward either parent's faith alignment — a source of genuine family tension, visible in the Keep as the child grows, and a meaningful design space for the player who is watching two faith alignments compete for a developing young person's loyalty. Political identity is contested: the bloodline that holds custody considers the child theirs; the bloodline they came from may dispute that claim. Cross-dynasty children are eligible for succession in both dynasties in principle, though political legitimacy scores reflect the complications of their identity.

New dynasty formation from cross-dynasty children is rare and consequential. A child of two bloodlines who survives to adulthood, accumulates sufficient political standing — territory held, armies led, faith role performed — and either the parent player chooses to split the bloodline or an event forces the issue, can found their own dynasty as a separate player faction. This is not a routine outcome. It is a late-game branching possibility that represents what happens when the competing claims of a hybrid lineage finally resolve by splitting rather than continuing to exist in tension.

Political marriages within lesser houses elevate that house's loyalty and bind it familially to the bloodline. The married bloodline member blends the Defense commitment role with a familial anchor — they are essentially a Governor whose presence also carries personal bond. If that member dies or is recalled, the lesser house's loyalty may fracture in ways that the simple departure of an administrative Governor would not produce, because what is lost is not just the function but the family connection.

Bloodline capture produces some of the most consequential diplomatic situations in the game. A captured bloodline member is a structural wound. If the captured member is the next rightful leader, the homeland suffers: confidence weakens, stability cracks, rival claimants gain ground, external powers sense vulnerability, internal factions imagine alternative futures. The player handling the capture has options: ransom (the standard negotiated return), execution (permanent removal from the rival's dynasty with the political cost of the act), enslavement (economically productive, politically damaging and internationally notable), or marriage into the capturing dynasty (converting the capture into a cross-dynasty relationship, with all the complexity that implies). The Captured Heir is a specific scenario — not just a captured member but the person the world was already treating as the next head, now in the hands of a rival — and its dynamics reflect that specificity.

War heroes represent a different kind of dynasty expansion. Exceptional commanders who earn titles and lands through military service create lesser houses under the main dynasty. These lesser houses begin loyal but autonomous. They have their own interests, their own internal politics, their own capacity for loyalty and for fracture. A lesser house governed by a bloodline member through marriage is more stable than one held purely by administrative title. A lesser house whose loyalty was built on a personal relationship with a now-dead head will recalculate that loyalty in the succession window. The lesser house web around a mature dynasty is one of the most complex management surfaces in the game — not because any individual lesser house is demanding, but because the aggregate of their relationships, their histories, and their current calculations creates a political landscape that rewards attention.

---

## Section 6: Generational Scale and Match Timeline

The match begins in Year 80 of Reclamation. The founding generation — the head of bloodline and their immediate family — is already established. The head is in their late thirties to mid-forties. Their best years as a direct combatant may be behind them. Their best years as a strategist, political operator, and parent are now. They have a spouse. They may have children ranging from young adolescent to early adult. One child may already be committed to a role. The dynasty is not new. It is entering its next phase.

A short match — two hours of real time — plays through roughly one generation. The founding head ages. The children develop, reach adulthood, begin their commitments. The head may die late in the match, triggering a succession among children who are now established enough to take the role. The founding generation produces the conditions for the next. A two-hour match ends with the sense that what the player built will continue.

A long match — ten hours or more — is a different experience. The founding head who was in their prime at match start is elderly at hour five. They are visibly aging in the Keep, their bearing changed, their movement slower, their presence carrying the gravity of decades of match history. Their conviction has deepened through every choice made under their leadership. Their faith intensity may have pushed into Fervent territory through years of accumulated spiritual weight. They are the dynasty's living memory of everything that has happened. They cannot last much longer.

The transition from founding generation to second generation is the first major generational event in a long match. It fires as a succession when the founding head finally dies — not as a crisis if the player has prepared for it, but as a narrative moment regardless. The children who were adolescent at match start are now in their thirties or forties, committed, experienced, carrying the marks of their roles. Some of them have their own children. The third generation is beginning to be born. The founding head's death is the end of the first chapter.

By match hour eight, if the match has run long, the founding generation is largely gone. Some may persist — an exceptionally long-lived member who was young at match start might still be in Tier 2 at hour eight, elderly, carrying the last direct memory of the dynasty's beginning. But the third generation is now active. The great-grandchildren of the founding couple may be born. The dynasty at match hour eight looks entirely different from the dynasty at match start: different people, different traits, different histories, different relationships with lesser houses that were themselves not yet formed when the match began.

The 40-60 living family members that a long match can produce are not a management problem. They are the system working. The three-tier model is specifically designed to make this scale manageable: the player directly manages eight to twelve committed members, interacts with up to twenty active members at a higher level, and is aware of an extended family managed at the branch level. The full family is present. The management surface remains defined. The depth that 40-60 living family members implies — the complexity, the competing claims, the multiple generations living simultaneously — is visible as texture, available as depth, but never overwhelming as management burden.

The Keep visualization tracks the generational progression across match time. Members age visibly. The founding head at hour eight does not look like the founding head at hour one — hair color has shifted, posture has changed, the face carries more of what it has lived through. The children who were adolescent in the Keep at match start are the adults at the center of the Keep at hour five. The grandchildren are the young ones growing in the background. The gallery of dead members on the Keep's longer walls grows. By hour eight, the player who has been watching it grow has a visual record of everyone who lived and died in this match. The gallery is the dynasty's history, made visible.

Natural death for founding generation members who were already middle-aged at match start is not a game penalty. It is an earned narrative moment. The founding head who was forty-two at match start and dies of illness at match hour six — having watched their children grow, having shaped the dynasty through its most critical early decisions, having been the center of the Keep for the first half of the match — that death is a landmark. The dynasty continues. The next generation takes over. The work they built is what the new generation inherits. The player feels this.

The dynasty at match end — in a successful long match — is the dynasty the player built. Not the dynasty that was handed to them at match start, with its predetermined family configuration and its founding head in their prime. The dynasty the player shaped through every succession, every marriage, every commitment, every child born and committed and lost and replaced. The founding generation is gone. Their grandchildren rule. The lesser houses that were tiny at match start are now established political entities with their own histories. The faith alignment of the dynasty has been shaped by a decade of match choices. The conviction posture is the accumulation of a thousand event decisions. This is what Bloodlines is designed to produce. The dynasty at match end is a character, built by play.

---

*PROPOSED / CB003 + SESSION 9 EXPANSION — AWAITING CANONICALIZATION*
*Added 2026-03-19*
*Source material: CREATIVE_BRANCH_003_BLOODLINE_FAMILY_SYSTEM.md, CREATIVE_BRANCH_003_HOUSE_LORE_AND_RIVALRIES.md (Section D: Personality Traits)*

---

## 2026-03-26 — Dynastic Feedback and Corrections (Session Ingestion)

Source: SESSION_2026-03-26_dynastic-feedback-and-corrections.md

---

### 2026-03-26 — CORRECTION NOTE: Marriage and Hybrid Heirs

**Original text (from earlier design sessions):**
> "Cross-dynasty marriages create new dynastic branches. Children may declare loyalty to either bloodline or form a new dynasty entirely."

**Correction (Lance Fisher, 2026-03-26):**
Children cannot freely declare loyalty or form a new dynasty. It is highly controlled and determined by the head of the household/player. It can be PROPOSED in-game with a set of advantages or disadvantages but is ultimately determined by the player. This correction was applied to the Design Content section (line 62) during the initial ingestion of this session's content. Recorded here as a formal correction note per additive-only archival rules.

---

### 2026-03-26 — CORRECTION NOTE: Mixed Bloodline Dynasties

**Original text (from earlier design sessions):**
> "If a bloodline repeatedly takes wives of another bloodline, children create complex inheritance and legitimacy dynamics. Children of mixed bloodline can branch off as own dynasty through blood and marriage. May announce loyalty to one bloodline or another. Creates emergent dynastic webs, rival claims, legitimacy disputes. Late game allows newly formed dynasties to emerge from royalty of older houses to make claims over current leaders."

**Correction and Expansion (Lance Fisher, 2026-03-26):**
There would be ample notification about the status of a member of the bloodline or children born seeking to branch off. Ultimately the head of the bloodline would make the decision. This correction was applied to the Player Manual Expansion section (line 93) and the Bloodline Loyalty Slider (lines 95-99) during the initial ingestion of this session's content. Recorded here as a formal correction note per additive-only archival rules.

---

### 2026-03-26 — Mixed Bloodline Dynasty Defection Slider

A slider tracks the intentions of children and members regarding defection from the dynasty:

- **Loyal to head of bloodline (vassal):** The member is committed to the dynasty and follows the head's direction without question.
- **Neutral (NPC behavior):** The member is not actively disloyal but is not firmly committed. They may drift based on events, treatment, and opportunities.
- **Seeking to join a different bloodline entirely:** The member is actively looking for an exit. If unchecked, they will attempt to defect.

Based on the decision of the head of the family, a member showing defection intent could result in forced combat between the member and the dynasty's forces. The head of the bloodline makes the final call on how to handle each case.

This works in reverse as well: members of other bloodlines may seek to join the player's dynasty. This requires a successful diplomatic action to initiate before the member can be accepted.

The slider provides ongoing awareness to the player about internal dynasty stability. Ample notification is given about any member's shifting loyalties before a crisis point is reached.

---

### 2026-03-26 — Training Paths Update (Mysticism Added)

Training paths for bloodline members born into the dynasty are now confirmed as seven paths:

1. **Military Command** — martial leadership, army command, battlefield tactics
2. **Governance** — territory administration, population management, law
3. **Religious Leadership** — faith expansion, covenant authority, spiritual influence
4. **Diplomacy** — negotiation, alliance building, cross-dynasty relations
5. **Covert Operations** — espionage, sabotage, intelligence gathering, assassination
6. **Economic Stewardship/Trade** — commerce, trade routes, resource management
7. **Mysticism** (NEW, 2026-03-26) — less focused on expanding and increasing faith, more focused on the offensive/defensive abilities of faith in war. This path trains members toward combat applications of faith power rather than institutional faith growth. Mystics are faith warriors, not faith administrators.

---

### 2026-03-26 — Roles Update

Confirmed possible roles for bloodline members:

- **Commanders** — military leadership on the battlefield
- **Governors** — territorial administration and stability
- **Diplomats** — foreign relations and negotiation
- **Ideological Leader (Priest, Priestess)** — faith authority and covenant representation
- **Merchant** — economic leadership and trade network management
- **Sorcerer** — offensive/defensive faith abilities in war, mystical operations

The Sorcerer role is the functional deployment of members trained on the Mysticism path. It represents the combat-faith axis as distinct from the institutional-faith axis represented by the Ideological Leader role.

---

### 2026-03-26 — Polygamy Rules Clarification (Faith-Specific)

Polygamy is permitted only under Blood Dominion and The Wild. This is confirmed and locked.

In other faiths (Old Light, The Order), members born into the bloodline can be withheld from marriage UNTIL a suitable enslavement or capture happens that presents a marriage opportunity.

Marriage does not happen naturally in Bloodlines. It is highly controlled and determined by the head of the household/player. There is no automatic matchmaking, no background marriage events. Every marriage is a deliberate player decision.

---

### 2026-03-26 — Marriage Control Clarification

Marriage in Bloodlines is never automatic. Key principles:

1. **Player-controlled:** The head of the household/player determines all marriages. No marriage happens without explicit player authorization.
2. **Strategic transaction:** Every marriage carries diplomatic, dynastic, and political weight. It is never a background event.
3. **Withholding is valid:** Members can be kept unmarried indefinitely. In non-polygamy faiths (Old Light, The Order), this is a core strategy for preserving bloodline purity until a suitable captured or enslaved member of another bloodline becomes available.
4. **No natural matchmaking:** The game does not automatically pair members. Marriage proposals come from the player or from other dynasties (player or AI), and are always subject to approval by the head of the household.

---

### 2026-03-26 — Starting Leader Options with Age Ranges

Starting leader options with confirmed specific age ranges:

| Option | Age Range | Notes |
|--------|-----------|-------|
| Father (born during The Frost) | 30-40 years old | The primary founding head option. Old enough to carry authority and history, young enough to rule for a meaningful portion of the match. |
| Eldest Son | 10-20 years old | A younger start. The player begins with a leader still growing into their role. Earlier succession is not a concern, but the leader lacks the experience and authority of the Father. |
| Second Eldest | Less than 10 years old | The youngest option. Requires a regent. High-risk, high-reward start with maximum development potential but immediate governance vulnerability. |
| Brother of the King (uncle) | 25-35 years old | Slightly younger than the Father. Offers a lateral start with different political dynamics and succession implications. |
| Brother of the King's first eldest | 12-16 years old | A cousin-generation start. Combines youth with a different branch of the family tree, creating unique succession dynamics. |

---

### 2026-04-14 — Defensive Fortification Doctrine Integration

Source: Lance W. Fisher direction, session of 2026-04-14. Full doctrine: `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`. Defender specification: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker specification: `04_SYSTEMS/SIEGE_SYSTEM.md`. This section integrates the doctrine into the Dynastic System.

The primary dynastic keep is the canonical bloodline seat. Dynasty and fortification are inseparable at the apex.

**The keep as bloodline seat.** A primary keep housing key bloodline figures is canonically the hardest target in the game when fully developed. The keep is the dynasty's physical center: where the head of bloodline resides, where succession chambers sit, where the family is depicted in the Keep/home UI (locked 2026-03-18), and where the Covenant speaker, governor, and inner-circle committed members may live during threat windows.

**Bloodline presence confers defensive leverage.** Members of the dynasty in the keep produce measurable defensive bonuses:

- Head of bloodline present — garrison cohesion, loyalty surge, reserve availability bonus, symbolic weight.
- Heir designate present — succession-intact resolve bonus; high legitimacy penalty if the heir is lost during defense.
- Commander present — defensive aura extending into garrison coordination, faster reserve commitment, stronger flanking.
- Governor present — supply-under-siege bonus, loyalty protection, repair tempo increase.
- Ideological leader present — faith intensity bonus, access to covenant defensive operations.
- Diplomat present — reduced coalition-forming difficulty for relief; faster enemy-unrest triggers.
- Spymaster present — counter-sabotage bonus, siege-preparation visibility.
- Merchant present — supply acquisition bonus under siege.
- Sorcerer present (mysticism path) — specialist faith or mystic defensive operations.

A keep that holds its bloodline fights differently from one emptied of bloodline. This is the bridge between the Dynastic system and the Fortification system.

**Succession under siege.** The Dynasty Consequence Cascade (implemented 2026-04-14) handles bloodline member loss during combat. Under the Fortification Doctrine, siege conditions produce specific additional cascade pathways:

- **Keep capture with intact succession line** — the captured head and heir together is a structural wound beyond capture of either alone. The doctrine formalizes this as a multi-member dynasty capture event with compounded legitimacy, conviction, and coalition consequences.
- **Heir in keep during fall** — captured heirs carry the Captured Heir Homeland Penalty (proposed canon, now strengthened into locked implication): a large negative modifier on the homeland's loyalty, military cohesion, and coalition-forming while the heir remains captive.
- **Succession during active siege** — if the head of bloodline falls during a siege defense, succession triggers immediately per the cascade but under a siege-specific regency: the promoted heir assumes command with a legitimacy shock window until the siege resolves.
- **Commander capture during sortie** — a bloodline commander leading a sortie who is captured during the sally triggers the captive ledger path. Ransom dynamics (future extension) will allow negotiated release.

**Keep-as-final-retreat.** When the outer works and inner ring are breached, the head of bloodline retreats to the final defensive core (keep proper). The core is designed for siege-within-a-siege. Its fall is the fall of the house. Dynasty members in the core at the moment of fall are subject to full cascade resolution: kill, capture, or escape (if a secret passage / sally port is present and intact).

**Dynastic commitment to defensive roles.** The Defense commitment role (established in canonical role categories) becomes central under the Fortification Doctrine. A member assigned to the Defense role at a fortified settlement anchors loyalty and stability with the doctrinal leverage forms. Multiple members may be committed across different fortified settlements to strengthen the realm-wide defensive network.

**Bloodline keep as apex covenant site.** The keep may house the highest-tier covenant structure the realm has built. Apex-tier faith buildings (Level 5 apex) inside the keep produce canonically powerful defensive effects per the Fortification System specification. This integrates the Dynasty, Faith, and Fortification systems at the apex of play.

**Fortification as generational project.** A primary keep developed to fortress-citadel tier is multi-generational work. A founding head establishes the outer works. An heir consolidates the inner ring. A grandchild ruler may complete the final core and faith-warded sanctums. This canonical pattern aligns with the multi-generation match scope (2-3 generations per 6-10 hour session).

**Captured bloodline and ransom economy (additive to canonical capture handling).** The canonical Captured Bloodline Handling already lists ransom, marriage, execution, and political pressure as canonical outcomes. Under the Fortification Doctrine, captive dynasty members held inside a captor's fortified keep generate canonical ransom income (influence trickle, implemented 2026-04-14). Held captives inside a keep with apex fortification generate at elevated rates and are proportionally harder to rescue by covert operations. Rescue operations against a fortified keep require siege-grade commitment.

**Defensive specialization within dynasty management.** The Defense commitment role can specialize: city defense (urban-fortified), border defense (frontier forts), keep defense (bloodline seat). Each specialization modifies the member's contribution. Apex-tier fortification may require a member with keep-defense specialization as a precondition for certain unlocks.

**Dynasty-siege interaction patterns.** A dynasty besieging another dynasty's keep generates dynasty-to-dynasty political consequences beyond the immediate military contest. Capture of a besieging commander by a sally-forth defender is canonical. Rescue of a captured bloodline member by a relief army is canonical. These multi-dynasty interactions feed the diplomatic and political event pipeline.

**Fortification doctrine is canonical.** Dynastic system implementations honoring the keep-as-bloodline-seat position must not reduce, dilute, or contradict the doctrine's ten pillars.
