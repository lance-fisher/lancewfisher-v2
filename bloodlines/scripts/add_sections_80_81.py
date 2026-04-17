"""
Adds Sections 80 (Declared Time Model) and 81 (Commander System) to Design Bible v3.2.
Run: python scripts/add_sections_80_81.py
"""

import os

bible_path = "D:/ProjectsHome/Bloodlines/18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md"

part_xv = r"""

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
"""

old_footer = "\n---\n\n*End of Design Bible v3.2*\n*This document incorporates all design decisions through Session 12 (2026-04-07).*\n*Version 3.2 supersedes Version 3.1 for all active design purposes.*\n*The Version 3.1 document is preserved in 19_ARCHIVE/ per additive-only archival rules.*\n*Lance Fisher is the sole authority on canon, deprecation, and removal.*\n\n---"

new_footer = "\n---\n\n*End of Design Bible v3.2*\n*This document incorporates all design decisions through Session 13 (2026-04-07).*\n*Version 3.2 supersedes Version 3.1 for all active design purposes.*\n*The Version 3.1 document is preserved in 19_ARCHIVE/ per additive-only archival rules.*\n*Lance Fisher is the sole authority on canon, deprecation, and removal.*\n\n---"

with open(bible_path, 'r', encoding='utf-8') as f:
    content = f.read()

if old_footer in content:
    content = content.replace(old_footer, part_xv + new_footer)
    print("Footer found and replaced.")
else:
    content = content + part_xv + new_footer
    print("Footer not found, appended at end.")

with open(bible_path, 'w', encoding='utf-8') as f:
    f.write(content)

lines = content.count('\n')
print(f"Done. Bible is now {lines} lines.")
