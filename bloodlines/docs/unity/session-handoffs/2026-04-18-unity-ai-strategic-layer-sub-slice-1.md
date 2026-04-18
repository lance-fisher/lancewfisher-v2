# Unity Slice Handoff: AI Strategic Layer -- Sub-Slice 1

- Date: 2026-04-18
- Session: claude-ai-strategic-layer-2026-04-18
- Branch: codex/unity-fortification-siege
- Contract Revision: 10

## Goal

Port the AI strategic brain's territory expansion, scout/harass dispatch, world-pressure posture conditioning, and reinforcement routing from `src/game/core/ai.js` into a proper DOTS ISystem. Replace the MonoBehaviour-based debug-surface tick approach that was incompatible with ECS batch-mode smoke testing.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs`
  `AIStrategicPosture` enum (Expand/Consolidate/Defend) and `AIStrategyComponent` IComponentData. Carries per-faction timers, accumulators, posture state, issued-order counts, and target CP IDs.

- `unity/Assets/_Bloodlines/Code/AI/EnemyAIStrategySystem.cs`
  `EnemyAIStrategySystem` (partial struct ISystem, SimulationSystemGroup, UpdateAfter WorldPressureEscalationSystem). Implements all four strategy passes per tick: `UpdatePosture` (world pressure level → posture), `RunTerritoryExpansion` (nearest unowned CP, issues move orders to combat units), `RunScoutHarass` (lowest-loyalty hostile CP, dispatches LightCavalry/MeleeRecon), `RunReinforcement` (idle combat units to lowest-loyalty owned CP < 60 loyalty). Interval floors set at 0.001f to support 0f smoke-test intervals.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIStrategySmokeValidation.cs`
  4-phase smoke validator:
  - Phase 1: AIStrategyComponent defaults (Posture=Expand, intervals correct)
  - Phase 2: Territory expansion fires on first tick (interval=0f), picks player CP, issues 3 move orders
  - Phase 3: WorldPressure Level=3 pre-seeded → posture transitions to Defend on first tick
  - Phase 4: LightCavalry scout dispatched toward lowest-loyalty hostile CP

- `scripts/Invoke-BloodlinesUnityAIStrategySmokeValidation.ps1`
  Batch runner; artifact at `artifacts/unity-ai-strategy-smoke.log`.

### Modified Files

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AIStrategy.cs`
  Slimmed to public debug API only (`TryDebugGetAIStrategy`). All tick logic moved to `EnemyAIStrategySystem`. Removed `TickAIStrategyFactions` and all helper methods that duplicated the ISystem.

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  Removed `TickAIStrategyFactions(entityManager, Time.deltaTime)` call from `Update()`. Strategy is now ticked by `EnemyAIStrategySystem` each frame.

- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
  Seeds `AIStrategyComponent` on non-player Kingdom faction entities alongside `AIEconomyControllerComponent`:
  - ExpansionIntervalSeconds = 8f
  - ScoutHarassIntervalSeconds = 12f
  - WorldPressureResponseIntervalSeconds = 15f
  - ReinforcementIntervalSeconds = 10f
  - CurrentPosture = Expand

- `unity/Assembly-CSharp.csproj` -- registered `AIStrategyComponent.cs`, `EnemyAIStrategySystem.cs`
- `unity/Assembly-CSharp-Editor.csproj` -- registered `BloodlinesAIStrategySmokeValidation.cs`
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- revision 9 → 10, lane claimed

## Key Design Decision

The initial approach put strategy tick logic in the MonoBehaviour debug surface partial class. That approach was fundamentally incompatible with ECS batch-mode smoke testing because `world.Update()` in a bare `new World()` does not invoke MonoBehaviour methods. The fix was to create `EnemyAIStrategySystem` as a proper `ISystem`, which can be added to the SimulationSystemGroup in smoke tests via `sg.AddSystemToUpdateList(world.GetOrCreateSystem<EnemyAIStrategySystem>())`.

The interval floor was set to 0.001f (not 0.1f as originally written) so that smoke tests can set interval=0f and have the system fire on the first tick at batch-mode dt=0.016f.

## Verification Results

```
BLOODLINES_AI_STRATEGY_SMOKE PASS
Phase 1 PASS: AIStrategyComponent defaults; Posture=Expand ExpansionInterval=8.
Phase 2 PASS: TerritoryCommandsIssued=3 ExpansionTarget=cp_player Posture=Expand.
Phase 3 PASS: Posture=Defend WorldPressureLevelCached=3.
Phase 4 PASS: scout dispatched; ScoutHarassOrdersIssued=1 HarassTarget=cp_player.
```

All 8 validation gates green:
1. dotnet build Assembly-CSharp.csproj: 0 errors
2. dotnet build Assembly-CSharp-Editor.csproj: 0 errors
3. Bootstrap runtime smoke: PASS
4. Combat smoke: exit 0
5. Scene shells (Bootstrap + Gameplay): both green
6. data-validation.mjs: exit 0
7. runtime-bridge.mjs: exit 0
8. Contract staleness check: revision=10, PASS

## Current Readiness

- EnemyAIStrategySystem running in game loop (seeded from SkirmishBootstrapSystem)
- All 4 strategy passes implemented and smoke-tested
- Debug surface read API (`TryDebugGetAIStrategy`) available for HUD integration

## Browser Reference

- `ai.js` `pickTerritoryTarget` (~747): territory expansion scoring (nearest, unowned, low-loyalty bias)
- `ai.js` `pickScoutHarassTarget` (~412): scout harass target selection (lowest-loyalty hostile CP at medium range)
- `ai.js` `getWorldPressureRaidTarget` (~817): world pressure posture conditioning

## Next Action

Sub-slice 2: supply chain / convoy management (`ai.js` ~1100 tickSupplyChain, tickConvoyDispatching). Alternatively, claim Tier 2 batch systems (marriage, lesser house defection, minor house levies, renown scoring) or victory conditions system.
