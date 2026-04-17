# CREATIVE BRANCH 002 — BLOODLINE MEMBERS: FULL SYSTEM DESIGN
**Status:** PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON
**Date:** Design Content — 2026-03-18, Creative Branch 002
**Scope:** Specialization paths, Frost Elder, lifecycle mechanics, Keep visualization

---

## OVERVIEW

Bloodline Members are the hero-equivalent anchors of each dynasty. In the Warcraft 3-aligned model, they are the micro-management layer — individual figures with names, histories, and irreplaceable skill sets. Losing a Bloodline Member is never just a stat loss. It is the loss of a person the player has watched grow, a function they depended on, and a story thread that ends permanently.

Each dynasty may have up to 20 active members. Members beyond that number exist in dormancy — alive, named, tracked in lineage, but not available for deployment. Every active member occupies one of the specialization paths described below. Together, the active roster represents what the dynasty can do in the world.

---

## SECTION A: SPECIALIZATION PATHS

PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON

Paths are chosen at birth and trained from childhood. The path is not a class assigned at a menu — it is a life direction set in motion early and visible in how the child grows. A Warlord child trains with weapons. A Scholar child is sent to tutors. The player assigns the path, but it is framed as a decision made for the child's future, not a game mechanic toggle.

---

### PATH 1: WARLORD

**Core role:** The dynasty's primary military instrument. Warlords lead armies from the front, amplify the combat effectiveness of every unit under their command, and are the centerpiece of offensive strategy. Without a Warlord, campaigns are possible but costly. With a strong Warlord, armies punch above their numbers.

**Primary ability (active): Battle Cry**
The Warlord issues a commanding shout that grants all friendly units within a medium radius +15% attack speed and +10% movement speed for 12 seconds. At higher levels, the radius expands and secondary effects layer in (brief fear immunity, morale restoration). This is the ability that turns an even fight into a rout. Cooldown: 45 seconds.

**Secondary ability (passive): Command Presence**
All units within the Warlord's tactical radius receive a flat attack bonus and a small armor bonus. The radius scales with level. The bonus applies continuously — the Warlord does not need to act, only to be present. This means a Warlord positioned at the center of a formation fundamentally changes what that formation can do.

**Conviction interaction:**
- Moral Warlord: Battle Cry also heals nearby friendly units for a small amount. Command Presence includes a morale bonus that reduces desertion risk during prolonged sieges.
- Neutral Warlord: No modifier changes. Standard path performance.
- Cruel Warlord: Battle Cry applies a brief terror effect to enemy units at the edge of the radius — low-morale enemy units may rout rather than fight. Command Presence trades the armor bonus for an additional damage modifier. The dynasty's population conviction may drift toward Cruel if the Warlord is active frequently and the player pursues aggressive tactics.

**Faith interaction:**
- Latent to Active: No mechanical change. The Warlord functions on martial instinct, not faith doctrine.
- Devout and above: If the Warlord's faith aligns with the dynasty's primary faith, Battle Cry gains a faith component — a visible prayer effect, and a 5% bonus to the ability's duration. This does not stack with conviction effects, it replaces the neutral version.
- Fervent/Apex: The Warlord becomes a declared holy warrior. Battle Cry is renamed and re-skinned to reflect the faith's doctrine. In light doctrine faiths, this produces a cleansing/sanctified version. In dark doctrine faiths, a consuming/blood-pact version. Ability behavior is identical; visual and narrative framing changes.
- A Fervent Warlord on the Priest path is not possible — Warlord and Priest are separate specializations — but a Warlord can hold personal faith conviction that modifies their behavior without being a Priest.

**Level progression:**
- Level 1: Base stats. Battle Cry radius: small. Command Presence radius: adjacent units only.
- Level 2: Battle Cry cooldown reduced to 38 seconds. Command Presence radius expands to a medium zone.
- Level 3: Battle Cry duration extended to 16 seconds. Warlord gains personal combat stat increase (the member themselves fights better when deployed on the battlefield as a unit).
- Level 4 (irreversible divergence): The player chooses one of two divergence paths:
  - Path A — Conquering General: Battle Cry also applies a slow to enemy units at the edge of the radius. Command Presence now applies a siege bonus when the Warlord leads an assault on fortifications.
  - Path B — Defender of the Blood: Battle Cry gains a secondary pulse 5 seconds after the initial shout, re-triggering the speed bonus. Command Presence radius expands to its maximum size and also provides a damage reduction modifier to friendly units.
- Level 5: The capstone ability unlocks — Warlords choose either Decisive Engagement (a once-per-match ability that forces all units in a large radius to fight at full morale regardless of conditions) or Last Stand (a once-per-match ability that triggers automatically if the Warlord would die in combat, granting them a 30-second window of heightened stats before the death is resolved).

**Death consequence:**
- The dynasty loses Battle Cry and Command Presence immediately. Any campaign in progress suffers a flat army morale penalty.
- If the Warlord was Level 3 or higher, a dynasty-wide conviction event fires — the loss is significant enough to affect how the population perceives the dynasty's military future.
- All units the Warlord personally trained (a tracking tag on certain units) lose their bonus stat and may require retraining under a new Warlord.
- If the Warlord had a named rival in another dynasty, that rival's dynasty receives a morale bonus.

**Rare variant: The Unbroken Warlord**
Appears approximately once every 20 matches (procedurally seeded, not guaranteed). This Warlord was born during a siege, survived a childhood battle, and has never been defeated. Their Command Presence aura is twice the normal radius at Level 1, a size no normal Warlord reaches until Level 4. Their Battle Cry does not have a cooldown — it has a charge system, recovering one charge every 60 seconds, holding a maximum of 3 charges. Enemy units that witness the Unbroken Warlord fight have a chance to flee rather than engage. The Unbroken Warlord cannot use the Last Stand capstone — they do not yield. If they would die, they die. No delay. The narrative weight of their death is proportionally larger.

---

### PATH 2: SCHOLAR

**Core role:** The dynasty's intelligence and knowledge layer. Scholars do not fight. They think. They research advantages, decode enemy capabilities, advise on diplomatic possibilities, and expand the dynasty's understanding of the world. A dynasty without a Scholar is flying blind — relying on raw power where precision would serve better.

**Primary ability (active): Intelligence Brief**
The Scholar produces a tactical assessment of a selected enemy dynasty. For 60 seconds, the player can see the enemy dynasty's current active Bloodline Member count, the names and levels of their top three members, and any active faith/conviction status effects on their population. This is intelligence that would otherwise require a covert network or be completely hidden. Cooldown: 4 minutes. At higher levels, the brief includes building status, resource stock estimates, and current diplomatic obligations.

**Secondary ability (passive): Research Momentum**
All research/technology unlocks in the dynasty's development tree complete 12% faster while the Scholar is active. This is a continuous passive that represents the Scholar directing the dynasty's intellectual resources. Multiple Scholars do not stack this bonus — the cap is 20% per match regardless of Scholar count.

**Conviction interaction:**
- Moral Scholar: Intelligence Brief also includes a flag if the target dynasty has recently performed actions classified as atrocities (mass executions, scorched earth, sacrifice of captives). This gives the Moral dynasty early warning of Cruel opponents and may trigger conviction events.
- Neutral Scholar: Standard path performance.
- Cruel Scholar: Intelligence Brief is extended by 30 seconds and also reveals the target dynasty's faith vulnerability — which faith-based events would cause them the most instability. The Cruel Scholar treats knowledge as a weapon rather than a tool. Research Momentum extends to include building construction speeds, not just research trees.

**Faith interaction:**
- A Scholar with any faith intensity above Latent may choose to dedicate their research to sacred knowledge. If they do, the Research Momentum passive applies double speed to faith-related unlocks (doctrine upgrades, sacred rituals, faith building construction) but drops to 6% for secular research. This is a player decision made when the Scholar reaches Active faith intensity.
- A Scholar who is Fervent or Apex but dedicated to secular research experiences an internal conviction conflict — a recurring event that may shift their personal conviction or faith intensity until the contradiction is resolved.

**Level progression:**
- Level 1: Intelligence Brief reveals member count and top-one member name. Research Momentum at 12%.
- Level 2: Intelligence Brief reveals top-three members. Scholar can now advise on a diplomatic exchange — once per game phase, they can evaluate a proposed treaty and flag whether it is likely to hold based on the other dynasty's current state.
- Level 3: Intelligence Brief adds building and resource estimates. Research Momentum increases to 16%.
- Level 4 (divergence):
  - Path A — Arcane Historian: The Scholar's knowledge becomes retrospective. They can analyze why a battle was won or lost, producing a "Battle Lesson" that grants the dynasty a 10% combat stat bonus in the next battle fought under similar terrain or conditions.
  - Path B — Spymaster's Advisor: The Scholar and Assassin/Spy path interact. The Scholar provides targeting analysis for Assassin operations, increasing their success rate by 25%. The Scholar also produces counter-intelligence — a passive ability to detect when an enemy Spy is operating against this dynasty.
- Level 5: Capstone — Mastermind (once per match, the Scholar produces a Grand Analysis that reveals the full state of every active dynasty on the map for 90 seconds — all members, all buildings, all faith and conviction status, all diplomatic relationships currently active) or Legacy Codex (the Scholar begins writing a codex during the match; if the Scholar survives to match end, the codex carries forward as a dynasty artifact that provides permanent bonuses in future matches — this is a long-horizon investment that rewards Scholar survival).

**Death consequence:**
- Research Momentum drops to zero. All research timers immediately extend to their base duration.
- Any Intelligence Briefs currently active expire instantly.
- If the Scholar was writing a Legacy Codex, the unfinished codex is lost permanently.
- A dynasty-wide event fires called "Loss of Counsel" — diplomatic relationship values with all neutral parties drop slightly, representing the dynasty's reduced ability to read political situations.
- If no other Scholar is in the active roster, the dynasty is intelligence-blind for a period proportional to how long the Scholar had been active.

**Rare variant: The Last Remembrancer**
A Scholar who is the final living keeper of a body of knowledge from before The Great Frost. Their Intelligence Brief at Level 1 already functions at what would normally be Level 3 capability. Their Research Momentum passive is 25% from birth. They carry a unique passive called Pre-Frost Record — once per game phase, they can activate a piece of recovered knowledge that grants the dynasty a one-time permanent unlock that would otherwise require multiple research steps. The Last Remembrancer cannot be the target of Assassination — they are protected by a tacit agreement across all dynasties that this knowledge should not die with them. This protection breaks if the dynasty declares open war on two or more other dynasties simultaneously.

---

### PATH 3: PRIEST / PRIESTESS

**Core role:** The dynasty's faith anchor. The Priest does not manage doctrine from a distance — they embody it. Their presence in the realm raises faith intensity among the population. Their presence in a ritual space amplifies the effects of faith-based events. Their death leaves a spiritual wound that takes longer to heal than any physical wound.

**Primary ability (active): Consecration**
The Priest designates a location (a building, a battlefield, a territory) as consecrated ground. For the next 8 in-game hours (a defined period of match time), faith intensity within that territory rises at double its normal rate. Population conviction events that occur on consecrated ground have their moral weight amplified — positive faith events produce larger conviction gains, negative events produce larger conviction costs. Cooldown: One full in-game day cycle. The Priest must be physically present at the location to perform Consecration.

**Secondary ability (passive): Living Doctrine**
While the Priest is active, the dynasty's primary faith has an additional +5% intensity gain rate dynasty-wide. This compounds with natural faith growth. At high levels, this passive can be the difference between a faith stalling at Active and advancing to Devout without the player needing to trigger explicit faith events.

**Conviction interaction:**
- Moral Priest: Consecration also reduces illness event severity on consecrated ground. The Living Doctrine passive contributes to conviction drift toward Moral across the population — the Priest's presence is, over time, a moral steadying force.
- Neutral Priest: Standard path performance.
- Cruel Priest (internal contradiction event): A Cruel conviction player with a Priest is not mechanically barred from the contradiction, but the game acknowledges it. A Cruel Priest experiences recurring personal conviction conflict events. If unresolved, the Priest will eventually convert — they shift to the dark doctrine equivalent (Blood Rite Officiant) through a narrative event, rather than dying or being removed. This is the only path in the game where a member can functionally change their specialization — and only as a consequence of sustained moral contradiction.

**Faith interaction:**
This path is intrinsically the most faith-dependent. The Priest's abilities scale in power with faith intensity:
- Latent (0-20%): The Priest functions but their abilities are muted. Consecration produces a minor bonus. Living Doctrine passive is at half strength.
- Active (20-40%): Standard ability strength.
- Devout (40-60%): Consecration duration extends by 50%. Population conviction events on consecrated ground produce dynasty-wide ripple events — not just local effects.
- Fervent (60-80%): The Priest can perform the Rite of Passage — a ceremony for newborn Bloodline Members that locks in a faith intensity starting point above Latent for that child.
- Apex (80-100%): The Priest becomes capable of Faith Intercession — a once-per-match ability that calls on the faith to protect the dynasty from a single catastrophic event (a siege reaching the Keep, a famine, a succession crisis). The faith responds according to its doctrine — light doctrine produces a miracle-framed protection, dark doctrine produces a cost-framed bargain.

**Light doctrine path:** This path is the standard Priest path described above.

**Dark doctrine equivalent — Blood Rite Officiant:**
The dark doctrine Priest. Where the Priest elevates, the Officiant exacts. The Officiant's version of Consecration is called Bloodmark — it designates a location as a sacrifice site. For the defined period, Born of Sacrifice rituals performed at that location produce +30% power at +15% additional cost. Population in the territory experience a fear-faith response: faith intensity rises faster, but conviction drifts toward Cruel. The Living Doctrine passive for the Officiant raises faith intensity dynasty-wide at the same rate but generates periodic conviction pressure events — the population is aware something dark is being practiced. The Officiant is most effective in a Cruel conviction dynasty where conviction pressure events are already normalized. In a Moral dynasty, a Blood Rite Officiant is a catastrophe waiting to unfold.

**Level progression (Priest):**
- Level 1: Consecration and Living Doctrine at base strength.
- Level 2: Living Doctrine rises to +8% intensity gain rate. Consecration can now be applied to units in the field — a consecrated army receives a small faith-based healing pulse every 30 seconds while active.
- Level 3: The Priest may now train acolytes — a new unit type that spawns at the faith building and has reduced combat capability but provides a small Living Doctrine bonus in their assigned territory even when the Priest is elsewhere.
- Level 4 (divergence):
  - Path A — High Celebrant: The Priest's Consecration can be made permanent — at double the normal faith cost, they can permanently designate one territory as Holy Ground. Holy Ground has all Consecration effects active at all times and cannot have its faith intensity reduced below Active by any event short of the territory changing hands.
  - Path B — The Healer's Path: The Priest's active ability shifts from Consecration to Laying of Hands — a targeted single-unit heal that removes any status effect (poison, cursed conviction state, injury from a combat wound) and restores the target Bloodline Member to full health. This heal has a 10-minute cooldown. At Level 5, Laying of Hands can be used on dying members to prevent death — once, ever, for the entire match.
- Level 5: Capstone — Martyr's Legacy (if the Priest dies at Level 5, they do not simply die; they die as a martyr, and the faith intensity across the entire dynasty surges to maximum for a period before beginning its natural decay, a last gift of unprecedented power) or Eternal Witness (the Priest survives death once, appearing to die but returning at Level 1 in the same location 10 in-game days later — same name, same lineage, different body, the dynasty experiencing this as a miraculous event of the highest faith significance).

**Death consequence:**
- Living Doctrine drops to zero immediately. Dynasty-wide faith intensity begins natural decay without the passive support.
- If the Priest was the only faith anchor in the active roster, all Consecration sites lose their status.
- A faith crisis event fires dynasty-wide. Population conviction and faith intensity both suffer. The severity scales with how long the Priest had been active.
- If the Priest was killed by an enemy dynasty, the faith of that dynasty's population responds negatively — killing a Priest is treated as a transgression even across faith lines.
- If the Priest died performing a ritual (Born of Sacrifice or faith event), the ritual completes at double power as a final act.

**Rare variant: The Walking Miracle**
A Priest born with what appears to be miraculous capability from childhood. From Level 1, their Living Doctrine passive is at 20% intensity gain (normally a Level 5 Priest reaches 15%). Their Consecration can designate two locations simultaneously. Most importantly: the Walking Miracle has a passive called The Evidence — enemies who fight or assassinate members of this dynasty suffer a 5% effectiveness penalty while the Walking Miracle is alive, representing a subtle divine protection no one can fully articulate. The Walking Miracle cannot voluntarily be converted to Blood Rite Officiant through the Cruel conviction path — the internal conflict event fires, but always resolves in the Priest's favor, reinforcing their conviction toward Moral.

---

### PATH 4: GOVERNOR

**Core role:** The dynasty's administrative backbone. Governors do not win battles. They make battles worth winning. A conquered territory without a Governor is a liability. A territory with a skilled Governor becomes an engine of production, loyalty, and stability that compounds over time. In long matches, the dynasty with the best Governors wins wars before they start.

**Primary ability (active): Imperial Edict**
The Governor issues a formal decree covering their assigned territory. The player selects one of three edict types: Prosperity (resource output +25% for 5 in-game days), Loyalty (population loyalty to the dynasty +20, reducing rebellion risk for 10 in-game days), or Mobilization (recruitment speed for all unit types in the territory +35% for 5 in-game days). Only one edict can be active at a time. Cooldown after edict expires: 2 in-game days.

**Secondary ability (passive): Stable Administration**
The Governor's presence reduces the natural decay rate of loyalty and resource output in the assigned territory. Territories without a Governor experience slow loyalty decay and production inefficiency. Territories with an active Governor hold their condition longer and recover from damage (siege, famine, illness) faster.

**Conviction interaction:**
- Moral Governor: Prosperity edicts produce an additional population happiness modifier. Population in this territory have reduced illness event frequency — the Governor's fairness protects public health. Loyalty edict duration extends to 15 in-game days.
- Neutral Governor: Standard path performance.
- Cruel Governor: Mobilization edict produces a full +50% recruitment speed instead of +35%, but the territory's population conviction drifts Cruel during active Mobilization. Prosperity edicts in a Cruel Governor's territory are taxed prosperity — production output is higher but a portion goes to the dynasty's central stockpile at double the normal rate, and the local population does not benefit. Loyalty is maintained through fear: Loyalty edict duration is 10 days but includes a suppression modifier that prevents any rebellion events regardless of conditions (including conditions that would normally guarantee rebellion under a Moral governor).

**Faith interaction:**
- A Governor with Devout faith or higher may declare a territory a Faith Territory — a permanent status that raises faith intensity in the territory passively. This is a decision, not an ability, and it is not reversible without a political event.
- A Governor who is Fervent may perform the blessing of public works — faith building construction in their territory is 20% faster and the completed buildings receive a permanent +10% effectiveness bonus.

**Level progression:**
- Level 1: Imperial Edict and Stable Administration at base strength. One edict type available (player chooses which at assignment).
- Level 2: All three edict types unlocked. The Governor can now assign a deputy — a non-Bloodline administrator unit that maintains 50% of the Stable Administration passive if the Governor needs to be recalled to the Keep.
- Level 3: Stable Administration reduces resource decay in territory to near-zero during normal conditions. Edict recovery window between edicts reduces to 1 in-game day.
- Level 4 (divergence):
  - Path A — Builder of Empires: The Governor unlocks a unique action — Grand Project. Once per match phase, the Governor can commission a settlement-level construction project that permanently upgrades the territory's production cap by one tier. This takes significant time and resources but the result is permanent.
  - Path B — The People's Hand: The Governor develops a personal relationship with the territory's population. The territory gains a named loyalty modifier tied to the Governor's name — "The People's Loyalty to [Name]." If this Governor is ever recalled or dies, the population experiences a grief event. But while they are present, the territory has a loyalty floor that cannot drop below a set threshold regardless of dynasty-wide conditions.
- Level 5: Capstone — Realm Architect (once per match, the Governor can reorganize the territory's production infrastructure — this is a complex action that takes time but permanently restructures the territory to prioritize one resource type at the cost of another, at a ratio significantly better than normal allocation tools) or Legacy Governor (the Governor's administration leaves such a deep institutional mark that upon their death or retirement, the territory retains 75% of their Stable Administration passive permanently, as a lasting administrative culture).

**Death consequence:**
- Stable Administration ends. The territory immediately begins loyalty and production decay at the natural rate.
- If an edict was active, it expires immediately.
- The territory fires a "Governor Lost" event — population morale drops, loyalty falls by a flat amount, and the territory's vulnerability to rebellion increases for a period.
- If the Governor had reached Level 4 Path B and built named loyalty, the grief event is significant: loyalty falls sharply before recovering, and the population will have a reduced response to whoever replaces this Governor.
- If the Governor died due to assassination by a rival dynasty, the population's hostility to that dynasty increases permanently in the territory.

**Daughters as Governors:** A daughter Governor benefits from the positive healing/sustainment modifier — in the Governor context, this translates as a bonus to illness event resistance in the territory and a longer duration on Loyalty edicts. The attack modifier penalty has no direct mechanical expression in the Governor path, since Governors do not personally fight. A daughter Governor is in many practical ways slightly stronger than a son Governor for this path.

**Rare variant: The Eternal Steward**
A Governor whose administrative competence is so foundational that territories they manage never experience production loss under any condition short of active siege. Their Stable Administration passive is active at full strength even when they are physically at the Keep rather than in their territory (the only Governor who can administrate remotely without a deputy). Their Imperial Edict has no cooldown — the period between edicts does not exist. They cycle freely, limited only by the edict duration itself. Once per match, the Eternal Steward can Absorb a Crisis — canceling any rebellion, famine, or illness event in their territory with no mechanical consequence, representing an administration so prepared for disaster that disaster cannot find purchase.

---

### PATH 5: MERCHANT LORD / TRADE MISTRESS

**Core role:** The dynasty's economic engine. The Merchant Lord extends the dynasty's reach beyond its territories through trade networks, brokered agreements, and resource arbitrage. They turn surpluses into advantages and advantages into dominance. A dynasty with an active Merchant Lord at mid-game has resource options that pure territorial control cannot match.

**Primary ability (active): Open Route**
The Merchant Lord establishes a trade route between two points the player designates — two of the dynasty's own territories, or a territory and a neutral trading partner. While the route is active, a defined resource flows in the designated direction at an enhanced rate. The Merchant Lord can have one active route per two levels (one at Level 1-2, two at Level 3-4, three at Level 5). Each route generates a small ongoing income regardless of what is being traded.

**Secondary ability (passive): Market Intelligence**
The Merchant Lord passively tracks the resource prices of all factions within diplomatic contact range. The player sees, at any time, which factions are surplus in which resources and which are deficit — information that is otherwise opaque. This turns trade negotiations from guesswork into informed leverage.

**Conviction interaction:**
- Moral Merchant: Trade agreements brokered by the Moral Merchant include a trust modifier — other dynasties (AI or player) are more likely to honor trade terms, and the route is less likely to be sabotaged.
- Neutral Merchant: Standard path performance.
- Cruel Merchant: Open Route can be used for a blockade variant — instead of establishing a trade flow, the Cruel Merchant can close a neutral trade route their dynasty does not control, starving a rival faction of a resource they depend on. This is a covert economic weapon. If discovered, it constitutes an act of economic war with diplomatic consequences.

**Faith interaction:**
- A Merchant Lord with Devout or higher faith in a trade-neutral faith (one that does not restrict commerce) receives no change.
- A Merchant Lord with Devout or higher faith in a faith that holds wealth as morally complex (specific to the faith system design) will experience recurring conviction events — the merchant life and the faith doctrine are in tension. This is not punishing; it is character pressure that produces interesting gameplay decisions.

**Level progression:**
- Level 1: One active trade route. Market Intelligence covers immediate neighbor factions only.
- Level 2: Trade routes generate 15% more per cycle. Market Intelligence expands to all factions within two political degrees.
- Level 3: Two active trade routes. The Merchant Lord can now identify and exploit a surplus — once per game phase, they can purchase a surplus resource from a neutral faction at below-market rate, using their personal reputation as collateral.
- Level 4 (divergence):
  - Path A — The Grand Exchange: The Merchant Lord establishes a permanent trade hub in one territory. The hub generates passive income independent of any active trade route and attracts neutral traders who occasionally bring rare resources not otherwise available in this match.
  - Path B — The Shadow Market: The Merchant Lord establishes a covert trade network. Trades conducted through the shadow market are not visible to other dynasties' Scholars or spies. The Cruel Merchant's blockade ability becomes undetectable. The shadow market also enables trading in non-standard goods — information, political favors, covert military access.
- Level 5: Capstone — Dynasty Endowment (once per match, the Merchant Lord converts accumulated trade reputation into a permanent resource cap increase for the dynasty — the dynasty can now stockpile more of every resource type than before) or The Living Market (the Merchant Lord's trade network becomes self-sustaining — all routes continue generating income for 10 in-game days after the Merchant Lord's death or capture, as the network operates on momentum).

**Death consequence:**
- All active trade routes close immediately.
- If the Merchant Lord had established a Grand Exchange hub (Level 4 Path A), the hub loses the Merchant Lord's direction and revenue drops to 40% of its normal output.
- Resource income that the dynasty had been treating as guaranteed drops immediately. Dynasties that over-relied on trade without a backup Merchant Lord may face sudden resource deficits.
- Neutral trading partners who had established a relationship with this Merchant Lord by name reduce their willingness to trade with the dynasty — they will trade again, but the personal relationship is gone.
- A "Disrupted Commerce" event fires in territories that depended on trade income, causing loyalty decay.

**Daughters as Trade Mistresses:** The positive sustainment modifier for daughters translates here as route endurance — trade routes established by a daughter Merchant tend to stay active longer and resist disruption (enemy sabotage, political volatility) more effectively. In the Merchant path, daughters have a marginal mechanical advantage over sons in terms of long-term trade stability.

**Rare variant: The Architect of Markets**
This Merchant Lord appeared once in the recorded history of their people and shaped commerce in a way that outlasted them by generations. From Level 1, they can open three simultaneous trade routes, not one. Their Market Intelligence has full map coverage at Level 1 — every faction, every resource, every surplus and deficit visible from session start. Once per match, they can broker a Prosperity Agreement — a multi-party trade arrangement that involves three or more dynasties simultaneously and produces a bonus for all parties that is larger than any bilateral trade can achieve. The Prosperity Agreement is immune to Cruel Merchants' sabotage — the trust weight of the Architect's name protects it.

---

### PATH 6: ASSASSIN / SHADOW

**Core role:** The dynasty's covert instrument. The Assassin does not appear in battle formations. They move through the political layer of the game, removing threats before they become crises, gathering intelligence that cannot be obtained through honest means, and extending the dynasty's reach into territory it does not officially control. An Assassin changes the risk calculus for every other dynasty — knowing one is active changes how opponents play.

**Primary ability (active): Contract**
The Assassin is assigned a target — a named enemy Bloodline Member. The contract activates a multi-phase operation: approach, positioning, execution. The player can optionally add resources to the contract to accelerate phases or reduce detection risk. The success probability is displayed as an estimate throughout the operation. If the contract succeeds, the target Bloodline Member dies — no combat, no battle, just a death event in the enemy's Keep with a cause listed as "unknown" or "suspicious" unless the operation is traced back. If the contract fails (discovery during any phase), the Assassin may retreat safely or be captured, depending on how the failure resolves. Cooldown after any contract (success or failure): one full in-game day cycle.

**Secondary ability (passive): Shadow Network**
The Assassin maintains a network of informants in territories adjacent to the dynasty's borders. While the Assassin is active, the player receives warning events when specific types of actions are taken by neighboring dynasties — military mobilizations, faith-based rituals, assassination contracts launched against this dynasty. The Shadow Network does not reveal everything; it reveals specific categories of activity the player can configure.

**Conviction interaction:**
- Moral Assassin: The Assassin will only accept contracts against targets who meet defined criteria — enemies of the dynasty who have committed documented acts of violence against the dynasty's population, or combatants. They will refuse contracts against Scholars, Priests, children, or any member in dormancy. Contract success rates are slightly lower (the Moral Assassin approaches cautiously, which reduces leverage) but discovery rates are also lower (they leave less evidence).
- Neutral Assassin: No restrictions on contract targets. Standard success and discovery rates.
- Cruel Assassin: Contracts against any target including non-combatants, scholars, and faith leaders. When a contract succeeds, the Cruel Assassin can choose to leave a signature — a deliberate message to the enemy dynasty that this was intentional. This creates terror: the enemy dynasty's population conviction drifts Cruel, their Bloodline Members begin suffering a morale penalty, and their Frost Elder (if still active) ages faster. The signature cannot be traced to this dynasty diplomatically, but over time, patterns emerge.

**Faith interaction:**
- An Assassin with any faith intensity above Latent exists in permanent tension with the faith's doctrine (for light doctrine faiths). The faith produces recurring conviction conflict events for the Assassin personally.
- A dark doctrine faith has no such conflict with an Assassin — the Shadow path is acknowledged and accommodated within dark doctrine theology. An Assassin in a dark doctrine dynasty experiences no faith tension and may receive faith bonuses for successful contracts (the faith counts elimination of enemies as devotion).

**Level progression:**
- Level 1: One active contract at a time. Shadow Network covers immediately adjacent territories.
- Level 2: Contract phases resolve faster. The Assassin can maintain one permanent informant in an enemy territory — a planted agent that provides continuous intelligence from that territory without a contract being active.
- Level 3: Two active contracts simultaneously. The Assassin can now stage deaths as accidents — successful contracts at Level 3 or higher can be disguised as natural causes, making discovery by the enemy Scholar even harder.
- Level 4 (divergence):
  - Path A — Master of Blades: The Assassin's contracts achieve a base 20% success rate increase. At Level 5, the Assassin can execute a Decapitation Contract — an operation against the enemy's highest-level active member, regardless of protection status.
  - Path B — Architect of Shadows: The Assassin becomes the Spymaster. Shadow Network expands to full map coverage. The Assassin can now run double agents — placing informants within enemy dynasties who serve this dynasty while appearing to serve the enemy. Double agents can misdirect enemy Assassin contracts, cause them to fail without apparent cause.
- Level 5: Capstone — Ghost Protocol (the Assassin becomes functionally invisible to all detection mechanisms, including Scholar Intelligence Briefs — they do not appear in any enemy dynasty's member list; for the enemy, this member does not exist) or The Last Lesson (once per match, the Assassin can execute a contract with 100% success probability, no phases, no risk — the target dies, no discovery possible; the ability represents a moment of absolute professional perfection that can only happen once).

**Death consequence:**
- Shadow Network collapses instantly. All informants scatter or go silent.
- Any active contract fails and the enemy dynasty receives a warning that an operation was attempted.
- If the Assassin was running double agents, those agents lose their handler — some will be discovered by the enemy dynasty, triggering diplomatic incidents.
- The dynasty loses its covert layer until a new Assassin is trained — which requires a new birth event and childhood period.
- If the Assassin was a Cruel Assassin who had been leaving signatures, their death may be treated by the enemy dynasty as retaliation completed, de-escalating tensions that had been building.

**Rare variant: The Name That Isn't**
An Assassin whose identity has been erased so completely that even their own dynasty's records show only a symbol, not a name. The player can assign a name, but lineage tracking maintains the symbol alongside it. From Level 1, this Assassin cannot be detected by any Shadow Network or Scholar Intelligence Brief — they are categorically invisible to opponent detection systems. Their Contract ability has no cooldown phases — it is a single resolution event (success or failure, no intermediate phases to interrupt). They can run three simultaneous contracts at Level 1. The Name That Isn't has one limitation: they cannot form personal relationships. They cannot hold a named trade agreement, cannot be the subject of a diplomatic hostage arrangement, and cannot form a marriage bond. They are a weapon, not a person, and the game reflects that without sentimentality.

---

### PATH 7: HERALD / DIPLOMAT

**Core role:** The dynasty's political interface with the world. The Herald manages relationships, brokers agreements, prevents unnecessary wars, and when wars happen, manages the aftermath. In a game where every dynasty's actions have diplomatic weight, the Herald is the member who turns that weight into leverage rather than letting it fall as burden.

**Primary ability (active): State Envoy**
The Herald is dispatched to another dynasty as a formal diplomatic envoy. While dispatched, the Herald opens a negotiation window that allows the player to propose, counter-propose, and finalize political agreements — non-aggression pacts, trade terms, military alliances, prisoner exchanges. The Herald's level determines how complex an agreement can be negotiated. The envoy period lasts for a defined window; if agreement is not reached, the Herald returns. While dispatched, the Herald is not available for other duties. Cooldown: 3 in-game days after returning from any envoy mission.

**Secondary ability (passive): Court Reputation**
The Herald builds and maintains the dynasty's political reputation passively. All diplomatic relationship values decay more slowly while a Herald is active. New dynasties met for the first time receive a starting relationship value 10 points higher than baseline. The dynasty's diplomatic agreements have a lower failure/betrayal rate from AI dynasties while a Herald is active — the Herald's ongoing presence signals that the dynasty takes its agreements seriously.

**Conviction interaction:**
- Moral Herald: All agreements negotiated by the Moral Herald include an Honor Bond modifier — if the other party breaks the agreement, they suffer a dynasty-wide reputation penalty with all other factions. This creates a social enforcement mechanism. The Moral Herald also has a unique ability: they can call for a Blood Peace — a formal cessation of hostilities that requires both sides to commit Bloodline Member time to the peace process. Once established, a Blood Peace cannot be broken for a defined period regardless of circumstances.
- Neutral Herald: Standard path performance.
- Cruel Herald: The Herald can negotiate under false pretenses — proposed agreements include hidden trap clauses that the player designs. When the other party activates the agreement's terms, the trap clause activates in the Cruel dynasty's favor. The Cruel Herald can also conduct coercive diplomacy — using the threat of the dynasty's Warlord as a negotiating tool in ways that increase agreement success rates but damage long-term relationships.

**Faith interaction:**
- A Herald with Devout faith or higher becomes a Faith Diplomat — they can negotiate faith-specific agreements (mutual faith protection pacts, shared sacred site access, doctrine non-interference treaties) that are not available to Heralds below Devout.
- A Herald negotiating with a dynasty of the same faith receives a baseline relationship bonus.
- A Herald negotiating with a dynasty of an opposing faith should be prepared for harder negotiations — the player is warned of this in the envoy mission preview.

**Level progression:**
- Level 1: Basic agreement types — non-aggression, basic trade terms. Court Reputation at base strength.
- Level 2: Alliance framework available. The Herald can now negotiate prisoner exchanges and ransoms, resolving hostage situations that would otherwise require military action.
- Level 3: Full diplomatic suite unlocked — multi-party agreements, marriage proposals (cross-dynasty), vassal relationships. The Herald can now maintain two simultaneous active agreements instead of one.
- Level 4 (divergence):
  - Path A — Master Negotiator: Agreement terms the Herald negotiates are 25% more favorable to this dynasty across all categories. The player presents the same offer but gets better terms — representing the Herald's personal negotiating skill.
  - Path B — The Peace Architect: The Herald can establish a Permanent Embassy in one other dynasty's territory. The embassy produces continuous intelligence about that dynasty (lower quality than a Scholar's brief but passive) and prevents that dynasty from taking hostile action against this dynasty without triggering a diplomatic penalty first.
- Level 5: Capstone — The Grand Treaty (once per match, the Herald can negotiate a comprehensive multi-party agreement involving all active dynasties simultaneously — a World Agreement that addresses resource, military, and faith relationships in a single binding document; the specific terms must be negotiated but the framework is unprecedented in diplomatic history and produces substantial long-term bonuses for the dynasty that initiates it) or The Ambassador's Legacy (when this Herald dies, every agreement they negotiated remains in force for an additional 15 in-game days before expiring, and all parties are notified of the Herald's death — generating a dynasty-wide reputation boost from the grief of other dynasties who respected this person).

**Death consequence:**
- Court Reputation drops to baseline. All diplomatic relationship values begin natural decay at the standard rate.
- Any active envoy mission fails instantly. If the Herald was mid-negotiation, the agreement collapses and the other dynasty registers a breach of diplomatic process.
- Active agreements lose their Herald's integrity modifier — AI dynasties are slightly more likely to break terms without the Herald's ongoing oversight.
- A "Loss of Voice" event fires — the dynasty loses its primary diplomatic instrument and must rely on direct power for a period.
- If the Herald was a Moral Herald who had established Blood Peaces, those peaces remain in force for their duration but cannot be renewed without a replacement Herald reaching Level 3.

**Daughters as Heralds:** The sustainment modifier for daughters translates directly in the Herald path as relationship endurance — diplomatic agreements negotiated by a daughter Herald have a longer natural lifespan before expiring. In court reputation terms, daughter Heralds build relationship capital more slowly in militaristic dynasties (the negative modifier represents some factions taking less seriously an envoy who cannot personally threaten them) but maintain it better over time. This is a subtle tendency, not a hard rule.

**Rare variant: The Living Peace**
A Herald whose personal presence has such gravitational political weight that other dynasties will not attack this dynasty while the Living Peace is the active diplomatic member. This is a passive aura — not an ability, not a timed effect, not a limited charge. As long as the Living Peace is alive and active, AI dynasties treat direct military aggression against this dynasty as a last resort and require triggering conditions to override that reluctance. The Living Peace cannot be assassinated — not because they are protected mechanically, but because the game calculates that no dynasty would accept the reputation cost of killing them. The Living Peace has one vulnerability: if their dynasty behaves in ways that fundamentally violate the political trust they embody (breaking major agreements, mass atrocities, faith persecution), the aura collapses and the Living Peace enters a personal crisis event that may permanently reduce their effectiveness.

---

### PATH 8: HEIR APPARENT

**Core role:** The designated successor. The Heir Apparent is the dynasty's continuity under pressure. They are not the most powerful member today. They are the member whose survival is most important to tomorrow. Their existence is a statement about the dynasty's future, and the game treats it that way.

**Primary ability (active): Succession Rehearsal**
The Heir Apparent spends time in direct apprenticeship with the dynasty's senior members. The player designates which member the Heir is apprenticing with. For the duration of the apprenticeship (3 in-game days), the Heir gains 50% of that member's passive ability as an additional passive on top of their own path bonuses. After completing three distinct apprenticeships across the match, the Heir permanently gains a small bonus reflecting what they learned from each. This is the system by which an Heir eventually becomes well-rounded rather than simply a successor.

**Secondary ability (passive): Dynasty Anchor**
While the Heir Apparent is active, the dynasty's succession is stable. Succession crises do not fire while the Heir Apparent is alive. If the current leader dies (including the Frost Elder), succession passes to the Heir Apparent smoothly with no stability penalty. Without a designated Heir, leader death triggers a succession crisis event with significant negative consequences.

**Conviction interaction:**
- Moral Heir: The Succession Rehearsal absorbs not only the passive ability of the mentor but also their conviction weight — a Moral Heir apprenticing with a Moral mentor receives a conviction bonus. A Moral Heir apprenticing with a Cruel mentor triggers a conflict event — the Heir resists learning from cruelty and the apprenticeship produces a fractured outcome (reduced bonuses, conviction pressure on both).
- Neutral Heir: Absorbs apprenticeship bonuses cleanly regardless of mentor's conviction.
- Cruel Heir: Absorbs apprenticeship bonuses at full strength from any mentor. Can also learn from covert observation of enemy dynasties — a unique mechanic where the Cruel Heir can designate an enemy member as an unwilling mentor, gaining a partial version of their passive through observation and study of captured intelligence.

**Faith interaction:**
- If the Heir has higher faith intensity than the current dynasty leader, a succession tension event may fire — the dynasty's faithful population is quietly lobbying for the Heir to take the throne sooner. This is not a crisis; it is political pressure that the player can use or ignore.
- At Fervent faith intensity, the Heir can be anointed by the Priest — a ceremony that formally declares them the faith's chosen successor. This grants a dynasty-wide faith intensity bonus and reduces the succession crisis penalty to zero (already zero if the Heir Apparent passive is active, but this also covers unexpected multi-member death cascades).

**Level progression:**
- Level 1: Dynasty Anchor active. Succession Rehearsal available with one apprenticeship slot.
- Level 2: The Heir gains a personal leadership ability — once per game phase, they can issue a Minor Edict in the absence of the current leader, covering any territory without a Governor. This represents them practicing leadership.
- Level 3: Succession Rehearsal absorbs increase to 75% of mentor passive. The Heir can now defend themselves in the field — they are deployable as a mid-tier combat unit, not a Warlord, but not helpless.
- Level 4 (divergence):
  - Path A — The Heir Ready: The Heir has completed their preparation. They receive a permanent +15% bonus to all stats across every path they have apprenticed in. When they assume leadership (upon the current leader's death), they inherit the current leader's level rather than their own level, capping at the current leader's maximum.
  - Path B — The Shadow Heir: The Heir has decided they will not wait. They develop political ambitions that the player can direct constructively or destructively. Constructively: they build their own independent support network, reducing succession crisis risk further and giving the dynasty a resilient dual-leadership structure. Destructively: a Cruel dynasty can authorize the Shadow Heir to accelerate succession through events that weaken the current leader's position.
- Level 5: Capstone — The Dynasty Made Flesh (upon assuming leadership after the Frost Elder or current leader dies, the Heir Apparent becomes a genuinely powerful leader whose combined apprenticeship bonuses and leadership ability produce a member stronger than any path specialist — not the best fighter, not the best administrator, but the best overall sovereign the dynasty has produced) or The Eternal Heir (the Heir never assumes leadership — they choose to remain the Heir forever, providing Dynasty Anchor and Succession Rehearsal for the entire match; the dynasty never needs to worry about succession again, and the Heir's bonuses from all apprenticeships compound across the match without ever being spent on an actual succession).

**Death consequence:**
- Dynasty Anchor ends. Succession is now unstable.
- If the Heir died before the current leader, the dynasty must designate a new Heir from the active roster. Until a new Heir is designated, succession risk events can fire.
- If the Heir dies as a result of assassination and the player can determine which dynasty ordered it, a blood debt event fires — this assassination is treated as a declaration of dynastic war regardless of other diplomatic relationships.
- If the Heir was the last young member (all other members are senior), the dynasty enters a generational crisis — there is no clear next generation, and the population begins to question the dynasty's future.

**Rare variant: The Once and Future Heir**
An Heir Apparent who embodies the dynasty's potential so completely that their existence changes how other dynasties perceive this one. At Level 1, they already have completed one apprenticeship (assigned randomly at birth — the player does not choose which path; the game reflects what the Heir was drawn to naturally). Court Reputation (normally a Herald passive) applies in a reduced form to all diplomacy this dynasty undertakes while the Once and Future Heir is active, because other dynasties are negotiating with their future in mind. When this Heir eventually assumes leadership, all members of the dynasty receive a permanent +5% to all stats — this is not the leader's ability, it is the effect of inspiration, of being led by someone the dynasty has been anticipating for their entire existence.

---

## SECTION B: THE FROST ELDER — STARTING LEADER DESIGN

PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON

### The Frost Elder in Context

The Frost Elder is unique in the Bloodlines system for a single defining reason: they will die before the match ends. This is not a possibility. It is a certainty written into the character. The Great Frost killed people who were young and strong. The Frost Elder survived it, but the survival cost them. They carry the cold in their bones, and the cold will eventually collect what it is owed.

This design priority governs everything about the Frost Elder: they are powerful early, they are emotionally resonant throughout, and their death is the first major narrative event of every match. Players should feel this death coming. They should be preparing for it. And when it happens, it should land.

---

### What Frost Elders Have That Others Do Not

**Living Memory:**
Frost Elders survived The Great Frost. This gives them access to knowledge that cannot be earned through research, experience, or faith — only through having been there. Living Memory manifests as a passive ability that provides the dynasty with three pre-match advantages, selected from a larger pool and unique to each Frost Elder archetype:
- Ancient Trade Paths (trade route options in the first game phase that no other dynasty can access)
- Pre-Frost Military Records (starting army with one veteran unit — a unit with a permanent experience bonus from training that predates the match)
- The Survivor's Fortifications (one territory starts with a wall-level defensive structure that would normally require significant construction)
- Names from Before (the Frost Elder knows the names of ancient political entities; specific diplomacy options open in early-match negotiations that close after the Frost Elder dies)
- Frost-Harden Technique (units trained during the Frost Elder's lifetime have 10% higher cold-terrain combat effectiveness — relevant in specific map types)

Each Frost Elder archetype has a fixed set of two Living Memory benefits from this pool, established at character design.

**Counsel of the Aged:**
Once per in-game day, the Frost Elder can issue counsel to any active Bloodline Member. This functions as a temporary buff — the chosen member gains +20% to their primary ability's effectiveness for 24 in-game hours. This cannot be used on the Frost Elder themselves. The Counsel represents the Frost Elder passing hard-won knowledge to the next generation in real time.

**The Weight of Having Survived:**
The Frost Elder's presence passively raises the population's conviction stability. While the Frost Elder is alive, conviction drift events (events that push the population toward extremes of Moral or Cruel) have their severity reduced by 25%. The population's sense that someone who survived The Great Frost is leading them produces a steadying effect. This disappears entirely when the Frost Elder dies.

**Last Lesson:**
When the Frost Elder's health reaches its final threshold (approximately 15-20% of their life remaining), they can deliver a Last Lesson to one Bloodline Member. This is a one-time, permanent ability transfer — the chosen member permanently gains a passive ability that reflects what the Frost Elder most valued. The Last Lesson ability is specific to each Frost Elder archetype and is the most powerful single benefit the Frost Elder can provide. It cannot be rushed — the Frost Elder must reach the threshold naturally, and the player must choose a recipient before the Elder dies. If the Frost Elder dies suddenly (assassination, combat) without having reached the threshold or used Last Lesson, it is lost.

---

### How the Approaching Death Manifests in Gameplay

The Frost Elder does not have a visible countdown timer. There is no health bar drain during peaceful periods. This is a deliberate design choice: the death should feel real, not like a mechanic.

Instead, aging manifests through event signals and visual changes:

**Visual aging (Keep UI):**
In the Keep interior, the Frost Elder's portrait and representation changes over match phases. Early match: upright, active, visually commanding. Mid-match: seated more often, the Keep's eldest chair is theirs. Late match: a visible illness marker appears on their representation. The player sees the Elder slowing down in their own home before it is mechanically relevant.

**Frost Sickness events:**
At a point in the mid-to-late match (randomized within a window, weighted by map difficulty and match intensity), Frost Sickness events begin firing for the Frost Elder. These are illness events specific to this member:
- Minor: "The Elder rested poorly last night. Their counsel is unfocused." (Counsel of the Aged is unavailable for one day.)
- Moderate: "The Elder's breathing is labored. They have stayed near the fire and refused to leave the Keep." (The Elder cannot be deployed to the field.)
- Severe: "The Elder called for the family. They have things to say." (Last Lesson window opens even if the health threshold has not been reached. The player is alerted that time is very short.)

The events escalate naturally. A player who has been watching will recognize the pattern. The first Minor event is a warning. The Severe event is the final warning.

**Death window:**
After the Severe event fires, the Frost Elder will die within a defined window. The exact moment is not fixed — there is variance within the window. The player cannot prevent it. They can use the Last Lesson if they have not yet, and they should be prepared for the succession.

---

### What Happens When the Frost Elder Dies

**Succession resolution:**
If a Heir Apparent is designated, succession passes to them immediately and smoothly. The transition is narrated in the Keep — the Heir Apparent is shown assuming the Elder's chair. If no Heir is designated, a succession crisis fires: the active roster's senior members compete for leadership, and the player must choose from among them through a crisis resolution event. The crisis takes in-game time and produces political instability.

**Morale cascade:**
The Frost Elder's death triggers a dynasty-wide morale event. For 3 in-game days, population loyalty drops slightly, resource output is reduced 10%, and Bloodline Member abilities have a minor effectiveness penalty. This is grief. It is not catastrophic; it is real. The reduction represents people being distracted, disorganized, and sad.

**The Weight of Having Survived — collapse:**
The conviction stability passive ends. If the dynasty had been relying on it to buffer against conviction drift, the next few conviction events may have unusually large effects. The player should be aware of this vulnerability in the days following the Elder's death.

**Conviction signal:**
A dynasty-wide conviction event fires. Its content varies by how the Frost Elder lived. If the Elder was Moral, the event is a eulogy that reinforces Moral conviction. If the Elder was Neutral, the event is respectful but does not push in either direction. If the Elder was Cruel, the event is complex — the population respected the Elder's strength but there is also a quiet relief, and the player can direct the resulting conviction energy toward either axis.

**The Frost Elder's Grave:**
The Keep UI gains a permanent grave marker in the courtyard or equivalent structure after the Frost Elder dies. This is not a gameplay mechanic. It is a visual record. It stays for the entire match. Future generations of Bloodline Members can be shown paying respects to it — a quiet acknowledgment that this person existed and that their dynasty remembers them.

---

### Can a Player Extend the Frost Elder's Active Period?

Yes, through specific faith mechanics and player decisions — but with costs and limits.

**Faith Intercession (Priest required):**
If the dynasty has an active Priest at Fervent faith intensity or higher and the Frost Elder is the current leader, the Priest may perform a Rite of Extended Years during the Severe illness event window. This delays the death window by a defined period (not indefinitely). The rite costs faith intensity (the dynasty's faith drops by one tier for 5 in-game days while the resources of belief are concentrated on one person). This is a meaningful sacrifice — the dynasty trades a faith tier to keep the Elder alive longer.

**Specific event choices:**
Several Frost Sickness events offer branching choices. The player who consistently chooses "rest the Elder rather than deploy them" reduces the severity escalation rate slightly. Pushing the Elder into field deployment during Moderate illness events accelerates the Severe event. This is not a survival system — it is a pacing system. Player choices affect when, not whether.

**Born of Sacrifice (last resort):**
In a dark doctrine dynasty, a Born of Sacrifice ritual can be directed at the Frost Elder's illness — offering something of great value to buy more time. The cost is significant and the game makes clear this is not a solution, only a delay, with a debt attached. This option is available to dark doctrine players and presents a genuine moral weight: what would you sacrifice to keep the person who keeps the dynasty stable?

---

### Three Frost Elder Archetypes

**Archetype 1: THE FROST COMMANDER**
*Father or elder uncle who led armies during The Great Frost. The military figure. Scarred, direct, respected by soldiers.*

**Starting stat profile:**
- Combat: High
- Diplomacy: Low
- Administration: Moderate
- Faith: Low to Moderate (the Frost was not a spiritual event to them; it was a military crisis they managed)
- Conviction: Tends toward Neutral or Moral; the experience of loss made them pragmatic but not cruel

**Living Memory benefits:**
- Pre-Frost Military Records (starting veteran unit)
- Frost-Harden Technique (cold-terrain combat bonus for all trained units)

**Unique passive — The Old Formation:**
Units led directly by the Frost Commander on the battlefield have +20% damage resistance. This is not a battle cry or an active. It is simply what happens when soldiers see this old warrior at the front. They do not break while he is with them.

**Last Lesson (delivered to the chosen Bloodline Member permanently):**
"Hold the Line" — the recipient permanently gains a passive that prevents their assigned army from routing (fleeing in panic) for 30 seconds after they should have, giving them one window to turn a losing fight. Usable once per battle but not once per match — once per battle, every battle, for the rest of the match.

**Narrative weight of death:**
The Frost Commander's death should feel like the departure of a generation's military certainty. Players who relied on their armies feel the gap most acutely. In the Keep, the remaining Bloodline Members gather in the war room rather than the family chamber — soldiers mourn differently.

---

**Archetype 2: THE FROST SAGE**
*The dynasty's eldest advisor. Not a warrior. A keeper of records, a reader of signs, a person who survived The Great Frost by understanding it rather than fighting it.*

**Starting stat profile:**
- Combat: Very Low (the Frost Sage does not fight)
- Diplomacy: High
- Administration: High
- Faith: High (the Frost was deeply spiritual to them; it confirmed their faith rather than testing it)
- Conviction: Tends toward Moral (having seen great suffering, they are committed to preventing more)

**Living Memory benefits:**
- Names from Before (early-match diplomacy options unavailable to others)
- Ancient Trade Paths (trade routes in the first phase that no other dynasty can access)

**Unique passive — The Witness Account:**
Once per game phase, the Frost Sage can invoke a historical precedent — they describe what happened during a similar crisis in The Great Frost and what worked. The player receives one guaranteed positive resolution to any current crisis event (famine, diplomatic breakdown, faith crisis) by choosing the Sage's recommended response. The Sage's memory contains actual solutions.

**Last Lesson (delivered to the chosen Bloodline Member permanently):**
"What The Frost Taught" — the recipient permanently gains a research bonus (+25% speed to all unlocks for the rest of the match) and gains a once-per-match ability called Pattern Recognition (for 60 seconds, enemy Bloodline Members' next action is predicted — the player sees what type of ability an enemy member is about to use before they use it).

**Narrative weight of death:**
The Frost Sage's death is the quietest. They decline slowly, not dramatically. The Keep's fire seems lower the night they go. The loss is cognitive — the dynasty loses its memory of a world before this one. In the Keep, books are visible near the Sage's space; after the death, those books close.

---

**Archetype 3: THE FROST MOTHER**
*The matriarch who kept families alive through The Great Frost. Not a general and not a scholar — a sustainer. People are alive because she made decisions no one else would make and kept the group together when it wanted to fracture.*

**Starting stat profile:**
- Combat: Low
- Diplomacy: Moderate to High (she kept people together by knowing each person's worth)
- Administration: Very High (she has been managing scarcity since before most Bloodline Members were born)
- Faith: Moderate to High (she is spiritual but practical about it — the faith serves the living, not the other way around)
- Conviction: Strongly Moral

**Living Memory benefits:**
- The Survivor's Fortifications (one territory starts with a defensive structure)
- Frost-Harden Technique (cold-terrain combat bonus for trained units)

**Unique passive — The Mother's Economy:**
During any resource shortage event (famine, siege, supply disruption), the Frost Mother's administration of the Keep reduces consumption rates by 20%. She has done this before. She knows exactly how to stretch what is there, who gets less so others survive, and how to keep people from losing hope while the count is short. In normal conditions this passive has no visible effect. In a shortage, it is the reason the dynasty survives.

**Unique interaction — the daughters of the dynasty:**
All daughter Bloodline Members born while the Frost Mother is the active leader receive an enhanced version of their natural healing/sustainment modifier. The Frost Mother's influence on their upbringing pushes the positive modifier further than it would naturally be. This is not a universal effect — sons are unaffected. Daughters born under her leadership carry a particular kind of endurance that the Frost Mother herself embodies.

**Last Lesson (delivered to the chosen Bloodline Member permanently):**
"What We Owe Each Other" — the recipient permanently gains a loyalty floor effect on any territory they administer or lead (territory loyalty cannot drop below a moderate baseline while this member is present) and a personal ability called The Reckoning (once per match, they can identify and directly prevent one dynasty member's death from illness — not combat, not assassination, but illness; the Frost Mother's lesson was that illness killed more than swords during The Great Frost, and this recipient learned to see it coming).

**Narrative weight of death:**
The Frost Mother's death is the most emotionally significant of the three archetypes for most players. She was never the most powerful member. She was the most human. In the Keep, younger members are gathered close. Daughters in the active roster experience a unique grief event that temporarily increases their ability effectiveness as they channel what they learned into action — a mechanical expression of carrying someone forward.

---

## SECTION C: BLOODLINE MEMBER LIFECYCLE

PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON

### Birth

New Bloodline Members are born through the Marriage Agreement system. A Marriage Agreement must exist between a dynasty member and either a member of another dynasty or an approved external person (managed through the Herald's diplomatic functions or through specific political events). Marriage Agreements are formal, recorded agreements, not background simulation.

Birth is not continuous or automatic. The game tracks marriage agreements and allows birth events to fire under the following conditions:
- The marriage is active (both parties alive and not captured)
- A birth eligibility window has opened (a defined period after the marriage is established)
- The dynasty's current active member count is below the cap, or the player has designated a dormancy slot for the new child

Birth events fire as narrative events in the Keep UI. The player is notified, the child is named (default name provided; player may rename immediately), and the child enters the childhood phase.

If a marriage agreement exists but the active cap is full and no dormancy slot is available, birth events are held in a queue. A child may be born and placed immediately into dormancy if the player accepts. This represents a child growing up outside the active court — present in the lineage, alive, but not yet integrated into the dynasty's operations.

---

### Childhood Phase

Children exist in a defined childhood phase. During this phase, they are:
- Visible in the Keep UI (small representation in the family tree, different from active members)
- Not deployable and not targetable by Assassination (protected status during childhood)
- Slowly developing toward their specialization path

The childhood phase is where the path assignment happens. The player is shown the child's developing tendencies (a simple indicator: "shows aptitude for military discipline" or "gravitates toward the Keep's prayer room") and makes the specialization assignment during this window. The assignment is made in the child's mid-childhood — not at birth, not at adulthood, but in between, reflecting a decision made when the child's nature is becoming visible but before it is fixed.

The childhood phase has a defined duration in match time. At the end of this phase, the child transitions to active status as a Level 1 Bloodline Member.

One exception: if the dynasty experiences a severe crisis (the Keep is captured, the Frost Elder dies in the same phase, a faith catastrophe event fires), childhood protection may be reduced. Children are not invulnerable — they are protected by convention and social weight, and that protection can erode under extreme pressure. This is rare. It is not intended to be common gameplay. It is intended to signal that the match has gone to a dark place.

---

### Specialization Assignment

The player assigns the specialization path. This is a player decision framed as a life choice made for this child. The UI presents:
- The child's name and emerging tendencies
- The available paths (those the dynasty currently supports — some paths require infrastructure to have been established)
- A brief description of what life in that path will mean

The player selects. The selection is locked in. From that point, the child's childhood representation changes to reflect the path. A Warlord-path child is shown in training armor. A Scholar-path child is shown with books. A Priest-path child is shown near the faith building.

The player cannot change the path after assignment. This is intentional. Children grow into who they will be. The decision matters.

---

### Active Service and Level Progression

Bloodline Members advance through five levels. Advancement is experience-based, but experience is not a single unified currency. Different paths accumulate experience through different activities:

- Warlords gain experience through battles participated in, units commanded, victories led
- Scholars gain experience through completed research, Intelligence Briefs delivered, advisor actions taken
- Priests gain experience through rituals performed, faith intensity maintained above Devout, conviction events resolved
- Governors gain experience through territory loyalty maintained, edicts issued, crises managed without loss
- Merchants gain experience through trade routes established and maintained, resources exchanged, agreements brokered
- Assassins gain experience through contracts completed (not only successful — failed contracts with clean retreats also produce experience)
- Heralds gain experience through agreements finalized, envoy missions completed, relationships maintained above baseline
- Heirs gain experience through apprenticeships completed and governance actions taken in the leader's absence

Experience events are visible to the player as small notifications. The member is growing. The player can see it happening.

Level 4 is the divergence threshold. At Level 3 completion, the player is presented with the divergence choice for that member. This is not a passive transition — the player must actively choose which direction this member's development takes. The choice is permanent and represents a crystallization of who this member has become.

---

### Retirement vs. Death

Under normal conditions, Bloodline Members do not retire. They serve until they die. The game does not include an aging-out mechanic for standard members — only the Frost Elder is guaranteed to age out.

A member may enter semi-retirement through player choice — they can be assigned to the Keep as an advisor (active dormancy) where they provide a reduced passive benefit but are not deployable. This is a player decision, not a game-triggered event. It is available as a way to protect a valuable high-level member from combat risk while still benefiting from their presence.

Death is the standard endpoint. It may come through:
- Combat (deployed to battle and killed)
- Assassination (enemy Assassin contract succeeds)
- Illness event (random, weighted by match conditions, more common in late match)
- Born of Sacrifice (player chooses to sacrifice the member for a ritual outcome)
- Frost Sickness (Frost Elder only)
- Execution (if captured and the captor dynasty chooses execution over ransom)
- Accident events (rare political events that can kill members not expected to be at risk)

---

### The 20-Member Active Cap

When the active roster reaches 20 members, the dynasty is at capacity. New births are routed to dormancy. Dormant members exist in the lineage (visible in the family tree) and are alive, but they are not deployable, do not provide passive bonuses, and cannot use abilities.

A dormant member can be activated when an active member dies, creating an available slot. The player chooses which dormant member to activate. The activation takes a brief period of match time (the member is summoned to the court and integrated) — it is not instant.

Dormant members do not progress through levels while dormant. Their age is tracked but their abilities do not develop. A member who spends a long period in dormancy and is then activated enters at the level they held when they became dormant, not at a higher level.

This creates a strategic layer around the cap: players who carefully manage dormancy keep a trained bench of level-two and three members waiting. Players who do not manage it bring in Level 1 members late in the match when experience gained time is limited.

---

### Captured Members

When a Bloodline Member is captured (taken prisoner during a defeat, ambushed by an Assassin who chooses capture over killing, or taken as part of a political event), they enter a captured state.

While captured:
- The member's passive abilities are inactive. Their presence in the dynasty's roster counts as an occupied slot but provides no benefit.
- The member's path abilities cannot be activated.
- If the captured member was the primary faith anchor (Priest) and no other Priest is active, faith intensity begins decaying.
- If the captured member was the primary Warlord and a campaign is active, that campaign suffers a sustained morale penalty.
- The Heir Apparent captured triggers a specific event: the home dynasty's population enters a grief-fear state that produces a loyalty penalty across all territories (the Captured Heir Homeland Penalty, per canon).

Ransom and rescue mechanics:
- The Herald can negotiate ransom through envoy mechanics. The cost is set by the captor dynasty and involves resources or concessions.
- The Assassin can attempt a rescue operation (a contract variant targeting the capture site rather than a person). Success probability depends on Assassin level and the security of the captor's facility.
- If neither ransom nor rescue occurs within a defined period, the captor dynasty may execute the member — a decision they make based on the value of the member to this dynasty and their own conviction alignment (a Cruel dynasty is more likely to execute; a Moral dynasty is more likely to maintain the hostage for leverage).

---

### Mixed-Dynasty Children

When a Marriage Agreement produces a child between members of two different dynasties, the child is born with a dual heritage. At birth, the player must make a designation choice:

**Option A — Claim the child for this dynasty.** The child enters the normal childhood phase as a Bloodline Member of this dynasty. Their dual heritage is noted in their lineage record but does not affect gameplay mechanically, except that they have a relationship modifier with the other parent's dynasty — they are never fully an enemy to that faction.

**Option B — Allow the child to remain with the other dynasty.** The child is raised in the other dynasty. They appear in lineage records. They may appear later in the match as a diplomatic event — a cross-dynasty relative who creates interesting political pressure.

**Option C — The New Dynasty Path (PROPOSED, not yet canon):** If the child is the third or later generation of cross-dynasty marriages (the child's parents both have mixed heritage from multiple dynasties), the game presents a New Dynasty Option. The player can declare this child the founding member of a new dynasty branch, launching a separate political entity. This is a major late-game event with significant resource and political consequences. It represents the natural result of dynastic intermarriage at scale — eventually, new lineages emerge. Full mechanics for this path require their own design document.

---

## SECTION D: THE KEEP INTERIOR — BLOODLINE MEMBER VISUALIZATION

PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON

### The Representation System

Bloodline Members in the Keep are represented through a layered portrait and spatial system. The Keep's interior is divided into functional zones — the war room, the family chamber, the faith room, the study, the court hall, the courtyard — and members are naturally associated with the zones that match their path.

The Warlord is visible in the war room. The Scholar is in the study. The Priest is in the faith room. The Governor cycles between the court hall and their assigned territory view. The Assassin is rarely visible in the Keep at all — they appear briefly in the shadows of the family chamber, present but not foregrounded. The Herald holds court.

This spatial organization is not decorative. It is the UI through which the player accesses each member's abilities and status. Clicking on the Warlord in the war room opens the Warlord's ability panel. The Keep's interior is the living interface.

---

### Portrait System

Each member has a portrait. The portrait is generated from a combination of the member's dynasty (setting base visual style), path (affecting clothing and bearing), conviction alignment (affecting facial expression, posture, and the quality of light in the portrait), and faith intensity (affecting specific visual markers — faith symbols, ceremonial elements, visible fervor or absence thereof).

The portrait changes over time as the member advances:
- Level 1-2: Young, relatively informal. The bearing of someone in early service.
- Level 3-4: More assured. The clothing and visual markers of someone who has become what they were trained to be.
- Level 5: Fully realized. The portrait at Level 5 is the most visually distinctive — this member has become something complete.

A member's conviction shift is visible in their portrait before the player receives a formal notification. A member drifting Cruel looks different month to month — something in the expression. A member strengthening in Moral faith intensity seems to carry more light in their frame. These are subtle signals before mechanical ones.

---

### Family Tree View

The Keep contains a family tree interface — a full genealogical record of every member born into the dynasty since the match started, living and dead. Active members have full-color portraits. Dormant members are shown slightly desaturated. Dead members are shown in a faded memorial style — present in the tree, visible in their lineage position, but clearly gone.

The family tree is where mixed-dynasty lineage is visible. Children of cross-dynasty marriages have a split visual element in their portrait border reflecting their dual heritage. The tree tracks every relationship, every marriage agreement, every birth.

Players who pay attention to the family tree over a long match develop an emotional relationship with it that would not exist if members were pure stat nodes. A dynasty that started with one Frost Elder and seven other members and now has a full tree of three generations is a living thing. The tree makes that visible.

---

### Keep Condition as Dynastic Health Signal

The Keep's interior reflects the dynasty's condition without explicit status bars:

**Thriving dynasty (15+ active members, high loyalty, high faith intensity):**
The Keep is full. The fire burns high. Multiple members are visible in their respective rooms simultaneously. Children can be seen in the family chamber. The Priest's room glows with faith light. The war room has maps unrolled and figures gathered. The courtyard is active.

**Frost Elder only (early match, only the Elder and a few young members):**
The Keep is spacious and quiet. The Frost Elder is prominently placed — the center of gravity in a largely empty space. The other rooms are present but not densely occupied. The family tree is small. The atmosphere is one of beginning rather than loss.

**Dynasty under pressure (members captured, faith in crisis, low loyalty):**
The fire is lower. Portraits on the wall look toward the viewer differently — the posture of worry is visible. The Priest's room is empty if no Priest is active, and the faith light is absent. The war room has fewer figures gathered. The family chamber has memorial plaques visible for recently lost members.

**Succession crisis (leader just died, no Heir designated):**
The Keep's interior responds immediately. No figure occupies the leader's chair. The family chamber is crowded. The visual is of a vacuum — the absence of the leader is felt as an architectural emptiness at the center of the space.

**Faith apex (80-100% faith intensity dynasty):**
The Keep's visual style shifts toward the faith's aesthetic. A light doctrine faith at apex fills the Keep with symbolic light, architectural details corresponding to the faith's sacred imagery, and the Priest's room becomes the visual center of gravity. A dark doctrine faith at apex produces a different visual register — deeper shadows, sacrificial markers, the Officiant's room commanding a gravity that edges toward the unsettling.

---

### Conviction Alignment Visual Signals

Each active member carries a visual conviction signal in their portrait and in the way they are rendered in the Keep:

- Moral conviction: warm tones in the portrait, upright posture, the person looks like they are at ease with themselves. The member in the Keep moves with purpose toward other people.
- Neutral conviction: balanced, professional. The visual language of competence without declared moral weight.
- Cruel conviction: colder tones, a specific quality of stillness in the portrait that reads as controlled rather than peaceful. The member in the Keep occupies space rather than sharing it.

When a member is in a conviction conflict event (the internal tension of a Cruel Priest, a Moral Assassin being asked to kill a non-combatant), their portrait shows the strain. This is a visual tell before the event resolves — players who watch the portraits will see the crisis coming.

---

### The Frost Elder's Visual Arc

The Frost Elder has the most deliberately designed visual progression in the Keep:

**Early match:** They occupy the most prominent position. The largest chair, the best light, the wall behind them carrying whatever their dynasty considers honorable history. They look old but vital — this is someone who has earned their authority.

**Mid-match (first Frost Sickness events):** Subtly, the visual weight shifts. The chair is the same; the way they sit in it has changed slightly. They are still the center, but the center is less steady. Other members are beginning to fill adjacent spaces.

**Late match (Severe illness window):** The Frost Elder is shown near the fire. A younger member sits close. The portrait has deepened in shadow. The visual language is of ending rather than leading. The other members' faces are turned toward the Elder in a way that reads as listening carefully while there is still time.

**After death:** The Frost Elder's chair remains. It is not removed. It will never be removed. The chair is the grave marker for match gameplay purposes. In the Keep's interior, no living member will sit in it. It is present as space that was occupied and will remain occupied in memory for the rest of the match. When a new player moves through the Keep interface after the Elder's death, they will see that chair, and if they have been watching the Elder since match start, they will feel what it means that it is empty.

---

*End of Creative Branch 002 — Bloodline Members*
*All content marked: PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON*
*Design Content — 2026-03-18, Creative Branch 002*
