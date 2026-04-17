# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 12
Author: Claude
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Session

Session 12 closed two canonical debts:

1. **Legibility-follows-depth debt from Session 10.** The commander keep-presence sortie went live in simulation in Session 10, but had no UI surface. Session 12 adds a Dynasty panel action button with live state feedback (ready, active, cooldown remaining, commander absent, no threat).

2. **Longer-siege AI first canonical layer.** Session 11's gap analysis flagged this as a top remaining deficit. Session 12 adds relief-window awareness: when player combat units approach the besieger's stage point, Stonehelm holds its final assault commit until the relief column is neutralized or absorbed.

Both changes honor the Canonical Interdependency Mandate by connecting to multiple already-live systems (sortie uses commander + fort + ward + reserve data; relief-window uses siege lines + AI decision tree + player army position).

## Top-Line Verdict

Session 12 advanced Vector 3 (Military) and Vector 6 (Legibility) simultaneously. No placeholder UI was introduced. No canon was simplified. All systems wire into live simulation, live AI awareness, and live UI feedback.

Scheduled continuity: Session 12 follows Session 11 under the Claude/Codex sequential exhaustion chain. A Codex session runs overnight starting whenever this Claude session closes. The scheduled task `bloodlines-claude-resume-after-codex` fires once at 1:00 AM local (in ~3 hours) for Claude to resume after Codex's first run.

## Browser Reference Simulation — Session 12 Additions

### Commander Sortie UI Button (Vector 6: Legibility)

- `src/game/main.js` — `issueKeepSortie` imported from simulation.
- New action row in `renderDynastyPanel` immediately after the governor line. Reads `getRealmConditionSnapshot().fortification` and renders a canonical action button with one of six labels depending on live state:
  - `Call Sortie` — green-ready when `sortieReady && commanderAtKeep && threatActive`.
  - `Sortie Active` — disabled during the 12-second active window, shows "Defenders surging at {primaryKeepName}".
  - `Sortie On Cooldown` — disabled during 60-second cooldown, shows remaining seconds.
  - `Sortie Requires Commander` — disabled when commander is not at keep.
  - `Sortie (No Threat)` — disabled when no enemy combatants at keep.
  - `Sortie Unavailable` — fallback warning state.
- Clicking the ready state calls `issueKeepSortie(state, "player", primaryKeep.id)` and triggers `renderPanels()` to refresh UI immediately.
- Integration check: the button reads the same snapshot consumer as the fort pressure pill, keeping UI state consistent.

### Longer-Siege AI: Relief-Window Awareness (Vector 3: Military)

- `src/game/core/ai.js` — new helper `isReliefArmyApproaching(state, targetFactionId, siegeStagePoint, reliefRadius = 380)`.
  - Counts target-faction combat units (non-worker) within `reliefRadius` of the siege stage point but not already inside the engaged zone (d > 90 tiles from stage).
  - Returns true at 3+ approaching combat units.
  - Canonical reference: master doctrine section X (defensive fortification must be a major path to power; relief armies must materially affect siege pacing).
- New `else if` branch in the final assault-commit path: when a relief army is approaching, AI pulls its siege force back to the stage point and emits the canonical message "Stonehelm spots a player relief column and holds the breach commit until the field is secured." Uses `enemy.ai.lastReliefDelaySecond` to rate-limit the message like all other siege-stage messages.
- The check only fires when `playerKeepFortified && formalSiegeLinesFormed`, so it doesn't interfere with siege preparation phases.

### Canonical Interdependency Check

Both changes connect to multiple live systems:

- **Sortie UI** reads: commander battlefield presence (Session 10), `CommanderAtKeepTag` equivalent, realm snapshot fort block (Session 9), fort threat detection (Session 5), ward state (Session 5), primary keep settlement metadata (Session 2), action button canon pattern (Session 7 captivity ops).
- **Relief-window AI** reads: siege army composition (Session 6), operational force (Session 8), formal siege line formation (Session 8), player unit positions + roles (Session 1 foundation), siege stage point derived from command halls (Session 6 stage logic).

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

New test coverage:

- `tests/runtime-bridge.mjs` — sortie happy path (commander at keep + threat injected + cooldown cleared + invocation succeeds + sortieActive set + second invocation denied during cooldown).
- `tests/runtime-bridge.mjs` — relief-window AI awareness (fortified player keep + enemy AI reaches attack-commit window + 4 player relief units injected near stage + `updateEnemyAi` tick + assertion that no NEW attack-on-hall commands are issued while relief approaches).

## Gap Analysis Reclassification (Session 12)

| System | Previous | Current |
|---|---|---|
| Commander sortie UI surface | DOCUMENTED (Session 11 deficit list) | LIVE (Dynasty panel action button with 6 live states) |
| Longer-siege AI adaptation | DOCUMENTED | PARTIAL (relief-window awareness LIVE; repeated-assault windows + supply protection patrols + post-repulse adjustment still DOCUMENTED) |
| AI world reactivity to player movement | DOCUMENTED | PARTIAL (siege AI now responds to player relief column; other movement types still pending) |

## Top Remaining Structural Deficits After Session 12

1. **Longer-siege AI subsequent layers.** Relief-window landed. Still missing: repeated-assault window logic (retreat to resupply after repulse), supply-protection patrols, post-repulse tactical adjustment, mid-siege weather/night tactics.
2. **Faith prototype enablement.** `faiths.json` still has all four covenants `prototypeEnabled: false`. No L3-L5 unit rosters.
3. **House-select visual verification.** Disk fix correct; fresh browser context still needed to verify visually.
4. **Continental / naval foundation.** Still single-continent.
5. **Dual-clock declaration seam.** Still single real-time clock.
6. **Ironmark Blood Production loop depth.** Still partial.

## Session 13 Next Action

Priority by leverage:

1. **Faith prototype enablement** — flip `faiths.json` covenant flags, add per-covenant building progression (Wayshrine → Hall → Grand Sanctuary), add L3 faith unit roster (8 units: 2 per covenant per doctrine path). Big vector 4 push.
2. **Longer-siege AI next layer: post-repulse adjustment** — when Stonehelm's assault is repulsed (cohesion strain threshold hit), it should retreat to stage, resupply, and re-attempt.
3. **House-select fresh-browser visual verification** (quick).
4. **Sortie cooldown UI tick-down in real time** — currently snapshot-driven, should live-update visually.

## Alternation Chain Note

- This session (Session 12) is **Claude** authored.
- The next agent firing should be **Codex** (user will manually trigger near Claude's usage-limit approach, or via Codex's own scheduler if set up).
- Claude's scheduled resume fire is at 1:00 AM local tonight (`bloodlines-claude-resume-after-codex` task).
- Future Claude fires will be set up after observing the first Codex→Claude handoff quality.

## Preservation Statement

No canonical system was reduced, substituted, or sidelined. Session 12 moved 1 item from DOCUMENTED to LIVE and 2 items from DOCUMENTED to PARTIAL. All tests green. Runtime verified.
