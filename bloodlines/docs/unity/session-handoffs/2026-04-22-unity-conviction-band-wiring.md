# Unity Conviction Band Wiring Slice (2026-04-22)

## Status

- Branch: `codex/unity-conviction-band-wiring-finish`
- State: complete on branch, full governed gate green

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`
  now reads `ConvictionComponent` and applies the canonical conviction table as protection on starvation-side losses:
  negative loyalty deltas are divided by `max(1, LoyaltyProtectionMultiplier)` and famine population decline is reduced by
  `PopulationGrowthMultiplier` with a floor of one head when any decline occurs.
- `unity/Assets/_Bloodlines/Code/Economy/CapPressureResponseSystem.cs`
  now applies the same conviction loyalty-protection multiplier to negative cap-pressure loyalty loss so the downstream
  loyalty seam is consistent across starvation and over-cap pressure.
- `unity/Assets/_Bloodlines/Code/Combat/PendingCommanderCaptureComponent.cs`
  introduces a narrow commander-only capture bridge for conviction-driven `CaptureMultiplier` consumption. The helper
  resolves the attacker's faction conviction band, converts the canonical multiplier into a deterministic capture chance,
  and marks lethal commander defeats for capture without widening generic militia identity.
- `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs`
  and
  `unity/Assets/_Bloodlines/Code/Combat/ProjectileImpactSystem.cs`
  now call the commander-capture helper when lethal damage lands on a commander.
- `unity/Assets/_Bloodlines/Code/Combat/DeathResolutionSystem.cs`
  now resolves pending commander capture before dead-tag cleanup, writes `CapturedMemberElement` to the captor faction
  root, and marks the matched `DynastyMemberComponent` as `Captured`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
  now runs starvation-protection, cap-pressure-protection, and commander-capture phases in dedicated ECS worlds. The
  validator proves higher conviction reduces starvation loss, reduces cap-pressure loyalty loss, and enables commander
  capture to emit a captive plus captured dynasty-member state.

## Validation Completed

- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- `artifacts/Invoke-LocalUnityBootstrapRuntimeSmoke.ps1` under `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`: PASS
- `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` under wrapper lock: PASS
- `artifacts/Invoke-LocalUnityValidateCanonicalSceneShells.ps1` under wrapper lock: PASS
- `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` under wrapper lock: PASS
- `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1` under wrapper lock: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`: PASS after contract bump
- `artifacts/Invoke-LocalUnityConvictionSmoke.ps1` under wrapper lock: PASS

## Scope Decision

- The slice closes on the narrower commander-only interpretation of `CaptureMultiplier`.
- Generic combat units still do not carry captive-ready dynasty identity, so ordinary militia / workers remain outside
  this conviction capture hook until a broader unit-to-dynasty captive bridge exists.
- `CapturedMemberElement` emission is routed through the defeated commander's dynasty member record and the captor
  faction root buffer only.

## Recommended Next Action

1. Commit and push the validated conviction-band wiring slice on `codex/unity-conviction-band-wiring-finish`.
2. Merge the branch to `master` and rerun the full governed gate on merged `master`.
3. After the landing continuity pass, continue the next Codex priority slice from the player-facing HUD lane:
   fortification legibility or victory-distance readout.
