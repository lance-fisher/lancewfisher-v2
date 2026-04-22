# Unity Player HUD Slice: Fortification Legibility Readout

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-player-hud-fortification`
- Status: validated on branch

## Goal

Port the browser fortification legibility block into the active player HUD lane
as a settlement-scoped ECS read-model under `unity/Assets/_Bloodlines/Code/HUD/`
without widening the paused fortification lane or touching Claude's AI-owned
`unity/Assets/_Bloodlines/Code/AI/**` path.

## Browser Reference

- `src/game/core/simulation.js`
  - `getRealmConditionSnapshot` (14291-14764)
  - fortification block (14568-14629)
    - `tier`
    - `threatActive`
    - `readyReserves`
    - `recoveringReserves`
    - ward / commander / sortie / imminent-engagement context
- Unity-side supporting read seams:
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/BreachSealingProgressComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/DestroyedCounterRecoveryProgressComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveAssignmentComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationSettlementLinkComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDComponent.cs`
  now defines the player-owned settlement HUD snapshot with:
  - `SettlementId`
  - `OwnerFactionId`
  - `Tier`
  - `OpenBreachCount`
  - `ReserveFrontage`
  - `MusteredDefenderCount`
  - `ReadyReserveCount`
  - `RecoveringReserveCount`
  - `ThreatActive`
  - `SealingProgress01`
  - `RecoveryProgress01`
- `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDSystem.cs`
  now projects live fortification state into that read-model using ECS-only reads:
  - lazily attaches `FortificationHUDComponent` through `EntityCommandBuffer`
  - resolves reserve frontage from live linked same-faction defenders
  - resolves mustered defenders from `ReserveDutyState.Engaged` and `Muster`
  - reads `FortificationReserveComponent` for ready/recovering/threat telemetry
  - reads `BreachSealingProgressComponent` and
    `DestroyedCounterRecoveryProgressComponent` to surface normalized repair
    progress
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes the parseable
  `FortificationHUD|SettlementId=...|...` readout via
  `TryDebugGetFortificationHUDSnapshot(...)`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`
  now prove four dedicated phases:
  1. intact keep baseline
  2. threat-active reserve mustering
  3. tier-scaled sealing progress
  4. post-breach destroyed-counter recovery progress
- Narrow shared-file edits were applied to:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`
  so the new HUD runtime and editor validation files compile in the initialized
  local project.

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Unity exited with code 0`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Unity exited with code 0`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=78, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Fortification HUD smoke validation passed.`

## Unity-Side Simplifications Deferred

- This slice only ports the read-model and proof seam. It does not render an
  on-screen fortification panel yet.
- The broader browser fortification block still includes sortie timers,
  commander/governor/ward labels, and imminent-engagement detail that are not
  surfaced in this narrow HUD slice.
- Victory-distance readout remains the next open follow-up inside the same HUD
  lane.

## Exact Next Action

1. Stage the fortification HUD slice files plus continuity and contract updates,
   commit them on `codex/unity-player-hud-fortification`, and push to `origin`.
2. After the landing continuity pass, continue the HUD lane with the remaining
   victory-distance readout slice on a fresh Codex branch.
