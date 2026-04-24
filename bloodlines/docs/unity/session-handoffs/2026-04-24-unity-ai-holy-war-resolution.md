# Unity Slice Handoff: ai-strategic-layer / AI Holy War Resolution (sub-slice 26)

**Date:** 2026-04-24
**Lane:** ai-strategic-layer
**Branch:** claude/unity-ai-holy-war-resolution
**Status:** Complete

## Goal

Port the two-phase holy war resolution lifecycle from the browser into Unity ECS. Phase A (declaration resolution at `ResolveAtInWorldDays`) materializes the `ActiveHolyWarElement` buffer entry on the source faction, applies the initial intensity boost and loyalty drain, records conviction events, and finalizes the `DynastyOperationComponent`. Phase B (war-tick sustained effects, per-frame) applies per-frame loyalty drain to the target, fires periodic intensity and loyalty pulses at the canonical 60 in-world-day interval (30 real seconds at DaysPerRealSecond=2), and prunes expired war entries from the buffer at `WarExpiresAtInWorldDays`.

Prior sub-slice 21 dispatched the holy war and attached `DynastyOperationHolyWarComponent` but intentionally deferred all resolution effects. This slice delivers those deferred effects.

## Browser Reference

`src/game/core/simulation.js`:
- `tickDynastyOperations` holy_war branch (~5590-5645): declaration resolution
- `createHolyWarEntry` (~10505-10520): war entry shape
- `tickFaithHolyWars` (~4160-4215): sustained per-frame drain and pulse
- Constants: `HOLY_WAR_PULSE_INTERVAL_SECONDS = 30` (line 9776), `HOLY_WAR_SUSTAINED_LOYALTY_DRAIN_MULTIPLIER = 1.5` (line 9777), `HOLY_WAR_SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER = 0.12` (line 9778, deferred)

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` -- sustained holy war pressure on target loyalties and faith intensity feed directly into the multi-faction political pressure dynamics that drive strategy depth.

## Work Completed

- `unity/Assets/_Bloodlines/Code/AI/ActiveHolyWarElement.cs` -- `IBufferElementData` stored on source faction entity; mirrors `faction.faith.activeHolyWars` entries; fields: Id, TargetFactionId, FaithId, DocPath, DeclaredAtInWorldDays, LastPulseAtInWorldDays, ExpiresAtInWorldDays, IntensityPulse, LoyaltyPulse; capped at 6 by AIHolyWarResolutionSystem
- `unity/Assets/_Bloodlines/Code/AI/ActiveHolyWarElement.cs.meta`
- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarResolutionSystem.cs` -- `[UpdateAfter(AIHolyWarExecutionSystem)]`; Phase A: walks `DynastyOperationHolyWarComponent` entities (Active=true, InWorldDays >= ResolveAtInWorldDays); void if source lost faith or target gone; creates ActiveHolyWarElement entry on source (deduplicated by target, cap 6); boosts source intensity by `max(3, intensityPulse * 2)` (browser simulation.js:5611); drains target lowest control point by `max(2, loyaltyPulse)` (browser :5615); applies conviction events (source dark=Desecration+2 / light=Oathkeeping+2, target Stewardship-1) if ConvictionComponent present; pushes declaration narrative; flips Active=false. Phase B: walks all faction entities with ActiveHolyWarElement buffer; prunes expired entries (ExpiresAtInWorldDays <= inWorldDays) with player-related expiration narrative; applies sustained loyalty drain per frame via `loyaltyPulse * 1.5 * dt`; fires pulse at 60 in-world-day interval (boost source intensity, drain target control point loyalty)
- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarResolutionSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HolyWarResolution.cs` -- `TryDebugGetActiveHolyWars(factionId, out count, out targetFactionIds, out expiresAtInWorldDays)`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HolyWarResolution.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarResolutionSmokeValidation.cs` -- 4-phase smoke: (1) declaration landing: op finalized, buffer entry created, intensity boosted, CP loyalty drained, narrative pushed; (2) void target gone: op finalized, no buffer entry; (3) pulse and tick: LastPulseAt=0 + inWorldDays=100 triggers pulse, intensity boosted, CP loyalty drained, LastPulseAt updated; (4) war expiration: entry past ExpiresAt pruned, expiration narrative pushed
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarResolutionSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnityHolyWarResolutionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (ActiveHolyWarElement, AIHolyWarResolutionSystem, BloodlinesDebugCommandSurface.HolyWarResolution)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesHolyWarResolutionSmokeValidation)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 131→132; added sub-slice 26 browser reference and owned paths

## Scope Discipline

- Did not modify `AIHolyWarExecutionSystem` (dispatch path unchanged)
- Did not port `adjustLegitimacy` sustained legitimacy drain (no canonical Legitimacy field outside dynasty-core lane; deferred)
- Did not port `ensureMutualHostility` at declaration resolution (no hostility component pair ported yet; deferred)
- Did not port `syncFaithIntensityState` (faith intensity level/tier synchronization; not yet in Unity)
- Did not implement divine right resolution (sub-slice 27, separate lane work)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Holy war resolution smoke -- PASS (4 phases green: declaration landing, void no target, pulse and tick, war expiration)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=132)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to sub-slice 27: divine right resolution effects -- fires at `DynastyOperationDivineRightComponent.ResolveAtInWorldDays`; on success applies apex faith claim; on failure fires `failDivineRightDeclaration` with cooldown and legitimacy penalty; records conviction events (oathkeeping for light / desecration for dark, +3).
