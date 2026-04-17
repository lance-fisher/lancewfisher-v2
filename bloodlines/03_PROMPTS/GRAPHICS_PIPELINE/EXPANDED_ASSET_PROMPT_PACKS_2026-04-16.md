# Expanded Asset Prompt Packs

Established: 2026-04-16
Scope: production-oriented prompt packs for future Bloodlines concept and placeholder generation

## 1. Purpose

These prompt packs deepen the original concept framework so future sessions can generate assets with less drift and more direct production value.

Use them together with:

- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/BLOODLINES_VISUAL_PRODUCTION_BIBLE_2026-04-15.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/HOUSE_VISUAL_IDENTITY_PACKS_2026-04-15.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/UNIT_VISUAL_DIRECTION_PACKS_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/BUILDING_FAMILY_DIRECTION_PACKS_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/TERRAIN_AND_BIOME_DIRECTION_PACKS_2026-04-16.md`

## 2. Shared Prompt Packet

Every visual-generation prompt should explicitly include:

- asset id
- asset family
- House or universal usage
- gameplay function
- gameplay-height readability goal
- silhouette anchors
- material family
- color discipline
- prohibited drift notes
- stage label
- version number
- output expectation

Base clause:

`Bloodlines, serious medieval dynasty RTS, grounded battlefield presentation, Command and Conquer Generals / Zero Hour readability discipline, no cartoon fantasy, no glossy mobile rendering, no anime, no oversized fantasy exaggeration, top-down gameplay-height legibility preserved.`

## 3. Unit Sheet Prompt Pack

### Use for

- militia
- core infantry
- elite infantry
- ranged
- cavalry
- workers
- command units

### Prompt skeleton

`Create a Bloodlines unit concept sheet for [asset family] used by [House or universal lane]. Show front, three-quarter, and gameplay-height read. Emphasize [silhouette anchors]. Materials are [materials]. Color direction is [palette discipline]. The unit must read clearly as [role] at RTS camera height. Keep detail concentrated on shield plane, weapon profile, helm shape, banner trim, and command cloth only where it improves recognition. Background neutral. Include restrained labeling bands for variant A/B/C. Preserve grounded medieval construction logic. Avoid cinematic clutter, heroic posing, exaggerated fantasy armor, or decorative noise.`

### House rider clause

Append one House-specific clause from the identity pack, for example:

- Ironmark: `square shield mass, dark forge iron, ember cloth restraint, brutal but disciplined weight`
- Hartvale: `warm agrarian defense identity, layered cloth, civic protector read, natural but not druidic fantasy excess`

### Output expectation

- concept sheet
- neutral background
- variant columns
- stage tag in filename

## 4. Building Family Prompt Pack

### Use for

- keep or command hall
- barracks
- blacksmith
- market
- tower
- wall
- gate
- granary
- well
- storehouse
- faith structures

### Prompt skeleton

`Create a Bloodlines building family sheet for [building family] in the [universal or House] lane. Show gameplay-height silhouette, construction logic, entrance side, roofline, wall mass, and destruction-state readiness. The building must read instantly as [gameplay function]. Materials are [stone, timber, plaster, iron, cloth mix]. Keep the form practical for a large-scale medieval RTS. Emphasize recognizable roof shapes, tower massing, gate profile, workshop fixtures, or storehouse volume depending on role. Avoid ornate fantasy cathedrals, tiny clutter props, glossy finish, or over-rendered cinematic rubble.`

### Required variant rows

- base state
- upgrade candidate if relevant
- damage or breach candidate if relevant
- House overlay candidate if relevant

## 5. Terrain And Biome Prompt Pack

### Use for

- terrain surfaces
- biome sheets
- road and path language
- riverbank and shoreline transitions
- cliff transitions
- resource-ground treatment

### Prompt skeleton

`Create a Bloodlines terrain lookdev sheet for [terrain family]. Show top-down and slight-angle readability for Unity RTS play height. Include clean tile or strip comparisons for [surface types]. The terrain must support unit readability and structure placement. Materials should feel grounded, weathered, and strategic rather than painterly or decorative. Keep contrast controlled so roads, riverbanks, cliffs, and resource seams remain legible without overwhelming units or buildings.`

### Mandatory clarity notes

- readable from zoomed-out play
- edge blends visible but not muddy
- clutter limited
- path and ford language unmistakable

## 6. Environment And Logistics Prompt Pack

### Use for

- bridges
- carts
- wagons
- fencing
- resource piles
- siege debris
- shrines
- statues
- water infrastructure

### Prompt skeleton

`Create a Bloodlines environment setpiece sheet for [family]. Focus on medium-scale RTS readability, grounded construction, and tactical context. Show how the object reads near roads, keeps, rivers, fields, or siege lines. Keep the silhouette practical and broad enough to register at gameplay height. Avoid prop spam, tiny decorative clutter, or whimsical fantasy detail.`

## 7. Icon And Emblem Prompt Pack

### Use for

- unit icons
- building icons
- dynasty emblems
- resource icons
- faith symbols
- command markers

### Prompt skeleton

`Create a Bloodlines icon sheet for [icon family]. Prioritize legibility at small command-bar size. Use bold shape hierarchy, controlled internal detail, and grounded heraldic or material cues. Preserve faction distinction without cartoon outlines or glossy mobile-game highlights. Provide small-size and medium-size read checks on neutral and dark backgrounds.`

### Required checks

- 32px read
- 64px read
- dynasty confusion test
- resource confusion test where relevant

## 8. Material Board Prompt Pack

### Use for

- shared materials
- House trims
- ground families
- weathering studies
- banner cloth rules

### Prompt skeleton

`Create a Bloodlines material board for [family]. Show restrained swatches and applied fragment examples for wood, stone, iron, cloth, leather, plaster, mud, soot, or trim as relevant. The goal is not photoreal spectacle; the goal is RTS readability with grounded medieval material identity. Keep values controlled so unit and structure silhouettes survive at distance.`

## 9. Destruction And Upgrade Prompt Pack

### Use for

- wall damage
- breach states
- building upgrades
- worn versus improved civic states

### Prompt skeleton

`Create a Bloodlines progression sheet for [asset family] showing base state, upgrade state, and destruction or breach state where relevant. The family must remain recognizable in every state. Preserve scale, entry logic, faction read, and functional identity. Damage should read as military consequence, not cinematic debris spectacle.`

## 10. Negative Prompt Discipline

Use negative clauses when needed:

- no cartoon fantasy
- no oversized pauldrons
- no exaggerated monster proportions
- no glossy mobile-game rendering
- no anime facial treatment
- no ornamental clutter fields
- no hyper-saturated color spread
- no magical visual noise unless a settled faith asset explicitly requires a restrained sacred accent

## 11. Unity Export Notes For Prompt Runs

If a generation pass is intended to support Unity staging, require:

- neutral background or transparent-safe isolation when possible
- front, three-quarter, and gameplay-height read if applicable
- sheet labeling that matches manifest ids
- version-safe filenames
- no final-stage naming on first-pass output

## 12. Output Naming Reminder

Use the established pattern:

`bl_[assetclass]_[lane]_[family]_[variant]_[stage]_v###`

Do not collapse stage tags until the asset family is truly final.
