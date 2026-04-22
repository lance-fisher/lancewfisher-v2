# Unity Conviction Band Wiring Slice (2026-04-22)

## Status

- Branch: `codex/unity-conviction-band-wiring`
- State: in progress, checkpointed with build + conviction smoke green

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`
  now reads `ConvictionComponent` and applies the canonical conviction table as protection on starvation-side losses:
  negative loyalty deltas are divided by `max(1, LoyaltyProtectionMultiplier)` and famine population decline is reduced by
  `PopulationGrowthMultiplier` with a floor of one head when any decline occurs.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
  now runs a fifth starvation-protection phase in a dedicated ECS world. It seeds identical famine conditions for an
  `ApexMoral` faction and a `Neutral` faction, then proves the conviction wiring reduces both loyalty loss and
  population decline for the higher-band faction.

## Validation Completed At Checkpoint

- `dotnet build bloodlines/unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build bloodlines/unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`: PASS

## Unresolved Seam Before The Slice Can Close

- The directive calls for a `CaptureMultiplier` hook in `AttackResolutionSystem`, but the live Unity combat runtime does
  not have a generic captive-ready member identity on every combat unit.
- `CapturedMemberElement` requires a stable `MemberId` and `MemberTitle`. Today that identity exists on
  `DynastyMemberComponent` and `CommanderComponent`, not on ordinary militia / worker units.
- The next action should decide whether this hook:
  1. applies only to commander / bloodline-backed combatants for this slice, or
  2. waits for a broader combat-unit-to-dynasty-member capture bridge.

## Recommended Next Action

1. Resolve the combat capture scope.
2. If commander-only is approved, wire `AttackResolutionSystem` to capture lethal commander defeats into
   `CapturedMemberElement`, mark the corresponding dynasty member `Captured`, and extend the conviction smoke or combat
   smoke with a deterministic proof phase.
3. Re-run the full 10-gate chain plus conviction smoke, then append continuity and land the slice.
