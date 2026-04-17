# Merge Plan 2026-04-17: Claude Economy + AI Branch and Codex Projectile Branch

## State at time of this document

Master (`origin/master`) is at the post-Session 119 merge (`a8dd553`):
combat foundation + full economy through cap-pressure + gather + trickle.

Active concurrent branches on origin:

- `codex/unity-combat-foundation` — already merged into master via `a8dd553`.
- `codex/unity-projectile-combat` — projectile combat work. A local auto-commit
  (`10a7895` on the Claude economy branch) already contains both
  Codex's projectile code AND Claude's AI construction work because the
  continuation-platform auto-continue loop committed combined working-tree
  state. That commit is NOT on master yet; it's on
  `claude/unity-enemy-ai-economic-base`.
- `claude/unity-enemy-ai-economic-base` — Session 120 through Session 124
  Claude work plus the mixed-in Codex projectile files from `10a7895`.
  This branch is the most forward branch at the moment.

## Sessions on the Claude branch (all pushed)

```
c11b3c4  Session 124: enemy AI militia posture
ed01487  Session 123: loyalty + population density HUD readout
3300b8e  Session 122: stability-surplus loyalty restoration
0b7b350  Session 121 addendum: AI construction smoke proof + handoff
10a7895  Session 121: projectile system + enemy AI combat integration
          (combined Claude AI construction + Codex projectile - auto-commit)
2189337  Session 120: enemy AI economic base
```

## Sessions on the Codex branch (reported complete)

- Projectile combat: `ProjectileComponent`, `ProjectileFactoryComponent`,
  `ProjectileMovementSystem`, `ProjectileImpactSystem`, plus a second phase
  in `BloodlinesCombatSmokeValidation` that proves bowman-vs-villager.
- Codex's self-reported HEAD on the branch is `8710141` (Document Unity
  combat foundation slice) but actual projectile work lives in later
  commits Codex made.

## Merge order

Because the combined auto-commit `10a7895` already sits on the Claude
branch, the Claude branch effectively contains BOTH lanes. The merge
path is:

1. Fetch origin and inspect `codex/unity-projectile-combat` vs
   `claude/unity-enemy-ai-economic-base` to confirm any divergence.
2. If `codex/unity-projectile-combat` head has work not yet reflected on
   `claude/unity-enemy-ai-economic-base` (likely: Codex may have pushed
   tightening / smoke phase 2 work post-10a7895), merge or cherry-pick
   those commits onto a throwaway integration branch first.
3. Merge `claude/unity-enemy-ai-economic-base` into `master` with
   `--no-ff`.
4. Resolve any conflicts in shared wiring files. Expected collision
   points at time of writing:
   - `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
   - `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs`
   - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
   - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
   - `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs`
   - `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`,
     `continuity/PROJECT_STATE.json`
5. Run all governed wrappers on master:
   - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
   - `node tests/data-validation.mjs`
   - `node tests/runtime-bridge.mjs`
6. Rename session numbers in `CURRENT_PROJECT_STATE.md` if Codex's
   branch used overlapping Session N numbering. Claude used
   Sessions 120-124; Codex should pick Session 125+ for their next
   slice.

## Expected final master state after merge

Bootstrap runtime smoke success line should carry:

- `productionProgressAdvancementVerified=True`
- `constructionProgressAdvancementVerified=True`
- `constructedProductionQueued=True`
- `gatherDepositObserved=True`
- `trickleGainObserved=True`
- `starvationObserved=True`
- `loyaltyDeclineObserved=True`
- `capPressureObserved=True`
- `stabilitySurplusObserved=True`
- `aiActivityObserved=True`
- `aiConstructionObserved=True`
- `aiMilitaryOrdersIssued >= 1`

Combat smoke success line should carry:

- Phase 1 melee: `Combat smoke validation passed: dead='enemy', ...`
- Phase 2 ranged (projectile): projectile seen in flight, target died

## Prompt for Codex's next slice

See `docs/unity/CODEX_NEXT_PROMPT_ATTACK_MOVE.md` in the repo.
