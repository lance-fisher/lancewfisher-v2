# Runbook

## Prerequisites

- Python 3
- Node.js 24+

## Serve The Project

From the repository root:

```bash
python -m http.server 8077 --directory deploy
```

## Open

- Archive: `http://localhost:8077/bloodlines/`
- Prototype: `http://localhost:8077/bloodlines/play.html`

## Prototype Controls

- `W`, `A`, `S`, `D` or arrow keys: move camera
- Left click: select
- Left drag: box-select player units
- Right click ground: move
- Right click resource: gather with selected workers
- Right click enemy: attack
- Build buttons appear when Workers are selected
- Train buttons appear when a Command Hall or Barracks is selected

## Validation

From the repository root:

```bash
node deploy/bloodlines/tests/data-validation.mjs
```

## Smoke Test

1. Start a skirmish from `play.html`.
2. Select Workers and gather gold and wood.
3. Build a House, Farm, Well, and Barracks.
4. Train at least one Worker and one military unit.
5. Confirm resources, population, and selected-unit debug values update.
6. Survive an enemy wave and destroy the enemy Command Hall, or confirm loss state if yours is destroyed.

## Troubleshooting

- If the game is blank, confirm the page is being served over HTTP, not opened directly from disk.
- If data fails to load, verify the server root is `deploy/`.
- If validation fails, fix the referenced JSON definition before changing gameplay logic.
