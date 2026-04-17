# Fortification + Siege + Population Cycle + Realm Legibility Wave

> **For future sessions:** This plan advances Bloodlines toward its canonical full-scale form, not toward a reduced substitute. Every task is additive. Nothing in canon is compressed. Where the runtime must diverge from canon for runtime-scale reasons (e.g. 18s visible growth vs 90s canonical deep cycle), both are preserved — the runtime overlays a faster visible heartbeat onto a slower canonical consequence cycle.

**Authored:** 2026-04-14
**Target session:** Continuation from 2026-04-14 governance consolidation
**Source ground truth:**
- `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md`
- `04_SYSTEMS/SIEGE_SYSTEM.md`
- `04_SYSTEMS/POPULATION_SYSTEM.md`
- `04_SYSTEMS/RESOURCE_SYSTEM.md`
- `NEXT_SESSION_HANDOFF.md`
- `tasks/todo.md` (Next Session Priorities 2026-04-15 onward)

---

## Goal

Close the canon-runtime gap across four load-bearing pillars in one coordinated wave:

1. **Stone + iron economy** — activate the canonical resource chain that unlocks fortification and mid-to-late game military.
2. **Fortification tier metadata + wall/tower/gatehouse/keep buildings** — turn control points and settlements into canonical settlement classes with real defensive architecture.
3. **First siege engine (ram) + assault failure penalty scaffold** — make wall damage distinct from unit damage, and punish wave-spam against fortifications.
4. **Canonical 90-second population cycle + famine + water crisis scaffolding** — move from the 18-second shortcut to the canon's real heartbeat with consequence modeling.
5. **Realm condition legibility** — surface the 11 canonical pressure states in the HUD so the player inhabits the realm rather than piloting an economy.

This wave does not finish the fortification/siege system — it builds the **foundation** that every subsequent lane (reserve cycling, governor specialization, faith-integrated fortification, rescue/ransom operations, commander keep presence) depends on. The next wave after this one will layer on top of this foundation without refactoring it.

---

## Architecture

**Philosophy.** Extend the existing sound runtime. Don't rewrite it. The simulation.js tick function already dispatches 12 per-stage updates (simulation.js:1604-1623); add new stages in place. The data layer already loads JSON; add new entries to existing files. The renderer already draws tile-based geometry; add new entity draw types. The UI already has panel sections; add new pressure-state rows.

**Non-breaking additive principle.** Every data change adds new entries with `prototypeEnabled` flags or equivalent so existing tests keep passing. Every simulation change adds new stage functions rather than rewriting existing ones. Every renderer change adds new draw calls layered over the existing stack.

**Canon fidelity priorities.** Where canon and runtime disagree (population cycle rate, settlement class model, wall damage model), canon wins on the deep structural mechanic, runtime wins on feedback cadence. Both are preserved explicitly.

**Hardcode-to-data migration.** Where new systems introduce tuning values, those values live in data (`data/*.json`) from the start. Existing hardcoded values in simulation.js and ai.js stay put for this wave — migration is a later task.

**Tech Stack:** ES modules, vanilla JS, JSON data, Canvas 2D. No new dependencies.

---

## Task List

### Task 1: Enable Stone and Iron Resources (Economy Foundation)

**Files:**
- Modify: `data/resources.json` — flip `enabledInPrototype` for stone and iron to `true`
- Modify: `data/maps/ironmark-frontier.json` — add stone and iron resource nodes to the map
- Modify: `data/buildings.json` — add `quarry` (stone dropoff) and `iron_mine` (iron dropoff, requires proximity to iron node)
- Modify: `data/buildings.json` — extend `mine_works` to be the gold dropoff only (rename internally still `mine_works` for compatibility); new `quarry` and `iron_mine` are distinct
- Modify: `src/game/core/simulation.js` — resource accounting already handles arbitrary resource IDs; verify no hardcoded resource allowlists block stone/iron
- Modify: `src/game/core/renderer.js` — add draw cases for stone nodes (gray circles) and iron nodes (dark rust circles)
- Modify: `src/game/main.js` — add quarry and iron_mine to the command panel build buttons; add stone and iron to the resource bar pills
- Modify: `tests/data-validation.mjs` — update to assert stone and iron are now `enabledInPrototype: true` (replace the earlier assertion that they were NOT enabled)

**Canon compliance:**
- Canon: "Stone and iron must be primary resources; stone is fortification material; iron needs wood for smelting" (RESOURCE_SYSTEM.md + POPULATION_SYSTEM.md canon)
- Implementation: stone is a straight gather-deliver loop; iron is **gated by smelting**, which means the iron_mine building only produces iron if there is wood in the faction stockpile above a threshold. Iron gathering from a map iron node still requires a worker and dropoff; but the act of producing iron (the node depleting into faction stockpile) consumes wood at a ratio (e.g. 1 iron ← 0.5 wood).
- For this wave, the smelting chain is the simplest possible: at iron dropoff, if wood < iron-delivered × 0.5, iron is not credited (returned to node); worker idles briefly with a "no smelting fuel" message. This is the **minimum viable canon-faithful smelting chain** without yet adding a smelter building. The smelter as a distinct structure is deferred to the next wave.

**Step 1.1: Update `data/resources.json`.**

```json
[
  { "id": "gold", "name": "Gold", "category": "primary", "enabledInPrototype": true, "description": "Currency, taxation, recruitment, trade, and diplomacy." },
  { "id": "food", "name": "Food", "category": "primary", "enabledInPrototype": true, "description": "Population growth and sustainment support." },
  { "id": "water", "name": "Water", "category": "primary", "enabledInPrototype": true, "description": "Population health and growth support." },
  { "id": "wood", "name": "Wood", "category": "primary", "enabledInPrototype": true, "description": "Early construction, fuel, and military infrastructure material." },
  { "id": "stone", "name": "Stone", "category": "primary", "enabledInPrototype": true, "description": "Fortification and major structure material." },
  { "id": "iron", "name": "Iron", "category": "primary", "enabledInPrototype": true, "description": "Mid-to-late game military modernization material. Production consumes wood as smelting fuel." },
  { "id": "influence", "name": "Influence", "category": "advanced", "enabledInPrototype": true, "description": "Political leverage, strategic pressure, and systemic soft power." }
]
```

**Step 1.2: Add map nodes to `data/maps/ironmark-frontier.json` `resourceNodes` array:**

- Player-side stone node at (x=12.5, y=10.5) with amount 2400
- Player-side iron node at (x=8.5, y=14.5) with amount 1800
- Enemy-side stone node at (x=58.5, y=36.5) with amount 2400
- Enemy-side iron node at (x=62.5, y=32.5) with amount 1800
- Two neutral/contested stone nodes near the center at (x=34.5, y=18.5) amount 1600 and (x=38.5, y=26.5) amount 1600

**Step 1.3: Add buildings to `data/buildings.json`:**

```json
{
  "id": "quarry",
  "name": "Quarry",
  "prototypeEnabled": true,
  "footprint": { "w": 2, "h": 2 },
  "health": 360,
  "buildTime": 12,
  "dropoffResources": ["stone"],
  "cost": { "gold": 40, "wood": 40 }
},
{
  "id": "iron_mine",
  "name": "Iron Mine",
  "prototypeEnabled": true,
  "footprint": { "w": 2, "h": 2 },
  "health": 360,
  "buildTime": 14,
  "dropoffResources": ["iron"],
  "smeltingFuelRatio": 0.5,
  "smeltingFuelResource": "wood",
  "cost": { "gold": 55, "wood": 50 }
}
```

**Step 1.4: Verify simulation resource accounting.**

Read `simulation.js` gather/dropoff code path. If it uses a generic `faction.resources[resourceId] += amount` pattern, stone and iron work automatically. If there's a hardcoded allowlist, extend it to include all `enabledInPrototype: true` resource IDs.

**Step 1.5: Implement smelting fuel consumption at iron dropoff.**

In the `updateUnits` worker-dropoff branch, when a worker delivers to an `iron_mine` (identified by `building.smeltingFuelRatio > 0`):
```javascript
const fuelNeeded = deliveredAmount * building.smeltingFuelRatio;
const fuelResource = building.smeltingFuelResource; // "wood"
if ((faction.resources[fuelResource] ?? 0) >= fuelNeeded) {
  faction.resources[fuelResource] -= fuelNeeded;
  faction.resources.iron = (faction.resources.iron ?? 0) + deliveredAmount;
} else {
  // Return to node; briefly stall worker
  node.amount = Math.min(node.maxAmount ?? Infinity, node.amount + deliveredAmount);
  postMessage(state, faction.id, "Iron Mine cannot smelt: insufficient wood fuel.", 6);
}
```

**Step 1.6: Add renderer cases for stone and iron nodes.**

In `renderer.js` resource draw branch, add:
- `type === "stone"`: gray (#7a7a7a) filled circle with darker outline, 15px radius
- `type === "iron"`: dark rust (#6b3f1f) filled circle with orange accent (#b86a2a) outline, 15px radius

**Step 1.7: Add resource bar pills for stone and iron.**

In `main.js` `renderPanels()` resource bar branch, add stone (icon: small gray square) and iron (icon: small rust square) to the 8-pill resource bar (expanding to 10 pills).

**Step 1.8: Add quarry and iron_mine to build commands.**

In `main.js` command panel worker-selected branch, add build buttons for quarry (cost 40 gold, 40 wood) and iron_mine (cost 55 gold, 50 wood). Order: dwelling, farm, well, lumber_camp, mine_works, quarry, iron_mine, barracks.

**Step 1.9: Update tests.**

Modify `tests/data-validation.mjs`:
- Remove any assertion that stone/iron are `enabledInPrototype: false`.
- Add assertion: `resources.filter(r => r.category === "primary" && r.enabledInPrototype).length === 6` (gold, food, water, wood, stone, iron).
- Add assertion: `buildings.find(b => b.id === "quarry").dropoffResources.includes("stone")`.
- Add assertion: `buildings.find(b => b.id === "iron_mine").smeltingFuelResource === "wood"`.

**Step 1.10: Verify.**

```bash
node tests/data-validation.mjs  # expect: "Bloodlines data validation passed."
node --check src/game/core/simulation.js
node --check src/game/main.js
node --check src/game/core/renderer.js
```

Open `play.html` and verify: stone/iron nodes render on map, quarry/iron_mine buildable, worker delivers stone, iron_mine stalls if no wood.

**Step 1.11: Commit.**

```bash
git add data/resources.json data/maps/ironmark-frontier.json data/buildings.json src/game/core/simulation.js src/game/core/renderer.js src/game/main.js tests/data-validation.mjs
git commit -m "Enable stone and iron economy with smelting fuel chain"
```

---

### Task 2: Fortification Tier Metadata on Settlements and Control Points

**Files:**
- Modify: `data/maps/ironmark-frontier.json` — add `settlementClass` and `fortificationTier` to control points
- Modify: `src/game/core/simulation.js` — add settlement-class data model, derived fortification ceiling, upgrade mechanics
- Modify: `src/game/core/renderer.js` — visual tier indicator (subtle outline style) on control points
- Modify: `src/game/main.js` — display settlement class + fortification tier in Selection panel and debug overlay
- Modify: `tests/data-validation.mjs` — assert control points have settlementClass

**Canon compliance:**
- Canon: 6 settlement classes with escalating defensive ceilings: border settlement → military fort → trade town → regional stronghold → primary dynastic keep → fortress-citadel (developed) (DEFENSIVE_FORTIFICATION_DOCTRINE.md)
- Implementation: each control point and settlement gets a `settlementClass` property (one of the 6 canonical strings). Each class has a `defensiveCeiling` (max fortification tier achievable at this class). `fortificationTier` is a number 0-5 that increments as walls/towers/keeps are constructed at the site. Upgrading class requires a major commitment (declared in data, not implemented as a buildable action in this wave — noted as future work).

**Step 2.1: Add settlement class definitions to a new file `data/settlement-classes.json`:**

```json
[
  { "id": "border_settlement", "name": "Border Settlement", "defensiveCeiling": 1, "description": "Warning and delay. Stockade, watch tower, local militia. Expected to warn and retreat. Loss under serious assault is normal." },
  { "id": "military_fort", "name": "Military Fort", "defensiveCeiling": 2, "description": "Holds against raids and modest sieges. Walls, rotating garrison, engineer assignment, supply for medium-term siege." },
  { "id": "trade_town", "name": "Trade Town", "defensiveCeiling": 2, "description": "Protects economic core. Gates, walls, civic guard, reserve mustering. Economic value exceeds defensive scaling." },
  { "id": "regional_stronghold", "name": "Regional Stronghold", "defensiveCeiling": 3, "description": "Forces a real siege. Layered defenses, inner ring, dedicated garrison, significant supply, tower networks, signal chains." },
  { "id": "primary_dynastic_keep", "name": "Primary Dynastic Keep", "defensiveCeiling": 4, "description": "Bloodline seat. Full layered fortification, apex garrison, commander presence, faith infrastructure, succession line." },
  { "id": "fortress_citadel", "name": "Fortress-Citadel", "defensiveCeiling": 5, "description": "The hardest target in the realm. Actively-invested primary keep with every leverage form maxed." }
]
```

**Step 2.2: Update `data/maps/ironmark-frontier.json` control points with settlement class:**

- Oakwatch March → `border_settlement`
- Rivergate Ford → `trade_town`
- Stonefield Watch → `military_fort`

Each control point gets `fortificationTier: 0` initially.

**Step 2.3: Assign a primary keep designation to the Command Hall.**

In the map JSON, add a `settlements` array with one entry per faction:
- Ironmark Command Hall → `primary_dynastic_keep`, `fortificationTier: 0`
- Stonehelm Command Hall → `primary_dynastic_keep`, `fortificationTier: 0`

**Step 2.4: Update `data-loader.js` to load `settlement-classes.json`.**

**Step 2.5: In `simulation.js`, attach `settlementClass` and `fortificationTier` fields to control-point objects at map load.**

Expose a derived `defensiveCeiling` getter on each control point by looking up its class.

**Step 2.6: Render the settlement class indicator on control points.**

In `renderer.js`, after the existing control-point flag-pole draw, add a small class-code label under the name label (e.g. "Border" in 9px dim text). Use a thicker stroke for higher-tier classes.

**Step 2.7: Surface in Selection panel.**

When a control point is selected (if selectable — check main.js selection model), the Selection panel shows:
- Class: Border Settlement (defensive ceiling 1)
- Fortification tier: 0 / 1

**Step 2.8: Update tests.**

In `data-validation.mjs`, add:
- Assertion: `map.controlPoints.every(cp => cp.settlementClass)` 
- Assertion: `classes.length === 6`
- Assertion: every control-point settlementClass maps to a class ID

**Step 2.9: Verify.**

```bash
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
```

Open `play.html`, verify control points show their settlement class label.

**Step 2.10: Commit.**

```bash
git add data/settlement-classes.json data/maps/ironmark-frontier.json src/game/core/data-loader.js src/game/core/simulation.js src/game/core/renderer.js src/game/main.js tests/data-validation.mjs
git commit -m "Add canonical settlement classes and fortification tier metadata"
```

---

### Task 3: Fortification Buildings (Wall, Tower, Gatehouse, Keep Tier 1)

**Files:**
- Modify: `data/buildings.json` — add wall segment, watch tower, gatehouse, keep tier 1 definitions
- Modify: `src/game/core/simulation.js` — wall/tower as distinct building categories; fortificationTier increments when built near a settlement
- Modify: `src/game/core/renderer.js` — wall as a connected segmented structure; tower as a distinct tall shape; gatehouse as a notched wall with gap; keep tier 1 as a bolder command-hall variant
- Modify: `src/game/main.js` — add build-mode buttons for fortification buildings
- Modify: `tests/data-validation.mjs` — assert new buildings exist with stone cost

**Canon compliance:**
- Canon: layered defense (outer works, inner ring, final core) with walls × towers, gates × killing bays, cisterns/granaries (FORTIFICATION_SYSTEM.md)
- Implementation: the first wave introduces the four most load-bearing fortification classes (wall segment, watch tower, gatehouse, keep-tier-1 as a Command-Hall upgrade). These stand alone as distinct buildings. The ecosystem interactions (flanking tower effects on walls, killing-bay bonus for gates, reserve-yard cycling) are deferred to the next wave. That wave will add `fortificationRole` properties and process ecosystem bonuses in a new simulation stage `updateFortificationEcosystem`.

**Step 3.1: Add to `data/buildings.json`:**

```json
{
  "id": "wall_segment",
  "name": "Wall Segment",
  "prototypeEnabled": true,
  "fortificationRole": "wall",
  "fortificationTierContribution": 1,
  "footprint": { "w": 2, "h": 1 },
  "health": 900,
  "buildTime": 10,
  "cost": { "stone": 45, "wood": 5 },
  "blocksPassage": true,
  "armor": 8,
  "notes": ["Canonical outer-works component. Health is structural — partial damage reduces effective ceiling."]
},
{
  "id": "watch_tower",
  "name": "Watch Tower",
  "prototypeEnabled": true,
  "fortificationRole": "tower",
  "fortificationTierContribution": 1,
  "footprint": { "w": 2, "h": 2 },
  "health": 650,
  "buildTime": 14,
  "cost": { "stone": 60, "wood": 20 },
  "sightBonusRadius": 180,
  "auraAttackMultiplier": 1.08,
  "auraRadius": 140,
  "notes": ["Canonical tower overwatch. Sight bonus + minor attack aura for adjacent defenders."]
},
{
  "id": "gatehouse",
  "name": "Gatehouse",
  "prototypeEnabled": true,
  "fortificationRole": "gate",
  "fortificationTierContribution": 2,
  "footprint": { "w": 3, "h": 2 },
  "health": 1200,
  "buildTime": 18,
  "cost": { "stone": 90, "wood": 40, "iron": 15 },
  "blocksPassage": false,
  "armor": 12,
  "notes": ["Canonical gate with killing-bay. Gate corridor is the only wall opening that does not block passage."]
},
{
  "id": "keep_tier_1",
  "name": "Inner Keep",
  "prototypeEnabled": true,
  "fortificationRole": "keep",
  "fortificationTierContribution": 2,
  "footprint": { "w": 4, "h": 4 },
  "health": 2600,
  "buildTime": 45,
  "cost": { "stone": 220, "wood": 80, "iron": 60, "gold": 180 },
  "armor": 16,
  "requiresUpgrade": "command_hall",
  "populationCapBonus": 18,
  "notes": ["Canonical primary-keep foundation. Constructed adjacent to the Command Hall. Unlocks higher fortification tier at the settlement."]
}
```

**Step 3.2: In `simulation.js`, add logic for fortificationTier increment.**

When a building with `fortificationRole` completes within N tiles of a settlement (control point or command hall), add its `fortificationTierContribution` to the nearest settlement's `fortificationTier`, capped at the settlement's `defensiveCeiling`.

```javascript
function onFortificationBuildingComplete(state, building) {
  if (!building.fortificationRole) return;
  const settlement = nearestSettlement(state, building);
  if (!settlement) return;
  const ceiling = settlementClassById(settlement.settlementClass).defensiveCeiling;
  settlement.fortificationTier = Math.min(
    ceiling,
    settlement.fortificationTier + (building.fortificationTierContribution ?? 1)
  );
  postMessage(state, settlement.factionId, 
    `${settlement.name} advances to fortification tier ${settlement.fortificationTier} / ${ceiling}.`, 8);
}
```

**Step 3.3: `blocksPassage` honoring.**

In unit movement steering (utils.js `moveToward` or simulation.js movement step), if the next step intersects a building footprint that has `blocksPassage: true` and belongs to a hostile faction, the unit cannot cross it. For now, simple bounding-box test is sufficient. (Full pathfinding around walls is deferred.)

**Step 3.4: Wall damage vs unit damage.**

Add a `damageType` property to unit attacks. Default units have `damageType: "standard"`. Add a `structuralDamageMultiplier` to buildings with `fortificationRole: "wall" | "gate" | "tower"`. When a standard unit attacks a wall, damage × structuralDamageMultiplier (default 0.25). This makes walls truly costly to break with line infantry, canonically aligned with wave-spam-denial doctrine.

```javascript
// simulation.js attack resolution
const target = entity;
const attackerDamage = attackerWeapon.attackDamage;
let finalDamage = attackerDamage;
if (target.type === "building" && target.fortificationRole) {
  const structuralMultiplier = target.structuralDamageMultiplier ?? 0.25;
  finalDamage *= structuralMultiplier;
}
target.health -= finalDamage;
```

Add `structuralDamageMultiplier: 0.2` to wall_segment, 0.15 to watch_tower, 0.3 to gatehouse, 0.1 to keep_tier_1 in the building definitions.

**Step 3.5: Tower sight and aura.**

When rendering units and selecting targets, towers that are friendly to a unit within their `sightBonusRadius` extend the unit's effective sight for AI targeting and renderer visibility (if fog-of-war is ever added). Tower auraAttackMultiplier applies to friendly units within auraRadius same way commander aura does.

**Step 3.6: Renderer draw cases.**

In `renderer.js` buildings draw branch, add:
- `wall_segment`: rectangle with heavy gray fill, thick dark stone outline, segmented texture (vertical gray lines every 8px)
- `watch_tower`: tall cylinder (circle on top of a rectangle), stone gray, accent color trim
- `gatehouse`: wall rect with a notched opening in the center, flanked by two mini-towers
- `keep_tier_1`: large square with four corner accents, inner smaller square

**Step 3.7: Build-mode UI.**

In `main.js` command panel, when a worker is selected, the build options now include (after existing seven):
- Wall Segment (45 stone, 5 wood)
- Watch Tower (60 stone, 20 wood)
- Gatehouse (90 stone, 40 wood, 15 iron)
- Inner Keep (220 stone, 80 wood, 60 iron, 180 gold) — available only if Command Hall exists

Disable build buttons that exceed current resources, same pattern as existing buildings.

**Step 3.8: Update tests.**

- Add assertion: `buildings.find(b => b.id === "wall_segment").fortificationRole === "wall"`
- Add assertion: `buildings.filter(b => b.fortificationRole).length === 4`
- Add assertion: `buildings.find(b => b.id === "keep_tier_1").cost.stone === 220`

Add a runtime-bridge test: build a wall_segment near the command hall, verify the Ironmark settlement's fortificationTier increments to 1.

**Step 3.9: Verify.**

```bash
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
```

Open `play.html`, build each fortification type, verify they render and increment fortification tier.

**Step 3.10: Commit.**

```bash
git add data/buildings.json src/game/core/simulation.js src/game/core/renderer.js src/game/main.js tests/data-validation.mjs tests/runtime-bridge.mjs
git commit -m "Add canonical fortification buildings: walls, towers, gatehouses, keep"
```

---

### Task 4: First Siege Engine (Ram) + Assault Failure Penalty Scaffold

**Files:**
- Modify: `data/units.json` — add ram (siege engine class)
- Modify: `src/game/core/simulation.js` — siege engine damage type, assault failure penalty logic
- Modify: `src/game/core/renderer.js` — draw ram as a distinct wheeled-log shape
- Modify: `src/game/main.js` — add ram to training commands (from barracks, or a new "Siege Workshop" if we're canonical — let's add to barracks for this wave, noting the workshop for next wave)
- Modify: `tests/data-validation.mjs` — assert ram exists with siege damage type

**Canon compliance:**
- Canon: siege engines are slow, expensive, escort-dependent, must produce canonical wall-breaching damage that line infantry cannot; wave-spam is a losing branch (SIEGE_SYSTEM.md)
- Implementation: the ram is the first siege engine. Extends the unit schema with `siegeClass`, `structuralDamageMultiplier: 3.5`. A ram attacking a wall does full structural damage (3.5 × base). A ram attacking a unit does reduced damage (0.4 × base) because it's a structural weapon. Ram moves at 30 speed (very slow), large health (450), large cost (150 wood, 100 iron, 90 gold), population cost 2.

**Step 4.1: Add to `data/units.json`:**

```json
{
  "id": "ram",
  "name": "Battering Ram",
  "stage": 2,
  "role": "siege-engine",
  "siegeClass": "ram",
  "prototypeEnabled": true,
  "populationCost": 2,
  "health": 450,
  "speed": 30,
  "attackDamage": 22,
  "attackRange": 20,
  "attackCooldown": 1.8,
  "sight": 120,
  "structuralDamageMultiplier": 3.5,
  "antiUnitDamageMultiplier": 0.4,
  "cost": { "gold": 90, "wood": 150, "iron": 100 },
  "notes": [
    "Canonical first siege engine. Primary purpose: gate and wall-base breach.",
    "Anti-unit damage reduced because it is a structural weapon.",
    "Slow, expensive, requires escort — canonical siege constraints."
  ]
}
```

**Step 4.2: Extend unit attack resolution to honor siege multipliers.**

In the attack-resolution block, when the attacker has `structuralDamageMultiplier` and `antiUnitDamageMultiplier`:
- If target is a building: `final = base × structuralDamageMultiplier × (target.structuralDamageMultiplier ?? 1)` — note the ram's 3.5 multiplier counters the wall's 0.2 multiplier, producing net 0.7× against walls (still reduced but much higher than line infantry's 0.2×)

Wait — the math needs care. Let me re-specify:
- Default unit attacking wall: base × wall.structuralDamageMultiplier = base × 0.2 (tiny)
- Ram attacking wall: base × wall.structuralDamageMultiplier × ram.structuralDamageMultiplier = base × 0.2 × 3.5 = base × 0.7 (7x more effective than infantry but still reduced vs wall armor)
- Ram attacking unit: base × ram.antiUnitDamageMultiplier = base × 0.4 (reduced)
- Default unit attacking unit: base (no change)

This preserves:
1. Walls are hard for infantry (canon) — base × 0.2
2. Rams break walls canonically — base × 0.7 (3.5x infantry)
3. Rams are weak vs units (canon: need escort) — base × 0.4
4. Infantry kills units fine — base × 1.0

**Step 4.3: Assault failure penalty scaffold.**

Track per-attack-force morale/cohesion as aggregate state on faction: `faction.assaultCohesion = 1.0` (default). When a faction's combat units die within the aura radius of a fortified building (fortificationRole set) belonging to a hostile faction, increment an `assaultFailureStrain` counter. When `assaultFailureStrain` exceeds a threshold (say, 6 combat units lost near fortifications in 30 seconds), apply a cohesion penalty: all surviving combat units of that faction get attack × 0.85 for 20 seconds. Reset strain counter after penalty.

This is the minimum viable wave-spam denial that canon demands. Future waves will expand this to include supply drain, tempo penalty, repair-window bonus for defender, etc.

```javascript
// In simulation.js finalizeDeaths or similar
function tickAssaultCohesion(state, dt) {
  for (const faction of state.factions.values()) {
    // Decay strain over time
    faction.assaultFailureStrain = Math.max(0, (faction.assaultFailureStrain ?? 0) - dt * 0.1);
    // Check threshold
    if ((faction.assaultFailureStrain ?? 0) > 6 && !(faction.cohesionPenaltyUntil > state.timeSeconds)) {
      faction.cohesionPenaltyUntil = state.timeSeconds + 20;
      faction.assaultFailureStrain = 0;
      postMessage(state, faction.id, 
        `${faction.name} assault force shaken: cohesion penalty in effect.`, 10);
    }
  }
}

// In unit attack resolution, apply cohesion penalty to attack damage
function getAttackMultiplier(attacker, state) {
  const faction = state.factions.get(attacker.factionId);
  if (faction.cohesionPenaltyUntil > state.timeSeconds) {
    return 0.85;
  }
  return 1.0;
}

// On combat unit death near hostile fortification
function onCombatUnitDeath(state, unit) {
  const nearFortification = isWithinFortificationAura(state, unit, unit.factionId);
  if (nearFortification) {
    const faction = state.factions.get(unit.factionId);
    faction.assaultFailureStrain = (faction.assaultFailureStrain ?? 0) + 1;
  }
}
```

**Step 4.4: Renderer ram draw.**

In `renderer.js` units draw, add `role === "siege-engine"` case: draw as a horizontal rectangle (log body) with two small circles underneath (wheels). Faction primary color for the log, accent for wheels. Larger footprint than infantry (8×4 units or so, scaled for visibility).

**Step 4.5: Training UI.**

In `main.js`, barracks training panel, add ram training button when conditions met: (a) player has ≥ 150 wood and 100 iron and 90 gold, (b) player has at least one barracks with training queue slot.

Future: move ram to a dedicated Siege Workshop building. For this wave, barracks is adequate.

**Step 4.6: Tests.**

- Add assertion: `units.find(u => u.id === "ram").siegeClass === "ram"`
- Add assertion: `units.find(u => u.id === "ram").structuralDamageMultiplier > 1`
- Add runtime-bridge test: spawn a ram and a wall, have the ram attack, verify wall health decreases at ram-damage rate; also have a swordsman attack the wall, verify it decreases at infantry-damage rate (much slower).

**Step 4.7: Verify + commit.**

```bash
node tests/data-validation.mjs && node tests/runtime-bridge.mjs
git add data/units.json src/game/core/simulation.js src/game/core/renderer.js src/game/main.js tests/data-validation.mjs tests/runtime-bridge.mjs
git commit -m "Add Battering Ram siege engine and assault cohesion penalty scaffold"
```

---

### Task 5: Canonical 90-Second Population Cycle with Famine + Water Crisis

**Files:**
- Modify: `src/game/core/simulation.js` — add `tickRealmConditionCycle` stage, run every 90 seconds
- Modify: `data/` — add `data/realm-conditions.json` with pressure thresholds
- Modify: `tests/runtime-bridge.mjs` — add famine and water crisis tests

**Canon compliance:**
- Canon: 90-second cycles, famine after 2 consecutive sub-food cycles, water crisis triggers outmigration (POPULATION_SYSTEM.md + Master Doctrine 2026-04-14)
- Implementation: the existing 18-second `tickPopulationGrowth` **stays in place** as the visible heartbeat. A new `tickRealmConditionCycle` fires every 90 seconds. It evaluates the canonical pressure states and fires famine / water-crisis events with real consequence (reduced growth, loyalty impact, outmigration).

**Step 5.1: Add `data/realm-conditions.json`:**

```json
{
  "cycleSeconds": 90,
  "thresholds": {
    "foodStrainRatio": 1.0,
    "foodFamineConsecutiveCycles": 2,
    "waterStrainRatio": 1.0,
    "waterCrisisConsecutiveCycles": 1,
    "populationCapPressureRatio": 0.95
  },
  "effects": {
    "famine": {
      "populationDeclinePerCycle": 1,
      "loyaltyDeltaPerCycle": -4,
      "unitMoraleMultiplier": 0.9
    },
    "waterCrisis": {
      "outmigrationPerCycle": 1,
      "loyaltyDeltaPerCycle": -6,
      "territoryAgricultureMultiplier": 0.7
    },
    "capPressure": {
      "loyaltyDeltaPerCycle": -1
    }
  }
}
```

**Step 5.2: Extend `data-loader.js` to load `realm-conditions.json`.**

**Step 5.3: In `simulation.js`, add the cycle tick.**

```javascript
function tickRealmConditionCycle(state, dt) {
  state.realmCycleAccumulator = (state.realmCycleAccumulator ?? 0) + dt;
  const cycleSeconds = state.realmConditions?.cycleSeconds ?? 90;
  if (state.realmCycleAccumulator < cycleSeconds) return;
  state.realmCycleAccumulator -= cycleSeconds;
  state.realmCycleCount = (state.realmCycleCount ?? 0) + 1;

  for (const faction of state.factions.values()) {
    // Food strain
    const foodNeeded = faction.populationTotal;
    const foodOnHand = faction.resources.food ?? 0;
    if (foodOnHand < foodNeeded * state.realmConditions.thresholds.foodStrainRatio) {
      faction.foodStrainStreak = (faction.foodStrainStreak ?? 0) + 1;
    } else {
      faction.foodStrainStreak = 0;
    }
    if (faction.foodStrainStreak >= state.realmConditions.thresholds.foodFamineConsecutiveCycles) {
      applyFamine(state, faction);
    }

    // Water strain
    const waterNeeded = faction.populationTotal;
    const waterOnHand = faction.resources.water ?? 0;
    if (waterOnHand < waterNeeded * state.realmConditions.thresholds.waterStrainRatio) {
      faction.waterStrainStreak = (faction.waterStrainStreak ?? 0) + 1;
    } else {
      faction.waterStrainStreak = 0;
    }
    if (faction.waterStrainStreak >= state.realmConditions.thresholds.waterCrisisConsecutiveCycles) {
      applyWaterCrisis(state, faction);
    }

    // Cap pressure
    if (faction.populationTotal >= faction.populationCap * state.realmConditions.thresholds.populationCapPressureRatio) {
      applyCapPressure(state, faction);
    }
  }
}

function applyFamine(state, faction) {
  const effects = state.realmConditions.effects.famine;
  faction.populationTotal = Math.max(0, faction.populationTotal - effects.populationDeclinePerCycle);
  for (const cp of state.controlPoints) {
    if (cp.ownerFactionId === faction.id) {
      cp.loyalty = Math.max(0, cp.loyalty + effects.loyaltyDeltaPerCycle);
    }
  }
  postMessage(state, faction.id, `${faction.name} suffers famine: population and loyalty declining.`, 12);
  // Conviction event
  addConvictionEvent(state, faction.id, "famine_endured", -0.5);
}

function applyWaterCrisis(state, faction) {
  const effects = state.realmConditions.effects.waterCrisis;
  faction.populationTotal = Math.max(0, faction.populationTotal - effects.outmigrationPerCycle);
  for (const cp of state.controlPoints) {
    if (cp.ownerFactionId === faction.id) {
      cp.loyalty = Math.max(0, cp.loyalty + effects.loyaltyDeltaPerCycle);
    }
  }
  postMessage(state, faction.id, `${faction.name} suffers water crisis: people leaving territory.`, 12);
}

function applyCapPressure(state, faction) {
  const effects = state.realmConditions.effects.capPressure;
  for (const cp of state.controlPoints) {
    if (cp.ownerFactionId === faction.id) {
      cp.loyalty = Math.max(0, cp.loyalty + effects.loyaltyDeltaPerCycle);
    }
  }
}
```

**Step 5.4: Hook into `stepSimulation`.**

Add `tickRealmConditionCycle(state, dt)` after `updateFaithExposure` in the stepSimulation dispatch.

**Step 5.5: Write runtime-bridge test.**

Scenario: set player food reserve to 0, run the simulation for 200 seconds, verify famine fires twice and population declines.

**Step 5.6: Verify + commit.**

```bash
node tests/runtime-bridge.mjs  # expect famine + water crisis tests green
git add data/realm-conditions.json src/game/core/data-loader.js src/game/core/simulation.js tests/runtime-bridge.mjs
git commit -m "Add canonical 90-second realm condition cycle with famine and water crisis"
```

---

### Task 6: Realm Condition Legibility UI (HUD + Panel)

**Files:**
- Modify: `src/game/main.js` — add pressure state pills to HUD or a new realm-condition panel section
- Modify: `src/game/styles.css` — styling for pressure indicators (green/yellow/red)
- Modify: `src/game/core/simulation.js` — expose a `getRealmConditionSnapshot(faction)` helper returning the 11 pressure states

**Canon compliance:**
- Canon: 11 pressure states must be player-visible with green/yellow/red urgency + next-cycle prediction (Next-Lane map section 4)
- Implementation: this wave surfaces the 6 most load-bearing pressure states (population, food, water, loyalty, fortification, military). The remaining 5 (iron scarcity, gold liquidity, supply line health, faith intensity, conviction posture) are deferred — conviction and faith are already partially visible in existing panels, iron and gold will be naturally visible from the resource bar.

**Step 6.1: Add `getRealmConditionSnapshot(state, factionId)` helper in simulation.js.**

Returns:
```javascript
{
  population: { value, cap, strain: "green"|"yellow"|"red", trend: "+N"|"-N" },
  food: { stock, need, strain, famineStreak },
  water: { stock, need, strain, crisisStreak },
  loyalty: { average, min, trend, strain },
  fortification: { primaryKeepTier, ceiling, strain },
  military: { total, combatCapacity, strain }
}
```

**Step 6.2: Add HUD realm-condition bar.**

In play.html, add a new HTML row under the resource bar called `realm-condition-bar`. Style it as a horizontal band with 6 pressure-state pills, each showing:
- Label (Pop, Food, Water, Loyalty, Fort, Army)
- Numeric value (current / max or current / needed)
- Color dot (green / yellow / red)
- Trend arrow (↑ ↓ or →)

**Step 6.3: Wire to each frame's panel render.**

In `main.js` `renderPanels()`, after the existing resource bar update, compute and render the realm-condition bar for the player faction.

**Step 6.4: CSS.**

Add `.realm-condition-bar` with horizontal flex layout, subtle border, and `.pressure-pill.green`, `.pressure-pill.yellow`, `.pressure-pill.red` for dot coloring.

**Step 6.5: Verify + commit.**

Open `play.html`, verify the new band appears and updates. Force a famine (starve population) and verify food pill goes red.

```bash
git add src/game/main.js src/game/styles.css src/game/core/simulation.js play.html
git commit -m "Add realm condition legibility bar to HUD"
```

---

### Task 7: Verification Sweep

```bash
cd D:/ProjectsHome/FisherSovereign/lancewfisher-v2/bloodlines
node tests/data-validation.mjs     # all existing + new assertions pass
node tests/runtime-bridge.mjs      # all existing + new scenarios pass
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
```

Open `play.html` in browser, play through 3-4 minutes:
- Build quarry and iron_mine, verify stone and iron accumulate
- Verify iron_mine stalls without wood fuel
- Build wall, tower, gatehouse around Command Hall, verify fortification tier rises to 3
- Build a ram in barracks, attack an enemy wall, verify wall breaks faster than with infantry
- Let food drop, verify famine fires after 2 cycles and pop declines
- Verify realm-condition bar shows correct green/yellow/red state

---

### Task 8: Continuity Updates (Phase 5 per directive)

**Files:**
- Modify: `00_ADMIN/CHANGE_LOG.md` — add 2026-04-14 entry for this wave
- Modify: `CURRENT_PROJECT_STATE.md` — add runtime status bullets for stone/iron/fortification/siege/realm cycle
- Modify: `NEXT_SESSION_HANDOFF.md` — update next recommended action to reflect what's done
- Modify: `continuity/PROJECT_STATE.json` — update `primary_next_focus` to reflect advanced state
- Modify: `tasks/todo.md` — mark this wave's items completed, add next-wave items
- Modify: `docs/DEVELOPMENT_REALITY_REPORT.md` — additively note new runtime capabilities
- Modify: `docs/COMPLETION_STAGE_GATES.md` — update gate-status table (Gate 1 now closer to met, Gate 2 advanced, Gate 4 bloodline layer met)

**Step 8.1:** Write new change log entry.
**Step 8.2:** Update project state bullets.
**Step 8.3:** Update handoff with new "next recommended action" that lists the **next wave's** priorities:
- Reserve cycling for garrisons
- Governor specialization (city / border / keep)
- Faith-integrated fortification bonuses
- Commander presence at keep
- Captured member rescue / ransom operations
- Full pathfinding around walls
- Siege Workshop as distinct building
- Additional siege engines (siege tower, trebuchet, ballista)
- AI awareness of fortification tier (refuse underforce assaults)
- Expand realm legibility to all 11 pressure states

---

## Remember

- **Additive only** — nothing deleted, every existing system preserved
- **Canon fidelity** — each task closes a canon-runtime gap, none compress canon
- **Scale-first** — extensibility for eventual 9 houses, 4 faiths, 5 stages, naval, L3-L5 units, multiple maps
- **Frequent commits** — one commit per task, recoverable if any step fails
- **Tests green** — every task ends with tests green before moving on
- **Ground truth preserved** — the prior simulation.js stages stay in place; new stages are added in the dispatch

## Execution Handoff

This plan is being executed inline in the current session, not dispatched to a subagent. Each task is a commit checkpoint. If the session runs out of budget mid-task, the next session resumes at the unmarked task.
