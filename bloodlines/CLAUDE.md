# Bloodlines Project Governance

Bloodlines is a dynasty-driven strategy project where faith, sacrifice, and bloodline shape civilizations across a preserved long-form canon and an active implementation stack.

## Canonical Root

The one canonical Bloodlines session root is:

- `D:\ProjectsHome\Bloodlines`

Current filesystem reality:

- `D:\ProjectsHome\Bloodlines` resolves to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`.
- The parent path is a physical backing path, not a second project root.
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines` is preserved as a compatibility copy only.

## Session Startup

Before doing meaningful work, read:

1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md`
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md`
8. `SOURCE_PROVENANCE_MAP.md`

Then load subsystem detail from the numbered folders and `docs/` as needed.

## Preservation Rules

1. Nothing relevant may be deleted without explicit authorization.
2. Nothing relevant may be silently replaced with a thinner substitute.
3. Preserve all competing versions unless Lance explicitly authorizes reduction.
4. If a source differs, preserve both and document the relationship.
5. New information may extend the project, but it must not erase earlier context.
6. `archive_preserved_sources/`, `19_ARCHIVE/`, and `governance/` are preservation zones.

## Single-Root Rule

All future Bloodlines work should happen in this root unless Lance explicitly directs otherwise.

That includes:

- canon and design-bible work
- lore
- prompts
- planning and roadmap material
- runtime code
- tests
- Unity work
- imported outside Bloodlines source material
- session continuity files

Do not create new parallel Bloodlines roots elsewhere in the workspace.

## Storage Rules

- Canon and settled design: numbered design folders and `18_EXPORTS/`
- Raw session and prompt material: `02_SESSION_INGESTIONS/` and `03_PROMPTS/`
- Runtime code and tests: `src/`, `data/`, `tests/`, `play.html`
- Unity continuation: `unity/` and `docs/unity/`
- Outside source imports: `archive_preserved_sources/`
- Imported rule and governance surfaces: `governance/`
- Cross-session state: root continuity files and `continuity/`

## Scope Directive

Bloodlines has no imposed small-scope ceiling. The design should not be flattened to match the current runtime. The correct continuation model is to preserve the full vision and move implementation toward it in reviewable slices.

## Owner Direction Override

The active non-negotiable owner direction is recorded here:

- `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`

That direction supersedes any older Bloodlines prompt, handoff, plan, or roadmap that assumes MVP framing, phased release, or scope reduction.

The governing implications are:

- Bloodlines ships as the full canonical realization of the design bible
- Unity 6.3 LTS with DOTS / ECS is the shipping engine
- `unity/` is the only active Unity work target
- the browser runtime is frozen as a behavioral specification and must not receive new systems
- Wwise audio polish and Netcode for Entities multiplayer remain in scope unless Lance removes them later
- if older documents recommend reducing scope or building a smaller release first, treat that guidance as stale

## Current Direction

- Browser runtime is preserved as frozen behavioral specification.
- Dynasty consequence cascade is live in the browser specification.
- Fortification and siege doctrine are canonically locked and remain targets for Unity realization.
- Unity continuation is the active shipping lane.
- Full commercial polish remains in scope for art, audio, UX, onboarding, tutorials, campaign, lobby, HUD, and in-game panels.
- The project is both a preserved archive and an active implementation workspace, but new implementation direction now points at full-canon Unity delivery only.
