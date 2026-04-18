---
name: rts-pattern-review
description: Invoke when designing or reviewing a new resource system, production chain, unit role, build order, economy mechanic, or any Bloodlines system that governs how players allocate time and resources during a match. Detects RTS-specific dysfunction patterns: resource inflation/deflation, dominant strategies, unit role overlap, build-order imbalance, degenerate playstyle traps, idle-worker waste, and APM-gated wins. Trigger phrases: "review economy", "review unit roles", "review build chain", "is this a dominant strategy", new resource or production mechanic, "economy balance".
---

# RTS Pattern Review Skill (Bloodlines)

Bloodlines is a long-form dynasty RTS where match length spans 30-120 minutes. Systemic dysfunction in economy or unit roles does not manifest immediately — it compounds over the match arc. This skill detects the patterns that produce broken late-games, NPE spirals, or APM-dependent wins.

This skill focuses on systemic patterns. For single-element balance (one unit, one faith), use balance-and-counterplay instead.

## Pattern 1: Resource inflation / deflation spiral

**Inflation:** A resource income scales faster than the ability to spend it. Result: players accumulate uncapped reserves. Strategic decisions about resource management disappear. Late-game becomes a spam war with no tradeoffs.

**Deflation:** A resource is chronically scarce even with optimal play. Result: players are locked out of entire tech trees. One early mistake becomes a permanent handicap. Catch-up mechanics are blocked.

**Check:** At 30 minutes, 60 minutes, and match-end with correct play:
- Can the player spend all generated income of the primary resource (food, gold, wood, faith)?
- Is there at least one income-vs-spending tradeoff decision every 5-10 minutes?
- Does population growth track resource income growth, or does one outpace the other?

**Red flag:** Any resource that has no believable spending ceiling before the end of the match, or any resource that drops to zero for more than 3 minutes of correct play.

## Pattern 2: Dominant strategy emergence

A dominant strategy is one where:
- It has a higher expected win rate than all alternatives against a similarly-skilled opponent.
- It is accessible early (does not require late-game tech).
- Executing it correctly requires less strategic decision-making than countering it requires.

The signal: if a skilled player would always choose the same opening across all matchups and all map types, a dominant strategy likely exists.

**Check:** Is there a build order or faction-specific tech path that beats all opponents if executed correctly, regardless of what the opponent does? If yes, that is a dominant strategy.

## Pattern 3: Unit role overlap erasing meaningful choice

RTS unit roles should be distinct: anti-infantry, anti-armor, siege, support, scout, raider, line unit. When two units share the same role at similar cost, one becomes strictly better and the other disappears from play.

**Check:** For each proposed unit, state its primary role and its secondary role. Then ask: is there another unit in the same faction with the same primary role at equal or lower cost? If yes, either the new unit needs a distinct secondary role that the existing unit lacks, or the existing unit needs differentiation.

## Pattern 4: Build-order imbalance

A rush build order is intended to be strong early and weak late. A turtle build order should be strong late and weak early. The tension between them creates the strategic layer.

If a rush build is also strong late, or if a turtle build becomes viable early (because the turtler can produce units faster than the rusher can attack), the rush-turtle-boom triangle collapses.

**Check:** Identify the earliest pressure timing for the proposed system. Identify the earliest viable defense timing. If the defender can be fully prepared before the attacker can arrive with meaningful force, the rush line is unviable (too slow). If the attacker can arrive with force before any meaningful defense is possible, the rush line is dominant (too fast). The gap between "earliest attacker readiness" and "earliest defender readiness" should be less than 90 seconds in game time.

## Pattern 5: Kiting, blobbing, turtling traps

**Kite trap:** One unit type beats all others if microed correctly, regardless of composition. Result: APM decides the match, not strategy.

**Blob trap:** The optimal strategy is always to produce maximum units of one type and attack-move. Result: composition decisions disappear.

**Turtle trap:** Defensive upgrades stack to the point where an entrenched defender cannot be dislodged by an equal-resource attacker. Result: all matches go to the income-advantage win condition with no agency for the disadvantaged player.

**Check:** For each proposed element, identify whether it enables any of these three traps. A Bloodlines fortification system specifically risks the turtle trap; cross-reference `04_SYSTEMS/TERRITORY_SYSTEM.md` for the canonical siege and fortification-reduction mechanics.

## Pattern 6: Idle-worker waste

If the correct optimal play requires players to manually reassign workers continuously (or lose significant income if they do not), this is an APM-gated win condition. Strategy-depth games require strategic decisions, not mechanical precision.

**Check:** Is there an idle-worker penalty? Is there an automatic-return mechanic? Can the game be played at the strategic level without per-worker micro every 30 seconds?

## Pattern 7: APM-gated vs strategy-gated wins

APM-gated wins mean higher-APM players win regardless of strategic quality. Strategy-gated wins mean better macro/strategic decisions win even at lower APM ceilings.

Bloodlines targets strategy-gated wins. The target is: a player executing 50 actions per minute but making better strategic decisions should beat a player executing 150 actions per minute making poor strategic decisions.

**Check:** Can a player who queues all production, issues broad attack-move orders, and manages at the army-group level (not unit level) be competitive? If not, the game is APM-gated.

## Report format

For each pattern checked, state one of:

- `CLEAR`: "No [pattern name] detected. Reasoning: [one sentence]."
- `RISK`: "Risk: [pattern name]. Trigger: [specific mechanic or cost]. Proposed mitigation: [concrete change]."
- `UNRESOLVABLE`: "Pattern [name] cannot be assessed without implementation-level data. Flag for post-implementation review after [specific milestone]."

## What this skill does NOT do

- Does not check single-element balance in isolation (balance-and-counterplay).
- Does not validate C# code (unity-ecs-discipline).
- Does not validate canon compliance (canon-enforcement).
- Does not produce designs — it reviews them.
