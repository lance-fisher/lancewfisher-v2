# BLOODLINES — CONTINUATION PROMPT FOR NEXT SESSION

Continue active development on the RTS project BLOODLINES from the canonical root:

- `D:\ProjectsHome\Bloodlines`

This is not a scope-reduction session. Build toward the full intended game at full scale, with house identity, dynastic consequence, faith divergence, territorial rule, fortification depth, siege commitment, large armies, and strategic zoom continuity preserved as non-negotiable direction.

## Read First

Read these files before making meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
6. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
9. `D:\ProjectsHome\Bloodlines\01_CANON\DEFENSIVE_FORTIFICATION_DOCTRINE.md`
10. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\FORTIFICATION_SYSTEM.md`
11. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\SIEGE_SYSTEM.md`
12. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
13. `D:\ProjectsHome\Bloodlines\tasks\todo.md`

## Current State At End Of Session 4

### Browser/spec lane

Live and validated:

- dynasty consequence cascade
- commander attachment and aura
- territory capture plus stabilization
- covenant exposure, doctrine commitment, doctrine effects
- conviction ledger with bucketed runtime accrual
- seven-resource battlefield economy plus influence
- stone and iron production with canonical smelting chain
- fortification buildings: `wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`
- settlement classes and fortification tier ceilings
- first siege engine: `ram`
- assault cohesion strain for wave-spam denial
- canonical 90-second realm condition cycle
- HUD/resource/realm-condition legibility layer
- fortified-keep AI refusal without siege support
- fortified-settlement reserve cycling with fallback-to-triage and reserve mustering
- primary-keep reserve snapshot metadata in HUD

Validation currently green:

- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `node --check src/game/main.js`
- `node --check src/game/core/simulation.js`
- `node --check src/game/core/renderer.js`
- `node --check src/game/core/ai.js`
- `node --check src/game/core/data-loader.js`
- `node --check src/game/core/utils.js`

### Unity lane

- Canonical Unity project remains `D:\ProjectsHome\Bloodlines\unity`
- `_Bloodlines/` folder structure, JSON importer, and ScriptableObject definitions are in place
- ECS runtime code has not started yet
- Do not open `D:\ProjectsHome\Bloodlines\Bloodlines` as the primary project; it remains a preserved stub

## Blocker

Before writing ECS code, resolve the Unity editor-version question with Lance.

Installed editors:

- `6000.3.13f1` (Unity 6.3 LTS, approved)
- `6000.4.2f1` (Unity 6.4)

Canonical `unity/` project currently targets 6.4. Approved architecture says 6.3 LTS. Ask if not already answered. Recommended path remains: downgrade to `6000.3.13f1`, then re-resolve DOTS packages for LTS compliance.

## Next Correct Work

### If Unity version remains unresolved

Continue the browser/spec lane upward:

1. governor specialization (`city`, `border`, `keep`)
2. captured-member rescue / ransom operations
3. faith-integrated fortification bonuses
4. deeper commander keep-presence bonuses beyond current reserve commitment
5. `siege_tower` and `trebuchet`
6. AI siege preparation beyond simple refusal
7. full 11-state realm-condition dashboard

### If Unity version is resolved

Immediately do:

1. open `D:\ProjectsHome\Bloodlines\unity` in the locked editor
2. run `Bloodlines -> Import -> Sync JSON Content`
3. verify `.asset` generation under `Assets/_Bloodlines/Data/*`
4. begin the first ECS foundation wave:
   - components
   - systems
   - authoring + baking
   - bootstrap scene
   - Ironmark Frontier gameplay scene
   - battlefield camera and input

## Preserve

- delete nothing without explicit authorization
- keep both browser and Unity lanes in parallel
- keep all continuity files current
- keep design additions additive
- do not reduce or reinterpret Bloodlines into a smaller game

## End-Of-Session Protocol

Before ending the next meaningful work block:

1. update `CURRENT_PROJECT_STATE.md`
2. update `NEXT_SESSION_HANDOFF.md`
3. update `continuity/PROJECT_STATE.json`
4. update `00_ADMIN/CHANGE_LOG.md`
5. update `tasks/todo.md`
6. write the next continuation prompt in `03_PROMPTS/`
7. keep tests green
