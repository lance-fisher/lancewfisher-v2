# Bloodlines Visual Production Bible

Established: 2026-04-15
Status: active graphics-lane foundation
Scope: production-facing visual doctrine for Bloodlines asset generation, review, and Unity integration

## 1. Purpose

This document translates Bloodlines visual canon into a production-safe operating standard.

It is not a replacement for the wider design corpus. It is the graphics lane reference that future Claude Code, Codex, and human contributors can use when creating or reviewing:

- units
- buildings
- terrain
- biome packs
- set pieces
- iconography
- House identity sheets
- placeholder art
- first-pass production assets

## 2. Canon Anchors

This production bible is constrained by:

- `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
- `01_CANON/CANONICAL_RULES.md`
- `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md`
- `13_AUDIO_VISUAL/MASTER_DOCTRINE_AUDIO_VISUAL_INTEGRATION_2026-04-14.md`
- `06_FACTIONS/FOUNDING_HOUSES.md`
- `06_FACTIONS/HOUSE_IDENTITY_DOCTRINE_ADDENDUM_2026-04-14.md`
- `09_WORLD/TERRAIN_SYSTEM.md`
- `10_UNITS/UNIT_INDEX.md`
- `12_UI_UX/MASTER_DOCTRINE_UI_INTEGRATION_2026-04-14.md`
- `docs/unity/README.md`

## 3. Visual Philosophy

Bloodlines must look like a grand dynastic medieval RTS built for long-form play, not a stylized fantasy spectacle.

The target is:

- readable at RTS gameplay height
- serious in tone
- materially grounded
- disciplined in silhouette
- atmospheric without clutter
- detailed where identity matters
- restrained where battlefield readability matters more

This is a modernized readability tier adjacent to `Command & Conquer: Generals` and `Zero Hour`, not a photoreal AAA simulation and not a cartoon fantasy game.

## 4. Camera Assumptions

All asset decisions must assume three reads:

1. Strategic gameplay read: the player is evaluating formations, buildings, and terrain in active combat.
2. Mid-distance inspection read: the player is close enough to register House identity, faith overlays, and upgrade state.
3. Close presentation read: the player is in menus, portraits, keep view, or concept review and can appreciate finer trim and material work.

Gameplay read is the priority. If close detail weakens gameplay read, the detail loses.

## 5. Silhouette Discipline

Every asset family needs one dominant shape before any surface detail is considered.

Rules:

- Units must read by body mass, weapon reach, mount state, shield profile, banner usage, and motion pattern.
- Buildings must read by roofline, tower massing, footprint, wall thickness, gate logic, and attached production elements.
- Terrain features must read by contour, edge profile, and passability implication.
- Command units, elite units, and faith units may add detail, but they cannot become noisy.
- No unit should require texture reading to identify class.
- No building should require icon hover to identify function in its mature state.

## 6. Read Bands

### Far gameplay band

Must communicate:

- side ownership
- unit class
- formation width
- structure function
- defensive threat
- route and chokepoint logic

### Mid band

Must communicate:

- House identity
- faith overlay
- damage state
- upgrade state
- command presence
- construction phase

### Close band

May communicate:

- textile weave
- heraldic variation
- tool wear
- social rank
- age and repair history

## 7. Asset Density Targets

Density means how much visual information is allowed before readability breaks.

### Units

- One dominant silhouette read.
- One secondary read for House or role identity.
- One tertiary read for faith, command, or elite status.
- No more than two freely hanging decorative elements on standard line units.
- Armor break-up should come from structure, not ornament spam.

### Buildings

- One primary function silhouette.
- One support silhouette cluster such as chimney, siege frame, granary venting, stable awning, or tower crown.
- Surface breakup through material joins, braces, shutters, ladders, and banners, not through random debris.

### Terrain and environment

- Terrain readability first.
- Ground texture variation should reinforce movement, water, and ownership logic.
- Set pieces must help orientation, not drown paths and edges.

## 8. Terrain Style

Terrain should feel geologically and agriculturally credible. Ground is not a generic color field.

Core ground logic:

- reclaimed land shows human use, drainage, paths, and old boundaries
- frontier land shows harsher transitions and weaker maintenance
- defended land shows intentional clearing, watch lines, and road servicing
- neglected land shows erosion, rutting, rubble spread, and unmanaged edge growth

No biome may rely on saturation gimmicks to feel distinct.

## 9. Architecture Style

Architecture must express:

- the House
- the economy that built it
- the threat model it expects
- the faith overlay it serves
- the stage of settlement development

Buildings are built by people with load-bearing logic, labor limits, and material constraints. Decorative fantasy forms without structural explanation are prohibited.

## 10. Siege Style

Siege visuals must feel brutal, practical, and engineered.

Requirements:

- engines look assembled from real beams, braces, axle weight, and hauling logic
- siege camps look temporary but functional
- supply lines look vulnerable and important
- breach points, damage, and repair efforts must read immediately
- defenders and attackers must be visually distinguishable at walls and gates

## 11. Battlefield Clutter Limits

Clutter is controlled aggressively.

Do not allow:

- random prop spam around every building
- overgrown edge dressing that obscures passability
- oversized banners on standard troops
- cinematic debris fields that hide unit selection or pathing
- thick fog, particles, or grading that suppresses unit read

Allowed clutter is deliberate clutter:

- camp equipment near camps
- repair material near construction
- harvest matter near farms
- rubble at damaged structures
- corpses and broken gear only where battle consequence is being communicated

## 12. Texture and Material Strategy

Material families should stay limited and legible.

Core families:

- dressed stone
- rough stone
- timber
- weathered timber
- iron
- worked steel
- leather
- wool or coarse cloth
- fine dyed cloth
- rope and canvas
- earth and mud
- plaster
- thatch
- tile

Rules:

- use value separation more than noisy normal detail
- material change should explain construction or status
- House identity comes through palette and usage pattern, not through fantasy shader tricks
- weathering must match site condition and maintenance level

## 13. Color Discipline By Dynasty

House color is a directional system, not a full-surface flood fill.

Apply House color primarily to:

- banners
- shield paint
- scabbard or sash accents
- command cloth
- architectural trims
- ownership markers
- icon and portrait treatment

Do not turn armor, walls, or terrain into flat faction-colored surfaces.

## 14. Mood and Lighting Direction

Bloodlines lighting should support weight and clarity.

Defaults:

- daylight is cool-neutral to warm-neutral, never washed out
- overcast is acceptable and often useful for battlefield legibility
- dusk and firelight are reserved for dramatic or faith-lane moments, not constant saturation
- interior or keep-view lighting may be richer than battlefield lighting

The battlefield must remain readable under all chosen moods.

## 15. Map Readability Principles

Maps must let the player answer these questions quickly:

- where can I move
- where can cavalry charge
- where is water
- what is defensible
- what is exposed
- where are resources
- where is the nearest settlement or route junction

Roads, rivers, elevation breaks, settlement silhouettes, and tree lines must carry this load.

## 16. Destruction-State Philosophy

Destruction is a readability system, not just a damage effect.

Each relevant asset family should define:

- intact state
- damaged state
- critical state
- ruined or burned-out state when canonically relevant

Damage must alter silhouette, not just texture. A damaged tower, gate, or siege engine should read damaged without requiring inspection.

## 17. Construction-Phase Philosophy

Construction must look like real progress, not a magical pop-in.

Expected stages:

- footprint marking
- frame and scaffold
- partial enclosure
- functional but unfinished
- completed

Construction visuals must help the player gauge risk and completion without opening UI.

## 18. Icon Clarity Standards

Icons must be crisp, symbolic, and hierarchy-first.

Rules:

- one dominant symbol per icon
- no muddy gradients
- no texture-heavy mini paintings
- House and faith marks must stay distinct at small sizes
- status overlays must not erase base symbol recognition

## 19. Environmental Storytelling Standards

Environmental storytelling is allowed only when it remains subordinate to gameplay.

Use it to show:

- wealth versus neglect
- war damage
- supply activity
- religious presence
- civic order
- route importance
- reclaimed versus abandoned space

Do not let storytelling props obscure unit interaction zones or terrain edges.

## 20. House and Faction Distinction Rules

Houses are dynastic civilizations, not color swaps.

Distinction comes from:

- material use
- silhouette preference
- banner language
- command-unit presentation
- settlement organization
- roof and tower profile
- equipment cut
- controlled accent colors
- homeland influence

Faith overlays modify House identity. They do not erase it.

## 21. Consistency Rules For AI-Generated Or Human-Made Assets

Every asset batch must state:

- source brief used
- House or universal lane
- gameplay read target
- stage label
- review tag

Consistency rules:

- use the same camera assumptions in concept prompts
- use the House identity packs, not memory or improvisation
- preserve realistic scale and material logic
- keep prompt negatives active against cartoon, anime, glossy mobile, and over-rendered fantasy drift
- compare new assets against existing approved sheets before promotion

## 22. Allowed Visual Moves

Allowed:

- grounded medieval realism
- disciplined exaggeration for gameplay silhouette
- stronger detail on command, elite, and House-defining units
- weathering tied to environment and use
- faith overlays that remain materially plausible
- deliberate banner systems and ownership markings

## 23. Prohibited Visual Moves

Prohibited unless explicitly routed into a separate experiment lane:

- cartoon stylization
- anime proportioning
- oversized fantasy armor spikes
- floating ornament
- magical glowing trim as baseline faction identity
- glossy mobile-game rendering
- photoreal mud overload that kills unit read
- monochrome faction floods across whole assets
- decorative clutter that obscures pathing and combat state

## 24. What "More Detail" Means In Bloodlines

More detail does not mean more noise.

In Bloodlines, more detail means:

- better material separation
- stronger construction logic
- cleaner heraldic and trim placement
- more readable damage and repair state
- richer command and elite distinction
- more credible civic and military wear patterns

If detail stops helping the player identify, judge, or emotionally place the asset, it is excess.

## 25. Approval Baseline

An asset family is ready for concept review only if it:

- obeys the correct House or universal brief
- reads at gameplay height
- fits the stage ladder
- has a manifest entry
- includes Unity import notes

Without those, it remains staging material only.
