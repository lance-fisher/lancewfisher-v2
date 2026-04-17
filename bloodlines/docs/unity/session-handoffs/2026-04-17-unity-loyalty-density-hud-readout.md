# 2026-04-17 Unity Loyalty + Population Density HUD Readout

## Goal

Make civilizational loyalty and population density visible on the battlefield
HUD so the player can see strain and surplus pressure forming in Play Mode
without opening the debug console. Closes the visible-gap between the
Session 117-122 canonical realm-condition effects (famine/water/cap/surplus
loyalty deltas) and what the player can actually observe on screen.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  - `FactionHudSnapshot` struct gains `Loyalty`, `LoyaltyMax`, and
    `LoyaltyFound` fields.
  - `GetControlledFactionSnapshot` now also reads the controlled faction's
    `FactionLoyaltyComponent` when present and populates the snapshot.
  - `BuildHudText` adds:
    - a density readout `density <percent>%` computed from
      `PopulationTotal / PopulationCap` on the existing population line, so
      the same cap-pressure threshold the canonical system reads is
      visible to the player
    - a `<b>Loyalty</b>: <current>/<max>` segment on the same line when
      the controlled faction has a `FactionLoyaltyComponent`
  - Layout stays on the existing three-column single-row pattern so the
    HUD height does not need to grow.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
  Success line still ends with `stabilitySurplusObserved=True` alongside
  `aiConstructionObserved=True`, `aiActivityObserved=True`,
  `capPressureObserved=True`, `loyaltyDeclineObserved=True`,
  `starvationObserved=True`, `trickleGainObserved=True`,
  `gatherDepositObserved=True`, and all construction + production progress
  fields. HUD extension is purely additive and does not perturb the
  governed smoke flow.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The player can now see, at a glance in Play Mode:

- population total / cap + density percentage
- civilizational loyalty out of 100 (when the controlled faction carries
  `FactionLoyaltyComponent`)

This makes the Session 117-122 famine / water-crisis / cap-pressure loyalty
deltas and the new stability-surplus restoration legible without any
external debug surface.

## Next Action

Still on `claude/unity-enemy-ai-economic-base`:

1. Enemy AI militia posturing: when militia count is above a threshold,
   issue move orders toward the nearest hostile control point. Pairs
   with Codex's projectile + attack-move work once merged.
2. Loyalty-color legibility on the HUD (green / yellow / red bands based
   on `LoyaltyGreenFloor` and `LoyaltyYellowFloor`) once rich-text color
   formatting is standardized.
3. Per-faction realm-condition strain readout (`FoodStrainStreak`,
   `WaterStrainStreak`) under a diagnostics toggle.

Concurrent work: Codex on `codex/unity-projectile-combat` (finished per
its prior report, but also iterating on combat AI integration).
