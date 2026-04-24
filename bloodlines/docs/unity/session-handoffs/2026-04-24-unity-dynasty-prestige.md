# Unity Slice Handoff: dynasty-prestige / Dynasty Prestige Decay Modulation

**Date:** 2026-04-24
**Lane:** dynasty-prestige
**Branch:** claude/unity-dynasty-prestige
**Status:** Complete

## Goal

Add prestige "drift" to the existing dynasty renown system by implementing `DynastyPrestigeDecayModulatorSystem`, which adjusts `DynastyRenownComponent.RenownDecayRate` each tick based on the dynasty's current political health. The prior `DynastyRenownAccumulationSystem` applied a fixed decay of 0.45/day; this slice makes that rate dynamic so interregnum, succession crisis, and conviction extremes meaningfully alter prestige trajectory.

## Browser Reference

Absent -- no dynasty-level prestige decay exists in simulation.js; the browser tracks per-member renown only. This slice lifts the design-bible dynasty prestige surface onto existing Unity ECS political state.

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` -- dynasty prestige maximization is a canonical victory-adjacent playstyle; renown must respond meaningfully to political health state.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPrestigeDecayModulatorSystem.cs` -- `[UpdateBefore(DynastyRenownAccumulationSystem)]`; per-faction decay rate computation: base 0.45 + InterregnumPenalty 0.225 (if DynastyStateComponent.Interregnum) + CrisisPenalty 0.1125 (if SuccessionCrisisComponent.RecoveryProgressPct < 1) + ApexCruelPenalty 0.0675 (ConvictionBand.ApexCruel) - ApexMoralDiscount 0.0675 (ConvictionBand.ApexMoral); floor 0.10/day; writes result to DynastyRenownComponent.RenownDecayRate
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPrestigeDecayModulatorSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyPrestige.cs` -- `TryDebugGetPrestigeDecayRate(factionId, out decayRate, out interregnumActive, out crisisActive, out convictionBand)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyPrestige.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyPrestigeDriftSmokeValidation.cs` -- 3-phase smoke: (1) healthy dynasty uses base rate; interregnum adds canonical penalty; (2) active succession crisis adds additional penalty and stacks with interregnum; (3) ApexCruel exceeds neutral, ApexMoral falls below neutral, never below floor 0.10
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyPrestigeDriftSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityDynastyPrestigeDriftSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 2 new Compile entries
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 130→131; added dynasty-prestige lane entry

## Scope Discipline

- Did not modify DynastyRenownAccumulationSystem (decay is applied there as-written; this system only updates the rate before accumulation runs)
- Did not add new fields to DynastyRenownComponent (RenownDecayRate already exists)
- Did not wire population prestige from DynastyXPAwardSystem cross-match bonuses (separate lane concern)
- Did not implement per-member individual renown drift (per-member renown is owned by the dynasty-core lane)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Dynasty prestige drift smoke -- PASS (3 phases green)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=131)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to next unclaimed lane: audio dispatch foundation (Wwise scaffolding) or dynasty persistence (save/load for cross-match dynasty state).
