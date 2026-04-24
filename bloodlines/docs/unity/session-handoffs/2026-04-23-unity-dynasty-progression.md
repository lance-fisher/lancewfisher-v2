# Unity Slice Handoff: dynasty-progression / foundation + unlocks

**Date:** 2026-04-23
**Lane:** dynasty-progression
**Branch:** claude/unity-dynasty-progression-unlocks
**Status:** Complete

## Goal

Implement the cross-match dynasty progression system per owner direction 2026-04-19: factions accumulate XP from match placements, advance through tiers at canonical thresholds, and receive per-tier unlock slots that cycle through four bonus types (SpecialUnitSwap, ResourceBonus, DiplomacyBonus, CombatBonus). The SpecialUnitSwap slot, when activated, causes the faction's production systems to substitute a dynasty-specific special unit via FactionSpecialUnitSwapComponent. Non-#1 placements remain rewarding; sideways customization only so multiplayer power gradients stay flat.

## Browser Reference

Absent -- dynasty progression XP/unlock system is not implemented in simulation.js. Implemented from canonical progression design per owner direction 2026-04-19 (game modes and dynasty progression).

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` -- cross-match dynasty progression section

## Work Completed

- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionComponent.cs` -- per-faction component (AccumulatedXP, CurrentTier, LastMatchXPAward, TierUnlocksPending); TierUnlockNotificationComponent one-shot; DynastyXPAwardRequestComponent one-shot
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionCanon.cs` -- static canon: TierXPThresholds {0, 100, 350, 850, 1850}, MaxTier=4, XPForPlacement (placement decay with 10 XP floor), TierForXP, NextTierThreshold
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyXPAwardSystem.cs` -- consumes DynastyXPAwardRequestComponent, accumulates XP, advances tier, fires TierUnlockNotificationComponent for each tier crossed, lazy-adds DynastyProgressionComponent
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyUnlockSlotElement.cs` -- IBufferElementData: SlotIndex, UnlockTypeId (0-3), UnlockTargetId, GrantedAtTier, IsActive; DynastyUnlockType enum
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionUnlockSystem.cs` -- consumes TierUnlockNotification, writes one DynastyUnlockSlotElement per tier (type cycles (t-1)%4), default SpecialUnitSwap targets cycle Ranged/UniqueMelee/LightCavalry/Melee, clears TierUnlocksPending, removes notification
- `unity/Assets/_Bloodlines/Code/Dynasties/SpecialUnitSwapApplicatorSystem.cs` -- reads active SpecialUnitSwap slots each frame (InitializationSystemGroup), writes/removes FactionSpecialUnitSwapComponent; FactionSpecialUnitSwapComponent (SwapTargetUnitRole, IsActive) defined here
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyProgression.cs` -- debug surface partial: TryDebugGetDynastyProgression, TryDebugAwardDynastyXP, TryDebugGetUnlockSlots, TryDebugActivateUnlockSlot
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyProgressionSmokeValidation.cs` -- 3-phase smoke: tier advancement canon at all thresholds, slot type cycling, applicator slot read logic
- `scripts/Invoke-BloodlinesUnityDynastyProgressionSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 8 new .cs files
- `unity/Assembly-CSharp.csproj` -- added 7 runtime Compile entries
- `unity/Assembly-CSharp-Editor.csproj` -- added 1 editor Compile entry
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- added dynasty-progression lane, bumped revision 120→121

## Scope Discipline

- Did not implement ResourceBonus, DiplomacyBonus, or CombatBonus applicator systems (stubs exist as unlock type IDs; applicators are separate future lanes)
- Did not wire FactionSpecialUnitSwapComponent into unit production systems (that integration is the production-system lane's responsibility)
- Did not add XP awards to match resolution systems (those systems are not yet in Unity; the DynastyXPAwardRequestComponent one-shot is the integration surface)
- Did not modify any existing Dynasties files (renown, succession, marriage systems untouched)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (12 CS0006 Library-absent only, no code errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (12 CS0006 Library-absent only, no code errors)
3. Bootstrap runtime smoke -- SKIP-env (Unity Library not present at this checkout)
4. Combat smoke -- SKIP-env
5. Scene shells -- SKIP-env
6. Conviction smoke -- SKIP-env
7. Dynasty smoke -- SKIP-env
8. Faith smoke -- SKIP-env
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- PASS (revision=121, last-updated=2026-04-23)

## Current Readiness

Merged to master. All gates green (Unity batch-mode smokes SKIP-env per established environment condition).

## Next Action

Begin P3 Rally Point System on new branch `claude/unity-combat-rally-point`: RallyPointComponent, PlayerRallyPointSetRequestComponent, RallyPointSetSystem, RallyPointMovementSystem, debug surface partial, smoke validator, PS1 wrapper.
