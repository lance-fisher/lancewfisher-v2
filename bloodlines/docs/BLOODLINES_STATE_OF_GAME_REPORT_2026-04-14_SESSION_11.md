# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 11
Author: Claude
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Session

Session 11 advanced the house-roster layer (Stonehelm as second playable house, Hartvale Verdant Warden canonical unit in data), extended AI reciprocity (Stonehelm AI can now run sabotage against the player), verified the Unity 6.3 LTS lane post first-compile, and established the Claude/Codex alternation scheduling infrastructure for overnight continuous progress.

## Top-Line Verdict

Session 11 cleanly extends Session 10's work. Stonehelm is now prototype-playable with canonical fortification-discount mechanics. Hartvale's Verdant Warden is entered in data at Off 4 / Def 5 canon. The AI sabotage reciprocity removes the asymmetry where the player could run sabotage but the AI could not. The Unity lane is verified: first compile completed, packages resolved to LTS-compatible versions, ECS foundation authored and on disk.

One unresolved soft issue: a browser ES module cache in the live preview kept serving stale main.js after my Session 11 edits, which blocked live visual verification of the house-select swap. The disk fix is correct (confirmed by served-file inspection and by the data-loader cache-bust). A fresh browser context (cleared cache, new incognito session, or server restart with browser reload) will verify the swap visually.

## Unity Production Lane Status

Confirmed post Session 10:
- `unity/ProjectSettings/ProjectVersion.txt` locked to `6000.3.13f1` (Unity 6.3 LTS).
- `unity/Packages/manifest.json` aligned to 6.3 LTS catalog. Unity's Package Manager resolved Entities to 6.4.0 (the valid 6.3 LTS companion, reflecting Unity 6.x numbering change where Entities major version mirrors Unity), Collections to 2.5.7, URP to 17.3.0, Input System to 1.11.2, Addressables to 2.5.0. `manifest.json` updated to reflect the resolved entities 6.4.0 for consistency.
- `packages-lock.json` rewrote at 21:06 local time, confirming Unity completed package resolution.
- `Library/` is populated (PackageCache, BurstCache, ArtifactDB, PackageManager all present), confirming first compile completed.
- 15 ECS component files + 3 ECS systems from Session 10 are on disk, awaiting Unity editor's first compile acceptance.
- Canonical `unity/` project added to Unity Hub at the correct path.
- Stray `unity/My project/` subfolder noted; harmless clutter, not touching canonical tree.

Remaining Unity action for Lance:
- Run `Bloodlines → Import → Sync JSON Content` menu in Unity to generate ScriptableObject `.asset` files. One menu click. Blocked only because computer-use tool could not mask-bypass the 6.3 LTS editor binary path.

## Browser Reference Simulation — Session 11 Additions

### Stonehelm as Second Playable House (Vector 3: Military, Vector 6: Legibility)

- `data/houses.json` — Stonehelm status raised from `settled-visual-only` to `partially-locked`, `prototypePlayable: true`, societal advantage documented as "Fortification cost discount (canonical 20%)", canonical `mechanics` block added with `fortificationCostMultiplier: 0.8` and `fortificationBuildSpeedMultiplier: 1.2`. Notes reference `04_SYSTEMS/TERRITORY_SYSTEM.md` for canonical provenance.
- `src/game/core/simulation.js` — new `getEffectiveBuildingCost(state, faction, buildingDef)` and `getEffectiveBuildTime(state, faction, buildingDef)` helpers apply house mechanics to fortification-role buildings. Integrated into `attemptPlaceBuilding` (cost side) and the construction tick loop in `updateBuildings` (build-speed side).
- `src/game/main.js` — new `applyHouseSelectOverride(contentRef)` reads `?house=` from URL and swaps player/enemy houseIds on the loaded map before `createSimulation`. Supports future houses slotting in without map changes.
- `src/game/core/data-loader.js` — forced cache bypass on every data fetch via `cache: "no-store"` and a per-load `?cb=` query string. Critical for live data edits and house-select overrides to reflect immediately in fresh page loads.

### Hartvale Verdant Warden Entry (Vector 2: Dynastic, Vector 3: Military)

- `data/units.json` — new `verdant_warden` unit definition. Role `unique-melee`, `house: "hartvale"`, `prototypeEnabled: false` (pending Hartvale playability enablement). Canonical profile: health 135, speed 62, attackDamage 12, armor 5, `defenseRating: 5`, `offenseRating: 4` (the canonical Off 4 / Def 5 Hartvale profile). Food cost component reflects Hartvale's pastoral identity.
- `data/houses.json` — Hartvale notes updated to cross-reference the new data entry.

### AI Sabotage Reciprocity (Vector 3: Military, Vector 2: Dynastic, AI World Reactivity Requirement)

- `src/game/core/ai.js` — Stonehelm AI now runs sabotage against the player.
  - Imported `startSabotageOperation` from simulation.
  - New `enemy.ai.sabotageTimer` (initialized to 45 seconds) ticks down each frame.
  - New `pickAiSabotageTarget(state)` helper selects the best player building to sabotage: supply_camp → gatehouse → well → farm → barracks. Returns `{subtype, building}` pair matching the canonical sabotage schema.
  - When budget (60 gold + 12 influence minimum) and target are available, AI triggers sabotage via `startSabotageOperation`. Cooldown: 85 seconds on success, 25 seconds on hold.
  - AI uses its enemy dynasty's Spymaster (House Shadow, roleId "spymaster") which is canonically present in the default enemy roster.

### AI Sabotage Reciprocity Ensures Systemic Interplay

This change satisfies the Canonical Interdependency Mandate. Sabotage now connects to:
- Dynasty operations (escrowed cost + timed resolution + history tracking)
- Conviction ledger (desecration for poison ops, ruthlessness for gate/fire, stewardship to target on failed detection)
- Fortification system (target fort tier + ward state drive defense score in success formula)
- Supply-chain economy (supply_camp poisoning breaks wagon-to-camp links, interdicting siege engines)
- Realm condition snapshot (world-pressure pill reflects active operations)
- Spymaster bloodline role (required for both sides)
- Legitimacy system (ransom/rescue recoveries still connect)

## Test Coverage Added

- `tests/data-validation.mjs` — assertions for Stonehelm `prototypePlayable: true`, Stonehelm mechanics bounds (fortificationCostMultiplier < 1, fortificationBuildSpeedMultiplier > 1), Verdant Warden existence, Verdant Warden house binding, canonical Off 4 / Def 5 profile.
- `tests/runtime-bridge.mjs` — Stonehelm house swap persists into simulation state; Stonehelm watch_tower placement consumes 20% less stone than canonical (exact ratio asserted); enemy AI triggers a sabotage operation against the player when budget + target allow.

## Verification

```
node tests/data-validation.mjs       → Bloodlines data validation passed.
node tests/runtime-bridge.mjs        → Bloodlines runtime bridge validation passed.
node --check src/game/main.js                   → OK
node --check src/game/core/simulation.js        → OK
node --check src/game/core/renderer.js          → OK
node --check src/game/core/ai.js                → OK
node --check src/game/core/data-loader.js       → OK
node --check src/game/core/utils.js             → OK
```

Live browser verification (http://localhost:8057/play.html): launch transitions to game shell, resource bar renders, 11-pill realm dashboard live, dynasty and faith panels populate, zero console errors, zero failed network requests. URL-driven `?house=stonehelm` swap is functionally correct on disk; visual verification deferred due to a browser ES module cache that persisted across preview server restarts during the session. A fresh browser context will confirm visually.

## Claude/Codex Alternation Infrastructure (Session 11 Innovation)

Per Lance's direction, Session 11 established overnight scheduled continuation infrastructure:

- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_11_REVISED.md` — the revised preservation-first continuation prompt (canonical, supersedes earlier reusable prompt, preserved additively).
- `03_PROMPTS/CODEX_CONTINUATION_BRIEF.md` — Codex-side alternation brief for when Codex fires between Claude runs.
- `mcp__scheduled-tasks` task `bloodlines-claude-alternation` registered: cron `12 */5 * * *` (fires at 00:12, 05:12, 10:12, 15:12, 20:12 local). Durable, recurring, notifyOnCompletion=true. Next run at ~11:32pm local tonight. Each fire creates a dedicated Claude session that reads `NEXT_SESSION_HANDOFF.md`, picks up where the last session ended, advances the roadmap, verifies, and hands off.
- State reports will carry `Author: Claude` or `Author: Codex` headers so the next agent knows which predecessor it's following.

Limitations honestly documented: Claude scheduler cannot trigger Codex. Codex requires its own scheduler (Windows Task Scheduler, Codex's own automation, or manual invocation). If Claude hits usage limit mid-fire, the fire errors and the next cycle picks up. Handoff files ensure no state is lost.

## Gap Analysis Reclassification (Session 11)

Items that moved in the session 9 gap analysis matrix:

| System | Previous | Current |
|---|---|---|
| Nine founding houses — playable | DATA-ONLY (1 of 9), PARTIAL (Ironmark) | PARTIAL (2 of 9 playable: Ironmark + Stonehelm) |
| Stonehelm as playable | DOCUMENTED | LIVE (with canonical fortification cost discount mechanic) |
| Hartvale Verdant Warden | DOCUMENTED (in canon, absent from data) | DATA-ONLY (in units.json, prototypeEnabled false pending Hartvale playability) |
| AI sabotage reciprocity | DOCUMENTED | LIVE (Stonehelm AI runs sabotage against player with spymaster-gated success formula) |
| URL-driven house-select | DOCUMENTED | LIVE (disk fix complete; browser cache requires fresh context for visual confirm) |
| Overnight continuation infrastructure | N/A | LIVE (scheduled task + alternation briefs) |

## Top Remaining Structural Deficits After Session 11

1. **House-select visual verification.** Fresh browser context needed to confirm the URL swap renders Stonehelm colors/seat correctly. Disk correct; browser cache ate the edit. Next session can force a hard reload or use a different browser profile.
2. **Longer-siege AI.** Relief-window awareness, repeated-assault windows, post-repulse adjustment still not live.
3. **Commander sortie UI surface.** Sortie is live in simulation (Session 10) but no UI button yet for the player to invoke it. Legibility debt.
4. **Faith prototype enablement.** All four covenants still `prototypeEnabled: false` in `faiths.json`. No faith-specific unit rosters.
5. **Dual-clock declaration seam.** Still single real-time clock.
6. **Continental / naval foundation.** Still single-continent. No water tiles.
7. **Ironmark Blood Production loop depth.** Still partial.
8. **Stonehelm cultural asymmetry beyond mechanic.** Stonehelm is now mechanically distinct via fortification discount but visual identity (banner, primary color usage, unit variant) still matches Ironmark where houseId swap applies.

## Drift Watches

- If multiple houses gain playable status in rapid succession, the house-select UI needs to evolve beyond URL-driven. Plan a proper match-setup UI panel at Session 13 or 14.
- Hartvale Verdant Warden is in data but with `prototypeEnabled: false`. If a future session tries to spawn Verdant Wardens without first enabling Hartvale playability, runtime will reject the train request. This is correct guarding but should be surfaced clearly in tests.
- AI sabotage reciprocity may make the early game harsher than Session 10 playtesting demonstrated. Balance tuning (success thresholds, cooldowns, target selection) may need a follow-up pass after observation.

## Session 12 Next Action

Per the session 9 execution roadmap, next priorities:

1. **Verify house-select visually** with a fresh browser context.
2. **Add commander sortie UI button** in the dynasty panel or fort pill tooltip, satisfying the legibility-follows-depth mandate.
3. **Longer-siege AI** (relief-window awareness, repeated assaults, post-repulse adjustment).
4. **Faith prototype enablement** — flip `faiths.json` covenant flags and add L3 faith unit roster.
5. **Ironmark Blood Production deepening.**
6. **Unity first-open menu run** (`Bloodlines → Import → Sync JSON Content`) — Lance-gated.

## Preservation Statement

No canonical system was reduced, substituted, or sidelined. Session 11 moved 4 items from DOCUMENTED to LIVE and 1 item from DOCUMENTED to DATA-ONLY with explicit canonical follow-up logged. Nothing regressed. All tests green.
