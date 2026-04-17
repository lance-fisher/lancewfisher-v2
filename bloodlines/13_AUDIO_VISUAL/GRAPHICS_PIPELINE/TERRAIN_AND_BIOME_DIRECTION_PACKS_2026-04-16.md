# Terrain And Biome Direction Packs

Status: additive graphics-lane production reference

## Purpose

This document expands the terrain visual track into production-direction notes suitable for map building, terrain material grouping, and environment coherence inside Unity 6.3.

## Shared Terrain Rules

- terrain must stay readable at gameplay height first
- transitions must explain pathing and biome shift, not just look pretty
- roads, rivers, cliffs, and resource-ground cues carry strategic information
- clutter is support, never the primary read

## 1. Plains And Managed Ground

- Primary read: traversable open land with route and field logic
- Material family: grass, dry soil, old boundary traces, cultivation wear
- Readability risk: over-busy grass noise
- Required variants: reclaimed plains, trampled ground, dry wear, field edge

## 2. Roads, Paths, And March Wear

- Primary read: movement guidance and political infrastructure
- Material family: packed dirt, wheel wear, stone paving where relevant, drainage edges
- Readability risk: bright stripes or muddy disappearance
- Required variants: main road, secondary road, worn path, siege wear, convoy wear

## 3. Cliffs, Ridges, And Highlands

- Primary read: elevation, defensibility, path denial
- Material family: exposed rock, fractured stone, sparse grass, gravel
- Readability risk: uncertain passability
- Required variants: walkable slope, cliff face, ridge crest, path foot, highland edge

## 4. Riverbanks, Fords, And Shorelines

- Primary read: water barrier and crossing logic
- Material family: bank soil, shallow water, wet edge, reeds, stones
- Readability risk: reflective visual noise hiding path decisions
- Required variants: dry shore, wet bank, ford lane, steep bank, reed edge, landing zone

## 5. Resource Ground Treatment

- Lumber sites: stumps, cut lanes, cart wear, tool traces
- Quarry sites: spoil dust, broken stone, haul paths
- Iron sites: slag, soot-dark wear, extraction ruts, controlled ember cues
- Wells and water support: damp foot traffic, civic wear, bucket zones

Rule:

The ground treatment should help identify the node before zooming in, but should never replace the node silhouette itself.

## 6. Biome Edge Blends

- farm to path
- path to mud
- grass to stone
- plains to forest
- shoreline to dry land
- sacred ground to surrounding normal terrain

Rule:

Edge blends should feel like a gradient of use, erosion, or ecology, not a hard paint seam.

## 7. Bridge And Water Infrastructure Lane

- timber frontier bridge
- stone civic bridge
- fortified gate bridge
- ruin-span salvage bridge
- aqueduct and water works
- dock and landing support

Rule:

These assets must communicate crossing logic, approach width, and choke value before detail.

## 8. Logistics And Setpiece Lane

- supply wagons
- field camps
- fences and palisade markers
- ruin debris
- shrines
- watchfires and signal posts

Rule:

These setpieces should strengthen world coherence without hiding units, roads, or structure purpose.

## 9. Map-Building Coherence Notes

- Hartvale-like productive belts want fields, wells, roads, and storehouse logic grouped visibly
- Stonehelm-like defensive regions want ridge control, wall fit, and clear artillery or tower lanes
- Ironmark-like production zones want harsher extraction ground, forge routes, and industrial wear
- Trueborn or Whitehall civic cores want cleaner roads, more controlled shorelines, and clearer institution-facing structure placement

## 10. Unity 6.3 Use

Future terrain implementation should group these into:

- `Art/Terrain/Biomes/`
- `Art/Terrain/Transitions/`
- `Art/Terrain/Roads/`
- `Art/Terrain/ResourceGround/`
- `Art/Environment/Setpieces/Bridges/`
- `Art/Environment/Setpieces/Waterworks/`
- `Art/Environment/Logistics/`

These packs are direction documents, not final material definitions.
