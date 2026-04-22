# Unity Player HUD Slice: Victory Distance Readout

- Date: 2026-04-22
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-hud-victory-distance-readout`
- Status: implemented; awaiting governed landing commit

## Goal

Add the next HUD follow-up in the existing player-HUD lane: a per-faction victory
distance readout that surfaces progress toward Command Hall Fall, Territorial
Governance, and Divine Right through a HUD-owned ECS buffer plus a parseable
debug seam.

## Browser Reference

- `src/game/core/simulation.js`
  - `getTerritorialGovernanceProfile` (1207-1353)
  - `serializeTerritorialGovernanceRecognition` (1434-1532)
  - territorial-governance victory completion (1684-1688)
  - command-hall fall resolution (7827-7833)
  - divine-right declaration completion (10734-10742)
- `tests/runtime-bridge.mjs`
  - Territorial Governance victory assertions (8905-8929)
  - Divine Right completion assertions (8240-8245)

## What Changed

- `unity/Assets/_Bloodlines/Code/HUD/VictoryConditionReadoutComponent.cs`
  adds the HUD-owned per-faction `DynamicBuffer<VictoryConditionReadoutComponent>`
  plus `VictoryConditionReadoutRefreshComponent` throttle state.
- `unity/Assets/_Bloodlines/Code/HUD/VictoryConditionReadoutSystem.cs`
  now runs in `PresentationSystemGroup`, attaches the readout buffer to faction
  roots, refreshes it on an in-world-day cadence, and computes three progress
  tracks:
  - `TerritorialGovernance` from loyal-held territory share plus the live player
    sovereignty-hold timer when the realm is fully integrated
  - `DivineRight` from faith level/intensity threshold progress
  - `CommandHallFall` from destroyed hostile command-hall state
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetVictoryReadout(factionId, out string readout)` with a
  parseable multi-line
  `VictoryReadout|FactionId=...|ConditionId=...|ProgressPct=...|IsLeading=...|TimeRemainingEstimateInWorldDays=...`
  surface.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryReadoutSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityVictoryReadoutSmokeValidation.ps1`
  now prove four dedicated phases:
  - partial Territorial Governance progress from majority loyal claims
  - positive Divine Right progress from high faith intensity
  - `CommandHallFall` completion when an enemy hall is destroyed
  - per-condition `IsLeading` flags on the dominant faction
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were locally corrected so analyzer/source-generator references point back to
  this worktree's `unity/Library/PackageCache` instead of another stale Codex
  checkout. That repair was required before the governed `dotnet build` gates
  could turn green in this workspace.

## Validation Proof

- Dedicated victory readout smoke:
  - `Victory readout smoke validation passed.`
- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)` with pre-existing editor warnings only

## Unity-Side Simplifications Deferred

- Territorial Governance uses the live sovereignty-hold timer only for the
  player faction because the current `VictoryStateComponent` tracks a single
  hold counter. Rival factions still surface loyalty-share progress, but not a
  rival-specific hold ETA.
- Divine Right exposes threshold progress and a zero-day ETA only when already
  complete. There is no predictive intensity-growth model yet, so in-progress
  entries report `NaN` time remaining by design.
- This slice stops at the per-faction read-model and debug seam. The consolidated
  leaderboard/panel follow-up remains separate work.

## Exact Next Action

1. Run the full governed 10-gate chain after updating the contract and continuity
   files.
2. Stage the victory-readout slice files plus continuity / contract updates,
   commit them on `codex/unity-hud-victory-distance-readout`, and push to
   `origin`.
3. Continue the HUD lane with the victory-condition leaderboard panel once this
   slice is preserved.
