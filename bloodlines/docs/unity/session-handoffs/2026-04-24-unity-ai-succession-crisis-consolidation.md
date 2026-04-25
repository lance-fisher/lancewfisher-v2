# Session Handoff: Sub-slice 34 -- AI Succession Crisis Consolidation

## Goal

Implement the AI enemy faction's automatic succession-crisis consolidation timer and execution: when a succession crisis is active and resources and an active ruler are available, the AI consolidates the court, recovering legitimacy, control-point loyalty, and conviction.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisConsolidationComponent.cs` -- single float timer (`SuccessionCrisisTimer`, default 12s).
- `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisConsolidationSystem.cs` -- timer-driven consolidation. Gates: active `SuccessionCrisisComponent` with severity > None; HeadOfBloodline member not Fallen/Captured; resources >= cost per severity tier. On success: deduct resources, legitimacy+recovery (clamped 0-100), all owned CP loyalty+recovery (clamped 0-100), Stewardship+gain via `ConvictionScoring.ApplyEvent`, push `DeclareInWorldTimeRequest` for computed day delta, remove `SuccessionCrisisComponent`, push Info narrative. Timers: 60s on success, 18s on failed attempt, 12s when no crisis.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SuccessionCrisisConsolidation.cs` -- debug surface.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisConsolidationSmokeValidation.cs` -- 4-phase smoke validator (consolidates-when-available, retry-when-resources-short, skips-when-no-crisis, not-yet-fired). Marker: `BLOODLINES_SUCCESSION_CRISIS_CONSOLIDATION_SMOKE PASS/FAIL`.
- `scripts/Invoke-BloodlinesUnitySuccessionCrisisConsolidationSmokeValidation.ps1` -- PS1 wrapper.

### Severity Cost/Recovery Table (simulation.js ~55-111)

| Severity   | Gold | Influence | Legit | Loyalty | Stewardship | World Days |
|------------|------|-----------|-------|---------|-------------|-----------|
| Minor (1)  | 80   | 18        | +4    | +3      | +2          | 15        |
| Moderate(2)| 110  | 24        | +6    | +4      | +3          | 20        |
| Major (3)  | 145  | 32        | +8    | +5      | +4          | 25        |
| Catastrophic(4)| 190| 42     | +10   | +6      | +5          | 30        |

### Modified Files

- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries.
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped to revision 139.

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: 0 errors.
- `dotnet build Assembly-CSharp-Editor.csproj`: 0 errors.
- `node tests/data-validation.mjs`: exit 0.
- `node tests/runtime-bridge.mjs`: exit 0.
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: exit 0, revision 139 confirmed current.

## Current Readiness

Branch `claude/unity-ai-succession-crisis-consolidation` is ready to merge to master.

## Next Action

Merge sub-slice 34 to master. Next candidates in the ai-strategic-layer lane: faith commitment auto-selection, covenant test action dispatch, or scout-raid dispatch system.

## Browser Reference

- `src/game/core/ai.js` updateEnemyAi successionCrisisTimer block (~1167-1185).
- `src/game/core/simulation.js` `getSuccessionCrisisTerms` (~4651-4693), `consolidateSuccessionCrisis` (~4695-4741).
