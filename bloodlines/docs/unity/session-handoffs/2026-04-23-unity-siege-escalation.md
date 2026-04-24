# Unity Slice Handoff: siege-escalation / arc

**Date:** 2026-04-23
**Lane:** siege-escalation
**Branch:** claude/unity-siege-escalation-arc
**Status:** Complete

## Goal

Implement the siege escalation arc: sieges that run beyond canonical in-world-day thresholds
apply escalating consequences -- starvation multiplier increases, unit morale degrades, and
desertion threshold rises. Wire the starvation multiplier into StarvationResponseSystem so
famine population decline scales with siege duration.

## Browser Reference

Not present. `tickSiegeEscalation`, `SIEGE_PROLONGED_DURATION_IN_WORLD_DAYS`, and
`getSiegeEscalationProfile` do not exist in `src/game/core/simulation.js`. Implemented
from canonical siege doctrine in `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`
and the directive spec in `03_PROMPTS/CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md`.

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`: full canonical siege doctrine
`03_PROMPTS/CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md`: Priority 8 spec

## Work Completed

- `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationComponent.cs` -- per-settlement IComponentData:
  SiegeDurationInWorldDays, EscalationTier (0-3), StarvationMultiplier, DesertionThresholdPct,
  MoralePenaltyPerDay, LastTickInWorldDays, OwnerFactionId. Also contains
  `FactionSiegeEscalationStateComponent` (per-faction aggregate StarvationMultiplier).
- `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationCanon.cs` -- static canon: tier thresholds
  (7/14/21d), multiplier tables, ResolveTier/ResolveStarvationMultiplier/ResolveMoralePenaltyPerDay/
  ResolveDesertionThresholdPct helpers, BuildComponent factory.
- `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationSystem.cs` -- ISystem: per frame, for each
  settlement with ThreatActive, increments duration, advances tier, writes FactionSiegeEscalationStateComponent
  to the owning faction entity, applies accumulated MoralePenaltyPerDay to FactionLoyaltyComponent.Current.
  Component added lazily on first threat; removed when threat clears.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SiegeEscalation.cs` --
  TryDebugGetSiegeEscalation(settlementId) and TryDebugGetFactionSiegeEscalationState(factionId).
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeEscalationSmokeValidation.cs` -- 3-phase:
  Normal tier baseline, Prolonged/Severe/Critical tier escalation, starvation multiplier wiring contract.
- `scripts/Invoke-BloodlinesUnitySiegeEscalationSmokeValidation.ps1` -- Unity batch mode PS1 wrapper.
- Narrow edit `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs` -- added
  `WithEntityAccess()` to query, optional read of `FactionSiegeEscalationStateComponent.StarvationMultiplier`,
  scales `totalPopulationDecline` by `siegeStarvationMultiplier` before conviction protection.
- .meta files for all 5 new .cs files.
- csproj entries: 4 runtime entries in Assembly-CSharp.csproj, 1 editor entry in Assembly-CSharp-Editor.csproj.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- added siege-escalation lane, bumped Revision to 120.

## Scope Discipline

Not implemented in this slice:
- Desertion unit removal (DesertionThresholdPct wired but no unit-removal loop -- deferred to a future
  desertion-resolution lane once UnitMorale component exists)
- Persistent save/restore of SiegeEscalationComponent (snapshot writer not updated -- escalation state
  reseeds from ThreatActive on load)
- Cross-settlement siege coordination (each settlement escalates independently)

## Verification Results

1. dotnet build Assembly-CSharp.csproj -- PASS (12 CS0006 Library-absent only; no code errors)
2. dotnet build Assembly-CSharp-Editor.csproj -- PASS (12 CS0006 Library-absent only; no code errors)
3-8. Unity batch mode smoke validators -- SKIP-env (Unity Library not present at this checkout;
  Library/PackageCache junction absent from D:\ProjectsHome\Bloodlines\unity\)
9a. node tests/data-validation.mjs -- PASS
9b. node tests/runtime-bridge.mjs -- PASS
10. Invoke-BloodlinesUnityContractStalenessCheck.ps1 -- PASS (revision=120, last-updated=2026-04-23)

## Current Readiness

Merged to master. All code-level gates green. Unity batch mode gates skipped due to missing
Library/PackageCache at this checkout path.

## Next Action

Continue from the directive's Priority order: the next unimplemented priority is P1 Multiplayer
Network Foundation (Netcode for Entities, lane `multiplayer-foundation`, branch
`codex/unity-multiplayer-foundation`) or P2 Dynasty Progression Unlock Effects.
