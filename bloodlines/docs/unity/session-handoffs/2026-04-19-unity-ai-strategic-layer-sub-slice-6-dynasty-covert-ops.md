# Unity AI Strategic Layer Sub-Slice 6: Dynasty Covert Ops Dispatch

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-dynasty-covert-ops`
**Lane:** ai-strategic-layer
**Contract Revision:** 18

---

## Goal

Port the dynasty-aware covert/diplomatic operations dispatch block from `src/game/core/ai.js`
`updateEnemyAi` (~lines 2419-2678) to Unity DOTS/ECS. This includes all nine timer-driven
operations: assassination, missionary, holy war, divine right, captive recovery, marriage
proposal, marriage inbox accept, non-aggression pact proposal, and lesser-house promotion.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs`
  Nine countdown timers (default values from ai.js: assassination 80s, missionary 70s,
  holy war 95s, divine right 140s, captive recovery 60s, marriage proposal 90s, marriage
  inbox 30s, pact proposal 120s, lesser-house promotion 60s). Thirty-one context flags
  covering every gate condition in the ai.js dispatch block. Dispatch result fields
  `LastFiredOp` (CovertOpKind enum) and `LastFiredOpTime`. CovertOpKind 10-value enum:
  None, Assassination, Missionary, HolyWar, DivineRight, CaptiveRescue, CaptiveRansom,
  MarriageProposal, MarriageInboxAccept, PactProposal, LesserHousePromotion.

- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateAfter(typeof(AISiegeOrchestrationSystem))]`.
  Three phases per entity per frame:
  1. `TickTimers`: decrements all nine timers by `SystemAPI.Time.DeltaTime`.
  2. `ApplyPressureCaps`: mirrors ai.js pressure-cap chain. DarkExtremesSourceFocused clamps
     assassination to 6s; PlayerWorldPressureLevel 2/1 clamps to 12s/22s; convergence caps
     via `math.min`; HolyWarSourceFocused/PlayerDivineRightActive clamp missionary to 8s/6s;
     CaptivesSourceFocused clamps captive recovery to 6s.
  3. `TryFireOps`: iterates operations in exact ai.js priority order, fires the first whose
     timer has expired and whose gate conditions are met, writes LastFiredOp and
     LastFiredOpTime, and returns. Captive recovery branch: HighPriorityCaptive or
     EnemyIsHostileToPlayer yields CaptiveRescue; otherwise CaptiveRansom.
     Pact proposal branch: shouldPropose = EnemyUnderSuccessionPressure or
     EnemyArmyCount <= 3 or PlayerGovernanceNearVictory.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAICovertOpsSmokeValidation.cs`
  Five-phase smoke validator. Each phase creates an isolated World, seeds
  MatchProgressionComponent, player faction, and an AI faction with AICovertOpsComponent
  configured to fire exactly one operation. All firing timers set to -1f (already expired)
  so they fire with deltaTime=0 in test worlds. Non-firing timers set to 999f.
  - Phase 1: DarkExtremesSourceFocused + LiveIntelOnPlayer -> Assassination
  - Phase 2: HolyWarSourceFocused + canPressureFaith (EnemyHasFaith, EnemyFaithDiffersFromPlayer,
    EnemyFaithIntensity=20) -> Missionary
  - Phase 3: TensionSignalCount=2, EnemyHasFaith, PlayerHasFaith, not harmonious,
    EnemyFaithIntensity=25 -> HolyWar
  - Phase 4: PactTermsAvailable, EnemyUnderSuccessionPressure -> PactProposal
  - Phase 5: HasCaptiveTarget, HighPriorityCaptive -> CaptiveRescue
  Artifact: `../artifacts/unity-ai-covert-ops-smoke.log`
  Marker: `BLOODLINES_AI_COVERT_OPS_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAICovertOpsSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAICovertOpsSmokeValidation.RunBatchAICovertOpsSmokeValidation`.

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision=18 current PASS

Covert ops smoke: all 5 phases PASS
- Phase 1 PASS: LastFiredOp=Assassination (dark extremes -> assassination)
- Phase 2 PASS: LastFiredOp=Missionary (holy war source -> missionary)
- Phase 3 PASS: LastFiredOp=HolyWar (tension signals -> holy war)
- Phase 4 PASS: LastFiredOp=PactProposal (succession crisis -> pact proposal)
- Phase 5 PASS: LastFiredOp=CaptiveRescue (high priority captive -> rescue-first)

---

## Key Design Notes

**Timer-fire pattern in test worlds.** Unity ECS test worlds run `world.Update()` with
DeltaTime=0. Timers seeded at `0.001f` never expire because `0.001f - 0f = 0.001f > 0`.
The fix is seeding firing timers at `-1f` so they are already expired at first update.
This is the canonical pattern for all covert-ops and similar timer-driven smoke tests.

**Pressure caps.** All caps use `math.min(timer, cap)` which only tightens timers that
have not yet expired. A timer already at -1f is unaffected by caps.

**Operation execution deferred.** The system writes `LastFiredOp` and resets the fired
timer to the canonical default (the same default the browser uses). Actual operation
execution (startAssassinationOperation, etc.) is reserved for a later integration pass.
This slice ports the scheduling and dispatch logic only, matching the browser spec.

**Missing fields discovered during build.** `EnemyIsHostileToPlayer` was required by
`AICovertOpsSystem.cs` for the captive rescue-first gate but was absent from the initial
`AICovertOpsComponent.cs`. Added before first successful build.

---

## Current Readiness

Branch `claude/unity-ai-dynasty-covert-ops` is ready to merge. All gates green, contract
at revision 18, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-dynasty-covert-ops` into master.
2. Claim sub-slice 7 (build timer chain) on new branch `claude/unity-ai-build-timer-chain`.
   Browser reference: `src/game/core/ai.js` building construction/upgrade timer block
   ~lines 1060-1100.
3. Confirm Codex `fortification-siege-imminent-engagement` status before claiming any
   Codex-owned paths.
