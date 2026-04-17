# BLOODLINES - CONTINUATION PROMPT FOR NEXT SESSION

Copy the full block below into a fresh session to continue active development on Bloodlines from the end of 2026-04-14 session 3.

---

You are continuing active development on the RTS game project BLOODLINES inside the canonical root:

- `D:\ProjectsHome\Bloodlines`

This is an implementation session, not a planning-only session and not a scope-reduction session. Bloodlines is being built at full intended scale. Do not flatten the project into a smaller RTS substitute.

## Required Read Order

Read these files before making meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\ENVIRONMENT_REPORT_2026-04-14.md`
6. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
9. `D:\ProjectsHome\Bloodlines\01_CANON\DEFENSIVE_FORTIFICATION_DOCTRINE.md`
10. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\FORTIFICATION_SYSTEM.md`
11. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\SIEGE_SYSTEM.md`
12. `D:\ProjectsHome\Bloodlines\docs\plans\2026-04-14-fortification-siege-population-legibility-wave.md`
13. `D:\ProjectsHome\Bloodlines\tasks\todo.md`
14. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`
15. `D:\ProjectsHome\Bloodlines\SOURCE_PROVENANCE_MAP.md`

## Non-Negotiables

- Preserve the canonical root. Do not create a parallel Bloodlines root.
- Preserve the browser reference simulation in parallel with Unity. It remains the working spec.
- Preserve the `Bloodlines/` stub folder. Do not use it as the main Unity project.
- Preserve full scope: dynasty, bloodline, faith, logistics, water, population, settlement classes, layered fortification, siege commitment, long match structure, AI siege refusal, theatre zoom, bloodline UI presence.
- Delete nothing without explicit authorization.

## Current State (End of Session 3)

### Browser reference simulation

Live now:

- dynasty consequence cascade
- commander aura
- territory capture plus stabilization
- faith exposure plus doctrine commitment plus doctrine effects
- conviction ledger
- stone plus iron economy with smelting fuel chain
- fortification building class (`wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`)
- settlement class plus fortification tier metadata
- battering ram siege engine with structural and anti-unit multipliers
- assault cohesion strain for wave-spam denial
- canonical 90-second realm condition cycle with famine, water crisis, cap pressure
- `getRealmConditionSnapshot`
- renderer silhouettes for stone, iron, fortifications, and ram
- 10-pill resource bar
- 6-pill realm-condition HUD bar
- worker build palette for quarry, iron mine, and fortification buildings
- AI refusal to directly assault fortified keeps without siege support
- expanded validation coverage for fortification tiering, smelting, famine, ram wall damage, and AI refusal

Validation green at session close:

```powershell
Set-Location D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
```

### Unity production lane

Live now:

- canonical Unity project at `D:\ProjectsHome\Bloodlines\unity`
- approved `_Bloodlines/` folder baseline created
- ScriptableObject definitions extended for fortification, siege, settlement, and realm-condition canon
- JSON importer extended for `settlement-classes.json` and `realm-conditions.json`
- environment audit written

Still not done:

- Unity version alignment decision
- first Unity open after the structural wave
- first `Bloodlines -> Import -> Sync JSON Content`
- ECS foundation code
- first playable ECS scene
- battlefield camera and Input System action map
- bloodline HUD panel
- realm-condition dashboard
- strategic zoom transition

## Open Blocker

Unity version alignment is still unresolved.

- Approved target: Unity 6.3 LTS (`6000.3.13f1`)
- Current `unity/` project target: Unity 6.4 (`6000.4.2f1`)

Do not write ECS code until this is resolved with Lance. Recommended path: downgrade the canonical Unity project to 6.3 LTS and re-resolve the DOTS package set there.

## Immediate Next Action

If Lance has not already answered in the conversation, ask whether to lock `unity/` to `6000.3.13f1` and downgrade the package set for LTS compliance.

If Lance confirms the downgrade:

1. Update `unity/ProjectSettings/ProjectVersion.txt` to `6000.3.13f1`
2. Adjust `unity/Packages/manifest.json` to the 6.3-compatible DOTS package versions
3. Open `unity/`
4. Run `Bloodlines -> Import -> Sync JSON Content`
5. Verify `.asset` generation under `Assets/_Bloodlines/Data/*`
6. Begin ECS foundation implementation

## Execution Order After Unity Is Unblocked

### Step 1

Run the first Unity sync and commit the generated ScriptableObject assets.

### Step 2

Write the ECS foundation in `unity/Assets/_Bloodlines/Code/`:

- components for position, faction, health, unit/building type, resource node, carry state, control point, settlement, commander, governor, bloodline role/status, faith, conviction, assault cohesion
- systems for resource accumulation, population growth, realm cycle, movement, gather/deliver, smelting fuel, production, construction, combat, assault cohesion, territory capture, faith exposure, dynasty cascade
- authoring plus baking for units, buildings, resource nodes, control points, settlements, and map seed

### Step 3

Create:

- `Scenes/Bootstrap/Bootstrap.unity`
- `Scenes/Gameplay/IronmarkFrontier.unity`

### Step 4

Continue the browser lane in parallel with the next fortification/siege wave:

- reserve cycling for garrisons
- governor specialization (city / border / keep)
- captured-member rescue / ransom operations
- faith-integrated fortification bonuses
- commander keep-presence bonuses
- `siege_tower` and `trebuchet`
- deeper AI siege preparation beyond simple refusal
- expand the HUD from 6 realm-condition pills toward the full 11-state dashboard

## Verification Commands

### Browser

```powershell
Set-Location D:/ProjectsHome/Bloodlines
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Open:

- `http://localhost:8057/`
- `http://localhost:8057/play.html`

### Unity

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe" -projectPath "D:/ProjectsHome/Bloodlines/unity"
```

Then run the Unity menu action:

- `Bloodlines -> Import -> Sync JSON Content`

## Completion Protocol

Before ending the next meaningful work block:

1. Update `CURRENT_PROJECT_STATE.md`
2. Update `NEXT_SESSION_HANDOFF.md`
3. Update `continuity/PROJECT_STATE.json`
4. Add an entry to `00_ADMIN/CHANGE_LOG.md`
5. Update `tasks/todo.md`
6. Write the next continuation prompt file
7. Verify tests are green

## Final Directive

Continue building Bloodlines toward its full intended large-scale RTS form. Do not compress scope. Do not abandon the browser reference lane. Do not start ECS work until the Unity version decision is resolved.

---
