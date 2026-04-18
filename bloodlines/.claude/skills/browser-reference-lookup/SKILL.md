---
name: browser-reference-lookup
description: Invoke when starting a new Unity slice and the browser behavioral spec source needs to be identified, when asked what the browser does for a specific system, or when a slice handoff needs a Browser Reference citation. Searches src/game/core/simulation.js (14,868 LOC) and src/game/core/ai.js (3,141 LOC) for the named function or system cluster. Returns a file:line citation ready to paste into a slice handoff. Trigger phrases: "what does the browser do for X", "find browser reference for X", "cite browser source for X", "browser spec for X", "simulation.js function for X".
---

# Browser Reference Lookup Skill (Bloodlines)

The browser runtime (`src/game/core/simulation.js` and `src/game/core/ai.js`) is the frozen behavioral specification for the Bloodlines simulation. Every Unity ECS slice must cite its browser reference in the slice handoff. This skill finds that citation.

The browser is NOT legacy code to be replaced. It is an executable specification. "What the browser does" is "what Unity must also do."

## How to search

### Step 1: Identify the search term

Use the system name as the search term. Examples:
- For conviction scoring: search `conviction`, `convictionScore`, `updateConviction`
- For dynasty succession: search `succession`, `dynastySuccession`, `performSuccession`
- For fortification tiers: search `fortification`, `fortificationTier`, `advanceFortification`
- For save/load: search `exportStateSnapshot`, `restoreStateSnapshot`, `snapshot`
- For faith: search `faith`, `faithCommitment`, `applyFaithEffect`
- For AI expansion: search `aiExpansion`, `expandAI`, `evaluateExpansion`

### Step 2: Search simulation.js

```bash
grep -n "<search_term>" src/game/core/simulation.js | head -30
```

Record the line number and function name of the most relevant match.

### Step 3: Read the function cluster (5-10 lines of context)

```bash
sed -n '<start_line>,<start_line+50>p' src/game/core/simulation.js
```

Read enough to understand what the function does and what fields it reads/writes. This informs what the ECS implementation must replicate.

### Step 4: Search ai.js if simulation.js has no match

```bash
grep -n "<search_term>" src/game/core/ai.js | head -20
```

AI decision-making, pathfinding heuristics, and tactical evaluation logic lives in `ai.js`. Simulation state management lives in `simulation.js`. If the system is about how the AI decides what to do (rather than what happens when it does it), check `ai.js`.

### Step 5: Form the citation

The citation format for a slice handoff's Browser Reference section:

```
Browser Reference: src/game/core/simulation.js:<line> <functionName>
```

or for ai.js:

```
Browser Reference: src/game/core/ai.js:<line> <functionName>
```

If multiple functions are relevant (e.g. a system has both a scoring function and an application function), list both:

```
Browser Reference:
  src/game/core/simulation.js:<line1> <functionName1>  (scoring)
  src/game/core/simulation.js:<line2> <functionName2>  (application)
```

## Common function clusters by system (quick lookup)

These are approximate anchors. Line numbers shift as the file is edited. Always verify with grep.

| System | Search term | File |
|---|---|---|
| Conviction scoring | `convictionScore` or `updateConviction` | simulation.js |
| Faith commitment | `faithCommitment` or `applyFaith` | simulation.js |
| Dynasty succession | `performSuccession` or `dynastySuccession` | simulation.js |
| Dynasty fallen ledger | `fallenLedger` or `recordFallen` | simulation.js |
| Food/water economy | `updateFood` or `updateWater` or `tickEconomy` | simulation.js |
| Population growth | `populationGrowth` or `tickPopulation` | simulation.js |
| Territory control | `controlPoint` or `captureControl` | simulation.js |
| Fortification tiers | `fortificationTier` or `advanceFortification` | simulation.js |
| Siege mechanics | `siegePhase` or `applySiege` or `siegeAttrition` | simulation.js |
| State snapshot capture | `exportStateSnapshot` | simulation.js |
| State snapshot restore | `restoreStateSnapshot` | simulation.js |
| AI expansion | `evaluateExpansion` or `aiExpansion` | ai.js |
| AI military orders | `issueMilitaryOrder` or `planAttack` | ai.js |
| AI faith pressure | `aiFaith` or `faithPressure` | ai.js |
| AI economic base | `aiEconomy` or `buildEconomy` | ai.js |

## What to do when the browser has NO match

If `grep` finds no match in either file for the system being implemented:

1. Try synonyms (e.g. "imminent" vs "threat", "muster" vs "rally").
2. Try reading the neighboring functions to the closest match — the browser sometimes inlines logic rather than extracting named functions.
3. If still no match after synonym search: the system may not be in the browser spec. This is a CANON GAP. Do not implement from instinct. Surface the gap to the operator using the canon-enforcement skill's `CANON GAP` format.

## Report format

Return to the calling session:

```
Browser Reference: src/game/core/simulation.js:<line> <functionName>
Summary: <one sentence: what the function does and what state it reads/writes>
ECS translation note: <one sentence: the key thing to preserve when porting to DOTS, e.g. "FactionId string key must be preserved across entity recreations">
```

If no match found:

```
Browser Reference: NOT FOUND
Files searched: simulation.js, ai.js
Terms tried: [list]
Recommended action: Invoke canon-enforcement with CANON GAP status for this system.
```

## What this skill does NOT do

- Does not port the browser logic to ECS (that's the implementation slice).
- Does not check canon compliance beyond citation (canon-enforcement does that).
- Does not modify simulation.js or ai.js — these files are frozen.
- Does not read any browser file other than simulation.js and ai.js.
