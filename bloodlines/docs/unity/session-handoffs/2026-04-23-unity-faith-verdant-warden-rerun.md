# 2026-04-23 Unity Faith Verdant Warden Rerun Follow-Up

## Slice Summary

- Lane: `faith-verdant-warden`
- Branch: `codex/unity-faith-verdant-warden-rerun`
- Upstream reality during this rerun:
  - `origin/master` merged `codex/unity-faith-verdant-warden` while this
    rerun branch was still validating locally, so the original Verdant Warden
    runtime slice was already canonical before this follow-up landed
- Browser/runtime references rechecked during rerun:
  - `src/game/core/simulation.js` `getVerdantWardenSupportProfile`
  - `src/game/core/simulation.js` `getVerdantWardenZoneSupportProfile`
  - `src/game/core/simulation.js` `applyControlPointLoyaltyDelta`
  - fortification-defense Verdant support stack in
    `getSettlementDefenseState`

## What Landed

- Preserved the already-landed canonical Verdant Warden runtime on
  `origin/master` instead of replacing it with the local rerun variant. The
  authoritative runtime handoff remains
  `docs/unity/session-handoffs/2026-04-23-unity-faith-verdant-warden.md`.
- Hardened the governed validation wrappers:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` now treats an
    explicit PASS marker as authoritative even when Unity batchmode returns
    `-1` after a successful combat validation run.
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1` now does the same
    for the siege smoke gate.
- Normalized generated project metadata:
  - `unity/Assembly-CSharp.csproj` and
    `unity/Assembly-CSharp-Editor.csproj` now point Unity.Entities analyzers at
    the canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
    surface instead of a stale prior worktree path.
  - the duplicate rerun-only
    `BloodlinesDebugCommandSurface.VerdantWarden.cs` registration was dropped
    so the canonical master line keeps the existing
    `BloodlinesDebugCommandSurface.Faith.VerdantWarden.cs` partial only.
- This worktree still required a local `unity/Library` junction to the
  canonical `D:\ProjectsHome\Bloodlines\unity\Library` surface so governed
  `.csproj` build gates could resolve `Library\ScriptAssemblies`.

## Validation

- Dedicated Verdant Warden smoke on the already-landed canonical runtime: PASS
  - `Verdant Warden smoke validation passed.`
- Governed full validation chain: PASS
  - `Build succeeded.`
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=98, last-updated=2026-04-23 is current.`

## Deferred / Notes

- Because the upstream `codex/unity-faith-verdant-warden` branch landed before
  this rerun branch rebased, this follow-up intentionally narrows itself to
  validation-wrapper reliability and `.csproj` path canonicalization only.
- The canonical bootstrap and canonical-scene-shell wrappers still target
  `D:\ProjectsHome\Bloodlines\unity`; branch-local validation therefore
  continued to rely on the local `unity/Library` junction repair.

## Exact Next Action

1. Claim Priority 8 `faith-exposure-walker` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open `codex/unity-faith-exposure-walker` from updated `master`.
3. Port sacred-site exposure walking plus wayshrine amplification with a
   dedicated smoke validator.
