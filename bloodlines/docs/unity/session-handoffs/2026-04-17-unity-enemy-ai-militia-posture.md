# 2026-04-17 Unity Enemy AI Militia Posturing

## Goal

Give the enemy AI a military intent. Before this slice, AI-produced militia
stood idle at their barracks while the AI economy kept running. Enemy
militia now issue move orders toward the nearest hostile control point on
their own interval, making the enemy visible on the map as a dynamic
aggressor rather than a static production target.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/AI/AIEconomyControllerComponent.cs`:
  - `MilitaryPostureAccumulator` / `MilitaryPostureIntervalSeconds`
    (default 4s)
  - `MilitaryPostureMinimumMilitiaCount` (default 2) and
    `MilitaryPostureApproachRadius` (default 2.5f)
  - `MilitaryPostureOrdersIssued` telemetry counter
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  `SpawnFactionEntity` seeds the four new military-posture fields on AI
  factions.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs`:
  - `TickAIFactions` now advances a fourth accumulator and fires a
    military posture pass on its own interval.
  - `AIRunMilitaryPosture` returns early when the AI faction has fewer
    than `MilitaryPostureMinimumMilitiaCount` live militia. Otherwise it
    finds a hostile control-point anchor via
    `TryFindHostileControlPointAnchor` (prefers owned-by-other-faction
    control points over neutral ones) and issues `MoveCommandComponent`
    orders toward the anchor for every live militia whose current
    destination is not already near the anchor. Idempotent across ticks
    so militia settle at the anchor instead of thrashing.
  - `TryDebugGetAIMilitaryOrdersIssued(factionId, out ordersIssued)` debug
    API for governed observation.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - Final success diagnostics line now carries `aiMilitaryOrdersIssued`
    so the governed smoke records the AI's posture intent alongside
    existing AI economy and construction proofs.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  via the wrapper lock. Success line ends with `aiMilitaryOrdersIssued=2`
  alongside preserved `aiConstructionObserved=True`,
  `stabilitySurplusObserved=True`, and all prior proofs. Enemy issued move
  orders to both of its starting militia units, targeting the nearest
  non-enemy control-point anchor.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The enemy AI is now a live opposed player that:

- gathers gold to the nearest resource node
- trains villagers up to a worker target
- trains militia up to a military target
- builds dwellings, farms, and wells up to target counts
- issues move orders for its militia toward the nearest hostile control
  point, making the enemy visible on the map as a dynamic aggressor
  rather than a static production target

The simulation is now a living opposed match rather than a sandbox.
Combat resolution when militia meet remains Codex's lane.

## Next Action

1. Enemy AI combat bark: when enemy militia reach a hostile control
   point, initiate capture by holding position within capture radius.
2. AI wood / stone / iron worker diversification: second-tier workers
   pulled off gold once gold stockpile is surplus.
3. Draft the next Codex prompt (explicit attack-move / attack orders)
   so combat + AI posture meet cleanly.

Concurrent work: Codex on `codex/unity-projectile-combat` (their
projectile slice integrated into master via an auto-commit; see
commit `10a7895`).
