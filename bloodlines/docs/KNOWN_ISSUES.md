# Known Issues

- The current vertical slice implements only the tactical RTS layer. The declared-time strategic layer is not yet present.
- Only Ironmark has active strategic gameplay identity. The other eight houses are preserved in data but remain mostly visual/canonical placeholders in accordance with the 2026-04-07 canon reset.
- Unit movement uses lightweight steering, not full pathfinding. Large clumps can snag on buildings or each other.
- Combat has no formations, stances, morale routing, siege logic, or commander bonuses yet.
- Farms and Wells support food and water economy, but full shortage consequences are not yet modeled.
- Territory control and neutral tribe pressure now exist in prototype form, but province governance, occupation states, loyalty cascades, and the Trueborn city are not yet implemented.
- Faith, conviction, bloodline roles, and victory conditions exist in data definitions but are mostly stubs for future systems.
- The enemy AI now contests territory in early prototype form, but it still lacks deeper strategic priorities, commander logic, faith behavior, diplomacy, and multi-path victory reasoning.
- Placeholder art uses geometric silhouettes and debug-forward UI.
