---
name: canon-enforcement
description: Invoke before implementing any gameplay system, faction, house, unit, building, faith, conviction, dynasty, victory path, or match-structure change, or when evaluating a design proposal for Bloodlines. Checks the proposal against the design bible, the active owner direction files, and the no-scope-reduction non-negotiables. Use whenever a session is about to touch canonical gameplay or produce a slice handoff that needs a canon citation.
---

# Canon Enforcement Skill (Bloodlines)

Run this check before committing a gameplay change or writing a slice handoff. The job is to prevent silent drift from the design bible and the locked owner direction.

## What to load (in order)

1. `governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` — product quality metric, graphics fidelity ceiling.
2. `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md` — full-canon Unity delivery, no MVP, no scope cuts on gameplay.
3. The specific `04_SYSTEMS/*.md` subsystem doc that governs the system being touched (e.g. `CONVICTION_SYSTEM.md`, `DYNASTIC_SYSTEM.md`, `FAITH_SYSTEM.md`, `TERRITORY_SYSTEM.md`, `RESOURCE_SYSTEM.md`, `POPULATION_SYSTEM.md`, `BORN_OF_SACRIFICE_SYSTEM.md`).
4. For houses: `06_FACTIONS/FOUNDING_HOUSES.md`. For faiths: `07_FAITHS/FOUR_ANCIENT_FAITHS.md`.
5. For authoritative spec-level behavior: the matching function cluster in `src/game/core/simulation.js` or `src/game/core/ai.js`.

## Check the proposal against these non-negotiables

Pass criteria (ALL must hold):

- **No gameplay scope reduction.** No MVP framing. No "phase 1 smaller version." No "simplified for now." No "later." If the proposal defers a canonical mechanic, reject.
- **No victory-path collapse.** Canonical victory paths live in `data/victory-conditions.json`. The proposal may not eliminate, soft-merge, or make structurally impossible any listed path. New viable paths emerging from mechanical interactions must be preserved, not blocked.
- **No playstyle collapse.** If the proposal forces a single optimal strategy or makes offense/defense/faith/economic/territorial/alliance lines non-viable, reject.
- **No graphics escalation.** Per the 2026-04-17 owner direction, the fidelity ceiling is Zero Hour (2003) / Warcraft III (2002). Reject PBR, HDRP, ray tracing, motion capture, AAA animation pipelines, and similar escalations.
- **No browser-source extension.** `src/game/core/*.js` is frozen as behavioral specification. The proposal may read it; it may not add or modify systems there.
- **No silent drop of browser-era behavior.** If the browser implements X and Unity does not, the slice either ports X, documents a replacement that achieves the same canonical intent, or flags X for operator acknowledgement. Never skip silently.
- **Canonical depth preserved.** All canonical fields, buckets, bands, tiers, roles, paths, statuses, multipliers, and thresholds must round-trip from canon to implementation. No "we'll fill in the rest later."

## Report format

Always return one of exactly three results to the calling session:

- `CANON PASS`: "Proposal matches canon. Cites: `<doc>:<section>` and `src/game/core/simulation.js:<line>:<function>`." Include the exact citation strings so the slice handoff can paste them verbatim.
- `CANON CONFLICT`: "Proposal conflicts with `<doc>:<section>`. The canonical behavior is `<quoted canonical text>`. The proposal deviates by `<specific deviation>`. To proceed, either match canon or surface the deviation to the operator for explicit acknowledgement."
- `CANON GAP`: "No canonical source found for `<topic>`. The operator must be consulted before this slice lands. Do not implement from instinct."

## Citation format for slice handoffs

Every slice handoff must include both citations. Use this format:

```
Browser Reference: src/game/core/simulation.js:<line> <functionName>
Canon Reference: <doc-path>:<section heading>
```

## What this skill does NOT do

- Does not validate code correctness (that's the unity-ecs-discipline skill).
- Does not validate performance (that's the performance-and-scale skill).
- Does not evaluate balance or counterplay (that's balance-and-counterplay).
- Does not invoke validators. It only reads and reports.

## Trigger phrases to watch for

- "implementing <system>"
- "design for <faction / unit / faith / house>"
- "new <unit / building / mechanic>"
- "simpler version"
- "MVP"
- "phase 1"
- "for now"
- "later slice"
- "cut scope"
- "defer"

When any of these appear, run the check before code is written.
