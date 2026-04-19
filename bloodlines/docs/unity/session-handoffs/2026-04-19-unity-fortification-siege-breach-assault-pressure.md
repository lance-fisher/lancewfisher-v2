# 2026-04-19 Unity Fortification Siege Breach Assault Pressure

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-breach-assault-pressure`. The branch initially
started from `origin/master` `8826d855` after Claude's ai-strategic-layer
sub-slice 16 narrative message bridge, then rebased onto `origin/master`
`dfec72f5` after Claude's sub-slice 17 narrative back-wire landed. The final
landed contract target is revision `34`.

This slice picks the highest-leverage `OpenBreachCount` consumer inside the
current lane boundaries: breach-aware assault pressure. Pathing was deferred
because the active contract does not give the fortification lane ownership of
`unity/Assets/_Bloodlines/Code/Pathing/**`, and debug-only legibility was lower
leverage than making breaches change live assault outcomes. The delivered seam
lets open breaches grant hostile assault units a bounded exploitation bonus
inside the breached settlement's threat envelope, so the broken line now
changes attacker tempo beyond reserve frontage and tier loss.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Siege/BreachAssaultPressureSystem.cs`
  - new runtime consumer of `FortificationComponent.OpenBreachCount`
  - runs after fortification destruction resolution and before field-water
    strain application
  - scans breached settlements and marks hostile units inside the breached
    settlement threat radius with a bounded assault-pressure bonus
- `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  - additive breach telemetry fields:
    `BreachAssaultAdvantageActive`, `BreachOpenCount`,
    `BreachTargetSettlementId`, `BreachAssaultAttackMultiplier`, and
    `BreachAssaultSpeedMultiplier`
- `unity/Assets/_Bloodlines/Code/Siege/FieldWaterStrainSystem.cs`
  - final attack and speed multipliers now consume the breach-assault bonus
    alongside existing field-water and siege-support modifiers
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportCanon.cs`
  - canonical bounded breach-exploitation tuning:
    +8% attack and +4% speed per open breach, capped at 3 breaches
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs`
  - validation world now includes the new breach-pressure system so the siege
    smoke reflects live runtime ordering
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachAssaultPressureSmokeValidation.cs`
  - dedicated 4-phase validator covering intact-wall baseline, single-breach
    attacker bonus, defender exclusion, and multi-breach scaling with radius
    gating
  - artifact marker:
    `BLOODLINES_BREACH_ASSAULT_PRESSURE_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnityBreachAssaultPressureSmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Browser And Canon References

- `src/game/core/simulation.js:190-193`
  - assault-failure tempo/cooldown constants used as the broader assault-tempo
    frame for the breach consumer
- `docs/unity/SYSTEM_MAP.md:138-156`
  - breach planning, reserve commitment, and canonical fortification/siege tick
    ordering
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
  - Pillar 2 connective tissue (`Inner fallback positions ↔ Final core`)
  - Pillar 5 no-wave-spam posture and tempo drag requirements
  - Pillar 6 breach planning / elite exploit force implications
- `04_SYSTEMS/SIEGE_SYSTEM.md`
  - breach planning, breach assault force, flooding reserves, exploitation plan,
    and tempo penalty framing

## Validation

The slice is green on the rebased worktree
`D:\BLBAP\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene shells: Bootstrap + Gameplay
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` (rerun after the
    revision-34 contract update; see below)
11. dedicated breach-assault-pressure smoke

Dedicated breach-assault-pressure smoke phases:

- Phase 1 PASS: intact wall keeps breach assault pressure inactive
- Phase 2 PASS: a single breach grants the hostile attacker the expected
  `1.08x` attack and `1.04x` speed exploitation bonus
- Phase 3 PASS: the settlement owner does not receive the breach assault bonus
- Phase 4 PASS: two open breaches scale the nearby attacker to `1.16x` attack
  while a distant attacker outside threat radius stays baseline

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-assault-pressure-smoke.log`

Local csproj refresh note:

- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` were
  refreshed locally because Unity had not regenerated them in this worktree
  after the rebase onto `dfec72f5`. The missing existing entries for
  `FortificationStructureResolutionSystem.cs` and
  `BloodlinesWallSegmentDestructionSmokeValidation.cs` were restored, and the
  new breach-assault files were added. Both csproj files remain gitignored and
  are not part of the commit.

## Branch State

- Branch: `codex/unity-fortification-breach-assault-pressure`
- Master base at close: `dfec72f5`
- Contract revision at handoff close: `34`
- Branch status: ready for push and merge coordination after revision-34 breach
  assault pressure delivery

## Next Action

1. Push `codex/unity-fortification-breach-assault-pressure`.
2. Merge it to `master` via the merge-temp ceremony.
3. Follow with the next `OpenBreachCount` consumer that stays within ownership:
   either breach HUD / legibility or a coordinated pathing / exploit slice after
   lane ownership for pathing is explicitly claimed or split.
