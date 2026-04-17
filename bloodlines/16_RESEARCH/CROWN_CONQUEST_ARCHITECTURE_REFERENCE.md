# Crown & Conquest Architecture Reference

Extracted from `frontier-bastion/` (Crown & Conquest RTS) for Bloodlines prototype reference.
These patterns were proven in a working 2.5D isometric RTS and may inform Bloodlines' game systems.

## ECS Entity/Component Model
- Lightweight ECS-ish pattern (not full archetype storage)
- Entities are numeric IDs with attached component objects
- Components: Position (fixed-point), Health, UnitState, BuildingState, ResourceCarry
- Systems operate on component queries each tick
- Separation: sim/ (deterministic logic), render/ (display), ui/ (input)

## Fixed-Point Math for Determinism
- FP_SCALE = 100 (all positions stored as integers * 100)
- Eliminates floating-point nondeterminism across machines
- Essential for future lockstep multiplayer
- Pattern: `const fpX = Math.round(realX * FP_SCALE)`
- Division: `Math.floor((a * FP_SCALE) / b)` to avoid float intermediates

## 20 TPS Deterministic Simulation Loop
- Fixed timestep: 50ms per tick (20 ticks per second)
- Simulation runs independently of render framerate
- Game loop: accumulate dt, step sim at fixed rate, render at requestAnimationFrame
- All game logic in sim tick, never in render or UI
- Enables replay, save/load, and future netcode

## A* Pathfinding
- Standard A* with cardinal + diagonal movement
- Tile-based grid (80x80 maps)
- Elevation affects movement cost
- Path caching for performance
- Adaptable to hex grids or larger maps for Bloodlines

## Seeded RNG for Determinism
- Custom seeded PRNG (not Math.random())
- Same seed = same game every time
- Critical for replay system and multiplayer sync
- Seed stored in save files

## Data-Driven Definitions
- Units, buildings, upgrades, terrain, waves defined as config objects
- Not hardcoded in logic — data drives behavior
- Easy to add new unit types without changing systems code
- Pattern: `buildings.ts`, `units.ts`, `upgrades.ts`, `terrain.ts`

## Save/Load Serialization
- Full game state serializable to JSON
- Entity list + component data + resource state + wave progress
- Load reconstructs complete game state
- Pattern useful for Bloodlines save system

## Isometric Projection
- Tile dimensions: W=64, H=32
- Elevation offset: 16px per level
- Screen-to-tile and tile-to-screen conversion functions
- Camera system with pan, zoom, and bounds clamping
