# BLOODLINES — CONTINUATION PROMPT FOR NEXT SESSION

**Copy and paste the entire block below (between the two horizontal rules) into a fresh session to continue building Bloodlines toward full completion at full intended scale.**

This prompt is self-contained. It does not depend on conversation memory. It is written so that any capable AI coding assistant operating in this machine environment can pick up where the previous session left off without losing ground.

---

You are continuing active development on the RTS game project BLOODLINES.

This is not a planning session, not a conceptual session, and not a reduction-of-scope session. This is a live local development environment. The machine already has the required toolchain. The canonical Bloodlines project already exists. The previous session delivered: a browser reference simulation wave (stone + iron economy with smelting, canonical fortification building class, first siege engine, 90-second realm condition cycle with famine + water crisis + cap pressure, assault cohesion strain for wave-spam denial, realm condition snapshot export for UI legibility) AND a Unity production project alignment wave (full approved `Assets/_Bloodlines/` structure, ScriptableObject definitions extended for all new canon, JSON → ScriptableObject importer extended, documentation seeded, stub folder preserved with notice, environment report written).

Your responsibility is to continue pushing Bloodlines toward its full intended final form at grand epic scale, across both the Unity production lane and the browser reference lane, without compressing the design and without substituting a smaller version for the real thing.

---

## NON-NEGOTIABLE OPERATING TRUTH

Bloodlines is being built at full intended scale. There is no MVP-first constraint. There is no "start small" philosophy. There is no authority to strip away complexity simply because it is harder to build. Every next move must push the project materially closer to the full large-scale RTS it is intended to become, with:

- large unit counts
- multi-house / multi-faction systems (9 canonical houses)
- deep economy with gold, food, water, wood, stone, iron, influence
- the 90-second canonical population cycle with famine, water crisis, outmigration, cap pressure
- faith systems (4 covenants, light/dark doctrines, L3/L4/L5 unit rosters, Apex-tier manifestations)
- dynasty / bloodline systems (20-member roster, succession cascade, commander/governor/diplomat/ideological-leader/spymaster/merchant/sorcerer/heir/head roles, capture/rescue/ransom)
- territory systems with loyalty, stabilization, 6 canonical settlement classes (border → fortress-citadel)
- canonical fortification architecture (outer works / inner ring / final core) with walls × towers, gates × killing bays, garrison × reserves, faith-integrated defensive expressions
- siege as campaign-scale commitment with engines (ram / siege tower / trebuchet / ballista / mantlet / bombard), engineers, logistics, scouting, breach planning, elites, faith powers, sabotage, multi-front timing, isolation
- assault failure penalties that canonically deny wave-spam against developed fortifications
- long matches (6-10 hours real time, 5-stage match structure, declared-time dual-clock, 3-5 years in-world per major siege)
- AI-driven factions that refuse underforce assaults and plan siege preparation
- larger regional and theatre-of-war awareness with zoom transition and fog-of-war scaling
- 11 canonical realm pressure states surfaced in the UI
- top bloodline members structurally visible in the HUD

Do not compress this scope. Do not reinterpret "progress" as "reduce complexity". Do not propose an MVP-first substitute. Do not flatten the dynastic, faith, territorial, or fortification systems into generic RTS conventions.

---

## GAMEPLAY AND VISUAL DIRECTION (NON-NEGOTIABLE)

The close-in battlefield feel of Bloodlines must be similar in spirit to **Command & Conquer: Generals** and **Command & Conquer: Generals Zero Hour**:

- immediately readable RTS battlefield
- intuitive base building with familiar structure placement
- recognizable command-and-control flow
- clear and satisfying construction behavior
- responsive unit control
- strong visual identity of units and buildings
- legible silhouettes
- top-down or angled battlefield presentation
- the feeling of commanding a live war zone

Bloodlines goes beyond that foundation with:

- **Expanded command layer** — more menus, more prompts, more next-step guidance, more dynastic decisions, more visible system interplay between war / dynasty / economy / faith / leadership
- **Scale transition** — zoom out into a larger theatre-of-war view with continuity of war-space between battlefield and strategic scales
- **Bloodline command presence** — top bloodline members shown in the UI at all times, not bolted on; structurally integrated into the command experience

---

## APPROVED ENGINE + TOOLCHAIN (NON-NEGOTIABLE)

- **Engine:** Unity 6.3 LTS (`6000.3.13f1`). Both 6.3 LTS and 6.4 are installed locally; the approved target is 6.3 LTS.
- **Core architecture:** DOTS / ECS (`com.unity.entities`), Burst, Collections, Unity Mathematics, Entities.Graphics.
- **Rendering:** URP (Universal Render Pipeline).
- **Input:** Unity Input System 1.19+.
- **Addressables:** com.unity.addressables 2.9+.
- **Networking (staged):** Netcode for Entities, deferred until multiplayer lane activates.
- **IDE:** Visual Studio (VS 18 Community locally; VS 2022 also acceptable).
- **3D:** Blender 5.1.
- **VCS:** Git + GitHub Desktop.
- **Audio (staged):** Wwise — not yet installed; `Audio/Middleware/` reserved.

Do not replace the approved engine stack without a clearly demonstrated technical blocker.

---

## CANONICAL ROOT + READ ORDER

Canonical session root: `D:\ProjectsHome\Bloodlines` (resolves through a junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`). Do not create parallel roots.

Read these files in order before making meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\ENVIRONMENT_REPORT_2026-04-14.md`
6. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md` (large — skim structure, deep-read on demand)
9. `D:\ProjectsHome\Bloodlines\01_CANON\DEFENSIVE_FORTIFICATION_DOCTRINE.md`
10. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\FORTIFICATION_SYSTEM.md`
11. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\SIEGE_SYSTEM.md`
12. `D:\ProjectsHome\Bloodlines\docs\plans\2026-04-14-fortification-siege-population-legibility-wave.md`
13. `D:\ProjectsHome\Bloodlines\tasks\todo.md`
14. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`
15. `D:\ProjectsHome\Bloodlines\SOURCE_PROVENANCE_MAP.md`

Then load subsystem detail from the numbered canon folders and `docs/` as needed.

---

## CURRENT STATE (End of 2026-04-14 Session 2)

### Browser Reference Simulation (`<repo>/play.html`, `<repo>/src/game/`, `<repo>/data/`)

Live systems as of session 2 close:

- Dynasty cascade (head / heir / commander / governor succession, legitimacy delta, captive ledger, ransom influence trickle, fallen ledger)
- Commander aura (attack multiplier, sight bonus, aura radius based on renown + doctrine)
- Territory capture + stabilization (neutral → occupied → stabilized at loyalty 72, governor bonuses)
- Faith exposure, doctrine commitment, doctrine effects (4 covenants with light/dark paths, prototype effects live)
- Conviction ledger (stewardship / ruthlessness / oathkeeping / desecration buckets, 5 bands)
- Stone + iron economy with canonical smelting chain (iron_mine consumes wood at 0.5 ratio; ore returns to node if fuel insufficient)
- Fortification building class (wall_segment, watch_tower, gatehouse, keep_tier_1) with structural damage multipliers (wall 0.2, tower 0.15, gate 0.3, keep 0.1)
- First siege engine (Battering Ram: structural 3.5×, anti-unit 0.4×, Stage 2, pop cost 2, expensive stone + wood + iron cost)
- Settlement class + fortification tier metadata (6 classes: border_settlement → fortress_citadel)
- Canonical 90-second realm condition cycle (famine after 2 consecutive cycles, water crisis after 1 cycle, cap pressure at 95% occupancy)
- Assault cohesion strain (wave-spam denial: threshold 6 deaths near hostile fortification → 20-second 0.85× cohesion penalty)
- Realm condition snapshot export (`getRealmConditionSnapshot`) returning the 11-state legibility snapshot

Tests green: `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs`.

Pending in browser lane (for this next session to continue):

- Renderer draw cases for stone / iron nodes (gray circles, rust circles)
- Renderer draw cases for wall_segment, watch_tower, gatehouse, keep_tier_1, ram (distinct shapes per fortification role, siege engine wheel glyph)
- Renderer draw label for settlement class + fortification tier on control points
- `main.js` build buttons for quarry, iron_mine, wall_segment, watch_tower, gatehouse, keep_tier_1 + ram training from barracks
- `main.js` resource bar pills for stone + iron
- `main.js` realm condition HUD bar showing the 11 pressure states (or at least the 6 most load-bearing)
- `play.html` markup for the realm condition HUD bar
- New test assertions for: stone/iron enabled, quarry + iron_mine schemas, wall/tower/gate/keep exist with fortification role, ram exists with structural multiplier, famine cycle fires under starvation
- AI extension: enemy AI refuses to attack fortified keeps without siege units (canonical AI requirement)

### Unity Production Project (`<repo>/unity/`)

Full approved `Assets/_Bloodlines/` baseline created (Art, Audio, Code, Data, Prefabs, Scenes, Materials, Shaders, Animation, Docs — with all canonical sub-sub-folders). ScriptableObject layer extended for all new canon. JSON → ScriptableObject importer covers all 12 canonical JSON files including new `settlement-classes.json` and `realm-conditions.json`.

Not yet done:

- Unity version alignment decision (see Open Decision below)
- First `Bloodlines → Import → Sync JSON Content` invocation (generates all `.asset` files under `Assets/_Bloodlines/Data/*`)
- First `IComponentData` / `ISystem` / `Authoring` / `Baking` code
- First playable ECS scene
- Battlefield camera + Input System action map
- Top-bloodline-members HUD panel
- Realm condition dashboard
- Strategic zoom transition

### Stub folder preserved

`<repo>/Bloodlines/` is a fresh Unity 6.4 URP 3D template stub (not the real project). Preserved in place with `STUB_TEMPLATE_NOTICE.md`. Do not open it as the primary Unity project. Do not delete it without explicit authorization.

---

## OPEN DECISION (BLOCKS ECS CODE)

**Unity version alignment.** Installed editors: `6000.3.13f1` (Unity 6.3 LTS, approved) and `6000.4.2f1` (Unity 6.4). The canonical `<repo>/unity/` project currently targets Unity 6.4. Approved architecture says Unity 6.3 LTS.

**Your first action:** ask Lance which path to take (A) accept drift to Unity 6.4 and update the approval statement, (B) downgrade `unity/` to 6.3 LTS and re-resolve DOTS packages to 6.3 LTS-compatible versions, or (C) dual-editor. If Lance has already answered in the conversation, follow that answer. The recommended choice is **(B) downgrade to 6.3 LTS** for LTS compliance per the approved toolchain.

Do not write ECS code until this is resolved.

---

## EXECUTION SEQUENCE (Do These In Order)

### Step 1 — Confirm / Resolve Unity Version

If Lance has not yet answered, ask. If Lance has said "downgrade to 6.3 LTS", edit `<repo>/unity/ProjectSettings/ProjectVersion.txt` to `6000.3.13f1` and re-resolve `<repo>/unity/Packages/manifest.json` to 6.3 LTS-compatible Entities / Burst / Collections / Mathematics / Entities.Graphics versions. Verify manifest changes compile cleanly on first Unity open.

### Step 2 — Unity First Open + Sync

Open `<repo>/unity/` in Unity Hub using the locked version. Let Unity regenerate `.meta` files for the newly created folders under `Assets/_Bloodlines/`. Then run the menu command: **Bloodlines → Import → Sync JSON Content**. Verify all ScriptableObject `.asset` files appear under `Assets/_Bloodlines/Data/*/`. Commit them to git.

### Step 3 — ECS Foundation

In `<repo>/unity/Assets/_Bloodlines/Code/`, write the minimum viable ECS foundation. All of this is full production code — no placeholders, no "good enough for prototype" compromises on architecture. Make it scale.

**Components (`Code/Components/`):**

- `PositionComponent` (float3 — use Unity Mathematics)
- `FactionComponent` (FixedString32Bytes factionId + owner color)
- `HealthComponent` (current + max)
- `UnitTypeComponent` (FixedString32Bytes typeId + siegeClass + structuralDamageMultiplier + antiUnitDamageMultiplier)
- `BuildingTypeComponent` (FixedString32Bytes typeId + fortificationRole + fortificationTierContribution + structuralDamageMultiplier + smeltingFuelResource + smeltingFuelRatio + blocksPassage)
- `ResourceNodeComponent` (resource type enum + amount)
- `ResourceCarryComponent` (for workers: carrying type + amount)
- `ControlPointComponent` (settlementClass + fortificationTier + defensiveCeiling + loyalty + captureProgress + ownerFactionId)
- `SettlementComponent` (settlementClass + fortificationTier + defensiveCeiling + anchorBuildingEntity)
- `CommanderComponent` (memberId + renown + auraRadius)
- `GovernorComponent` (memberId + bonusMultiplier)
- `BloodlineRoleComponent` (role enum: head / heir / commander / governor / diplomat / ideological_leader / merchant / sorcerer / spymaster)
- `BloodlineStatusComponent` (status enum: active / commanding / governing / captured / fallen / displaced / dormant)
- `FaithComponent` (discoveredFaiths + selectedFaith + doctrinePath + intensity + exposureMap)
- `ConvictionComponent` (stewardship + ruthlessness + oathkeeping + desecration + derivedScore + band)
- `AssaultCohesionComponent` (faction-level: strain + penaltyUntil)

**Systems (`Code/Systems/`):**

- `ResourceAccumulationSystem` — passive trickle from farms / wells / control points
- `PopulationGrowthSystem` — visible heartbeat (faster cadence for feedback)
- `RealmConditionCycleSystem` — canonical 90-second deep cycle; mirror the browser `tickRealmConditionCycle`: food strain streak, famine at 2 consecutive cycles, water crisis at 1 cycle, cap pressure
- `UnitMovementSystem` — Burst-compiled steering; use `IJobEntity` for scale
- `UnitGatherSystem` — worker assignment to resource nodes, carry/deliver loop
- `SmeltingFuelSystem` — on delivery to iron_mine: consume wood at 0.5 ratio, return ore to node if insufficient
- `BuildingProductionSystem` — queued unit spawning from command_hall / barracks / future siege_workshop
- `ConstructionSystem` — worker-driven build progress, fortificationTier advancement on completion
- `CombatTargetingSystem` — nearest enemy in sight range
- `CombatDamageSystem` — apply damage with canonical structural + anti-unit multipliers; wall-vs-infantry = base × 0.2, wall-vs-ram = base × 0.7, ram-vs-unit = base × 0.4
- `AssaultCohesionSystem` — track strain, apply 0.85× cohesion penalty at threshold 6 for 20 seconds
- `TerritoryCaptureSystem` — neutral → occupied → stabilized at loyalty 72
- `FaithExposureSystem` — sacred site proximity exposure
- `DynastyCascadeSystem` — succession, capture/kill resolution, legitimacy delta, captive ledger

**Authoring + Baking (`Code/Authoring/` + `Code/Baking/`):**

- `UnitAuthoring` + `UnitBaker`
- `BuildingAuthoring` + `BuildingBaker`
- `ResourceNodeAuthoring` + `ResourceNodeBaker`
- `ControlPointAuthoring` + `ControlPointBaker`
- `SettlementAuthoring` + `SettlementBaker`
- `MapSeedAuthoring` + `MapSeedBaker` (reads `MapDefinition` ScriptableObject and spawns the seeded world)

**Bootstrap scene (`Scenes/Bootstrap/Bootstrap.unity`):**

- Subscene with `MapSeedAuthoring` referencing `Data/MapDefinitions/ironmark_frontier.asset`
- Camera rig (ECS-native or classic camera attached to entity)
- Input System action map asset referenced

**Gameplay scene (`Scenes/Gameplay/IronmarkFrontier.unity`):**

- Loads Bootstrap subscene
- Contains rendered canvas for UI overlay (UI Toolkit or uGUI — recommend UI Toolkit for data-binding)

### Step 4 — Battlefield Camera + Input (Generals/Zero Hour Feel)

**`Code/Camera/`:**

- `BattlefieldCamera.cs` — pan with WASD/arrows, zoom with mouse scroll, edge-scroll toggle, orbit optional
- `StrategicZoomCamera.cs` — tween to theatre-of-war view (higher altitude, wider FOV, different fog settings)
- `CameraModeController.cs` — switches between battlefield and strategic, preserves smooth transition, binds to a keybind (default: Z or hold-spacebar)

**`Code/Input/`:**

- `BloodlinesInputActions.inputactions` — Input System asset with: `Pan`, `Zoom`, `Select`, `DragSelect`, `Command` (right-click), `BuildMode`, `CancelBuild`, `ZoomMode`, `Pause`, `PanicReturnToBase`
- `InputHandler.cs` — system that reads input actions and translates to ECS commands
- `SelectionSystem.cs` — click-select + drag-box select

### Step 5 — HUD With Bloodline Presence (Canonical Non-Negotiable)

**`Code/UI/`:**

- `HudRoot.uxml` + `HudRoot.uss` + `HudRoot.cs` — persistent HUD with:
  - Resource bar (10 pills: gold, food, water, wood, stone, iron, influence, available pop, total pop, territory count)
  - **Top bloodline members panel** (always visible — head, heir, commander, governor, ideological leader, spymaster if assigned; each with name, role, status, location)
  - Selection panel (contextual)
  - Command panel (contextual)
  - Dynasty panel (expandable — full roster)
  - Faith panel (exposures, doctrine, intensity)
  - Realm condition dashboard (11 pressure states with green/yellow/red bands + next-cycle predictions)
  - Message log (event feed with canonical events: famine warning, water crisis, succession event, fortification tier advancement, assault cohesion shake, territory stabilized, covenant discovered)
  - Match-event prompt overlays (succession choice, covenant commitment, doctrine selection, ransom decision)
- `BloodlineHudBinding.cs` — subscribes to dynasty state changes and drives the UI
- `RealmConditionHudBinding.cs` — reads the realm condition snapshot and paints bands

### Step 6 — Strategic Zoom + Theatre View

- Expand map rendering so that at strategic zoom the camera sees the full map with aggregated icon representations (province ownership colors, army icons rather than individual units, fortification tier heatmap overlay)
- Fog-of-war: battlefield = unit-sight-radius; strategic = faction-wide revealed-territory + scouting intel
- Transition animation that feels continuous (one camera rig, varying height + FOV + fog profile)

### Step 7 — Continue Browser Reference Wave In Parallel

Between Unity passes, or when Unity is waiting on something, continue the browser reference wave from the plan at `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md`:

- Extend `src/game/core/renderer.js` for new node types + fortification buildings + ram + settlement-class labels
- Extend `src/game/main.js` for new build buttons + stone/iron resource pills + realm condition HUD bar
- Extend `play.html` with the HUD bar markup
- Expand `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` with assertions covering the new canon
- Extend `src/game/core/ai.js` so the enemy AI refuses to attack fortified keeps without siege units

The browser reference stays ahead of Unity as the working spec. Do not let Unity fall so far behind that the reference drifts out of sync.

### Step 8 — Extend Canon Layers As You Go

As each system is implemented, additively extend the canonical design corpus:

- Reserve cycling for garrisons (canon: canonical but not yet live)
- Governor specialization (city / border / keep)
- Captured member rescue / ransom operations
- Faith-integrated fortification bonuses (covenant-specific defensive expressions: Old Light pyre wards, Blood Dominion altar rites, Order edict wards, Wild root wards)
- Commander keep-presence bonuses
- Second and third siege engines (siege tower, trebuchet)
- Second playable house (Stonehelm) with full strategic identity
- Second map

Every canon extension must be additive. Never reduce. Never replace large design files with shorter substitutes.

---

## PRESERVATION RULES (NON-NEGOTIABLE)

- Nothing deleted without explicit authorization from Lance.
- Preservation mode is the default.
- If two files differ, preserve both.
- If multiple versions compete, keep originals and document the relationship.
- The `<repo>/Bloodlines/` stub folder stays in place unless Lance explicitly authorizes deletion.
- The browser reference simulation at `<repo>/play.html` + `<repo>/src/game/` + `<repo>/data/` is preserved in parallel with Unity.
- All JSON schema changes are additive.
- All C# ScriptableObject changes are additive.
- All canonical documents are additive — do not compress, summarize away, or condense.
- Tests stay green after every wave.

---

## DO NOT REGRESS

- Do not reintroduce `deploy/bloodlines` as an active root.
- Do not create new parallel Bloodlines roots outside the canonical root.
- Do not delete preserved source bundles in `archive_preserved_sources/`.
- Do not replace large design files with short summaries.
- Do not ignore the continuity layer (`CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`).
- Do not drift away from the 2026-04-14 master doctrine.
- Do not reduce canonical scope. Bloodlines is being built at full intended scale.
- Do not abandon the browser reference simulation — it is the working spec.
- Do not open `<repo>/Bloodlines/` as the primary Unity project — open `<repo>/unity/`.
- Do not introduce an engine other than Unity 6.3 LTS without a clearly demonstrated technical blocker and explicit authorization from Lance.

---

## VERIFICATION COMMANDS

### Browser reference simulation

```powershell
Set-Location D:/ProjectsHome/Bloodlines
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Open:

- `http://localhost:8057/`
- `http://localhost:8057/play.html`

Validation:

```powershell
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
```

### Unity canonical project

```powershell
# Open in approved version (once Unity version decision is locked):
& "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe" -projectPath "D:/ProjectsHome/Bloodlines/unity"

# After first open, from Unity menu: Bloodlines → Import → Sync JSON Content
```

---

## COMPLETION PROTOCOL (At End Of Meaningful Work Block)

Before ending the session:

1. Update `CURRENT_PROJECT_STATE.md` with what is now live.
2. Update `NEXT_SESSION_HANDOFF.md` with the next recommended action.
3. Update `continuity/PROJECT_STATE.json` with machine-readable state changes.
4. Add a dated entry to `00_ADMIN/CHANGE_LOG.md`.
5. Update `tasks/todo.md` — mark completed items, add new next-wave items.
6. If new outside Bloodlines material was found during the session, import it into `archive_preserved_sources/` and update `SOURCE_PROVENANCE_MAP.md`.
7. Write a new continuation prompt at `03_PROMPTS/CONTINUATION_PROMPT_<DATE>_SESSION_N.md` so future sessions inherit the current state.
8. Verify tests are green.

---

## FINAL DIRECTIVE

Continue building Bloodlines toward its full, forceful, expansive, memorable, mechanically rich game experience. Do not bottleneck. Do not reduce. Do not compress. Do not summarize away the vision.

First, resolve the Unity version alignment decision with Lance. Then execute the ECS foundation step by step while continuing the browser reference wave in parallel. Surface the bloodline presence in the HUD as soon as the ECS foundation supports it. Build the battlefield camera to feel like Generals / Zero Hour. Build the strategic zoom transition to feel like the same war viewed at a larger scale.

Move the project materially closer to the monumental game it is intended to become.

Read the files listed above. Then begin.

---
