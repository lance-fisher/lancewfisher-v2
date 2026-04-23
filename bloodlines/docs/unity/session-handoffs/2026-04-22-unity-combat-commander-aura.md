# 2026-04-22 Unity Combat Commander Aura

## Slice Summary

- Lane: `combat-commander-aura`
- Branch: `codex/unity-combat-commander-aura`
- Browser reference searches used:
  - `COMMANDER_BASE_AURA_RADIUS`
  - `getCommanderAuraProfile`
  - `CONVICTION_BAND_EFFECTS`
  - `getConvictionBandEffects`
  - faith doctrine aura bonuses from `data/faiths.json`

## What Landed

- Added `CommanderAuraComponent`, `CommanderAuraRecipientComponent`, `CommanderAuraCanon`, and `CommanderAuraSystem` under `unity/Assets/_Bloodlines/Code/Combat/` so living commanders now project doctrine-aware, conviction-scaled aura profiles onto nearby same-faction units each simulation frame.
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CommanderAura.cs` so commander aura state is queryable by member id or unit id without widening the AI-owned lane.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCommanderAuraSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityCommanderAuraSmokeValidation.ps1` to prove in-range buff application, out-of-range non-application, debug readout availability, and cleanup after commander death.
- Narrow shared-file seams now consume commander aura effects:
  - `AttackResolutionSystem.cs` applies the live aura attack multiplier to the unit attack-cadence seam by dividing the post-hit cooldown reset by the strongest active aura multiplier.
  - `CombatStanceResolutionSystem.cs` applies the live aura morale bonus as retreat-threshold resistance only.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now explicitly include the new runtime and editor files for this slice.

## Validation

- Dedicated commander aura smoke: PASS
  - `Commander aura smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed 10-gate chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Canonical scene shell validation passed.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=95, last-updated=2026-04-22 is current.`

## Deferred / Notes

- The browser reference exposes commander radius, attack, and sight directly. The directive also required speed and morale bonuses, so `CommanderAuraCanon` extends the browser profile additively while still anchoring doctrine radius/attack/sight values to the frozen spec.
- Conviction scaling currently reuses the existing conviction combat-pressure multiplier surface via `ConvictionBandEffects.ForBand(band).AttackMultiplier` until the browser spec exposes a dedicated commander-aura band table.
- Unity still emits pre-existing ordering warnings for older systems during some batch validations. The commander aura slice added new update ordering only on its own seam and did not widen those existing warnings.

## Exact Next Action

- Claim Priority 6 `fortification-postures` from `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`, branch from updated `master`, and port imminent-engagement fortification postures with a dedicated smoke validator.
