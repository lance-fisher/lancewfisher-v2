# BLOODLINES — BIBLE ADDITION: TIME MODEL, COMMANDER SYSTEM, AND SESSION ADDITIONS
## Additive to Bible v3.1 — Session 2026-04-07
## Document Authority: This document introduces new canonical content across four major design areas.
## All content is additive. Nothing in this document supersedes existing bible content except where
## explicitly noted. Lance Fisher is sole authority on canon, deprecation, and removal.
## Suggested designation: Bible Addendum A — to be integrated as new sections in next versioned bible.

---

## ADDENDUM SECTION 1: THE DECLARED TIME MODEL
### Supersedes and replaces the time model content in BLOODLINES_ADDITIVE_INTEGRATION.md
### Proposed bible placement: New Section in Part VIII (Match Structure), following the Five-Stage Architecture

---

### THE FUNDAMENTAL DESIGN INSIGHT

The time problem in Bloodlines — how a live real-time strategy game coexists with a generational dynasty simulation across a 6-10 hour session — resolves through a principle called declared time.

A battle takes 20-40 minutes of the player's actual session time. When that battle concludes, the game declares that a defined amount of in-world time has elapsed. The player experienced a fast, tactical, real-time engagement with all the immediacy and responsiveness of Command and Conquer-style play. The world experienced a campaign of several months or several years. Both are simultaneously true and neither contradicts the other, because the player never had to hold both frames in their head at the same time.

This is not a compromise or a trick. It is how all historical narrative works at every level of the form. A chapter takes twenty minutes to read and covers three years of a character's life. The reader never feels the contradiction because the narrative manages the frame. Bloodlines manages the frame the same way — through what it tells the player when the engagement concludes.

The alternative approaches that do not work for Bloodlines are worth naming explicitly so they can be discarded cleanly:

A single universal year-to-hour ratio cannot satisfy both games simultaneously. If one real hour equals ten in-world years, a single 30-minute battle covers five in-world years, which is absurd as a tactical engagement but reasonable as a dynasty clock. If one real hour equals one in-world year, a battle lasts a few months of in-world time but the dynasty barely ages across the full match. No single ratio works because the RTS layer and the dynasty layer operate at genuinely different time scales that cannot be collapsed into one number without breaking one or both.

Pausing dynasty time during battles is an admission of failure — it acknowledges the two systems cannot actually coexist and imposes a mechanical separation that the design philosophy explicitly rejects.

Making dynasty time primary and abstracting battles loses the Command and Conquer immediacy that is the game's foundational RTS identity.

Making battle time primary and treating dynasty as a notification layer produces a dynasty simulation that feels thin and bolted-on rather than genuinely generational.

Declared time solves all of these by not trying to reconcile the two clocks into one. They are different clocks. The player experiences the battle clock during battles and the dynasty clock during the strategic layer between battles. The declaration at battle's end is the seam that joins them. Done correctly, the seam is invisible.

---

### HOW DECLARED TIME WORKS IN PRACTICE

When a battle concludes — whether the player fought it directly or it resolved under Commander direction — the game presents a brief declaration screen or overlay that announces the elapsed in-world time. This is not a loading screen. It is a narrative beat. The world has moved. Here is how much.

The declaration is not arbitrary. It is calculated from the engagement type, size, duration, and outcome. A small skirmish at a frontier resource node declares that three to six months have passed. A significant pitched battle between two standing armies declares one to two in-world years. A sustained siege campaign fought across multiple connected engagements declares three to five years. Stage One engagements, which are small and contained by design, declare modest amounts — a few months each — because the dynasty is young and the world is still being discovered, and time should feel slow at the beginning. By Stage Four, a single major engagement might declare three years elapsed, and the player feels the weight of that.

The specific declaration table, which maps engagement type and stage to in-world time elapsed, is a primary design lever for controlling dynasty pacing. Adjusting these numbers adjusts how many generations mature across the match's full arc. The design target — two to three generations achievable in a full 6-10 hour session — requires that by the match's mid-stages, major engagements are declaring enough time that a child born in Stage One can be a senior military figure by Stage Four or Five. This requires major engagements in Stages Three and Four to declare roughly two to four years each.

The declaration is presented as a brief narrative statement in the voice of the match world, not as a game-system notification. Not "elapsed time: 2 years." Rather, something that reflects what the battle represented in the world's history. The format serves both the time declaration purpose and the story-generator purpose that the design philosophy identifies as the game's ultimate test.

---

### THE STRATEGIC LAYER: WHERE THE DYNASTY GAME LIVES

Following the declaration, the player enters the strategic layer. This is not a menu or a pause screen. It is the regional and sovereign view of the game — the player looking out at the full match state and making the decisions that determine what happens next. The strategic layer is where the empire is actually run. The battles are the dramatic foreground. The strategic layer between them is the substance of the dynasty game.

The strategic layer has two primary components: the events queue and the commitment phase.

The events queue surfaces everything that accumulated during the elapsed in-world time. Dynasty events: a bloodline member has reached a development milestone, an heir has matured into an adult role, a spouse is expecting a child, a senior member is showing the first signs of age-related decline. Diplomatic events: a trade agreement has reached its renewal threshold, a previously neutral dynasty has made a territorial claim adjacent to a contested province, an ally has sent a delegation requesting military coordination. Operational events: a rogue operative has advanced to a new position, a mystic's calendar window has opened, an operation in progress has encountered a complication. Faith events: intensity in a recently conquered territory has risen to the threshold for the second-tier building, a rival covenant's spread is approaching the boundary of a region the player considers theirs. Conviction events: the Pattern Amplification system has registered a behavioral milestone, a population segment's loyalty has crossed a threshold in either direction.

The player works through these events. Some require immediate decisions. A succession event triggered by an unexpected death cannot wait — the player must designate a replacement and manage the political consequences before moving on. A trade agreement renewal can be deferred to a later strategic window. Most events sit between these extremes: they benefit from attention but do not demand it immediately, and the player's judgment about prioritization is itself a meaningful strategic decision. A player who consistently processes their queue fully and thoughtfully runs a more coherent empire than a player who repeatedly defers events and lets situations deteriorate before addressing them.

The commitment phase is where the player defines what happens next across every active front. Where do armies move? Which battles does the player intend to personally attend, and which will be delegated to Commanders? Which operations are being initiated, advanced, or paused? What diplomatic communications are being sent? Where are bloodline members being reassigned? Which faith rituals are being ordered?

These commitments take real planning time in the session — the player is thinking, assessing, deciding. And they take in-world time to execute: an army moving three provinces over requires several months of in-world transit, declared as elapsed time when the player next surfaces from a battle. A rogue operative setting up a new position in enemy territory requires months of careful movement, accumulated across several battle-strategic cycles. The player must commit to timelines before returning to direct engagement, accepting that those timelines will run while they are focused elsewhere.

When the commitment phase is complete, the player drops into their next direct engagement. The match continues on every front simultaneously, with Commanders executing their directives, operations advancing, caravans moving, tribes being influenced, faith spreading. The player surfaces again when their direct engagement concludes, finds that the world has moved by however much the next declaration states, and works through a new events queue shaped by everything that happened while they were fighting.

This cycle — battle, declaration, events queue, commitment phase, battle — is the rhythm of a Bloodlines session. The battles are what the player directly experiences as the RTS game. The strategic layer is where the dynasty simulation lives. The declared time is the seam that makes both feel real simultaneously.

---

### DYNASTY EVENTS DURING LIVE BATTLE

Dynasty events and operational events do not interrupt live battles. This is absolute. The player in the middle of a Command and Conquer-style engagement is not pulled out of it by a popup announcing that their heir has reached adolescence. The tactical layer has the player's full attention. The dynasty layer queues everything and waits.

Events that accumulate during a battle are held in the queue and surfaced at the declaration screen following the battle's conclusion. The player fights, the battle ends, the declaration is made, and then the player learns everything that happened in the world while they were engaged.

The exception is events of immediate existential consequence to the active battle: the Commander of the army being fought is killed or routed, a supply line to the active battlefield has been cut and attrition is beginning, a bloodline member present in the engagement has been wounded. These are surfaced as in-battle notifications because they affect decisions the player is making right now. They are presented as tactical intelligence, not as dynasty management prompts. The player is not being pulled into a management screen — they are being given information that changes what they should do on the battlefield in the next 30 seconds.

Everything else queues. The player trusts that the world is running — Commanders are executing their directives, operations are advancing, caravans are moving — and they focus on the battle in front of them. When the battle ends, they find out what happened.

---

### STAGE-BY-STAGE TIME FEEL

Stage One matches the feel the player described as the RTS campaign opening — small base, gathering resources, a tribe appears, what do we do? Each of those early encounters is brief, real-time, contained. The time declarations are modest: three to six months per encounter. The dynasty grows slowly and deliberately. A child born early in Stage One has not yet reached childhood by Stage One's end. The founding head is still in their prime. The world is being discovered.

Stage Two engagements grow in scale as expansion becomes more ambitious and iron-era armies take the field. Time declarations increase: six months to a year per major engagement. The dynasty begins to feel it — bloodline members' ages are visibly advancing, children are reaching adolescence, the first succession questions are appearing in the events queue as distant concerns rather than immediate crises.

Stage Three engagements are the first full-scale conflicts between founding houses. Time declarations of one to two years per major engagement are appropriate. By Stage Three's end, a founding head who began in their late 30s is now in their 50s. The heir designated in Stage One or early Stage Two is now an adult figure with their own established capability. The dynasty has genuine generational depth — the founding generation and the next generation are both active.

Stage Four is heavy combat. Major battles declare two to three years. Prestige Wars, sieges, and sustained campaigns declare three to five. The founding head, if still alive, is aging into their 60s or 70s and their decline is becoming mechanically visible — reduced command radius, slower physical recovery from wounds, elevated succession urgency. The grandchildren of the founding marriage, if the player invested in bloodline development through the earlier stages, are reaching adolescence or early adulthood. The dynasty has genuine multi-generational depth.

Stage Five is the match's culmination. The battles are the largest and most consequential. Time declarations are significant but the match is approaching resolution — the declarations do not need to carry additional generational weight, because the generations have already developed. Stage Five is the harvest of everything planted in Stages One through Four. The bloodline that the player has been building across the full session is now at its most developed, its most complex, and its most at stake.

---

### THE ROGUE AND MYSTIC TIME MODEL

Covert and faith operations find their most natural mechanical home under the declared time model.

A rogue being navigated into enemy territory is not a battle. It is a slow-moving operation that runs in the background across multiple battle-strategic cycles. The player initiates it during the commitment phase of a strategic layer window — setting the operative's objective, their route approach, the patience threshold the player will accept before redirecting, and the military support directive for any nearby friendly army that might provide extraction cover. The operative then advances across subsequent battle-declaration cycles, each one moving them closer to the objective or reporting complications that require course correction.

A rogue operation described in the Bible as taking several in-world years to complete does not occupy two years of real session time. It runs across multiple cycles while the player is fighting battles, addressing events queues, and managing other commitments. In real session time it resolves over the course of an hour or two, depending on how many battle cycles intervene and how complex the operative's position has become. In in-world time it has been a multi-year covert campaign. Both are simultaneously true. The player experiences the session-time version. The match world records the in-world version.

The requirement that a rogue reach a certain capability or position threshold before attempting a high-value operation maps directly to this model. Building an agent network inside an enemy territory requires time and patience. Each battle cycle that passes while the operative is in position and accumulating contacts represents months of careful work — relationship building, information gathering, risk assessment, physical positioning. The player cannot accelerate this by fighting more aggressively on other fronts. The operative's work is time-gated in the in-world sense, which is exactly what makes covert operations feel like covert operations rather than instant-use abilities.

The calendar timing of mystic operations — the lunar events, solstices, and celestial events established in the Operations System — gains mechanical depth under the declared time model. The player can see from the strategic layer that a Blood Moon is three in-world months away. That means one or two battle cycles before the window opens. The player plans around it: fight this engagement, process the events queue, move the mystic into position, and the Blood Moon window will be open at the conclusion of the next cycle. The timing is not a passive countdown. It is a strategic constraint that the player incorporates into their planning. Missing the window means waiting for the next occurrence, and the match does not wait.

---

### MULTIPLAYER: THE SHARED MATCH CALENDAR

In a multiplayer match with human players operating simultaneously, the declared time model must extend to a shared framework that prevents different players' dynasties from aging at different rates.

The match runs on a shared calendar — a single in-world timeline that all players and all AI dynasties operate against. This calendar advances based on the match's aggregate activity rather than any individual player's battle frequency. A player who fights five engagements in a real-world hour does not advance the calendar faster than a player who fights two. Both are engaging with events within the same shared timeline. The calendar advances at a rate tied to the overall match progression, calibrated by stage, that keeps all dynasties in roughly the same generational neighborhood.

Individual players' battle conclusions still declare time and surface event queues. But the declaration is drawn from the shared calendar rather than calculated independently per player. A player concluding a battle during a period when the shared calendar has advanced six months receives a six-month declaration. Another player concluding a battle in the same shared period receives the same declaration. The world has moved six months for everyone simultaneously, even though each player was doing different things during that real-world interval.

At any given moment in a multiplayer session, different players occupy different points in their personal battle-strategic cycle. Player A is in a live battle. Player B just concluded a battle and is in their events queue. Player C has committed their strategic orders and is watching a Commander-directed operation resolve while waiting to drop into their next engagement. Player D is deep in the commitment phase, repositioning armies and initiating operations. All of these states are happening simultaneously. The shared match calendar continues advancing in the background regardless of where each player is in their personal cycle.

The match does not pause for individual players' strategic layer windows. The world continues operating — AI-directed armies execute their directives, operations advance, caravans move, tribes accumulate relationship history — while any given player is in their strategic window. This means a player who spends a long time in the strategic layer is allowing more real-world time to pass before their next personal engagement, which allows more things to happen on the fronts they are not attending. This is not a penalty. It is the reality of a large empire — time keeps moving whether you are watching it or not. A player with well-constructed Commanders and clear directives on all fronts can afford a long strategic window. A player with vague directives and weak Commanders cannot.

Diplomatic interactions between human players in real time are handled through the strategic layer interface, which functions as a persistent communication channel available to all players simultaneously regardless of their current battle or strategic state. A diplomatic message sent by Player A while they are in a live battle is received by Player B while they are in their strategic window. Neither player needs to be in the same mode to communicate. The communication happens in in-world time — the message takes in-world travel time to arrive, calculated automatically based on the diplomatic channel and the current calendar position.

---

## ADDENDUM SECTION 2: THE COMMANDER SYSTEM
### New canonical content — no equivalent in Bible v3.1
### Proposed bible placement: New Section in Part VI (Military Systems), following Section 38 (Army Composition and the Recruitment Slider System)

---

### THE FOUNDATIONAL PRINCIPLE

The Commander system exists to solve a specific problem without using the design solution that Bloodlines explicitly rejects.

The problem: a match-scale Bloodlines session produces an empire with multiple active military fronts, simultaneous operations, contested trade routes, and diplomatic situations that all require attention simultaneously. The player has one body and one field of focus. They cannot personally command everything at once.

The rejected solution: impose a numerical cap. The player can only directly control X armies. Beyond X, armies cannot be activated or committed. This is the conventional RTS approach to preventing scope overload. It is explicitly not Bloodlines' approach. The design philosophy states that the player must always be permitted to try to control everything personally. The limitation must be human, not mechanical.

The Commander system is the infrastructure that makes this true. Every army in the match is always commandable by the player. The player never reaches a wall that says "you cannot control this." What the player reaches, if they overextend, is the natural consequence of having distributed their attention too thin — Commanders without sufficient capability executing directives without sufficient specificity on fronts the player is not watching. Those fronts produce worse outcomes than the player would have produced personally. The match shows the player what overextension costs. It does not prevent them from overextending.

The Commander slot on every army is always occupied by one of two things: a player-designated Commander operating under player-set directives, or the player themselves in direct control. Switching between these two states is immediate and available at any time from the strategic layer. The player can pick up any army, at any time, and take it over personally. They can release any army to Commander direction at any time. The toggle is instant and unrestricted.

---

### THE TWO COMMANDER TYPES

#### Bloodline Member Commander

Any bloodline member with sufficient military development can be assigned as Commander of a specific army. This is the premier option. A bloodline member commanding an army brings everything they are to that army — their accumulated military experience measured as a developed attribute, their conviction posture which shapes how they execute under ambiguous orders, their faith alignment which affects the morale of troops sharing that alignment, their personal tactical history which the Tactical Memory system records and compounds, and their personal relationships with units and sub-commanders who have served under them across multiple engagements.

An army under a veteran bloodline Commander is a qualitatively different thing from an army under an AI Field Commander. The veteran Commander does not just execute the directive — they execute it as someone with a reputation among the troops, a history of making good calls under pressure, and a presence that the units respond to emotionally as well as functionally. The morale mechanics, the conviction-alignment bonuses, the command radius effects from the War General role — these all belong to the bloodline Commander's presence. They do not transfer to an AI Field Commander.

Every bloodline Commander has a command capacity — the scale of force they can effectively lead while maintaining full command effectiveness. This is not a hard cap on army size. It is a quality threshold. A Commander assigned an army within their capacity leads it at full effectiveness. A Commander assigned an army that exceeds their capacity leads everything within their capacity at full effectiveness and everything beyond it at degraded effectiveness — as though those units had no Commander at all. The player who assigns their newly committed War General to lead the match's largest army is not blocked from doing so. They are simply doing it badly, and the match will demonstrate what badly looks like in the next engagement that exceeds the Commander's threshold.

Command capacity grows with development. A War General who has led armies across multiple stages of the match has a larger effective capacity than one who was committed in Stage Three and has fought one major engagement. The development investment in bloodline Commanders is a direct investment in the empire's ability to run itself effectively when the player is not personally present.

A bloodline Commander given a directive executes it according to two simultaneous influences: the specific directive content, and their personal tactical profile built from their match history. A Commander who has spent multiple stages in aggressive offensive operations executes even a Hold directive with more aggressive margins than a Commander whose history is defensive. A Commander whose conviction history is Moral will not execute a directive in ways that generate significant Cruel conviction events without additional player authorization — their personal conviction creates behavioral boundaries on their interpretation of orders. A Commander at Fervent faith intensity will prioritize protection of faith infrastructure within their operational area even when the directive does not specify it, because that is who they are.

This means the player's choice of bloodline Commander for a given army is itself a strategic decision. A cautious Commander with a conservative tactical profile is not a good choice for a Push This Front directive on a volatile contested border. An aggressive Commander with a Cruel conviction record is not a good choice for a Secure This Region directive in newly governed territory where the player is trying to build population loyalty. Matching the Commander to the mission is the player's responsibility and a genuine source of strategic depth.

A bloodline Commander can be set to full directive-free autonomy, in which case they operate entirely on their own tactical judgment within their understood strategic context. This is the highest-trust delegation available. A Commander in full autonomy does not ask for permission — they identify threats, opportunities, and objectives within their area of responsibility and act on their own assessment. The quality of that assessment depends entirely on the Commander's development, conviction posture, and faith alignment. A Legendary Member War General operating with full autonomy is a genuine strategic asset. A recently promoted Commander with minimal engagement history operating with full autonomy in a complex situation is a liability.

A bloodline Commander can be recalled from any army during the strategic layer and reassigned elsewhere. The recall does not happen instantaneously in the world — it takes in-world transit time for the Commander to physically move from one army to another. During that transit period, the army they left operates under AI Field Commander direction with the last active directive. The player must plan Commander reassignments with enough lead time to account for this, which means the strategic layer commitment phase includes thinking about where key bloodline Commanders need to be for the next stage of the match and initiating their movement before that stage arrives.

#### AI Field Commander

When no bloodline member is available or the player chooses not to commit one, an AI Field Commander occupies the slot. AI Field Commanders are competent. They execute directives reliably. They do not make catastrophically bad decisions under normal conditions. What they do not bring is anything personal — no command aura, no conviction weight, no accumulated relationship with the troops, no tactical profile shaped by real engagement history, no faith alignment that amplifies the army's morale, no reputation among the units that makes them fight harder because of who is leading them.

An army under an AI Field Commander performs at its baseline capability. The units are whatever they are. The equipment is whatever it is. The doctrine is whatever the directive specifies. Nothing amplifies them beyond their raw values. An army under a veteran bloodline Commander at matching capacity performs meaningfully above those same raw values.

AI Field Commanders do not develop. They do not gain experience from engagements. They are not affected by conviction events on the fronts they manage. They will not be the subject of assassination or capture operations by a sophisticated rival, because they do not have bloodline member status. They are reliable, generic, and replaceable. This makes them appropriate for stable secondary fronts where the directive is clear, the opposition is not sophisticated, and the player has assessed that a bloodline Commander's amplification is not necessary to achieve the objective.

The player who relies heavily on AI Field Commanders across most fronts while personally attending battles on one primary front is making a deliberate strategic choice: concentrate bloodline Commander quality where it matters most and accept baseline performance everywhere else. This is a valid approach. It is also the approach that leaves the most margin for AI Field Commanders to produce suboptimal outcomes on the fronts the player is not watching, because they cannot adapt intelligently to circumstances the directive did not anticipate.

---

### THE DIRECTIVE SYSTEM

When an army is operating under any Commander rather than the player's direct control, it operates under a Directive. The Directive is the standing order — the mission the Commander pursues until the player changes it, the objective is achieved, or the conditions make it impossible. Directives are not simple attack/defend toggles. They are specific, nuanced strategic instructions with enough precision to run a complex empire across multiple simultaneous fronts.

Directives are set during the strategic layer commitment phase. They persist across battle cycles until the player changes them. A well-constructed directive requires no adjustment for several cycles — the Commander is doing exactly what the player intended. A directive that becomes mismatched to a changing situation requires updating when the player surfaces from their next battle. Monitoring directive-to-situation fit is one of the primary skills of managing a large empire in Bloodlines.

#### Territorial Directives

**Hold This Line** — The army maintains its current position and engages anything that enters a defined perimeter threshold. It does not advance beyond defensive necessity. It does not pursue routed enemies past a player-specified pursuit threshold. It holds. A bloodline Commander executing this directive applies their personal judgment about when holding requires a counter-attack to prevent breakthrough — an aggressive Commander will counter-attack more readily than a cautious one. An AI Field Commander holds more rigidly to the literal position without adaptation. The player can specify the pursuit threshold: do not pursue at all, pursue until the enemy is off the immediate engagement zone, pursue until the enemy is across the nearest terrain barrier.

**Advance to This Position** — The army moves toward a specified map location and secures it, engaging resistance along the route as necessary. When the position is reached, the army defaults to Hold This Line unless a follow-on directive is pre-set. The player can chain directives: advance to Position A, then advance to Position B, then hold. The Commander executes the chain without requiring player intervention between steps unless something unexpected interrupts the sequence.

**Secure This Region** — Broader than Advance to Position. The army pacifies and establishes control across a defined territory rather than moving to a point. The Commander handles resistance as it finds it, establishes garrison sub-units at key locations within the region, and reports mission complete when the region's overall stability threshold is met. This directive is appropriate for consolidation following a major offensive push — the player fights the major battle personally, then sets Secure This Region on the follow-up army while attending to other fronts.

**Push This Front** — An aggressive advance directive without a specified terminus. The army presses the current contested line forward, taking ground where it can and holding where resistance stiffens. The Commander's tactical profile determines how aggressively they interpret the directive. An aggressive bloodline Commander with a Relentless attitude modifier will push significantly harder and accept more attrition than a cautious Commander with a Measured attitude. The player must choose the right Commander and attitude combination for the pressure level they want to apply.

**Fall Back to This Position** — The army withdraws to a specified location, managing the retreat in as orderly a fashion as the operational situation permits. Used when a front is untenable or when the player is consolidating force ahead of a major personal engagement and needs a specific army repositioned without managing the withdrawal themselves. A bloodline Commander with strong defensive experience manages retreat engagements better than an AI Field Commander — rear-guard actions during withdrawal are among the highest-skill tactical situations an army faces.

**Contain and Watch** — The army maintains a position at a defined distance from an opponent's territory or force, observing and reporting without initiating engagement unless the opponent crosses a defined threshold. This is the military equivalent of Demonstrate Force combined with observation capacity. It generates ongoing intelligence about the watched opponent's movements and keeps the army positioned to transition quickly to Push This Front if the diplomatic situation changes.

#### Combat Directives

**Eliminate This Opponent** — The army prioritizes engagements with a specified enemy dynasty's forces above all other operational considerations. It pursues, engages, and sustains pressure against that opponent as long as the directive is active. Sub-objectives within the directive can be set: eliminate their field armies while leaving infrastructure intact, eliminate their ability to project force in this region, target specifically the forces commanded by a named bloodline Commander. This is the directive for focused, sustained pressure against a specific enemy — useful when the player wants one front prosecuted aggressively while they attend another personally.

**Suppress This Area** — The army maintains combat pressure on a defined territory without committing to full conquest or sustained occupation. It degrades the enemy's capacity to use that area productively — disrupting resource gathering, interdicting supply routes, threatening garrisons — without the full governance and supply cost of claiming and holding the territory. Useful for keeping an opponent occupied and resource-depleted while the player's primary effort is elsewhere.

**Intercept and Destroy** — The army positions at a defined corridor or approach and engages enemy forces moving through it. Reactive rather than aggressive — the army does not range far from its position but responds with full force to anything that enters its intercept zone. Useful for protecting a supply route, a key resource node, or a strategic terrain feature without committing to a broader offensive.

**Protect This Asset** — The army positions around a specified settlement, resource node, trade route section, or bloodline member location and treats its defense as the primary mission above all other engagement opportunities. Engagement is reactive — the army does not pursue, it defends. Useful when a high-value asset is under threat and the player cannot personally attend its defense.

#### Operational Directives

**Secure This Resource** — The army moves to claim and hold a specified resource node, handling whatever opposition it finds and establishing garrison infrastructure sufficient to make the claim durable. Completing this directive returns the resource to the player's economic network. The Commander manages both the military and the immediate garrison construction phases.

**Establish Forward Position** — The army advances to a specified location and constructs a forward base there, including initial fortification, supply depot, and garrison structure. This directive includes the construction phase explicitly — the army is not simply occupying the position but building the infrastructure that makes it a functional forward base. The bloodline Commander managing this directive influences the quality and defensibility of what gets built, with experienced Commanders producing more effective forward positions than AI Field Commanders.

**Clear This Route** — The army patrols a specified supply or trade route, engaging hostile forces encountered along it and returning to the route after each engagement. Useful for making a contested supply corridor usable without committing to full territorial control of the surrounding area.

**Support This Operation** — The army positions to provide military cover for a covert or faith operation in progress within a defined area. It does not initiate engagement unprovoked but responds to threats that could compromise the operation. This directive is what allows a rogue moving through dangerous territory to have meaningful extraction cover without the covering army being so close that it blows the operative's cover. The player sets both the support radius — how close the army sits to the operative's route — and the engagement trigger: respond to any threat that enters this zone, or respond only to threats that directly approach the operative's last known position.

#### Diplomatic Directives

**Demonstrate Force** — The army moves to the border of a specified dynasty's territory and establishes a visible presence without initiating engagement. This is a political signal with military expression — it tells the target dynasty that the player is paying attention to this border and has committed forces to it. The diplomatic calculation adjustment this creates in the target dynasty's AI or in the human player's perception is the directive's objective. No blood is shed unless the target initiates.

**Monitor and Report** — The army maintains observation of a specified area and generates regular intelligence reports about movement, construction, and military composition within its observation range, without engaging. The Commander intelligence capacity matters here — a bloodline Commander with high Spymaster experience or a naturally observant tactical profile generates richer and more accurate intelligence than an AI Field Commander conducting the same observation.

**Enforce Boundary** — The army intercepts and engages any force crossing a defined territorial line without standing authorization to do so. The player specifies the line and the exception list — which dynasties have crossing rights and which do not. Everything not on the exception list that crosses the line is engaged. The army does not cross the line itself. This is the military expression of a formal territorial warning, and triggering it is the other dynasty's choice.

#### Siege Directives

**Encircle and Starve** — The army encircles a target settlement and maintains the encirclement, denying resupply without initiating direct assault unless explicitly ordered otherwise. The siege timeline is determined by the target's food and water reserves as known through intelligence or estimated from the settlement's visible size and population. The player is given an estimated resolution window at directive set and updated as conditions change. A player whose intelligence reveals a well-stocked target with a year's reserves can plan to attend other fronts for several match cycles before needing to check on the siege. A player operating without good intelligence is working with an estimate that may be wrong in either direction.

**Assault When Ready** — The army prepares a siege and initiates the assault when the Commander judges conditions favorable — after bombardment has degraded wall integrity, after the supply situation has weakened the garrison, after the tactical opening presents itself. The Commander's tactical profile shapes the timing judgment. A cautious Commander waits for maximum favorable conditions before ordering the assault, accepting a longer siege timeline in exchange for a higher probability of clean breakthrough. An aggressive Commander initiates earlier, accepting higher assault casualties in exchange for faster resolution. The player can override the Commander's judgment at any time by switching to direct control.

**Siege and Negotiate** — The army encircles the target but the Commander is authorized to receive and evaluate surrender offers within player-specified acceptable parameters. The player defines the terms they will accept before the directive is set: these specific resources in reparation, this population loyalty guarantee, this diplomatic recognition of the player's territorial claim. The Commander evaluates any surrender offer against those parameters and accepts or rejects autonomously. Offers below the parameters are rejected without escalation to the player. Offers at or above the parameters are accepted. This directive allows a siege to resolve through negotiation without requiring the player to be personally present at the negotiation.

---

### ATTITUDE MODIFIERS

Applied on top of any directive, attitude modifiers shape how the Commander interprets ambiguous situations — the inevitable gaps between what the directive specifies and what the actual ground situation presents. No directive can anticipate everything. Attitude modifiers define how the Commander fills those gaps.

**Aggressive** — When in doubt, engage. Press advantages as they appear. Do not wait for perfect conditions before acting. Accept higher casualty rates in exchange for faster objective completion and more decisive outcomes. This modifier applied to a Push This Front directive produces the most aggressive possible advance; applied to a Hold This Line directive, it produces a hold that includes opportunistic counter-attacks against any weakness that presents itself.

**Measured** — Execute the directive at a sustainable pace. Engage when conditions favor engagement. Withdraw when conditions do not. Preserve force integrity alongside objective progress. The middle position between Aggressive and Cautious — appropriate for most standard operational situations where the player wants reliable performance without accepting either the higher casualties of Aggressive or the slower pace of Cautious.

**Cautious** — Minimize risk. Do not engage unless the directive specifically requires it and conditions are favorable. Set retreat thresholds conservatively — withdraw when the engagement becomes costly rather than sustaining through adverse conditions. Prioritize force preservation over objective completion speed. Appropriate for armies the player cannot afford to lose and situations where the objective is valuable but not urgent.

**Economical** — Minimize resource expenditure alongside objective completion. Avoid costly sieges that can be bypassed in favor of flanking approaches. Accept slower progress to avoid engagements whose cost exceeds the immediate value of their outcome. The Goldgrave Commander disposition — warfare as cost-benefit calculation, not as dominance expression.

**Relentless** — Do not stop. Do not rest for extended periods. Do not allow routed enemies to recover before re-engaging. Pursue the directive to completion without consolidation pauses. This modifier generates higher army attrition over time and accelerates the supply consumption rate but produces faster objective completion than any other modifier. Appropriate for time-sensitive strategic objectives where speed matters more than the army's condition afterward.

**Disciplined** — Honor all standing conviction constraints and diplomatic agreements even when violating them would benefit the immediate objective. Do not generate Cruel conviction events through battlefield behavior the player did not explicitly authorize. Do not attack targets covered by active diplomatic agreements even if militarily convenient. Appropriate for Moral conviction dynasties whose Commanders must maintain the dynasty's behavioral record even when the player is not watching.

**Opportunistic** — Execute the primary directive but act on significant opportunities that emerge within the operational area even if they are outside the directive's strict scope. A Commander with this modifier who is executing Secure This Resource notices that a rival's important caravan is passing within engagement range and takes it, even though the directive did not specify caravan interdiction. This is a high-autonomy modifier that requires strong trust in the Commander's judgment — it produces creative results when the Commander is capable and unpredictable results when they are not.

Attitude modifiers stack with the Commander's personal profile. A bloodline Commander with an aggressive tactical history executing a directive under a Cautious modifier will be more aggressive than a cautious Commander under the same modifier. The modifier shifts the Commander's behavior but does not override their developed character. The player is influencing, not replacing, the Commander's judgment. An AI Field Commander under any modifier executes that modifier more literally and less adaptively than a bloodline Commander — they follow the modifier's rules without the nuance that a real tactical personality would apply.

---

### THE DIRECT CONTROL TOGGLE

At any point during the match, the player can switch any army from Commander direction to direct player control. The toggle is immediate. The transition is seamless. The army was under Commander direction — it is now under the player. The battle view opens at the current state of the engagement. The player inherits whatever tactical situation the Commander has produced up to that moment: their positioning, their current engagements, their supply status, the morale state of their squads.

If the player switches to direct control of an army that is mid-engagement under a Commander, they do not restart the battle. They enter it in its current state and take over from that position. A battle going badly under Commander direction can be rescued by the player taking control — if the player is good enough at the tactical layer to recover the situation from a disadvantaged position. A battle going well under Commander direction can be finished personally by the player if they want the satisfaction of the close or the specific tactical outcome that only direct control can produce.

When the player releases direct control — by choosing to return to the strategic layer or by the engagement concluding — the Commander resumes authority under the last active directive. If the player took direct control mid-battle and the battle has not concluded, the Commander picks up from the current tactical position and continues executing. The player can drop in and out of any battle as many times as they choose in a single engagement.

The Commander does not disappear during the player's direct control window. They remain present in the battle as a passive morale anchor and command aura provider — the units still respond to the Commander's reputation and presence even while the player is making the tactical decisions. The Commander's passive bonuses apply. Their presence continues to generate the morale effects and conviction-alignment bonuses that come from having a capable bloodline figure leading the force.

This means taking direct control of a strong bloodline Commander's army is a different experience from taking direct control of an AI Field Commander's army — the passive effects that came from the Commander's presence continue. The player is adding their tactical control on top of the Commander's passive amplification.

---

### COMMAND CAPACITY AND THE OVERWHELM MECHANIC IN FULL

There is no numerical cap on how many armies the player can have active in the match simultaneously. There is no interface that tells the player "you have reached your command limit." No hard wall exists.

What exists is a different kind of constraint. The player's personal attention is finite. When they are in direct control of a battle, every other army in the match is operating under Commander direction. If those Commanders are bloodline members with strong development, matched to their missions, operating under precise directives with appropriate attitude modifiers, those armies will perform well without the player. If those Commanders are AI Field Commanders with vague directives and mismatched attitudes for the situations they face, those armies will produce suboptimal outcomes on their fronts.

The player who has twelve active armies and is personally fighting one battle is trusting eleven Commanders to run eleven fronts correctly. If those eleven Commanders are capable and their directives are current and appropriate, the player is running a genuine empire. If those eleven Commanders are undermanned or their directives have become mismatched to conditions that changed since the last strategic window, those eleven fronts are producing degraded outcomes, and the player will discover this at the next declaration.

The bloodline member Commander investment is what makes large-scale empire management viable. A player who has developed military-oriented bloodline members and deployed them as Commanders across the match's most important fronts has genuine delegates — capable people doing important things in important places on the player's behalf. A player who committed all their bloodline members to governance, faith, and diplomatic roles has capable people doing important things elsewhere and AI Field Commanders of generic capability on every military front. That is a real strategic tradeoff with real consequences at match scale.

The player who attempts to personally command everything in a large empire will not be stopped. They will simply discover, through failures on the fronts they are not attending, that attention is the scarcest resource in the game. Every battle they fight personally is a battle they are not watching elsewhere. Every strategic window they spend in one region is time not spent reviewing the situation on another front. The empire that grows large enough to be genuinely difficult to manage is the empire that is playing Bloodlines at the scale it was designed for. The overwhelm is the point. The Commander system is the infrastructure that lets a player manage that overwhelm intelligently rather than by simply having less.

---

### COMMANDER DEVELOPMENT OVER THE MATCH

Bloodline member Commanders develop in ways that AI Field Commanders do not. This development is one of the primary returns on the investment of assigning bloodline members to military roles across the match's full arc.

A bloodline Commander gains campaign experience from each engagement they lead — whether player-directed or operating under a directive. The experience is not a simple counter. It is a qualitative record of what kind of engagements they have led, what outcomes they produced, what tactical situations they navigated successfully and which ones cost them. A Commander who has led twelve successful defensive engagements has developed specific expertise in defensive situations. A Commander who has led offensive campaigns across three different terrain types has developed flexibility. A Commander who has never led a siege and is now assigned an Encircle and Starve directive is operating outside their experience, and the directive execution will reflect it.

Faith intensity deepens naturally for bloodline Commanders through sustained military service. A Commander who has led armies through campaigns of significant consequence, who has watched soldiers die and made the decisions that caused it, develops a relationship with their covenant's theology that is different from a Commander who has only administered in safety. High-faith bloodline Commanders carry that intensity into the armies they lead — it affects the morale of faith-aligned troops around them and generates passive faith intensity in their operational area.

Conviction shapes and is shaped by the Commander's tactical record. A bloodline Commander who has consistently honored prisoner status, maintained supply agreements with civilian populations, and operated within the dynasty's established conviction posture develops a Moral conviction record that is their own — separate from but aligned with the dynasty's overall conviction axis. A Commander given Cruel directives and operating under attitudes that generate conviction costs over multiple engagements will develop a Cruel personal record that persists as part of who they are, even if the dynasty's overall axis is more Moral. The Commander's conviction history affects how their troops respond to them and what the dynasty's behavior registers as at the level of the armies they command.

The Legendary Member emergence event — documented in the Political Events System — can occur to bloodline Commanders who have accumulated sufficient development across all these dimensions. A Commander whose engagement history, faith depth, conviction coherence, and specific notable achievements cross the Legendary threshold becomes a match-world significant figure. Other dynasties know who they are. Their presence at the head of an army generates diplomatic signals. They become assassination targets for sophisticated rival operations players. Their death in battle is registered as a major match event. The investment in developing a capable bloodline Commander to Legendary status is among the highest-return strategic investments a player can make — and losing that figure to an engagement they attended personally, or to an operation that succeeded because their protective army had a vague directive, is among the match's most consequential failures.

---

### HOW THE COMMANDER SYSTEM AND THE TIME MODEL INTERLOCK

The Commander system and the declared time model are two halves of the same solution to the same problem.

The declared time model makes the dynasty game real by advancing in-world time meaningfully between battle engagements. The Commander system makes the military game manageable across a large empire by giving the player genuine delegates rather than abstract AI behavior. Together they produce the specific experience Bloodlines is designed to create: a player fighting a real battle in the foreground while a real empire runs — imperfectly, according to the quality of the Commanders and directives the player has established — in the background.

During the strategic layer commitment phase, the player reviews their Commanders and directives. The events queue has told them what changed during the last batch of declared time. The armies that were executing directives have produced outcomes — territories held or lost, operations advanced or complicated, resources secured or contested. The player adjusts directives that no longer match the situation. They reassign Commanders whose positions have changed. They initiate operations that the elapsed time has made feasible. They commit movement orders for armies that need to reposition before the next major personal engagement.

The rogue that was navigating into enemy territory has advanced to a new position. The player checks the operative's status, sees that they are within range of the target but the lunar timing is still several months away, and adjusts the support directive on the nearby army to account for a new patrol route the intelligence reports identified. Then the player drops into the next battle.

This is the game. These are the decisions that matter. The battle is dramatic. The strategic layer is where those battles are won and lost before they happen.

---

## ADDENDUM SECTION 3: INTEGRATION NOTES FOR NEXT BIBLE VERSION

### Section 45 — Match Stages

Section 45 is fully superseded by BLOODLINES_FIVE_STAGES.md. The five stages are: Stage One (Roots), Stage Two (Reach), Stage Three (Reckoning), Stage Four (Dominion), Stage Five (The Grand Culmination). The old four stage names are retired to the Historical Design Archive.

### New Section — The Declared Time Model

Insert as new section in Part VIII (Match Structure and Victory), following the Five-Stage Architecture section. This document's Addendum Section 1 is the complete text.

### New Section — The Commander System

Insert as new section in Part VI (Military Systems), following Section 38 (Army Composition and the Recruitment Slider System). This document's Addendum Section 2 is the complete text. Section number to be assigned based on final integration sequence.

### Cross-Reference Updates Required After Integration

Section 36 (Siege and Naval Warfare) — references to siege mechanics should cross-reference the Siege Directives section of the Commander System.

Section 37 (The Bloodline Member in Combat) — the Commander System's treatment of bloodline member Commanders extends the content of this section and should be cross-referenced.

Section 38 (Army Composition and the Recruitment Slider System) — the Commander System's note about command capacity interaction with army composition should be cross-referenced here.

Sections 63-68 (Operations System) — the rogue and mystic time model described in Addendum Section 1 extends the operations content and should be cross-referenced.

Section 46 (Match Scale, Pacing, and Recovery Mechanics) — the 90-second cycle description should be updated to reference the declared time model as the match's dual-clock architecture.

Section 12 (Bloodline Member Functional Roles) — the War General role description should cross-reference the Commander System's expanded treatment of bloodline member command.

Section 47 (Political Events System) — the Legendary Member Emergence event should cross-reference the Commander System's treatment of Commander development as a path to Legendary status.

---

## ADDENDUM SECTION 4: UPDATED OPEN DESIGN WORK QUEUE

The following items are open for development in future sessions. This list reflects the current state after this session's work and supersedes the open queue in BLOODLINES_CLAUDECODE_HANDOFF.md.

**The declared time declaration table** — specific in-world time elapsed per engagement type, indexed to stage, engagement size, and engagement outcome. This is the primary lever for controlling dynasty pacing and needs precise values before the time model is fully specified.

**The strategic layer UX** — how the player moves through the events queue, commits orders, and reviews operation progress. The design principle is clear; the interface architecture that makes it feel fluid rather than procedural needs detailed design.

**The shared multiplayer timeline architecture** — exactly how the match calendar advances, how individual players' battle timing feeds into it, and what happens when a player is in the strategic layer while an opponent initiates an engagement affecting their territory.

**Operation timer definitions** — how many in-world months or years each class of covert and faith operation requires at each stage of execution, so that players can plan around them with precision.

**Commander development attribute definitions** — the specific attributes that track bloodline Commander development (campaign experience, tactical profile, faith depth, conviction record) and how they interact numerically with directive execution quality and command capacity.

**Victory condition thresholds** — unchanged from prior queue. All four unspecified thresholds need design decisions: Faith Divine Right global faith share percentage, Currency Dominance adoption threshold, Dynastic Prestige victory total, Territorial Governance loyalty percentage and coverage definition.

**Section 62** — unchanged from prior queue. What belongs in the missing section?

**Section 69 completion** — unchanged from prior queue. The closing design philosophy statement needs its full text restored.

**Hartvale unique unit** — unchanged from prior queue. CB002 Verdant Warden vs CB004 Hearthmasters. Requires Lance's decision.

**Naval doctrine detail** — fleet combat mechanics, naval engagement rules, and naval operation integration with the operations system.

**Secondary continent tribe profiles** — specific archetypes, names, and characteristics for tribes on secondary continents who have no prior founding house relationship.

**Foothold mechanics detail** — specific building requirements, garrison thresholds, development timelines, and vulnerability mechanics for secondary continent footholds.

**Water infrastructure costs** — specific resource costs, build times, capacity values, and prerequisite chains for aqueducts, cisterns, irrigation networks, and desalination infrastructure.

---

*End of Bible Addendum A*
*All content in this document is new canonical material for Bloodlines v3.1 and subsequent versions.*
*Addendum A covers: The Declared Time Model, The Commander System, Five-Stage Integration Notes, Updated Open Design Queue.*
*Document prepared: 2026-04-07*
*Lance Fisher is sole authority on canon, deprecation, and removal.*
*Suggested file name for project directory: BLOODLINES_ADDENDUM_A.md*
