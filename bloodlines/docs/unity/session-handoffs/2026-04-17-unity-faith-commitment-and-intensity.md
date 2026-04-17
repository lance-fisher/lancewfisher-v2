# 2026-04-17 Unity Faith Commitment And Intensity (Tier 1 Migration Slice)

## Goal

Third Tier 1 slice from the 2026-04-17 browser-to-Unity migration plan. Ports
the canonical faith commitment and five-tier intensity system from the browser
runtime into ECS. `FaithStateComponent` already existed but was inert — no
exposure accumulation, no commitment path, no tier resolution. This slice
wires the full behavioral seam: exposure → threshold → commitment → tier.

## Browser Reference

- `src/game/core/simulation.js:6` — `FAITH_EXPOSURE_THRESHOLD` constant (100).
- `src/game/core/simulation.js:179` — `FAITH_INTENSITY_TIERS` table (Apex 80,
  Fervent 60, Devout 40, Active 20, Latent 1, Unawakened 0).
- `src/game/core/simulation.js:1903` — `getFaithTier(intensity)` resolver.
- `src/game/core/simulation.js:1907` — `syncFaithIntensityState(faction)`.
- `src/game/core/simulation.js:8174` — `updateFaithExposure(state, dt)`.
- `src/game/core/simulation.js:9694` — `chooseFaithCommitment(state, factionId, faithId, doctrinePath)`.

## Canon Reference

- `04_SYSTEMS/FAITH_SYSTEM.md` — four covenants, two doctrine paths, five
  intensity tiers; faith commitment is a one-way gate that governs every
  downstream faith behavior.

## Work Completed

### Faith scoring (new folder)
- `unity/Assets/_Bloodlines/Code/Faith/FaithIntensityTiers.cs`: canonical
  five-tier threshold constants plus the commitment threshold and starting
  commitment intensity. `ResolveLevel(intensity)` returns the tier index.
- `unity/Assets/_Bloodlines/Code/Faith/FaithScoring.cs`: pure, deterministic
  helpers reusable by systems, debug surfaces, and future AI decision code.
  - `SyncLevel(ref FaithStateComponent)`: clamps Intensity into
    `[0, IntensityMax]` and refreshes Level from the tier table.
  - `RecordExposure(buffer, CovenantId, amount)`: adds exposure to the
    matching buffer element (or inserts a new one), clamping the result.
  - `GetExposure(buffer, CovenantId)`: returns the current exposure.
  - `Commit(ref FaithStateComponent, buffer, target, path)`: returns
    `CommitmentResult` enum (Committed, AlreadyCommitted, ExposureBelowThreshold,
    InvalidFaith). On Committed, sets SelectedFaith, DoctrinePath,
    Intensity=20, Level=Active.

### ECS system (new)
- `unity/Assets/_Bloodlines/Code/Faith/FaithIntensityResolveSystem.cs`:
  `[BurstCompile]` ISystem in SimulationSystemGroup. Every tick, iterates
  entities with `FaithStateComponent` and calls `FaithScoring.SyncLevel`.
  Idempotent when state is unchanged; guarantees downstream readers see
  Level matching Intensity without a per-mutation refresh call.

### Debug command surface (new partial)
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs`:
  - `TryDebugGetFaithState(factionId, out FaithStateComponent)`
  - `TryDebugGetFaithExposure(factionId, CovenantId, out float)`
  - `TryDebugRecordFaithExposure(factionId, CovenantId, amount)`
  - `TryDebugCommitFaith(factionId, CovenantId, DoctrinePath)` — returns
    `FaithScoring.CommitmentResult`.

### Governed smoke validator (new editor)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithSmokeValidation.cs`:
  editor-only `[MenuItem]` + `RunBatchFaithSmokeValidation` entry. Four
  phases in isolated ECS worlds:
  1. **Baseline** — new faction reports Unawakened (level 0), selected None,
     intensity 0.
  2. **Exposure threshold** — 60 exposure (< 100) blocks commitment with
     `ExposureBelowThreshold`.
  3. **Commitment** — 100 exposure unlocks commitment; on Commit the
     faction carries BloodDominion+Dark, Intensity=20, Level=Active (2).
     A second commit call returns `AlreadyCommitted`.
  4. **Intensity tier** — setting intensity to 80 resolves to Apex (5);
     setting to 60 resolves to Fervent (4); setting to 150 clamps to 100
     and holds Apex.

### Wrapper (new)
- `scripts/Invoke-BloodlinesUnityFaithSmokeValidation.ps1`:
  batch-mode wrapper calling
  `Bloodlines.EditorTools.BloodlinesFaithSmokeValidation.RunBatchFaithSmokeValidation`.

## Scope Discipline

- No sacred-site exposure walker. The browser reads world.sacredSites and
  walks units per tick; Unity has no sacred-site entities yet. The
  `RecordExposure` helper is the seam the future exposure-generation
  system will call once sacred sites exist.
- No structure intensity regen (`updateFaithStructureIntensity`). That
  requires building-role classification (shrine/hall/sanctuary) which is
  not modeled yet.
- No wayshrine amplification. Same dependency chain.
- No covenant tests, no holy wars, no divine right. Tier 2 items.
- No conviction event on commit. The browser posts a conviction event on
  commitment; I deferred that wiring to avoid pulling Conviction into this
  slice's diff surface. It is a one-line addition once the operator approves
  the cross-system bridge.

## Verification

- `Invoke-BloodlinesUnityFaithSmokeValidation.ps1` — passed:
  `Faith smoke validation passed: baselinePhase=True, exposureThresholdPhase=True, commitmentPhase=True, intensityTierPhase=True. Baseline: selected=None, intensity=0, level=Unawakened. Threshold: exposureAtBlock=60, requiredThreshold=100, result=ExposureBelowThreshold. Commit: selected=BloodDominion, path=Dark, intensity=20, level=2, recommitBlock=AlreadyCommitted. IntensityTier: apex@80=level5, fervent@60=level4, clamped@150=100.`
- Additional gates (bootstrap, combat, graphics, scene shells, conviction,
  dynasty, data-validation, runtime-bridge, staleness) to run at the merge
  boundary.

## Next Action (Within Migration Plan)

After this slice merges:

1. Wire faith commitment into SkirmishBootstrap so live skirmish carries a
   live `FaithStateComponent` and exposure buffer on every kingdom faction.
2. Add the conviction event post-commit (single call to
   `ConvictionScoring.ApplyEvent` from inside `FaithScoring.Commit` or
   from a dedicated bridge system).
3. Begin the state snapshot / restore slice (Tier 1 item 5). Faith +
   Dynasty + Conviction all need to round-trip.

## Branch

- Branch: `claude/unity-faith-commitment`
- Base: master tip after Dynasty slice merge.
