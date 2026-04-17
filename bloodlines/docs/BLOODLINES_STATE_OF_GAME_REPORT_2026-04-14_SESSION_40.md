# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 40
Author: Claude

## Scope

Direct battlefield-hero renown award hook LIVE. Closes the "manual renown edit" gap from Sessions 35-38: real combat events now grant renown to the active commander (or fallback recipient), driving the lesser-house promotion pipeline naturally over a real match without test-side or UI-side fiat. Worker kills are canonically excluded — peasant deaths are not glory. Building kills only count for fortification-roled buildings, matching the canonical fortification doctrine. Vector 2 advanced eighth consecutive session.

## Changes Landed

### Renown award constants and helper (`src/game/core/simulation.js`)

- New exported constants:
  - `RENOWN_CAP = 100` — hard ceiling, leaves headroom for design balance.
  - `RENOWN_AWARD_COMBAT_KILL = 1` — per-kill grant.
  - `RENOWN_AWARD_FORTIFICATION_KILL = 2` — heavier weight for breaking walls/towers/gates/keeps.
- New internal helper `findRenownAwardRecipient(faction)` — preference order:
  1. Active commander (roleId === "commander")
  2. Head of bloodline (roleId === "head_of_bloodline")
  3. Any military_command path member
  4. Any available member as last resort
- New internal helper `awardRenownToFaction(state, factionId, amount, reason)`:
  - Looks up recipient.
  - Caps at `RENOWN_CAP`.
  - Appends ledger entry with `amount`, `reason`, and dual-clock `atInWorldDays`.
  - Truncates ledger to last 12 entries.
  - Returns recipient or null if no change.

### `applyDamage` hook

- Added at confirmed-kill point (after `entity.health = 0` and the existing assault-failure cohesion strain block).
- Activates only when `attackerFactionId` exists and is different from the killed entity's faction (no friendly fire, no environmental).
- For unit kills: looks up `unitDef.role`. If non-worker, calls `awardRenownToFaction(state, attackerFactionId, RENOWN_AWARD_COMBAT_KILL, "kill_${typeId}")`.
- For building kills: looks up `buildingDef.fortificationRole`. If truthy, calls `awardRenownToFaction(state, attackerFactionId, RENOWN_AWARD_FORTIFICATION_KILL, "fortkill_${typeId}")`.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 40 combat-kill block):
  - Stages a player combat unit attacking an enemy combat unit with health=1.
  - Ticks simulation up to 30 times until kill resolves.
  - Verifies recipient renown grew above baseline.
  - Verifies recipient `renownLedger` has at least one entry.
- `tests/runtime-bridge.mjs` (Session 40 worker-kill exclusion block):
  - Stages player combat unit killing enemy worker (health=1).
  - Verifies kill resolves but recipient renown is UNCHANGED.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (combat-kill grant + worker-kill exclusion).
- Syntax clean.

## Canonical Interdependency Check

Battlefield-hero renown award now connects to:

1. **Combat system**: hooked into the existing `applyDamage` death-confirmation path.
2. **Lesser-house pipeline** (S35): renown is the gate; matches that organically push commander past `LESSER_HOUSE_RENOWN_THRESHOLD` (30) auto-trigger candidate detection.
3. **AI lesser-house promotion** (S38): same — AI naturally promotes its own commanders after sustained combat success.
4. **Conviction system** (indirectly): commander capture/kill resolution already adjusts conviction; renown is a parallel signal at the dynasty register.
5. **Save/resume**: renown and renownLedger are part of dynasty member records, deep-cloned automatically.
6. **Fortification doctrine** (canonical 2026-04-14): fortification-roled buildings carry doubled renown weight.

Satisfies Canonical Interdependency Mandate (≥2 live systems touched — exceeds it dramatically).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Direct battlefield-hero renown award hook | DOCUMENTED | LIVE (combat-kill +1, fortification-kill +2, worker-kill excluded, RENOWN_CAP=100, ledger trace) |
| Lesser-house pipeline organic activation | PARTIAL | LIVE (commanders accumulate renown through real play; lesser-house promotion no longer requires manual member edits) |

## Combined Vector 2 Status (Eight Sessions)

| Session | Vector 2 Advance |
|---|---|
| S33 | Marriage system first canonical layer |
| S34 | Marriage UI panel |
| S35 | Lesser-house promotion pipeline (player) |
| S36 | AI marriage proposal reciprocity |
| S37 | AI marriage inbox processing |
| S38 | AI lesser-house promotion logic |
| S39 | Marriage proposal expiration + decline path |
| S40 | Direct battlefield-hero renown award hook |

Vector 2 has gone from "stagnant since Session 14" (per S33 report) to a fully operational dynasty formation system with: bidirectional marriage (propose, accept, decline, expire on both sides), bidirectional cadet-branch creation (player + AI), and battlefield-driven hero accumulation that organically activates the cadet pipeline through real combat. The dynastic register breathes.

## Session 41 Next Action

- Lesser-house loyalty drift mechanic (foundation for future defection mechanics; cadet-branch loyalty currently fixed at 75 forever).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for Session 27 sortie).
- Or: Mixed-bloodline defection slider runtime (children with cross-house metadata could shift faction loyalty).
- Or: Faith-compatibility weighting in AI marriage decisions.
- Or: Lesser-house unit-levy mechanic (cadet branches contribute small retinue units).
- Or: Pivot to non-Vector-2 work (Vector 1 Civilizational, Vector 4 Faith, Vector 5 World, or Vector 6 Legibility) to balance progress.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 moved PARTIAL → LIVE. Vector 2 advanced eighth consecutive session — extends the longest single-vector advance streak in project history. Tests green.
