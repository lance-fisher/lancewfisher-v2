# Unity Slice Handoff: faith-combat-doctrine / Faith Doctrine Combat Wiring

**Date:** 2026-04-24
**Lane:** faith-combat-doctrine
**Branch:** claude/unity-faith-combat-doctrine
**Status:** Complete

## Goal

Wire the faith doctrine territorial effects (stabilizationMultiplier and captureMultiplier from faiths.json) into ControlPointCaptureSystem so committed covenants gain meaningful territorial bonuses. Commander aura effects (auraAttackMultiplier, auraRadiusBonus, auraSightBonus) were already wired via CommanderAuraCanon in a prior slice; this slice closes the remaining gap.

## Browser Reference

`simulation.js` ~170: `DOCTRINE_DEFAULTS` (auraAttackMultiplier, auraRadiusBonus, auraSightBonus, stabilizationMultiplier, captureMultiplier, populationGrowthMultiplier)
`simulation.js` ~581: `getFaithDoctrineEffects(state, factionId)` -- reads `faith.prototypeEffects[doctrinePath]`
`data/faiths.json`: canonical prototypeEffects per covenant per doctrinePath (4 covenants × 2 paths)

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` -- faith must meaningfully affect gameplay, not just be flavor. Doctrine paths create sideways differentiation: light = territorial/stabilizing, dark = aggressive/capturing.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Faith/FaithDoctrineEffectsComponent.cs` -- `FaithDoctrineEffectsComponent` (IComponentData per faction: AuraAttackMultiplier, AuraRadiusBonus, AuraSightBonus, StabilizationMultiplier, CaptureMultiplier, PopulationGrowthMultiplier); `Defaults()` static factory returns identity values
- `unity/Assets/_Bloodlines/Code/Faith/FaithDoctrineCanon.cs` -- `FaithDoctrineCanon.Resolve(CovenantId, DoctrinePath)` static lookup matching faiths.json prototypeEffects for all 4 covenants × 2 paths; returns Defaults() for unbound factions
- `unity/Assets/_Bloodlines/Code/Faith/FaithDoctrineSyncSystem.cs` -- `[UpdateBefore(ControlPointCaptureSystem)]`; reads FaithStateComponent per faction; writes/updates FaithDoctrineEffectsComponent; lazy-creates via em.AddComponentData on first update
- `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs` -- narrow edits: passive stabilization, active stabilization now multiplied by `ResolveFaithDoctrineStabilizationMultiplier(entityManager, factionId)`; capture rate progress now multiplied by `ResolveFaithDoctrineCaptureMultiplier(entityManager, factionId)`; two new static helpers added following existing ResolveStabilizationMultiplier pattern
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.FaithDoctrine.cs` -- TryDebugGetFaithDoctrineEffects (factionId → all 6 effect fields)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithDoctrineCombatWiringSmokeValidation.cs` -- 3-phase smoke: (1) committed faction gets non-default effects, unbound faction gets identity; (2) light doctrine has higher stabilizationMultiplier than dark across all 4 covenants; (3) dark doctrine has higher captureMultiplier than light across all 4 covenants
- `scripts/Invoke-BloodlinesUnityFaithDoctrineCombatWiringSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 5 new .cs files
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not wire `populationGrowthMultiplier` (no population system exists yet to wire into)
- Did not modify CommanderAuraCanon or CommanderAuraSystem (aura effects already wired)
- Did not implement per-unit faith-doctrine morale bonuses (no morale system exists yet)
- Did not add per-covenant special unit types or abilities (separate lane concern)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Faith doctrine combat wiring smoke -- PASS (3 phases green)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=129)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to next unclaimed Claude Code lane: skirmish game mode manager or dynasty prestige (renown decay + prestige drift distinct from the already-implemented renown accumulation system).
