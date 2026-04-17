# SESSION INGESTION: Match Structure, Time System, and Multiplayer Engagement Doctrine

Date: 2026-04-15
Source: Lance, provided during Session 82 as canonical prompt insert v1.0
Classification: Canonical, additive, nothing superseded

## Ingestion Context

This material was provided verbatim by Lance during Session 82. It locks behavior that was previously underspecified or inconsistently described across sessions. The document is additive: nothing in the existing Bloodlines Design Bible is superseded, condensed, removed, or reorganized by this ingestion. Conflicts between this material and prior canon must be flagged for explicit resolution.

## Raw Input

The full canonical text is preserved in the companion file:
`02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine_RAW.md`

## Summary of Canon Locked

### Part One: Five-Stage Match Structure

- Stage One: Founding (intimate, localized opening; home seat, food/water security, basic borders, geography exploration)
- Stage Two: Expansion and Identity (territorial expansion, faith covenant commitment, economy compounding, first rival contact)
- Stage Three: Encounter, Establishment, and the Neutral City (rival contact, forward bases, trade, Trueborn City as strategic variable)
- Stage Four: War and the Turning of Tides (sustained campaigns, siege operations, Great Reckoning, fortification doctrine payoff)
- Stage Five: Final Convergence (apex systems, Divine Right, coalition, final sovereignty)

Stages advance by readiness conditions, not timers. Three broader phases overlay the five stages: Emergence (S1-early S2), Commitment (late S2-S4), Resolution (S5).

### Part Two: Declared Dual-Clock Time Model

- Clock One (Battle Clock): real-time RTS combat, 20-40 real minutes per major engagement.
- Clock Two (Dynasty Clock): in-world historical time, aging, succession, operations, diplomacy.
- Declaration Seam: when battle ends, the game declares elapsed dynasty-clock time. Canonical ranges: 3-6 months for skirmishes, 1-2 years for major battles, 3-5 years for sieges.
- Session rhythm: Battle, Declaration, Events Queue, Commitment Phase, Next Battle.
- Internal processing heartbeat: ~90 seconds of game time for resource, population, faith, conviction, event processing.
- Mode-based temporal context, not a speed slider.

### Part Three: Live Strategic Layer

- World remains active when zoomed out. Army movement is icon-based at strategic scale.
- Prominent date display reinforces dynastic scale.
- Single-player pacing controls: normal, slow, pause-for-orders, auto-pause on major events.
- Multiplayer: shared live calendar, no individual pause, no speed slider.

### Part Four: Multiplayer Time and Engagement Doctrine

- Shared match calendar, no individual pause or speed control.
- Permitted pause forms: unanimous, majority vote, host/admin in private lobby, emergency tokens (limited, short, non-chainable).
- Commanders and delegation are critical in multiplayer (attention trade-off).
- Imminent Engagement Warning System:
  - Fires when hostile engagement becomes inevitable.
  - Provides a short live countdown (10-30 seconds depending on battle type).
  - Modifiers increase or decrease warning time (watchtowers, scouting, ambush, terrain).
  - Pre-battle command panel shows forces, commanders, composition, terrain, posture options.
  - Player choices: enter battle, delegate, change posture, swap commander, commit reinforcements, protect bloodline members.
  - Countdown is NOT a pause. World continues. Restricted to locally relevant actions only.
  - Timer expiry starts battle under current conditions.

### Part Five: Implementation Guardrails

Locked constraints for any implementing agent. Key items: no timer-based stages, no universal time ratio, no dynasty pause during battle, no combat interruption by ordinary events, strategic layer is not a pause menu, support 2-3 visible generations per match.

### Part Six: Master Prompt Insert Paragraph

Condensed inline version for future session prompts. Preserved in the raw file.

## Cross-Reference Against Existing Canon

The following existing surfaces touch the same design space and must be checked for alignment:

1. `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` - contains prior match-structure and time references
2. `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md` - integrated bible
3. `11_MATCHFLOW/MATCH_STRUCTURE.md` - existing match-flow documentation
4. `01_CANON/BLOODLINES_DESIGN_BIBLE.md` - core canon
5. `src/game/core/simulation.js` - already has `dualClock` state with `inWorldDays` and `declarations`
6. `data/realm-conditions.json` - 90-second cycle is the internal processing heartbeat
7. `04_SYSTEMS/SYSTEM_INDEX.md` - system registry

## Conflict Notes (flagged for resolution)

### CONFLICT 1: Stage count and naming (PARTIALLY RESOLVED)

The older `11_MATCHFLOW/MATCH_STRUCTURE.md` (2026-03-15) says "four strategic stages" with a Level 5 Apex beyond:
- Stage 1: Founding
- Stage 2: Consolidation
- Stage 3: Ideological Expansion
- Stage 4: Irreversible Divergence
- Level 5: Apex (beyond the four stages)

The new canonical material says five stages:
- Stage 1: Founding
- Stage 2: Expansion and Identity
- Stage 3: Encounter, Establishment, and the Neutral City
- Stage 4: War and the Turning of Tides
- Stage 5: Final Convergence

The fortification doctrine section (added 2026-04-14 to the same file) already uses the new five-stage naming, which means the five-stage structure was already partially canonical. The new material locks it definitively.

Resolution: the new five-stage naming supersedes the older four-stage structure. The older naming is preserved historically in the file but is no longer the canonical stage framework.

### CONFLICT 2: Faith commitment timing

The older match structure says: "At the end of this stage [Stage 1] the player selects one of the four ancient faiths."

The new canonical material says: "The faith covenant commitment arrives here [Stage Two] and is one of the most consequential decisions of the entire match."

And: "Faith commitment should not be rushed during Stage One."

Resolution: the new material explicitly places faith commitment in Stage Two and says it should not be rushed in Stage One. This supersedes the older "end of Stage 1" placement. Flag for Lance confirmation if needed, but the newer material is the more considered and detailed version.

### CONFLICT 3: Stage transition mechanism (ADDITIVE, NO CONFLICT)

The older material is silent on how stages advance. The new material explicitly says readiness conditions, no timers. This is additive, not conflicting.

### NO CONFLICT: Dual-clock model

The dual-clock model with `dualClock.inWorldDays` already exists in `src/game/core/simulation.js` (Session 26). The existing implementation tracks in-world days and declaration events. The new canonical material locks the architectural description of how the two clocks interact and defines the declaration seam, session rhythm, and canonical declaration ranges. The existing implementation aligns with the new material. No code changes required.

### NO CONFLICT: 90-second realm cycle

The new material names an "internal processing heartbeat, approximately every 90 seconds of game time." The existing `REALM_CYCLE_DEFAULT_SECONDS = 90` in `simulation.js` already implements this. Perfect alignment.

### NO CONFLICT: Multiplayer engagement doctrine

The multiplayer engagement doctrine (imminent engagement warning system, shared calendar, no individual pause) is entirely new material. No prior canon conflicts because this area was previously described as "not yet fully defined" in `11_MATCHFLOW/MATCH_STRUCTURE.md`.

### NO CONFLICT: Live strategic layer

The live strategic layer doctrine (world remains active at zoomed-out scale, icon-based army movement, date display) is new canonical material. The existing theatre zoom and strategic zoom references in prior canon align with this direction.

## Actions Taken

1. Raw input preserved verbatim in `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine_RAW.md`.
2. This summary and conflict analysis stored in the current file.
3. `11_MATCHFLOW/MATCH_STRUCTURE.md` to be updated with an additive Session 82 section noting the new five-stage lock and dual-clock architecture.
4. `01_CANON/CANON_LOCK.md` to be updated with the new canonical locks.
5. `04_SYSTEMS/SYSTEM_INDEX.md` to be updated referencing the new engagement warning and multiplayer time systems.
6. The design bible export (v3.4) is not updated in this session because a full bible re-export is a significant operation. The ingestion material and conflict notes are sufficient for continuity.
