# OPERATIONS SYSTEM — Covert, Faith, and Military Operations

**Status:** PROPOSED — Recovered from Session 10 mobile conversation (2026-03-21)
**Origin:** Utopia-inspired thievery/mystic/attack system adapted for Bloodlines RTS framework
**Date:** 2026-03-21

---

## Section 1: Operations System Architecture

**Three operation categories, one unified philosophy:**

Every operation in Bloodlines follows the same core loop: **Commit → Navigate → Execute → Consequence**. The player assigns an operative (or army), physically controls or directs them through the world, meets the operation's execution conditions, and then deals with the outcome — both the immediate effect and the systemic ripple (conviction, diplomacy, faith intensity, population reaction).

**The three categories:**

1. **Covert Operations** — Executed by Rogue-class units and Spymaster-committed bloodline members. Intelligence, sabotage, theft, assassination. The player physically navigates the operative through enemy territory.

2. **Faith Operations** — Executed by Mystic-class units (covenant-specific) and Priest/Priestess bloodline members. Offensive rituals, defensive wards, utility divination. Some require cosmic timing, some require gathered components, some require positional alignment.

3. **Military Operations** — Executed by armies. Distinct attack types beyond "fight the enemy army." Plunder, raze, siege, night assault, massacre. Chosen at the point of committing forces, with timing and method affecting both outcome and conviction.

**Key architectural principles:**
- Operations are **not abstract**. The player controls the unit on the map in real-time
- Every operation has **execution conditions** (position, timing, items, or combinations)
- Every operation has **failure states** with real consequences (capture, exposure, diplomatic incident)
- Operations interact with **conviction, faith, and diplomacy** — there is no consequence-free action
- The roster is designed for **expansion** — each category has a defined structure that new operations slot into cleanly
- Timing (time of day, moon phase, season, world events) is a **universal modifier** across all three categories, not exclusive to any one

**Design inspiration:** Utopia's thievery/mystic/attack operation system, translated from abstract percentage-roll mechanics into Bloodlines' real-time tactical execution model. The C&C engineer infiltration parallel — sneaking a fast unit through enemy defenses for a high-value objective — is the core gameplay feel. Operations are not click-and-wait. They are player-controlled tactical micro-plays on the live map.

---

## Section 2A: Covert Operations Roster (10 Operations)

Executed by **Rogue-class units** and **Spymaster-committed bloodline members**. Rogues are trained from the Barracks at Level 2+ with a Spymaster network established. Bloodline members committed as Spymaster can execute any covert operation personally with higher success potential but catastrophic risk if captured.

---

### Intelligence Operations (4)

| Operation | Execution | Timing Factor | Failure Consequence |
|-----------|-----------|---------------|---------------------|
| **Scout Defenses** — Reveal garrison strength, fortification layout, and trap placement in a target territory | Navigate rogue to territory edge, maintain undetected observation for 1 cycle | Fog and night improve concealment; clear daylight increases detection | Rogue detected and pursued; target alerted to surveillance |
| **Infiltrate Court** — Expose diplomatic agreements, active alliances, and pending betrayals of target dynasty | Rogue must reach the target's Settlement Hall or Dynasty Estate undetected | Festival or diplomatic event provides cover (distracted guards) | Rogue captured; diplomatic incident if Spymaster bloodline member |
| **Map Network** — Reveal trade route paths, caravan schedules, and resource flow of target dynasty | Rogue must shadow a trade caravan for 2 cycles without detection | Caravan must be active; rainy/muddy seasons leave tracks (easier to follow, easier to be tracked) | Rogue exposed; target reroutes trade (temporary economic disruption to both) |
| **Identify Bloodline** — Reveal locations, roles, and traits of target dynasty's active bloodline members | Rogue must reach the target's Keep interior zone | Only possible during periods when the target's Keep is not on high alert (no active war declaration against your dynasty) | Rogue killed or captured; target receives counter-intelligence on your Spymaster network |

### Sabotage Operations (3)

| Operation | Execution | Timing Factor | Failure Consequence |
|-----------|-----------|---------------|---------------------|
| **Arson** — Destroy a specific building in enemy territory | Navigate rogue to the target building, plant incendiary, evacuate before detonation | Night operations reduce detection chance; dry season increases fire spread (more damage, harder to contain) | Rogue captured; building damaged but not destroyed; conviction cost for civilian casualties if housing targeted |
| **Poison Supply** — Contaminate food stores or water supply in a target territory, reducing population health and morale | Rogue must reach Granary or Well undetected and remain for interaction duration | Harvest season (full granaries) maximizes impact; winter contamination compounds starvation pressure | Rogue captured; partial contamination detected and purged; population enraged (loyalty boost against you) |
| **Sabotage Infrastructure** — Disable a military or economic building for multiple cycles | Rogue must reach target building and complete a timed sabotage interaction | Buildings under construction are easier targets; night reduces guard presence | Rogue detected mid-sabotage; building partially damaged; alert status raised across territory |

### Direct Action Operations (3)

| Operation | Execution | Timing Factor | Failure Consequence |
|-----------|-----------|---------------|---------------------|
| **Assassinate** — Kill a specific bloodline member of a rival dynasty | Rogue (or Spymaster bloodline member) must reach the target's physical location on the map and execute a timed strike | Target is most vulnerable during travel between territories, during rituals, or during diplomatic visits outside their Keep | Failed attempt: rogue killed, target survives with injury, diplomatic crisis triggered, conviction cost, war justification granted to victim |
| **Kidnap** — Capture a bloodline member and extract them to your territory alive | Rogue team (minimum 2 rogues or 1 Spymaster) must reach target, subdue them, and navigate back to friendly territory undetected | Target vulnerability windows same as Assassinate; extraction is harder during high alert or daylight | Failed kidnap: rogues killed, target escapes injured, immediate war justification, heavy conviction cost |
| **Incite Revolt** — Trigger a loyalty crisis in a target territory by agitating the population | Rogue must operate within the territory for 3+ cycles, interacting with population centers | Territories with low loyalty are easier targets; territories recently taxed heavily or with troops conscripted are vulnerable | Rogue detected: expelled or captured; target population loyalty temporarily increases (rallying effect against foreign interference) |

---

## Section 2B: Faith Operations Roster (8-10 per Covenant)

Each covenant gets its own operation set split across three categories: **Offensive**, **Defensive/Utility**, and **Doctrine-Specific** (light and dark paths get different operations). Mystic-class units are covenant-specific — an Old Light Mystic cannot cast Blood Dominion operations.

**Universal execution principle:** Faith operations consume **faith intensity** as fuel. Using operations depletes the resource you spent infrastructure building up. Powerful operations require higher intensity thresholds to even attempt, and the intensity cost means reckless casting weakens your covenant's foundation. This creates the same tension Utopia had with rune management.

---

### OLD LIGHT (The Remembrance)

*Flavor: Fire, light, memory, ancestral power. Light doctrine preserves and illuminates. Dark doctrine purifies and burns.*

**Offensive (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Pillar of Dawn** — Column of burning light strikes a target area, damaging units and structures | Mystic must be positioned on elevated terrain with line of sight to target; requires Dawn item (gathered from Sacred Ground) | Can only be cast at actual dawn (first light cycle); overcast weather blocks it | Area damage to units and structures; morale shock to survivors; faith intensity cost: moderate |
| **Ancestral Judgment** — Calls down a vision of ancestral wrath on a target army, breaking morale | Mystic must possess a Relic of Remembrance (crafted at Hall of Remembrance) and be within 2 territories of target | More effective during solstice events; target army's morale vulnerability increases at night | Target army suffers severe morale penalty; routing units take additional casualties; faith intensity cost: high |
| **Searing Brand** — Marks a target territory, reducing faith intensity of any rival covenant operating there | Mystic must physically enter the target territory and perform a channeled ritual at the territory's faith building (or where one would be) | Ritual takes 2 cycles; interruption wastes the attempt and the intensity cost | Rival faith intensity in territory reduced by 15-25% for 5 cycles; their faith buildings produce at reduced rate; faith intensity cost: moderate |

**Defensive/Utility (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Beacon of Memory** — Reveals all unit movements within a large radius for a duration | Mystic must be positioned at a Wayshrine or higher Old Light building | Night casting extends duration; Sacred Ground positioning doubles radius | Complete vision of military and operative movements within radius for 3 cycles; faith intensity cost: low |
| **Veil of the Ancestors** — Conceals a friendly army from enemy vision for a limited duration | Mystic must travel with the army and maintain channeling (cannot perform other actions) | Fog or forest terrain extends duration; open plains reduce it | Army invisible to enemy scouts, watchtowers, and intelligence operations for duration; broken by attacking or entering enemy fortification zone; faith intensity cost: moderate |
| **Restoration of the Hearth** — Heals population health and morale in a territory | Mystic must be stationed at the territory's settlement and perform a public ritual | Most effective during festivals or after a military victory (population receptive); diminished returns if population loyalty is below 30% (they reject the faith) | Population health restored, morale boosted, loyalty nudged upward; faith intensity cost: low |

**Doctrine-Specific (Light — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Chronicle Ward** — Permanently increases a territory's resistance to covert operations and rival faith spread | Mystic performs an extended ritual (5 cycles) at The Eternal Cathedral or Hall of Remembrance | Uninterrupted ritual required; territory must be at Devout+ faith intensity | Territory gains permanent counter-intelligence bonus and faith erosion resistance; stacks with Spymaster counter-intelligence; faith intensity cost: high |
| **Light of Revelation** — Exposes all covert operatives (rogues, enemy mystics) within a territory | Mystic channels at any Old Light building for 1 cycle | Full moon amplifies range to adjacent territories | All hidden enemy units in territory revealed and marked for 3 cycles; faith intensity cost: moderate |

**Doctrine-Specific (Dark — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **The Inquisition** — Systematic suppression campaign that roots out rival faith and disloyal population | Mystic plus military escort occupy a territory and conduct a multi-cycle purge | Requires military presence to enforce; territories with mixed faith population yield more "results" but higher conviction cost | Rival faith intensity gutted; disloyal population removed (killed or exiled); major Cruel conviction shift; faith intensity cost: moderate |
| **Pyre of Purification** — Destroys a rival faith building with holy fire | Mystic must reach the rival faith building and perform a channeled destruction ritual (3 cycles) | Building must be undefended by military garrison; night approach reduces detection | Rival faith building completely destroyed; territory faith intensity for that covenant drops to zero; massive diplomatic consequence; conviction cost: severe Cruel shift; faith intensity cost: high |

---

### BLOOD DOMINION (The Covenant of Blood)

*Flavor: Blood, sacrifice, binding oaths, life-force as currency. Light doctrine channels willing sacrifice into power. Dark doctrine takes blood by force.*

**Offensive (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Blood Tide** — Curses a target army, causing wounds to resist healing and reinforcement to arrive weakened | Mystic must sacrifice a living captive (prisoner of war or population volunteer) at a Blood-Altar within range of target | New moon amplifies curse duration; sacrificing a bloodline captive dramatically increases potency | Target army's healing disabled for 3 cycles; reinforcements arrive at 70% strength; faith intensity cost: moderate; conviction cost: Cruel shift (scaled by sacrifice type) |
| **Sanguine Pact** — Binds two enemy units or squads together so damage to one transfers partially to the other | Mystic must have line of sight to both targets and possess a Binding Cord (crafted at Covenant Hall from blood offerings) | Eclipse events allow binding of three targets instead of two | 40% damage transfer between bound targets for 5 cycles; breaking the bind requires the targets to separate beyond visual range of each other; faith intensity cost: moderate |
| **Hemorrhage** — Drains resources from a target territory's economic output over time | Mystic must infiltrate target territory and perform a blood ritual at a hidden location (not at a building — must find concealed ground) | Wet season (rain) washes away ritual markings faster (shorter duration); dry ground sustains the curse longer | Target territory resource production reduced by 20% for 4 cycles; affected population experiences malaise and reduced work output; faith intensity cost: high |

**Defensive/Utility (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Bloodward** — Creates a perimeter around a territory that alerts when any hostile unit crosses it | Mystic must walk the territory's border and mark four cardinal points with blood offerings | Larger territories take longer to ward; wards persist until physically disrupted by an enemy unit finding and destroying a marker | Invisible tripwire; any hostile crossing triggers immediate alert with location; faith intensity cost: low |
| **Covenant Shield** — Temporary defensive aura that absorbs incoming damage for a friendly army | Mystic must sacrifice personal health (squad member lost) to fuel the shield | Shield strength scales with faith intensity tier; Blood Moon doubles absorption capacity | Army absorbs next X points of damage before squad losses begin; faith intensity cost: moderate; population cost: 1 squad member from mystic's own squad |
| **Blood Sight** — Reveals the health, morale, and faith intensity of all units and territories within range | Mystic must perform a self-bleeding ritual at a Blood-Altar (costs mystic squad health) | More effective during new moon and blood moon; rain reduces range | Complete tactical intelligence on target area for 2 cycles; reveals hidden weaknesses in army composition and territorial loyalty; faith intensity cost: low; health cost to mystic |

**Doctrine-Specific (Light — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Rite of Willing Sacrifice** — Volunteers from the population offer blood to empower a friendly army with supernatural vitality | Mystic performs ceremony at Covenant Hall with willing population participants; requires population loyalty above 60% | Harvest season amplifies the ritual's potency (abundance willingly shared); winter volunteers are fewer (population stressed) | Friendly army gains regeneration and morale immunity for 3 cycles; population health reduced temporarily but loyalty increases (they chose to give); faith intensity cost: moderate |
| **Oath of Blood Brotherhood** — Permanently bonds two allied armies from different dynasties, granting shared health regeneration | Mystic performs ceremony with both armies present at a Blood-Altar; both dynasties must consent | Requires formal alliance between the dynasties; ceremony takes 2 cycles; bond breaks if alliance breaks | Bonded armies share 15% of healing effects; if one is destroyed, the other suffers morale shock; faith intensity cost: high |

**Doctrine-Specific (Dark — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Exsanguination** — Drains the life force from a captive population to restore your own army and territory | Mystic performs ritual at Blood-Altar with captive population present (prisoners of war or enslaved territory population) | New moon maximizes extraction; number of captives scales the effect | Army fully healed; territory resource production boosted for 2 cycles; captive population killed; extreme Cruel conviction shift; diplomatic horror reaction from all dynasties; faith intensity cost: moderate |
| **Blood Curse** — Places a lasting curse on a rival dynasty's bloodline, causing their bloodline members to suffer health degradation | Mystic must possess a blood sample (obtained through combat, espionage, or captured bloodline member) and perform ritual at The Wound | Curse strengthens during blood moon; waning moon weakens it | Target dynasty's bloodline members suffer gradual health loss and trait degradation for 5 cycles; can be lifted by Old Light purification or The Order's adjudication; faith intensity cost: very high; conviction cost: severe Cruel shift |

---

### THE ORDER (The Iron Law)

*Flavor: Structure, law, judgment, enforcement. Light doctrine codifies justice and protects the innocent. Dark doctrine enforces absolute compliance through terror of law.*

**Offensive (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Edict of Condemnation** — Formally condemns a target dynasty, reducing their diplomatic standing with all neutral parties | Mystic must read the formal edict from a Hall of Mandate or The Iron Spire; edict must name specific violations (real or fabricated) | Waning moon amplifies (endings and judgments); must be cast during Day cycle (public pronouncement) | Target dynasty suffers -20% diplomatic reputation with all neutral dynasties for 4 cycles; trade agreement costs increase; faith intensity cost: moderate |
| **Iron Shackles** — Slows a target army's movement speed and reduces their ability to retreat or disengage | Mystic must be within line of sight and channel for the duration of the effect; requires Iron Chain item (forged at Hall of Mandate) | Winter amplifies (cold iron bites deeper); cavalry targets suffer the effect more severely than infantry | Target army movement speed reduced by 40% for 3 cycles; routing units cannot flee beyond a defined radius; faith intensity cost: moderate |
| **Mandate of Seizure** — Legitimizes the capture of a specific enemy territory in the eyes of the world, reducing conquest conviction costs | Mystic performs formal adjudication ritual at a Lawpost or higher Order building, presenting evidence of the target's violations | Must declare specific territory and specific justification; fabricated justifications risk exposure and backfire | If evidence is genuine: conquest conviction cost for that territory reduced to zero; if fabricated and exposed: double conviction cost and diplomatic penalty; faith intensity cost: low |

**Defensive/Utility (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Codex of Law** — Establishes formal legal protections in a territory that reduce population unrest and increase loyalty stability | Mystic must remain stationed at a territory's governance building for 3 cycles, codifying laws | Territory must not be under active military siege; population must be above starvation threshold | Territory loyalty floor raised permanently (cannot drop below 25%); population unrest events reduced by 50%; faith intensity cost: low |
| **Adjudicator's Eye** — Reveals the true conviction posture and hidden diplomatic agreements of a target dynasty | Mystic performs a formal inquiry ritual at The Iron Spire | More effective during waning moon; target dynasty's attempts to conceal conviction through diplomacy are pierced | Target dynasty's actual conviction band revealed to all players; hidden alliances and secret agreements exposed; faith intensity cost: moderate |
| **Garrison Discipline** — Permanently improves the defensive effectiveness of a territory's garrison through imposed military discipline | Mystic must spend 2 cycles at a military building within the territory, conducting inspections and imposing standards | Effect is permanent once established; garrison must not be below 50% strength when discipline is imposed | Garrison defensive value increased by +1 for all units in the territory; desertion rate reduced to zero; faith intensity cost: low |

**Doctrine-Specific (Light — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Shield of the Just** — Creates a protective legal aura around a friendly army that increases the conviction cost for enemies who attack it | Mystic travels with the army and maintains a channeled legal pronouncement declaring the army's purpose as defensive or retaliatory | Army must not be the aggressor (must be defending territory or retaliating against a declared war); shield breaks if the army initiates unprovoked aggression | Enemies attacking the shielded army suffer conviction costs as if attacking a neutral party; shielded army gains +1 Def; faith intensity cost: moderate |
| **Writ of Sanctuary** — Declares a territory as formally protected neutral ground; attacking it triggers automatic diplomatic penalties for the aggressor | Mystic performs extended ceremony (5 cycles) at The Iron Spire; territory must have Order faith intensity at Fervent+ | Writ must be renewed every 10 cycles or it expires; territory cannot be used as a military staging ground while under Sanctuary | Territory becomes functionally neutral; any dynasty attacking it suffers Trueborn-level diplomatic consequences; faith intensity cost: high |

**Doctrine-Specific (Dark — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Martial Law** — Imposes absolute control over a territory, eliminating all dissent but suppressing economic and population growth | Mystic plus military garrison impose formal martial law; requires military presence exceeding the territory's normal garrison requirement | Winter imposition is most effective (population already indoors and compliant); spring imposition generates more resistance | All loyalty events suppressed; population cannot revolt; but economic output reduced by 30% and population growth halted; Cruel conviction shift; faith intensity cost: moderate |
| **Trial by Ordeal** — Publicly executes accused criminals, traitors, or prisoners of war to demonstrate the absolute authority of law | Mystic conducts formal trial at Hall of Mandate with accused present; trial is public (all adjacent territory populations witness) | Day cycle only (public spectacle); festival timing increases audience and amplifies effect | Loyalty in home territory increases through fear; adjacent enemy territories suffer morale penalty; conviction cost: moderate Cruel shift (scaled by whether accused were genuinely guilty or politically targeted); faith intensity cost: low |

---

### THE WILD (The First Growth)

*Flavor: Nature, predation, cycles of life and death, the untamed world. Light doctrine harmonizes with nature's cycles. Dark doctrine unleashes nature's cruelty.*

**Offensive (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Call of the Pack** — Summons predatory animals from the surrounding terrain to attack a target army or territory | Mystic must be positioned in Ancient Forest, Badlands, or Tundra terrain; requires no crafted item (the land itself responds) | Full moon maximizes predator aggression; spring increases animal population (more respondents); winter wolves are the most dangerous variant | Pack of wild predators attacks target for 2 cycles; uncontrollable once summoned (may attack non-target units in the area); damage scales with terrain type; faith intensity cost: moderate |
| **Rot and Ruin** — Accelerates decay and corruption in a target territory's agricultural and wooden infrastructure | Mystic must plant a Blight Spore (gathered during autumn in forest terrain) at the target territory's food production site | Spring amplifies (growing season accelerates rot); dry summer slows spread; rain accelerates it | Food production reduced by 40% for 3 cycles; wooden structures take structural damage over time; affected population suffers disease events; faith intensity cost: moderate |
| **Feral Terror** — Overwhelms a target territory's population with primal fear, causing panic and flight | Mystic must perform a howling ritual at the territory's edge during night; requires elevated position (hill, ridge, watchtower) | New moon maximizes terror (darkness amplifies primal fear); forest terrain between mystic and target amplifies sound propagation | Population panics; civilian movement becomes chaotic; garrison morale reduced; if combined with military assault, defenders fight at -2 morale; faith intensity cost: high |

**Defensive/Utility (3):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Verdant Regrowth** — Accelerates natural resource regeneration in a friendly territory, restoring depleted farmland and forests | Mystic must perform planting ritual at the territory's agricultural or forest zone; requires seeds gathered from The First Grove or Spirit Lodge | Spring casting doubles the regeneration rate; casting on Sacred Ground produces permanent fertility bonus | Depleted food and wood production restored to full within 2 cycles; terrain damage from razing or fire healed; faith intensity cost: low |
| **Nature's Veil** — Dense growth conceals military movements through forest and wilderness terrain | Mystic must channel while positioned in forest terrain; effect extends along connected forest zones | Autumn leaf fall reduces concealment; spring and summer foliage maximizes it; does not function in open terrain | Armies moving through affected forest zones are invisible to non-adjacent enemies; movement speed reduced by 10% (dense growth); faith intensity cost: moderate |
| **Beast Communion** — Establishes communication with animal populations in a region, providing passive intelligence on all movement | Mystic performs communion ritual at a Grove Marker or Spirit Lodge | Animal intelligence is terrain-dependent: forest gives best coverage; open plains give minimal coverage; water zones provide no animal intelligence | Passive detection of all unit movement through terrain with animal populations; persists for 5 cycles; does not reveal unit composition, only presence and direction; faith intensity cost: low |

**Doctrine-Specific (Light — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Harmony of Seasons** — Synchronizes a territory's agricultural cycle with natural rhythms, permanently improving food output | Mystic performs a season-long ritual (must begin at the start of spring and complete at the end of autumn) | Interrupted ritual fails completely; requires the mystic to remain in the territory for the entire duration (3 full seasons) | Territory food production permanently increased by 25%; population health improved; natural disaster frequency reduced; faith intensity cost: high (spread across the ritual duration) |
| **Guardian Spirit** — Binds a territorial animal guardian to a specific territory that patrols and defends against covert operatives | Mystic performs bonding ritual at The First Grove with a captured apex predator (bear, wolf pack alpha, great stag) | Animal must be captured alive (requires specialized Wild military units); ritual takes 2 cycles | Territory gains a permanent patrol unit that detects and attacks covert operatives automatically; guardian cannot leave the territory; if killed, requires full re-bonding; faith intensity cost: moderate |

**Doctrine-Specific (Dark — 2):**

| Operation | Execution | Timing/Condition | Effect |
|-----------|-----------|-----------------|--------|
| **Blight Plague** — Unleashes a devastating agricultural plague across multiple territories, destroying food production over a wide area | Mystic must cultivate a Blight Bloom (requires 3 Blight Spores planted at The First Grove during autumn, matured through winter) and release it at a crossroads or trade route junction | Can only be released during spring (growing season carries the plague); wind direction determines spread pattern | Food production destroyed across 2-4 territories for 3 cycles; population starvation events cascade; massive diplomatic consequence; conviction cost: severe Cruel shift; faith intensity cost: very high |
| **Apex Predator** — Transforms a Wild faith military unit into a monstrous feral version with dramatically increased combat stats but no player control | Mystic performs dark ritual at a Spirit Lodge during blood moon or new moon; target unit must be a bear rider or similar Wild cavalry | Blood moon produces the most powerful transformation; unit cannot be recalled or controlled once transformed | Transformed unit gains +4 Off / +2 Def and attacks everything in its path — friendly and enemy alike — for 3 cycles before dying of exhaustion; faith intensity cost: very high; conviction cost: Cruel shift (unleashing uncontrollable destruction) |

---

## Section 2C: Military Operations Roster (6 Attack Types)

In Utopia, when you sent an army you chose *what kind* of attack. Conquer was just one option. Bloodlines translates this into the RTS framework: when committing an army to assault a territory, the player selects an **operation doctrine** that governs how the army behaves during and after the engagement. This affects targeting priority, resource yield, conviction consequence, and post-battle territory state.

**The army fights regardless — the operation doctrine determines what they do with their victory.**

| Operation | Behavior | Timing Modifier | Conviction Impact | Strategic Use |
|-----------|----------|----------------|-------------------|---------------|
| **Conquer** — Seize and hold the territory | Army engages military targets, secures fortifications, occupies governance structures. Population is subdued but preserved. Territory transitions to your control with existing infrastructure intact but loyalty starts low. | Dawn assault grants morale bonus to attackers (symbolic new beginning); night assault reduces defender detection but no morale bonus | Neutral — standard act of war; no conviction shift unless civilian casualties are excessive | Default military expansion. You want the territory and its productive capacity. |
| **Plunder** — Strip the territory of portable resources and withdraw | Army targets economic buildings (markets, granaries, treasuries, lumber camps). Resource stores are seized. Army withdraws after extraction — no territory claim made. | Harvest season maximizes food/gold yield (stores are full); raiding during winter yields less but causes more population suffering | Minor Cruel shift — you took their livelihood; worse if population starves afterward as a consequence | Economic warfare. Weaken an enemy's economy without committing to holding the territory. Hit-and-run resource denial. |
| **Pillage** — Destroy economic and civic infrastructure | Army systematically destroys buildings. Farms burned, markets demolished, wells poisoned. Population displaced. Territory left economically devastated but not claimed. | Dry season amplifies fire damage to wooden structures; stone buildings require dedicated siege to destroy regardless of season | Moderate Cruel shift — you destroyed civilian infrastructure; scales with civilian casualties | Punitive warfare. Make a territory worthless to the enemy even if you can't hold it. Deny economic base. |
| **Raze** — Total destruction of all structures and fortifications | Army destroys everything — military, economic, civic, and faith buildings. Territory reduced to bare land. Population scattered. Territory becomes unclaimed wasteland. | Night razing reduces intervention from adjacent territory garrisons (slower response); scorched earth in summer creates lasting terrain damage | Severe Cruel shift — near-universal condemnation; Trueborn City diplomatic penalty; population horror reaction across the map | Scorched earth. Used when you cannot hold territory but refuse to let the enemy benefit from it. Extreme denial strategy. |
| **Siege** — Prolonged investment designed to force surrender without full assault | Army encircles fortified position and cuts supply lines. No direct assault unless the player orders escalation. Garrison starves over time. Population morale erodes. Surrender yields intact territory with higher starting loyalty than Conquer (they yielded rather than were beaten). | Winter siege is devastating (cold + starvation compound); spring/summer sieges take longer but population has forage options | Moral if you accept surrender terms fairly; Cruel if you starve civilians deliberately or refuse surrender | Patient territorial acquisition. Preserves your army strength. Higher loyalty outcome but costs time — vulnerable to relief forces. |
| **Massacre** — Systematic elimination of enemy military AND population | Army kills all military personnel and then turns on the civilian population. Surviving population flees to adjacent territories. Territory claimed but nearly empty — must be repopulated from your own pool. | Night massacre reduces escape rate (fewer survivors flee); festival timing is especially heinous (conviction cost multiplied) | Extreme Cruel shift — the single most condemnable military action; automatic Trueborn coalition trigger if city is aware; all neutral dynasties shift hostile; your own population loyalty drops across ALL territories | Terror weapon. Sends an unmistakable message. Functionally eliminates a territory's identity. Almost never strategically optimal — the conviction and diplomatic cost is designed to be ruinous. But it exists because the choice must exist. |

**Key design notes:**
- Operation doctrine is chosen **before** the army engages. It can be changed mid-operation only by withdrawing and re-committing (costs time and exposes the army during transition)
- Defenders don't know which operation the attacker has chosen until the behavior becomes apparent (plunder targets economics first, raze targets everything, etc.)
- A Moral conviction dynasty physically cannot order Massacre — the army refuses. This is the only hard-lock in the system. All other operations are available to all conviction bands with scaling consequences
- Siege is the only operation that doesn't require winning a direct battle — it's a war of attrition and patience

---

## Section 3: The Timing System

Time is a universal strategic layer in Bloodlines. Every operation — covert, faith, and military — is affected by when it happens.

### 3.1 Time Cycles

Bloodlines runs on a **day/night cycle** with sub-divisions:

| Period | Duration (real-time) | Characteristics |
|--------|---------------------|-----------------|
| **Dawn** | ~2 minutes | Transition light; fog burns off; guard shifts change. Morale bonus for dawn assaults. Mystic operations requiring "first light" activate here. |
| **Day** | ~8 minutes | Full visibility. Maximum economic output. Guard alert at peak. Worst time for covert operations. Best time for open military engagement. |
| **Dusk** | ~2 minutes | Transition to darkness; visibility drops. Trade caravans seek shelter. Last window for daylight-dependent faith operations. |
| **Night** | ~8 minutes | Reduced visibility. Covert operation success rates increase. Population rests (reduced economic output). Night assault penalties for attackers (unfamiliar terrain) but detection penalties for defenders (reduced awareness). Dark faith operations are amplified. |

One full day/night cycle = ~20 minutes real-time. A 6-hour match covers roughly **18 in-game days**.

### 3.2 Lunar Cycle

The moon cycles through four phases across the match, each lasting approximately **4-5 in-game days**:

| Phase | Effect on Operations |
|-------|---------------------|
| **New Moon** | Maximum darkness at night. Covert operations at peak effectiveness. Blood Dominion curse operations amplified. Night military operations benefit most. |
| **Waxing Moon** | Growing light. Blood Dominion vitality operations strengthened (growing power). The Wild animal activity increases (hunting season). |
| **Full Moon** | Maximum night visibility. Old Light operations amplified (lunar fire). Covert operations harder (bright nights). The Wild predator aggression peaks (Call of the Pack most effective). |
| **Waning Moon** | Fading light. The Order operations strengthened (judgment and endings). Defensive faith operations at peak. |

### 3.3 Seasonal Cycle

Four seasons rotate across the match. Each season lasts approximately **4-5 in-game days** (one season roughly aligns with one lunar cycle):

| Season | Economic Effect | Military Effect | Faith/Covert Effect |
|--------|----------------|-----------------|---------------------|
| **Spring** | Planting season — food production ramps up; timber growth accelerates; flood risk in River Valleys | Mud slows cavalry and siege equipment movement; rivers swell (naval advantage, ford crossings blocked) | The Wild operations amplified; Rot and Ruin spreads fastest; nature-based mystic operations at peak |
| **Summer** | Peak food production; water consumption increases; drought risk in Badlands | Full mobility; dry ground favors cavalry; fire operations (Arson, Raze) spread fastest | Old Light Pillar of Dawn at maximum potency; heat exhaustion affects prolonged Siege operations |
| **Autumn** | Harvest — granaries fill (Plunder yields maximum); Blight Spore gathering for The Wild; timber harvest peak | Cooling weather favors infantry operations; harvest caravans create Plunder targets and trade route vulnerability | Blood Dominion sacrifice rituals most potent (harvest symbolism); The Wild Blight Spore gathering window |
| **Winter** | Food consumption increases; production drops; water sources may freeze in Tundra/Frost Ruins; population health declines without adequate housing | Siege devastating (cold + starvation); movement slowed in northern terrain; supply lines fragile | The Order operations strengthened (harsh judgment); covert operations in snow leave tracks (reduced effectiveness in Tundra/Highlands) |

### 3.4 Celestial Events (Rare)

These occur 2-3 times per match at semi-random intervals. They are **visible to all players** in advance (the sky shows signs). Each creates a window of 1-2 in-game days.

| Event | Frequency | Effect |
|-------|-----------|--------|
| **Solar Eclipse** | Once per match | All faith operations cost 50% less intensity. Apex-tier faith operations become possible outside normal thresholds. Blood Dominion dark doctrine operations at maximum potency. Old Light operations weakened (sun obscured). |
| **Blood Moon** | Once per match | Blood Dominion faith intensity regenerates at 3x rate. The Wild predator animals become frenzied (uncontrollable but devastating). Covert assassination operations gain potency bonus. Population fear increases across the map. |
| **Solstice (Summer/Winter)** | Twice per match | Summer Solstice: Old Light apex operations available; longest day cycle (extended economic output, compressed night). Winter Solstice: longest night cycle; The Order and covert operations amplified; siege attrition at maximum. |
| **Comet Sighting** | 0-1 per match (random) | All dynasties receive a conviction event: interpret the omen. Choice shifts conviction and faith intensity. Mystic operations of any faith cast during the comet's visibility have double duration. |

### 3.5 Timing and Dishonor — The Cruel Timing Advantage

Cruel conviction dynasties gain access to **dishonorable timing combinations** — sequences of operations that are more effective together but carry compounding conviction costs:

| Combination | Execution | Total Conviction Cost | Strategic Value |
|-------------|-----------|----------------------|-----------------|
| **Night Burning + Dawn Assault** | Arson (rogue) targets housing at night → army attacks at dawn while population is panicked and displaced | Arson Cruel shift + amplified cost for targeting civilians at night + attack during crisis = heavy compound Cruel | Defenders fight at reduced morale, civilian infrastructure already burning, garrison response disrupted by civilian panic |
| **Poison Supply + Winter Siege** | Poison Supply (rogue) contaminates food → army initiates siege during winter | Poison Cruel shift + Siege starvation Cruel shift (if surrender refused) = severe compound Cruel | Siege duration halved; population and garrison starve at accelerated rate; surrender comes faster |
| **Massacre + Feral Terror** | Feral Terror (Wild mystic) panics population → Massacre doctrine army enters while civilians are fleeing | Feral Terror Cruel shift + Massacre extreme Cruel shift = catastrophic compound Cruel | Fewer survivors escape (panicked population runs into the army instead of away); territory emptied more completely |
| **Incite Revolt + Conquer** | Incite Revolt (rogue, 3+ cycles) destabilizes territory → army arrives to "liberate" during the revolt | Incite Revolt: minimal conviction cost (covert) + Conquer: neutral = **low total cost** | This is the *smart* cruel play — destabilize from within, then arrive as a liberator. If the revolt isn't traced back to you, conviction cost is near zero and starting loyalty is higher |

**Design principle:** The cruelest actions are the most obvious. The most sophisticated plays minimize conviction cost while achieving similar outcomes. A dynasty that burns housing and massacres populations will win battles but lose the political war. A dynasty that destabilizes from within and arrives as a liberator wins both.

---

## Section 4: New Unit Types

The operations system introduces two new unit categories to the game:

### Rogue Unit
- **Availability:** Level 2+, requires Spymaster network (bloodline member committed as Spymaster)
- **Stats:** Off 3 / Def 2 — not a combat unit; fragile if detected
- **Squad size:** 3 (not 5) — smaller profile for infiltration
- **Training building:** Barracks with Spymaster network upgrade
- **Movement:** Fastest non-cavalry unit in the game; can traverse terrain obstacles other infantry cannot
- **Stealth:** Invisible to enemies unless detected by watchtowers, patrols, or counter-intelligence operations
- **Population cost:** Standard infantry population draw

### Mystic Unit (Covenant-Specific)
- **Availability:** Level 3+ (faith units tier), requires Devout faith intensity and appropriate covenant building
- **Stats:** Off 2 / Def 3 — fragile but slightly more resilient than rogues due to faith protection
- **Squad size:** 1 (solo operative) — mystics operate alone
- **Training building:** Covenant-specific faith building (Wayshrine/Blood-Altar/Lawpost/Grove Marker or higher)
- **Movement:** Standard infantry speed; some terrain bonuses based on covenant (Wild mystics move faster in forest, Order mystics move faster on roads)
- **Faith intensity:** Mystic units have a personal faith intensity pool that depletes as operations are cast; recharges near faith buildings or through meditation (stationary channeling)
- **Population cost:** Higher than standard infantry — mystics represent rare individuals with covenant training

---

## Section 5: Counter-Play and Detection

### Counter-Covert Operations
- **Watchtowers** detect rogues within their vision radius during day; reduced effectiveness at night
- **Spymaster counter-intelligence** (defensive Spymaster commitment) passively increases detection rate across all owned territories
- **Old Light's Light of Revelation** and **The Order's Adjudicator's Eye** are direct counter-covert faith operations
- **The Wild's Guardian Spirit** provides automated territorial counter-intelligence
- **Patrol routes** — garrison units assigned to patrol paths have increased rogue detection chance

### Counter-Faith Operations
- **Faith intensity defense** — higher friendly faith intensity in a territory provides passive resistance to enemy faith operations
- **Rival mystic presence** — a mystic stationed in a territory can attempt to interrupt enemy mystic channeling if detected
- **Building destruction** — destroying an enemy's faith buildings eliminates the infrastructure their mystics need to recharge
- **Old Light Chronicle Ward** provides permanent faith erosion resistance
- **The Order's Writ of Sanctuary** makes faith operations in protected territories diplomatically dangerous

### Counter-Military Operations
- **Fortifications** reduce the effectiveness of Plunder and Pillage (attackers must breach defenses first)
- **Garrison strength** determines whether Raze and Massacre can be fully executed or only partially
- **Relief forces** — allied armies arriving during a Siege break the encirclement
- **Population evacuation** — territories with advance warning (intelligence operations) can evacuate population before Massacre, reducing the attacker's conviction reward
- **Diplomatic alliances** — attacking an allied dynasty's territory triggers alliance response regardless of operation type

---

## Section 6: Faith Switching

**Status: PROPOSED — noted during Session 10 mobile conversation**

Players CAN switch their dynasty's covenant faith after the initial selection at the end of Stage 1. This is permitted but comes at enormous cost:

- **Faith intensity reset:** All accumulated faith intensity drops to zero. Every faith building must be re-consecrated to the new covenant (multi-cycle process per building)
- **Population disruption:** Population that followed the previous covenant experiences a loyalty crisis. Percentage of population that rejects the new faith and either revolts or emigrates
- **Bloodline member crisis:** Bloodline members committed to faith roles (Priest/Priestess) must convert or be removed from their roles. Conversion takes multiple cycles and may fail
- **Diplomatic consequence:** Dynasties and populations that shared your previous faith view the switch as apostasy. Former faith allies may break alliances
- **Military roster loss:** All faith-specific military units (Level 3+) become unavailable until the new covenant's faith intensity reaches the required thresholds
- **Conviction impact:** Faith switching generates a significant conviction event (direction depends on the reasons and manner of the switch)

**Design intent:** Faith switching must be possible because the choice must exist. But it must be so costly that it is genuinely a last resort — a decision made in desperation or as part of a radical strategic pivot, not a casual optimization. The player who switches faiths is starting their faith progression over from scratch in the mid-to-late game while dealing with the fallout from abandoning their previous commitment.

---

## Roster Summary

| Category | Count | Subcategories |
|----------|-------|---------------|
| Covert Operations | 10 | Intelligence (4), Sabotage (3), Direct Action (3) |
| Old Light Faith Operations | 10 | Offensive (3), Defensive (3), Light Doctrine (2), Dark Doctrine (2) |
| Blood Dominion Faith Operations | 10 | Offensive (3), Defensive (3), Light Doctrine (2), Dark Doctrine (2) |
| The Order Faith Operations | 10 | Offensive (3), Defensive (3), Light Doctrine (2), Dark Doctrine (2) |
| The Wild Faith Operations | 10 | Offensive (3), Defensive (3), Light Doctrine (2), Dark Doctrine (2) |
| Military Operations | 6 | Conquer, Plunder, Pillage, Raze, Siege, Massacre |

**Total:** 56 distinct named operations across all categories.

---

*This document was recovered from Session 10 mobile conversation transcript (2026-03-21). Old Light and partial Blood Dominion content is verbatim from that session. Blood Dominion completion, The Order, and The Wild faith operations were designed in Session 10 continuation to match the established patterns and covenant identities.*
