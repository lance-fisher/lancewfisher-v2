# CREATIVE BRANCH 002 — POLITICAL EVENTS SPECIFICATION
**PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON**
*(Design Content — 2026-03-18, Creative Branch 002)*

---

## Overview

This document contains the full specification for the Bloodlines political events system. Events are emergent — they fire based on game state conditions that players create through their decisions over the course of a match. No two matches produce the same sequence. The 25+ events below span six categories: Faith, Dynastic, Economic, Military, Diplomatic, and World.

Events are not punishment dispensers. They are pressure systems that reward attentive play and punish neglect. Every event creates an actor and a reactor. The actor had choices that led here. The reactor has choices about what to do next. The world's history within the match is the engine that makes events feel earned rather than arbitrary.

---

## FAITH EVENTS

---

**[THE SCHISM]**
- Category: Faith
- Trigger condition: Two players share the same faith but have diverged to opposite ends of its doctrine path (e.g., both follow Ashkari, but one has built toward the Sacrificial branch and the other toward the Ascendant branch). Faith intensity for both must be Devout (40%) or higher. The divergence must persist for 15+ minutes of match time without either player moving toward reconciliation (no Faith Alliance formed, no shared faith buildings constructed cooperatively).
- Who receives it: Both diverged players receive the primary event notification. All other players who share that faith receive a secondary notification indicating schism is underway.
- Effect: A third doctrine path unlocks — the Splinter Path — available only to players inside the schism. The Splinter Path offers faster faith intensity gains but at the cost of permanent isolation from the parent faith's diplomatic bonuses. Both schism players receive a conviction pressure event: their faith-aligned population begins generating unrest proportional to how far faith intensity has climbed. If either player holds an ecclesiastical building, it generates 15% less faith intensity per structure until the schism resolves or hardens. A global diplomatic notification fires to all players indicating which faith has fractured. Neutral minor tribes whose faith alignment matches the parent faith become diplomatically destabilized — they will not enter agreements with either schism player unless one player renounces the Splinter Path.
- Duration: Ongoing until one of three resolution conditions: (a) one schism player is eliminated, (b) one player formally renounces their doctrine path (loses all doctrine-specific bonuses, returns to base faith intensity), or (c) both players form a Faith Alliance (suppresses the schism and unlocks the Reunification bonus — faith intensity gains +10% for both players for 20 minutes).
- Counter-play: The non-schism player can attempt to recruit both schism players against a common enemy by proposing a Military Alliance — the shared threat temporarily suppresses schism unrest. A third player who follows the same faith can position themselves as arbiter: if they have higher faith intensity than either schism player, they can propose mediation, which forces a Cordial diplomatic floor between the schism parties for 10 minutes.
- Conviction interaction: Moving into the Splinter Path costs 8 conviction points on first entry (the act of formal doctrinal separation is a moral statement). Renouncing the Splinter Path returns 4 of those points.
- Faith interaction: Core driver. Requires Devout+ faith intensity for both parties. Schism resolution via Reunification grants the fastest faith intensity boost available outside the Covenant Test.
- Frequency / repeatability: Once per faith per match globally. If the Ashkari faith schisms, it cannot schism again. A second doctrinal divergence between Ashkari players produces only minor diplomatic unrest, not a full Schism event.

---

**[HOLY PILGRIMAGE]**
- Category: Faith
- Trigger condition: Any player reaches Fervent faith intensity (60-80%) and has held that intensity for 10+ continuous minutes without entering a war state. The player's dynasty must have at least one Bloodline Member alive and at least one faith structure constructed.
- Who receives it: The trigger player receives the primary event. The Trueborn City receives a secondary notification (a Pilgrimage delegation is en route).
- Effect: A Pilgrimage caravan unit spawns at the trigger player's primary faith structure. The caravan is a non-combat unit that moves toward the Trueborn City along roads. While en route, all territories the caravan passes through receive a passive faith intensity boost of +5% per territory per minute of caravan presence — affecting any player who controls those territories regardless of faith alignment. If the caravan reaches Trueborn City, the trigger player receives: +15% faith intensity, a one-time Conviction bonus of +10, and a Diplomatic Reputation buff making the trigger player appear Cordial to all currently Neutral parties for 20 minutes. If the caravan is attacked or destroyed by any player, that player receives the same consequence as attacking a Trueborn Representative — automatic Trueborn Summons fires against the attacker, plus -20 conviction.
- Duration: One-time event. The caravan exists until it reaches Trueborn City or is destroyed. Rewards are applied at arrival.
- Counter-play: Enemies cannot attack the caravan without catastrophic diplomatic consequence, but they can close roads (destroy road structures) to slow or reroute the caravan. A competing player who is also Fervent or higher can launch their own Pilgrimage — if both caravans reach Trueborn City within the same 5-minute window, both players receive the faith intensity bonus but neither receives the Diplomatic Reputation buff (Trueborn cannot honor two pilgrims simultaneously).
- Conviction interaction: Launching a Pilgrimage is a Moral act — the trigger player gains +5 conviction on caravan spawn. Destroying a caravan is a major Cruel act.
- Faith interaction: Core driver. Apex faith players (80%+) cannot trigger a Pilgrimage — they have already transcended the need for external validation and have access to Apex mechanics that supersede this event's rewards.
- Frequency / repeatability: Once per player per match. A player who already completed a Pilgrimage cannot trigger another.

---

**[FAITH PERSECUTION]**
- Category: Faith
- Trigger condition: A player deliberately destroys or captures a faith structure belonging to a different faith (not merely a military building in a contested territory — the structure must be a dedicated faith building). Alternatively, fires if a player executes a Bloodline Member whose faith alignment differs from their own.
- Who receives it: All players who share the victimized faith receive the notification. The trigger player receives a private consequence notification.
- Effect: The trigger player is declared a Persecutor by the global diplomatic system. Every player who shares the victimized faith receives: +5% faith intensity, a passive combat bonus of +8% damage against the Persecutor's units, and the option to declare Holy War (see Holy War event) immediately without the standard conviction cost. Minor tribes of the victimized faith become permanently hostile to the Persecutor regardless of prior agreements. The Trueborn City issues a formal denunciation: all diplomatic agreements between the Persecutor and Trueborn-aligned minor factions are suspended for 15 minutes.
- Duration: Persecutor status persists for 20 minutes or until one of two conditions: (a) the Persecutor constructs a faith structure for the victimized faith in their own territory (a symbolic act of restitution, costs conviction to commission), or (b) the Persecutor surrenders a Bloodline Member to the victimized faith's player as hostage.
- Counter-play: The Persecutor can attempt to divide the responding coalition by offering Trade Agreements to the weaker members of the victimized-faith bloc. The Persecutor's own faith may rally to their defense if faith intensity is high enough — Fervent+ players of the Persecutor's faith receive a Mirror Holy War option to counter-declare.
- Conviction interaction: Each faith structure destroyed costs -12 conviction. Each Bloodline Member executed for faith costs -20 conviction. If the Persecutor is already in the Cruel band, the conviction costs are halved (cruelty normalized) but the diplomatic consequence is doubled (the world notices the pattern).
- Faith interaction: Victimized faith players receive intensity boost. Persecutor's faith population generates unrest if the persecution conflicts with their doctrine's stated values (doctrine-dependent).
- Frequency / repeatability: Can fire multiple times per match per player. There is no cap — a player who systematically destroys faith structures will accumulate compounding Persecutor status durations.

---

**[DIVINE FAVOR]**
- Category: Faith
- Trigger condition: A player completes three faith-aligned actions within a single 20-minute window without any interruption by Cruel conviction acts. Faith-aligned actions include: completing a faith structure, reaching a new faith intensity tier, completing a faith doctrine upgrade, or offering tribute to the Trueborn City in the name of their faith.
- Who receives it: The trigger player only.
- Effect: A Divine Favor state activates for 15 minutes. During this state: faith structures produce +20% intensity, Bloodline Members with faith alignment gain a passive +15% to all stats, and any Trade Agreement or Faith Alliance proposed by this player during the window carries a reputation bonus that makes neutral parties 30% more likely to accept. The event fires a visible world notification (all players see the notification that a dynasty has achieved Divine Favor) but does not reveal the favored player's position or territory. The notification creates natural diplomacy: other players will investigate who achieved it.
- Duration: 15 minutes from trigger, then expires. Cannot be renewed by triggering the conditions again within the same 30-minute window.
- Counter-play: There is no direct mechanic to cancel another player's Divine Favor. The indirect response is to accelerate your own faith actions to achieve it simultaneously, which does not cancel the first player's favor but does award your own (two players can hold Divine Favor simultaneously — this creates an interesting competitive dynamic as both race to capitalize on the window).
- Conviction interaction: Divine Favor cannot fire if the player is currently in the Cruel band. If a player achieves Neutral band and holds it for 10 minutes before the three faith actions complete, Divine Favor fires with a bonus — +5 additional minutes of duration.
- Faith interaction: Requires Active (20%+) faith intensity as a floor. The event is more powerful the higher the faith intensity at time of trigger — at Apex (80%+), the stat bonuses on Bloodline Members increase to +25%.
- Frequency / repeatability: Once per player per 45-minute match window. In long matches (4+ hours), this can fire twice per player.

---

**[THE HOLY WAR DECLARATION]**
- Category: Faith
- Trigger condition: A player in Active faith intensity or higher declares war on another player who shares their faith, OR a player explicitly announces Holy War through the diplomatic interface against a player of a different faith who has committed Faith Persecution against them (see Faith Persecution event). Holy War Declaration can also be triggered proactively by any Fervent+ player against a declared Persecutor.
- Who receives it: All players receive the global notification. Players of the declaring player's faith receive a secondary notification with the option to join the Holy War as co-belligerents.
- Effect: The declaring player enters a Holy War state. In this state: all military units receive +10% damage against the declared enemy, faith intensity cannot decrease for the duration (it is locked at current level), and any Bloodline Member who dies in combat against the declared enemy generates a Martyr bonus — +8% faith intensity added to the declaring player's total on each Bloodline Member death (enemy deaths, not friendly deaths). Co-belligerent players who join receive the same combat bonus but not the Martyr mechanic. Neutral players who share the declared enemy's faith receive pressure to respond — their minor faction allies will generate unrest if the neutral player does not intervene within 15 minutes.
- Duration: Holy War state persists until one of three conditions: (a) the declared enemy is eliminated, (b) the declaring player achieves their stated war objective (capturing a faith structure, reaching a territory threshold — declared at war initiation), or (c) the declaring player's faith intensity drops below Active (20%) due to prolonged war without faith maintenance.
- Counter-play: The target of a Holy War can seek a Counter-Faith Alliance — any player of their faith or a compatible faith who is currently Cordial or higher can offer a defensive Faith Alliance. A declared target who achieves Divine Favor during the Holy War receives temporary immunity to the Martyr mechanic (Bloodline Member deaths do not generate enemy intensity for 10 minutes).
- Conviction interaction: Declaring Holy War against a player of your own faith costs -15 conviction (fratricide). Declaring Holy War against a genuine Persecutor costs 0 conviction. If the declared enemy has a higher Moral conviction score than the declaring player, the Holy War declaration costs an additional -5 conviction — the world's moral accounting is precise.
- Faith interaction: Core mechanic. Faith intensity locks at declaration for the declaring player. If the Holy War is won decisively (enemy eliminated or objective achieved), faith intensity increases by one full tier.
- Frequency / repeatability: Once per player per match. Holy War is a statement of purpose, not a tactic to cycle.

---

**[FAITH REFORMATION]**
- Category: Faith
- Trigger condition: A player who previously reached Devout faith intensity (40%+) experiences a faith intensity collapse — dropping from Devout or higher to Latent (below 20%) within a 15-minute window. The collapse must be caused by game-state events (failed Covenant Test, extended war against a faith ally, conviction slide into Cruel band) rather than deliberate player abandonment of faith buildings.
- Who receives it: The trigger player receives the Reformation option. All players of the same faith receive a notification that a dynasty has entered spiritual crisis.
- Effect: The trigger player is offered a Reformation choice with a 5-minute decision window. Three paths: (a) Recant — formally abandon the faith, lose all doctrine bonuses but permanently remove faith-related event vulnerabilities. Conviction cost: -10. (b) Recommit — begin a 20-minute faith rebuilding sequence. Faith intensity gains are doubled but all diplomatic bonuses from faith are suspended during the rebuild. No conviction cost. (c) Heresy — formally declare against the faith's doctrine, creating a new splinter identity. Unlock unique Heretic units with unconventional ability sets. Faith intensity locks at current (collapsed) level but cannot decline further. Conviction cost: -15 but gain +10 in a different form — a Defiance bonus that adds +12% to all Bloodline Member stats for 10 minutes.
- Duration: The Reformation choice window is 5 minutes. After selection, effects are permanent for the match.
- Counter-play: Other players of the same faith can attempt to influence the reforming player's choice by sending diplomatic messages during the 5-minute window. A Faith Alliance proposal sent during the window increases the chance the reforming player selects Recommit (mechanical bias, not override — the player still chooses).
- Conviction interaction: Path-dependent as noted above. Heresy generates a strong conviction signal that affects all three Pillars — Dynasty sees the faith collapse and may trigger succession instability, Conviction records the defiance.
- Faith interaction: Core driver. Reformation only fires on collapse events, not deliberate abandonment.
- Frequency / repeatability: Once per player per match. A player who has already Reformed cannot Reform again — their spiritual identity is set.

---

## DYNASTIC EVENTS

---

**[SUCCESSION CRISIS]**
- Category: Dynastic
- Trigger condition: A Bloodline Member who is the current dynasty's designated Heir dies in combat, and no secondary Bloodline Member has been designated as heir, AND the dynasty has been in the match for 30+ minutes (time threshold prevents this from firing in early-game skirmishes where designation hasn't occurred yet).
- Who receives it: The trigger player receives the full event. All neighboring dynasties receive a notification that the dynasty is in succession crisis.
- Effect: The dynasty enters a 10-minute Interregnum state. During Interregnum: resource generation drops 15% across all categories, military unit production time increases 20%, and all diplomatic agreements become fragile — any player can attempt to break a Trade Agreement with the crisis dynasty without the normal diplomatic cost. The trigger player must designate a new Heir from surviving Bloodline Members within the 10-minute window. If no designation occurs, the crisis hardens into a Dynastic Fracture (see below variant). If designation occurs within the window, a Restoration bonus fires: +10% resource generation for 15 minutes, representing the dynasty's relief and re-stabilization. Neighboring dynasties who were Hostile may attempt opportunistic expansion — territorial border checks are relaxed during Interregnum.
- Duration: 10-minute active window. Resolves on designation or hardens to Dynastic Fracture.
- Dynastic Fracture variant: If no Heir is designated within 10 minutes, the dynasty's secondary military commander (highest-level non-Bloodline unit) attempts to seize power. The player retains control but loses all Bloodline Member bonuses for 20 minutes — units operate as generic commanders. The usurper unit gains a permanent +20% stat bonus but cannot grow through the Bloodline progression tree.
- Counter-play: Players with an Allied relationship to the crisis dynasty can offer a Bloodline Marriage during the Interregnum to shore up succession — this does not resolve the crisis mechanically but adds +1 available Bloodline Member candidate to the trigger player's pool.
- Conviction interaction: A Moral dynasty that loses their Heir in battle receives +5 conviction from the Martyr's Legacy — the Heir died fighting, not hiding. A Cruel dynasty that loses their Heir receives an additional -5 conviction — the dynasty's cruelty contributed to the Heir's death (narrative framing).
- Faith interaction: If the deceased Heir was faith-aligned at Devout+, the faith structure associated with their doctrine generates +10% intensity for 5 minutes as the faith community mourns (passive effect, no player action required).
- Frequency / repeatability: Can fire multiple times per player per match — every qualifying Heir death is a potential Succession Crisis.

---

**[THE BLOODLINE CAPTURE]**
- Category: Dynastic
- Trigger condition: A Bloodline Member from any dynasty is captured in combat (health reduced to zero in a scenario where the capturing player's military unit has a Capture-equipped unit present, and the downed Bloodline Member is in controlled territory at time of downing).
- Who receives it: The capturing player and the captured player both receive the event notification. All players with diplomatic relationships to either party receive a secondary notification.
- Effect: The captured Bloodline Member is held as a Hostage at the capturing player's hold structure. The hostage creates ongoing pressure: the captured player loses access to all abilities and progression of the captured Bloodline Member for the duration of captivity. The capturing player gains a Leverage state — they may use the hostage in one of four ways: (a) Ransom — demand resources (negotiated amount, capturing player sets terms, captured player can counter-offer). (b) Execution — kill the hostage. Grants +30% military production speed for 20 minutes but costs -25 conviction and triggers a permanent diplomatic reputation hit with all Cordial and Allied parties. (c) Trade — exchange the hostage for a diplomatic concession (transition from Hostile to Neutral, from Neutral to Cordial). (d) Retain — keep the hostage indefinitely as ongoing leverage (the captured player's diplomatic behavior becomes constrained — they cannot declare war on the capturing player while their Bloodline Member is held, without executing their own hostage first, which costs conviction). Rescue missions: the captured player can attempt a covert military operation to free the hostage — a small elite unit assault on the hold structure. Success frequency is based on unit composition and territorial control, not random.
- Duration: Hostage state persists until one of the four resolution paths is completed, or until the capturing player's hold structure is destroyed (automatic release).
- Counter-play: Rescue mission as described. The captured player can also use diplomacy — offering another player an incentive to pressure the captor into release.
- Conviction interaction: Execution is the most significant conviction loss in the game short of genocide. Ransom and Trade are conviction-neutral. Retaining a hostage indefinitely while making no move toward resolution generates slow Cruel drift (-2 conviction per 10 minutes of unresolved captivity).
- Faith interaction: If the captured Bloodline Member is the dynasty's primary faith figure (the member who has been performing faith actions), their captivity halts all faith intensity gains for the capturing player's faith system — the faith community will not honor a faith that imprisons representatives of other faiths without resolution.
- Frequency / repeatability: Can fire multiple times per match per player — each Bloodline Member capture is a separate event.

---

**[THE MARRIAGE PROPOSITION]**
- Category: Dynastic
- Trigger condition: Two dynasties have been at Cordial diplomatic status for 20+ continuous minutes, and both have at least one eligible Bloodline Member (defined as a Bloodline Member who is not currently designated as primary Heir, not currently captured, and not already in a prior Marriage Agreement).
- Who receives it: Both dynasties receive the event notification simultaneously.
- Effect: The Marriage Proposition offers a formal Marriage Agreement with the following terms: both parties designate one Bloodline Member each. The Bloodline Members enter a paired state — they cannot be assigned to the same military operation (they are not deployed together as a safety measure). In exchange: both dynasties receive a Bloodline Merge bonus — each player gains access to one passive ability from the other player's Bloodline progression tree. The merged ability is the highest-tier ability the partner has already unlocked. Additionally, both dynasties receive a Diplomatic Stability bonus — transitions from Cordial to Hostile now require a 10-minute cooling-off period (immediate escalation is blocked). If either dynasty later breaks the Marriage Agreement (by declaring war on the partner), the breaking dynasty loses both Bloodline Members from the paired state — not to death, but to a 20-minute estrangement period during which the Bloodline Members are non-functional.
- Duration: Marriage Agreements are permanent until broken. They survive match stage transitions.
- Counter-play: A third player who opposes the alliance can attempt to manufacture a pretense for conflict between the two parties — forcing a diplomatic incident that makes marriage politically untenable. This requires active manipulation (attacking border territories to provoke misattribution) and is high-risk if discovered.
- Conviction interaction: Proposing and accepting a Marriage is a Moral act (+4 conviction each). Breaking a Marriage Agreement for war is a Cruel act (-18 conviction) — the world regards it as a betrayal of sacred covenant.
- Faith interaction: If both dynasties share the same faith, the Marriage Agreement generates a joint faith intensity bonus (+8% for both) as the faiths recognize the union as doctrinally significant. If the faiths differ, no faith bonus fires — but no faith penalty fires either. Mixed-faith marriages are politically neutral.
- Frequency / repeatability: One active Marriage Agreement per dynasty at any time. A dynasty can form sequential marriages if a prior agreement dissolves (through breach or partner elimination), but not simultaneous ones.

---

**[HEIR EMERGENCE]**
- Category: Dynastic
- Trigger condition: A dynasty's Bloodline Member who has been designated as Heir reaches Level 3 progression milestone while the current dynasty leader (primary Bloodline Member) is still alive and active at Level 4+ progression. The generational gap creates tension: a powerful heir and a powerful leader in the same dynasty.
- Who receives it: The trigger player receives the event. All players with Cordial or Allied diplomatic relationships to the trigger dynasty receive a secondary notification.
- Effect: The dynasty enters a Generational Tension state. The trigger player must choose within 8 minutes: (a) Crown the Heir — the Heir becomes the dynasty's primary leader. The current leader becomes an Elder Advisor, retaining all abilities but losing primary command of military units. The Heir gains +15% to all stats immediately (the surge of new authority) but the Elder Advisor generates ongoing tension (-5% resource generation for 15 minutes while the transition settles). (b) Suppress the Heir — the current leader retains primacy. The Heir's progression is capped at Level 3 until the leader dies or is incapacitated. Suppression costs -8 conviction (the dynasty is denying its natural succession) but prevents the immediate disruption. (c) Declare Co-Regency — both operate as equals. Combined stat bonuses are 80% of what each would receive individually, but the dynasty gains access to dual-command military operations (one Bloodline Member can command a secondary military force simultaneously with the primary force, without the normal single-command limit).
- Duration: The 8-minute decision window is hard. After 8 minutes without choice, Suppression is automatically applied (the elder holds power). Effects of the chosen path persist for the remainder of the match.
- Counter-play: Rivals who observe the notification can attempt to create a crisis during the 8-minute window to force hasty decision-making. Allies can offer support for the Heir Emergence (pledging diplomatic recognition of the new leader) which biases the trigger player toward Crowning.
- Conviction interaction: Suppression costs conviction. Crowning is conviction-neutral. Co-Regency is a Moral act (+6 conviction) — sharing power is recognized as generous governance.
- Faith interaction: If the Heir has higher faith intensity alignment than the current leader, Crowning generates a faith intensity bonus (+10%) as the faith community welcomes a more devout ruler. If the Heir has lower faith alignment, the faith community generates 5% unrest on Crowning.
- Frequency / repeatability: Once per dynasty per match. Heir Emergence represents a singular generational moment.

---

**[DYNASTIC RIVALRY]**
- Category: Dynastic
- Trigger condition: Two dynasties have been in a War or Hostile diplomatic state for 30+ continuous minutes without either achieving decisive military advantage (neither has eliminated a Bloodline Member from the other, neither has taken more than 2 territories from the other during this period). The prolonged standoff creates institutional rivalry.
- Who receives it: Both rival dynasties receive the event. Minor tribes in contested border territories receive a secondary notification.
- Effect: The Dynastic Rivalry state formalizes the conflict's narrative. Mechanically: both dynasties receive a Rivalry Bonus — +12% to all military unit stats when fighting each other specifically (not against other players, only the rival). This represents the adaptive learning that comes from prolonged conflict against a known enemy. Minor tribes in contested territories become unreliable for both parties — their loyalty can shift based on which rival most recently performed a positive action in their territory (bringing resources, completing faith acts, stationing protective units). The Rivalry also creates a Prestige mechanic: the first rival to eliminate a Bloodline Member from the other during the Rivalry state receives a global prestige notification and a +10% diplomatic reputation boost with all Neutral parties (the world respects the decisive actor in a known conflict).
- Duration: Rivalry state persists until one party is eliminated, or both parties agree to a formal Peace (transitioning to Neutral state via diplomatic interface). A Peace signed during Rivalry grants both parties a +10% resource generation bonus for 20 minutes — the end of a cold war releases productive capacity.
- Counter-play: A third party can attempt to break the Rivalry by offering both parties a common threat (proposing a trilateral conflict elsewhere). If both rivals accept an external Military Alliance against a common enemy, the Rivalry state suspends (bonuses freeze but do not disappear) until the common threat is resolved.
- Conviction interaction: Dynastic Rivalry is conviction-neutral in itself. Actions taken within the Rivalry context carry their normal conviction weights. A Moral player who achieves the Prestige moment by defending a minor tribe (rather than striking the rival directly) receives +5 conviction bonus on the Prestige notification.
- Faith interaction: If both rivals share the same faith, the Rivalry generates ongoing faith community unrest — the faith's minor tribe allies find the intra-faith war troubling. Faith intensity for both parties loses 3% every 10 minutes while Rivalry persists between same-faith dynasties.
- Frequency / repeatability: One active Dynastic Rivalry per dynasty at any time. A dynasty cannot be in simultaneous declared Rivalries with two parties.

---

## ECONOMIC EVENTS

---

**[TRADE ROUTE COLLAPSE]**
- Category: Economic
- Trigger condition: An active Trade Route Establishment (one of the seven agreement types) is severed because the territory through which the route passes is captured by a third party, or the originating or receiving trade building is destroyed. Alternatively, fires if a player with 3+ active trade routes simultaneously loses 2 of them within a 5-minute window.
- Who receives it: Both parties in the severed Trade Route receive the primary event. The player who caused the severance (if a third party) receives a secondary notification indicating they have triggered economic disruption.
- Effect: Both trade route parties experience an immediate resource generation drop of 20% across all traded resource types for 10 minutes. The route's specific traded resources drop 35% during that same window. A Trade Collapse Notification fires to all players indicating the regional economy has destabilized — this is an opportunity signal for trade-oriented players to offer replacement agreements. The Trueborn City registers the collapse and briefly increases its own trade volume — Trueborn-allied minor factions become more willing to enter new Trade Agreements (5-minute window of elevated trade receptivity).
- Duration: The 20%/35% penalty runs for 10 minutes. After that, resource generation normalizes to base (but the lost trade bonuses from the severed route remain absent until a new route is established).
- Counter-play: Both affected parties can independently pursue replacement trade routes. A player who caused the collapse (intentionally by capturing the route territory) should expect the two affected parties to seek each other and form a defensive economic alliance.
- Conviction interaction: Deliberately destroying a trade building to collapse a rival's route costs -8 conviction (economic sabotage). Losing a route to military action is conviction-neutral.
- Faith interaction: If both trade route parties share the same faith, the faith community mourns the economic disruption — faith intensity for both players drops 5% as faith structures lose patronage income.
- Frequency / repeatability: Fires each time a qualifying collapse occurs. No cap.

---

**[RESOURCE SHORTAGE]**
- Category: Economic
- Trigger condition: Any player falls below 100 units of two or more of the six primary resources simultaneously for 10+ continuous minutes. This represents genuine economic crisis, not momentary expenditure.
- Who receives it: The trigger player receives the event. Players who are Cordial or Allied with the trigger player receive a secondary notification that the dynasty is struggling.
- Effect: The trigger player enters Scarcity State. In Scarcity: unit production halts for any unit requiring the deficient resources, population growth stops, and existing population begins generating Hunger Unrest — a pressure that, if left unresolved for another 10 minutes, triggers a Population Revolt (see military events). The event also fires a market opportunity for other players: all players can offer Emergency Trade (a one-time resource transfer without a formal Trade Agreement) to the struggling dynasty. Emergency Trade costs the giving player nothing diplomatically but earns +5 Diplomatic Reputation with the receiver and moves the diplomatic state one step toward Cordial if currently Neutral. Players who are Hostile cannot offer Emergency Trade without first moving to Neutral.
- Duration: Scarcity State persists until both deficient resources rise above 200 units simultaneously. Hunger Unrest persists for 5 minutes after Scarcity resolves (lingering community memory of the shortage).
- Counter-play: The trigger player can prioritize resource recovery through aggressive expansion (capturing resource-rich territories), Emergency Trade requests, or sacrificing population for resource infusion (if their faith doctrine supports population sacrifice for resource gain). Rivals can exploit Scarcity by timing military pressure to coincide with the production halt.
- Conviction interaction: Deliberately starving a territory you control (neglecting resource distribution to manage population) is tracked — if Scarcity is detected in a Cruel dynasty, the conviction cost doubles for the unrest that follows.
- Faith interaction: If the shortage affects food or water, faith structures in affected territories generate 10% less intensity (the community cannot support faith practice when survival is threatened). Faith-related resource production (faith structures sometimes produce resource equivalents based on doctrine) is suspended during Scarcity.
- Frequency / repeatability: Can fire multiple times per player per match — each qualifying scarcity period generates the event independently.

---

**[THE CURRENCY WAR]**
- Category: Economic
- Trigger condition: Two players who both have active Trade Agreements with a third party (the "neutral hub") begin competing for the neutral hub's exclusive trade relationship — detected when both players have offered Trade Route Establishment to the same minor tribe or neutral faction within the same 10-minute window, and the minor faction can only accept one.
- Who receives it: Both competing players and the neutral hub (minor tribe or neutral faction) receive the event. The Trueborn City receives a notification.
- Effect: The neutral hub enters a Bidding State for 8 minutes. During bidding: both competing players can escalate their trade offers by committing resources from their own stockpile (minimum bid escalation: 50 gold per round). The minor faction's acceptance threshold rises 10% with each competing escalation. At the end of 8 minutes, the highest committing player wins the exclusive trade relationship. The losing player does not receive their committed resources back but receives a Consolation Relationship — the minor tribe remains Neutral (rather than becoming hostile). The winning player receives the trade route plus a Market Dominance bonus: +8% gold generation for 15 minutes, representing the economic confidence of a contested victory. The Trueborn City observes: if the bid escalation exceeded 500 total gold committed, Trueborn issues an Economic Interest declaration — they will send a trade delegation to the winning player's territory within 10 minutes (a valuable diplomatic opportunity).
- Duration: 8-minute bidding window. Trade route and bonus effects are permanent until route collapse.
- Counter-play: A third player who is uninvolved can attempt to poach the minor tribe entirely by offering Military Protection (a non-trade offer) during the bidding window. Military Protection from a third party makes the minor tribe Cordial to that player and suspicious of both bidders — the tribe may withdraw from bidding entirely, leaving both bidders without the route.
- Conviction interaction: Currency War is conviction-neutral in itself. Bid escalation tactics that involve resource deception (promising resources you do not have and defaulting) cost -10 conviction when the default is detected.
- Faith interaction: If the minor tribe has faith alignment, the highest-bidding player's faith alignment matters — a bid from a player of the same faith as the tribe receives a 20% discount on bid escalation (the tribe values faith commonality and requires less gold to be persuaded).
- Frequency / repeatability: Once per minor tribe per match. A tribe that has already been through a Currency War will not accept a second bidding process — their relationship with the winner is locked.

---

**[MARKET MANIPULATION]**
- Category: Economic
- Trigger condition: A player with 4+ active Trade Agreements simultaneously requests renegotiation of all agreements within a 5-minute window — attempting to change terms of existing agreements in their favor. This represents a player leveraging market position to extract value.
- Who receives it: All affected trade partners receive the renegotiation request.
- Effect: Each trade partner must independently decide to Accept, Counter-Offer, or Refuse within 5 minutes. Refusals do not immediately break the trade agreement but move the relationship one step toward Hostile diplomatically. If two or more partners refuse within the same window, a Trade Bloc forms — the refusing partners automatically become Cordial to each other (bypassing the normal relationship-building process) and share economic intelligence (each bloc member can see the manipulating player's resource totals). The Trade Bloc persists for 20 minutes and creates a diplomatic pressure entity — the manipulating player's agreements with the bloc members are all flagged as Fragile until one of the bloc members' normal cordial threshold is achieved through positive actions. If all renegotiations succeed, the manipulating player receives The Monopolist title — +15% gold generation but a permanent -10% diplomatic receptivity from all Neutral parties for the rest of the match (the reputation of a manipulator follows).
- Duration: 5-minute decision window. Trade Bloc lasts 20 minutes. Monopolist title is permanent.
- Counter-play: The manipulating player can attempt to divide the Trade Bloc by offering preferential terms to the strongest bloc member privately — a visible side deal that may fracture bloc solidarity (the receiving member may accept and leave the bloc, reducing its power).
- Conviction interaction: Market Manipulation is a mild Cruel act (-5 conviction per successful forced renegotiation). If all partners accept without resistance, no additional conviction cost (they agreed). If partners were coerced through military positioning during the negotiation window, the conviction cost doubles.
- Faith interaction: Players of faith doctrines that emphasize community and sharing (doctrine-dependent) receive a faith intensity penalty of 8% for Market Manipulation — their faith communities view the behavior as contrary to doctrine.
- Frequency / repeatability: Once per player per match. The Monopolist reputation, once earned, is not escapable through repetition.

---

## MILITARY EVENTS

---

**[VETERAN ARMY REVOLT]**
- Category: Military
- Trigger condition: A player's military forces have been continuously deployed in War state for 40+ minutes without returning to a garrison or rest state, AND the player's conviction is in the Cruel band, AND food resources have been below 150 units for 15+ minutes. All three conditions must be simultaneously true.
- Who receives it: The trigger player receives the event. No external notification — revolt is an internal affair.
- Effect: 30% of the trigger player's non-Bloodline military units enter a Mutiny State. Mutinous units stop accepting player commands but do not immediately attack — they form a stationary force that demands resolution within 8 minutes. The player has three resolution paths: (a) Pay the Troops — commit 200 gold immediately. Units return to service. Conviction: neutral. (b) Purge the Mutiny — send Bloodline Members to suppress. Suppression costs 10% of the Bloodline Member's current health. Units are disbanded (removed from the field). Conviction: -10. (c) Negotiate — reduce conviction to Neutral band (if possible within 8 minutes through faith actions or moral acts) and the units return automatically. If no resolution occurs within 8 minutes, the mutinous units defect to the nearest Neutral minor tribe, joining that tribe's defensive force — they fight against the trigger player if attacked.
- Duration: 8-minute resolution window. After resolution (or defection), event closes.
- Counter-play: Rivals who observe prolonged war without food production can pre-position to capitalize on the defection outcome — capturing the minor tribe the defectors join, or recruiting those defectors through a diplomatic overture to the minor tribe.
- Conviction interaction: All three trigger conditions include conviction state. Resolution paths have conviction costs as described. The event itself does not add or subtract conviction — it is the consequence of prior conviction behavior.
- Faith interaction: If the mutinous units are faith-aligned (they have been used to fight a Holy War), their mutiny generates a faith community signal — faith intensity drops 5% as the community sees their fighters demoralized.
- Frequency / repeatability: Once per conviction descent into Cruel per match. A player who resolves a revolt and slides back into Cruel can trigger it again — but the second revolt requires 50% of the army to mutiny (escalating instability).

---

**[GARRISON DEFECTION]**
- Category: Military
- Trigger condition: A player captures a territory from another player and places garrison units there, but does not perform any positive stability action (constructing a building, stationing a Bloodline Member, or initiating trade with the territory's resident population) within 20 minutes of capture. The conquered population becomes restless.
- Who receives it: The capturing player receives the event notification. The original territory owner receives a notification that resistance is forming in the lost territory.
- Effect: 25% of the garrison unit strength defects to a local Resistance cell. The Resistance cell is a neutral mini-faction that occupies a corner of the captured territory — not controlled by the original owner, not controlled by the capturing player. The Resistance will harass garrison units (periodic 8% damage ticks on adjacent garrison forces) but cannot capture territory itself. The original territory owner can attempt to re-ignite the Resistance into a full rebellion by making contact (sending a diplomatic unit or Bloodline Member to the Resistance cell) — this converts the Resistance from neutral harasser to active ally, turning the defectors into a recapture force under the original owner's partial command. The capturing player can eliminate the Resistance cell by stationing a Bloodline Member in the territory for 10 minutes (presence stabilizes the population).
- Duration: Resistance cell persists until eliminated or converted to active alliance. Harassment continues throughout.
- Counter-play: The capturing player's most effective response is speed — performing a stability action within the 20-minute window prevents the event from firing entirely. Prevention is the intended design lesson: conquest without governance creates self-inflicted military burden.
- Conviction interaction: If the capturing player is in the Moral band and performs a stability action immediately (within 5 minutes of capture), they receive a Benevolent Conqueror bonus — +8% to resource generation in the new territory for 20 minutes (the population responds positively to respectful governance). Ignoring the territory and letting the Resistance form costs -5 conviction (neglect is a form of cruelty).
- Faith interaction: If the captured territory has a faith structure of a different faith than the capturing player, and the capturing player destroys it within the 20-minute window (before performing stability actions), the Garrison Defection event fires immediately without the 20-minute delay — destruction of faith is an accelerant.
- Frequency / repeatability: Fires for each qualifying captured territory. No cap — a player who captures many territories rapidly and governs none of them will face multiple simultaneous Resistance cells.

---

**[SIEGE ATTRITION]**
- Category: Military
- Trigger condition: A siege operation (military units stationed adjacent to an enemy fortification without breaching it) has persisted for 20+ minutes without either side achieving resolution. The stalemate creates resource and morale pressure.
- Who receives it: Both the besieging player and the besieged player receive the event simultaneously.
- Effect: For the besieging player: food and water consumption rates for siege units increase 25% (extended campaigns are expensive). Gold generation drops 10% (military focus crowds out economic attention). For the besieged player: food and water generation inside the besieged territory drops 30% (supply lines are cut). However, the besieged player receives a Defender's Resolve bonus — all garrison and defense units in the besieged territory gain +15% combat stats (the morale effect of holding under pressure). The event also fires a global notification that a major siege is underway — neutral minor tribes may send Mercenary Availability offers to both sides (see Mercenary Availability event). After 10 additional minutes of unresolved siege (30 total), a Secondary Siege Attrition fires: both sides lose 5% of their total food stores per 5 minutes until the siege resolves.
- Duration: Event is ongoing as long as the siege continues. Effects stack with the secondary attrition trigger.
- Counter-play: The besieging player's best response to attrition is breakthrough — committing Bloodline Members to a direct assault rather than waiting. The besieged player's best response is calling for allied relief — an Allied player who breaks through the siege line from outside receives the Siege Breaker title (a diplomatic prestige bonus that moves all Neutral parties one step toward Cordial with the siege breaker for 15 minutes).
- Conviction interaction: Siege Attrition is conviction-neutral. The subsequent actions taken during siege (executing prisoners, burning supply routes) carry their normal conviction weights.
- Faith interaction: If the besieged fortification contains a faith structure and the besieging player is of a different faith, the faith community of the besieged player generates a Holy War call — a passive 5% faith intensity gain per 10 minutes of siege for the besieged player, representing spiritual rallying under threat.
- Frequency / repeatability: One active Siege Attrition event per siege operation. Can fire multiple times if multiple sieges are underway simultaneously.

---

**[MERCENARY AVAILABILITY]**
- Category: Military
- Trigger condition: Any active war state has persisted for 25+ minutes globally (any war between any two players, not necessarily involving the player receiving the event). The extended conflict creates demand for independent military contractors. Alternatively, fires immediately when a Siege Attrition event reaches 20-minute threshold.
- Who receives it: All players not currently in an active war state receive the availability notification. Players in war states receive it 5 minutes later (they are preoccupied).
- Effect: A Mercenary Company spawns at a designated neutral location (map-edge, not near any player's starting territory). The Mercenary Company is a purchasable military unit group — 8 standard units and one elite commander — available for gold. The Company fights for whoever hires them first (first come, first served). Cost scales with match duration: base cost 150 gold in the first hour, +50 gold per additional 30-minute increment. Once hired, the Company operates for 20 minutes under player control before disbanding (they are not permanent). If no player hires the Company within 10 minutes of availability, the Company attacks the most recently contested territory (the territory that changed hands most recently in the match), acting as a chaotic third force.
- Duration: 10-minute hire window. 20-minute service period once hired. Event is one-time per trigger condition.
- Counter-play: Multiple players can bid for the Mercenary Company simultaneously — the first player to send gold to the Mercenary location wins. A player who suspects a rival will hire the Company can attempt to intercept the mercenaries' path with military units to delay their deployment.
- Conviction interaction: Hiring mercenaries to do your fighting is conviction-neutral. Using mercenaries to commit atrocities (attacking civilian structures, executing prisoners) carries the normal conviction cost — the mercenaries are your proxy, and their actions are your responsibility.
- Faith interaction: If the Mercenary Company's elite commander has a visible faith alignment (procedurally assigned at spawn), hiring a commander whose faith conflicts with your own faith doctrine at Fervent+ costs 5% faith intensity — the faith community disapproves of employing contradictory-faith warriors.
- Frequency / repeatability: Fires once per qualifying trigger condition per match phase. Maximum three Mercenary Availability events per match (one per major match stage).

---

**[CONQUERED TERRITORY RISING]**
- Category: Military
- Trigger condition: A player who has captured 5+ territories from a single enemy dynasty (not scattered captures from multiple enemies, specifically concentrated conquest from one source) has held those territories for 30+ minutes without the conquered dynasty being eliminated. The surviving enemy dynasty's population in captured territories reaches a tipping point.
- Who receives it: The conquering player receives the primary event. The surviving conquered dynasty receives a notification that their people are rising.
- Effect: The conquered dynasty receives a Liberation Movement — a passive military force that spawns in the most recently captured territory. The Liberation force is proportional to the number of territories held by the conquering player (1 unit per captured territory, up to 8 units). The Liberation force moves toward the nearest territory the conquered dynasty still controls (trying to return home). Along the way, it frees captured territories: if the Liberation force passes through a captured territory with only garrison units (no Bloodline Member present), it has a 40% chance of reclaiming the territory for the conquered dynasty automatically (representing a popular uprising, not the Liberation force's military superiority). A Bloodline Member presence in the territory drops the reclaim chance to 5%. All players receive a global notification that a Liberation Movement has formed — it is visible on the map.
- Duration: The Liberation force persists for 20 minutes or until it is destroyed or reaches conquered dynasty territory.
- Counter-play: The conquering player's response is to station Bloodline Members in threatened territories (removes reclaim vulnerability) and to pursue the elimination of the conquered dynasty before the Liberation force becomes a rallying symbol. A third player who assists the Liberation force receives +8 Diplomatic Reputation with the conquered dynasty and all their Allied parties.
- Conviction interaction: A Moral conquering player who constructs governance buildings in conquered territories (demonstrating intent to integrate rather than exploit) reduces the Liberation chance to 15% per territory and prevents the event from firing entirely if buildings are present in all 5 territories. Cruel conquering players who have been extracting conquered territory without governance receive the full force of the event.
- Faith interaction: If the Liberation force passes through a territory with a faith structure of the conquered dynasty's faith, it gains +15% combat stats — the faith becomes a rallying force for the liberation.
- Frequency / repeatability: Once per conquering dynasty per pair (one Liberation event per 5+ territory capture from the same dynasty). If the conquering player captures 10 territories from the same enemy, a second Liberation Movement spawns at the 10-territory threshold.

---

## DIPLOMATIC EVENTS

---

**[ALLIANCE BETRAYAL]** *(Settled Canon — Full Specification)*
- Category: Diplomatic
- Trigger condition: A player moves their diplomatic relationship from Allied to War in a single step without an intermediate Hostile period, OR breaks a Non-Aggression Pact by attacking a party they have a NAP with, OR uses diplomatic cover (maintains Cordial or Allied status) while actively positioning military forces for an attack that is later executed. All three forms are recognized as Alliance Betrayal by the game system.
- Who receives it: The betrayed player receives the primary event notification. All players receive a global notification indicating which dynasty has committed Alliance Betrayal.
- Effect: The betraying player receives: -25 conviction immediately, a permanent Betrayer Reputation flag visible to all players in diplomatic interfaces (replaced with the Rehabilitated flag after 30 minutes of Moral-band conviction acts), and a diplomatic receptivity penalty — all players are 40% less likely to accept any new agreement with the betraying player for 20 minutes. The betrayed player receives: automatic Cordial diplomatic status with all Neutral parties for 15 minutes (sympathy diplomacy), a 15% military defense bonus for 10 minutes (the world's acknowledgment of their vulnerability), and the right to immediately declare war without any diplomatic cost. Minor tribes who observed the betrayal (in territories adjacent to the conflict) become permanently Hostile to the betraying dynasty.
- Duration: Conviction hit is permanent. Betrayer Reputation flag lasts 30 minutes. Defense bonus and sympathy diplomacy last 15 and 20 minutes respectively.
- Counter-play: The betraying player's only recovery path is consistent Moral-band behavior for 30 minutes — aggressive diplomatic outreach, non-aggressive actions, completing faith acts. There is no shortcut to Rehabilitated status. The diplomatic cost of betrayal is meant to be genuinely painful.
- Conviction interaction: Core mechanic as described. The -25 conviction is the single largest one-time conviction cost available to a player through a diplomatic action.
- Faith interaction: Faith communities of the betraying player's faith generate unrest — betrayal is contrary to most faith doctrines' stated values. Faith intensity drops 8% on betrayal.
- Frequency / repeatability: Can fire for each qualifying betrayal act. A player who commits Alliance Betrayal twice receives compounding reputation damage — the second Betrayer flag reduces diplomatic receptivity by 60% and adds an additional 10-minute duration.

---

**[EMBASSY INSULT]**
- Category: Diplomatic
- Trigger condition: A player who has an active diplomatic representative in another dynasty's territory (via a Diplomatic Mission, one of the seven agreement infrastructure structures) issues a public diplomatic declaration that is hostile, dismissive, or accusatory toward the host dynasty. This includes: refusing a public arbitration request at Trueborn City, publicly rejecting a Marriage Proposition in a hostile manner, or sending a military unit into the diplomatic zone around an embassy building.
- Who receives it: The insulted dynasty receives the primary event. All players receive a secondary notification of the diplomatic incident.
- Effect: The insulted dynasty may issue a formal Protest within 5 minutes. If a Protest is issued, the insulting dynasty's diplomatic representative is expelled — removing any infrastructure bonuses the Diplomatic Mission provided. The insulting dynasty receives a Diplomatic Contempt flag — all players in Neutral or Cordial state move one step toward Hostile automatically (they interpret the public insult as a sign of broader disrespect). If no Protest is issued within 5 minutes (the insulted dynasty swallows the insult), the insulting dynasty's conviction drops -5 for the insult but the insulted dynasty's conviction gains +4 (moral credit for restraint). Minor tribes who witnessed the incident become 20% more difficult to diplomatically recruit for the insulting dynasty for 15 minutes.
- Duration: Contempt flag lasts 15 minutes. Relationship shifts are permanent unless recovered through positive diplomatic acts.
- Counter-play: The insulting player can issue a formal Apology through the diplomatic interface — this costs 100 gold as a gesture of sincerity and moves the insulted dynasty's diplomatic state back one step toward its prior position. If the Apology is accepted, the Contempt flag is lifted early. If rejected, the flag remains and the gold is lost (a genuine risk to the apologizing player).
- Conviction interaction: The insult costs -5 conviction for the insulter. Restraint earns +4 conviction for the insulted party. Formal Apology with Acceptance earns +6 conviction for the insulting player.
- Faith interaction: If both dynasties share the same faith, the insult generates additional faith community unrest — intra-faith disrespect is theologically problematic. Faith intensity drops 5% for the insulting player.
- Frequency / repeatability: Can fire multiple times per relationship pair per match. A dynasty that has committed Embassy Insult twice against the same target cannot issue an Apology — the second offense is beyond repair.

---

**[NEUTRAL PARTY APPEAL]**
- Category: Diplomatic
- Trigger condition: Any player who has been in a War state for 20+ minutes without making material progress (no territories captured, no Bloodline Members eliminated) may issue a Ceasefire Appeal to the Trueborn City, requesting neutral arbitration. Alternatively, the Trueborn City may issue an unsolicited Arbitration Demand if two players have been in war for 40+ minutes and the war has generated significant civilian disruption (multiple territory captures and recaptures of the same territories, indicating a grinding stalemate).
- Who receives it: Both warring parties receive the Appeal or Demand. All players receive a global notification that arbitration is underway.
- Effect: Both parties enter a 10-minute negotiation window. During this window, military actions are not halted (there is no forced ceasefire) but attacking during arbitration costs -8 conviction per military engagement initiated. The Trueborn City proposes terms based on the current territorial state — the party with more territory at arbitration initiation is asked to yield one territory as a peace gesture. The other party is asked to pay a resource tribute (50 gold, 50 food). If both parties accept, they enter a Mediated Peace — they cannot re-enter War state for 15 minutes. If either party refuses, the refusing party receives the Intransigent flag — -15% diplomatic receptivity from all Neutral parties for 10 minutes (the world sees who is choosing to fight).
- Duration: 10-minute negotiation window. Mediated Peace lasts 15 minutes. Intransigent flag lasts 10 minutes.
- Counter-play: A warring player who is winning the war has no incentive to accept arbitration — they should refuse. The Intransigent flag is an acceptable cost if the military campaign is progressing. The design intention is that arbitration is genuinely valuable to losing players and genuinely useless to winning players, which makes it an accurate signal of relative military position.
- Conviction interaction: Accepting arbitration and honoring the peace is a Moral act (+6 conviction for both parties). Accepting and then attacking within the 15-minute Mediated Peace window is an Alliance Betrayal (with all corresponding consequences).
- Faith interaction: If one of the warring parties has Fervent or higher faith intensity, their faith community generates pressure for peace — faith intensity drops 3% per 10 minutes of continued war during arbitration (the faith community is watching and is troubled by the dynasty's rejection of peace).
- Frequency / repeatability: Once per war pair per conflict instance. A new war between the same parties generates a fresh eligibility window.

---

**[REFUGEE CRISIS]**
- Category: Diplomatic
- Trigger condition: A player's territory is captured or destroyed at a sufficient scale to displace a significant portion of their simulated population — specifically, if 3+ territories are captured within a 15-minute window. The dispossessed population flows toward the nearest stable dynasty.
- Who receives it: The player who has lost territory receives the displacement event. The player(s) whose territory the refugees are flowing toward receive the refugee arrival notification.
- Effect: A Refugee Column spawns at the border of the losing player's remaining territory, moving toward the nearest stable dynasty (highest resource generation and lowest conflict activity). The receiving player must make a decision within 8 minutes: (a) Accept Refugees — the population integrates into the receiving territory. +10% food and water consumption for 15 minutes (feeding the displaced) but +8% population growth rate afterward. The displaced dynasty becomes Cordial to the accepting player permanently (baseline — diplomatic actions can change this further). (b) Reject Refugees — the Refugee Column is turned away. The rejected population becomes a Minor Tribe — a neutral, homeless faction that will aggressively contest unoccupied resource territories. The rejecting dynasty receives -12 conviction and a Cruel Reputation flag visible to all Cordial and Allied parties for 10 minutes. (c) Exploit Refugees — offer them settlement in exchange for labor conscription. The population integrates with +15% resource generation for 10 minutes but the accepting player receives -8 conviction and the displaced dynasty's diplomatic attitude toward the accepting player moves to Hostile (they recognize the exploitation).
- Duration: 8-minute decision window. Integration or exploitation effects last 15 minutes. Homeless Minor Tribe is permanent until absorbed or eliminated.
- Counter-play: The displaced dynasty can attempt to recapture their lost territories before the Refugee Column reaches another player — if the territories are retaken, the Refugee Column turns back and the event resolves without reaching the decision window.
- Conviction interaction: Path-dependent as described above. Accepting refugees is the only major diplomatic event where a Moral act carries a real short-term cost (increased consumption) — the design is intentional. Morality should have real costs, not just benefits.
- Faith interaction: If the refugees share the accepting player's faith, Accept Refugees grants +8% faith intensity (the community welcomes its own). If the faith differs, Accept Refugees is faith-neutral. Exploit Refugees always costs 5% faith intensity regardless of shared faith — the faith community does not approve of exploiting the vulnerable.
- Frequency / repeatability: Can fire multiple times per match — each qualifying displacement event (3+ territories lost in 15 minutes) generates a Refugee Column.

---

**[TRADE WAR DECLARATION]**
- Category: Diplomatic
- Trigger condition: A player who has 3+ active Trade Agreements unilaterally cancels all of them simultaneously and publicly announces trade sanctions against a specific named target dynasty. Alternatively, fires if a player deliberately routes a competing Trade Route through another player's territory without permission (cutting off their existing trade flow).
- Who receives it: The declaring player and all sanctioned parties receive the event. The Trueborn City receives a notification.
- Effect: The sanctioning player becomes a Trade War Actor — all their existing trade agreements are suspended for 10 minutes while the market adjusts (even agreements unrelated to the sanctioned party). The sanctioned dynasty receives: a 15% boost to independent (non-trade) resource generation for 20 minutes (the pressure of sanctions forces internal development), and an automatic Sympathy relationship with all Neutral parties (they move to Cordial for 10 minutes). The Trueborn City issues a Market Disruption warning — all Mercenary Availability events in the match are triggered early if this fires before their normal trigger conditions (the economic disruption destabilizes military markets). All players who had active trade agreements with the sanctioning player may choose to Form a Trade Bloc (as in Market Manipulation) without any bid requirement — automatic Cordial status among bloc members.
- Duration: Trade suspension for the sanctioning player lasts 10 minutes. Sanctions effects on the target last 20 minutes. Sympathy relationship lasts 10 minutes.
- Counter-play: The sanctioning player's strategic goal should be to isolate the target before the sympathy period expires. If the target can form a Trade Bloc during the sympathy period, the sanctioning player will face a united economic front. Speed is the sanctioning player's advantage.
- Conviction interaction: Trade War Declaration is conviction-neutral if announced openly (it is a legitimate economic tactic). Using it as a prelude to military action without declaring intent (and then attacking during the suspension period) costs -15 conviction (economic warfare as military cover is treated as a form of Alliance Betrayal).
- Faith interaction: If the sanctioning player's faith doctrine emphasizes economic justice or communal support, the faith community generates 8% unrest — the doctrine and the action are in conflict.
- Frequency / repeatability: Once per player per match as an initiating action. A player can be the target of a Trade War declaration multiple times.

---

## WORLD EVENTS

---

**[THE COVENANT TEST]** *(Settled Canon — Full Specification)*
- Category: World
- Trigger condition: Any dynasty reaches Level 4 progression in their Bloodline development tree. The Covenant fires specifically for the dynasty that achieved Level 4 — it is a personal test, not a global event.
- Who receives it: The trigger dynasty receives the Test mandate. All other players receive a global notification that a Covenant Test has been issued for that dynasty (they know the stakes but not the specific requirement).
- Effect: The trigger dynasty's faith makes a demand appropriate to its doctrine path. The nature of the demand is doctrine-specific: Sacrifice doctrines demand a specific sacrifice act (a Bloodline Member sacrificed, or a specified number of enemy units offered at a faith structure). Construction doctrines demand a grand structure completed within the test window. War doctrines demand the capture of a specific named territory within the test window. The test window is 20 minutes from notification. Failure: faith intensity drops one full tier (e.g., Fervent drops to Active). The Bloodline Member who triggered Level 4 enters a Doubt State — 20% stat reduction for 10 minutes. Success: faith intensity rises one full tier (capped at Apex). The Apex mechanics for the dynasty's faith doctrine unlock permanently — these are the match's most powerful faith abilities, unavailable by any other path. Success also fires a global notification that the dynasty has passed the Covenant — all players are aware that Apex mechanics are now active.
- Duration: 20-minute test window. Effects are permanent post-resolution.
- Counter-play: Opposing players who receive the global notification can attempt to disrupt the test — attacking the dynasty to prevent them from completing their war objective, destroying a structure required for the sacrifice, or contesting the demanded territory. Disrupting the Covenant Test of another player is a high-stakes gambit — it earns +15 diplomatic prestige from the tested dynasty's rivals (they appreciate the interference) but costs -8 conviction (interfering with sacred obligations is morally questionable).
- Conviction interaction: The specific act demanded by the Covenant Test carries its own conviction weight. Sacrifice-doctrine tests carry a conviction cost for the sacrifice. War-doctrine tests carry the conviction weight of any military action taken. The Test cannot waive these costs — completing the Covenant faithfully means bearing the conviction consequences honestly.
- Faith interaction: Core mechanic. The Covenant Test is the Faith Pillar's defining moment. Apex mechanics represent the game's deepest faith expression — available only to those who have committed fully and paid the price.
- Frequency / repeatability: Once per dynasty per match. The Covenant fires exactly once — when Level 4 is first achieved. There is no second chance.

---

**[THE GREAT RECKONING]** *(Settled Canon — Full Specification)*
- Category: World
- Trigger condition: Any single dynasty controls 70% or more of the match's total territorial area simultaneously.
- Who receives it: All players receive the Reckoning notification simultaneously. The Trueborn City issues a formal declaration.
- Effect: The hegemon dynasty is designated The Ascendant. All other surviving dynasties immediately receive the Coalition Mandate — a passive global bonus applied simultaneously: +20% to all military unit stats, +15% to resource generation, and a forced diplomatic floor — all non-hegemon dynasties are locked at Neutral or higher diplomatic status with each other for the duration of the Reckoning (they cannot enter War state with each other). The Trueborn immediately mobilizes — the Trueborn Summons fires automatically (see below for full Trueborn Summons specification). The Great Reckoning ends when the hegemon drops below 60% territorial control. When Reckoning ends: all Coalition bonuses expire, the forced diplomatic floor lifts, and the Coalition members return to their prior diplomatic states. If the hegemon is eliminated during Reckoning, a Victory Condition check fires — the match ends if the Coalition agrees to it (requiring 60%+ of surviving players to vote for match conclusion).
- Duration: Ongoing until hegemon drops below 60% or is eliminated.
- Counter-play: The Ascendant dynasty's only strategic path through the Reckoning is to eliminate Coalition members faster than the Coalition can coordinate. If the Ascendant can reduce player count by elimination before Coalition coordination solidifies, the 60% threshold is easier to maintain because the map has fewer contested territories. The Reckoning is designed to be solvable but difficult — not a guaranteed game-ending event, but the defining challenge of the match's final phase.
- Conviction interaction: The Ascendant's conviction does not change on Reckoning trigger — the Reckoning is a consequence of success, not a moral judgment. However, the Coalition members who commit atrocities during Reckoning defense (genocide of captured territories, mass execution of prisoners) still suffer their normal conviction costs — the Reckoning does not morally license all behavior in the Coalition's name.
- Faith interaction: Each faith represented in the Coalition generates its own religious response to the Reckoning. Doctrine-dependent events may fire within the Reckoning — a faith at Apex intensity during the Reckoning may trigger its Apex mechanics in defense of the Coalition, adding a second layer of power for the most faith-committed defenders.
- Frequency / repeatability: Once per match globally. Once the Reckoning fires and the hegemon returns below 60%, it cannot fire again for the same dynasty — the Reckoning is a once-in-a-match world event.

---

**[THE TRUEBORN SUMMONS]**
- Category: World
- Trigger condition: Fires in two scenarios: (1) automatically as part of The Great Reckoning when the 70% threshold is reached, or (2) independently when any player kills or captures a Trueborn Representative in Trueborn City territory.
- Who receives it: All players receive the Summons notification. The triggering player receives a specific Judgment notification indicating they are the target.
- Effect: The Trueborn City mobilizes its military reserve — a Trueborn Legion spawns at Trueborn City and begins moving toward the triggering event's source (in Reckoning, toward the hegemon's nearest territorial border; in the assassination scenario, directly toward the perpetrating player's nearest territory). The Trueborn Legion is a powerful, independent, non-controllable military force with the following properties: higher individual unit stats than most player armies at equivalent development stage, no morale or supply chain vulnerability (they do not tire, do not revolt, do not suffer siege attrition), and a specific target objective (they do not wander or attack bystanders). The Legion can be slowed by military opposition but cannot be turned — eliminating one unit from the Legion causes two to regenerate over the next 5 minutes. The Legion can be neutralized permanently by: (a) offering a tribute of 500 gold and 200 of each primary resource to the Trueborn City diplomatically, (b) returning a captured Trueborn Representative (if that was the trigger), or (c) eliminating 80% of the Legion's total strength within 10 minutes without it regenerating (this is extremely difficult and intended to be a last resort). If the Trueborn Legion reaches the target player's territory and the target has not neutralized it, the Legion will besiege the target's primary hold structure for 30 minutes — during this siege, the target player cannot receive new diplomatic agreements or trade from any party.
- Duration: The Legion persists until neutralized or until it completes its objective (siege conclusion). If the siege is completed and the hold falls, the match ends for the besieged player.
- Counter-play: Allies of the targeted player cannot assist in neutralizing the Trueborn Legion without becoming secondary targets (the Legion adds them to its list for interference). This is intentional — Trueborn judgment is not a coalition problem, it is a specific reckoning. Rivals of the targeted player may attempt to intercept the Legion and slow it (distracting it with military units), which gives the targeted player time to neutralize it through tribute.
- Conviction interaction: The Trueborn Summons does not change the triggering player's conviction — their actions have already cost them conviction through the underlying event. The Summons is the world's institutional response, not its moral judgment. That judgment already happened.
- Faith interaction: If the targeted player's faith has Apex intensity (80%+), their faith community is aware of the Trueborn Legion approach and generates a Rallying Cry — +15% to the targeted player's military defense stats for the duration of the Legion's approach. The faith is protecting its own.
- Frequency / repeatability: Can fire twice in a match maximum — once as part of the Great Reckoning, and once for the assassination trigger. The assassination trigger can only fire once per match regardless of how many representatives are attacked (subsequent attacks on Trueborn Representatives in the same match escalate the existing Legion rather than spawning a second one).

---

**[THE LONG WINTER]**
- Category: World
- Trigger condition: Food resources across the entire map (aggregate total of all player stockpiles and resource nodes) drop below 40% of starting capacity simultaneously, due to sustained military conflict destroying food-producing territories (farmland structures destroyed, water sources cut). This requires significant sustained warfare — it cannot happen through normal play without deliberate large-scale agricultural destruction. Fires at the earliest in the Ideological Expansion match stage (Stage 3).
- Who receives it: All players receive the global notification simultaneously.
- Effect: A 20-minute famine period begins. During The Long Winter: food generation from all sources drops an additional 25% globally, population growth halts universally, and food consumption by military units doubles (armies require more to stay operational in lean conditions). The Long Winter forces all players to make strategic resource decisions simultaneously — armies or populations, but not both at full capacity. Players who had been maintaining food surpluses are shielded (their surplus carries them through the worst of the winter). Players who had been running food deficits before the Winter hits enter immediate Scarcity (see Economic Events). Two additional mechanics activate during the Winter: (a) Famine Diplomacy — all trade proposals involving food receive a 50% receptivity bonus (everyone wants food, everyone is willing to negotiate). (b) Survival Coalition pressure — players whose food drops below 50 units during the Winter receive pressure from their population to seek alliance, generating a natural diplomatic convergence among the weakest players.
- Duration: 20 minutes. After 20 minutes, food generation returns to normal rates (the winter breaks). Recovery takes an additional 10 minutes as food node production rebuilds.
- Counter-play: Players who anticipate the Long Winter can stockpile food before the trigger threshold is reached — this requires tracking aggregate map food levels, which is visible in the map resource overlay. Players who have diversified into faith-based resource production (some doctrines allow faith structures to produce food equivalents) are partially insulated. Aggressive military players who have been destroying farmland should be aware they are pulling the trigger on this event and plan accordingly.
- Conviction interaction: A player who deliberately destroys the food systems of enemy territories in order to trigger the Long Winter receives -15 conviction — the act of weaponizing famine is among the game's cruelest recognizable actions. A player who offers food to struggling dynasties during the Winter receives +8 conviction per Emergency Trade completed.
- Faith interaction: Faith doctrines that include resource stewardship as a principle generate a Faith Response to the Long Winter — affected players whose faith is Active or higher receive a Pastoral bonus: food generation from faith structures (if doctrine supports it) increases 30% during the Winter, representing the faith community pooling resources.
- Frequency / repeatability: Once per match globally. The Long Winter is a world event that can only occur once — agricultural recovery after the Winter prevents a second collapse.

---

## Event Design Principles

**Events are consequences, not punishment.** Every event in this system is the legible result of choices players made during the match. The Succession Crisis fires because the player did not designate a secondary heir. The Veteran Army Revolt fires because the player ran their army into the ground without food or mercy. The Long Winter fires because someone has been burning farms. Players who are paying attention can see every event coming before it arrives — and players who are not paying attention discover that the world has been keeping score. The design explicitly rejects the randomized disaster card model, where a player might lose progress through no fault of their own. In Bloodlines, every event has a reason, and every reason can be found somewhere in the match's history.

**Every event creates an actor and a reactor, and the reactor has real options.** A political event that only affects the receiving player and offers no response path is not an event — it is a penalty. Every event in this system is designed with counter-play as a first-class requirement. The Siege Attrition pressures the besieger and the besieged simultaneously, with each having meaningful options. The Alliance Betrayal gives the betrayed party a sympathy window and diplomatic momentum. The Refugee Crisis gives the receiving player three genuinely different paths with distinct downstream consequences. Reactive decision-making is strategy, not suffering.

**Conviction and faith are not score systems — they are pressure systems.** Events interact with the Three Pillars not to reward good players and punish bad players, but to create pressure that shapes behavior and opens or closes options. A player who has maintained Moral conviction for the entire match has access to options that a Cruel player simply does not have — they can absorb an Embassy Insult with grace and earn conviction credit, they can trigger Divine Favor, they can accept refugees and receive the positive integration outcome. A Cruel player is not losing — they have traded those options for different ones: they can execute without the normal conviction cost, they can suppress dissent, they can rule through fear. The design goal is not to make Moral play obviously superior, but to make each conviction orientation coherent and internally consistent.

**Scale and timing are designed to prevent event flooding.** Events that can repeat have repeat restrictions calibrated to their impact. The Schism fires once per faith globally. The Covenant Test fires once per dynasty. The Great Reckoning fires once per match. Events that can fire repeatedly (Trade Route Collapse, Siege Attrition, Garrison Defection) are calibrated to feel earned each time rather than mechanically automatic — the trigger conditions require sustained player behavior, not a single action. In long matches (4+ hours with up to 10 players), the event system can generate dozens of events, but the distribution across all players and all categories ensures that no single player is overwhelmed while others experience nothing. The Mercenary Availability cap at three per match is a specific example of this calibration — mercenaries should feel like an unusual opportunity, not a standing market.

**Coalition-forming events are intentional diplomatic design.** The game benefits from moments where multiple players are pulled toward the same decision simultaneously. The Great Reckoning does this explicitly — all non-hegemons are pushed toward coalition. The Long Winter does it subtly — resource scarcity makes all weakened players suddenly interested in the same conversations. The Holy War Declaration does it through shared identity — faith allies respond without being asked. These coalition-forming events are not accidents of design; they are the mechanism by which the game's political landscape shifts organically from competitive to cooperative and back. Players who understand that these events are coming and position themselves to be the natural center of the resulting coalition are playing political strategy at the deepest available level.

**The world has memory, and events reflect that memory.** The Dynastic Rivalry event fires because two players have been fighting for 30 minutes without resolution — the game noticed. The Currency War fires because two players converged on the same minor tribe in the same window — the game noticed that too. The Conquered Territory Rising fires because a player held five territories for 30 minutes without governing them — the neglect was tracked. This is the core design philosophy of the entire event system: the match generates its own history, and the events are the history speaking back. Players who treat the match as a series of isolated tactical decisions will be repeatedly surprised by events that feel arbitrary. Players who treat the match as a living political environment — where their patterns accumulate meaning — will feel the events as inevitable and earned. The goal is that, at the end of a match, every event that fired could be explained by pointing to specific decisions that preceded it.

---

*Document End — CREATIVE BRANCH 002 — POLITICAL EVENTS SPECIFICATION*
*PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON*
*(Design Content — 2026-03-18, Creative Branch 002)*
