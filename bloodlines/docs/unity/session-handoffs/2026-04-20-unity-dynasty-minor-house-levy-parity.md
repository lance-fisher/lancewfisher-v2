# 2026-04-20 Unity Dynasty Minor-House Levy Parity

## Scope

Prompt-accurate continuation of the active dynasty house parity lane on
`codex/unity-dynasty-minor-house-levy-parity`.

This slice starts from the already-landed marriage and lesser-house parity
work. The goal was to replace the retired Tier 2 minor-house levy placeholder
with the canonical browser levy state machine and to widen breakaway spawning so
defected minor houses arrive with the live ECS surfaces that levy behavior
expects.

## Browser References

- `src/game/core/simulation.js`
  - `spawnDefectedMinorTerritoryClaim` (~6801)
  - `spawnDefectedMinorFaction` (~6851)
  - `getMinorHouseClaim` (~6982)
  - `getMinorHouseCombatUnits` (~6987)
  - `ensureMinorHouseLevyState` (~6996)
  - `getMinorHouseRetinueCap` (~7011)
  - `pickMinorHouseLevyProfile` (~7024)
  - `spawnMinorHouseRetinueUnit` (~7034)
  - `getMinorHousePressureOpportunityProfile` (~13913)
  - `tickMinorHouseTerritorialLevies` (~7060)
- `src/game/core/ai.js`
  - read-only confirmation that this slice did not widen the AI strategic layer

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - expands `MinorHouseLevyComponent` from a three-field timer into the runtime
    payload the browser levy loop expects:
    - origin parent faction id
    - claim control-point id
    - levy status enum
    - selected levy unit id
    - retinue count and cap
    - last levy timestamp and last levy unit id
    - parent-pressure level, status, tempo, and retinue-cap bonus
  - adds `MinorHouseLevyStatus`
- `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - defected minor houses now spawn with:
    - `ResourceStockpileComponent`
    - `PopulationComponent`
    - `DynastyStateComponent` seeded to legitimacy `30`
    - inherited `FaithStateComponent` when the parent is committed
    - levy provenance (`OriginFactionId`, claim id, forming status)
    - a stabilized border-claim `ControlPointComponent` with:
      - loyalty `62`
      - food trickle `0.08`
      - influence trickle `0.06`
      - settlement class `border_settlement`
  - claim placement now resolves from the parent realm's nearest owned control
    point or settlement and chooses the least-crowded offset among eight
    candidate positions
- `unity/Assets/_Bloodlines/Code/Dynasties/MinorHouseLevySystem.cs`
  - replaces the placeholder interval-only levy loop with the browser-shaped
    state machine
  - landless, dispossessed, contested, unsettled, awaiting-food,
    awaiting-influence, levying, mustered, raised, and stalled states are now
    explicit
  - claim loyalty gate is now canonical at `48`
  - levy progress now decays at the canonical `0.6` pace when the minor house
    is blocked instead of accumulating forever
  - retinue cap now depends on claim stability, claim loyalty, territory count,
    and parent world-pressure bonus, clamped to `[1, 6]`
  - world-pressure tempo is now lifted from the browser profile:
    - levy tempo multiplier `1 + level*0.22 + scoreBonus`
    - retake tempo multiplier `1 + level*0.28 + scoreBonus`
    - retinue-cap bonus `0/1/2` by pressure level
  - levy profile selection now matches the browser:
    - `militia`
    - `swordsman`
    - `bowman` at high-loyalty, deeper-retinue claims
  - levies now spend food, influence, and claim loyalty, then spawn a real ECS
    combat unit with movement, combat, stance, and projectile data
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMinorHouseLevyParitySmokeValidation.cs`
  - dedicated 4-phase parity validator
- `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1`
  - batch-mode wrapper for the dedicated validator

## Design Notes

- This slice deliberately stays out of `unity/Assets/_Bloodlines/Code/AI/**`.
- Breakaway spawning now covers the minimum browser-equivalent runtime surfaces
  needed for a minor house to exist as a live splinter polity:
  territory, stockpile, population pool, hostility, and levy cadence.
- The browser still carries deeper founder-copy, ceremonial messaging, and
  fully autonomous minor-house AI behavior. Those remain valid follow-ups, but
  the spawned minor faction is no longer a registry-only placeholder in Unity.
- The new levy unit spawner copies the canonical `militia`, `swordsman`, and
  `bowman` stats directly from `data/units.json` so this slice does not need to
  reach into editor-only definition assets at runtime.

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
11. lesser-house loyalty parity regression smoke
12. minor-house levy parity smoke

Minor-house parity smoke phases:

- Phase 1 PASS: lesser-house defection now spawns a live minor faction with
  levy provenance plus a stabilized border claim
- Phase 2 PASS: landless minor houses decay levy progress instead of raising
  phantom troops
- Phase 3 PASS: a pressured stabilized claim raises a `bowman`, spends food and
  influence, burns claim loyalty, and updates retinue state
- Phase 4 PASS: a cap-hitting retinue enters `Mustered`, clears progress, and
  blocks over-mustering

Regression note:

- the prior `BloodlinesLesserHouseLoyaltyParitySmokeValidation` was re-run
  green after the breakaway-spawn widening so the new claim and stockpile
  seeding did not break the day-16 defection proof

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean worktree, those gates were run through temporary worktree-safe
  wrapper copies that preserved the same execute methods and pass markers while
  targeting `D:\BLAICD\bloodlines\unity`

## Current Readiness

This slice is complete and the dynasty-house parity lane can pause cleanly.

## Next Action

1. Push `codex/unity-dynasty-minor-house-levy-parity`.
2. Merge to `master` if the rebased tree is still clean.
3. Under current governance, move to a fresh non-AI lane next, with
   scout-raid/logistics-interdiction work the cleanest unblocked target if the
   AI strategic layer remains contract-owned elsewhere.
4. Optional dynasty follow-up only if the owner wants more parity before that:
   founder-member and ceremonial-message provenance on defected minor houses.
