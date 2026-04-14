# Development Reality Report

## Current Reality

Bloodlines was not a game project when this session began. It was a documentation-heavy design archive inside a static website folder. The authoritative project root is `deploy/bloodlines/`, not the mirror `bloodlines/` folder at repository root.

What existed was real and valuable:

- A substantial canon corpus.
- A working archive viewer.
- An archival workflow.
- Strong design intent spanning RTS combat, dynastic simulation, faith, conviction, operations, trade, and late-game sovereignty pressure.

What did not exist:

- A playable prototype.
- Engine code.
- Runtime gameplay data.
- Tests for gameplay assets or systems.
- A shipping-oriented roadmap.

## What Currently Exists

### Runtime surfaces

- `index.html` is a password-gated archive viewer that fetches Markdown documents and can read/write idea inbox data through `api/submit.php`.
- `play.html` is now the first playable RTS vertical slice added in this session.

### Authoritative design and canon sources

- Primary active canon snapshot: `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md`
- Full integrated continuity reference: `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md`
- Canon lock and supersession record: `01_CANON/CANONICAL_RULES.md`
- Historical session memory and preserved alternatives: `01_CANON/BLOODLINES_MASTER_MEMORY.md`

### Detailed subsystem files

- Core systems are documented in `04_SYSTEMS/`.
- Match flow, naval, and political systems are documented in `11_MATCHFLOW/`.
- Operations, diplomacy, and strategic actions are documented in `08_MECHANICS/`.
- Houses, faiths, units, world generation, lore, UI, and A/V direction have their own preserved files.

## What Is Playable

The first playable vertical slice now exists in `play.html`.

Current playable scope:

- Launch scene / menu
- One skirmish map
- Ironmark as the first playable house
- WASD or arrow-key camera movement
- Left-click selection and box selection
- Right-click move, gather, and attack commands
- Worker gathering for gold and wood
- Passive food and water economy through Farms and Wells
- Housing-driven population cap and pooled population growth
- Building placement from Workers
- Command Hall worker training
- Barracks infantry training
- Basic enemy economy/combat AI
- Placeholder win/loss conditions
- Debug HUD with resources, population, selection state, and house name

## What Is Only Documented

Large, still-non-runtime systems remain document-level only:

- Strategic layer and declared time model
- Commander delegation system
- Full bloodline family tree gameplay
- Marriage, succession, capture, ransom, enslavement
- Faith divergence beyond data stubs
- Conviction consequences beyond data stubs
- Operations and covert play
- Trueborn rise arc
- Neutral trade hub gameplay
- Naval warfare
- AI kingdoms and minor tribes at full design depth
- Large-map province governance and late-match sovereignty mechanics

## Mechanics Already Partially Implemented

- RTS resource loop: partially implemented
- Population/housing support loop: partially implemented
- House data model: partially implemented
- Basic military production and combat: partially implemented
- Enemy AI: partially implemented
- Victory condition structure: data-defined but not fully implemented
- Faith, conviction, bloodline roles: data-defined stubs only

## Authoritative Files

Use these in order:

1. `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md`
2. `01_CANON/CANONICAL_RULES.md`
3. `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md`
4. `01_CANON/BLOODLINES_MASTER_MEMORY.md`
5. System-specific files under `04_SYSTEMS`, `08_MECHANICS`, `10_UNITS`, `11_MATCHFLOW`, `12_UI_UX`, and related folders

## Older But Still Valuable

- `01_CANON/BLOODLINES_MASTER_MEMORY.md` preserves historical tensions, abandoned ideas, and earlier branches that still matter as design reservoir.
- `19_ARCHIVE/LEGACY_ARCHIVE_INDEX.md` references pre-project source material that seeded the formal archive.
- Earlier sections inside the system files remain historically valuable even where later canon supersedes them.

## Key Contradictions and Unresolved Design Tensions

### Explicitly resolved by later canon

- Resource count: older five-primary model versus later six-primary model with iron. Active canon is six primaries plus influence as advanced/political resource.
- Match progression: older four-stage structure versus later five-stage structure. Active canon is the five-stage model.
- Victory paths: older six-path model versus later five-path model. Dynastic Prestige is now a modifier system, not a standalone victory condition.
- House profiles: CB004 strategic detail for eight non-Ironmark houses was later voided. Only Ironmark remains fully locked strategically.
- Scouts: separate scout unit was removed in settled canon. Swordsmen hold the early reconnaissance role, while `Scout Rider` exists later in the unit ladder.

### Still unresolved or intentionally open

- Exact numerical thresholds for most victory conditions
- Full strategic identities for the eight non-Ironmark houses after CB004 voiding
- Detailed commander implementation values and declaration table values
- Full strategic-layer integration order for dynastic, faith, and operations systems

## Minimum Playable Vertical Slice

The minimum vertical slice for Bloodlines is not a dynasty manager. It is a readable RTS battle economy that already feels like Bloodlines rather than a generic prototype.

Minimum acceptable slice:

- One house with real identity: Ironmark
- One map with expansion pressure
- Workers, housing, gold, wood, food, water
- Barracks and infantry loop
- Combat, selection, responsiveness, attack waves
- Clear HUD and debug visibility
- Data-first content organization

That slice now exists.

## Recommended Production Path

1. Harden the current battle-layer slice until it is stable, legible, and fun.
2. Expand from one-house Ironmark into subtle shared-roster asymmetry for the remaining houses without violating the 2026-04-07 canon reset.
3. Add province/territory ownership and loyalty before deep dynasty UI.
4. Add bloodline member commander/governor roles next, because they are the bridge between RTS and dynasty simulation.
5. Add faith selection and low-tier faith pressure before deep doctrine divergence.
6. Add the post-battle strategic layer and declared-time loop after the live tactical layer is reliable.
7. Add operations, neutral hub, Trueborn rise, AI kingdoms, and long-match scaling in that order.

## Production Recommendation

Do not restart Bloodlines in another stack right now. The repository is a static site, the archive already lives here, and the fastest path to a playable foundation is a no-build browser RTS using ES modules, JSON data, and canvas plus DOM HUD. That is the path now established in this session.

## New Convergence Documents

To prevent Bloodlines from drifting back into endless design expansion, two production-governance files now sit alongside this report:

- `docs/DEFINITIVE_DECISIONS_REGISTER.md` lists the design and product decisions that must be explicitly locked.
- `docs/COMPLETION_STAGE_GATES.md` defines what must be true before the project can be treated as entering completion-stage production.
