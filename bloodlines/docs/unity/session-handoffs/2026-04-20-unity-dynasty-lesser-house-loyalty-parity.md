# 2026-04-20 Unity Dynasty Lesser-House Loyalty Parity

## Scope

Prompt-accurate continuation of the post-marriage dynasty parity lane on
`codex/unity-dynasty-lesser-house-loyalty-parity`.

This slice starts from the already-landed Tier 2 dynasty surface plus the
immediately prior marriage-parity correction. The goal was to harden the
existing Unity `LesserHouseLoyaltyDriftSystem` against the canonical browser
references for mixed-bloodline pressure, death-dissolution aftermath, world
pressure, and breakaway minor-house defection timing.

## Browser References

- `src/game/core/simulation.js`
  - `tickLesserHouseLoyaltyDrift` (~6631)
  - `spawnDefectedMinorFaction` (~6851)
  - `tickMinorHouseTerritorialLevies` (~7060) as the next follow-up surface
- `src/game/core/ai.js`
  - read-only confirmation that no AI strategic-layer files were widened for
    this parity slice

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - expands `LesserHouseElement` with the runtime state the browser loop
    expects:
    - mixed-bloodline source house id and pressure
    - marital-anchor target house id, marriage id, child count, pressure, and
      active/fractured status
    - world-pressure status, level, and pressure
    - current daily loyalty delta
    - first-zero day and departure day timestamps for defection timing
  - adds `LesserHouseMaritalAnchorStatus` and
    `LesserHouseWorldPressureStatus`
- `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - now mirrors the browser drift stack instead of applying a flat placeholder
    delta
  - computes the base daily delta from legitimacy, oathkeeping, ruthlessness,
    and fallen-ledger pressure
  - adds mixed-bloodline hostility pressure when the founder's mixed house is
    under strain
  - resolves marriage-anchor recovery or fracture using live
    `MarriageComponent`, `MarriageChildElement`, `FactionHouseComponent`, and
    hostility state
  - folds `WorldPressureComponent` into the lesser-house drift calculation
  - starts a 5-day grace window when loyalty first reaches zero, then defects
    only after the grace window expires
  - applies browser-style fallout on defection:
    - dynasty legitimacy `-6`
    - conviction ruthlessness `+1`
    - hostile minor-house breakaway spawn with reciprocal `HostilityComponent`
      links to the parent faction
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesLesserHouseLoyaltyParitySmokeValidation.cs`
  - dedicated 3-phase parity validator
- `scripts/Invoke-BloodlinesUnityLesserHouseLoyaltyParitySmokeValidation.ps1`
  - batch-mode wrapper for the dedicated validator

## Design Notes

- This slice deliberately stays out of the AI strategic layer. No files under
  `unity/Assets/_Bloodlines/Code/AI/**` were touched.
- The system builds on the prior marriage-parity slice rather than replacing
  it. `DynastyMixedBloodlineComponent`, `MarriageChildElement`, and
  `MarriageDeathDissolutionSystem` are now active inputs to lesser-house drift.
- The Unity-side defection window is explicit and auditable:
  - day 0 crossing starts the grace timer
  - the lesser house remains present during the next 5 in-world days
  - the breakaway spawn occurs on the first daily tick after the grace window
    has elapsed
- Breakaway spawning continues to use the existing `MinorHouseLevyComponent`
  seam so the next minor-house parity slice can tighten levy behavior without
  reshaping the defection path again.

## Validation

The slice is green on `D:\BLAICD\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene shells: Bootstrap + Gameplay
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. lesser-house loyalty parity smoke

Lesser-house parity smoke phases:

- Phase 1 PASS: active mixed-bloodline marriage anchor produces a positive
  daily recovery delta of `0.66` and advances loyalty from `50.00` to `50.66`
- Phase 2 PASS: hostile death-driven anchor fracture plus overwhelming world
  pressure produces a negative daily delta of `-1.10`, drops loyalty from
  `50.00` to `48.90`, and marks the anchor as fractured
- Phase 3 PASS: zero loyalty starts the grace timer at day `11`, holds through
  day `15`, defects on day `16`, applies legitimacy `20 -> 14`,
  ruthlessness `0 -> 1`, and spawns hostile breakaway faction
  `minor-cadet-ashfall`

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean worktree, those gates were run through temporary worktree-safe
  wrapper copies that preserved the same execute methods and pass markers while
  targeting `D:\BLAICD\bloodlines\unity`

## Current Readiness

This slice is complete and the active dynasty parity lane is ready for push.

## Next Action

1. Push `codex/unity-dynasty-lesser-house-loyalty-parity`.
2. Continue the same dynasty lane on a fresh `codex/unity-dynasty-*` branch.
3. Tighten `MinorHouseLevySystem` parity next, especially territorial-levy
   timing and breakaway-spawn integration.
4. After the dynasty parity stack closes, move into covert-ops or
   scout-raid/logistics-interdiction follow-ups from the directive.
