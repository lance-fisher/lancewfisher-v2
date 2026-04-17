# 2026-04-17 Unity Conviction Scoring And Bands (Tier 1 Migration Slice)

## Goal

First Tier 1 slice from the 2026-04-17 browser-to-Unity migration plan: port the
canonical conviction scoring and band resolution from the browser runtime into
ECS. `ConvictionComponent` already existed on every faction entity but was
inert (Band always Neutral, Score never derived). This slice makes it live.

## Browser Reference

- `src/game/core/simulation.js:4229` — `deriveConvictionScore(buckets)`: pure
  score = (stewardship + oathkeeping) - (ruthlessness + desecration).
- `src/game/core/simulation.js:4233` — `refreshConvictionBand(state, factionId)`:
  recomputes score, resolves band by threshold, writes `faction.conviction.bandId`.
- `src/game/core/simulation.js:4245` — `recordConvictionEvent(state, factionId, bucket, amount, reason)`:
  mutates bucket, appends event ledger entry, re-refreshes band.
- `src/game/core/simulation.js:1849` — `CONVICTION_BAND_EFFECTS`: five-band
  multiplier table for stabilization, reserve heal, loyalty protection, capture,
  population growth, and attack.

## Canon Reference

- `04_SYSTEMS/CONVICTION_SYSTEM.md` — the four-bucket, five-band spectrum.
- `data/conviction-states.json` — the thresholds (Apex Moral >= 75, Moral >= 25,
  Neutral >= -24, Cruel >= -74, Apex Cruel below).

## Work Completed

### Shared scoring helpers (new folder)
- `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoring.cs`:
  pure static `DeriveScore`, `ResolveBand`, `Refresh(ref ConvictionComponent)`,
  and `ApplyEvent(ref ConvictionComponent, ConvictionBucket, float)` helpers.
  Score thresholds mirror `data/conviction-states.json` as named constants so a
  future data-driven override can replace them without touching call sites.
- `unity/Assets/_Bloodlines/Code/Conviction/ConvictionBandEffects.cs`:
  per-band `ConvictionBandEffects` readonly struct with the six canonical
  multipliers. `ForBand(ConvictionBand)` returns the exact values from the
  browser `CONVICTION_BAND_EFFECTS` table, 1:1.

### ECS system (new)
- `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoringSystem.cs`:
  `[BurstCompile]` `ISystem` in `SimulationSystemGroup`. Every tick, iterates
  entities with `ConvictionComponent` and calls `ConvictionScoring.Refresh`.
  Idempotent when buckets are unchanged, so safe to run unconditionally. This
  guarantees downstream readers (future loyalty, capture, population growth
  consumers) see a consistent band without waiting for a bucket mutation.

### Debug command surface (new partial)
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Conviction.cs`:
  - `TryDebugRecordConvictionEvent(factionId, bucket, amount)`: immediately
    applies the bucket delta and refreshes the band (matches the browser's
    tick-0 semantics on `recordConvictionEvent`).
  - `TryDebugGetConvictionState(factionId, out ConvictionComponent)`
  - `TryDebugGetConvictionBandEffects(factionId, out ConvictionBandEffects)`

### Governed smoke validator (new editor)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`:
  editor-only `[MenuItem]` + `RunBatchConvictionSmokeValidation` entry. Four
  phases in isolated ECS worlds:
  1. **Neutral baseline** — empty buckets resolve to Neutral band at score 0.
  2. **Moral ascent** — +50 stewardship → Moral at 50; +40 oathkeeping → Apex
     Moral at 90.
  3. **Cruel descent** — +30 ruthlessness → Cruel at -30; +100 desecration →
     Apex Cruel at -130.
  4. **Band effects** — apex moral stabilization = 1.22, apex cruel capture =
     1.22 and attack = 1.12, neutral all multipliers = 1.
  Writes `artifacts/unity-conviction-smoke.log`. Isolated worlds so it cannot
  interfere with the combat, bootstrap, or graphics smokes.

### Wrapper (new)
- `scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`:
  batch-mode wrapper calling
  `Bloodlines.EditorTools.BloodlinesConvictionSmokeValidation.RunBatchConvictionSmokeValidation`.

## Scope Discipline

- `ConvictionComponent` shape was unchanged. No data-definition churn; no
  migration path needed.
- No gameplay system consumes band effects yet. This slice ends at the
  "band resolves correctly" boundary. Downstream consumers (loyalty
  stabilization multiplier, population growth modifier, capture-speed
  modifier) land in later slices so this one stays reviewable.
- No changes to Bootstrap scene, no changes to combat systems, no changes to
  AI. No effect on any other smoke.

## Verification

- `scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1` — passed:
  `Conviction smoke validation passed: neutralPhase=True, moralAscentPhase=True, cruelDescentPhase=True, bandEffectsPhase=True. Neutral baseline: band=Neutral, score=0. Moral ascent: moralBand=Moral@50, apexBand=ApexMoral@90. Cruel descent: cruelBand=Cruel@-30, apexBand=ApexCruel@-130. Band effects: apexMoral.stabilization=1.22, apexCruel.capture=1.22, neutral.stabilization=1.`
- `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` — passed, no regression.
- Remaining governance gates to run after merge (bootstrap runtime, graphics,
  scene shells, data-validation, runtime-bridge, staleness).

## Next Action (Within Migration Plan)

After this slice merges:

1. Wire `ConvictionBandEffects` into downstream consumers one at a time
   - `LoyaltyProtectionMultiplier` into loyalty decline (small, clean first
     consumer).
   - `PopulationGrowthMultiplier` into `PopulationGrowthSystem`.
   - `CaptureMultiplier` into `ControlPointCaptureSystem`.
2. Begin the Dynasty system slice (Tier 1 item 2): `createDynastyState`,
   member lifecycle, role chain, succession, backfill, fallen ledger. That
   slice unblocks the remaining Tier 1 work (faith commitment, match
   progression, etc.) per the migration plan.

## Branch

- Branch: `claude/unity-conviction-scoring`
- Base: `a27f37a` (master tip after workspace handoff archive commit)
