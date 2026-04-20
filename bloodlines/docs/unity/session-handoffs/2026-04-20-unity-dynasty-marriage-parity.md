# 2026-04-20 Unity Dynasty Marriage Parity

## Scope

Prompt-accurate continuation of the post-fortification priority stack on
`codex/unity-dynasty-marriage-parity`.

This slice starts from the already-landed Tier 2 dynasty foundation rather than
re-porting marriages from zero. The goal was to bring the existing Unity
marriage loop into parity with the canonical browser references for proposal
expiration, gestation timing, mixed-bloodline child provenance, and
death-driven dissolution.

## Browser References

- `src/game/core/simulation.js`
  - `MARRIAGE_GESTATION_IN_WORLD_DAYS` (6088)
  - `MARRIAGE_DISSOLUTION_LEGITIMACY_LOSS` (6089)
  - `MARRIAGE_DISSOLUTION_OATHKEEPING_GAIN` (6090)
  - `dissolveMarriageFromDeath` (6382)
  - `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS` (7272)
  - `tickMarriageProposalExpiration` (7274)
  - `acceptMarriage` mixed-bloodline child timestamps (7425, 7440)
  - `tickMarriageDissolutionFromDeath` (7471)
  - `tickMarriageGestation` (7496)
- `tests/runtime-bridge.mjs`
  - mixed-bloodline child assertions (3180-3229)
  - death-driven dissolution assertions (3270-3297)

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - corrected the public parity constants to browser values:
    - proposal expiration: `90`
    - gestation: `280`
  - added `MarriageComponent.DissolvedAtInWorldDays`
  - added `MarriageChildElement` to track generated child member ids
  - added `DynastyMixedBloodlineComponent` so gestated children carry head-house
    and spouse-house provenance
- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs`
  - now expires pending proposals at 90 in-world days
  - respects `ExpiresAtInWorldDays` when already authored on the proposal
- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs`
  - now uses the canonical 280-day gestation window
  - records the generated child id onto the primary marriage
  - adds `DynastyMixedBloodlineComponent` to the spawned child
  - resolves child provenance from `FactionHouseComponent` so the data matches
    the mixed-bloodline browser assertions
- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs`
  - new system that ports `dissolveMarriageFromDeath`
  - marks both marriage records with `Dissolved=true` and
    `DissolvedAtInWorldDays`
  - applies legitimacy loss and oathkeeping mourning effects to both sides
  - runs before gestation so dissolved marriages cannot keep producing children
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMarriageParitySmokeValidation.cs`
  - dedicated 4-phase parity validator
- `scripts/Invoke-BloodlinesUnityMarriageParitySmokeValidation.ps1`
  - batch-mode wrapper for the dedicated validator

## Design Notes

- This slice deliberately stays out of the AI strategic layer. No files under
  `unity/Assets/_Bloodlines/Code/AI/**` were touched.
- The new mixed-bloodline metadata is minimal and runtime-facing. It gives the
  existing lesser-house and restore parity work a stable seam without rewriting
  broader dynasty schemas.
- Dissolution uses the browser's legitimacy and oathkeeping values directly:
  `-2` legitimacy and `+1` oathkeeping on both houses involved in the death.
- The browser runtime's earlier Tier 2 placeholders in Unity (`30` days and
  `60` days) are now explicitly retired in favor of the canonical `90` and
  `280` values.

## Validation

The slice is green on `D:\BLDMP\bloodlines`:

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
11. marriage parity smoke

Marriage parity smoke phases:

- Phase 1 PASS: proposal remains pending at in-world day 89
- Phase 2 PASS: proposal expires at in-world day 90
- Phase 3 PASS: gestation at in-world day 280 produces exactly one mixed-bloodline child and records it on the marriage
- Phase 4 PASS: spouse death dissolves both marriage records, applies legitimacy and oathkeeping effects, and prevents a due child from spawning

Validation-path note:

- the checked-in bootstrap runtime and canonical scene-shell wrappers still pin
  `-projectPath` to `D:\ProjectsHome\Bloodlines\unity`
- for this clean worktree, those gates were run through temporary worktree-safe
  wrapper copies that preserved the same execute methods and pass markers while
  targeting `D:\BLDMP\bloodlines\unity`

## Current Readiness

This slice is complete and the new dynasty marriage parity lane is ready for
push.

## Next Action

1. Push `codex/unity-dynasty-marriage-parity`.
2. Continue the dynasty parity lane on a fresh `codex/unity-dynasty-*` branch.
3. Use the new mixed-bloodline metadata and dissolution hook to tighten
   lesser-house loyalty drift parity next.
4. Follow that with minor-house territorial levy and breakaway-spawn parity
   instead of reopening the marriage surface from zero.
