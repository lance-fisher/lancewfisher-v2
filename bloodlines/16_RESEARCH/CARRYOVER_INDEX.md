# Carryover Index

Documents what was carried from other game projects into Bloodlines.
Generated: 2026-03-22

## Source: frontier-bastion/ (Crown & Conquest RTS)

### Architecture Patterns (CROWN_CONQUEST_ARCHITECTURE_REFERENCE.md)
- ECS entity/component model
- Fixed-point math (FP_SCALE=100) for deterministic simulation
- 20 TPS fixed-timestep game loop
- A* pathfinding (adaptable to hex/square grids)
- Seeded RNG for replay and multiplayer
- Data-driven definitions (units, buildings, terrain as config)
- Save/load serialization pattern
- Isometric camera and projection math

### Design Elements (CROWN_CONQUEST_DESIGN_ELEMENTS.md)
- Elevation combat mechanics (higher ground advantages)
- Map archetypes (Rush, Turtle, Expansion)
- Neutral structures as contested objectives
- Resource gathering and economy model
- Base building with adjacency and tech tree
- Unit training, control groups, formations
- Wave survival mode concept

### Not Carried (Available in frontier-bastion/ if needed later)
- Full TypeScript source code (~3000+ lines in frontier-bastion/frontier-bastion/src/)
- Canvas 2D rendering pipeline
- Sprite sheet handling
- Minimap implementation
- Input handling system

## Source: "bloodlines game/" (Legacy Design Texts)
- Already archived in 19_ARCHIVE/ as LEGACY_ prefixed files (2026-03-15)
- Contains: AI prompt, bible, master prompt
- See 19_ARCHIVE/LEGACY_ARCHIVE_INDEX.md for details

## Source: galleon-splash/
- Not a game project (splash page). No content carried.

## Archival Status
- "bloodlines game/" folder: Archived to _archive/2026-03-22/
- frontier-bastion/: Pending archive (design docs extracted, code available if needed)
- galleon-splash/: Not a game, remains as-is
