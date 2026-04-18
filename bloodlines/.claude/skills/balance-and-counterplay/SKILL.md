---
name: balance-and-counterplay
description: Invoke when reviewing or proposing a new unit, house, faith doctrine, conviction mechanic, fortification tier, or match-structure element for Bloodlines. Checks whether the element can be countered, whether it collapses any canonical victory path, whether it creates a dominant strategy, and whether it serves the strategy-depth product quality metric. Trigger phrases: "review balance of", "is this balanced", "counterplay", "dominant strategy", new unit or faith design proposal, "does this break X".
---

# Balance and Counterplay Skill (Bloodlines)

The product quality metric for Bloodlines is strategy depth, balance, replayability, and multiple viable playstyles across the canonical victory paths in `data/victory-conditions.json`. This skill checks a proposed element against that standard.

## What to load before running

1. `data/victory-conditions.json` — the canonical victory paths. Every balance check must confirm that at least one path is not made strictly non-viable.
2. `06_FACTIONS/FOUNDING_HOUSES.md` — the founding houses and their canonical asymmetry profiles. A new unit or doctrine must not eliminate the asymmetry that makes a house distinctive.
3. `07_FAITHS/FOUR_ANCIENT_FAITHS.md` — the four faiths and their canonical conviction-band interactions.
4. `04_SYSTEMS/CONVICTION_SYSTEM.md` — conviction bands and the reward/risk structure that governs faith-driven gameplay.
5. The specific `04_SYSTEMS/*.md` doc for the subsystem the proposal touches (e.g. `TERRITORY_SYSTEM.md` for fortifications, `RESOURCE_SYSTEM.md` for economic mechanics).

## The five counterplay questions

Ask all five. Record the answer for each.

### 1. What does the opponent do about this?

Name at least two concrete opponent responses that are accessible mid-game without requiring one specific tech or house. If no accessible counter exists, this is a balance risk.

### 2. What does the player give up to use it?

Every strong element must have a real opportunity cost. Identify: resource cost, time cost, tech path locked out, or positional/strategic constraint. "It costs gold" is not sufficient unless the gold cost represents a meaningful strategic choice. If the element has no meaningful opportunity cost, this is a balance risk.

### 3. Is there a cheese line?

A cheese line is any strategy that:
- Wins before the opponent can respond, given correct opening play.
- Bypasses more than two canonical systems simultaneously (e.g. skips economy, skips conviction, skips fortification buildup).
- Requires the opponent to have scouted it specifically to counter it.

If a cheese line exists, flag it. The element may still be viable with a timing or cost adjustment.

### 4. Is there a hard counter that collapses agency?

A hard counter is not a problem. A hard counter that makes the proposed element unplayable in all contexts is a problem. The test: can the proposed element still contribute meaningfully when its hard counter is present? If no, the design is fragile and needs a secondary role or a contingency.

### 5. Does this force a single optimal strategy?

If using this element makes one build order or tech path strictly dominant (higher win rate, lower decision cost, smaller skill gap), it collapses strategic variety. Check: after adding this element, can a player win without it in the same context? Can a player win against it without using the hard counter? If both are yes, the element is probably balanced.

## Victory path collapse check

Open `data/victory-conditions.json`. For each canonical victory path:

- Military domination
- Economic supremacy
- Faith ascendancy
- Dynastic legitimacy
- Territorial control
- Alliance network

Ask: "After adding this element, is this path still a viable path to victory against a competent opponent?" If the answer for any path is "No, because this element directly or indirectly makes this path impossible or requires a specific response this path cannot provide," that is a BALANCE CONFLICT.

## Report format

Return exactly one of:

**BALANCE PASS**: "No balance hazards found. Counterplay: [two options]. Opportunity cost: [specific cost]. No cheese line identified. All six victory paths remain viable."

**BALANCE RISK**: "Risk: [description]. Path/playstyle affected: [which one]. Proposed adjustment: [specific change — cost, timing, constraint, or secondary role]. If adjustment is not made, [specific downstream consequence]."

**BALANCE CONFLICT**: "Conflicts with canonical victory path [path name] and/or house asymmetry [house name]. The conflict is [specific description]. This must be resolved before this element merges. Options: [two or more resolution paths that preserve the canonical conflict structure]."

## What this skill does NOT do

- Does not validate code correctness (unity-ecs-discipline).
- Does not validate ECS performance (performance-and-scale).
- Does not check canon source compliance (canon-enforcement).
- Does not design new systems from scratch — it reviews proposals.
- Does not comment on graphics or art fidelity.
