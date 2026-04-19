# Bloodlines Owner Direction - 2026-04-19

Owner: Lance Fisher
Supersedes: None as a hard replacement. This file amends both
`governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md` and
`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md`
by narrowing the shipping game-mode set, lowering the graphics ceiling
to an AI-asset-driven baseline, and naming a new dynasty progression
system. Older guidance that contradicts this file is overridden on the
specific points called out here; the rest of the prior direction
remains active.

This file records an active non-negotiable project direction for
Bloodlines. It is additive governance.

## Shipping Game Modes

Bloodlines ships with two game modes only:

- **Skirmish vs AI.** Single-player and any-number-of-AI matches
  against the canonical AI on canonical maps with the full canonical
  ruleset. AI behavior matches or exceeds the browser reference
  simulation. The full canonical mechanical scope must be reachable
  inside skirmish: every faction, every faith, every doctrine path,
  every victory path, every late-game arc, every dynasty interaction.
- **Multiplayer.** Netcode for Entities, full canonical ruleset,
  canonical maps. Same mechanical scope as skirmish. Lobby and
  matchmaking infrastructure ships. Spectator support is in scope
  if it is reachable from the canonical match flow without breaking
  Netcode determinism.

Both modes share one canonical match-structure pipeline (the same
five-stage match progression already documented in the canon).

## Explicitly Removed From Scope

The following are removed from the active shipping plan. Remove them
from any roadmap, prompt, planning document, or implementation backlog
that still lists them as in-flight work:

- **Campaign.** No story campaign, no scripted scenarios, no narrative
  mission chain. The canon's narrative emerges from skirmish and
  multiplayer matches via dynasty arcs, faith events, and world
  pressure escalation. There is no scripted single-player narrative
  layer to build.
- **Tutorial.** No interactive tutorial mode. The UI must teach the
  game inline via tooltips, panel labels, contextual readouts, and
  good information density. Onboarding is the responsibility of the
  HUD and the canonical match-flow surfaces, not a separate tutorial
  mode. This overrides the 2026-04-17 direction's mention of "full
  polish across onboarding [and] tutorials" -- onboarding remains a
  polish target, tutorials do not.

These removals are not "deferred for later." They are out of scope.
If a campaign or tutorial returns to scope, it requires an explicit
new owner direction file from Lance.

## Graphics Direction: AI-Created, Below Or At Generals Zero Hour

The 2026-04-17 ceiling clamped graphics to Generals Zero Hour /
Warcraft III era polish. This file reaffirms that ceiling as a hard
upper bound and clarifies the production path:

- **Asset source.** Graphics are produced primarily by AI generation
  pipelines. Human art direction stays operator-owned; production is
  AI-driven. Hand-authored art is acceptable when AI output is
  insufficient for a specific canonical-identity asset (faction
  silhouettes, faction crests, hero unit silhouettes), but is not the
  default path.
- **Quality ceiling.** Generals Zero Hour fidelity is the upper
  bound, and the actual delivered fidelity may run somewhat lower
  than that benchmark. "Maybe even less" than Zero Hour is acceptable
  if it preserves silhouette clarity at RTS zoom and faction
  legibility. Strategy depth is not traded for graphics.
- **All earlier "no PBR / no HDRP / no ray tracing" rules from the
  2026-04-17 direction remain in effect.** AI generation does not
  unlock AAA pipelines; it makes the era-appropriate ceiling cheaper
  to reach.
- **No stylistic regression below silhouette legibility.** AI-generated
  assets that fail the legibility test at RTS zoom are rejected even
  if they look richer in close-up.

If an asset proposal exceeds the Zero Hour ceiling or requires
pipelines (PBR, HDRP, ray tracing, motion capture, AAA animation
networks) that the 2026-04-17 direction forbade, decline it. This
file does not loosen those constraints.

## Dynasty Progression System

A new persistent progression system is canonically in scope. This
system makes losing matches still rewarding and gives top-performing
dynasties durable identity rewards.

Design intent (mechanical specifics to be determined):

- **Cross-match XP for top dynasties.** Dynasties that perform well
  across matches accrue progression XP. "Top" is not strictly
  first-place: placing strongly across the canonical victory paths,
  surviving deep into a match, accumulating renown via canonical
  dynasty member arcs, and similar achievements all contribute. A
  match that does not end at #1 can still award meaningful
  progression to the player's dynasty.
- **Tier-based bonuses.** Progression unlocks tiers. Tiers grant
  bonuses that interact with the canonical mechanical surface. Bonus
  shape is not a flat power increase; bonuses are *options* the
  player can configure into the dynasty for the next match.
- **Example: dynasty-special-unit swaps.** A canonical example of
  the bonus shape: at a tier, the player may swap one dynasty-
  specific special unit for a different dynasty-specific special
  unit available to that house's progression options. This preserves
  faction identity while expanding strategic variety. Other bonus
  shapes (different doctrine entry slots, alternate building
  prerequisites, alternate starting council compositions, alternate
  faith doctrine starting commitments) are reserved for design
  exploration.
- **Multiplayer-fair.** The progression system must not produce a
  meta-power gradient that disadvantages new players in multiplayer.
  Bonuses are sideways trades and customization options, not strict
  upgrades. Concrete balancing rules are a future canon update.
- **Replayability driver.** The progression system is one of the
  primary tools used to satisfy the 2026-04-17 direction's
  "replayability" and "multiple playstyles" quality gates. Different
  dynasty tier configurations should produce visibly different
  optimal play across matches.

Implementation timing is not bound by this file. The system is
canonically in scope, but the order in which it is built relative to
the rest of the Unity slice plan is open.

## Implications For Active Work

- The Unity migration plan continues. Sub-slices keep porting the
  browser reference until parity is reached.
- The match-flow surfaces (lobby, HUD, in-game panels) ship with the
  same fidelity ceiling as before, minus tutorial flows.
- Onboarding effort routes into HUD and tooltip work, not into a
  dedicated tutorial mode.
- Campaign-related backlog items, if any exist in older planning
  docs, are dropped without ceremony. They are not paused; they are
  removed.
- Any prior reference to "tutorial mode," "campaign mode," or
  "scripted scenarios" should be interpreted as historical context
  only.
- The dynasty progression system gets its own canon page and its own
  data file (path TBD; likely `data/dynasty-progression.json` or a
  similar canonical surface) when the design lands.

## Interaction With Prior Owner Direction

The 2026-04-16 owner direction still applies on:

- Unity 6.3 LTS with DOTS / ECS as the shipping engine.
- The browser runtime frozen as behavioral specification.
- `unity/` as the only active Unity work target.
- Wwise audio in scope.
- Netcode for Entities multiplayer in scope.
- No MVP framing, no phased gameplay release, no scope-cutting on
  gameplay.

The 2026-04-17 owner direction still applies on:

- Strategy depth, balance, replayability, multiple playstyles as the
  primary product quality gates.
- Graphics ceiling at Generals Zero Hour / Warcraft III era. (This
  file lowers the realistic delivered fidelity below that ceiling
  while keeping the ceiling intact.)
- Audio fidelity ceiling at the same era.
- UX information-density philosophy.
- All "out of scope" graphics and audio pipelines remain out of
  scope.

Where the 2026-04-16 direction lists "campaign" and "tutorials" as
polish targets, this file removes them. Where the 2026-04-17
direction mentions "tutorials" inside the UX polish list, this file
removes that line item; onboarding-via-HUD remains.

## Authoritative References

- Victory paths canonical list: `data/victory-conditions.json`.
- Match structure: `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine*.md`.
- Canonical design bible: `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`.
- Continuity prompt enforcing prior direction: `03_PROMPTS/BLOODLINES_UNITY_CONTINUITY_PROMPT_v3.md`.
- Prior owner direction (still active, interpreted through this amendment): `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`.
- Prior owner direction (still active, interpreted through this amendment): `governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md`.
